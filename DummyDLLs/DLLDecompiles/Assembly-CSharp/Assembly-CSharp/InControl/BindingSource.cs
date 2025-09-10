using System;
using System.IO;

namespace InControl
{
	// Token: 0x020008DA RID: 2266
	public abstract class BindingSource : IEquatable<BindingSource>
	{
		// Token: 0x06004F13 RID: 20243
		public abstract float GetValue(InputDevice inputDevice);

		// Token: 0x06004F14 RID: 20244
		public abstract bool GetState(InputDevice inputDevice);

		// Token: 0x06004F15 RID: 20245
		public abstract bool Equals(BindingSource other);

		// Token: 0x17000A33 RID: 2611
		// (get) Token: 0x06004F16 RID: 20246
		public abstract string Name { get; }

		// Token: 0x17000A34 RID: 2612
		// (get) Token: 0x06004F17 RID: 20247
		public abstract string DeviceName { get; }

		// Token: 0x17000A35 RID: 2613
		// (get) Token: 0x06004F18 RID: 20248
		public abstract InputDeviceClass DeviceClass { get; }

		// Token: 0x17000A36 RID: 2614
		// (get) Token: 0x06004F19 RID: 20249
		public abstract InputDeviceStyle DeviceStyle { get; }

		// Token: 0x06004F1A RID: 20250 RVA: 0x0016F705 File Offset: 0x0016D905
		public static bool operator ==(BindingSource a, BindingSource b)
		{
			return a == b || (a != null && b != null && a.BindingSourceType == b.BindingSourceType && a.Equals(b));
		}

		// Token: 0x06004F1B RID: 20251 RVA: 0x0016F72C File Offset: 0x0016D92C
		public static bool operator !=(BindingSource a, BindingSource b)
		{
			return !(a == b);
		}

		// Token: 0x06004F1C RID: 20252 RVA: 0x0016F738 File Offset: 0x0016D938
		public override bool Equals(object obj)
		{
			return this.Equals((BindingSource)obj);
		}

		// Token: 0x06004F1D RID: 20253 RVA: 0x0016F746 File Offset: 0x0016D946
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x17000A37 RID: 2615
		// (get) Token: 0x06004F1E RID: 20254
		public abstract BindingSourceType BindingSourceType { get; }

		// Token: 0x06004F1F RID: 20255
		public abstract void Save(BinaryWriter writer);

		// Token: 0x06004F20 RID: 20256
		public abstract void Load(BinaryReader reader, ushort dataFormatVersion);

		// Token: 0x17000A38 RID: 2616
		// (get) Token: 0x06004F21 RID: 20257 RVA: 0x0016F74E File Offset: 0x0016D94E
		// (set) Token: 0x06004F22 RID: 20258 RVA: 0x0016F756 File Offset: 0x0016D956
		internal PlayerAction BoundTo { get; set; }

		// Token: 0x17000A39 RID: 2617
		// (get) Token: 0x06004F23 RID: 20259 RVA: 0x0016F75F File Offset: 0x0016D95F
		internal virtual bool IsValid
		{
			get
			{
				return true;
			}
		}
	}
}
