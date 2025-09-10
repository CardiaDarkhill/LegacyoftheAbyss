using System;

// Token: 0x02000155 RID: 341
public static class CameraShakeConverter
{
	// Token: 0x06000A5B RID: 2651 RVA: 0x0002EB84 File Offset: 0x0002CD84
	public static CameraShakeWorldForceFlag ToFlag(this CameraShakeWorldForceIntensities intensity)
	{
		CameraShakeWorldForceFlag result;
		switch (intensity)
		{
		case CameraShakeWorldForceIntensities.None:
			result = CameraShakeWorldForceFlag.None;
			break;
		case CameraShakeWorldForceIntensities.Small:
			result = CameraShakeWorldForceFlag.Small;
			break;
		case CameraShakeWorldForceIntensities.Medium:
			result = CameraShakeWorldForceFlag.Medium;
			break;
		case CameraShakeWorldForceIntensities.Intense:
			result = CameraShakeWorldForceFlag.Intense;
			break;
		default:
			result = CameraShakeWorldForceFlag.None;
			break;
		}
		return result;
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x0002EBBC File Offset: 0x0002CDBC
	public static CameraShakeWorldForceFlag ToFlagMax(this CameraShakeWorldForceIntensities intensity)
	{
		CameraShakeWorldForceFlag result;
		switch (intensity)
		{
		case CameraShakeWorldForceIntensities.None:
			result = CameraShakeWorldForceFlag.None;
			break;
		case CameraShakeWorldForceIntensities.Small:
			result = CameraShakeWorldForceFlag.Small;
			break;
		case CameraShakeWorldForceIntensities.Medium:
			result = CameraShakeWorldForceFlag.Medium;
			break;
		case CameraShakeWorldForceIntensities.Intense:
			result = CameraShakeWorldForceFlag.Intense;
			break;
		default:
			result = CameraShakeWorldForceFlag.None;
			break;
		}
		return result;
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x0002EBF4 File Offset: 0x0002CDF4
	public static CameraShakeWorldForceIntensities ToIntensity(this CameraShakeWorldForceFlag flag)
	{
		switch (flag)
		{
		case CameraShakeWorldForceFlag.None:
			return CameraShakeWorldForceIntensities.None;
		case CameraShakeWorldForceFlag.Small:
			return CameraShakeWorldForceIntensities.Small;
		case CameraShakeWorldForceFlag.Medium:
			return CameraShakeWorldForceIntensities.Medium;
		case CameraShakeWorldForceFlag.Intense:
			return CameraShakeWorldForceIntensities.Intense;
		}
		return CameraShakeWorldForceIntensities.None;
	}
}
