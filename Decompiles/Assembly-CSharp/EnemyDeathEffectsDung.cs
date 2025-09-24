using System;
using UnityEngine;

// Token: 0x020002D4 RID: 724
public class EnemyDeathEffectsDung : EnemyDeathEffects
{
	// Token: 0x060019C3 RID: 6595 RVA: 0x0007649C File Offset: 0x0007469C
	protected override void EmitEffects(GameObject corpseObj)
	{
		if (corpseObj != null)
		{
			SpriteFlash component = corpseObj.GetComponent<SpriteFlash>();
			if (component != null)
			{
				component.flashDung();
			}
		}
		this.deathPuffDung.Spawn(base.transform.position + this.effectOrigin);
		base.EmitSound();
		base.ShakeCameraIfVisible("AverageShake");
		GameManager.instance.FreezeMoment(1);
	}

	// Token: 0x040018B7 RID: 6327
	[Header("Dung Variables")]
	public GameObject deathPuffDung;
}
