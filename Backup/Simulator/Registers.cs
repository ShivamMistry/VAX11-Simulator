using System;

using VAX11Internals;

namespace VAX11Simulator
{
	/// <summary>
	/// VAX11 Registers.
	/// </summary>
	public class Registers
	{

		#region Internal Classes - Register, VAX11_PSL

		/// <summary>
		/// Class representing a single register
		/// </summary>
		public class Register
		{
			#region Members

			protected int _iValue;

			#endregion

			#region Casting Operators

			/// <summary>
			/// Gets register's contents
			/// </summary>
			/// <param name="r">Register to get value from</param>
			/// <returns>The register's contents</returns>
			public static implicit operator int(Register r)
			{
				return r._iValue;
			}

			public static implicit operator uint(Register r)
			{
				return (uint)r._iValue;
			}

			public static implicit operator long(Register r)
			{
				return (long)((uint)r);
			}

			/// <summary>
			/// Convert int value (VAX-11 long) into register
			/// </summary>
			/// <param name="i">Register's value</param>
			/// <returns>Register</returns>
			public static implicit operator Register(int i)
			{
				Register res = new Register();
				res._iValue = i;
				return res;
			}

			/// <summary>
			/// Convert uint value (VAX-11 long) into register
			/// </summary>
			/// <param name="u">Register's value</param>
			/// <returns>Register</returns>
			public static implicit operator Register(uint u)
			{
				Register res = new Register();
				res._iValue = (int)u;
				return res;
			}


			#endregion

			#region Operators
			
			/// <summary>
			/// Increase the register's value
			/// </summary>
			/// <param name="r">Register with value</param>
			/// <returns>Register with the same value + 1</returns>
			public static Register operator++(Register r)
			{
				Register temp = new Register();
				temp._iValue = r._iValue + 1;
				return temp;
			}

			#endregion

			#region Methods

			/// <summary>
			/// Writes byte to the register. The rest of the register is unchanged.
			/// </summary>
			/// <param name="b">byte to write</param>
			public void WriteByte(byte b)
			{
				_iValue = (int)((_iValue & 0xFFFFFF00) + b);
			}

			/// <summary>
			/// Writes VAX-11 word to the register. The rest of the register is unchanged.
			/// </summary>
			/// <param name="iWord">word to write</param>
			public void WriteWord(int iWord)
			{
				if (iWord != (iWord & 0x0000FFFF)) throw new PanicException(); 
				_iValue = (int)((_iValue & 0xFFFF0000) + iWord);
			}

			/// <summary>
			/// Writes VAX-11 long to the register.
			/// </summary>
			/// <param name="iLong">Value to write</param>
			public void WriteLong(int iLong)
			{
				_iValue = iLong;
			}

			/// <summary>
			/// Reads the lower byte of the register
			/// </summary>
			/// <returns>Lower byte of the register</returns>
			public byte ReadByte()
			{
				return (byte)(_iValue & 0x000000FF);
			}

			/// <summary>
			/// Reads the lower word of the register
			/// </summary>
			/// <returns>Lower word of the register</returns>
			public int ReadWord()
			{
				return _iValue & 0x0000FFFF;
			}

			/// <summary>
			/// returns the register value
			/// </summary>
			/// <returns></returns>
			public int ReadLong()
			{
				return _iValue;
			}

			/// <summary>
			/// Just for debugging mode. Should never be used in the project
			/// </summary>
			/// <returns>string contains the registers value</returns>
			public override string ToString()
			{
				return _iValue.ToString();
			}

			public string ToString16()
			{
				return Convert.ToString(_iValue, 16);
			}

			/// <summary>
			/// Gets bits iStart-iEnd. First bit is 0
			/// </summary>
			/// <param name="iStart">starting bit</param>
			/// <param name="iEnd">end bit</param>
			/// <returns>int containing the requested bits</returns>
			public int GetBits(int iStart, int iEnd)
			{
				int mask = 0;
				if (iEnd < iStart) throw new PanicException();
				for (int iCounter = 0; iCounter < iEnd - iStart + 1; ++iCounter)
					mask = (mask << 1) | 1;
				mask = mask << iStart;
				return (int)((uint)(_iValue & mask) >> iStart);
			}

