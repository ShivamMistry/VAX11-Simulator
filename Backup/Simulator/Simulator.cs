using System;

using VAX11Internals;
using VAX11Settings;
using System.Threading;

namespace VAX11Simulator
{
	/// <summary>
	/// The Simulator.
	/// </summary>
	public class Simulator
	{
		#region Members

		private Registers r;
		private Memory Mem;
		private VAX11Simulator.Console _console;
		private SimEvent[] HardwareInterruptsChecker = SimEvent.GenerateVAX11EventsVector();
		private Thread OutputInterruptThread;
		public bool OutputThreadOn;

		/// <summary>
		/// First dynamic allocated block address
		/// </summary>
		private int _iFirstAllocatedCell = 0;

		#endregion

		#region Properties

		/// <summary>
		/// Access the simulator's memory
		/// </summary>
		public Memory memory
		{
			get { return Mem; }
		}

		/// <summary>
		/// Access to the simulator's registers
		/// </summary>
		public Registers R
		{
			get { return r; }
		}

		/// <summary>
		/// Access to the simulator's console
		/// </summary>
		public VAX11Simulator.Console Console
		{
			get { return _console; }
		}

		/// <summary>
		/// First dynamic allocated block address
		/// </summary>
		public int FirstAllocatedAddress
		{
			get { return _iFirstAllocatedCell; }
			set { _iFirstAllocatedCell = value; }
		}

		#endregion

		#region Constructor

		/// <summary>
		/// Consturctor
		/// </summary>
		/// <param name="cbUserCode">User Program to load</param>
		/// <param name="con">Console for the simulator</param>
		public Simulator(ProgramBlock cbUserCode, VAX11Simulator.Console con)
		{
			// Allocate registers and set default values
			r = new Registers();
			r[14] = Settings.Simulator.iSP;
			r[15] = cbUserCode.EntryPoint;
			r[17] = r[14] + 0x100; // SCBB


			// Allocate memory and load the user's program to there
			Mem = new Memory(0, int.MaxValue);
			Mem.WriteProgram(cbUserCode);

			// Save console reference
			_console = con;

			// Initalize interrupt vectors
			foreach (SimEvent e in HardwareInterruptsChecker)
				Mem.Write(e.SCBB_OFFSET + r[17].ReadLong(), new CodeBlock(0L, 4), false); 

			OutputThreadOn = false;
		}

		~Simulator()
		{
			if (OutputThreadOn)
			{
				OutputInterruptThread.Abort();
			}
			OutputThreadOn = false;
		}

		#endregion

		#region Enums and Consts

		/// <summary>
		/// Address of the software interrupt with IPL = 1
		/// </summary>
		public const int SOFTWARE_INTERRUPTS_BASE_ADDRESS = 0x84;

		/// <summary>
		/// Times in clock cycles for the diffrent addressing modes
		/// </summary>
		enum AddressingModeTimes
		{
			Register = 2,
			Register_Deferred = 4,
			Autoincrement__Immediate = 5,
			Autodecrement = 5,
			Autoincrement_Deferred__Absolute = 7,
			Displacement__Relative = 4,
			Displacement_Deferred__Relative_Deferred = 6,
			Index = 4,
			Literal = 1,
		}

		/// <summary>
		/// For use of the string opcodes to calculate times for system time
		/// </summary>
		public const int ACCESS_MEMORY_FACTOR_TIME = 10;

		#endregion

		#region Methods

		#region Interrupts

