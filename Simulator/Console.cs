using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Data;
using System.Drawing.Imaging;
using System.Timers;

using VAX11Internals;
using VAX11Settings;

namespace VAX11Simulator
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Console : System.Windows.Forms.Form
	{
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Console));
			this.ScrollBar = new System.Windows.Forms.VScrollBar();
			this.mnuPopupMain = new System.Windows.Forms.ContextMenu();
			this.mnuConsoleCopyAll = new System.Windows.Forms.MenuItem();
			this.mnuConsolePaste = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.mnuConsoleSaveOutputToFile = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.mnuConsoleAlwaysOnTop = new System.Windows.Forms.MenuItem();
			this.dotNetMenuProvider1 = new DotNetWidgets.DotNetMenuProvider();
			this.imgListtoolBar = new System.Windows.Forms.ImageList(this.components);
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.SuspendLayout();
			// 
			// ScrollBar
			// 
			this.ScrollBar.CausesValidation = false;
			this.ScrollBar.Cursor = System.Windows.Forms.Cursors.Default;
			this.ScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.ScrollBar.LargeChange = 24;
			this.ScrollBar.Location = new System.Drawing.Point(722, 0);
			this.ScrollBar.Maximum = 23;
			this.ScrollBar.Name = "ScrollBar";
			this.ScrollBar.Size = new System.Drawing.Size(17, 426);
			this.ScrollBar.TabIndex = 0;
			this.ScrollBar.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ScrollBar_KeyUp);
			this.ScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.ScrollBar_Scroll);
			// 
			// mnuPopupMain
			// 
			this.mnuPopupMain.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.mnuConsoleCopyAll,
																						 this.mnuConsolePaste,
																						 this.menuItem3,
																						 this.mnuConsoleSaveOutputToFile,
																						 this.menuItem5,
																						 this.mnuConsoleAlwaysOnTop});
			// 
			// mnuConsoleCopyAll
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuConsoleCopyAll, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuConsoleCopyAll, 1);
			this.mnuConsoleCopyAll.Index = 0;
			this.mnuConsoleCopyAll.Text = "&Copy All";
			this.mnuConsoleCopyAll.Click += new System.EventHandler(this.mnuConsoleCopyAll_Click);
			// 
			// mnuConsolePaste
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuConsolePaste, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuConsolePaste, 2);
			this.mnuConsolePaste.Index = 1;
			this.mnuConsolePaste.Text = "&Paste";
			this.mnuConsolePaste.Click += new System.EventHandler(this.mnuConsolePaste_Click);
			// 
			// menuItem3
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem3, true);
			this.menuItem3.Index = 2;
			this.menuItem3.Text = "-";
			// 
			// mnuConsoleSaveOutputToFile
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuConsoleSaveOutputToFile, true);
			this.dotNetMenuProvider1.SetImageIndex(this.mnuConsoleSaveOutputToFile, 0);
			this.mnuConsoleSaveOutputToFile.Index = 3;
			this.mnuConsoleSaveOutputToFile.Text = "&Save Output to File";
			this.mnuConsoleSaveOutputToFile.Click += new System.EventHandler(this.mnuConsoleSaveOutputToFile_Click);
			// 
			// menuItem5
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.menuItem5, true);
			this.menuItem5.Index = 4;
			this.menuItem5.Text = "-";
			// 
			// mnuConsoleAlwaysOnTop
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuConsoleAlwaysOnTop, true);
			this.mnuConsoleAlwaysOnTop.Index = 5;
			this.mnuConsoleAlwaysOnTop.Text = "&Always On Top";
			this.mnuConsoleAlwaysOnTop.Click += new System.EventHandler(this.mnuConsoleAlwaysOnTop_Click);
			// 
			// dotNetMenuProvider1
			// 
			this.dotNetMenuProvider1.ImageList = this.imgListtoolBar;
			this.dotNetMenuProvider1.OwnerForm = this;
			// 
			// imgListtoolBar
			// 
			this.imgListtoolBar.ColorDepth = System.Windows.Forms.ColorDepth.Depth16Bit;
			this.imgListtoolBar.ImageSize = new System.Drawing.Size(16, 16);
			this.imgListtoolBar.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListtoolBar.ImageStream")));
			this.imgListtoolBar.TransparentColor = System.Drawing.Color.FromArgb(((System.Byte)(189)), ((System.Byte)(189)), ((System.Byte)(189)));
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.FileName = "Output.txt";
			this.saveFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
			// 
			// Console
			// 
			this.AutoScale = false;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(739, 426);
			this.ContextMenu = this.mnuPopupMain;
			this.Controls.Add(this.ScrollBar);
			this.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Console";
			this.Text = "Console Window";
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Console_KeyDown);
			this.Closing += new System.ComponentModel.CancelEventHandler(this.Console_Closing);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Console_KeyPress);
			this.VisibleChanged += new System.EventHandler(this.Console_VisibleChanged);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Console_Paint);
			this.ResumeLayout(false);

		}
		#endregion

		private System.ComponentModel.IContainer components;

		#region Members

		private bool bTextMode = true; //start in text mode
	
			#region interrupts

		public const int INIT_CLOCK = 1;
		public const int ABORT_CLOCK = 0;
		private bool _activeTimer = false;

			#endregion

			#region text mode
		private const int LineLength = 80;
		private const int NumOfLines = 25;
		private int TextX = 745;
		private int TextY = 450;
		private int caretX, caretY;
		private Pen textpen;

		private ArrayList ScreenBuffer;//saves the text on screen
		private CodeBlock InputBuffer; //saves the chars that pressed that not have been read.
		private int UpperLine; //saves the num of the first line that shown in the console

		private int characterWidth = 9;
		private int characterHeight = 14;
		private int lineSpace = 3;

		private bool caretVisible = true;
		private Thread caretThread;	
		public static ArrayList AllThreads = new ArrayList(); //saves all the threads in all consoles

		private bool bNowInInputFunction; //for release the lock after enter.
		private bool bNowExiting = false; //for don't return the last char and for release the lock
		private bool bWritting = false; //for allowing get outside the the borders
		private int iBufferPrinted = 0; //for printing the input buffer

		private System.Timers.Timer NeedToRepaint;
		private bool bTextChanged = false;

		#endregion

			#region graph mode
		private Graphics graphics;
		//IntPtr hdc;
		private int GraphicX = 647;
		private int GraphicY = 506;
		private Color BkGroundColor = Color.White;
		private Color FgColor = Color.Black;
		private Pen GraphPen;
		private Font Graphfont;
		private System.Drawing.Bitmap PrevImage;
			#endregion

			#region Popup Menu and scroller
		private System.Windows.Forms.VScrollBar ScrollBar;
		private System.Windows.Forms.ContextMenu mnuPopupMain;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem mnuConsoleCopyAll;
		private System.Windows.Forms.MenuItem mnuConsolePaste;
		private System.Windows.Forms.MenuItem mnuConsoleSaveOutputToFile;
		private System.Windows.Forms.MenuItem mnuConsoleAlwaysOnTop;
		private DotNetWidgets.DotNetMenuProvider dotNetMenuProvider1;
		private System.Windows.Forms.ImageList imgListtoolBar;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
			#endregion

		private string _sInputFileName = "";
		private string _sOutputFileName = "";
		StreamWriter fOutputFile = null;
		StreamReader fInputFile = null;

		#endregion

		#region Properites

		public bool activeTimer
		{
			set 
			{ 
				if ((!_activeTimer) && (value)) InterruptsEvent(SimulatorEvents.CLOCK_INTERRUPT, INIT_CLOCK);
				else if ((_activeTimer) && (!value)) InterruptsEvent(SimulatorEvents.CLOCK_INTERRUPT, ABORT_CLOCK);
				_activeTimer = value; 
			}
			get { return _activeTimer; }

			
		}

			#region text mode
		/// <summary>
		/// 0-79 (LineLength - 1)
		/// </summary>
		public int x
		{
			get 
			{
				if (!bTextMode) 
				{
					caretThread.Abort();
					throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_GRAPH_MODE,-1);
				}
				return caretX;
			}
			set 
			{
				if (!bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_GRAPH_MODE,-1);
				if ((value < 0 || value >= LineLength) && (!bWritting)) 
					throw new RuntimeError(SimulatorMessage.ACCESS_OUT_OF_CONSOLE, -1);
				caretX = value;
			}
		}

		/// <summary>
		/// 0-last_line
		/// </summary>
		public int y
		{
			get 
			{
				if (!bTextMode) 
				{
					caretThread.Abort();
					throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_GRAPH_MODE,-1);
				}
				return caretY;
			}
			set 
			{
				if (!bTextMode) //in the same page.
				{
					caretThread.Abort();
					throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_GRAPH_MODE,-1);
				}
				int DownLine = (ScreenBuffer.Count - NumOfLines < UpperLine ) ? ScreenBuffer.Count : UpperLine + NumOfLines - 1;
				if ((value < UpperLine || value >= DownLine) && (!bWritting))
					throw new RuntimeError(SimulatorMessage.ACCESS_OUT_OF_CONSOLE, -1);
				caretY = value;
			}
		}
		#endregion

			#region graph mode

		public int MaxX
		{
			get
			{
				if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
				return GraphicX;
			}
		}

		public int MaxY
		{
			get
			{
				if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
				return GraphicY;
			}
		}

			#endregion

		#endregion

		#region init

			#region text mode

		/// <summary>
		/// Clear screen and input buffer
		/// </summary>
		public void ClearScreen()
		{
			if (!bTextMode) 
			{
				caretThread.Abort();
				throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_GRAPH_MODE,-1);
			}
			ScreenBuffer = new ArrayList();
			InputBuffer = new CodeBlock();
			ScreenBuffer.Insert(0, new CodeBlock());
			caretX =caretY = 0;
			this.Invalidate();
		}

			#endregion

			#region graph mode

		public void InitGraph()
		{
			bTextMode = false;
			caretVisible = false;
			caretThread.Abort();
			this.BackColor = Color.White;
			this.Cursor = Cursors.Default;

			//Graphics graphics = Graphics.FromHwnd (this.Handle) ;
			//hdc = graphics.GetHdc();
			GraphPen = new Pen(FgColor);
			Graphfont = new Font("Courier new", 10);

			ScrollBar.Visible = false;
			this.Size = new System.Drawing.Size (GraphicX, GraphicY);

			PrevImage = new System.Drawing.Bitmap(GraphicX, GraphicY);
			graphics = Graphics.FromImage(PrevImage);
			ClearDevice();
		}

		public void CloseGraph()
		{
			bTextMode = true;
			GraphPen.Dispose();
			this.BackColor = Settings.Simulator.ConsoleBackGroundColor;
			this.Size = new System.Drawing.Size (TextX, TextY);
			ScrollBar.Visible = true;
			this.Invalidate();
			this.Cursor = Cursors.IBeam;
			caretThread = new Thread(new ThreadStart(ShowCaret));
			caretThread.Name = "caret thread";
			AllThreads.Add (caretThread); //add the caret thread to list for closing
			caretThread.Start();
		}

			#endregion

		#endregion

		#region Delegates and Events

		/// <summary>
		/// Delegate for exit console
		/// </summary>
		public delegate void dExitFunction(bool bWaitOneKey);
		public dExitFunction m_Exit;

			#region interrupts
		public delegate void InterruptsDelegate(SimulatorEvents Interrupt, int info);
		public event InterruptsDelegate InterruptsEvent;
			#endregion

			#region text mode
		/// <summary>
		/// Delegate for accessing output
		/// </summary>
		public delegate void dOutputFunction(CodeBlock text);
		public dOutputFunction m_Output;

		/// <summary>
		/// Event will rise on key-press
		/// </summary>
		public delegate void dKeyPressed(char cKey);
		public event dKeyPressed KeyPressed;

		private System.Threading.AutoResetEvent eWaitForInput;

			#endregion

		#endregion

		#region Constructor & Destructor

		/// <summary>
		/// Constructor
		/// </summary>
		public Console(string sInputFile, string sOutputFile)
		{
			_sInputFileName = sInputFile;
			_sOutputFileName = sOutputFile;

			InitializeComponent();
			textpen = new Pen(Settings.Simulator.ConsoleTextColor); //color of the init text
			this.BackColor = Settings.Simulator.ConsoleBackGroundColor;
			
			caretThread = new Thread(new ThreadStart(ShowCaret));
			caretThread.Name = "caret thread";
			AllThreads.Add (caretThread); //add the caret thread to list for closing
			caretThread.Start();

			m_Output	= new dOutputFunction(this.Write);
			m_Exit		= new dExitFunction(this.exitConsole);

			ScreenBuffer = new ArrayList();
			ScreenBuffer.Insert(0, new CodeBlock());
			InputBuffer = new CodeBlock();
			//geting input from input file
			if (_sInputFileName != "")
			{
				fInputFile = new StreamReader(_sInputFileName);
				string sTempReadFromFile = fInputFile.ReadToEnd();
				sTempReadFromFile = sTempReadFromFile.Replace("\r\n", "\n");
				InputBuffer += (CodeBlock)sTempReadFromFile;
				InputBuffer += 0xff;
				fInputFile.Close();
			}

			bNowInInputFunction = false;

			eWaitForInput = new AutoResetEvent(false);

			PrevImage = new System.Drawing.Bitmap(GraphicX, GraphicY);

			NeedToRepaint = new System.Timers.Timer();
			NeedToRepaint.Elapsed+=new ElapsedEventHandler(ConsoleUpdateTime);
			NeedToRepaint.Interval = 100;
			NeedToRepaint.Enabled = false;

			
		}	


		
		/// <summary>
		/// Clean up any resources being used. and stop events
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

		#endregion

		#region Output

			#region text mode
		/// <summary>
		/// this function gets string and add it to the current location, it overwrite on old
		/// text if exsist
		/// </summary>
		/// <param name="text">the text that need to print</param>
		public void Write (CodeBlock text)
		{
			if (!bTextMode) 
			{
				caretThread.Abort();
				throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_GRAPH_MODE,-1);
			}
			lock(this)
			{
				DoWrite(text);
				Thread.Sleep(1); // wait for slowing output so the simulator will do more work
			}
		}

		/// <summary>
		/// this function actualy write on the console
		/// </summary>
		/// <param name="text">the text that need to print</param>
		private void DoWrite (CodeBlock text)
		{
			bWritting = true;
			bTextChanged = true;
			int FirstX = x;
			bool bAddChar; //check if need to add new char or need to overwrite 
			for (int pos = 0; pos < text.Size; ++pos)
			{
				if (ScrollBar.Maximum != ScrollBar.Value)
				{
					ScrollBar.Value = ScrollBar.Maximum;
					if (ScrollBar.Maximum - NumOfLines + 2 > 0)
						UpperLine = ScrollBar.Maximum - NumOfLines + 2;
					else
						UpperLine = 0;
					this.Invalidate();
				}
				bAddChar = true;
				switch (text[pos]) //for spaceal chars
				{
					case 0x08 : //case we got backspace
						if ((x > 0) && (y >= 0))
							if (((CodeBlock)ScreenBuffer[y])[x-1] == (char) 0x09)
							{
								FirstX = x;
								for (int i = (x % 8) ;((x > 0) &&((CodeBlock)ScreenBuffer[y])[x-1] == (char) 0x09) && ((i % 8 != 0) || (bAddChar)); --i)
								{
									if (((CodeBlock)ScreenBuffer[y]).Size != 0)
										ScreenBuffer[y] = ((CodeBlock)ScreenBuffer[y]).SubBlock(0,((CodeBlock)ScreenBuffer[y]).Size - 1);
									if ( x > 0 ) --x;
									bAddChar = false;
								}
								this.Invalidate(new Rectangle((FirstX - x)*characterWidth, (y - UpperLine)*(lineSpace+characterHeight),
									characterWidth*FirstX+2,(characterHeight+lineSpace)));
								continue;
							}
						if (x == 0) 
						{
							if (y > 0)
							{//go up one line
								ScreenBuffer.RemoveAt(y);
								ScreenBuffer.Capacity = y;
								FirstX = x = ((CodeBlock)ScreenBuffer[--y]).Size;
								this.Invalidate(new Rectangle(0, (y - UpperLine +1)*(lineSpace+characterHeight),
									characterWidth,(characterHeight+lineSpace)));
								if (y < UpperLine) 
								{
									--UpperLine;
									if (ScrollBar.Maximum > 23) ScrollBar.Value = --ScrollBar.Maximum;
									this.Invalidate(new Rectangle(0, 0, 
										((CodeBlock)ScreenBuffer[(y - UpperLine)]).Size*characterWidth,(characterHeight+lineSpace)));
									ScrollBar.LargeChange = NumOfLines -1;
								}
								else
								{
									if (UpperLine >0) --UpperLine;
									if (ScrollBar.Maximum > 23) ScrollBar.Value = --ScrollBar.Maximum;
									ScrollBar.LargeChange = NumOfLines -1;							
									this.Invalidate();
								}
							}
							else
							{
								continue;
							}	
						}
						//delete one char
						if (((CodeBlock)ScreenBuffer[y]).Size != 0)
							ScreenBuffer[y] = ((CodeBlock)ScreenBuffer[y]).SubBlock(0,((CodeBlock)ScreenBuffer[y]).Size - 1);
						if ( x > 0 ) --x;
						continue;
					case 0x0a : //case we got enter
						x = ((CodeBlock)ScreenBuffer[y]).Size;		
						ScreenBuffer[y] = ((CodeBlock)ScreenBuffer[y]) + (byte)0x0a;
						//x++; if caret will do problems with enter, this is the reason
						if (UpperLine + NumOfLines > ScreenBuffer.Count)
							this.Invalidate(new Rectangle(FirstX*characterWidth, (y - UpperLine)*(lineSpace+characterHeight),
								characterWidth*(x - FirstX)+2,(characterHeight+lineSpace)));
						FirstX = x = 0 ;
						ScreenBuffer.Insert(++y, new CodeBlock());
						if (y >= UpperLine + NumOfLines) //check if needed to draw all one line up
						{
							++UpperLine;
							ScrollBar.Value = ++ScrollBar.Maximum;
							ScrollBar.LargeChange = NumOfLines - 1;
							//this.Invalidate();
							NeedToRepaint.Enabled = true;
						}			
						continue;
					case 0x0d : //case we got return
						this.Invalidate(new Rectangle(FirstX*characterWidth, (y - UpperLine)*(lineSpace+characterHeight),
							characterWidth*(x - FirstX)+2,(characterHeight+lineSpace)));
						FirstX = x = 0 ;
						bAddChar = false;
						continue;
					case 0x09 : //case we got tab
						for (int i = (x % 8) ; (i % 8 != 0) || (bAddChar); ++i)
						{
							ScreenBuffer[y] = ((CodeBlock)ScreenBuffer[y]) + (byte)0x09;
							x++;
							bAddChar = false;
						}
						break;

					case 0x1a : // Control-Z
						DoWrite ((new CodeBlock ())+ '^' + 'Z');
						bAddChar = false;
						break;

					case 0xFF : // Control-Z
						DoWrite ((new CodeBlock ())+ '^' + 'Z');
						bAddChar = false;
						break;

					case 0x04 : // Control-D
						DoWrite ((new CodeBlock ())+ '^' + 'D');
						bAddChar = false;
						break;
				}
				//add the char to screen
				if (bAddChar)
				{
					//check if need to add new char or need to overwrite 
					if (((CodeBlock)ScreenBuffer[y]).Size > x)
						((CodeBlock)ScreenBuffer[y])[x] = (byte)text[pos];
					else
						ScreenBuffer[y] = ((CodeBlock)ScreenBuffer[y]) + (byte)text[pos];
					x++;
				}
				//if needed to get line down.
				if (((CodeBlock)ScreenBuffer[y]).Size == LineLength)
				{
					if (UpperLine + NumOfLines > ScreenBuffer.Count)
						this.Invalidate(new Rectangle(FirstX*characterWidth, (y - UpperLine)*(lineSpace+characterHeight),
							characterWidth*(x - FirstX)+2,(characterHeight+lineSpace)));
					FirstX = x = 0 ;
					y++ ;
					if (y >= UpperLine + NumOfLines) 
					{
						++UpperLine;
						ScrollBar.Value = ++ScrollBar.Maximum;
						ScrollBar.LargeChange = NumOfLines -1;
						//this.Invalidate();
						NeedToRepaint.Enabled = true;
					}
					ScreenBuffer.Insert(y, new CodeBlock());
				}
			}			
			this.Invalidate(new Rectangle(Math.Min(FirstX,x)*characterWidth, (y - UpperLine)*(lineSpace+characterHeight),
				characterWidth*(Math.Abs(x - FirstX))+2,(characterHeight+lineSpace)));
			bWritting = false;
		}


			#endregion

			#region graph mode

		public void PutPixel(int x, int y)
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
//				Graphics graphics = Graphics.FromHdc (hdc);
				bUglyBugFixing = true;
				Rectangle rec = new Rectangle(x, y, 1, 1);
				graphics.DrawEllipse(GraphPen, rec);
				this.Invalidate(new Rectangle(x, y, 1, 1));
			}
		}

		public void Line(int x1, int y1, int x2, int y2)
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
//				Graphics graphics = Graphics.FromHdc (hdc);
				graphics.DrawLine(GraphPen, x1, y1, x2, y2);
				bUglyBugFixing = true;
				this.Invalidate(new Rectangle(Math.Min(x1, x2),Math.Min(y1, y2),
					Math.Abs(x1 - x2), Math.Abs(y1 - y2)));
			}
		}

		public void ClearDevice()
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
				//Graphics graphics = Graphics.FromHdc (hdc);Graphics graphics = Graphics.FromImage (PrevImage);
				SolidBrush Brush = new SolidBrush(BkGroundColor);
				graphics.FillRectangle(Brush, 0, 0, GraphicX, GraphicY);
				//PrevImage.
				bUglyBugFixing = true;
				this.Invalidate();
			}
		}

		public void SetColor(int red, int green, int blue)
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
				GraphPen = new Pen(Color.FromArgb(red,green,blue));
			}
		}
		public void Rectangle(int x1, int y1, int x2, int y2)
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
//				Graphics graphics = Graphics.FromHdc (hdc);
				Rectangle rec = new Rectangle(Math.Min(x1, x2),Math.Min(y1, y2),
					Math.Abs(x1 - x2), Math.Abs(y1 - y2));
				graphics.DrawRectangle (GraphPen, rec);
				bUglyBugFixing = true;
				this.Invalidate(rec);
			}
		}

		public void Circle(int x, int y, int radios)
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
//				Graphics graphics = Graphics.FromHdc (hdc);
				Rectangle rec = new Rectangle(x, y, radios, radios);
				graphics.DrawEllipse(GraphPen, rec);
				bUglyBugFixing = true;
				this.Invalidate(rec);
			}
		}

		public void SetFont(string familyName,int fontSize)
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
				System.Drawing.FontStyle fontstyle = new FontStyle();
				Graphfont = new Font(familyName, fontSize,fontstyle);
			}
		}

		public void OutTextXY(int x, int y, string text)
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
//				Graphics graphics = Graphics.FromHdc (hdc);
				Brush brush = new SolidBrush(GraphPen.Color);
				graphics.DrawString(text, Graphfont, brush, x, y);
				bUglyBugFixing = true;
				this.Invalidate(new Rectangle(x, y, text.Length, Graphfont.Height));
			}
		}

