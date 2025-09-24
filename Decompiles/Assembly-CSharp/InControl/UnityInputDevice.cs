using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000932 RID: 2354
	public class UnityInputDevice : InputDevice
	{
		// Token: 0x17000B7E RID: 2942
		// (get) Token: 0x06005389 RID: 21385 RVA: 0x0017DFB6 File Offset: 0x0017C1B6
		// (set) Token: 0x0600538A RID: 21386 RVA: 0x0017DFBE File Offset: 0x0017C1BE
		public int JoystickId { get; private set; }

		// Token: 0x0600538B RID: 21387 RVA: 0x0017DFC7 File Offset: 0x0017C1C7
		public UnityInputDevice(int joystickId, string joystickName) : this(null, joystickId, joystickName)
		{
		}

		// Token: 0x0600538C RID: 21388 RVA: 0x0017DFD4 File Offset: 0x0017C1D4
		public UnityInputDevice(InputDeviceProfile deviceProfile, int joystickId, string joystickName)
		{
			this.profile = deviceProfile;
			this.JoystickId = joystickId;
			if (joystickId != 0)
			{
				base.SortOrder = 100 + joystickId;
			}
			UnityInputDevice.SetupAnalogQueries();
			UnityInputDevice.SetupButtonQueries();
			base.AnalogSnapshot = null;
			if (this.IsKnown)
			{
				base.Name = this.profile.DeviceName;
				base.Meta = this.profile.DeviceNotes;
				base.DeviceClass = this.profile.DeviceClass;
				base.DeviceStyle = this.profile.DeviceStyle;
				int analogCount = this.profile.AnalogCount;
				for (int i = 0; i < analogCount; i++)
				{
					InputControlMapping inputControlMapping = this.profile.AnalogMappings[i];
					if (Utility.TargetIsAlias(inputControlMapping.Target))
					{
						Logger.LogError(string.Concat(new string[]
						{
							"Cannot map control \"",
							inputControlMapping.Name,
							"\" as InputControlType.",
							inputControlMapping.Target.ToString(),
							" in profile \"",
							deviceProfile.DeviceName,
							"\" because this target is reserved as an alias. The mapping will be ignored."
						}));
					}
					else
					{
						InputControl inputControl = base.AddControl(inputControlMapping.Target, inputControlMapping.Name);
						inputControl.Sensitivity = Mathf.Min(this.profile.Sensitivity, inputControlMapping.Sensitivity);
						inputControl.LowerDeadZone = Mathf.Max(this.profile.LowerDeadZone, inputControlMapping.LowerDeadZone);
						inputControl.UpperDeadZone = Mathf.Min(this.profile.UpperDeadZone, inputControlMapping.UpperDeadZone);
						inputControl.Raw = inputControlMapping.Raw;
						inputControl.Passive = inputControlMapping.Passive;
					}
				}
				int buttonCount = this.profile.ButtonCount;
				for (int j = 0; j < buttonCount; j++)
				{
					InputControlMapping inputControlMapping2 = this.profile.ButtonMappings[j];
					if (Utility.TargetIsAlias(inputControlMapping2.Target))
					{
						Logger.LogError(string.Concat(new string[]
						{
							"Cannot map control \"",
							inputControlMapping2.Name,
							"\" as InputControlType.",
							inputControlMapping2.Target.ToString(),
							" in profile \"",
							deviceProfile.DeviceName,
							"\" because this target is reserved as an alias. The mapping will be ignored."
						}));
					}
					else
					{
						base.AddControl(inputControlMapping2.Target, inputControlMapping2.Name).Passive = inputControlMapping2.Passive;
					}
				}
				return;
			}
			base.Name = "Unknown Device";
			base.Meta = "\"" + joystickName + "\"";
			for (int k = 0; k < this.NumUnknownButtons; k++)
			{
				base.AddControl(InputControlType.Button0 + k, "Button " + k.ToString());
			}
			for (int l = 0; l < this.NumUnknownAnalogs; l++)
			{
				base.AddControl(InputControlType.Analog0 + l, "Analog " + l.ToString(), 0.2f, 0.9f);
			}
		}

		// Token: 0x0600538D RID: 21389 RVA: 0x0017E2C8 File Offset: 0x0017C4C8
		public override void Update(ulong updateTick, float deltaTime)
		{
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

		// Token: 0x0600538E RID: 21390 RVA: 0x0017E404 File Offset: 0x0017C604
		private static void SetupAnalogQueries()
		{
			if (UnityInputDevice.analogQueries == null)
			{
				UnityInputDevice.analogQueries = new string[10, 20];
				for (int i = 1; i <= 10; i++)
				{
					for (int j = 0; j < 20; j++)
					{
						UnityInputDevice.analogQueries[i - 1, j] = "joystick " + i.ToString() + " analog " + j.ToString();
					}
				}
			}
		}

		// Token: 0x0600538F RID: 21391 RVA: 0x0017E46C File Offset: 0x0017C66C
		private static void SetupButtonQueries()
		{
			if (UnityInputDevice.buttonQueries == null)
			{
				UnityInputDevice.buttonQueries = new string[10, 20];
				for (int i = 1; i <= 10; i++)
				{
					for (int j = 0; j < 20; j++)
					{
						UnityInputDevice.buttonQueries[i - 1, j] = "joystick " + i.ToString() + " button " + j.ToString();
					}
				}
			}
		}

		// Token: 0x06005390 RID: 21392 RVA: 0x0017E4D3 File Offset: 0x0017C6D3
		public override bool ReadRawButtonState(int index)
		{
			return index < 20 && Input.GetKey(UnityInputDevice.buttonQueries[this.JoystickId - 1, index]);
		}

		// Token: 0x06005391 RID: 21393 RVA: 0x0017E4F4 File Offset: 0x0017C6F4
		public override float ReadRawAnalogValue(int index)
		{
			if (index < 20)
			{
				return Input.GetAxisRaw(UnityInputDevice.analogQueries[this.JoystickId - 1, index]);
			}
			return 0f;
		}

		// Token: 0x17000B7F RID: 2943
		// (get) Token: 0x06005392 RID: 21394 RVA: 0x0017E519 File Offset: 0x0017C719
		public override bool IsSupportedOnThisPlatform
		{
			get
			{
				return this.profile == null || this.profile.IsSupportedOnThisPlatform;
			}
		}

		// Token: 0x17000B80 RID: 2944
		// (get) Token: 0x06005393 RID: 21395 RVA: 0x0017E530 File Offset: 0x0017C730
		public override bool IsKnown
		{
			get
			{
				return this.profile != null;
			}
		}

		// Token: 0x17000B81 RID: 2945
		// (get) Token: 0x06005394 RID: 21396 RVA: 0x0017E53B File Offset: 0x0017C73B
		public override int NumUnknownButtons
		{
			get
			{
				return 20;
			}
		}

		// Token: 0x17000B82 RID: 2946
		// (get) Token: 0x06005395 RID: 21397 RVA: 0x0017E53F File Offset: 0x0017C73F
		public override int NumUnknownAnalogs
		{
			get
			{
				return 20;
			}
		}

		// Token: 0x0400537C RID: 21372
		private static string[,] analogQueries;

		// Token: 0x0400537D RID: 21373
		private static string[,] buttonQueries;

		// Token: 0x0400537E RID: 21374
		public const int MaxDevices = 10;

		// Token: 0x0400537F RID: 21375
		public const int MaxButtons = 20;

		// Token: 0x04005380 RID: 21376
		public const int MaxAnalogs = 20;

		// Token: 0x04005382 RID: 21378
		private readonly InputDeviceProfile profile;
	}
}
