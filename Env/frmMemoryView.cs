using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using VAX11Simulator;
using VAX11Settings;

namespace VAX11Environment
{
	/// <summary>
	/// Summary description for frmMemoryView.
	/// </summary>
	public class frmMemoryView : System.Windows.Forms.UserControl
	{
		#region members

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.DataGrid gridMemory;
		private DataTable MemoryTable;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private Memory mem;
		private System.Windows.Forms.VScrollBar vScrollBar1;
		private System.Windows.Forms.RadioButton rHex;
		private System.Windows.Forms.RadioButton rDec;
		private System.Windows.Forms.TextBox textCellValue;
		private System.Windows.Forms.TextBox txtAddress;
		private int iBaseAddr = 0;
		private System.Windows.Forms.Label CellValueLabel;
		private System.Windows.Forms.Label BaseAddressLabel;
		private System.Windows.Forms.Label CellLabel;
		private bool bnowResizing = false; // for prevent refreshing
		private bool bChangeTextAddress = true;
		private ArrayList ChangesAddress = new ArrayList(); //buffer to save the changes and update all together
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.panel1 = new System.Windows.Forms.Panel();
			this.textCellValue = new System.Windows.Forms.TextBox();
			this.CellLabel = new System.Windows.Forms.Label();
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.txtAddress = new System.Windows.Forms.TextBox();
			this.CellValueLabel = new System.Windows.Forms.Label();
			this.rHex = new System.Windows.Forms.RadioButton();
			this.rDec = new System.Windows.Forms.RadioButton();
			this.BaseAddressLabel = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.gridMemory = new System.Windows.Forms.DataGrid();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridMemory)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Window;
			this.panel1.Controls.AddRange(new System.Windows.Forms.Control[] {
																				 this.textCellValue,
																				 this.CellLabel,
																				 this.vScrollBar1,
																				 this.txtAddress,
																				 this.CellValueLabel,
																				 this.rHex,
																				 this.rDec,
																				 this.BaseAddressLabel,
																				 this.groupBox1});
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(656, 40);
			this.panel1.TabIndex = 0;
			// 
			// textCellValue
			// 
			this.textCellValue.Enabled = false;
			this.textCellValue.Location = new System.Drawing.Point(264, 8);
			this.textCellValue.MaxLength = 3;
			this.textCellValue.Name = "textCellValue";
			this.textCellValue.Size = new System.Drawing.Size(72, 20);
			this.textCellValue.TabIndex = 13;
			this.textCellValue.Text = "0";
			this.textCellValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textCellValue_KeyPress);
			this.textCellValue.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textCellValue_KeyUp);
			// 
			// CellLabel
			// 
			this.CellLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.CellLabel.Location = new System.Drawing.Point(200, 20);
			this.CellLabel.Name = "CellLabel";
			this.CellLabel.Size = new System.Drawing.Size(64, 16);
			this.CellLabel.TabIndex = 19;
			this.CellLabel.Text = "00000000";
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.LargeChange = 1;
			this.vScrollBar1.Location = new System.Drawing.Point(152, 8);
			this.vScrollBar1.Maximum = 0;
			this.vScrollBar1.Minimum = -67108848;
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(18, 18);
			this.vScrollBar1.TabIndex = 18;
			this.vScrollBar1.ValueChanged += new System.EventHandler(this.vScrollBar1_ValueChanged);
			// 
			// txtAddress
			// 
			this.txtAddress.Location = new System.Drawing.Point(80, 8);
			this.txtAddress.MaxLength = 8;
			this.txtAddress.Name = "txtAddress";
			this.txtAddress.Size = new System.Drawing.Size(72, 20);
			this.txtAddress.TabIndex = 17;
			this.txtAddress.Text = "00000000";
			this.txtAddress.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.txtAddress.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAddress_KeyPress);
			this.txtAddress.TextChanged += new System.EventHandler(this.txtAddress_TextChanged);
			// 
			// CellValueLabel
			// 
			this.CellValueLabel.Location = new System.Drawing.Point(200, 6);
			this.CellValueLabel.Name = "CellValueLabel";
			this.CellValueLabel.Size = new System.Drawing.Size(56, 16);
			this.CellValueLabel.TabIndex = 16;
			this.CellValueLabel.Text = "Cell Value";
			// 
			// rHex
			// 
			this.rHex.Enabled = false;
			this.rHex.Location = new System.Drawing.Point(344, 20);
			this.rHex.Name = "rHex";
			this.rHex.Size = new System.Drawing.Size(48, 14);
			this.rHex.TabIndex = 15;
			this.rHex.Text = "Hex";
			this.rHex.CheckedChanged += new System.EventHandler(this.rHex_CheckedChanged);
			// 
			// rDec
			// 
			this.rDec.Checked = true;
			this.rDec.Enabled = false;
			this.rDec.Location = new System.Drawing.Point(344, 4);
			this.rDec.Name = "rDec";
			this.rDec.Size = new System.Drawing.Size(48, 14);
			this.rDec.TabIndex = 14;
			this.rDec.TabStop = true;
			this.rDec.Text = "Dec";
			this.rDec.CheckedChanged += new System.EventHandler(this.rDec_CheckedChanged);
			// 
			// BaseAddressLabel
			// 
			this.BaseAddressLabel.Location = new System.Drawing.Point(8, 10);
			this.BaseAddressLabel.Name = "BaseAddressLabel";
			this.BaseAddressLabel.Size = new System.Drawing.Size(80, 16);
			this.BaseAddressLabel.TabIndex = 12;
			this.BaseAddressLabel.Text = "Base Address";
			// 
			// groupBox1
			// 
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.groupBox1.Location = new System.Drawing.Point(0, 32);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(656, 8);
			this.groupBox1.TabIndex = 10;
			this.groupBox1.TabStop = false;
			// 
			// gridMemory
			// 
			this.gridMemory.AllowSorting = false;
			this.gridMemory.CaptionVisible = false;
			this.gridMemory.CausesValidation = false;
			this.gridMemory.DataMember = "";
			this.gridMemory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridMemory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
			this.gridMemory.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.gridMemory.Location = new System.Drawing.Point(0, 40);
			this.gridMemory.Name = "gridMemory";
			this.gridMemory.PreferredColumnWidth = 1;
			this.gridMemory.ReadOnly = true;
			this.gridMemory.RowHeadersVisible = false;
			this.gridMemory.RowHeaderWidth = 8;
			this.gridMemory.Size = new System.Drawing.Size(656, 222);
			this.gridMemory.TabIndex = 1;
			this.gridMemory.CurrentCellChanged += new System.EventHandler(this.Grid_CurCellChange);
			// 
			// frmMemoryView
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.gridMemory,
																		  this.panel1});
			this.Name = "frmMemoryView";
			this.Size = new System.Drawing.Size(656, 262);
			this.Resize += new System.EventHandler(this.frmMemoryView_Resize);
			this.Load += new System.EventHandler(this.frmMemoryView_Load);
			this.Validating += new System.ComponentModel.CancelEventHandler(this.frmMemoryView_Validating);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridMemory)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		#region Constructor

		public frmMemoryView()
		{
			mem = new Memory(0, int.MaxValue);
			iBaseAddr = 0;
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

		#endregion

		#region Consts

		const int COLS_IN_LINE = 16;
		const int ROWS_IN_PAGE = 32;
		const int page = COLS_IN_LINE*ROWS_IN_PAGE; //change only after know that scroller is in the range.
		const int ScrollerLength = 21;

		#endregion

		#region Methods

		public void SetMemoryObject(Memory m)
		{
			mem = m;
		}


		/// <summary>
		/// this function gets the changes and saves it to buffer of changing
		/// </summary>
		/// <param name="iStartingAddress"></param>
		/// <param name="iLength"></param>
		public void UpdateCells(int iStartingAddress, int iLength)
		{

			if (!(inTheRange(iStartingAddress) || inTheRange(iStartingAddress + iLength -1)))
				return ;
			for (int i=0; i < iLength; ++i)
				ChangesAddress.Add (iStartingAddress + i);
		}

		public void UpdateMemoryDisplay() 
		{
			for (int element = 0; element < ChangesAddress.Count; ++element)
			{
				if (!inTheRange((int)ChangesAddress[element]))
					continue;
				UpdateCell(((int)ChangesAddress[element] - iBaseAddr) % COLS_IN_LINE +1, 
					((int)ChangesAddress[element] - iBaseAddr) / COLS_IN_LINE, 
					mem.Read((int)ChangesAddress[element], 1, false)[0].ToString());
			}
			ChangesAddress.Clear();
		}

		/// <summary>
		/// this function do the actually updating on screen
		/// </summary>
		/// <param name="iColumn">cell position</param>
		/// <param name="iRow">cell position</param>
		/// <param name="sCellValue">the value</param>
		private void UpdateCell(int iColumn, int iRow, string sCellValue)
		{
			// changing the value in the char cell
			DataTable SourceTable = (DataTable) gridMemory.DataSource;
			DataRow tempRow = SourceTable.Rows[iRow];
			//if (rDec.Checked)
			sCellValue = Convert.ToString (Convert.ToInt16(sCellValue), 16);
			
			tempRow[iColumn] = new String('0', 2 - sCellValue.Length) + sCellValue.ToUpper();
			// changing the value in the last cell

			char[] sValues = tempRow[COLS_IN_LINE + 1].ToString().ToCharArray();
			int iCell = Convert.ToInt16 ((string)tempRow[iColumn], 16);
			sValues[iColumn - 1] = 
				((iCell > 31) && (iCell < 177)) ? Convert.ToChar(iCell) : '.' ;
			tempRow[COLS_IN_LINE + 1] = new string (sValues);
		}

		/// <summary>
		/// this function gets address and check if it the shown memory
		/// </summary>
		/// <param name="i">the Address that we need to check</param>
		/// <returns>bool, true if is the range</returns>
		private bool inTheRange(int iAddress)
		{
			return (iAddress >= iBaseAddr && iAddress < (iBaseAddr + COLS_IN_LINE * ROWS_IN_PAGE));
		}

		public void DrawMemory(int iStartingAddress)
		{
			MemoryTable.Clear();
			iBaseAddr = iStartingAddress - iStartingAddress % COLS_IN_LINE;
			if (iStartingAddress % COLS_IN_LINE != 0) iStartingAddress -= iStartingAddress % COLS_IN_LINE;

			int iFillSize = 8;//Convert.ToString(iStartingAddress+COLS_IN_LINE*ROWS_IN_PAGE, 16).Length;

			object[] oLine = new object[COLS_IN_LINE + 2];

			for (int iLine = 0; iLine < ROWS_IN_PAGE; ++iLine)
			{
				// Draw addresses on the left side
				string sLine = Convert.ToString((iStartingAddress + iLine * COLS_IN_LINE), 16).ToUpper();
				if (sLine.Length < iFillSize) 
					sLine = new String('0', iFillSize - sLine.Length) + sLine;
				oLine[0] = sLine;
				string values = "";
				for (int iCounter = 0; iCounter < COLS_IN_LINE; ++iCounter)
				{
					string sCellOutput;
					int iCell = mem.Read(iStartingAddress+iCounter+ iLine * COLS_IN_LINE, 1, false);
					if (Settings.Simulator.bShowMemoryAsHex) 
					{
						sCellOutput = Convert.ToString( iCell, 16).ToUpper();
						if (sCellOutput.Length == 1) sCellOutput = "0" + sCellOutput;
					}
					else sCellOutput = iCell.ToString();
					oLine[iCounter+1] = sCellOutput;
					if ((iCell > 31) && (iCell < 177))
						values += Convert.ToChar (iCell);
					else
						values += '.';
				}
				oLine[COLS_IN_LINE + 1] = values;
				MemoryTable.Rows.Add(oLine);
			}
			gridMemory.DataSource = MemoryTable;
		}

		public void ChangeColumnsWidth() 
		{
			if (this.ClientSize.Width < 400) return;
			try
			{
				DataTable dataTable = (DataTable)gridMemory.DataSource;
				// Clear any existing table styles.
				if (dataTable == null) return;
				gridMemory.TableStyles.Clear();

				// Use mapping name that is defined in the data source.
				DataGridTableStyle tableStyle = new DataGridTableStyle();
				tableStyle.MappingName = dataTable.TableName;
				tableStyle.RowHeadersVisible = false;
				// Now create the column styles within the table style.
				DataColumn dataColumn = dataTable.Columns[0];
				DataGridTextBoxColumn columnStyle = new DataGridTextBoxColumn();
				columnStyle.TextBox.Enabled = true;
				columnStyle.HeaderText = dataColumn.ColumnName;
				columnStyle.MappingName = dataColumn.ColumnName;
				columnStyle.Width = this.ClientSize.Width/(COLS_IN_LINE*2+8+8)*6;
				// Add the new column style to the table style.
				tableStyle.GridColumnStyles.Add(columnStyle);
				gridMemory.TableStyles.Add(tableStyle);

				for (int i = 1; i <= COLS_IN_LINE; ++i)
				{
					dataTable = (DataTable)gridMemory.DataSource;
					tableStyle.MappingName = dataTable.TableName;
					dataColumn = dataTable.Columns[i];
					columnStyle = new DataGridTextBoxColumn();
					columnStyle.TextBox.Enabled = true;
					columnStyle.HeaderText = dataColumn.ColumnName;
					columnStyle.MappingName = dataColumn.ColumnName;
					columnStyle.Width = this.ClientSize.Width/(COLS_IN_LINE*2+8+8)*2+1;
					// Add the new column style to the table style.
					tableStyle.GridColumnStyles.Add(columnStyle);
				}
				dataTable = (DataTable)gridMemory.DataSource;
				tableStyle.MappingName = dataTable.TableName;
				dataColumn = dataTable.Columns[COLS_IN_LINE+1];
				columnStyle = new DataGridTextBoxColumn();
				columnStyle.TextBox.Enabled = true;
				columnStyle.HeaderText = dataColumn.ColumnName;
				columnStyle.MappingName = dataColumn.ColumnName;
				columnStyle.Width = this.ClientSize.Width - this.ClientSize.Width/(COLS_IN_LINE*2+8+8)*(6+COLS_IN_LINE*2) -COLS_IN_LINE - ScrollerLength;
				// Add the new column style to the table style.
				tableStyle.GridColumnStyles.Add(columnStyle);
			}
			catch(Exception e) { MessageBox.Show(e.Message); }

			rDec.Location = new Point(Math.Max(344, this.ClientSize.Width - 48), rDec.Location.Y);
			rHex.Location = new Point(Math.Max(344, this.ClientSize.Width - 48), rHex.Location.Y);
			textCellValue.Location = new Point(Math.Max(264, this.ClientSize.Width - (48 + 4 + 72)), textCellValue.Location.Y);
			CellLabel.Location = new Point(Math.Max(200, this.ClientSize.Width - (48 + 4 + 72 + 56)),CellLabel.Location.Y);
			CellValueLabel.Location = new Point(Math.Max(200, this.ClientSize.Width - (48 + 4 + 72 + 56)),CellValueLabel.Location.Y);
		}

		public void ResetView()
		{
			textCellValue.Text = "0";
			iBaseAddr = 0;
			vScrollBar1.Value = 0;
			txtAddress.Text = "00000000";
			CellLabel.Text = "00000000";
			rDec.Enabled = false;
			rHex.Enabled = false;
			rDec.Checked = true; 
		}
		#endregion

		#region Events

		private void frmMemoryView_Load(object sender, System.EventArgs e)
		{
			// Making table
			MemoryTable = new DataTable("Memory View");
			DataColumn ConstractorColumn;

			ConstractorColumn = new DataColumn("Address", typeof(string));
			ConstractorColumn.MaxLength = 8;
			ConstractorColumn.ReadOnly = true;
			ConstractorColumn.Caption = "Address";
			ConstractorColumn.AllowDBNull = false;
			MemoryTable.Columns.Add(ConstractorColumn);

			for (int i = 0; i < COLS_IN_LINE; ++i)
			{
				ConstractorColumn = new DataColumn(Convert.ToString(i, 16).ToUpper(), typeof(string));
				ConstractorColumn.MaxLength = 2;
				ConstractorColumn.ReadOnly = false;
				ConstractorColumn.Caption = Convert.ToString(i, 16).ToUpper();
				ConstractorColumn.AllowDBNull = false;
				MemoryTable.Columns.Add(ConstractorColumn);
			}
			ConstractorColumn = new DataColumn("Values", typeof(string));
			ConstractorColumn.MaxLength = COLS_IN_LINE;
			ConstractorColumn.ReadOnly = false;
			ConstractorColumn.Caption = "Values";
			//ConstractorColumn.AllowDBNull = false;
			MemoryTable.Columns.Add(ConstractorColumn);

			//ChangeColumnsWidth();
			DrawMemory(iBaseAddr);

		}

		private void frmMemoryView_Resize(object sender, System.EventArgs e)
		{
			bnowResizing = true;
			ChangeColumnsWidth();
			gridMemory.AllowSorting = false;
			bnowResizing = false;
			this.Validate();
		}

		private void vScrollBar1_ValueChanged(object sender, System.EventArgs e)
		{
			if (!bChangeTextAddress) return;
			string Text = Convert.ToString(-(page * vScrollBar1.Value), 16).ToUpper();
//			if (string.Compare(new String('0', 8 - Text.Length) + Text, txtAddress.Text) < 0)
//			{
//				--vScrollBar1.Value;
//				return;
//			}
			txtAddress.Text = new String('0', 8 - Text.Length) + Text;
			iBaseAddr = -(page * vScrollBar1.Value);
		}

		private void txtAddress_TextChanged(object sender, System.EventArgs e)
		{
			//txtAddress.Text = txtAddress.Text.ToUpper();
			if (txtAddress.Text == "") return;

			try
			{
				int uiValue = int.Parse(txtAddress.Text, System.Globalization.NumberStyles.AllowHexSpecifier);

				//				if (uiValue > int.MaxValue - ROWS_IN_PAGE * COLS_IN_LINE)
				//				{
				//					MessageBox.Show("Invalid address, please put a valid address and try again");
				//					return;
				//				}

				DrawMemory((int)uiValue);

				bChangeTextAddress = false;
				vScrollBar1.Value = -(int)((uint)uiValue / page);
				bChangeTextAddress = true;
			}
			catch (OverflowException )
			{
				MessageBox.Show("Invalid address, please put a valid address and try again");
			}

		}

		private void txtAddress_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			int KeyCode = (int)e.KeyChar;  
			if (!
				((KeyCode >= '0' && KeyCode <= '9') || (KeyCode >= 'A' && KeyCode <= 'F')
				|| (KeyCode >= 'a' && KeyCode <= 'f'))
				&& KeyCode != 8 && KeyCode != 46) e.Handled=true;
		}

		private void frmMemoryView_Validating(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = bnowResizing;
		}

		protected void Grid_CurCellChange(object sender, EventArgs e)
		{

			// prepare to get data from table
			DataTable SourceTable = (DataTable) gridMemory.DataSource;
			DataRow tempRow = SourceTable.Rows[gridMemory.CurrentCell.RowNumber];

			// if we not on the cell from 1 to 15
			if ((gridMemory.CurrentCell.ColumnNumber < 1) || (gridMemory.CurrentCell.ColumnNumber > COLS_IN_LINE))
			{
				textCellValue.Enabled = false;
				rDec.Enabled = false;
				rHex.Enabled = false;
				CellLabel.Text = tempRow[0].ToString();
				return;
			}

			// if we on cells 1 to 15
			textCellValue.Enabled = true;
			rDec.Enabled = true;
			rHex.Enabled = true;
			int iAddress = iBaseAddr + 16 * gridMemory.CurrentCell.RowNumber + gridMemory.CurrentCell.ColumnNumber -1;
			string Text = Convert.ToString(iAddress, 16).ToUpper();
			CellLabel.Text = new String('0', 8 - Text.Length) + Text;
			
			if (rDec.Checked) 
				textCellValue.Text = Convert.ToInt16 (tempRow[gridMemory.CurrentCell.ColumnNumber].ToString(),16).ToString();
			else
				textCellValue.Text = tempRow[gridMemory.CurrentCell.ColumnNumber].ToString();
		}

		private void rDec_CheckedChanged(object sender, System.EventArgs e)
		{
			if (rDec.Checked)
			{
				if (textCellValue.Text.Length > 0)
					textCellValue.Text = Convert.ToInt16 (textCellValue.Text,16).ToString();
				textCellValue.MaxLength = 3;
			}
		}

		private void rHex_CheckedChanged(object sender, System.EventArgs e)
		{
			if (rHex.Checked)
			{
				if (textCellValue.Text.Length > 0)
					textCellValue.Text = Convert.ToString (Convert.ToInt16 (textCellValue.Text,10), 16).ToUpper();
				textCellValue.MaxLength = 2;
			}		
		}
		
		private void textCellValue_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			int KeyCode = e.KeyChar;
			if (rDec.Checked)
			{
				if (!(KeyCode >= '0' && KeyCode <= '9') && KeyCode != 8 && KeyCode != 46)
				{
					e.Handled=true;
					return;
				}
			}
			if (!((KeyCode >= '0' && KeyCode <= '9') || (KeyCode >= 'A' && KeyCode <= 'F')
				|| (KeyCode >= 'a' && KeyCode <= 'f')) && KeyCode != 8 && KeyCode != 46)
			{
				e.Handled=true;
				return;
			}
		}

		private void textCellValue_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			string sCellValue = textCellValue.Text;
			if (sCellValue == "") sCellValue = "0";
			if (rDec.Checked && Convert.ToInt32 (sCellValue) > 255)
			{
				sCellValue = "255";
				textCellValue.Text = "255";
			}
			// changing the value in the memory
			mem.Write (iBaseAddr+ (gridMemory.CurrentCell.ColumnNumber - 1) + 
				gridMemory.CurrentCell.RowNumber * COLS_IN_LINE, 
				new VAX11Internals.CodeBlock((int) 1, Convert.ToByte(sCellValue, (rDec.Checked) ? 10 : 16)));
			
			//UpdateCell(gridMemory.CurrentCell.ColumnNumber, gridMemory.CurrentCell.RowNumber, sCellValue);
		}

		#endregion
	}
}
