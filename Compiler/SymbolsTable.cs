using System;
using System.Collections;

namespace VAX11Compiler
{
	public class SymbolsTableEntry
	{
		private string _Name;
		private long _Value;
		private SymbolsTable.SymbolType _Type;
		private int _Line;

		/// <summary>
		/// Symbol's Name
		/// </summary>
		public string Name
		{
			get
			{
				return _Name;	
			}
		}
		
		public long Value
		{
			get
			{
				return _Value;
			}
		}

		/// <summary>
		/// Type of symbol - constant or label
		/// </summary>
		public SymbolsTable.SymbolType Type
		{
			get
			{
				return _Type;
			}
		}

		/// <summary>
		/// Line Number where the symbol is located
		/// </summary>
		public int Line
		{
			get
			{
				return _Line;
			}
		}


		public SymbolsTableEntry(string sName, long iValue,
			SymbolsTable.SymbolType vType, int iLine)
		{
			_Name = sName;
			_Value = iValue;
			_Type = vType;
			_Line = iLine;
		}

	}

	/// <summary>
	/// Symbols Table containter
	/// </summary>
	public class SymbolsTable
	{
		public enum SymbolType { LABEL, CONSTANT };

		private Hashtable sybHash;
		public SymbolsTable()
		{
			sybHash = new Hashtable();
		}

		/// <summary>
		/// Add new entry to the list
		/// </summary>
		/// <param name="vNewEntry">The entry to add</param>
		/// <remarks>Throw CompileError if the label already defined</remarks>
		public void AddEntry(SymbolsTableEntry vNewEntry)
		{
			if (sybHash.ContainsKey(vNewEntry.Name))
				throw new CompileError(CompilerMessage.LABEL_ALREADY_DEFINED);
			sybHash[vNewEntry.Name] = vNewEntry;
		}

		/// <summary>
		/// Remove all entries from the symbols list
		/// </summary>
		public void ResetTable()
		{
			sybHash.Clear();
		}

		/// <summary>
		/// Gets the number/address related to the given label
		/// </summary>
		/// <param name="sSymbolName">Label to exaime</param>
		/// <returns>SymbolsTableEntry with the requested information</returns>
		/// <remarks>Throw ComplierError if label not defined</remarks>
		public SymbolsTableEntry SymbolValue(string sSymbolName)
		{
			if (!sybHash.ContainsKey(sSymbolName))
				throw new CompileError(CompilerMessage.UNDEFINED_SYMBOL);
			return (SymbolsTableEntry)sybHash[sSymbolName];
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return sybHash.GetEnumerator();
		}
	}
}
