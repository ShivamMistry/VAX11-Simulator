using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using PropertyBagLib;
using VAX11Simulator;
using VAX11Settings;
using VAX11Internals;

namespace VAX11Environment
{
	/// <summary>
	/// Summary description for RegistersView.
	/// </summary>
	public class frmRegistersView : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.PropertyGrid lstRegisters;

		PropertyBag bag1;

		Registers reg;
		private DotNetWidgets.DotNetMenuProvider dotNetMenuProvider1;
		private System.Windows.Forms.ContextMenu mnuRegisters;
		private System.Windows.Forms.MenuItem mnuRegistersHex;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmRegistersView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Temporary, until the class gets the real register class.
			reg = new Registers();
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
			this.lstRegisters = new System.Windows.Forms.PropertyGrid();
			this.dotNetMenuProvider1 = new DotNetWidgets.DotNetMenuProvider();
			this.mnuRegisters = new System.Windows.Forms.ContextMenu();
			this.mnuRegistersHex = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// lstRegisters
			// 
			this.lstRegisters.CommandsVisibleIfAvailable = true;
			this.lstRegisters.ContextMenu = this.mnuRegisters;
			this.lstRegisters.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lstRegisters.LargeButtons = false;
			this.lstRegisters.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.lstRegisters.Name = "lstRegisters";
			this.lstRegisters.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.lstRegisters.Size = new System.Drawing.Size(232, 318);
			this.lstRegisters.TabIndex = 2;
			this.lstRegisters.Text = "Registers";
			this.lstRegisters.ToolbarVisible = false;
			this.lstRegisters.ViewBackColor = System.Drawing.SystemColors.Window;
			this.lstRegisters.ViewForeColor = System.Drawing.SystemColors.WindowText;
			this.lstRegisters.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstRegisters_MouseDown);
			// 
			// dotNetMenuProvider1
			// 
			this.dotNetMenuProvider1.OwnerForm = null;
			// 
			// mnuRegisters
			// 
			this.mnuRegisters.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.mnuRegistersHex});
			// 
			// mnuRegistersHex
			// 
			this.dotNetMenuProvider1.SetDrawSpecial(this.mnuRegistersHex, true);
			this.mnuRegistersHex.Index = 0;
			this.mnuRegistersHex.Text = "Hex";
			this.mnuRegistersHex.Click += new System.EventHandler(this.mnuRegistersHex_Click);
			// 
			// frmRegistersView
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.lstRegisters});
			this.Name = "frmRegistersView";
			this.Size = new System.Drawing.Size(232, 318);
			this.Load += new System.EventHandler(this.frmRegistersView_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void frmRegistersView_Load(object sender, System.EventArgs e)
		{
			CreateRegistersBag();
		}

		/// <summary>
		/// Creates the display of all the register. If we changes the visible register - for example the visible of the special registers,
		/// we need to call this function again so the registers view will display the correct information
		/// </summary>
		public void CreateRegistersBag()
		{
			bag1 = new PropertyBag();
			bag1.GetValue += new PropertySpecEventHandler(this.bag1_GetValue);
			bag1.SetValue += new PropertySpecEventHandler(this.bag1_SetValue);
			for (int i = 0; i < 12; ++i)
				bag1.Properties.Add(new PropertySpec("R" + i.ToString(), typeof(string), 
					"Registers", "General Register"));
			bag1.Properties.Add(new PropertySpec("R12", typeof(string), "Registers", 
				"Argument Pointer - contains the address of the base of a software data structure called the argument list, which is maintained for procedure calls."));
			bag1.Properties.Add(new PropertySpec("R13", typeof(string), "Registers", 
				"Frame Pointer - contains the address of the base of a software data structure stored on the stack called the stack frame, which is maintained for procedure calls."));
			bag1.Properties.Add(new PropertySpec("R14", typeof(string), "Registers", 
				"Stack Pointer - contains the address of the base (also called the top) of a stack maintained for subroutine and procedure calls."));
			bag1.Properties.Add(new PropertySpec("R15", typeof(string), "Registers",
				"Program Counter - contains the address of the next byte to be processed in the instruction stream."));
			bag1.Properties.Add(new PropertySpec("PSW", typeof(string), "Registers",
				"The Processor Status Word - a special processor register that a program uses to check its status and to control synchronous error conditions."));

			bag1.Properties.Add(new PropertySpec("Cycles", typeof(string), "Registers",
				"Clock cycles since the simulator startup"));

			bag1.Properties.Add(new PropertySpec("N", typeof(uint), "VAX11 Flags", "Negative flag."));
			bag1.Properties.Add(new PropertySpec("Z", typeof(uint), "VAX11 Flags",	"Zero flag."));
			bag1.Properties.Add(new PropertySpec("V", typeof(uint), "VAX11 Flags",	"Overflow flag."));
			bag1.Properties.Add(new PropertySpec("C", typeof(uint), "VAX11 Flags",	"Carry flag."));

			if (Settings.Simulator.bShowSpecialRegisters)
			{

				bag1.Properties.Add(new PropertySpec("SCBB", typeof(string), "Privileged Registers", "System Control Block Base"));
				bag1.Properties.Add(new PropertySpec("IPL", typeof(string), "Privileged Registers", "Interrupt Priority Level"));
				bag1.Properties.Add(new PropertySpec("PSL", typeof(string), "Privileged Registers", "Processor Status Longword"));
				bag1.Properties.Add(new PropertySpec("SIRR", typeof(string), "Privileged Registers", "Software Interrupt Request"));
				bag1.Properties.Add(new PropertySpec("SISR", typeof(string), "Privileged Registers", "Software Interrupt Summery"));
				bag1.Properties.Add(new PropertySpec("ICCS", typeof(string), "Privileged Registers", "Interval Clock Control/Status"));
				bag1.Properties.Add(new PropertySpec("NICR", typeof(string), "Privileged Registers", "Next Interval Count Register"));
				bag1.Properties.Add(new PropertySpec("ICR", typeof(string), "Privileged Registers", "Interval Count Register"));
				bag1.Properties.Add(new PropertySpec("RXCS", typeof(string), "Privileged Registers", "Console Receive Control/Status"));
				bag1.Properties.Add(new PropertySpec("RXDB", typeof(string), "Privileged Registers", "Console Receive Data Buffer"));
				bag1.Properties.Add(new PropertySpec("TXCS", typeof(string), "Privileged Registers", "Console Transmit Control/Status"));
				bag1.Properties.Add(new PropertySpec("TXDB", typeof(string), "Privileged Registers", "Console Transmit Data Buffer"));
				
			}

			ArrayList objs = new ArrayList();
			objs.Add(bag1);
			lstRegisters.SelectedObjects = objs.ToArray();
		}


		private void bag1_GetValue(object sender, PropertySpecEventArgs e)
		{
			if (e.Property.Category == "Registers")
			{
				uint iValue;
				if (e.Property.Name == "PSW") iValue = (uint)reg.PSL.PSW;
				else if (e.Property.Name == "Cycles") iValue = (uint)reg.SystemTime;
				else iValue = (uint)reg[int.Parse(e.Property.Name.Substring(1))];

				if (Settings.Simulator.bShowRegistersInHex) e.Value = iValue.ToString("x").ToUpper();
				else e.Value = iValue.ToString();
			}
			else if (e.Property.Category == "VAX11 Flags")
			{
				switch (e.Property.Name)
				{
					case "V": e.Value = reg.PSL.V; break;
					case "C": e.Value = reg.PSL.C; break;
					case "N": e.Value = reg.PSL.N; break;
					case "Z": e.Value = reg.PSL.Z; break;
				};
			}
			else if (e.Property.Category == "Privileged Registers")
			{
				uint iValue;
				if (e.Property.Name == "PSL") iValue = (uint)reg.PSL.ReadLong();
				else iValue = (uint)reg[RegistersSettings.GetPrivilegedRegister(e.Property.Name)];
				if (Settings.Simulator.bShowRegistersInHex) e.Value = iValue.ToString("x").ToUpper();
				else e.Value = iValue.ToString();
			}
		}

		private void bag1_SetValue(object sender, PropertySpecEventArgs e)
		{
			if (e.Property.Category == "Registers")
			{
				try
				{
					uint iValue;
					if (Settings.Simulator.bShowRegistersInHex) iValue = 
						uint.Parse((string)e.Value, System.Globalization.NumberStyles.AllowHexSpecifier);
					else iValue = uint.Parse((string)e.Value);

					if (e.Property.Name == "PSW") reg.PSL.PSW = (Registers.Register)iValue;
					else if (e.Property.Name == "Cycles") return;
					else reg[int.Parse(e.Property.Name.Substring(1))] = iValue;

					UpdateRegistersDisplay(); // changing PSW changes the flags too
				}
				catch { }
			}
			else if (e.Property.Category == "VAX11 Flags")
			{
				byte boolValue = (int)e.Value != 0 ? (byte)1 :(byte) 0;
				switch (e.Property.Name)
				{
					case "V": reg.PSL.V = boolValue; break;
					case "C": reg.PSL.C = boolValue; break;
					case "N": reg.PSL.N = boolValue; break;
					case "Z": reg.PSL.Z = boolValue; break;
				};
				UpdateRegistersDisplay(); // changing the flags changes PSW, PSL too
			}

			
		}


		/// <summary>
		/// Select Register class whichs registers will be display in the registers view.
		/// </summary>
		/// <param name="r">Register class to display</param>
		public void SetRegisersSource(Registers r)
		{
			reg = r;
		}

		/// <summary>
		/// Update the view - if the values of the registers had changed.
		/// </summary>
		public void UpdateRegistersDisplay()
		{
			lstRegisters.Refresh();
		}

		private void lstRegisters_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (Settings.Simulator.bShowRegistersInHex)
				mnuRegistersHex.Checked = true;
			else mnuRegistersHex.Checked = false;
		}

		private void mnuRegistersHex_Click(object sender, System.EventArgs e)
		{
			if (Settings.Simulator.bShowRegistersInHex)
			{
				Settings.Simulator.bShowRegistersInHex = false;
				Settings.SaveSettings();
				mnuRegistersHex.Checked = false;
				UpdateRegistersDisplay();
			}
			else
			{
				Settings.Simulator.bShowRegistersInHex = true;
				Settings.SaveSettings();
				mnuRegistersHex.Checked = true;
				UpdateRegistersDisplay();
			}
		}



	}
}
