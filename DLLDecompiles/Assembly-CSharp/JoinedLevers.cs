using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200050F RID: 1295
public class JoinedLevers : MonoBehaviour
{
	// Token: 0x06002E32 RID: 11826 RVA: 0x000CAE44 File Offset: 0x000C9044
	private void Awake()
	{
		if (this.readPersistent)
		{
			this.readPersistent.OnSetSaveState += this.OnSetSaveState;
		}
		foreach (Lever lever in this.levers)
		{
			if (lever)
			{
				lever.OnHit.AddListener(new UnityAction(this.OnAnyLeverHit));
			}
		}
	}

	// Token: 0x06002E33 RID: 11827 RVA: 0x000CAEB0 File Offset: 0x000C90B0
	private void OnAnyLeverHit()
	{
		foreach (Lever lever in this.levers)
		{
			if (lever)
			{
				lever.SetActivatedInert(true);
			}
		}
	}

	// Token: 0x06002E34 RID: 11828 RVA: 0x000CAEE8 File Offset: 0x000C90E8
	private void OnSetSaveState(bool value)
	{
		if (!value)
		{
			return;
		}
		foreach (Lever lever in this.levers)
		{
			if (lever)
			{
				lever.SetActivatedInert(true);
			}
		}
		if (this.animator && this.animator.HasParameter(JoinedLevers._activateAnimParam, null))
		{
			this.animator.SetTrigger(JoinedLevers._activateAnimParam);
		}
	}

	// Token: 0x06002E35 RID: 11829 RVA: 0x000CAF5C File Offset: 0x000C915C
	public void ResetLevers()
	{
		if (this.isForced)
		{
			return;
		}
		foreach (Lever lever in this.levers)
		{
			if (lever)
			{
				lever.SetActivatedInert(false);
			}
		}
		if (this.animator && this.animator.HasParameter(JoinedLevers._resetAnimParam, null))
		{
			this.animator.SetTrigger(JoinedLevers._resetAnimParam);
		}
	}

	// Token: 0x06002E36 RID: 11830 RVA: 0x000CAFD2 File Offset: 0x000C91D2
	public void SetLeversActivated()
	{
		this.OnSetSaveState(true);
	}

	// Token: 0x06002E37 RID: 11831 RVA: 0x000CAFDB File Offset: 0x000C91DB
	public void ForceActivateLevers()
	{
		this.isForced = true;
		this.OnAnyLeverHit();
		this.PlayHitAnim();
	}

	// Token: 0x06002E38 RID: 11832 RVA: 0x000CAFF0 File Offset: 0x000C91F0
	public void PlayHitAnim()
	{
		if (!this.animator)
		{
			return;
		}
		int shortNameHash = this.animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
		if (shortNameHash == JoinedLevers._hitCAnim || shortNameHash == JoinedLevers._hitCcAnim)
		{
			return;
		}
		if (this.animator.HasParameter(JoinedLevers._hitAnimParam, null))
		{
			this.animator.SetTrigger(JoinedLevers._hitAnimParam);
		}
	}

	// Token: 0x06002E39 RID: 11833 RVA: 0x000CB05C File Offset: 0x000C925C
	public void ForceResetLevers()
	{
		this.isForced = false;
		this.ResetLevers();
	}

	// Token: 0x04003076 RID: 12406
	private static readonly int _hitAnimParam = Animator.StringToHash("Hit");

	// Token: 0x04003077 RID: 12407
	private static readonly int _hitCAnim = Animator.StringToHash("Hit C");

	// Token: 0x04003078 RID: 12408
	private static readonly int _hitCcAnim = Animator.StringToHash("Hit CC");

	// Token: 0x04003079 RID: 12409
	private static readonly int _activateAnimParam = Animator.StringToHash("Activate");

	// Token: 0x0400307A RID: 12410
	private static readonly int _resetAnimParam = Animator.StringToHash("Reset");

	// Token: 0x0400307B RID: 12411
	[SerializeField]
	private Lever[] levers;

	// Token: 0x0400307C RID: 12412
	[SerializeField]
	private Animator animator;

	// Token: 0x0400307D RID: 12413
	[SerializeField]
	private PersistentBoolItem readPersistent;

	// Token: 0x0400307E RID: 12414
	private bool isForced;
}
