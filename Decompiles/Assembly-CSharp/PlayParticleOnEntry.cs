using System;
using UnityEngine;

// Token: 0x0200025D RID: 605
public class PlayParticleOnEntry : MonoBehaviour
{
	// Token: 0x060015C4 RID: 5572 RVA: 0x000624B0 File Offset: 0x000606B0
	private void Start()
	{
		this.particle = base.GetComponent<ParticleSystem>();
		if (this.particle)
		{
			this.particle.Stop();
		}
	}

	// Token: 0x060015C5 RID: 5573 RVA: 0x000624D6 File Offset: 0x000606D6
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.particle)
		{
			this.particle.Play();
		}
	}

	// Token: 0x060015C6 RID: 5574 RVA: 0x000624F0 File Offset: 0x000606F0
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (this.particle)
		{
			this.particle.Stop();
		}
	}

	// Token: 0x04001461 RID: 5217
	private ParticleSystem particle;
}
