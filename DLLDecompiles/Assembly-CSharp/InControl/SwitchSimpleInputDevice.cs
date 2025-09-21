using System;

namespace InControl
{
	// Token: 0x0200091F RID: 2335
	public class SwitchSimpleInputDevice : InputDevice, VibrationManager.IVibrationMixerProvider
	{
		// Token: 0x060052AD RID: 21165 RVA: 0x0017AAF4 File Offset: 0x00178CF4
		public SwitchSimpleInputDevice() : base("Switch")
		{
			base.Meta = "JoyCon/Pro Controller";
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
			base.AddControl(InputControlType.Action1, "B");
			base.AddControl(InputControlType.Action2, "A");
			base.AddControl(InputControlType.Action3, "Y");
			base.AddControl(InputControlType.Action4, "X");
			base.AddControl(InputControlType.LeftBumper, "Left Bumper");
			base.AddControl(InputControlType.RightBumper, "Right Bumper");
			base.AddControl(InputControlType.LeftStickButton, "Left Stick Button");
			base.AddControl(InputControlType.RightStickButton, "Right Stick Button");
			base.AddControl(InputControlType.Select, "Minus");
			base.AddControl(InputControlType.Start, "Plus");
		}

		// Token: 0x060052AE RID: 21166 RVA: 0x0017ACEC File Offset: 0x00178EEC
		~SwitchSimpleInputDevice()
		{
		}

		// Token: 0x060052AF RID: 21167 RVA: 0x0017AD14 File Offset: 0x00178F14
		public override void Update(ulong updateTick, float deltaTime)
		{
			base.Commit(updateTick, deltaTime);
		}

		// Token: 0x17000B4C RID: 2892
		// (get) Token: 0x060052B0 RID: 21168 RVA: 0x0017AD1E File Offset: 0x00178F1E
		public bool IsConnected
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060052B1 RID: 21169 RVA: 0x0017AD21 File Offset: 0x00178F21
		VibrationMixer VibrationManager.IVibrationMixerProvider.GetVibrationMixer()
		{
			return null;
		}

		// Token: 0x040052D2 RID: 21202
		private const float LowerDeadZone = 0.2f;

		// Token: 0x040052D3 RID: 21203
		private const float UpperDeadZone = 0.9f;

		// Token: 0x040052D4 RID: 21204
		private const float AnalogStickNormalize = 3.051851E-05f;
	}
}
