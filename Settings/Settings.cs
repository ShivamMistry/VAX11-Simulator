using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Soap;
using System.IO;

namespace VAX11Settings
{
	/// <summary>
	/// Summary description for Settings.
	/// </summary>
	public class Settings
	{
		#region members

		private static bool _bSettingSaved = true;

		#endregion

		#region Properties

		public static bool bSettingSaved
		{
			get { return _bSettingSaved; }
			set { _bSettingSaved = value; }
		}

		#endregion

		public class Environment
		{
			public const string FILE_FILTERS = @"Assembly files (*.asm)|*.asm|Text files (*.txt)|*.txt|All files (*.*)|*.*";
			public const string DAT_FILE_FILTERS = @"Text files (*.txt)|*.txt|All files (*.*)|*.*";
			public const string MESSAGEBOXS_TITLE = "VAX11 Simulator";
			public const string PRINTING_LOGO = "VAX11 Simulator by Nir Adar and Rotem Grosman";
			public const string SIM_VERSION = "1.03";
			public const string EditorFontName = "Courier New";
			public const float EditorFontSize = 9.75F;

			//colors settings
			public static Color TEXT_COLOR			= Color.Black;
			public static Color COMMENT_COLOR		= Color.Green;
			public static Color LABEL_COLOR			= Color.Brown;
			public static Color FUNCTION_COLOR		= Color.Red;
			public static Color DIRECTIVE_COLOR		= Color.Blue;
			public static Color COMMAND_COLOR		= Color.Blue;
			public static Color STRING_COLOR		= Color.Gray;
			public static Color BACKGROUND_COLOR	= Color.White;
			public static Color DEBUG_LINE_COLOR	= Color.Yellow;
			public static Color BREAKPOINT_COLOR	= Color.LawnGreen;
			public static Color ERRORS_COLOR		= Color.LightSteelBlue;

			// General Settings
			public static bool OpenLSTFileAfterCompile = false;
			public static bool bDoSyntaxHighlight = true;
			public static bool bShowAgentOnStartup = true;
			public static bool bLoadLastFileOnStartup = false;
			public static string sLastFileLocation = "";
			public static string[] a_sLastFiles = { "", "", "", "", "" };

			// Layout Settings
			public static string EditModeLayout = "";
			public static string DebugModeLayout = "";
			public static string SpecialLayout = "";
		}

		public class Assembler
		{
			public const int BytesInLine = 8; // How many bytes will be display in each line in the lst file
			public static bool bOptimaizeCode = true;
			public static bool bSaveLSTFileAfterCompile = false;
		}

		public class Simulator
		{
			// Memory
			public static int PageSize = 512; // In bytes
			public static int MemorySize = 65536;
			public static bool bShowAccessesToMemory = false;
			public static bool bShowPhysicalAddresses = false;
			public static bool bShowPageFaults = false;
			public static bool bEnablePhysicalMemorySimulation = false;
			public static bool bFillUninitalizeMemoryWithGarbage = false;
			public static bool bShowMemoryAsHex = true;

			// Console
			public static Color ConsoleTextColor = Color.FromArgb(0,200,0);
			public static Color ConsoleBackGroundColor = Color.Black;
			public static bool bConsoleAlwaysOnTopOnDebug = false;

			// Clock
			public static int ClockResolution = 10;

			// General
			public static bool bShowRegistersInHex = true;
			public static bool bShowSpecialRegisters = false;
			public static bool bShowDebugInformation = false;

			// Stack
			public static int iSP = 0xFFFF00;
			public static int iStackSize = 0x10000;


		}


		/// <summary>
		/// Load all program's settings
		/// </summary>
		public static void LoadSettings()
		{
			FileStream fXMLSettingsFile;
			SettingsCover s = new SettingsCover();
			SoapFormatter sf = new SoapFormatter();
			if (!File.Exists(Application.StartupPath + @"\Settings.xml"))
			{
				fXMLSettingsFile = new FileStream(Application.StartupPath + @"\Settings.xml", FileMode.Create, FileAccess.Write);
				
				sf.Serialize(fXMLSettingsFile, s);
				fXMLSettingsFile.Close();
			}

			fXMLSettingsFile = new FileStream(Application.StartupPath + @"\Settings.xml", FileMode.Open, FileAccess.Read);
			s = (SettingsCover)sf.Deserialize(fXMLSettingsFile);

			s.UpdateGlobalSettings(true);
		}

		/// <summary>
		/// Saves program's settings to XML file
		/// </summary>
		public static void SaveSettings()
		{
			try
			{
			
				FileStream fXMLSettingsFile;
				SettingsCover s = new SettingsCover();
				s.UpdateGlobalSettings(false);

				fXMLSettingsFile = new FileStream(Application.StartupPath + @"\Settings.xml", FileMode.Create, FileAccess.Write);


				SoapFormatter sf = new SoapFormatter();
				sf.Serialize(fXMLSettingsFile, s);
				fXMLSettingsFile.Close();
				_bSettingSaved = true;
			}
			catch 
			{
				_bSettingSaved = false;
			}
		}
	}
}
