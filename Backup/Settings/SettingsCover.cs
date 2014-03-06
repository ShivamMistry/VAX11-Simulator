using System;
using System.Drawing;

namespace VAX11Settings
{
	[Serializable]
	public class SettingsCover
	{

		public _Environment Environment = new SettingsCover._Environment();
		public _Assembler Assembler = new SettingsCover._Assembler();
		public _Simulator Simulator = new SettingsCover._Simulator();

		[Serializable]
			public class _Environment
		{
			//colors settings
			public Color TEXT_COLOR = Color.Black;
			public Color COMMENT_COLOR = Color.Green;
			public Color LABEL_COLOR = Color.Brown;
			public Color FUNCTION_COLOR = Color.Red;
			public Color DIRECTIVE_COLOR = Color.Blue;
			public Color COMMAND_COLOR = Color.Blue;
			public Color STRING_COLOR = Color.Gray;
			public Color BACKGROUND_COLOR = Color.White;
			public Color DEBUG_LINE_COLOR = Color.Yellow;
			public Color BREAKPOINT_COLOR = Color.LawnGreen;
			public Color ERRORS_COLOR = Color.LightSteelBlue;

			// general settings
			public bool OpenLSTFileAfterCompile = false;
			public bool bDoSyntaxHighlight = true;
			public bool bShowAgentOnStartup = true;
			public bool bLoadLastFileOnStartup = false;
			public string sLastFileLocation = "";
			public string[] a_sLastFiles = { "", "", "", "", "" };

			// Layout Settings
			public string EditModeLayout = "";
			public string DebugModeLayout = "";
		}

		[Serializable]
		public class _Assembler
		{
			public bool bOptimaizeCode = true;
			public bool bSaveLSTFileAfterCompile = false;
		}

		[Serializable]
		public class _Simulator
		{
			// Memory
			public int PageSize = 512; // In bytes
			public int MemorySize = 65536;
			public bool bShowAccessesToMemory = false;
			public bool bShowPhysicalAddresses = false;
			public bool bShowPageFaults = false;
			public bool bEnablePhysicalMemorySimulation = false;
			public bool bFillUninitalizeMemoryWithGarbage = false;
			public bool bShowMemoryAsHex = true;

			
			// Console
			public Color ConsoleTextColor = Color.FromArgb(0,200,0);
			public Color ConsoleBackGroundColor = Color.Black;
			public bool bConsoleAlwaysOnTopOnDebug = false;

			// Clock
			public int ClockResolution = 10;

			// General
			public bool bShowRegistersInHex = true;
			public bool bShowSpecialRegisters = false;
			public bool bShowDebugInformation = false;

			// Stack
			public int iSP = 0xFFFF00;
			public int iStackSize = 0x10000;
		}

