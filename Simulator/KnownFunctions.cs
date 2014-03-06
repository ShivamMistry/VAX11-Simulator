using System;
using VAX11Internals;
using System.Text.RegularExpressions;

namespace VAX11Simulator
{
	/// <summary>
	/// This class contains the simulator functions
	/// </summary>
	public class KnownFunctions
	{

		/// <summary>
		/// this function read chars from memory and return the string that end with `\0`
		/// </summary>
		/// <param name="address">the start address of the string</param>
		/// <returns>the string</returns>
		private static string ReadString (Simulator sim, int address)
		{
			string str = "";
			char chr;
			while ((chr = (char)sim.memory.Read(address++,1)) != 0)
			{
				str += chr;
			}
			return str;
		}

		/// <summary>
		/// this function implements the vax printf and all it codes, the function throw 
		/// exception if the string format is illegal.
		/// </summary>
		/// <param name="bPrintToSreen">true if we print to screen</param>
		/// <remarks>TESTED!!! This function was fully tested - no future work is required</remarks>
		public static CodeBlock printf(Simulator theSimulator, bool bPrintToSreen)
		{
			int NumOfParam = (int) theSimulator.memory.Read (theSimulator.R[12], 4);
			string sFormat = ReadString(theSimulator, 
				(int)(theSimulator.memory.Read(theSimulator.R[12] + 4 * ((bPrintToSreen) ? 1 : 2), 4)));
			// the list start from 1, and the first one was the Format String if printf or if bPrintToScreen is false, we assume there
			// is another parameter - the output target.
			int ParamNumber = bPrintToSreen ? 2 : 3; 
			CodeBlock RetBlock = new CodeBlock();
			theSimulator.R[0] = -1;
			while (sFormat != "")
			{
				int per = sFormat.IndexOf("%") ;
				int slesh = sFormat.IndexOf("\\");

				if (((per < slesh) && (per != -1)) || ( per != -1 && slesh == -1)) //the next special char is %
				{
					// If the number of the parameters needed is bigger than the number that was send
					if (ParamNumber > NumOfParam) 
					{
						// Error: TOO_FEW_PARAMS
						return (CodeBlock)"";
					}

					string sTempString ;
					RetBlock += (CodeBlock) sFormat.Substring(0, per);
					sFormat = sFormat.Substring(per);
					per = 1;
					Match Fields = Regex.Match(sFormat,@"^(%(\-)?[0-9]+((\.)?[0-9]+)?)") ;
					int iTemp;
					int iIndex = (Fields.Length != 0) ? Fields.Length : per;
					switch(sFormat[iIndex])
					{
						case 'd':
							iTemp = (int)theSimulator.memory.Read (theSimulator.R[12]+4*ParamNumber++, 4);
							//iTemp = iTemp & 0xffff;
							sTempString = ((int)iTemp).ToString();
							break;
						case 'u':
							sTempString = ((long)theSimulator.memory.Read (theSimulator.R[12]+4*ParamNumber++, 4)).ToString();
							break;
						case 'o':
							sTempString = Convert.ToString(((int)theSimulator.memory.Read (theSimulator.R[12]+4*ParamNumber++, 4)),8);
							break;
						case 'x':
							iTemp = (int)theSimulator.memory.Read (theSimulator.R[12]+4*ParamNumber++, 4); 
							iTemp = iTemp & 0xffff;
							sTempString = (System.Convert.ToInt32(iTemp)).ToString("x");
							break;
						case 'X':
							iTemp = (int)theSimulator.memory.Read (theSimulator.R[12]+4*ParamNumber++, 4);
							iTemp = iTemp & 0xffff;
							sTempString = (System.Convert.ToInt32(iTemp)).ToString("x").ToUpper();
							break;
						case 'c':
							sTempString = new string((char)(theSimulator.memory.Read(theSimulator.R[12]+4*ParamNumber++, 4)), 1);
							break;
						case 's':
							int iStrAddr = (int)theSimulator.memory.Read (theSimulator.R[12]+4*ParamNumber++, 4);
							sTempString = ReadString(theSimulator, iStrAddr);
							break;
						case 'l':
							if ((iIndex + 1 < sFormat.Length) && (sFormat[iIndex + 1]=='d'))
							{
								sTempString = ((int)theSimulator.memory.Read (theSimulator.R[12]+4*ParamNumber++, 4)).ToString();
								sFormat = sFormat.Remove (iIndex + 1,1);
							}
							else if ((iIndex + 1 < sFormat.Length) && ((sFormat[iIndex + 1]=='x') || (sFormat[iIndex + 1]=='X')))
							{
								sTempString = ((int)theSimulator.memory.Read 
									(theSimulator.R[12]+4*ParamNumber++, 4)).ToString(sFormat[iIndex + 1].ToString());
								sFormat = sFormat.Remove (iIndex + 1,1);
							}
							else
								// Error: ILLEGAL_STRING_FORMAT
								return (CodeBlock)"";
							break;
						default:
							// Error: ILLEGAL_STRING_FORMAT
							return (CodeBlock)"";
					}
					if (Fields.Value != "")
					{
						int period = Regex.Match(Fields.Value, (@"\.")).Index;
						int iFirstField = Convert.ToInt32 (Fields.ToString().Substring(1, (period>0)?period-1:Fields.Length -1));
						int iSecField;
						bool bZeros = Regex.IsMatch(Fields.Value, @"^(%((0)|(\-0)))");
						if (period>0) 
							iSecField = Convert.ToInt32(Fields.ToString().Substring(period+1,Fields.Length - 1 -period));
						else
							iSecField = -1;
						if (iFirstField < 0) iFirstField = -iFirstField;
					{
						if (iFirstField - sTempString.Length > 0)
							for (int i=iFirstField - sTempString.Length;i>0;i--)
								if (bZeros)
									sTempString = '0' +sTempString;
								else
									sTempString = ' ' +sTempString;
					}	
						if ((iSecField != -1) && (iSecField < sTempString.Length)) //cut the output if there is number with fraction
							sTempString = sTempString.Substring(0, iSecField);
						
					}
					RetBlock += (CodeBlock) sTempString;
					if (sFormat.Length > 1) 
						sFormat = sFormat.Substring((Fields.Length != 0) ? Fields.Length + 1 : ++per);
				}
				else if (slesh!=-1) //the next special char is \
				{
					if (sFormat.Length < 1)
					{
						// Error: ILLEGAL_STRING_FORMAT
						return (CodeBlock)"";
					}
					RetBlock += (CodeBlock) sFormat.Substring(0, slesh);


					if (sFormat.Length > 0) sFormat = sFormat.Substring(++slesh);
				}
				else // we don't have ether \ nor %
				{
					RetBlock += (CodeBlock) sFormat;
					sFormat = "";
				}
			}

			if (bPrintToSreen)
			{
				try	{ theSimulator.Console.Invoke(theSimulator.Console.m_Output, new object[] { RetBlock }); }
				catch { throw new RuntimeError(SimulatorMessage.CONSOLE_EXIT, theSimulator.R[15]); }
			}
			theSimulator.R[0] = 0;
			return RetBlock;
		}

