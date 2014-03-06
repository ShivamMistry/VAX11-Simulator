using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace VAX11Environment
{
	/// <summary>
	/// Summary description for frmCompileMessages.
	/// </summary>
	public class frmCompileMessages : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.TextBox txtOutput;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Constructor - Does nothing
		/// </summary>
		public frmCompileMessages()
		{
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.txtOutput = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// txtOutput
			// 
			this.txtOutput.AcceptsReturn = true;
			this.txtOutput.AcceptsTab = true;
			this.txtOutput.BackColor = System.Drawing.Color.White;
			this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtOutput.Multiline = true;
			this.txtOutput.Name = "txtOutput";
			this.txtOutput.ReadOnly = true;
			this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.txtOutput.Size = new System.Drawing.Size(292, 266);
			this.txtOutput.TabIndex = 0;
			this.txtOutput.Text = "";
			// 
			// frmCompileMessages
			// 
			//this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.txtOutput});
			this.Name = "frmCompileMessages";
			this.Text = "Output";
			this.Load += new System.EventHandler(this.frmCompileMessages_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void frmCompileMessages_Load(object sender, System.EventArgs e)
		{
		
		}

		/// <summary>
		/// Set/Get the text in output window
		/// </summary>
		public string Output
		{
			get
			{
				return txtOutput.Text;
			}
			set
			{
				txtOutput.Text = value;
			}
		}
	}
}
