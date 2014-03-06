using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace VAX11Environment
{
	/// <summary>
	/// Summary description for frmGoto.
	/// </summary>
	public class frmGoto : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.Button bCancle;
		private System.Windows.Forms.TextBox NumBox;
		private System.Windows.Forms.Label GotoLable;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmGoto()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.NumBox = new System.Windows.Forms.TextBox();
			this.GotoLable = new System.Windows.Forms.Label();
			this.bOK = new System.Windows.Forms.Button();
			this.bCancle = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// NumBox
			// 
			this.NumBox.Location = new System.Drawing.Point(12, 24);
			this.NumBox.MaxLength = 7;
			this.NumBox.Name = "NumBox";
			this.NumBox.Size = new System.Drawing.Size(224, 20);
			this.NumBox.TabIndex = 0;
			this.NumBox.Text = "1";
			this.NumBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumBox_KeyPress);
			this.NumBox.TextChanged += new System.EventHandler(this.NumBox_TextChanged);
			this.NumBox.VisibleChanged += new System.EventHandler(this.frmGoto_Enter);
			this.NumBox.Enter += new System.EventHandler(this.frmGoto_Enter);
			// 
			// GotoLable
			// 
			this.GotoLable.Location = new System.Drawing.Point(12, 8);
			this.GotoLable.Name = "GotoLable";
			this.GotoLable.Size = new System.Drawing.Size(176, 23);
			this.GotoLable.TabIndex = 1;
			this.GotoLable.Text = "Line number (1 - 1):";
			// 
			// bOK
			// 
			this.bOK.Location = new System.Drawing.Point(80, 56);
			this.bOK.Name = "bOK";
			this.bOK.TabIndex = 2;
			this.bOK.Text = "&OK";
			this.bOK.Click += new System.EventHandler(this.bOK_Click);
			// 
			// bCancle
			// 
			this.bCancle.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancle.Location = new System.Drawing.Point(160, 56);
			this.bCancle.Name = "bCancle";
			this.bCancle.TabIndex = 3;
			this.bCancle.Text = "&Cancel";
			// 
			// frmGoto
			// 
			this.AcceptButton = this.bOK;
			this.AutoScale = false;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.bCancle;
			this.ClientSize = new System.Drawing.Size(248, 86);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.bCancle,
																		  this.bOK,
																		  this.NumBox,
																		  this.GotoLable});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmGoto";
			this.ShowInTaskbar = false;
			this.Text = "Go To Line";
			this.TopMost = true;
			this.Enter += new System.EventHandler(this.frmGoto_Enter);
			this.ResumeLayout(false);

		}
		#endregion

		private void NumBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			string sTemp = NumBox.Text;
			if (NumBox.SelectedText.Length > 0)
				sTemp = NumBox.Text.Substring (0, NumBox.SelectionStart);
			int KeyCode = (int)e.KeyChar;  
			if ((KeyCode >= '0' && KeyCode <= '9') && (NumBox.Text.Length < 9))
				sTemp += e.KeyChar ;
			if (KeyCode != 8) e.Handled=true;
			if (NumBox.SelectedText.Length > 0)
			{
				if (NumBox.SelectionStart + NumBox.SelectedText.Length < NumBox.Text.Length)
					sTemp += NumBox.Text.Substring (NumBox.SelectionStart + NumBox.SelectedText.Length);
				
			}
			NumBox.Text = sTemp;
		}

		#region properties

		private int _LastLine = 1;
		public int LastLine
		{
			get
			{
				return _LastLine;
			}
			set
			{
				if (value >= 0) 
				{
					_LastLine = (value != 0)? value : 1;
					GotoLable.Text = "Line number (1 - " + _LastLine.ToString() + "):" ;
				}
				else 
					throw (new VAX11Internals.PanicException());
			}
		}

		private int _CurrentLine = 1;
	
		public int CurrentLine
		{
			get
			{
				return _CurrentLine;
			}
			set
			{
				if ((value > 0) && (value <_LastLine))
				{
					_CurrentLine = value;
					NumBox.Text = value.ToString();
				}
				else
				{
					if (value > 0)
					{
						_CurrentLine = _LastLine;
						NumBox.Text = _LastLine.ToString();
					}
					else
					{
						_CurrentLine = 1;
						NumBox.Text = "1";
					}
				}		
			}
		}


		public bool OK = false;
		#endregion

		private void frmGoto_Enter(object sender, System.EventArgs e)
		{
			NumBox.Focus();
			NumBox.SelectAll();
			this.SelectNextControl(this, true, true, true, true);
		}

		private void NumBox_TextChanged(object sender, System.EventArgs e)
		{
			if (NumBox.Text.Length > 0)
				CurrentLine = Convert.ToInt32 (NumBox.Text);
		}

		private void bOK_Click(object sender, System.EventArgs e)
		{
			OK = true;
			this.Close();
		}
	}
}
