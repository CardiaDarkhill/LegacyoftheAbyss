using System;
using UnityEngine;

// Token: 0x020002D3 RID: 723
public class EnemyDeathEffectsBubble : EnemyDeathEffects
{
	// Token: 0x060019C1 RID: 6593 RVA: 0x00076428 File Offset: 0x00074628
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
		base.ShakeCameraIfVisible("EnemyKillShake");
		GameManager.instance.FreezeMoment(1);
		Object.Instantiate<GameObject>(this.bubblePopPrefab, base.transform.position + this.effectOrigin, Quaternion.identity);
	}

	// Token: 0x040018B6 RID: 6326
	[Header("Bubble Effects")]
	public GameObject bubblePopPrefab;
}
