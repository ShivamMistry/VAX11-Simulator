using System;

namespace VAX11Compiler
{
	public class CompileError : Exception
	{
		public CompileError(CompilerMessage msg)
		{
			_ErrorCode = msg;
			_ExtraInformation = 0;
		}

		public CompileError(CompilerMessage msg, int iExtraInformation)
		{
			_ErrorCode = msg;
			_ExtraInformation = iExtraInformation;
		}

		public CompilerMessage ErrorCode
		{
			get
			{
				return _ErrorCode;
			}
		}
		
		public int ExtraInformation
		{
			get
			{
				return _ExtraInformation;
			}
		}

		private readonly CompilerMessage _ErrorCode;
		private readonly int _ExtraInformation;
	}
}
