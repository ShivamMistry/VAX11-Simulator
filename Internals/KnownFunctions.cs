using System;
using System.Collections;

using VAX11Compiler;

namespace VAX11Internals
{
	/// <summary>
	/// Summary description for KnownFunctions.
	/// </summary>
	public class KnownFunctions
	{
		private class SystemCallEntry
		{
			public string	sFunctionName;
			public int		iAddress;
			public int		iClockCycles;
		
			public SystemCallEntry(string FunctionName, int Address, int ClockCycles)
			{
				sFunctionName	= FunctionName;
				iAddress		= Address;
				iClockCycles	= ClockCycles;
			}

		}

		private static SystemCallEntry[] SysCalls =
			{
				new SystemCallEntry("GETCHAR", 0xffffff, 16),
				new SystemCallEntry("PUTCHAR", 0xfffffe, 16),
				new SystemCallEntry("GETS", 0xfffffd, 124),
				new SystemCallEntry("PUTS", 0xfffffc, 125),
				new SystemCallEntry("SCANF", 0xfffffb, 710),
				new SystemCallEntry("PRINTF", 0xfffffa, 700),
				new SystemCallEntry("SPRINTF", 0xfffff9, 700),
				new SystemCallEntry("MALLOC", 0xffff8, 25),
				new SystemCallEntry("EXIT", 0xfffff7, 15),
				new SystemCallEntry("FREE", 0xfffff6, 25),
				new SystemCallEntry("CLEARDEVICE", 0xffffdf, 60),
				new SystemCallEntry("LINE", 0xffffde, 80),
				new SystemCallEntry("PUTPIXEL", 0xffffdd, 45),
				new SystemCallEntry("CIRCLE", 0xffffdc, 165),
				new SystemCallEntry("RECTANGLE", 0xffffdb, 100),
				new SystemCallEntry("SETCOLOR", 0xffffda, 35),
				new SystemCallEntry("GETMAXX", 0xffffd9, 20),
				new SystemCallEntry("GETMAXY", 0xffffd8, 20),
				new SystemCallEntry("OUTTEXTXY", 0xffffd7, 315),
				new SystemCallEntry("SETFONT", 0xffffd6, 110),
				new SystemCallEntry("INITGRAPH", 0xffffd5, 315),
				new SystemCallEntry("CLOSEGRAPH", 0xffffd4, 115),
			};

		private static Hashtable ProcHash = new Hashtable();
		private static Hashtable ProcValuesHash = new Hashtable();

		static KnownFunctions()
		{
			foreach (SystemCallEntry entry in SysCalls)
			{
				ProcHash[entry.sFunctionName] = entry;
				ProcValuesHash[entry.iAddress] = entry;
			}
		}

		/// <summary>
		/// Gets known function name and return its address. Might throw CompileError.
		/// </summary>
		/// <param name="sFunctionName">Function Name</param>
		/// <returns>Function Address</returns>
		public static int GetAddress(string sFunctionName)
		{
			sFunctionName = sFunctionName.ToUpper();
			if (!ProcHash.ContainsKey(sFunctionName))
			{
				throw new CompileError(CompilerMessage.UNRECOGNIZED_PROCEDURE);
			}
			return ((SystemCallEntry)ProcHash[sFunctionName]).iAddress;
		}

		/// <summary>
		/// Get function name from its address.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string GetFunctionName(int key)
		{
			if (!ProcValuesHash.ContainsKey(key)) return "";
			return ((SystemCallEntry)ProcValuesHash[key]).sFunctionName;
		}

		/// <summary>
		/// Get clock cycles for each systel call
		/// </summary>
		/// <param name="sFunctionName">SysCall Name</param>
		/// <returns></returns>
		public static int GetCycles(string sFunctionName)
		{
			sFunctionName = sFunctionName.ToUpper();
			if (!ProcHash.ContainsKey(sFunctionName)) throw new PanicException();
			return ((SystemCallEntry)ProcHash[sFunctionName]).iClockCycles;
		}
	}
}