			/// <summary>
			/// Gets bits iStart-iEnd. First bit is 0
			/// </summary>
			/// <param name="iStart">starting bit</param>
			/// <param name="iEnd">end bit</param>
			/// <param name="iValue">value to get bits from</param>
			/// <returns>int containing the requested bits</returns>
			public static int GetBits(int iStart, int iEnd, int iValue)
			{
				int mask = 0;
				if (iEnd < iStart) throw new PanicException();
				for (int iCounter = 0; iCounter < iEnd - iStart + 1; ++iCounter)
					mask = (mask << 1) | 1;
				mask = mask << iStart;
				return (iValue & mask) >> iStart;
			}

			/// <summary>
			/// Sets bits iStart-iEnd. First bit is 0
			/// </summary>
			/// <param name="iStart">starting bit</param>
			/// <param name="iEnd">end bit</param>
			public void SetBits(int iStart, int iEnd, int iValue)
			{
				int mask = 0;
				if (iEnd < iStart || iValue > Math.Pow(2, iEnd - iStart + 1) - 1) throw new PanicException();
				for (int iCounter = 0; iCounter < iEnd - iStart + 1; ++iCounter)
					mask = (mask << 1) | 1;
				mask = mask << iStart;
				_iValue = (_iValue & ~mask) | (iValue << iStart);
			}

			#endregion
		}


		public class VAX11_PSL : Register
		{
			#region Properties

			/// <summary>
			/// Carry Condition Bit - Bit 0 of the PSL
			/// </summary>
			public byte C
			{
				get 
				{
					return (byte)(_iValue & 0x00000001); 
				}
				set 
				{ 
					CheckValue(value); 
					_iValue = (int)((_iValue & 0xFFFFFFFE) + value);
				}
			}

			/// <summary>
			/// Overflow Condition Code - Bit 1 of the PSL
			/// </summary>
			public byte V
			{
				get 
				{
					return (byte)((_iValue & 0x00000002) >> 1); 
				}
				set 
				{ 
					CheckValue(value); 
					_iValue = (int)((_iValue & 0xFFFFFFFD) + value * 2);
				}
			}

			/// <summary>
			/// Zero Condition Code - Bit 2 of the PSL
			/// </summary>
			public byte Z
			{
				get 
				{
					return (byte)((_iValue & 0x00000004) >> 2); 
				}
				set 
				{ 
					CheckValue(value); 
					_iValue = (int)((_iValue & 0xFFFFFFFB) + value * 4);
				}
			}

			/// <summary>
			/// Negative Condition Code - Bit 3 of the PSL
			/// </summary>
			public byte N
			{
				get 
				{
					return (byte)((_iValue & 0x00000008) >> 3); 
				}
				set 
				{ 
					CheckValue(value); 
					_iValue = (int)((_iValue & 0xFFFFFFF7) + value * 8);
				}
			}

			/// <summary>
			/// Trace Bit - Bit 4 of the PSL
			/// </summary>
			public byte T
			{
				get 
				{
					return (byte)((_iValue & 0x00000010) >> 4); 
				}
				set 
				{ 
					CheckValue(value); 
					_iValue = (int)((_iValue & 0xFFFFFFEF) + value * 16);
				}
			}

			/// <summary>
			/// Trace Pending Bit - Bit 30 of the PSL
			/// </summary>
			public byte TP
			{
				get 
				{
					return (byte)GetBits(30, 30); 
				}
				set 
				{ 
					CheckValue(value); 
					SetBits(30, 30, value);
				}
			}

			/// <summary>
			/// integer Overflow Bit - Bit 5 of the PSL
			/// </summary>
			public byte IV
			{
				get 
				{
					return (byte)((_iValue & 0x00000020) >> 5); 
				}
				set 
				{ 
					CheckValue(value); 
					_iValue = (int)((_iValue & 0xFFFFFFDF) + value * 32);
				}
			}

