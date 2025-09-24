using System;
using System.IO;

namespace InControl
{
	// Token: 0x020008EB RID: 2283
	public class UnknownDeviceBindingSource : BindingSource
	{
		// Token: 0x17000A70 RID: 2672
		// (get) Token: 0x06004FFC RID: 20476 RVA: 0x00171EDB File Offset: 0x001700DB
		// (set) Token: 0x06004FFD RID: 20477 RVA: 0x00171EE3 File Offset: 0x001700E3
		public UnknownDeviceControl Control { get; protected set; }

		// Token: 0x06004FFE RID: 20478 RVA: 0x00171EEC File Offset: 0x001700EC
		internal UnknownDeviceBindingSource()
		{
			this.Control = UnknownDeviceControl.None;
		}

		// Token: 0x06004FFF RID: 20479 RVA: 0x00171EFF File Offset: 0x001700FF
		public UnknownDeviceBindingSource(UnknownDeviceControl control)
		{
			this.Control = control;
		}

		// Token: 0x06005000 RID: 20480 RVA: 0x00171F10 File Offset: 0x00170110
		public override float GetValue(InputDevice device)
		{
			return this.Control.GetValue(device);
		}

		// Token: 0x06005001 RID: 20481 RVA: 0x00171F2C File Offset: 0x0017012C
		public override bool GetState(InputDevice device)
		{
			return device != null && Utility.IsNotZero(this.GetValue(device));
		}

		// Token: 0x17000A71 RID: 2673
		// (get) Token: 0x06005002 RID: 20482 RVA: 0x00171F40 File Offset: 0x00170140
		public override string Name
		{
			get
			{
				if (base.BoundTo == null)
				{
					return "";
				}
				string text = "";
				if (this.Control.SourceRange == InputRangeType.ZeroToMinusOne)
				{
					text = "Negative ";
				}
				else if (this.Control.SourceRange == InputRangeType.ZeroToOne)
				{
					text = "Positive ";
				}
				InputDevice device = base.BoundTo.Device;
				if (device == InputDevice.Null)
				{
					string str = text;
					UnknownDeviceControl control = this.Control;
					return str + control.Control.ToString();
				}
				InputControl control2 = device.GetControl(this.Control.Control);
				if (control2 == InputControl.Null)
				{
					string str2 = text;
					UnknownDeviceControl control = this.Control;
					return str2 + control.Control.ToString();
				}
				return text + control2.Handle;
			}
		}

		// Token: 0x17000A72 RID: 2674
		// (get) Token: 0x06005003 RID: 20483 RVA: 0x00172008 File Offset: 0x00170208
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
					return "Unknown Controller";
				}
				return device.Name;
			}
		}

		// Token: 0x17000A73 RID: 2675
		// (get) Token: 0x06005004 RID: 20484 RVA: 0x00172043 File Offset: 0x00170243
		public override InputDeviceClass DeviceClass
		{
			get
			{
				return InputDeviceClass.Controller;
			}
		}

		// Token: 0x17000A74 RID: 2676
		// (get) Token: 0x06005005 RID: 20485 RVA: 0x00172046 File Offset: 0x00170246
		public override InputDeviceStyle DeviceStyle
		{
			get
			{
				return InputDeviceStyle.Unknown;
			}
		}

		// Token: 0x06005006 RID: 20486 RVA: 0x0017204C File Offset: 0x0017024C
		public override bool Equals(BindingSource other)
		{
			if (other == null)
			{
				return false;
			}
			UnknownDeviceBindingSource unknownDeviceBindingSource = other as UnknownDeviceBindingSource;
			return unknownDeviceBindingSource != null && this.Control == unknownDeviceBindingSource.Control;
		}

		// Token: 0x06005007 RID: 20487 RVA: 0x00172088 File Offset: 0x00170288
		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			UnknownDeviceBindingSource unknownDeviceBindingSource = other as UnknownDeviceBindingSource;
			return unknownDeviceBindingSource != null && this.Control == unknownDeviceBindingSource.Control;
		}

		// Token: 0x06005008 RID: 20488 RVA: 0x001720C0 File Offset: 0x001702C0
		public override int GetHashCode()
		{
			return this.Control.GetHashCode();
		}

		// Token: 0x17000A75 RID: 2677
		// (get) Token: 0x06005009 RID: 20489 RVA: 0x001720E1 File Offset: 0x001702E1
		public override BindingSourceType BindingSourceType
		{
			get
			{
				return BindingSourceType.UnknownDeviceBindingSource;
			}
		}

		// Token: 0x17000A76 RID: 2678
		// (get) Token: 0x0600500A RID: 20490 RVA: 0x001720E4 File Offset: 0x001702E4
		internal override bool IsValid
		{
			get
			{
				if (base.BoundTo == null)
				{
					Logger.LogError("Cannot query property 'IsValid' for unbound BindingSource.");
					return false;
				}
				InputDevice device = base.BoundTo.Device;
				return device == InputDevice.Null || device.HasControl(this.Control.Control);
			}
		}

		// Token: 0x0600500B RID: 20491 RVA: 0x0017212C File Offset: 0x0017032C
		public override void Load(BinaryReader reader, ushort dataFormatVersion)
		{
			UnknownDeviceControl control = default(UnknownDeviceControl);
			control.Load(reader);
			this.Control = control;
		}

		// Token: 0x0600500C RID: 20492 RVA: 0x00172150 File Offset: 0x00170350
		public override void Save(BinaryWriter writer)
		{
			this.Control.Save(writer);
		}
	}
}
