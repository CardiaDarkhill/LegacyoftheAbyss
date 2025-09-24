using System;
using System.IO;

namespace InControl
{
	// Token: 0x020008E1 RID: 2273
	public class KeyBindingSource : BindingSource
	{
		// Token: 0x17000A41 RID: 2625
		// (get) Token: 0x06004F3E RID: 20286 RVA: 0x0016FABC File Offset: 0x0016DCBC
		// (set) Token: 0x06004F3F RID: 20287 RVA: 0x0016FAC4 File Offset: 0x0016DCC4
		public KeyCombo Control { get; protected set; }

		// Token: 0x06004F40 RID: 20288 RVA: 0x0016FACD File Offset: 0x0016DCCD
		internal KeyBindingSource()
		{
		}

		// Token: 0x06004F41 RID: 20289 RVA: 0x0016FAD5 File Offset: 0x0016DCD5
		public KeyBindingSource(KeyCombo keyCombo)
		{
			this.Control = keyCombo;
		}

		// Token: 0x06004F42 RID: 20290 RVA: 0x0016FAE4 File Offset: 0x0016DCE4
		public KeyBindingSource(params Key[] keys)
		{
			this.Control = new KeyCombo(keys);
		}

		// Token: 0x06004F43 RID: 20291 RVA: 0x0016FAF8 File Offset: 0x0016DCF8
		public override float GetValue(InputDevice inputDevice)
		{
			if (!this.GetState(inputDevice))
			{
				return 0f;
			}
			return 1f;
		}

		// Token: 0x06004F44 RID: 20292 RVA: 0x0016FB10 File Offset: 0x0016DD10
		public override bool GetState(InputDevice inputDevice)
		{
			return this.Control.IsPressed;
		}

		// Token: 0x17000A42 RID: 2626
		// (get) Token: 0x06004F45 RID: 20293 RVA: 0x0016FB2C File Offset: 0x0016DD2C
		public override string Name
		{
			get
			{
				return this.Control.ToString();
			}
		}

		// Token: 0x17000A43 RID: 2627
		// (get) Token: 0x06004F46 RID: 20294 RVA: 0x0016FB4D File Offset: 0x0016DD4D
		public override string DeviceName
		{
			get
			{
				return "Keyboard";
			}
		}

		// Token: 0x17000A44 RID: 2628
		// (get) Token: 0x06004F47 RID: 20295 RVA: 0x0016FB54 File Offset: 0x0016DD54
		public override InputDeviceClass DeviceClass
		{
			get
			{
				return InputDeviceClass.Keyboard;
			}
		}

		// Token: 0x17000A45 RID: 2629
		// (get) Token: 0x06004F48 RID: 20296 RVA: 0x0016FB57 File Offset: 0x0016DD57
		public override InputDeviceStyle DeviceStyle
		{
			get
			{
				return InputDeviceStyle.Unknown;
			}
		}

		// Token: 0x06004F49 RID: 20297 RVA: 0x0016FB5C File Offset: 0x0016DD5C
		public override bool Equals(BindingSource other)
		{
			if (other == null)
			{
				return false;
			}
			KeyBindingSource keyBindingSource = other as KeyBindingSource;
			return keyBindingSource != null && this.Control == keyBindingSource.Control;
		}

		// Token: 0x06004F4A RID: 20298 RVA: 0x0016FB98 File Offset: 0x0016DD98
		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			KeyBindingSource keyBindingSource = other as KeyBindingSource;
			return keyBindingSource != null && this.Control == keyBindingSource.Control;
		}

		// Token: 0x06004F4B RID: 20299 RVA: 0x0016FBD0 File Offset: 0x0016DDD0
		public override int GetHashCode()
		{
			return this.Control.GetHashCode();
		}

		// Token: 0x17000A46 RID: 2630
		// (get) Token: 0x06004F4C RID: 20300 RVA: 0x0016FBF1 File Offset: 0x0016DDF1
		public override BindingSourceType BindingSourceType
		{
			get
			{
				return BindingSourceType.KeyBindingSource;
			}
		}

		// Token: 0x06004F4D RID: 20301 RVA: 0x0016FBF4 File Offset: 0x0016DDF4
		public override void Load(BinaryReader reader, ushort dataFormatVersion)
		{
			KeyCombo control = default(KeyCombo);
			control.Load(reader, dataFormatVersion);
			this.Control = control;
		}

		// Token: 0x06004F4E RID: 20302 RVA: 0x0016FC1C File Offset: 0x0016DE1C
		public override void Save(BinaryWriter writer)
		{
			this.Control.Save(writer);
		}
	}
}
