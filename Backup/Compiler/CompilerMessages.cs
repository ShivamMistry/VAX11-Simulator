using System;
using System.Collections;

namespace VAX11Compiler
{


	/// <summary>
	/// Functions uses this emum to report about the different errors
	/// it found in the code 
	/// </summary>
	public enum CompilerMessage
	{
		CLEAR_TASK_LIST,	// ugly. we send this message in order to clear the task list
		NEW_ORG,		    // ugly2. we send this message to define new region of code
		COMPILE_SUCCEED, 
		COMPILE_FAILED,		/* Only DoPass1(), DoPass2() functions should use this code */
		NUMBER_EXPECTED,
		UNEXPECTED_END_OF_LINE,
		LABEL_ALREADY_DEFINED,
        DOUBLE_LABEL_IN_LINE,
        ILLEGAL_ADDRESSING_MODE,
        NUMBER_IS_TOO_BIG,
        REGISTER_EXPECTED,
        SYMBOL_EXPECTED,
        REGISTER_IS_NOT_ALLOWED_HERE,
        UNRECOGNIZED_PROCEDURE,
        PARENTHESES_EXPECTED,
		UNRECOGNIZED_OPCODE_NAME,
    	IMMEDIATE_ADDRESSING_MODE_CANNOT_BE_DESTINATION,
        RECTANGLE_PARENTHESES_EXPECTED,
        RN_MUST_BE_DIFFERENT_THAN_RX,
        END_OF_OPERAND_EXPECTED,
        END_OF_LINE_OR_COMMENT_EXPECTED,
		IMMEDIATE_ADDRESSING_MODE_IS_NOT_ALLOWED_HERE,
        UNRECOGNIZED_DIRECTIVE,
        DEFINED_SYMBOL_OR_NUMBER_EXPECTED,
        INVERTED_COMMAS_EXPECTED,
        VALUE_EXPECTED,
		COMMA_EXPECTED,
		PC_CANNOT_BE_USED_HERE,
        DISPLACEMENT_TOO_BIG,
        PROCEDURE_NOT_ALLOWED_HERE,
        DIVIDE_BY_ZERO,
        UNDEFINED_SYMBOL,
        PROGRAM_MUST_BEGIN_WITH_TEXT,
        PRIVILEGED_REGISTER_CANT_BE_USED_HERE,
		ILLEGAL_EXPRESSION,
		EXPECTED_PRIVILEGED_REGISTER,
		ILLEGAL_STRING_FORMAT,
		ILLEGAL_STRING_FORMAT_STRING,
		ORG_CANT_USED_TO_JUMP_BACKWARD,
		WORD_REQUIRE_POSITIVE_NUMBERS_USE_INT,
		VALUE_OUT_OF_INT_RANGE_USE_LONG,
		ASCIC_STRING_TOO_LONG,
		VALUE_OUT_OF_BYTE_RANGE_USE_INT,
		NUMBER_IS_TOO_SMALL,
		CANT_DEFINE_TWO_ENTRY_POINTS,
		INVALID_ENTRY_POINT,
		CANT_USE_NEGATIVE_NUMBERS_ON_ORG,
		SPACE_CANT_BE_NEGATIVE,
		INDEX_CANT_BE_THE_SAME_AS_BASE_REG,
	}

	/// <summary>
	/// Compiler uses this class to create human-readable messages
	/// for the users
	/// </summary>
	public class CompilerMessagesStrings
	{
		private static string[] MessagesArray = 
		{
			"NOT TO BE DISPLAYED - Internal Error",
			"NOT TO BE DISPLAYED - Internal Error",
			"Compile Succeed",
			"Compile Failed",
			"Number expected",
			"Unexpected end of line",
            "Label already defined",
            "Double label in line",
            "Illegal addressing mode",
			"Number is too big",
			"Register expected",
			"Symbol expected",
			"Register is not allowed here",
			"Unrecognized procedure",
            ") expected",
			"Unrecognized opcode name",
			"Immediate addressing mode cannot be destination",
			"] expected",
            "Rn must be different than Rx",
            "End of operand expected",
            "End of line or comment expected",
			"Using of Literal/Immediate addressing mode is not allowed here. The opcode require using of other addressing modes",
			"Unrecognized Directive",
			"Defined symbol or number expected",
			"\" expected",
			"Value (number or symbol) expected",
			", expected",
            "PC cannot be used here",
            "Displacement too big",
            "Procedure not allowed here",
			"Divide by zero",
			"Undefined symbol",
			"Program must begin with .TEXT",
			"Privileged register can't be used here",
			"Illegal expression",
			"Expected: Privileged register",
			"Illegal string format - \\ used with wrong control char",
			"Illegal string format - Single \" appears in the middle of the string",
			"ORG directive can't be used to jump backward",
			"Negative numbers can't be assigned to .WORD. Consider using of .INT instead",
			"The given value is out of .INT maximum range (-32,768 .. 32,767). Consider using of .LONG instead",
			"Can't declare strings with more than 255 characters using .ASCIC",
			"The given value is out of signed .BYTE maximum range (-128 .. 127). Consider using of .INT instead",
			"Number is too small",
			"Can't define two entry points for the program. Please remove one of the definitions",
			"The entry point specific isn't valid label or address",
			"Can't use negative numbers on .ORG directive",
			".SPACE can't accept negative values",
			"Index Register can't be the same as the base register"
		};

		/// <summary>
		/// Gets string from enum
		/// </summary>
		/// <param name="m">The enum representing the message</param>
		/// <returns>String Message</returns>
		public static string GetMessageText(CompilerMessage m)
		{
			return MessagesArray[(int)m];
		}

	}
}
