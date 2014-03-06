using System;
using PCSpeakerLib;

using VAX11Internals;

namespace VAX11Simulator
{
	/// <summary>
	/// This class implements all of the VAX11 Opcodes.
	/// </summary>
	public class VAX11Opcodes
	{
		public delegate void OpcodeImplementation(Simulator TheSimulator, int Opcode, Operand[] ops);

		#region Special Functions

		/// <summary>
		/// Default function for all unsupported functions
		/// </summary>
		/// <param name="TheSimulator">The simulator to work on</param>
		/// <param name="Opcode">Command opcode</param>
		/// <param name="ops">operands</param>
		public static void UnsupportedOpcode(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			throw new RuntimeError(SimulatorMessage.UNSUPPORTED_OPCODE, theSimulator.R.PC);
		}

		/// <summary>
		/// Function for all Macro Opcodes
		/// </summary>
		public static void MacroOpcode(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Do nothing, as we never reach this point
			throw new PanicException();
		}

		#endregion


		#region A Opcodes

		/// <summary>
		/// Implementation of acbb, acbw, acbl opcodes
		/// </summary>
		public static void acb(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[5] - '0';

			// Opcode Format: opcode limit.rx, add.rx, index.mx, displ.bw

			// index <-- index + add;
			long newIndex = ops[2].Op + ops[1].Op;
			theSimulator.WriteOnDestination(ops[2], iOperandSize, newIndex);
			
			// if {{add GEQ 0} AND {index LEQ limit}} OR
			//	{{add LSS 0} AND {index GEQ limit}} then
			if ((Operand.IsNeg(ops[1].Op, iOperandSize) == 0 && 
				Operand.Compare(ops[2], iOperandSize, ops[0].Op, iOperandSize) == -1)
				||
				(Operand.IsNeg(ops[1].Op, iOperandSize) == 1 && 
				Operand.Compare(ops[2], iOperandSize, ops[0].Op, iOperandSize) == 1))
			{
				theSimulator.R[15] = ops[3].EffectiveAddress;

				//	PC <-- PC + SEXT (displ);
			}

			theSimulator.R.PSL.SetFlags(-1, 
				Operand.SumHasOverflow(ops[2].Op, ops[1].Op, iOperandSize), Operand.IsZero(ops[2].Op + ops[1].Op, iOperandSize), Operand.IsNeg(ops[2].Op + ops[1].Op, iOperandSize));
		}

		/// <summary>
		/// Implementation of adawi opcode
		/// </summary>
		public static void adawi(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Opcode format: opcode add.rw, sum.mw
			// Operation: tmp <- add; sum <- sum + tmp;

			if (!ops[1].HasEffectiveAddress)
			{
				theSimulator.SendEvent(SimulatorEvents.RESERVED_OPERAND, -1);
				return;
			}

			// Bit 0 of the address must be zero
			if (ops[1].EffectiveAddress % 2 == 1 && !ops[1].IsRegister) theSimulator.SendEvent(SimulatorEvents.RESERVED_OPERAND, -1);

			add(theSimulator, 0xA0, ops);

		}

		/// <summary>
		/// Implementation of addb2, addw2, addl3, addb3, addw3, addl3 opcodes
		/// </summary>
		public static void add(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iOperandSize;

			if (Opcode == 0xC0 || Opcode == 0xC1) iOperandSize = 4; // addl2, addl3
			else if (Opcode == 0xA0 || Opcode == 0xA1) iOperandSize = 2; // addw2, addw3
			else if (Opcode == 0x80 || Opcode == 0x81) iOperandSize = 1; // addb2, addb3
			else throw new PanicException();

			if (Opcode == 0xC0 || Opcode == 0xA0 || Opcode == 0x80)
				theSimulator.WriteOnDestination(ops[1], iOperandSize, ops[0].Op + ops[1].Op);
			else theSimulator.WriteOnDestination(ops[2], iOperandSize, ops[0].Op + ops[1].Op);

			// N and Z are set as always.  V is set if SIGNED overflow occurs, and
			// C is set if UNSIGNED integer overflow occurs.  C is always set to 0
			// for real data types.

			theSimulator.R.PSL.SetFlags(Operand.HasCarry(ops[0].Op + ops[1].Op, iOperandSize), 
				Operand.SumHasOverflow(ops[0].Op, ops[1].Op, iOperandSize), Operand.IsZero(ops[0].Op + ops[1].Op, iOperandSize), Operand.IsNeg(ops[0].Op + ops[1].Op, iOperandSize));
		}

		/// <summary>
		/// Implementation of adwc opcode
		/// </summary>
		public static void adwc(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iOldC = theSimulator.R.PSL.C, iOperandSize = 4;
			theSimulator.WriteOnDestination(ops[1], iOperandSize, ops[0].Op + ops[1].Op + iOldC);

			// N and Z are set as always.  V is set if SIGNED overflow occurs, and
			// C is set if UNSIGNED integer overflow occurs. 
			sbyte iV = Operand.SumHasOverflow(ops[0].Op, ops[1].Op, iOperandSize);
			if (Operand.SumHasOverflow(ops[0].Op + ops[1].Op, iOldC, iOperandSize) == 1) iV = 1;
			theSimulator.R.PSL.SetFlags(Operand.HasCarry(ops[0].Op + ops[1].Op + iOldC, iOperandSize), iV, 
				Operand.IsZero(ops[0].Op + ops[1].Op + iOldC, iOperandSize), Operand.IsNeg(ops[0].Op + ops[1].Op + iOldC, iOperandSize));
		}

		/// <summary>
		/// Implementation of aoblss, aobleq opcodes
		/// </summary>
		public static void aob(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Opcode Format: opcode limit.rl, index.ml, displ.bb

			// index <-- index + 1;
			theSimulator.WriteOnDestination(ops[1], 4, ops[1].Op + 1);

			sbyte i_V = (0x8 * (int)Math.Pow(0x10, 4 * 2 - 1)) - 1 == ops[1].Op ? (sbyte)1 : (sbyte)0;
			theSimulator.R.PSL.SetFlags(-1, i_V, Operand.IsZero(ops[1].Op + 1, 4), Operand.IsNeg(ops[1].Op + 1, 4));

			// aoblss (if index LSS limit)
			if (Opcode == 0xF2 && (int)ops[1].Op + 1 < (int)ops[0].Op) theSimulator.R[15] = ops[2].EffectiveAddress;
				// aobleq (if index LEQ limit)
			else if (Opcode == 0xF3 && (int)ops[1].Op + 1 <= (int)ops[0].Op) theSimulator.R[15] = ops[2].EffectiveAddress;

		}

		/// <summary>
		/// Implementation of ashl, ashq opcodes
		/// </summary>
		public static void ash(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[3] - '0';
			long result;
			sbyte iV = 0;

			// Opcode Format: opcode cnt.rb, src.rx, dst.wx

			// 2. If cnt GEQ 32 (ASHL) or cnt GEQ 64 (ASHQ); the destination operand is replaced by 0
			if ((sbyte)ops[0].Op >= 8 * iOperandSize) result = 0;
				// 3. If cnt LEQ -32 (ASHL) or cnt LEQ -63 (ASHQ); all the bits of the destination operand
				// are copies of the sign bit of the source operand.
			else if ((Opcode == 0x78 && (sbyte)ops[0].Op <= -32) || (Opcode == 0x79 && (sbyte)ops[0].Op <= -63))
			{
				result = 0;
				if (Operand.IsNeg(ops[1].Op, iOperandSize) == 1)
					for (int iCounter = 0; iCounter < iOperandSize; ++iCounter) result = (result * 0x100) + 0xFF;
			}
			else if ((sbyte)ops[0].Op == 0) result = ops[1].Op;
			else if ((sbyte)ops[0].Op > 0)
			{
				result = ops[1].Op << ((sbyte)ops[0].Op);
				if ((Operand.IsNeg(result, iOperandSize) == 1 && Operand.IsNeg(ops[1].Op, iOperandSize) == 0)
					|| (Operand.IsNeg(result, iOperandSize) == 0 && Operand.IsNeg(ops[1].Op, iOperandSize) == 1))
					iV = 1;
			}
			else
			{
				if (Opcode == 0x78) result = (int)ops[1].Op >> Math.Abs(((sbyte)ops[0].Op));
				else result = ops[1].Op >> Math.Abs(((sbyte)ops[0].Op));
			}

			theSimulator.WriteOnDestination(ops[2], iOperandSize, result);
			theSimulator.R.PSL.SetFlags(0, iV, 
				Operand.IsZero(result, iOperandSize), Operand.IsNeg(result, iOperandSize));
			
		}

		#endregion

		#region B Opcodes

