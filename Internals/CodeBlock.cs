using System;

using VAX11Compiler;

namespace VAX11Internals
{
	/// <summary>
	/// Summary description for CodeBlock.
	/// </summary>
	public class CodeBlock
	{

		#region Consturctors

		/// <summary>
		/// Constructor
		/// </summary>
		public CodeBlock()
		{
		}

		/// <summary>
		/// Make *iBlockSize* Number from the number iNum
		/// </summary>
		/// <param name="iNum">The number to save</param>
		/// <param name="iBlockSize">requested block size</param>
		/// <remarks>VAX11 is little-endian</remarks>
		public CodeBlock(long iNum, int iBlockSize)
		{
			// Negative block size
			if (iBlockSize < 0) throw new PanicException();

			// Block is too small to contain the number
			if (Math.Pow(2, iBlockSize * BYTE_SIZE) - 1 < iNum) 
				throw new CompileError(CompilerMessage.NUMBER_IS_TOO_BIG);
			else if (-Math.Pow(2, iBlockSize * BYTE_SIZE) / 2 > iNum) throw new CompileError(CompilerMessage.NUMBER_IS_TOO_SMALL);
			_MachineCode = new byte[iBlockSize];

			// Save the number as little endian number :) me gosu
			long lMask = 0xFF;
			ushort iShift = 0;
			for (int iCounter = 0; iCounter < iBlockSize; ++iCounter)
			{
				_MachineCode[iCounter] = (byte)((iNum & lMask) >> iShift);
				lMask *= 0x100;
				iShift += 8;
			}
		}

		/// <summary>
		/// Creates CodeBlock in size iBlockSize with specific fill character
		/// </summary>
		/// <param name="iBlockSize">requested block size</param>
		/// <param name="fill">The fill</param>
		public CodeBlock(int iBlockSize, byte fill)
		{
			// Negative block size
			if (iBlockSize < 0) throw new PanicException();

			_MachineCode = new byte[iBlockSize];
			for (int iCounter = 0; iCounter < iBlockSize; _MachineCode[iCounter++] = fill);
		}

		#endregion

		#region Properties

		/// <summary>
		/// CodeBlock Data
		/// </summary>
		public byte[] MachineCode
		{
			get
			{
				return _MachineCode;
			}
			set
			{
				_MachineCode = (byte[])value.Clone();
			}
		}


		/// <summary>
		/// Block Size
		/// </summary>
		public int Size
		{
			get
			{
				if (_MachineCode == null) return 0;
				return _MachineCode.Length;
			}
		}


		#endregion

		#region Members

		/// <summary>
		/// Define the size of a byte. It won't help to change this value,
		/// as many parts of the code assume its 8. Its here only to make the
		/// code read-able, not making it more general
		/// </summary>
		private const int BYTE_SIZE = 8;

		/// <summary>
		/// CodeBlock Data
		/// </summary>
		private byte[] _MachineCode = null;

		#endregion

		#region Operators

		/// <summary>
		/// Indexer
		/// </summary>
		/// <remarks>Might throw IndexOutOfRangeException</remarks>
		public byte this[int iIndex]
		{
			get
			{
				if (_MachineCode == null) throw new PanicException();
				return _MachineCode[iIndex];
			}
			set
			{
				if (_MachineCode == null) throw new PanicException();
				_MachineCode[iIndex] = value;
			}
		}


		/// <summary>
		/// Create new CodeBlock from 2 existing blocks
		/// </summary>
		/// <param name="a">First Block</param>
		/// <param name="b">Second Block</param>
		/// <returns>New block</returns>
		public static CodeBlock operator +(CodeBlock a, CodeBlock b) 
		{
			// If one of the block is an empty block
			if (a._MachineCode == null && b._MachineCode != null) return b.Clone();
			else if (a._MachineCode != null && b._MachineCode == null) return a.Clone();
			else if (a._MachineCode == null && b._MachineCode == null) return new CodeBlock();
			
			// If none of the blocks is empty, we creates new block that will contains both blocks
			CodeBlock res = new CodeBlock();
			byte[] temp = new byte[a._MachineCode.Length + b._MachineCode.Length];
			int iIndex, iIndex1;
			for (iIndex = 0; iIndex < a._MachineCode.Length; iIndex++) 
				temp[iIndex] = a._MachineCode[iIndex];
			for (iIndex1 = 0; iIndex1 < b._MachineCode.Length; iIndex1++)
				temp[iIndex + iIndex1] = b._MachineCode[iIndex1];
			res._MachineCode = temp;
			return res;
		}

		public override bool Equals(object o) 
		{
			CodeBlock rhs = (CodeBlock)o;
            if (_MachineCode.Length != rhs._MachineCode.Length)
            {
                return false;
            }
            for (int iCounter = 0; iCounter < _MachineCode.Length; ++iCounter)
            {
                if (_MachineCode[iCounter] != rhs._MachineCode[iCounter])
                {
                    return false;
                }
            }
			return true;
		}

		/// <summary>
		/// Temp, will never be called, just for C# requeriments
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return _MachineCode[0];
		}

		#endregion

		#region Casting Operators

		/// <summary>
		/// Convert from byte to CodeBlock
		/// </summary>
		/// <param name="b">Byte to convert</param>
		/// <returns>CodeBlock contains the byte contents</returns>
		public static implicit operator CodeBlock(byte b)
		{
			CodeBlock res = new CodeBlock();
			res._MachineCode = new byte[1];
			res[0] = b;
			return res;
		}

		/// <summary>
		/// Convert from long to CodeBlock
		/// </summary>
		/// <param name="iNum">long to convert</param>
		/// <returns>CodeBlock contains the long contents</returns>
		public static implicit operator CodeBlock(long iNum)
		{
			CodeBlock res = new CodeBlock();

			// If we wants to create CodeBlock containing zero
			if (iNum == 0) return new CodeBlock(0L, 1);

			while (iNum > 0)
			{
				res = res + (byte)(iNum % 0x100);
				iNum /= 0x100;
			}
			return res;
		}

