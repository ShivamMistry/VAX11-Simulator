using System;
using System.Runtime.InteropServices;

namespace PCSpeakerLib
{
	public enum PCSpeaker
	{
        DefaultDurTime = 300
	}

	// Unmanaged win32 calls
	// Only make one instance
	// But what about other Apps?
	public sealed class MessageBeepPInvoke
	{
		private MessageBeepPInvoke() {}
		public static readonly MessageBeepPInvoke theMessageBeepPInvoke = 
			new MessageBeepPInvoke();
		///////////////////////////////////////////////////////////////////
		[DllImport("kernel32.dll")]
		public static extern bool Beep(int frequency, int duration);
		[DllImport("user32.dll")]
		public static extern bool MessageBeep(OldBeepTypes beepType);
		///////////////////////////////////////////////////////////////////
		public enum OldBeepTypes : uint 
		{
			SimpleBeep = 0xffffffff,
			IconAsterisk = 0x00000040,
			IconExclamation = 0x00000030,
			IconHand = 0x00000010,
			IconQuestion = 0x00000020,
			Ok = 0x00000000
		};
		public void PlayBeep(int frequency, int duration)
		{
			Beep(frequency, duration);
		}
		public void PlayMessageBeep(OldBeepTypes msg)
		{
			MessageBeep(msg);
		}
	}
}