		/// <summary>
		/// Implementation of bneq, bnequ, beql, beqlu, bgtr, bgeq, bleq, blss
		/// bgtru, blequ, bvc, bvs, bgequ, bcc, blssu, bcs opcodes
		/// </summary>
		public static void b(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			theSimulator.R.PSL.SetFlags(-1, -1, -1, -1);
			
			bool bJumpAccepted = false;

			// bneq, bnequ
			if (Opcode == 0x12 && theSimulator.R.PSL.Z == 0) bJumpAccepted = true;
				// beql, beqlu
			else if (Opcode == 0x13 && theSimulator.R.PSL.Z == 1) bJumpAccepted = true;
				// bgtr
			else if (Opcode == 0x14 && (theSimulator.R.PSL.Z | theSimulator.R.PSL.N) == 0) bJumpAccepted = true;
				// bleq
			else if (Opcode == 0x15 && (theSimulator.R.PSL.Z | theSimulator.R.PSL.N) == 1) bJumpAccepted = true;
				// bgeq
			else if (Opcode == 0x18 && theSimulator.R.PSL.N == 0) bJumpAccepted = true;
				// blss
			else if (Opcode == 0x19 && theSimulator.R.PSL.N == 1) bJumpAccepted = true;
				// bgtru
			else if (Opcode == 0x1A && (theSimulator.R.PSL.Z | theSimulator.R.PSL.C) == 0) bJumpAccepted = true;
				// blequ
			else if (Opcode == 0x1B && (theSimulator.R.PSL.Z | theSimulator.R.PSL.C) == 1) bJumpAccepted = true;
				// bvc
			else if (Opcode == 0x1C && theSimulator.R.PSL.V == 0) bJumpAccepted = true;
				// bvs
			else if (Opcode == 0x1D && theSimulator.R.PSL.V == 1) bJumpAccepted = true;			
				// bgequ, bcc
			else if (Opcode == 0x1E && theSimulator.R.PSL.C == 0) bJumpAccepted = true;	
				// blssu, bcs
			else if (Opcode == 0x1F && theSimulator.R.PSL.C == 1) bJumpAccepted = true;	

			if (bJumpAccepted) theSimulator.R.PC = ops[0].EffectiveAddress;
		}


		/// <summary>
		/// Implementation of bbc, bbs opcodes
		/// Note: all the BB opcodes share code, so bug on one should be fix on several places
		/// </summary>
		public static void bb(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Opcode Format: opcode pos.rl, base.ab, displ.bb

			//teststate = if {BBS} then 1 else 0;
			int teststate = Opcode == 0xE0 ? 1 : 0;

			uint pos = (uint)ops[0].Op;

			// 1.	A reserved operand fault occurs if pos GTRU 31 and the bit is contained in a register.
			if (pos > 31 && ops[1].IsRegister)
			{
				// 2.	On a reserved operand fault, the condition codes are unpredictable.
				Random r = new Random();
				theSimulator.R.PSL.SetFlags((sbyte)r.Next(0x2), (sbyte)r.Next(0x2), (sbyte)r.Next(0x2), (sbyte)r.Next(0x2));

				theSimulator.SendEvent(SimulatorEvents.RESERVED_OPERAND, -1);
				return;
			}

			int theBit;
			//if FIELD (pos, 1, base) EQL teststate then
			if (ops[1].IsRegister)
			{
				theBit = theSimulator.R[ops[1].EffectiveAddress].GetBits((int)pos, (int)pos);
			}
			else
			{
				int theAddress = ops[1].EffectiveAddress + (int)pos / 32;
				theBit = Registers.Register.GetBits((int)(pos % 32), (int)(pos % 32), theSimulator.memory[theAddress]);
			}

			//		PC <- PC + SEXT (displ);
			if (theBit == teststate) theSimulator.R[15] = ops[2].EffectiveAddress;
		
		}

		
		/// <summary>
		/// Implementation of bbcs, bbcc, bbsc, bbss, bbssi, bbcci opcodes
		/// Note: all the BB opcodes share code, so bug on one should be fix on several places
		/// </summary>
		public static void bb2(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Opcode Format: opcode pos.rl, base.ab, displ.bb

			
			int teststate = (Opcode == 0xE4 || Opcode == 0xE2 || Opcode == 0xE6) ? 1 : 0;
			int newbitvalue = (Opcode == 0xE3 || Opcode == 0xE2 || Opcode == 0xE6) ? 1 : 0;

			uint pos = (uint)ops[0].Op;

			// 1.	A reserved operand fault occurs if pos GTRU 31 and the bit is contained in a register.
			if (pos > 31 && ops[1].IsRegister)
			{
				// 2.	On a reserved operand fault, the condition codes are unpredictable.
				Random r = new Random();
				theSimulator.R.PSL.SetFlags((sbyte)r.Next(0x2), (sbyte)r.Next(0x2), (sbyte)r.Next(0x2), (sbyte)r.Next(0x2));

				theSimulator.SendEvent(SimulatorEvents.RESERVED_OPERAND, -1);
				return;
			}

			int theBit;
			//if FIELD (pos, 1, base) EQL teststate then
			if (ops[1].IsRegister)
			{
				theBit = theSimulator.R[ops[1].EffectiveAddress].GetBits((int)pos, (int)pos);
				theSimulator.R[ops[1].EffectiveAddress].SetBits((int)pos, (int)pos, newbitvalue); // set bit
			}
			else
			{
				int theAddress = ops[1].EffectiveAddress + (int)pos / 32;
				theBit = Registers.Register.GetBits((int)(pos % 32), (int)(pos % 32), theSimulator.memory[theAddress]);
				Registers.Register tmp = (Registers.Register)((int)theSimulator.memory.Read(theAddress, 4));
				tmp.SetBits((int)(pos % 32), (int)(pos % 32), newbitvalue);
			}

			//		PC <- PC + SEXT (displ);
			if (theBit == teststate) theSimulator.R[15] = ops[2].EffectiveAddress;
		
		}


		/// <summary>
		/// Implementation of bicb2, bicw2, bicl2 opcodes
		/// </summary>
		public static void bic2(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			long dst = ops[1].Op & (~ops[0].Op);
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.WriteOnDestination(ops[1], iOperandSize, dst);
			theSimulator.R.PSL.SetFlags(-1, 0, Operand.IsZero(dst, iOperandSize), 
				Operand.IsNeg(dst, iOperandSize));
		}

		/// <summary>
		/// Implementation of bicb3, bicw3, bicl3 opcodes
		/// </summary>
		public static void bic3(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			long dst = ops[1].Op & (~ops[0].Op);
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.WriteOnDestination(ops[2], iOperandSize, dst);
			theSimulator.R.PSL.SetFlags(-1, 0, Operand.IsZero(dst, iOperandSize), 
				Operand.IsNeg(dst, iOperandSize));
		}

		/// <summary>
		/// Implementation of bicpsw opcode
		/// </summary>
		public static void bicpsw(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			if ((ops[0].Op & 0xFFFF0000) != 0)
			{
				theSimulator.SendEvent(SimulatorEvents.RESERVED_OPERAND, -1);
				return;
			}
			int dst = theSimulator.R.PSL.PSW & (int)(~ops[0].Op);
			theSimulator.R.PSL.PSW = dst;

			// No need to set flags
			theSimulator.R.PSL.SetFlags(-1, -1, -1, -1);
		}

		/// <summary>
		/// Implementation of bisb2, bisw2, bisl2 opcodes
		/// </summary>
		public static void bis2(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			long dst = ops[1].Op | ops[0].Op;
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.WriteOnDestination(ops[1], iOperandSize, dst);
			theSimulator.R.PSL.SetFlags(-1, 0, Operand.IsZero(dst, iOperandSize), 
				Operand.IsNeg(dst, iOperandSize));
		}

		/// <summary>
		/// Implementation of bisb3, bisw3, bisl3 opcodes
		/// </summary>
		public static void bis3(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			long dst = ops[1].Op | ops[0].Op;
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.WriteOnDestination(ops[2], iOperandSize, dst);
			theSimulator.R.PSL.SetFlags(-1, 0, Operand.IsZero(dst, iOperandSize), 
				Operand.IsNeg(dst, iOperandSize));
		}

		/// <summary>
		/// Implementation of bispsw opcode
		/// </summary>
		public static void bispsw(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			if ((ops[0].Op & (~Operand.Mask(2))) != 0)
			{
				theSimulator.SendEvent(SimulatorEvents.RESERVED_OPERAND, -1);
				return;
			}
			int dst = theSimulator.R.PSL.PSW | (int)ops[0].Op;
			theSimulator.R.PSL.PSW = dst;

			// No need to set flags
			theSimulator.R.PSL.SetFlags(-1, -1, -1, -1);
		}


		/// <summary>
		/// Implementation of bitb, bitw, bitl opcodes
		/// </summary>
		public static void bit(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			long res = ops[0].Op & ops[1].Op;
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.R.PSL.SetFlags(-1, 0, Operand.IsZero(res, iOperandSize), 
				Operand.IsNeg(res, iOperandSize));
		}

		
		/// <summary>
		/// Implementation of blbc, blbs opcodes
		/// </summary>
		public static void blb(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iBit = 1 - (Opcode - 0xE8);		// :)
			if ((ops[0].Op & 1) == iBit) theSimulator.R[15] = ops[1].EffectiveAddress;
		}
		
		/// <summary>
		/// Implementation of bpt opcode
		/// </summary>
		public static void bpt(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			theSimulator.R.PSL.TP = 0;
			theSimulator.R.PSL.SetFlags(0, 0, 0, 0);
			// TODO: Raise break-point exception
		}

		/// <summary>
		/// Implementation of brb, brw, jmp opcodes
		/// </summary>
		public static void br(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			theSimulator.R[15] = ops[0].EffectiveAddress;
			theSimulator.R.PSL.SetFlags(-1, -1, -1, -1);
		}

		#endregion

		#region C Opcodes

