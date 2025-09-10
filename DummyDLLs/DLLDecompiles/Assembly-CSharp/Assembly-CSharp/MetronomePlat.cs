using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200051B RID: 1307
public class MetronomePlat : MonoBehaviour
{
	// Token: 0x06002F0C RID: 12044 RVA: 0x000CFC05 File Offset: 0x000CDE05
	private void Awake()
	{
		this.ticker.ReceivedEvent += delegate()
		{
			this.receivedEvent = true;
		};
		this.hasVibrationRegion = this.heroVibrationRegion;
	}

	// Token: 0x06002F0D RID: 12045 RVA: 0x000CFC2F File Offset: 0x000CDE2F
	private void Start()
	{
		base.StartCoroutine(this.SwingRoutine());
	}

	// Token: 0x06002F0E RID: 12046 RVA: 0x000CFC3E File Offset: 0x000CDE3E
	private IEnumerator SwingRoutine()
	{
		bool flipped = this.inverted;
		this.animator.Play(flipped ? MetronomePlat._inAnimatorState : MetronomePlat._outAnimatorState, 0, 1f);
		for (;;)
		{
			bool changedState = false;
			float elapsed = 0f;
			float duration = this.ticker.TickDelay;
			float nextUpdateElapsed = 0f;
			while (elapsed <= duration)
			{
				float num = elapsed / duration;
				num = this.metronomeSwingCurve.Evaluate(num);
				if (!changedState && num >= this.metronomeTickPoint)
				{
					if (flipped)
					{
						this.animator.Play(MetronomePlat._inAnimatorState);
						this.inAudio.PlayOnSource(this.audioSource);
					}
					else
					{
						this.animator.Play(MetronomePlat._outAnimatorState);
						this.outAudio.PlayOnSource(this.audioSource);
					}
					if (this.hasVibrationRegion)
					{
						this.heroVibrationRegion.StartVibration();
					}
					changedState = true;
				}
				if (flipped)
				{
					num = 1f - num;
				}
				bool flag = true;
				if (this.metronomeFpsLimit > 0f)
				{
					if (elapsed >= nextUpdateElapsed)
					{
						nextUpdateElapsed = elapsed + 1f / this.metronomeFpsLimit;
					}
					else
					{
						flag = false;
					}
				}
				if (flag)
				{
					float lerpedValue = this.metronomeRotateRange.GetLerpedValue(num);
					this.metronome.SetLocalRotation2D(lerpedValue);
				}
				yield return null;
				elapsed += Time.deltaTime;
			}
			while (!this.receivedEvent)
			{
				yield return null;
			}
			this.receivedEvent = false;
			flipped = !flipped;
		}
		yield break;
	}

	// Token: 0x06002F0F RID: 12047 RVA: 0x000CFC50 File Offset: 0x000CDE50
	public void NotifyRetracted()
	{
		if (this.registeredNotifiers == null)
		{
			return;
		}
		if (this.iteratingNotifiers == null)
		{
			this.iteratingNotifiers = new List<MetronomePlat.INotify>(this.registeredNotifiers.Count);
		}
		this.iteratingNotifiers.AddRange(this.registeredNotifiers);
		foreach (MetronomePlat.INotify notify in this.iteratingNotifiers)
		{
			notify.PlatRetracted(this);
		}
		this.iteratingNotifiers.Clear();
	}

	// Token: 0x06002F10 RID: 12048 RVA: 0x000CFCE4 File Offset: 0x000CDEE4
	public bool RegisterNotifier(MetronomePlat.INotify notify)
	{
		if (this.registeredNotifiers == null)
		{
			this.registeredNotifiers = new HashSet<MetronomePlat.INotify>();
		}
		return this.registeredNotifiers.Add(notify);
	}

	// Token: 0x06002F11 RID: 12049 RVA: 0x000CFD05 File Offset: 0x000CDF05
	public bool UnregisterNotifier(MetronomePlat.INotify notify)
	{
		return this.registeredNotifiers.Remove(notify);
	}

	// Token: 0x040031B2 RID: 12722
	[SerializeField]
	private TimedTicker ticker;

	// Token: 0x040031B3 RID: 12723
	[SerializeField]
	private bool inverted;

	// Token: 0x040031B4 RID: 12724
	[Space]
	[SerializeField]
	private Animator animator;

	// Token: 0x040031B5 RID: 12725
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x040031B6 RID: 12726
	[SerializeField]
	private AudioEvent inAudio;

	// Token: 0x040031B7 RID: 12727
	[SerializeField]
	private AudioEvent outAudio;

	// Token: 0x040031B8 RID: 12728
	[Space]
	[SerializeField]
	private Transform metronome;

	// Token: 0x040031B9 RID: 12729
	[SerializeField]
	private MinMaxFloat metronomeRotateRange;

	// Token: 0x040031BA RID: 12730
	[SerializeField]
	[Range(0f, 1f)]
	private float metronomeTickPoint = 0.5f;

	// Token: 0x040031BB RID: 12731
	[SerializeField]
	private AnimationCurve metronomeSwingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x040031BC RID: 12732
	[SerializeField]
	private float metronomeFpsLimit;

	// Token: 0x040031BD RID: 12733
	[Space]
	[SerializeField]
	private HeroVibrationRegion heroVibrationRegion;

	// Token: 0x040031BE RID: 12734
	private bool hasVibrationRegion;

	// Token: 0x040031BF RID: 12735
	private bool receivedEvent;

	// Token: 0x040031C0 RID: 12736
	private HashSet<MetronomePlat.INotify> registeredNotifiers;

	// Token: 0x040031C1 RID: 12737
	private List<MetronomePlat.INotify> iteratingNotifiers;

	// Token: 0x040031C2 RID: 12738
	private static readonly int _inAnimatorState = Animator.StringToHash("In");

	// Token: 0x040031C3 RID: 12739
	private static readonly int _outAnimatorState = Animator.StringToHash("Out");

	// Token: 0x02001828 RID: 6184
	public interface INotify
	{
		// Token: 0x0600902C RID: 36908
		void PlatRetracted(MetronomePlat plat);
	}
}
