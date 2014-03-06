using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace VAX11Environment
{
	/// <summary>
	/// Summary description for frmAddEditWatch.
	/// </summary>
	public class frmAddEditWatch : System.Windows.Forms.Form
	{
		public enum watchTypeEnum
		{
			Byte_Number,
			Word_Number,
			Long_Number,
			Quad_Number,
			Char,
			C_String,
			Pascal_String
		}
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label lblWatchName;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Label lblWatchType;
		private System.Windows.Forms.Button buttonAddEdit;
		private System.Windows.Forms.Button buttonCancle;
		private System.Windows.Forms.ComboBox typeList;

		private bool bEditMode; //chose add or edit mode

		private string _Name; //saves the name of the watch
		/// <summary>
		/// Saves the name of the watch
		/// </summary>
		public string WatchName
		{
			get
			{
				return _Name;
			}
			set
			{
				_Name = value;
			}
		}
		
		private watchTypeEnum _type; //saves the type of the watch
		/// <summary>
		/// Saves the type of the watch
		/// </summary>
		public watchTypeEnum WatchType
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		public frmAddEditWatch()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			bEditMode = false;
		}

		public frmAddEditWatch(bool _bEditMode, string sName, watchTypeEnum eType)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			bEditMode = _bEditMode;
			WatchName = sName;
			WatchType = eType;
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
			this.lblWatchName = new System.Windows.Forms.Label();
			this.textName = new System.Windows.Forms.TextBox();
			this.buttonAddEdit = new System.Windows.Forms.Button();
			this.buttonCancle = new System.Windows.Forms.Button();
			this.lblWatchType = new System.Windows.Forms.Label();
			this.typeList = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// lblWatchName
			// 
			this.lblWatchName.Location = new System.Drawing.Point(8, 16);
			this.lblWatchName.Name = "lblWatchName";
			this.lblWatchName.Size = new System.Drawing.Size(80, 20);
			this.lblWatchName.TabIndex = 0;
			this.lblWatchName.Text = "Watch Name:";
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(80, 16);
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(136, 20);
			this.textName.TabIndex = 1;
			this.textName.Text = "";
			// 
			// buttonAddEdit
			// 
			this.buttonAddEdit.Location = new System.Drawing.Point(240, 16);
			this.buttonAddEdit.Name = "buttonAddEdit";
			this.buttonAddEdit.TabIndex = 2;
			this.buttonAddEdit.Text = "&Add";
			this.buttonAddEdit.Click += new System.EventHandler(this.buttonAddEdit_Click);
			// 
			// buttonCancle
			// 
			this.buttonCancle.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancle.Location = new System.Drawing.Point(240, 48);
			this.buttonCancle.Name = "buttonCancle";
			this.buttonCancle.TabIndex = 3;
			this.buttonCancle.Text = "&Cancel";
			// 
			// lblWatchType
			// 
			this.lblWatchType.Location = new System.Drawing.Point(8, 48);
			this.lblWatchType.Name = "lblWatchType";
			this.lblWatchType.Size = new System.Drawing.Size(72, 16);
			this.lblWatchType.TabIndex = 4;
			this.lblWatchType.Text = "Watch Type:";
			// 
			// typeList
			// 
			this.typeList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.typeList.Items.AddRange(new object[] {
														  "Byte Number",
														  "Word Number",
														  "Long Number",
														  "Quad Number",
														  "Char",
														  "C String (asciz)",
														  "Pascal String (ascic)"});
			this.typeList.Location = new System.Drawing.Point(80, 48);
			this.typeList.Name = "typeList";
			this.typeList.Size = new System.Drawing.Size(136, 21);
			this.typeList.TabIndex = 5;
			// 
			// frmAddEditWatch
			// 
			this.AcceptButton = this.buttonAddEdit;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancle;
			this.ClientSize = new System.Drawing.Size(322, 80);
			this.Controls.Add(this.typeList);
			this.Controls.Add(this.lblWatchType);
			this.Controls.Add(this.buttonCancle);
			this.Controls.Add(this.buttonAddEdit);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.lblWatchName);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "frmAddEditWatch";
			this.ShowInTaskbar = false;
			this.Text = "frmAddEditWatch";
			this.Load += new System.EventHandler(this.frmAddEditWatch_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void frmAddEditWatch_Load(object sender, System.EventArgs e)
		{
			typeList.SelectedIndex = 0;
			if (bEditMode)
			{
				this.Text = "Edit Watch";
				buttonAddEdit.Text = "&Edit";
			}
			else
			{
				this.Text = "Add Watch";
				buttonAddEdit.Text = "&Add";
			}
		}

		private void buttonAddEdit_Click(object sender, System.EventArgs e)
		{
			WatchType = (watchTypeEnum)typeList.SelectedIndex;
			WatchName = textName.Text;
			this.Close();
		}
	}
}
