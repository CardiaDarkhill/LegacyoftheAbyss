using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000341 RID: 833
public class AutoRecycleSelf : MonoBehaviour
{
	// Token: 0x06001CFB RID: 7419 RVA: 0x0008695F File Offset: 0x00084B5F
	private void Awake()
	{
		this.FindChildren();
	}

	// Token: 0x06001CFC RID: 7420 RVA: 0x00086968 File Offset: 0x00084B68
	private void OnEnable()
	{
		ComponentSingleton<AutoRecycleSelfCallbackHooks>.Instance.OnUpdate += this.OnUpdate;
		if (ObjectPool.IsCreatingPool)
		{
			return;
		}
		this.timer = 0f;
		this.stateTracker = 0;
		switch (this.afterEvent)
		{
		case AfterEvent.TIME:
			if (this.timeToWait > 0f)
			{
				this.recycleTimerRunning = true;
				if (this.unscaledTime)
				{
					this.timer = Time.realtimeSinceStartup + this.timeToWait;
				}
				else
				{
					this.timer = this.timeToWait;
				}
			}
			this.subscribeLevelUnload = true;
			break;
		case AfterEvent.TK2D_ANIM_END:
			this.tk2dAnimator = base.GetComponent<tk2dSpriteAnimator>();
			this.hasAnimator = (this.tk2dAnimator != null);
			this.subscribeLevelUnload = true;
			break;
		case AfterEvent.LEVEL_UNLOAD:
			this.subscribeLevelUnload = true;
			break;
		case AfterEvent.AUDIO_CLIP_END:
			this.audioSource = base.GetComponent<AudioSource>();
			if (this.audioSource == null)
			{
				Debug.LogError(base.name + " requires an AudioSource to auto-recycle itself.");
				this.validAudioSource = false;
			}
			else
			{
				this.validAudioSource = true;
			}
			break;
		case AfterEvent.MECANIM_ANIM_END:
			this.animator = base.GetComponent<Animator>();
			this.hasAnimator = (this.animator != null);
			this.subscribeLevelUnload = true;
			break;
		}
		if (this.subscribeLevelUnload)
		{
			this.subbed = true;
			AutoRecycleSelf.activeRecyclers.Add(this);
		}
	}

	// Token: 0x06001CFD RID: 7421 RVA: 0x00086AC8 File Offset: 0x00084CC8
	private void OnDisable()
	{
		ComponentSingleton<AutoRecycleSelfCallbackHooks>.Instance.OnUpdate -= this.OnUpdate;
		this.animator = null;
		this.hasAnimator = false;
		this.hasTk2dAnimator = false;
		this.tk2dAnimator = null;
		this.recycleTimer = null;
		if (this.recycleTimerRunning)
		{
			this.recycleTimerRunning = false;
			this.RecycleSelf();
		}
		if (this.subbed)
		{
			this.subbed = false;
			AutoRecycleSelf.activeRecyclers.Remove(this);
		}
		if (!this.subscribeLevelUnload)
		{
			return;
		}
		bool applicationIsQuitting = this.ApplicationIsQuitting;
	}

	// Token: 0x06001CFE RID: 7422 RVA: 0x00086B4E File Offset: 0x00084D4E
	private void OnDestroy()
	{
		if (this.subbed)
		{
			this.subbed = false;
			AutoRecycleSelf.activeRecyclers.Remove(this);
		}
	}

	// Token: 0x06001CFF RID: 7423 RVA: 0x00086B6B File Offset: 0x00084D6B
	private void OnUpdate()
	{
		this.RecycleUpdate();
	}

	// Token: 0x06001D00 RID: 7424 RVA: 0x00086B73 File Offset: 0x00084D73
	private void OnTransformChildrenChanged()
	{
		this.childrenValid = false;
	}

