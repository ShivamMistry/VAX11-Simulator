using System;
using System.Threading;
using System.Timers;

using VAX11Simulator;
using VAX11Internals;
using VAX11Settings;


namespace VAX11Environment
{
	/// <summary>
	/// VAX-11 Program.
	/// </summary>
	public class Program
	{
		#region Members

		private VAX11Simulator.Simulator theSimulator;
		private VAX11Simulator.Console con;
		private Thread clockThread;
		private frmMain _theMainApplication;
		private ProgramBlock theProgram;
		private BreakPointList _BreakPointList;
		private bool bInDebug;
		private bool bStepIn;
		private bool bConsoleClosed = false;

		#endregion

		#region Globals - LOL

		public frmRegistersView frmRegistersOutput;
		public frmMemoryView	frmMemoryOutput;
		public frmStackView		frmStackOutput;

		#endregion

		#region Semaphores

		/// <summary>
		/// When the program reachs a break-point, it waits on this semaphore
		/// </summary>
		public System.Threading.AutoResetEvent BreakPointWait;

		#endregion

		#region Events and Delegates

		/// <summary>
		/// theProgram - Program that reached the break-point.
		/// iPC - PC when the break-point occured.
		/// sMessage - message to display to the user, or null if none.
		/// </summary>
		public delegate void BreakPointHandler(Program theProgram, int iPC, string sMessage);

		/// <summary>
		/// Event raises when debugged application reachs break-point.
		/// </summary>
		public event BreakPointHandler OnBreakPoint;


		/// <summary>
		/// Delegate for program end handler
		/// </summary>
		public delegate void ProgramEndHandler(string sExitMessage);

		#endregion

		#region Constructor

		/// <summary>
		/// Creates new program. Get CodeBlock to load into the simulator's memory
		/// </summary>
		/// <param name="cbProgram">CodeBlock to load into the simulator's memory</param>
		/// <param name="DebugMode">True if the new program suppose to run in debug mode</param>
		public Program(ProgramBlock cbProgram, bool DebugMode, frmMain theMainApplication, string sInputFile, string sOutputFile)
		{
			theProgram = cbProgram;
			_theMainApplication = theMainApplication;
			BreakPointWait = new AutoResetEvent(false);
			_BreakPointList = new BreakPointList();
			bInDebug = DebugMode;
			con = new VAX11Simulator.Console(sInputFile, sOutputFile);
			theSimulator = new VAX11Simulator.Simulator(theProgram, con);
			theSimulator.memory.OnMemoryAccess += new MemoryAccessedFunc(MemoryAcessedHandler);
			theSimulator.memory.OnPageFault += new PageFaultFunc(PageFaultHandler);
			con.InterruptsEvent += new VAX11Simulator.Console.InterruptsDelegate(InterruptHendler);
		}



		#endregion

		#region Methods


