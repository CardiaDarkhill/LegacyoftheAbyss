using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x0200091D RID: 2333
	public class PS5SimpleInputDevice : InputDevice
	{
		// Token: 0x060052A4 RID: 21156 RVA: 0x0017A7FC File Offset: 0x001789FC
		public PS5SimpleInputDevice() : base("DualSense™")
		{
			base.Meta = "PS5 DualSense™";
			base.AddControl(InputControlType.LeftStickLeft, "Left Stick Left", 0.2f, 0.9f);
			base.AddControl(InputControlType.LeftStickRight, "Left Stick Right", 0.2f, 0.9f);
			base.AddControl(InputControlType.LeftStickUp, "Left Stick Up", 0.2f, 0.9f);
			base.AddControl(InputControlType.LeftStickDown, "Left Stick Down", 0.2f, 0.9f);
			base.AddControl(InputControlType.RightStickLeft, "Right Stick Left", 0.2f, 0.9f);
			base.AddControl(InputControlType.RightStickRight, "Right Stick Right", 0.2f, 0.9f);
			base.AddControl(InputControlType.RightStickUp, "Right Stick Up", 0.2f, 0.9f);
			base.AddControl(InputControlType.RightStickDown, "Right Stick Down", 0.2f, 0.9f);
			base.AddControl(InputControlType.LeftTrigger, "Left Trigger", 0.2f, 0.9f);
			base.AddControl(InputControlType.RightTrigger, "Right Trigger", 0.2f, 0.9f);
			base.AddControl(InputControlType.DPadUp, "DPad Up", 0.2f, 0.9f);
			base.AddControl(InputControlType.DPadDown, "DPad Down", 0.2f, 0.9f);
			base.AddControl(InputControlType.DPadLeft, "DPad Left", 0.2f, 0.9f);
			base.AddControl(InputControlType.DPadRight, "DPad Right", 0.2f, 0.9f);
			base.AddControl(InputControlType.Action1, "Cross");
			base.AddControl(InputControlType.Action2, "Circle");
			base.AddControl(InputControlType.Action3, "Square");
			base.AddControl(InputControlType.Action4, "Triangle");
			base.AddControl(InputControlType.LeftBumper, "Left Bumper");
			base.AddControl(InputControlType.RightBumper, "Right Bumper");
			base.AddControl(InputControlType.LeftStickButton, "Left Stick Button");
			base.AddControl(InputControlType.RightStickButton, "Right Stick Button");
			base.AddControl(InputControlType.TouchPadButton, "Touchpad Click");
			base.AddControl(InputControlType.Options, "Options");
		}

		// Token: 0x060052A5 RID: 21157 RVA: 0x0017A9F6 File Offset: 0x00178BF6
		public override void Update(ulong updateTick, float deltaTime)
		{
			base.Commit(updateTick, deltaTime);
		}

		// Token: 0x060052A6 RID: 21158 RVA: 0x0017AA00 File Offset: 0x00178C00
		private static int GetNativeVibrationValue(float strength)
		{
			return Mathf.Clamp(Mathf.FloorToInt(strength * 256f), 0, 255);
		}

		// Token: 0x17000B4A RID: 2890
		// (get) Token: 0x060052A7 RID: 21159 RVA: 0x0017AA19 File Offset: 0x00178C19
		public bool IsConnected
		{
			get
			{
				return false;
			}
		}

		// Token: 0x040052CD RID: 21197
		private const float LowerDeadZone = 0.2f;

		// Token: 0x040052CE RID: 21198
		private const float UpperDeadZone = 0.9f;

		// Token: 0x040052CF RID: 21199
		private const int VibrationMotorMax = 255;

		// Token: 0x02001B63 RID: 7011
		private class ButtonMap
		{
			// Token: 0x04009C8E RID: 40078
			public InputControlType ControlType;

			// Token: 0x04009C8F RID: 40079
			public string ButtonName;

			// Token: 0x04009C90 RID: 40080
			public string UnityKeyName;
		}
	}
}
