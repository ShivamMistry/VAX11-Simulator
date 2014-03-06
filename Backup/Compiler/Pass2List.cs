using System;
using System.Collections;
using System.Collections.Specialized;

namespace VAX11Compiler
{

	/// <summary>
	/// Single entry on the Pass2List
	/// </summary>
	class Pass2Entry
	{
		#region Members

		/// <summary>
		/// LC - Location where the update need to take place
		/// </summary>
		private int _Where;

		/// <summary>
		/// How many bytes are allowed to be change
		/// </summary>
		private int _Size;

		/// <summary>
		/// Expression that need calculate
		/// </summary>
		private string _Expression;

		/// <summary>
		/// True if the number is allowed to be negative
		/// </summary>
		private bool _Negative;

		/// <summary>
		/// True if the expression is operand. else if it is other expression
		/// </summary>
		private bool _IsOperand;

		/// <summary>
		/// The operand type, in case the expression is operand
		/// </summary>
		private string _OperandType;

		/// <summary>
		/// Constant name, in case we got value, not label
		/// </summary>
		private string _constantName;

		/// <summary>
		/// True if the expression is value. else if it label
		/// </summary>
		private bool _IsValue;

		#endregion

		#region Constructors
        
		/// <summary>
		/// Consturctor for creating labels entries
		/// </summary>
		/// <param name="iWhere">Where should we insert the label (LC)</param>
		/// <param name="iSize">Size of the block</param>
		/// <param name="sExpression">Expression to calculate</param>
		/// <param name="bNegative">Is the operand allowed to be negative</param>
		public Pass2Entry(int iWhere, int iSize, string sExpression, 
			bool bNegative)
		{
			_Where = iWhere;
			_Size = iSize;
			_Expression = sExpression;
			_Negative = bNegative;
			_IsOperand = false;
			_IsValue = false;
			_constantName = "";
		}

		/// <summary>
		/// Consturctor for creating opeands label
		/// </summary>
		/// <param name="iWhere">Where the operand is located (LC)</param>
		/// <param name="iSize">Size of block code to insert</param>
		/// <param name="sExpression">Expression to calculate</param>
		/// <param name="bNegative">Is the operand allowed to be negative</param>
		/// <param name="operandType">Type of operand</param>
		public Pass2Entry(int iWhere, int iSize, string sExpression, 
			bool bNegative, string operandType)
		{
			_Where = iWhere;
			_Size = iSize;
			_Expression = sExpression;
			_Negative = bNegative;
			_IsOperand = true;
			_OperandType = operandType;
			_IsValue = false;
			_constantName = "";
		}

		/// <summary>
		/// Consturctor for creating values
		/// </summary>
		/// <param name="sConstantName">Constant Name</param>
		/// <param name="sExpression">Constant Value</param>
		/// <param name="iCurLineNumber">Line number where the value is defined</param>
		public Pass2Entry(string sConstantName, string sExpression, int iCurLineNumber)
		{
			_Where			= iCurLineNumber;
			_Size			= 4;
			_Expression		= sExpression;
			_constantName	= sConstantName;
			_Negative		= true;
			_IsOperand		= false;
			_IsValue		= true;
		}

		#endregion

		#region Properties

		/// <summary>
		/// LC - Location where the update need to take place
		/// </summary>
		public int Where
		{
			get
			{
				return _Where;
			}
		}

		/// <summary>
		/// How many bytes are allowed to be change
		/// </summary>
		public int Size
		{
			get
			{
				return _Size;
			}
		}

		/// <summary>
		/// Expression that need calculate
		/// </summary>
		public string Expression
		{
			get
			{
				return _Expression;
			}
		}

		/// <summary>
		/// True if the number is allowed to be negative
		/// </summary>
		public bool Negative
		{
			get
			{
				return _Negative;
			}
		}

		/// <summary>
		/// True if the expression is operand. else if it is other expression
		/// </summary>
		public bool IsOperand
		{
			get
			{
				return _IsOperand;
			}
		}

		/// <summary>
		/// The operand type, in case the expression is operand
		/// </summary>
		public string OpType
		{
			get
			{
				return _OperandType;
			}
		}

		/// <summary>
		/// Constant name, in case we got value, not label
		/// </summary>
		public string ConstantName
		{
			get
			{
				return _constantName;
			}
		}


		/// <summary>
		/// True if the expression is value. else if it label
		/// </summary>
		public bool IsValue
		{
			get
			{
				return _IsValue;
			}
		}

		#endregion
	}

	/// <summary>
	/// Pass2List Table containter
	/// </summary>
	class Pass2List
	{

		#region Members

		/// <summary>
		/// List contains all pass2 entries.
		/// </summary>
		private ListDictionary _Pass2List;

		#endregion

		#region Constructor

		/// <summary>
		/// Creates new empty pass2 list
		/// </summary>
		public Pass2List()
		{
			_Pass2List = new ListDictionary();
		}

		#endregion

		#region Methods

		/// <summary>
		/// Add new entry to the list
		/// </summary>
		/// <param name="vNewEntry">The entry to add</param>
		/// <remarks>Throw CompileError if entry on the same line already defined.
		/// Assuming we will add double entry for the same LC only if we on pass2 and the label wasn't defined</remarks>
		public void AddEntry(Pass2Entry vNewEntry)
		{
			if (_Pass2List.Contains(vNewEntry.Where))
				throw new CompileError(CompilerMessage.UNDEFINED_SYMBOL);
			_Pass2List.Add(vNewEntry.Where,vNewEntry);
		}

		/// <summary>
		/// Remove all entries from the symbols list
		/// </summary>
		public void ResetTable()
		{
			_Pass2List.Clear();
		}


		/// <summary>
		/// Returns all values saved in the list
		/// </summary>
		/// <returns>ICollection with the values</returns>
		public ListDictionary GetPass2List()
		{
			return _Pass2List;
		}

		#endregion

	}
}
