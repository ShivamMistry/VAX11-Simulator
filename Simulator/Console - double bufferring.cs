using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using System.Data;
using System.Drawing.Imaging;

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
			// 
			// Console
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.AutoScroll = true;
			this.AutoScrollMargin = new System.Drawing.Size(400, 400);
			this.AutoScrollMinSize = new System.Drawing.Size(400, 400);
			this.ClientSize = new System.Drawing.Size(722, 426);
			this.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Console";
			this.Text = "Console Window";
			this.Closing += new System.ComponentModel.CancelEventHandler(this.Console_Closing);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Console_KeyPress);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Console_Paint);

		}
		#endregion

		#region Members

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private bool bTextMode = true; //start in text mode
		private System.Threading.Thread _activeThread = null;
		private System.Timers.Timer _activeTimer = null;
		private System.Drawing.Bitmap PrevImage;

			#region text mode
		private const int LineLength = 80;
		private const int NumOfLines = 25;
		private int caretX, caretY;
		private Pen textpen;

		private ArrayList ScreenBuffer;//saves the text on screen
		private CodeBlock InputBuffer; //saves the chars that pressed that not been read.
		private int UpperLine; //saves the num of the first line that shown in the consone

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


		#endregion

			#region graph mode
		private Graphics graphics;
		//IntPtr hdc;
		private int _MaxX = 800;
		private int _MaxY = 600;
		private Color BkGroundColor = Color.White;
		private Color FgColor = Color.Black;
		private Pen GraphPen;
		private Font Graphfont;



			#endregion

		#endregion

		#region Properites

		public System.Threading.Thread activeThread
		{
			set { _activeThread = value; }
		}
		
		public System.Timers.Timer activeTimer
		{
			set { _activeTimer = value; }
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
				if (!bTextMode) 
				{
					caretThread.Abort();
					throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_GRAPH_MODE,-1);
				}
				int DownLine = (ScreenBuffer.Count - 25 < UpperLine ) ? ScreenBuffer.Count : UpperLine + 25;
				if ((value < 0 || value >= DownLine) && (!bWritting))
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
				return _MaxX;
			}
		}

		public int MaxY
		{
			get
			{
				if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
				return _MaxY;
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

			//Graphics graphics = Graphics.FromHwnd (this.Handle) ;
			//hdc = graphics.GetHdc();
			GraphPen = new Pen(FgColor);
			Graphfont = new Font("Courier new", 10);

			graphics = Graphics.FromImage(PrevImage);
			ClearDevice();
		}

		public void CloseGraph()
		{
			bTextMode = true;
			caretThread.Start();
			GraphPen.Dispose();

			this.Invalidate();
		}

			#endregion

		#endregion

		#region Delegates and Events

		/// <summary>
		/// Delegate for exit console
		/// </summary>
		public delegate void dExitFunction();
		public dExitFunction m_Exit;

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
		public Console()
		{
			InitializeComponent();
			textpen = new Pen(Settings.Simulator.ConsoleTextColor); //color of the init text
			this.BackColor = Settings.Simulator.ConsoleBackGroundColor;
			
			caretThread = new Thread(new ThreadStart(ShowCaret));
			AllThreads.Add (caretThread); //add the caret thread to list for closing
			caretThread.Start();

			m_Output	= new dOutputFunction(this.Write);
			m_Exit		= new dExitFunction(this.exitConsole);

			ScreenBuffer = new ArrayList();
			InputBuffer = new CodeBlock();
			ScreenBuffer.Insert(0, new CodeBlock());

			bNowInInputFunction = false;

			eWaitForInput = new AutoResetEvent(false);

			PrevImage = new System.Drawing.Bitmap(_MaxX, _MaxY);
		}	


		
		/// <summary>
		/// Clean up any resources being used. and stop events
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if (bNowExiting) eWaitForInput.Set();
			if (bTextMode) caretThread.Abort();
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
		private void Write (CodeBlock text)
		{
			if (!bTextMode) 
			{
				caretThread.Abort();
				throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_GRAPH_MODE,-1);
			}
			lock(this)
			{
				DoWrite(text);
			}
		}

		/// <summary>
		/// this function actualy write on the console
		/// </summary>
		/// <param name="text">the text that need to print</param>
		private void DoWrite (CodeBlock text)
		{
			bWritting = true;
			int FirstX = x;
			bool bAddChar;
			for (int pos = 0; pos < text.Size; ++pos)
			{
				bAddChar = true;
				switch (text[pos]) //for spaceal chars
				{
					case 0x08 : //case we gut backspace
						if (x == 0) 
							if (y > 0)
							{//go up one line
								ScreenBuffer.RemoveAt(y);
								FirstX = x = ((CodeBlock)ScreenBuffer[--y]).Size;
								this.Invalidate(new Rectangle(FirstX*characterWidth, (y+1)*(lineSpace+characterHeight),
									characterWidth,(characterHeight+lineSpace)));
								if (y < UpperLine) 
								{
									--UpperLine;
								}
							}
							else
							{
								continue;
							}	
						//delete one char
						ScreenBuffer[y] = ((CodeBlock)ScreenBuffer[y]).SubBlock(0,((CodeBlock)ScreenBuffer[y]).Size - 1);
						x--;
						continue;
					case 0x0a : //case we gut enter
						if (UpperLine + NumOfLines > ScreenBuffer.Count)
							this.Invalidate(new Rectangle(FirstX*characterWidth, y*(lineSpace+characterHeight),
								characterWidth*(x - FirstX + 1)+2,(characterHeight+lineSpace)));
						FirstX = x = 0 ;
						y++ ;
						ScreenBuffer.Insert(y, new CodeBlock());
						if (y >= UpperLine + NumOfLines) //check if needed to draw all one line up
						{
							++UpperLine;
							this.Invalidate();
						}			
						continue;
					case 0x0d : //case we gut return
						this.Invalidate(new Rectangle(FirstX*characterWidth, y*(lineSpace+characterHeight),
							characterWidth*(x - FirstX + 1)+2,(characterHeight+lineSpace)));
						FirstX = x = 0 ;
						bAddChar = false;
						continue;

				}
				//if needed to get line down.
				if (((CodeBlock)ScreenBuffer[y]).Size == LineLength)
				{
					if (UpperLine + NumOfLines > ScreenBuffer.Count)
						this.Invalidate(new Rectangle(FirstX*characterWidth, y*(lineSpace+characterHeight),
							characterWidth*(x - FirstX + 1)+2,(characterHeight+lineSpace)));
					FirstX = x = 0 ;
					y++ ;
					if (y >= UpperLine + NumOfLines) 
					{
						++UpperLine;
						this.Invalidate();
					}
					ScreenBuffer.Insert(y, new CodeBlock());
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
			}			
			this.Invalidate(new Rectangle(Math.Min(FirstX,x)*characterWidth, y*(lineSpace+characterHeight),
				characterWidth*(Math.Abs(x - FirstX) + 1)+2,(characterHeight+lineSpace)));
			bWritting = false;
		}


		/// <summary>
		/// do double buffering to paint text
		/// </summary>
		private void DrawTextImage ()
		{
			//PrevImage = new System.Drawing.Bitmap(_MaxX, _MaxY);
			Graphics textgraphics = Graphics.FromImage(PrevImage);
			
			Font font = new Font("Courier new", characterWidth);
			Brush brush = new SolidBrush(Settings.Simulator.ConsoleTextColor);
			int DownLine = (ScreenBuffer.Count - 25 < UpperLine) ? ScreenBuffer.Count : UpperLine + 25;
			for (int i = UpperLine; i < DownLine; i++) 
			{
				for (int j = 0; j < ((CodeBlock)ScreenBuffer[i]).Size; j++)
				{
					textgraphics.DrawString(((char)((CodeBlock)ScreenBuffer[i])[j]).ToString(), font, brush, 
						new Point((j*characterWidth),(i - UpperLine)*(lineSpace+characterHeight)));
				}
			}
			//draw caret
			if (caretVisible)
			{
				int x = caretX * characterWidth;
				int y = caretY * (lineSpace + characterHeight);
				textgraphics.DrawLine(textpen, x, y, x, y + lineSpace + characterHeight-1);
			}
		}
		
			#endregion

			#region graph mode

		public void PutPixel(int x, int y)
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
//				Graphics graphics = Graphics.FromHdc (hdc);
				graphics.DrawLine(GraphPen, x, y, x, y);
				this.Invalidate(new Rectangle(x, y, 1, 1));
			}
		}

		public void Line(int x1, int y1, int x2, int y2)
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
//				Graphics graphics = Graphics.FromHdc (hdc);
				Graphics graphics = Graphics.FromImage (PrevImage);
				graphics.DrawLine(GraphPen, x1, y1, x2, y2);
				
				this.Invalidate(new Rectangle(Math.Min(x1, x2),Math.Min(y1, y2),
					Math.Abs(x1 - x2), Math.Abs(y1 - y2)));
			}
		}

		public void ClearDevice()
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
				//Graphics graphics = Graphics.FromHdc (hdc);
				
				SolidBrush Brush = new SolidBrush(BkGroundColor);
				graphics.FillRectangle(Brush, 0, 0, _MaxX, _MaxY);
				//PrevImage.
				this.Invalidate();
			}
		}

		public void SetColor(Color color)
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
				GraphPen = new Pen(color);
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
				this.Invalidate(rec);
			}
		}

		public void Circle(int x, int y, int radios)
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
//				Graphics graphics = Graphics.FromHdc (hdc);
				Rectangle rec = new Rectangle(x - radios,y - radios,x + radios,y + radios);
				graphics.DrawEllipse(GraphPen, rec);
				this.Invalidate(rec);
			}
		}

		public void SetFont(string familyName,int fontSize, System.Drawing.FontStyle FontStyle)
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
				Graphfont = new Font(familyName, fontSize,FontStyle);
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
				this.Invalidate(new Rectangle(x, y, text.Length, Graphfont.Height));
			}
		}

