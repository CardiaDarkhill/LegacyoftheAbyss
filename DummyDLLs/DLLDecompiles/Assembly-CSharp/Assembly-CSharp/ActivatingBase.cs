using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000487 RID: 1159
public abstract class ActivatingBase : DebugDrawColliderRuntimeAdder
{
	// Token: 0x170004F9 RID: 1273
	// (get) Token: 0x060029D8 RID: 10712 RVA: 0x000B5DE4 File Offset: 0x000B3FE4
	// (set) Token: 0x060029D9 RID: 10713 RVA: 0x000B5DEC File Offset: 0x000B3FEC
	public bool IsActive { get; private set; }

	// Token: 0x170004FA RID: 1274
	// (get) Token: 0x060029DA RID: 10714 RVA: 0x000B5DF5 File Offset: 0x000B3FF5
	public virtual bool IsPaused
	{
		get
		{
			return false;
		}
	}

	// Token: 0x170004FB RID: 1275
	// (get) Token: 0x060029DB RID: 10715 RVA: 0x000B5DF8 File Offset: 0x000B3FF8
	public int BranchIndex
	{
		get
		{
			return this.branchIndex;
		}
	}

	// Token: 0x060029DC RID: 10716 RVA: 0x000B5E00 File Offset: 0x000B4000
	protected virtual void Start()
	{
		this.IsActive = !this.startActive;
		this.SetActive(this.startActive, false);
	}

	// Token: 0x060029DD RID: 10717 RVA: 0x000B5E20 File Offset: 0x000B4020
	public void SetActive(bool value, bool isInstant = false)
	{
		if (this.setActiveDelayedRoutine != null)
		{
			base.StopCoroutine(this.setActiveDelayedRoutine);
			this.setActiveDelayedRoutine = null;
		}
		if (isInstant || this.activateDelay <= 0f)
		{
			this.SetActiveInstant(value, isInstant);
			return;
		}
		this.targetActiveState = value;
		this.setActiveDelayedRoutine = this.ExecuteDelayed(this.activateDelay, new Action(this.SetActiveDelayed));
	}

	// Token: 0x060029DE RID: 10718 RVA: 0x000B5E86 File Offset: 0x000B4086
	public void Toggle()
	{
		this.SetActive(!this.IsActive, false);
	}

	// Token: 0x060029DF RID: 10719 RVA: 0x000B5E98 File Offset: 0x000B4098
	public void DeactivateWarning()
	{
		this.OnDeactivateWarning();
	}

	// Token: 0x060029E0 RID: 10720 RVA: 0x000B5EA0 File Offset: 0x000B40A0
	private void SetActiveDelayed()
	{
		this.SetActiveInstant(this.targetActiveState, false);
	}

	// Token: 0x060029E1 RID: 10721 RVA: 0x000B5EAF File Offset: 0x000B40AF
	private void SetActiveInstant(bool value, bool isInstant)
	{
		this.OnActiveStateUpdate(value, isInstant);
		if (this.IsActive != value)
		{
			this.IsActive = value;
			if (value)
			{
				this.OnActivate();
				return;
			}
			this.OnDeactivate();
		}
	}

	// Token: 0x060029E2 RID: 10722
	protected abstract void OnActiveStateUpdate(bool isActive, bool isInstant);

	// Token: 0x060029E3 RID: 10723 RVA: 0x000B5ED9 File Offset: 0x000B40D9
	protected virtual void OnActivate()
	{
	}

	// Token: 0x060029E4 RID: 10724 RVA: 0x000B5EDB File Offset: 0x000B40DB
	protected virtual void OnDeactivateWarning()
	{
	}

	// Token: 0x060029E5 RID: 10725 RVA: 0x000B5EDD File Offset: 0x000B40DD
	protected virtual void OnDeactivate()
	{
	}

	// Token: 0x060029E6 RID: 10726 RVA: 0x000B5EE0 File Offset: 0x000B40E0
	protected static void PlayAnim(ActivatingBase runner, Animator animator, string animName, bool fromEnd)
	{
		if (animator.cullingMode != AnimatorCullingMode.AlwaysAnimate)
		{
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}
		if (runner.cullWaitRoutine != null)
		{
			runner.StopCoroutine(runner.cullWaitRoutine);
			runner.cullWaitRoutine = null;
		}
		animator.Play(animName, 0, fromEnd ? 1f : 0f);
		if (fromEnd)
		{
			animator.Update(0f);
			animator.cullingMode = AnimatorCullingMode.CullCompletely;
			return;
		}
		runner.cullWaitRoutine = runner.StartCoroutine(ActivatingBase.CullAfterAnim(runner, animator));
	}

	// Token: 0x060029E7 RID: 10727 RVA: 0x000B5F58 File Offset: 0x000B4158
	protected static void PlayAnim(ActivatingBase runner, Animator animator, int animHash, bool fromEnd)
	{
		if (animator.cullingMode != AnimatorCullingMode.AlwaysAnimate)
		{
			animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
		}
		if (runner.cullWaitRoutine != null)
		{
			runner.StopCoroutine(runner.cullWaitRoutine);
			runner.cullWaitRoutine = null;
		}
		animator.Play(animHash, 0, fromEnd ? 1f : 0f);
		if (fromEnd)
		{
			animator.Update(0f);
			animator.cullingMode = AnimatorCullingMode.CullCompletely;
			return;
		}
		runner.cullWaitRoutine = runner.StartCoroutine(ActivatingBase.CullAfterAnim(runner, animator));
	}

	// Token: 0x060029E8 RID: 10728 RVA: 0x000B5FCF File Offset: 0x000B41CF
	private static IEnumerator CullAfterAnim(ActivatingBase runner, Animator animator)
	{
		yield return null;
		AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
		if (state.loop)
		{
			yield return new WaitForSeconds(state.length);
		}
		else
		{
			AnimatorStateInfo currentAnimatorStateInfo;
			do
			{
				yield return null;
				currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
			}
			while (currentAnimatorStateInfo.normalizedTime < 1f - Mathf.Epsilon && currentAnimatorStateInfo.fullPathHash == state.fullPathHash);
		}
		yield return null;
		animator.cullingMode = AnimatorCullingMode.CullCompletely;
		runner.cullWaitRoutine = null;
		yield break;
	}

	// Token: 0x060029E9 RID: 10729 RVA: 0x000B5FE5 File Offset: 0x000B41E5
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.TerrainCollider, false);
	}

	// Token: 0x04002A55 RID: 10837
	[SerializeField]
	private int branchIndex;

	// Token: 0x04002A56 RID: 10838
	[Space]
	[SerializeField]
	private bool startActive = true;

	// Token: 0x04002A57 RID: 10839
	[SerializeField]
	private float activateDelay;

	// Token: 0x04002A58 RID: 10840
	private Coroutine setActiveDelayedRoutine;

	// Token: 0x04002A59 RID: 10841
	private bool targetActiveState;

	// Token: 0x04002A5A RID: 10842
	private Coroutine cullWaitRoutine;
}