		/// <summary>
		/// Convert string into BlockCode
		/// </summary>
		/// <param name="s">The String</param>
		/// <returns>CodeBlock reperesenting the string</returns>
		public static explicit operator CodeBlock(string s)
		{
			char[] temp = s.ToCharArray();
			CodeBlock res = new CodeBlock(0L, temp.Length);
			for (int iIndex = 0; iIndex < temp.Length; ++iIndex)
				res[iIndex] = (byte)(temp[iIndex]);
			return res;
		}

		/// <summary>
		/// Converts CodeBlock to long. If the CodeBlock is bigger than 8, it takes
		/// only the 8 first bytes
		/// </summary>
		/// <param name="b">CodeBlock to convert</param>
		/// <returns>long value</returns>
		public static explicit operator long(CodeBlock b)
		{
			return CastingHelpFunction(b, 8);
		}

        public static explicit operator string(CodeBlock b)
        {
            string str = "";
            for (int i = 0; i < b.Size; i++)
            {
                str += b[i].ToString();
            }
            return str;
        }

        /// <summary>
        /// Converts CodeBlock to int. If the CodeBlock is bigger than 4, it takes
        /// only the 4 first bytes
        /// </summary>
        /// <param name="b">CodeBlock to convert</param>
        /// <returns>int value</returns>
        public static implicit operator int(CodeBlock b)
		{
			return (int)CastingHelpFunction(b, 4);
		}

		/// <summary>
		/// Converts CodeBlock to byte. If the CodeBlock is bigger than 1, it takes
		/// only the 1 first byte
		/// </summary>
		/// <param name="b">CodeBlock to convert</param>
		/// <returns>byte value</returns>
		public static explicit operator byte(CodeBlock b)
		{
			return (byte)CastingHelpFunction(b, 1);
		}

		/// <summary>
		/// Helping function for all the casting operators. Do the actual casting,
		/// making sure that the casting result is legal.
		/// </summary>
		/// <param name="b">CodeBlock to convert</param>
		/// <param name="iMaxSize">Maximum size (in bytes) for the result</param>
		/// <returns>long number containing the result</returns>
		private static long CastingHelpFunction(CodeBlock b, int iMaxSize)
		{
			int iSize = b.Size > iMaxSize ? iMaxSize : b.Size;
			long lResult = b[iSize - 1];
			for (int iCounter = iSize - 2; iCounter >= 0; --iCounter)
			{
				lResult *= 0x100;
				lResult += b[iCounter];
			}
			
			return lResult;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Create a copy of existing block
		/// </summary>
		/// <returns></returns>
		public CodeBlock Clone()
		{
			CodeBlock res = new CodeBlock();
			res._MachineCode = (byte[])_MachineCode.Clone();
			return res;
		}

		/// <summary>
		/// Gets a sub block from the block
		/// </summary>
		/// <param name="iStartPosition">Starting Position</param>
		/// <returns></returns>
		public CodeBlock SubBlock(int iStartPosition)
		{
			// Illegal starting position
			if (iStartPosition > _MachineCode.Length) throw new PanicException();
			CodeBlock res = new CodeBlock(0L, _MachineCode.Length - iStartPosition);
			for (int iCounter = iStartPosition; iCounter < _MachineCode.Length; ++iCounter)
				res[iCounter - iStartPosition] = _MachineCode[iCounter];
			return res;
		}

		/// <summary>
		/// Gets a sub block from the block
		/// </summary>
		/// <param name="iStartPosition">Starting Position</param>
		/// <param name="iMaxLen">Maximum size</param>
		/// <returns></returns>
		public CodeBlock SubBlock(int iStartPosition, int iMaxLen)
		{
			// Illegal starting position
			if (_MachineCode == null) throw new PanicException();
			if (iStartPosition > _MachineCode.Length) throw new PanicException();
			int iEndPosition = (_MachineCode.Length < iStartPosition + iMaxLen) ? _MachineCode.Length : iStartPosition + iMaxLen;
			CodeBlock res = new CodeBlock(0L, iEndPosition - iStartPosition);
			for (int iCounter = iStartPosition; iCounter < iEndPosition; ++iCounter)
				res[iCounter - iStartPosition] = _MachineCode[iCounter];
			return res;
		}


		/// <summary>
		/// Copy a sub block to the block from specific location. (override existing data).
		/// If the sub block is bigger then the current size, we reallocate the block to fit the new size
		/// </summary>
		/// <param name="iStartPosition">Starting position to copy to</param>
		/// <param name="subBlock">the sub block to copy</param>
		/// <param name="ExtraSize">If we need to extend the block, specify the numbers of bytes (extra then needed to add).</param>
		public void CopySubBlock(int iStartPosition, CodeBlock subBlock, int ExtraSize)
		{
			if (_MachineCode == null)
			{
				_MachineCode = subBlock._MachineCode;
				return;
			}

			if (_MachineCode.Length < iStartPosition) throw new PanicException();
			if (subBlock.Size + iStartPosition > _MachineCode.Length)
			{
				byte[] temp = new byte[subBlock.Size + iStartPosition + ExtraSize];
				for (int iCounter = 0; iCounter < _MachineCode.Length; iCounter++)
					temp[iCounter] = _MachineCode[iCounter];
				_MachineCode = temp;
			}

			for (int iCounter = iStartPosition; iCounter < iStartPosition + subBlock.Size; ++iCounter)
				_MachineCode[iCounter] = subBlock[iCounter - iStartPosition];

		}

		#endregion

	}

}
