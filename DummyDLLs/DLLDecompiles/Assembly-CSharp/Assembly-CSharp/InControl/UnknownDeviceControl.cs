using System;
using System.IO;

namespace InControl
{
	// Token: 0x020008ED RID: 2285
	public struct UnknownDeviceControl : IEquatable<UnknownDeviceControl>
	{
		// Token: 0x06005013 RID: 20499 RVA: 0x001722B0 File Offset: 0x001704B0
		public UnknownDeviceControl(InputControlType control, InputRangeType sourceRange)
		{
			this.Control = control;
			this.SourceRange = sourceRange;
			this.IsButton = Utility.TargetIsButton(control);
			this.IsAnalog = !this.IsButton;
		}

		// Token: 0x06005014 RID: 20500 RVA: 0x001722DB File Offset: 0x001704DB
		internal float GetValue(InputDevice device)
		{
			if (device == null)
			{
				return 0f;
			}
			return InputRange.Remap(device.GetControl(this.Control).Value, this.SourceRange, InputRangeType.ZeroToOne);
		}

		// Token: 0x17000A77 RID: 2679
		// (get) Token: 0x06005015 RID: 20501 RVA: 0x00172303 File Offset: 0x00170503
		public int Index
		{
			get
			{
				return this.Control - (this.IsButton ? InputControlType.Button0 : InputControlType.Analog0);
			}
		}

		// Token: 0x06005016 RID: 20502 RVA: 0x00172320 File Offset: 0x00170520
		public static bool operator ==(UnknownDeviceControl a, UnknownDeviceControl b)
		{
			if (a == null)
			{
				return b == null;
			}
			return a.Equals(b);
		}

		// Token: 0x06005017 RID: 20503 RVA: 0x0017233C File Offset: 0x0017053C
		public static bool operator !=(UnknownDeviceControl a, UnknownDeviceControl b)
		{
			return !(a == b);
		}

		// Token: 0x06005018 RID: 20504 RVA: 0x00172348 File Offset: 0x00170548
		public bool Equals(UnknownDeviceControl other)
		{
			return this.Control == other.Control && this.SourceRange == other.SourceRange;
		}

		// Token: 0x06005019 RID: 20505 RVA: 0x00172368 File Offset: 0x00170568
		public override bool Equals(object other)
		{
			return this.Equals((UnknownDeviceControl)other);
		}

		// Token: 0x0600501A RID: 20506 RVA: 0x00172376 File Offset: 0x00170576
		public override int GetHashCode()
		{
			return this.Control.GetHashCode() ^ this.SourceRange.GetHashCode();
		}

		// Token: 0x0600501B RID: 20507 RVA: 0x0017239B File Offset: 0x0017059B
		public static implicit operator bool(UnknownDeviceControl control)
		{
			return control.Control > InputControlType.None;
		}

		// Token: 0x0600501C RID: 20508 RVA: 0x001723A6 File Offset: 0x001705A6
		public override string ToString()
		{
			return string.Format("UnknownDeviceControl( {0}, {1} )", this.Control.ToString(), this.SourceRange.ToString());
		}

		// Token: 0x0600501D RID: 20509 RVA: 0x001723D4 File Offset: 0x001705D4
		internal void Save(BinaryWriter writer)
		{
			writer.Write((int)this.Control);
			writer.Write((int)this.SourceRange);
		}

		// Token: 0x0600501E RID: 20510 RVA: 0x001723EE File Offset: 0x001705EE
		internal void Load(BinaryReader reader)
		{
			this.Control = (InputControlType)reader.ReadInt32();
			this.SourceRange = (InputRangeType)reader.ReadInt32();
			this.IsButton = Utility.TargetIsButton(this.Control);
			this.IsAnalog = !this.IsButton;
		}

		// Token: 0x0400509C RID: 20636
		public static readonly UnknownDeviceControl None = new UnknownDeviceControl(InputControlType.None, InputRangeType.None);

		// Token: 0x0400509D RID: 20637
		public InputControlType Control;

		// Token: 0x0400509E RID: 20638
		public InputRangeType SourceRange;

		// Token: 0x0400509F RID: 20639
		public bool IsButton;

		// Token: 0x040050A0 RID: 20640
		public bool IsAnalog;
	}
}
