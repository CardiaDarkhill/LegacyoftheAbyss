using System;
using UnityEngine;

// Token: 0x0200028C RID: 652
public class WaterfallParticles : MonoBehaviour
{
	// Token: 0x060016E0 RID: 5856 RVA: 0x00066FA3 File Offset: 0x000651A3
	private void Start()
	{
		if (this.splashParticles)
		{
			this.splashParticles.Stop();
		}
	}

	// Token: 0x060016E1 RID: 5857 RVA: 0x00066FC0 File Offset: 0x000651C0
	private void Update()
	{
		if (this.splashParticles)
		{
			RaycastHit2D raycastHit2D = Physics2D.CircleCast(base.transform.position + this.raycastOrigin, this.raycastRadius, Vector2.down, this.raycastDistance, this.raycastLayers);
			if (raycastHit2D.collider != null)
			{
				if (!this.splashParticles.isPlaying)
				{
					this.splashParticles.Play();
				}
				this.splashParticles.transform.position = raycastHit2D.point + Vector2.up * this.splashOffset;
				return;
			}
			if (this.splashParticles.isPlaying)
			{
				this.splashParticles.Stop();
			}
		}
	}

	// Token: 0x060016E2 RID: 5858 RVA: 0x0006708C File Offset: 0x0006528C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (base.enabled && collision.tag == "Nail Attack")
		{
			float num = Mathf.Sign(collision.transform.position.x - base.transform.position.x);
			if (this.slashParticles)
			{
				this.slashParticles.transform.SetScaleX(-num);
				this.slashParticles.transform.SetPositionY(collision.transform.position.y);
				this.slashParticles.Play();
			}
			if (this.slashSound)
			{
				this.slashSound.transform.SetPositionY(collision.transform.position.y);
				this.slashSound.pitch = Random.Range(this.minPitch, this.maxPitch);
				this.slashSound.Play();
			}
		}
	}

	// Token: 0x060016E3 RID: 5859 RVA: 0x00067180 File Offset: 0x00065380
	private void OnDrawGizmosSelected()
	{
		if (base.enabled)
		{
			Gizmos.DrawWireSphere(base.transform.position + this.raycastOrigin, this.raycastRadius);
			Gizmos.DrawLine(base.transform.position + this.raycastOrigin, base.transform.position + this.raycastOrigin + Vector3.down * this.raycastDistance);
		}
	}

	// Token: 0x04001561 RID: 5473
	public Vector2 raycastOrigin;

	// Token: 0x04001562 RID: 5474
	public float raycastDistance = 2f;

	// Token: 0x04001563 RID: 5475
	public LayerMask raycastLayers;

	// Token: 0x04001564 RID: 5476
	public float raycastRadius = 1f;

	// Token: 0x04001565 RID: 5477
	[Space]
	public ParticleSystem splashParticles;

	// Token: 0x04001566 RID: 5478
	public float splashOffset = -0.35f;

	// Token: 0x04001567 RID: 5479
	[Space]
	public ParticleSystem slashParticles;

	// Token: 0x04001568 RID: 5480
	public AudioSource slashSound;

	// Token: 0x04001569 RID: 5481
	public float minPitch = 0.85f;

	// Token: 0x0400156A RID: 5482
	public float maxPitch = 1.15f;
}
