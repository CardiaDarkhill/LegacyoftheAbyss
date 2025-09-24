using System;
using System.IO;
using InControl;

// Token: 0x02000461 RID: 1121
public sealed class PlaystationSwipeInputSource : BindingSource
{
	// Token: 0x17000484 RID: 1156
	// (get) Token: 0x0600282E RID: 10286 RVA: 0x000B1DFB File Offset: 0x000AFFFB
	// (set) Token: 0x0600282F RID: 10287 RVA: 0x000B1E03 File Offset: 0x000B0003
	public PlaystationSwipeInputSource.Swipe SwipeDirection { get; private set; }

	// Token: 0x06002830 RID: 10288 RVA: 0x000B1E0C File Offset: 0x000B000C
	private PlaystationSwipeInputSource()
	{
	}

	// Token: 0x06002831 RID: 10289 RVA: 0x000B1E14 File Offset: 0x000B0014
	public PlaystationSwipeInputSource(PlaystationSwipeInputSource.Swipe swipeDirection)
	{
		this.SwipeDirection = swipeDirection;
	}

	// Token: 0x06002832 RID: 10290 RVA: 0x000B1E23 File Offset: 0x000B0023
	public override float GetValue(InputDevice inputDevice)
	{
		return (float)(this.GetState(inputDevice) ? 1 : 0);
	}

	// Token: 0x06002833 RID: 10291 RVA: 0x000B1E33 File Offset: 0x000B0033
	public override bool GetState(InputDevice inputDevice)
	{
		return false;
	}

	// Token: 0x06002834 RID: 10292 RVA: 0x000B1E38 File Offset: 0x000B0038
	public override bool Equals(BindingSource other)
	{
		if (other == null)
		{
			return false;
		}
		PlaystationSwipeInputSource playstationSwipeInputSource = other as PlaystationSwipeInputSource;
		return playstationSwipeInputSource != null && this.SwipeDirection == playstationSwipeInputSource.SwipeDirection;
	}

	// Token: 0x17000485 RID: 1157
	// (get) Token: 0x06002835 RID: 10293 RVA: 0x000B1E70 File Offset: 0x000B0070
	public override string Name
	{
		get
		{
			return string.Format("Swipe {0}", this.SwipeDirection);
		}
	}

	// Token: 0x17000486 RID: 1158
	// (get) Token: 0x06002836 RID: 10294 RVA: 0x000B1E88 File Offset: 0x000B0088
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

	// Token: 0x17000487 RID: 1159
	// (get) Token: 0x06002837 RID: 10295 RVA: 0x000B1EC3 File Offset: 0x000B00C3
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

	// Token: 0x17000488 RID: 1160
	// (get) Token: 0x06002838 RID: 10296 RVA: 0x000B1EDF File Offset: 0x000B00DF
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

	// Token: 0x17000489 RID: 1161
	// (get) Token: 0x06002839 RID: 10297 RVA: 0x000B1EFB File Offset: 0x000B00FB
	public override BindingSourceType BindingSourceType
	{
		get
		{
			return BindingSourceType.DeviceBindingSource;
		}
	}

	// Token: 0x0600283A RID: 10298 RVA: 0x000B1EFE File Offset: 0x000B00FE
	public override void Save(BinaryWriter writer)
	{
		writer.Write((int)this.SwipeDirection);
	}

	// Token: 0x0600283B RID: 10299 RVA: 0x000B1F0C File Offset: 0x000B010C
	public override void Load(BinaryReader reader, ushort dataFormatVersion)
	{
		this.SwipeDirection = (PlaystationSwipeInputSource.Swipe)reader.ReadInt32();
	}

	// Token: 0x02001777 RID: 6007
	public enum Swipe
	{
		// Token: 0x04008E33 RID: 36403
		Up,
		// Token: 0x04008E34 RID: 36404
		Right,
		// Token: 0x04008E35 RID: 36405
		Down,
		// Token: 0x04008E36 RID: 36406
		Left
	}
}
