using System;

using VAX11Compiler;

namespace VAX11Internals
{
	/// <summary>
	/// Help function for analyzing strings (VAX11 code).
	/// </summary>
	public class CodeProcessor
	{

		/// <summary>
		/// helping function that check if the given word is directive.
		/// </summary>
		/// <param name="Word">the word that need to be checked</param>
		/// <returns>true if directive, else false</returns>
		public static bool IsDirective (string word)
		{
			if (word.IndexOf(".") != -1) word = word.Substring(word.IndexOf(".")+1);
			word = word.ToUpper();
			if (word=="DATA"||word=="TEXT"||word=="ORG"||word=="SPACE"||word=="SET"
				||word=="ASCII"||word=="ASCIZ"||word=="ASCIC"||word=="BYTE"||word=="WORD"
				||word=="INT"||word=="LONG"||word=="QUAD"||word=="ENTRYPOINT") 
				return true;
			else
				return false;
		}

		/// <summary>
		/// helping function that check if the given word is a system call.
		/// </summary>
		/// <param name="Word">the word that need to be checked</param>
		/// <returns>true if system call, else false</returns>
		public static bool IsSystemCall (string word)
		{
			if (word.IndexOf(".") != -1) word = word.Substring(word.IndexOf(".")+1);
			try
			{
				VAX11Internals.KnownFunctions.GetAddress(word);
				return true;
			}
			catch (CompileError) { return false; }
		}

		/// <summary>
		/// helping function that check if the given word is a opcode.
		/// </summary>
		/// <param name="Word">the word that need to be checked</param>
		/// <returns>true if opcode, else false</returns>
		public static bool IsOpcode (string word)
		{
			try
			{
				OpcodeEntry CurCommand = new OpcodeEntry(word);
				return true;
			}
			catch (CompileError) { return false; }
		}
	}
}
