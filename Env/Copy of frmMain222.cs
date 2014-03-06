using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Crownwood.Magic.Common;
using Crownwood.Magic.Docking;
using Crownwood.Magic.Controls;
using Microsoft.Win32;

using VAX11Compiler;


namespace VAX11Simulator
{
	/// <summary>
	/// Vax11 Simulator Main Class.
	/// </summary>
	public class frmMain : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem mnuFile;
		private System.Windows.Forms.MenuItem mnuFileNew;
		private System.Windows.Forms.MenuItem mnuFileOpen;
		private System.Windows.Forms.MenuItem mnuFileSave;
		private System.Windows.Forms.MenuItem mnuFilePrintPrintCode;
		private System.Windows.Forms.MenuItem mnuFilePrintPrintSymbolTable;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem mnuFileExit;
		private System.Windows.Forms.MenuItem mnuEdit;
		private System.Windows.Forms.MenuItem menuItem42;
		private System.Windows.Forms.MenuItem menuItem37;
		private System.Windows.Forms.MenuItem mnuBuild;
		private System.Windows.Forms.MenuItem mnuDebug;
		private System.Windows.Forms.MenuItem menuItem19;
		private System.Windows.Forms.MenuItem menuItem25;
		private System.Windows.Forms.MenuItem mnuView;
		private System.Windows.Forms.MenuItem mnuHelp;
		private System.Windows.Forms.MenuItem menuItem28;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem mnuEditUndo;
		private System.Windows.Forms.MenuItem mnuEditRedo;
		private System.Windows.Forms.MenuItem mnuEditCut;
		private System.Windows.Forms.MenuItem mnuEditCopy;
		private System.Windows.Forms.MenuItem mnuEditPaste;
		private System.Windows.Forms.MenuItem mnuEditDelete;
		private System.Windows.Forms.MenuItem mnuEditSelectAll;
		private System.Windows.Forms.MenuItem mnuEditFind;
		private System.Windows.Forms.MenuItem mnuEditReplace;
		private System.Windows.Forms.MenuItem mnuBuildCompile;
		private System.Windows.Forms.MenuItem mnuDebugRun;
		private System.Windows.Forms.MenuItem mnuDebugResetProgram;
		private System.Windows.Forms.MenuItem mnuDebugStepOver;
		private System.Windows.Forms.MenuItem mnuDebugStepInto;
		private System.Windows.Forms.MenuItem mnuDebugRunToCursor;
		private System.Windows.Forms.MenuItem mnuDebugWatch;
		private System.Windows.Forms.MenuItem mnuDebugAddWatch;
		private System.Windows.Forms.MenuItem mnuDebugDelWatch;
		private System.Windows.Forms.MenuItem mnuDebugBreakPoint;
		private System.Windows.Forms.MenuItem mnuViewRegisters;
		private System.Windows.Forms.MenuItem mnuViewStack;
		private System.Windows.Forms.MenuItem mnuViewWatchs;
		private System.Windows.Forms.MenuItem mnuViewMemory;
		private System.Windows.Forms.MenuItem mnuTools;
		private System.Windows.Forms.MenuItem mnuToolsHex;
		private System.Windows.Forms.MenuItem mnuToolsOptions;
		private System.Windows.Forms.MenuItem mnuHelpContents;
		private System.Windows.Forms.MenuItem mnuHelpAbout;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.StatusBarPanel sbpMessage;
		private System.Windows.Forms.StatusBarPanel sbpCurrentLocation;
		private System.Windows.Forms.ImageList imgListWindows;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.MenuItem mnuViewTaskList;
		private System.Windows.Forms.MenuItem mnuViewOutput;
		private System.Windows.Forms.MenuItem mnuFileSaveAs;
		private System.Windows.Forms.MenuItem mnuFileSaveTable;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem mnuFileClose;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;

		// Support for docking windows
		private readonly Crownwood.Magic.Docking.DockingManager 
			_dockingManager = null;

		// Refering to docking windows using the following variables
		private Crownwood.Magic.Docking.Content 
			cntCompileMessage = null;
		private Crownwood.Magic.Docking.Content 
			cntTaskList = null;

		// TabGroup variables
		private Crownwood.Magic.Controls.TabbedGroups tgDocuments;
		private System.Windows.Forms.PrintDialog printDialog1;
		private System.Drawing.Printing.PrintDocument printDocument;
		private System.Windows.Forms.MenuItem mnuCloseAll;
		private System.Windows.Forms.ImageList imgListtoolBar;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ToolBarButton toolBarNew;
		private System.Windows.Forms.ToolBarButton toolBarOpen;
		private System.Windows.Forms.ToolBarButton toolBarSave;
		private System.Windows.Forms.ToolBarButton toolBarSaveAll;
		private System.Windows.Forms.ToolBarButton toolBarSpace1;
		private System.Windows.Forms.ToolBarButton toolBarPrint;
		private System.Windows.Forms.ToolBarButton toolBarFind;
		private System.Windows.Forms.ToolBarButton toolBarSpace2;
		private System.Windows.Forms.ToolBarButton toolBarCut;
		private System.Windows.Forms.ToolBarButton toolBarCopy;
		private System.Windows.Forms.ToolBarButton toolBarPaste;
		private System.Windows.Forms.ToolBarButton toolBarSpace3;
		private System.Windows.Forms.ToolBarButton toolBarUndo;
		private System.Windows.Forms.ToolBarButton toolBarRedo;

		// For new documents numbering
		private static int docCount = 0;



		private VAX11Compiler.Assembler _assembler;

		private string sLastCompiledFile = "";

		/// <summary>
		/// Init Docking Windows, Listen to events
		/// </summary>
		public frmMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Create the object that manages the docking state
			_dockingManager = new DockingManager(this, VisualStyle.IDE);

			// Without the following line, docking windows might do really starnge things...
			// wooo... scary... :-)
			
