using System;
using UnityEngine;

// Token: 0x0200030D RID: 781
public class ParticleDamageHero : MonoBehaviour
{
	// Token: 0x06001BAC RID: 7084 RVA: 0x0008104C File Offset: 0x0007F24C
	private void Start()
	{
		HeroBox heroBox = HeroController.instance.heroBox;
		this.system = base.GetComponent<ParticleSystem>();
		ParticleSystem.TriggerModule trigger = this.system.trigger;
		trigger.enabled = true;
		while (trigger.colliderCount > 0)
		{
			trigger.RemoveCollider(0);
		}
		trigger.AddCollider(heroBox.GetComponent<Collider2D>());
		trigger.inside = ParticleSystemOverlapAction.Ignore;
		trigger.outside = ParticleSystemOverlapAction.Ignore;
		trigger.enter = ParticleSystemOverlapAction.Callback;
		trigger.exit = ParticleSystemOverlapAction.Ignore;
		trigger.colliderQueryMode = ParticleSystemColliderQueryMode.One;
		trigger.radiusScale = this.system.collision.radiusScale;
	}

	// Token: 0x06001BAD RID: 7085 RVA: 0x000810E8 File Offset: 0x0007F2E8
	private void OnParticleTrigger()
	{
		if (this.system.GetSafeTriggerParticlesSize(ParticleSystemTriggerEventType.Enter) <= 0)
		{
			return;
		}
		HeroBox heroBox = HeroController.instance.heroBox;
		if (!HeroBox.Inactive)
		{
			heroBox.CheckForDamage(base.gameObject);
		}
	}

	// Token: 0x04001AB8 RID: 6840
	private ParticleSystem system;
}
