using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized;

using VAX11Internals;
using VAX11Settings;

namespace VAX11Compiler
{
	/// <summary>
	/// Assembler - Input: Assembly code. Output: Machine Code.
	/// </summary>
	public class Assembler
	{

		#region Members

		/// <summary>
		/// List containing all compile error
		/// </summary>
		private CompilerComment			_comments;

		/// <summary>
		/// List contains all the symbols defined in the code
		/// </summary>
		private SymbolsTable			_symbolsTable;

		/// <summary>
		/// List of symbols to be calculate during pass2.
		/// </summary>
		private Pass2List				_pass2List;

		/// <summary>
		/// Matchs between line numbers and addresses
		/// </summary>
		private LinesLocations			_linesLocations;

		/// <summary>
		/// Compiler current state - at pass1, at pass2 or idle
		/// </summary>
		private CompilerStates			_state;

		/// <summary>
		/// Block containing all the program machine code and also other program's specific information
		/// </summary>
		private ProgramBlock			_programCode;

		/// <summary>
		/// Array containing the original user code, for display purpose
		/// </summary>
		private string[] _sOriginalCode = null;

		/// <summary>
		/// Flag the tells if we need to look for special label for the program startup.
		/// If its value is "" we looks for the default entry-point. else we look for the special label.
		/// </summary>
		private string _LocateEntryPoint = "";

		#endregion

		#region Events

		/// <summary>
		/// Delegate for detecting error event
		/// </summary>
		public delegate void compileErrorHandler(int iLine, CompilerMessage msg);

		/// <summary>
		///  the Event
		/// </summary>
		public event compileErrorHandler OnCompileError;

		#endregion

		#region Properties

		/// <summary>
		/// Number of errors during the compile
		/// </summary>
		public int NumberOfErrors
		{
			get { return _comments.comments.Count; }
		}

		/// <summary>
		/// Compiler's symbols table
		/// </summary>
		public SymbolsTable TheSymbolsTable
		{
			get { return _symbolsTable; }
		}

		/// <summary>
		/// compiler's comments collected during compilation
		/// </summary>
		public CompilerComment CompileMessages
		{
			get { return _comments; }
		}

		/// <summary>
		/// Matchs between line numbers and addresses
		/// </summary>
		public LinesLocations Lineslocations
		{
			get { return _linesLocations; }
		}


		public ProgramBlock ProgramMachineCode
		{
			get { return _programCode; }
		}

		#endregion

		#region enums

		private enum CompilerStates { IDLE, PASS1, PASS2};

		#endregion

		#region Constructor

		/// <summary>
		/// Default constructor
		/// </summary>
		public Assembler()
		{
			// Reset all internal data structures
			ResetAll();
		}

		#endregion

		#region Display LST File Methods

		/// <summary>
		/// Help function for the GetLSTFile() method
		/// </summary>
		/// <param name="num">Number to return its hex eqvivalent</param>
		/// <returns>String representing the number</returns>
		private string WideHex(int num)
		{
			string res = string.Format("{0:X}", num);
			return (res.Length == 1) ? "0" + res : res;
		}

		/// <summary>
		/// Generate and returns LST file
		/// </summary>
		/// <returns>LST File</returns>
		public string GetLSTFile()
		{
			string sLST = "";
			int iCount;
			int iMaxLine = _linesLocations.MaxLine;
			ProgramBlock.BlockContainer SubBlock = _programCode.GetFirst();
			int BlockPosition = (SubBlock == null) ? 0 : SubBlock.iAddress;
			int BlockEnd = (SubBlock == null) ? _programCode.Size : SubBlock.theBlock.Size + BlockPosition ;


			sLST = "*********************************************************************************\n" +
				"* VAX11 Simulator                                          Version " + Settings.Environment.SIM_VERSION + "  - 2003 *\n" +
				"*                                                                               *\n" +
				"* Nir Adar & Rotem Grosman            The Technion - Electrical Eng. Department *\n" +
				"*********************************************************************************\n\n\n";

			IDictionaryEnumerator it = _symbolsTable.GetEnumerator();
			it.Reset();
			bool bHaveMoreLables = it.MoveNext();
			if (bHaveMoreLables)
			{
				sLST += "Symbols Table:\n";
				// print the symbols table
				sLST += string.Format("{0,-9}{1, -25}{2, -16}{3,-16}{4}\n", "Line", "Symbol Name", "Value (Dec)", "Value (Hex)", "Type");
				while (bHaveMoreLables)
				{
					SymbolsTableEntry cur = ((SymbolsTableEntry)(it.Value));
					sLST += string.Format("{0,-9}{1, -25}{2, -16}{2, -16:X}{3}\n", cur.Line, cur.Name, cur.Value, cur.Type);
					bHaveMoreLables = it.MoveNext();
				}
			}
			// print machinecode and code
			sLST += "\n\n" + string.Format("{0, -9}", "Adr")
				+ "Machine Code" + new string(' ', (Settings.Assembler.BytesInLine + 1) * 2 + 10 - "Machine Code".Length + 2)
				+ "Source Code\n";
			for (int iCurLine = 0; iCurLine < iMaxLine; ++iCurLine)
			{
				// get the next block if needed
				if (BlockEnd <= BlockPosition)
				{
					SubBlock = _programCode.GetNext();
					BlockPosition = (SubBlock == null) ? _programCode.Size : SubBlock.iAddress ;
					BlockEnd = (SubBlock == null) ? _programCode.Size : SubBlock.theBlock.Size + BlockPosition ;
				}

				
				string sLine;
				iCount = 0;
				// check if current line contains errors
				int iErrorIndex = _comments.ErrorEntryIndex(iCurLine + 1);
				if (SubBlock == null)
				{
					sLine = "";
					if (iErrorIndex != -1) sLine += "Error (in line " +((int)(iCurLine + 1)).ToString() + "): " + 
											   CompilerMessagesStrings.GetMessageText(((CompilerCommentEntry)_comments.comments[iErrorIndex]).msgID) + "\n";
					sLine += new string (' ', 34);
				}
				else if (iErrorIndex != -1)
				{// have error
					sLST += "Error (in line " +((int)(iCurLine + 1)).ToString() + "): " + 
						CompilerMessagesStrings.GetMessageText(((CompilerCommentEntry)_comments.comments[iErrorIndex]).msgID) + "\n";
					if (_linesLocations.GetEndAddress(iCurLine + 1) == -1)
						sLine = new string (' ', 34);
					else
					{
						sLine = string.Format("{0,-9}", WideHex(BlockPosition)+ ":");
						for (iCount=1; 
							((iCurLine<(iMaxLine - 1) && BlockPosition < _linesLocations.GetEndAddress(iCurLine + 1)) ||
							(iCurLine==(iMaxLine - 1) && BlockPosition < BlockEnd)) && (iCount++ <= Settings.Assembler.BytesInLine) 
							; ++BlockPosition)
						{
							// code bytes
							sLine += WideHex(_programCode[BlockPosition]) + " ";
							
						}
						// print space between the machinecode and the line numbers
						int iOffset = (5 + (Settings.Assembler.BytesInLine + 1) * 2 + 10) - sLine.Length + 1;
						sLine += new string (' ', iOffset);
					}
				}
				else
				{   //don't have error this line
					sLine = string.Format("{0,-9}", WideHex(BlockPosition)+ ":");
					// print machine code
					if ((iCurLine < iMaxLine - 1) && (_linesLocations.GetStartingAddress(iCurLine+2)) == -1)
					{ //have error next line
						int iNextLineStartPos = BlockEnd;
						for (int iCheckLine = iCurLine+3; 
							(iCheckLine < iMaxLine ) && (iCheckLine < SubBlock.iLastLineInBlock); 
							++iCheckLine)
							if (_linesLocations.GetStartingAddress(iCheckLine) != -1)
							{
								iNextLineStartPos = _linesLocations.GetStartingAddress(iCheckLine);
								break;
							}
						iNextLineStartPos = System.Math.Min (iNextLineStartPos, _linesLocations.GetEndAddress(iCurLine+1));
						if (BlockEnd <= BlockPosition)
						{
							SubBlock = _programCode.GetNext();
							BlockPosition = (SubBlock == null) ? _programCode.Size : SubBlock.iAddress ;
							BlockEnd = (SubBlock == null) ? _programCode.Size : SubBlock.theBlock.Size + BlockPosition ;
						}

						for (iCount=1; 
							((iCurLine<(iMaxLine - 1) && BlockPosition < iNextLineStartPos) ||
							(iCurLine==(iMaxLine - 1) && BlockPosition < BlockEnd)) && (iCount++ <= Settings.Assembler.BytesInLine) 
							; ++BlockPosition)
						{
							// code bytes
							sLine += WideHex(_programCode[BlockPosition]) + " ";
						}
					}
					else //don't have error
						for (iCount=1; 
							((iCurLine<(iMaxLine - 1) && BlockPosition < _linesLocations.GetStartingAddress(iCurLine+2)) ||
							(iCurLine==(iMaxLine - 1) && BlockPosition < BlockEnd)) && (iCount++ <= Settings.Assembler.BytesInLine) 
							; ++BlockPosition)
						{
							// code bytes
							sLine += WideHex(_programCode[BlockPosition]) + " ";
							
						}									
					// print space between the machinecode and the line numbers
					int iOffset = (5 + (Settings.Assembler.BytesInLine + 1) * 2 + 10) - sLine.Length + 1;
					sLine += new string (' ', iOffset);
				}
				// tab here
				sLine += string.Format("{0,-5}",(iCurLine+1).ToString()); // line number
				sLine += _sOriginalCode[iCurLine]; // original code
				sLST += sLine + "\n";

				// if code bytes more then one line split lines
				while (iCount > Settings.Assembler.BytesInLine)
				{
					bool bAddChar = false;
					sLine = new string (' ', 9);
					for (iCount=1; 
						((iCurLine< (iMaxLine - 1) && BlockPosition < _linesLocations.GetStartingAddress(iCurLine+2)) ||
						(iCurLine == (iMaxLine - 1) && BlockPosition < BlockEnd)) && (iCount++ <= Settings.Assembler.BytesInLine)
						; ++BlockPosition)
					{	// code bytes
						sLine += WideHex(_programCode[BlockPosition]) + " ";
						bAddChar = true;
					}
					if (bAddChar)
						sLST += sLine + "\n";
				}
			}
			return sLST;
		}