		/// <summary>
		/// this function implements the vax scanf and all it codes, the function throw 
		/// exception if the string format is illegal.
		/// </summary>
		public static void scanf(Simulator theSimulator)
		{
			int NumOfParam = (int) theSimulator.memory.Read (theSimulator.R[12], 4);
			string sFormat = ReadString(theSimulator, 
				(int)(theSimulator.memory.Read(theSimulator.R[12] + 4, 4)));
			int ParamNumber = 2 ;
			byte ReadChar = 0;
			theSimulator.R[0] = 0;
			int iPosition = 0;
			while (iPosition < sFormat.Length)
			{
				
				//getting chars
				ReadChar = theSimulator.Console.getchar(); 

				if (ReadChar == 0xFF) //EOF CASE
				{
					theSimulator.R[0] = -1;
					return;
				}

				while ((iPosition < sFormat.Length) && (sFormat[iPosition] != '%'))
				{
					//skipping whitespaces in format string
					if ((sFormat[iPosition] == ' ') || (sFormat[iPosition] == '	') || 
						(sFormat[iPosition] == '\n') || (sFormat[iPosition] == '\r'))
					{
						++iPosition;
						continue;
					}

					//skipping whitespaces in input
					while ((ReadChar == (byte)' ') || (ReadChar == 0x0D) || (ReadChar == 0)
						|| (ReadChar == (byte)'	') || (ReadChar == 0x0A))
						ReadChar = theSimulator.Console.getchar();

					//compare non whitespaces between input and format string
					if (ReadChar != sFormat[iPosition]) return; //stop scanf, ILLEGAL_INPUT
                    ++iPosition;
					if (iPosition < sFormat.Length)
						ReadChar = theSimulator.Console.getchar();

					if (ReadChar == 0xFF) //EOF CASE
					{
						theSimulator.R[0] = -1;
						return;
					}
				}

				if ((iPosition + 1< sFormat.Length) && (sFormat[iPosition] == '%')) //the next special char is %
				{

					//read the adderss to write to
					int iStrAddr = (int)theSimulator.memory.Read (theSimulator.R[12]+4*ParamNumber++, 4);
					bool bNeedEnterWhile = true; //for star option, chack that input after star is correct, despite that we don't need the value
					bool bHaveStar = false;
					while (bNeedEnterWhile)
					{
						bNeedEnterWhile = false;
						string buffer = "";
						switch (sFormat[++iPosition])
						{
							case 'd':
								//skipping whitespaces in input
								while ((ReadChar == (byte)' ') || (ReadChar == 0x0D) || (ReadChar == 0)
									|| (ReadChar == (byte)'	') || (ReadChar == 0x0A))
									ReadChar = theSimulator.Console.getchar();

								if (ReadChar == '-') 
								{
									buffer = "-";
									ReadChar = theSimulator.Console.getchar();
								}
								while ((ReadChar >= '0') && (ReadChar <='9'))
								{
									buffer += ((char)ReadChar).ToString();
									ReadChar = theSimulator.Console.getchar();
								}
								if ((ReadChar == (byte)' ') || (ReadChar == 0x0D) || (ReadChar == 0) || 
									(ReadChar == (byte)'	') || (ReadChar == 0x0A))
								{
									if (bHaveStar) break;
									if (buffer[0] == '-' && buffer.Length == 1) return;
									if (buffer.Length == 0) return;
									int iWant2C = System.Convert.ToInt32(buffer);
									theSimulator.memory.Write (iStrAddr, new CodeBlock((long)iWant2C,4));
									break;
								}
								else //ILLEGAL_INPUT
									return;
							case 'u':
								//skipping whitespaces in input
								while ((ReadChar == (byte)' ') || (ReadChar == 0x0D) || (ReadChar == 0)
									|| (ReadChar == (byte)'	') || (ReadChar == 0x0A))
									ReadChar = theSimulator.Console.getchar();

								while ((ReadChar >= '0') && (ReadChar <='9'))
								{
									buffer += ((char)ReadChar).ToString();
									ReadChar = theSimulator.Console.getchar();
								}
								if ((ReadChar == (byte)' ') || (ReadChar == 0x0D) || (ReadChar == 0) || 
									(ReadChar == (byte)'	') || (ReadChar == 0x0A))
								{
									if (bHaveStar) break;
									uint iWant2C = System.Convert.ToUInt32(buffer);
									theSimulator.memory.Write (iStrAddr, new CodeBlock ((long) iWant2C,4));
									break;
								}
								else //ILLEGAL_INPUT
									return;
							case 'o':
								//skipping whitespaces in input
								while ((ReadChar == (byte)' ') || (ReadChar == 0x0D) || (ReadChar == 0)
									|| (ReadChar == (byte)'	') || (ReadChar == 0x0A))
									ReadChar = theSimulator.Console.getchar();

								while ((ReadChar >= '0') && (ReadChar <='7'))
								{
									buffer += ((char)ReadChar).ToString();
									ReadChar = theSimulator.Console.getchar();
								}
								if ((ReadChar == (byte)' ') || (ReadChar == 0x0D) || (ReadChar == 0) || 
									(ReadChar == (byte)'	') || (ReadChar == 0x0A))
								{
									if (bHaveStar) break;
									int iWant2C = System.Convert.ToInt32(buffer, 8);
									theSimulator.memory.Write (iStrAddr, new CodeBlock((long) iWant2C, 4));
									break;
								}
								else //ILLEGAL_INPUT
									return;
							case 'x':
								//skipping whitespaces in input
								while ((ReadChar == (byte)' ') || (ReadChar == 0x0D) || (ReadChar == 0)
									|| (ReadChar == (byte)'	') || (ReadChar == 0x0A))
									ReadChar = theSimulator.Console.getchar();

								if (ReadChar == '0')
								{
									ReadChar = theSimulator.Console.getchar();
									if ((ReadChar == 'x') || (ReadChar == 'X'))
									{
										buffer = "";
										ReadChar = theSimulator.Console.getchar();
									}	
								}
								while (((ReadChar >= '0') && (ReadChar <='9')) || ((ReadChar >= 'a') && (ReadChar <='f')) ||
									((ReadChar >= 'A') && (ReadChar <='F')))
								{
									buffer += ((char)ReadChar).ToString();
									ReadChar = theSimulator.Console.getchar();
								}
								if ((ReadChar == (byte)' ') || (ReadChar == 0x0D) || (ReadChar == 0) || 
									(ReadChar == (byte)'	') || (ReadChar == 0x0A))
								{
									if (bHaveStar) break;
									int iWant2C = System.Convert.ToInt32(buffer, 16);
									theSimulator.memory.Write (iStrAddr, new CodeBlock ((long)iWant2C, 4));
									break;
								}
								else //ILLEGAL_INPUT
									return;
							case 'c':
								if (bHaveStar) break;
								theSimulator.memory.Write (iStrAddr, new CodeBlock((long)ReadChar, 4));
								break;
							case 's':
								//skipping whitespaces in input
								while ((ReadChar == (byte)' ') || (ReadChar == 0x0D) || (ReadChar == 0)
									|| (ReadChar == (byte)'	') || (ReadChar == 0x0A))
									ReadChar = theSimulator.Console.getchar();

								while ((ReadChar != (byte)' ') && (ReadChar != 0x0D) && (ReadChar != 0) && 
									(ReadChar != (byte)'	') && (ReadChar != 0x0A))
								{
									buffer += ((char)ReadChar).ToString();
									ReadChar = theSimulator.Console.getchar();
								}
								if (bHaveStar) break;
								buffer += '\0';
								theSimulator.memory.Write(iStrAddr, (CodeBlock) buffer);
								break;
							case '*':
								if (bHaveStar) return; // Error: ILLEGAL_STRING_FORMAT
								if (iPosition + 1 >= sFormat.Length) return; // Error: ILLEGAL_STRING_FORMAT
								bNeedEnterWhile = true;
								bHaveStar = true;
								ParamNumber--; // ugly fix to ugly problem
								break;
							default:
								// Error: ILLEGAL_STRING_FORMAT
								return;
						}
					}
					if (!bHaveStar) 
					{
						// If the number of the parameters needed is bigger than the number that was send
						if (ParamNumber -1 > NumOfParam) 
						{
							// Error: TOO_FEW_PARAMS
							return; 
						}
						++theSimulator.R[0];
					}
					++iPosition;
				}
				else // we don't have %, or we don't have another char after it
				{
					// Error: ILLEGAL_STRING_FORMAT
					return;
				}
			}
			return;
		}

