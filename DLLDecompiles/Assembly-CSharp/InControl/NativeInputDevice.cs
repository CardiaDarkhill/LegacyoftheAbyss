using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000915 RID: 2325
	public class NativeInputDevice : InputDevice
	{
		// Token: 0x17000B41 RID: 2881
		// (get) Token: 0x0600526F RID: 21103 RVA: 0x00178B57 File Offset: 0x00176D57
		// (set) Token: 0x06005270 RID: 21104 RVA: 0x00178B5F File Offset: 0x00176D5F
		public uint Handle { get; private set; }

		// Token: 0x17000B42 RID: 2882
		// (get) Token: 0x06005271 RID: 21105 RVA: 0x00178B68 File Offset: 0x00176D68
		// (set) Token: 0x06005272 RID: 21106 RVA: 0x00178B70 File Offset: 0x00176D70
		public InputDeviceInfo Info { get; private set; }

		// Token: 0x06005273 RID: 21107 RVA: 0x00178B79 File Offset: 0x00176D79
		internal NativeInputDevice()
		{
		}

		// Token: 0x06005274 RID: 21108 RVA: 0x00178B94 File Offset: 0x00176D94
		internal void Initialize(uint deviceHandle, InputDeviceInfo deviceInfo, InputDeviceProfile deviceProfile)
		{
			this.Handle = deviceHandle;
			this.Info = deviceInfo;
			this.profile = deviceProfile;
			base.SortOrder = (int)(1000U + this.Handle);
			this.numUnknownButtons = Math.Min((int)this.Info.numButtons, 20);
			this.numUnknownAnalogs = Math.Min((int)this.Info.numAnalogs, 20);
			this.buttons = new short[this.Info.numButtons];
			this.analogs = new short[this.Info.numAnalogs];
			base.AnalogSnapshot = null;
			this.controlSourceByTarget = new InputControlSource[531];
			base.ClearInputState();
			base.ClearControls();
			if (this.IsKnown)
			{
				base.Name = (this.profile.DeviceName ?? this.Info.name);
				base.Name = base.Name.Replace("{NAME}", this.Info.name).Trim();
				base.Meta = (this.profile.DeviceNotes ?? this.Info.name);
				base.DeviceClass = this.profile.DeviceClass;
				base.DeviceStyle = this.profile.DeviceStyle;
				int analogCount = this.profile.AnalogCount;
				for (int i = 0; i < analogCount; i++)
				{
					InputControlMapping inputControlMapping = this.profile.AnalogMappings[i];
					InputControl inputControl = base.AddControl(inputControlMapping.Target, inputControlMapping.Name);
					inputControl.Sensitivity = Mathf.Min(this.profile.Sensitivity, inputControlMapping.Sensitivity);
					inputControl.LowerDeadZone = Mathf.Max(this.profile.LowerDeadZone, inputControlMapping.LowerDeadZone);
					inputControl.UpperDeadZone = Mathf.Min(this.profile.UpperDeadZone, inputControlMapping.UpperDeadZone);
					inputControl.Raw = inputControlMapping.Raw;
					inputControl.Passive = inputControlMapping.Passive;
					this.controlSourceByTarget[(int)inputControlMapping.Target] = inputControlMapping.Source;
				}
				int buttonCount = this.profile.ButtonCount;
				for (int j = 0; j < buttonCount; j++)
				{
					InputControlMapping inputControlMapping2 = this.profile.ButtonMappings[j];
					base.AddControl(inputControlMapping2.Target, inputControlMapping2.Name).Passive = inputControlMapping2.Passive;
					this.controlSourceByTarget[(int)inputControlMapping2.Target] = inputControlMapping2.Source;
				}
			}
			else
			{
				base.Name = "Unknown Device";
				base.Meta = this.Info.name;
				for (int k = 0; k < this.NumUnknownButtons; k++)
				{
					base.AddControl(InputControlType.Button0 + k, "Button " + k.ToString());
				}
				for (int l = 0; l < this.NumUnknownAnalogs; l++)
				{
					base.AddControl(InputControlType.Analog0 + l, "Analog " + l.ToString(), 0.2f, 0.9f);
				}
			}
			this.skipUpdateFrames = 1;
		}

		// Token: 0x06005275 RID: 21109 RVA: 0x00178E9B File Offset: 0x0017709B
		internal void Initialize(uint deviceHandle, InputDeviceInfo deviceInfo)
		{
			this.Initialize(deviceHandle, deviceInfo, this.profile);
		}

		// Token: 0x06005276 RID: 21110 RVA: 0x00178EAC File Offset: 0x001770AC
		public override void Update(ulong updateTick, float deltaTime)
		{
			this.SendStatusUpdates();
			if (this.skipUpdateFrames > 0)
			{
				this.skipUpdateFrames--;
				return;
			}
			IntPtr source;
			if (Native.GetDeviceState(this.Handle, out source))
			{
				Marshal.Copy(source, this.buttons, 0, this.buttons.Length);
				source = new IntPtr(source.ToInt64() + (long)(this.buttons.Length * 2));
				Marshal.Copy(source, this.analogs, 0, this.analogs.Length);
			}
			if (this.IsKnown)
			{
				int analogCount = this.profile.AnalogCount;
				for (int i = 0; i < analogCount; i++)
				{
					InputControlMapping inputControlMapping = this.profile.AnalogMappings[i];
					float value = inputControlMapping.Source.GetValue(this);
					InputControl control = base.GetControl(inputControlMapping.Target);
					if (!inputControlMapping.IgnoreInitialZeroValue || !control.IsOnZeroTick || !Utility.IsZero(value))
					{
						float value2 = inputControlMapping.ApplyToValue(value);
						control.UpdateWithValue(value2, updateTick, deltaTime);
					}
				}
				int buttonCount = this.profile.ButtonCount;
				for (int j = 0; j < buttonCount; j++)
				{
					InputControlMapping inputControlMapping2 = this.profile.ButtonMappings[j];
					bool state = inputControlMapping2.Source.GetState(this);
					base.UpdateWithState(inputControlMapping2.Target, state, updateTick, deltaTime);
				}
				return;
			}
			for (int k = 0; k < this.NumUnknownButtons; k++)
			{
				base.UpdateWithState(InputControlType.Button0 + k, this.ReadRawButtonState(k), updateTick, deltaTime);
			}
			for (int l = 0; l < this.NumUnknownAnalogs; l++)
			{
				base.UpdateWithValue(InputControlType.Analog0 + l, this.ReadRawAnalogValue(l), updateTick, deltaTime);
			}
		}

		// Token: 0x06005277 RID: 21111 RVA: 0x0017905B File Offset: 0x0017725B
		public override bool ReadRawButtonState(int index)
		{
			return index < this.buttons.Length && this.buttons[index] > -32767;
		}

		// Token: 0x06005278 RID: 21112 RVA: 0x00179079 File Offset: 0x00177279
		public override float ReadRawAnalogValue(int index)
		{
			if (index < this.analogs.Length)
			{
				return (float)this.analogs[index] / 32767f;
			}
			return 0f;
		}

		// Token: 0x06005279 RID: 21113 RVA: 0x0017909B File Offset: 0x0017729B
		private static byte FloatToByte(float value)
		{
			return (byte)(Mathf.Clamp01(value) * 255f);
		}

		// Token: 0x0600527A RID: 21114 RVA: 0x001790AA File Offset: 0x001772AA
		public override void Vibrate(float leftSpeed, float rightSpeed)
		{
			this.sendVibrate = true;
			this.vibrateToSend = new Vector2(leftSpeed, rightSpeed);
		}

		// Token: 0x0600527B RID: 21115 RVA: 0x001790C0 File Offset: 0x001772C0
		public override void VibrateTriggers(float leftTriggerSpeed, float rightTriggerSpeed)
		{
			this.sendVibrateTriggers = true;
			this.vibrateTriggersToSend = new Vector2(leftTriggerSpeed, rightTriggerSpeed);
		}

		// Token: 0x0600527C RID: 21116 RVA: 0x001790D6 File Offset: 0x001772D6
		public override void SetLightColor(float red, float green, float blue)
		{
			this.sendLightColor = true;
			this.lightColorToSend = new Vector3(red, green, blue);
		}

		// Token: 0x0600527D RID: 21117 RVA: 0x001790ED File Offset: 0x001772ED
		public override void SetLightFlash(float flashOnDuration, float flashOffDuration)
		{
			this.sendLightFlash = true;
			this.lightFlashToSend = new Vector2(flashOnDuration, flashOffDuration);
		}

		// Token: 0x0600527E RID: 21118 RVA: 0x00179104 File Offset: 0x00177304
		private void SendStatusUpdates()
		{
			if (this.sendVibrate && InputManager.CurrentTime - this.lastTimeVibrateWasSent > 0.02f)
			{
				Native.SetHapticState(this.Handle, NativeInputDevice.FloatToByte(this.vibrateToSend.x), NativeInputDevice.FloatToByte(this.vibrateToSend.y));
				this.sendVibrate = false;
				this.lastTimeVibrateWasSent = InputManager.CurrentTime;
				this.vibrateToSend = Vector2.zero;
			}
			if (this.sendVibrateTriggers && InputManager.CurrentTime - this.lastTimeVibrateTriggersWasSent > 0.02f)
			{
				Native.SetTriggersHapticState(this.Handle, NativeInputDevice.FloatToByte(this.vibrateTriggersToSend.x), NativeInputDevice.FloatToByte(this.vibrateTriggersToSend.y));
				this.sendVibrateTriggers = false;
				this.lastTimeVibrateTriggersWasSent = InputManager.CurrentTime;
				this.vibrateTriggersToSend = Vector2.zero;
			}
			if (this.sendLightColor && InputManager.CurrentTime - this.lastTimeLightColorWasSent > 0.02f)
			{
				Native.SetLightColor(this.Handle, NativeInputDevice.FloatToByte(this.lightColorToSend.x), NativeInputDevice.FloatToByte(this.lightColorToSend.y), NativeInputDevice.FloatToByte(this.lightColorToSend.z));
				this.sendLightColor = false;
				this.lastTimeLightColorWasSent = InputManager.CurrentTime;
				this.lightColorToSend = Vector3.zero;
			}
			if (this.sendLightFlash && InputManager.CurrentTime - this.lastTimeLightFlashWasSent > 0.02f)
			{
				Native.SetLightFlash(this.Handle, NativeInputDevice.FloatToByte(this.lightFlashToSend.x), NativeInputDevice.FloatToByte(this.lightFlashToSend.y));
				this.sendLightFlash = false;
				this.lastTimeLightFlashWasSent = InputManager.CurrentTime;
				this.lightFlashToSend = Vector2.zero;
			}
		}

		// Token: 0x0600527F RID: 21119 RVA: 0x001792B0 File Offset: 0x001774B0
		public string GetAppleGlyphNameForControl(InputControlType controlType)
		{
			InputControlSource inputControlSource = this.controlSourceByTarget[(int)controlType];
			if (inputControlSource.SourceType != InputControlSourceType.None)
			{
				InputControlSourceType sourceType = inputControlSource.SourceType;
				IntPtr zero;
				uint num;
				if (sourceType != InputControlSourceType.Button)
				{
					if (sourceType != InputControlSourceType.Analog)
					{
						zero = IntPtr.Zero;
						num = 0U;
					}
					else
					{
						num = Native.GetAnalogGlyphName(this.Handle, (uint)inputControlSource.Index, out zero);
					}
				}
				else
				{
					num = Native.GetButtonGlyphName(this.Handle, (uint)inputControlSource.Index, out zero);
				}
				if (num > 0U)
				{
					this.glyphName.Clear();
					int num2 = 0;
					while ((long)num2 < (long)((ulong)num))
					{
						this.glyphName.Append((char)Marshal.ReadByte(zero, num2));
						num2++;
					}
					return this.glyphName.ToString();
				}
			}
			return "";
		}

		// Token: 0x06005280 RID: 21120 RVA: 0x00179368 File Offset: 0x00177568
		public bool HasSameVendorID(InputDeviceInfo deviceInfo)
		{
			return this.Info.HasSameVendorID(deviceInfo);
		}

		// Token: 0x06005281 RID: 21121 RVA: 0x00179384 File Offset: 0x00177584
		public bool HasSameProductID(InputDeviceInfo deviceInfo)
		{
			return this.Info.HasSameProductID(deviceInfo);
		}

		// Token: 0x06005282 RID: 21122 RVA: 0x001793A0 File Offset: 0x001775A0
		public bool HasSameVersionNumber(InputDeviceInfo deviceInfo)
		{
			return this.Info.HasSameVersionNumber(deviceInfo);
		}

		// Token: 0x06005283 RID: 21123 RVA: 0x001793BC File Offset: 0x001775BC
		public bool HasSameLocation(InputDeviceInfo deviceInfo)
		{
			return this.Info.HasSameLocation(deviceInfo);
		}

		// Token: 0x06005284 RID: 21124 RVA: 0x001793D8 File Offset: 0x001775D8
		public bool HasSameSerialNumber(InputDeviceInfo deviceInfo)
		{
			return this.Info.HasSameSerialNumber(deviceInfo);
		}

		// Token: 0x17000B43 RID: 2883
		// (get) Token: 0x06005285 RID: 21125 RVA: 0x001793F4 File Offset: 0x001775F4
		public string ProfileName
		{
			get
			{
				if (this.profile != null)
				{
					return this.profile.GetType().Name;
				}
				return "N/A";
			}
		}

		// Token: 0x17000B44 RID: 2884
		// (get) Token: 0x06005286 RID: 21126 RVA: 0x00179414 File Offset: 0x00177614
		public override bool IsSupportedOnThisPlatform
		{
			get
			{
				return this.profile == null || this.profile.IsSupportedOnThisPlatform;
			}
		}

		// Token: 0x17000B45 RID: 2885
		// (get) Token: 0x06005287 RID: 21127 RVA: 0x0017942B File Offset: 0x0017762B
		public override bool IsKnown
		{
			get
			{
				return this.profile != null;
			}
		}

		// Token: 0x17000B46 RID: 2886
		// (get) Token: 0x06005288 RID: 21128 RVA: 0x00179436 File Offset: 0x00177636
		public override int NumUnknownButtons
		{
			get
			{
				return this.numUnknownButtons;
			}
		}

		// Token: 0x17000B47 RID: 2887
		// (get) Token: 0x06005289 RID: 21129 RVA: 0x0017943E File Offset: 0x0017763E
		public override int NumUnknownAnalogs
		{
			get
			{
				return this.numUnknownAnalogs;
			}
		}

		// Token: 0x0400529F RID: 21151
		private const int maxUnknownButtons = 20;

		// Token: 0x040052A0 RID: 21152
		private const int maxUnknownAnalogs = 20;

		// Token: 0x040052A3 RID: 21155
		private short[] buttons;

		// Token: 0x040052A4 RID: 21156
		private short[] analogs;

		// Token: 0x040052A5 RID: 21157
		private InputDeviceProfile profile;

		// Token: 0x040052A6 RID: 21158
		private int skipUpdateFrames;

		// Token: 0x040052A7 RID: 21159
		private int numUnknownButtons;

		// Token: 0x040052A8 RID: 21160
		private int numUnknownAnalogs;

		// Token: 0x040052A9 RID: 21161
		private InputControlSource[] controlSourceByTarget;

		// Token: 0x040052AA RID: 21162
		private bool sendVibrate;

		// Token: 0x040052AB RID: 21163
		private float lastTimeVibrateWasSent;

		// Token: 0x040052AC RID: 21164
		private Vector2 vibrateToSend;

		// Token: 0x040052AD RID: 21165
		private bool sendVibrateTriggers;

		// Token: 0x040052AE RID: 21166
		private float lastTimeVibrateTriggersWasSent;

		// Token: 0x040052AF RID: 21167
		private Vector2 vibrateTriggersToSend;

		// Token: 0x040052B0 RID: 21168
		private bool sendLightColor;

		// Token: 0x040052B1 RID: 21169
		private float lastTimeLightColorWasSent;

		// Token: 0x040052B2 RID: 21170
		private Vector3 lightColorToSend;

		// Token: 0x040052B3 RID: 21171
		private bool sendLightFlash;

		// Token: 0x040052B4 RID: 21172
		private float lastTimeLightFlashWasSent;

		// Token: 0x040052B5 RID: 21173
		private Vector2 lightFlashToSend;

		// Token: 0x040052B6 RID: 21174
		private readonly StringBuilder glyphName = new StringBuilder(256);

		// Token: 0x040052B7 RID: 21175
		private const string defaultGlyphName = "";
	}
}
