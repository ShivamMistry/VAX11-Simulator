using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;

using VAX11Settings;

namespace VAX11Environment
{
	/// <summary>
	/// SuHmmary description for frmSplashScreen.
	/// </summary>
	public class frmSplashScreen : System.Windows.Forms.Form
	{

		#region Members

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		
		private System.Windows.Forms.PictureBox pictureBox1;

		static string[] theArgs;
		
		System.Timers.Timer timerFirstDoc;
		frmMain mainApplication;

		#endregion

		#region Constructor, Destructor

		public frmSplashScreen()
		{
			// Allow only one application to run
			if (RunningInstance() != null) Application.Exit();

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

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

		#region Main

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) 
		{
			// Load Settings
			Settings.LoadSettings();

			// Command line parameters stuff
			theArgs = args;
			if (args.Length > 0 && ParseCommandLineParameters(args) == true) return;

			// Start the splash screen and the main app
			Application.Run(new frmSplashScreen());
			
		}

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmSplashScreen));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.White;
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(344, 200);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// frmSplashScreen
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.White;
			this.ClientSize = new System.Drawing.Size(344, 200);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmSplashScreen";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "VAX11 Simulator";
			this.TopMost = true;
			this.TransparencyKey = System.Drawing.Color.White;
			this.VisibleChanged += new System.EventHandler(this.frmSplashScreen_VisibleChanged);
			this.ResumeLayout(false);

		}
		#endregion

		#region Events Handlers
		
		private void mainApplication_OnFrmMainExit()
		{
			if (Settings.bSettingSaved == false) Settings.SaveSettings();
			this.Close();
		}


		private void TimerEventProcessorFirstDoc(object source, System.Timers.ElapsedEventArgs e)
		{
			timerFirstDoc.Enabled = false;
            SetVisible(false);
            if (mainApplication.InvokeRequired)
            {
                ActivateCallback callback = new ActivateCallback(mainApplication.Activate);
                this.Invoke(callback, new object[] { });
            }
            else
            {
                mainApplication.Activate();
            }
		}

        private delegate void ActivateCallback();

        private delegate void SetVisibleCallback(bool value);

        private void SetVisible(bool value)
        {
            if (this.InvokeRequired)
            {
                SetVisibleCallback d = new SetVisibleCallback(SetVisible);
                this.Invoke(d, new object[] { value });
            }
            else
            {
                this.Visible = value;
            }
            Control.CheckForIllegalCrossThreadCalls = false;
        }

		
		private void frmSplashScreen_VisibleChanged(object sender, System.EventArgs e)
		{
			if (this.Visible == true)
			{
				mainApplication = new frmMain(theArgs);
				mainApplication.Visible = true;
				mainApplication.GiveFocusToTheActiveDocument();

				timerFirstDoc = new System.Timers.Timer();
				timerFirstDoc.Elapsed+=new System.Timers.ElapsedEventHandler(TimerEventProcessorFirstDoc);
				timerFirstDoc.Interval = 2000;
				timerFirstDoc.Enabled = true;

				mainApplication.OnFrmMainExit += new VAX11Environment.frmMain.ExitFromProgram(mainApplication_OnFrmMainExit);
			}
		}


		#endregion

		#region Help Functions

		/// <summary>
		/// This function checks if there is already active instance of the application
		/// and if yes, it returns Process class describing it
		/// </summary>
		/// <returns>Second instance of the application, or NULL</returns>
		public static Process RunningInstance() 
 		{ 
 			Process current = Process.GetCurrentProcess(); 
 			Process[] processes = Process.GetProcessesByName (current.ProcessName); 
 
 			//Loop through the running processes in with the same name 
 			foreach (Process process in processes) 
 			{ 
 				//Ignore the current process 
 				if (process.Id != current.Id) 
 				{ 
 					//Make sure that the process is running from the exe file. 
 					if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName) 
 					{ 
 						//Return the other process instance. 
 						return process; 
 					} 
 				} 
 			} 
 			//No other instance was found, return null. 
 			return null; 
 		} 
 
		#endregion

		#region Parse Command Line Parameters

		/// <summary>
		/// Parse command line parameters. Returns true if we did operation and don't need to run the simulator.
		/// Else return false
		/// </summary>
		/// <param name="args">Command line arguments</param>
		/// <returns>True if we did operation and don't need to run the simulator, else false</returns>
		private static bool ParseCommandLineParameters(string[] args)
		{
			// TODO: all
			return false;
		}

		#endregion

	}
}
