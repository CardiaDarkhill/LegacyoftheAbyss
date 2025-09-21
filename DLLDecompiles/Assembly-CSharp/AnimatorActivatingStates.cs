using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200048B RID: 1163
public class AnimatorActivatingStates : ActivatingBase
{
	// Token: 0x060029FB RID: 10747 RVA: 0x000B6509 File Offset: 0x000B4709
	protected override void Awake()
	{
		base.Awake();
		this.UpdateHash();
		this.hasAnimator = (this.animator != null);
	}

	// Token: 0x060029FC RID: 10748 RVA: 0x000B6529 File Offset: 0x000B4729
	protected override void Start()
	{
		base.Start();
		this.hasStarted = true;
	}

	// Token: 0x060029FD RID: 10749 RVA: 0x000B6538 File Offset: 0x000B4738
	protected virtual void OnValidate()
	{
		this.UpdateHash();
	}

	// Token: 0x060029FE RID: 10750 RVA: 0x000B6540 File Offset: 0x000B4740
	private void UpdateHash()
	{
		this.deactivate = new AnimatorHashCache(this.deactivateAnim);
		this.activate = new AnimatorHashCache(this.activateAnim);
		this.reactivate = new AnimatorHashCache(this.reactivateAnim);
	}

	// Token: 0x060029FF RID: 10751 RVA: 0x000B6578 File Offset: 0x000B4778
	protected override void OnActiveStateUpdate(bool value, bool isInstant)
	{
		if (!this.hasAnimator)
		{
			return;
		}
		bool fromEnd = !this.hasStarted || isInstant;
		int animHash;
		if (value)
		{
			animHash = ((!base.IsActive) ? this.activate.Hash : this.reactivate.Hash);
		}
		else
		{
			animHash = this.deactivate.Hash;
		}
		this.PlayAnim(animHash, fromEnd);
	}

	// Token: 0x06002A00 RID: 10752 RVA: 0x000B65D4 File Offset: 0x000B47D4
	protected void PlayAnim(string animName, bool fromEnd)
	{
		ActivatingBase.PlayAnim(this, this.animator, animName, fromEnd);
	}

	// Token: 0x06002A01 RID: 10753 RVA: 0x000B65E4 File Offset: 0x000B47E4
	protected void PlayAnim(int animHash, bool fromEnd)
	{
		ActivatingBase.PlayAnim(this, this.animator, animHash, fromEnd);
	}

	// Token: 0x06002A02 RID: 10754 RVA: 0x000B65F4 File Offset: 0x000B47F4
	protected override void OnDeactivateWarning()
	{
		this.OnWarned.Invoke();
	}

	// Token: 0x06002A03 RID: 10755 RVA: 0x000B6601 File Offset: 0x000B4801
	protected override void OnDeactivate()
	{
		this.OnDeactivated.Invoke();
	}

	// Token: 0x04002A77 RID: 10871
	[SerializeField]
	private Animator animator;

	// Token: 0x04002A78 RID: 10872
	[SerializeField]
	private string deactivateAnim;

	// Token: 0x04002A79 RID: 10873
	[SerializeField]
	private string activateAnim;

	// Token: 0x04002A7A RID: 10874
	[SerializeField]
	private string reactivateAnim;

	// Token: 0x04002A7B RID: 10875
	[Space]
	public UnityEvent OnWarned;

	// Token: 0x04002A7C RID: 10876
	public UnityEvent OnDeactivated;

	// Token: 0x04002A7D RID: 10877
	private bool hasAnimator;

	// Token: 0x04002A7E RID: 10878
	private bool hasStarted;

	// Token: 0x04002A7F RID: 10879
	private AnimatorHashCache deactivate;

	// Token: 0x04002A80 RID: 10880
	private AnimatorHashCache activate;

	// Token: 0x04002A81 RID: 10881
	private AnimatorHashCache reactivate;
}
