using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace VAX11Environment
{
	/// <summary>
	/// Bases Calculator.
	/// </summary>
	public class BaseCalc : System.Windows.Forms.UserControl
	{

		private long val = 0;
		private bool bJustCalc = false;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton bin_radio;
		private System.Windows.Forms.RadioButton hex_radio;
		private System.Windows.Forms.RadioButton dec_radio;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Button btnClear;
		private System.Windows.Forms.TextBox BaseValue;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public BaseCalc()
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.bin_radio = new System.Windows.Forms.RadioButton();
			this.hex_radio = new System.Windows.Forms.RadioButton();
			this.dec_radio = new System.Windows.Forms.RadioButton();
			this.panel2 = new System.Windows.Forms.Panel();
			this.btnClear = new System.Windows.Forms.Button();
			this.BaseValue = new System.Windows.Forms.TextBox();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.bin_radio);
			this.panel1.Controls.Add(this.hex_radio);
			this.panel1.Controls.Add(this.dec_radio);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(258, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(344, 40);
			this.panel1.TabIndex = 7;
			// 
			// bin_radio
			// 
			this.bin_radio.Location = new System.Drawing.Point(264, 8);
			this.bin_radio.Name = "bin_radio";
			this.bin_radio.Size = new System.Drawing.Size(48, 24);
			this.bin_radio.TabIndex = 8;
			this.bin_radio.Text = "Bin";
			this.bin_radio.KeyDown += new System.Windows.Forms.KeyEventHandler(this.bin_radio_KeyDown);
			this.bin_radio.CheckedChanged += new System.EventHandler(this.bin_radio_CheckedChanged);
			// 
			// hex_radio
			// 
			this.hex_radio.Location = new System.Drawing.Point(192, 8);
			this.hex_radio.Name = "hex_radio";
			this.hex_radio.Size = new System.Drawing.Size(48, 24);
			this.hex_radio.TabIndex = 7;
			this.hex_radio.Text = "Hex";
			this.hex_radio.KeyDown += new System.Windows.Forms.KeyEventHandler(this.hex_radio_KeyDown);
			this.hex_radio.CheckedChanged += new System.EventHandler(this.hex_radio_CheckedChanged);
			// 
			// dec_radio
			// 
			this.dec_radio.Checked = true;
			this.dec_radio.Location = new System.Drawing.Point(120, 8);
			this.dec_radio.Name = "dec_radio";
			this.dec_radio.Size = new System.Drawing.Size(48, 24);
			this.dec_radio.TabIndex = 6;
			this.dec_radio.TabStop = true;
			this.dec_radio.Text = "Dec";
			this.dec_radio.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dec_radio_KeyDown);
			this.dec_radio.CheckedChanged += new System.EventHandler(this.dec_radio_CheckedChanged);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.btnClear);
			this.panel2.Controls.Add(this.BaseValue);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(352, 40);
			this.panel2.TabIndex = 8;
			// 
			// btnClear
			// 
			this.btnClear.Location = new System.Drawing.Point(240, 8);
			this.btnClear.Name = "btnClear";
			this.btnClear.Size = new System.Drawing.Size(48, 20);
			this.btnClear.TabIndex = 8;
			this.btnClear.Text = "CLR";
			this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
			// 
			// BaseValue
			// 
			this.BaseValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.BaseValue.Location = new System.Drawing.Point(32, 8);
			this.BaseValue.MaxLength = 10;
			this.BaseValue.Name = "BaseValue";
			this.BaseValue.Size = new System.Drawing.Size(200, 20);
			this.BaseValue.TabIndex = 7;
			this.BaseValue.Text = "0";
			this.BaseValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.BaseValue.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BaseValue_MouseDown);
			this.BaseValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.BaseValue_KeyPress);
			this.BaseValue.TextChanged += new System.EventHandler(this.BaseValue_TextChanged);
			// 
			// BaseCalc
			// 
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "BaseCalc";
			this.Size = new System.Drawing.Size(602, 40);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BaseCalc_KeyDown);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		private void dec_radio_CheckedChanged(object sender, System.EventArgs e)
		{
			BaseValue.MaxLength = 10;
			bJustCalc = true;
			BaseValue.Text = val.ToString();
		}

		private void hex_radio_CheckedChanged(object sender, System.EventArgs e)
		{
			BaseValue.MaxLength = 8;
			bJustCalc = true;
			BaseValue.Text = Convert.ToString(val,16).ToUpper();
		}

		private void bin_radio_CheckedChanged(object sender, System.EventArgs e)
		{
			BaseValue.MaxLength = 32;
			bJustCalc = true;
			BaseValue.Text = Convert.ToString(val,2);
		}

		private void BaseValue_TextChanged(object sender, System.EventArgs e)
		{
			if (bJustCalc) 
			{
				bJustCalc = false;
				return;
			}
			if (BaseValue.Text == "")
			{
				val =0;
				return;
			}
			try
			{
				if (dec_radio.Checked) val = Convert.ToUInt32(BaseValue.Text,10);
				else if (hex_radio.Checked) val = Convert.ToInt64(BaseValue.Text,16);
				else val = Convert.ToInt64(BaseValue.Text,2);
				//BaseValue.Text = BaseValue.Text.ToUpper();
			}
			catch
			{
				if (dec_radio.Checked)
				{
					val = uint.MaxValue;
					BaseValue.Text = val.ToString();
					BaseValue.Invalidate();
				}
				else if (hex_radio.Checked)
					BaseValue.Text = Convert.ToString(int.MaxValue, 16).ToUpper();
				else 
					BaseValue.Text = Convert.ToString(int.MaxValue, 2);
			}
		}

		private void BaseValue_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			BaseValue.SelectAll();
		}

		private void btnClear_Click(object sender, System.EventArgs e)
		{
			BaseValue.Text = new String('0',1 ) ;
			val = 0;
			BaseValue.Focus();
		}

		private void bin_radio_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			BaseValue.Focus();
		}

		private void hex_radio_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			BaseValue.Focus();
		}

		private void dec_radio_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			BaseValue.Focus();
		}

		private void BaseCalc_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			BaseValue.Focus();
		}

		private void BaseValue_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			if ((e.KeyChar == 8)||(e.KeyChar == 46)) //backspace or del pressed
				return;
			if ((dec_radio.Checked) && !((e.KeyChar >= '0') && (e.KeyChar <='9')))
				e.Handled = true;
			else if ((bin_radio.Checked) && !((e.KeyChar >= '0') && (e.KeyChar <='1')))
				e.Handled = true;
			else if ((hex_radio.Checked) && !(((e.KeyChar >= '0') && (e.KeyChar <='9')) || 
				((e.KeyChar.ToString().ToUpper()[0]>='A') && (e.KeyChar.ToString().ToUpper()[0]<='F'))))
				e.Handled = true;

		}





	}
}