		/// <summary>
		/// Main program loop. Maintains the running program.
		/// When in debug mode, it returns when it reachs break-point.
		/// In normal mode the program is the main loop of a thread, so it has infinite loop.
		/// </summary>
		public void Run()
		{
			try
			{
				while (!bConsoleClosed)
				{
					// Break-point?
					if (bInDebug)
					{
						if ( (_BreakPointList.ContainsPC(theSimulator.R[15])) || (bStepIn))
						{
							UpdateDebugViews();

							if (OnBreakPoint != null) OnBreakPoint(this, theSimulator.R[15], null);
							BreakPointWait.WaitOne();
						}
					}

					// Perform next command
					theSimulator.PerformNextCommand();
					Thread.Sleep(0);
				}
				
			}
			catch (RuntimeError e)
			{
				// Make sure that the output thread will be closed. (We hope)
				theSimulator.OutputThreadOn = false;

				if (e.ErrorCode == SimulatorMessage.CONSOLE_EXIT) return;

				// Display message
				string s;
				if (e.ErrorCode == SimulatorMessage.NORMAL_EXIT)
				{
					if (e.ExtraInformation == 0)
						s = "\nProgram ended successfully. Press any key...";
					else 
						s = "\nProgram ended with errorcode " + e.ExtraInformation + ". Press any key...";
				}
				else if (e.ErrorCode == SimulatorMessage.SYSTEM_HALTED)
				{
					s = "\n" + SimulatorMessagesStrings.GetMessageText(e.ErrorCode, e.ExtraInformation)+ ". Press any key...";
				}
					// ILLEGAL_EXIT occurs when we write on the console, when it already has disposed
				else
				{
					s = "\nRuntime Error: " + SimulatorMessagesStrings.GetMessageText(e.ErrorCode, e.ExtraInformation)+ ". Press any key...";

					if (Settings.Simulator.bShowDebugInformation) s += GenerateDebugInformation(e);
				}
				try
				{
					con.Invoke (theSimulator.Console.m_Output, new object[] { (CodeBlock)s });
				}
				catch (VAX11Simulator.RuntimeError)
				{
				}

				if (bInDebug)
				{
					UpdateDebugViews();
				}

				theSimulator.Console.exitConsole(true);

				// Must be so ugly cuz of c# stupid components threading policy
				if (bInDebug) _theMainApplication.Invoke(new ProgramEndHandler(_theMainApplication.InterfaceDoRestartDebuggedProgram), new object[] {s});

				_theMainApplication.Activate();
			}
		}



		/// <summary>
		/// Adds a break-point
		/// </summary>
		/// <param name="iPC">PC of the break-point</param>
		public void AddBreakPoint(int iLine, int iPC)
		{
			_BreakPointList.AddEntry(new BreakPointEntry(iLine, iPC));
		}

		/// <summary>
		/// Deletes a break-point
		/// </summary>
		/// <param name="iPC">PC of the break-point</param>
		public void DelBreakPoint(int iLine)
		{
			_BreakPointList.DelEntry(iLine);
		}


		public void ClearAllBreakPoints()
		{
			_BreakPointList.ResetTable();
		}

		/// <summary>
		/// Shows the console
		/// </summary>
		public void ShowConsole()
		{
			con.Show();
		}


		/// <summary>
		/// Sample memory hanlder. If you extend the memory, for example adding watchs, etc,
		/// take a look in this place
		/// </summary>
		private void MemoryAcessedHandler(int iAddress, int iPhysicalAddress, int iValue, bool IsWrite)
		{
			if (Settings.Simulator.bShowAccessesToMemory)
			{
				string s = "";
				if (Settings.Simulator.bShowPhysicalAddresses)
					s = "[PHYSICAL: " + Convert.ToString(iPhysicalAddress, 16) + "] ";
				s += "[MEM: " + Convert.ToString(iAddress, 16) + "] [VALUE: " + Convert.ToString(iValue, 16) + ((IsWrite) ? "] [WRITE]" : "] [READ]") + "\n";
				con.Invoke (theSimulator.Console.m_Output, new object[] { (CodeBlock)s });
			}
			if (bInDebug && IsWrite) frmMemoryOutput.UpdateCells(iAddress, 1);
		}

		/// <summary>
		/// Page fault handler
		/// </summary>
		private void PageFaultHandler(int iNewVirtualPage, int iNewPhysicalPage, bool bNeedSwapping, int iOldVirtualPage)
		{
			if (Settings.Simulator.bShowPageFaults)
			{
				string s = 
					"PAGEFAULT: [VPAGE: " + Convert.ToString(iNewVirtualPage, 16) + "] [PPAGE: " + Convert.ToString(iNewPhysicalPage, 16) + ((bNeedSwapping) ? "] [VPOLD: " + Convert.ToString(iOldVirtualPage, 16) + "]" : "] [NOSWAP]") + "\n";
				con.Invoke (theSimulator.Console.m_Output, new object[] { (CodeBlock)s });
			}
		}


