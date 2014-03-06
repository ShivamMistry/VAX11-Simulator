using System;
using System.Collections;

namespace VAX11Environment
{
	/// <summary>
	/// BreakPointEntry - saves one breakpoint information.
	/// </summary>
	public class BreakPointEntry
	{
		#region Members

		private int _PC;
		private int _LINE;

		#endregion

		#region Properties

		public int PC
		{
			get { return _PC; }
			set { _PC = value; }
		}

		public int Line
		{
			get { return _LINE; }
			set { _LINE = value; }
		}

		#endregion

		public BreakPointEntry(int iLine, int iPC)
		{
			_PC = iPC;
			_LINE = iLine;
		}
	}


	/// <summary>
	/// BreakPoint Table containter
	/// </summary>
	public class BreakPointList
	{
		private int icount = 0;
		private int _iCurItem = 0;
		IDictionaryEnumerator BreakPointEnum = null;

		private Hashtable _BreakPointsList;
		public BreakPointList()
		{
			_BreakPointsList = new Hashtable();
		}

		/// <summary>
		/// Add new entry to the list
		/// </summary>
		/// <param name="vNewEntry">The entry to add</param>
		/// <remarks>If the entry is already exists, we ingore it</remarks>
		public void AddEntry(BreakPointEntry vNewEntry)
		{
			if (_BreakPointsList.Contains(vNewEntry.Line)) return;
			_BreakPointsList.Add(vNewEntry.Line, vNewEntry);
			++icount;
		}

		/// <summary>
		/// Remove all entries from the symbols list
		/// </summary>
		public void ResetTable()
		{
			_BreakPointsList.Clear();
		}


		/// <summary>
		/// Returns all values saved in the list
		/// </summary>
		public Hashtable GetBreakPointsList()
		{
			return _BreakPointsList;
		}

		/// <summary>
		/// Deletes a break-point. If it doesn't exists, we ignore it
		/// </summary>
		/// <param name="iPC">PC of the break-point to delete</param>
		public void DelEntry(int iLine)
		{
			if (_BreakPointsList.Contains(iLine))	
			{
				_BreakPointsList.Remove(iLine);
				--icount;
			}
		}

		/// <summary>
		/// Returns true if the given Line is in the break-point list
		/// </summary>
		/// <param name="iLine">LINE to check</param>
		/// <returns>Returns true if the given Line is in the break-point list</returns>
		public bool Contains(int iLine)
		{
			return _BreakPointsList.Contains(iLine);
		}


		/// <summary>
		/// 
		/// </summary>
		public void GetFirst()
		{
			if (icount == 0) return;
			_iCurItem = 0;
			BreakPointEnum = _BreakPointsList.GetEnumerator();
			BreakPointEnum.Reset();
		}

		/// <summary>
		/// move the enumeretor to the next member
		/// </summary>
		/// <returns>if succseded to increase</returns>
		public bool GetNext()
		{
			if (icount == 0) return false;
			if (BreakPointEnum.MoveNext())
			{
				++_iCurItem;
				return true;
			}
			else
			{
				_iCurItem = -1;
				return false;
			}
		}

		/// <summary>
		/// returns the Line from the enumeretor
		/// </summary>
		public int GetCurrentLine()
		{
			return (int) ((BreakPointEntry)BreakPointEnum.Value).Line;
		}

		/// <summary>
		/// returns the PC from the enumeretor
		/// </summary>
		public int GetCurrentPC()
		{
			return (int) ((BreakPointEntry)BreakPointEnum.Value).PC;
		}

		public bool ContainsPC(int iPC)
		{
			GetFirst();
			while (GetNext())
			{
				if (GetCurrentPC() == iPC)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Change all line numbers for breakpoint from selected start 
		/// line number by selected offset
		/// </summary>
		/// <param name="iFromLine">Start line number</param>
		/// <param name="iOffset">Offset (in line numbers)</param>
		public void updateBreakPointLinesNumber(int iFromLine, int iOffset)
		{
			if (iOffset > 0)
			{
				Hashtable TempBreakPointsList = new Hashtable();
				GetFirst();
				while (GetNext())
				{
					if (GetCurrentLine() > iFromLine)
						TempBreakPointsList.Add	(((BreakPointEntry)BreakPointEnum.Value).Line + iOffset, new BreakPointEntry (((BreakPointEntry)BreakPointEnum.Value).Line + iOffset, -1));
					else
						TempBreakPointsList.Add	(((BreakPointEntry)BreakPointEnum.Value).Line, new BreakPointEntry (((BreakPointEntry)BreakPointEnum.Value).Line, -1));
				}
				_BreakPointsList = TempBreakPointsList;
			}
			else if (iOffset < 0)
				RemoveLines(iFromLine, iOffset);
		}
		/// <summary>
		/// this procedure remove all the line between iFromLine and FromLine+Offset, and 
		/// update the lines of other lines
		/// </summary>
		/// <param name="iFromLine">Start line number</param>
		/// <param name="iOffset">Offset (in line numbers)</param>
		public void RemoveLines(int iFromLine, int iOffset)
		{
			Hashtable TempBreakPointsList = new Hashtable();
			GetFirst();
			while (GetNext())
			{
				if (GetCurrentLine() > iFromLine - iOffset)
					TempBreakPointsList.Add	(((BreakPointEntry)BreakPointEnum.Value).Line + iOffset, new BreakPointEntry (((BreakPointEntry)BreakPointEnum.Value).Line + iOffset, -1));
				else if (GetCurrentLine() <= iFromLine)
					TempBreakPointsList.Add	(((BreakPointEntry)BreakPointEnum.Value).Line, new BreakPointEntry (((BreakPointEntry)BreakPointEnum.Value).Line, -1));
			}
			_BreakPointsList = TempBreakPointsList;
		}
	}
}