		/// <summary>
		/// Get events and translate it to VAX-11 events
		/// </summary>
		/// <param name="e">The Event</param>
		/// <param name="iValue">Addition Information</param>
		public void SendEvent(SimulatorEvents e, int iValue)
		{
			// Handle all posibble events
			if (e == SimulatorEvents.CLOCK_INTERRUPT)
			{
				lock(this)
				{// if (!HardwareInterruptsChecker[(int)SimulatorEvents.CLOCK_INTERRUPT].EventOccured)
				{
					if (HardwareInterruptsChecker[(int)SimulatorEvents.CLOCK_INTERRUPT].IPL < r.PSL.IPL) return;
					// ICCS = 24, ICR = 26
					if ((r[24] & (uint)1) != 0)					// Run
					{
						if ((uint)r[26] + (uint)Settings.Simulator.ClockResolution * 1000 > (uint)r[26]) 
							r[26] += Settings.Simulator.ClockResolution * 1000;
						else r[26] = 0;
					}
					if ((r[24] & (uint)0x80) != 0) r[24] = (uint)r[24] | (uint)0x80000000;	// Err bit
					else r[24] = (uint)r[24] | (uint)0x80;						// Int ICR

					if ((int)r[26] == 0)
					{
						HardwareInterruptsChecker[(int)SimulatorEvents.CLOCK_INTERRUPT].EventOccured = true;
						r[26] = r[25].ReadLong();
					}
				}
				}
			}
			else if (e == SimulatorEvents.INPUT_INTERRUPT)
			{
				lock(this)
				{
					r[33] = iValue;			// RXDB
					r[32].SetBits(7,7,1);	// RXCS

					// Strange, but if we write "(int)(SimulatorEvents.INPUT_INTERRUPT)"
					// directly in the array [], it won't work
					int iInputInterruptIndex = (int)(SimulatorEvents.INPUT_INTERRUPT);

					if (HardwareInterruptsChecker[iInputInterruptIndex].IPL < r.PSL.IPL) return;
					if (r[32].GetBits(6, 6) == 1)
					{
						HardwareInterruptsChecker[iInputInterruptIndex].EventOccured = true;
					}
				}
			}
			else if (e == SimulatorEvents.OUTPUT_INTERRUPT)
			{
				lock(this)
				{
					int iOutputInterruptIndex = (int)(SimulatorEvents.OUTPUT_INTERRUPT);
					// Strange, but if we write "(int)(SimulatorEvents.OUTPUT_INTERRUPT)"
					// directly in the array [], it won't work
					if (HardwareInterruptsChecker[iOutputInterruptIndex].IPL < r.PSL.IPL) return;
					if (r[34].GetBits(6, 6) == 1)//if the output interrupts are activate
					{
						r[34].SetBits(7,7,0);
						//HardwareInterruptsChecker[iOutputInterruptIndex].EventOccured = true;
						if (OutputThreadOn)
							OutputInterruptThread.Abort();
						OutputInterruptThread = new Thread(new ThreadStart(OutputInterruptHandler));
						OutputThreadOn = true;
						OutputInterruptThread.Priority = ThreadPriority.AboveNormal;
						OutputInterruptThread.Name = "Output Interrupt Handler";
						OutputInterruptThread.Start();
						Console.AllThreads.Add(OutputInterruptThread);
					}
					else
					{
						if (OutputThreadOn)
							OutputInterruptThread.Abort();
					}

				}
			}
			else if (e == SimulatorEvents.POWER_DOWN_INTERRUPT)
			{
				//console has been close...
				if (OutputThreadOn)
					OutputInterruptThread.Abort();
			}
			else if(e == SimulatorEvents.ARITHMETIC)
			{
				int iInterruptIndex = (int)SimulatorEvents.ARITHMETIC;
				if (HardwareInterruptsChecker[iInterruptIndex].IPL < r.PSL.IPL) return;
				push(iValue, 4); // push the extra info that describe the arithmetic exception
				HardwareInterruptsChecker[iInterruptIndex].EventOccured = true;
			}
			else if (e == SimulatorEvents.RESERVED_OPERAND)
			{
				int iInterruptIndex = (int)SimulatorEvents.RESERVED_OPERAND;
				HardwareInterruptsChecker[iInterruptIndex].EventOccured = true;
			}
			else
				throw new PanicException();

			if (HardwareInterruptsChecker[(int)e].Type == SimulatorEventsTypes.FAULT)
			{
				switch(e)
				{
					case SimulatorEvents.RESERVED_OPERAND:
						throw new RuntimeError(SimulatorMessage.RESERVED_OPERAND, r[15].ReadLong());
					default:
						throw new RuntimeError(SimulatorMessage.GENERAL_FAULT_EXCEPTION, r[15].ReadLong());
				}
			}

			else if (HardwareInterruptsChecker[(int)e].Type == SimulatorEventsTypes.TRAP)
			{
				int iAddress = HardwareInterruptsChecker[(int)SimulatorEvents.ARITHMETIC].SCBB_OFFSET + r[17];
				if (((int)Mem.Read(iAddress, 4, false)) == 0) // then we need to halt, no one catch the trap
				{
					switch(e)
					{
						case SimulatorEvents.ARITHMETIC:
						{
							switch(iValue)
							{
								case (int)ARITHMETIC_TRAPS.INTEGER_DIVIDE_BY_ZERO:
									throw new RuntimeError(SimulatorMessage.DIVIDE_BY_ZERO, r[15].ReadLong());
								case (int)ARITHMETIC_TRAPS.SUBSCRIPT_RANGE:
									throw new RuntimeError(SimulatorMessage.SUBSCRIPT_RANGE, r[15].ReadLong());
								default:
									throw new RuntimeError(SimulatorMessage.ARITHMETIC_EXCEPTION, r[15].ReadLong());
							};
						}
						default:
							throw new RuntimeError(SimulatorMessage.TRAP_NOT_HANDLED, r[15].ReadLong());

					}
				}
			}
		}


