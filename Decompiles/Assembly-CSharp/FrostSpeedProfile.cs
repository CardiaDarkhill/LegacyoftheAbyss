using System;
using UnityEngine;

// Token: 0x020000D7 RID: 215
[CreateAssetMenu(menuName = "Profiles/Frost Speed")]
public class FrostSpeedProfile : ScriptableObject
{
	// Token: 0x17000080 RID: 128
	// (get) Token: 0x060006CF RID: 1743 RVA: 0x0002264F File Offset: 0x0002084F
	public float FrostSpeed
	{
		get
		{
			return this.frostSpeed;
		}
	}

	// Token: 0x040006AD RID: 1709
	[SerializeField]
	private float frostSpeed;
}
