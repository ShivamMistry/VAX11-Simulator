using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using VAX11Settings;

namespace VAX11Environment
{
	/// <summary>
	/// Summary description for frmInputOutputFiles.
	/// </summary>
	public class frmInputOutputFiles : System.Windows.Forms.Form
	{

		#region Controls

		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnSelectInputFile;
		private System.Windows.Forms.TextBox txtInputFileName;
		private System.Windows.Forms.Button btnSelectOutputFile;
		private System.Windows.Forms.TextBox txtOutputFileName;
		private System.Windows.Forms.CheckBox chkAppend;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion

		#region Members

		private string _sInputFileName = "";
		private string _sOutputFileName = "";

		#endregion

		#region Properties

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

		public frmInputOutputFiles(string inputFile, string outputFile)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_sInputFileName = inputFile;
			_sOutputFileName = outputFile;
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
			this.label2 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.panel3 = new System.Windows.Forms.Panel();
			this.chkAppend = new System.Windows.Forms.CheckBox();
			this.btnSelectOutputFile = new System.Windows.Forms.Button();
			this.txtOutputFileName = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnSelectInputFile = new System.Windows.Forms.Button();
			this.txtInputFileName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.panel1.SuspendLayout();
			this.panel3.SuspendLayout();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.Color.White;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(177)));
			this.label2.Location = new System.Drawing.Point(24, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "Input/Output Files";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Window;
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.groupBox2,
																				 this.label2,
																				 this.label3});
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(488, 64);
			this.panel1.TabIndex = 3;
			// 
			// groupBox2
			// 
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.groupBox2.Location = new System.Drawing.Point(0, 56);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(488, 8);
			this.groupBox2.TabIndex = 7;
			this.groupBox2.TabStop = false;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(48, 32);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(368, 16);
			this.label3.TabIndex = 8;
			this.label3.Text = "Please select input and output files for your program.";
			// 
			// panel3
			// 
			this.panel3.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.chkAppend,
																				 this.btnSelectOutputFile,
																				 this.txtOutputFileName,
																				 this.label4,
																				 this.groupBox1,
																				 this.btnSelectInputFile,
																				 this.txtInputFileName,
																				 this.label1});
			this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel3.Location = new System.Drawing.Point(0, 64);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(488, 208);
			this.panel3.TabIndex = 5;
			// 
			// chkAppend
			// 
			this.chkAppend.Location = new System.Drawing.Point(384, 91);
			this.chkAppend.Name = "chkAppend";
			this.chkAppend.Size = new System.Drawing.Size(80, 16);
			this.chkAppend.TabIndex = 11;
			this.chkAppend.Text = "&Append";
			this.chkAppend.Visible = false;
			// 
			// btnSelectOutputFile
			// 
			this.btnSelectOutputFile.Location = new System.Drawing.Point(336, 88);
			this.btnSelectOutputFile.Name = "btnSelectOutputFile";
			this.btnSelectOutputFile.Size = new System.Drawing.Size(24, 20);
			this.btnSelectOutputFile.TabIndex = 10;
			this.btnSelectOutputFile.Text = "...";
			this.btnSelectOutputFile.Click += new System.EventHandler(this.btnSelectOutputFile_Click);
			// 
			// txtOutputFileName
			// 
			this.txtOutputFileName.Location = new System.Drawing.Point(120, 88);
			this.txtOutputFileName.Name = "txtOutputFileName";
			this.txtOutputFileName.Size = new System.Drawing.Size(200, 20);
			this.txtOutputFileName.TabIndex = 9;
			this.txtOutputFileName.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(24, 91);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(72, 16);
			this.label4.TabIndex = 8;
			this.label4.Text = "Output File:";
			// 
			// groupBox1
			// 
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.groupBox1.Location = new System.Drawing.Point(0, 200);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(488, 8);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			// 
			// btnSelectInputFile
			// 
			this.btnSelectInputFile.Location = new System.Drawing.Point(336, 32);
			this.btnSelectInputFile.Name = "btnSelectInputFile";
			this.btnSelectInputFile.Size = new System.Drawing.Size(24, 20);
			this.btnSelectInputFile.TabIndex = 5;
			this.btnSelectInputFile.Text = "...";
			this.btnSelectInputFile.Click += new System.EventHandler(this.btnSelectInputFile_Click);
			// 
			// txtInputFileName
			// 
			this.txtInputFileName.Location = new System.Drawing.Point(120, 32);
			this.txtInputFileName.Name = "txtInputFileName";
			this.txtInputFileName.Size = new System.Drawing.Size(200, 20);
			this.txtInputFileName.TabIndex = 4;
			this.txtInputFileName.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "Input File:";
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(312, 288);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 6;
			this.btnOK.Text = "&OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(400, 288);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 7;
			this.btnCancel.Text = "&Cancel";
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.FileName = "doc1";
			// 
			// frmInputOutputFiles
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(488, 320);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnCancel,
																		  this.btnOK,
																		  this.panel3,
																		  this.panel1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmInputOutputFiles";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Set Input/Output Files";
			this.Load += new System.EventHandler(this.frmInputOutputFiles_Load);
			this.panel1.ResumeLayout(false);
			this.panel3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btnSelectInputFile_Click(object sender, System.EventArgs e)
		{
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				txtInputFileName.Text = openFileDialog1.FileName;
			}
		}

		private void btnSelectOutputFile_Click(object sender, System.EventArgs e)
		{
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				txtOutputFileName.Text = saveFileDialog1.FileName;
			}
		}

		private void frmInputOutputFiles_Load(object sender, System.EventArgs e)
		{
			openFileDialog1.Filter	= Settings.Environment.DAT_FILE_FILTERS;
			saveFileDialog1.Filter	= Settings.Environment.DAT_FILE_FILTERS;
			txtOutputFileName.Text	= _sOutputFileName;
			txtInputFileName.Text	= _sInputFileName;
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			_sOutputFileName = txtOutputFileName.Text;
			_sInputFileName = txtInputFileName.Text;
			this.Close();
		}

	}
}
