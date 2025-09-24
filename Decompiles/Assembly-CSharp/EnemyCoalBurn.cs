using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200022E RID: 558
public class EnemyCoalBurn : MonoBehaviour
{
	// Token: 0x06001494 RID: 5268 RVA: 0x0005CAEC File Offset: 0x0005ACEC
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer != 11 || this.currentlyBurning.ContainsKey(collision.gameObject))
		{
			return;
		}
		GameObject gameObject = collision.gameObject;
		HealthManager component = gameObject.GetComponent<HealthManager>();
		if (component && component.ImmuneToCoal)
		{
			return;
		}
		Vector3 position = collision.transform.position;
		PlayParticleEffects playParticleEffects = this.burningEffectPrefab.Spawn<PlayParticleEffects>();
		playParticleEffects.StopParticleSystems();
		playParticleEffects.ClearParticleSystems();
		Collider2D component2 = gameObject.GetComponent<Collider2D>();
		if (component2)
		{
			Bounds bounds = component2.bounds;
			float num = position.y - bounds.min.y;
			playParticleEffects.transform.SetPosition2D(position.x, position.y - num);
		}
		FollowTransform component3 = playParticleEffects.GetComponent<FollowTransform>();
		if (component3)
		{
			component3.Target = gameObject.transform;
		}
		playParticleEffects.PlayParticleSystems();
		this.currentlyBurning.Add(collision.gameObject, new EnemyCoalBurn.Effects
		{
			Particles = playParticleEffects,
			Follow = component3
		});
	}

	// Token: 0x06001495 RID: 5269 RVA: 0x0005CBF8 File Offset: 0x0005ADF8
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer != 11 || !this.currentlyBurning.ContainsKey(collision.gameObject))
		{
			return;
		}
		EnemyCoalBurn.Effects effects = this.currentlyBurning[collision.gameObject];
		effects.Particles.StopParticleSystems();
		if (effects.Follow)
		{
			effects.Follow.Target = null;
		}
		this.currentlyBurning.Remove(collision.gameObject);
	}

	// Token: 0x040012F0 RID: 4848
	[SerializeField]
	private PlayParticleEffects burningEffectPrefab;

	// Token: 0x040012F1 RID: 4849
	private Dictionary<GameObject, EnemyCoalBurn.Effects> currentlyBurning = new Dictionary<GameObject, EnemyCoalBurn.Effects>();

	// Token: 0x02001544 RID: 5444
	private struct Effects
	{
		// Token: 0x0400868C RID: 34444
		public PlayParticleEffects Particles;

		// Token: 0x0400868D RID: 34445
		public FollowTransform Follow;
	}
}
