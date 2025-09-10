using System;
using System.Runtime.InteropServices;

namespace XInputDotNetPure
{
	// Token: 0x020008D8 RID: 2264
	public class GamePad
	{
		// Token: 0x06004F0B RID: 20235 RVA: 0x0016F630 File Offset: 0x0016D830
		public static GamePadState GetState(PlayerIndex playerIndex)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(GamePadState.RawState)));
			int num = (int)Imports.XInputGamePadGetState((uint)playerIndex, intPtr);
			GamePadState.RawState rawState = (GamePadState.RawState)Marshal.PtrToStructure(intPtr, typeof(GamePadState.RawState));
			return new GamePadState(num == 0, rawState);
		}

		// Token: 0x06004F0C RID: 20236 RVA: 0x0016F678 File Offset: 0x0016D878
		public static void SetVibration(PlayerIndex playerIndex, float leftMotor, float rightMotor)
		{
			Imports.XInputGamePadSetState((uint)playerIndex, leftMotor, rightMotor);
		}
	}
}
