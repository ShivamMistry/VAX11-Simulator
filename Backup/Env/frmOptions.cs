using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using PropertyBagLib;
using VAX11Settings;
using VAX11Internals;

namespace VAX11Environment
{
	/// <summary>
	/// Summary description for frmOptions.
	/// </summary>
	public class frmOptions : System.Windows.Forms.Form
	{

		#region Members

		private SettingsCover theSettings;

		private PropertyBag bagEnvironment;
		private PropertyBag bagAssembler;
		private PropertyBag bagSimulator;

		private bool _bColorsSettingsChanged;
		private InterfaceColorSchemes curScheme;

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.PropertyGrid gridOptions;
		private System.Windows.Forms.Button btnDefaults;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		
		#endregion

		#region Properties

		
		/// <summary>
		/// Tells if the color settings has changed during the last time we opened the option window
		/// </summary>
		public bool ColorsSettingsChanged
		{
			get { return _bColorsSettingsChanged; }
		}


		#endregion


		#region Constructor, Destructor

		public frmOptions()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();


			_bColorsSettingsChanged = false;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.gridOptions = new System.Windows.Forms.PropertyGrid();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.btnDefaults = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// gridOptions
			// 
			this.gridOptions.CommandsVisibleIfAvailable = true;
			this.gridOptions.LargeButtons = false;
			this.gridOptions.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.gridOptions.Location = new System.Drawing.Point(184, 8);
			this.gridOptions.Name = "gridOptions";
			this.gridOptions.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.gridOptions.Size = new System.Drawing.Size(304, 328);
			this.gridOptions.TabIndex = 4;
			this.gridOptions.Text = "PropertyGrid";
			this.gridOptions.ViewBackColor = System.Drawing.SystemColors.Window;
			this.gridOptions.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// btnOK
			// 
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(328, 352);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "&OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(416, 352);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// treeView1
			// 
			this.treeView1.ImageIndex = -1;
			this.treeView1.Location = new System.Drawing.Point(8, 8);
			this.treeView1.Name = "treeView1";
			this.treeView1.SelectedImageIndex = -1;
			this.treeView1.Size = new System.Drawing.Size(168, 328);
			this.treeView1.TabIndex = 7;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			// 
			// btnDefaults
			// 
			this.btnDefaults.Location = new System.Drawing.Point(8, 352);
			this.btnDefaults.Name = "btnDefaults";
			this.btnDefaults.TabIndex = 8;
			this.btnDefaults.Text = "&Defaults";
			this.btnDefaults.Click += new System.EventHandler(this.btnDefaults_Click);
			// 
			// frmOptions
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(498, 384);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnDefaults,
																		  this.treeView1,
																		  this.btnCancel,
																		  this.btnOK,
																		  this.gridOptions});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmOptions";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Options";
			this.Load += new System.EventHandler(this.frmOptions_Load);
			this.ResumeLayout(false);

		}
		#endregion

		#region Enums

		enum InterfaceColorSchemes
		{
			Current, Microsoft_Sytle, Borland_Classic
		}

		#endregion




		private void frmOptions_Load(object sender, System.EventArgs e)
		{
			// Add items to the TreeView
			treeView1.Nodes.Add(new TreeNode("Environment"));
			treeView1.Nodes.Add(new TreeNode("Assembler"));
			treeView1.Nodes.Add(new TreeNode("Simulator"));

			// Load Settings
			theSettings = new SettingsCover();
			theSettings.UpdateGlobalSettings(false);

			// Environment Settings
			bagEnvironment = new PropertyBag();
			bagEnvironment.GetValue += new PropertySpecEventHandler(this.bagEnvironment_GetValue);
			bagEnvironment.SetValue += new PropertySpecEventHandler(this.bagEnvironment_SetValue);

			bagEnvironment.Properties.Add(new PropertySpec("Text Color", typeof(Color), 
				"Colors", "Normal text color."));
			bagEnvironment.Properties.Add(new PropertySpec("Comments Color", typeof(Color), 
				"Colors", "Comments start after # sign."));
			bagEnvironment.Properties.Add(new PropertySpec("Labels Color", typeof(Color), 
				"Colors", "Labels are identifiers following by : that appears on start of lines."));
			bagEnvironment.Properties.Add(new PropertySpec("OS Functions Color", typeof(Color), 
				"Colors", "The simulator supports some high-level functions, as printf, getchar, etc.\nThese functions are known as operation system functions."));
			bagEnvironment.Properties.Add(new PropertySpec("Directives Color", typeof(Color), 
				"Colors", "Directives are commands meant for the assembler, that doesn't appear on the final machine code. Examples: .word, .space"));
			bagEnvironment.Properties.Add(new PropertySpec("Commands Color", typeof(Color), 
				"Colors", "VAX11 commands (opcodes) color."));
			bagEnvironment.Properties.Add(new PropertySpec("Strings Color", typeof(Color), 
				"Colors", "Strings are text appear betweens \"\"."));
			bagEnvironment.Properties.Add(new PropertySpec("Background Color", typeof(Color), 
				"Colors", "Documents Background Color."));
			bagEnvironment.Properties.Add(new PropertySpec("Current Line Color", typeof(Color), 
				"Colors", "Color of the next line that will be executed (debug mode)"));
			bagEnvironment.Properties.Add(new PropertySpec("Break Point Color", typeof(Color), 
				"Colors", "Break Point Color"));
			bagEnvironment.Properties.Add(new PropertySpec("Errors Color", typeof(Color), 
				"Colors", "The background color of the lines that the assembler finds errors at"));

			curScheme = InterfaceColorSchemes.Current;
			bagEnvironment.Properties.Add(new PropertySpec("Color Scheme", typeof(InterfaceColorSchemes), 
				"Colors", "Color scheme is predefined sets of colors for the simulator"));

			bagEnvironment.Properties.Add(new PropertySpec("Do Syntax Highlight", typeof(bool), 
				"General", "Select if the environment should highlight special VAX11 words."));
			bagEnvironment.Properties.Add(new PropertySpec("Show LST file after compile", typeof(bool), 
				"General", "If set, the LST file created during the compilation will be displayed after compile ends successfully."));
			bagEnvironment.Properties.Add(new PropertySpec("Show Agent on Startup", typeof(bool), 
				"General", "The agent, Merlin, is welcome the users of VAX11 Simulator every time the program runs."));

			// Canceled due to .Net bug.
//			bagEnvironment.Properties.Add(new PropertySpec("Load last opened file on startup", typeof(bool), 
//				"General", "If selected, every time the program runs, it will try to open the last file the user worked on."));

			// Assembler
			bagAssembler = new PropertyBag();
			bagAssembler.GetValue += new PropertySpecEventHandler(this.bagAssembler_GetValue);
			bagAssembler.SetValue += new PropertySpecEventHandler(this.bagAssembler_SetValue);
			bagAssembler.Properties.Add(new PropertySpec("Optimize Code", typeof(bool), 
				"General", "VAX11 Simulator generates smaller code than the old SIM simulator used by the Technion. Set this option to false if you wish the assembler to generate code as SIM does, without its enhancements."));
			bagAssembler.Properties.Add(new PropertySpec("Save LST file after compile", typeof(bool), 
				"General", "Select if the assembler should save LST file after successful compilation. LST file is text file containing the machine code and the source code of the compiled program."));

			// Simulator
			bagSimulator = new PropertyBag();
			bagSimulator.GetValue += new PropertySpecEventHandler(this.bagSimulator_GetValue);
			bagSimulator.SetValue += new PropertySpecEventHandler(this.bagSimulator_SetValue);

			bagSimulator.Properties.Add(new PropertySpec("Clock Resolution", typeof(int), 
				"Clock", "Time in milliseconds between clock ticks."));

			bagSimulator.Properties.Add(new PropertySpec("Text Color", typeof(Color), 
				"Console", "The color of the console's output."));
			bagSimulator.Properties.Add(new PropertySpec("Background Color", typeof(Color), 
				"Console", "Background color for the console."));
			bagSimulator.Properties.Add(new PropertySpec("Always On Top on Debug Mode", typeof(bool), 
				"Console", "If selected, the console window of debugged application will be above all other windows, even when deactived."));

			bagSimulator.Properties.Add(new PropertySpec("Show Registers in Hex", typeof(bool), 
				"General", "If true, while debugging, the registers values will be displayed in Hex basis. Else it will be displayed as decimal numbers."));
			bagSimulator.Properties.Add(new PropertySpec("Show Special Registers", typeof(bool), 
				"General", "If true, while debugging, the special VAX11 registers will be displayed among the general registers."));
			bagSimulator.Properties.Add(new PropertySpec("Show Debug Information", typeof(bool),
				"General", "If true, detailed information about the machine state will be printed when run-time errors occurs."));

			bagSimulator.Properties.Add(new PropertySpec("SP Startup Location", typeof(int),
				"Stack", "Select the startup location for the simulator's Stack Pointer."));
			bagSimulator.Properties.Add(new PropertySpec("Stack Maximum Size", typeof(int),
				"Stack", "Select the maximum size for the simulator's Stack Pointer."));

			bagSimulator.Properties.Add(new PropertySpec("Page Size", typeof(int), 
				"Memory", "Memory Page Size"));
			bagSimulator.Properties.Add(new PropertySpec("Physical Memory Size", typeof(int), 
				"Memory", "Physical Memory Size. The memory size should be multiple of the page size."));
			bagSimulator.Properties.Add(new PropertySpec("Show Memory Accesses", typeof(bool), 
				"Memory", "If selected, the simulator will display information about accesses to memory after each command."));
			bagSimulator.Properties.Add(new PropertySpec("Enable Physical Memory Simulation", typeof(bool), 
				"Memory", "If selected, the physical memory simulation engine will be enabled. Note that it will reduce the simulator performance by around 30%"));
			bagSimulator.Properties.Add(new PropertySpec("Show Physical Addresses", typeof(bool), 
				"Memory", "If selected, the simulator will show \"physical\" addressing for every virtual address."));
			bagSimulator.Properties.Add(new PropertySpec("Show Page Faults", typeof(bool), 
				"Memory", "If selected, the simulator will display message when page-fault occurs."));
			bagSimulator.Properties.Add(new PropertySpec("Fill Memory with Garbage", typeof(bool), 
				"Memory", "If true, uninitalize memory cells will contain garbage. If false, it will contain zeros"));

			// Select the environment settings
			treeView1.SelectedNode = treeView1.Nodes[0];

		}

		/// <summary>
		/// Interface for getting the simulator's settings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bagSimulator_GetValue(object sender, PropertySpecEventArgs e)
		{
			switch(e.Property.Name)
			{
				case "Page Size":
					e.Value = theSettings.Simulator.PageSize;
					break;
				case "Physical Memory Size":
					e.Value = theSettings.Simulator.MemorySize;
					break;
				case "Show Memory Accesses":
					e.Value = theSettings.Simulator.bShowAccessesToMemory;
					break;
				case "Enable Physical Memory Simulation":
					e.Value = theSettings.Simulator.bEnablePhysicalMemorySimulation;
					break;
				case "Show Physical Addresses":
					e.Value = theSettings.Simulator.bShowPhysicalAddresses;
					break;
				case "Show Page Faults":
					e.Value = theSettings.Simulator.bShowPageFaults;
					break;
				case "Fill Memory with Garbage":
					e.Value = theSettings.Simulator.bFillUninitalizeMemoryWithGarbage;
					break;
				case "Text Color":
					e.Value = theSettings.Simulator.ConsoleTextColor;
					break;
				case "Background Color":
					e.Value = theSettings.Simulator.ConsoleBackGroundColor;
					break;
				case "Always On Top on Debug Mode":
					e.Value = theSettings.Simulator.bConsoleAlwaysOnTopOnDebug;
					break;
				case "Clock Resolution":
					e.Value = theSettings.Simulator.ClockResolution;
					break;
				case "Show Registers in Hex":
					e.Value = theSettings.Simulator.bShowRegistersInHex;
					break;
				case "Show Special Registers":
					e.Value = theSettings.Simulator.bShowSpecialRegisters;
					break;
				case "Show Debug Information":
					e.Value = theSettings.Simulator.bShowDebugInformation;
					break;
				case "SP Startup Location":
					e.Value = theSettings.Simulator.iSP;
					break;
				case "Stack Maximum Size":
					e.Value = theSettings.Simulator.iStackSize;
					break;
				default:
					throw new PanicException();
			};
		}

		/// <summary>
		/// Interface for setting the simulator's settings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bagSimulator_SetValue(object sender, PropertySpecEventArgs e)
		{
			switch(e.Property.Name)
			{
				case "Page Size":
					theSettings.Simulator.PageSize = (int)e.Value < 8 ? 8 : (int)e.Value;
					break;
				case "Physical Memory Size":
					int iVal = (int)e.Value;
					if (theSettings.Simulator.PageSize > iVal) theSettings.Simulator.MemorySize = theSettings.Simulator.PageSize;
					else theSettings.Simulator.MemorySize = iVal % theSettings.Simulator.PageSize == 0 ? 
						iVal : iVal - iVal % theSettings.Simulator.PageSize + theSettings.Simulator.PageSize;
					break;
				case "Show Memory Accesses":
					theSettings.Simulator.bShowAccessesToMemory = (bool)e.Value;
					break;
				case "Enable Physical Memory Simulation":
					theSettings.Simulator.bEnablePhysicalMemorySimulation = (bool)e.Value;
					if ((bool)e.Value == false)
					{
						theSettings.Simulator.bShowPhysicalAddresses = false;
						theSettings.Simulator.bShowPageFaults = false;
						gridOptions.Refresh();
					}
					break;
				case "Show Physical Addresses":
					theSettings.Simulator.bShowPhysicalAddresses = (bool)e.Value;
					if ((bool)e.Value == true)
					{
						theSettings.Simulator.bEnablePhysicalMemorySimulation = true;
						gridOptions.Refresh();
					}
					break;
				case "Show Page Faults":
					theSettings.Simulator.bShowPageFaults = (bool)e.Value;
					if ((bool)e.Value == true)
					{
						theSettings.Simulator.bEnablePhysicalMemorySimulation = true;
						gridOptions.Refresh();
					}
					break;
				case "Fill Memory with Garbage":
					theSettings.Simulator.bFillUninitalizeMemoryWithGarbage = (bool)e.Value;
					break;
				case "Text Color":
					theSettings.Simulator.ConsoleTextColor = (Color)e.Value;
					break;
				case "Background Color":
					theSettings.Simulator.ConsoleBackGroundColor = (Color)e.Value;
					break;
				case "Always On Top on Debug Mode":
					theSettings.Simulator.bConsoleAlwaysOnTopOnDebug = (bool)e.Value;
					break;
				case "Clock Resolution":
					theSettings.Simulator.ClockResolution = (int)e.Value;
					break;
				case "Show Registers in Hex":
					theSettings.Simulator.bShowRegistersInHex = (bool)e.Value;
					break;
				case "Show Special Registers":
					theSettings.Simulator.bShowSpecialRegisters = (bool)e.Value;
					break;
				case "Show Debug Information":
					theSettings.Simulator.bShowDebugInformation = (bool)e.Value;
					break;
				case "SP Startup Location":
					theSettings.Simulator.iSP = (int)e.Value;
					break;
				case "Stack Maximum Size":
					theSettings.Simulator.iStackSize = (int)e.Value;
					break;
				default:
					throw new PanicException();
			};

		}


		/// <summary>
		/// Interface for getting the assembler settings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bagAssembler_GetValue(object sender, PropertySpecEventArgs e)
		{
			switch(e.Property.Name)
			{
				case "Optimize Code":
					e.Value = theSettings.Assembler.bOptimaizeCode;
					break;
				case "Save LST file after compile":
					e.Value = theSettings.Assembler.bSaveLSTFileAfterCompile;
					break;
				default:
					throw new PanicException();
			};
		}

		/// <summary>
		/// Interface for setting the assembler's settings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bagAssembler_SetValue(object sender, PropertySpecEventArgs e)
		{
			switch(e.Property.Name)
			{
				case "Optimize Code":
					theSettings.Assembler.bOptimaizeCode = (bool)e.Value;
					break;
				case "Save LST file after compile":
					theSettings.Assembler.bSaveLSTFileAfterCompile = (bool)e.Value;
					break;
				default:
					throw new PanicException();
			};

		}

		/// <summary>
		/// Interface for getting the environment settings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bagEnvironment_GetValue(object sender, PropertySpecEventArgs e)
		{
			switch(e.Property.Name)
			{
				case "Text Color":
					e.Value = theSettings.Environment.TEXT_COLOR;
					break;
				case "Comments Color":
					e.Value = theSettings.Environment.COMMENT_COLOR;
					break;
				case "Labels Color":
					e.Value = theSettings.Environment.LABEL_COLOR;
					break;
				case "OS Functions Color":
					e.Value = theSettings.Environment.FUNCTION_COLOR;
					break;
				case "Directives Color":
					e.Value = theSettings.Environment.DIRECTIVE_COLOR;
					break;
				case "Commands Color":
					e.Value = theSettings.Environment.COMMAND_COLOR;
					break;
				case "Strings Color":
					e.Value = theSettings.Environment.STRING_COLOR;
					break;
				case "Background Color":
					e.Value = theSettings.Environment.BACKGROUND_COLOR;
					break;
				case "Current Line Color":
					e.Value = theSettings.Environment.DEBUG_LINE_COLOR;
                    break;
				case "Break Point Color":
					e.Value = theSettings.Environment.BREAKPOINT_COLOR;
					break;
				case "Errors Color":
					e.Value = theSettings.Environment.ERRORS_COLOR;
					break;
				case "Color Scheme":
					e.Value = curScheme;
					break;
				case "Do Syntax Highlight":
					e.Value = theSettings.Environment.bDoSyntaxHighlight;
					break;
				case "Show LST file after compile":
					e.Value = theSettings.Environment.OpenLSTFileAfterCompile;
					break;
				case "Show Agent on Startup":
					e.Value = theSettings.Environment.bShowAgentOnStartup;
					break;
				case "Load last opened file on startup":
					e.Value = theSettings.Environment.bLoadLastFileOnStartup;
					break;

				default:
					throw new PanicException();
			};

		}

		/// <summary>
		/// Interface for getting the environment settings
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void bagEnvironment_SetValue(object sender, PropertySpecEventArgs e)
		{
			switch(e.Property.Name)
			{
				case "Text Color":
					theSettings.Environment.TEXT_COLOR = (Color)e.Value;
					_bColorsSettingsChanged = true;
					break;
				case "Comments Color":
					theSettings.Environment.COMMENT_COLOR = (Color)e.Value;
					_bColorsSettingsChanged = true;
					break;
				case "Labels Color":
					theSettings.Environment.LABEL_COLOR = (Color)e.Value;
					_bColorsSettingsChanged = true;
					break;
				case "OS Functions Color":
					theSettings.Environment.FUNCTION_COLOR = (Color)e.Value;
					_bColorsSettingsChanged = true;
					break;
				case "Directives Color":
					theSettings.Environment.DIRECTIVE_COLOR = (Color)e.Value;
					_bColorsSettingsChanged = true;
					break;
				case "Commands Color":
					theSettings.Environment.COMMAND_COLOR = (Color)e.Value;
					_bColorsSettingsChanged = true;
					break;
				case "Strings Color":
					theSettings.Environment.STRING_COLOR = (Color)e.Value;
					_bColorsSettingsChanged = true;
					break;
				case "Background Color":
					theSettings.Environment.BACKGROUND_COLOR = (Color)e.Value;
					_bColorsSettingsChanged = true;
					break;
				case "Current Line Color":
					theSettings.Environment.DEBUG_LINE_COLOR = (Color)e.Value;
					_bColorsSettingsChanged = true;
					break;
				case "Break Point Color":
					theSettings.Environment.BREAKPOINT_COLOR = (Color)e.Value;
					_bColorsSettingsChanged = true;
					break;
				case "Errors Color":
					theSettings.Environment.ERRORS_COLOR = (Color)e.Value;
					_bColorsSettingsChanged = true;
					break;
				case "Color Scheme":
					if ((InterfaceColorSchemes)e.Value == InterfaceColorSchemes.Microsoft_Sytle)
					{
						theSettings.Environment.TEXT_COLOR = Color.Black;
						theSettings.Environment.COMMENT_COLOR = Color.Green;
						theSettings.Environment.LABEL_COLOR = Color.Brown;
						theSettings.Environment.FUNCTION_COLOR = Color.Red;
						theSettings.Environment.DIRECTIVE_COLOR = Color.Blue;
						theSettings.Environment.COMMAND_COLOR = Color.Blue;
						theSettings.Environment.STRING_COLOR = Color.Gray;
						theSettings.Environment.BACKGROUND_COLOR = Color.White;
						theSettings.Environment.DEBUG_LINE_COLOR = Color.Yellow;
						theSettings.Environment.BREAKPOINT_COLOR = Color.LawnGreen;
						theSettings.Environment.ERRORS_COLOR	 = Color.LightSteelBlue;
						_bColorsSettingsChanged = true;
					}
					else if ((InterfaceColorSchemes)e.Value == InterfaceColorSchemes.Borland_Classic)
					{
						theSettings.Environment.TEXT_COLOR = Color.Yellow;
						theSettings.Environment.COMMENT_COLOR = Color.Aqua;
						theSettings.Environment.LABEL_COLOR = Color.Yellow;
						theSettings.Environment.FUNCTION_COLOR = Color.Yellow;
						theSettings.Environment.DIRECTIVE_COLOR = Color.White;
						theSettings.Environment.COMMAND_COLOR = Color.White;
						theSettings.Environment.STRING_COLOR = Color.FromArgb(255, 77, 32);
						theSettings.Environment.BACKGROUND_COLOR = Color.DarkBlue;
						theSettings.Environment.DEBUG_LINE_COLOR = Color.YellowGreen;
						theSettings.Environment.BREAKPOINT_COLOR = Color.ForestGreen;
						theSettings.Environment.ERRORS_COLOR	 = Color.LightSteelBlue;
						_bColorsSettingsChanged = true;
					}
					else if ((InterfaceColorSchemes)e.Value == InterfaceColorSchemes.Current)
					{
						theSettings.Environment.TEXT_COLOR = Settings.Environment.TEXT_COLOR;
						theSettings.Environment.COMMENT_COLOR = Settings.Environment.COMMENT_COLOR;
						theSettings.Environment.LABEL_COLOR = Settings.Environment.LABEL_COLOR;
						theSettings.Environment.FUNCTION_COLOR = Settings.Environment.FUNCTION_COLOR;
						theSettings.Environment.DIRECTIVE_COLOR = Settings.Environment.DIRECTIVE_COLOR;
						theSettings.Environment.COMMAND_COLOR = Settings.Environment.COMMAND_COLOR;
						theSettings.Environment.STRING_COLOR = Settings.Environment.STRING_COLOR;
						theSettings.Environment.BACKGROUND_COLOR = Settings.Environment.BACKGROUND_COLOR;
						theSettings.Environment.BREAKPOINT_COLOR = Settings.Environment.BREAKPOINT_COLOR;
						theSettings.Environment.ERRORS_COLOR	= Settings.Environment.ERRORS_COLOR;
						_bColorsSettingsChanged = false;
					}

					curScheme = (InterfaceColorSchemes)e.Value;
					gridOptions.Refresh();

					break;
				case "Do Syntax Highlight":
					theSettings.Environment.bDoSyntaxHighlight = (bool)e.Value;
					_bColorsSettingsChanged = true;
					break;
				case "Show LST file after compile":
					theSettings.Environment.OpenLSTFileAfterCompile = (bool)e.Value;
					break;
				case "Show Agent on Startup":
					theSettings.Environment.bShowAgentOnStartup = (bool)e.Value;
					break;
				case "Load last opened file on startup":
					theSettings.Environment.bLoadLastFileOnStartup = (bool)e.Value;
					break;

				default:
					throw new PanicException();
			};
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			theSettings.UpdateGlobalSettings(true);
			Settings.SaveSettings();
			this.Close();
		}

		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			ArrayList objs = new ArrayList();
			
			switch (e.Node.Text)
			{
				case "Environment":
					objs.Add(bagEnvironment);
					break;
				case "Assembler":
					objs.Add(bagAssembler);
					break;
				case "Simulator":
					objs.Add(bagSimulator);
					break;
			};

			gridOptions.SelectedObjects = objs.ToArray();
		}

		private void btnDefaults_Click(object sender, System.EventArgs e)
		{
			SettingsCover s = new SettingsCover();
			theSettings = s;
			gridOptions.Refresh();
			_bColorsSettingsChanged = true;
		}

	}
}
