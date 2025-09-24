using System;
using System.IO;

namespace InControl
{
	// Token: 0x020008DE RID: 2270
	public class DeviceBindingSource : BindingSource
	{
		// Token: 0x17000A3A RID: 2618
		// (get) Token: 0x06004F27 RID: 20263 RVA: 0x0016F76A File Offset: 0x0016D96A
		// (set) Token: 0x06004F28 RID: 20264 RVA: 0x0016F772 File Offset: 0x0016D972
		public InputControlType Control { get; protected set; }

		// Token: 0x06004F29 RID: 20265 RVA: 0x0016F77B File Offset: 0x0016D97B
		internal DeviceBindingSource()
		{
			this.Control = InputControlType.None;
		}

		// Token: 0x06004F2A RID: 20266 RVA: 0x0016F78A File Offset: 0x0016D98A
		public DeviceBindingSource(InputControlType control)
		{
			this.Control = control;
		}

		// Token: 0x06004F2B RID: 20267 RVA: 0x0016F799 File Offset: 0x0016D999
		public override float GetValue(InputDevice inputDevice)
		{
			if (inputDevice == null)
			{
				return 0f;
			}
			return inputDevice.GetControl(this.Control).Value;
		}

		// Token: 0x06004F2C RID: 20268 RVA: 0x0016F7B5 File Offset: 0x0016D9B5
		public override bool GetState(InputDevice inputDevice)
		{
			return inputDevice != null && inputDevice.GetControl(this.Control).State;
		}

		// Token: 0x17000A3B RID: 2619
		// (get) Token: 0x06004F2D RID: 20269 RVA: 0x0016F7D0 File Offset: 0x0016D9D0
		public override string Name
		{
			get
			{
				if (base.BoundTo == null)
				{
					return "";
				}
				InputDevice device = base.BoundTo.Device;
				if (device.GetControl(this.Control) == InputControl.Null)
				{
					return this.Control.ToString();
				}
				return device.GetControl(this.Control).Handle;
			}
		}

		// Token: 0x17000A3C RID: 2620
		// (get) Token: 0x06004F2E RID: 20270 RVA: 0x0016F830 File Offset: 0x0016DA30
		public override string DeviceName
		{
			get
			{
				if (base.BoundTo == null)
				{
					return "";
				}
				InputDevice device = base.BoundTo.Device;
				if (device == InputDevice.Null)
				{
					return "Controller";
				}
				return device.Name;
			}
		}

		// Token: 0x17000A3D RID: 2621
		// (get) Token: 0x06004F2F RID: 20271 RVA: 0x0016F86B File Offset: 0x0016DA6B
		public override InputDeviceClass DeviceClass
		{
			get
			{
				if (base.BoundTo != null)
				{
					return base.BoundTo.Device.DeviceClass;
				}
				return InputDeviceClass.Unknown;
			}
		}

		// Token: 0x17000A3E RID: 2622
		// (get) Token: 0x06004F30 RID: 20272 RVA: 0x0016F887 File Offset: 0x0016DA87
		public override InputDeviceStyle DeviceStyle
		{
			get
			{
				if (base.BoundTo != null)
				{
					return base.BoundTo.Device.DeviceStyle;
				}
				return InputDeviceStyle.Unknown;
			}
		}

		// Token: 0x06004F31 RID: 20273 RVA: 0x0016F8A4 File Offset: 0x0016DAA4
		public override bool Equals(BindingSource other)
		{
			if (other == null)
			{
				return false;
			}
			DeviceBindingSource deviceBindingSource = other as DeviceBindingSource;
			return deviceBindingSource != null && this.Control == deviceBindingSource.Control;
		}

		// Token: 0x06004F32 RID: 20274 RVA: 0x0016F8DC File Offset: 0x0016DADC
		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			DeviceBindingSource deviceBindingSource = other as DeviceBindingSource;
			return deviceBindingSource != null && this.Control == deviceBindingSource.Control;
		}

		// Token: 0x06004F33 RID: 20275 RVA: 0x0016F910 File Offset: 0x0016DB10
		public override int GetHashCode()
		{
			return this.Control.GetHashCode();
		}

		// Token: 0x17000A3F RID: 2623
		// (get) Token: 0x06004F34 RID: 20276 RVA: 0x0016F931 File Offset: 0x0016DB31
		public override BindingSourceType BindingSourceType
		{
			get
			{
				return BindingSourceType.DeviceBindingSource;
			}
		}

		// Token: 0x06004F35 RID: 20277 RVA: 0x0016F934 File Offset: 0x0016DB34
		public override void Save(BinaryWriter writer)
		{
			writer.Write((int)this.Control);
		}

		// Token: 0x06004F36 RID: 20278 RVA: 0x0016F942 File Offset: 0x0016DB42
		public override void Load(BinaryReader reader, ushort dataFormatVersion)
		{
			this.Control = (InputControlType)reader.ReadInt32();
		}

		// Token: 0x17000A40 RID: 2624
		// (get) Token: 0x06004F37 RID: 20279 RVA: 0x0016F950 File Offset: 0x0016DB50
		internal override bool IsValid
		{
			get
			{
				if (base.BoundTo == null)
				{
					Logger.LogError("Cannot query property 'IsValid' for unbound BindingSource.");
					return false;
				}
				return base.BoundTo.Device.HasControl(this.Control) || Utility.TargetIsStandard(this.Control);
			}
		}
	}
}
