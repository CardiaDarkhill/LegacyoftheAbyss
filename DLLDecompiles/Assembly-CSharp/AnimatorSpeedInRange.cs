using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200048D RID: 1165
public class AnimatorSpeedInRange : MonoBehaviour
{
	// Token: 0x06002A0A RID: 10762 RVA: 0x000B66C9 File Offset: 0x000B48C9
	private void Start()
	{
		this.animator.speed = 0f;
		this.activeParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		this.hasStarted = true;
	}

	// Token: 0x06002A0B RID: 10763 RVA: 0x000B66F0 File Offset: 0x000B48F0
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!this.hasStarted)
		{
			return;
		}
		this.insideColliders.AddIfNotPresent(other);
		if (this.insideColliders.Count == 1)
		{
			this.animator.speed = 1f;
			this.activeParticles.Play(true);
		}
	}

	// Token: 0x06002A0C RID: 10764 RVA: 0x000B673D File Offset: 0x000B493D
	private void OnTriggerExit2D(Collider2D other)
	{
		this.insideColliders.Remove(other);
		if (this.insideColliders.Count == 0)
		{
			this.animator.speed = 0f;
			this.activeParticles.Stop(true);
		}
	}

	// Token: 0x04002A84 RID: 10884
	[SerializeField]
	private Animator animator;

	// Token: 0x04002A85 RID: 10885
	[SerializeField]
	private ParticleSystem activeParticles;

	// Token: 0x04002A86 RID: 10886
	private bool hasStarted;

	// Token: 0x04002A87 RID: 10887
	private readonly List<Collider2D> insideColliders = new List<Collider2D>();
}