		/// <summary>
		/// Implementation of calls opcode
		/// </summary>
		public static void callg(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			theSimulator.R.PSL.SetFlags(0, 0, 0, 0);

			int SPA = theSimulator.R[14] & 0x03;
			for (int iCounter = 0; iCounter < SPA; ++iCounter) theSimulator.push(0, 1);
			int retAddress = theSimulator.R.PC;
			theSimulator.R.PC = ops[1].EffectiveAddress;
			int RegMask = (int)theSimulator.memory.Read(theSimulator.R[15], 2);
			for (int iCounter = 11; iCounter >= 0; --iCounter)
				if ( ((RegMask >> (iCounter)) & 1) == 1 ) 
					theSimulator.push(theSimulator.R[iCounter], 4);
			theSimulator.push(retAddress, 4);
			theSimulator.push(theSimulator.R[13], 4);
			theSimulator.push(theSimulator.R[12], 4);
			int Others = (SPA << 30) + (0 << 29) + ((RegMask & 0xFFF) << 0x10) + (theSimulator.R.PSL & 0xFFFF);
			theSimulator.push(Others, 4);
			theSimulator.push(0, 4);
			theSimulator.R[13] = theSimulator.R[14].ReadLong();
			theSimulator.R[12] = (int)ops[0].EffectiveAddress; // global area address
			theSimulator.R.PC += 2;

			calls_callg_common(theSimulator, ops);
		}

		/// <summary>
		/// Implementation of calls opcode
		/// </summary>
		public static void calls(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			theSimulator.R.PSL.SetFlags(0, 0, 0, 0);

			int N = (int)ops[0].Op;
			theSimulator.push(N, 4);
			int temp = theSimulator.R[14];
			int SPA = theSimulator.R[14] & 0x03;
			for (int iCounter = 0; iCounter < SPA; ++iCounter) theSimulator.push(0, 1);
			int retAddress = theSimulator.R.PC;
			theSimulator.R.PC = ops[1].EffectiveAddress;
			int RegMask = (int)theSimulator.memory.Read(theSimulator.R[15], 2);
			for (int iCounter = 11; iCounter >= 0; --iCounter)
				if ( ((RegMask >> (iCounter)) & 1) == 1 ) 
					theSimulator.push(theSimulator.R[iCounter], 4);
			theSimulator.push(retAddress, 4);
			theSimulator.push(theSimulator.R[13], 4);
			theSimulator.push(theSimulator.R[12], 4);
			int Others = (SPA << 30) + (1 << 29) + ((RegMask & 0xFFF) << 0x10) + (theSimulator.R.PSL & 0xFFFF);
			theSimulator.push(Others, 4);
			theSimulator.push(0, 4);
			theSimulator.R[13] = theSimulator.R[14].ReadLong();
			theSimulator.R[12] = temp;
			theSimulator.R.PC += 2;

			calls_callg_common(theSimulator, ops);
		}



		/// <summary>
		/// Help function that contains the common part of calls and callg.
		/// </summary>
		private static void calls_callg_common(Simulator theSimulator, Operand[] ops)
		{

			// If it is known function
			if (VAX11Internals.KnownFunctions.GetFunctionName(ops[1].EffectiveAddress) != "")
			{
				string sFunctionName = VAX11Internals.KnownFunctions.GetFunctionName(ops[1].EffectiveAddress);

				switch (sFunctionName)
				{
					case "EXIT":
						KnownFunctions.exit(theSimulator);
						break;
					case "GETCHAR":
						KnownFunctions.getchar(theSimulator);
						break;
					case "GETS":
						KnownFunctions.gets(theSimulator);
						break;
					case "SPRINTF":
						KnownFunctions.sprintf(theSimulator);
						break;
					case "PRINTF":
						KnownFunctions.printf(theSimulator, true);
						break;
					case "PUTCHAR":
						KnownFunctions.putchar(theSimulator);
						break;
					case "PUTS":
						KnownFunctions.puts(theSimulator);
						break;
					case "SCANF":
						KnownFunctions.scanf(theSimulator);
						break;

					case "MALLOC":
						KnownFunctions.malloc(theSimulator);
						break;
					case "FREE":
						KnownFunctions.free(theSimulator);
						break;

					case "CLEARDEVICE":
						KnownFunctions.cleardevice(theSimulator);
						break;
					case "LINE":
						KnownFunctions.line(theSimulator);
						break;
					case "PUTPIXEL":
						KnownFunctions.putpixel(theSimulator);
						break;
					case "CIRCLE":
						KnownFunctions.circle(theSimulator);
						break;
					case "RECTANGLE":
						KnownFunctions.rectangle(theSimulator);
						break;
					case "SETCOLOR":
						KnownFunctions.setcolor(theSimulator);
						break;
					case "GETMAXX":
						KnownFunctions.getmaxx(theSimulator);
						break;
					case "GETMAXY":
						KnownFunctions.getmaxx(theSimulator);
						break;
					case "OUTTEXTXY":
						KnownFunctions.outtextxy(theSimulator);
						break;
					case "SETFONT":
						KnownFunctions.setfont(theSimulator);
						break;
					case "INITGRAPH":
						KnownFunctions.initgraph(theSimulator);
						break;
					case "CLOSEGRAPH":
						KnownFunctions.closegraph(theSimulator);
						break;
					default:
						throw new PanicException();
				};

				theSimulator.R.SystemTime += VAX11Internals.KnownFunctions.GetCycles(sFunctionName);

				// Return from known function
				ret(theSimulator, 0x04, ops);
			}
		}



		/// <summary>
		/// Implementation of caseb, casew, casel opcodes
		/// </summary>
		public static void case2(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Format: opcode selector.rx, base.rx, limit.rx,   displ[0].bw,...,displ[limit].bw
			
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';

			// tmp <-- selector - base;
			uint tmp = (uint)(ops[0].Op - ops[1].Op);
			uint limit = (uint)ops[2].Op;

			// PC <-- PC + if tmp LEQU limit then
			if (tmp <= limit)
			{
				theSimulator.R[15] += (short)theSimulator.memory.Read((int)(theSimulator.R[15].ReadLong() + 2 * tmp), 2);
				//		SEXT (displ [tmp])
			}
			else
			{
				//		else {2 + 2* ZEST (limit)};
				theSimulator.R.PC += 2 * limit + 2;
			}

			// N <-- temp LSS limit;
			// Z <-- temp EQL limit;
			// V <-- 0;
			// C <-- temp LSSU limit;
			theSimulator.R.PSL.SetFlags(tmp < limit ? (sbyte)1 : (sbyte)0, 0, Operand.IsZero((int)tmp - (int)limit, iOperandSize), 
				Operand.IsZero((int)tmp - (int)limit, iOperandSize));

		}

		/// <summary>
		/// Implementation of cmpb, cmpw, cmpl opcodes
		/// </summary>
		public static void cmp(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iSize;
			if (Opcode == 0xD1) iSize = 4;
			else if (Opcode == 0xB1) iSize = 2;
			else iSize = 1;

			sbyte c = ((uint)ops[0].Op) < ((uint)ops[1].Op) ? (sbyte)1 : (sbyte)0;
			sbyte z = ops[0].Op == ops[1].Op ? (sbyte)1 : (sbyte)0;
			sbyte n = (Operand.Compare(ops[0].Op, iSize, ops[1].Op, iSize) == -1) ? (sbyte)1 : (sbyte)0;
			theSimulator.R.PSL.SetFlags(c, 0, z, n);
		}

		/// <summary>
		/// Implementation of cmpc3 opcode
		/// </summary>
		public static void cmpc3(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			ushort len = (ushort)ops[0].Op;
			int addr1 = (int)ops[1].EffectiveAddress;
			int addr2 = (int)ops[2].EffectiveAddress;

			int iCounter;

			for (iCounter = 0; theSimulator.memory[addr1+iCounter] == theSimulator.memory[addr2+iCounter] && 
				iCounter < len; ++iCounter);

			if (len == iCounter) theSimulator.R.PSL.SetFlags(0, 0, 1, 0);
			else
			{
				byte byte1 = theSimulator.memory[addr1+iCounter];
				byte byte2 = theSimulator.memory[addr2+iCounter];
				theSimulator.R.PSL.SetFlags(byte1 < byte2 ? (sbyte)1 : (sbyte)0, 0, 0, 
					((sbyte)byte1 < (sbyte)byte2) ? (sbyte)1 : (sbyte)0);
			}

			theSimulator.R[0] = len - iCounter;
			theSimulator.R[1] = addr1 + iCounter;
			theSimulator.R[2] = len - iCounter;
			theSimulator.R[3] = addr2 + iCounter;

			theSimulator.R.SystemTime += len / Simulator.ACCESS_MEMORY_FACTOR_TIME;

		}

		
		/// <summary>
		/// Implementation of cmpc5 opcode
		/// </summary>
		public static void cmpc5(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			ushort len1 = (ushort)ops[0].Op;
			int addr1 = (int)ops[1].EffectiveAddress;
			byte fill = (byte)ops[2].Op;
			ushort len2 = (ushort)ops[3].Op;
			int addr2 = (int)ops[4].EffectiveAddress;

			int iCounter;

			for (iCounter = 0; theSimulator.memory[addr1+iCounter] == theSimulator.memory[addr2+iCounter] && 
				iCounter < Math.Min(len1, len2); ++iCounter);

			if (len1 < len2 && iCounter == len1)
				for (iCounter = len1; fill == theSimulator.memory[addr2+iCounter] && iCounter < len2; ++iCounter);
			else if (len1 > len2 && iCounter == len2)
				for (iCounter = len2; fill == theSimulator.memory[addr2+iCounter] && iCounter < len1; ++iCounter);			

			if (Math.Max(len1, len2) == iCounter) theSimulator.R.PSL.SetFlags(0, 0, 1, 0);
			else
			{
				byte byte1 = theSimulator.memory[addr1+iCounter];
				byte byte2 = theSimulator.memory[addr2+iCounter];
				theSimulator.R.PSL.SetFlags(byte1 < byte2 ? (sbyte)1 : (sbyte)0, 0, 0, 
					((sbyte)byte1 < (sbyte)byte2) ? (sbyte)1 : (sbyte)0);
			}

			theSimulator.R[0] = Math.Max(len1 - iCounter, 0);
			theSimulator.R[1] = Math.Min(addr1 + iCounter, addr1 + len1);
			theSimulator.R[2] = Math.Max(len2 - iCounter, 0);
			theSimulator.R[3] = Math.Min(addr2 + iCounter, addr2 + len2);

			theSimulator.R.SystemTime += iCounter / Simulator.ACCESS_MEMORY_FACTOR_TIME;

		}

