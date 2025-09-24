using System;
using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

// Token: 0x020004BE RID: 1214
public class CogEnergyTimeline : UnlockablePropBase, IMutable
{
	// Token: 0x17000516 RID: 1302
	// (get) Token: 0x06002BD2 RID: 11218 RVA: 0x000C005E File Offset: 0x000BE25E
	public bool Muted
	{
		get
		{
			return this.muted;
		}
	}

	// Token: 0x06002BD3 RID: 11219 RVA: 0x000C0066 File Offset: 0x000BE266
	private void Start()
	{
		this.UpdateEnergy(0f, false);
		this.SetComplete(false);
	}

	// Token: 0x06002BD4 RID: 11220 RVA: 0x000C007B File Offset: 0x000BE27B
	private void OnValidate()
	{
		if (this.director != null && this.directorMuter == null)
		{
			this.directorMuter = this.director.GetComponent<Mutable>();
		}
	}

	// Token: 0x06002BD5 RID: 11221 RVA: 0x000C00AC File Offset: 0x000BE2AC
	public void SetEnergy(float newEnergy, bool animate)
	{
		if (this.animateRoutine != null)
		{
			base.StopCoroutine(this.animateRoutine);
			this.animateRoutine = null;
		}
		if (Math.Abs(newEnergy - this.energy) > Mathf.Epsilon)
		{
			if (newEnergy > this.energy)
			{
				if (this.retractAudioLoop.isPlaying)
				{
					this.retractAudioLoop.Stop();
				}
			}
			else if (!this.retractAudioLoop.isPlaying)
			{
				this.retractAudioLoop.Play();
			}
		}
		if (animate)
		{
			this.animateRoutine = base.StartCoroutine(this.Animate(newEnergy));
			return;
		}
		this.UpdateEnergy(newEnergy, false);
		this.SetComplete(this.isOpen && newEnergy / this.maxEnergy > 0.99f);
	}

	// Token: 0x06002BD6 RID: 11222 RVA: 0x000C0162 File Offset: 0x000BE362
	private IEnumerator Animate(float newEnergy)
	{
		float startEnergy = this.energy;
		this.SetComplete(false);
		this.OnAnimateStart.Invoke();
		this.hitSounds.SpawnAndPlayOneShot(base.transform.position, null);
		for (float elapsed = 0f; elapsed < this.animateTime; elapsed += Time.deltaTime)
		{
			float t = this.animateCurve.Evaluate(elapsed / this.animateTime);
			this.UpdateEnergy(Mathf.Lerp(startEnergy, newEnergy, t), true);
			yield return null;
		}
		this.UpdateEnergy(newEnergy, true);
		if (newEnergy / this.maxEnergy > 0.99f)
		{
			this.OnAnimateComplete.Invoke();
			this.finishSound.SpawnAndPlayOneShot(base.transform.position, null);
			if (this.benchActivateDelay > 0f)
			{
				yield return new WaitForSeconds(this.benchActivateDelay);
			}
			if (this.isOpen)
			{
				this.SetComplete(true);
			}
		}
		else
		{
			this.SetComplete(false);
		}
		yield break;
	}

	// Token: 0x06002BD7 RID: 11223 RVA: 0x000C0178 File Offset: 0x000BE378
	private void UpdateEnergy(float newEnergy, bool playSound = true)
	{
		this.energy = newEnergy;
		float num = Mathf.Clamp01(this.energy / this.maxEnergy);
		bool flag = num > 0.01f;
		if (!flag && this.wasAbove)
		{
			this.backAtStartSound.SpawnAndPlayOneShot(base.transform.position, null);
		}
		this.wasAbove = flag;
		if (!this.director)
		{
			return;
		}
		bool mute = this.muted;
		if (!playSound)
		{
			this.SetMute(true);
		}
		this.director.time = (double)(this.maxTime * num);
		this.director.Evaluate();
		this.SetMute(mute);
	}

	// Token: 0x06002BD8 RID: 11224 RVA: 0x000C0218 File Offset: 0x000BE418
	private void SetComplete(bool value)
	{
		if (this.realBenchInteract)
		{
			if (value)
			{
				this.realBenchInteract.Activate();
			}
			else
			{
				this.realBenchInteract.Deactivate(false);
			}
		}
		if (this.realBenchFadeGroup)
		{
			this.realBenchFadeGroup.FadeTo(value ? 1f : 0f, this.realBenchFadeTime, null, false, null);
		}
		if (this.activateObj)
		{
			this.activateObj.SetActive(value);
		}
		if (value && this.retractAudioLoop.isPlaying)
		{
			this.retractAudioLoop.Stop();
		}
	}

	// Token: 0x06002BD9 RID: 11225 RVA: 0x000C02B3 File Offset: 0x000BE4B3
	public override void Open()
	{
		this.isOpen = true;
		this.SetEnergy(this.maxEnergy, true);
	}

	// Token: 0x06002BDA RID: 11226 RVA: 0x000C02C9 File Offset: 0x000BE4C9
	public override void Opened()
	{
		this.isOpen = true;
		this.SetEnergy(this.maxEnergy, false);
	}

	// Token: 0x06002BDB RID: 11227 RVA: 0x000C02DF File Offset: 0x000BE4DF
	public void SetMute(bool value)
	{
		this.muted = value;
		if (this.directorMuter)
		{
			this.directorMuter.SetMute(value);
		}
	}

	// Token: 0x04002D29 RID: 11561
	[SerializeField]
	private PlayableDirector director;

	// Token: 0x04002D2A RID: 11562
	[SerializeField]
	private Mutable directorMuter;

	// Token: 0x04002D2B RID: 11563
	[SerializeField]
	private float maxTime;

	// Token: 0x04002D2C RID: 11564
	[SerializeField]
	private float maxEnergy = 360f;

	// Token: 0x04002D2D RID: 11565
	[SerializeField]
	private float animateTime;

	// Token: 0x04002D2E RID: 11566
	[SerializeField]
	private AnimationCurve animateCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002D2F RID: 11567
	[SerializeField]
	private AudioEventRandom hitSounds;

	// Token: 0x04002D30 RID: 11568
	[SerializeField]
	private AudioEvent finishSound;

	// Token: 0x04002D31 RID: 11569
	[SerializeField]
	private AudioEvent backAtStartSound;

	// Token: 0x04002D32 RID: 11570
	[SerializeField]
	private AudioSource retractAudioLoop;

	// Token: 0x04002D33 RID: 11571
	[Space]
	[SerializeField]
	private InteractableBase realBenchInteract;

	// Token: 0x04002D34 RID: 11572
	[SerializeField]
	private NestedFadeGroupBase realBenchFadeGroup;

	// Token: 0x04002D35 RID: 11573
	[SerializeField]
	private float realBenchFadeTime;

	// Token: 0x04002D36 RID: 11574
	[SerializeField]
	private float benchActivateDelay;

	// Token: 0x04002D37 RID: 11575
	[SerializeField]
	private GameObject activateObj;

	// Token: 0x04002D38 RID: 11576
	[Space]
	[SerializeField]
	private UnityEvent OnAnimateStart;

	// Token: 0x04002D39 RID: 11577
	[SerializeField]
	private UnityEvent OnAnimateComplete;

	// Token: 0x04002D3A RID: 11578
	private bool wasAbove;

	// Token: 0x04002D3B RID: 11579
	private float energy;

	// Token: 0x04002D3C RID: 11580
	private Coroutine animateRoutine;

	// Token: 0x04002D3D RID: 11581
	private bool isOpen;

	// Token: 0x04002D3E RID: 11582
	private bool muted;
}
