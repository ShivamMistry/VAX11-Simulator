using System;

using VAX11Internals;

namespace VAX11Simulator
{
	/// <summary>
	/// Summary description for Operand.
	/// </summary>
	public class Operand
	{
		#region Members

		/// <summary>
		/// Effective Address
		/// </summary>
		private int _ea;

		/// <summary>
		/// Operand Contents
		/// </summary>
		private long _operand;

		/// <summary>
		/// Is the operand has legal effective address?
		/// </summary>
		private bool _hasEffectiveAddress;

		/// <summary>
		/// Is the operand is register?
		/// </summary>
		private bool _operandIsRegister;

		#endregion

		#region Properties

		/// <summary>
		/// Effective Address
		/// </summary>
		public int EffectiveAddress
		{
			get { return _ea; }
		}

		/// <summary>
		/// Operand Contents
		/// </summary>
		public long Op
		{
			get { return _operand; }
		}

		/// <summary>
		/// Is the operand has legal effective address?
		/// </summary>
		public bool HasEffectiveAddress
		{
			get { return _hasEffectiveAddress; }
		}

		/// <summary>
		/// Is the operand is register?
		/// </summary>
		public bool IsRegister
		{
			get { return _operandIsRegister; }
		}

		#endregion

		#region Constructor

		public Operand(int iEffectiveAddress, long lOperand, bool isRegister, bool HasEffectiveAddress)
		{
			_ea = iEffectiveAddress;
			_operand = lOperand;
			_hasEffectiveAddress = HasEffectiveAddress;
			_operandIsRegister = isRegister;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Checks if the given number is negative number, if we takes iSize bytes from it
		/// </summary>
		/// <param name="lValue">The number</param>
		/// <param name="iSize">Number of bytes to check</param>
		/// <returns>1 or 0 (for using as N flag mainly)</returns>
		public static sbyte IsNeg(long lValue, int iSize)
		{
			long mask = 0;
			for (int iCounter = 0; iCounter < iSize; ++iCounter) mask = (mask * 0x100) + 0xFF;
			return ((ulong)(lValue & mask) >= 0x8 * Math.Pow(0x10, iSize * 2 - 1)) ? (sbyte)1 : (sbyte)0;
		}

		/// <summary>
		/// Checks if the given number is equal to zero, if we takes iSize bytes from it
		/// </summary>
		/// <param name="lValue">The number</param>
		/// <param name="iSize">Number of bytes to check</param>
		/// <returns>1 or 0 (for using as Z flag mainly)</returns>
		public static sbyte IsZero(long lValue, int iSize)
		{
			// Mask for the value
			long mask = 0;
			for (int iCounter = 0; iCounter < iSize; ++iCounter) mask = (mask * 0x100) + 0xFF;

			CodeBlock temp = new CodeBlock(lValue & mask, iSize);
			return (int)(temp) == 0 ? (sbyte)1 : (sbyte)0;
		}

		/// <summary>
		/// Checks if the given number has carry, as number of iSize bytes.
		/// </summary>
		/// <param name="lValue">The number</param>
		/// <param name="iSize">Number of bytes to check</param>
		/// <returns>1 or 0 (for using as C flag mainly)</returns>
		public static sbyte HasCarry(long lValue, int iSize)
		{
			// I think I can do better, but for now its enough
			return NumberCanBePresentedNbytesBlock(lValue, iSize);
		}


		/// <summary>
		/// Checks if the given sum of two numbers has overflow, as number of iSize bytes.
		/// </summary>
		/// <returns>1 or 0 (for using as V flag mainly)</returns>
		public static sbyte SumHasOverflow(long lValue1, long lValue2, int iSize)
		{
			if (Operand.IsNeg(lValue1 ^ lValue2, iSize) == 0)
			{
				return Operand.IsNeg(lValue1 + lValue2, iSize) != Operand.IsNeg(lValue1, iSize) ? (sbyte)1 : (sbyte)0;
			}
			return 0;
		}


		/// <summary>
		/// Is the number can be presented as iSize bytes?
		/// </summary>
		/// <param name="lValue">The number</param>
		/// <param name="iSize">Number of bytes to check</param>
		/// <returns>1 if yes else 0</returns>
		public static sbyte NumberCanBePresentedNbytesBlock(long lValue, int iSize)
		{
			if (lValue == 0) return (sbyte)1;
			int iBlockSize;
			if (lValue < 0) iBlockSize = Convert.ToInt32(Math.Log(Math.Abs(lValue) * 2, 2)) / 8 + 1;
			else iBlockSize = Convert.ToInt32(Math.Log(lValue, 2)) / 8 + 1;

			return iBlockSize > iSize ? (sbyte)1 : (sbyte)0;
		}


		/// <summary>
		/// Compare 2 values. Returns 1 is the first one is bigger, 0 is they are equal, or -1
		/// if the second is bigger
		/// </summary>
		/// <param name="lValue1">First Value</param>
		/// <param name="iSize1">First Value's Size</param>
		/// <param name="lValue2">Second Value</param>
		/// <param name="iSize2">Second Value Size</param>
		/// <returns>1, 0 or -1, telling which one of the values is bigger</returns>
		public static sbyte Compare(long lValue1, int iSize1, long lValue2, int iSize2)
		{
			int iMSB1 = GetBits(iSize1 * 8 - 1, iSize1 * 8 - 1, lValue1);
			int iMSB2 = GetBits(iSize2 * 8 - 1, iSize2 * 8 - 1, lValue2);
			
			// If the numbers are from opposite sign:
			if ((iMSB1 ^ iMSB2) == 1) return (iMSB1 == 0) ? (sbyte)1 : (sbyte)-1;

			// if one number is on more bytes the the second one
			if (iSize1 != iSize2)
			{
				// search on the upper bytes for bits that deffered from the MSB
				for (int iCounter = Math.Max(iSize1 * 8, iSize2 * 8) - 1; 
					iCounter >= Math.Min(iSize1 * 8, iSize2 * 8); --iCounter)
				{
					if (GetBits(iCounter, iCounter, (iSize1 > iSize2) ? lValue1 : lValue2) != iMSB1)
					{
						if (iMSB1 == 0) return (iSize1 > iSize2) ? (sbyte)1 : (sbyte)-1;
						else return (iSize1 > iSize2) ? (sbyte)-1 : (sbyte)1;
					}
				}
			}

			for (int iCounter = Math.Min(iSize1 * 8, iSize2 * 8) - 1; iCounter >= 0; --iCounter)
			{
				int bit1, bit2;
				if ((bit1 = GetBits(iCounter, iCounter, lValue1)) != (bit2 = GetBits(iCounter, iCounter, lValue2)))
				{
					return (bit1 > bit2) ? (sbyte)1 : (sbyte)-1;
				}
			}

			return 0;
		}

		
		/// <summary>
		/// Compare 2 unsigned values. Returns 1 is the first one is bigger, 0 is they are equal, or -1
		/// if the second is bigger
		/// </summary>
		/// <param name="lValue1">First Value</param>
		/// <param name="iSize1">First Value's Size</param>
		/// <param name="lValue2">Second Value</param>
		/// <param name="iSize2">Second Value Size</param>
		/// <returns>1, 0 or -1, telling which one of the values is bigger</returns>
		public static sbyte CompareUnsigned(long lValue1, int iSize1, long lValue2, int iSize2)
		{
			// if one number is on more bytes the the second one
			if (iSize1 != iSize2)
			{
				// search on the upper bytes for bits that deffered from the MSB
				for (int iCounter = Math.Max(iSize1 * 8, iSize2 * 8) - 1; 
					iCounter >= Math.Min(iSize1 * 8, iSize2 * 8); --iCounter)
				{
					if (GetBits(iCounter, iCounter, (iSize1 > iSize2) ? lValue1 : lValue2) != 0)
					{
						return (iSize1 > iSize2) ? (sbyte)1 : (sbyte)-1;
					}
				}
			}

			for (int iCounter = Math.Min(iSize1 * 8, iSize2 * 8) - 1; iCounter >= 0; --iCounter)
			{
				int bit1, bit2;
				if ((bit1 = GetBits(iCounter, iCounter, lValue1)) != (bit2 = GetBits(iCounter, iCounter, lValue2)))
				{
					return (bit1 > bit2) ? (sbyte)1 : (sbyte)-1;
				}
			}

			return 0;
		}


		/// <summary>
		/// Create word mask with 11..11 on iSize bytes. Can be use in many places in the project,
		/// but many places don't use it as we wrote this function only late in the project.
		/// It can be updated as will.
		/// </summary>
		/// <param name="iSize">Number of bytes</param>
		/// <returns>bytes filled with bits sets to 1</returns>
		public static long Mask(int iSize)
		{
			long mask = 0;
			for (int iCounter = 0; iCounter < iSize; ++iCounter) mask = (mask * 0x100) + 0xFF;
			return mask;
		}

		/// <summary>
		/// Gets bits iStart-iEnd. First bit is 0
		/// </summary>
		/// <param name="iStart">starting bit</param>
		/// <param name="iEnd">end bit</param>
		/// <param name="iValue">value to get bits from</param>
		/// <returns>int containing the requested bits</returns>
		public static int GetBits(int iStart, int iEnd, long lValue)
		{
			long mask = 0;
			if (iEnd < iStart) throw new PanicException();
			for (int iCounter = 0; iCounter < iEnd - iStart + 1; ++iCounter)
				mask = (mask << 1) | 1;
			mask = mask << iStart;
			return (int)((lValue & mask) >> iStart);
		}

		/// <summary>
		/// Convert numbet to long. Gets the previous number size (in bytes) as parameters
		/// </summary>
		/// <param name="lNum">The number</param>
		/// <param name="BytesNumber">Number of bytes, must be 1, 2, 4 or 8</param>
		/// <returns>the number as long value</returns>
		public static long ConvertToLong(long lNum, int BytesNumber)
		{
			switch(BytesNumber)
			{
				case 1:
					return (long)((sbyte)lNum);
				case 2:
					return (long)((short)lNum);
				case 4:
					return (long)((int)lNum);
				case 8:
					return lNum;
				default:
					throw new PanicException();
			}
		}

		#endregion

		#region Casting Operators
		
		public static implicit operator long(Operand o)
		{
			return o._operand;
		}

		#endregion
	}
}
