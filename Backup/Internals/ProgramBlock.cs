using System;
using System.Collections;

namespace VAX11Internals
{
	/// <summary>
	/// ProgramBlock contains machine code of program, ready to run.
	/// </summary>
	public class ProgramBlock
	{

		#region Consturctors

		/// <summary>
		/// Creates an empty program block
		/// </summary>
		public ProgramBlock()
		{
			_theProgram = new ArrayList();
		}

		#endregion

		#region Members

		/// <summary>
		/// Entry point address for the program
		/// </summary>
		int _entryPoint;

		/// <summary>
		/// Blocks iterator
		/// </summary>
		int _iCurItem;

		/// <summary>
		/// The program's machine code
		/// </summary>
		ArrayList _theProgram;

		#endregion

		#region Propreties

		/// <summary>
		/// Entry point address for the program
		/// </summary>
		public int EntryPoint
		{
			get { return _entryPoint; }
			set { _entryPoint = value; }
		}

		/// <summary>
		/// Returns the end address of the block with the highest address. Assume it is the last block in the blocks array
		/// </summary>
		public int Size
		{
			get
			{
				if (_theProgram.Count == 0) return 0;
				BlockContainer tmp = ((BlockContainer) _theProgram[_theProgram.Count-1]);
				return tmp.iAddress + tmp.theBlock.Size;
			}
			
		}

		public int LastLine
		{
			set
			{ 
				if (_theProgram.Count == 0) return; 
				((BlockContainer) _theProgram[_theProgram.Count-1]).iLastLineInBlock = value;
			}
		}

		#endregion

		#region Methods

		#region Iterator Methods

		/// <summary>
		/// Returns first code block
		/// </summary>
		/// <returns>CodeBlock or null</returns>
		public BlockContainer GetFirst()
		{
			if (_theProgram.Count == 0) return null;
			_iCurItem = 0;
			return (BlockContainer)_theProgram[0];
		}

		/// <summary>
		/// Return the next code block
		/// </summary>
		/// <returns>CodeBlock or null</returns>
		public BlockContainer GetNext()
		{
			++_iCurItem;
			if (_iCurItem < _theProgram.Count) return (BlockContainer)_theProgram[_iCurItem];
			return null;
		}

		#endregion

		#region General Methods

		/// <summary>
		/// Insert new block to the program block. need code and address to put it on.
		/// Assuming the best - it is the caller responsible to make sure there are no בלוקים חופפים
		/// </summary>
		/// <param name="newBlock">the codeblock</param>
		/// <param name="iAddress">starting address for the block</param>
		public void AddBlock(CodeBlock newBlock, int iAddress, int iLastLine)
		{
			BlockContainer newBlockContainer = new BlockContainer(newBlock, iAddress, iLastLine);
			_theProgram.Add(newBlockContainer);

		}

		#endregion

		#endregion

		#region Sub Classes

		/// <summary>
		/// Contains CodeBlock + Address. Actually allocate more space than needed
		/// in order to avoid many memory allocations.
		/// </summary>
		public class BlockContainer
		{

			#region Globals
			
			public int iAddress;
			public int iLastLineInBlock;

			#endregion

			#region Consts

			/// <summary>
			/// Default size for codeblock saved in the blockcontainer to avoid lots of memory allocations
			/// </summary>
			private const int PRIVATE_PAGE_SIZE = 1024;

			#endregion

			#region Members


			/// <summary>
			/// Index of the last used byte of theBlock
			/// </summary>
			private int iNextIndexToUse = 0;


			/// <summary>
			/// The actual data we save
			/// </summary>
			private CodeBlock _theBlock;

			#endregion

			#region Constructor

			public BlockContainer(CodeBlock block, int address, int iLastLine)
			{
				_theBlock = new CodeBlock(0L, PRIVATE_PAGE_SIZE);
				_theBlock.CopySubBlock(iNextIndexToUse, block, PRIVATE_PAGE_SIZE);
				iNextIndexToUse += block.Size;

				iAddress = address;
				iLastLineInBlock = iLastLine;
			}

			#endregion

			#region Properties
			
			/// <summary>
			/// Sets the CodeBlock we save or get _copy_ of that block
			/// </summary>
			public CodeBlock theBlock
			{
				get
				{
					// We actually save bigger block then we need so we create block in the
					// right size on request
					CodeBlock res = new CodeBlock(0L, iNextIndexToUse);
					for (int iCounter = 0; iCounter < iNextIndexToUse; ++iCounter)
						res[iCounter] = _theBlock[iCounter];
					return res;
				}

				set
				{
					_theBlock.CopySubBlock(0, value, PRIVATE_PAGE_SIZE);
					iNextIndexToUse = value.Size;
				}

			}

			/// <summary>
			/// The size of the saved block
			/// </summary>
			public int Size
			{
				get { return iNextIndexToUse; }
			}

			#endregion

			#region Methods

			/// <summary>
			/// Append block to the end of the saved code block
			/// </summary>
			/// <param name="blk">the block to append</param>
			public void Append(CodeBlock blk)
			{
				_theBlock.CopySubBlock(iNextIndexToUse, blk, PRIVATE_PAGE_SIZE);
				iNextIndexToUse += blk.Size;
			}

			#endregion

			#region Operators

			/// <summary>
			/// Indexer
			/// </summary>
			/// <remarks>Might throw IndexOutOfRangeException</remarks>
			public byte this[int iIndex]
			{
				get { return _theBlock[iIndex]; }
				set { _theBlock[iIndex] = value; }
			}

			#endregion
		}

		#endregion

		#region Operators

		/// <summary>
		/// Add a codeblock to the end of the current block
		/// </summary>
		/// <param name="theBlock">The program we want to append data to</param>
		/// <param name="blk">the block to add</param>
		/// <returns>Reference to the given program</returns>
		public static ProgramBlock operator +(ProgramBlock theProg, CodeBlock blk)
		{
			// If the given program is illegal - go to hell
			if (theProg == null) throw new PanicException();

			// else, if the given program is empty, lets create an empty block, starting at 0. Default Behavior
			if (theProg._theProgram.Count == 0) theProg.AddBlock(new CodeBlock(), 0, 0);

			// Add the new block to the end of the last block.
			BlockContainer tmp = (BlockContainer)theProg._theProgram[theProg._theProgram.Count - 1];
			tmp.Append(blk);

			return theProg;
		}

		/// <summary>
		/// Indexer
		/// </summary>
		/// <remarks>Might throw IndexOutOfRangeException. Will send ya to hell if you touch illegal position</remarks>
		public byte this[int iIndex]
		{
			get
			{
				foreach (Object o in _theProgram)
				{
					BlockContainer curBlock = (BlockContainer)o;
					if (curBlock.iAddress  <= iIndex && curBlock.iAddress + curBlock.Size > iIndex)
						return curBlock[iIndex - curBlock.iAddress];
				}
				
				// We should never get here if we got valid addres
				throw new PanicException();
			}
			set
			{
				foreach (Object o in _theProgram)
				{
					BlockContainer curBlock = (BlockContainer)o;
					if (curBlock.iAddress  <= iIndex && curBlock.iAddress + curBlock.Size > iIndex)
					{
						curBlock[iIndex - curBlock.iAddress] = value;
						return;
					}
				}

				// We should never get here if we got valid addres
				throw new PanicException();
			}
		}


		#endregion

	}
}
