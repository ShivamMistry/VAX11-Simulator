using System;

namespace VAX11Simulator
{
	public enum SimulatorEvents
	{
		RESERVED_OPERAND = 0,
		ARITHMETIC = 1,
		CLOCK_INTERRUPT = 2,
		OUTPUT_INTERRUPT = 3,
		INPUT_INTERRUPT = 4,
		POWER_DOWN_INTERRUPT = 5
		//CLOCK_TICK
	};


	public enum SimulatorEventsTypes
	{
		INTERRUPT,
		FAULT,
		TRAP,
		TRAP_OR_FAULT
	}

	/// <summary>
	/// Type Code to push when there is arithmetic exception
	/// </summary>
	public enum ARITHMETIC_TRAPS
	{
		INTEGER_OVERFLOW = 1,
		INTEGER_DIVIDE_BY_ZERO = 2,
		FLOATING_OVERFLOW = 3,
		FLOATING_DIVIDE_BY_ZERO = 4,
		FLOATING_UNDERFLOW = 5,
		DECIMAL_OVERFLOW = 6,
		SUBSCRIPT_RANGE = 7
	};

	public class SimEvent
	{
		private SimulatorEvents _e;
		public SimulatorEvents e
		{
			get { return _e; }
		}

        private int _SCBB_OFFSET;
		public int SCBB_OFFSET
		{
			get { return _SCBB_OFFSET; }
		}

		private bool _EventOccured;
		public bool EventOccured
		{
			get { return _EventOccured; }
			set { _EventOccured = value; } 
		}

		private int _IPL;
		public int IPL
		{
			get { return _IPL; }
		}

		private SimulatorEventsTypes _type;
		public SimulatorEventsTypes Type
		{
			get { return _type; }
		}

		public SimEvent(SimulatorEvents vEvent, int vOffset, int vIPL, SimulatorEventsTypes vType)
		{
			_e = vEvent;
			_SCBB_OFFSET = vOffset;
			_IPL = vIPL;
			_EventOccured = false;
			_type = vType;
		}

		public static SimEvent[] GenerateVAX11EventsVector()
		{
			// For future additions: this array is ordered by IPL from maximum to minimum.
			// Also it need to be placed in SimulatorEvents in the same order, as we use the number as index of the array
			SimEvent[] retValue = 
			{
				new SimEvent(SimulatorEvents.RESERVED_OPERAND,	0x18, 31, SimulatorEventsTypes.FAULT),
				new SimEvent(SimulatorEvents.ARITHMETIC,		0x34, 31, SimulatorEventsTypes.TRAP),
				new SimEvent(SimulatorEvents.CLOCK_INTERRUPT,	0xC0, 24, SimulatorEventsTypes.INTERRUPT),
				new SimEvent(SimulatorEvents.OUTPUT_INTERRUPT,	0xFC, 20, SimulatorEventsTypes.INTERRUPT),
				new SimEvent(SimulatorEvents.INPUT_INTERRUPT,	0xF8, 20, SimulatorEventsTypes.INTERRUPT)
			};
            return retValue;
		}
	}
}