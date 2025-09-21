using System;
using System.Runtime.InteropServices;

namespace XInputDotNetPure
{
	// Token: 0x020008D0 RID: 2256
	internal class Imports
	{
		// Token: 0x06004EE7 RID: 20199
		[DllImport("XInputInterface32", EntryPoint = "XInputGamePadGetState")]
		public static extern uint XInputGamePadGetState32(uint playerIndex, IntPtr state);

		// Token: 0x06004EE8 RID: 20200
		[DllImport("XInputInterface32", EntryPoint = "XInputGamePadSetState")]
		public static extern void XInputGamePadSetState32(uint playerIndex, float leftMotor, float rightMotor);

		// Token: 0x06004EE9 RID: 20201
		[DllImport("XInputInterface64", EntryPoint = "XInputGamePadGetState")]
		public static extern uint XInputGamePadGetState64(uint playerIndex, IntPtr state);

		// Token: 0x06004EEA RID: 20202
		[DllImport("XInputInterface64", EntryPoint = "XInputGamePadSetState")]
		public static extern void XInputGamePadSetState64(uint playerIndex, float leftMotor, float rightMotor);

		// Token: 0x06004EEB RID: 20203 RVA: 0x0016F244 File Offset: 0x0016D444
		public static uint XInputGamePadGetState(uint playerIndex, IntPtr state)
		{
			if (IntPtr.Size == 4)
			{
				return Imports.XInputGamePadGetState32(playerIndex, state);
			}
			return Imports.XInputGamePadGetState64(playerIndex, state);
		}

		// Token: 0x06004EEC RID: 20204 RVA: 0x0016F25D File Offset: 0x0016D45D
		public static void XInputGamePadSetState(uint playerIndex, float leftMotor, float rightMotor)
		{
			if (IntPtr.Size == 4)
			{
				Imports.XInputGamePadSetState32(playerIndex, leftMotor, rightMotor);
				return;
			}
			Imports.XInputGamePadSetState64(playerIndex, leftMotor, rightMotor);
		}
	}
}
