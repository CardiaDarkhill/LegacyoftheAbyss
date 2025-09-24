using System;
using UnityEngine;

// Token: 0x02000317 RID: 791
public class SetNailDamage : MonoBehaviour
{
	// Token: 0x06001BED RID: 7149 RVA: 0x00082163 File Offset: 0x00080363
	private void OnEnable()
	{
		if (!this.damager)
		{
			return;
		}
		this.damager.damageDealt = Mathf.FloorToInt((float)PlayerData.instance.nailDamage * this.multiplier);
	}

	// Token: 0x04001AEB RID: 6891
	[SerializeField]
	private DamageEnemies damager;

	// Token: 0x04001AEC RID: 6892
	[Space]
	[SerializeField]
	private float multiplier;
}