		/// <summary>
		/// Implementation of clrb, clrw, clrl, clrq opcodes
		/// </summary>
		public static void clr(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.WriteOnDestination(ops[0], iOperandSize, 0);
			theSimulator.R.PSL.SetFlags(-1, 0, 1, 0);
		}

		/// <summary>
		/// Implementation of CVTBL, CVTBW, CVTLB, CVTLW, CVTWB, CVTWL opcodes
		/// </summary>
		public static void cvt(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Format: opcode src.rx, dst.wy

			int iSrcSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			int iDstSize = new OpcodeEntry(Opcode, -1).OpType[3] - '0';
			
			// Copy and extend sign bit
			int iSignBitMask = (Registers.Register.GetBits(iSrcSize * 8 - 1, iSrcSize * 8 - 1, (int)ops[0].Op) == 1) ? -1 : 0;

			Registers.Register iReg = (Registers.Register)((int)ops[0].Op);
			if (iSrcSize < iDstSize) iReg.SetBits(iSrcSize * 8, iDstSize * 8 - 1, iSignBitMask);
			int iRes = iReg.ReadLong();
			
			theSimulator.WriteOnDestination(ops[1], iDstSize, iRes);


			//	N <-- dst LSS 0;
			//	Z <-- dst EQL 0;
			//	V <-- {src cannot be represented in dst};
			//	C <-- 0;
			theSimulator.R.PSL.SetFlags(0, Operand.NumberCanBePresentedNbytesBlock(ops[0].Op, iDstSize), Operand.IsZero(ops[0].Op, iDstSize),
				Operand.IsNeg(ops[0].Op, iDstSize));

		}


		#endregion

		#region D Opcodes

		/// <summary>
		/// Implementation of decb, decw, decl opcodes
		/// </summary>
		public static void dec(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.WriteOnDestination(ops[0], iOperandSize, ops[0].Op - 1);

			// N and Z are set as always.  V is set if 80..0 is decremented to
			// 7F..F.  C is set if 0 is decremented to FF..F.

			sbyte i_V = 0x8 * (long)Math.Pow(0x10, iOperandSize * 2 - 1) == ops[0].Op ? (sbyte)1 : (sbyte)0;
			sbyte i_C = ops[0].Op == 0 ? (sbyte)1 : (sbyte)0;

			theSimulator.R.PSL.SetFlags(i_C, i_V , Operand.IsZero(ops[0].Op - 1, iOperandSize), 
				Operand.IsNeg(ops[0].Op - 1, iOperandSize));
		}

		/// <summary>
		/// Implementation of divb2, divw2, divl3, divb3, divw3, divl3 opcodes
		/// </summary>
		public static void div(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Format:
			//			opcode divr.rx, quo.mx				2 operand
			//			opcode divr.rx, divd.rx, quo.wx 	3 operand

			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';

			//		1. Available for byte, word, long integers and any real type.
			//        For integers, result is based on SIGNED operands.

			if (ops[0].Op == 0 || ( Operand.Compare(ops[0].Op, iOperandSize, (0x8 * (int)Math.Pow(0x10, iOperandSize * 2 - 1)), iOperandSize) == 0
				&& ops[1].Op == -1))
			{
				// In the integer divisor operand is 0, then in 2 operand integer format, 
				// the quotient operand is not affected; in 3 operand format the quotient 
				// operand is replaced by the dividend operand.

				if (Opcode == 0xC7 || Opcode == 0xA7 || Opcode == 0x87)
					theSimulator.WriteOnDestination(ops[2], iOperandSize, ops[1].Op);
				theSimulator.R.PSL.V = 1;
				if (ops[0].Op == 0) theSimulator.SendEvent(SimulatorEvents.ARITHMETIC, (int)ARITHMETIC_TRAPS.INTEGER_DIVIDE_BY_ZERO);
				return;
			}


			//      3. N, Z, are set as always.  V is set if overflow occurs, which can
			//         happen as follows:
			//         a. Division by zero is attempted.  (This also causes an exception)
			//         b. 80..0 is divided by -1.
			//         c. A real divisor has a negative exponent (i.e. represents a fraction),
			//            and the result exponent is too large
			//         C is always set to 0, since the operation always assumes signed numbers

			// N <-- quo LSS 0; Z <-- quo EQL 0; V <-- {overflow} OR {divr EQL 0}; C <-- 0;

			long num0 = Operand.ConvertToLong(ops[0].Op, iOperandSize);
			long num1 = Operand.ConvertToLong(ops[1].Op, iOperandSize);


			theSimulator.R.PSL.SetFlags(0, 0, Operand.IsZero(num1 / num0, iOperandSize), Operand.IsNeg(num1 / num0, iOperandSize));


			//		2. For integer division, the result is truncated toward zero - e.g.
			//
			//         7 / 2          => 3
			//         7 / -2         => -3
			//         -7 / 2         => -3
			//         -7 / -2        => 3


			if (Opcode == 0xC6 || Opcode == 0xA6 || Opcode == 0x86)
				theSimulator.WriteOnDestination(ops[1], iOperandSize, num1 / num0);
			else theSimulator.WriteOnDestination(ops[2], iOperandSize, num1 / num0);

		}

		#endregion

		#region E Opcodes

		/// <summary>
		/// Implementation of ediv opcode
		/// </summary>
		public static void ediv(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// TODO:  need debug. 

			// Format: opcode divr.rl, divd.rq, quo.wl, rem.wl

			if (ops[0].Op == 0 || ( Operand.Compare(ops[0].Op, 4, (0x8 * (int)Math.Pow(0x10, 4 * 2 - 1)), 4) == 0
				&& ops[1].Op == -1))
			{
				// In the integer divisor operand is 0, then in 2 operand integer format, 
				// the quotient operand is not affected; in 3 operand format the quotient 
				// operand is replaced by the dividend operand.

				theSimulator.WriteOnDestination(ops[2], 4, ops[1].Op);
				theSimulator.WriteOnDestination(ops[3], 4, 0);
				theSimulator.R.PSL.V = 1;
				if (ops[0].Op == 0) theSimulator.SendEvent(SimulatorEvents.ARITHMETIC, (int)ARITHMETIC_TRAPS.INTEGER_DIVIDE_BY_ZERO);
				return;
			}

			//      3. N, Z, are set as always.  V is set if overflow occurs, which can
			//         happen as follows:
			//         a. Division by zero is attempted.  (This also causes an exception)
			//         b. 80..0 is divided by -1.
			//         c. A real divisor has a negative exponent (i.e. represents a fraction),
			//            and the result exponent is too large
			//         C is always set to 0, since the operation always assumes signed numbers

			// N <-- quo LSS 0; Z <-- quo EQL 0; V <-- {overflow} OR {divr EQL 0}; C <-- 0;
	

			theSimulator.R.PSL.SetFlags(0, 0, Operand.IsZero(ops[1].Op / (int)ops[0].Op, 4), Operand.IsNeg(ops[1].Op / (int)ops[0].Op, 4));


			theSimulator.WriteOnDestination(ops[2], 4, ops[1].Op / (int)ops[0].Op);
			theSimulator.WriteOnDestination(ops[3], 4, ops[1].Op % (int)ops[0].Op);

		}

		#endregion

		#region H Opcodes

		/// <summary>
		/// Implementation of halt opcode
		/// </summary>
		public static void halt(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			throw new RuntimeError(SimulatorMessage.SYSTEM_HALTED, theSimulator.R.PC, 0);
		}

		#endregion

		#region I Opcodes

		/// <summary>
		/// Implementation of incb, incw, incl opcodes
		/// </summary>
		public static void inc(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.WriteOnDestination(ops[0], iOperandSize, ops[0].Op + 1);

			// N and Z are set as always.  V is set if 7F..F is incremented to
			// 80..0.  C is set if FF..F is incremented to 0.


			sbyte i_V = 0x8 * (int)Math.Pow(0x10, iOperandSize * 2 - 1) - 1 == ops[0].Op ? (sbyte)1 : (sbyte)0;
			long mask = 0;
			for (int iCounter = 0; iCounter < iOperandSize; ++iCounter) mask = (mask * 0x100) + 0xFF;
			sbyte i_C = mask == ops[0].Op ? (sbyte)1 : (sbyte)0;

			theSimulator.R.PSL.SetFlags(i_C, i_V , Operand.IsZero(ops[0].Op + 1, iOperandSize), 
				Operand.IsNeg(ops[0].Op + 1, iOperandSize));
		}