			/// <summary>
			/// Floating Underflow Bit - Bit 6 of the PSL
			/// </summary>
			public byte FU
			{
				get 
				{
					return (byte)((_iValue & 0x00000040) >> 6); 
				}
				set 
				{ 
					CheckValue(value); 
					_iValue = (int)((_iValue & 0xFFFFFFBF) + value * 64);
				}
			}

			/// <summary>
			/// Decimal Overflow Bit - Bit 7 of the PSL
			/// </summary>
			public byte DV
			{
				get 
				{
					return (byte)((_iValue & 0x00000080) >> 7); 
				}
				set 
				{ 
					CheckValue(value); 
					_iValue = (int)((_iValue & 0xFFFFFF7F) + value * 128);
				}
			}

			/// <summary>
			/// The processor's Interrupt Priority Level (bits 16-20)
			/// </summary>
			public byte IPL
			{
				get
				{
					return (byte)((_iValue & 0x001F0000) >> 16); 
				}
				set
				{
					if (value < 0 || value > 31) throw new PanicException();
					_iValue = (int)((_iValue & 0xFFE0FFFF) + value * 0x10000);
				}
			}

			public int PSW
			{
				get
				{
					return _iValue & 0xFFFF;
				}
				
				set
				{
					_iValue = _iValue - (_iValue & 0xFFFF) + value;

				}
			}

			#endregion

			#region Methods

			/// <summary>
			/// Allows only the values 0 or 1 for a bit
			/// </summary>
			/// <param name="b">value to check</param>
			private void CheckValue(byte b)
			{
				if (b != 0 && b != 1) throw new PanicException();
			}


			/// <summary>
			/// Set the major PSW flags. 0 or 1 for value, -1 for no change
			/// </summary>
			/// <param name="c">C Bit</param>
			/// <param name="v">V Bit</param>
			/// <param name="z">Z Bit</param>
			/// <param name="n">N Bit</param>
			public void SetFlags(sbyte c, sbyte v, sbyte z, sbyte n)
			{
				if (c != -1) C = (byte)c;
				if (v != -1) V = (byte)v;
				if (z != -1) Z = (byte)z;
				if (n != -1) N = (byte)n;
			}

			#endregion
		}

		#endregion

		#region Members

		/// <summary>
		/// Registers Array. Note that no all the elements in the array are accessable.
		/// </summary>
		Register[] Reg;

		/// <summary>
		/// Processor Status LongWord
		/// </summary>
		VAX11_PSL _PSL;

		/// <summary>
		/// Time in cycles from the system startup
		/// </summary>
		Register _SystemTime;

		#endregion

		#region Consturctors

		/// <summary>
		/// Consturctor. Creates all the registers
		/// </summary>
		public Registers()
		{
			Reg = new Register[36];
			for (int i = 0; i < 36; Reg[i++] = new Register());
			_PSL = new VAX11_PSL();
			_SystemTime = new Register();
		}

		#endregion

		#region Properties & Indexer

		/// <summary>
		/// Indexer
		/// </summary>
		/// <remarks>Might throw IndexOutOfRangeException</remarks>
		public Register this[int iIndex]
		{
			get
			{
				IsLegalRegister(iIndex);
				return Reg[iIndex];
			}
			set
			{
				IsLegalRegister(iIndex);
				Reg[iIndex] = value;
			}
		}

		/// <summary>
		/// Get reference to the PSL register
		/// </summary>
		public VAX11_PSL PSL
		{
			get { return this._PSL; }
			set { _PSL = value; }
		}

		/// <summary>
		/// Time in cycles from the system startup
		/// </summary>
		public Register SystemTime
		{
			get {return _SystemTime; }
			set { _SystemTime = value; }
		}

		public Register PC
		{
			get { return Reg[15]; }
			set { Reg[15] = value; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// Helping function that tells if we trying to access legal register or not
		/// </summary>
		/// <param name="iIndex">Register's number</param>
		private void IsLegalRegister(int iIndex)
		{
			if (iIndex == 16 || iIndex == 19 || iIndex == 22 || iIndex == 23
				|| (iIndex > 26 && iIndex < 32 && iIndex != 30)) throw new PanicException();
		}

		#endregion

	}
}
