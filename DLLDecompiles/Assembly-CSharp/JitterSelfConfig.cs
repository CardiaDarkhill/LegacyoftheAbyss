using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020003F5 RID: 1013
[Serializable]
public struct JitterSelfConfig
{
	// Token: 0x060022A0 RID: 8864 RVA: 0x0009F434 File Offset: 0x0009D634
	private bool IsInEditMode()
	{
		return !Application.isPlaying;
	}

	// Token: 0x04002171 RID: 8561
	[Tooltip("How often to move per second. Set to 0 for every frame.")]
	public float Frequency;

	// Token: 0x04002172 RID: 8562
	public Vector3 AmountMin;

	// Token: 0x04002173 RID: 8563
	public Vector3 AmountMax;

	// Token: 0x04002174 RID: 8564
	[ModifiableProperty]
	[Conditional("IsInEditMode", true, true, false)]
	public bool UseCameraRenderHooks;

	// Token: 0x04002175 RID: 8565
	public MinMaxFloat Delay;
}