		/// <summary>
		/// this function implements the vax printf and all it codes, the function throws an
		/// exception if the string format is illegal.
		/// </summary>
		public static void sprintf(Simulator theSimulator)
		{
			CodeBlock theString = printf(theSimulator, false);
			
			// Check if printf ended correctly
			if (theSimulator.R[0].ReadLong() == -1) return;

			int iaTarget = (int)(theSimulator.memory.Read(theSimulator.R[12] + 4, 4));
			theSimulator.memory.Write(iaTarget, theString);
		}

		/// <summary>
		/// Implementation of puts function
		/// </summary>
		/// <param name="theSimulator"></param>
		public static void puts(Simulator theSimulator)
		{
			// read the string and convert it to CodeBlock for printing
			string sString = ReadString(theSimulator, (int)(theSimulator.memory.Read(theSimulator.R[12] + 4, 4)));
			CodeBlock res = (CodeBlock) sString;

			// assume fail
			theSimulator.R[0] = -1;

			try	{theSimulator.Console.Invoke(theSimulator.Console.m_Output, new object[] { res + '\n'}); }
			catch { throw new RuntimeError(SimulatorMessage.CONSOLE_EXIT, theSimulator.R[15]); }

			// printing was success
			theSimulator.R[0] = 0;
		}

		/// <summary>
		/// Implemantation of putchar function
		/// </summary>
		public static void putchar(Simulator theSimulator)
		{
			theSimulator.R[0] = -1;
			try	
			{
				theSimulator.Console.Invoke(theSimulator.Console.m_Output, 
					new object[] { (CodeBlock)(theSimulator.memory.Read(theSimulator.R[12] + 4, 4))[0]}); 
				theSimulator.R[0] = 0;
			}
			catch
			{ 
				throw new RuntimeError(SimulatorMessage.CONSOLE_EXIT, theSimulator.R[15]); 
			}
		}

