using System;
using System.Collections;

using VAX11Internals;
using VAX11Settings;

namespace VAX11Simulator
{

	#region Memory Delegates

	/// <summary>
	/// Delegate for accessing memory event.
	/// </summary>
	public delegate void MemoryAccessedFunc(int iAddress, int iPhysicalAddress, int iValue, bool IsWrite);

	/// <summary>
	/// Delegate for page-fault
	/// </summary>
	public delegate void PageFaultFunc(int iNewVirtualPage, int iNewPhysicalPage, bool bNeedSwapping, int iOldVirtualPage);


	#endregion


	/// <summary>
	/// VAX-11 Memory implemenatation
	/// </summary>
	public class Memory
	{

		#region Members

		/// <summary>
		/// Memory capacity
		/// </summary>
		private int _capacity;

		/// <summary>
		/// Memory Page Size
		/// </summary>
		private int _pageSize;

		/// <summary>
		/// Default value for memory cells.
		/// </summary>
		private byte _fill;

		/// <summary>
		/// Hash table thats saves all the program's memory
		/// </summary>
		private Hashtable PagesHash;

		/// <summary>
		/// Physical memory simulation
		/// </summary>
		private PhysicalMemorySimulation PhysicalMemory;


		/// <summary>
		/// Should we enable the physical memory simulation?
		/// </summary>
		private bool _enablePhyicalMemorySimulation;


		/// <summary>
		/// Heap Manager for dynamic memory allocations
		/// </summary>
		private VAX11HeapManager HeapManager;

		#endregion

		#region Constructor

