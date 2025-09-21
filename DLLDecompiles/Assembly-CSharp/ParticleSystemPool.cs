using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200025B RID: 603
public class ParticleSystemPool : MonoBehaviour, IInitialisable
{
	// Token: 0x060015B9 RID: 5561 RVA: 0x00062141 File Offset: 0x00060341
	private void OnValidate()
	{
		if (this.initialPoolSize <= 0)
		{
			this.initialPoolSize = 1;
		}
	}

	// Token: 0x060015BA RID: 5562 RVA: 0x00062154 File Offset: 0x00060354
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		if (this.original == null)
		{
			base.enabled = false;
			return true;
		}
		this.hasParticle = true;
		Transform transform = this.original.transform;
		this.parentTrans = transform.parent;
		this.initialPos = transform.localPosition;
		this.initialRotation = transform.localRotation;
		this.initialScale = transform.localScale;
		this.original.main.playOnAwake = false;
		this.original.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		this.spawnedParticles.Add(this.original);
		for (int i = 0; i < this.initialPoolSize - 1; i++)
		{
			ParticleSystem particleSystem = Object.Instantiate<ParticleSystem>(this.original, this.original.transform.parent);
			particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			this.spawnedParticles.Add(particleSystem);
		}
		return true;
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x00062242 File Offset: 0x00060442
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasStarted)
		{
			return false;
		}
		this.hasStarted = true;
		return true;
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x0006225D File Offset: 0x0006045D
	private void Awake()
	{
		this.OnAwake();
	}

	// Token: 0x060015BD RID: 5565 RVA: 0x00062268 File Offset: 0x00060468
	public void PlayParticles()
	{
		if (!this.hasParticle)
		{
			return;
		}
		bool flag = true;
		ParticleSystem particleSystem = null;
		for (int i = 0; i < this.spawnedParticles.Count; i++)
		{
			ParticleSystem particleSystem2 = this.spawnedParticles[i];
			if (!particleSystem2.IsAlive(true))
			{
				particleSystem = particleSystem2;
				flag = false;
				break;
			}
		}
		if (flag)
		{
			ParticleSystem particleSystem3 = Object.Instantiate<ParticleSystem>(this.original, this.original.transform.parent);
			this.spawnedParticles.Add(particleSystem3);
			particleSystem = particleSystem3;
		}
		particleSystem.gameObject.SetActive(false);
		if (this.deparentOnPlay)
		{
			Transform transform = particleSystem.transform;
			transform.parent = this.parentTrans;
			transform.localPosition = this.initialPos;
			transform.localRotation = this.initialRotation;
			transform.localScale = this.initialScale;
			transform.SetParent(null, true);
		}
		particleSystem.gameObject.SetActive(true);
		particleSystem.Play();
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x00062348 File Offset: 0x00060548
	public void StopParticles()
	{
		foreach (ParticleSystem particleSystem in this.spawnedParticles)
		{
			particleSystem.Stop(true);
		}
	}

	// Token: 0x060015BF RID: 5567 RVA: 0x0006239C File Offset: 0x0006059C
	public bool IsAlive()
	{
		using (List<ParticleSystem>.Enumerator enumerator = this.spawnedParticles.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.IsAlive(true))
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060015C1 RID: 5569 RVA: 0x00062412 File Offset: 0x00060612
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x04001454 RID: 5204
	[SerializeField]
	private ParticleSystem original;

	// Token: 0x04001455 RID: 5205
	[SerializeField]
	private int initialPoolSize = 1;

	// Token: 0x04001456 RID: 5206
	[SerializeField]
	private bool deparentOnPlay;

	// Token: 0x04001457 RID: 5207
	private Transform parentTrans;

	// Token: 0x04001458 RID: 5208
	private Vector3 initialPos;

	// Token: 0x04001459 RID: 5209
	private Quaternion initialRotation;

	// Token: 0x0400145A RID: 5210
	private Vector3 initialScale;

	// Token: 0x0400145B RID: 5211
	private bool noParticle;

	// Token: 0x0400145C RID: 5212
	private readonly List<ParticleSystem> spawnedParticles = new List<ParticleSystem>();

	// Token: 0x0400145D RID: 5213
	private bool hasAwaken;

	// Token: 0x0400145E RID: 5214
	private bool hasStarted;

	// Token: 0x0400145F RID: 5215
	private bool hasParticle;
}
