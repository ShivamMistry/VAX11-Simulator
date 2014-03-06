using System;

using VAX11Internals;

namespace VAX11Simulator
{
	/// <summary>
	/// Functions uses this emum to report about the different errors
	/// it found in the code 
	/// </summary>
	public enum SimulatorMessage
	{
		NORMAL_EXIT,
		SYSTEM_HALTED,
		DIVIDE_BY_ZERO,
		ILLEGAL_POSITION,
		UNRECOGNIZED_OPCODE_NAME,
		UNSUPPORTED_OPCODE,
		ILLEGAL_ADDRESSING_MODE,
		TOO_FEW_PARAMS,
		WRONG_NUMBER_OF_PARAMS,
		ACCESS_OUT_OF_CONSOLE,
		INPUT_FUNCTIONS_NOT_ALLOWED,
		NOT_ALLOWED_IN_GRAPH_MODE,
		NOT_ALLOWED_IN_TEXT_MODE,
		PRIVILEGED_IS_WRITE_ONLY,
		PRIVILEGED_IS_READ_ONLY,
		QUAD_CAN_BE_USED_ONLY_ON_EVEN_REGISTERS,
		PC_CANT_BE_USED_AS_INDEX_REG,
		ARITHMETIC_EXCEPTION,
		TRAP_NOT_HANDLED,
		RESERVED_OPERAND,
		GENERAL_FAULT_EXCEPTION,
		MEMORY_ACCESS_FAULT,
		SUBSCRIPT_RANGE,
		CONSOLE_EXIT
	}

	/// <summary>
	/// Compiler uses this class to create human-readable messages
	/// for the users
	/// </summary>
	public class SimulatorMessagesStrings
	{
		private static string[] MessagesArray = 
		{
			"Normal exit",
			"Program reached HALT instruction",
			"Divide by zero",
			"Illegal Position",
			"Unrecognized opcode name",
			"Unsupported opcode",
			"Illegal addressing mode",
			"Too few parameters",
			"Wrong number of parameters",
			"Accessing out of console window",
			"Can't use operation system input commands while using Interrupts",
			"Can't use this function in graphic mode",
			"Can't use this function in text mode",
			"NOT TO BE DISPLAYED - Internal Error",
			"NOT TO BE DISPLAYED - Internal Error",
			"Quad commands can be used only on even registers",
			"The PC can't be used as an index register",
			"Arithmetic exception not handled",
			"Trap not handled",
			"Reserved operand fault",
			"General Fault Exception",
			"Memory access fault",
			"Subscript out of range fault",
			"Console ended"
		};

		/// <summary>
		/// Gets string from enum
		/// </summary>
		/// <param name="m">The enum representing the message</param>
		/// <returns>String Message</returns>
		public static string GetMessageText(SimulatorMessage m, int iExtraInformation)
		{
			if (m == SimulatorMessage.PRIVILEGED_IS_READ_ONLY)
			{
				return RegistersSettings.GetPrivilegedRegisterName(iExtraInformation)
					+ " is read-only register";
			}
			else if (m == SimulatorMessage.PRIVILEGED_IS_WRITE_ONLY)
			{
				return RegistersSettings.GetPrivilegedRegisterName(iExtraInformation)
					+ " is write-only register";
			}
			else return MessagesArray[(int)m];
		}

	}
}
