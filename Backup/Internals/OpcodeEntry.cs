using System;
using System.Collections;

using VAX11Compiler;
using VAX11Simulator;

namespace VAX11Internals
{
	/// <summary>
	/// OpcodeEntry contains information about an opcode.
	/// </summary>
	class OpcodeEntry
	{
		#region Internal Class - OpCodeData

		/// <summary>
		/// Private class that contains opcode information
		/// </summary>
		private class OpCodeData : IComparable
		{
			#region Public Variables
			
			/// <summary>
			/// Opcode Name
			/// </summary>
			public string OpName;

			/// <summary>
			/// Number of operands
			/// </summary>
			public int NumberOfOperands;

			/// <summary>
			/// Operands type. Syntax of this field is located in the documentation
			/// </summary>
			public string OpType;

			/// <summary>
			/// Opcode value
			/// </summary>
			public int OpCode;

			/// <summary>
			/// Implementation function for the opcode
			/// </summary>
			public VAX11Opcodes.OpcodeImplementation ImplementationFunction;


			/// <summary>
			/// Number of cycles per instruction
			/// </summary>
			public int Cycles;

			/// <summary>
			/// Help URL for the opcode
			/// </summary>
			public string HelpEntry;

			#endregion

			#region Construction

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="sOpName">Opcode Name</param>
			/// <param name="iNumberOfOperands">Number of operands for the current command</param>
			/// <param name="sOpType">Information about the operands. See documentation for more info</param>
			/// <param name="iOpCode">The opcode value</param>
			/// <param name="impFunc">Implemantation function for the commands</param>
			/// <param name="sHelpEntry">Help URL for the opcode</param>
			public OpCodeData(string sOpName, int iNumberOfOperands,
				string sOpType, int iOpCode, VAX11Opcodes.OpcodeImplementation impFunc, int iCycles, string sHelpEntry)
			{
				OpName = sOpName;
				OpCode = iOpCode;
				NumberOfOperands = iNumberOfOperands;
				OpType = sOpType;
				ImplementationFunction = impFunc;
				Cycles = iCycles;
				HelpEntry = sHelpEntry;
			}

			#endregion

			#region Methods

			/// <summary>
			/// Compare 2 opcodes. Compare is by name. It used by the hashtable
			/// </summary>
			/// <param name="o">OpCodeData to compare to</param>
			/// <returns>int contains the compare result.</returns>
			public int CompareTo(object o)
			{
				return String.Compare(OpName, ((OpCodeData)o).OpName);
			}

			#endregion
		}

		#endregion

		#region Opcodes Data

