using System;
using System.Collections;

namespace VAX11Compiler
{
	/// <summary>
	/// Compiler Comment Container
	/// </summary>

	public class CompilerComment
	{
		private ArrayList _commentsList;

		/// <summary>
		/// Constructor
		/// </summary>
		public CompilerComment()
		{
			_commentsList = new ArrayList();
		}

		/// <summary>
		/// Add entry to the list
		/// </summary>
		/// <param name="iLine">Line Number</param>
		/// <param name="iAddress">Message related to that line</param>
		public void AddEntry(int iLine, CompilerMessage msg)
		{
			CompilerCommentEntry en = new CompilerCommentEntry(iLine, msg);
			_commentsList.Add(en);
		}

		/// <summary>
		/// Clear all entries
		/// </summary>
		public void ResetTable()
		{
			_commentsList.Clear();
		}

		/// <summary>
		/// gets line number an return his index in the list, if there is no error in this line return -1
		/// </summary>
		/// <returns>return the index of the error, if there is no error in this line return -1</returns>
		public int ErrorEntryIndex (int iLine)
		{
			for (int index = 0; index < _commentsList.Count; ++index)
				if (((CompilerCommentEntry)_commentsList[index]).Line == iLine)
					return index;
			return -1;
		}


		public ArrayList comments
		{
			get
			{
				return _commentsList;
			}
		}

		/// <summary>
		/// Change all line numbers for errors from selected start 
		/// line number by selected offset. doesn't check nothing. assuming the user
		/// knows what he's doing
		/// </summary>
		/// <param name="iFromLine">Start line number</param>
		/// <param name="iOffset">Offset (in line numbers)</param>
		public void updateErrorsLinesNumber(int iFromLine, int iOffset)
		{
			if (iOffset > 0)
				for (int iCounter = 0; iCounter < _commentsList.Count; ++iCounter)
				{
					CompilerCommentEntry curLine = ((CompilerCommentEntry)_commentsList[iCounter]);
					if (curLine.Line >= iFromLine) curLine.Line += iOffset;
				}
			else if (iOffset < 0)
				RemoveLines(iFromLine, iOffset);
		}

		/// <summary>
		/// Change all line numbers for errors from selected start 
		/// line number by selected offset. doesn't check nothing. assuming the user
		/// knows what he's doing
		/// </summary>
		/// <param name="iFromLine">Start line number</param>
		/// <param name="iOffset">Offset (in line numbers)</param>
		public void RemoveLines(int iFromLine, int iOffset)
		{
			for (int iCounter = 0; iCounter < _commentsList.Count; ++iCounter)
			{
				CompilerCommentEntry curLine = ((CompilerCommentEntry)_commentsList[iCounter]);
				if (curLine.Line > iFromLine - iOffset)
					curLine.Line += iOffset;
				else if (curLine.Line > iFromLine) 
					_commentsList.Remove(curLine);
			}
		}
	}



	/// <summary>
	/// Single entry in the compiler comment
	/// </summary>
	public class CompilerCommentEntry
	{
		private int _Line;

		public int Line
		{
			get { return _Line; }
			set { _Line = value; }
		}

		private readonly CompilerMessage _msgID;
		public CompilerMessage msgID
		{
			get
			{
				return _msgID;
			}
		}

		public CompilerCommentEntry(int iLine, CompilerMessage m)
		{
			_Line = iLine;
			_msgID = m;
		}
	}
}
