using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x0200091B RID: 2331
	public class PS4SimpleInputDevice : InputDevice, VibrationManager.IVibrationMixerProvider
	{
		// Token: 0x0600529A RID: 21146 RVA: 0x0017A4F4 File Offset: 0x001786F4
		public PS4SimpleInputDevice() : base("DUALSHOCK®4")
		{
			base.Meta = "PS4 DUALSHOCK®4";
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
			this.vibrationMixer = new GamepadVibrationMixer(GamepadVibrationMixer.PlatformAdjustments.DualShock);
		}

		// Token: 0x0600529B RID: 21147 RVA: 0x0017A6FA File Offset: 0x001788FA
		public override void Update(ulong updateTick, float deltaTime)
		{
			base.Commit(updateTick, deltaTime);
		}

		// Token: 0x0600529C RID: 21148 RVA: 0x0017A704 File Offset: 0x00178904
		private static int GetNativeVibrationValue(float strength)
		{
			return Mathf.Clamp(Mathf.FloorToInt(strength * 256f), 0, 255);
		}

		// Token: 0x17000B48 RID: 2888
		// (get) Token: 0x0600529D RID: 21149 RVA: 0x0017A71D File Offset: 0x0017891D
		public bool IsConnected
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600529E RID: 21150 RVA: 0x0017A720 File Offset: 0x00178920
		VibrationMixer VibrationManager.IVibrationMixerProvider.GetVibrationMixer()
		{
			return null;
		}

		// Token: 0x040052C7 RID: 21191
		private const float LowerDeadZone = 0.2f;

		// Token: 0x040052C8 RID: 21192
		private const float UpperDeadZone = 0.9f;

		// Token: 0x040052C9 RID: 21193
		private GamepadVibrationMixer vibrationMixer;

		// Token: 0x040052CA RID: 21194
		private const int VibrationMotorMax = 255;

		// Token: 0x02001B62 RID: 7010
		private class ButtonMap
		{
			// Token: 0x04009C8B RID: 40075
			public InputControlType ControlType;

			// Token: 0x04009C8C RID: 40076
			public string ButtonName;

			// Token: 0x04009C8D RID: 40077
			public string UnityKeyName;
		}
	}
}