/*		public void Fill(int x, int y,Color color)
		{
			if (bTextMode) throw new RuntimeError(SimulatorMessage.NOT_ALLOWED_IN_TEXT_MODE,-1);
			lock(this)
			{
				Brush brush = new SolidBrush(GraphPen.Color);
				graphics.FillClosedCurve (brush, new PointF(x, y));
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
			lock(this)
			{

				bNowInInputFunction = true;
				if (InputBuffer.Size == 0)
					eWaitForInput.WaitOne();
				iBufferPrinted--;
				res = InputBuffer[0];
				InputBuffer = InputBuffer.SubBlock(1);
			}
			return res;
		}

			#endregion

		#endregion

		#region events and threads


		/// <summary>
		/// this function execute when the console need to be updated on screen.
		/// </summary>
		private void Console_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if (bTextMode == true)
			{
				DrawTextImage();
				Graphics GraphGraphics = e.Graphics;
				GraphGraphics.DrawImage (PrevImage,new Point(0,0));
				Thread.Sleep(100);
			}
			else
			{
				Graphics GraphGraphics = e.Graphics;
				GraphGraphics.DrawImage (PrevImage,new Point(0,0));
				//Graphics graphics = Graphics.FromHdc (hdc);
				//GraphGraphics = graphics;
			}
		}

		/// <summary>
		/// this function added the key that pressed to input buffer
		/// </summary>
		private void Console_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if (bTextMode == true)
			{
				if (bNowInInputFunction && KeyPressed != null)
					throw new RuntimeError(SimulatorMessage.INPUT_FUNCTIONS_NOT_ALLOWED, -1);

				char cChar = e.KeyChar;
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
						iBufferPrinted--;
					if (InputBuffer.Size > 1) 
						InputBuffer = InputBuffer.SubBlock(0, InputBuffer.Size - 2);
					else
						InputBuffer = InputBuffer.SubBlock(0, InputBuffer.Size - 1);
				}
				
				if ((bNowInInputFunction && cChar == 0x0d) || (bNowExiting)) 
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
		public void exitConsole()
		{
			// Read one key.
			bNowExiting = true;
			eWaitForInput.WaitOne();

			this.Visible = false;
			System.Windows.Forms.Application.DoEvents();
			//this.Close();
		}


		/// <summary>
		/// Starting function for the caret's thread
		/// </summary>
		private void ShowCaret()
		{
			try 
			{
				while (true) 
				{
					this.Invalidate(new Rectangle(caretX*characterWidth, caretY*(characterHeight+lineSpace),
						characterWidth+2,characterHeight+lineSpace));
					Thread.Sleep(350);
					caretVisible = !caretVisible;
				}
			} 
			catch (Exception) 
			{
			}
		}

		/// <summary>
		/// window event, called when the form closed
		/// </summary>
		private void Console_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if ( _activeThread != null ) _activeThread.Abort();
			if ( _activeTimer != null ) _activeTimer.Dispose();
		}

		#endregion
	}
}
