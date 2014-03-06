using System;
using System.Collections;

using VAX11Compiler;

namespace VAX11Internals
{
	/// <summary>
	/// Containing strings representing registers names.
	/// </summary>
	public class RegistersSettings
	{
		private const byte MAX_REGULAR_REGISTER_INDEX = 15;
		private static Hashtable RegHash = new Hashtable();
		private static Hashtable RegValuesHash = new Hashtable();

		static RegistersSettings()
		{
			for (byte i = 0; i < 15; ++i) RegHash["R" +i.ToString()] = i;
			
			RegHash["AP"]		 = (byte)12;
			RegHash["FP"]		 = (byte)13;
			RegHash["SP"]		 = (byte)14;
			RegHash["PC"]		 = (byte)15;
			RegHash["SCBB"]      = (byte)17;
			RegHash["IPL"]       = (byte)18;
			RegHash["SIRR"]      = (byte)20;
			RegHash["SISR"]      = (byte)21;
			RegHash["ICCS"]      = (byte)24;
			RegHash["NICR"]      = (byte)25;
			RegHash["ICR"]       = (byte)26;
			RegHash["OSC"]       = (byte)30;
			RegHash["RXCS"]      = (byte)32;
			RegHash["RXDB"]      = (byte)33;
			RegHash["TXCS"]      = (byte)34;
			RegHash["TXDB"]      = (byte)35;

			IDictionaryEnumerator it = RegHash.GetEnumerator();
			it.Reset(); 
			while (it.MoveNext())
			{
				RegValuesHash[it.Value] = it.Key;
			}
		}

		public static byte GetRegister(string sRegName)
		{
			sRegName = sRegName.ToUpper();
			if (!RegHash.ContainsKey(sRegName))
			{
				throw new CompileError(CompilerMessage.ILLEGAL_ADDRESSING_MODE);
			}
			if ((byte)RegHash[sRegName] > MAX_REGULAR_REGISTER_INDEX)
				throw new CompileError(CompilerMessage.PRIVILEGED_REGISTER_CANT_BE_USED_HERE);
			return (byte)RegHash[sRegName];
		}

		/// <summary>
		/// Gets a number representing a special register from its name
		/// </summary>
		/// <param name="sRegName">Register name</param>
		/// <returns>Number representing the special register</returns>
		public static byte GetPrivilegedRegister(string sRegName)
		{
			sRegName = sRegName.ToUpper();
			if (!RegHash.ContainsKey(sRegName))
			{
				throw new CompileError(CompilerMessage.ILLEGAL_ADDRESSING_MODE);
			}
			if ((byte)RegHash[sRegName] <= MAX_REGULAR_REGISTER_INDEX)
				throw new CompileError(CompilerMessage.EXPECTED_PRIVILEGED_REGISTER);
			return (byte)RegHash[sRegName];
		}

		/// <summary>
		/// Get privileged register name from its number.
		/// </summary>
		/// <param name="key">register number</param>
		/// <returns>String containing the register's name</returns>
		public static string GetPrivilegedRegisterName(int key)
		{
			if (!RegValuesHash.ContainsKey((byte)key)) throw new PanicException();
			return (string)RegValuesHash[(byte)key];
		}
	}
}

