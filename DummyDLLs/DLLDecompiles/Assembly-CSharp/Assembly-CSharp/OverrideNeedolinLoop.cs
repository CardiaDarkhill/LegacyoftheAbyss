using System;
using UnityEngine;

// Token: 0x02000128 RID: 296
public class OverrideNeedolinLoop : MonoBehaviour
{
	// Token: 0x170000BC RID: 188
	// (get) Token: 0x06000916 RID: 2326 RVA: 0x0002A48A File Offset: 0x0002868A
	// (set) Token: 0x06000917 RID: 2327 RVA: 0x0002A492 File Offset: 0x00028692
	public AudioClip NeedolinClip
	{
		get
		{
			return this.needolinClip;
		}
		set
		{
			this.needolinClip = value;
		}
	}

	// Token: 0x170000BD RID: 189
	// (get) Token: 0x06000918 RID: 2328 RVA: 0x0002A49B File Offset: 0x0002869B
	// (set) Token: 0x06000919 RID: 2329 RVA: 0x0002A4A3 File Offset: 0x000286A3
	public bool DoSync { get; set; } = true;

	// Token: 0x170000BE RID: 190
	// (get) Token: 0x0600091A RID: 2330 RVA: 0x0002A4AC File Offset: 0x000286AC
	public static bool IsOverridden
	{
		get
		{
			return OverrideNeedolinLoop._current && (OverrideNeedolinLoop._current.reverseSyncDirection || !OverrideNeedolinLoop._current.syncToSource || OverrideNeedolinLoop._current.syncToSource.isPlaying);
		}
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x0002A4EA File Offset: 0x000286EA
	private void Awake()
	{
		this.heroRange.InsideStateChanged += this.OnHeroRangeStateChanged;
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x0002A503 File Offset: 0x00028703
	private void OnEnable()
	{
		this.OnHeroRangeStateChanged(this.heroRange.IsInside);
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x0002A516 File Offset: 0x00028716
	private void OnDisable()
	{
		this.OnHeroRangeStateChanged(this.heroRange.IsInside);
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x0002A529 File Offset: 0x00028729
	private void OnDestroy()
	{
		this.heroRange.InsideStateChanged -= this.OnHeroRangeStateChanged;
		if (OverrideNeedolinLoop._current == this)
		{
			OverrideNeedolinLoop._current = null;
		}
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x0002A555 File Offset: 0x00028755
	private void OnHeroRangeStateChanged(bool isInside)
	{
		if (!base.isActiveAndEnabled)
		{
			isInside = false;
		}
		if (isInside)
		{
			OverrideNeedolinLoop._current = this;
			return;
		}
		if (OverrideNeedolinLoop._current == this)
		{
			OverrideNeedolinLoop._current = null;
		}
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x0002A580 File Offset: 0x00028780
	public float GetTimeLeft()
	{
		float length = this.lastSource.clip.length;
		float num = length - this.lastSource.time;
		if (this.dontLoop)
		{
			return num;
		}
		float num2 = length * 0.5f;
		if (num > num2)
		{
			num -= num2;
		}
		return num;
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x0002A5C8 File Offset: 0x000287C8
	public static void StartSyncedAudio(AudioSource targetSource, AudioClip defaultClip)
	{
		targetSource.volume = 1f;
		targetSource.loop = true;
		if (OverrideNeedolinLoop.IsOverridden)
		{
			OverrideNeedolinLoop._current.lastSource = targetSource;
			bool flag = false;
			if (targetSource.clip != null && OverrideNeedolinLoop._current.syncToSource && OverrideNeedolinLoop._current.syncToSource.clip != null)
			{
				flag = (targetSource.clip.frequency != OverrideNeedolinLoop._current.syncToSource.clip.frequency);
			}
			if (!targetSource.isPlaying || targetSource.clip != OverrideNeedolinLoop._current.NeedolinClip)
			{
				targetSource.clip = OverrideNeedolinLoop._current.NeedolinClip;
				targetSource.loop = !OverrideNeedolinLoop._current.dontLoop;
				targetSource.Play();
			}
			if (OverrideNeedolinLoop._current.reverseSyncDirection)
			{
				if (OverrideNeedolinLoop._current.syncToSource)
				{
					if (!flag)
					{
						OverrideNeedolinLoop._current.syncToSource.timeSamples = targetSource.timeSamples;
					}
					else
					{
						OverrideNeedolinLoop._current.syncToSource.time = targetSource.time;
					}
					if (!OverrideNeedolinLoop._current.syncToSource.isPlaying)
					{
						OverrideNeedolinLoop._current.syncToSource.volume = 0f;
						OverrideNeedolinLoop._current.syncToSource.Play();
						return;
					}
				}
			}
			else if (OverrideNeedolinLoop._current.DoSync && OverrideNeedolinLoop._current.syncToSource)
			{
				if (!flag)
				{
					targetSource.timeSamples = OverrideNeedolinLoop._current.syncToSource.timeSamples;
					return;
				}
				targetSource.time = OverrideNeedolinLoop._current.syncToSource.time;
				return;
			}
		}
		else if (!targetSource.isPlaying || targetSource.clip != defaultClip)
		{
			targetSource.clip = defaultClip;
			targetSource.Play();
		}
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x0002A797 File Offset: 0x00028997
	public void SetReverseSync()
	{
		this.reverseSyncDirection = true;
	}

	// Token: 0x040008CF RID: 2255
	[SerializeField]
	private AudioSource syncToSource;

	// Token: 0x040008D0 RID: 2256
	[SerializeField]
	private AudioClip needolinClip;

	// Token: 0x040008D1 RID: 2257
	[SerializeField]
	private TrackTriggerObjects heroRange;

	// Token: 0x040008D2 RID: 2258
	[SerializeField]
	private bool dontLoop;

	// Token: 0x040008D3 RID: 2259
	[SerializeField]
	private bool reverseSyncDirection;

	// Token: 0x040008D4 RID: 2260
	private AudioSource lastSource;

	// Token: 0x040008D5 RID: 2261
	private static OverrideNeedolinLoop _current;
}