		/// <summary>
		/// Implementation of index opcode
		/// </summary>
		public static void index(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// opcode	 subscript.rl, low.rl, high.rl, size.rl, indexin.rl, indexout.wl

			// indexout <-- {indexin + subscript} *size; 
			int indexout = ((int)ops[4].Op + (int)ops[0].Op) * (int)ops[3].Op;


			// if {subscript LSS low} or {subscript GTR high} then 
			if (((int)ops[0].Op < (int)ops[1].Op) || ((int)ops[0].Op > (int)ops[2].Op))
				//		{subscript range trap};
				theSimulator.SendEvent(SimulatorEvents.ARITHMETIC, (int)ARITHMETIC_TRAPS.SUBSCRIPT_RANGE);

			// N <-- indexout LSS 0; Z <-- indexout EQL 0; V <-- 0; C <-- 0;
			theSimulator.R.PSL.SetFlags(0, 0, Operand.IsZero(indexout, 4), Operand.IsNeg(indexout, 4));


		}

		/// <summary>
		/// Implementation of mighty insque opcode
		/// </summary>
		public static void insque(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Format: opcode entry.ab, pred.ab
			//If (all memory accesses can be completed) then 
			// begin
			//		(entry) <-- (pred);		forward link of entry
			//		(entry+4) <-- pred;		backward link of entry
			//		((pred)+4) <-- entry;		backward link of successor
			//		(pred) <-- entry;		forward link of predecessor
			// end;
			// else
			// begin
			//		{backup instruction};		
			//		{initiate fault}
			// end;

			int entry	= (int)ops[0].EffectiveAddress;
			int pred	= (int)ops[1].EffectiveAddress;

			// (entry) <-- (pred)
			int pred_original_next = theSimulator.memory.Read(pred, 4);
			theSimulator.WriteOnDestination(ops[0], 4, pred_original_next);

			// (entry+4) <-- pred
			Operand tmp = new Operand(ops[0].EffectiveAddress + 4, ops[0].Op, ops[0].IsRegister, ops[0].HasEffectiveAddress);
			theSimulator.WriteOnDestination(tmp, 4, pred);

			// ((pred)+4) <-- entry
			Operand pred_next_prev = new Operand(pred_original_next + 4, 0, false, true);
			theSimulator.WriteOnDestination(pred_next_prev, 4, entry);

			// (pred) <-- entry
			theSimulator.WriteOnDestination(ops[1], 4, entry);
			

			// Flags:
			// N <-- (entry) LSS (entry+4);
			// Z <-- (entry) EQL (entry+4);
			// V <-- 0;
			// C <-- (entry) LSSU (entry+4);

			sbyte iN = Operand.IsNeg((int)theSimulator.memory.Read(entry, 4) - (int)theSimulator.memory.Read(entry+4, 4), 4);
			sbyte iZ = Operand.IsZero((int)theSimulator.memory.Read(entry, 4) - (int)theSimulator.memory.Read(entry+4, 4), 4);
			sbyte iC = (uint)theSimulator.memory.Read(entry, 4) < (uint)theSimulator.memory.Read(entry+4, 4) ? (sbyte)1 : (sbyte)0;
            
		    theSimulator.R.PSL.SetFlags(iC, 0, iZ, iN);


		}



		#endregion

		#region J Opcodes

		/// <summary>
		/// Implementation of jsb, bsbb opcodes
		/// </summary>
		public static void jsb(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			theSimulator.push(theSimulator.R[15], 4);
			theSimulator.R[15] = ops[0].EffectiveAddress;
			//theSimulator.R.PSL.SetFlags(-1, -1, -1, -1);
		}

		#endregion

		#region L Opcodes

		/// <summary>
		/// Implementation of locc, skpp opcodes
		/// </summary>
		public static void locc_skpc(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			byte theChar = (byte)ops[0].Op;
			ushort len = (ushort)ops[1].Op;
			int addr = ops[2].EffectiveAddress;
			bool bFind = (1 - (Opcode - 0x3A)) == 1 ? true : false;
			int iCounter;
			for (iCounter = 0; iCounter < len; ++iCounter)
			{
				if (bFind && theSimulator.memory[addr+iCounter] == theChar) break;
				else if (!bFind && theSimulator.memory[addr+iCounter] != theChar) break;
			}

			theSimulator.R[0] = len - iCounter;
			theSimulator.R[1] = addr + iCounter;

			theSimulator.R.PSL.SetFlags(0, 0, ((int)theSimulator.R[0] == 0) ? (sbyte)(1-(Opcode - 0x3A)) : (sbyte)(Opcode - 0x3A), 0);

			theSimulator.R.SystemTime += len / Simulator.ACCESS_MEMORY_FACTOR_TIME;

		}

		#endregion

		#region M Opcodes

		/// <summary>
		/// Implementation of matchc opcode.
		/// Last opcode to implement, thanks god!
		/// </summary>
		public static void matchc(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Format: opcode objlen.rw, objaddr.ab, srclen.rw, srcaddr.ab
			
			// Find substring (object) in character string

			ushort objlen = (ushort)ops[0].Op;
			int objaddr = ops[1].EffectiveAddress;
			ushort srclen = (ushort)ops[2].Op;
			int srcaddr = ops[3].EffectiveAddress;

			int iCounter = 0;
			CodeBlock obj = theSimulator.memory.Read(objaddr, objlen, true);

			// Dummy, ALEK doing all the work. for displaying accesses purposes
			theSimulator.memory.Read(srcaddr, srclen, true);

			bool bFoundMatch = false;

			for (iCounter = 0; iCounter < srclen - objlen; ++iCounter)
			{
				if (obj == theSimulator.memory.Read(srcaddr+iCounter, objlen, false))
				{
					bFoundMatch = true;
					break;
				}
			}

			theSimulator.R[0] = bFoundMatch ? (int)0 : (int)objlen;
			theSimulator.R[1] = bFoundMatch ? objaddr + objlen : objaddr;
			theSimulator.R[2] =  bFoundMatch ? srclen - (iCounter + objlen) : 0; 
			theSimulator.R[3] = bFoundMatch ? srcaddr - (iCounter + objlen) : srcaddr + srclen;

			theSimulator.R.PSL.SetFlags(0, 0, (int)theSimulator.R[0] == 0 ? (sbyte)1 : (sbyte)0, 0);
		}

		/// <summary>
		/// Implementation of mcomb, mcomw, mcoml opcodes
		/// </summary>
		public static void mcom(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.WriteOnDestination(ops[1], iOperandSize, ~ops[0].Op);
			theSimulator.R.PSL.SetFlags(-1, 0, Operand.IsZero(~ops[0].Op, iOperandSize), Operand.IsNeg(~ops[0], iOperandSize));
		}

		/// <summary>
		/// Implementation of mfpr opcode
		/// </summary>
		public static void mfpr(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iRegNumber = ops[0].EffectiveAddress;
			// SIRR, NICR, TXDB are write only
			if (iRegNumber == 20 || iRegNumber == 25 || iRegNumber == 35)
				throw new RuntimeError(SimulatorMessage.PRIVILEGED_IS_WRITE_ONLY, theSimulator.R[15], ops[0].EffectiveAddress);
											  
			theSimulator.WriteOnDestination(ops[1], 4, ops[0].Op);

			if (iRegNumber == 33) // RXDB
			{
				theSimulator.R[32].SetBits(7, 7, 0); // RXCS
			}
		}


		/// <summary>
		/// Implementation of mnegb, mnegw, mnegl opcodes
		/// </summary>
		public static void mneg(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Format: opcode src.rx, dst.wx

			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			long iValue;
			if (Opcode == 0x8E)  iValue = (long)((sbyte)ops[0].Op); // MNEGB
			else if (Opcode == 0xAE)  iValue = (long)((short)ops[0].Op); // MNEGW
			else if (Opcode == 0xCE)  iValue = (long)((int)ops[0].Op); // MNEGL
			else throw new PanicException();

			// 1. Integer overflow occurs if the source Operand is the largest negative integer 
			// (which has no positive counterpart).
			// On overflow, the destination operand is replaced by the source operand.

			if ( Operand.Compare(ops[0].Op, iOperandSize, (0x8 * (int)Math.Pow(0x10, iOperandSize * 2 - 1)), iOperandSize) == 0) 
			{
				theSimulator.WriteOnDestination(ops[1], iOperandSize, ops[0]);
				theSimulator.R.PSL.SetFlags(1, 1, 0, 1);
				return;
			}

			iValue = -iValue;
			theSimulator.WriteOnDestination(ops[1], iOperandSize, iValue);

			// N <-- dst LSS 0; Z <-- dst EOL 0; V <-- overflow; C <-- dst NEQ 0;
			theSimulator.R.PSL.SetFlags((iValue != 0) ? (sbyte)1 : (sbyte)0, 0, Operand.IsZero(iValue, iOperandSize), 
				Operand.IsNeg(iValue, iOperandSize));


		}

		/// <summary>
		/// Implementation of movb, movw, movl, movq opcodes
		/// </summary>
		public static void mov(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[3] - '0';
			theSimulator.WriteOnDestination(ops[1], iOperandSize, ops[0].Op);
			theSimulator.R.PSL.SetFlags(0, 0, Operand.IsZero(ops[0].Op, iOperandSize), Operand.IsNeg(ops[0], iOperandSize));
		}

		/// <summary>
		/// Implementation of movab, movaw, moval, movaq opcodes
		/// </summary>
		public static void mova(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.WriteOnDestination(ops[1], 4, ops[0].EffectiveAddress);
			theSimulator.R.PSL.SetFlags(0, 0, Operand.IsZero(ops[0].Op, iOperandSize), Operand.IsNeg(ops[0], iOperandSize));
		}

