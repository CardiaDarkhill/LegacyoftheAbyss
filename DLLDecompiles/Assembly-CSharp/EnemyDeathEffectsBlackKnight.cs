using System;
using UnityEngine;

// Token: 0x020002D2 RID: 722
public class EnemyDeathEffectsBlackKnight : EnemyDeathEffects
{
	// Token: 0x060019BF RID: 6591 RVA: 0x000763E8 File Offset: 0x000745E8
	protected override void EmitEffects(GameObject corpseObj)
	{
		if (corpseObj != null)
		{
			SpriteFlash component = corpseObj.GetComponent<SpriteFlash>();
			if (component != null)
			{
				component.flashFocusHeal();
			}
		}
		base.ShakeCameraIfVisible("AverageShake");
	}
}