/*		public void Fill(int x, int y)
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
				Brush brush = new SolidBrush(GraphPen.Color);
				System.Drawing.Region points = {new System.Drawing.Region(
												   ((float)x,(float) y)};
				graphics.FillRegion (brush, points);
				bUglyBugFixing = true;
				this.Invalidate();
			}
		}*/

			#endregion

		#endregion

		#region Input


			#region text mode
		/// <summary>
		/// read one char from buffer, if it empty wait until key press
		/// </summary>
		/// <returns>the key's value</returns>
		public byte getchar()
		{
			if (!bTextMode) 
			{
				caretThread.Abort();
				throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_GRAPH_MODE,-1);
			}
			byte res;

			bNowInInputFunction = true;
			if (InputBuffer.Size == 0)
			{
				if (_sInputFileName != "")
					return 0xFF;
				else
					eWaitForInput.WaitOne();
			}
			if (bNowExiting) return 0xFF; // magic number - EOF
			iBufferPrinted--;
			res = InputBuffer[0];
			InputBuffer = InputBuffer.SubBlock(1);

			return res;
		}

		/// <summary>
		/// check if the input buffer is empty
		/// </summary>
		/// <returns>the answer in boolean</returns>
		public bool isEmptyBuffer()
		{
			return (InputBuffer.Size == 0);
		}

		public byte viewFirstChar()
		{
			if (InputBuffer.Size == 0)
				eWaitForInput.WaitOne();
				//throw (new PanicException()); //should use before isEmptyBuffer
            return InputBuffer[0];
		}

		public void unReadChar(byte theChar)
		{
			InputBuffer = theChar + InputBuffer;
		}

			#endregion

		#endregion

		#region events and threads


		bool bUglyBugFixing = true;
		/// <summary>
		/// this function execute when the console need to be updated on screen.
		/// </summary>
		private void Console_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (bTextMode == true)
			{
				Graphics textgraphics = e.Graphics;
				Font font = new Font("Courier new", characterWidth);
				Brush brush = new SolidBrush(Settings.Simulator.ConsoleTextColor);
				int DownLine = (ScreenBuffer.Count - NumOfLines < UpperLine) ? ScreenBuffer.Count : UpperLine + NumOfLines;
				for (int i = 0; i < DownLine - UpperLine; i++) 
				{
					for (int j = 0; j < ((CodeBlock)ScreenBuffer[i + UpperLine]).Size; j++)
					{
						if ((char)((CodeBlock)ScreenBuffer[i + UpperLine])[j] != 0x0a)
						{
							if ((char)((CodeBlock)ScreenBuffer[i + UpperLine])[j] != 0x09)
								textgraphics.DrawString(((char)((CodeBlock)ScreenBuffer[i + UpperLine])[j]).ToString(),
									font, brush, new Point((j*characterWidth),i *(lineSpace+characterHeight)));
							else
								textgraphics.DrawString(" ", font, brush, new Point((j*characterWidth),
									i *(lineSpace+characterHeight)));
						}
					}
				}
				//draw caret
				if (caretVisible)
				{
					int x = caretX * characterWidth;
					int y = (caretY - UpperLine) * (lineSpace + characterHeight) ;
					textgraphics.DrawLine(textpen, x, y, x, y + lineSpace + characterHeight-1);
				}
			}
			else
			{
				Graphics GraphGraphics = e.Graphics;
				GraphGraphics.DrawImage (PrevImage,new Point(0,0));
				if (bUglyBugFixing) Invalidate();
				bUglyBugFixing = !bUglyBugFixing;
				//Graphics graphics = Graphics.FromHdc (hdc);
				//GraphGraphics = graphics;
			}
		}

		/// <summary>
		/// this function added the key that pressed to input buffer
		/// </summary>
		private void Console_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (bNowExiting)
			{
				eWaitForInput.Set();
				return;
			}
			if ((_sInputFileName != "") && (!bNowExiting)) 
				return;
			if (e.KeyChar == 0x1a) // Control-Z
				InterruptsEvent(SimulatorEvents.INPUT_INTERRUPT, 0xff);
			else if (e.KeyChar != 0x03) //Control-Break
				InterruptsEvent(SimulatorEvents.INPUT_INTERRUPT, e.KeyChar);
			else
			{
				this.Close();
				return;
			}
			if (bTextMode == true)
			{
				if (bNowInInputFunction && KeyPressed != null)
					throw new RuntimeError(SimulatorMessage.INPUT_FUNCTIONS_NOT_ALLOWED, -1);

				char cChar = e.KeyChar;
				if (cChar == 0x1a) // Control-Z
					InputBuffer += 0xff;
				else
					InputBuffer += cChar;

				if (bNowInInputFunction)
				{
					if (iBufferPrinted++ > 0)
						DoWrite ((CodeBlock) e.KeyChar);
					else
					{
						DoWrite (InputBuffer);
						iBufferPrinted = InputBuffer.Size;
					}
				}
				if (cChar == 0x08)
				{
					if (iBufferPrinted > 1) 
						iBufferPrinted -= 2;
					else
						--iBufferPrinted;

					if (InputBuffer.Size > 1)
					{
						if ((InputBuffer[InputBuffer.Size - 2] == 0x04) || (InputBuffer[InputBuffer.Size - 2] == 0xff))
							DoWrite(new CodeBlock() + 0x08);
						InputBuffer = InputBuffer.SubBlock(0, InputBuffer.Size - 2);
					}
					else
						InputBuffer = InputBuffer.SubBlock(0, InputBuffer.Size - 1);
				}
				
				if (bNowInInputFunction && cChar == 0x0d) 
				{
					DoWrite ((CodeBlock) 0x0a);
					++iBufferPrinted;
					eWaitForInput.Set();
				}
			}
			
			
		}

		/// <summary>
		/// function to close up the console
		/// </summary>
		/// <param name="bWaitForOneKey">if needed to wait for key</param>
		public void exitConsole(bool bWaitForOneKey)
		{
			// Read one key.
			bNowExiting = true;
			if (bWaitForOneKey)
				eWaitForInput.WaitOne();
			SaveOutputFile();
//			if (_activeTimer) InterruptsEvent(SimulatorEvents.CLOCK_INTERRUPT, ABORT_CLOCK);
//			if (bTextMode) caretThread.Abort();
//			this.Visible = false;
//			System.Windows.Forms.Application.DoEvents();
			this.Close();
		}

		/// <summary>
		/// Starting function for the caret's thread
		/// </summary>
		private void ShowCaret()
		{
			while (true) 
			{
				this.Invalidate(new Rectangle(caretX*characterWidth, (caretY - UpperLine)*(characterHeight+lineSpace),
					characterWidth+2,characterHeight+lineSpace));
				Thread.Sleep(350);
				caretVisible = !caretVisible;
			}
		}


		/// <summary>
		/// window event, called when the form closed
		/// </summary>
		private void Console_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			SaveOutputFile();
			if (_activeTimer) InterruptsEvent(SimulatorEvents.CLOCK_INTERRUPT, ABORT_CLOCK);
			if (bNowExiting) eWaitForInput.Set();
			if (bTextMode) caretThread.Abort();
			if ( bNowExiting ) 
				InterruptsEvent(SimulatorEvents.POWER_DOWN_INTERRUPT, 0);
			else
				InterruptsEvent(SimulatorEvents.POWER_DOWN_INTERRUPT, 1);
		}

		/// <summary>
		/// this procedure is called by the NeedToUpdate timer
		/// </summary>
		private void ConsoleUpdateTime(object source, ElapsedEventArgs e)
		{
			if (bTextChanged)
			{
				bTextChanged = false;
				NeedToRepaint.Enabled = false;
				this.Invalidate();
			}
		}

		/// <summary>
		/// changing the text position
		/// </summary>
		private void ScrollBar_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if (UpperLine != e.NewValue)
				this.Invalidate();
			else return;
			if (e.NewValue + NumOfLines - 2 > ScrollBar.Maximum)
				UpperLine = ScrollBar.Maximum - NumOfLines + 1;
			else
				UpperLine = e.NewValue;
			if (UpperLine > 0)
				e.NewValue = ScrollBar.Value = UpperLine;
			else
				e.NewValue = ScrollBar.Value = UpperLine = 0;
		}

		private void ScrollBar_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			//if (e.KeyValue == (int)System.Windows.Forms.Keys.F11)
			//	this.characterHeight++;//here send to main form, next step
		}

		private void mnuConsoleAlwaysOnTop_Click(object sender, System.EventArgs e)
		{
			if (mnuConsoleAlwaysOnTop.Checked == true)
			{
				this.TopMost = mnuConsoleAlwaysOnTop.Checked = false;
			}
			else
			{
				this.TopMost = mnuConsoleAlwaysOnTop.Checked = true;
			}
		}

		private void Console_VisibleChanged(object sender, System.EventArgs e)
		{
			if (this.Visible == true) mnuConsoleAlwaysOnTop.Checked = this.TopMost;
		}


		/// <summary>
		/// write output file if needed
		/// </summary>
		/// <returns>nothing</returns>
		private void SaveOutputFile()
		{
			if (_sOutputFileName != "")
			{
				fOutputFile = new StreamWriter(_sOutputFileName, false);
				fOutputFile.Write (GetText().Replace("\n","\r\n"));
				fOutputFile.Close();
			}
		}

		#endregion

		#region popup menu

		/// <summary>
		/// gets the text as long stream
		/// </summary>
		/// <returns>all the text</returns>
		private string GetText ()
		{
			string sText = "";
			for (int iLine = 0; iLine < ScreenBuffer.Count ; ++iLine)
			{
				char[] temp = new char[((CodeBlock)ScreenBuffer[iLine]).Size];
				for (int iColunm = 0; iColunm < ((CodeBlock)ScreenBuffer[iLine]).Size; ++iColunm)
					 temp[iColunm] = ((char)((CodeBlock)ScreenBuffer[iLine])[iColunm]);
				sText += new string(temp);
			}
			return sText;
		}

		/// <summary>
		/// save output to file
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnuConsoleSaveOutputToFile_Click(object sender, System.EventArgs e)
		{
			DialogResult res = saveFileDialog1.ShowDialog();
			if (res != DialogResult.OK) return;
			try
			{
				StreamWriter outputFile = new StreamWriter(saveFileDialog1.FileName, false);
				/*string sFile = GetText();
				sFile.Split(new char[] {'\n'});*/
				outputFile.WriteLine(/*sFile*/GetText().Replace("\n","\r\n"));
				outputFile.Close();
				return;
			}
				//TODO: check exception
			catch
			{
				MessageBox.Show("Saving file didn't succseeded");
				return;
			}
		}

		/// <summary>
		/// copy all the text in the console to clip board
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void mnuConsoleCopyAll_Click(object sender, System.EventArgs e)
		{
			Clipboard.SetDataObject(GetText().Replace("\n","\r\n"));
		}

		private void mnuConsolePaste_Click(object sender, System.EventArgs e)
		{
			IDataObject CurrentClip = Clipboard.GetDataObject();
			//if clipboard contains only text
			if (CurrentClip.GetDataPresent(DataFormats.Html)) return;
			if(CurrentClip.GetDataPresent(DataFormats.Text))
			{
				string sPaste = ((string)CurrentClip.GetData(DataFormats.Text)).Replace("\r\n", "\n");
				InputBuffer += (CodeBlock)sPaste;
				for (int index = 0; index < sPaste.Length; ++index)
				{
					if (bNowInInputFunction)
					{
						if (iBufferPrinted++ > 0)
							DoWrite ((CodeBlock) sPaste[index]);
						else
						{
							DoWrite (InputBuffer);
							iBufferPrinted = InputBuffer.Size;
						}
					}
					if (sPaste[index] == 0x08)
					{
						if (iBufferPrinted > 1) 
							iBufferPrinted -= 2;
						else
							iBufferPrinted--;
						if (InputBuffer.Size > 1) 
							InputBuffer = InputBuffer.SubBlock(0, InputBuffer.Size - 2);
						else
							InputBuffer = InputBuffer.SubBlock(0, InputBuffer.Size - 1);
					}
					if ((bNowInInputFunction && sPaste[index] == 0x0d) || (bNowExiting)) 
					{
						DoWrite ((CodeBlock) 0x0a);
						++iBufferPrinted;
						eWaitForInput.Set();
					}
				}
			}
		}

		#endregion

		protected override bool IsInputKey(System.Windows.Forms.Keys keyData)
		{
			if (keyData == Keys.Up || keyData == Keys.Down || keyData == Keys.Right || keyData == Keys.Left) return true;
			return false;
		}
		
		private void Console_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Right || e.KeyCode == Keys.Left)
			{
				e.Handled = true;

			}
		}

	}
}
