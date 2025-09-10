using System;
using System.Runtime.InteropServices;

namespace InControl
{
	// Token: 0x02000914 RID: 2324
	internal static class Native
	{
		// Token: 0x06005263 RID: 21091
		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_Init")]
		public static extern void Init(NativeInputOptions options);

		// Token: 0x06005264 RID: 21092
		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_Stop")]
		public static extern void Stop();

		// Token: 0x06005265 RID: 21093
		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_GetVersionInfo")]
		public static extern void GetVersionInfo(out NativeVersionInfo versionInfo);

		// Token: 0x06005266 RID: 21094
		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_GetDeviceInfo")]
		public static extern bool GetDeviceInfo(uint handle, out InputDeviceInfo deviceInfo);

		// Token: 0x06005267 RID: 21095
		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_GetDeviceState")]
		public static extern bool GetDeviceState(uint handle, out IntPtr deviceState);

		// Token: 0x06005268 RID: 21096
		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_GetDeviceEvents")]
		public static extern int GetDeviceEvents(out IntPtr deviceEvents);

		// Token: 0x06005269 RID: 21097
		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_SetHapticState")]
		public static extern void SetHapticState(uint handle, byte lowFrequency, byte highFrequency);

		// Token: 0x0600526A RID: 21098
		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_SetTriggersHapticState")]
		public static extern void SetTriggersHapticState(uint handle, byte leftTrigger, byte rightTrigger);

		// Token: 0x0600526B RID: 21099
		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_SetLightColor")]
		public static extern void SetLightColor(uint handle, byte red, byte green, byte blue);

		// Token: 0x0600526C RID: 21100
		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_SetLightFlash")]
		public static extern void SetLightFlash(uint handle, byte flashOnDuration, byte flashOffDuration);

		// Token: 0x0600526D RID: 21101
		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_GetAnalogGlyphName")]
		public static extern uint GetAnalogGlyphName(uint handle, uint index, out IntPtr glyphName);

		// Token: 0x0600526E RID: 21102
		[DllImport("InControlNative", CallingConvention = CallingConvention.Cdecl, EntryPoint = "InControl_GetButtonGlyphName")]
		public static extern uint GetButtonGlyphName(uint handle, uint index, out IntPtr glyphName);

		// Token: 0x0400529D RID: 21149
		private const string libraryName = "InControlNative";

		// Token: 0x0400529E RID: 21150
		private const CallingConvention callingConvention = CallingConvention.Cdecl;
	}
}
