using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000903 RID: 2307
	public enum InputDeviceDriverType : ushort
	{
		// Token: 0x040051F9 RID: 20985
		Unknown,
		// Token: 0x040051FA RID: 20986
		HID,
		// Token: 0x040051FB RID: 20987
		USB,
		// Token: 0x040051FC RID: 20988
		Bluetooth,
		// Token: 0x040051FD RID: 20989
		[InspectorName("XInput")]
		XInput,
		// Token: 0x040051FE RID: 20990
		[InspectorName("DirectInput")]
		DirectInput,
		// Token: 0x040051FF RID: 20991
		[InspectorName("RawInput")]
		RawInput,
		// Token: 0x04005200 RID: 20992
		[InspectorName("AppleGameController")]
		AppleGameController,
		// Token: 0x04005201 RID: 20993
		[InspectorName("SDLJoystick")]
		SDLJoystick,
		// Token: 0x04005202 RID: 20994
		[InspectorName("SDLController")]
		SDLController
	}
}
