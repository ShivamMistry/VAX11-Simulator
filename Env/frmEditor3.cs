using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using RichTextExLib;
using System.Text.RegularExpressions;
using System.Drawing.Text;
using System.Threading;

using VAX11Compiler;
using VAX11Settings;

namespace VAX11Environment
{
	/// <summary>
	/// Summary description for frmEditor.
	/// </summary>
	public class frmEditor : System.Windows.Forms.UserControl
	{
		
		#region Members

		/// <summary>
		/// RichTextBox containing the document
		/// </summary>
		private RichTextExLib.RichTextBoxEx txtDocument;

		/// <summary>
		/// Internal string name
		/// </summary>
		/// <remarks>
		/// We initalize the string with empty string,
		/// so we will be able to indentify new document
		/// that weren't save yet
		/// </remarks>
		private string _sFileName = "";

		const short WM_PAINT = 0x00f;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Did we save the current document
		/// </summary>
		private bool _bDocumentSaved = true;

		/// <summary>
		/// Helping variable for not mark document as changed while loading
		/// </summary>
		private bool bLoadingDocument = false;

		private int iLastLineDelWas = -1;
		private int iLastLineEnterWas = -1;
		private int iLastLineBackSpaceWas = -1;
		private bool StartOfLine;

		/// <summary>
		/// for updating changes on lines when selecting text and replacing it.
		/// </summary>
		private int iSelectionLines;

		/// <summary>
		/// Compiled code corresponding for the text
		/// </summary>
		private VAX11Compiler.Assembler _assembler;


		/// <summary>
		/// Output window text related to this window
		/// </summary>
		private string _OutputText;


		/// <summary>
		/// Flag that tells if the code need recompile or not
		/// </summary>
		private bool _bNeedCompile;


		/// <summary>
		/// Save error handler function in order to send messages to the frmMain about the error
		/// specific for this document
		/// </summary>
		private Assembler.compileErrorHandler _HandleErrorFunction;

		/// <summary>
		/// Should we color syntax in the editor or not?
		/// Might change per window, as windows like .lst file doesn't need coloring.
		/// </summary>
		private bool _bDoColor = true;
		/// <summary>
		/// stop update the location of the cursore when just passing the text like coloring
		/// </summary>
		private bool _bStopUpdateLocation = false;
		private bool _bColoringBkGround = false;
		private bool _bTextConverted = false;
		private string _RealText = "";


		/// <summary>
		/// Important! this list saves lines numbers, not PC
		/// </summary>
		public BreakPointList bList = new BreakPointList();

		private string _sInputFileName = "";
		private string _sOutputFileName = "";

		#endregion

		#region Controls

		private System.Windows.Forms.ContextMenu contextMenuEditor;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem mnuPopupUndo;
		private System.Windows.Forms.MenuItem mnuPopupRedo;
		private System.Windows.Forms.MenuItem mnuPopupCut;
		private System.Windows.Forms.MenuItem mnuPopupCopy;
		private System.Windows.Forms.MenuItem mnuPopupPaste;
		private System.Windows.Forms.MenuItem mnuPopupDelete;
		private System.Windows.Forms.MenuItem mnuPopupSelectAll;
		private System.Windows.Forms.ImageList imgListtoolBar;
		private DotNetWidgets.DotNetMenuProvider dotNetMenuProvider1;
		private RichTextExLib.RichTextBoxEx TempRTB;

		#endregion

		#region Properties

			#region Document's saving related properties

		/// <summary>
		/// Did we save the current document
		/// </summary>
		public bool bDocumentSaved
		{
			get
			{
				return _bDocumentSaved;
			}
			set
			{
				_bDocumentSaved = value;
			}
		}

		
		/// <summary>
		/// File Name (including path)
		/// </summary>
		public string sFileName
		{
			get
			{
				return _sFileName;
			}

			set
			{
				_sFileName = value;
			}

		}

			#endregion

			#region Assembler related properties

		/// <summary>
		/// Compiled code corresponding for the text
		/// </summary>
		public VAX11Compiler.Assembler Assembler
		{
			get { return _assembler; }
		}

		/// <summary>
		/// Flag that tells if the code need recompile or not
		/// </summary>
		public bool bNeedCompile
		{
			get { return _bNeedCompile; }
			set { _bNeedCompile = value; }
		}

			#endregion
		
			#region Interface properties

		/// <summary>
		/// Should we color syntax in the editor or not?
		/// Might change per window, as windows like .lst file doesn't need coloring.
		/// </summary>
		public bool bDoColor
		{
			get
			{
				return _bDoColor;
			}

			set
			{
				_bDoColor = value;
			}
		}

		private int BackGroundLine = -1;
		private int ErrorLine = -1;
		public int iBkGroundLine
		{
			get
			{
				return BackGroundLine;
			}
			set
			{
				BackGroundLine = value;
			}
		}

		/// <summary>
		/// Sets the document backcolor
		/// </summary>
		public Color DocumentBackColor
		{
			set { txtDocument.BackColor = value; }
		}


			#endregion

			#region General properties
		public int LastLine
		{
			get	{return txtDocument.Lines.Length;}
		}
		public string RealText
		{
			get
			{
				if (!_bTextConverted)
				{
					_RealText = GetTextFromRtf (txtDocument.Rtf);
					_bTextConverted = true;
				}
				return _RealText;
			}
			set
			{
				txtDocument.Rtf = GetRTFfromText(value);
			}
		}

		public int TextSize
		{
			get
			{
				return RealText.Length;
			}
		}

		/// <summary>
		/// Access to the document.
		/// </summary>
		public string theDocument
		{
			get
			{
				return RealText;
			}
			set
			{
				bLoadingDocument = true;
				RealText = value;
				bLoadingDocument = false;
				_bNeedCompile = true;
			}
		}


		/// <summary>
		/// Output window text related to this window
		/// </summary>
		public string OutputText
		{
			get { return _OutputText; }

			set { _OutputText = value; }
		}

			#endregion

			#region Run-Time related properties

		
		public string sInputFileName
		{
			get { return _sInputFileName; }
			set { _sInputFileName = value; }
		}

		public string sOutputFileName
		{
			get { return _sOutputFileName; }
			set { _sOutputFileName = value; }
		}


			#endregion

		#endregion

		#region Events

		/// <summary>
		/// Delegate for position event
		/// </summary>
		public delegate void PositionEventFunc(Point Location);

		/// <summary>
		/// Occurs when caret's position is changing
		/// </summary>
		public event PositionEventFunc OnPositionChange;

		/// <summary>
		/// Delegate for breaking debug mode
		/// </summary>
		public delegate void StopDebug(string sExitMessage);

		/// <summary>
		/// Occurs when the text is changing
		/// </summary>
		public event StopDebug OnStopDebug;

		/// <summary>
		/// Delegate for updating errors' line numbers
		/// </summary>
		public delegate void ChangingLinesPositions(int iFromLine, int iOffset);

		/// <summary>
		/// Occurs when the number of lines is changing
		/// </summary>
		public event ChangingLinesPositions NewErrorsLines;

		private bool _Paint = true;
		private bool bkGroundNeedReColor = false;

		private void txtDocument_TextChanged(object sender, System.EventArgs e)
		{

			if (bLoadingDocument) return;
			if (_bDoColor == false) return;
			if (_bColoringBkGround) return;

			_bDocumentSaved = false;
			_bTextConverted = false;
			_bNeedCompile = true;

			if (iSelectionLines != 0)
			{
				UpdateLines (txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart), iSelectionLines);
				iSelectionLines = 0;
			}

			// stop running if text is changed
			if (OnStopDebug != null)
				OnStopDebug("User stopped debugged application");


			if (iLastLineDelWas != -1)
			{
				int icurLine = iLastLineDelWas;
				iLastLineDelWas = -1;
				int iTmpPos = txtDocument.SelectionStart;

				// continue treat on background color bug
				if (bkGroundNeedReColor)
				{
					if (icurLine == BackGroundLine)
						ColorBackGroundLine(icurLine, Settings.Environment.DEBUG_LINE_COLOR);
					else
						ColorBackGroundLine(icurLine, Settings.Environment.BREAKPOINT_COLOR);
					txtDocument.SelectionStart = iTmpPos;
				}
			}
			else if (iLastLineBackSpaceWas == -1 && iLastLineEnterWas == -1)
			{
				int iTmpPos = txtDocument.SelectionStart;
				int icurLine = txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart) + 1;
				if (icurLine == BackGroundLine)
				{
					UnColorBackGroundLine(icurLine);
					ColorBackGroundLine(icurLine, Settings.Environment.DEBUG_LINE_COLOR);
					txtDocument.SelectionStart = iTmpPos;
				}
				else if (bList.Contains(icurLine))
				{
					UnColorBackGroundLine(icurLine);
					ColorBackGroundLine(icurLine, Settings.Environment.BREAKPOINT_COLOR);
					txtDocument.SelectionStart = iTmpPos;
				}
			}