		/// <summary>
		/// Implementation of movc3 opcode
		/// </summary>
		public static void movc3(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// We takes the length as unsigned number
			ushort iLen = (ushort)ops[0].Op;
			int srcaddr = ops[1].EffectiveAddress;
			int dstaddr = ops[2].EffectiveAddress;
			for (int iCounter = 0; iCounter < iLen; ++iCounter)
				theSimulator.memory[dstaddr + iCounter] = theSimulator.memory[srcaddr + iCounter];

			// 1. After execution of MOVC3:
			//	R0 = 0, R1 = address of one byte beyond the source string
			//	R2 = 0, R3 = address of one byte beyond the destination string
			//	R4 = 0, R5 = 0
			theSimulator.R[0] = 0;	theSimulator.R[2] = 0;
			theSimulator.R[4] = 0;	theSimulator.R[5] = 0;
			theSimulator.R[1] = srcaddr + iLen;
			theSimulator.R[3] = dstaddr + iLen;

			// On MOVC3, or if the MOVC5 and the strings are of equal length, 
			// then Z is set and N, V, and C are cleared
			theSimulator.R.PSL.SetFlags(0, 0, 1, 0);

			theSimulator.R.SystemTime += iLen / Simulator.ACCESS_MEMORY_FACTOR_TIME;

		}

		/// <summary>
		/// Implementation of movc5 opcode
		/// </summary>
		public static void movc5(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Format opcode srclen.rw, srcaddr.ab, fill.rb, dstlen.rw, dstaddr.ab 

			// We takes the lengths as unsigned number
			ushort	iSrcLen = (ushort)ops[0].Op;
			ushort	iDstLen = (ushort)ops[3].Op;
			byte	fill	= (byte)ops[2].Op;

			int srcaddr = ops[1].EffectiveAddress;
			int dstaddr = ops[4].EffectiveAddress;

			// Copy the string
			for (int iCounter = 0; iCounter < Math.Min(iSrcLen, iDstLen); ++iCounter)
				theSimulator.memory[dstaddr + iCounter] = theSimulator.memory[srcaddr + iCounter];

			// pad the places that left on the destination
			if (iSrcLen < iDstLen)
			{
				for (int iCounter = iSrcLen; iCounter < iDstLen; ++iCounter)
					theSimulator.memory[dstaddr + iCounter] = fill;
			}

			//	After execution of MOVC5:
			//	R0 = number of unmoved bytes remaining in source string.
			//  R0 is non-zero only if source string is longer than destination string
			//	R1 address of one byte beyond the last byte in source string that was moved
			//	R2 = 0
			//	R3 = address of one byte beyond the destination string
			//	R4 = 0
			//	R5 = 0

			theSimulator.R[2] = 0;	theSimulator.R[4] = 0;	theSimulator.R[5] = 0;
			theSimulator.R[0] = iSrcLen > iDstLen ? iSrcLen - iDstLen : 0;
			theSimulator.R[1] = srcaddr + Math.Min(iSrcLen, iDstLen);
			theSimulator.R[3] = dstaddr + iDstLen;

			// N <-- srclen LSS dstlen;
			// Z <-- srclen EQL dstlen;
			// V <-- 0;
			// C <-- srclen LSSU dstlen;

			sbyte iN = (short)iSrcLen < (short)iDstLen ? (sbyte)1 : (sbyte)0;
			sbyte iZ = (short)iSrcLen < (short)iDstLen ? (sbyte)1 : (sbyte)0;
			sbyte iC = iSrcLen		  < iDstLen		   ? (sbyte)1 : (sbyte)0;

			theSimulator.R.PSL.SetFlags(iC, 0, iZ, iN);
			theSimulator.R.SystemTime += Math.Max(iSrcLen, iDstLen) / Simulator.ACCESS_MEMORY_FACTOR_TIME;
		}

		
		/// <summary>
		/// Implementation of movtc opcode
		/// </summary>
		public static void movtc(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Format opcode srclen.rw, srcaddr.ab, fill.rb, tbladdr.ab, dstlen.rw, dstaddr.ab 

			// We takes the lengths as unsigned number
			ushort	iSrcLen = (ushort)ops[0].Op;
			ushort	iDstLen = (ushort)ops[4].Op;
			
			byte	fill	= (byte)ops[2].Op;

			int srcaddr = ops[1].EffectiveAddress;
			int dstaddr = ops[5].EffectiveAddress;
			int tbladdr	= ops[3].EffectiveAddress;


			// Copy & translate the string
			for (int iCounter = 0; iCounter < Math.Min(iSrcLen, iDstLen); ++iCounter)
				theSimulator.memory[dstaddr + iCounter] = 
					theSimulator.memory[tbladdr + theSimulator.memory[srcaddr + iCounter]];

			// pad the places that left on the destination
			if (iSrcLen < iDstLen)
			{
				for (int iCounter = iSrcLen; iCounter < iDstLen; ++iCounter)
					theSimulator.memory[dstaddr + iCounter] = fill;
			}

			//	After execution of MOVTC:
			//	R0 = number of unmoved bytes remaining in source string.
			//  R0 is non-zero only if source string is longer than destination string
			//	R1 address of one byte beyond the last byte in source string that was translated
			//	R2 = 0
			//	R3 = address of one byte beyond the translated string
			//	R4 = 0
			//	R5 = address of one byte beyond the destination string

			theSimulator.R[2] = 0;	theSimulator.R[4] = 0;
			theSimulator.R[0] = iSrcLen > iDstLen ? iSrcLen - iDstLen : 0;
			theSimulator.R[1] = srcaddr + Math.Min(iSrcLen, iDstLen);
			theSimulator.R[3] = tbladdr;
			theSimulator.R[5] = dstaddr + iDstLen;

			// N <-- srclen LSS dstlen;
			// Z <-- srclen EQL dstlen;
			// V <-- 0;
			// C <-- srclen LSSU dstlen;

			sbyte iN = (short)iSrcLen < (short)iDstLen ? (sbyte)1 : (sbyte)0;
			sbyte iZ = (short)iSrcLen < (short)iDstLen ? (sbyte)1 : (sbyte)0;
			sbyte iC = iSrcLen		  < iDstLen		   ? (sbyte)1 : (sbyte)0;

			theSimulator.R.PSL.SetFlags(iC, 0, iZ, iN);
			theSimulator.R.SystemTime += iDstLen / Simulator.ACCESS_MEMORY_FACTOR_TIME;
		}

				
		/// <summary>
		/// Implementation of movtuc opcode
		/// </summary>
		public static void movtuc(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Format opcode srclen.rw, srcaddr.ab, esb.rb, tbladdr.ab, dstlen.rw, dstaddr.ab 

			// We takes the lengths as unsigned number
			ushort	iSrcLen = (ushort)ops[0].Op;
			ushort	iDstLen = (ushort)ops[4].Op;
			
			byte	esc		= (byte)ops[2].Op;

			int srcaddr = ops[1].EffectiveAddress;
			int dstaddr = ops[5].EffectiveAddress;
			int tbladdr	= ops[3].EffectiveAddress;

			sbyte iV = 0;

			// Copy & translate the string
			for (int iCounter = 0; iCounter < Math.Min(iSrcLen, iDstLen); ++iCounter)
			{
				if (theSimulator.memory[srcaddr + iCounter] == esc)
				{
					iV = 1;
					break;
				}
				theSimulator.memory[dstaddr + iCounter] = 
					theSimulator.memory[tbladdr + theSimulator.memory[srcaddr + iCounter]];
			}


			//	After execution of MOVTUC:
			//	R0 = number of unmoved bytes remaining in source string.
			//  R0 is non-zero only if source string is longer than destination string
			//	R1 address of one byte beyond the last byte in source string that was translated
			//	R2 = 0
			//	R3 = address of one byte beyond the translated string
			//	R4 = number of bytes remaining in the destination string.
			//	R5 = address of one byte beyond the destination string

			theSimulator.R[0] = iSrcLen > iDstLen ? iSrcLen - iDstLen : 0;
			theSimulator.R[1] = srcaddr + Math.Min(iSrcLen, iDstLen);
			theSimulator.R[2] = 0;
			theSimulator.R[3] = tbladdr;
			theSimulator.R[4] = iSrcLen < iDstLen ? iDstLen - iSrcLen : 0;
			theSimulator.R[5] = dstaddr + iDstLen;

			// N <-- srclen LSS dstlen;
			// Z <-- srclen EQL dstlen;
			// V <-- { terminated by escape };
			// C <-- srclen LSSU dstlen;

			sbyte iN = (short)iSrcLen < (short)iDstLen ? (sbyte)1 : (sbyte)0;
			sbyte iZ = (short)iSrcLen < (short)iDstLen ? (sbyte)1 : (sbyte)0;
			sbyte iC = iSrcLen		  < iDstLen		   ? (sbyte)1 : (sbyte)0;

			theSimulator.R.PSL.SetFlags(iC, iV, iZ, iN);
			theSimulator.R.SystemTime += iDstLen / Simulator.ACCESS_MEMORY_FACTOR_TIME;
		}



		/// <summary>
		/// Implementation of movpsl
		/// </summary>
		public static void movpsl(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			theSimulator.WriteOnDestination(ops[0], 4, (int)theSimulator.R.PSL);
			//theSimulator.R.PSL.SetFlags(-1, -1, -1, -1);
		}