		#endregion

		#region Main Compiler Methods

		/// <summary>
		/// Gets assembly code and compiles it. Saves the machine code in internal
		/// variable. Creates Symbols list. Creates list of all compile errors
		/// </summary>
		/// <param name="sAssemblyCode"></param>
		public ProgramBlock CompileCode(string sAssemblyCode)
		{
			bool bFoundErrors = false;

			// Reset all
			ResetAll();

			// Output will be on this object
			_programCode = new ProgramBlock();

			// Save original code, splited to lines
			_sOriginalCode = sAssemblyCode.Split(new char[] {'\n'});
			ProgramBlock tempBlock = new ProgramBlock();
			string[] sCleanCode = PreCompiler(sAssemblyCode);
			try { tempBlock = DoPass1(sCleanCode); } 
			catch (CompileError) { bFoundErrors = true; }
			try { DoPass2(ref tempBlock, bFoundErrors); } 
			catch (CompileError) { bFoundErrors = true; }

			_state = CompilerStates.IDLE;
			if (tempBlock.Size > 0 ) _programCode = tempBlock;

			if (bFoundErrors) throw new CompileError(CompilerMessage.COMPILE_FAILED);


			

			return _programCode;
		}


		/// <summary>
		/// Cleans the code, remove comments and spacing.
		/// Doesn't change lines numbering
		/// </summary>
		/// <param name="sAssemblyCode">Assembly code to clean</param>
		/// <returns>Clean code, each line of the original code 
		/// has seperate entry in the array</returns>
		private string[] PreCompiler(string sAssemblyCode)
		{

			// Die comments, DIE
			sAssemblyCode = Regex.Replace(sAssemblyCode, @"([^""#\n]*?""[^""\n]*("""")*[^""\n]*""[^""#\n]*)+(#([^\n])*)", @"$1", RegexOptions.Multiline);
			sAssemblyCode = Regex.Replace(sAssemblyCode,@"(^[^""#\n]*?)(#.*)", "$1", RegexOptions.Multiline);

			// Cleans whitespaces from the end of the code
			sAssemblyCode =	sAssemblyCode.TrimEnd(null);

			// TrimStart, TrimEnd
			sAssemblyCode = Regex.Replace(sAssemblyCode,@"^[ \t\r]+", "", RegexOptions.Multiline);
			sAssemblyCode = Regex.Replace(sAssemblyCode,@"[ \t\r]+$", "", RegexOptions.Multiline);

			// Support for ' and then space
			sAssemblyCode = Regex.Replace(sAssemblyCode,@"(^[^""]*?)' ", "$1$32", RegexOptions.Multiline);
			sAssemblyCode = Regex.Replace(sAssemblyCode,@"(^[^""]*?)\$\$32", "$1$32", RegexOptions.Multiline);


			// Cleans multiply spaces between words
			sAssemblyCode = Regex.Replace(sAssemblyCode, @"([^""[ ]+\n]*?""[^""\n]*("""")*[^""\n]*""[^""[ ]+\n]*)+([ ]+([^\n])*)", @"$1", RegexOptions.Multiline);


			// for debug: System.Windows.Forms.MessageBox.Show(sAssemblyCode);

			return sAssemblyCode.Split(new char[] {'\n'});
		}


