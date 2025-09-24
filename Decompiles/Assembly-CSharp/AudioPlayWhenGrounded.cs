using System;
using UnityEngine;

// Token: 0x0200010E RID: 270
public class AudioPlayWhenGrounded : MonoBehaviour
{
	// Token: 0x06000879 RID: 2169 RVA: 0x00027F90 File Offset: 0x00026190
	private void OnEnable()
	{
		if (!this.audioSource || !this.sweepCollider)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.audioSource.Stop();
	}

	// Token: 0x0600087A RID: 2170 RVA: 0x00027FC4 File Offset: 0x000261C4
	private void Update()
	{
		Sweep sweep = new Sweep(this.sweepCollider, 3, 3, 0.1f, 0.01f);
		this.isGrounded = sweep.Check(this.sweepDistance, 256);
		if (this.isGrounded && !this.CanPlay)
		{
			this.isGrounded = false;
		}
		if (this.isGrounded && !this.wasGrounded)
		{
			this.audioSource.Play();
		}
		else if (!this.isGrounded && this.wasGrounded)
		{
			this.audioSource.Stop();
		}
		this.wasGrounded = this.isGrounded;
	}

	// Token: 0x0400081C RID: 2076
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x0400081D RID: 2077
	[SerializeField]
	private Collider2D sweepCollider;

	// Token: 0x0400081E RID: 2078
	[SerializeField]
	private float sweepDistance = 0.2f;

	// Token: 0x0400081F RID: 2079
	public bool CanPlay = true;

	// Token: 0x04000820 RID: 2080
	private bool isGrounded;

	// Token: 0x04000821 RID: 2081
	private bool wasGrounded;
}
