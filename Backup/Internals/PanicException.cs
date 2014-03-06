using System;

namespace VAX11Internals
{
	/// <summary>
	/// We shuold never get this exception. This exception is thrown only
	/// if there are bugs in our code.
	/// </summary>

	class PanicException : Exception
	{
		public PanicException() : base("If you get this exception, then your code sucks")
		{
			
		}
	}
}