		public static void getchar(Simulator theSimulator)
		{
			int theChar = (int)theSimulator.Console.getchar();
			if (theChar == 0xFF) 
				theSimulator.R[0] = -1;
			else if (theChar == 0x0d) // got enter, but think it like return
				theSimulator.R[0] = 0x0a;
			else theSimulator.R[0] = theChar;
		}

		public static void gets(Simulator theSimulator)
		{
			int InputPosition = (int)(theSimulator.memory.Read(theSimulator.R[12] + 4, 4));
			byte ReadChar = theSimulator.Console.getchar();

			CodeBlock Data = new CodeBlock();
			if (ReadChar == 0xFF)  
			{
				theSimulator.R[0] = -1;
				return;
			}
			else if (ReadChar == 0x04)
			{
				int Read2Char = theSimulator.Console.viewFirstChar();
				if ((ReadChar != 0) && (ReadChar != 0x0D) && (ReadChar != 0x0A) && (ReadChar != 0xFF))
				{
					theSimulator.R[0] = 0;
					return;
				}
			}

			while ((ReadChar != 0) && (ReadChar != 0x0D) && (ReadChar != 0x0A) && (ReadChar != 0xFF))
			{
				Data += (byte) ReadChar;
				ReadChar = theSimulator.Console.getchar();
			}
			if (ReadChar == 0xFF)
				theSimulator.Console.unReadChar(ReadChar);
			Data += (byte) 0;
			theSimulator.memory.Write(InputPosition, Data);
			theSimulator.R[0] = InputPosition; //return in r0 the start address of input string
		}

