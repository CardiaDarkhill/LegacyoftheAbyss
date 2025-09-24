using System;
using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x02000249 RID: 585
public class LavaBellCoalRegionShield : MonoBehaviour
{
	// Token: 0x0600154C RID: 5452 RVA: 0x00060862 File Offset: 0x0005EA62
	private void OnEnable()
	{
		this.fadeGroup.AlphaSelf = 0f;
		this.SetParticleEmission(false);
	}

	// Token: 0x0600154D RID: 5453 RVA: 0x0006087B File Offset: 0x0005EA7B
	public void Begin()
	{
		base.StopAllCoroutines();
		this.fadeGroup.FadeTo(1f, this.fadeUpTime, null, false, null);
		this.SetParticleEmission(true);
	}

	// Token: 0x0600154E RID: 5454 RVA: 0x000608A4 File Offset: 0x0005EAA4
	public void End()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		base.StartCoroutine(this.FadeToDeactivate());
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x000608C4 File Offset: 0x0005EAC4
	private void SetParticleEmission(bool value)
	{
		ParticleSystem[] array = this.controlEmission;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].emission.enabled = value;
		}
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x000608F7 File Offset: 0x0005EAF7
	private IEnumerator FadeToDeactivate()
	{
		this.SetParticleEmission(false);
		yield return new WaitForSeconds(this.fadeGroup.FadeTo(0f, this.fadeDownTime, null, false, null));
		ParticleSystem[] array = this.controlEmission;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		bool waitForParticles = true;
		while (waitForParticles)
		{
			waitForParticles = false;
			yield return null;
			array = this.controlEmission;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].IsAlive(true))
				{
					waitForParticles = true;
					break;
				}
			}
		}
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x040013F2 RID: 5106
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	// Token: 0x040013F3 RID: 5107
	[SerializeField]
	private float fadeUpTime;

	// Token: 0x040013F4 RID: 5108
	[SerializeField]
	private float fadeDownTime;

	// Token: 0x040013F5 RID: 5109
	[SerializeField]
	private ParticleSystem[] controlEmission;
}