		/// <summary>
		/// The function handle the received interrupts - it changes the machine registers
		/// and status and perform the interrupts
		/// </summary>
		/// <returns>true if we handled interrupt</returns>
		bool HandleInterrupts()
		{

			bool bInterruptExists = false;
			int iAddress = 0; // dummy, stupid c# doesn't understand it doesn't need a initial value
			byte iIPL = 0; // dummy, stupid c# doesn't understand it doesn't need a initial value

			// Check if there if active hardware interrupt
			int iMaxInterruptIPL = 0, iMaxIndex = -1;
			for (int iCounter = 0; iCounter < HardwareInterruptsChecker.Length; ++iCounter)
			{
				if (HardwareInterruptsChecker[iCounter].EventOccured && 
					HardwareInterruptsChecker[iCounter].IPL > iMaxInterruptIPL)
				{
					iMaxInterruptIPL = HardwareInterruptsChecker[iCounter].IPL;
					iMaxIndex = iCounter;
					break;
				}
			}

			// There is interrupt to handle
			if (iMaxIndex != -1 && iMaxInterruptIPL > r[18]) // r[18] is IPL
			{
				HardwareInterruptsChecker[iMaxIndex].EventOccured = false;
				iAddress = HardwareInterruptsChecker[iMaxIndex].SCBB_OFFSET + r[17];
				iIPL = (byte)HardwareInterruptsChecker[iMaxIndex].IPL;
				bInterruptExists = true;

			}


			// Software interrupt
			if (!bInterruptExists && r[21].ReadLong() != 0) // SISR
			{
				int iMSB = Convert.ToInt32(Math.Log(r[21].ReadLong(), 2) + 1);
				if (iMSB > r[18]) // r[18] is IPL
				{
					r[21].SetBits(iMSB-1,iMSB-1, 0);
				
					iAddress = SOFTWARE_INTERRUPTS_BASE_ADDRESS + r[17] + (iMSB - 1) * 4;
					iIPL = (byte)iMSB;
					bInterruptExists = true;
				}
			}


			// If interrupt occured and also there is an interrupt handler
			if (bInterruptExists)
			{
				LaunchInterrupt(iAddress, iIPL);
				return true;
			}

			return false;
		}


		/// <summary>
		/// the actually call of the interrupt. called by HandleInterrupt and MTPR
		/// </summary>
		/// <param name="iAddress">Address to jump to</param>
		/// <param name="iNewIPL">New IPL for the system</param>
		internal void LaunchInterrupt(int iAddress, byte iNewIPL)
		{
			if (((int)Mem.Read(iAddress, 4, false)) != 0)
			{
				push(r.PSL.ReadLong(), 4);
				push(r[15], 4);
				r.PSL.SetFlags(0, 0, 0, 0);
				r.PSL.IPL = iNewIPL;
				r[18] = (int)r.PSL.IPL;
				r[15] = (int)Mem.Read(iAddress, 4, true);
			}
		}