		/// <summary>
		/// Pass 1 on the code. Gets code and returns machine code
		/// </summary>
		/// <param name="sAssemblyCode">clean code to analyze</param>
		/// <returns>Machine code</returns>
		/// <remarks>When we wants to add an error, we use AddCompileError(),
		/// not adding directly to _comments, in order to invoke an event
		/// right after adding the error to the comments list</remarks>
		private ProgramBlock DoPass1(string[] sAssemblyCode)
		{
			_state						= CompilerStates.PASS1;
			int iLC = 0, iLineNumber	= 0;
			ProgramBlock FinalCode		= new ProgramBlock();
			bool bFoundErrors			= false;
            
			// Lets find the first line that not empty
			while(iLineNumber < sAssemblyCode.Length && sAssemblyCode[iLineNumber] == "") 
			{
				// + 1 here and in the next of the function, cuz we counting from 0
				_linesLocations.AddEntry(iLineNumber + 1, iLC);
				++iLineNumber;
			}

			// Empty File?
			if (iLineNumber == sAssemblyCode.Length)
			{
				AddCompileError(1, CompilerMessage.PROGRAM_MUST_BEGIN_WITH_TEXT);

				// Don't continue to analyze the rest of the code:
				throw new CompileError(CompilerMessage.PROGRAM_MUST_BEGIN_WITH_TEXT);
			};

			// First line of code must be ".TEXT"
			if (sAssemblyCode[iLineNumber].ToUpper() != ".TEXT")
			{
				_linesLocations.AddEntry(iLineNumber + 1, iLC);
				AddCompileError(iLineNumber + 1, CompilerMessage.PROGRAM_MUST_BEGIN_WITH_TEXT);

				// Don't continue to analyze the rest of the code:
				throw new CompileError(CompilerMessage.PROGRAM_MUST_BEGIN_WITH_TEXT);
			}

			for (; iLineNumber < sAssemblyCode.Length; ++iLineNumber)
			{
				// Save the current line address
				_linesLocations.AddEntry(iLineNumber + 1, iLC);

				// If the line is empty, skip to the next one
				if (sAssemblyCode[iLineNumber] == "") continue;

				// If we got more than one label in the same line
				if (Regex.IsMatch(sAssemblyCode[iLineNumber], @"^[a-zA-Z_](\w)*:(\s)*[a-zA-Z_](\w)*:"))
				{
					bFoundErrors = true;
					AddCompileError(iLineNumber + 1, CompilerMessage.DOUBLE_LABEL_IN_LINE);
					// Go to analyze next line, don't continue with the current one
					continue;
				}

				int iTemp;
				// a label?
				if (Regex.IsMatch(sAssemblyCode[iLineNumber], @"^[a-zA-Z_](\w)*:"))
				{
					try
					{
						_symbolsTable.AddEntry(
							new SymbolsTableEntry(ReadNextWord(sAssemblyCode[iLineNumber], 
							out iTemp), iLC, SymbolsTable.SymbolType.LABEL, iLineNumber + 1));
						
						// Remove the label from the line to continue analayze it
						int iTemp2 = 0;
						ReadNextWord(sAssemblyCode[iLineNumber], out iTemp);
						CutAndUpdate(ref sAssemblyCode[iLineNumber], iTemp, ref iTemp2);
						ReadNextChar(sAssemblyCode[iLineNumber], out iTemp);
						CutAndUpdate(ref sAssemblyCode[iLineNumber], iTemp, ref iTemp2);

						// Line contains only label? If so, continue to the next line
						if (sAssemblyCode[iLineNumber] == "") continue;
					}
						// Label already defined
					catch(CompileError e)
					{
						bFoundErrors = true;
						if (e.ErrorCode == CompilerMessage.LABEL_ALREADY_DEFINED)
							AddCompileError(iLineNumber + 1, 
								CompilerMessage.LABEL_ALREADY_DEFINED);
						continue;
					}
				}
				
				// Directive
				if (sAssemblyCode[iLineNumber][0] == '.')
				{
					try
					{
						CodeBlock tempBlock = 
							AnalyzeDirective(sAssemblyCode[iLineNumber].Substring(1), out iTemp, iLineNumber + 1, iLC);
						FinalCode += tempBlock;
						iLC += tempBlock.Size;
						_linesLocations.SetEndAddress(iLineNumber + 1, iLC);
						// + 1 because we send Substring(1)
						if (iTemp + 1 < sAssemblyCode[iLineNumber].TrimEnd().Length)
						{
							bFoundErrors = true;
							AddCompileError(iLineNumber + 1, CompilerMessage.END_OF_LINE_OR_COMMENT_EXPECTED);
						}
						continue;
					}
					catch (CompileError e)
					{
						// Ugly as hell, but the fastest way to add .ORG support
						if (e.ErrorCode == CompilerMessage.NEW_ORG)
						{
							iLC = e.ExtraInformation;
							FinalCode.LastLine = iLineNumber + 1;
							FinalCode.AddBlock(new CodeBlock(), iLC, -1);
						}
						else
						{
							bFoundErrors = true;
							AddCompileError(iLineNumber + 1, e.ErrorCode);
						}
						continue;
					}
				}

					// We got a command
				else
				{
					try
					{
						CodeBlock tempBlock = AnalyzeCommand(sAssemblyCode[iLineNumber], iLC);
						FinalCode += tempBlock;
						iLC += tempBlock.Size;
						_linesLocations.SetEndAddress(iLineNumber + 1, iLC);
					}
					catch (CompileError e)
					{
						bFoundErrors = true;
						AddCompileError(iLineNumber + 1, e.ErrorCode);
						continue;
					}
				}
			}
			
			
			if (bFoundErrors) 
			{
				_programCode = FinalCode; // whatever
				throw new CompileError(CompilerMessage.COMPILE_FAILED);
			}

			return FinalCode;
		}


		/// <summary>
		/// Pass 2 on the code
		/// </summary>
		/// <param name="givenBlock">Code after pass1</param>
		/// <param name="bFoundErrorsOnFirstPass">flag tell if there were bugs on the
		/// first pass. if yes, no symbols will be written into the block, and we will only collect more errors</param>
		private void DoPass2(ref ProgramBlock givenBlock, bool bFoundErrorsOnFirstPass)
		{
			_state					= CompilerStates.PASS2;
			ListDictionary lst = _pass2List.GetPass2List();
			int iPos;
			bool bFoundErrors = false;


			IDictionaryEnumerator it = lst.GetEnumerator();
			it.Reset(); 
			while (it.MoveNext())
			{
				int iCur = (int)it.Key;
				CodeBlock theBlock;
				try
				{
					if (((Pass2Entry)lst[iCur]).IsValue == true)
					{
						_symbolsTable.AddEntry(new SymbolsTableEntry(
							((Pass2Entry)lst[iCur]).ConstantName, 
							CalcExpression(((Pass2Entry)lst[iCur]).Expression, out iPos, ((Pass2Entry)lst[iCur]).Size),
							SymbolsTable.SymbolType.CONSTANT, ((Pass2Entry)lst[iCur]).Where
							));

						continue;
					}
					else if (((Pass2Entry)lst[iCur]).IsOperand == true)
						theBlock = FetchOperand(((Pass2Entry)lst[iCur]).Expression, ((Pass2Entry)lst[iCur]).OpType,
							((Pass2Entry)lst[iCur]).Size, ((Pass2Entry)lst[iCur]).Where, false);
						// TODO: handle directives to send the type:
					else theBlock = new CodeBlock(CalcExpression(((Pass2Entry)lst[iCur]).Expression, out iPos, ((Pass2Entry)lst[iCur]).Size)
							 ,((Pass2Entry)lst[iCur]).Size);

					int iWhere = ((Pass2Entry)lst[iCur]).Where;

					if (!bFoundErrorsOnFirstPass) for (int iCounter2 = 0; iCounter2 < theBlock.Size; ++iCounter2)
													  givenBlock[iWhere+iCounter2] = theBlock[iCounter2];		

				}
				catch (CompileError e)
				{
					// Pay attention - can't assume CompilerMessage.UNDEFINED_SYMBOL is
					// the only error that can be occur here

					bFoundErrors = true;
					AddCompileError(_linesLocations.GetLineFromAddress(((Pass2Entry)lst[iCur]).Where),
						e.ErrorCode);
					continue;
				}
			}

			try
			{
				SetProgramEntryPoint(givenBlock);
			}
			catch
			{
				bFoundErrors = true;
				AddCompileError(1, CompilerMessage.INVALID_ENTRY_POINT);
			}

			if (bFoundErrors) 
			{
				//_programCode = 
				throw new CompileError(CompilerMessage.COMPILE_FAILED);
			}
		}

		#endregion

		#region Analyzing Text Methods

		/// <summary>
		/// Gets Dec number / Hex number / Label. Returns Dec number represent the value of the number / label
		/// </summary>
		/// <param name="sLabel">String to analyze</param>
		/// <param name="iExpressionLength">Expression length, in case we will have to throw exception.
		/// Thats the number of bytes that need to be save for pass2.
		/// </param>
		/// <returns>The value of the given string</returns>
		private long SolveLabel(string sLabel, int iExpressionLength)
		{
			try
			{
				// Hex?
				if (sLabel.ToUpper().StartsWith("0X"))
					return long.Parse(sLabel.Substring(2),
						System.Globalization.NumberStyles.AllowHexSpecifier);
				else if (sLabel.ToUpper().StartsWith("-0X"))
					return long.Parse("-" + sLabel.Substring(3),
						System.Globalization.NumberStyles.AllowHexSpecifier);

					// Char?
				else if (sLabel.StartsWith("'"))
				{
					if (sLabel.Length > 2) 
						throw new CompileError(CompilerMessage.END_OF_OPERAND_EXPECTED);
					else if (sLabel.Length == 1) throw new CompileError(CompilerMessage.VALUE_EXPECTED);
					return (long)sLabel[1];
				}
					// Dec
				else
					return long.Parse(sLabel);
			}
			catch (FormatException) // We got label, not a number
			{
				try
				{
					return (_symbolsTable.SymbolValue(sLabel)).Value;
				}
				catch // CompilerMessage.UNDEFINED_SYMBOL
				{
					// Assuming: it is the caller resposibility to
					// add the entry to Pass2List
					throw new CompileError(CompilerMessage.UNDEFINED_SYMBOL, iExpressionLength);
				}
			}
			catch (OverflowException) // Number is damn too big
			{
				throw new CompileError(CompilerMessage.NUMBER_IS_TOO_BIG);
			}
		}

