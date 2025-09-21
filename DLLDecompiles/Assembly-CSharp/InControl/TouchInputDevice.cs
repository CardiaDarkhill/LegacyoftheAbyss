using System;

namespace InControl
{
	// Token: 0x0200092A RID: 2346
	public class TouchInputDevice : InputDevice
	{
		// Token: 0x06005326 RID: 21286 RVA: 0x0017C860 File Offset: 0x0017AA60
		public TouchInputDevice() : base("Touch Input Device", true)
		{
			base.DeviceClass = InputDeviceClass.TouchScreen;
		}
	}
}
