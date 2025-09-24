using System;
using UnityEngine;

// Token: 0x02000473 RID: 1139
[CreateAssetMenu(menuName = "Platforms/Vibration/Vibration Data Asset")]
public sealed class VibrationDataAsset : ScriptableObject
{
	// Token: 0x1700049B RID: 1179
	// (get) Token: 0x060028B1 RID: 10417 RVA: 0x000B300C File Offset: 0x000B120C
	public VibrationData VibrationData
	{
		get
		{
			if (!this.disable)
			{
				return this.vibrationData;
			}
			return default(VibrationData);
		}
	}

	// Token: 0x060028B2 RID: 10418 RVA: 0x000B3034 File Offset: 0x000B1234
	public static implicit operator VibrationData(VibrationDataAsset asset)
	{
		if (asset)
		{
			return asset.VibrationData;
		}
		return default(VibrationData);
	}

	// Token: 0x060028B3 RID: 10419 RVA: 0x000B3059 File Offset: 0x000B1259
	public void SetData(VibrationData vibrationData)
	{
		this.vibrationData = vibrationData;
	}

	// Token: 0x040024B7 RID: 9399
	[SerializeField]
	private VibrationData vibrationData;

	// Token: 0x040024B8 RID: 9400
	[SerializeField]
	private bool disable;

	// Token: 0x040024B9 RID: 9401
	[TextArea(1, 20)]
	[SerializeField]
	private string comments;
}
