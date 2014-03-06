using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using PropertyBagLib;
using VAX11Simulator;
using VAX11Settings;
using VAX11Internals;

namespace VAX11Environment
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmStackView : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.PropertyGrid gridStack;
		const int CELLS_TO_DISPLAY = 20;
		PropertyBag bag1;
		private Memory mem;
		int iPrevSP;
		Registers Regs;

		public frmStackView()
		{

			
			mem = new Memory(0, int.MaxValue);
			Regs = new Registers();
			iPrevSP = Regs[14].ReadLong();

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
				if (components != null) 
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
			this.gridStack = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// gridStack
			// 
			this.gridStack.CommandsVisibleIfAvailable = true;
			this.gridStack.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridStack.LargeButtons = false;
			this.gridStack.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.gridStack.Name = "gridStack";
			this.gridStack.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.gridStack.Size = new System.Drawing.Size(136, 318);
			this.gridStack.TabIndex = 0;
			this.gridStack.Text = "gridStack";
			this.gridStack.ToolbarVisible = false;
			this.gridStack.ViewBackColor = System.Drawing.SystemColors.Window;
			this.gridStack.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// frmStackView
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.gridStack});
			this.Name = "frmStackView";
			this.Size = new System.Drawing.Size(136, 318);
			this.Load += new System.EventHandler(this.frmStackView_Load);
			this.ResumeLayout(false);

		}
		#endregion


		private void frmStackView_Load(object sender, System.EventArgs e)
		{
			CreateStackBag((uint)Regs[14].ReadLong());
		}


		public void CreateStackBag(uint SP)
		{
			bag1 = new PropertyBag();
			bag1.GetValue += new PropertySpecEventHandler(this.bag1_GetValue);
			bag1.SetValue += new PropertySpecEventHandler(this.bag1_SetValue);

			uint iLowAddress = Math.Max(0, (SP - 4 * CELLS_TO_DISPLAY / 4));
			uint iHighAddress = Math.Min((uint)0xffffffff, (uint)SP + 4 * CELLS_TO_DISPLAY);
			for (uint place = iLowAddress; place < SP; place+=4)
				bag1.Properties.Add(new PropertySpec(Convert.ToString(place, 16).ToUpper(), typeof(string), 
					"Stack", "The cells contain pairs of addresses and values, giving the user the ability to watch the stack"));

			bag1.Properties.Add(new PropertySpec("SP", typeof(string), 
				"Stack", "This cell contains the value of the top of the stack"));

			for (uint place = SP+4; place < iHighAddress; place+=4)
				bag1.Properties.Add(new PropertySpec(Convert.ToString(place, 16).ToUpper(), typeof(string), 
					"Stack", "The cells contain pairs of addresses and values, giving the user the ability to watch the stack"));
			
			ArrayList objs = new ArrayList();
			objs.Add(bag1);
			gridStack.SelectedObjects = objs.ToArray();
		}


		private void bag1_GetValue(object sender, PropertySpecEventArgs e)
		{

			int iVal;
			if (e.Property.Name != "SP")
				iVal = mem.Read(int.Parse((string)(e.Property.Name), System.Globalization.NumberStyles.AllowHexSpecifier), 4, false);
			else iVal = mem.Read(Regs[14].ReadLong(), 4, false);

			if (Settings.Simulator.bShowMemoryAsHex) e.Value = iVal.ToString("x").ToUpper();
			else e.Value = iVal.ToString();

		}

		private void bag1_SetValue(object sender, PropertySpecEventArgs e)
		{

			try
			{
				int iValue;
				if (Settings.Simulator.bShowMemoryAsHex) iValue = 
																int.Parse((string)e.Value, System.Globalization.NumberStyles.AllowHexSpecifier);
				else iValue = int.Parse((string)e.Value);

				int iAddr;
				if (e.Property.Name == "SP") iAddr = this.iPrevSP;
				else iAddr = int.Parse((string)e.Property.Name, System.Globalization.NumberStyles.AllowHexSpecifier);
				mem.Write(iAddr, new CodeBlock((long)iValue, 4));

				UpdateStackDisplay(); // changing PSW changes the flags too
			}
			catch { }
		}

		public void SetDataObjects(Memory m, Registers r)
		{
			mem = m;
			Regs = r;
		}

		public void UpdateStackDisplay()
		{
			if (iPrevSP != Regs[14].ReadLong()) CreateStackBag((uint)(iPrevSP = Regs[14].ReadLong()));
			gridStack.Refresh();
		}
	}
}
