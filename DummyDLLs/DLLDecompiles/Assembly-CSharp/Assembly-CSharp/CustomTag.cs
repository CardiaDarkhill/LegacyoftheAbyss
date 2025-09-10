using System;
using UnityEngine;

// Token: 0x020001A3 RID: 419
public class CustomTag : MonoBehaviour
{
	// Token: 0x17000196 RID: 406
	// (get) Token: 0x06001042 RID: 4162 RVA: 0x0004E5F7 File Offset: 0x0004C7F7
	public CustomTag.CustomTagTypes CustomTagType
	{
		get
		{
			return this.customTag;
		}
	}

	// Token: 0x06001043 RID: 4163 RVA: 0x0004E5FF File Offset: 0x0004C7FF
	public string GetCustomTagAsString()
	{
		return this.customTag.ToString();
	}

	// Token: 0x06001044 RID: 4164 RVA: 0x0004E612 File Offset: 0x0004C812
	public bool IsSuckable()
	{
		return this.customTag == CustomTag.CustomTagTypes.Bomb || this.customTag == CustomTag.CustomTagTypes.DustBomb || this.customTag == CustomTag.CustomTagTypes.LightningBola || this.customTag == CustomTag.CustomTagTypes.SkinnyMosquito;
	}

	// Token: 0x04000FC2 RID: 4034
	[SerializeField]
	private CustomTag.CustomTagTypes customTag;

	// Token: 0x020014E2 RID: 5346
	public enum CustomTagTypes
	{
		// Token: 0x0400850E RID: 34062
		Bomb,
		// Token: 0x0400850F RID: 34063
		DustBomb,
		// Token: 0x04008510 RID: 34064
		LightningBola,
		// Token: 0x04008511 RID: 34065
		LightningBolaBall,
		// Token: 0x04008512 RID: 34066
		SkinnyMosquito,
		// Token: 0x04008513 RID: 34067
		CogworkHatchling,
		// Token: 0x04008514 RID: 34068
		PreventBlackThread
	}
}