		/// <summary>
		/// Gets string with expression contains labels, numbers and operators
		/// and solves it. If the string doesn't contains ecuation, it throws exception.
		/// If one of the labels isn't defined, it throw exception, and it
		/// is the caller responsability to add the expression to Pass2List.
		/// If the user tries to divide by zero, it throws exception.
		/// If one of the numbers in the equation is too big, it throws an exception.
		/// If the result of the calculation is too big, we don't care and give inncorrect result :-)
		/// </summary>
		/// <param name="sExpression">The expression to solve</param>
		/// <param name="iExpressionLength">Returns how many chars from the given string were part of the equation</param>
		/// <returns>The calculation's result</returns>
		private long CalcExpression(string sExpression, out int iExpressionLength, int iSizeForPass2)
		{
			// Do we have legal expression?
			//Match m = Regex.Match(sExpression, @"^(('(\w)+)|((\-)?(\w)+))([\+\-/\*]{1}(('(\w)+)|((\w)+)))*"); /* working except with chars */
			Match m = Regex.Match(sExpression, @"^(('.)|((\-)?(\w)+))([\+\-/\*]{1}(('.)|((\w)+)))*");
			iExpressionLength = m.Value.Length; // save the location before throwing exception
			if (m.Value == "") throw new CompileError(CompilerMessage.VALUE_EXPECTED);
		

			// Analyze expression
			string[] sLabels = Regex.Split(m.Value, @"[\+\-/\*]");

			// Useable indexes: sOperators[1] ... sOperators[sOperators.Length-2]
			string[] sOperators = Regex.Split(m.Value, @"[^\+\-/\*]+");

			// lets get values out of the labels
			ArrayList a_iValues = new ArrayList();

			for (int iIndex = (sLabels[0] == "") ? 1 : 0; iIndex < sLabels.Length; ++iIndex)
				a_iValues.Add(SolveLabel(sLabels[iIndex], iSizeForPass2));

			// Support for (-) as first character:
			if (m.Value.StartsWith("-"))
			{
				a_iValues[0] = -((long)a_iValues[0]);

				// Remove the first - from the sOperators array
				string[] sOperatorsTemp = new string[sOperators.Length-1];
				for (int iCounter = 0; iCounter < sOperatorsTemp.Length; ++iCounter)
					sOperatorsTemp[iCounter] = sOperators[iCounter + 1];
				sOperators = sOperatorsTemp;
			}

			// Preparing...
			ArrayList a_iTemp = new ArrayList();
			ArrayList a_opTemp = new ArrayList();

			long iTemp = (long)a_iValues[0];

			// Calculate *,/ operations
			for (int iIndex = 1; iIndex < sOperators.Length - 1; ++iIndex)
			{
				if (sOperators[iIndex] == "*") iTemp *= (long)a_iValues[iIndex];
				else if (sOperators[iIndex] == "/")
				{
					// User tries to divide labels by 0?
					if ((long)a_iValues[iIndex] == 0)
						throw new CompileError(CompilerMessage.DIVIDE_BY_ZERO);

					iTemp /= (long)a_iValues[iIndex];
				}
				else
				{
					a_iTemp.Add(iTemp);
					a_opTemp.Add(sOperators[iIndex]);
					iTemp = (long)a_iValues[iIndex];
				}
			}
			a_iTemp.Add(iTemp);

			// Calculate +,- operations
			long iRes = (long)a_iTemp[0];
			for (int iIndex = 0; iIndex < a_opTemp.Count; ++iIndex)
			{
				if (a_opTemp[iIndex].ToString() == "+")
					iRes += (long)a_iTemp[iIndex+1];
				else if (a_opTemp[iIndex].ToString() == "-")
					iRes -= (long)a_iTemp[iIndex+1];
			}

			return iRes;
		}

		/// <summary>
		/// Gets a string. Reading a number from its begining.
		/// The number can be decimal or hex. The function returns that number
		/// and also returns the position on the text where the number ended.
		/// In case the expression is not a number, it throws an error.
		/// Legal number is ending with new line or space or comma.
		/// </summary>
		/// <param name="sExpression">Expression to read number from</param>
		/// <param name="iExpressionLength">Will return here how many chars thenumber is</param>
		/// <returns>Returns the number</returns>
		private int ReadNextNumber(string sExpression, out int iExpressionLength)
		{
			Match m = Regex.Match(sExpression, @"^(\s)*[0-9a-fA-Fx]+(\s)*");
			iExpressionLength = m.Value.Length;
			if (m.Value == "") throw new CompileError(CompilerMessage.NUMBER_EXPECTED);

			// Check if it is legal expression
			if (iExpressionLength < sExpression.Length && 
				Regex.Match(sExpression.Substring(iExpressionLength),
				@"[(\s)*\[\]\(\)\.,\*/\-\+]").Value == "") 
				throw new CompileError(CompilerMessage.NUMBER_EXPECTED);

			// Hex?
			if (m.Value.StartsWith("0x"))
				return int.Parse(m.Value.Substring(2), 
					System.Globalization.NumberStyles.AllowHexSpecifier);

				// If no, it is dec or error
			else
			{
				try
				{
					return int.Parse(m.Value.Trim());
				}
				catch (OverflowException)
				{
					throw new CompileError(CompilerMessage.NUMBER_IS_TOO_BIG);
				}
				catch (FormatException)
				{
					throw new CompileError(CompilerMessage.NUMBER_EXPECTED);
				}
			}
		}


		/// <summary>
		/// Read a single word from the given string. Returns the word,
		/// and also returns how many chars the word contains.
		/// In this case it was not necessary to return the chars number,
		/// but we do so in order to make all the function to look the same.
		/// </summary>
		/// <param name="sExpression">The string</param>
		/// <param name="iExpressionLength">Word Length</param>
		/// <returns>The word</returns>
		private string ReadNextWord(string sExpression, out int iExpressionLength)
		{
			Match m = Regex.Match(sExpression, @"^(\s)*[a-zA-Z_](\w)*(\s)*");
			iExpressionLength = m.Value.Length;
			if (m.Value == "") throw new CompileError(CompilerMessage.ILLEGAL_EXPRESSION);
			return m.Value.Trim();
		}

		/// <summary>
		/// Read next char, skip white spaces, returns the char and also
		/// returns the index of the next char after the one it returns
		/// </summary>
		/// <param name="sExpression">String to read from</param>
		/// <param name="iExpressionLength">The index of the next char after the one it returns</param>
		/// <returns>The next char</returns>
		private string ReadNextChar(string sExpression, out int iExpressionLength)
		{
			Match m = Regex.Match(sExpression, @"^(\s)*.(\s)*");
			iExpressionLength = m.Value.Length;
			if (m.Value != "") return m.Value.Trim();
			return "";
		}

