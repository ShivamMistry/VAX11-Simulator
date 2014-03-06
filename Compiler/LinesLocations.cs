using System;
using System.Collections;

using VAX11Internals;

namespace VAX11Compiler
{
	/// <summary>
	/// Summary description for LinesLocations.
	/// </summary>
	public class LinesLocations
	{
		#region Members

		private Hashtable linesHash;

		private int iMaxLine;

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public LinesLocations()
		{
			linesHash = new Hashtable();
		}

		#endregion

		#region Insert and delete items

		/// <summary>
		/// Add entry to Lines Locations list
		/// </summary>
		/// <param name="iLine">Line Number</param>
		/// <param name="iAddress">Address related to that line</param>
		public void AddEntry(int iLine, int iAddress)
		{
			if (linesHash.ContainsKey(iLine)) throw new PanicException();

			LineInformation newLine = new LineInformation(iLine, iAddress, -1);
			linesHash[iLine] = newLine;

			// Assuming the inserted line is a line with the largest index
			iMaxLine = iLine;
		}

		/// <summary>
		/// Clear all entries
		/// </summary>
		public void ResetTable()
		{
			linesHash.Clear();
		}

		#endregion


		#region Other Methods
		
		/// <summary>
		/// Get Max Line Number - O(1) :)
		/// </summary>
		public int MaxLine
		{
			get { return iMaxLine; }
		}

		/// <summary>
		/// Sets the end address of a line
		/// </summary>
		/// <param name="iLine">Line Number</param>
		/// <param name="iLineEnd">The address of the end of the line</param>
		public void SetEndAddress(int iLine, int iLineEnd)
		{
			if (!linesHash.ContainsKey(iLine)) throw new NoSuchLineException();
			((LineInformation)linesHash[iLine]).iEndAddress = iLineEnd;
		}

		/// <summary>
		/// Sets if there were errors on specific line
		/// </summary>
		/// <param name="iLine">Line Number</param>
		/// <param name="bHasError">True if errors found on the line</param>
		public void SetErrorsOnLine(int iLine, bool bHasError)
		{
			if (!linesHash.ContainsKey(iLine)) throw new NoSuchLineException();
			((LineInformation)linesHash[iLine]).bHasErrors = bHasError;
		}


		/// <summary>
		/// Indicate if a line in the code has errors.
		/// </summary>
		/// <param name="iLine">Line Number</param>
		/// <returns>True if the line has error, else false</returns>
		public bool LineHasErrors(int iLine)
		{
			if (!linesHash.ContainsKey(iLine)) throw new NoSuchLineException();
			return ((LineInformation)linesHash[iLine]).bHasErrors;
		}

		#endregion

		#region Match Addresses & Lines

		/// <summary>
		/// Get the address related to specific line
		/// </summary>
		/// <param name="iLine">The line</param>
		/// <returns>Starting address of the line</returns>
		public int GetStartingAddress(int iLine)
		{
			if (!linesHash.ContainsKey(iLine)) throw new NoSuchLineException();
			return ((LineInformation)linesHash[iLine]).iStartAddress;
		}

		/// <summary>
		/// Get the end line address
		/// </summary>
		/// <param name="iLine">The line</param>
		/// <returns>Ending address of the line</returns>
		public int GetEndAddress(int iLine)
		{
			if (!linesHash.ContainsKey(iLine)) throw new NoSuchLineException();
			return ((LineInformation)linesHash[iLine]).iEndAddress;
		}

		/// <summary>
		/// Gets line and returns its address
		/// </summary>
		/// <param name="iAddress">Address</param>
		/// <returns>Line Number</returns>
		public int GetLineFromAddress(int iAddress)
		{
			int iMax = MaxLine;
			int iCounter, retValue = 0;
			
			// iCounter < iMax or else if the line is the last line we will display line+1
			for (iCounter = 1; iCounter <= iMax; ++iCounter)
			{
				if (((LineInformation)linesHash[iCounter]).iStartAddress <= iAddress)
					retValue++;
				else
					break;
			}

			return retValue;
		}


		/// <summary>
		/// Set the address related to specific line
		/// </summary>
		/// <param name="iLine">The line</param>
		/// <param name="iNewAddress">Starting address of the line</param>
		public void SetStartingAddress(int iLine, int iNewAddress)
		{
			if (!linesHash.ContainsKey(iLine)) throw new NoSuchLineException();
			((LineInformation)linesHash[iLine]).iStartAddress = -1;
		}

		#endregion

		#region Sub-Class

		/// <summary>
		/// Saves Line Info
		/// </summary>
		private class LineInformation
		{
			/// <summary>
			/// Line Number
			/// </summary>
			public int iLine;

			/// <summary>
			/// Starting address of the line's code
			/// </summary>
			public int iStartAddress;

			/// <summary>
			/// Ending address of the line's code
			/// </summary>
			public int iEndAddress;


			/// <summary>
			/// true if the line has errors on it
			/// </summary>
			public bool bHasErrors;

			public LineInformation(int line, int startaddr, int endaddr)
			{
				iLine = line;
				iStartAddress = startaddr;
				iEndAddress = endaddr;
				bHasErrors = false;
			}
		}

		#endregion

	}

	class NoSuchLineException : Exception { }

}