	// Token: 0x06001D01 RID: 7425 RVA: 0x00086B7C File Offset: 0x00084D7C
	private void RecycleUpdate()
	{
		switch (this.afterEvent)
		{
		case AfterEvent.TIME:
			if (this.recycleTimerRunning)
			{
				if (this.unscaledTime)
				{
					if (Time.realtimeSinceStartup >= this.timer)
					{
						this.RecycleSelf();
						return;
					}
				}
				else
				{
					this.timer -= Time.deltaTime;
					if (this.timer <= 0f)
					{
						this.RecycleSelf();
						return;
					}
				}
			}
			break;
		case AfterEvent.TK2D_ANIM_END:
			if (this.hasTk2dAnimator)
			{
				if (this.stateTracker == 0)
				{
					if (this.tk2dAnimator.Playing)
					{
						this.stateTracker++;
						return;
					}
				}
				else if (!this.tk2dAnimator.Playing)
				{
					this.RecycleSelf();
					this.hasTk2dAnimator = false;
					return;
				}
			}
			break;
		case AfterEvent.LEVEL_UNLOAD:
			break;
		case AfterEvent.AUDIO_CLIP_END:
			if (Time.frameCount % 20 == 0)
			{
				this.Update20();
				return;
			}
			break;
		case AfterEvent.MECANIM_ANIM_END:
			if (this.hasAnimator)
			{
				if (this.stateTracker == 0)
				{
					this.stateTracker++;
					return;
				}
				if (this.stateTracker == 1)
				{
					this.stateTracker++;
					this.timer = this.animator.GetCurrentAnimatorStateInfo(0).length;
					return;
				}
				this.timer -= Time.deltaTime;
				if (this.timer <= 0f)
				{
					this.RecycleSelf();
					this.hasAnimator = false;
				}
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x06001D02 RID: 7426 RVA: 0x00086CDF File Offset: 0x00084EDF
	private void Update20()
	{
		if (!this.validAudioSource)
		{
			return;
		}
		if (this.audioSource.isPlaying)
		{
			return;
		}
		this.RecycleSelf();
	}

	// Token: 0x06001D03 RID: 7427 RVA: 0x00086CFE File Offset: 0x00084EFE
	private void OnApplicationQuit()
	{
		this.ApplicationIsQuitting = true;
	}

	// Token: 0x06001D04 RID: 7428 RVA: 0x00086D08 File Offset: 0x00084F08
	public static void RecycleActiveRecyclers()
	{
		AutoRecycleSelf.recycleList.AddRange(AutoRecycleSelf.activeRecyclers);
		foreach (AutoRecycleSelf autoRecycleSelf in AutoRecycleSelf.recycleList)
		{
			autoRecycleSelf.RecycleSelf();
		}
		AutoRecycleSelf.recycleList.Clear();
	}

	// Token: 0x06001D05 RID: 7429 RVA: 0x00086D70 File Offset: 0x00084F70
	private IEnumerator RecycleAfterTime(float wait)
	{
		this.recycleTimerRunning = true;
		if (this.unscaledTime)
		{
			this.timer = Time.realtimeSinceStartup + wait;
			yield return new WaitForSecondsRealtime(wait);
		}
		else
		{
			this.timer = wait;
			yield return new WaitForSeconds(wait);
		}
		if (this.recycleTimerRunning)
		{
			this.RecycleSelf();
		}
		this.recycleTimerRunning = false;
		this.recycleTimer = null;
		yield break;
	}

	// Token: 0x06001D06 RID: 7430 RVA: 0x00086D86 File Offset: 0x00084F86
	private void RecycleSelf()
	{
		this.recycleTimerRunning = false;
		this.OnBeforeRecycle.Invoke();
		base.gameObject.Recycle();
		this.OnRecycled();
	}

	// Token: 0x06001D07 RID: 7431 RVA: 0x00086DAB File Offset: 0x00084FAB
	public void ForceRecycle()
	{
		this.RecycleSelf();
	}

	// Token: 0x06001D08 RID: 7432 RVA: 0x00086DB3 File Offset: 0x00084FB3
	public void ActivateTimer()
	{
		if (this.recycleTimerRunning)
		{
			return;
		}
		if (this.timeToWait > 0f)
		{
			this.recycleTimer = base.StartCoroutine(this.RecycleAfterTime(this.timeToWait));
		}
	}

	// Token: 0x06001D09 RID: 7433 RVA: 0x00086DE3 File Offset: 0x00084FE3
	private void FindChildren()
	{
		if (this.childrenValid)
		{
			return;
		}
		this.childrenValid = true;
		this.recycleChildren.Clear();
		this.recycleChildren.AddRange(base.GetComponentsInChildren<AutoRecycleSelf.IRecycleResponder>(true));
	}

	// Token: 0x06001D0A RID: 7434 RVA: 0x00086E14 File Offset: 0x00085014
	private void OnRecycled()
	{
		this.FindChildren();
		int num = 0;
		for (int i = 0; i < this.recycleChildren.Count; i++)
		{
			AutoRecycleSelf.IRecycleResponder recycleResponder = this.recycleChildren[i];
			if (recycleResponder != null)
			{
				recycleResponder.OnRecycled();
				this.recycleChildren[num++] = recycleResponder;
			}
		}
		if (num < this.recycleChildren.Count)
		{
			this.recycleChildren.RemoveRange(num, this.recycleChildren.Count - num);
		}
	}

	// Token: 0x04001C57 RID: 7255
	[Header("Trigger Event Type")]
	public AfterEvent afterEvent;

	// Token: 0x04001C58 RID: 7256
	[Header("Time Event Settings")]
	public float timeToWait;

	// Token: 0x04001C59 RID: 7257
	public bool unscaledTime;

	// Token: 0x04001C5A RID: 7258
	[Space]
	public UnityEvent OnBeforeRecycle;

	// Token: 0x04001C5B RID: 7259
	private AudioSource audioSource;

	// Token: 0x04001C5C RID: 7260
	private bool validAudioSource;

	// Token: 0x04001C5D RID: 7261
	private bool ApplicationIsQuitting;

	// Token: 0x04001C5E RID: 7262
	private bool subscribeLevelUnload;

	// Token: 0x04001C5F RID: 7263
	private Coroutine recycleTimer;

	// Token: 0x04001C60 RID: 7264
	private bool recycleTimerRunning;

	// Token: 0x04001C61 RID: 7265
	private static HashSet<AutoRecycleSelf> activeRecyclers = new HashSet<AutoRecycleSelf>(200);

	// Token: 0x04001C62 RID: 7266
	private static List<AutoRecycleSelf> recycleList = new List<AutoRecycleSelf>();

	// Token: 0x04001C63 RID: 7267
	private bool subbed;

	// Token: 0x04001C64 RID: 7268
	private float timer;

	// Token: 0x04001C65 RID: 7269
	private bool hasTk2dAnimator;

	// Token: 0x04001C66 RID: 7270
	private bool hasAnimator;

	// Token: 0x04001C67 RID: 7271
	private tk2dSpriteAnimator tk2dAnimator;

	// Token: 0x04001C68 RID: 7272
	private Animator animator;

	// Token: 0x04001C69 RID: 7273
	private int stateTracker;

	// Token: 0x04001C6A RID: 7274
	private bool childrenValid;

	// Token: 0x04001C6B RID: 7275
	private List<AutoRecycleSelf.IRecycleResponder> recycleChildren = new List<AutoRecycleSelf.IRecycleResponder>();

	// Token: 0x02001607 RID: 5639
	public interface IRecycleResponder
	{
		// Token: 0x060088B2 RID: 34994
		void OnRecycled();
	}
}