		/// <summary>
		/// Replace strings as \n \r etc with their meaning. The text passed
		/// to this function should be surround by inverted commas.
		/// The function will remove the inverted commas
		/// </summary>
		/// <param name="sExpression">The expression to analyze</param>
		/// <param name="iExpressionLength">The index of the next char after the second inverted comma</param>
		/// <returns>The CodeBlock generated from the string</returns>
		private CodeBlock AnalyzeString(string sExpression, out int iExpressionLength)
		{
			// Find text between inverted commas
			Match m = Regex.Match(sExpression, @"^(\s)*""[^""]*(""""[^""]*)*[^""]*""(\s)*");
			iExpressionLength = m.Value.Length; 
			if (m.Value == "") throw new CompileError(CompilerMessage.INVERTED_COMMAS_EXPECTED);

			sExpression = sExpression.Trim();
			if (sExpression.Length != m.Value.Length) throw new CompileError(CompilerMessage.ILLEGAL_STRING_FORMAT_STRING);

			sExpression = sExpression.Substring(sExpression.IndexOf('"') + 1, 
				sExpression.LastIndexOf('"') - sExpression.IndexOf('"') - 1);

			CodeBlock cResult = new CodeBlock();
			for (int iCounter = 0; iCounter < sExpression.Length; ++ iCounter)
			{
				if (sExpression[iCounter] == '"') cResult += (CodeBlock)sExpression[iCounter++];
				else if ((sExpression[iCounter] == '\\') && (iCounter + 1 < sExpression.Length))
				{
					switch(sExpression[++iCounter])
					{
						case 'n':
							cResult += 0x0a;
							break;
						case 'r':
							cResult += 0x0d;
							break;
						case 't':
							cResult += 0x09;
							break;
						case 'b':
							cResult += 0x08;
							break;
						case '\\':
							cResult += 0x5c;
							break;
						case '%':
							cResult += 0x25;
							break;
						case '"':				//TODO: check if work correct
							cResult += 0x22;
							break;
						case '0':
							cResult += 0x00;
							break;
						default:
							throw new CompileError(CompilerMessage.ILLEGAL_STRING_FORMAT);
					}
				}
				else cResult += (CodeBlock)sExpression[iCounter];
			}

			return cResult;
		}

		#endregion

		#region Analyzing VAX11 Stuff Methods


		/// <summary>
		/// Analyze directive
		/// Assumings: 1. we calling to this function only if sExpression is
		/// a directive (the caller responsible to send only strings starting with
		/// dot.
		/// 2. The caller is responsible to see if there is anything after the
		/// directive. We don't check if there is garbage after legal directive
		/// </summary>
		/// <param name="sExpression">Directive to exaime</param>
		/// <param name="iExpressionLength">index to place after the directive ends</param>
		/// <param name="iCurLineNumber">The current line number</param>
		/// <param name="iLC">Current LC</param>
		/// <returns>The CodeBlock that the directive represents</returns>
		private CodeBlock AnalyzeDirective(string sExpression, out int iExpressionLength
			, int iCurLineNumber, int iLC)
		{

			int iOriginalSize = sExpression.Length;
			string WordToAnalyze;
			int iBlockSize, iPos;

			CodeBlock TempBlock = new CodeBlock();

			// Read first word
			WordToAnalyze = ReadNextWord(sExpression, out iExpressionLength);
			sExpression = sExpression.Substring(iExpressionLength);
			WordToAnalyze = WordToAnalyze.ToUpper();

			// Analyze word, perform its task
			if (WordToAnalyze == "DATA" || WordToAnalyze == "TEXT") return TempBlock;
			else if (WordToAnalyze == "SPACE")
			{
				try
				{
					Match m = Regex.Match(sExpression, @"^(('(\w)+)|((\-)?(\w)+))([\+\-/\*]{1}(('(\w)+)|((\w)+)))*");
					if (m.Value == "") throw new CompileError(CompilerMessage.VALUE_EXPECTED);
					if (m.Value != sExpression.TrimEnd()) throw new CompileError(CompilerMessage.END_OF_LINE_OR_COMMENT_EXPECTED);
					int tempOut;
					int iNum = (int)CalcExpression(m.Value, out tempOut, 0);
					CutAndUpdate(ref sExpression, m.Value.Length, ref iExpressionLength);
					if (iNum < 0) throw new CompileError(CompilerMessage.SPACE_CANT_BE_NEGATIVE);
					return new CodeBlock(0L, iNum);
				}
				catch (CompileError e)
				{
					if (e.ErrorCode == CompilerMessage.UNDEFINED_SYMBOL)
						throw new CompileError(CompilerMessage.DEFINED_SYMBOL_OR_NUMBER_EXPECTED);
					throw;
				}
			}
			else if (WordToAnalyze == "SET")
			{
				string TempName = ReadNextWord(sExpression, out iPos);
				CutAndUpdate(ref sExpression, iPos, ref iExpressionLength);
				
				if (ReadNextChar(sExpression, out iPos) != ",")
				{
					CutAndUpdate(ref sExpression, iPos, ref iExpressionLength);
					throw new CompileError(CompilerMessage.COMMA_EXPECTED);
				}
				CutAndUpdate(ref sExpression, iPos, ref iExpressionLength);

		
				try
				{
					_symbolsTable.AddEntry(new SymbolsTableEntry(TempName,
						CalcExpression(sExpression, out iPos, 0), // TODO: hope its ok
						SymbolsTable.SymbolType.CONSTANT, iCurLineNumber
						));
				}
				catch (CompileError e)
				{
					if ((e.ErrorCode == CompilerMessage.UNDEFINED_SYMBOL) && (_state == CompilerStates.PASS1))
						_pass2List.AddEntry(new Pass2Entry(TempName, sExpression,iCurLineNumber));
					else throw;
				}

				CutAndUpdate(ref sExpression, iPos, ref iExpressionLength);
				return TempBlock;
			}

			else if (WordToAnalyze == "ENTRYPOINT")
			{
				string TempName;
				try
				{
					TempName = ReadNextWord(sExpression, out iPos);
					CutAndUpdate(ref sExpression, iPos, ref iExpressionLength);
				}
				catch (CompileError)
				{
					try
					{
						// Ok, it is not a word. Maybe it is a number?
						TempName = ReadNextNumber(sExpression, out iPos).ToString();
						CutAndUpdate(ref sExpression, iPos, ref iExpressionLength);
					}
					catch (CompileError)
					{
						throw new CompileError(CompilerMessage.ILLEGAL_EXPRESSION);
					}
				}


				if (_LocateEntryPoint != "") throw new CompileError(CompilerMessage.CANT_DEFINE_TWO_ENTRY_POINTS);
				_LocateEntryPoint = TempName;

				return TempBlock;

			}
			else if (WordToAnalyze == "ORG")
			{
				try
				{
					int iJumpLocation = ReadNextNumber(sExpression, out iPos);
					CutAndUpdate(ref sExpression, iPos, ref iExpressionLength);
					if (iJumpLocation < iLC)
						throw new CompileError(CompilerMessage.ORG_CANT_USED_TO_JUMP_BACKWARD);
					throw new CompileError(CompilerMessage.NEW_ORG, iJumpLocation);
				}
				catch (CompileError e)
				{
					if (e.ErrorCode == CompilerMessage.NUMBER_EXPECTED)
					{
						// Not always true, but who cares
						if (sExpression[0] == '-')
							throw new CompileError(CompilerMessage.CANT_USE_NEGATIVE_NUMBERS_ON_ORG);
						else 
							throw new CompileError(CompilerMessage.ILLEGAL_EXPRESSION);
					}
					else throw;
				}
			}

			else if (WordToAnalyze == "ASCII")
			{
				TempBlock = AnalyzeString(sExpression, out iPos);
				CutAndUpdate(ref sExpression, iPos, ref iExpressionLength);
				iExpressionLength += 2; // for ""
				return TempBlock;
			}
			else if (WordToAnalyze == "ASCIZ")
			{
				TempBlock = AnalyzeString(sExpression, out iPos) + new CodeBlock(0L, 1);
				CutAndUpdate(ref sExpression, iPos, ref iExpressionLength);
				iExpressionLength += 2; // for ""
				return TempBlock;
			}
			else if (WordToAnalyze == "ASCIC")
			{
				TempBlock = AnalyzeString(sExpression, out iPos);
				if (TempBlock.Size > 255) throw new CompileError(CompilerMessage.ASCIC_STRING_TOO_LONG);
				CutAndUpdate(ref sExpression, iPos, ref iExpressionLength);
				iExpressionLength += 2; // for ""
				return new CodeBlock((long)TempBlock.Size, 1) + TempBlock;
			}
			else if (WordToAnalyze == "INCLUDE")
			{
				// Undocumented function - adds the file that appear after the word before the current one.
				// Doesn't check errors or anything. assumes there are only set commands on the file.
				// really gonna to not work on real cases.
			
				try
				{
					StreamReader inputFile = new StreamReader(sExpression, System.Text.Encoding.Default);
					CutAndUpdate(ref sExpression, sExpression.Length, ref iExpressionLength);
					string sFileContent = inputFile.ReadToEnd();
					inputFile.Close();
					Assembler temp = new Assembler();
					temp.CompileCode(sFileContent);
					IDictionaryEnumerator it = temp._symbolsTable.GetEnumerator();
					it.Reset();
					while (it.MoveNext()) _symbolsTable.AddEntry(((SymbolsTableEntry)(it.Value)));
					return temp.ProgramMachineCode.GetFirst().theBlock;
				}
				catch (Exception)
				{
					throw new PanicException();
				}
                
			}

			else if (WordToAnalyze == "BYTE") iBlockSize = 1;
			else if (WordToAnalyze == "WORD") iBlockSize = 2;
			else if (WordToAnalyze == "INT")  iBlockSize = 4;
			else if (WordToAnalyze == "LONG") iBlockSize = 4;
			else if (WordToAnalyze == "QUAD") iBlockSize = 8;
			else throw new CompileError(CompilerMessage.UNRECOGNIZED_DIRECTIVE);

			// Assuming we now at one of the byte/word/etc directives



			//string ch;
			int iCommaNumber = HowManyCharAppears(sExpression, ',');


			string[] values = sExpression.Split(new char[] {','});
			for (int iCounter = 0; iCounter < values.Length; ++iCounter)
			{
				long CurReadValue;
				try
				{
					CurReadValue = CalcExpression(values[iCounter].Trim(), out iPos, iBlockSize);

					// Some directives must contain sign value, other unsigned and other don't care
					if (WordToAnalyze == "WORD" && CurReadValue < 0) throw new CompileError(CompilerMessage.WORD_REQUIRE_POSITIVE_NUMBERS_USE_INT);
					else if (WordToAnalyze == "INT" && (CurReadValue < -32768 || CurReadValue > 32767)) throw new CompileError(CompilerMessage.VALUE_OUT_OF_INT_RANGE_USE_LONG);
					else if (WordToAnalyze == "BYTE" && CurReadValue < -128) throw new CompileError(CompilerMessage.VALUE_OUT_OF_BYTE_RANGE_USE_INT);
		
					TempBlock += new CodeBlock(CurReadValue, iBlockSize);
				}
				catch (CompileError e)
				{
					if (e.ErrorCode != CompilerMessage.UNDEFINED_SYMBOL) throw;
					// Add to pass2 list
					_pass2List.AddEntry(new Pass2Entry(iLC + iCounter * iBlockSize, iBlockSize, values[iCounter], true));
					// Not defined? fill will zeros and skip to next place
					TempBlock += new CodeBlock(0, iBlockSize);					
				}
			}
			iExpressionLength += sExpression.Length;

			
			return TempBlock;
		}

		// Assuming: the function gets legal operands, without whitespaces, comments, etc.
		// Therefore, no need to update len using out iExpression
		private CodeBlock FetchOperand(string sExpression,
			string sOpType, int iOpLen, int iLC, bool bIsPrivilegedCommand)
		{
			CodeBlock resBlock = new CodeBlock();
			int iPos;
			Match m;

			//
			// Privileged Register Operand
			//
			if (bIsPrivilegedCommand && sOpType == "p")
			{
				// Create output
				resBlock += RegistersSettings.GetPrivilegedRegister(ReadNextWord(sExpression, out iPos));
				return resBlock;
			}
			else
			{

				// Assuming: the function gets legal operands, without whitespaces, comments, etc:
				string sOperand = sExpression;

				//
				// literal or immediate operand (Addressing Modes 9/10)
				//
				if (sOperand.StartsWith("$") || sOperand.StartsWith("'"))
				{

					// Is it legal to use this addressing mode?
					if (sOpType == "a") throw new CompileError(CompilerMessage.IMMEDIATE_ADDRESSING_MODE_IS_NOT_ALLOWED_HERE);
					if (sOpType != "b" && sOpType != "r" ) 
						throw new CompileError(CompilerMessage.IMMEDIATE_ADDRESSING_MODE_IS_NOT_ALLOWED_HERE);

					long iNum = SolveLabel(sOperand.StartsWith("'") ? sOperand : sOperand.Substring(1), 1 + iOpLen);

					// If it is branch we save the offset from the current location
					if (sOpType == "b") iNum = iNum - iLC - iOpLen;

					// Create output

					// We ingore the iOpLen in this case:
					if (iNum >= 0 && iNum <= 63 && _state != CompilerStates.PASS2) 
						resBlock += (byte)iNum; // (Addressing Mode 9)
						// In the second case we don't ingore it:
						// Pay attention that in case it is pass2, the oplen already includes the 0x8F
						// (Addressing Mode 10)
					else resBlock += (byte)0x8F + new CodeBlock(iNum, (_state == CompilerStates.PASS2) ? iOpLen - 1 : iOpLen); 
					return resBlock;
				}

				

					//
					// Register addressing mode (Addressing Mode 1)
					//
				else if ((m = Regex.Match(sOperand.ToUpper(),@"^(([rR]((1[0-5]$)|([0-9]$)))|PC|SP|AP|FP)$")).Value != "")
				{
					// Is it legal to use this addressing mode?
					if (sOpType == "a" || sOpType == "b" ) 
						throw new CompileError(CompilerMessage.REGISTER_IS_NOT_ALLOWED_HERE);

					// Create output:
					resBlock += (CodeBlock)((byte)0x50 + RegistersSettings.GetRegister(m.Value));
					return resBlock;
				}


					//
					// Register deferred, auto increment, auto decrement, auto increment deferred
					// index for all the aboves
					// (Addressing modes: 2, 3, 4, 5, 8a, 8b, 8c, 8d)
					//

					// The following Regex has 2 exceptions that need manual handling:
					// It accept *(reg) and also -(reg)+ which are illegal
					//
					// Regular Expression: @"^[\-\*]?\((([rR]((1[0-5])|([0-9])))|PC|SP|AP|FP)\)(\+)?(\[)?$"
					// The following are the tests we ran. It behaves as expected in all the cases.
					//
					//		(r1)			(r14)			(r17)
					//		(r27)			+(r17)			-(r17)
					//		-(r12)			-(r12)+			-(r12)+[
					//		-(r12)+[v]		-(r12)+[r7]		-(r12)+[r12]
					//		-(r12)+[r17]	r12)+[r12]		*(r1)+
					//		*(r1)+[PC]		-(r7)[sp]		-(r7)[sp+]
					//		-(r7)+sp]

				else if ((m = Regex.Match(sOperand.ToUpper(),
					@"^[\-\*]?\((([rR]((1[0-5])|([0-9])))|PC|SP|AP|FP)\)(\+)?(\[(([rR]((1[0-5])|([0-9])))|PC|SP|AP|FP)\])?$")).Value != "")
				{
					// Check the regular expression's exceptions
					if (m.Value[0] == '*' && m.Value.IndexOf("+") == -1)
						throw new CompileError(CompilerMessage.ILLEGAL_ADDRESSING_MODE);
					if (m.Value[0] == '-' && m.Value.IndexOf("+") != -1)
						throw new CompileError(CompilerMessage.ILLEGAL_ADDRESSING_MODE);

					// Assumming: If we got here, then m.Value == sOperand
					bool bIsIndex			= sOperand.IndexOf("[") != -1;
					bool bIsIncrement		= sOperand.IndexOf("+") != -1;
					bool bIsDecrement		= sOperand.IndexOf("-") != -1;
					bool bIsDoubleDeferred	= sOperand.IndexOf("*") != -1;

					if (sOpType == "b") throw new CompileError(CompilerMessage.ILLEGAL_ADDRESSING_MODE);

					// Get first register
					byte firstRegister = RegistersSettings.GetRegister(sOperand.Substring(sOperand.IndexOf("(")+1, 
						sOperand.IndexOf(")") - sOperand.IndexOf("(") - 1));

					byte indexRegister;
					if (bIsIndex)
					{
						// Get index register
						indexRegister = RegistersSettings.GetRegister(sOperand.Substring(sOperand.IndexOf("[")+1, 
							sOperand.IndexOf("]") - sOperand.IndexOf("[") - 1));
						if (indexRegister == 15) throw new CompileError(CompilerMessage.PC_CANNOT_BE_USED_HERE);

						if (indexRegister == firstRegister) throw new CompileError(CompilerMessage.INDEX_CANT_BE_THE_SAME_AS_BASE_REG);

						// Do all indexes-modes
						// (Addressing modes: 8a, 8b, 8c, 8d)
						resBlock += (byte)((byte)0x40 + indexRegister);
					}

					// From now its easy :)

					// regisger deferred (Addressing Mode 2)
					if (!bIsIncrement && !bIsDecrement && !bIsDoubleDeferred)
						resBlock += (byte)((byte)0x60 + firstRegister);

						// auto increment (Addressing Mode 3)
					else if (bIsIncrement && !bIsDecrement && !bIsDoubleDeferred)
						resBlock += (byte)((byte)0x80 + firstRegister);

						// auto decrement (Addressing Mode 4)
					else if (!bIsIncrement && bIsDecrement && !bIsDoubleDeferred)
						resBlock += (byte)((byte)0x70 + firstRegister);

						// auto increment deferred (Addressing Mode 5)
					else if (bIsIncrement && !bIsDecrement && bIsDoubleDeferred)
						resBlock += (byte)((byte)0x90 + firstRegister);

						// The regular expression should prevent us from being here:
					else throw new PanicException();

					return resBlock;
				}

					//
					// (Addressing modes: 6, 7, 8e, 8f)
					//
				else if ((m = Regex.Match(sOperand,
					@"^(\*)?(\-)?(\w)+\((([rR]((1[0-5])|([0-9])))|PC|SP|AP|FP|pc|sp|ap|fp)\)(\[(([rR]((1[0-5])|([0-9])))|PC|SP|AP|FP|pc|sp|ap|fp)\])?$")).Value != "")
				{
					bool bIsDoubleDeferred	= sOperand.IndexOf("*") != -1;
					bool bIsIndex			= sOperand.IndexOf("[") != -1;

					if (bIsDoubleDeferred) sOperand = sOperand.Substring(1);
					long lOffset = SolveLabel((Regex.Match(sOperand, @"^(\-)?(\w)+")).Value, 4+1 + (bIsIndex ? 1 : 0));

					CodeBlock cOffset;
					if (lOffset < 0)
					{
						int iBlockSize = Math.Min((int)Math.Log(Math.Abs(lOffset) * 2, 2) / 8 + 1, 4);
						if (Settings.Assembler.bOptimaizeCode == false) iBlockSize = 4;
						cOffset = new CodeBlock(lOffset, iBlockSize);
					}
					else cOffset = lOffset;

					// Get first register
					byte firstRegister = RegistersSettings.GetRegister(sOperand.Substring(sOperand.IndexOf("(")+1, 
						sOperand.IndexOf(")") - sOperand.IndexOf("(") - 1));

					byte indexRegister;
					if (bIsIndex)
					{
						// Get index register
						indexRegister = RegistersSettings.GetRegister(sOperand.Substring(sOperand.IndexOf("[")+1, 
							sOperand.IndexOf("]") - sOperand.IndexOf("[") - 1));
						if (indexRegister == 15) throw new CompileError(CompilerMessage.PC_CANNOT_BE_USED_HERE);
						if (indexRegister == firstRegister) throw new CompileError(CompilerMessage.INDEX_CANT_BE_THE_SAME_AS_BASE_REG);

						// Addressing modes: 8e, 8f
						resBlock += (byte)((byte)0x40 + indexRegister);
					}

					// The following code appears again down in this function. Any change should be made
					// on both places.
					byte adrByte = firstRegister;
					if (this._state != CompilerStates.PASS2 && Settings.Assembler.bOptimaizeCode == true)
					{
						if (cOffset.Size == 1) adrByte += 0xA0;			// addressing mode 6a
						else if (cOffset.Size == 2) adrByte += 0xC0;	// addressing mode 6b
						else adrByte += 0xE0;							// addressing mode 6c
					}
					else
					{
						adrByte += 0xE0;								// addressing mode 6c
					}

					// addressing mode 7 (a, b, c)
					if (bIsDoubleDeferred) adrByte += 0x10;

					if (Settings.Assembler.bOptimaizeCode == true) resBlock += adrByte + cOffset;
					else resBlock += adrByte + new CodeBlock((long)cOffset, 4);
					
					return resBlock;
				}


					// Addressing mode for known functions calls
				else if ((m = Regex.Match(sOperand.ToUpper(), @"^\.[A-Za-z_]+[A-Za-z_0-9]*$", RegexOptions.Compiled)).Value != "")
				{
					resBlock += (byte)0x9F + new CodeBlock((long)KnownFunctions.GetAddress(sOperand.Substring(1)), 4);
					return resBlock;
				}

					// Extra addressing mode - comptaible with old simulator. Note
					// the same code appears also up in the function. change on both places.
				else if((m = Regex.Match(sOperand.ToUpper(),
					@"^(\w)+\[(([rR]((1[0-5])|([0-9])))|PC|SP|AP|FP|pc|sp|ap|fp)\]$", RegexOptions.Compiled)).Value != "")
				{

					// We allow expressions such 6[r0] to pass here by design. about negative offsets - fuck it
					// Get first register
					byte firstRegister = RegistersSettings.GetRegister(sOperand.Substring(sOperand.IndexOf("[")+1, 
						sOperand.IndexOf("]") - sOperand.IndexOf("[") - 1));

					if (firstRegister == 15) throw new CompileError(CompilerMessage.PC_CANNOT_BE_USED_HERE);

					resBlock += (byte)((byte)0x40 + firstRegister);
					resBlock += (byte)0xEF + new CodeBlock((SolveLabel(sOperand.Substring(0, sOperand.IndexOf("[")), 
						4 + 1 + 1) - iLC - (1 + 1 + 4)), 4);
					
					return resBlock;
				}

					// (Addressing modes: 11, 12, 13)
				else if ( 
					((m = Regex.Match(sOperand.ToUpper(),
					@"^(\*(\$)?)?[A-Z_]+[A-Z_0-9]*$", RegexOptions.Compiled)).Value != "")
					||
					((m = Regex.Match(sOperand.ToUpper(),
					@"(^(\*\$))((0X[A-F0-9]+$)|([0-9]+$))", RegexOptions.Compiled)).Value != "")
					)
				{
					bool bIsAsterisk = sOperand.IndexOf("*") != -1;
					bool bIsDollar = sOperand.IndexOf("$") != -1;

					
					// Absolute (Addressing Mode 11)
					if (bIsAsterisk && bIsDollar) 
						resBlock += (byte)0x9F + new CodeBlock(SolveLabel(sOperand.Substring(2), 4 + 1), 4);



					// Relative (Addressing Mode 12)
					if (!bIsAsterisk && !bIsDollar) 
					{
						if (sOpType == "b")
						{
							long iRelativeLocation = SolveLabel(sOperand, iOpLen) - iLC - iOpLen;
							if (Math.Pow(2, iOpLen * 7) <= iRelativeLocation || (-Math.Pow(2, iOpLen * 7) > iRelativeLocation))
								throw new CompileError(CompilerMessage.DISPLACEMENT_TOO_BIG);
							resBlock += new CodeBlock(iRelativeLocation, iOpLen);
						}
						else
							resBlock += (byte)0xEF + new CodeBlock((SolveLabel(sOperand, 
								4 + 1) - iLC - (1 + 4)), 4);
					}
						// Relative Deferred (Addressing Mode 13)
					else if (bIsAsterisk && !bIsDollar) 
					{
						long iRelativeLocation = SolveLabel(sOperand.Substring(1), 4+1) - iLC - iOpLen;
						resBlock += (byte)0xFF + new CodeBlock((SolveLabel(sOperand.Substring(1), 
							4 + 1) - iLC - (1 + 4)), 4);
					}

					return resBlock;
				}
					// Another shit - supports LABEL+Const, and LABEL+LABEL. duplicate relative addressing mode code
				else if ( 
					((m = Regex.Match(sOperand.ToUpper(),
					@"(^(0X[A-F0-9]+)|^([A-Z_0-9]+))[\+\-]((0X[A-F0-9]+$)|([A-Z_0-9]+$))", RegexOptions.Compiled)).Value != ""))
				{
					if (Regex.Match(sOperand.ToUpper(), @"^((0X[A-F0-9]+)|([0-9]+))[\+\-]((0X[A-F0-9]+$)|([0-9]+$))").Value != "") throw new CompileError(CompilerMessage.ILLEGAL_ADDRESSING_MODE);

					int temp;
					if (sOpType == "b")
					{
						long iRelativeLocation = CalcExpression(sOperand, out temp, iOpLen) - iLC - iOpLen;
						if (Math.Pow(2, iOpLen * 7) <= iRelativeLocation || (-Math.Pow(2, iOpLen * 7) > iRelativeLocation))
							throw new CompileError(CompilerMessage.DISPLACEMENT_TOO_BIG);
						resBlock += new CodeBlock(iRelativeLocation, iOpLen);
					}
					else
						resBlock += (byte)0xEF + new CodeBlock((CalcExpression(sOperand, 
							out temp, 4+1) - iLC - (1 + 4)), 4);
					return resBlock;
				}

				else if ( 
					((m = Regex.Match(sOperand.ToUpper(),
					@"^(\*)(\$)?(\w)+(\[(([rR]((1[0-5])|([0-9])))|PC|SP|AP|FP|pc|sp|ap|fp)\])$", RegexOptions.Compiled)).Value != ""))
				{
					bool bIsDollar = sOperand.IndexOf("$") != -1;

					int indexRegister = RegistersSettings.GetRegister(sOperand.Substring(sOperand.IndexOf("[")+1, 
						sOperand.IndexOf("]") - sOperand.IndexOf("[") - 1));

					if (indexRegister == 15) throw new CompileError(CompilerMessage.PC_CANNOT_BE_USED_HERE);

					resBlock += (byte)((byte)0x40 + indexRegister);
					resBlock +=  (bIsDollar) ? (byte)0x9F : (byte)0xFF;
					if (!bIsDollar)
						resBlock += new CodeBlock((SolveLabel(sOperand.Substring(1, sOperand.IndexOf("[") - 1), 
							4 + 1 + 1) - iLC - (1 + 1 + 4)), 4);
					else 
						resBlock += new CodeBlock((SolveLabel(sOperand.Substring(2, sOperand.IndexOf("[") - 2), 
							4 + 1 + 1)), 4);
					
					return resBlock;
				}
			}
			
			throw new CompileError(CompilerMessage.ILLEGAL_ADDRESSING_MODE);
		}

		/// <summary>
		/// The function gets a command with operands, 
		/// calculate its machine code and returns it
		/// </summary>
		/// <param name="sExpression">Command with operands</param>
		/// <param name="iLC">Current LC</param>
		/// <returns>CodeBlock with machine code that the command represents</returns>
		private CodeBlock AnalyzeCommand(string sExpression, int iLC)
		{
			int iPos = 0;
			string curWord = ReadNextWord(sExpression, out iPos);
			CutAndUpdate(ref sExpression, iPos, ref iPos);

			// Might throw exception:
			OpcodeEntry CurCommand = new OpcodeEntry(curWord);
			CodeBlock retBlock = CurCommand.OpCode;
			iLC += retBlock.Size;
			
			string[] sOperands = sExpression.Split(new char[] {','});
			for (int iCounter = 0; iCounter < sOperands.Length; ++iCounter)
				sOperands[iCounter] = sOperands[iCounter].Trim();
			
			// Too many operands were given
			if (sOperands.Length > CurCommand.NumberOfOperands) 
			{
				// For commands with 0 operands
				if (!(CurCommand.NumberOfOperands == 0 && sOperands.Length == 1
					&& sOperands[0] == ""))
					// And for the rest:
					throw new CompileError(CompilerMessage.END_OF_LINE_OR_COMMENT_EXPECTED);
			}
				// More operands needed
			else if (sOperands.Length < CurCommand.NumberOfOperands) 
				throw new CompileError(CompilerMessage.COMMA_EXPECTED);

			// Calculate operands
			CodeBlock CurOperand;
			for (int iCounter = 0; iCounter < CurCommand.NumberOfOperands; ++iCounter)
			{
				try
				{
					// Try to get the machine code for each operand
					CurOperand = FetchOperand(sOperands[iCounter], 
						new string(CurCommand.OpType[2*iCounter],1), 
						CurCommand.OpType[2*iCounter+1] - '0',
						iLC, OpcodeEntry.IsPrivilegedCommand(curWord));
				}
				catch (CompileError e)
				{
					// Sybmol not defined?
					if (e.ErrorCode == CompilerMessage.UNDEFINED_SYMBOL)
					{
						// Add to Pass2List. Will try to solve the expression in the
						// second pass
						int pass2Size;

						// (0.66): TODO: not fully tested
						// The following code did problems: example: if we are in 'a'
						// but the coding is label[rX] we need to save 6 bytes and not 5

						/*if (CurCommand.OpType[2*iCounter] == 'a')
							pass2Size = 4 + 1;
						else pass2Size = e.ExtraInformation;
						*/

						// (0.66): Therefore we write the following line, hoping it will be ok.
						pass2Size = e.ExtraInformation;
						

						_pass2List.AddEntry(new Pass2Entry(iLC, pass2Size, 
							sOperands[iCounter], true, new string(CurCommand.OpType[2*iCounter],1)));

						// Not defined? fill will zeros
						CurOperand = new CodeBlock(0, pass2Size);
					}
						// Other error? Stop analyzing the current command
					else throw;
				}

				iLC += CurOperand.Size;
				retBlock += CurOperand;
			}
			return retBlock;
		}

		/// <summary>
		/// Sets the program entry point. assuming this method is calling after all symbols were solved.
		/// </summary>
		void SetProgramEntryPoint(ProgramBlock theProg)
		{
			if (_LocateEntryPoint != "")
			{
				try
				{
					theProg.EntryPoint = int.Parse(_LocateEntryPoint) + 2;
					return;
				}
				catch(FormatException) { }

				try
				{
					// +2 for the mask word
					theProg.EntryPoint = (int)_symbolsTable.SymbolValue(_LocateEntryPoint).Value + 2;
				}
				catch (CompileError) // Only undefined symbol error can occur here
				{
					throw new CompileError(CompilerMessage.INVALID_ENTRY_POINT);
				}
			}
			else try
				 {
					 theProg.EntryPoint = (int)_symbolsTable.SymbolValue("main").Value + 2;
				 }
				 catch
				 {
					 theProg.EntryPoint = 2;
				 }

		}

		#endregion

		#region General Methods

		private void ResetAll()
		{
			_comments				= new CompilerComment();
			_symbolsTable			= new SymbolsTable();
			_programCode			= new ProgramBlock();
			_pass2List				= new Pass2List();
			_linesLocations			= new LinesLocations();
			_state					= CompilerStates.IDLE;
			_LocateEntryPoint		= "";
		}

		/// <summary>
		/// Add compile error to the list of errors
		/// </summary>
		/// <param name="iLine">Line where the error occured</param>
		/// <param name="msg">Message to display</param>
		private void AddCompileError(int iLine, CompilerMessage msg)
		{
			_comments.AddEntry(iLine, msg);
			_linesLocations.SetErrorsOnLine(iLine, true);
			_linesLocations.SetStartingAddress(iLine, -1);
			if (OnCompileError != null)	OnCompileError(iLine, msg);
		}

		private static void CutAndUpdate(ref string s, int pos, ref int len)
		{
			s = s.Substring(pos);
			len += pos;
		}

		public int GetLineFromPC (int iLine)
		{
			//_linesLocations.SetStartingAddress(iLine, -1);
			return _linesLocations.GetLineFromAddress(iLine);
		}

		private int HowManyCharAppears(string sExpression, char theChar)
		{
			int iCounter = 0;
			for (int i = 0; i < sExpression.Length; ++i) if (sExpression[i] == theChar) ++iCounter;
			return iCounter;
		}

		#endregion

	}
}