		/// <summary>
		/// Updates the global settings from this class settings,
		/// or update the class settings from the global settings
		/// </summary>
		/// <param name="bToGlobalSettings">true if we wants to update the global settings.
		/// false to update the object with the global settings</param>
		public void UpdateGlobalSettings(bool bToGlobalSettings)
		{
			if (bToGlobalSettings)
			{
				Settings.Environment.TEXT_COLOR = Environment.TEXT_COLOR;
				Settings.Environment.COMMENT_COLOR = Environment.COMMENT_COLOR;
				Settings.Environment.LABEL_COLOR = Environment.LABEL_COLOR;
				Settings.Environment.FUNCTION_COLOR = Environment.FUNCTION_COLOR;
				Settings.Environment.DIRECTIVE_COLOR = Environment.DIRECTIVE_COLOR;
				Settings.Environment.COMMAND_COLOR = Environment.COMMAND_COLOR;
				Settings.Environment.STRING_COLOR = Environment.STRING_COLOR;
				Settings.Environment.BACKGROUND_COLOR = Environment.BACKGROUND_COLOR;
				Settings.Environment.DEBUG_LINE_COLOR = Environment.DEBUG_LINE_COLOR;
				Settings.Environment.BREAKPOINT_COLOR = Environment.BREAKPOINT_COLOR;
				Settings.Environment.ERRORS_COLOR	  = Environment.ERRORS_COLOR;

				Settings.Environment.OpenLSTFileAfterCompile = Environment.OpenLSTFileAfterCompile;
				Settings.Environment.bDoSyntaxHighlight = Environment.bDoSyntaxHighlight;
				Settings.Environment.bShowAgentOnStartup = Environment.bShowAgentOnStartup;
				Settings.Environment.bLoadLastFileOnStartup = Environment.bLoadLastFileOnStartup;
				Settings.Environment.sLastFileLocation = Environment.sLastFileLocation;
				Settings.Environment.a_sLastFiles = Environment.a_sLastFiles;

				Settings.Environment.EditModeLayout = Environment.EditModeLayout;

				Settings.Assembler.bOptimaizeCode = Assembler.bOptimaizeCode;
				Settings.Assembler.bSaveLSTFileAfterCompile = Assembler.bSaveLSTFileAfterCompile;

				Settings.Simulator.PageSize = Simulator.PageSize;
				Settings.Simulator.MemorySize = Simulator.MemorySize;
				Settings.Simulator.bShowAccessesToMemory = Simulator.bShowAccessesToMemory;
				Settings.Simulator.bShowPhysicalAddresses = Simulator.bShowPhysicalAddresses;
				Settings.Simulator.bShowPageFaults = Simulator.bShowPageFaults;
				Settings.Simulator.bEnablePhysicalMemorySimulation = Simulator.bEnablePhysicalMemorySimulation;
				Settings.Simulator.bFillUninitalizeMemoryWithGarbage = Simulator.bFillUninitalizeMemoryWithGarbage;

				Settings.Simulator.ConsoleTextColor = Simulator.ConsoleTextColor;
				Settings.Simulator.ConsoleBackGroundColor = Simulator.ConsoleBackGroundColor;
				Settings.Simulator.bConsoleAlwaysOnTopOnDebug = Simulator.bConsoleAlwaysOnTopOnDebug;

				Settings.Simulator.ClockResolution = Simulator.ClockResolution;

				Settings.Simulator.bShowRegistersInHex = Simulator.bShowRegistersInHex;
				Settings.Simulator.bShowSpecialRegisters = Simulator.bShowSpecialRegisters;
				Settings.Simulator.bShowDebugInformation = Simulator.bShowDebugInformation;

				Settings.Simulator.iSP = Simulator.iSP;
				Settings.Simulator.iStackSize = Simulator.iStackSize;
			}
			else
			{
				Environment.TEXT_COLOR = Settings.Environment.TEXT_COLOR;
				Environment.COMMENT_COLOR = Settings.Environment.COMMENT_COLOR;
				Environment.LABEL_COLOR = Settings.Environment.LABEL_COLOR;
				Environment.FUNCTION_COLOR = Settings.Environment.FUNCTION_COLOR;
				Environment.DIRECTIVE_COLOR = Settings.Environment.DIRECTIVE_COLOR;
				Environment.COMMAND_COLOR = Settings.Environment.COMMAND_COLOR;
				Environment.STRING_COLOR = Settings.Environment.STRING_COLOR;
				Environment.BACKGROUND_COLOR = Settings.Environment.BACKGROUND_COLOR;
				Environment.DEBUG_LINE_COLOR = Settings.Environment.DEBUG_LINE_COLOR;
				Environment.BREAKPOINT_COLOR = Settings.Environment.BREAKPOINT_COLOR;
				Environment.ERRORS_COLOR	= Settings.Environment.ERRORS_COLOR;

				Environment.OpenLSTFileAfterCompile = Settings.Environment.OpenLSTFileAfterCompile;
				Environment.bDoSyntaxHighlight = Settings.Environment.bDoSyntaxHighlight;
				Environment.bShowAgentOnStartup = Settings.Environment.bShowAgentOnStartup;
				Environment.bLoadLastFileOnStartup = Settings.Environment.bLoadLastFileOnStartup;
				Environment.sLastFileLocation = Settings.Environment.sLastFileLocation;

				Environment.a_sLastFiles = Settings.Environment.a_sLastFiles;

				Environment.EditModeLayout = Settings.Environment.EditModeLayout;

				Assembler.bOptimaizeCode = Settings.Assembler.bOptimaizeCode;
				Assembler.bSaveLSTFileAfterCompile = Settings.Assembler.bSaveLSTFileAfterCompile;

				Simulator.PageSize = Settings.Simulator.PageSize;
				Simulator.MemorySize = Settings.Simulator.MemorySize;
				Simulator.bShowAccessesToMemory = Settings.Simulator.bShowAccessesToMemory;
				Simulator.bShowPhysicalAddresses = Settings.Simulator.bShowPhysicalAddresses;
				Simulator.bShowPageFaults = Settings.Simulator.bShowPageFaults;
				Simulator.bEnablePhysicalMemorySimulation = Settings.Simulator.bEnablePhysicalMemorySimulation;
				Simulator.bFillUninitalizeMemoryWithGarbage = Settings.Simulator.bFillUninitalizeMemoryWithGarbage;

				Simulator.ConsoleTextColor = Settings.Simulator.ConsoleTextColor;
				Simulator.ConsoleBackGroundColor = Settings.Simulator.ConsoleBackGroundColor;
				Simulator.bConsoleAlwaysOnTopOnDebug = Settings.Simulator.bConsoleAlwaysOnTopOnDebug;

				Simulator.ClockResolution = Settings.Simulator.ClockResolution;

				Simulator.bShowRegistersInHex = Settings.Simulator.bShowRegistersInHex;
				Simulator.bShowSpecialRegisters = Settings.Simulator.bShowSpecialRegisters;
				Simulator.bShowDebugInformation = Settings.Simulator.bShowDebugInformation;

				Simulator.iSP = Settings.Simulator.iSP;
				Simulator.iStackSize = Settings.Simulator.iStackSize;
			}
		}
	}
}