		private static OpCodeData[] opData = 
			{
				new OpCodeData("ACBB"  , 4, "r1r1w1b1"    , 0x9D, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.acb), 4, "op_ACB"),
				new OpCodeData("ACBL"  , 4, "r4r4w4b2"    , 0xF1, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.acb), 4, "op_ACB"),
				new OpCodeData("ACBW"  , 4, "r2r2w2b2"    , 0x3D, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.acb), 4, "op_ACB"),
				new OpCodeData("ADAWI" , 2, "r2m2"		  , 0x58, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.adawi), 1, "op_ADAWI"),
				new OpCodeData("ADDB2" , 2, "r1m1"        , 0x80, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.add), 1, "op_ADD"),
				new OpCodeData("ADDB3" , 3, "r1r1m1"      , 0x81, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.add), 1, "op_ADD"),
				new OpCodeData("ADDL2" , 2, "r4m4"        , 0xC0, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.add), 1, "op_ADD"),
				new OpCodeData("ADDL3" , 3, "r4r4m4"      , 0xC1, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.add), 1, "op_ADD"),
				new OpCodeData("ADDW2" , 2, "r2m2"        , 0xA0, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.add), 1, "op_ADD"),
				new OpCodeData("ADDW3" , 3, "r2r2m2"      , 0xA1, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.add), 1, "op_ADD"),
				new OpCodeData("ADWC"  , 2, "r4m4"        , 0xD8, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.adwc), 1, "op_ADWC"),
				new OpCodeData("AOBLEQ", 3, "r4w4b1"      , 0xF3, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.aob), 4, "op_AOB"),
				new OpCodeData("AOBLSS", 3, "r4w4b1"      , 0xF2, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.aob), 4, "op_AOB"),
				new OpCodeData("ASHL"  , 3, "r1r4w4"      , 0x78, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.ash), 3, "op_ASH"),
				new OpCodeData("ASHQ"  , 3, "r1r8w8"      , 0x79, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.ash), 3, "op_ASH"),
				new OpCodeData("BBC"   , 3, "r4v1b1"      , 0xE1, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bb), 3, "op_BB"),
				new OpCodeData("BBCC"  , 3, "r4v1b1"      , 0xE5, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bb2), 3, "op_BB"),
				new OpCodeData("BBCCI" , 3, "r4v1b1"      , 0xE7, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bb2), 3, "op_BB"),
				new OpCodeData("BBCS"  , 3, "r4v1b1"      , 0xE3, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bb2), 3, "op_BB"),
				new OpCodeData("BBS"   , 3, "r4v1b1"      , 0xE0, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bb), 3, "op_BB"),
				new OpCodeData("BBSC"  , 3, "r4v1b1"      , 0xE4, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bb2), 3, "op_BB"),
				new OpCodeData("BBSS"  , 3, "r4v1b1"      , 0xE2, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bb2), 3, "op_BB"),
				new OpCodeData("BBSSI" , 3, "r4v1b1"      , 0xE6, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bb2), 3, "op_BB"),
				new OpCodeData("BCC"   , 1, "b1"          , 0x1E, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("BCS"   , 1, "b1"          , 0x1F, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("BEQL"  , 1, "b1"          , 0x13, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("BEQLU" , 1, "b1"          , 0x13, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("BGEQ"  , 1, "b1"          , 0x18, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("BGEQU" , 1, "b1"          , 0x1E, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("BGTR"  , 1, "b1"          , 0x14, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("BGTRU" , 1, "b1"          , 0x1A, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("BICB2" , 2, "r1w1"        , 0x8A, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bic2), 1, "op_BIC"),
				new OpCodeData("BICB3" , 3, "r1r1w1"      , 0x8B, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bic3), 1, "op_BIC"),
				new OpCodeData("BICL2" , 2, "r4w4"        , 0xCA, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bic2), 1, "op_BIC"),
				new OpCodeData("BICL3" , 3, "r4r4w4"      , 0xCB, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bic3), 1, "op_BIC"),
				new OpCodeData("BICPSW", 1, "r2"          , 0xB9, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bicpsw), 1, "op_BICPSW"),
				new OpCodeData("BICW2" , 2, "r2w2"        , 0xAA, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bic2), 1, "op_BIC"),
				new OpCodeData("BICW3" , 3, "r2r2w2"      , 0xAB, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bic3), 1, "op_BIC"),
				new OpCodeData("BISB2" , 2, "r1w1"        , 0x88, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bis2), 1, "op_BIS"),
				new OpCodeData("BISB3" , 3, "r1r1w1"      , 0x89, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bis3), 1, "op_BIS"),
				new OpCodeData("BISL2" , 2, "r4w4"        , 0xC8, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bis2), 1, "op_BIS"),
				new OpCodeData("BISL3" , 3, "r4r4w4"      , 0xC9, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bis3), 1, "op_BIS"),
				new OpCodeData("BISPSW", 1, "r2"          , 0xB8, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bispsw), 1, "op_BISPSW"),
				new OpCodeData("BISW2" , 2, "r2w2"        , 0xA8, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bis2), 1, "op_BIS"),
				new OpCodeData("BISW3" , 3, "r2r2w2"      , 0xA9, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bis3), 1, "op_BIS"),
				new OpCodeData("BITB"  , 2, "r1r1"        , 0x93, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bit), 1, "op_BIT"),
				new OpCodeData("BITL"  , 2, "r4r4"        , 0xD3, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bit), 1, "op_BIT"),
				new OpCodeData("BITW"  , 2, "r2r2"        , 0xB3, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bit), 1, "op_BIT"),
				new OpCodeData("BLBC"  , 2, "r4b1"        , 0xE9, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.blb), 2, "op_BLB"),
				new OpCodeData("BLBS"  , 2, "r4b1"        , 0xE8, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.blb), 2, "op_BLB"),
				new OpCodeData("BLEQ"  , 1, "b1"          , 0x15, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("BLEQU" , 1, "b1"          , 0x1B, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("BLSS"  , 1, "b1"          , 0x19, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("BLSSU" , 1, "b1"          , 0x1F, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("BNEQ"  , 1, "b1"          , 0x12, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("BNEQU" , 1, "b1"          , 0x12, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("BPT"   , 0, ""            , 0x03, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.bpt), 7, "op_BPT"),
				new OpCodeData("BRB"   , 1, "b1"          , 0x11, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.br), 3, "op_BR_JMP"),
				new OpCodeData("BRW"   , 1, "b2"          , 0x31, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.br), 3, "op_BR_JMP"),
				new OpCodeData("BSBB"  , 1, "b1"          , 0x10, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.jsb), 4, "op_BSB"),
				new OpCodeData("BSBW"  , 1, "b2"          , 0x30, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.jsb), 4, "op_BSB"),
				new OpCodeData("BVC"   , 1, "b1"          , 0x1C, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("BVS"   , 1, "b1"          , 0x1D, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.b), 3, "op_B"),
				new OpCodeData("CALLG" , 2, "a1a4"        , 0xFA, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.callg), 6, "op_CALLG"),
				new OpCodeData("CALLS" , 2, "r1a4"        , 0xFB, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.calls), 6, "op_CALLS"),
				new OpCodeData("CASEB" , 3, "r1r1r1"      , 0x8F, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.case2), 7, "op_CASE"),
				new OpCodeData("CASEL" , 3, "r4r4r4"      , 0xCF, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.case2), 7, "op_CASE"),
				new OpCodeData("CASEW" , 3, "r2r2r2"      , 0xAF, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.case2), 7, "op_CASE"),
				new OpCodeData("CLRB"  , 1, "w1"          , 0x94, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.clr), 1, "op_CLR"),
				new OpCodeData("CLRL"  , 1, "w4"          , 0xD4, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.clr), 1, "op_CLR"),
				new OpCodeData("CLRQ"  , 1, "w8"          , 0x7C, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.clr), 1, "op_CLR"),
				new OpCodeData("CLRW"  , 1, "w2"          , 0xB4, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.clr), 1, "op_CLR"),
				new OpCodeData("CMPB"  , 2, "r1r1"        , 0x91, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.cmp), 1, "op_CMP"),
				new OpCodeData("CMPC3" , 3, "r2a1a1"      , 0x29, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.cmpc3), 1, "op_CMPC"),
				new OpCodeData("CMPC5" , 5, "r2a1r1r2a1"  , 0x2D, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.cmpc5), 1, "op_CMPC"),
				new OpCodeData("CMPL"  , 2, "r4r4"        , 0xD1, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.cmp), 1, "op_CMP"),
				new OpCodeData("CMPW"  , 2, "r2r2"        , 0xB1, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.cmp), 1, "op_CMP"),
				new OpCodeData("CVTBL" , 2, "r1w4"        , 0x98, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.cvt), 2, "op_CVT"),
				new OpCodeData("CVTBW" , 2, "r1w2"        , 0x99, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.cvt), 2, "op_CVT"),
				new OpCodeData("CVTLB" , 2, "r4w1"        , 0xF6, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.cvt), 2, "op_CVT"),
				new OpCodeData("CVTLW" , 2, "r4w2"        , 0xF7, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.cvt), 2, "op_CVT"),
				new OpCodeData("CVTWB" , 2, "r2w1"        , 0x33, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.cvt), 2, "op_CVT"),
				new OpCodeData("CVTWL" , 2, "r2w4"        , 0x32, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.cvt), 2, "op_CVT"),
				new OpCodeData("DECB"  , 1, "m1"          , 0x97, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.dec), 1, "op_DEC"),
				new OpCodeData("DECL"  , 1, "m4"          , 0xD7, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.dec), 1, "op_DEC"),
				new OpCodeData("DECW"  , 1, "m2"          , 0xB7, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.dec), 1, "op_DEC"),
				new OpCodeData("DIVB2" , 2, "r1w1"        , 0x86, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.div), 31, "op_DIV"),
				new OpCodeData("DIVB3" , 3, "r1r1w1"      , 0x87, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.div), 31, "op_DIV"),
				new OpCodeData("DIVL2" , 2, "r4w4"        , 0xC6, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.div), 31, "op_DIV"),
				new OpCodeData("DIVL3" , 3, "r4r4w4"      , 0xC7, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.div), 31, "op_DIV"),
				new OpCodeData("DIVW2" , 2, "r2w2"        , 0xA6, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.div), 31, "op_DIV"),
				new OpCodeData("DIVW3" , 3, "r2r2w2"      , 0xA7, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.div), 31, "op_DIV"),
				new OpCodeData("EDIV"  , 4, "r4r8w4w4"    , 0x7B, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.ediv), 31, "op_EDIV"),
				new OpCodeData("HALT"  , 0, ""            , 0x00, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.halt), 1, "op_HALT"),
				new OpCodeData("INCB"  , 1, "m1"          , 0x96, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.inc), 1, "op_INC"),
				new OpCodeData("INCL"  , 1, "m4"          , 0xD6, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.inc), 1, "op_INC"),
				new OpCodeData("INCW"  , 1, "m2"          , 0xB6, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.inc), 1, "op_INC"),
				new OpCodeData("INDEX" , 6, "r4r4r4r4r4w4", 0x0A, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.index), 3, "op_INDEX"),
				new OpCodeData("INSQUE", 2, "a1a1"        , 0x0E, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.insque), 9, "op_INSQUE"),
				new OpCodeData("JCC"   , 1, "b2"          , 0x31031F, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JCS"   , 1, "b2"          , 0x31031E, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JEQL"  , 1, "b2"          , 0x310312, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JEQLU" , 1, "b2"          , 0x310312, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JGEQ"  , 1, "b2"          , 0x310319, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JGEQU" , 1, "b2"          , 0x31031F, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JGTR"  , 1, "b2"          , 0x310315, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JGTRU" , 1, "b2"          , 0x31031B, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JLBC"  , 2, "r4b2"        , 0x3103E8, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JLBS"  , 2, "r4b2"        , 0x3103E9, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JLEQ"  , 1, "b2"          , 0x310314, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JLEQU" , 1, "b2"          , 0x31031A, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JLSS"  , 1, "b2"          , 0x310318, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JLSSU" , 1, "b2"          , 0x31031E, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JMP"   , 1, "a4"          , 0x17, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.br), 4, "op_BR_JMP"),
				new OpCodeData("JNEQ"  , 1, "b2"          , 0x310313, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JNEQU" , 1, "b2"          , 0x310313, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JSB"   , 1, "a1"          , 0x16, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.jsb), 4, "op_BSB"),
				new OpCodeData("JVC"   , 1, "b2"          , 0x31031D, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("JVS"   , 1, "b2"          , 0x31031C, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.MacroOpcode), 0, "NO_HELP"),
				new OpCodeData("LOCC"  , 3, "r1r2a1"      , 0x3A, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.locc_skpc), 6, "op_LOCC"),
				new OpCodeData("MATCHC", 4, "r2a1r2a1"    , 0x39, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.matchc), 7, "NO_HELP"),
				new OpCodeData("MCOMB" , 2, "r1w1"        , 0x92, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mcom), 2, "op_MCOM"),
				new OpCodeData("MCOML" , 2, "r4w4"        , 0xD2, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mcom), 2, "op_MCOM"),
				new OpCodeData("MCOMW" , 2, "r2w2"        , 0xB2, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mcom), 2, "op_MCOM"),
				new OpCodeData("MFPR"  , 2, "p4w4"        , 0xDB, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mfpr), 1, "topic_Interrupts"),
				new OpCodeData("MNEGB" , 2, "r1w1"        , 0x8E, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mneg), 1, "op_MNEG"),
				new OpCodeData("MNEGL" , 2, "r4w4"        , 0xCE, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mneg), 1, "op_MNEG"),
				new OpCodeData("MNEGW" , 2, "r2w2"        , 0xAE, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mneg), 1, "op_MNEG"),
				new OpCodeData("MOVAB" , 2, "a1w4"        , 0x9E, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mova), 1, "op_MovaAndPusha"),
				new OpCodeData("MOVAL" , 2, "a4w4"        , 0xDE, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mova), 1, "op_MovaAndPusha"),
				new OpCodeData("MOVAQ" , 2, "a8w4"        , 0x7E, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mova), 1, "op_MovaAndPusha"),
				new OpCodeData("MOVAW" , 2, "a2w4"        , 0x3E, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mova), 1, "op_MovaAndPusha"),
				new OpCodeData("MOVB"  , 2, "r1w1"        , 0x90, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mov), 1, "op_MOV"),
				new OpCodeData("MOVC3" , 3, "r2a1a1"      , 0x28, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.movc3), 1, "op_MOVC"),
				new OpCodeData("MOVC5" , 5, "r2a1r1r2a1"  , 0x2C, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.movc5), 1, "op_MOVC"),
				new OpCodeData("MOVL"  , 2, "r4w4"        , 0xD0, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mov), 1, "op_MOV"),
				new OpCodeData("MOVPSL", 1, "w4"          , 0xDC, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.movpsl), 1, "op_MOVPSL"),
				new OpCodeData("MOVQ"  , 2, "r8w8"        , 0x7D, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mov), 1, "op_MOV"),
				new OpCodeData("MOVTC" , 6, "r2a1r1a1r2a1", 0x2E, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.movtc), 1, "op_MOVTC"),
				new OpCodeData("MOVTUC", 6, "r2a1r1a1r2a1", 0x2F, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.movtuc), 1, "op_MOVTUC"),
				new OpCodeData("MOVW"  , 2, "r2w2"        , 0xB0, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mov), 1, "op_MOV"),
				new OpCodeData("MOVZBL", 2, "r1w4"        , 0x9A, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.movz), 1, "op_MOVZ"),
				new OpCodeData("MOVZBW", 2, "r1w2"        , 0x9B, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.movz), 1, "op_MOVZ"),
				new OpCodeData("MOVZWL", 2, "r2w4"        , 0x3C, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.movz), 1, "op_MOVZ"),
				new OpCodeData("MTPR"  , 2, "r4p4"        , 0xDA, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mtpr), 1, "topic_Interrupts"),
				new OpCodeData("MULB2" , 2, "r1w1"        , 0x84, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mul2), 11, "op_MUL"),
				new OpCodeData("MULB3" , 3, "r1r1w1"      , 0x85, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mul3), 11, "op_MUL"),
				new OpCodeData("MULL2" , 2, "r4w4"        , 0xC4, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mul2), 11, "op_MUL"),
				new OpCodeData("MULL3" , 3, "r4r4w4"      , 0xC5, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mul3), 11, "op_MUL"),
				new OpCodeData("MULW2" , 2, "r2r2"        , 0xA4, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mul2), 11, "op_MUL"),
				new OpCodeData("MULW3" , 3, "r2r2w2"      , 0xA5, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.mul3), 11, "op_MUL"),
				new OpCodeData("NOP"   , 0, ""            , 0x01, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.nop), 1, "NO_HELP"),
				new OpCodeData("POPR"  , 1, "r2"          , 0xBA, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.popr), 10, "op_POPR"),
				new OpCodeData("PUSHAB", 1, "a1"          , 0x9F, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.pusha), 3, "op_MovaAndPusha"),
				new OpCodeData("PUSHAL", 1, "a4"          , 0xDF, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.pusha), 3, "op_MovaAndPusha"),
				new OpCodeData("PUSHAQ", 1, "a8"          , 0x7F, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.pusha), 3, "op_MovaAndPusha"),
				new OpCodeData("PUSHAW", 1, "a2"          , 0x3F, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.pusha), 3, "op_MovaAndPusha"),
				new OpCodeData("PUSHL" , 1, "r4"          , 0xDD, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.push), 3, "op_PUSHL"),
				new OpCodeData("PUSHR" , 1, "r2"          , 0xBB, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.pushr), 10, "op_PUSHR"),
				new OpCodeData("REI"   , 0, ""            , 0x02, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.rei), 5, "op_REI"),
				new OpCodeData("REMQUE", 2, "a1w4"        , 0x0F, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.remque), 7, "op_REMQUE"),
				new OpCodeData("RET"   , 0, ""            , 0x04, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.ret), 4, "op_RET"),
				new OpCodeData("ROTL"  , 3, "r1r4w4"      , 0x9C, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.rotl), 3, "op_ROTL"),
				new OpCodeData("RSB"   , 0, ""            , 0x05, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.rsb), 3, "op_RSB"),
				new OpCodeData("SBWC"  , 2, "r4w4"        , 0xD9, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.sbwc), 2, "op_SBWC"),
				new OpCodeData("SCANC" , 4, "r2a1a1r1"    , 0x2A, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.scanc_spanc), 7, "op_SCANC_SPANC"),
				new OpCodeData("SKPC"  , 3, "r1r2a1"      , 0x3B, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.locc_skpc), 6, "op_LOCC"),
				new OpCodeData("SOBGEQ", 2, "w4b1"        , 0xF4, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.sob), 4, "op_SOB"),
				new OpCodeData("SOBGTR", 2, "w4b1"        , 0xF5, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.sob), 4, "op_SOB"),
				new OpCodeData("SPANC" , 4, "r2a1a1r1"    , 0x2B, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.scanc_spanc), 6, "op_SCANC_SPANC"),
				new OpCodeData("SUBB2" , 2, "r1w1"        , 0x82, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.sub), 2, "op_SUB"),
				new OpCodeData("SUBB3" , 3, "r1r1w1"      , 0x83, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.sub), 2, "op_SUB"),
				new OpCodeData("SUBL2" , 2, "r4w4"        , 0xC2, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.sub), 2, "op_SUB"),
				new OpCodeData("SUBL3" , 3, "r4r4w4"      , 0xC3, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.sub), 2, "op_SUB"),
				new OpCodeData("SUBW2" , 2, "r2w2"        , 0xA2, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.sub), 2, "op_SUB"),
				new OpCodeData("SUBW3" , 3, "r2r2w2"      , 0xA3, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.sub), 2, "op_SUB"),
				new OpCodeData("TSTB"  , 1, "r1"          , 0x95, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.tst), 1, "op_TST"),
				new OpCodeData("TSTL"  , 1, "r4"          , 0xD5, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.tst), 1, "op_TST"),
				new OpCodeData("TSTW"  , 1, "r2"          , 0xB5, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.tst), 1, "op_TST"),
				new OpCodeData("XORB2" , 2, "r1w1"        , 0x8C, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.xor2), 1, "op_XOR"),
				new OpCodeData("XORB3" , 3, "r1r1w1"      , 0x8D, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.xor3), 1, "op_XOR"),
				new OpCodeData("XORL2" , 2, "r4w4"        , 0xCC, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.xor2), 1, "op_XOR"),
				new OpCodeData("XORL3" , 3, "r4r4w4"      , 0xCD, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.xor3), 1, "op_XOR"),
				new OpCodeData("XORW2" , 2, "r2w2"        , 0xAC, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.xor2), 1, "op_XOR"),
				new OpCodeData("XORW3" , 3, "r2r2w2"      , 0xAD, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.xor3), 1, "op_XOR"),
				new OpCodeData("GAL_ZION" , 0, ""      , 0xFFFF, new VAX11Opcodes.OpcodeImplementation(VAX11Opcodes.UnsupportedOpcode), 1, "LOL")
			};

		#endregion

		#region Members

		/// <summary>
		/// Hashtable contains all the opcodes using opcode string as key
		/// </summary>
		private static Hashtable OpcodeHashTable = new Hashtable();

		/// <summary>
		/// Hashtable contains all the opcodes using opcode value as key
		/// </summary>
		private static Hashtable OpcodeValuesHashTable = new Hashtable();

		/// <summary>
		/// OpCode value
		/// </summary>
		private readonly int _OpCode;

		/// <summary>
		/// Number of operands
		/// </summary>
		private readonly int _NumberOfOperands;

		/// <summary>
		/// Information about the operands. See documentation for more info
		/// </summary>
		private readonly string _OpType;

		/// <summary>
		/// Implemantation function for the opcode
		/// </summary>
		private readonly VAX11Opcodes.OpcodeImplementation _impFunc;

		/// <summary>
		/// Help URL for current opcode.
		/// </summary>
		private readonly string _HelpEntry;

		/// <summary>
		/// Clock Cycles for the opcode.
		/// </summary>
		private readonly int _Cycles;

		#endregion

		#region Methods

		/// <summary>
		/// Returns true if the given command is privileged command
		/// </summary>
		/// <param name="sStr">The command</param>
		/// <returns>true/false</returns>
		public static bool IsPrivilegedCommand(string sStr)
		{
			return (sStr.ToUpper() == "MFPR" || sStr.ToUpper() == "MTPR") ? true : false;
		}

		#endregion

		#region Constructors

		/// <summary>
		/// initalize the data structure
		/// </summary>
		static OpcodeEntry()
		{
			for (int iIndex = 0; iIndex < opData.Length; ++iIndex)
			{
				OpcodeHashTable[opData[iIndex].OpName] = opData[iIndex];
				OpcodeValuesHashTable[opData[iIndex].OpCode] = opData[iIndex];
			}
		}

		/// <summary>
		/// Gets opcode name and returns all the information about it
		/// </summary>
		/// <param name="OpcodeName">Opcode Name</param>
		public OpcodeEntry(string OpcodeName)
		{
			OpcodeName = OpcodeName.ToUpper();
			if (OpcodeHashTable.ContainsKey(OpcodeName) == false) 
				throw new CompileError(CompilerMessage.UNRECOGNIZED_OPCODE_NAME);
			_OpCode = ((OpCodeData)OpcodeHashTable[OpcodeName]).OpCode;
			_NumberOfOperands = ((OpCodeData)OpcodeHashTable[OpcodeName]).NumberOfOperands;
			_OpType = ((OpCodeData)OpcodeHashTable[OpcodeName]).OpType;
			_impFunc = ((OpCodeData)OpcodeHashTable[OpcodeName]).ImplementationFunction;
			_HelpEntry = ((OpCodeData)OpcodeHashTable[OpcodeName]).HelpEntry;
			_Cycles = ((OpCodeData)OpcodeHashTable[OpcodeName]).Cycles;
		}


		/// <summary>
		/// Gets opcode value and returns all the information about it
		/// </summary>
		/// <param name="OpcodeValue">Opcode Value</param>
		/// <param name="iPC">Current PC, in case we throw runtime error</param>
		public OpcodeEntry(int OpcodeValue, int iPC)
		{
			if (OpcodeValuesHashTable.ContainsKey(OpcodeValue) == false) 
				throw new RuntimeError(SimulatorMessage.UNRECOGNIZED_OPCODE_NAME, iPC);
			_OpCode = ((OpCodeData)OpcodeValuesHashTable[OpcodeValue]).OpCode;
			_NumberOfOperands = ((OpCodeData)OpcodeValuesHashTable[OpcodeValue]).NumberOfOperands;
			_OpType = ((OpCodeData)OpcodeValuesHashTable[OpcodeValue]).OpType;
			_impFunc = ((OpCodeData)OpcodeValuesHashTable[OpcodeValue]).ImplementationFunction;
			_HelpEntry = ((OpCodeData)OpcodeValuesHashTable[OpcodeValue]).HelpEntry;
			_Cycles = ((OpCodeData)OpcodeValuesHashTable[OpcodeValue]).Cycles;
		}

		#endregion

		#region Properties

		/// <summary>
		/// OpCode value
		/// </summary>
		public int OpCode
		{
			get
			{
				return _OpCode;
			}
		}

		/// <summary>
		/// Number of operands
		/// </summary>
		public int NumberOfOperands
		{
			get
			{
				return _NumberOfOperands;
			}
		}
		
		/// <summary>
		/// Information about the operands. See documentation for more info
		/// </summary>
		public string OpType
		{
			get
			{
				return _OpType;
			}
		}
	

		public VAX11Opcodes.OpcodeImplementation ImplementationFunction
		{
			get
			{
				return _impFunc;
			}
		}

		public string HelpURL
		{
			get
			{
				return _HelpEntry;
			}
		}


		public int Cycles
		{
			get
			{
				return _Cycles;
			}
		}

		#endregion
	}
}