		/// <summary>
		/// Contructor
		/// </summary>
		/// <param name="fill">Default value for unsigned memory cells</param>
		/// <param name="capacity">Maximum memory capacity</param>
		public Memory(byte fill, int capacity)
		{
			PagesHash = new Hashtable();
			PhysicalMemory = new PhysicalMemorySimulation();
			if (Settings.Simulator.bShowPageFaults) PhysicalMemory.OnPageFault += new PageFaultFunc(PageFaultHandler);
			_capacity = capacity;
			_pageSize = Settings.Simulator.PageSize;
			_fill = fill;
			_enablePhyicalMemorySimulation = Settings.Simulator.bEnablePhysicalMemorySimulation;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Memory capacity
		/// </summary>
		public int Capacity
		{
			get 
			{
				return _capacity;
			}
		}

		/// <summary>
		/// Indexer
		/// </summary>
		/// <remarks>No need for lock</remarks>
		public byte this[int iIndex]
		{
			get { return Read(iIndex, 1, true)[0]; }
			set { Write(iIndex, (CodeBlock)value); }

		}

		#endregion

		#region Methods

		/// <summary>
		/// Writes program data to the memory
		/// </summary>
		/// <param name="cbData">The program to write</param>
		public void WriteProgram(ProgramBlock cbData)
		{
			// Load all the parts of the program to the memory
			for (ProgramBlock.BlockContainer cur = cbData.GetFirst(); cur != null; cur = cbData.GetNext())
				Write(cur.iAddress, cur.theBlock, false);

			// +0x100 just to be sure
			HeapManager = new VAX11HeapManager((uint)cbData.Size + 0x100, Settings.Simulator.iSP - Settings.Simulator.iStackSize);
		}


		/// <summary>
		/// Read block from the memory
		/// </summary>
		/// <param name="iAddress">Starting address</param>
		/// <param name="size">Number of bytes to read</param>
		/// <returns>CodeBlock contains the requested memory contents</returns>
		/// <remarks>Raise events when accessing the memory</remarks>
		public CodeBlock Read(int iAddress, int size, bool bReportRead)
		{
			lock (this)
			{
				CodeBlock Result = new CodeBlock(size, _fill);
				for (int iCounter = 0; iCounter < size; ++iCounter)
				{
					Result[iCounter] = ReadByte(iAddress + iCounter, bReportRead);
					if (OnMemoryAccess != null && bReportRead)
					{
						// If physical memory simulation is diabled, report physical address 0
						if (!_enablePhyicalMemorySimulation)
							OnMemoryAccess(iAddress + iCounter,  0 , Result[iCounter], false);
						else
							OnMemoryAccess(iAddress + iCounter, PhysicalMemory.GetPhysicalAddressFromVirtualAddress(iAddress + iCounter), Result[iCounter], false);
					}
				}
				return Result;
			}
		}

		/// <summary>
		/// Read block from the memory. ALWAYS REPORT ON ACCESS. SEE OVERLOADED FUNCTION!!!
		/// </summary>
		/// <param name="iAddress">Starting address</param>
		/// <param name="size">Number of bytes to read</param>
		/// <returns>CodeBlock contains the requested memory contents</returns>
		/// <remarks>Raise events when accessing the memory</remarks>
		public CodeBlock Read(int iAddress, int size)
		{
			return Read(iAddress, size, true);
		}

		/// <summary>
		/// Writes data to the memory
		/// </summary>
		/// <param name="iAddress">Starting address</param>
		/// <param name="cbData">The information to write</param>
		/// <remarks>Raise events when accessing the memory</remarks>
		public void Write(int iAddress, CodeBlock cbData)
		{
			lock(this)
			{
				for (int iCounter = 0; iCounter < cbData.Size; ++iCounter)
				{
					WriteByte(iAddress + iCounter, cbData[iCounter], true);
					if (OnMemoryAccess != null)
					{
						// If physical memory simulation is diabled, report physical address 0
						if (!_enablePhyicalMemorySimulation)
							OnMemoryAccess(iAddress + iCounter, 0 , cbData[iCounter], true);
						else
							OnMemoryAccess(iAddress + iCounter, PhysicalMemory.GetPhysicalAddressFromVirtualAddress(iAddress + iCounter), cbData[iCounter], true);
					}
				}
			}
		}

		
		/// <summary>
		/// Writes data to the memory
		/// </summary>
		/// <param name="iAddress">Starting address</param>
		/// <param name="cbData">The information to write</param>
		/// <param name="bReportWrite">True if we need to report about the written byte</param>
		/// <remarks>Raise events when accessing the memory</remarks>
		public void Write(int iAddress, CodeBlock cbData, bool bReportWrite)
		{
			lock(this)
			{
				for (int iCounter = 0; iCounter < cbData.Size; ++iCounter)
				{
					WriteByte(iAddress + iCounter, cbData[iCounter], bReportWrite);
					if (OnMemoryAccess != null && bReportWrite)
					{
						if (!_enablePhyicalMemorySimulation)
                            OnMemoryAccess(iAddress + iCounter, 0, cbData[iCounter], true);
						else
							OnMemoryAccess(iAddress + iCounter, PhysicalMemory.GetPhysicalAddressFromVirtualAddress(iAddress + iCounter), cbData[iCounter], true);
					}
				}
			}
		}



		/// <summary>
		/// Writes single byte to the memory
		/// </summary>
		/// <param name="iAddress">Address</param>
		/// <param name="data">What to write</param>
		private void WriteByte(int iAddress, byte data, bool bReportWrite)
		{
			if (bReportWrite && _enablePhyicalMemorySimulation) PhysicalMemory.PhysicalPageHit((int)((uint)iAddress / _pageSize));
			if (!PagesHash.ContainsKey((int)((uint)iAddress / _pageSize)))
			{
				PagesHash[(int)((uint)iAddress / _pageSize)] = (Settings.Simulator.bFillUninitalizeMemoryWithGarbage) ?
				CreateNewBlockAndFillItWithGarbage() : new CodeBlock(_pageSize, _fill);
			}
			((CodeBlock)PagesHash[(int)((uint)iAddress / _pageSize)])[(int)((uint)iAddress % _pageSize)] = data;
		}

		/// <summary>
		/// Reads single byte from the memory
		/// </summary>
		/// <param name="iAddress">Requested Address</param>
		/// <param name="bReportRead">True if we report about the read. If not, we don't need to change physical memory</param>
		/// <returns>The address's contents</returns>
		private byte ReadByte(int iAddress, bool bReportRead)
		{
			if (bReportRead && _enablePhyicalMemorySimulation) PhysicalMemory.PhysicalPageHit((int)((uint)iAddress / (uint)_pageSize));
			if (PagesHash.ContainsKey((int)((uint)iAddress / _pageSize)))
				return ((CodeBlock)PagesHash[(int)((uint)iAddress / _pageSize)])[(int)((uint)iAddress % _pageSize)];
			else if (Settings.Simulator.bFillUninitalizeMemoryWithGarbage)
			{
				// Create new page and fill it with garbage
				CodeBlock cNewBlock = CreateNewBlockAndFillItWithGarbage();
				PagesHash[(int)((uint)iAddress / _pageSize)] = cNewBlock;
				return ((CodeBlock)PagesHash[(int)((uint)iAddress / _pageSize)])[(int)((uint)iAddress % _pageSize)];
			}
			return _fill;
		}

		void PageFaultHandler(int iNewVirtualPage, int iNewPhysicalPage, bool bNeedSwapping, int iOldVirtualPage)
		{
			if (OnPageFault != null) OnPageFault(iNewVirtualPage, iNewPhysicalPage, bNeedSwapping, iOldVirtualPage);
		}

		/// <summary>
		/// Creates new CodeBlock in page size, and fill it with random values.
		/// </summary>
		CodeBlock CreateNewBlockAndFillItWithGarbage()
		{
			CodeBlock retValue = new CodeBlock((long)0, _pageSize);
			Random r = new Random();
			for (int i = 0; i < _pageSize; retValue[i++] = (byte)r.Next(0x100));
			return retValue;

			//For debug: return new CodeBlock(_pageSize, (byte)0x40);
		}

		/// <summary>
		/// Allocate dynamic memory.
		/// Might throw NoFreeMemoryException exception.
		/// </summary>
		/// <param name="iSize">The size of requested block</param>
		public int MemAllocation(int iSize)
		{
			if (HeapManager == null) throw new PanicException();
			return HeapManager.MemAllocation(iSize);

		}

		/// <summary>
		/// Free allocated memory
		/// </summary>
		/// <param name="iAddress">Address of the block to destroy</param>
		public void FreeMemory(uint uiAddress, int iSize)
		{
			if (HeapManager == null) throw new PanicException();
			HeapManager.FreeMemory(uiAddress, iSize);
		}

		#endregion

		#region Events

		public event MemoryAccessedFunc OnMemoryAccess;

		public event PageFaultFunc OnPageFault;

		#endregion

		#region Physical Memory Simulation

		public class PhysicalMemorySimulation
		{
			/// <summary>
			/// Conataining list of virtual pages
			/// </summary>
			private ArrayList VirtualPagesList;

			/// <summary>
			/// Conataining list of matching physical pages
			/// </summary>
			private ArrayList PhysicalPagesList;

			private int iMaxNumberOfPages;

			private int _pageSize;

			public event PageFaultFunc OnPageFault;


			public PhysicalMemorySimulation()
			{
				PhysicalPagesList = new ArrayList();
				VirtualPagesList = new ArrayList();

				iMaxNumberOfPages = Settings.Simulator.MemorySize / Settings.Simulator.PageSize;
				_pageSize = Settings.Simulator.PageSize;
			}

            /// <summary>
            /// Gets physical address from virutal address
            /// </summary>
            /// <param name="iVAddress">Virtual address</param>
            /// <returns>Physical Address</returns>
			public int GetPhysicalAddressFromVirtualAddress(int iVAddress)
			{
				int iPageOffset = (int)((uint)iVAddress % _pageSize);
				int iVirtualPage = (int)((uint)iVAddress / _pageSize);
				int iIndex = VirtualPagesList.IndexOf(iVirtualPage);
				return ((int)PhysicalPagesList[iIndex]) * _pageSize + iPageOffset;
			}


			/// <summary>
			/// Gets a virtual page and check if it there is a miss on the physical page
			/// </summary>
			/// <param name="iVirtualPage"></param>
			public void PhysicalPageHit(int iVirtualPage)
			{
				int iPhysicalPage;
				//
				// Page-Hit
				//
				if (VirtualPagesList.Contains(iVirtualPage))
				{
					// Move the page to the end of the lists (LRU implementation)
					int iIndex = VirtualPagesList.IndexOf(iVirtualPage);
					iPhysicalPage = (int)PhysicalPagesList[iIndex];
					VirtualPagesList.RemoveAt(iIndex);
					PhysicalPagesList.RemoveAt(iIndex);
				}

				//
				// Page-Fault
				//
				// There is space on the physical memory
				else if (VirtualPagesList.Count < iMaxNumberOfPages)
				{
					iPhysicalPage = GetFreePage();
					if (OnPageFault != null) OnPageFault(iVirtualPage, iPhysicalPage, false, 0);
				}
				// No free space - need to swap
				else
				{
					if (OnPageFault != null) OnPageFault(iVirtualPage, (int)PhysicalPagesList[0], true, (int)VirtualPagesList[0]);
					iPhysicalPage = (int)PhysicalPagesList[0];
					VirtualPagesList.RemoveAt(0);
					PhysicalPagesList.RemoveAt(0);
				}

				VirtualPagesList.Add(iVirtualPage);
				PhysicalPagesList.Add(iPhysicalPage);			
			}

			/// <summary>
			/// Returns an empty physical page
			/// </summary>
			/// <returns></returns>
			private int GetFreePage()
			{
				if (iMaxNumberOfPages == PhysicalPagesList.Count) throw new PanicException();

				// LOL, hack :)
				return PhysicalPagesList.Count;
			}
		}
		#endregion

		#region Heap Manager

		
		/// <summary>
		/// Heap Manager
		/// </summary>
		public class VAX11HeapManager
		{
			/// <summary>
			/// List of all the free memory blocks.
			/// </summary>
			private System.Collections.ArrayList FreeMemList;

			/// <summary>
			/// Base address for the heap
			/// </summary>
			private uint uiHeapBase;

			/// <summary>
			/// Total size of the heap
			/// </summary>
			private int iHeapSize;



			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="HeapBase">Base address for the heap</param>
			/// <param name="HeapSize">Size of the heap. we won't allocate more space if we reach that size</param>
			public VAX11HeapManager(uint HeapBase, int HeapSize)
			{
				iHeapSize = HeapSize;
				uiHeapBase = HeapBase;
				FreeMemList = new ArrayList();
				FreeMemList.Add(new FreeMemoryInformation(uiHeapBase, HeapSize));
			}

			/// <summary>
			/// Mark memory as allocated and returns its starting address. Changes nothing
			/// on the real simulator.
			/// Might throw NoFreeMemoryException exception.
			/// </summary>
			/// <param name="iSize">The size of requested block</param>
			public int MemAllocation(int iSize)
			{
				// Find block with size close to the requested size
				int iBlockLocation = FreeMemList.BinarySearch((FreeMemoryInformation)iSize);

				if (iBlockLocation < 0)
				{
					int iIndex = ~iBlockLocation;
					// If no block in the request size is free, throw exception
					if (iIndex == FreeMemList.Count) throw new NoFreeMemoryException();
				
					// Remove the old block from the list
					FreeMemoryInformation oldBlock = (FreeMemoryInformation)FreeMemList[iIndex];
					FreeMemList.RemoveAt(iIndex);
				
					// add new free block with the size that left
					FreeMemList.Add(new FreeMemoryInformation(oldBlock.uiAddress+(uint)iSize,
						oldBlock.iSize - iSize));
					FreeMemList.Sort();

					// return block
					return (int)oldBlock.uiAddress;
				}
				else
				{
					FreeMemoryInformation oldBlock = (FreeMemoryInformation)FreeMemList[iBlockLocation];
					FreeMemList.RemoveAt(iBlockLocation);
					return (int)oldBlock.uiAddress;
				}
			}

			/// <summary>
			/// Free allocated memory
			/// </summary>
			/// <param name="iAddress">Address of the block to destroy</param>
			public void FreeMemory(uint uiAddress, int iSize)
			{
				// If the memory is out of the heap area
				if (uiAddress < uiHeapBase || uiAddress + iSize > uiHeapBase + iHeapSize)
					throw new RuntimeError(SimulatorMessage.MEMORY_ACCESS_FAULT, (int)uiAddress);

				for (int iCounter = 0; iCounter < FreeMemList.Count; ++iCounter)
				{
					FreeMemoryInformation curBlock = (FreeMemoryInformation)FreeMemList[iCounter];

					// if the address is inside a free space
					if (uiAddress >= curBlock.uiAddress && 
						uiAddress < curBlock.uiAddress + curBlock.iSize) 
						throw new RuntimeError(SimulatorMessage.MEMORY_ACCESS_FAULT, (int)uiAddress);

						// if the end address is inside a free space
					else if (uiAddress + iSize > curBlock.uiAddress && 
						uiAddress + iSize < curBlock.uiAddress + curBlock.iSize)
						throw new RuntimeError(SimulatorMessage.MEMORY_ACCESS_FAULT, (int)uiAddress);

						// if the block to free includes a free area
					else if (uiAddress < curBlock.uiAddress && 
						uiAddress + iSize > curBlock.uiAddress + curBlock.iSize)
						throw new RuntimeError(SimulatorMessage.MEMORY_ACCESS_FAULT, (int)uiAddress);
				}


				FreeMemList.Add(new FreeMemoryInformation(uiAddress, iSize));
				FreeMemList.Sort();

			}

			public class NoFreeMemoryException : Exception {}


			/// <summary>
			/// Class for holding free memory blocks data
			/// </summary>
			private class FreeMemoryInformation : IComparable
			{
				public uint uiAddress;
				public int iSize;

				/// <summary>
				/// Constructor - fast initalize of the values
				/// </summary>
				/// <param name="address">address</param>
				/// <param name="size">size</param>
				public FreeMemoryInformation(uint address, int size)
				{
					uiAddress = address;
					iSize = size;
				}

				/// <summary>
				/// convert int to FreeMemoryInformation (in order to compare to FreeMemoryInformation).
				/// </summary>
				/// <param name="size">int value represent block size</param>
				/// <returns>new FreeMemoryInformation</returns>
				public static implicit operator FreeMemoryInformation(int size)
				{
					return new FreeMemoryInformation(0xFFFFFFFF, size);
				}

				public int CompareTo(object obj)
				{
					return iSize.CompareTo(((FreeMemoryInformation)obj).iSize);
				}
			}
		}

		#endregion

	}
}