		/// <summary>
		/// Implementation of exit function
		/// </summary>
		/// <param name="theSimulator"></param>
		public static void exit(Simulator theSimulator)
		{
			int iCode = (int)(theSimulator.memory.Read(theSimulator.R[12] + 4, 4));
			VAX11Opcodes.ret(theSimulator, 0x04, null);
			throw new RuntimeError(SimulatorMessage.NORMAL_EXIT, theSimulator.R[15], iCode);
		}

		/// <summary>
		/// Implementation of initgraph function
		/// </summary>
		/// <param name="theSimulator"></param>
		public static void initgraph(Simulator theSimulator)
		{
			theSimulator.Console.InitGraph();
			theSimulator.R[0] = 0;
		}

		/// <summary>
		/// Implementation of line function
		/// </summary>
		/// <param name="theSimulator"></param>
		public static void line(Simulator theSimulator)
		{
			int NumOfParams = (int) theSimulator.memory.Read (theSimulator.R[12], 4);
			if (NumOfParams != 4) throw new RuntimeError(SimulatorMessage.WRONG_NUMBER_OF_PARAMS, theSimulator.R[15]);
			int[] arr = new int[4];
			for (int ParamNumber = 1; ParamNumber <= 4; ParamNumber++)
				arr[ParamNumber - 1] = theSimulator.memory.Read(theSimulator.R[12]+4*ParamNumber, 4);
			theSimulator.Console.Line(arr[3], arr[2], arr[1], arr[0]);
		}


