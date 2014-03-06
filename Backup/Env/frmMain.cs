
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Text;
using Microsoft.Win32;

using VAX11Compiler;
using VAX11Internals;
using VAX11Simulator;
using VAX11Settings;


namespace VAX11Environment
{
	/// <summary>
	/// Vax11 Simulator Main Class.
	/// </summary>
	public class frmMain : System.Windows.Forms.Form
	{

		#region Form Controls

		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.StatusBarPanel sbpMessage;
		private System.Windows.Forms.StatusBarPanel sbpCurrentLocation;
		private System.Windows.Forms.ImageList imgListWindows;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.ImageList imgListtoolBar;
		private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
		private System.Windows.Forms.PrintDialog printDialog1;
		private DockingSuite.DockControl dockControlOutput;
		private DockingSuite.DockControl dockControlTaskList;
		private DockingSuite.DockControl dockControlRegisters;
		private DockingSuite.DockHost dockHostUp;
		private DockingSuite.DockHost dockHostLeft;
		private DockingSuite.DockHost dockHostDown;
		private DockingSuite.DockHost dockHostRegisters;
		private DockingSuite.DockPanel dockPanelRegisters;
		private DockingSuite.DockPanel dockPanel1;
		private DockingSuite.DockControl dockControlCalc;
		private DocumentManager.DocumentManager documentsManager;
		private System.Drawing.Printing.PrintDocument printDocument;

		private DotNetWidgets.DotNetToolbar dotNetToolbar1;


		#endregion

		#region Members

		// Saves the current line that the corsure stays in.
		private int iCurrentLine = 1;

		// For new documents numbering
		private static int docCount = 0;

		// Saves the running program
		private System.Collections.ArrayList ProgramPool;


		/// <summary>
		/// Saves the editor window of the debugged window
		/// </summary>
		frmEditor debugEditorWindow = null;


		Thread debugThread = null;

		/// <summary>
		/// Saves the program that running in debug mode, in there is no program like that then it null
		/// </summary>
		private Program debugProgram = null;

		/// <summary>
		/// Current Running Mode - for the menus
		/// </summary>
		enmEnvironmentMode curRunningMode = enmEnvironmentMode.EditorMode;

		// Refering to docking windows using the following variables
		private frmCompileMessages			cntCompileMessage = null;
		private frmTaskList					cntTaskList = null;
		private frmRegistersView 			cntRegistersView = null;
		private VAX11Environment.BaseCalc 	cntBasicCalc = null;
		private frmMemoryView				cntMemoryView = null;
		private frmStackView				cntStackView = null;

		private AgentObjects.IAgentCtlCharacter speaker;
		private DockingSuite.DockPanel dockPanelStack;
		private DockingSuite.DockPanel dockPanelMemory;
		private DockingSuite.DockControl dockControlStack;
		private DockingSuite.DockControl dockControlMemory;
		private DockingSuite.DockPanel dockPanelOutputTaskList;
		private AxAgentObjects.AxAgent agent;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private DotNetWidgets.DotNetMenuProvider dotNetMenuProvider1;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem mnuFile;
		private System.Windows.Forms.MenuItem mnuFileNew;
		private System.Windows.Forms.MenuItem mnuFileOpen;
		private System.Windows.Forms.MenuItem mnuFileClose;
		private System.Windows.Forms.MenuItem mnuCloseAll;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem mnuFileSave;
		private System.Windows.Forms.MenuItem mnuFileSaveAs;
		private System.Windows.Forms.MenuItem mnuFileSaveAll;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem mnuFilePrintPrintCode;
		private System.Windows.Forms.MenuItem mnuFilePrintPreview;
		private System.Windows.Forms.MenuItem mnuFilePrintLSTFile;
		private System.Windows.Forms.MenuItem mnuFileLastFile1;
		private System.Windows.Forms.MenuItem mnuFileLastFile2;
		private System.Windows.Forms.MenuItem mnuFileLastFile3;
		private System.Windows.Forms.MenuItem mnuFileLastFile4;
		private System.Windows.Forms.MenuItem mnuLastFilesSeperator;
		private System.Windows.Forms.MenuItem mnuFileExit;
		private System.Windows.Forms.MenuItem mnuEdit;
		private System.Windows.Forms.MenuItem mnuEditUndo;
		private System.Windows.Forms.MenuItem mnuEditRedo;
		private System.Windows.Forms.MenuItem menuItem42;
		private System.Windows.Forms.MenuItem mnuEditCut;
		private System.Windows.Forms.MenuItem mnuEditCopy;
		private System.Windows.Forms.MenuItem mnuEditPaste;
		private System.Windows.Forms.MenuItem mnuEditDelete;
		private System.Windows.Forms.MenuItem menuItem37;
		private System.Windows.Forms.MenuItem mnuEditSelectAll;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem mnuEditFind;
		private System.Windows.Forms.MenuItem mnuEditFindNext;
		private System.Windows.Forms.MenuItem mnuEditReplace;
		private System.Windows.Forms.MenuItem mnuGoTo;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem mnuInsertGeneral;
		private System.Windows.Forms.MenuItem mnuInsertBasicApplication;
		private System.Windows.Forms.MenuItem mnuInsertProgramEnd;
		private System.Windows.Forms.MenuItem mnuInsertFunctionDocumentation;
		private System.Windows.Forms.MenuItem mnuInsertLoop;
		private System.Windows.Forms.MenuItem mnuInsertFor;
		private System.Windows.Forms.MenuItem mnuInsertWhile;
		private System.Windows.Forms.MenuItem mnuInsertDo;
		private System.Windows.Forms.MenuItem mnuInsertOutput;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem mnuBuild;
		private System.Windows.Forms.MenuItem mnuBuildCompile;
		private System.Windows.Forms.MenuItem mnuBuildViewLSTFile;
		private System.Windows.Forms.MenuItem mnuBuildInputOutputFiles;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem mnuBuildRun;
		private System.Windows.Forms.MenuItem mnuDebug;
		private System.Windows.Forms.MenuItem mnuDebugRun;
		private System.Windows.Forms.MenuItem menuItem19;
		private System.Windows.Forms.MenuItem mnuDebugStepInto;
		private System.Windows.Forms.MenuItem mnuDebugRestartProgram;
		private System.Windows.Forms.MenuItem mnuDebugRunToCursor;
		private System.Windows.Forms.MenuItem menuItem25;
		private System.Windows.Forms.MenuItem mnuDebugWatch;
		private System.Windows.Forms.MenuItem mnuDebugAddWatch;
		private System.Windows.Forms.MenuItem mnuDebugBreakPoint;
		private System.Windows.Forms.MenuItem mnuDebugRemoveAllBreakPoints;
		private System.Windows.Forms.MenuItem mnuView;
		private System.Windows.Forms.MenuItem mnuViewRegisters;
		private System.Windows.Forms.MenuItem mnuViewStack;
		private System.Windows.Forms.MenuItem mnuViewMemory;
		private System.Windows.Forms.MenuItem mnuViewWatchs;
		private System.Windows.Forms.MenuItem mnuViewTaskList;
		private System.Windows.Forms.MenuItem mnuViewOutput;
		private System.Windows.Forms.MenuItem mnuTools;
		private System.Windows.Forms.MenuItem mnuToolsHex;
		private System.Windows.Forms.MenuItem mnuToolsOptions;
		private System.Windows.Forms.MenuItem mnuHelp;
		private System.Windows.Forms.MenuItem mnuHelpContents;
		private System.Windows.Forms.MenuItem mnuHelpGetStarted;
		private System.Windows.Forms.MenuItem mnuHelpGetHelp;
		private System.Windows.Forms.MenuItem menuItem28;
		private System.Windows.Forms.MenuItem mnuHelpAbout;
		private System.Windows.Forms.ContextMenu mnuDocumentsManager;
		private System.Windows.Forms.MenuItem mnuDocumentManagerSave;
		private System.Windows.Forms.MenuItem mnuDocumentManagerClose;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonNew;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonOpen;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonSave;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonSaveAll;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonPaste;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonPrint;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonFind;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonCut;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonCopy;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonCompile;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonExecute;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonRunDebug;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonRestartDebug;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonStep;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonBreakPoint;
		private DotNetWidgets.DotNetToolbarButtonItem tbButtonInOut;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem mnuDebugDelAllWatchs;

		private frmFind frmfind;
		private frmAbout frmabout;

		public Program DebugProgram { get { return debugProgram; } }

		#endregion

		#region Enums

		enum enmEnvironmentMode
		{
			EditorMode,
			DebugMode,
		};

		#endregion

		#region Constructor
        
		/// <summary>
		/// Init Docking Windows, Listen to events
		/// </summary>
		public frmMain(string[] args)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();


			// Create new Controls objects
			cntTaskList = new frmTaskList();
			cntCompileMessage = new frmCompileMessages();
			cntRegistersView = new frmRegistersView();
			cntBasicCalc = new BaseCalc();
			cntMemoryView = new frmMemoryView();
			cntStackView = new frmStackView();

			//creating forms
			frmfind = new frmFind();
			frmabout = new frmAbout();
			
			// Prepare program pool
			ProgramPool				= new System.Collections.ArrayList();

			//
			// Listen to events
			//
			cntTaskList.OnClickEvent += new frmTaskList.clickEventFunc(OnTaskListClick);
			frmfind.bFindNext.Click += new System.EventHandler(bFindNextClick);

			
		}


		// Pay attention that constructor is called before this function
		private void frmMain_Load(object sender, System.EventArgs e)
		{
			this.Text += Settings.Environment.SIM_VERSION;
			
			CreateDefaultditLayout();

			ArrangeControls();
			
			// TODO: add back if you add SandBar
			// if (Settings.Environment.EditModeLayout != "") sandBarManager1.SetLayout(Settings.Environment.EditModeLayout);



			// Show last opened file
			ShowLastOpenedFiles();
			
			// Create Documents Tab
			CreateNewDocument();

			// Create Documents Tab
			if (Settings.Environment.bLoadLastFileOnStartup && Settings.Environment.sLastFileLocation != "")
				InterfaceDoOpen(Settings.Environment.sLastFileLocation);

			StartAgent();

			GiveFocusToTheActiveDocument();
			InterfacePrepareEnvironment(enmEnvironmentMode.EditorMode);
		}



		private void CreateDefaultditLayout()
		{

			
			dockControlOutput.Controls.AddRange(new System.Windows.Forms.Control[] { this.cntCompileMessage});
			dockControlTaskList.Controls.AddRange(new System.Windows.Forms.Control[] { this.cntTaskList});
			dockControlRegisters.Controls.AddRange(new System.Windows.Forms.Control[] { this.cntRegistersView});
			dockControlMemory.Controls.AddRange(new System.Windows.Forms.Control[] { this.cntMemoryView});
			dockControlCalc.Controls.AddRange(new System.Windows.Forms.Control[] { this.cntBasicCalc } );
			dockControlStack.Controls.AddRange(new System.Windows.Forms.Control[] { this.cntStackView } );
			
			cntCompileMessage.Dock = DockStyle.Fill;
			cntTaskList.Dock = DockStyle.Fill;
			cntRegistersView.Dock = DockStyle.Fill;
			cntMemoryView.Dock = DockStyle.Fill;
			cntStackView.Dock = DockStyle.Fill;

			dockControlRegisters.Close();
			dockControlCalc.Close();

			dockControlMemory.Close();
			dockControlStack.Close();
		}


		#endregion

		#region Destructor

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			// Close all programs
			for (int i = 0; i < ProgramPool.Count; i++)
				try
				{
					Thread tempThread = (Thread)ProgramPool[i];
					tempThread.Abort();
				}
				catch { }

			// Hi carets, you are not going to escape. MUHAHA
			for (int i = 0; i < VAX11Simulator.Console.AllThreads.Count; i++)
				try
				{
					Thread tempThread = (Thread)VAX11Simulator.Console.AllThreads[i];
					tempThread.Abort();
				}
				catch { }



