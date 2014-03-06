using System;
using System.Text.RegularExpressions;
using System.Collections;

namespace VAX11Compiler
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			try
			{
				/*
					Console.WriteLine("{0}", 
						CompilerMessagesStrings.GetMessageText(
						CompilerMessage.DIVIDE_BY_ZERO));

					OpcodeEntry e = new OpcodeEntry("ADDW3");
					Console.WriteLine("{0:X} {1} {2}", e.OpCode,
						e.NumberOfOperands, e.OpType);
					Console.WriteLine("{0:X}", KnownFunctions.GetAddress("MALLOC"));
					Console.WriteLine("{0}", Registers.GetRegister("SP"));

					SymbolsTable t1 = new SymbolsTable();
					t1.AddEntry(new SymbolsTableEntry("Label1", 100, 
						SymbolsTable.SymbolType.LABEL, 10));
					t1.AddEntry(new SymbolsTableEntry("Label2", 100, 
						SymbolsTable.SymbolType.LABEL, 10));
					Console.WriteLine(t1.SymbolValue("Label1").Value.ToString());
		*/

				/*		CodeBlock b1 = new CodeBlock(3645, 4), b2 = new CodeBlock(15, 4);
						Console.WriteLine("{0:X} {1:X} {2:X} {3:X}", b2[0], b2[1], b2[2], b2[3]);
						CodeBlock b3 = b2 + b1 + b2;
						Console.WriteLine("{0:X} {1:X} {2:X} {3:X}", b3[0], b3[1], b3[2], b3[3]);
						Console.WriteLine("{0}", b3.Size);
			*/
				/*			LinesLocations l = new LinesLocations();
							l.AddEntry(1, 0);
							l.AddEntry(2, 10);
							l.AddEntry(4, 20);
							Console.WriteLine("{0}", l.MaxLine);
							Console.WriteLine("{0}", l.GetStartingAddress(2));*/

				/*		CompilerComment c = new CompilerComment();;
						c.AddEntry(1, CompilerMessage.COMMA_EXPECTED);
						c.AddEntry(2, CompilerMessage.COMPILE_SUCCEED);
						c.AddEntry(3, CompilerMessage.DIVIDE_BY_ZERO);

						for (int i = 0; i < c.comments.Count; ++i)
						{
							Console.WriteLine("{0} {1}", 
								((CompilerCommentEntry)(c.comments[i])).Line,
								((CompilerCommentEntry)(c.comments[i])).msgID);
						}
			*/
				/*	Assembler a = new Assembler();
					string temp1 = "Hi                            You    \nComment#Rules\n#\n     Weeee      Rulezzz #Hiii   \n\n\n";
					a.PreCompiler(temp1);
		*/
				/*			string strData = "Sally      sells sea shells by      the seashore";
							strData = Regex.Replace(strData,@"\s+", " ");
							Console.WriteLine("{0}", strData);
				*/
				/*			Assembler a = new Assembler();
							a.CalcExpression("34+4323-345");
							a.CalcExpression("3GUYF4+43_FJ23-345");
							a.CalcExpression("]3GUYF4+43_FJ23-345");
							a.CalcExpression("3GUYF4+43_FJ23-345]");
				*/
				/*			Pass2List l = new Pass2List();
							l.AddEntry(new Pass2Entry(1, 1, "BLAH", true));
							l.AddEntry(new Pass2Entry(2, 1, "BLAH", true));
							l.AddEntry(new Pass2Entry(3, 5, "BLAH", true));


							foreach (DictionaryEntry myEntry in l.GetPass2List()) 
							{
								Console.WriteLine("{0}", ((Pass2Entry)(myEntry.Value)).Where);
							}
				*/

				/*			Assembler a = new Assembler();
							int len;
							string temp = "";
							while (temp != "exit")
							{
								temp = Console.ReadLine();
								Console.WriteLine(a.CalcExpression(temp, out len));
							}
				
			*/

				/*				int _;
								_ = 5;
								Console.WriteLine(_);
				*/


				/*		Assembler a = new Assembler();
						int len;
						Console.WriteLine(a.ReadNextWord("g7hdGhgtrh ge ee", out len));
		*/
				//int i = int.Parse("A", System.Globalization.NumberStyles.AllowHexSpecifier);
				//Console.WriteLine(i);

				/*				Assembler a = new Assembler();
								int len;
								Console.WriteLine((a.ReadNextNumber("656 ghdhgtrh", out len)).ToString());
								Console.WriteLine((a.ReadNextNumber("789", out len)).ToString());
								Console.WriteLine((a.ReadNextNumber("789,", out len)).ToString());
								Console.WriteLine((a.ReadNextNumber("0xff", out len)).ToString());
								Console.WriteLine((a.ReadNextNumber("0x10", out len)).ToString());
								Console.WriteLine((a.ReadNextNumber("789&", out len)).ToString());
				*/

				/*			Assembler a = new Assembler();
							int len;
							Console.WriteLine("{0} : {1}", 
								a.ReadNextChar("      abc ", out len),
								len);
							Console.WriteLine("{0} : {1}", 
								a.ReadNextChar("      ", out len),
								len);*/

				/*		Assembler a = new Assembler();
						int len;
						Console.WriteLine(a.AnalyzeString(@"""Hello"", World""", out len));
				*/
		
				/*		Assembler a = new Assembler();
						int len;
						a.AnalyzeDirective("set HELL , 10", out len, 7);
						a.AnalyzeDirective("set HELL2 , 10*20", out len, 7);
						CodeBlock t = a.AnalyzeDirective("word 0x12*0x20", out len, 7);
						//CodeBlock t = a.AnalyzeDirective("byte 0x12*0x20", out len, 7);
						for (int i = 0; i < t.Size; ++i) Console.Write("{0} ", t[i]);
						Console.WriteLine("");
						CodeBlock t2 = a.AnalyzeDirective("word 0x12*HELL", out len, 7);
						for (int i = 0; i < t2.Size; ++i) Console.Write("{0} ", t2[i]);
						Console.WriteLine("");
						CodeBlock t3 = a.AnalyzeDirective("asciz \"abcd\"", out len, 7);
						for (int i = 0; i < t3.Size; ++i) Console.Write("{0} ", t3[i]);
						Console.WriteLine("");
						CodeBlock t4 = a.AnalyzeDirective("ascic \"abcd\"", out len, 7);
						for (int i = 0; i < t4.Size; ++i) Console.Write("{0} ", t4[i]);
						Console.WriteLine("");
						CodeBlock t5 = a.AnalyzeDirective("space 7", out len, 7);
						for (int i = 0; i < t5.Size; ++i) Console.Write("{0} ", t5[i]);
						Console.WriteLine("");
	
						CodeBlock t6 = a.AnalyzeDirective("byte 257, 8, 9", out len, 7);
						for (int i = 0; i < t6.Size; ++i) Console.Write("{0} ", t6[i]);
						Console.WriteLine("");			

		*/
				/*		Assembler a = new Assembler();
						int len;
						a.AnalyzeDirective("set HELL , 10", out len, 7);
						Console.WriteLine(a.SolveLabel("3563", 4));
						Console.WriteLine(a.SolveLabel("HELL", 4));
						Console.WriteLine(a.SolveLabel("0x45", 4));
						*/

				/*	string s = "(r3)+)(r12)";
					string m = (Regex.Match(s, @"\(r([0-9]|1[0-5])\)")).Value;
					Console.WriteLine(m);*/
	/*			
				string s = "r12     ,   (r30), r13  ";
				string s_1 = "r12";
				Console.WriteLine(Regex.Match(s, @"[^,(\s)]+(\s)*,*(\s)*").Value + "!");
				Console.WriteLine(Regex.Match(s_1, @"[^,(\s)]+(\s)*,*(\s)*").Value + "!");
				string s_2 = Regex.Match(s, @"[^,(\s)]+(\s)*,*(\s)*").Value;

				Console.WriteLine(Regex.Match(s_2, @"[^,(\s)]+").Value + "###");
			

				string s1 = "r1";
				string sRes1 = (Regex.Match(s1.ToUpper(),@"^[rR]((1[0-5])|([0-9]))(\s)*,*(\s)*")).Value;
//				string sRes1 = (Regex.Match(s1.ToUpper(), @"^(([rR]((1[0-5])|([0-9])))|AP|SP|PC|FP)(\s)*?,?(\s)*")).Value;
                Console.WriteLine(sRes1 + "!");
				string sRes2 = (Regex.Match(s1.ToUpper(), @"^(([rR]((1[0-5])|([0-9])))|AP|SP|PC|FP)")).Value;
				Console.WriteLine(sRes2 + "!");			


				string s2 = "(r1), (r2)";
				string sRes3 = (Regex.Match(s2.ToUpper(), 
					@"^[\-\*]?\((([rR]((1[0-5])|([0-9])))|AP|SP|PC|FP)\)(\+)?((\[)|(hhh))")).Value;
				Console.WriteLine(sRes3 + "!");	

				Console.WriteLine("-----------------------");
*/
				/*
								string s1 = "hi", sRes1;
								while (s1 != "exit")
								{
									s1 = Console.ReadLine();
									sRes1 = (Regex.Match(s1.ToUpper(),@"^(([rR]((1[0-5]$)|([0-9]$)))|PC|SP|AP|FP)")).Value;
									Console.WriteLine(sRes1 + "!");
								}
				*/
/*
				Assembler a = new Assembler();
				int len;
				a.AnalyzeDirective("set HELL , 10", out len, 7);
				CodeBlock t1 = a.FetchOperand("$34", "r", 2, 7, false);
				for (int i = 0; i < t1.Size; ++i) Console.Write("0x{0:X} ", t1[i]);
				Console.WriteLine("");	
				CodeBlock t2 = a.FetchOperand("$0xFFF", "r", 2, 7, false);
				for (int i = 0; i < t2.Size; ++i) Console.Write("0x{0:X} ", t2[i]);
				Console.WriteLine("");
				CodeBlock t3 = a.FetchOperand("r14", "r", 2, 7, false);
				for (int i = 0; i < t3.Size; ++i) Console.Write("0x{0:X} ", t3[i]);
				Console.WriteLine("");
				CodeBlock t4 = a.FetchOperand("pC", "r", 2, 7, false);
				for (int i = 0; i < t4.Size; ++i) Console.Write("0x{0:X} ", t4[i]);
				Console.WriteLine("");
				CodeBlock t5 = a.FetchOperand("$HELL", "r", 2, 7, false);
				for (int i = 0; i < t5.Size; ++i) Console.Write("0x{0:X} ", t5[i]);
				Console.WriteLine("");
				CodeBlock t6 = a.FetchOperand(@"(r11)", "r", 2, 7, false);
				for (int i = 0; i < t6.Size; ++i) Console.Write("0x{0:X} ", t6[i]);
				Console.WriteLine("");
				CodeBlock t7 = a.FetchOperand(@"(r3)+", "r", 2, 7, false);
				for (int i = 0; i < t7.Size; ++i) Console.Write("0x{0:X} ", t7[i]);
				Console.WriteLine("");
				CodeBlock t8 = a.FetchOperand(@"*(r3)+[r6]", "r", 2, 7, false);
				for (int i = 0; i < t8.Size; ++i) Console.Write("0x{0:X} ", t8[i]);
				Console.WriteLine("");
*/
	/*			string sValue = @"(r11)   , ";
				string res = Regex.Match(sValue, @"[^,(\s)]+(\s)*,*(\s)*", RegexOptions.Compiled).Value;
				Console.WriteLine(res);

				// @"^[\-\*]?\((([rR]((1[0-5])|([0-9])))|PC|SP|AP|FP)\)(\+)?(\[)?$"

/*				string s1 = "hi", sRes1;

				while (s1 != "exit")
				{
					s1 = Console.ReadLine();
					sRes1 = (Regex.Match(s1.ToUpper(),@"^[\-\*]?\((([rR]((1[0-5])|([0-9])))|PC|SP|AP|FP)\)(\+)?(\[(([rR]((1[0-5])|([0-9])))|PC|SP|AP|FP)\])?$")).Value;
					Console.WriteLine(sRes1 + "!");
				}
*/
/*				string s = "   (Hello)    ";
				s = s.Trim();
				Console.WriteLine(s);
				CodeBlock b = new CodeBlock();
				b = 0xDDEEFF;
				for (int i = 0; i < b.Size; ++i) Console.Write("0x{0:X} ", b[i]);
				Console.WriteLine("");
*/
/*				Assembler a = new Assembler();
				int len;

				CodeBlock t1 = a.AnalyzeCommand("movl (r1), r2", 10);
				for (int i = 0; i < t1.Size; ++i) Console.Write("0x{0:X} ", t1[i]);
				Console.WriteLine("");	

				a.AnalyzeDirective("set CON1 , 78", out len, 7);
				CodeBlock t2 = a.AnalyzeCommand("movl $CON1, (r2)+", 10);
				for (int i = 0; i < t2.Size; ++i) Console.Write("0x{0:X} ", t2[i]);
				Console.WriteLine("");

				CodeBlock t3 = a.AnalyzeCommand("addl3 $CON1, $4,*(R2)+", 10);
				for (int i = 0; i < t3.Size; ++i) Console.Write("0x{0:X} ", t3[i]);
				Console.WriteLine("");

				CodeBlock t4 = a.AnalyzeCommand("NOP", 10);
				for (int i = 0; i < t4.Size; ++i) Console.Write("0x{0:X} ", t4[i]);
				Console.WriteLine("");

				t4 = a.AnalyzeCommand("NOP", 10);
				for (int i = 0; i < t4.Size; ++i) Console.Write("0x{0:X} ", t4[i]);
				Console.WriteLine("");

				CodeBlock t3 = a.AnalyzeCommand("add2b $CON1, *(R2)+", out len, 10);
				for (int i = 0; i < t3.Size; ++i) Console.Write("0x{0:X} ", t3[i]);
				Console.WriteLine("");
				*/
				Assembler a = new Assembler();
				string[] sCode = { 
									 ".text",
									 "movb r1, r2",
									 ".set MEEEEEE,0x30+0x5e",
									 "BPT",
									 "NOP",
									 "CMPL $MEEEEEE, (r2)+",
									 "JEQL (r2)",
									 "HALT",
				};
				
				CodeBlock progrie = a.DoPass1(sCode);

				for (int i = 0; i < progrie.Size; ++i) Console.Write("0x{0:X} ", progrie[i]);
				Console.WriteLine("");

			}
			catch (CompileError e)
			{
				Console.WriteLine("Program crashed. Error: {0}", e.ErrorCode);
			}
		}
	}
}
