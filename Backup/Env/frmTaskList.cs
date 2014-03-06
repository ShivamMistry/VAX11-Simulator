using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace VAX11Environment
{
	/// <summary>
	/// Summary description for frmTaskList.
	/// </summary>
	public class frmTaskList : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.ListView lstTasks;
		private System.Windows.Forms.ColumnHeader colIcon;
		private System.Windows.Forms.ColumnHeader colDescription;
		private System.Windows.Forms.ColumnHeader colLineNumber;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ImageList imageList1;



		/// <summary>
		/// Enums for messages icons
		/// </summary>
		public enum Icons { Error = 0, Warning };


		/// <summary>
		/// Delegate for clicking event
		/// </summary>
		public delegate void clickEventFunc(int iLine);

		/// <summary>
		///  the Event
		/// </summary>
		public event clickEventFunc OnClickEvent;


		// internal const
		private const int LINENUMBER_COL = 2;

		/// <summary>
		/// Consturctor
		/// </summary>
		public frmTaskList()
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmTaskList));
			this.lstTasks = new System.Windows.Forms.ListView();
			this.colIcon = new System.Windows.Forms.ColumnHeader();
			this.colDescription = new System.Windows.Forms.ColumnHeader();
			this.colLineNumber = new System.Windows.Forms.ColumnHeader();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// lstTasks
			// 
			this.lstTasks.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					   this.colIcon,
																					   this.colDescription,
																					   this.colLineNumber});
			this.lstTasks.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstTasks.FullRowSelect = true;
			this.lstTasks.GridLines = true;
			this.lstTasks.MultiSelect = false;
			this.lstTasks.Name = "lstTasks";
			this.lstTasks.Size = new System.Drawing.Size(416, 262);
			this.lstTasks.SmallImageList = this.imageList1;
			this.lstTasks.TabIndex = 1;
			this.lstTasks.View = System.Windows.Forms.View.Details;
			this.lstTasks.Resize += new System.EventHandler(this.lstTasks_Resize);
			this.lstTasks.DoubleClick += new System.EventHandler(this.lstTasks_DoubleClick);
			// 
			// colIcon
			// 
			this.colIcon.Text = "";
			this.colIcon.Width = 19;
			// 
			// colDescription
			// 
			this.colDescription.Text = "Description";
			this.colDescription.Width = 260;
			// 
			// colLineNumber
			// 
			this.colLineNumber.Text = "Line";
			this.colLineNumber.Width = 50;
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.White;
			// 
			// frmTaskList
			// 
			//this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(416, 262);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.lstTasks});
			this.Name = "frmTaskList";
			this.Text = "Task List";
			this.Load += new System.EventHandler(this.frmTaskList_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void frmTaskList_Load(object sender, System.EventArgs e)
		{
			// Make the form looks nice
			ArrangeControls();
		}

		private void ArrangeControls()
		{
			colDescription.Width = lstTasks.Width 
				- colLineNumber.Width - colIcon.Width - 5;
		}

		private void lstTasks_Resize(object sender, System.EventArgs e)
		{
			ArrangeControls();
		}

		/// <summary>
		/// Function to add tasks to the Task List
		/// </summary>
		/// <param name="icon">Task Icon</param>
		/// <param name="sDescrition">Message to display</param>
		/// <param name="iLineNumber">Line Number to display</param>
		public void AddTask(Icons icon, string sDescrition, int iLineNumber)
		{
			ListViewItem newItem = new ListViewItem("", (int)icon);
			newItem.SubItems.Add(sDescrition);
			newItem.SubItems.Add(iLineNumber.ToString());
			
			// If no element or the new element is the one with the biggest line number, add it to the end
			if (lstTasks.Items.Count == 0 || Convert.ToInt32(lstTasks.Items[lstTasks.Items.Count - 1].SubItems[LINENUMBER_COL].Text) < iLineNumber)
			{
				lstTasks.Items.Add(newItem);
				return;
			}
			else
			{
				// Move on all the list, find the location where the error should be inserted.
				// If you're really bored, you can make it a binary search, as the array is sorted.
				// I assume most of the time the fist case occurs, so i don't care about expensive operation from time to time
				for (int iCounter = 0; iCounter < lstTasks.Items.Count; ++iCounter)
				{
					if (Convert.ToInt32(lstTasks.Items[iCounter].SubItems[LINENUMBER_COL].Text) > iLineNumber)
					{
						lstTasks.Items.Insert(iCounter, newItem);
						return;
					}
				}
			}
		}

		/// <summary>
		/// Clears all tasks from task list
		/// </summary>
		public void ClearTasks()
		{
			lstTasks.Items.Clear();
		}

		/// <summary>
		/// Change all line numbers for tasks from selected start 
		/// line number by selected offset
		/// </summary>
		/// <param name="iFromLine">Start line number</param>
		/// <param name="iOffset">Offset (in line numbers)</param>
		public void updateTasksLinesNumber(int iFromLine, int iOffset)
		{
			int iTemp;
			foreach (System.Windows.Forms.ListViewItem l in lstTasks.Items)
			{
				iTemp = Convert.ToInt32(lstTasks.Items[l.Index].SubItems[LINENUMBER_COL].Text);
				if (iTemp >= iFromLine)
				{
					iTemp += iOffset;
					lstTasks.Items[l.Index].SubItems[LINENUMBER_COL].Text = iTemp.ToString();
				}
			}
			this.Update();
		}

		private void lstTasks_DoubleClick(object sender, System.EventArgs e)
		{
			if (lstTasks.SelectedItems.Count > 0 && OnClickEvent != null)
				OnClickEvent(Convert.ToInt32(lstTasks.Items[lstTasks.SelectedItems[0].Index].SubItems[LINENUMBER_COL].Text));
		}



	}
}