		private void OutputInterruptHandler()
		{
			int iOutputInterruptIndex = (int)(SimulatorEvents.OUTPUT_INTERRUPT);
			while(OutputThreadOn)
			{
				Thread.Sleep(100);
				if (r[34].GetBits(6,7) == 3)
				{
					r[34].SetBits(7,7,0);
					try
					{
						_console.Invoke (_console.m_Output, new object[] { (CodeBlock) r[35].ReadByte() });
					}
					catch (System.InvalidOperationException)
					{
						OutputInterruptThread.Abort();
					}
					HardwareInterruptsChecker[iOutputInterruptIndex].EventOccured = true;
				}
			}
		}

		#endregion

		#region Main Methods

		/// <summary>
		/// Reads and performs the next command
		/// </summary>
		public void PerformNextCommand()
		{
			// Read Opcode, Analyze it
			byte NextOpcode = Mem[r[15]++];
			OpcodeEntry theOpcode = new OpcodeEntry(NextOpcode, r[15] - 1);

			// Add the time of the command to the system time
			r.SystemTime += theOpcode.Cycles;

			// Read all the operands
			Operand[] ops = new Operand[theOpcode.NumberOfOperands];
			for (int iCounter = 0; iCounter < theOpcode.NumberOfOperands; ++iCounter)
			{
				int iOperandSize = theOpcode.OpType[iCounter*2+1] - '0';
				ops[iCounter] = FetchOperand(iOperandSize, theOpcode.OpType[iCounter*2]);
			}

			// Perform the command
			theOpcode.ImplementationFunction(this, NextOpcode, ops);

			// Support Interrupts
			while (HandleInterrupts() == true);
		}

        
		/// <summary>
		/// Reads operand from the memory, returns its value and its effective address.
		/// Assuming: The operand is calculate as final number. 
		/// Example: lets say iSize is 1 and the EA contains the number 0xFF, Op will be -1
		/// </summary>
		/// <param name="iSize">the size of the requested operand</param>
		/// <returns>Operand's information</returns>
		private Operand FetchOperand(int iSize, char cType)
		{
			// Branch displacement addressing mode
			if (cType == 'b') 
			{
				// Increase System Time
				r.SystemTime += (int)AddressingModeTimes.Autoincrement__Immediate;

				int iValue = (int)(new CodeBlock(Mem.Read(r[15], iSize, true), iSize));
				if (((iValue >> (8 * (iSize) - 1)) & 1) != 0 )  
				{
					if (iSize == 1) iValue = (int)((sbyte)(iValue));
					else if (iSize == 2) iValue = (int)((short)(iValue));
					else throw new PanicException(); // 'b' mode supports only 1 or 2 bytes of location
				}
				
				r[15] += iSize;
				return new Operand(iValue + r[15], Mem.Read(iValue + r[15], iSize, false), false, true);
			}

			// Privileged Register
			else if (cType == 'p')
			{
				// Increase System Time
				r.SystemTime += (int)AddressingModeTimes.Register;

				byte iSpecialRegNumber = Mem[r[15]++];

				// Assuming we will use this addressing mode only when iSize = 4
				return new Operand(iSpecialRegNumber, r[iSpecialRegNumber], true, true);
			}

			// Dummy value. The value selected in a way that the finally catch won't change any exception that might be thrown
			Operand res = new Operand(-1, 0, false, true);

			try
			{
				byte FirstByte = Mem[r[15]++];
				bool bShowMemoryAccess = cType == 'w' ? false : true; // if we write an operand, hide the fact the we also read it

				// Addressing Mode 1 (Register)
				if (FirstByte >= 0x50 && FirstByte < 0x60)
				{
					// Increase System Time
					r.SystemTime += (int)AddressingModeTimes.Register;

					// Calculate and return result
					if (iSize == 4) res = new Operand(FirstByte - 0x50, r[FirstByte - 0x50], true, true);
					else if (iSize == 2) res = new Operand(FirstByte - 0x50, r[FirstByte - 0x50].ReadWord(), true, true);
					else if (iSize == 1) res = new Operand(FirstByte - 0x50, r[FirstByte - 0x50].ReadByte(), true, true);
					else if (iSize == 8) res = new Operand(FirstByte - 0x50, (long)r[FirstByte - 0x50] + r[FirstByte - 0x50 + 1] * 0x100000000L, true, true);
				}

				// Addressing Mode 2 (Register Deferred), Addressing Mode 3 (Autoincrement),
				// Addressing Mode 4 (Autodecrement), Addressing Mode 10 (Immediate)
				else if (FirstByte >= 0x60 && FirstByte < 0x90)
				{
					int iRegister = FirstByte & 0xF;

					// Addressing Mode 4 (Autodecrement)
					if (FirstByte >= 0x70 && FirstByte < 0x80)
					{
						r.SystemTime += (int)AddressingModeTimes.Autodecrement;
						r[iRegister] -= iSize;
					}

					// Addressing Mode 2 (Register Deferred)
					res = new Operand(r[iRegister], (long)Mem.Read(r[iRegister], iSize, bShowMemoryAccess), false, true);

					if (FirstByte >= 0x60 && FirstByte < 0x70) r.SystemTime += (int)AddressingModeTimes.Register_Deferred;
					

					// Addressing Mode 3 (Autoincrement), Addressing Mode 10 (Immediate)
					if (FirstByte >= 0x80 && FirstByte < 0x90)
					{
						r.SystemTime += (int)AddressingModeTimes.Autoincrement__Immediate;
						r[iRegister] += iSize;
					}
				}


				// Addressing Mode 5 (Autoincrement Deferred)
				// Addressing Mode 11 (Absolute)
				else if (FirstByte >= 0x90 && FirstByte < 0xA0)
				{
					r.SystemTime += (int)AddressingModeTimes.Autoincrement_Deferred__Absolute;
					int iReg = FirstByte - 0x90;
					int EA = Mem.Read(r[iReg], 4, true);
					r[iReg] += 4;
					res = new Operand(EA, (long)(Mem.Read(EA, iSize, bShowMemoryAccess)), false, true);
				}


				// Addressing Mode 6 (Displacement)
				// Addressing Mode 12 (Relative)
				else if ( (FirstByte >= 0xA0 && FirstByte < 0xB0) || (FirstByte >= 0xC0 && FirstByte < 0xD0)
					|| (FirstByte >= 0xE0 && FirstByte < 0xF0) )
				{
					r.SystemTime += (int)AddressingModeTimes.Displacement__Relative;
					int iOffsetSize = FirstByte / 0x10 - 10 > 0 ? FirstByte / 0x10 - 10 : 1;
					int iOffset = (int)Mem.Read(r[15], iOffsetSize, true);
					r[15] += iOffsetSize;
					int EA = ((int)r[FirstByte & 0xF]) + iOffset;
					res = new Operand(EA, (long)Mem.Read(EA, iSize, bShowMemoryAccess), false, true);
				}

				// Addressing Mode 7 (Displacement Deferred)
				// Addressing mode 13 (Relative Deferred)
				else if ( (FirstByte >= 0xB0 && FirstByte < 0xC0) || (FirstByte >= 0xD0 && FirstByte < 0xE0)
					|| (FirstByte >= 0xF0 && FirstByte <= 0xFF) )
				{
					r.SystemTime += (int)AddressingModeTimes.Displacement_Deferred__Relative_Deferred;
					int iOffsetSize = FirstByte / 0x10 - 11 > 0 ? FirstByte / 0x10 - 11 : 1;
					int iOffset = (int)Mem.Read(r[15], iOffsetSize, true);
					r[15] += iOffsetSize;
					int EA = Mem.Read(((int)r[FirstByte & 0xF]) + iOffset, 4, true);
					res = new Operand(EA, (long)Mem.Read(EA, iSize, bShowMemoryAccess), false, true);
				}


				// Addressing Mode 9 (Literal)
				else if (FirstByte < 0x40) 
				{
					r.SystemTime += (int)AddressingModeTimes.Literal;
					res = new Operand(0, FirstByte, false, false);
				}

				// Addressing Mode 8 (Index)
				else if (FirstByte >= 0x40 && FirstByte < 0x50)
				{
					int iStartingPC = r[15] - 1;
					if (Mem[r[15]] == 0x40) throw new RuntimeError(SimulatorMessage.ILLEGAL_ADDRESSING_MODE, iStartingPC);
					int iIndexReg = FirstByte & 0x0F;
					if (iIndexReg == 0xF) throw new RuntimeError(SimulatorMessage.PC_CANT_BE_USED_AS_INDEX_REG, iStartingPC);
					Operand PrimaryOp = FetchOperand(iSize, cType);
					if (PrimaryOp.IsRegister || PrimaryOp.HasEffectiveAddress != true)
						throw new RuntimeError(SimulatorMessage.ILLEGAL_ADDRESSING_MODE, iStartingPC);
					res = new Operand(PrimaryOp.EffectiveAddress + r[iIndexReg] * iSize, Mem.Read(PrimaryOp.EffectiveAddress + r[iIndexReg] * iSize, iSize, bShowMemoryAccess), false, true);
				}

				else throw new PanicException();

				return res;
			}
			finally
			{
				if ((cType == 'a') && (res.IsRegister || res.HasEffectiveAddress != true))
						throw new RuntimeError(SimulatorMessage.ILLEGAL_ADDRESSING_MODE, r[15]);
			}
		}