		public static void putpixel(Simulator theSimulator)
		{
			int NumOfParams = (int) theSimulator.memory.Read (theSimulator.R[12], 4);
			if (NumOfParams != 2) throw new RuntimeError(SimulatorMessage.WRONG_NUMBER_OF_PARAMS, theSimulator.R[15]);
			int[] arr = new int[2];
			for (int ParamNumber = 1; ParamNumber <= 2; ParamNumber++)
				arr[ParamNumber - 1] = theSimulator.memory.Read(theSimulator.R[12]+4*ParamNumber, 4);
			theSimulator.Console.PutPixel(arr[1], arr[0]);
		}


		/*public static void fill(Simulator theSimulator)
		{
			int NumOfParams = (int) theSimulator.memory.Read (theSimulator.R[12], 4);
			if (NumOfParams != 2) throw new RuntimeError(SimulatorMessage.WRONG_NUMBER_OF_PARAMS, theSimulator.R[15]);
			int[] arr = new int[2];
			for (int ParamNumber = 1; ParamNumber <= 2; ParamNumber++)
				arr[ParamNumber - 1] = theSimulator.memory.Read(theSimulator.R[12]+4*ParamNumber, 4);
			theSimulator.Console.Fill(arr[1], arr[0]);
		}*/


		public static void setcolor(Simulator theSimulator)
		{
			int NumOfParams = (int) theSimulator.memory.Read (theSimulator.R[12], 4);
			if (NumOfParams != 3) throw new RuntimeError(SimulatorMessage.WRONG_NUMBER_OF_PARAMS, theSimulator.R[15]);
			int[] arr = new int[3];
			for (int ParamNumber = 1; ParamNumber <= 3; ParamNumber++)
				arr[ParamNumber - 1] = theSimulator.memory.Read(theSimulator.R[12]+4*ParamNumber, 4);
			theSimulator.Console.SetColor(arr[2], arr[1], arr[0]);
		}
	
		
		public static void rectangle(Simulator theSimulator)
		{
			int NumOfParams = (int) theSimulator.memory.Read (theSimulator.R[12], 4);
			if (NumOfParams != 4) throw new RuntimeError(SimulatorMessage.WRONG_NUMBER_OF_PARAMS, theSimulator.R[15]);
			int[] arr = new int[4];
			for (int ParamNumber = 1; ParamNumber <= 4; ParamNumber++)
				arr[ParamNumber - 1] = theSimulator.memory.Read(theSimulator.R[12]+4*ParamNumber, 4);
			theSimulator.Console.Rectangle(arr[3], arr[2], arr[1], arr[0]);
		}


		public static void circle(Simulator theSimulator)
		{
			int NumOfParams = (int) theSimulator.memory.Read (theSimulator.R[12], 4);
			if (NumOfParams != 3) throw new RuntimeError(SimulatorMessage.WRONG_NUMBER_OF_PARAMS, theSimulator.R[15]);
			int[] arr = new int[3];
			for (int ParamNumber = 1; ParamNumber <= 3; ParamNumber++)
				arr[ParamNumber - 1] = theSimulator.memory.Read(theSimulator.R[12]+4*ParamNumber, 4);
			theSimulator.Console.Circle(arr[2], arr[1], arr[0]);
		}

		public static void outtextxy(Simulator theSimulator)
		{
			int NumOfParams = (int) theSimulator.memory.Read (theSimulator.R[12], 4);
			if (NumOfParams != 3) throw new RuntimeError(SimulatorMessage.WRONG_NUMBER_OF_PARAMS, theSimulator.R[15]);
			int[] arr = new int[3];
			for (int ParamNumber = 1; ParamNumber <= 3; ParamNumber++)
				arr[ParamNumber - 1] = theSimulator.memory.Read(theSimulator.R[12]+4*ParamNumber, 4);
			theSimulator.Console.OutTextXY(arr[2], arr[1], ReadString(theSimulator,arr[0]));
		}


		public static void setfont(Simulator theSimulator)
		{
			int NumOfParams = (int) theSimulator.memory.Read (theSimulator.R[12], 4);
			if (NumOfParams != 2) throw new RuntimeError(SimulatorMessage.WRONG_NUMBER_OF_PARAMS, theSimulator.R[15]);
			int[] arr = new int[2];
			for (int ParamNumber = 1; ParamNumber <= 2; ParamNumber++)
				arr[ParamNumber - 1] = theSimulator.memory.Read(theSimulator.R[12]+4*ParamNumber, 4);
			theSimulator.Console.SetFont(ReadString(theSimulator,arr[1]),arr[0]);
		}