			_dockingManager.OuterControl = statusBar1;
			_dockingManager.InnerControl = tgDocuments;

			//
			// Create new Contents objects
			//


			cntTaskList = _dockingManager.Contents.Add(new frmTaskList(), "Task List",
				imgListWindows, 2);
			cntCompileMessage = _dockingManager.Contents.Add(new frmCompileMessages(), "Output",
				imgListWindows, 1);
			
			WindowContent wc  = 
				_dockingManager.AddContentWithState(
				cntCompileMessage, State.DockBottom) as WindowContent;
			_dockingManager.AddContentToWindowContent(cntTaskList, wc);


			//
			// Create Documents Tab
			//

			CreateNewDocument();

			//
			// Listen to events
			//

			((frmTaskList)cntTaskList.Control).OnClickEvent += new frmTaskList.clickEventFunc(OnTaskListClick);

			// Prepare assembler
			_assembler = new Assembler();
			_assembler.OnCompileError += new Assembler.compileErrorHandler(HandleError);

		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmMain));
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.mnuFile = new System.Windows.Forms.MenuItem();
			this.mnuFileNew = new System.Windows.Forms.MenuItem();
			this.mnuFileOpen = new System.Windows.Forms.MenuItem();
			this.mnuFileClose = new System.Windows.Forms.MenuItem();
			this.mnuCloseAll = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.mnuFileSave = new System.Windows.Forms.MenuItem();
			this.mnuFileSaveAs = new System.Windows.Forms.MenuItem();
			this.mnuFileSaveTable = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.mnuFilePrintPrintCode = new System.Windows.Forms.MenuItem();
			this.mnuFilePrintPrintSymbolTable = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
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
			this.mnuEditReplace = new System.Windows.Forms.MenuItem();
			this.mnuBuild = new System.Windows.Forms.MenuItem();
			this.mnuBuildCompile = new System.Windows.Forms.MenuItem();
			this.mnuDebug = new System.Windows.Forms.MenuItem();
			this.mnuDebugRun = new System.Windows.Forms.MenuItem();
			this.mnuDebugResetProgram = new System.Windows.Forms.MenuItem();
			this.menuItem19 = new System.Windows.Forms.MenuItem();
			this.mnuDebugStepOver = new System.Windows.Forms.MenuItem();
			this.mnuDebugStepInto = new System.Windows.Forms.MenuItem();
			this.mnuDebugRunToCursor = new System.Windows.Forms.MenuItem();
			this.menuItem25 = new System.Windows.Forms.MenuItem();
			this.mnuDebugWatch = new System.Windows.Forms.MenuItem();
			this.mnuDebugAddWatch = new System.Windows.Forms.MenuItem();
			this.mnuDebugDelWatch = new System.Windows.Forms.MenuItem();
			this.mnuDebugBreakPoint = new System.Windows.Forms.MenuItem();
			this.mnuView = new System.Windows.Forms.MenuItem();
			this.mnuViewRegisters = new System.Windows.Forms.MenuItem();
			this.mnuViewStack = new System.Windows.Forms.MenuItem();
			this.mnuViewWatchs = new System.Windows.Forms.MenuItem();
			this.mnuViewMemory = new System.Windows.Forms.MenuItem();
			this.mnuViewTaskList = new System.Windows.Forms.MenuItem();
			this.mnuViewOutput = new System.Windows.Forms.MenuItem();
			this.mnuTools = new System.Windows.Forms.MenuItem();
			this.mnuToolsHex = new System.Windows.Forms.MenuItem();
			this.mnuToolsOptions = new System.Windows.Forms.MenuItem();
			this.mnuHelp = new System.Windows.Forms.MenuItem();
			this.mnuHelpContents = new System.Windows.Forms.MenuItem();
			this.menuItem28 = new System.Windows.Forms.MenuItem();
			this.mnuHelpAbout = new System.Windows.Forms.MenuItem();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.sbpMessage = new System.Windows.Forms.StatusBarPanel();
			this.sbpCurrentLocation = new System.Windows.Forms.StatusBarPanel();
			this.imgListWindows = new System.Windows.Forms.ImageList(this.components);
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.tgDocuments = new Crownwood.Magic.Controls.TabbedGroups();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.printDialog1 = new System.Windows.Forms.PrintDialog();
			this.printDocument = new System.Drawing.Printing.PrintDocument();
			this.imgListtoolBar = new System.Windows.Forms.ImageList(this.components);
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.toolBarNew = new System.Windows.Forms.ToolBarButton();
			this.toolBarOpen = new System.Windows.Forms.ToolBarButton();
			this.toolBarSave = new System.Windows.Forms.ToolBarButton();
			this.toolBarSaveAll = new System.Windows.Forms.ToolBarButton();
			this.toolBarSpace1 = new System.Windows.Forms.ToolBarButton();
			this.toolBarPrint = new System.Windows.Forms.ToolBarButton();
			this.toolBarFind = new System.Windows.Forms.ToolBarButton();
			this.toolBarSpace2 = new System.Windows.Forms.ToolBarButton();
			this.toolBarCut = new System.Windows.Forms.ToolBarButton();
			this.toolBarCopy = new System.Windows.Forms.ToolBarButton();
			this.toolBarPaste = new System.Windows.Forms.ToolBarButton();
			this.toolBarSpace3 = new System.Windows.Forms.ToolBarButton();
			this.toolBarUndo = new System.Windows.Forms.ToolBarButton();
			this.toolBarRedo = new System.Windows.Forms.ToolBarButton();
			((System.ComponentModel.ISupportInitialize)(this.sbpMessage)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpCurrentLocation)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tgDocuments)).BeginInit();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.mnuFile,
																					  this.mnuEdit,
																					  this.mnuBuild,
																					  this.mnuDebug,
																					  this.mnuView,
																					  this.mnuTools,
																					  this.mnuHelp});
			this.mainMenu1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// mnuFile
			// 
			this.mnuFile.Index = 0;
			this.mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuFileNew,
																					this.mnuFileOpen,
																					this.mnuFileClose,
																					this.mnuCloseAll,
																					this.menuItem3,
																					this.mnuFileSave,
																					this.mnuFileSaveAs,
																					this.mnuFileSaveTable,
																					this.menuItem4,
																					this.mnuFilePrintPrintCode,
																					this.mnuFilePrintPrintSymbolTable,
																					this.menuItem9,
																					this.mnuFileExit});
			this.mnuFile.Text = "&File";
			// 
			// mnuFileNew
			// 
			this.mnuFileNew.Index = 0;
			this.mnuFileNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.mnuFileNew.Text = "&New";
			this.mnuFileNew.Click += new System.EventHandler(this.mnuFileNew_Click);
			// 
			// mnuFileOpen
			// 
			this.mnuFileOpen.Index = 1;
			this.mnuFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.mnuFileOpen.Text = "&Open...";
			this.mnuFileOpen.Click += new System.EventHandler(this.mnuFileOpen_Click);
			// 
			// mnuFileClose
			// 
			this.mnuFileClose.Index = 2;
			this.mnuFileClose.Shortcut = System.Windows.Forms.Shortcut.CtrlW;
			this.mnuFileClose.ShowShortcut = false;
			this.mnuFileClose.Text = "&Close";
			this.mnuFileClose.Click += new System.EventHandler(this.mnuFileClose_Click);
			// 
			// mnuCloseAll
			// 
			this.mnuCloseAll.Index = 3;
			this.mnuCloseAll.Text = "Clos&e All";
			this.mnuCloseAll.Click += new System.EventHandler(this.mnuCloseAll_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 4;
			this.menuItem3.Text = "-";
			// 
			// mnuFileSave
			// 
			this.mnuFileSave.Index = 5;
			this.mnuFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.mnuFileSave.Text = "&Save";
			this.mnuFileSave.Click += new System.EventHandler(this.mnuFileSave_Click);
			// 
			// mnuFileSaveAs
			// 
			this.mnuFileSaveAs.Index = 6;
			this.mnuFileSaveAs.Text = "Save &As...";
			this.mnuFileSaveAs.Click += new System.EventHandler(this.mnuFileSaveAs_Click);
			// 
			// mnuFileSaveTable
			// 
			this.mnuFileSaveTable.Index = 7;
			this.mnuFileSaveTable.Text = "Sa&ve Symbol Table As...";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 8;
			this.menuItem4.Text = "-";
			// 
			// mnuFilePrintPrintCode
			// 
			this.mnuFilePrintPrintCode.Index = 9;
			this.mnuFilePrintPrintCode.Shortcut = System.Windows.Forms.Shortcut.CtrlP;
			this.mnuFilePrintPrintCode.Text = "&Print Code...";
			this.mnuFilePrintPrintCode.Click += new System.EventHandler(this.mnuFilePrintPrintCode_Click);
			// 
			// mnuFilePrintPrintSymbolTable
			// 
			this.mnuFilePrintPrintSymbolTable.Index = 10;
			this.mnuFilePrintPrintSymbolTable.Text = "Print Symbol &Table...";
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 11;
			this.menuItem9.Text = "-";
			// 
			// mnuFileExit
			// 
			this.mnuFileExit.Index = 12;
			this.mnuFileExit.Text = "E&xit";
			this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
			// 
			// mnuEdit
			// 
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
																					this.mnuEditReplace});
			this.mnuEdit.Text = "&Edit";
			// 
			// mnuEditUndo
			// 
			this.mnuEditUndo.Index = 0;
			this.mnuEditUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
			this.mnuEditUndo.Text = "&Undo";
			// 
			// mnuEditRedo
			// 
			this.mnuEditRedo.Index = 1;
			this.mnuEditRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
			this.mnuEditRedo.Text = "&Redo";
			// 
			// menuItem42
			// 
			this.menuItem42.Index = 2;
			this.menuItem42.Text = "-";
			// 
			// mnuEditCut
			// 
			this.mnuEditCut.Index = 3;
			this.mnuEditCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
			this.mnuEditCut.Text = "Cu&t";
			this.mnuEditCut.Click += new System.EventHandler(this.mnuEditCut_Click);
			// 
			// mnuEditCopy
			// 
			this.mnuEditCopy.Index = 4;
			this.mnuEditCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			this.mnuEditCopy.Text = "&Copy";
			this.mnuEditCopy.Click += new System.EventHandler(this.mnuEditCopy_Click);
			// 
			// mnuEditPaste
			// 
			this.mnuEditPaste.Index = 5;
			this.mnuEditPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
			this.mnuEditPaste.Text = "&Paste";
			this.mnuEditPaste.Click += new System.EventHandler(this.mnuEditPaste_Click);
			// 
			// mnuEditDelete
			// 
			this.mnuEditDelete.Index = 6;
			this.mnuEditDelete.Text = "&Delete";
			// 
			// menuItem37
			// 
			this.menuItem37.Index = 7;
			this.menuItem37.Text = "-";
			// 
			// mnuEditSelectAll
			// 
			this.mnuEditSelectAll.Index = 8;
			this.mnuEditSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
			this.mnuEditSelectAll.Text = "Select &All";
			this.mnuEditSelectAll.Click += new System.EventHandler(this.mnuEditSelectAll_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 9;
			this.menuItem1.Text = "-";
			// 
			// mnuEditFind
			// 
			this.mnuEditFind.Index = 10;
			this.mnuEditFind.Shortcut = System.Windows.Forms.Shortcut.F3;
			this.mnuEditFind.Text = "&Find...";
			// 
			// mnuEditReplace
			// 
			this.mnuEditReplace.Index = 11;
			this.mnuEditReplace.Shortcut = System.Windows.Forms.Shortcut.CtrlH;
			this.mnuEditReplace.Text = "&Replace...";
			// 
			// mnuBuild
			// 
			this.mnuBuild.Index = 2;
			this.mnuBuild.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnuBuildCompile});
			this.mnuBuild.Text = "&Build";
			// 
			// mnuBuildCompile
			// 
			this.mnuBuildCompile.Index = 0;
			this.mnuBuildCompile.Shortcut = System.Windows.Forms.Shortcut.CtrlF5;
			this.mnuBuildCompile.Text = "&Compile";
			this.mnuBuildCompile.Click += new System.EventHandler(this.mnuBuildCompile_Click);
			// 
			// mnuDebug
			// 
			this.mnuDebug.Index = 3;
			this.mnuDebug.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnuDebugRun,
																					 this.mnuDebugResetProgram,
																					 this.menuItem19,
																					 this.mnuDebugStepOver,
																					 this.mnuDebugStepInto,
																					 this.mnuDebugRunToCursor,
																					 this.menuItem25,
																					 this.mnuDebugWatch,
																					 this.mnuDebugBreakPoint});
			this.mnuDebug.Text = "&Debug";
			// 
			// mnuDebugRun
			// 
			this.mnuDebugRun.Index = 0;
			this.mnuDebugRun.Shortcut = System.Windows.Forms.Shortcut.F5;
			this.mnuDebugRun.Text = "&Run";
			// 
			// mnuDebugResetProgram
			// 
			this.mnuDebugResetProgram.Index = 1;
			this.mnuDebugResetProgram.Text = "Reset &Program";
			// 
			// menuItem19
			// 
			this.menuItem19.Index = 2;
			this.menuItem19.Text = "-";
			// 
			// mnuDebugStepOver
			// 
			this.mnuDebugStepOver.Index = 3;
			this.mnuDebugStepOver.Shortcut = System.Windows.Forms.Shortcut.F10;
			this.mnuDebugStepOver.Text = "Step &Over";
			// 
			// mnuDebugStepInto
			// 
			this.mnuDebugStepInto.Index = 4;
			this.mnuDebugStepInto.Shortcut = System.Windows.Forms.Shortcut.F11;
			this.mnuDebugStepInto.Text = "Step &Into";
			// 
			// mnuDebugRunToCursor
			// 
			this.mnuDebugRunToCursor.Index = 5;
			this.mnuDebugRunToCursor.Text = "Run &to Cursor";
			// 
			// menuItem25
			// 
			this.menuItem25.Index = 6;
			this.menuItem25.Text = "-";
			// 
			// mnuDebugWatch
			// 
			this.mnuDebugWatch.Index = 7;
			this.mnuDebugWatch.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						  this.mnuDebugAddWatch,
																						  this.mnuDebugDelWatch});
			this.mnuDebugWatch.Text = "&Watch";
			// 
			// mnuDebugAddWatch
			// 
			this.mnuDebugAddWatch.Index = 0;
			this.mnuDebugAddWatch.Shortcut = System.Windows.Forms.Shortcut.CtrlIns;
			this.mnuDebugAddWatch.Text = "&Add Watch";
			// 
			// mnuDebugDelWatch
			// 
			this.mnuDebugDelWatch.Index = 1;
			this.mnuDebugDelWatch.Text = "&Del Watch";
			// 
			// mnuDebugBreakPoint
			// 
			this.mnuDebugBreakPoint.Index = 8;
			this.mnuDebugBreakPoint.Shortcut = System.Windows.Forms.Shortcut.F9;
			this.mnuDebugBreakPoint.Text = "Add/Remove &Breakpoint";
			// 
			// mnuView
			// 
			this.mnuView.Index = 4;
			this.mnuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuViewRegisters,
																					this.mnuViewStack,
																					this.mnuViewWatchs,
																					this.mnuViewMemory,
																					this.mnuViewTaskList,
																					this.mnuViewOutput});
			this.mnuView.Text = "&View";
			// 
			// mnuViewRegisters
			// 
			this.mnuViewRegisters.Index = 0;
			this.mnuViewRegisters.Text = "&Registers";
			// 
			// mnuViewStack
			// 
			this.mnuViewStack.Index = 1;
			this.mnuViewStack.Text = "&Stacks";
			// 
			// mnuViewWatchs
			// 
			this.mnuViewWatchs.Index = 2;
			this.mnuViewWatchs.Text = "&Watchs";
			// 
			// mnuViewMemory
			// 
			this.mnuViewMemory.Index = 3;
			this.mnuViewMemory.Text = "M&emory";
			// 
			// mnuViewTaskList
			// 
			this.mnuViewTaskList.Index = 4;
			this.mnuViewTaskList.Text = "&Task List";
			this.mnuViewTaskList.Click += new System.EventHandler(this.mnuViewTaskList_Click);
			// 
			// mnuViewOutput
			// 
			this.mnuViewOutput.Index = 5;
			this.mnuViewOutput.Text = "&Output";
			this.mnuViewOutput.Click += new System.EventHandler(this.mnuViewOutput_Click);
			// 
			// mnuTools
			// 
			this.mnuTools.Index = 5;
			this.mnuTools.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.mnuToolsHex,
																					 this.mnuToolsOptions});
			this.mnuTools.Text = "&Tools";
			// 
			// mnuToolsHex
			// 
			this.mnuToolsHex.Index = 0;
			this.mnuToolsHex.Text = "&HEX Calculator...";
			// 
			// mnuToolsOptions
			// 
			this.mnuToolsOptions.Index = 1;
			this.mnuToolsOptions.Text = "&Options...";
			// 
			// mnuHelp
			// 
			this.mnuHelp.Index = 6;
			this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					this.mnuHelpContents,
																					this.menuItem28,
																					this.mnuHelpAbout});
			this.mnuHelp.Text = "&Help";
			// 
			// mnuHelpContents
			// 
			this.mnuHelpContents.Index = 0;
			this.mnuHelpContents.Shortcut = System.Windows.Forms.Shortcut.F1;
			this.mnuHelpContents.Text = "&Contents...";
			// 
			// menuItem28
			// 
			this.menuItem28.Index = 1;
			this.menuItem28.Text = "-";
			// 
			// mnuHelpAbout
			// 
			this.mnuHelpAbout.Index = 2;
			this.mnuHelpAbout.Text = "&About...";
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 395);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						  this.sbpMessage,
																						  this.sbpCurrentLocation});
			this.statusBar1.ShowPanels = true;
			this.statusBar1.Size = new System.Drawing.Size(688, 22);
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
			// tgDocuments
			// 
			this.tgDocuments.AllowDrop = true;
			this.tgDocuments.AtLeastOneLeaf = false;
			this.tgDocuments.CloseShortcut = System.Windows.Forms.Shortcut.CtrlF4;
			this.tgDocuments.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tgDocuments.Location = new System.Drawing.Point(0, 25);
			this.tgDocuments.Name = "tgDocuments";
			this.tgDocuments.ProminentLeaf = null;
			this.tgDocuments.ResizeBarColor = System.Drawing.SystemColors.Control;
			this.tgDocuments.Size = new System.Drawing.Size(688, 370);
			this.tgDocuments.TabIndex = 2;
			this.tgDocuments.PageCloseRequest += new Crownwood.Magic.Controls.TabbedGroups.PageCloseRequestHandler(this.tgDocuments_PageCloseRequest);
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.FileName = "doc1";
			// 
			// printDialog1
			// 
			this.printDialog1.Document = this.printDocument;
			// 
			// imgListtoolBar
			// 
			this.imgListtoolBar.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
			this.imgListtoolBar.ImageSize = new System.Drawing.Size(16, 16);
			this.imgListtoolBar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListtoolBar.ImageStream")));
			this.imgListtoolBar.TransparentColor = System.Drawing.Color.FromArgb(((System.Byte)(189)), ((System.Byte)(189)), ((System.Byte)(189)));
			// 
			// toolBar1
			// 
			this.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																						this.toolBarNew,
																						this.toolBarOpen,
																						this.toolBarSave,
																						this.toolBarSaveAll,
																						this.toolBarSpace1,
																						this.toolBarPrint,
																						this.toolBarFind,
																						this.toolBarSpace2,
																						this.toolBarCut,
																						this.toolBarCopy,
																						this.toolBarPaste,
																						this.toolBarSpace3,
																						this.toolBarUndo,
																						this.toolBarRedo});
			this.toolBar1.ButtonSize = new System.Drawing.Size(16, 16);
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.imgListtoolBar;
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(688, 25);
			this.toolBar1.TabIndex = 0;
			this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
			// 
			// toolBarNew
			// 
			this.toolBarNew.ImageIndex = 0;
			// 
			// toolBarOpen
			// 
			this.toolBarOpen.ImageIndex = 1;
			// 
			// toolBarSave
			// 
			this.toolBarSave.ImageIndex = 2;
			// 
			// toolBarSaveAll
			// 
			this.toolBarSaveAll.ImageIndex = 3;
			// 
			// toolBarSpace1
			// 
			this.toolBarSpace1.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarPrint
			// 
			this.toolBarPrint.ImageIndex = 4;
			// 
			// toolBarFind
			// 
			this.toolBarFind.ImageIndex = 5;
			// 
			// toolBarSpace2
			// 
			this.toolBarSpace2.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarCut
			// 
			this.toolBarCut.ImageIndex = 6;
			// 
			// toolBarCopy
			// 
			this.toolBarCopy.ImageIndex = 7;
			// 
			// toolBarPaste
			// 
			this.toolBarPaste.ImageIndex = 8;
			// 
			// toolBarSpace3
			// 
			this.toolBarSpace3.Style = System.Windows.Forms.ToolBarButtonStyle.Separator;
			// 
			// toolBarUndo
			// 
			this.toolBarUndo.ImageIndex = 9;
			// 
			// toolBarRedo
			// 
			this.toolBarRedo.ImageIndex = 10;
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(688, 417);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.tgDocuments,
																		  this.statusBar1,
																		  this.toolBar1});
			this.Menu = this.mainMenu1;
			this.MinimumSize = new System.Drawing.Size(480, 300);
			this.Name = "frmMain";
			this.Text = "VAX11 Simulator - Version 0.1";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Resize += new System.EventHandler(this.frmMain_Resize);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmMain_Closing);
			this.Load += new System.EventHandler(this.frmMain_Load);
			((System.ComponentModel.ISupportInitialize)(this.sbpMessage)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sbpCurrentLocation)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tgDocuments)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmMain());
		}


		private void mnuFileExit_Click(object sender, System.EventArgs e)
		{
			if (CloseAllDocuments() == true) Application.Exit();
		}

		// Pay attention that constructor is called before this function
		private void frmMain_Load(object sender, System.EventArgs e)
		{
			ArrangeControls();

			// TODO: Load settings
			

			// TODO: temp, remove me
			_dockingManager.HideAllContents();
			_dockingManager.ShowAllContents();


			GiveFocusToTheActiveDocument();
		}

		private void mnuFileOpen_Click(object sender, System.EventArgs e)
		{
			InterfaceDoOpen();
		}



		#region Output Window Interface
