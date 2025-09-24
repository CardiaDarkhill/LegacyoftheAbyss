using System;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000066 RID: 102
public class AnimatorSequence : SkippableSequence
{
	// Token: 0x06000290 RID: 656 RVA: 0x0000ED99 File Offset: 0x0000CF99
	protected void Awake()
	{
		this.fadeByController = 1f;
		if (this.continueStop)
		{
			this.continueStop.SetActive(false);
		}
	}

	// Token: 0x06000291 RID: 657 RVA: 0x0000EDBF File Offset: 0x0000CFBF
	public override void AllowSkip()
	{
		base.AllowSkip();
		if (this.continueStop)
		{
			this.continueStop.SetActive(true);
		}
	}

	// Token: 0x06000292 RID: 658 RVA: 0x0000EDE0 File Offset: 0x0000CFE0
	public override void Begin()
	{
		this.animator.gameObject.SetActive(true);
		this.animator.Play(this.animatorStateName, 0, 0f);
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x06000293 RID: 659 RVA: 0x0000EE0C File Offset: 0x0000D00C
	public override bool IsPlaying
	{
		get
		{
			if (!this.animator.isActiveAndEnabled)
			{
				return false;
			}
			float normalizedTime = this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
			float num = 1f - Mathf.Epsilon;
			if (this.isSkipped && !string.IsNullOrEmpty(this.skipStateName))
			{
				return normalizedTime < num;
			}
			return normalizedTime < Mathf.Min(this.normalizedFinishTime, num);
		}
	}

	// Token: 0x06000294 RID: 660 RVA: 0x0000EE74 File Offset: 0x0000D074
	public override void Skip()
	{
		if (this.isSkipped)
		{
			return;
		}
		this.isSkipped = true;
		if (base.WaitForSkip)
		{
			Audio.StopConfirmSound.SpawnAndPlayOneShot(Audio.DefaultUIAudioSourcePrefab, base.transform.position, null);
		}
		if (this.continueStop)
		{
			this.continueStop.SetActive(false);
		}
		if (!string.IsNullOrEmpty(this.skipStateName))
		{
			this.animator.Play(this.skipStateName);
			base.CanSkip = false;
			return;
		}
		OverrideFloat overrideFloat = this.normalizedSkipTime;
		if (overrideFloat != null && overrideFloat.IsEnabled)
		{
			AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(0);
			this.animator.Play(currentAnimatorStateInfo.shortNameHash, 0, this.normalizedSkipTime.Value);
			return;
		}
		this.animator.Update(1000f);
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x06000295 RID: 661 RVA: 0x0000EF45 File Offset: 0x0000D145
	public override bool IsSkipped
	{
		get
		{
			return this.isSkipped;
		}
	}

	// Token: 0x17000022 RID: 34
	// (get) Token: 0x06000296 RID: 662 RVA: 0x0000EF4D File Offset: 0x0000D14D
	// (set) Token: 0x06000297 RID: 663 RVA: 0x0000EF55 File Offset: 0x0000D155
	public override float FadeByController
	{
		get
		{
			return this.fadeByController;
		}
		set
		{
			this.fadeByController = value;
		}
	}

	// Token: 0x04000233 RID: 563
	[SerializeField]
	private Animator animator;

	// Token: 0x04000234 RID: 564
	[SerializeField]
	private string animatorStateName;

	// Token: 0x04000235 RID: 565
	[SerializeField]
	private float normalizedFinishTime;

	// Token: 0x04000236 RID: 566
	[SerializeField]
	private OverrideFloat normalizedSkipTime;

	// Token: 0x04000237 RID: 567
	[SerializeField]
	private string skipStateName;

	// Token: 0x04000238 RID: 568
	[Space]
	[SerializeField]
	private GameObject continueStop;

	// Token: 0x04000239 RID: 569
	private float fadeByController;

	// Token: 0x0400023A RID: 570
	private bool isSkipped;
}
