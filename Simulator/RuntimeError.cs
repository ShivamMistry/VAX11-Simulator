using System;

namespace VAX11Simulator
{
	public class RuntimeError : Exception
	{
		/// <summary>
		/// Consturctor
		/// </summary>
		/// <param name="msg">Error Description</param>
		/// <param name="iPC">Location of the error</param>
		public RuntimeError(SimulatorMessage msg, int iPC)
		{
			_ErrorCode = msg;
			_ExtraInformation = 0;
			_PC = iPC;
		}

		/// <summary>
		/// Consturctor
		/// </summary>
		/// <param name="msg">Error Description</param>
		/// <param name="iPC">Location of the error</param>
		/// <param name="iExtraInformation">Extra information about the error</param>
		public RuntimeError(SimulatorMessage msg, int iPC, int iExtraInformation)
		{
			_ErrorCode = msg;
			_ExtraInformation = iExtraInformation;
			_PC = iPC;
		}

		/// <summary>
		/// The error code
		/// </summary>
		public SimulatorMessage ErrorCode
		{
			get
			{
				return _ErrorCode;
			}
		}
		
		/// <summary>
		/// Field to save extra information about the error
		/// </summary>
		public int ExtraInformation
		{
			get
			{
				return _ExtraInformation;
			}
		}

		/// <summary>
		/// PC when the error occured
		/// </summary>
		public int PC
		{
			get
			{
				return _PC;
			}
		}

		private readonly SimulatorMessage _ErrorCode;
		private readonly int _ExtraInformation;
		private readonly int _PC;
	}
}