			if( disposing )
			{
				if (components != null) 
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmMain));
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.sbpMessage = new System.Windows.Forms.StatusBarPanel();
			this.sbpCurrentLocation = new System.Windows.Forms.StatusBarPanel();
			this.imgListWindows = new System.Windows.Forms.ImageList(this.components);
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.printDialog1 = new System.Windows.Forms.PrintDialog();
			this.printDocument = new System.Drawing.Printing.PrintDocument();
			this.imgListtoolBar = new System.Windows.Forms.ImageList(this.components);
			this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
			this.agent = new AxAgentObjects.AxAgent();
			this.dockHostUp = new DockingSuite.DockHost();
			this.dockPanel1 = new DockingSuite.DockPanel();
			this.dockControlCalc = new DockingSuite.DockControl();
			this.dockHostLeft = new DockingSuite.DockHost();
			this.dockHostDown = new DockingSuite.DockHost();
			this.dockPanelOutputTaskList = new DockingSuite.DockPanel();
			this.dockControlOutput = new DockingSuite.DockControl();
			this.dockControlTaskList = new DockingSuite.DockControl();
			this.dockPanelStack = new DockingSuite.DockPanel();
			this.dockControlStack = new DockingSuite.DockControl();
			this.dockPanelMemory = new DockingSuite.DockPanel();
			this.dockControlMemory = new DockingSuite.DockControl();
			this.dockHostRegisters = new DockingSuite.DockHost();
			this.dockPanelRegisters = new DockingSuite.DockPanel();
			this.dockControlRegisters = new DockingSuite.DockControl();
			this.documentsManager = new DocumentManager.DocumentManager();
			this.mnuDocumentsManager = new System.Windows.Forms.ContextMenu();
			this.mnuDocumentManagerSave = new System.Windows.Forms.MenuItem();
			this.mnuDocumentManagerClose = new System.Windows.Forms.MenuItem();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.dotNetMenuProvider1 = new DotNetWidgets.DotNetMenuProvider();
			this.mnuFile = new System.Windows.Forms.MenuItem();
			this.mnuFileNew = new System.Windows.Forms.MenuItem();
			this.mnuFileOpen = new System.Windows.Forms.MenuItem();
			this.mnuFileClose = new System.Windows.Forms.MenuItem();
			this.mnuCloseAll = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.mnuFileSave = new System.Windows.Forms.MenuItem();
			this.mnuFileSaveAs = new System.Windows.Forms.MenuItem();
			this.mnuFileSaveAll = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.mnuFilePrintPrintCode = new System.Windows.Forms.MenuItem();
			this.mnuFilePrintPreview = new System.Windows.Forms.MenuItem();
			this.mnuFilePrintLSTFile = new System.Windows.Forms.MenuItem();
			this.menuItem11 = new System.Windows.Forms.MenuItem();
			this.mnuFileLastFile1 = new System.Windows.Forms.MenuItem();
			this.mnuFileLastFile2 = new System.Windows.Forms.MenuItem();
			this.mnuFileLastFile3 = new System.Windows.Forms.MenuItem();
			this.mnuFileLastFile4 = new System.Windows.Forms.MenuItem();
			this.mnuLastFilesSeperator = new System.Windows.Forms.MenuItem();
			this.mnuFileExit = new System.Windows.Forms.MenuItem();
			this.mnuEdit = new System.Windows.Forms.MenuItem();
			this.mnuEditUndo = new System.Windows.Forms.MenuItem();
			this.mnuEditRedo = new System.Windows.Forms.MenuItem();
			this.menuItem42 = new System.Windows.Forms.MenuItem();
			this.mnuEditCut = new System.Windows.Forms.MenuItem();
			this.mnuEditCopy = new System.Windows.Forms.MenuItem();
			this.mnuEditPaste = new System.Windows.Forms.MenuItem();
			this.mnuEditDelete = new System.Windows.Forms.MenuItem();
			this.menuItem37 = new System.Windows.Forms.MenuItem();
			this.mnuEditSelectAll = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.mnuEditFind = new System.Windows.Forms.MenuItem();
			this.mnuEditFindNext = new System.Windows.Forms.MenuItem();
			this.mnuEditReplace = new System.Windows.Forms.MenuItem();
			this.mnuGoTo = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.mnuInsertGeneral = new System.Windows.Forms.MenuItem();
			this.mnuInsertBasicApplication = new System.Windows.Forms.MenuItem();
			this.mnuInsertProgramEnd = new System.Windows.Forms.MenuItem();
			this.mnuInsertFunctionDocumentation = new System.Windows.Forms.MenuItem();
			this.mnuInsertLoop = new System.Windows.Forms.MenuItem();
			this.mnuInsertFor = new System.Windows.Forms.MenuItem();
			this.mnuInsertWhile = new System.Windows.Forms.MenuItem();
			this.mnuInsertDo = new System.Windows.Forms.MenuItem();
			this.mnuInsertOutput = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuItem13 = new System.Windows.Forms.MenuItem();
			this.mnuBuild = new System.Windows.Forms.MenuItem();
			this.mnuBuildCompile = new System.Windows.Forms.MenuItem();
			this.mnuBuildViewLSTFile = new System.Windows.Forms.MenuItem();
			this.mnuBuildInputOutputFiles = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.mnuBuildRun = new System.Windows.Forms.MenuItem();
			this.mnuDebug = new System.Windows.Forms.MenuItem();
			this.mnuDebugRun = new System.Windows.Forms.MenuItem();
			this.menuItem19 = new System.Windows.Forms.MenuItem();
			this.mnuDebugStepInto = new System.Windows.Forms.MenuItem();
			this.mnuDebugRestartProgram = new System.Windows.Forms.MenuItem();
			this.mnuDebugRunToCursor = new System.Windows.Forms.MenuItem();
			this.menuItem25 = new System.Windows.Forms.MenuItem();
			this.mnuDebugWatch = new System.Windows.Forms.MenuItem();
			this.mnuDebugAddWatch = new System.Windows.Forms.MenuItem();
			this.mnuDebugDelAllWatchs = new System.Windows.Forms.MenuItem();
			this.mnuDebugBreakPoint = new System.Windows.Forms.MenuItem();
			this.mnuDebugRemoveAllBreakPoints = new System.Windows.Forms.MenuItem();
			this.mnuView = new System.Windows.Forms.MenuItem();
			this.mnuViewRegisters = new System.Windows.Forms.MenuItem();
			this.mnuViewStack = new System.Windows.Forms.MenuItem();
			this.mnuViewMemory = new System.Windows.Forms.MenuItem();
			this.mnuViewWatchs = new System.Windows.Forms.MenuItem();
			this.mnuViewTaskList = new System.Windows.Forms.MenuItem();
			this.mnuViewOutput = new System.Windows.Forms.MenuItem();
			this.mnuTools = new System.Windows.Forms.MenuItem();
			this.mnuToolsHex = new System.Windows.Forms.MenuItem();
			this.mnuToolsOptions = new System.Windows.Forms.MenuItem();
			this.mnuHelp = new System.Windows.Forms.MenuItem();
			this.mnuHelpContents = new System.Windows.Forms.MenuItem();
			this.mnuHelpGetStarted = new System.Windows.Forms.MenuItem();
			this.mnuHelpGetHelp = new System.Windows.Forms.MenuItem();
			this.menuItem28 = new System.Windows.Forms.MenuItem();
			this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.dotNetToolbar1 = new DotNetWidgets.DotNetToolbar();
			this.tbButtonNew = new DotNetWidgets.DotNetToolbarButtonItem();
			this.tbButtonOpen = new DotNetWidgets.DotNetToolbarButtonItem();
			this.tbButtonSave = new DotNetWidgets.DotNetToolbarButtonItem();
			this.tbButtonSaveAll = new DotNetWidgets.DotNetToolbarButtonItem();
			this.tbButtonPrint = new DotNetWidgets.DotNetToolbarButtonItem();
			this.tbButtonFind = new DotNetWidgets.DotNetToolbarButtonItem();
			this.tbButtonCut = new DotNetWidgets.DotNetToolbarButtonItem();
			this.tbButtonCopy = new DotNetWidgets.DotNetToolbarButtonItem();
			this.tbButtonPaste = new DotNetWidgets.DotNetToolbarButtonItem();
			this.tbButtonCompile = new DotNetWidgets.DotNetToolbarButtonItem();
			this.tbButtonExecute = new DotNetWidgets.DotNetToolbarButtonItem();
			this.tbButtonRunDebug = new DotNetWidgets.DotNetToolbarButtonItem();
			this.tbButtonRestartDebug = new DotNetWidgets.DotNetToolbarButtonItem();
			this.tbButtonStep = new DotNetWidgets.DotNetToolbarButtonItem();
			this.tbButtonBreakPoint = new DotNetWidgets.DotNetToolbarButtonItem();
			this.tbButtonInOut = new DotNetWidgets.DotNetToolbarButtonItem();
			((System.ComponentModel.ISupportInitialize)(this.sbpMessage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpCurrentLocation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.agent)).BeginInit();
			this.dockHostUp.SuspendLayout();
			this.dockPanel1.SuspendLayout();
			this.dockHostDown.SuspendLayout();
			this.dockPanelOutputTaskList.SuspendLayout();
			this.dockPanelStack.SuspendLayout();
			this.dockPanelMemory.SuspendLayout();
			this.dockHostRegisters.SuspendLayout();
			this.dockPanelRegisters.SuspendLayout();
			this.SuspendLayout();
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 493);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						  this.sbpMessage,
																						  this.sbpCurrentLocation});
			this.statusBar1.ShowPanels = true;
			this.statusBar1.Size = new System.Drawing.Size(815, 22);
			this.statusBar1.TabIndex = 1;
			// 
			// sbpMessage
			// 
			this.sbpMessage.Text = " Ready";
			// 
			// sbpCurrentLocation
			// 
			this.sbpCurrentLocation.Text = " Line:Offset";
			// 
			// imgListWindows
			// 
			this.imgListWindows.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.imgListWindows.ImageSize = new System.Drawing.Size(16, 16);
			this.imgListWindows.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListWindows.ImageStream")));
			this.imgListWindows.TransparentColor = System.Drawing.Color.FromArgb(((System.Byte)(236)), ((System.Byte)(233)), ((System.Byte)(216)));
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.FileName = "doc1";
			// 
			// printDialog1
			// 
			this.printDialog1.Document = this.printDocument;
			// 
			// printDocument
			// 
			this.printDocument.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.printDocument_BeginPrint);
			this.printDocument.EndPrint += new System.Drawing.Printing.PrintEventHandler(this.printDocument_EndPrint);
			this.printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument_PrintPage);
			// 
			// imgListtoolBar
			// 
			this.imgListtoolBar.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
			this.imgListtoolBar.ImageSize = new System.Drawing.Size(16, 16);
			this.imgListtoolBar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListtoolBar.ImageStream")));
			this.imgListtoolBar.TransparentColor = System.Drawing.Color.FromArgb(((System.Byte)(189)), ((System.Byte)(189)), ((System.Byte)(189)));
			// 
			// printPreviewDialog1
			// 
			this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
			this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
			this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
			this.printPreviewDialog1.Document = this.printDocument;
			this.printPreviewDialog1.Enabled = true;
			this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
			this.printPreviewDialog1.Location = new System.Drawing.Point(176, 54);
			this.printPreviewDialog1.MinimumSize = new System.Drawing.Size(375, 250);
			this.printPreviewDialog1.Name = "printPreviewDialog1";
			this.printPreviewDialog1.TransparencyKey = System.Drawing.Color.Empty;
			this.printPreviewDialog1.Visible = false;
			// 
			// agent
			// 
			this.agent.Enabled = true;
			this.agent.Location = new System.Drawing.Point(0, 400);
			this.agent.Name = "agent";
			this.agent.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("agent.OcxState")));
			this.agent.Size = new System.Drawing.Size(32, 32);
			this.agent.TabIndex = 3;
			// 
			// dockHostUp
			// 
			this.dockHostUp.Colors.TabHighlightColor = System.Drawing.Color.White;
			this.dockHostUp.Colors.TabStripBackColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(251)), ((System.Byte)(233)));
			this.dockHostUp.Colors.UseCustomTabStripBackColor = true;
			this.dockHostUp.Controls.Add(this.dockPanel1);
			this.dockHostUp.Dock = System.Windows.Forms.DockStyle.Top;
			this.dockHostUp.Location = new System.Drawing.Point(0, 26);
			this.dockHostUp.Name = "dockHostUp";
			this.dockHostUp.Size = new System.Drawing.Size(815, 61);
			this.dockHostUp.TabIndex = 5;
			// 
			// dockPanel1
			// 
			this.dockPanel1.AutoHide = false;
			this.dockPanel1.Controls.Add(this.dockControlCalc);
			this.dockPanel1.DockedHeight = 300;
			this.dockPanel1.DockedWidth = 815;
			this.dockPanel1.Location = new System.Drawing.Point(0, 0);
			this.dockPanel1.Name = "dockPanel1";
			this.dockPanel1.SelectedTab = this.dockControlCalc;
			this.dockPanel1.Size = new System.Drawing.Size(815, 57);
			this.dockPanel1.TabIndex = 5;
			this.dockPanel1.Text = "Docked Panel";
			// 
			// dockControlCalc
			// 
			this.dockControlCalc.FloatingHeight = 62;
			this.dockControlCalc.FloatingWidth = 600;
			this.dockControlCalc.Guid = new System.Guid("22475ac8-0d4b-4ea6-bab7-666e7f84857d");
			this.dockControlCalc.Location = new System.Drawing.Point(0, 20);
			this.dockControlCalc.Name = "dockControlCalc";
			this.dockControlCalc.PrimaryControl = null;
			this.dockControlCalc.Size = new System.Drawing.Size(815, 14);
			this.dockControlCalc.TabImage = null;
			this.dockControlCalc.TabIndex = 0;
			this.dockControlCalc.Text = "Calculator";
			// 
			// dockHostLeft
			// 
			this.dockHostLeft.Colors.TabHighlightColor = System.Drawing.Color.White;
			this.dockHostLeft.Colors.TabStripBackColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(251)), ((System.Byte)(233)));
			this.dockHostLeft.Colors.UseCustomTabStripBackColor = true;
			this.dockHostLeft.Dock = System.Windows.Forms.DockStyle.Left;
			this.dockHostLeft.Location = new System.Drawing.Point(0, 87);
			this.dockHostLeft.Name = "dockHostLeft";
			this.dockHostLeft.Size = new System.Drawing.Size(120, 406);
			this.dockHostLeft.TabIndex = 7;
			// 
			// dockHostDown
			// 
			this.dockHostDown.Colors.TabHighlightColor = System.Drawing.Color.White;
			this.dockHostDown.Colors.TabStripBackColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(251)), ((System.Byte)(233)));
			this.dockHostDown.Colors.UseCustomTabStripBackColor = true;
			this.dockHostDown.Controls.Add(this.dockPanelOutputTaskList);
			this.dockHostDown.Controls.Add(this.dockPanelStack);
			this.dockHostDown.Controls.Add(this.dockPanelMemory);
			this.dockHostDown.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.dockHostDown.Location = new System.Drawing.Point(120, 317);
			this.dockHostDown.Name = "dockHostDown";
			this.dockHostDown.Size = new System.Drawing.Size(559, 176);
			this.dockHostDown.TabIndex = 8;
			// 
			// dockPanelOutputTaskList
			// 
			this.dockPanelOutputTaskList.AutoHide = false;
			this.dockPanelOutputTaskList.Controls.Add(this.dockControlOutput);
			this.dockPanelOutputTaskList.Controls.Add(this.dockControlTaskList);
			this.dockPanelOutputTaskList.DockedHeight = 300;
			this.dockPanelOutputTaskList.DockedWidth = 47;
			this.dockPanelOutputTaskList.Location = new System.Drawing.Point(0, 4);
			this.dockPanelOutputTaskList.Name = "dockPanelOutputTaskList";
			this.dockPanelOutputTaskList.SelectedTab = this.dockControlTaskList;
			this.dockPanelOutputTaskList.Size = new System.Drawing.Size(44, 172);
			this.dockPanelOutputTaskList.TabIndex = 2;
			this.dockPanelOutputTaskList.Text = "Docked Panel";
			// 
			// dockControlOutput
			// 
			this.dockControlOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.dockControlOutput.Guid = new System.Guid("edc5bb92-4d3e-4522-b46e-07db1cedaf3f");
			this.dockControlOutput.Location = new System.Drawing.Point(0, 20);
			this.dockControlOutput.Name = "dockControlOutput";
			this.dockControlOutput.PrimaryControl = null;
			this.dockControlOutput.Size = new System.Drawing.Size(44, 129);
			this.dockControlOutput.TabImage = ((System.Drawing.Image)(resources.GetObject("dockControlOutput.TabImage")));
			this.dockControlOutput.TabIndex = 2;
			this.dockControlOutput.Text = "Output";
			// 
			// dockControlTaskList
			// 
			this.dockControlTaskList.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.dockControlTaskList.Guid = new System.Guid("cf61ff68-b146-4ebf-8872-b670b07b9c88");
			this.dockControlTaskList.Location = new System.Drawing.Point(0, 20);
			this.dockControlTaskList.Name = "dockControlTaskList";
			this.dockControlTaskList.PrimaryControl = null;
			this.dockControlTaskList.Size = new System.Drawing.Size(44, 129);
			this.dockControlTaskList.TabImage = ((System.Drawing.Image)(resources.GetObject("dockControlTaskList.TabImage")));
			this.dockControlTaskList.TabIndex = 1;
			this.dockControlTaskList.Text = "Task List";
			// 
			// dockPanelStack
			// 
			this.dockPanelStack.AutoHide = false;
			this.dockPanelStack.Controls.Add(this.dockControlStack);
			this.dockPanelStack.DockedHeight = 300;
			this.dockPanelStack.DockedWidth = 47;
			this.dockPanelStack.Location = new System.Drawing.Point(47, 4);
			this.dockPanelStack.Name = "dockPanelStack";
			this.dockPanelStack.SelectedTab = this.dockControlStack;
			this.dockPanelStack.Size = new System.Drawing.Size(44, 172);
			this.dockPanelStack.TabIndex = 3;
			this.dockPanelStack.Text = "Docked Panel";
			// 
			// dockControlStack
			// 
			this.dockControlStack.Guid = new System.Guid("8b26d887-a0ea-413c-8650-9e906d149510");
			this.dockControlStack.Location = new System.Drawing.Point(0, 20);
			this.dockControlStack.Name = "dockControlStack";
			this.dockControlStack.PrimaryControl = null;
			this.dockControlStack.Size = new System.Drawing.Size(44, 129);
			this.dockControlStack.TabImage = null;
			this.dockControlStack.TabIndex = 0;
			this.dockControlStack.Text = "Stack";
			// 
			// dockPanelMemory
			// 
			this.dockPanelMemory.AutoHide = false;
			this.dockPanelMemory.Controls.Add(this.dockControlMemory);
			this.dockPanelMemory.DockedHeight = 300;
			this.dockPanelMemory.DockedWidth = 465;
			this.dockPanelMemory.Location = new System.Drawing.Point(94, 4);
			this.dockPanelMemory.Name = "dockPanelMemory";
			this.dockPanelMemory.SelectedTab = this.dockControlMemory;
			this.dockPanelMemory.Size = new System.Drawing.Size(465, 172);
			this.dockPanelMemory.TabIndex = 4;
			this.dockPanelMemory.Text = "Docked Panel";
			// 
			// dockControlMemory
			// 
			this.dockControlMemory.Guid = new System.Guid("44ccf129-917a-45c5-8986-3d1a3fd7b0f6");
			this.dockControlMemory.Location = new System.Drawing.Point(0, 20);
			this.dockControlMemory.Name = "dockControlMemory";
			this.dockControlMemory.PrimaryControl = null;
			this.dockControlMemory.Size = new System.Drawing.Size(465, 129);
			this.dockControlMemory.TabImage = null;
			this.dockControlMemory.TabIndex = 0;
			this.dockControlMemory.Text = "Memory";
			// 
			// dockHostRegisters
			// 
			this.dockHostRegisters.Colors.TabHighlightColor = System.Drawing.Color.White;
			this.dockHostRegisters.Colors.TabStripBackColor = System.Drawing.Color.FromArgb(((System.Byte)(255)), ((System.Byte)(251)), ((System.Byte)(233)));
			this.dockHostRegisters.Colors.UseCustomTabStripBackColor = true;
			this.dockHostRegisters.Controls.Add(this.dockPanelRegisters);
			this.dockHostRegisters.Dock = System.Windows.Forms.DockStyle.Right;
			this.dockHostRegisters.Location = new System.Drawing.Point(679, 87);
			this.dockHostRegisters.Name = "dockHostRegisters";
			this.dockHostRegisters.Size = new System.Drawing.Size(136, 406);
			this.dockHostRegisters.TabIndex = 6;
			// 
			// dockPanelRegisters
			// 
			this.dockPanelRegisters.AutoHide = false;
			this.dockPanelRegisters.Controls.Add(this.dockControlRegisters);
			this.dockPanelRegisters.DockedHeight = 406;
			this.dockPanelRegisters.DockedWidth = 0;
			this.dockPanelRegisters.Location = new System.Drawing.Point(4, 0);
			this.dockPanelRegisters.Name = "dockPanelRegisters";
			this.dockPanelRegisters.SelectedTab = this.dockControlRegisters;
			this.dockPanelRegisters.Size = new System.Drawing.Size(132, 406);
			this.dockPanelRegisters.TabIndex = 1;
			this.dockPanelRegisters.Text = "Docked Panel";
			// 
			// dockControlRegisters
			// 
			this.dockControlRegisters.Guid = new System.Guid("c68ab0d1-9dce-4db4-b248-856e3221b2b5");
			this.dockControlRegisters.Location = new System.Drawing.Point(0, 20);
			this.dockControlRegisters.Name = "dockControlRegisters";
			this.dockControlRegisters.PrimaryControl = null;
			this.dockControlRegisters.Size = new System.Drawing.Size(132, 363);
			this.dockControlRegisters.TabImage = null;
			this.dockControlRegisters.TabIndex = 1;
			this.dockControlRegisters.Text = "Registers";
			// 
			// documentsManager
			// 
			this.documentsManager.AllowDrop = true;
			this.documentsManager.ContextMenu = this.mnuDocumentsManager;
			this.documentsManager.Dock = System.Windows.Forms.DockStyle.Fill;
			this.documentsManager.Location = new System.Drawing.Point(120, 87);
			this.documentsManager.Name = "documentsManager";
			this.documentsManager.Size = new System.Drawing.Size(559, 230);
			this.documentsManager.TabIndex = 0;
			this.documentsManager.TabStripBackColor = System.Drawing.SystemColors.InactiveCaption;
			this.documentsManager.CloseButtonPressed += new DocumentManager.DocumentManager.CloseButtonPressedEventHandler(this.documentsManager_CloseButtonPressed);
			this.documentsManager.DragEnter += new System.Windows.Forms.DragEventHandler(this.documentsManager_DragEnter);
			this.documentsManager.FocusedDocumentChanged += new System.EventHandler(this.documentsManager_FocusedDocumentChanged);
			this.documentsManager.VisibleChanged += new System.EventHandler(this.documentsManager_VisibleChanged);
			this.documentsManager.DragDrop += new System.Windows.Forms.DragEventHandler(this.documentsManager_DragDrop);
			// 
			// mnuDocumentsManager
			// 
			this.mnuDocumentsManager.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																								this.mnuDocumentManagerSave,
																								this.mnuDocumentManagerClose});
			// 
			// mnuDocumentManagerSave
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuDocumentManagerSave, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuDocumentManagerSave, 2);
			this.mnuDocumentManagerSave.Index = 0;
			this.mnuDocumentManagerSave.Text = "&Save";
			this.mnuDocumentManagerSave.Click += new System.EventHandler(this.mnuDocumentManagerSave_Click);
			// 
			// mnuDocumentManagerClose
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuDocumentManagerClose, true);
			this.mnuDocumentManagerClose.Index = 1;
			this.mnuDocumentManagerClose.Text = "&Close";
			this.mnuDocumentManagerClose.Click += new System.EventHandler(this.mnuDocumentManagerClose_Click);
			// 
			// dotNetMenuProvider1
			// 
			this.dotNetMenuProvider1.ImageList = this.imgListtoolBar;
			this.dotNetMenuProvider1.OwnerForm = this;
			// 
			// mnuFile
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuFile, true);
			this.mnuFile.Index = 0;
			this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuFileNew,
																					this.mnuFileOpen,
																					this.mnuFileClose,
																					this.mnuCloseAll,
																					this.menuItem3,
																					this.mnuFileSave,
																					this.mnuFileSaveAs,
																					this.mnuFileSaveAll,
																					this.menuItem2,
																					this.mnuFilePrintPrintCode,
																					this.mnuFilePrintPreview,
																					this.mnuFilePrintLSTFile,
																					this.menuItem11,
																					this.mnuFileLastFile1,
																					this.mnuFileLastFile2,
																					this.mnuFileLastFile3,
																					this.mnuFileLastFile4,
																					this.mnuLastFilesSeperator,
																					this.mnuFileExit});
			this.mnuFile.Text = "&File";
			// 
			// mnuFileNew
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuFileNew, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuFileNew, 0);
			this.mnuFileNew.Index = 0;
			this.mnuFileNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.mnuFileNew.Text = "&New";
			this.mnuFileNew.Click += new System.EventHandler(this.mnuFileNew_Click);
			// 
			// mnuFileOpen
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuFileOpen, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuFileOpen, 1);
			this.mnuFileOpen.Index = 1;
			this.mnuFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.mnuFileOpen.Text = "&Open...";
			this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
			// 
			// mnuFileClose
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuFileClose, true);
			this.mnuFileClose.Index = 2;
			this.mnuFileClose.Shortcut = System.Windows.Forms.Shortcut.CtrlF4;
			this.mnuFileClose.ShowShortcut = false;
			this.mnuFileClose.Text = "&Close";
			this.mnuFileClose.Click += new System.EventHandler(this.mnuFileClose_Click);
			// 
			// mnuCloseAll
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuCloseAll, true);
			this.mnuCloseAll.Index = 3;
			this.mnuCloseAll.Text = "Clos&e All";
			this.mnuCloseAll.Click += new System.EventHandler(this.mnuCloseAll_Click);
			// 
			// menuItem3
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem3, true);
			this.menuItem3.Index = 4;
			this.menuItem3.Text = "-";
			// 
			// mnuFileSave
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuFileSave, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuFileSave, 2);
			this.mnuFileSave.Index = 5;
			this.mnuFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.mnuFileSave.Text = "&Save";
			this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
			// 
			// mnuFileSaveAs
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuFileSaveAs, true);
			this.mnuFileSaveAs.Index = 6;
			this.mnuFileSaveAs.Shortcut = System.Windows.Forms.Shortcut.CtrlShiftS;
			this.mnuFileSaveAs.ShowShortcut = false;
			this.mnuFileSaveAs.Text = "Save &As...";
			this.mnuFileSaveAs.Click += new System.EventHandler(this.mnuFileSaveAs_Click);
			// 
			// mnuFileSaveAll
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuFileSaveAll, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuFileSaveAll, 3);
			this.mnuFileSaveAll.Index = 7;
			this.mnuFileSaveAll.Text = "Save A&ll";
			this.mnuFileSaveAll.Click += new System.EventHandler(this.mnuFileSaveAll_Click);
			// 
			// menuItem2
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem2, true);
			this.menuItem2.Index = 8;
			this.menuItem2.Text = "-";
			// 
			// mnuFilePrintPrintCode
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuFilePrintPrintCode, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuFilePrintPrintCode, 4);
			this.mnuFilePrintPrintCode.Index = 9;
			this.mnuFilePrintPrintCode.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
			this.mnuFilePrintPrintCode.Text = "&Print Code...";
			this.mnuFilePrintPrintCode.Click += new System.EventHandler(this.mnuFilePrintPrintCode_Click);
			// 
			// mnuFilePrintPreview
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuFilePrintPreview, true);
			this.mnuFilePrintPreview.Index = 10;
			this.mnuFilePrintPreview.Text = "Print Previe&w";
			this.mnuFilePrintPreview.Click += new System.EventHandler(this.mnuFilePrintPreview_Click);
			// 
			// mnuFilePrintLSTFile
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuFilePrintLSTFile, true);
			this.mnuFilePrintLSTFile.Index = 11;
			this.mnuFilePrintLSTFile.Text = "(Canceled) Print LS&T File...";
			this.mnuFilePrintLSTFile.Visible = false;
			// 
			// menuItem11
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem11, true);
			this.menuItem11.Index = 12;
			this.menuItem11.Text = "-";
			// 
			// mnuFileLastFile1
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuFileLastFile1, true);
			this.mnuFileLastFile1.Index = 13;
			this.mnuFileLastFile1.Text = "";
			this.mnuFileLastFile1.Visible = false;
			this.mnuFileLastFile1.Click += new System.EventHandler(this.mnuFileLastFile1_Click);
			// 
			// mnuFileLastFile2
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuFileLastFile2, true);
			this.mnuFileLastFile2.Index = 14;
			this.mnuFileLastFile2.Text = "";
			this.mnuFileLastFile2.Visible = false;
			this.mnuFileLastFile2.Click += new System.EventHandler(this.mnuFileLastFile2_Click);
			// 
			// mnuFileLastFile3
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuFileLastFile3, true);
			this.mnuFileLastFile3.Index = 15;
			this.mnuFileLastFile3.Text = "";
			this.mnuFileLastFile3.Visible = false;
			this.mnuFileLastFile3.Click += new System.EventHandler(this.mnuFileLastFile3_Click);
			// 
			// mnuFileLastFile4
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuFileLastFile4, true);
			this.mnuFileLastFile4.Index = 16;
			this.mnuFileLastFile4.Text = "";
			this.mnuFileLastFile4.Visible = false;
			this.mnuFileLastFile4.Click += new System.EventHandler(this.mnuFileLastFile4_Click);
			// 
			// mnuLastFilesSeperator
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuLastFilesSeperator, true);
			this.mnuLastFilesSeperator.Index = 17;
			this.mnuLastFilesSeperator.Text = "-";
			this.mnuLastFilesSeperator.Visible = false;
			// 
			// mnuFileExit
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuFileExit, true);
			this.mnuFileExit.Index = 18;
			this.mnuFileExit.Text = "E&xit";
			this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
			// 
			// mnuEdit
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuEdit, true);
			this.mnuEdit.Index = 1;
			this.mnuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuEditUndo,
																					this.mnuEditRedo,
																					this.menuItem42,
																					this.mnuEditCut,
																					this.mnuEditCopy,
																					this.mnuEditPaste,
																					this.mnuEditDelete,
																					this.menuItem37,
																					this.mnuEditSelectAll,
																					this.menuItem1,
																					this.mnuEditFind,
																					this.mnuEditFindNext,
																					this.mnuEditReplace,
																					this.mnuGoTo});
			this.mnuEdit.Text = "&Edit";
			// 
			// mnuEditUndo
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuEditUndo, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuEditUndo, 10);
			this.mnuEditUndo.Index = 0;
			this.mnuEditUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
			this.mnuEditUndo.Text = "(Canceled) &Undo";
			this.mnuEditUndo.Visible = false;
			this.mnuEditUndo.Click += new System.EventHandler(this.mnuEditUndo_Click);
			// 
			// mnuEditRedo
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuEditRedo, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuEditRedo, 11);
			this.mnuEditRedo.Index = 1;
			this.mnuEditRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
			this.mnuEditRedo.Text = "(Canceled) &Redo";
			this.mnuEditRedo.Visible = false;
			this.mnuEditRedo.Click += new System.EventHandler(this.mnuEditRedo_Click);
			// 
			// menuItem42
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem42, true);
			this.menuItem42.Index = 2;
			this.menuItem42.Text = "-";
			this.menuItem42.Visible = false;
			// 
			// mnuEditCut
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuEditCut, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuEditCut, 7);
			this.mnuEditCut.Index = 3;
			this.mnuEditCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
			this.mnuEditCut.Text = "Cu&t";
			this.mnuEditCut.Click += new System.EventHandler(this.mnuEditCut_Click);
			// 
			// mnuEditCopy
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuEditCopy, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuEditCopy, 8);
			this.mnuEditCopy.Index = 4;
			this.mnuEditCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			this.mnuEditCopy.Text = "&Copy";
			this.mnuEditCopy.Click += new System.EventHandler(this.mnuEditCopy_Click);
			// 
			// mnuEditPaste
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuEditPaste, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuEditPaste, 9);
			this.mnuEditPaste.Index = 5;
			this.mnuEditPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
			this.mnuEditPaste.Text = "&Paste";
			this.mnuEditPaste.Click += new System.EventHandler(this.mnuEditPaste_Click);
			// 
			// mnuEditDelete
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuEditDelete, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuEditDelete, 25);
			this.mnuEditDelete.Index = 6;
			this.mnuEditDelete.Text = "&Delete";
			this.mnuEditDelete.Click += new System.EventHandler(this.mnuEditDelete_Click);
			// 
			// menuItem37
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem37, true);
			this.menuItem37.Index = 7;
			this.menuItem37.Text = "-";
			// 
			// mnuEditSelectAll
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuEditSelectAll, true);
			this.mnuEditSelectAll.Index = 8;
			this.mnuEditSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
			this.mnuEditSelectAll.Text = "Select &All";
			this.mnuEditSelectAll.Click += new System.EventHandler(this.mnuEditSelectAll_Click);
			// 
			// menuItem1
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem1, true);
			this.menuItem1.Index = 9;
			this.menuItem1.Text = "-";
			// 
			// mnuEditFind
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuEditFind, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuEditFind, 6);
			this.mnuEditFind.Index = 10;
			this.mnuEditFind.Shortcut = System.Windows.Forms.Shortcut.CtrlF;
			this.mnuEditFind.Text = "&Find...";
			this.mnuEditFind.Click += new System.EventHandler(this.mnuEditFind_Click);
			// 
			// mnuEditFindNext
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuEditFindNext, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuEditFindNext, 5);
			this.mnuEditFindNext.Index = 11;
			this.mnuEditFindNext.Shortcut = System.Windows.Forms.Shortcut.F3;
			this.mnuEditFindNext.Text = "Find &Next";
			this.mnuEditFindNext.Click += new System.EventHandler(this.mnuEditFindNext_Click);
			// 
			// mnuEditReplace
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuEditReplace, true);
			this.mnuEditReplace.Index = 12;
			this.mnuEditReplace.Shortcut = System.Windows.Forms.Shortcut.CtrlH;
			this.mnuEditReplace.Text = "(Canceled) &Replace...";
			this.mnuEditReplace.Visible = false;
			// 
			// mnuGoTo
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuGoTo, true);
			this.mnuGoTo.Index = 13;
			this.mnuGoTo.Shortcut = System.Windows.Forms.Shortcut.CtrlG;
			this.mnuGoTo.Text = "&Go To...";
			this.mnuGoTo.Click += new System.EventHandler(this.mnuGoTo_Click);
			// 
			// menuItem5
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem5, true);
			this.menuItem5.Index = 2;
			this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuInsertGeneral,
																					  this.mnuInsertLoop,
																					  this.mnuInsertOutput});
			this.menuItem5.Text = "(Canceled) &Insert";
			this.menuItem5.Visible = false;
			// 
			// mnuInsertGeneral
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuInsertGeneral, true);
			this.mnuInsertGeneral.Index = 0;
			this.mnuInsertGeneral.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							 this.mnuInsertBasicApplication,
																							 this.mnuInsertProgramEnd,
																							 this.mnuInsertFunctionDocumentation});
			this.mnuInsertGeneral.Text = "&General";
			// 
			// mnuInsertBasicApplication
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuInsertBasicApplication, true);
			this.mnuInsertBasicApplication.Index = 0;
			this.mnuInsertBasicApplication.Text = "&Basic Application";
			// 
			// mnuInsertProgramEnd
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuInsertProgramEnd, true);
			this.mnuInsertProgramEnd.Index = 1;
			this.mnuInsertProgramEnd.Text = "Program &End";
			// 
			// mnuInsertFunctionDocumentation
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuInsertFunctionDocumentation, true);
			this.mnuInsertFunctionDocumentation.Index = 2;
			this.mnuInsertFunctionDocumentation.Text = "Function &Documentation";
			// 
			// mnuInsertLoop
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuInsertLoop, true);
			this.mnuInsertLoop.Index = 1;
			this.mnuInsertLoop.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						  this.mnuInsertFor,
																						  this.mnuInsertWhile,
																						  this.mnuInsertDo});
			this.mnuInsertLoop.Text = "&Loops";
			// 
			// mnuInsertFor
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuInsertFor, true);
			this.mnuInsertFor.Index = 0;
			this.mnuInsertFor.Text = "&For Loop";
			// 
			// mnuInsertWhile
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuInsertWhile, true);
			this.mnuInsertWhile.Index = 1;
			this.mnuInsertWhile.Text = "&While Loop";
			// 
			// mnuInsertDo
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuInsertDo, true);
			this.mnuInsertDo.Index = 2;
			this.mnuInsertDo.Text = "&Do..While Loop";
			// 
			// mnuInsertOutput
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuInsertOutput, true);
			this.mnuInsertOutput.Index = 2;
			this.mnuInsertOutput.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							this.menuItem8,
																							this.menuItem13});
			this.mnuInsertOutput.Text = "&Output";
			// 
			// menuItem8
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem8, true);
			this.menuItem8.Index = 0;
			this.menuItem8.Text = "&Printf Call";
			// 
			// menuItem13
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem13, true);
			this.menuItem13.Index = 1;
			this.menuItem13.Text = "P&uts Call";
			// 
			// mnuBuild
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuBuild, true);
			this.mnuBuild.Index = 3;
			this.mnuBuild.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnuBuildCompile,
																					 this.mnuBuildViewLSTFile,
																					 this.mnuBuildInputOutputFiles,
																					 this.menuItem6,
																					 this.mnuBuildRun});
			this.mnuBuild.Text = "&Build";
			// 
			// mnuBuildCompile
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuBuildCompile, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuBuildCompile, 16);
			this.mnuBuildCompile.Index = 0;
			this.mnuBuildCompile.Shortcut = System.Windows.Forms.Shortcut.F7;
			this.mnuBuildCompile.Text = "&Compile";
			this.mnuBuildCompile.Click += new System.EventHandler(this.mnuBuildCompile_Click);
			// 
			// mnuBuildViewLSTFile
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuBuildViewLSTFile, true);
			this.mnuBuildViewLSTFile.Index = 1;
			this.mnuBuildViewLSTFile.Shortcut = System.Windows.Forms.Shortcut.CtrlL;
			this.mnuBuildViewLSTFile.Text = "&View LST File";
			this.mnuBuildViewLSTFile.Click += new System.EventHandler(this.mnuBuildViewLSTFile_Click);
			// 
			// mnuBuildInputOutputFiles
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuBuildInputOutputFiles, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuBuildInputOutputFiles, 21);
			this.mnuBuildInputOutputFiles.Index = 2;
			this.mnuBuildInputOutputFiles.Text = "Set &Input/Output Files...";
			this.mnuBuildInputOutputFiles.Click += new System.EventHandler(this.mnuBuildInputOutputFiles_Click);
			// 
			// menuItem6
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem6, true);
			this.menuItem6.Index = 3;
			this.menuItem6.Text = "-";
			// 
			// mnuBuildRun
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuBuildRun, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuBuildRun, 14);
			this.mnuBuildRun.Index = 4;
			this.mnuBuildRun.Shortcut = System.Windows.Forms.Shortcut.CtrlF5;
			this.mnuBuildRun.Text = "&Execute";
			this.mnuBuildRun.Click += new System.EventHandler(this.mnuBuildRun_Click);
			// 
			// mnuDebug
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuDebug, true);
			this.mnuDebug.Index = 4;
			this.mnuDebug.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnuDebugRun,
																					 this.menuItem19,
																					 this.mnuDebugStepInto,
																					 this.mnuDebugRestartProgram,
																					 this.mnuDebugRunToCursor,
																					 this.menuItem25,
																					 this.mnuDebugWatch,
																					 this.mnuDebugBreakPoint,
																					 this.mnuDebugRemoveAllBreakPoints});
			this.mnuDebug.Text = "&Debug";
			// 
			// mnuDebugRun
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuDebugRun, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuDebugRun, 12);
			this.mnuDebugRun.Index = 0;
			this.mnuDebugRun.Shortcut = System.Windows.Forms.Shortcut.F5;
			this.mnuDebugRun.Text = "&Run";
			this.mnuDebugRun.Click += new System.EventHandler(this.mnuDebugRun_Click);
			// 
			// menuItem19
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem19, true);
			this.menuItem19.Index = 1;
			this.menuItem19.Text = "-";
			// 
			// mnuDebugStepInto
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuDebugStepInto, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuDebugStepInto, 15);
			this.mnuDebugStepInto.Index = 2;
			this.mnuDebugStepInto.Shortcut = System.Windows.Forms.Shortcut.F11;
			this.mnuDebugStepInto.Text = "&Step";
			this.mnuDebugStepInto.Click += new System.EventHandler(this.mnuDebugStepInto_Click);
			// 
			// mnuDebugRestartProgram
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuDebugRestartProgram, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuDebugRestartProgram, 13);
			this.mnuDebugRestartProgram.Index = 3;
			this.mnuDebugRestartProgram.Shortcut = System.Windows.Forms.Shortcut.ShiftF5;
			this.mnuDebugRestartProgram.Text = "Restart &Program";
			this.mnuDebugRestartProgram.Click += new System.EventHandler(this.mnuDebugRestartProgram_Click);
			// 
			// mnuDebugRunToCursor
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuDebugRunToCursor, true);
			this.mnuDebugRunToCursor.Index = 4;
			this.mnuDebugRunToCursor.Shortcut = System.Windows.Forms.Shortcut.F4;
			this.mnuDebugRunToCursor.Text = "(Canceled) Run &to Cursor";
			this.mnuDebugRunToCursor.Visible = false;
			// 
			// menuItem25
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem25, true);
			this.menuItem25.Index = 5;
			this.menuItem25.Text = "-";
			// 
			// mnuDebugWatch
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuDebugWatch, true);
			this.mnuDebugWatch.Index = 6;
			this.mnuDebugWatch.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						  this.mnuDebugAddWatch,
																						  this.mnuDebugDelAllWatchs});
			this.mnuDebugWatch.Text = "(Canceled) &Watchs";
			this.mnuDebugWatch.Visible = false;
			// 
			// mnuDebugAddWatch
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuDebugAddWatch, true);
			this.mnuDebugAddWatch.Index = 0;
			this.mnuDebugAddWatch.Shortcut = System.Windows.Forms.Shortcut.CtrlW;
			this.mnuDebugAddWatch.Text = "&Add Watch";
			this.mnuDebugAddWatch.Click += new System.EventHandler(this.mnuDebugAddWatch_Click);
			// 
			// mnuDebugDelAllWatchs
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuDebugDelAllWatchs, true);
			this.mnuDebugDelAllWatchs.Index = 1;
			this.mnuDebugDelAllWatchs.Text = "&Clear All Watchs";
			// 
			// mnuDebugBreakPoint
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuDebugBreakPoint, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuDebugBreakPoint, 19);
			this.mnuDebugBreakPoint.Index = 7;
			this.mnuDebugBreakPoint.Shortcut = System.Windows.Forms.Shortcut.F9;
			this.mnuDebugBreakPoint.Text = "Add/Remove &Breakpoint";
			this.mnuDebugBreakPoint.Click += new System.EventHandler(this.mnuDebugBreakPoint_Click);
			// 
			// mnuDebugRemoveAllBreakPoints
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuDebugRemoveAllBreakPoints, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuDebugRemoveAllBreakPoints, 20);
			this.mnuDebugRemoveAllBreakPoints.Index = 8;
			this.mnuDebugRemoveAllBreakPoints.Text = "Remove &All Breakpoint";
			this.mnuDebugRemoveAllBreakPoints.Click += new System.EventHandler(this.mnuDebugRemoveAllBreakPoints_Click);
			// 
			// mnuView
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuView, true);
			this.mnuView.Index = 5;
			this.mnuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuViewRegisters,
																					this.mnuViewStack,
																					this.mnuViewMemory,
																					this.mnuViewWatchs,
																					this.mnuViewTaskList,
																					this.mnuViewOutput});
			this.mnuView.Text = "&View";
			// 
			// mnuViewRegisters
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuViewRegisters, true);
			this.mnuViewRegisters.Index = 0;
			this.mnuViewRegisters.Text = "&Registers";
			this.mnuViewRegisters.Click += new System.EventHandler(this.mnuViewRegisters_Click);
			// 
			// mnuViewStack
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuViewStack, true);
			this.mnuViewStack.Index = 1;
			this.mnuViewStack.Text = "&Stack";
			this.mnuViewStack.Click += new System.EventHandler(this.mnuViewStack_Click);
			// 
			// mnuViewMemory
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuViewMemory, true);
			this.mnuViewMemory.Index = 2;
			this.mnuViewMemory.Text = "M&emory";
			this.mnuViewMemory.Click += new System.EventHandler(this.mnuViewMemory_Click);
			// 
			// mnuViewWatchs
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuViewWatchs, true);
			this.mnuViewWatchs.Index = 3;
			this.mnuViewWatchs.Text = "(Canceled) &Watchs";
			this.mnuViewWatchs.Visible = false;
			// 
			// mnuViewTaskList
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuViewTaskList, true);
			this.mnuViewTaskList.Index = 4;
			this.mnuViewTaskList.Text = "&Task List";
			this.mnuViewTaskList.Click += new System.EventHandler(this.mnuViewTaskList_Click);
			// 
			// mnuViewOutput
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuViewOutput, true);
			this.mnuViewOutput.Index = 5;
			this.mnuViewOutput.Text = "&Output";
			this.mnuViewOutput.Click += new System.EventHandler(this.mnuViewOutput_Click);
			// 
			// mnuTools
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuTools, true);
			this.mnuTools.Index = 6;
			this.mnuTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnuToolsHex,
																					 this.mnuToolsOptions});
			this.mnuTools.Text = "&Tools";
			// 
			// mnuToolsHex
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuToolsHex, true);
			this.mnuToolsHex.Index = 0;
			this.mnuToolsHex.Text = "&HEX Calculator...";
			this.mnuToolsHex.Click += new System.EventHandler(this.mnuToolsHex_Click);
			// 
			// mnuToolsOptions
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuToolsOptions, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuToolsOptions, 17);
			this.mnuToolsOptions.Index = 1;
			this.mnuToolsOptions.Text = "&Options...";
			this.mnuToolsOptions.Click += new System.EventHandler(this.mnuToolsOptions_Click);
			// 
			// mnuHelp
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuHelp, true);
			this.mnuHelp.Index = 7;
			this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuHelpContents,
																					this.mnuHelpGetStarted,
																					this.mnuHelpGetHelp,
																					this.menuItem28,
																					this.mnuHelpAbout});
			this.mnuHelp.Text = "&Help";
			// 
			// mnuHelpContents
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuHelpContents, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuHelpContents, 22);
			this.mnuHelpContents.Index = 0;
			this.mnuHelpContents.Shortcut = System.Windows.Forms.Shortcut.CtrlF1;
			this.mnuHelpContents.Text = "&Contents...";
			this.mnuHelpContents.Click += new System.EventHandler(this.mnuHelpContents_Click);
			// 
			// mnuHelpGetStarted
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuHelpGetStarted, true);
			this.mnuHelpGetStarted.Index = 1;
			this.mnuHelpGetStarted.Text = "Get &Started";
			this.mnuHelpGetStarted.Click += new System.EventHandler(this.mnuHelpGetStarted_Click);
			// 
			// mnuHelpGetHelp
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuHelpGetHelp, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuHelpGetHelp, 23);
			this.mnuHelpGetHelp.Index = 2;
			this.mnuHelpGetHelp.Shortcut = System.Windows.Forms.Shortcut.F1;
			this.mnuHelpGetHelp.Text = "&Get Help";
			this.mnuHelpGetHelp.Click += new System.EventHandler(this.mnuHelpGetHelp_Click);
			// 
			// menuItem28
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem28, true);
			this.menuItem28.Index = 3;
			this.menuItem28.Text = "-";
			// 
			// mnuHelpAbout
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuHelpAbout, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuHelpAbout, 26);
			this.mnuHelpAbout.Index = 4;
			this.mnuHelpAbout.Text = "&About...";
			this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuFile,
																					  this.mnuEdit,
																					  this.menuItem5,
																					  this.mnuBuild,
																					  this.mnuDebug,
																					  this.mnuView,
																					  this.mnuTools,
																					  this.mnuHelp});
			this.mainMenu1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// dotNetToolbar1
			// 
			this.dotNetToolbar1.Buttons.Add(this.tbButtonNew);
			this.dotNetToolbar1.Buttons.Add(this.tbButtonOpen);
			this.dotNetToolbar1.Buttons.Add(this.tbButtonSave);
			this.dotNetToolbar1.Buttons.Add(this.tbButtonSaveAll);
			this.dotNetToolbar1.Buttons.Add(this.tbButtonPrint);
			this.dotNetToolbar1.Buttons.Add(this.tbButtonFind);
			this.dotNetToolbar1.Buttons.Add(this.tbButtonCut);
			this.dotNetToolbar1.Buttons.Add(this.tbButtonCopy);
			this.dotNetToolbar1.Buttons.Add(this.tbButtonPaste);
			this.dotNetToolbar1.Buttons.Add(this.tbButtonCompile);
			this.dotNetToolbar1.Buttons.Add(this.tbButtonExecute);
			this.dotNetToolbar1.Buttons.Add(this.tbButtonRunDebug);
			this.dotNetToolbar1.Buttons.Add(this.tbButtonRestartDebug);
			this.dotNetToolbar1.Buttons.Add(this.tbButtonStep);
			this.dotNetToolbar1.Buttons.Add(this.tbButtonBreakPoint);
			this.dotNetToolbar1.Buttons.Add(this.tbButtonInOut);
			this.dotNetToolbar1.ImageList = this.imgListtoolBar;
			this.dotNetToolbar1.Location = new System.Drawing.Point(0, 0);
			this.dotNetToolbar1.MenuProvider = null;
			this.dotNetToolbar1.Name = "dotNetToolbar1";
			this.dotNetToolbar1.Size = new System.Drawing.Size(815, 26);
			this.dotNetToolbar1.TabIndex = 9;
			// 
			// tbButtonNew
			// 
			this.tbButtonNew.ImageIndex = 0;
			this.tbButtonNew.MenuInvoke = this.mnuFileNew;
			this.tbButtonNew.ToolTipText = "New File";
			// 
			// tbButtonOpen
			// 
			this.tbButtonOpen.ImageIndex = 1;
			this.tbButtonOpen.MenuInvoke = this.mnuFileOpen;
			this.tbButtonOpen.ToolTipText = "Open";
			// 
			// tbButtonSave
			// 
			this.tbButtonSave.ImageIndex = 2;
			this.tbButtonSave.MenuInvoke = this.mnuFileSave;
			this.tbButtonSave.ToolTipText = "Save";
			// 
			// tbButtonSaveAll
			// 
			this.tbButtonSaveAll.ImageIndex = 3;
			this.tbButtonSaveAll.MenuInvoke = this.mnuFileSaveAll;
			this.tbButtonSaveAll.ToolTipText = "Save All";
			// 
			// tbButtonPrint
			// 
			this.tbButtonPrint.BeginGroup = true;
			this.tbButtonPrint.ImageIndex = 4;
			this.tbButtonPrint.MenuInvoke = this.mnuFilePrintPrintCode;
			this.tbButtonPrint.ToolTipText = "Print";
			// 
			// tbButtonFind
			// 
			this.tbButtonFind.ImageIndex = 6;
			this.tbButtonFind.MenuInvoke = this.mnuEditFind;
			this.tbButtonFind.ToolTipText = "Find";
			// 
			// tbButtonCut
			// 
			this.tbButtonCut.BeginGroup = true;
			this.tbButtonCut.ImageIndex = 7;
			this.tbButtonCut.MenuInvoke = this.mnuEditCut;
			this.tbButtonCut.ToolTipText = "Cut";
			// 
			// tbButtonCopy
			// 
			this.tbButtonCopy.ImageIndex = 8;
			this.tbButtonCopy.MenuInvoke = this.mnuEditCopy;
			this.tbButtonCopy.ToolTipText = "Copy";
			// 
			// tbButtonPaste
			// 
			this.tbButtonPaste.ImageIndex = 9;
			this.tbButtonPaste.MenuInvoke = this.mnuEditPaste;
			this.tbButtonPaste.ToolTipText = "Paste";
			// 
			// tbButtonCompile
			// 
			this.tbButtonCompile.BeginGroup = true;
			this.tbButtonCompile.ImageIndex = 16;
			this.tbButtonCompile.MenuInvoke = this.mnuBuildCompile;
			this.tbButtonCompile.ToolTipText = "Compile";
			// 
			// tbButtonExecute
			// 
			this.tbButtonExecute.ImageIndex = 14;
			this.tbButtonExecute.MenuInvoke = this.mnuBuildRun;
			this.tbButtonExecute.ToolTipText = "Execute";
			// 
			// tbButtonRunDebug
			// 
			this.tbButtonRunDebug.BeginGroup = true;
			this.tbButtonRunDebug.ImageIndex = 12;
			this.tbButtonRunDebug.MenuInvoke = this.mnuDebugRun;
			this.tbButtonRunDebug.ToolTipText = "Run";
			// 
			// tbButtonRestartDebug
			// 
			this.tbButtonRestartDebug.ImageIndex = 13;
			this.tbButtonRestartDebug.MenuInvoke = this.mnuDebugRestartProgram;
			this.tbButtonRestartDebug.ToolTipText = "Restart Program";
			// 
			// tbButtonStep
			// 
			this.tbButtonStep.ImageIndex = 15;
			this.tbButtonStep.MenuInvoke = this.mnuDebugStepInto;
			this.tbButtonStep.ToolTipText = "Step";
			// 
			// tbButtonBreakPoint
			// 
			this.tbButtonBreakPoint.ImageIndex = 19;
			this.tbButtonBreakPoint.MenuInvoke = this.mnuDebugBreakPoint;
			this.tbButtonBreakPoint.ToolTipText = "Add/Remove Breakpoint";
			// 
			// tbButtonInOut
			// 
			this.tbButtonInOut.BeginGroup = true;
			this.tbButtonInOut.ImageIndex = 21;
			this.tbButtonInOut.MenuInvoke = this.mnuBuildInputOutputFiles;
			this.tbButtonInOut.Text = "In/Out";
			this.tbButtonInOut.ToolTipText = "Input/Output Files";
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(815, 515);
			this.Controls.Add(this.documentsManager);
			this.Controls.Add(this.dockHostDown);
			this.Controls.Add(this.dockHostLeft);
			this.Controls.Add(this.dockHostRegisters);
			this.Controls.Add(this.dockHostUp);
			this.Controls.Add(this.agent);
			this.Controls.Add(this.dotNetToolbar1);
			this.Controls.Add(this.statusBar1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu1;
			this.MinimumSize = new System.Drawing.Size(480, 300);
			this.Name = "frmMain";
			this.Text = "VAX11 Simulator - Version ";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Resize += new System.EventHandler(this.frmMain_Resize);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmMain_Closing);
			this.Load += new System.EventHandler(this.frmMain_Load);
			((System.ComponentModel.ISupportInitialize)(this.sbpMessage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpCurrentLocation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.agent)).EndInit();
			this.dockHostUp.ResumeLayout(false);
			this.dockPanel1.ResumeLayout(false);
			this.dockHostDown.ResumeLayout(false);
			this.dockPanelOutputTaskList.ResumeLayout(false);
			this.dockPanelStack.ResumeLayout(false);
			this.dockPanelMemory.ResumeLayout(false);
			this.dockHostRegisters.ResumeLayout(false);
			this.dockPanelRegisters.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		
		public string ReturnInformation()
		{
			return frmabout.lblInformation.Text + frmabout.Controls.Count.ToString();
		}

		#endregion

		#region Sub-Windows

		#region Output Window Interface
		/***************************************************************************/
		/*						Output Window Interface functions				   */
		/***************************************************************************/

		/// <summary>
		/// Clears the output window
		/// </summary>
		public void ClearOutputWindow()
		{
			cntCompileMessage.Output = "";
		}

		/// <summary>
		/// Append text to output window
		/// </summary>
		/// <param name="sOutput">The text to append</param>
		public void AppendToOutputWindow(string sOutput)
		{
			cntCompileMessage.Output += sOutput + "\r\n";
		}

		public void SetOutputWindowContent(string sOutput)
		{
			cntCompileMessage.Output = sOutput;
		}

		public string GetOutputWindowContent()
		{
			return cntCompileMessage.Output;
		}

		#endregion

		#region Tasks List Interface

		/// <summary>
		/// Interface function to add tasks to the Task List
		/// </summary>
		/// <param name="taskType">Task type - error / warning</param>
		/// <param name="TaskMessage">Message to display</param>
		/// <param name="iLine">Line Number to display</param>

		private void AddTask(frmTaskList.Icons taskType, string
			TaskMessage, int iLine)
		{
			cntTaskList.AddTask(taskType,
				TaskMessage, iLine);
		}


		/// <summary>
		/// Clears all tasks from task list
		/// </summary>

		private void ClearTasks()
		{
			cntTaskList.ClearTasks();
		}

		/// <summary>
		/// Change all line numbers for tasks from selected start 
		/// line number by selected offset
		/// </summary>
		/// <param name="iFromLine">Start line number</param>
		/// <param name="iOffset">Offset (in line numbers)</param>

		private void updateTasksLinesNumber(int iFromLine, int iOffset)
		{
			cntTaskList.updateTasksLinesNumber(iFromLine, iOffset);
		}

		/// <summary>
		/// Function to handle clicks on tasks list.
		/// Not to be called by the user, only by events
		/// </summary>
		/// <remark>
		/// Don't change its parameters unless you change also
		/// frmTaskList.clickEventFunc delegate, as this function
		/// is used as delegate
		/// </remark>
		/// <param name="iLine">The functions gets the line where the user pressed</param>

		private void OnTaskListClick(int iLine)
		{
			// Check if there is an open document
			if (documentsManager.TabStrips.Count == 0) return;

			try
			{
				// Find the current docuemnt
				frmEditor theDoc = GetActiveDocument();

				documentsManager.Select();
			
				// Select the line
				theDoc.SetLine(iLine);
				theDoc.FocusDocument();
				theDoc.ColorErrorLine(iLine);
			}

			catch (NoActivePageException) { return ; }
	
		}
		#endregion

		#endregion

		#region Documents Interface

		/// <summary>
		/// Gets a refrence to the active document
		/// </summary>
		/// <returns></returns>
		private frmEditor GetActiveDocument()
		{
			// Check if there is an open document
			if (documentsManager.FocusedDocument == null) throw new NoActivePageException();
			return (frmEditor)documentsManager.FocusedDocument.Control;
		}

		/// <summary>
		/// Gets the active document title
		/// </summary>
		/// <returns>the title</returns>
		private string GetActiveDocumentTitle()
		{
			// Check if there is an open document
			if (documentsManager.FocusedDocument == null) throw new NoActivePageException();
			return documentsManager.FocusedDocument.Text;
		}

		/// <summary>
		/// Sets the active document title
		/// </summary>
		/// <param name="s">new title</param>
		private void SetActiveDocumentTitle(string s)
		{
			// Check if there is an open document
			if (documentsManager.FocusedDocument == null) throw new NoActivePageException();
			documentsManager.FocusedDocument.Text = s;
		}

		/// <summary>
		/// Remove the active document
		/// </summary>
		private void RemoveActiveDocument()
		{
			if (documentsManager.FocusedDocument == null) return;
			documentsManager.RemoveDocument(documentsManager.FocusedDocument);
		}



		/// <summary>
		/// Update the status bar location when caret changes position
		/// </summary>
		/// <param name="Location">The new location</param>
		/// <remarks>Not a general function - relies on the struct of the statusBar</remarks>
		void OnDocumentPosition(Point Location)
		{
			iCurrentLine = Location.X ;
			statusBar1.Panels[1].Text = " Ln " + Location.X.ToString() + 
				"\tCol " + Location.Y.ToString();
		}

		/// <summary>
		/// Redraw all documents
		/// </summary>
		void ReColorAllDocuments()
		{
			if (documentsManager.TabStrips.Count == 0) return;


			foreach (DocumentManager.MdiTabStrip m in documentsManager.TabStrips)
			{
				foreach (DocumentManager.Document d in m.Documents)
				{
					frmEditor curDoc = (frmEditor)d.Control;
					bool bVisibleForm = curDoc.Visible;
					curDoc.bDoColor = Settings.Environment.bDoSyntaxHighlight;
					curDoc.ColorAllLines();
					curDoc.Visible = bVisibleForm;
					curDoc.ClearAllBreakPoints();
					curDoc.ColorAllBreakPoints();
					curDoc.ColorStep();
				}
			}
		}

		/// <summary>
		/// Select all text in the active document
		/// </summary>
		void InterfaceSelectAll()
		{
			try { GetActiveDocument().SelectAll(); }
			catch (NoActivePageException) { return ; }		
		}


		#endregion

		#region Files - Creating, loading, saving and closing


		/// <summary>
		/// Create new document
		/// </summary>
		private void CreateNewDocument()
		{

			// Create new editor window.
			frmEditor frmNewEditorWindow = new frmEditor(new Assembler.compileErrorHandler(HandleError), this);
			frmNewEditorWindow.SetDragNDrop(new System.Windows.Forms.DragEventHandler(this.documentsManager_DragEnter), new System.Windows.Forms.DragEventHandler(this.documentsManager_DragDrop));

			// File name to display
			// Lolish bug here : if docCount > int.MaxValue it will be funny
			string sFileName = "Untitled" + ++docCount + ".asm";

			DocumentManager.Document theDocument = new DocumentManager.Document(frmNewEditorWindow, sFileName);
			documentsManager.AddDocument(theDocument);
			documentsManager.TabStrips[0].SelectedDocument = theDocument;
			documentsManager.FocusedDocument = theDocument;

			// Set syntax highlight option for the document
			frmNewEditorWindow.bDoColor = Settings.Environment.bDoSyntaxHighlight;

			// Event Hooking
			frmNewEditorWindow.OnPositionChange += new frmEditor.PositionEventFunc(OnDocumentPosition);
			
		}


		/// <summary>
		/// Open assembly file
		/// </summary>
		/// <returns>Success or fail</returns>
		/// <param name="sFileName">File Name, should be always null,
		/// unless we know the file name the user wants to open</param>
		private bool DoOpen(string sFileName)
		{
			try
			{
				string sFileToOpen;
				if (sFileName == null)
				{
					// Asks the user for the filename to open
					openFileDialog1.Filter = Settings.Environment.FILE_FILTERS;
					openFileDialog1.FileName = "";
					DialogResult res = openFileDialog1.ShowDialog();
					sFileToOpen = openFileDialog1.FileName;
					if (res != DialogResult.OK) return false;
//					frmOpenDialog x = new frmOpenDialog();
//					x.ShowDialog();
//					sFileToOpen = x.FileName;
				}
				else sFileToOpen = sFileName;

				// Is the file already open?
				foreach (DocumentManager.MdiTabStrip m in documentsManager.TabStrips)
				{
					foreach (DocumentManager.Document d in m.Documents)
					{
						frmEditor theDoc = (frmEditor)d.Control;
						if (theDoc.sFileName == sFileToOpen)
						{
							documentsManager.FocusedDocument = d;
							if (theDoc.bDocumentSaved) return true;
							MessageBox.Show("There is already an open copy of the file.", Settings.Environment.MESSAGEBOXS_TITLE);
							return true;
						}
					}
				}


				// Open document specific by openFileDialog1.FileName
				StreamReader inputFile = new StreamReader(sFileToOpen, System.Text.Encoding.Default);
				string sFileContent = inputFile.ReadToEnd();
				inputFile.Close();

				// File name to display
				string sShownFileName = sFileToOpen.Substring(sFileToOpen.LastIndexOf(@"\") + 1);
				CreateNewTabAndPutThereTheDocument(sFileToOpen, sShownFileName, sFileContent, Settings.Environment.bDoSyntaxHighlight);

				// Save the file path
				Settings.Environment.sLastFileLocation = sFileToOpen;
				UpdateSettingsWithNewOpenedFile(sFileToOpen);
				Settings.SaveSettings();
				ShowLastOpenedFiles();

				return true;
			}
			catch (Exception exp)
			{
				// if sFileName != null, then the user is stupid and we don't need to give any message
				if (sFileName == null) MessageBox.Show(exp.Message, "Error", MessageBoxButtons.AbortRetryIgnore,MessageBoxIcon.Error);
				return false;
			}
		}


		/// <summary>
		/// The name says everything :)
		/// </summary>
		/// <param name="sFullFileName">Full path of the file</param>
		/// <param name="sShortFileName">Short name - for displaying</param>
		/// <param name="sDocumentData">The document to load</param>
		/// <param name="bDoColors">Should we do syntax highlight?</param>
		void CreateNewTabAndPutThereTheDocument(string sFullFileName, string sShortFileName, string sDocumentData, bool bDoColors)
		{
			// Create new editor window.
			frmEditor frmNewEditorWindow = new frmEditor(new Assembler.compileErrorHandler(HandleError), this);
			frmNewEditorWindow.SetDragNDrop(new System.Windows.Forms.DragEventHandler(this.documentsManager_DragEnter), new System.Windows.Forms.DragEventHandler(this.documentsManager_DragDrop));

			DocumentManager.Document theDocument = new DocumentManager.Document(frmNewEditorWindow, sShortFileName);
			documentsManager.AddDocument(theDocument);
			documentsManager.FocusedDocument = theDocument;

			// Activate the new page, put the file content there, update its properties
			frmNewEditorWindow.bDoColor = bDoColors;
			frmNewEditorWindow.theDocument	= sDocumentData;
			frmNewEditorWindow.sFileName = sFullFileName;
			frmNewEditorWindow.ColorAllLines();

			// Event Hooking
			frmNewEditorWindow.OnPositionChange += new frmEditor.PositionEventFunc(OnDocumentPosition);
			
		}

		/// <summary>
		/// Preparing document to be close. After calling to this function,
		/// we need to remove the page tab from the documents list
		/// </summary>
		/// <param name="frmToClose">The document we wish to close</param>
		/// <returns>
		/// True - its ok to close document now. 
		/// False - document isn't ready to be close. caller function should halt the close process.
		/// </returns>
		private bool PrepareDocumentForClosing(frmEditor frmToClose)
		{
			// Need save? 
			if (frmToClose.bDocumentSaved == true) return true;

			// if find form is open for corrent document, close it.
			if (frmfind.Visible) frmfind.Hide();

			// Else, Ask to save the file
			DialogResult res = MessageBox.Show(
				"The text in the file has changed.\n\nDo you want to save the changes?",
				Settings.Environment.MESSAGEBOXS_TITLE, 
				MessageBoxButtons.YesNoCancel, System.Windows.Forms.MessageBoxIcon.Warning);

			if (res == DialogResult.Yes)
			{
				// Assuming: the active document is the one we save
				bool bSaved = InterfaceDoSave();
				if (!bSaved) return false;
			}
			else if (res == DialogResult.Cancel)
			{
				return false;
			}

			// Falls down to
			return true;
		}


		/// <summary>
		/// Save a document.
		/// </summary>
		/// <param name="frmToSave">The document to save</param>
		/// <returns>
		/// True - Saved Successfully
		/// False - Save Failed
		/// </returns>
		/// <remarks>Assuming sFileName is not empty string. 
		/// If it is not the case, you should use InterfaceDoSave 
		/// </remarks>
		bool DoSave(frmEditor frmToSave)
		{
			try
			{
				StreamWriter outputFile = new StreamWriter(frmToSave.sFileName, false, System.Text.Encoding.Default);
				string[] sFile = frmToSave.theDocument.Split(new char[] {'\n'});
				for (int i = 0; i < sFile.Length; ++i) 
					outputFile.WriteLine(sFile[i]);
				outputFile.Close();

				frmToSave.bDocumentSaved = true;

				StatusBarMessage("Item Saved");

				return true;
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message, "Error", MessageBoxButtons.AbortRetryIgnore,MessageBoxIcon.Error);
				return false;
			}
		}


		/// <summary>
		/// Interface Save - Actions to do when the user press on "Save" button
		/// </summary>
		/// <returns>Success or fail</returns>
		bool InterfaceDoSave()
		{
			try
			{
				frmEditor activeDoc = GetActiveDocument(); // Find the current docuemnt
				if (activeDoc.sFileName == "") return InterfaceDoSaveAs(); // Set file name for the saving process.
				return DoSave(activeDoc);
			}
			catch (NoActivePageException) { return false; }
		}


		/// <summary>
		/// Interface Save As - Actions to do when the user press on "Save As" button
		/// </summary>
		/// <returns>Success or fail</returns>
		bool InterfaceDoSaveAs()
		{
			try
			{

				// Find the current docuemnt
				frmEditor activeDoc = GetActiveDocument();

				// Set default file name for the saving process.
				// Might need change if we will mark unsaved pages with *
				saveFileDialog1.FileName = activeDoc.sFileName != "" ? activeDoc.sFileName : GetActiveDocumentTitle();
				saveFileDialog1.Filter = Settings.Environment.FILE_FILTERS;

				// ask for file name
				DialogResult res = saveFileDialog1.ShowDialog();
				if (res != DialogResult.OK) return false;

				// set the filename for the document
				activeDoc.sFileName = saveFileDialog1.FileName;
				SetActiveDocumentTitle(saveFileDialog1.FileName.Substring(
					saveFileDialog1.FileName.LastIndexOf(@"\") + 1));

				// saving process
				if (DoSave(activeDoc))
				{
					// Save the file path
					Settings.Environment.sLastFileLocation = saveFileDialog1.FileName;
					UpdateSettingsWithNewOpenedFile(saveFileDialog1.FileName);
					Settings.SaveSettings();
					ShowLastOpenedFiles();
					UpdateDocumentsContextMenu();

					return true;
				}
				else return false;
			}
			catch (NoActivePageException)
			{
				return false;
			}
		}


		/// <summary>
		/// Close the active document
		/// </summary>
		/// <param name="documentTabToClose">Document to close. null if we want to close the active window</param>
		/// <param name="editorToClose">Editor to close. null to close the active window</param>
		/// <exception cref="PanicException">If one of the parameters is null and the second is not we throw PanicException</exception>
		/// <returns>Success or fail</returns>
		bool InterfaceDoClose(frmEditor editorToClose, DocumentManager.Document documentTabToClose)
		{
			if ( (editorToClose == null && documentTabToClose != null) || (editorToClose != null && documentTabToClose == null) )
				throw new PanicException();
			try
			{
				// Find the current docuemnt
				frmEditor activeDoc = editorToClose == null ? GetActiveDocument() : editorToClose;

				// stop update error lines
				activeDoc.NewErrorsLines -= new frmEditor.ChangingLinesPositions(updateTasksLinesNumber);

				// Prepare document for closing
				if (!PrepareDocumentForClosing(activeDoc)) return false;

				// if in debugmode stop the program
				if (debugProgram != null && debugEditorWindow == activeDoc)
				{
					InterfaceDoRestartDebuggedProgram("Exit debug mode because editor had been closed");
					activeDoc.iBkGroundLine = -1;
				}

				// Stop listen to events. Must for the statusbar to work properly
				activeDoc.OnPositionChange -= new frmEditor.PositionEventFunc(OnDocumentPosition);
				ClearDocumentInfoStatusBar();

				if (documentTabToClose == null) RemoveActiveDocument();
				else documentsManager.RemoveDocument(documentTabToClose);
				if (documentsManager.FocusedDocument == null) ClearDocumentInfoStatusBar();

				UpdateDocumentsContextMenu();

				return true;
			}
			catch (NoActivePageException) { return false; }
		}



		/// <summary>
		/// Close all open documents
		/// </summary>
		/// <returns>Success or Fail</returns>
		private bool CloseAllDocuments()
		{
			while (InterfaceDoClose(null, null));
			try { GetActiveDocument(); }
			catch (NoActivePageException) { return true; }
			return false;
		}

		/// <summary>
		/// Do all the things need to be done when the user press on "Open" button.
		/// </summary>
		/// <param name="sFileName">File Name, should be always null,
		/// unless we know the file name the user wants to open</param>
		/// <returns>success or failure</returns>
		private bool InterfaceDoOpen(string sFileName)
		{
			// If user wants to open a document when he just started to work,
			// we first close the default blank document, and only then open his document.
			bool bCloseFirstDocument = false;
			try
			{
				frmEditor activeDoc = GetActiveDocument();
				if (docCount == 1 && activeDoc.sFileName == "" && activeDoc.bDocumentSaved == true)
					bCloseFirstDocument = true;
			}
			catch (NoActivePageException) { }

                        


			// Now start the actual opening process.
			if (DoOpen(sFileName))
			{
				if (bCloseFirstDocument) documentsManager.TabStrips[0].Documents.Remove(documentsManager.TabStrips[0].Documents[0]);
				GiveFocusToTheActiveDocument();
				OnDocumentPosition(new Point(1, 1));
				frmEditor activeDoc = GetActiveDocument();
				activeDoc.bDocumentSaved = true;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Interface Save All - Actions to do when the user press on "Save All" button
		/// </summary>
		void InterfaceDoSaveAll()
		{
			if (documentsManager.TabStrips.Count == 0) return;


			foreach (DocumentManager.MdiTabStrip m in documentsManager.TabStrips)
			{
				foreach (DocumentManager.Document d in m.Documents)
				{
					frmEditor theDoc = (frmEditor)d.Control;
					if (theDoc.sFileName == "")
					{
						documentsManager.FocusedDocument = d;
						InterfaceDoSaveAs();
					}
					else DoSave(theDoc);
				}
			}
		}

		/// <summary>
		/// Display the recent files on the file menu
		/// </summary>
		void ShowLastOpenedFiles()
		{

			for (int iCounter = 0; iCounter < Settings.Environment.a_sLastFiles.Length;
				++iCounter) if (Settings.Environment.a_sLastFiles[iCounter] != "")
								mnuLastFilesSeperator.Visible = true;

			if (Settings.Environment.a_sLastFiles[0] != "")
			{
				mnuFileLastFile1.Visible = true;
				mnuFileLastFile1.Text = Settings.Environment.a_sLastFiles[0];
			}
			if (Settings.Environment.a_sLastFiles[1] != "")
			{
				mnuFileLastFile2.Visible = true;
				mnuFileLastFile2.Text = Settings.Environment.a_sLastFiles[1];
			}
			if (Settings.Environment.a_sLastFiles[2] != "")
			{
				mnuFileLastFile3.Visible = true;
				mnuFileLastFile3.Text = Settings.Environment.a_sLastFiles[2];
			}
			if (Settings.Environment.a_sLastFiles[3] != "")
			{
				mnuFileLastFile4.Visible = true;
				mnuFileLastFile4.Text = Settings.Environment.a_sLastFiles[3];
			}

		}

		/// <summary>
		/// Update last open files list. doesn't save settings
		/// </summary>
		/// <param name="sOpenedFile">full file name of the new file</param>
		void UpdateSettingsWithNewOpenedFile(string sOpenedFile)
		{
			for (int iCounter = 0; iCounter < Settings.Environment.a_sLastFiles.Length;
				++iCounter)
			{
				if (Settings.Environment.a_sLastFiles[iCounter] == sOpenedFile)
				{
					if (iCounter == 0) return;
					for (int jCounter = iCounter; jCounter > 0; --jCounter)
						Settings.Environment.a_sLastFiles[jCounter] = Settings.Environment.a_sLastFiles[jCounter - 1];
					Settings.Environment.a_sLastFiles[0] = sOpenedFile;
					return;
				}
			}
			
			// If we got here, we didn't open the file before
			for (int iCounter = Settings.Environment.a_sLastFiles.Length - 1; iCounter > 0; --iCounter)
				Settings.Environment.a_sLastFiles[iCounter] = Settings.Environment.a_sLastFiles[iCounter - 1];
			Settings.Environment.a_sLastFiles[0] = sOpenedFile;
		}

		#endregion

		#region Interface - Helping Function

		/// <summary>
		/// Update StatusBar message
		/// </summary>
		/// <remarks>Not a general function. Will need update if we update the status bar</remarks>
		/// <param name="message"></param>
		private void StatusBarMessage(string message)
		{
			statusBar1.Panels[0].Text = " " + message;
		}

		/// <summary>
		/// The name says everything
		/// </summary>
		public void GiveFocusToTheActiveDocument()
		{
			documentsManager.Focus();
			if (documentsManager.FocusedDocument != null)
			{
				documentsManager.FocusedDocument.Control.Focus();
				((frmEditor)documentsManager.FocusedDocument.Control).FocusDocument();
			}
			else if (documentsManager.TabStrips.Count > 0)
			{
				documentsManager.TabStrips[0].SelectedDocument = documentsManager.TabStrips[0].Documents[0];
				documentsManager.FocusedDocument = documentsManager.TabStrips[0].Documents[0];
				//ldocumentsManager.FocusedDocument.Control.Focus();
//				((frmEditor)documentsManager.FocusedDocument.Control).FocusDocument();
			}
		}

		/// <summary>
		/// Update controls when the window size is changing.
		/// Update the status bar looks. One of the most
		/// ungeneral function I had ever seen. Me don't care.
		/// Hope you will enjoy all the magic numbers :-)
		/// </summary>
		private void ArrangeControls()
		{
			statusBar1.Panels[0].Width = Math.Max(statusBar1.Panels[0].MinWidth,statusBar1.Width - 140);
			statusBar1.Panels[1].Width = 140;

		}


		/// <summary>Clears document info form status bar</summary>
		/// <remarks>Not a general function</remarks>
		private void ClearDocumentInfoStatusBar()
		{
			statusBar1.Panels[1].Text = "";
		}

		/// <summary>
		/// Do all actions needed to handle "Paste" command
		/// </summary>
		private void InterfaceDoPaste()
		{
			try { GetActiveDocument().Paste(); }
			catch (NoActivePageException) { return; }
		}

		/// <summary>
		/// Do all actions needed to handle "Cut" command
		/// </summary>
		public void InterfaceDoCut()
		{
			try { GetActiveDocument().Cut(); }
			catch (NoActivePageException) { return; }	
		}

		/// <summary>
		/// Do all actions needed to handle "Delete" command
		/// </summary>
		public void InterfaceDoDelete()
		{
			try { GetActiveDocument().Delete(); }
			catch (NoActivePageException) { return; }	
		}
		/// <summary>
		/// Creates new file, focus on it and update all display elements.
		/// </summary>
		private void InterfaceNewFile()
		{
			CreateNewDocument();
			GiveFocusToTheActiveDocument();
			StatusBarMessage("Ready");
			OnDocumentPosition(new Point(1, 1));
		}

		/// <summary>
		/// Do all actions needed to handle "Copy" command
		/// </summary>
		private void InterfaceDoCopy()
		{
			try { GetActiveDocument().Copy(); }
			catch (NoActivePageException) { return; }
		}

		/// <summary>
		/// Print the active document
		/// </summary>
		private void InterfaceDoPrint()
		{
			if (printDialog1.ShowDialog() == DialogResult.Cancel) return;
			printDocument.Print();

		}

		/// <summary>
		/// Undo Command - We made support for it but didn't implement it as the project was over
		/// </summary>
		private void InterfaceDoUndo()
		{
			try { GetActiveDocument().Undo(); }
			catch (NoActivePageException) { return ; }
		}

		/// <summary>
		/// Redo Command - We made support for it but didn't implement it as the project was over
		/// </summary>
		private void InterfaceDoRedo()
		{
			try { GetActiveDocument().Redo(); }
			catch (NoActivePageException) { return ; }
		}

		/// <summary>
		/// Open "Goto Line" window.
		/// </summary>
		private void InterfaceGoTo()
		{
			try 
			{
				frmGoto frmgoto = new frmGoto();
				frmgoto.LastLine = GetActiveDocument().LastLine;
				frmgoto.CurrentLine =  iCurrentLine;
				frmgoto.ShowDialog();
				if (!frmgoto.OK) return;
				GetActiveDocument().MoveCaretToLine (frmgoto.CurrentLine, true);
			}
			catch (NoActivePageException) { return ; }
		}

		/// <summary>
		/// Open "Find" Window
		/// </summary>
		private void InterfaceFind()
		{
			if (frmfind.Visible) 
				frmfind.Focus();
			frmfind.Show();
		}

		// Implement "Find Next" Command
		private void InterfaceFindNext()
		{
			if (frmfind.FindText == "") InterfaceFind();
			else
				bFindNextClick (this, null);
		}

		/// <summary>
		/// Prepare the environment menus and interface
		/// </summary>
		private void InterfacePrepareEnvironment(enmEnvironmentMode mode)
		{
			curRunningMode = mode;

			bool bInDebug = mode == enmEnvironmentMode.DebugMode ? true : false;
            
			mnuViewMemory.Enabled = bInDebug;
			mnuViewRegisters.Enabled = bInDebug;
			mnuViewStack.Enabled = bInDebug;
			mnuViewTaskList.Enabled = !bInDebug;
			mnuViewOutput.Enabled = !bInDebug;

			bool bHasOpenDoc = false;
			try { GetActiveDocument(); bHasOpenDoc = true; } catch { }
			mnuFileClose.Enabled = bHasOpenDoc;
			mnuCloseAll.Enabled = bHasOpenDoc;
			mnuFileSave.Enabled = bHasOpenDoc;
			mnuFileSaveAll.Enabled = bHasOpenDoc;
			mnuFileSaveAs.Enabled = bHasOpenDoc;
			mnuFilePrintPreview.Enabled = bHasOpenDoc;
			mnuFilePrintPrintCode.Enabled = bHasOpenDoc;

			mnuEditCopy.Enabled = bHasOpenDoc;
			mnuEditCut.Enabled = bHasOpenDoc;
			mnuEditDelete.Enabled = bHasOpenDoc;
			mnuEditPaste.Enabled = bHasOpenDoc;
			mnuEditSelectAll.Enabled = bHasOpenDoc;
			mnuEditFind.Enabled = bHasOpenDoc;
			mnuEditFindNext.Enabled = bHasOpenDoc;
			mnuGoTo.Enabled = bHasOpenDoc;

			mnuDebugBreakPoint.Enabled = bHasOpenDoc;
			mnuDebugRemoveAllBreakPoints.Enabled = bHasOpenDoc;
			mnuDebugRestartProgram.Enabled = bHasOpenDoc;
			mnuDebugRun.Enabled = bHasOpenDoc;
			mnuDebugStepInto.Enabled = bHasOpenDoc;

			mnuBuildCompile.Enabled = bHasOpenDoc;
			mnuBuildInputOutputFiles.Enabled = bHasOpenDoc;
			mnuBuildRun.Enabled = bHasOpenDoc;
			mnuBuildViewLSTFile.Enabled = bHasOpenDoc;

			mnuHelpGetHelp.Enabled = bHasOpenDoc;

			tbButtonBreakPoint.Enabled = bHasOpenDoc;
			tbButtonExecute.Enabled = bHasOpenDoc;
			tbButtonCompile.Enabled = bHasOpenDoc;
			tbButtonRestartDebug.Enabled = bHasOpenDoc;
			tbButtonInOut.Enabled = bHasOpenDoc;
			tbButtonStep.Enabled = bHasOpenDoc;
			tbButtonRunDebug.Enabled = bHasOpenDoc;

			tbButtonCopy.Enabled = bHasOpenDoc;
			tbButtonCut.Enabled = bHasOpenDoc;
			tbButtonPaste.Enabled = bHasOpenDoc;
			tbButtonFind.Enabled = bHasOpenDoc;

			tbButtonPrint.Enabled = bHasOpenDoc;
			tbButtonSave.Enabled = bHasOpenDoc;
			tbButtonSaveAll.Enabled = bHasOpenDoc;

			
			
		}


		#endregion

		#region Compiler's Related Functions

		/// <summary>
		/// Adds an error to the task list and to the output list.
		/// If msg is CLEAR_TASK_LIST, it will clear the task list
		/// </summary>
		/// <param name="iLine">Line number for the message</param>
		/// <param name="msg">Object contains the message information</param>
		void HandleError(int iLine, CompilerMessage msg)
		{
			if (msg == CompilerMessage.CLEAR_TASK_LIST) { ClearTasks(); return; }

			AddTask(frmTaskList.Icons.Error, 
				CompilerMessagesStrings.GetMessageText(msg), iLine);

			AppendToOutputWindow(
				string.Format("Error({0}): ", iLine) + CompilerMessagesStrings.GetMessageText(msg));
		}
		

		/// <summary>
		/// The function add new tab with LST file for the active document
		/// </summary>
		/// <param name="bDoCompile">true if the function should compile the code before creating the LST</param>
		private void InterfaceShowLSTFile(bool bDoCompile)
		{
			try
			{
				// Compile the active document
				if (bDoCompile) InterfaceDoCompile(false);

				// Find the active docuemnt
				frmEditor theDoc = GetActiveDocument();

				// If the document needs compile, don't show the LST file.
				if (/*theDoc.bNeedCompile ||*/ theDoc.theDocument == "")
				{
					MessageBox.Show
						("No LST file available for this file. Please compile it and try again",
						Settings.Environment.MESSAGEBOXS_TITLE);
					return;
				}

				// Set file name
				string sFileName = theDoc.sFileName;
				if (sFileName == "") sFileName = GetActiveDocumentTitle();
				sFileName = sFileName.Substring(0, sFileName.LastIndexOf(".")) + ".lst";

				// Create short file name
				string sShortFileName = (sFileName.LastIndexOf(@"\") == -1) ? 
					sFileName : sFileName.Substring(sFileName.LastIndexOf(@"\")+1);

				string sDoc = theDoc.Assembler.GetLSTFile();

				CreateNewTabAndPutThereTheDocument(sFileName, sShortFileName, sDoc, false);

			}

				// No active document
			catch (NoActivePageException)
			{
				MessageBox.Show("No Active Document!", Settings.Environment.MESSAGEBOXS_TITLE);
				return ;
			}
		}


		
		/// <summary>
		/// Compile The Active Document
		/// </summary>
		public void InterfaceDoCompile(bool bShowLSTFileIfNeeded)
		{
			ClearTasks();
			ClearOutputWindow();

			frmEditor theDoc = null;

			try
			{
				// Find the current docuemnt
				theDoc = GetActiveDocument();
				theDoc.OutputText = "";

				AppendToOutputWindow(
					string.Format("------ Build started: File Name: {0} ------\r\n", 
					GetActiveDocumentTitle()));

				AppendToOutputWindow("Performing main compilation...\r\n");


				// Do compile
				ProgramBlock progrie = theDoc.Assembler.CompileCode(theDoc.theDocument);

				theDoc.bNeedCompile = false;
				
				// Show LST File
				if (Settings.Environment.OpenLSTFileAfterCompile && bShowLSTFileIfNeeded)
					InterfaceShowLSTFile(false);


				// Need to save lst file?
				if (Settings.Assembler.bSaveLSTFileAfterCompile)
				{
					// Set file name
					string sFileName = theDoc.sFileName;
					if (sFileName == "") sFileName = GetActiveDocumentTitle();
					sFileName = sFileName.Replace(".asm", ".lst");

					StreamWriter outputLSTFile = new StreamWriter(sFileName, true);

					string[] sLstFile = theDoc.Assembler.GetLSTFile().Split(new char[] {'\n'});
					for (int i = 0; i < sLstFile.Length; ++i) 
						outputLSTFile.WriteLine(sLstFile[i]);

					outputLSTFile.Close();
				}
			}
			catch (CompileError)
			{
				theDoc.NewErrorsLines += new frmEditor.ChangingLinesPositions(updateTasksLinesNumber);
			}
				// No active document
			catch (NoActivePageException)
			{
				return ;
			}

			int iNumberOfErrors = theDoc.Assembler.NumberOfErrors;
			AppendToOutputWindow(string.Format("\r\nBuild complete -- {0} errors",iNumberOfErrors));
			AppendToOutputWindow("\r\n\r\n---------------------- Done ----------------------");
			if (iNumberOfErrors == 0) 
			{
				AppendToOutputWindow("Build succeeded");
				StatusBarMessage("Build succeeded");
			}
			else 
			{
				AppendToOutputWindow("Build failed");
				StatusBarMessage("Build failed");
				MessageBox.Show("Compile Failed: There were build errors.", 
					Settings.Environment.MESSAGEBOXS_TITLE,MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation);
			}

			theDoc.OutputText = GetOutputWindowContent();
		}


		#endregion

		#region Printing
		private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			try { GetActiveDocument().PrintPage(sender, e); }
			catch (NoActivePageException) { }
		}

		private void printDocument_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			try { GetActiveDocument().BeginPrint(sender, e); }
			catch (NoActivePageException) { }
		}

		private void printDocument_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			try { GetActiveDocument().EndPrint(sender, e); }
			catch (NoActivePageException) { }
		}

		#endregion

		#region Simulator

		void InterfaceDoExecute(bool bDebugMode, bool bStepByStep)
		{
			try
			{
				// Find the current docuemnt
				frmEditor theDoc = GetActiveDocument();

				// Empty file? We don't really want to run it.
				// Lets the user see what we think
				if (theDoc.theDocument == "") { InterfaceDoCompile(true); return; }

				// If the document needs compiling, do it
				if (theDoc.bNeedCompile) InterfaceDoCompile(true);

				// If there were compile errors, don't run anything
				if (theDoc.bNeedCompile) return;

				// Create new program
				Program theProg = new Program(theDoc.Assembler.ProgramMachineCode, bDebugMode, this, theDoc.sInputFileName, theDoc.sOutputFileName);

				// Debug mode stuff
				if (bDebugMode)
				{
					InterfacePrepareEnvironment(enmEnvironmentMode.DebugMode);

					AppendToOutputWindow("\r\nDebug started for " + GetActiveDocumentTitle());
					theDoc.bList.GetFirst(); 
					theDoc.OnStopDebug += new frmEditor.StopDebug(InterfaceDoRestartDebuggedProgram);
					while (theDoc.bList.GetNext()) 
					{
						try 
						{
							if (theDoc.bList.GetCurrentPC() == -1)
								theProg.AddBreakPoint(theDoc.bList.GetCurrentLine(), 
									GetActiveDocument().Assembler.Lineslocations.GetStartingAddress(theDoc.bList.GetCurrentLine()));
							else
                                theProg.AddBreakPoint(theDoc.bList.GetCurrentLine(), theDoc.bList.GetCurrentPC()); 
						}
						catch {};
					}
					theProg.bStepByStep = bStepByStep;
					theProg.OnBreakPoint += new Program.BreakPointHandler(_BreakPointHandler);
					debugProgram = theProg;
					debugEditorWindow = theDoc;

					// Hide useless windows
					dockHostDown.Controls.Remove(dockPanelOutputTaskList);

					// Show Registers window if hidden
					dockControlRegisters.EnsureVisible(dockHostRegisters);
					cntRegistersView.SetRegisersSource(theProg.sim.R);

					// Show Memory
					dockControlMemory.EnsureVisible(dockHostDown);
					cntMemoryView.ResetView();
					cntMemoryView.SetMemoryObject(theProg.sim.memory);
					cntMemoryView.DrawMemory(0);

					// Show Stack
					dockPanelStack.DockedWidth = 150; // hard-coded!! :)
					dockControlStack.EnsureVisible(dockHostDown);
					cntStackView.SetDataObjects(theProg.sim.memory, theProg.sim.R);
					cntStackView.UpdateStackDisplay();

					// Connect Registers display to the debugged application
					theProg.frmRegistersOutput	= cntRegistersView;
					theProg.frmMemoryOutput		= cntMemoryView;
					theProg.frmStackOutput		= cntStackView;

					// Should the console be always on top?
					theProg.sim.Console.TopMost = Settings.Simulator.bConsoleAlwaysOnTopOnDebug;
				}

				// We're ready. Show the console
				theProg.ShowConsole();

				// Start the program as new thread
				Thread prog = new Thread(new ThreadStart(theProg.Run));
				prog.Name = GetActiveDocumentTitle() + " Program";
				if (bDebugMode) debugThread = prog;
				prog.Start();
				ProgramPool.Add (prog);
				//TODO: Del Me: if (!bDebugMode) theProg = null;
			}
				// No active document
			catch (NoActivePageException)
			{
				MessageBox.Show ("Can't execute program - there is no active program.", Settings.Environment.MESSAGEBOXS_TITLE);
				return ;
			}
		}

		#endregion

		#region Menus

			#region File Menu

		
			#region Files Shortcuts

		private void mnuFileLastFile1_Click(object sender, System.EventArgs e)
		{
			InterfaceDoOpen(mnuFileLastFile1.Text);
		}

		private void mnuFileLastFile2_Click(object sender, System.EventArgs e)
		{
			InterfaceDoOpen(mnuFileLastFile2.Text);
		}

		private void mnuFileLastFile3_Click(object sender, System.EventArgs e)
		{
			InterfaceDoOpen(mnuFileLastFile3.Text);
		}

		private void mnuFileLastFile4_Click(object sender, System.EventArgs e)
		{
			InterfaceDoOpen(mnuFileLastFile4.Text);
		}

		#endregion


		private void mnuFileOpen_Click(object sender, System.EventArgs e)
		{
			InterfaceDoOpen(null);
			InterfacePrepareEnvironment(curRunningMode);
		}

		private void mnuFileNew_Click(object sender, System.EventArgs e)
		{
			InterfaceNewFile();
			InterfacePrepareEnvironment(curRunningMode);
		}

		private void mnuFileClose_Click(object sender, System.EventArgs e)
		{
			InterfaceDoClose(null, null);
			InterfacePrepareEnvironment(curRunningMode);
		}

		private void mnuFileSaveAs_Click(object sender, System.EventArgs e)
		{
			InterfaceDoSaveAs();
		}

		private void mnuFileSave_Click(object sender, System.EventArgs e)
		{
			InterfaceDoSave();
		}
		
		private void mnuFilePrintPrintCode_Click(object sender, System.EventArgs e)
		{
			InterfaceDoPrint();
		}

		private void mnuCloseAll_Click(object sender, System.EventArgs e)
		{
			CloseAllDocuments();
			InterfacePrepareEnvironment(curRunningMode);
		}

		private void mnuFileExit_Click(object sender, System.EventArgs e)
		{
			if (CloseAllDocuments() == true) 
				//Application.Exit();
				OnFrmMainExit();
		}

		private void mnuFileSaveAll_Click(object sender, System.EventArgs e)
		{
			InterfaceDoSaveAll();
		}

		private void mnuFilePrintPreview_Click(object sender, System.EventArgs e)
		{
			printPreviewDialog1.ShowDialog();
		}


		#endregion

			#region Edit Menu

		
		private void mnuEditSelectAll_Click(object sender, System.EventArgs e)
		{
			InterfaceSelectAll();
		}

		private void mnuEditCut_Click(object sender, System.EventArgs e)
		{
			InterfaceDoCut();
		}

		private void mnuEditDelete_Click(object sender, System.EventArgs e)
		{
			InterfaceDoDelete();
		}
		private void mnuEditCopy_Click(object sender, System.EventArgs e)
		{
			InterfaceDoCopy();
		}

		private void mnuEditPaste_Click(object sender, System.EventArgs e)
		{
			InterfaceDoPaste();
		}
		
		private void mnuEditUndo_Click(object sender, System.EventArgs e)
		{
			InterfaceDoUndo();
		}

		private void mnuEditRedo_Click(object sender, System.EventArgs e)
		{
			InterfaceDoRedo();
		}

		private void mnuGoTo_Click(object sender, System.EventArgs e)
		{
			InterfaceGoTo();
		}

		private void mnuEditFind_Click(object sender, System.EventArgs e)
		{
			InterfaceFind();
		}
		private void mnuEditFindNext_Click(object sender, System.EventArgs e)
		{
			InterfaceFindNext();
		}
		#endregion

			#region View Menu
			
		private void mnuViewTaskList_Click(object sender, System.EventArgs e)
		{
			dockControlTaskList.EnsureVisible(dockHostDown);
		}

		private void mnuViewOutput_Click(object sender, System.EventArgs e)
		{
			dockControlOutput.EnsureVisible(dockHostDown);
		}

		private void mnuViewRegisters_Click(object sender, System.EventArgs e)
		{
			dockControlRegisters.EnsureVisible(dockHostRegisters);
		}

		
		private void mnuViewMemory_Click(object sender, System.EventArgs e)
		{
			dockControlMemory.EnsureVisible(dockHostDown);
		}

		private void mnuViewStack_Click(object sender, System.EventArgs e)
		{
			dockControlStack.EnsureVisible(dockHostDown);
		}



		#endregion

			#region Build Menu

		private void mnuBuildCompile_Click(object sender, System.EventArgs e)
		{
			InterfaceDoCompile(true);
		}
		
		private void mnuBuildRun_Click(object sender, System.EventArgs e)
		{
			InterfaceDoExecute(false, false);
		}

		private void mnuBuildViewLSTFile_Click(object sender, System.EventArgs e)
		{
			InterfaceShowLSTFile(true);
		}

		private void mnuBuildInputOutputFiles_Click(object sender, System.EventArgs e)
		{
			try
			{
				frmEditor theDoc = GetActiveDocument();
				frmInputOutputFiles frmInOut = new frmInputOutputFiles(theDoc.sInputFileName, theDoc.sOutputFileName);
				if (frmInOut.ShowDialog(this) == DialogResult.OK)
				{
					theDoc.sInputFileName = frmInOut.sInputFileName;
					theDoc.sOutputFileName = frmInOut.sOutputFileName;
				}
			}
			catch (NoActivePageException) {}
		}

		#endregion

			#region Tools Menu

		private void mnuToolsHex_Click(object sender, System.EventArgs e)
		{
			dockControlCalc.EnsureVisible(dockHostUp);
		}

		private void mnuToolsOptions_Click(object sender, System.EventArgs e)
		{
			frmOptions options = new frmOptions();
			if (options.ShowDialog() == DialogResult.OK)
			{
				if (options.ColorsSettingsChanged == true) ReColorAllDocuments();
				if (debugProgram != null) debugProgram.frmRegistersOutput.UpdateRegistersDisplay();

				cntRegistersView.CreateRegistersBag();
			}
		}

		#endregion

			#region Debug Menu

		
		private void mnuDebugRun_Click(object sender, System.EventArgs e)
		{
			InterfaceDoDebugRun();
		}

		private void mnuDebugStepInto_Click(object sender, System.EventArgs e)
		{
			InterfaceDoStep();
		}


		private void mnuDebugBreakPoint_Click(object sender, System.EventArgs e)
		{
			InterfaceDoBreakPoint();
		}

		private void mnuDebugRestartProgram_Click(object sender, System.EventArgs e)
		{
			InterfaceDoRestartDebuggedProgram("User stopped debugged application");
		}
		
		private void mnuDebugRunToCursor_Click(object sender, System.EventArgs e)
		{
		
		}

		private void mnuDebugRemoveAllBreakPoints_Click(object sender, System.EventArgs e)
		{
			InterfaceDoRemoveAllBreakPoints();
		}


		#endregion

		#endregion

		#region Form's Events


		private void frmMain_Resize(object sender, System.EventArgs e)
		{
			ArrangeControls();
		}


		private void frmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (CloseAllDocuments() == false) e.Cancel = true;
			else
			{
				// todo: try
				//Settings.Environment.EditModeLayout = sandBarManager1.GetLayout();
				Settings.bSettingSaved = false; // lame me
				OnFrmMainExit();
			}
		}

		
		private void documentsManager_CloseButtonPressed(object sender, DocumentManager.CloseButtonPressedEventArgs e)
		{
			InterfaceDoClose((frmEditor)e.TabStrip.SelectedDocument.Control, e.TabStrip.SelectedDocument);
		}
	


		private void bFindNextClick(object sender, System.EventArgs e)
		{
			if (!GetActiveDocument().FindPosition (frmfind.FindText, frmfind.FindUp, frmfind.FindCase))
				MessageBox.Show ("Cannot find \"" + frmfind.FindText + "\"", Settings.Environment.MESSAGEBOXS_TITLE);
			else
				frmfind.Hide();
			this.Focus();
		}

		#endregion

		#region Debug


		/// <summary>
		/// Handler to break-point events.
		/// </summary>
		/// <param name="theProgram">The program the event occured on</param>
		/// <param name="iPC">PC of the breakpoint</param>
		/// <param name="sMessage">Message to add to output window</param>
		void _BreakPointHandler(Program theProgram, int iPC, string sMessage)
		{

			try
			{
				frmEditor theDoc = debugEditorWindow;
				int iLineNum = theDoc.Assembler.GetLineFromPC(iPC);

				theDoc.ColorStep(iLineNum);
				if (sMessage != null) AppendToOutputWindow(sMessage);
			}
			catch (NoActivePageException) 
			{
				MessageBox.Show("Error: No Active Page on frmMain::_BreakPointHandler().", Settings.Environment.MESSAGEBOXS_TITLE);
				return;
			}
		}


		/// <summary>
		/// Do Run Command in debug mode
		/// </summary>
		public void InterfaceDoDebugRun()
		{
			if (debugProgram == null) InterfaceDoExecute(true, false);
			else 
			{
				GetActiveDocument().ClearStep();
				debugProgram.bStepByStep = false;
				debugProgram.BreakPointWait.Set();
			}
		}

		/// <summary>
		/// Add BreakPoint to debug program
		/// </summary>
		public void InterfaceDoBreakPoint()
		{
			try
			{
				frmEditor theDoc = GetActiveDocument();
				if (theDoc.bList.Contains(iCurrentLine))
				{
					theDoc.DelBreakPoint(iCurrentLine);
					if (debugProgram != null)//the program is running now 
						debugProgram.DelBreakPoint(iCurrentLine);
				}
				else
				{
					theDoc.AddBreakPoint(iCurrentLine, -1); 
					if (debugProgram != null)//the program is running now 
					{
						try 
						{ 
							debugProgram.AddBreakPoint(iCurrentLine, 
								debugEditorWindow.Assembler.Lineslocations.GetStartingAddress(iCurrentLine)); 
						}
						catch {};
					}
					if (theDoc.bNeedCompile) 
						theDoc.bList.AddEntry(new BreakPointEntry(iCurrentLine, -1));
					else // the document compiled and have PC to line update
					{
						theDoc.bList.AddEntry(new BreakPointEntry(iCurrentLine, 
							theDoc.Assembler.Lineslocations.GetStartingAddress(iCurrentLine)));
					}
				}
			}
			catch (NoActivePageException)
			{
				return ;
			}
		}

		/// <summary>
		/// Do a single step
		/// </summary>
		public void InterfaceDoStep()
		{
			if (debugProgram == null) InterfaceDoExecute(true, true);
			else 
			{
				debugProgram.bStepByStep = true;
				debugProgram.BreakPointWait.Set();
			}
		}

		/// <summary>
		/// Restart the debugged application and bring the environment back into normal mode
		/// </summary>
		/// <param name="sExitMessage">Exit Message, will be append to output window. null for no message</param>
		public void InterfaceDoRestartDebuggedProgram(string sExitMessage)
		{
			InterfacePrepareEnvironment(enmEnvironmentMode.EditorMode);

			cntRegistersView.SetRegisersSource(new VAX11Simulator.Registers());

			// Hide Debug Windows
			dockControlRegisters.Close();
			dockControlMemory.Close();
			dockControlStack.Close();

			// Show if hidden
			dockHostDown.Controls.Add(dockPanelOutputTaskList);

			if (debugProgram != null)
			{
				if (sExitMessage != null) AppendToOutputWindow(sExitMessage);
				debugEditorWindow.OnStopDebug -= new frmEditor.StopDebug(InterfaceDoRestartDebuggedProgram);
				//removing the BackGround Coloring
				debugEditorWindow.ClearStep();

				// bug bug bug bug bug bug bug bug exitConsole(false);
				debugProgram.sim.Console.Dispose();

				debugThread.Abort();
				debugProgram = null;
				debugThread = null;
				debugEditorWindow = null;
			}
		}

		public void InterfaceDoRemoveAllBreakPoints()
		{
			try
			{
				frmEditor theDoc = GetActiveDocument();
				theDoc.ClearAllBreakPoints();
				theDoc.bList.ResetTable();
				if (debugProgram != null)//the program is running now 
				{
					try 
					{ 
						debugProgram.ClearAllBreakPoints();
					}
					catch {};
				}
			}
			catch (NoActivePageException)
			{
				return ;
			}
		}


		private void mnuDebugAddWatch_Click(object sender, System.EventArgs e)
		{
		
		}

		#endregion

		#region Help System

		private void mnuHelpAbout_Click(object sender, System.EventArgs e)
		{
			frmabout.ShowDialog();
			
		}

		private void mnuHelpContents_Click(object sender, System.EventArgs e)
		{
			if (!File.Exists(Application.StartupPath + @"\VAX11Simulator.chm"))
			{
				MessageBox.Show("Help file for the VAX11 Simulator doesn't exists. Please reinstall the simulator to use help."
					, Settings.Environment.MESSAGEBOXS_TITLE);
				return;
			}

			System.Windows.Forms.Help.ShowHelp(this, Application.StartupPath + @"\VAX11Simulator.chm");
		}

		private void mnuHelpGetStarted_Click(object sender, System.EventArgs e)
		{
			if (!File.Exists(Application.StartupPath + @"\VAX11Simulator.chm"))
			{
				MessageBox.Show("Help file for the VAX11 Simulator doesn't exists. Please reinstall the simulator to use help."
					, Settings.Environment.MESSAGEBOXS_TITLE);
				return;
			}

			System.Windows.Forms.Help.ShowHelp(this, Application.StartupPath + @"\VAX11Simulator.chm", "topic_GetStarted.htm");
		}

		
		private void mnuHelpGetHelp_Click(object sender, System.EventArgs e)
		{
			// Check that the help file exists
			if (!File.Exists(Application.StartupPath + @"\VAX11Simulator.chm"))
			{
				MessageBox.Show("Help file for the VAX11 Simulator doesn't exists. Please reinstall the simulator to use help."
					, Settings.Environment.MESSAGEBOXS_TITLE);
				return;
			}

			// Find the current word
			string sWord = "";
			try
			{
				// Find the current docuemnt
				frmEditor theDoc = GetActiveDocument();
				sWord = theDoc.GetCurrentWord().Trim();
			}
			catch (NoActivePageException)
			{ return ; 	}

			// Build the url for the help
			string sURL = "NO_HELP.htm";
			if (CodeProcessor.IsOpcode(sWord))
			{
				OpcodeEntry op = new OpcodeEntry(sWord);
				sURL = op.HelpURL + ".htm";
			}
			else if (CodeProcessor.IsDirective(sWord)) sURL = "dir_" + sWord.Substring(sWord.IndexOf(".")+1) + ".htm";
			else if (CodeProcessor.IsSystemCall(sWord)) sURL = "sys_" + sWord.Substring(sWord.IndexOf(".")+1) + ".htm";

			// Luanch the help file
			System.Windows.Forms.Help.ShowHelp(this, Application.StartupPath + @"\VAX11Simulator.chm", sURL);
		}

		#endregion

		#region Divil Shit

		#region Document Manager PopupMenu

		private void documentsManager_FocusedDocumentChanged(object sender, System.EventArgs e)
		{
			UpdateDocumentsContextMenu();
			InterfacePrepareEnvironment(curRunningMode);

			if (documentsManager.FocusedDocument != null) 
				SetOutputWindowContent(GetActiveDocument().OutputText);
		}

		private void mnuDocumentManagerSave_Click(object sender, System.EventArgs e)
		{
			InterfaceDoSave();
		}

		private void mnuDocumentManagerClose_Click(object sender, System.EventArgs e)
		{
			InterfaceDoClose(null, null);
		}


		private void UpdateDocumentsContextMenu()
		{
			if (documentsManager.FocusedDocument != null)
			{

				// TODO: Add this line if we back to sandbar:
				//this.menuBar1.SetSandBarMenu(this.documentsManager, this.mnuDocumentsManager);

				documentsManager.ContextMenu = mnuDocumentsManager;
				mnuDocumentManagerSave.Text = "&Save " + GetActiveDocumentTitle();
			}
			else
			{
				// TODO: Add this line if we back to sandbar:
				//this.menuBar1.SetSandBarMenu(this.documentsManager, this.EmptyContextMenuBar);
				documentsManager.ContextMenu = null;
			}
		}

		#endregion


		
		private void documentsManager_VisibleChanged(object sender, System.EventArgs e)
		{
			if (documentsManager.Visible == true) GiveFocusToTheActiveDocument();
		}

		private void documentsManager_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			// make sure they're actually dropping files (not text or anything else)
			if( e.Data.GetDataPresent(DataFormats.FileDrop, false) == true )
				// allow them to continue
				// (without this, the cursor stays a "NO" symbol
				e.Effect = DragDropEffects.All;
		}

		private void documentsManager_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			// transfer the filenames to a string array
			// (yes, everything to the left of the "=" can be put in the 
			// foreach loop in place of "files", but this is easier to understand.)
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			// loop through the string array, adding each filename to the ListBox
			foreach( string file in files )
			{
				InterfaceDoOpen(file);
			}

		}

		#endregion

		#region Agent

		
		/// <summary>
		/// Starts an agent the welcome the user
		/// </summary>
		void StartAgent()
		{
			if (Settings.Environment.bShowAgentOnStartup)
			{
				agent.Characters.Load("Merlin","merlin.acs");
				speaker=agent.Characters["Merlin"];
				speaker.Left = 760;
				speaker.Top = 350;
				speaker.Show (0);
				speaker.Speak("Welcome to VAX11 Simulator","");
				speaker.Play("Greet");
				System.Windows.Forms.Timer tmrAgent = new System.Windows.Forms.Timer();
				tmrAgent.Tick += new EventHandler(killAgent);
				tmrAgent.Interval = 5500;
				tmrAgent.Enabled = true;
			}
		}

		/// <summary>
		/// Kill the stupid agent that appear on the start
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void killAgent(object sender, System.EventArgs e)
		{
			if (speaker.Visible == true)
			{
				speaker.Hide(0);
				((System.Windows.Forms.Timer)sender).Interval = 2000;
			}
			else
			{
				agent.Dispose();
				((System.Windows.Forms.Timer)sender).Dispose();
			}
		}

		#endregion




		#region SplashScreen

		/// <summary>
		/// Delegate for po
		/// </summary>
		public delegate void ExitFromProgram();

		/// <summary>
		/// Occurs when caret's position is changing
		/// </summary>
		public event ExitFromProgram OnFrmMainExit;

		#endregion

	}
}
