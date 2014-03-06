using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace VAX11Environment
{
	/// <summary>
	/// Summary description for find.
	/// </summary>
	public class frmFind : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textFind;
		private System.Windows.Forms.CheckBox cCases;
		public System.Windows.Forms.Button bFindNext;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rUp;
		private System.Windows.Forms.RadioButton rDown;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#region properties

		private string _FindText = "";
		public string FindText
		{
			get 
			{
				return this.textFind.Text;
			}
			set
			{
				this.textFind.Text = value;
			}
		}

		private bool _FindUp = false;
		public bool FindUp
		{
			get 
			{
				return (rUp.Checked);
			}
		}

		private bool _FindCase = false;
		public bool FindCase
		{
			get
			{
				return (cCases.Checked);
			}
		}
		#endregion

		public frmFind()
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
			this.label1 = new System.Windows.Forms.Label();
			this.textFind = new System.Windows.Forms.TextBox();
			this.cCases = new System.Windows.Forms.CheckBox();
			this.bFindNext = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rDown = new System.Windows.Forms.RadioButton();
			this.rUp = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(4, 14);
			this.label1.Name = "label1";
			this.label1.TabIndex = 0;
			this.label1.Text = "Find what:";
			// 
			// textFind
			// 
			this.textFind.Location = new System.Drawing.Point(64, 12);
			this.textFind.Name = "textFind";
			this.textFind.Size = new System.Drawing.Size(200, 20);
			this.textFind.TabIndex = 1;
			this.textFind.Text = "";
			this.textFind.TextChanged += new System.EventHandler(this.textFind_TextChanged);
			// 
			// cCases
			// 
			this.cCases.Location = new System.Drawing.Point(6, 70);
			this.cCases.Name = "cCases";
			this.cCases.Size = new System.Drawing.Size(104, 16);
			this.cCases.TabIndex = 2;
			this.cCases.Text = "Match &cases";
			// 
			// bFindNext
			// 
			this.bFindNext.Enabled = false;
			this.bFindNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.bFindNext.Location = new System.Drawing.Point(274, 11);
			this.bFindNext.Name = "bFindNext";
			this.bFindNext.Size = new System.Drawing.Size(70, 23);
			this.bFindNext.TabIndex = 3;
			this.bFindNext.Text = "&Find Next";
			// 
			// bCancel
			// 
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.bCancel.Location = new System.Drawing.Point(274, 40);
			this.bCancel.Name = "bCancel";
			this.bCancel.Size = new System.Drawing.Size(70, 23);
			this.bCancel.TabIndex = 4;
			this.bCancel.Text = "Cancel";
			this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.rDown,
																					this.rUp});
			this.groupBox1.Location = new System.Drawing.Point(160, 48);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(104, 40);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Direction";
			// 
			// rDown
			// 
			this.rDown.Checked = true;
			this.rDown.Location = new System.Drawing.Point(48, 13);
			this.rDown.Name = "rDown";
			this.rDown.Size = new System.Drawing.Size(53, 24);
			this.rDown.TabIndex = 1;
			this.rDown.TabStop = true;
			this.rDown.Text = "&Down";
			// 
			// rUp
			// 
			this.rUp.Location = new System.Drawing.Point(8, 17);
			this.rUp.Name = "rUp";
			this.rUp.Size = new System.Drawing.Size(40, 16);
			this.rUp.TabIndex = 0;
			this.rUp.Text = "&Up";
			// 
			// frmFind
			// 
			this.AcceptButton = this.bFindNext;
			this.AutoScale = false;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.bCancel;
			this.ClientSize = new System.Drawing.Size(352, 96);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBox1,
																		  this.bCancel,
																		  this.bFindNext,
																		  this.cCases,
																		  this.textFind,
																		  this.label1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmFind";
			this.ShowInTaskbar = false;
			this.Text = "Find";
			this.TopMost = true;
			this.Closing += new System.ComponentModel.CancelEventHandler(this.frmFind_Closing);
			this.Load += new System.EventHandler(this.frmFind_Load);
			this.VisibleChanged += new System.EventHandler(this.frmFind_VisibleChanged);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void textFind_TextChanged(object sender, System.EventArgs e)
		{
			if (textFind.Text.Length == 0)
				bFindNext.Enabled = false;
			else if (!bFindNext.Enabled)
				bFindNext.Enabled = true;
		}

		private void frmFind_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			this.Hide();
			e.Cancel = true;
		}

		private void frmFind_VisibleChanged(object sender, System.EventArgs e)
		{
			if (this.Visible)
			{
				textFind.Focus();
				_FindCase = FindCase;
				_FindUp = FindUp;
				_FindText = FindText;
			}
		}

		private void bCancel_Click(object sender, System.EventArgs e)
		{
			this.Hide();
			textFind.Text = _FindText;
			cCases.Checked = _FindCase;
			if (_FindUp)
				rUp.Checked = true;
			else
				rDown.Checked = true;	
		}

		private void frmFind_Load(object sender, System.EventArgs e)
		{
			this.Visible = true;
		}
	}
}