/***************************************************************************/
/*						Output Window Interface functions				   */
/***************************************************************************/

		/// <summary>
		/// Clears the output window
		/// </summary>
		public void ClearOutputWindow()
		{
			((frmCompileMessages)cntCompileMessage.Control).Output = "";
		}

		/// <summary>
		/// Append text to output window
		/// </summary>
		/// <param name="sOutput">The text to append</param>
		public void AppendToOutputWindow(string sOutput)
		{
			((frmCompileMessages)cntCompileMessage.Control).Output += 
				sOutput + "\r\n";
		}

		#endregion

		#region Tasks List Interface

		/// <summary>
		/// Interface function to add tasks to the Task List
		/// </summary>
		/// <param name="taskType">Task type - error / warning</param>
		/// <param name="TaskMessage">Message to display</param>
		/// <param name="iLine">Line Number to display</param>

		protected void AddTask(frmTaskList.Icons taskType, string
			TaskMessage, int iLine)
		{
			((frmTaskList)cntTaskList.Control).AddTask(taskType,
				TaskMessage, iLine);
		}


		/// <summary>
		/// Clears all tasks from task list
		/// </summary>

		protected void ClearTasks()
		{
			((frmTaskList)cntTaskList.Control).ClearTasks();
		}

		/// <summary>
		/// Change all line numbers for tasks from selected start 
		/// line number by selected offset
		/// </summary>
		/// <param name="iFromLine">Start line number</param>
		/// <param name="iOffset">Offset (in line numbers)</param>

		protected void updateTasksLinesNumber(int iFromLine, int iOffset)
		{
			((frmTaskList)cntTaskList.Control).updateTasksLinesNumber(
				iFromLine, iOffset);
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
			if (tgDocuments.ActiveLeaf == null || tgDocuments.ActiveLeaf.TabPages.Count == 0) return;

			int iIndex;

			// Find the current docuemnt
			for (iIndex = 0; iIndex < tgDocuments.ActiveLeaf.TabPages.Count; ++iIndex)
			{
				// TODO: Won't work on more than one leaf:
				if (((frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control).sFileName == sLastCompiledFile)
					break;
			}
			
			if (tgDocuments.ActiveLeaf == null || iIndex == tgDocuments.ActiveLeaf.TabPages.Count) return;

			tgDocuments.Select();
			Crownwood.Magic.Controls.TabGroupLeaf tgl = 
				tgDocuments.ActiveLeaf == null ?
				tgDocuments.RootSequence.AddNewLeaf()
				: tgDocuments.ActiveLeaf  as TabGroupLeaf;

			// Activate the page
			if (tgl.TabPages.Count > 0) tgl.TabPages[iIndex].Selected = true;

			// Select the line
			((frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control).SetLine(iLine);

			((frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control).FocusDocument();
	
		}
		#endregion

		#region Documents Interface

		/// <summary>
		/// Get Active Document index on PageTab
		/// </summary>
		/// <returns>The index of the document</returns>

		private int GetActiveDocumentIndex()
		{
			// Check if there is an open document
			if (tgDocuments.ActiveLeaf == null || tgDocuments.ActiveLeaf.TabPages.Count == 0) throw new NoActivePageException();

			// Find the current docuemnt
			for (int iIndex = 0; iIndex < tgDocuments.ActiveLeaf.TabPages.Count; ++iIndex)
			{
				if (tgDocuments.ActiveLeaf.TabPages[iIndex].Selected == true)
					return iIndex;
			}

			throw new NoActivePageException();
		}


		/// <summary>
		/// Update the status bar location when caret changes position
		/// </summary>
		/// <param name="Location">The new location</param>
		/// <remarks>Not a general function - relies on the struct of the statusBar</remarks>
		void OnDocumentPosition(Point Location)
		{
			statusBar1.Panels[1].Text = " Ln " + Location.X.ToString() + 
			"\tCol " + Location.Y.ToString();
		}

		#endregion

		#region Files - Creating, loading, saving and closing


		/// <summary>
		/// Create new document
		/// </summary>
		private void CreateNewDocument()
		{
			// Access the active leaf or default leaf group
			Crownwood.Magic.Controls.TabGroupLeaf tgl = 
				tgDocuments.ActiveLeaf == null ?
				tgDocuments.RootSequence.AddNewLeaf()
				: tgDocuments.ActiveLeaf  as TabGroupLeaf;

			// Create a new tab
			// Lolish bug here : if docCount > int.MaxValue it will be funny
			Crownwood.Magic.Controls.TabPage tabDocument = new Crownwood.Magic.Controls.TabPage("Untitled" + ++docCount + ".asm", new frmEditor(), null);

			// Add the tab to the default leaf
			tgl.TabPages.Add(tabDocument);

			// Activate the new page
			if (tgl.TabPages.Count > 0) tgl.TabPages[tgl.TabPages.Count - 1].Selected = true;

			// Event Hooking
			((frmEditor)tabDocument.Control).OnPositionChange += new frmEditor.PositionEventFunc(OnDocumentPosition);
		}


		/// <summary>
		/// Open assembly file
		/// </summary>
		/// <returns>Success or fail</returns>
		private bool DoOpen()
		{
			// Asks the user for the filename to open
			openFileDialog1.Filter = Settings.Environment.FILE_FILTERS;
			openFileDialog1.FileName = "";
			DialogResult res = openFileDialog1.ShowDialog();
			
			if (res != DialogResult.OK) return false;
			
			// Open document specific by openFileDialog1.FileName
			string sFileContent = "";
			try
			{
				StreamReader inputFile = new 
					StreamReader(openFileDialog1.FileName);

				sFileContent = inputFile.ReadToEnd();
				inputFile.Close();
			}
			catch (Exception exp)
			{
				MessageBox.Show(exp.Message, "Error", MessageBoxButtons.AbortRetryIgnore,MessageBoxIcon.Error);
				return false;
			}

			// File name to display
			string sFileName = openFileDialog1.FileName.Substring(
				openFileDialog1.FileName.LastIndexOf(@"\") + 1);

			CreateNewTabAndPutThereTheDocument(openFileDialog1.FileName, sFileName, sFileContent);

			return true;
		}

		void CreateNewTabAndPutThereTheDocument(string sFullFileName, string sShortFileName, string sDocumentData)
		{
			// Access the active leave or default leaf group
			Crownwood.Magic.Controls.TabGroupLeaf tgl = 
				tgDocuments.ActiveLeaf == null ?
				tgDocuments.RootSequence.AddNewLeaf()
				: tgDocuments.ActiveLeaf  as TabGroupLeaf;

			// Create a new tab for the file	
			Crownwood.Magic.Controls.TabPage tabDocument = new Crownwood.Magic.Controls.TabPage(sShortFileName, new frmEditor(), null);

			// Add the tab to the default leaf
			tgl.TabPages.Add(tabDocument);

			// Activate the new page, put the file content there, update its properties
			if (tgl.TabPages.Count > 0) 
			{
				tgl.TabPages[tgl.TabPages.Count - 1].Selected = true;
				((frmEditor)tgl.TabPages[tgl.TabPages.Count - 1].Control).theDocument
					= sDocumentData;
				((frmEditor)tgl.TabPages[tgl.TabPages.Count - 1].Control).sFileName
					= sFullFileName;
			}

			// Event Hooking
			((frmEditor)tabDocument.Control).OnPositionChange += new frmEditor.PositionEventFunc(OnDocumentPosition);

			
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

			// Fall down to

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
				StreamWriter outputFile = new StreamWriter(frmToSave.sFileName, false);
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
				// Find the current docuemnt
				int iIndex = GetActiveDocumentIndex();

				// Set file name for the saving process.

				if (((frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control).
					sFileName == "")
				{
					return InterfaceDoSaveAs();
				}

				return DoSave((frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control);

			}
			catch (NoActivePageException)
			{
				return false;
			}
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
				int iIndex = GetActiveDocumentIndex();

				// Set default file name for the saving process.
				// Might need change if we will mark unsaved pages with *
				saveFileDialog1.FileName = ((frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control)
					.sFileName != "" ? ((frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control)
					.sFileName : tgDocuments.ActiveLeaf.TabPages[iIndex].Title;


				saveFileDialog1.Filter = Settings.Environment.FILE_FILTERS;

				// ask for file name
				DialogResult res = saveFileDialog1.ShowDialog();
				if (res != DialogResult.OK) return false;

				// set the filename for the document
				((frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control)
					.sFileName = saveFileDialog1.FileName;
				tgDocuments.ActiveLeaf.TabPages[iIndex].Title = 
                    saveFileDialog1.FileName.Substring(
					saveFileDialog1.FileName.LastIndexOf(@"\") + 1);

				// saving process
				return DoSave((frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control);
			}
			catch (NoActivePageException)
			{
				return false;
			}
		}


		/// <summary>
		/// Close the active document
		/// </summary>
		/// <returns>Success or fail</returns>
		bool InterfaceDoClose()
		{
			// Find the current docuemnt
			try
			{
				int iIndex = GetActiveDocumentIndex();

				// Prepare document for closing
				if (!PrepareDocumentForClosing(
					(frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control)) return false;

				if (tgDocuments.ActiveLeaf != null)
				{
					// Remove document from the tab
					tgDocuments.ActiveLeaf.TabPages.RemoveAt(iIndex);

				
					if (tgDocuments.ActiveLeaf != null && tgDocuments.ActiveLeaf.TabPages.Count == 0)
						ClearDocumentInfoStatusBar();
				}
				return true;
			}
			catch (NoActivePageException)
			{
				return false;
			}
		}

		/// <summary>
		/// Close all open documents
		/// </summary>
		/// <returns>Success or Fail</returns>
		/// <remarks>There is altrnative implemantion for this function
		/// in the TODO list, that is better (but have little bug).
		/// In our free time, we can replace this function.
		/// </remarks>
		private bool CloseAllDocuments()
		{
			
			while (InterfaceDoClose());
			try
			{
				int iIndex = GetActiveDocumentIndex();
			}
			catch (NoActivePageException)
			{
				return true;
			}
			return false;
		}

		private void InterfaceDoOpen()
		{
			if (DoOpen())
			{
				GiveFocusToTheActiveDocument();
				OnDocumentPosition(new Point(1, 1));
			}
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
		private void GiveFocusToTheActiveDocument()
		{
			if (tgDocuments.ActiveLeaf == null) return;
			tgDocuments.ActiveLeaf.TabPages[tgDocuments.ActiveLeaf.TabPages.Count-1]
				.Focus();
			((frmEditor)tgDocuments.ActiveLeaf.TabPages[tgDocuments.ActiveLeaf.TabPages.Count-1].Control)
				.Focus();
			((frmEditor)tgDocuments.ActiveLeaf.TabPages[tgDocuments.ActiveLeaf.TabPages.Count-1].Control).FocusDocument();
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


		private void InterfaceDoPaste()
		{
			try
			{
				// Find the current docuemnt
				int iIndex = GetActiveDocumentIndex();

				// Do action
				((frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control)
					.Paste();
				// TODO: Undo/Redo Support
			}
			catch (NoActivePageException)
			{
				return ;
			}
		}

		public void InterfaceDoCut()
		{
			try
			{
				// Find the current docuemnt
				int iIndex = GetActiveDocumentIndex();

				// Do action
				((frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control)
					.Cut();
				// TODO: Undo/Redo Support
			}
			catch (NoActivePageException)
			{
				return ;
			}	
		}

		private void InterfaceNewFile()
		{
			CreateNewDocument();
			GiveFocusToTheActiveDocument();
			StatusBarMessage("Ready");
			OnDocumentPosition(new Point(1, 1));
		}

		private void InterfaceDoCopy()
		{
			try
			{
				// Find the current docuemnt
				int iIndex = GetActiveDocumentIndex();

				// Do action
				((frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control)
					.Copy();
				// TODO: Undo/Redo Support
			}
			catch (NoActivePageException)
			{
				return ;
			}
		}


		#endregion

		#region Compiler's Related Functions

		void HandleError(int iLine, CompilerMessage msg)
		{
			AddTask(frmTaskList.Icons.Error, 
				CompilerMessagesStrings.GetMessageText(msg), iLine);
			AppendToOutputWindow(
			string.Format("Error({0}): ", iLine) + CompilerMessagesStrings.GetMessageText(msg));
		}
		
		#endregion

		private void mnuFileNew_Click(object sender, System.EventArgs e)
		{
			InterfaceNewFile();
		}


		private void mnuViewTaskList_Click(object sender, System.EventArgs e)
		{
			// TODO: Possible bug here. Close Task List and Output,
			// then show it. I think it behave strange
			_dockingManager.ShowContent(cntTaskList);
		}

		private void mnuViewOutput_Click(object sender, System.EventArgs e)
		{
			_dockingManager.ShowContent(cntCompileMessage);
		}

		private void tgDocuments_PageCloseRequest(Crownwood.Magic.Controls.TabbedGroups tg, Crownwood.Magic.Controls.TGCloseRequestEventArgs e)
		{

			if (PrepareDocumentForClosing((frmEditor)e.TabPage.Control)) 
			{
				// Stop listen to events. Must for the statusbar to work properly
				((frmEditor)e.TabPage.Control).OnPositionChange -= new frmEditor.PositionEventFunc(OnDocumentPosition);;
				this.ClearDocumentInfoStatusBar();

				return;

			}
			
			e.Cancel = true;
		}


		private void mnuFileClose_Click(object sender, System.EventArgs e)
		{
			InterfaceDoClose();
		}

		private void mnuFileSaveAs_Click(object sender, System.EventArgs e)
		{
			InterfaceDoSaveAs();
		}


		private void mnuFileSave_Click(object sender, System.EventArgs e)
		{
			InterfaceDoSave();
		}

		private void frmMain_Resize(object sender, System.EventArgs e)
		{
			ArrangeControls();
		}

		private void mnuFilePrintPrintCode_Click(object sender, System.EventArgs e)
		{
			if (printDialog1.ShowDialog() == DialogResult.Cancel) return;
			// TODO: Printing
		}

		private void frmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (CloseAllDocuments() == false) e.Cancel = true;
		}

		private void mnuCloseAll_Click(object sender, System.EventArgs e)
		{
			CloseAllDocuments();
		}

		private void mnuEditSelectAll_Click(object sender, System.EventArgs e)
		{
			try
			{
				// Find the current docuemnt
				int iIndex = GetActiveDocumentIndex();

				// Do action
				((frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control)
					.SelectAll();
			}
			catch (NoActivePageException)
			{
				return ;
			}		
		}

		private void mnuEditCut_Click(object sender, System.EventArgs e)
		{
			InterfaceDoCut();
		}

		private void mnuEditCopy_Click(object sender, System.EventArgs e)
		{
			InterfaceDoCopy();
		}

		private void mnuEditPaste_Click(object sender, System.EventArgs e)
		{
			InterfaceDoPaste();
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			// TODO: Finish toolbar

			if (e.Button == toolBarNew) InterfaceNewFile();
			if (e.Button == toolBarOpen) InterfaceDoOpen();
			if (e.Button == toolBarSave) InterfaceDoSave();
			//if (e.Button == toolBarSaveAll) ;
			//if (e.Button == toolBarPrint) ;
			//if (e.Button == toolBarFind) ;
			if (e.Button == toolBarCut) InterfaceDoCut();
			if (e.Button == toolBarCopy) InterfaceDoCopy();
			if (e.Button == toolBarPaste) InterfaceDoPaste();
			//if (e.Button == toolBarUndo) ;
			//if (e.Button == toolBarRedo) ;
		}

		private void mnuBuildCompile_Click(object sender, System.EventArgs e)
		{
			ClearTasks();
			ClearOutputWindow();
			try
			{
				// Find the current docuemnt
				int iIndex = GetActiveDocumentIndex();

				AppendToOutputWindow(
				string.Format("------ Build started: File Name: {0} ------\r\n", 
				tgDocuments.ActiveLeaf.TabPages[iIndex].Title));

				AppendToOutputWindow("Performing main compilation...\r\n");

				// Internal variables update
				sLastCompiledFile = ((frmEditor)(tgDocuments.ActiveLeaf.TabPages[iIndex].Control)).sFileName;


				// Do compile
				CodeBlock progrie = 
					_assembler.CompileCode(
					((frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control).theDocument);
				
				// TODO: Rewrite, it sucks, won't work always
				string sFileName = ((frmEditor)tgDocuments.ActiveLeaf.TabPages[iIndex].Control).sFileName;
				if (sFileName == "") sFileName = tgDocuments.ActiveLeaf.TabPages[iIndex].Title;
				sFileName = sFileName.Replace(".asm", ".lst");

				string sDoc = _assembler.GetLSTFile();
				//StreamWriter w = new StreamWriter(sFileName, true);
				
				//w.Write(sDoc);
				//w.Close();
				CreateNewTabAndPutThereTheDocument(sFileName, 
					(tgDocuments.ActiveLeaf.TabPages[iIndex].Title).Replace(".asm", ".lst"),
					sDoc);

				
			}
			catch (CompileError)
			{

			}

			// No active document
			catch (NoActivePageException)
			{
				return ;
			}

			int iNumberOfErrors = _assembler.NumberOfErrors;
			AppendToOutputWindow(string.Format("\r\nBuild complete -- {0} errors",iNumberOfErrors));
			AppendToOutputWindow("\r\n\r\n---------------------- Done ----------------------");
			if (iNumberOfErrors == 0) AppendToOutputWindow("Build succeeded");
			else 
			{
				AppendToOutputWindow("Build failed");
				MessageBox.Show("Compile Failed: There were build errors.", 
					Settings.Environment.MESSAGEBOXS_TITLE,MessageBoxButtons.OK,
					MessageBoxIcon.Exclamation);
			}

		}



	}
}
