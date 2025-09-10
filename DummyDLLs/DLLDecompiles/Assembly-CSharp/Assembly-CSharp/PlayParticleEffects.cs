using System;
using System.Linq;
using TeamCherry.NestedFadeGroup;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020006F4 RID: 1780
public class PlayParticleEffects : MonoBehaviour
{
	// Token: 0x06003FC7 RID: 16327 RVA: 0x00119472 File Offset: 0x00117672
	private void Awake()
	{
		if (this.useChildren)
		{
			this.particleEffects = (from ps in base.GetComponentsInChildren<ParticleSystem>()
			where !this.particleEffects.Contains(ps)
			select ps).Concat(this.particleEffects).ToArray<ParticleSystem>();
		}
	}

	// Token: 0x06003FC8 RID: 16328 RVA: 0x001194A9 File Offset: 0x001176A9
	private void OnEnable()
	{
		ComponentSingleton<PlayParticleEffectsCallbackHooks>.Instance.OnUpdate += this.OnUpdate;
	}

	// Token: 0x06003FC9 RID: 16329 RVA: 0x001194C1 File Offset: 0x001176C1
	private void OnDisable()
	{
		ComponentSingleton<PlayParticleEffectsCallbackHooks>.Instance.OnUpdate -= this.OnUpdate;
	}

	// Token: 0x06003FCA RID: 16330 RVA: 0x001194D9 File Offset: 0x001176D9
	private void OnUpdate()
	{
		if (this.clearTimeLeft > 0f)
		{
			this.clearTimeLeft -= Time.deltaTime;
			if (this.clearTimeLeft <= 0f)
			{
				this.ClearParticleSystems();
			}
		}
	}

	// Token: 0x06003FCB RID: 16331 RVA: 0x00119510 File Offset: 0x00117710
	public void PlayParticleSystems()
	{
		if (this.fadeGroup)
		{
			this.fadeGroup.FadeTo(1f, this.fadeInDuration, null, false, null);
		}
		this.clearTimeLeft = 0f;
		foreach (ParticleSystem particleSystem in this.particleEffects)
		{
			if (this.deparentOnPlay)
			{
				particleSystem.transform.SetParent(null, true);
			}
			if (this.useEmission)
			{
				particleSystem.emission.enabled = true;
			}
			else
			{
				particleSystem.Play();
			}
		}
		this.OnPlay.Invoke();
	}

	// Token: 0x06003FCC RID: 16332 RVA: 0x001195A8 File Offset: 0x001177A8
	public void StopParticleSystems()
	{
		if (this.fadeGroup)
		{
			this.fadeGroup.FadeTo(0f, this.fadeOutDuration, null, false, null);
		}
		foreach (ParticleSystem particleSystem in this.particleEffects)
		{
			if (this.useEmission)
			{
				particleSystem.emission.enabled = false;
			}
			else
			{
				particleSystem.Stop();
				this.clearTimeLeft = this.fadeOutDuration;
			}
		}
		this.OnStop.Invoke();
	}

	// Token: 0x06003FCD RID: 16333 RVA: 0x0011962C File Offset: 0x0011782C
	public void ClearParticleSystems()
	{
		ParticleSystem[] array = this.particleEffects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Clear();
		}
	}

	// Token: 0x06003FCE RID: 16334 RVA: 0x00119656 File Offset: 0x00117856
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.useCollider)
		{
			this.PlayParticleSystems();
		}
	}

	// Token: 0x06003FCF RID: 16335 RVA: 0x00119666 File Offset: 0x00117866
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (this.useCollider)
		{
			this.StopParticleSystems();
		}
	}

	// Token: 0x06003FD0 RID: 16336 RVA: 0x00119678 File Offset: 0x00117878
	public bool IsAlive()
	{
		ParticleSystem[] array = this.particleEffects;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].IsAlive(true))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003FD1 RID: 16337 RVA: 0x001196A8 File Offset: 0x001178A8
	public bool IsPlaying()
	{
		ParticleSystem[] array = this.particleEffects;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].isPlaying)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0400416C RID: 16748
	[SerializeField]
	private ParticleSystem[] particleEffects;

	// Token: 0x0400416D RID: 16749
	[SerializeField]
	private bool useChildren;

	// Token: 0x0400416E RID: 16750
	[SerializeField]
	private bool deparentOnPlay;

	// Token: 0x0400416F RID: 16751
	[SerializeField]
	private bool useEmission;

	// Token: 0x04004170 RID: 16752
	[SerializeField]
	private bool useCollider;

	// Token: 0x04004171 RID: 16753
	[Space]
	[SerializeField]
	private NestedFadeGroupBase fadeGroup;

	// Token: 0x04004172 RID: 16754
	[SerializeField]
	private float fadeInDuration;

	// Token: 0x04004173 RID: 16755
	[SerializeField]
	private float fadeOutDuration;

	// Token: 0x04004174 RID: 16756
	[Space]
	public UnityEvent OnPlay;

	// Token: 0x04004175 RID: 16757
	public UnityEvent OnStop;

	// Token: 0x04004176 RID: 16758
	private float clearTimeLeft;
}
