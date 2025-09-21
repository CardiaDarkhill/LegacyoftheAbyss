using System;
using UnityEngine;

// Token: 0x020002D5 RID: 725
public class EnemyDeathEffectsNoEffect : EnemyDeathEffects
{
	// Token: 0x060019C5 RID: 6597 RVA: 0x00076510 File Offset: 0x00074710
	protected override void EmitEffects(GameObject corpseObj)
	{
		if (corpseObj != null)
		{
			SpriteFlash component = corpseObj.GetComponent<SpriteFlash>();
			if (component != null)
			{
				component.FlashEnemyHit();
			}
		}
		base.ShakeCameraIfVisible();
	}
}
