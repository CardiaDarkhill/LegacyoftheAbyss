using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000284 RID: 644
public class StopParticleSystemsOnStart : MonoBehaviour, IBeginStopper
{
	// Token: 0x060016B7 RID: 5815 RVA: 0x000663D7 File Offset: 0x000645D7
	private void OnValidate()
	{
		if (this.frameDelay < 0)
		{
			this.frameDelay = 0;
		}
	}

	// Token: 0x060016B8 RID: 5816 RVA: 0x000663EC File Offset: 0x000645EC
	private void Awake()
	{
		this.systems = new List<ParticleSystem>();
		if (this.onlyChildren == null || this.onlyChildren.Length == 0)
		{
			base.GetComponentsInChildren<ParticleSystem>(true, this.systems);
			return;
		}
		foreach (GameObject gameObject in this.onlyChildren)
		{
			if (gameObject)
			{
				gameObject.GetComponentsInChildren<ParticleSystem>(true, this.systems);
			}
		}
	}

	// Token: 0x060016B9 RID: 5817 RVA: 0x00066451 File Offset: 0x00064651
	private void OnEnable()
	{
		this.DoBeginStop();
	}

	// Token: 0x060016BA RID: 5818 RVA: 0x00066459 File Offset: 0x00064659
	public void DoBeginStop()
	{
		if (this.stopDelayRoutine != null)
		{
			base.StopCoroutine(this.stopDelayRoutine);
		}
		this.stopDelayRoutine = base.StartCoroutine(this.StopDelay());
	}

	// Token: 0x060016BB RID: 5819 RVA: 0x00066481 File Offset: 0x00064681
	private void OnDisable()
	{
		base.StopCoroutine(this.stopDelayRoutine);
	}

	// Token: 0x060016BC RID: 5820 RVA: 0x0006648F File Offset: 0x0006468F
	private IEnumerator StopDelay()
	{
		WaitForEndOfFrame wait = new WaitForEndOfFrame();
		int num;
		for (int i = 0; i <= this.frameDelay; i = num + 1)
		{
			yield return wait;
			num = i;
		}
		this.DoStop();
		yield break;
	}

	// Token: 0x060016BD RID: 5821 RVA: 0x000664A0 File Offset: 0x000646A0
	public void DoStop()
	{
		foreach (ParticleSystem particleSystem in this.systems)
		{
			if (!particleSystem.CompareTag("Ignore Particle Stop"))
			{
				particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			}
		}
	}

	// Token: 0x04001533 RID: 5427
	[SerializeField]
	private int frameDelay;

	// Token: 0x04001534 RID: 5428
	[SerializeField]
	private GameObject[] onlyChildren;

	// Token: 0x04001535 RID: 5429
	private List<ParticleSystem> systems;

	// Token: 0x04001536 RID: 5430
	private Coroutine stopDelayRoutine;
}
