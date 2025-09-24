using System;
using UnityEngine;

// Token: 0x0200022D RID: 557
public class DisableParticleCollisonDelay : MonoBehaviour
{
	// Token: 0x06001491 RID: 5265 RVA: 0x0005CA69 File Offset: 0x0005AC69
	public void Awake()
	{
		this.particle_system = base.GetComponent<ParticleSystem>();
	}

	// Token: 0x06001492 RID: 5266 RVA: 0x0005CA78 File Offset: 0x0005AC78
	public void Update()
	{
		if (!this.didCollisionEnd)
		{
			if (!this.played && this.particle_system.IsAlive())
			{
				this.played = true;
			}
			if (this.played)
			{
				this.timer += Time.deltaTime;
				if (Time.deltaTime >= this.delay)
				{
					this.particle_system.collision.enabled = false;
				}
			}
		}
	}

	// Token: 0x040012EB RID: 4843
	public float delay;

	// Token: 0x040012EC RID: 4844
	private float timer;

	// Token: 0x040012ED RID: 4845
	private bool played;

	// Token: 0x040012EE RID: 4846
	private bool didCollisionEnd;

	// Token: 0x040012EF RID: 4847
	private ParticleSystem particle_system;
}