		/// <summary>
		/// Generate detailed report when the simulator crashs
		/// </summary>
		/// <param name="e">The error caused the crash</param>
		/// <returns>Detailed error message</returns>
		private string GenerateDebugInformation(RuntimeError e)
		{
			string s = "";
			s += "\n\nDEBUG Information:\n---------------------------------\n";
			for (int iCounter = 0; iCounter < 12; iCounter++) s += "R" + iCounter + "=" + theSimulator.R[iCounter].ToString16() + "\t";
			s += "AP=" + theSimulator.R[12].ToString16() + "\tFP=" + theSimulator.R[13].ToString16() + "\tSP=" +
				theSimulator.R[14].ToString16() + "\tPC=" + theSimulator.R[15].ToString16() + "\t";
			s += "C=" + theSimulator.R.PSL.C + "\tV=" + theSimulator.R.PSL.V
				+ "\tN=" + theSimulator.R.PSL.N + "\tZ=" + theSimulator.R.PSL.Z + "\n";
			s += "Error occured at address " + Convert.ToString(e.PC, 16) + "\n";
			s += "STACK:\n---------------------------------\n";

			int iCurrAddr = theSimulator.R[14];
			for (int iCounter = 0; iCounter < 3; ++iCounter) // 3 = number of lines
			{
				s+= "0x" + Convert.ToString(iCurrAddr, 16) + ":\t";
				for (int i = 0; i < 8; ++i)
				{
					s += Convert.ToString((uint)theSimulator.memory.Read(iCurrAddr, 4, false), 8).ToUpper() + "\t";
					iCurrAddr += 4;
				}
				s += "\n";
			}
			return s;
		}

		/// <summary>
		/// Update the display of the stack, registers and memory windows
		/// </summary>
		public void UpdateDebugViews()
		{
			frmRegistersOutput.UpdateRegistersDisplay();
			frmStackOutput.UpdateStackDisplay();
			frmMemoryOutput.UpdateMemoryDisplay();
		}



		#endregion

		#region Interrupts

		private void InterruptHendler (SimulatorEvents Interrupt, int info)
		{
			if (Interrupt == SimulatorEvents.CLOCK_INTERRUPT)
			{
				if (info == VAX11Simulator.Console.INIT_CLOCK)
					InitClockInterrupt();
				else if (info == VAX11Simulator.Console.ABORT_CLOCK)
					AbortClockInterrupt();
				else throw new PanicException();
			}
			else if (Interrupt == SimulatorEvents.POWER_DOWN_INTERRUPT)
			{
				//if we want to send event do it here.
				bConsoleClosed = true;
				BreakPointWait.Set();
				if (bInDebug && info == 1) _theMainApplication.Invoke(new ProgramEndHandler(_theMainApplication.InterfaceDoRestartDebuggedProgram), new object[] {"Program ended because Console has been closed"});
			}
			else
				theSimulator.SendEvent(Interrupt, info);
		}

		private void InitClockInterrupt()
		{
			clockThread = new Thread(new ThreadStart(ClockThreadHandler));
			clockThread.Name = "clock interrupt";
			clockThread.Start();	
		}

		private void AbortClockInterrupt()
		{
			if (clockThread.IsAlive) clockThread.Abort();
		}

		private void ClockThreadHandler()
		{
			try 
			{
				while (con.activeTimer) 
				{
					Thread.Sleep(Settings.Simulator.ClockResolution);
					SimulatorTimeCallback();
				}
			} 
			catch (Exception) 
			{
			}
		}

		private void SimulatorTimeCallback()
		{
			theSimulator.SendEvent(SimulatorEvents.CLOCK_INTERRUPT, -1);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Get access to the program's simulator. Used mainly in debug mode
		/// </summary>
		public Simulator sim
		{
			get { return theSimulator; }
		}


		/// <summary>
		/// Do we run in step-by-step mode?
		/// </summary>
		public bool bStepByStep
		{
			get { return bStepIn; }
			set { bStepIn = value; }
		}

		/// <summary>
		/// Contains the list of all breakpoints for the current program
		/// </summary>
		public BreakPointList cBreakPointList
		{
			get { return _BreakPointList; }
			set { _BreakPointList = value; }
		}

		/// <summary>
		/// Is the program running in debug mode?
		/// </summary>
		public bool bInDebugMode
		{
			get { return bInDebug; }
		}

		#endregion
	}
}