		/// <summary>
		/// Implementation of cleardevice function
		/// </summary>
		/// <param name="theSimulator"></param>
		public static void cleardevice(Simulator theSimulator)
		{
			theSimulator.Console.ClearDevice();
			theSimulator.R[0] = 0;
		}

		/// <summary>
		/// Implementation of closegraph function
		/// </summary>
		/// <param name="theSimulator"></param>
		public static void closegraph(Simulator theSimulator)
		{
			theSimulator.Console.CloseGraph();
			theSimulator.R[0] = 0;
		}

		/// <summary>
		/// Implementation of malloc function
		/// </summary>
		public static void malloc(Simulator theSimulator)
		{
			try
			{
				int iSize = (int) theSimulator.memory.Read (theSimulator.R[12]+4, 4);
				int iAddress = theSimulator.memory.MemAllocation(iSize + 8);
				theSimulator.R[0] = iAddress + 8;
				theSimulator.memory.Write(iAddress, new CodeBlock((long)iSize, 4));
				theSimulator.memory.Write(iAddress+4, new CodeBlock(0L, 4));
				
				// no memory allocation yet
				if (theSimulator.FirstAllocatedAddress == 0) theSimulator.FirstAllocatedAddress = iAddress;
				else
				{
					int iLastAddress = theSimulator.FirstAllocatedAddress;
					int iCurAddress = theSimulator.FirstAllocatedAddress;
					while (iCurAddress != 0)
					{
						iCurAddress = theSimulator.memory.Read(iCurAddress+4, 4);
						if (iCurAddress != 0) iLastAddress = iCurAddress;
					}
					theSimulator.memory.Write(iLastAddress+4, new CodeBlock((long)iAddress, 4));
				}
				
			}
			catch(Memory.VAX11HeapManager.NoFreeMemoryException)
			{
                theSimulator.R[0] = 0;
				return;
			}
		}

		/// <summary>
		/// Implementation of free function
		/// </summary>
		public static void free(Simulator theSimulator)
		{
			// Read address to free
			int iAddress = (int) theSimulator.memory.Read (theSimulator.R[12]+4, 4) - 8;

			// How many bytes we want to free
			int iSize = theSimulator.memory.Read(iAddress, 4);

			int iNextBlockAddress = theSimulator.memory.Read(iAddress+4, 4);

			// mark the memory as free in the internal database. might throw run-time exception
			theSimulator.memory.FreeMemory((uint)iAddress, iSize + 8);
            

			// if we're the first node
			if (theSimulator.FirstAllocatedAddress == iAddress)
			{
				theSimulator.FirstAllocatedAddress = iNextBlockAddress;
			}
			else
			{
				// search for the previous node in the linked list
				int iLastAddress = theSimulator.FirstAllocatedAddress;
				int iCurAddress = theSimulator.FirstAllocatedAddress;
				while (iCurAddress != iAddress && iCurAddress != 0)
				{
					iCurAddress = theSimulator.memory.Read(iCurAddress+4, 4);
					if (iCurAddress != 0 && iCurAddress != iAddress) iLastAddress = iCurAddress;
				}
				if (iCurAddress == 0) throw new RuntimeError(SimulatorMessage.MEMORY_ACCESS_FAULT, iAddress);
				
				// Make the previous node to point to the next node
				theSimulator.memory.Write(iLastAddress+4, new CodeBlock((long)iNextBlockAddress, 4));
			}

			// Fill header with 11's
			theSimulator.memory.Write(iAddress, new CodeBlock(Operand.Mask(8), 8));
		}

		/// <summary>
		/// Implementation of getmaxx function
		/// </summary>
		/// <param name="theSimulator"></param>
		public static void getmaxx(Simulator theSimulator)
		{
			theSimulator.R[0] = 640;
		}

		
		/// <summary>
		/// Implementation of getmaxy function
		/// </summary>
		/// <param name="theSimulator"></param>
		public static void getmaxy(Simulator theSimulator)
		{
			theSimulator.R[0] = 480;
		}
	}
}
