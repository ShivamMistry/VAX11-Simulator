using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using VAX11Settings;

namespace VAX11Environment
{
	/// <summary>
	/// Summary description for frmAbout.
	/// </summary>
	public class frmAbout : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnOK;
		public System.Windows.Forms.Label lblInformation;
		private System.Windows.Forms.LinkLabel linkMagic;
		private System.Windows.Forms.PictureBox pictureBox1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmAbout()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			lblInformation.Text = "Version: " + Settings.Environment.SIM_VERSION
				+ "\n\n" + "Authors: Nir Adar and Rotem Grosman\n" + 
				"The Technion - Electrical Eng. Department";
			linkMagic.Links.Add(linkMagic.Text.IndexOf("Divil Software") ,linkMagic.Text.Length, "http://www.divil.co.uk/net");
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmAbout));
			this.btnOK = new System.Windows.Forms.Button();
			this.lblInformation = new System.Windows.Forms.Label();
			this.linkMagic = new System.Windows.Forms.LinkLabel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.BackColor = System.Drawing.SystemColors.Control;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnOK.Location = new System.Drawing.Point(112, 296);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 0;
			this.btnOK.Text = "OK";
			// 
			// lblInformation
			// 
			this.lblInformation.BackColor = System.Drawing.Color.White;
			this.lblInformation.Location = new System.Drawing.Point(24, 200);
			this.lblInformation.Name = "lblInformation";
			this.lblInformation.Size = new System.Drawing.Size(248, 56);
			this.lblInformation.TabIndex = 2;
			this.lblInformation.Text = "Product Information";
			// 
			// linkMagic
			// 
			this.linkMagic.Location = new System.Drawing.Point(24, 264);
			this.linkMagic.Name = "linkMagic";
			this.linkMagic.Size = new System.Drawing.Size(272, 16);
			this.linkMagic.TabIndex = 5;
			this.linkMagic.TabStop = true;
			this.linkMagic.Text = "This software contains GUI code from Divil Software";
			this.linkMagic.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkMagic_LinkClicked);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(8, 8);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(288, 160);
			this.pictureBox1.TabIndex = 6;
			this.pictureBox1.TabStop = false;
			// 
			// frmAbout
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.White;
			this.CancelButton = this.btnOK;
			this.ClientSize = new System.Drawing.Size(298, 327);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.linkMagic);
			this.Controls.Add(this.lblInformation);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmAbout";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About VAX11 Simulator";
			this.Load += new System.EventHandler(this.frmAbout_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmAbout_Paint);
			this.ResumeLayout(false);

		}
		#endregion

		private void frmAbout_Load(object sender, System.EventArgs e)
		{
		}

		private void linkMagic_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			//linkMagic.Links[0].Visited = true;
			System.Diagnostics.Process.Start("http://www.divil.co.uk/net");
		}

		private void frmAbout_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			this.StartPosition = FormStartPosition.CenterParent;
		}






	}
}