		/// <summary>
		/// Implementation of movzbw, movzbl, movzwl opcodes
		/// </summary>
		public static void movz(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iTargetSize = new OpcodeEntry(Opcode, -1).OpType[3] - '0';
			theSimulator.WriteOnDestination(ops[1], iTargetSize, ops[0].Op > 0 ? ops[0].Op : ~ops[0].Op);
			theSimulator.R.PSL.SetFlags(-1, 0, Operand.IsZero(ops[0].Op, iTargetSize), 0);
		}

		/// <summary>
		/// Implementation of mtpr opcode
		/// </summary>
		public static void mtpr(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iRegNumber = ops[1].EffectiveAddress;
			// SCBB, ICR, RXDB: Read only register
			if (iRegNumber == 17 || iRegNumber == 26 || iRegNumber == 33)
				throw new RuntimeError(SimulatorMessage.PRIVILEGED_IS_READ_ONLY, theSimulator.R[15], iRegNumber);
				// IPL: Only bits 4:0 are taken. All the rest considered as zeros
			else if (iRegNumber == 18)
			{
				theSimulator.R[18].WriteLong((int)ops[0].Op & 0x1F);
				theSimulator.R.PSL.IPL = (byte)(ops[0].Op & 0x1F);
			}
				// SIRR: Only bits 3:0 are taken. We also change SISR (21)
			else if (iRegNumber == 20)
			{
				int iValue = (int)ops[0].Op & 0xF;
				theSimulator.R[20].WriteLong(iValue);
				if (iValue != 0)
				{
					if (iValue > theSimulator.R[18]) // R[18] is IPL. Interrupt should be taken immediatly
					{
						theSimulator.LaunchInterrupt(Simulator.SOFTWARE_INTERRUPTS_BASE_ADDRESS + theSimulator.R[17] + (iValue - 1) * 4
							, (byte)iValue);
						
					}
					else theSimulator.R[21] = theSimulator.R[21] | (1 << (iValue - 1));
				}
			}
				// ICCS: Changing bits makes special effects. Bits 30:8, 3:1 must be zero.
			else if (iRegNumber == 24)
			{
				Registers.Register temp = (int)ops[0].Op;
				if (temp.GetBits(8, 30) != 0 || temp.GetBits(1, 3) != 0) theSimulator.SendEvent(SimulatorEvents.RESERVED_OPERAND, -1);
				theSimulator.Console.activeTimer = (temp.GetBits(0,0) == 1) ? true : false;
				if (temp.GetBits(0, 0) == 0 && temp.GetBits(5, 5) == 1)
					theSimulator.R[26]++;
				// 25 is NICR
				if (temp.GetBits(4, 4) == 1) theSimulator.R[26] = theSimulator.R[25];
				temp.SetBits(31, 31, 0);
				temp.SetBits(7, 7, 0);
				temp.SetBits(4, 4, 0);
				theSimulator.R[24] = (int)temp;
			}
				// 30 is OSC
			else if (iRegNumber == 30)
			{
				MessageBeepPInvoke MBOld = MessageBeepPInvoke.theMessageBeepPInvoke;
				MBOld.PlayBeep((int)ops[0].Op, (int)PCSpeaker.DefaultDurTime);
			}
			else if (iRegNumber == 34)
			{
				if (Registers.Register.GetBits(6, 6, (int)ops[0].Op) == 1)
				{
					theSimulator.R[34] = (int)ops[0].Op;
					theSimulator.SendEvent(SimulatorEvents.OUTPUT_INTERRUPT, -1);
				}
				//theSimulator.R[34].SetBits(7, 7, 0);

			}
			else if (iRegNumber == 35)
			{
				theSimulator.R[35] = (int)ops[0].Op;
				if (theSimulator.R[34].GetBits(6,6) == 1)
				{
					theSimulator.R[34].SetBits(7,7,1);
					//theSimulator.SendEvent(SimulatorEvents.OUTPUT_INTERRUPT, -1);
				}
			}
			else theSimulator.WriteOnDestination(ops[1], 4, ops[0].Op);
		
		}

		/// <summary>
		/// Implementation of mulb2, mulw2, mull2 opcodes
		/// </summary>
		public static void mul2(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			long res = ops[0].Op * ops[1].Op;
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.WriteOnDestination(ops[1], iOperandSize, res);
			theSimulator.R.PSL.SetFlags(0,  Operand.HasCarry(ops[0].Op * ops[1].Op, iOperandSize) 
				/* Although it is check carry function, it applies as overflow in this case */, 
				Operand.IsZero(res, iOperandSize), Operand.IsNeg(res, iOperandSize));
		}

		/// <summary>
		/// Implementation of mulb3, mulw3, mull3 opcodes
		/// </summary>
		public static void mul3(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			long res = ops[0].Op * ops[1].Op;
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.WriteOnDestination(ops[2], iOperandSize, res);
			theSimulator.R.PSL.SetFlags(0, Operand.HasCarry(ops[0].Op * ops[1].Op, iOperandSize) 
				/* Although it is check carry function, it applies as overflow in this case */,
				Operand.IsZero(res, iOperandSize), Operand.IsNeg(res, iOperandSize));
		}

		#endregion

		#region N Opcodes

		/// <summary>
		/// Implementation of nop opcode
		/// </summary>
		public static void nop(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			return;
		}

		#endregion

		#region P Opcodes

		/// <summary>
		/// Implementation of popr opcode
		/// </summary>
		public static void popr(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			Registers.Register temp = (int)ops[0].Op;
			for (int i = 0; i < 15; ++i) 
				if (temp.GetBits(i, i) != 0) theSimulator.R[i] = theSimulator.pop(4);
		}

		/// <summary>
		/// Implementation of pushl opcode
		/// </summary>
		public static void push(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.push(ops[0].Op, iOperandSize);
			theSimulator.R.PSL.SetFlags(0, 0, Operand.IsZero(ops[0].Op, iOperandSize), Operand.IsNeg(ops[0], iOperandSize));
		}

		/// <summary>
		/// Implementation of pushab, pushaw, pushal
		/// </summary>
		public static void pusha(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.push(ops[0].EffectiveAddress, iOperandSize);
			theSimulator.R.PSL.SetFlags(-1, 0, Operand.IsZero(ops[0].Op, iOperandSize), Operand.IsNeg(ops[0], iOperandSize));
		}

		/// <summary>
		/// Implementation of pushr opcode
		/// </summary>
		public static void pushr(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			Registers.Register temp = (int)ops[0].Op;
			for (int i = 14; i >=0; --i) 
				if (temp.GetBits(i, i) != 0) theSimulator.push(theSimulator.R[i], 4);
		}

		#endregion

		#region R Opcodes

		/// <summary>
		/// Implementation of remque opcode
		/// </summary>
		public static void remque(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Format: opcode entry.ab, addr.wl
			//
			// If (all memory accesses can be completed) then 
			// begin
			//		((entry+4)) <-- (entry); 	forward link of predecessor
			//		((entry)+4) <-- (entry+4); 	backward link of successor
			//		addr <-- entry; 
			// end;
			// else
			// begin
			//		{backup instruction};		
			//		{initiate fault}			
			// end;

			int entry	= (int)ops[0].EffectiveAddress;

			// ((entry+4)) <-- (entry)
			theSimulator.memory.Write((int)theSimulator.memory.Read(entry+4, 4), theSimulator.memory.Read(entry, 4));

			// ((entry)+4) <-- (entry+4)
			theSimulator.memory.Write((int)(theSimulator.memory.Read(entry, 4)) + 4, theSimulator.memory.Read(entry+4, 4));

			// addr <-- entry
			theSimulator.WriteOnDestination(ops[1], 4, entry);




			// Flags:
			// N <-- (entry) LSS (entry+4);
			// Z <-- (entry) EQL (entry+4);	
			// V <-- entry EQL (entry+4);
			// C <-- (entry) LSSU (entry+4);

			sbyte iN = Operand.IsNeg((int)theSimulator.memory.Read(entry, 4) - (int)theSimulator.memory.Read(entry+4, 4), 4);
			sbyte iZ = Operand.IsZero((int)theSimulator.memory.Read(entry, 4) - (int)theSimulator.memory.Read(entry+4, 4), 4);
			sbyte iV = Operand.IsZero(entry - (int)theSimulator.memory.Read(entry+4, 4), 4);
			sbyte iC = (uint)theSimulator.memory.Read(entry, 4) < (uint)theSimulator.memory.Read(entry+4, 4) ? (sbyte)1 : (sbyte)0;
            
			theSimulator.R.PSL.SetFlags(iC, iV, iZ, iN);


		}


		/// <summary>
		/// Implementation of rei opcode
		/// </summary>
		public static void rei(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			theSimulator.R[15].WriteLong(theSimulator.pop(4));
			theSimulator.R.PSL.WriteLong(theSimulator.pop(4));
			theSimulator.R[18].WriteLong(theSimulator.R.PSL.IPL);

			if (theSimulator.R.PSL.GetBits(28, 29) != 0 ||
				theSimulator.R.PSL.GetBits(21, 21) != 0	||
				theSimulator.R.PSL.GetBits(8, 15) != 0)
			{
				theSimulator.SendEvent(SimulatorEvents.RESERVED_OPERAND, -1);
				return;
			}
		}