		/// <summary>
		/// Gets destination operand, and writes value on its effective address.
		/// </summary>
		/// <param name="dst">Destination operand</param>
		/// <param name="iSize">Operand Size</param>
		/// <param name="lValue">Value to write</param>
		internal void WriteOnDestination(Operand dst, int iSize, long lValue)
		{
			// Is the operand has legal effective address?
			if (dst.HasEffectiveAddress == false) 
				throw new RuntimeError(SimulatorMessage.ILLEGAL_ADDRESSING_MODE, r[15]);

			// Mask for the value
			long mask = 0;
			for (int iCounter = 0; iCounter < iSize; ++iCounter) mask = (mask * 0x100) + 0xFF;

			// Write in register
			if (dst.IsRegister)
			{
				if (iSize == 1) r[dst.EffectiveAddress].WriteByte((byte)(lValue & mask));
				if (iSize == 2) r[dst.EffectiveAddress].WriteWord((int)(lValue & mask));
				if (iSize == 4) r[dst.EffectiveAddress].WriteLong((int)(lValue & mask));
				if (iSize == 8)
				{
					if (dst.EffectiveAddress % 2 != 0) 
						throw new RuntimeError(SimulatorMessage.QUAD_CAN_BE_USED_ONLY_ON_EVEN_REGISTERS, r[15]);
					CodeBlock temp = new CodeBlock(lValue, 8);
					r[dst.EffectiveAddress].WriteLong((int)temp.SubBlock(0, 4));
					r[dst.EffectiveAddress+1].WriteLong((int)temp.SubBlock(4, 4));
				}
			}
			// Write in Memory
			else 
				Mem.Write(dst.EffectiveAddress, new CodeBlock(lValue & mask, iSize));
		}

		#endregion

		#region Stack

		/// <summary>
		/// Implementation of VAX-11 push. The implementation is in this class
		/// as we use it in many commands, and we want it to be effective and atomic
		/// </summary>
		/// <param name="data">The data to push</param>
		/// <param name="iSize">Number of bytes to push. Must be 1, 2, 4 or 8</param>
		internal void push(long data, int iSize)
		{
			if (iSize != 1 && iSize != 2 && iSize != 4 && iSize != 8) throw new PanicException(); 
			r[14] -= iSize;

			if (iSize == 1) data = data & 0xFF;
			else if (iSize == 2) data = data & 0xFFFF;
			
			Mem.Write(r[14], new CodeBlock(data, iSize));

			
		}

		/// <summary>
		///  Implementation of VAX-11 pop.
		/// </summary>
		/// <param name="iSize">number of bytes to pop</param>
		/// <returns>The value readed from the stack</returns>
		internal int pop(int iSize)
		{
			if (iSize != 1 && iSize != 2 && iSize != 4) throw new PanicException(); 
			int iRetValue = Mem.Read(r[14], iSize, true);
			r[14] += iSize;

			return iRetValue;
		}

		#endregion

		#endregion
	}
}