			DoColorLine ();
		}


		private void txtDocument_SelectionChanged(object sender, System.EventArgs e)
		{
			// Raise event - location changed
			if ((_bStopUpdateLocation == false) && OnPositionChange != null)
				OnPositionChange(GetLocationOnRTB(txtDocument));	
		}


		private void frmEditor_VisibleChanged(object sender, System.EventArgs e)
		{
			if (this.Visible == false) return;

			// Raise event - location changed
			if ((_bStopUpdateLocation == false) && OnPositionChange != null) 
				OnPositionChange(GetLocationOnRTB(txtDocument));


			// Magic numbers (Clears task list)
			_HandleErrorFunction(0, CompilerMessage.CLEAR_TASK_LIST);

			// Send all error messages
			ArrayList cComments = _assembler.CompileMessages.comments;
			for (int iCounter = 0; iCounter < cComments.Count; ++iCounter)
				_HandleErrorFunction(((CompilerCommentEntry)cComments[iCounter]).Line, 
					((CompilerCommentEntry)cComments[iCounter]).msgID);
		}


		private void frmEditor_Load(object sender, System.EventArgs e)
		{
			txtDocument.BackColor = Settings.Environment.bDoSyntaxHighlight ?
				Settings.Environment.BACKGROUND_COLOR : Color.White;
			txtDocument.Font = new Font(Settings.Environment.EditorFontName, Settings.Environment.EditorFontSize);
		}


		public void SetDragNDrop(System.Windows.Forms.DragEventHandler enter, System.Windows.Forms.DragEventHandler drop)
		{
			txtDocument.DragEnter += new System.Windows.Forms.DragEventHandler(enter);
			txtDocument.DragDrop += new System.Windows.Forms.DragEventHandler(drop);
		}


		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			if (m.Msg == WM_PAINT)
			{
				if (_Paint)
				{
					base.WndProc(ref m);   // if we decided to paint this control, just call the RichTextBox WndProc
				}
				else
					m.Result = IntPtr.Zero;   //  not painting, must set this to IntPtr.Zero if not painting otherwise serious problems.
			}
			else
				base.WndProc (ref m);   // message other than WM_PAINT, just do what you normally do.

		}


		private void txtDocument_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{

			if (e.KeyChar == 13)//enter pressed
			{
				int iTmpPos = txtDocument.SelectionStart;

				// continue treat on background color bug
				if (bkGroundNeedReColor)
				{
					int icurLine = txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart);
					txtDocument.SelectionStart = Math.Max(iTmpPos - 2, 0);
					_bDontRefresh = true;
					this.SetVisibleCore(false);
					DoColorLine();
					this.SetVisibleCore(true);
					_bDontRefresh = false;

					if (StartOfLine)
						ColorBackGround(icurLine+1);
					else
						ColorBackGround(icurLine);
//					if (icurLine == BackGroundLine)
//						ColorBackGroundLine(icurLine, Settings.Environment.DEBUG_LINE_COLOR);
//					else
//						ColorBackGroundLine(icurLine, Settings.Environment.BREAKPOINT_COLOR);
					txtDocument.SelectionStart = iTmpPos;
				}
				else
				{
					// color 2 lines
					txtDocument.SelectionStart = Math.Max(iTmpPos - 2, 0);
					_bDontRefresh = true;
					this.SetVisibleCore(false);
					DoColorLine();
					txtDocument.SelectionStart = iTmpPos;
					this.SetVisibleCore(true);
					_bDontRefresh = false;
				}

				/*	NeedCompiling = true;   RUNTIME_COMPILER
					eWaitForCompile.Set();*/
			}	
	
			if (e.KeyChar == 8)//backspace pressed
			{
				int iTmpPos = txtDocument.SelectionStart;

				// continue treat on background color bug
				if (bkGroundNeedReColor)
				{
					int icurLine = txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart) + 1;

					if (icurLine == BackGroundLine)
						ColorBackGroundLine(icurLine, Settings.Environment.DEBUG_LINE_COLOR);
					else
						ColorBackGroundLine(icurLine, Settings.Environment.BREAKPOINT_COLOR);
					txtDocument.SelectionStart = iTmpPos;
				}
			}

			// Must be here so pressing enter in end of line will present the right line number
			// on status bar
			if ( OnPositionChange != null) OnPositionChange(GetLocationOnRTB(txtDocument));

			/*else if ((e.KeyChar == 8) && (lastNumOfLines != txtDocument.Lines.Length))//backspace pressed 
			{													RUNTIME_COMPILER
				lastNumOfLines = txtDocument.Lines.Length;
				NeedCompiling = true;
				eWaitForCompile.Set();
			}*/
		}


		private void frmEditor_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == Keys.F1)
			{
				MessageBox.Show(GetCurrentWord());
			}
		}


		private void frmEditor_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			bkGroundNeedReColor = false;
			
			iLastLineDelWas = -1;
			iLastLineEnterWas = -1;
			iLastLineBackSpaceWas = -1;
			iSelectionLines = txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart)
				- txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart + txtDocument.SelectionLength);
			if (ErrorLine != -1)
				ClearErrorLine();
			if (e.KeyValue == 13)//enter pressed
			{
				// fixing continous background color bug
				iLastLineEnterWas = txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart);
				if ((iLastLineEnterWas == BackGroundLine - 1) || (bList.Contains(iLastLineEnterWas + 1)))
				{
					int iTempPosition = txtDocument.SelectionStart;
					int iTempLength = txtDocument.SelectionLength;
					UnColorBackGroundLine(iLastLineEnterWas + 1);
					txtDocument.Select(iTempPosition, iTempLength);
					bkGroundNeedReColor = true;
				}
				else
					bkGroundNeedReColor = false;
				if (GetLocationOnRTB(txtDocument).Y != 1)
				{
					StartOfLine = false;
					UpdateLines(iLastLineEnterWas + 1, +1);
				}
				else
				{
					StartOfLine = true;
					UpdateLines(iLastLineEnterWas, +1);
				}
			}
			else if (e.KeyValue == 8)//backspace pressed
			{
				iLastLineBackSpaceWas = txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart);
				if ((GetLocationOnRTB(txtDocument).Y == 1) //backspace at the start of a line
					&& (GetLocationOnRTB(txtDocument).X != 1)) //backspace not at the first line
				{
					// fixing background cgolor bug
					int iTmpPos = txtDocument.SelectionStart;
					if ((iLastLineBackSpaceWas == BackGroundLine) || (bList.Contains(iLastLineBackSpaceWas)))
					{
						UnColorBackGroundLine(iLastLineBackSpaceWas);
						txtDocument.SelectionStart = iTmpPos;
						bkGroundNeedReColor = true;
						if (bList.Contains(iLastLineBackSpaceWas + 1))
							DelBreakPoint(iLastLineBackSpaceWas + 1);
					}
					else 
					{
						if (bList.Contains(iLastLineBackSpaceWas + 1))
							DelBreakPoint(iLastLineBackSpaceWas + 1);
						txtDocument.SelectionStart = iTmpPos;
						bkGroundNeedReColor = false;
					}
					UpdateLines(iLastLineBackSpaceWas + 1, -1);
				}
			}
			else if (e.KeyValue == 46)//del pressed
			{
				// fixing background color bug
				int iTmpPos = txtDocument.SelectionStart;
				iLastLineDelWas = txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart) + 1;
				// chack if del is in the end of the line
				if (GetLocationOnRTB(txtDocument).Y > txtDocument.Lines[iLastLineDelWas - 1].Length)
				{
					//--iSelectionLines;
					if ((iLastLineDelWas == BackGroundLine) || (bList.Contains(iLastLineDelWas)))
					{       
						UnColorBackGroundLine(iLastLineDelWas);
						txtDocument.SelectionStart = iTmpPos;
						bkGroundNeedReColor = true;
						if (bList.Contains(iLastLineDelWas + 1))
							DelBreakPoint(iLastLineDelWas + 1);
					}
					else 
					{
						if (bList.Contains(iLastLineDelWas + 1))
							DelBreakPoint(iLastLineDelWas + 1);
						txtDocument.SelectionStart = iTmpPos;
						bkGroundNeedReColor = false;
					}
					UpdateLines(iLastLineDelWas + 1, -1);
				}
			}
		}


		private void txtDocument_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (_bColoringBkGround || !this._Paint)
				e.Cancel = true;
			else
				e.Cancel = false;
		}

		private void txtDocument_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (ErrorLine != -1)
				ClearErrorLine();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmEditor));
			this.txtDocument = new RichTextExLib.RichTextBoxEx();
			this.contextMenuEditor = new System.Windows.Forms.ContextMenu();
			this.mnuPopupUndo = new System.Windows.Forms.MenuItem();
			this.mnuPopupRedo = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.mnuPopupCut = new System.Windows.Forms.MenuItem();
			this.mnuPopupCopy = new System.Windows.Forms.MenuItem();
			this.mnuPopupPaste = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.mnuPopupSelectAll = new System.Windows.Forms.MenuItem();
			this.mnuPopupDelete = new System.Windows.Forms.MenuItem();
			this.imgListtoolBar = new System.Windows.Forms.ImageList(this.components);
			this.dotNetMenuProvider1 = new DotNetWidgets.DotNetMenuProvider();
			this.TempRTB = new RichTextExLib.RichTextBoxEx();
			this.SuspendLayout();
			// 
			// txtDocument
			// 
			this.txtDocument.AcceptsTab = true;
			this.txtDocument.AllowDrop = true;
			this.txtDocument.ContextMenu = this.contextMenuEditor;
			this.txtDocument.DetectUrls = false;
			this.txtDocument.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtDocument.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.txtDocument.Name = "txtDocument";
			this.txtDocument.Size = new System.Drawing.Size(480, 230);
			this.txtDocument.TabIndex = 0;
			this.txtDocument.Text = "";
			this.txtDocument.WordWrap = false;
			this.txtDocument.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmEditor_KeyDown);
			this.txtDocument.SelectionChanged += new System.EventHandler(this.txtDocument_SelectionChanged);
			this.txtDocument.MouseDown += new System.Windows.Forms.MouseEventHandler(this.txtDocument_MouseDown);
			this.txtDocument.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDocument_KeyPress);
			this.txtDocument.Validating += new System.ComponentModel.CancelEventHandler(this.txtDocument_Validating);
			this.txtDocument.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmEditor_KeyUp);
			this.txtDocument.TextChanged += new System.EventHandler(this.txtDocument_TextChanged);
			// 
			// contextMenuEditor
			// 
			this.contextMenuEditor.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																							  this.mnuPopupUndo,
																							  this.mnuPopupRedo,
																							  this.menuItem3,
																							  this.mnuPopupCut,
																							  this.mnuPopupCopy,
																							  this.mnuPopupPaste,
																							  this.menuItem8,
																							  this.mnuPopupSelectAll,
																							  this.mnuPopupDelete});
			// 
			// mnuPopupUndo
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuPopupUndo, true);
			this.mnuPopupUndo.Index = 0;
			this.mnuPopupUndo.Text = "&Undo";
			this.mnuPopupUndo.Visible = false;
			// 
			// mnuPopupRedo
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuPopupRedo, true);
			this.mnuPopupRedo.Index = 1;
			this.mnuPopupRedo.Text = "&Redo";
			this.mnuPopupRedo.Visible = false;
			// 
			// menuItem3
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem3, true);
			this.menuItem3.Index = 2;
			this.menuItem3.Text = "-";
			this.menuItem3.Visible = false;
			// 
			// mnuPopupCut
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuPopupCut, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuPopupCut, 1);
			this.mnuPopupCut.Index = 3;
			this.mnuPopupCut.Text = "Cu&t";
			this.mnuPopupCut.Click += new System.EventHandler(this.mnuPopupCut_Click);
			// 
			// mnuPopupCopy
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuPopupCopy, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuPopupCopy, 2);
			this.mnuPopupCopy.Index = 4;
			this.mnuPopupCopy.Text = "&Copy";
			this.mnuPopupCopy.Click += new System.EventHandler(this.mnuPopupCopy_Click);
			// 
			// mnuPopupPaste
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuPopupPaste, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuPopupPaste, 3);
			this.mnuPopupPaste.Index = 5;
			this.mnuPopupPaste.Text = "&Paste";
			this.mnuPopupPaste.Click += new System.EventHandler(this.mnuPopupPaste_Click);
			// 
			// menuItem8
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem8, true);
			this.menuItem8.Index = 6;
			this.menuItem8.Text = "-";
			// 
			// mnuPopupSelectAll
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuPopupSelectAll, true);
			this.mnuPopupSelectAll.Index = 7;
			this.mnuPopupSelectAll.Text = "Select &All";
			this.mnuPopupSelectAll.Click += new System.EventHandler(this.mnuPopupSelectAll_Click);
			// 
			// mnuPopupDelete
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuPopupDelete, true);
			this.mnuPopupDelete.Index = 8;
			this.mnuPopupDelete.Text = "&Delete";
			this.mnuPopupDelete.Click += new System.EventHandler(this.mnuPopupDelete_Click);
			// 
			// imgListtoolBar
			// 
			this.imgListtoolBar.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
			this.imgListtoolBar.ImageSize = new System.Drawing.Size(16, 16);
			this.imgListtoolBar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListtoolBar.ImageStream")));
			this.imgListtoolBar.TransparentColor = System.Drawing.Color.FromArgb(((System.Byte)(189)), ((System.Byte)(189)), ((System.Byte)(189)));
			// 
			// dotNetMenuProvider1
			// 
			this.dotNetMenuProvider1.ImageList = this.imgListtoolBar;
			this.dotNetMenuProvider1.OwnerForm = null;
			// 
			// TempRTB
			// 
			this.TempRTB.ContextMenu = this.contextMenuEditor;
			this.TempRTB.DetectUrls = false;
			this.TempRTB.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TempRTB.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.TempRTB.Name = "TempRTB";
			this.TempRTB.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.TempRTB.Size = new System.Drawing.Size(480, 230);
			this.TempRTB.TabIndex = 1;
			this.TempRTB.TabStop = false;
			this.TempRTB.Text = "";
			this.TempRTB.WordWrap = false;
			// 
			// frmEditor
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.txtDocument,
																		  this.TempRTB});
			this.Name = "frmEditor";
			this.Size = new System.Drawing.Size(480, 230);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDocument_KeyPress);
			this.VisibleChanged += new System.EventHandler(this.frmEditor_VisibleChanged);
			this.Load += new System.EventHandler(this.frmEditor_Load);
			this.Validating += new System.ComponentModel.CancelEventHandler(this.txtDocument_Validating);
			this.TextChanged += new System.EventHandler(this.txtDocument_TextChanged);
			this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmEditor_KeyUp);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmEditor_KeyDown);
			this.ResumeLayout(false);

		}
		#endregion

		#region Interface
		/// <summary>
		/// Give the focus to the docuemnt
		/// </summary>
		public void FocusDocument()
		{
			txtDocument.Focus();
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			/*RuntimeCompileThread.Abort();								RUNTIME_COMPILER  
			_HandleErrorFunction(0, CompilerMessage.CLEAR_TASK_LIST);*/
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}


		public void SetLine(int iLine)
		{
			int iOffset = 0; 
			for (int i = 0; i < iLine - 1; ++i) 
			{ 
				iOffset += txtDocument.Lines[i].Length + 1; 
			}
			txtDocument.Select(iOffset, 0);
		}


		public frmEditor(Assembler.compileErrorHandler HandleErrorFunction)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Prepare assembler
			_assembler = new VAX11Compiler.Assembler();
			_assembler.OnCompileError += new Assembler.compileErrorHandler(HandleErrorFunction);

			// Save the handler
			_HandleErrorFunction = HandleErrorFunction;

			/*RuntimeCompileThread = new Thread(new ThreadStart(RuntimeCompile));		RUNTIME_COMPILER
			RuntimeCompileThread.Name = "runtime compile thread";
			eWaitForCompile = new AutoResetEvent(false);
			RuntimeCompileThread.Start();*/
		}

		
		/// <summary>
		/// At the end of the following code, iLine contains the 
		/// caret's line number and iCol contains the column.
		/// </summary>
		private Point GetLocationOnRTB(RichTextBoxEx RTB)
		{
			int iLine = RTB.GetLineFromCharIndex(RTB.SelectionStart) + 1;
			int iCol = RTB.CurrentColumn;
			return new Point(iLine, iCol);
		}


		private void UpdateLines(int iFromLine, int iOffset)
		{
			// fix break point refrences
			bList.updateBreakPointLinesNumber(iFromLine, iOffset);
			// fix step color refrence
			if (BackGroundLine != -1)
				BackGroundLine += iOffset;
			// fix error-line color refrence
			if (ErrorLine != -1)
				ErrorLine += iOffset;

			// fix errors refrences

			_assembler.CompileMessages.updateErrorsLinesNumber(iFromLine, iOffset);
			if (NewErrorsLines != null)	NewErrorsLines(iFromLine, iOffset);
		}

		#endregion

		#region Printing

		private int m_nFirstChar = 0; // First character to be printed by RichTextBoxEx
		private int m_nPageNumber = 0; // Current Page Number

		public void PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			// Increase Page Number
			++m_nPageNumber;

			// make the RichTextBox calculate and render as much text as will fit on the page
			// and remember the last character printed for the next page
			m_nFirstChar = txtDocument.FormatRange(false, e, m_nFirstChar, TextSize);


			// Draw our logo
			e.Graphics.DrawLine(System.Drawing.Pens.Black, e.MarginBounds.Left, 
				e.MarginBounds.Bottom, e.MarginBounds.Left + e.MarginBounds.Width, 
				e.MarginBounds.Bottom);

			Font logoFont = new System.Drawing.Font("Arial", 10, FontStyle.Bold);
			SizeF logoSize = e.Graphics.MeasureString(VAX11Settings.Settings.Environment.PRINTING_LOGO, logoFont);
			SizeF pageNumberInfo = e.Graphics.MeasureString(m_nPageNumber.ToString(), logoFont);
			e.Graphics.DrawString(VAX11Settings.Settings.Environment.PRINTING_LOGO, logoFont, new SolidBrush(Color.Black), e.MarginBounds.Left, e.MarginBounds.Bottom + 12);
			e.Graphics.DrawString(m_nPageNumber.ToString(), logoFont, new SolidBrush(Color.Black), e.MarginBounds.Left + e.MarginBounds.Width - pageNumberInfo.Width, e.MarginBounds.Bottom + 12);

			// More pages?
			if (m_nFirstChar < TextSize)
				e.HasMorePages = true;
			else
				e.HasMorePages = false;
		}


		public void EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			// Clean up cached information
			txtDocument.FormatRangeDone();
		}


		public void BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
		{
			// Start at the beginning of the text
			m_nFirstChar = 0;

			// Reset Page Number
			m_nPageNumber = 0;
		}
		#endregion

		#region Undo redo
		
		public void Undo()
		{
			if (txtDocument.CanUndo == true)
			{
				txtDocument.Undo();
				//txtDocument.ClearUndo();
				_bNeedCompile = true;
			}
		}

		public void Redo()
		{
			if (txtDocument.CanRedo == true)
			{
				txtDocument.Redo();
				_bNeedCompile = true;
			}
		}

		#endregion

		#region Copying, cuting, pasting, deleting

		/// <summary>
		/// Helping variable for not refresh document while pasting
		/// </summary>
		private bool _bDontRefresh = false;


		public void Cut()
		{
			int iFromLine = txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart + txtDocument.SelectionLength);
			txtDocument.Cut();

			int NewLine = txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart);
			// fix refrences
			//UpdateLines(NewLine, NewLine - iFromLine);
			_bNeedCompile = true;
			if (OnStopDebug != null)
				OnStopDebug("User stopped debugged application");
		}

		public void Copy()
		{
			txtDocument.Copy();
		}

		/// <summary>
		/// this function paste text and make sure that it is in the right colors
		/// </summary>
		public void Paste()
		{
			_bStopUpdateLocation = true;
			IDataObject CurrentClip = Clipboard.GetDataObject();
			//if clipboard contains only text
			//if (CurrentClip.GetDataPresent(DataFormats.Html)) return;
			if(CurrentClip.GetDataPresent(DataFormats.Text))
			{
				_bDocumentSaved = false;
				_bTextConverted = false;
				_bDontRefresh = true;
				bLoadingDocument = true;
				_bNeedCompile = true;
				//get the lines from memory
				string sLine = (string) CurrentClip.GetData(DataFormats.Text);
				string[] sLines = sLine.Split(new char[] {'\n'});
				int iFromLine = txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart);
				int iLinePos;

				//paste the lines
			{
				int MarkedLines = txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart + txtDocument.SelectionLength);
				int OldLine = txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart + txtDocument.SelectionLength);
				int iTempPosition = txtDocument.SelectionStart;
				int iTempLength = txtDocument.SelectionLength;

				// removing color.
				UnColorBackGroundLine(OldLine + 1);
				// return to previos position
				txtDocument.Select(iTempPosition, iTempLength);
				
				// We want to take only the text, and strip its formatting
				DataFormats.Format myFormat = DataFormats.GetFormat(DataFormats.Text);
				txtDocument.Paste(myFormat);

				int newLine = txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart);
				// fix refrences
				UpdateLines(Math.Min (OldLine, newLine)	+ 1, newLine - MarkedLines);
				if (OldLine < newLine)
                    ColorBackGround(OldLine + 1);
				else
					ColorBackGround(newLine * 2 - MarkedLines - 1);

				//txtDocument.Select(iTempPosition, iTempLength);
			}

				int iOldPosition = txtDocument.SelectionStart;
				//coloring
				txtDocument.Select(txtDocument.GetLinePosition(iFromLine), 0);
				this.SetVisibleCore(false);
				for (int i=0; i < sLines.Length; i++)
				{
					DoColorLine ();
					iLinePos = txtDocument.GetLinePosition(iFromLine+i+1);

					// In case people paste information from outside-applications
					// (aka refrences)
					if (iLinePos < 0) iLinePos = 0;
					txtDocument.Select(iLinePos, 0);
				}
				txtDocument.Select(iOldPosition, 0);

				this.SetVisibleCore(true);
				_bDontRefresh = false;
				bLoadingDocument = false;

				if ((_bStopUpdateLocation == false) && OnPositionChange != null)
					OnPositionChange(GetLocationOnRTB(txtDocument));
				if (OnStopDebug != null)
					OnStopDebug("User stopped debugged application");
			}
			_bStopUpdateLocation = false;
		}

		public void SelectAll()
		{
			txtDocument.SelectAll();
		}

		public void Delete()
		{
			IDataObject CurrentClip = Clipboard.GetDataObject();
			Cut();
			if (CurrentClip != null)
				Clipboard.SetDataObject(CurrentClip);
			else
				Clipboard.SetDataObject(new DataObject());
		}

		#endregion

		#region Coloring text
		
		
		/// <summary>
		/// helping function that check if the given word is directive.
		/// </summary>
		/// <param name="Word">the word that need to be checked</param>
		/// <returns>true if directive, else false</returns>
		private bool IsDirective (string Word)
		{
			Word = Word.ToUpper();
			if (Word=="DATA"||Word=="TEXT"||Word=="ORG"||Word=="SPACE"||Word=="SET"
				||Word=="ASCII"||Word=="ASCIZ"||Word=="ASCIC"||Word=="BYTE"||Word=="WORD"
				||Word=="INT"||Word=="LONG"||Word=="QUAD"||Word=="ENTRYPOINT") 
				return true;
			else
				return false;
		}

		/// <summary>
		///  for coloring the current line.
		/// </summary>
		private void DoColorLine()
		{
			if (_bDoColor == false) return;
			if (_bColoringBkGround) return;
			////_bStopUpdateLocation = true;

			// prepare for coloring
			int iOldSelectionStart = txtDocument.SelectionStart;
			int iOldSelectionLength = txtDocument.SelectionLength;
			int iStartPos = txtDocument.GetLinePosition(
				txtDocument.GetLineFromCharIndex(iOldSelectionStart));
			int iEndPos;
			if (txtDocument.Lines.Length > txtDocument.GetLineFromCharIndex(iStartPos))
				iEndPos = iStartPos + txtDocument.Lines
					[txtDocument.GetLineFromCharIndex(iStartPos)].Length;
			else
				iEndPos = 0; 
			
			this.SetVisibleCore(false);
			ColorLine(iStartPos, iEndPos);
			if (txtDocument.SelectionStart >=0)
				txtDocument.Select(iOldSelectionStart, iOldSelectionLength);
			if (!_bDontRefresh) this.SetVisibleCore(true);
			////_bStopUpdateLocation = false;
		}


		/// <summary>
		/// function that virtualy color each char in color (to prevent unnecessary coloring)
		/// </summary>
		/// <param name="CharColorArray">this array contains the color for each char</param>
		/// <param name="iStartPos">the start position of the section that we need to color</param>
		/// <param name="iLength">the length of the section that we need to color</param>
		/// <param name="CharColor">the color of the section</param>
		private void ColorText(ref Color[] CharColorArray,int iStartPos, int iLength, Color CharColor)
		{
			for (int index = 0; index < iLength; index++)
				CharColorArray[index + iStartPos] = CharColor;
		}
	

		/// <summary>
		/// color the all text
		/// </summary>
		public void ColorAllLines()
		{
			if (_bColoringBkGround) return;
			if (_bDoColor == false) 
			{
				int iStartPosition = txtDocument.SelectionStart;
				int iSelectionLength = txtDocument.SelectionLength;
				this.SetVisibleCore(false);
				txtDocument.SelectAll();
				txtDocument.SelectionColor = Color.Black;
				txtDocument.Select(iStartPosition, iSelectionLength);
				this.SetVisibleCore(true);
				txtDocument.BackColor = Color.White;
				return;
			}

			txtDocument.BackColor = Settings.Environment.BACKGROUND_COLOR;

			int iStartPos = 0;
			int iLength = 0;
			int iLine = 0;
			bLoadingDocument = true;
			this.SetVisibleCore(false);

			while ( iLine < txtDocument.Lines.Length )
			{
				iLength = txtDocument.Lines[iLine].Length ;
				ColorLine(iStartPos, iStartPos + iLength);
				iStartPos += iLength + 1;
				iLine++;
			}
			this.SetVisibleCore(true);
			bLoadingDocument = false;
			txtDocument.Select(0, 0);
		}


		/// <summary>
		/// this function decided for every char in wich color he will be.
		/// </summary>
		/// <param name="iStartPos">the start position of the line that need to be colored</param>
		/// <param name="iEndPos">the end position of the line that need to be colored</param>
		private void ColorLine (int iStartPos, int iEndPos)
		{
			string sLine = RealText.Substring(iStartPos, iEndPos - iStartPos);
			bool wasCommand = false;
			int iLineStart = iStartPos,iLineEnd = iEndPos; //for strings
			Color[] CharColorArray = new Color[iEndPos-iStartPos] ;
			//color all unknown text
			ColorText (ref CharColorArray, iStartPos - iLineStart, iEndPos - iStartPos,
				VAX11Settings.Settings.Environment.TEXT_COLOR);

			//coloring comments
			if (Regex.IsMatch(sLine, "#"))
			{
				int iStartMark = Regex.Match(sLine, "#").Index;
				bool bColored = false;
				int addition = 0;
				while((!bColored) && (Regex.IsMatch(sLine, "#")))
				{
					if (Regex.Matches(sLine.Substring(0,iStartMark),"\"").Count % 2 == 0)
					{
						iStartMark += iStartPos + addition;
						ColorText (ref CharColorArray, iStartMark-iLineStart, iEndPos-iStartMark,
							VAX11Settings.Settings.Environment.COMMENT_COLOR);
						iEndPos = iStartMark;
						sLine = RealText.Substring(iStartPos, iEndPos - iStartPos);
						bColored = true;
					}
					else
					{
						addition += iStartMark;
						if (sLine.Length > iStartMark) 
						{
							sLine = sLine.Substring(iStartMark+1);
							addition++;
						}
						int iQuote =Regex.Match(sLine,"\"").Index;
						if (iQuote > 0)
						{
							addition += iQuote ;
							if (sLine.Length > iQuote)
							{
								sLine = sLine.Substring(iQuote+1);
								addition++;
							}
						}
						iStartMark = Regex.Match(sLine, "#").Index;
					}
				}
				if (bColored)
					sLine = RealText.Substring(iStartPos, iEndPos - iStartPos);
				else
					sLine = RealText.Substring(iLineStart, iLineEnd - iLineStart);
			}

			//coloring labels
			if (Regex.IsMatch(sLine, @"^(\s)*[a-zA-Z_](\w)*:"))
			{
				ColorText (ref CharColorArray,iStartPos-iLineStart, Regex.Match(sLine, ":").Index + 1,
					VAX11Settings.Settings.Environment.LABEL_COLOR);
				iStartPos += Regex.Match(sLine, ":").Index + 1;
				sLine = RealText.Substring(iStartPos, iEndPos - iStartPos);
			}
			
			//coloring all the other things
			int words=Regex.Matches (sLine, "[_a-zA-Z0-9]+" ).Count;
			for (int iLoop = 0; iLoop <words; ++iLoop)
			{
				int iStartWord = Regex.Match (sLine, "[_a-zA-Z0-9]").Index ;
				int iLength = Regex.Match(sLine, @"(\w)+").Length;
				iStartPos += iStartWord;
				string sWord = RealText.Substring(iStartPos, iLength );
				char cBefore;
				if (iStartWord == 0)
					cBefore = ' ';
				else
					cBefore = sLine[iStartWord - 1];

				if (cBefore == '.')
				{
					bool isFunc = true;
					try
					{
						VAX11Internals.KnownFunctions.GetAddress(sWord);
					}
					catch (CompileError)
					{
						isFunc = false;
					}
					if (isFunc) //coloring known functions
					{
						ColorText (ref CharColorArray,(iStartPos-1)-iLineStart, iLength + 1,
							VAX11Settings.Settings.Environment.FUNCTION_COLOR);
					}
					else //can be directive
					{
						if (IsDirective(sWord))
						{
							ColorText (ref CharColorArray,(iStartPos-1)-iLineStart, iLength + 1,
								VAX11Settings.Settings.Environment.DIRECTIVE_COLOR);
						}
					}
				}
				if (!wasCommand && (cBefore == ' ' || cBefore == '\t'))
				{					
					try
					{
						wasCommand = true;
						new VAX11Internals.OpcodeEntry( sWord );
					}
					catch (VAX11Compiler.CompileError)
					{
						wasCommand = false;
					}
					if (wasCommand) //coloring commands
					{
						ColorText (ref CharColorArray,iStartPos-iLineStart, iLength,
							VAX11Settings.Settings.Environment.COMMAND_COLOR);
					}
				}
				iStartPos += iLength;
				sLine = RealText.Substring(iStartPos, iEndPos - iStartPos);
			}

			//coloring strings
			iStartPos = iLineStart;
			sLine = RealText.Substring(iStartPos, iEndPos - iStartPos);
			while (Regex.IsMatch(sLine, "\""))
			{
				int iStringEnd,iStringStart = iStartPos + Regex.Match(sLine, "\"").Index;
				sLine = RealText.Substring(iStringStart + 1, iEndPos - (iStringStart + 1));
				if (Regex.IsMatch(sLine, "\"")) 
					iStringEnd = iStringStart + Regex.Match(sLine, "\"").Index + 2;
				else
					iStringEnd = iEndPos;
				if (sLine != "") sLine = RealText.Substring(iStringEnd, iEndPos - iStringEnd);

				ColorText (ref CharColorArray,iStringStart-iLineStart, iStringEnd-iStringStart,
					VAX11Settings.Settings.Environment.STRING_COLOR);

				iStartPos = iStringEnd;
			}

			ColorNow (CharColorArray, iLineStart, iLineEnd-iLineStart); //do the actual coloring
		}


		/// <summary>
		/// this procedure do the actual coloring
		/// </summary>
		/// <param name="CharColorArray">array that contain for every char in wich color he suppouse to be</param>
		/// <param name="iStartPos">the start position of the line</param>
		/// <param name="iLength">the end position of the line</param>
		private void ColorNow (Color[] CharColorArray,int iStartPos,int iLength)
		{
			int index = 0;
			////_bStopUpdateLocation = true;
			while (index < iLength)
			{
				int iCurrentColor = index + 1;
				Color CurrentColor = CharColorArray[index];
				/*txtDocument.Select(index + iStartPos,1);
				if (txtDocument.SelectionColor == CurrentColor)
				{
					index++;
					continue;
				}*/
				while ((iCurrentColor < iLength) && ((CurrentColor == CharColorArray[iCurrentColor]) 
					/*||(RealText[iCurrentColor] == ' ') /*|| (RealText[iCurrentColor + 1] == '	')*/))
					iCurrentColor++;
				txtDocument.Select(index + iStartPos, iCurrentColor - index);
				txtDocument.SelectionColor = CharColorArray[index];
				index = iCurrentColor;
			}
			////_bStopUpdateLocation = false;
		}

	
		#endregion

		#region RichTextFormatTools

		/// <summary>
		/// this function gets the RTF and returns his header (until the start of the text)
		/// </summary>
		/// <param name="oldRtf">the oreginal file, after the end of the function don't have header</param>
		/// <returns>the RTF header</returns>
		private string GetRTFHeader(ref string oldRtf)
		{
			int iSubString = oldRtf.IndexOf(@"{\colortbl") + (@"{\colortbl").Length + 1;
			string newRtf = oldRtf.Substring (0, iSubString);
			oldRtf = oldRtf.Remove(0, iSubString);
			iSubString = oldRtf.IndexOf(";}") + (@";}").Length + 1;
			newRtf += oldRtf.Substring (0, iSubString);
			oldRtf = oldRtf.Remove (0, iSubString);
			//till now its the real headder, now removing the settings
			if (Regex.IsMatch(oldRtf, @"\\fs[0-9]+"))
			{
				int iSub = Regex.Match(oldRtf, @"\\fs[0-9]+").Index;
				int iLen = Regex.Match(oldRtf, @"\\fs[0-9]+").Length;
				newRtf += oldRtf.Substring(0, iSub + iLen);
				oldRtf = oldRtf.Remove(0, iSub + iLen);
			}
			if (Regex.IsMatch(oldRtf, @"^\\f[0-9]+"))
			{
				int iSub = Regex.Match(oldRtf, @"^\\f[0-9]+").Index;
				int iLen = Regex.Match(oldRtf, @"^\\f[0-9]+").Length;
				newRtf += oldRtf.Substring(0, iSub + iLen);
				oldRtf = oldRtf.Remove(0, iSub + iLen);
			}
			if (oldRtf[0] == ' ') 
			{
				newRtf += " ";
				oldRtf = oldRtf.Remove(0, 1);
			}
			return newRtf;
		}

		/// <summary>
		/// this function gets the color table from header of RTF
		/// </summary>
		/// <param name="oldRtf">the RTF header, after the function end, contains the rest of the header</param>
		/// <returns>color table</returns>
		/// <remarks>assume that oldRtf end with the end of the color table</remarks>
		private string GetColorTableFromHeader (ref string oldRtf)
		{
			int iSubString = oldRtf.IndexOf(@"{\colortbl") + (@"{\colortbl").Length + 1;
			//getting the ColorTable
			string ColorTable = oldRtf.Substring(iSubString);
			//removing the ColorTable from the source
			oldRtf = oldRtf.Remove(iSubString, oldRtf.Length - iSubString);
			return ColorTable;
		}


		/// <summary>
		/// returns the index of the color, if color don't exsist return negative number
		/// </summary>
		/// <param name="ColorTable">The color table, in RTF</param>
		/// <param name="WantedColor">The color that needed to find</param>
		/// <returns>the index of the color, negative if don't exsist</returns>
		private int FindColorIndex (string ColorTable, Color WantedColor)
		{
			int iWantedColor = -1;
			System.Text.RegularExpressions.MatchCollection Colors = 
				Regex.Matches (ColorTable, @";\\red[0-9]+\\green[0-9]+\\blue[0-9]+");
			//for every color in the table, find his components
			for (int iColor = 0; iColor < Colors.Count; ++iColor)
			{
				//getting the RGB string for color #iColor
				string sRGB = Colors[iColor].Value;

				//getting the red value from color #iColor
				System.Text.RegularExpressions.Match ColorComponent = Regex.Match(sRGB, "[0-9]+");
				//and saving his value to iRed
				int iRed = Convert.ToInt16 (ColorComponent.Value);
				//removing the Red Color Component from RGB
				sRGB = sRGB.Remove (ColorComponent.Index, ColorComponent.Length);

				//getting the green value from color #iColor
				ColorComponent = Regex.Match(sRGB, "[0-9]+");
				//and saving his value to iGreen
				int iGreen = Convert.ToInt16 (ColorComponent.Value);
				//removing the Green Color Component from RGB
				sRGB = sRGB.Remove (ColorComponent.Index, ColorComponent.Length);

				//getting the blue value from color #iColor
				ColorComponent = Regex.Match(sRGB, "[0-9]+");
				//and saving his value to iBlue
				int iBlue = Convert.ToInt16 (ColorComponent.Value);

				if ((WantedColor.R==iRed) && (WantedColor.G==iGreen) && (WantedColor.B==iBlue))
					iWantedColor = iColor;
			}
			if (iWantedColor == -1) iWantedColor = -Colors.Count;
			return iWantedColor;
		}

		/// <summary>
		/// this function Add the color to the table and return his index
		/// </summary>
		/// <param name="ColorTable">The color table in RTF</param>
		/// <param name="WantedColor">The color that we need to add</param>
		/// <returns>The index of the color (never negative)</returns>
		private int AddColorToTable (ref string ColorTable, Color WantedColor)
		{
			int ColorIndex = FindColorIndex(ColorTable,WantedColor);
			// if the wanted color is unfound then add it to the end of the list
			if (ColorIndex < 0)
			{
				int iSubString  = ColorTable.IndexOf(";}");
				string newRtf = ColorTable.Substring (0, iSubString);
				ColorTable = ColorTable.Remove (0, iSubString + (@";}").Length + 1);
				newRtf += @";\red" + WantedColor.R.ToString() + @"\green" + WantedColor.G.ToString() + @"\blue" + WantedColor.B.ToString();
				newRtf += @";}";
				ColorTable = newRtf + ColorTable;
				return -ColorIndex;
			}
			else
				return ColorIndex;
		}


		/// <summary>
		/// this function return one line in RTF
		/// </summary>
		/// <param name="oldRtf">the sorce</param>
		/// <param name="LineNumber">number of the line</param>
		/// <returns>the line in RTF</returns>
		private string GetRTFLine (string oldRtf, int LineNumber)
		{
			int iSubString;
			for (int iCurrentLine = 1; (iCurrentLine < LineNumber); ++iCurrentLine)
			{
				//need to check that the LineNumber is valid
				iSubString = oldRtf.IndexOf(@"\par") + (@"\par").Length + 2;
				oldRtf = oldRtf.Remove (0, iSubString);
			}
			iSubString = oldRtf.IndexOf(@"\par") + (@"\par").Length + 2;
			return oldRtf.Substring (0, iSubString);
		}


		/// <summary>
		/// this function return the lines till LineNumer in RTF
		/// </summary>
		/// <param name="oldRtf">the sorce</param>
		/// <param name="LineNumber">number of the line</param>
		/// <returns>the lines in RTF</returns>
		private string GetRtfTillLineNum (string oldRtf, int LineNumber)
		{
			int iSubString;
			string newRtf = string.Empty;
			for (int iCurrentLine = 1; (iCurrentLine < LineNumber); ++iCurrentLine)
			{
				//need to check that the LineNumber is valid
				iSubString = oldRtf.IndexOf(@"\par") + (@"\par").Length + 2;
				newRtf += oldRtf.Substring (0, iSubString);
				oldRtf = oldRtf.Remove (0, iSubString);
			}
			return newRtf;
		}

		/// <summary>
		/// this function return the lines from LineNumer in RTF
		/// </summary>
		/// <param name="oldRtf">the sorce</param>
		/// <param name="LineNumber">number of the line</param>
		/// <returns>the lines in RTF</returns>
		private string GetRtfFromLineNum (string oldRtf, int LineNumber)
		{
			int iSubString;
			for (int iCurrentLine = 1; (iCurrentLine < LineNumber + 1); ++iCurrentLine)
			{
				//need to check that the LineNumber is valid
				iSubString = oldRtf.IndexOf(@"\par") + (@"\par").Length + 2;
				oldRtf = oldRtf.Remove (0, iSubString);
			}
			return oldRtf;
		}

		/// <summary>
		/// this function color the background of the LineNumber in BkGrnd Color
		/// </summary>
		private void ColorBackGroundLine(int LineNumber, Color BkGrnd)
		{
			if (LineNumber < 1)
				LineNumber = 1;
			bool wasSaved = _bDocumentSaved;
			string oldRtf = txtDocument.Rtf;
			string newRtf = GetRTFHeader(ref oldRtf);
			string ColorTable = GetColorTableFromHeader (ref newRtf);
			int iColorIndex = AddColorToTable(ref ColorTable, BkGrnd);
			newRtf += ColorTable;
			newRtf += GetRtfTillLineNum(oldRtf, LineNumber);
			string lineRtf = GetRTFLine (oldRtf, LineNumber);
			
			//			int iSubString = lineRtf.IndexOf(" ") ;
			//			newRtf += lineRtf.Substring (0, iSubString);
			//			lineRtf = lineRtf.Remove(0, iSubString);
			int iSubString;

			while (lineRtf.StartsWith(" ") || lineRtf.StartsWith(@"\tab") || 
				Regex.IsMatch(lineRtf, @"^\\cf[0-9]+"))
			{
				if (lineRtf.StartsWith(" "))
				{
					newRtf += " ";
					lineRtf = lineRtf.Remove(0, 1);
				}
				else if (lineRtf.StartsWith(@"\tab"))
				{
					newRtf += @"\tab";
					lineRtf = lineRtf.Remove(0, 4);
				}
				else
				{
					int iSub = Regex.Match(lineRtf, @"^\\cf[0-9]+").Index;
					int iLen = Regex.Match(lineRtf, @"^\\cf[0-9]+").Length;
					newRtf += lineRtf.Substring(iSub, iLen);
					lineRtf = lineRtf.Remove(iSub, iLen);					
				}
			}

			newRtf += @"\highlight" + ((int)(iColorIndex + 1)).ToString() + " ";

			int iComment = lineRtf.IndexOf (@"#");
			iSubString = lineRtf.LastIndexOf (@"\par");
			if (iComment == -1) iComment = iSubString;
			iSubString = System.Math.Min(iComment, iSubString);
			string sEndLine = lineRtf.Substring (iSubString);
			lineRtf = lineRtf.Remove (iSubString, sEndLine.Length);
			while (lineRtf.EndsWith(" ") || lineRtf.EndsWith(@"\tab") || 
				Regex.IsMatch(lineRtf, @"\\cf[0-9]+$"))
			{
				if (lineRtf.EndsWith(" "))
				{
					sEndLine = " " + sEndLine;
					lineRtf = lineRtf.Remove(lineRtf.LastIndexOf(" "), 1);
				}
				else if (lineRtf.EndsWith(@"\tab"))
				{
					sEndLine = @"\tab" + sEndLine;
					lineRtf = lineRtf.Remove(lineRtf.LastIndexOf(@"\tab"), 4);
				}
				else
				{
					int iSub = Regex.Match(lineRtf, @"\\cf[0-9]+$").Index;
					int iLen = Regex.Match(lineRtf, @"\\cf[0-9]+$").Length;
					sEndLine = lineRtf.Substring(iSub, iLen) + sEndLine;
					lineRtf = lineRtf.Remove(iSub, iLen);					
				}
			}
			if (lineRtf.IndexOf(@"\highlight0") == -1)
			{
				newRtf += lineRtf + @"\highlight0";
				if (sEndLine[0] != '\\') 
					sEndLine = " " + sEndLine;
			}
			else
				newRtf += lineRtf;
			newRtf += sEndLine;
			newRtf += GetRtfFromLineNum (oldRtf, LineNumber);

			_bColoringBkGround = true;
			this._Paint = false;

			TempRTB.Rtf = txtDocument.Rtf;
			txtDocument.Visible = false;

			txtDocument.Rtf = newRtf;

			MoveCaretToLine(LineNumber);
			txtDocument.Visible = true;

			this._Paint = true;
			_bColoringBkGround = false;
			_bDocumentSaved = wasSaved;
		}

		/// <summary>
		/// this function remove the color of the background of line number LineNumber
		/// </summary>
		/// <remarks>
		/// this function assume that there is background color in LineNumber
		/// if there isn't than the function remove the next background it found if at all
		/// </remarks>
		private void UnColorBackGroundLine(int LineNumber)
		{	
			if (LineNumber < 1) 
				LineNumber = 1;
			bool wasSaved = _bDocumentSaved;
			string oldRtf = txtDocument.Rtf;
			string newRtf = GetRTFHeader(ref oldRtf);
			newRtf += GetRtfTillLineNum(oldRtf, LineNumber);
			string lineRtf = GetRTFLine (oldRtf, LineNumber);
			int iSubString;
			string sPrefix = "";
		
			if (LineNumber == 1)
			{
				iSubString = newRtf.LastIndexOf (@"\highlight");
				if ((lineRtf.Length > 0) && (lineRtf[0] == ' '))
					lineRtf = lineRtf.Remove(0, 1);
				if (iSubString == -1) 
				{
					int iParPos = lineRtf.IndexOf (@"\par");
					iSubString = lineRtf.IndexOf (@"\highlight");
					if ((iParPos <= iSubString) || (iSubString == -1)) return; //there is no highlight on first line
					lineRtf = lineRtf.Remove (iSubString, @"\highlight".Length);
					while (lineRtf[iSubString] != '\\' && lineRtf[iSubString] != ' ')
						lineRtf = lineRtf.Remove (iSubString, 1);
				}
				else
				{
					newRtf = newRtf.Remove (iSubString, @"\highlight".Length);
					while (newRtf[iSubString] != '\\' && newRtf[iSubString] != ' ')
						newRtf = newRtf.Remove (iSubString, 1);
				}
			}
			else
			{	
				iSubString = lineRtf.IndexOf(@"\highlight");
				if (iSubString == -1) return;
				sPrefix = lineRtf.Substring (0, iSubString);
				newRtf += sPrefix;
				lineRtf = lineRtf.Remove (0, iSubString + @"\highlight".Length);
				lineRtf = lineRtf.Remove (0, Regex.Match(lineRtf, "^[0-9]+").Length);
			}
			iSubString = lineRtf.IndexOf(@"\highlight0");

			if (iSubString >= 0) 
			{
				if ((LineNumber != 1) && (lineRtf[0] != '\\'))
				{
					lineRtf = lineRtf.Remove (0,1);
					--iSubString;
				}
				string sTempLine = lineRtf.Substring (0, iSubString);
				lineRtf = lineRtf.Remove (0, iSubString + @"\highlight0".Length);
				if (sPrefix.EndsWith(@"\tab") || (Regex.IsMatch(sPrefix, @"\\cf[0-9]+$")))
					sTempLine = " " + sTempLine;
				newRtf += sTempLine;
				bool bLastWordIsOrder = false;
				while (!bLastWordIsOrder && (sTempLine.EndsWith(" ") || sTempLine.EndsWith(@"\tab") || 
					Regex.IsMatch(sTempLine, @"\\cf[0-9]+$")))
				{
					if (lineRtf.EndsWith(" "))
						sTempLine = sTempLine.Remove(sTempLine.LastIndexOf(" "), 1);
					else if (lineRtf.EndsWith(@"\tab"))
						sTempLine = sTempLine.Remove(sTempLine.LastIndexOf(@"\tab"), 4);
					else
					{
						int iSub = Regex.Match(sTempLine, @"\\cf[0-9]+$").Index;
						int iLen = Regex.Match(sTempLine, @"\\cf[0-9]+$").Length;
						sTempLine = sTempLine.Remove(iSub, iLen);
						bLastWordIsOrder = true;
					}
				}
				if (bLastWordIsOrder) newRtf+= " ";
			}

			if ((lineRtf.Length > 0) && (lineRtf[0] != '\\')) lineRtf = lineRtf.Remove (0,1);
			newRtf += lineRtf;
			newRtf += GetRtfFromLineNum (oldRtf, LineNumber);

			_bColoringBkGround = true;
			this._Paint = false;

			TempRTB.Rtf = txtDocument.Rtf;
			txtDocument.Visible = false;

			txtDocument.Rtf = newRtf;

			MoveCaretToLine(LineNumber);
			txtDocument.Visible = true;

			this._Paint = true;
			_bColoringBkGround = false;
			_bDocumentSaved = wasSaved;
		}


		/// <summary>
		/// gets the RTF and returns only the text without the rich format
		/// </summary>
		/// <param name="Rtf">from RTF</param>
		/// <returns>cleen text</returns>
		private string GetTextFromRtf (string Rtf)
		{
			GetRTFHeader (ref Rtf);
			//			int iPosition = Rtf.IndexOf(" ") + 1;
			//			if ((iPosition > 0) && (Rtf.Substring(0, iPosition).IndexOf(@"\tab") > 0)) 
			//				Rtf = "	" + Rtf.Remove(0, iPosition);
			//			else
			//				Rtf = Rtf.Remove(0, iPosition);
			Rtf = Regex.Replace(Rtf,@"\\par","");
			Rtf = Regex.Replace(Rtf,@"\\cf[0-9]+ ","");
			Rtf = Regex.Replace(Rtf,@"\\cf[0-9]+","");
			Rtf = Regex.Replace(Rtf,@"\\tab ","	");
			Rtf = Regex.Replace(Rtf,@"\\tab","	");
			Rtf = Regex.Replace(Rtf,@"\\\\",@"\");
			Rtf = Regex.Replace(Rtf,@"\\highlight[0-9]+ ","");
			Rtf = Regex.Replace(Rtf,@"\\highlight[0-9]+","");
			Rtf = Regex.Replace(Rtf,((char)0xd).ToString(),"");
			Rtf = Rtf.Remove (Rtf.LastIndexOf ('}') - 1,Rtf.Length - (Rtf.LastIndexOf ('}') - 1));
			return Rtf;
		}


		/// <summary>
		/// this function get line and return its position on the start of the line
		/// </summary>
		/// <param name="LineNumber">the line number</param>
		/// <returns>the position on the start of the line</returns>
		public int GetPositionFromLine (int LineNumber)
		{
			int iLine = 0;
			for (; (iLine < txtDocument.Lines.Length) &&
				(txtDocument.GetLinePosition (iLine) < txtDocument.GetLinePosition (LineNumber - 1))
				; ++iLine);
			if ((iLine <= txtDocument.Lines.Length) && (iLine > 0))
				return txtDocument.GetLinePosition (iLine);
			else
				return 0;
		}

		/// <summary>
		/// this procedure move the caret to begining of LineNumber
		/// </summary>
		/// <param name="LineNumber">the number of line to move caret to</param>
		public void MoveCaretToLine (int LineNumber)
		{
			_bStopUpdateLocation = true;
			txtDocument.Select(GetPositionFromLine (Math.Max(LineNumber - 3, 0)),0);
			txtDocument.Select(GetPositionFromLine (Math.Min(LineNumber + 3, txtDocument.Lines.Length)),0);
			txtDocument.Select(GetPositionFromLine (LineNumber),0);
			_bStopUpdateLocation = false;
		}


		/// <summary>
		/// returns tipical empty header
		/// </summary>
		/// <remarks>don't forget that need to follow space or \per and end with }</remarks>
		/// <returns></returns>
		private string MakeEmptyHeader ()
		{
			return @"{\rtf1\fbidis\ansi\ansicpg1255\deff0\deflang1037{\fonttbl{\f0\fnil\fcharset0 Courier New;}}" 
				+ '\n' + @"\viewkind4\uc1\pard\ltrpar\f0\fs20";
		}


		/// <summary>
		/// this function gets regular text and change it to RTF without colors or someting
		/// </summary>
		/// <param name="sText">the original text</param>
		/// <returns>Rich text format</returns>
		public string GetRTFfromText (string sText)
		{
			string sRTF = MakeEmptyHeader();
			if ((sText.Length > 1) || (sText != "\n"))
				sRTF += " ";
			sText = sText.Replace(@"\",@"\\");
			sText = sText.Replace("\r","");
			sText = sText.Replace("\n","\\par\r\n");
			sText = sText.Replace("	",@"\tab ");
			return sRTF + sText + "}";
		}


		/// <summary>
		/// getting line, removing colors, prepering for add color to line
		/// </summary>
		/// <param name="sLine">the line in RTF</param>
		/// <returns>the line without coloring in RTF</returns>
		private string BuildRTFLine (string sLine)
		{
			sLine = Regex.Replace(sLine,@"\\cf[0-9]+ ","");
			sLine = Regex.Replace(sLine,@"\\cf[0-9]+","");
			return sLine;
		}

		/// <summary>
		/// add color to RTF line
		/// </summary>
		/// <param name="sLine">the RTF line</param>
		/// <param name="Position">from where to color</param>
		/// <param name="iColor">the color index</param>
		/// <returns>the RTF line</returns>
		private string AddColorToRTFLine (string sLine, int iPosition, int iColor)
		{
			int iRealPosition = 0, iCurrentPosition = 0;
			bool bWasSyntax = false;
			while (iCurrentPosition < iPosition)
			{
				bWasSyntax = true;
				string sTempLine = sLine.Substring (iRealPosition);
				if (Regex.IsMatch (sTempLine,@"^\\cf[0-9]+ "))
					iRealPosition += Regex.Match (sTempLine,@"^\\cf[0-9]+ ").Length;
				else if	(Regex.IsMatch (sTempLine,@"^\\tab "))
				{
					iRealPosition += 5;
					++iCurrentPosition;
				}
				else if (Regex.IsMatch (sTempLine,@"^\\cf[0-9]+"))
					iRealPosition += Regex.Match (sTempLine,@"^\\cf[0-9]+").Length;
				else if	(Regex.IsMatch (sTempLine,@"^\\tab"))
				{
					iRealPosition += 4;
					++iCurrentPosition;
				}
				else if (Regex.IsMatch (sTempLine,@"^\\highlight[0-9]+ "))
					iRealPosition += Regex.Match (sTempLine,@"^\\highlight[0-9]+ ").Length;
				else if (Regex.IsMatch (sTempLine,@"^\\highlight[0-9]+"))
					iRealPosition += Regex.Match (sTempLine,@"^\\highlight[0-9]+").Length;
				else
				{
					++iRealPosition;
					++iCurrentPosition;
					bWasSyntax = false;
				}
			}
			if (iCurrentPosition == iPosition)
			{
				if ((bWasSyntax) && (sLine[iRealPosition] == ' '))
					--iRealPosition;
				return sLine.Insert (iRealPosition, @"\cf" + iColor.ToString() + " ");
			}
			else
				throw new VAX11Internals.PanicException();
		}


		#endregion

		#region find, replace

		/// <summary>
		/// Find string in the text and move the cursor to his position
		/// </summary>
		/// <param name="sFind">the text that needed to be found</param>
		/// <param name="bSearchUp">if need to serch up or down</param>
		/// <param name="bCase">if case is sensitive</param>
		/// <returns>return if found </returns>
		public bool FindPosition (string sFind, bool bSearchUp, bool bCase)
		{
			int iPos, iLastPosition ;
			
			if (bSearchUp)
			{
				iLastPosition = 0;
				iPos = txtDocument.SelectionStart - 1;
			}
			else
			{
				iLastPosition = RealText.Length;
				iPos = txtDocument.SelectionStart+txtDocument.SelectionLength;
			}
			while ((bSearchUp) ? iPos > iLastPosition : iPos < iLastPosition)
			{
				int iStartPos = iPos;
				int iInternalPos = 0;
				for (; iInternalPos < sFind.Length; ++iInternalPos)
				{
					if ((sFind[iInternalPos] == RealText[iPos]) || ((!bCase) && 
						sFind[iInternalPos].ToString().ToUpper() == RealText[iPos].ToString().ToUpper()))
						++iPos;
					else
					{
						if (bSearchUp) iPos = iStartPos - iInternalPos ;
						break;
					}
					if ((bSearchUp) ? iPos < iLastPosition : iPos > iLastPosition) break;
				}
				if (iInternalPos == sFind.Length)
				{
					txtDocument.SelectionStart = iStartPos;
					txtDocument.SelectionLength = sFind.Length;
					return true;
				}
				if (bSearchUp) --iPos;
				else ++iPos;
			}
			return false;
		}
		#endregion

		#region BreakPoint list and debug mode coloring

		/// <summary>
		/// this function color all the break points that in bList and Step bkground color
		/// </summary>
		public void ColorAllBreakPoints()
		{
			bList.GetFirst();
			while (bList.GetNext()) 
			{
				if (bList.GetCurrentLine() != BackGroundLine)
					ColorBackGroundLine(bList.GetCurrentLine(), Settings.Environment.BREAKPOINT_COLOR);
			}
		}


		/// <summary>
		/// this function clean all the background colors without cleaning the bList
		/// </summary>
		public void ClearAllBreakPoints()
		{
			bList.GetFirst();
			while (bList.GetNext()) 
			{
				if (bList.GetCurrentLine() != BackGroundLine)
					UnColorBackGroundLine(bList.GetCurrentLine());
			}
		}
	

		/// <summary>
		/// this function color the step by step back ground line
		/// </summary>
		/// <param name="iLine">which line to color</param>
		public void ColorStep (int iLine)
		{
			ClearStep();
			if (bList.Contains (iLine))
				UnColorBackGroundLine(iLine);
			ColorBackGroundLine(iLine, Settings.Environment.DEBUG_LINE_COLOR);
			BackGroundLine = iLine;
		}

		public void ColorStep ()
		{
			if (BackGroundLine != -1)
                ColorStep(BackGroundLine);
		}

		public void ClearStep()
		{
			int iTempPosition = txtDocument.SelectionStart;
			int iTempLength = txtDocument.SelectionLength;

			if (BackGroundLine != -1)
				UnColorBackGroundLine(BackGroundLine);
			if (bList.Contains (BackGroundLine))
				ColorBackGroundLine(BackGroundLine, Settings.Environment.BREAKPOINT_COLOR);
			BackGroundLine = -1;

			txtDocument.Select(iTempPosition, iTempLength);
		}

		public void ColorErrorLine (int iLine)
		{
			if (ErrorLine != -1)
			{
				UnColorBackGroundLine (ErrorLine);
				if (bList.Contains(ErrorLine))
					ColorBackGroundLine(ErrorLine, Settings.Environment.BREAKPOINT_COLOR);
			}
			if (bList.Contains (iLine))
				UnColorBackGroundLine (iLine);
			ColorBackGroundLine(iLine, Color.LightSteelBlue);
			ErrorLine = iLine;
		}
		public void ClearErrorLine()
		{
			int iTempPosition = txtDocument.SelectionStart;
			int iTempLength = txtDocument.SelectionLength;

			if (ErrorLine != -1)
				UnColorBackGroundLine(ErrorLine);
			if (bList.Contains (ErrorLine))
				ColorBackGroundLine(ErrorLine, Settings.Environment.BREAKPOINT_COLOR);
			ErrorLine = -1;

			txtDocument.Select(iTempPosition, iTempLength);
		}
		/// <summary>
		/// Adds a break-point and color it background
		/// </summary>
		/// <param name="iPC">PC of the break-point</param>
		public void AddBreakPoint(int iLine, int iPC)
		{
			bList.AddEntry(new BreakPointEntry(iLine, iPC));
			if ((BackGroundLine != iLine) && (ErrorLine != iLine))
			{
				int iTempPosition = txtDocument.SelectionStart;
				int iTempLength = txtDocument.SelectionLength;

				ColorBackGroundLine(iLine, Settings.Environment.BREAKPOINT_COLOR);

				txtDocument.Select(iTempPosition, iTempLength);
			}
		}

		/// <summary>
		/// Deletes a break-point and remove it background
		/// </summary>
		/// <param name="iPC">PC of the break-point</param>
		public void DelBreakPoint(int iLine)
		{
			bList.DelEntry(iLine);
			if ((BackGroundLine != iLine) && (ErrorLine != iLine))
			{
				int iTempPosition = txtDocument.SelectionStart;
				int iTempLength = txtDocument.SelectionLength;

				UnColorBackGroundLine (iLine);

				txtDocument.Select(iTempPosition, iTempLength);
			}
		}


		/// <summary>
		/// this procedure color the line it the right background color, 
		/// before calling this procedure, need to uncolor line
		/// </summary>
		/// <param name="iLine">the line that need to be colored</param>
		public void ColorBackGround(int iLine)
		{
			if (BackGroundLine == iLine)
				ColorBackGroundLine(iLine, Settings.Environment.DEBUG_LINE_COLOR);
			else if (ErrorLine == iLine)
				ColorBackGroundLine(iLine, Color.LightSteelBlue);
			else if (bList.Contains (iLine))
				ColorBackGroundLine(iLine, Settings.Environment.BREAKPOINT_COLOR);
		}

		#endregion

		#region help

		public int GetCurLine()
		{
			return txtDocument.GetLineFromCharIndex(txtDocument.SelectionStart) + 1;
		}

		/// <summary>
		/// Gets the current word
		/// </summary>
		/// <returns>the current word or empty string</returns>
		public string GetCurrentWord()
		{
			// set first line and last line as the current position
			int iStartPosition = txtDocument.SelectionStart;
			int iEndPosition = txtDocument.SelectionStart;

			// if we're on end of the text, go one character back to find the last word
			if (iStartPosition == TextSize && TextSize > 0) --iStartPosition;

			// find the start of the word
			while (iStartPosition > 0 && iStartPosition < TextSize &&
				( Regex.IsMatch(RealText[iStartPosition].ToString(),@"(\w)+")))
				--iStartPosition;

			// find the end of the word
			while ((iEndPosition < TextSize) && 
				( Regex.IsMatch(RealText[iEndPosition].ToString(),@"(\w)+")))
				++iEndPosition;
			
			// return the word
			if (iStartPosition < TextSize ) return RealText.Substring(iStartPosition, iEndPosition - iStartPosition);
			else return "";
		}

		#endregion

		#region Popup Menu

		private void mnuPopupCut_Click(object sender, System.EventArgs e)
		{
			Cut();
		}

		private void mnuPopupCopy_Click(object sender, System.EventArgs e)
		{
			Copy();
		}

		private void mnuPopupPaste_Click(object sender, System.EventArgs e)
		{
			Paste();
		}

		private void mnuPopupDelete_Click(object sender, System.EventArgs e)
		{
			Delete();
		}

		private void mnuPopupSelectAll_Click(object sender, System.EventArgs e)
		{
			SelectAll();
		}

		#endregion

		#region runtime compile thread				RUNTIME_COMPILER
			/*private bool NeedCompiling = false;
			private System.Threading.AutoResetEvent eWaitForCompile;
			private Thread RuntimeCompileThread;
			private void RuntimeCompile()
			{
				bool NowCompiling = false;
				while (true)
				{
					eWaitForCompile.WaitOne();
					if (NowCompiling) eWaitForCompile.WaitOne();
					NowCompiling = true;
					NeedCompiling = false;
					_HandleErrorFunction(0, CompilerMessage.CLEAR_TASK_LIST);
					try
					{
						_assembler.CompileCode(RealText);
					}
					catch (CompileError)
					{
					}
					NowCompiling = false;
					if (NeedCompiling) eWaitForCompile.Set();
				}
			}*/
		#endregion
	}
}