		/// <summary>
		/// Implementation of ret opcode
		/// </summary>
		public static void ret(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// 1.	sp <-- fp+4
			theSimulator.R[14] = theSimulator.R[13] + 4;

			// 2.	temp <-- Misc. + PSW
			int temp = theSimulator.pop(4);

			// 3.	Restoring the pc, fp, ap registers
			theSimulator.R[12] = theSimulator.pop(4);
			theSimulator.R[13] = theSimulator.pop(4);
			theSimulator.R[15] = theSimulator.pop(4);

			// 4.	Restoring r0-r11 according to the mask
			int RegMask = (temp >> 16) & 0xFFFF;
			for (int iCounter = 0; iCounter <= 11; ++iCounter)
				if ( ((RegMask >> iCounter) & 1) == 1 ) 
					theSimulator.R[iCounter] = theSimulator.pop(4);

			// 5.	Restoring SP using SPA from temp
			int SPA = ((temp >> 29) >> 1) & 0x03;
			for (int iCounter = 0; iCounter < SPA; ++iCounter) theSimulator.pop(1);

			// 6.	Restoring PSW from temp
			theSimulator.R.PSL.PSW = temp & 0xFFFF;

			// 7.	Skipping the fill using SPA
			int s = ((temp >> 28) >> 1) & 1;

			// 8.	If S = 1, then read N and jump over the parameters. (Assuming each parameter is exact one longword)
			if (s == 1)
			{
				int N = theSimulator.pop(4); // N
				for (int iCounter = 0; iCounter < N; ++iCounter) theSimulator.pop(4);
			}
				// callg - nothing needs to be done
			else return;
		}

		/// <summary>
		/// Implementation of rsb opcode
		/// </summary>
		public static void rsb(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			theSimulator.R[15] = (int)theSimulator.pop(4);
			theSimulator.R.PSL.SetFlags(-1, -1, -1, -1);
		}

		/// <summary>
		/// Implementation of rotl opcode
		/// </summary>
		public static void rotl(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Format: opcode cnt.rb, src.rl, dst.wl
			sbyte cnt = (sbyte)ops[0].Op;
			Registers.Register src = (Registers.Register)(int)ops[1].Op;
			int dst;
			
			//dst <-- src rotated cnt bits;
			if (cnt > 0)
			{
				int theOutgoingBits = src.GetBits(32 - (cnt % 32), 31);
				int theRestBits	 = src.GetBits(0, 32 - (cnt % 32) - 1);
				dst = (theRestBits << (cnt % 32)) + theOutgoingBits;
			}
			else if (cnt < 0)
			{
				int theOutgoingBits = src.GetBits(0, Math.Abs(cnt) % 32 - 1);
				dst = (int)((uint)src >> (Math.Abs(cnt) % 32)) + theOutgoingBits * (int)Math.Pow(2, 32 - Math.Abs(cnt) % 32) ;
			}
			else dst = src;

			theSimulator.WriteOnDestination(ops[2], 4, dst);

			// N <-- dst LSS 0; Z <-- dst EQL 0; V <-- 0; C <-- C;
			theSimulator.R.PSL.SetFlags(-1, 0, Operand.IsZero(dst, 4), Operand.IsNeg(dst, 4));
		}

		#endregion

		#region S Opcodes


		/// <summary>
		/// Implementation of scanc, spanc opcodes
		/// </summary>
		public static void scanc_spanc(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Format: opcode len.rw, addr.ab, tbladdr.ab, mask.rb
			ushort len = (ushort)ops[0].Op;
			int addr = ops[1].EffectiveAddress;
			int tbladdr = ops[2].EffectiveAddress;
			byte mask = (byte)ops[3].Op;
			bool bStopOnZero = (Opcode == 0x2B) ? true : false; // Or stop on non-zero
		
			int iCounter;
			for (iCounter = 0; iCounter < len; ++iCounter)
			{
				byte theByte = theSimulator.memory[tbladdr + (byte)theSimulator.memory[addr+iCounter]];
				if (bStopOnZero && (theByte & mask)  == 0) break;
				else if (!bStopOnZero && (theByte & mask) != 0) break;
			}

			theSimulator.R[0] = len - iCounter; // Not sure it is correct
			theSimulator.R[1] = addr + iCounter;
			theSimulator.R[2] = 0;
			theSimulator.R[3] = tbladdr;

			theSimulator.R.PSL.SetFlags(0, 0, (int)theSimulator.R[0] == 0 ? (sbyte)1 : (sbyte)0, 0);
		}


		/// <summary>
		/// Implementation of sobgtr, sobgeq opcodes
		/// </summary>
		/// <remarks>TESTED!!! This opcode was fully tested - no future work is required</remarks>
		public static void sob(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			theSimulator.WriteOnDestination(ops[0], 4, ops[0].Op - 1);
			sbyte i_V = (0x8 * (int)Math.Pow(0x10, 4 * 2 - 1)) == (int)ops[0].Op ? (sbyte)1 : (sbyte)0;
			theSimulator.R.PSL.SetFlags(-1, i_V, Operand.IsZero(ops[0].Op - 1, 4), Operand.IsNeg(ops[0].Op - 1, 4));

			// sobgeq
			if (Opcode == 0xF4 && ((int)ops[0].Op - 1) >= 0) theSimulator.R[15] = ops[1].EffectiveAddress;
			// sobgtr
			else if (Opcode == 0xF5 && ((int)ops[0].Op - 1) > 0) theSimulator.R[15] = ops[1].EffectiveAddress;
		}

		/// <summary>
		/// Implementation of subb2, subw2, subl3, subb3, subw3, subl3 opcodes
		/// </summary>
		/// <remarks>TESTED!!! This opcode was fully tested - no future work is required</remarks>
		public static void sub(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iOperandSize;

			if (Opcode == 0xC2 || Opcode == 0xC3) iOperandSize = 4; // subl2, subl3
			else if (Opcode == 0xA2 || Opcode == 0xA3) iOperandSize = 2; // subw2, subw3
			else if (Opcode == 0x82 || Opcode == 0x83) iOperandSize = 1; // subb2, subb3
			else throw new PanicException();

			if (Opcode == 0xC2 || Opcode == 0xA2 || Opcode == 0x82)
				theSimulator.WriteOnDestination(ops[1], iOperandSize, ops[1].Op - ops[0].Op);
			else theSimulator.WriteOnDestination(ops[2], iOperandSize, ops[1].Op - ops[0].Op);

			// N and Z are set as always.  V is set if SIGNED overflow occurs, and
			// C is set if UNSIGNED integer overflow occurs.  C is always set to 0
			// for real data types.
			// Integer overflow occurs if the input operands to the subtract are of different signs
			// and the sign of the result is the sign of the subtrahend
			sbyte iC = Operand.CompareUnsigned(ops[0].Op, iOperandSize, ops[1].Op, iOperandSize) == 1 ? (sbyte)1 : (sbyte)0;
			theSimulator.R.PSL.SetFlags(iC, Operand.SumHasOverflow(ops[1].Op, -ops[0].Op, iOperandSize), 
				Operand.IsZero(ops[1].Op - ops[0].Op, iOperandSize), Operand.IsNeg(ops[1].Op - ops[0].Op, iOperandSize));
		}


		/// <summary>
		/// Implementation of sbwc opcodes
		/// </summary>
		public static void sbwc(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			// Format: opcode sub.rl, dif.ml

			int res = (int)ops[1].Op - (int)ops[0].Op - (int)theSimulator.R.PSL.C;
			theSimulator.WriteOnDestination(ops[1], 4, res);


			// N and Z are set as always.  V is set if SIGNED overflow occurs, and
			// C is set if UNSIGNED integer overflow occurs.  C is always set to 0
			// for real data types.

			sbyte iV = Math.Max(Operand.SumHasOverflow(ops[1].Op, -ops[0].Op, 4),
				Operand.SumHasOverflow((int)ops[1].Op - (int)ops[0].Op, -(int)theSimulator.R.PSL.C, 4));

			theSimulator.R.PSL.SetFlags(Operand.HasCarry(res, 4), iV, Operand.IsZero(res, 4), Operand.IsNeg(res, 4));
		}


		#endregion

		#region T Opcodes

		/// <summary>
		/// Implementation of tstb, tstw, tstl opcodes
		/// </summary>
		/// <remarks>TESTED!!! This opcode was fully tested - no future work is required</remarks>
		public static void tst(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.R.PSL.SetFlags(0, 0, Operand.IsZero(ops[0].Op, iOperandSize), Operand.IsNeg(ops[0], iOperandSize));
		}

		#endregion

		#region X Opcodes


		/// <summary>
		/// Implementation of xorb2, xorw2, xorl2 opcodes
		/// </summary>
		/// <remarks>TESTED!!! This opcode was fully tested - no future work is required</remarks>
		public static void xor2(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			long dst = ops[0].Op ^ ops[1].Op;
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.WriteOnDestination(ops[1], iOperandSize, dst);
			theSimulator.R.PSL.SetFlags(-1, 0, Operand.IsZero(dst, iOperandSize), 
				Operand.IsNeg(dst, iOperandSize));
		}

		/// <summary>
		/// Implementation of xorb3, xorw3, xorl3 opcodes
		/// </summary>
		/// <remarks>TESTED!!! This opcode was fully tested - no future work is required</remarks>
		public static void xor3(Simulator theSimulator, int Opcode, Operand[] ops)
		{
			long dst = ops[0].Op ^ ops[1].Op;
			int iOperandSize = new OpcodeEntry(Opcode, -1).OpType[1] - '0';
			theSimulator.WriteOnDestination(ops[2], iOperandSize, dst);
			theSimulator.R.PSL.SetFlags(-1, 0, Operand.IsZero(dst, iOperandSize), 
				Operand.IsNeg(dst, iOperandSize));
		}

		#endregion

	}
}
