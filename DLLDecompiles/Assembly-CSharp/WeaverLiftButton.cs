using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000588 RID: 1416
public class WeaverLiftButton : MonoBehaviour
{
	// Token: 0x060032AE RID: 12974 RVA: 0x000E1A10 File Offset: 0x000DFC10
	private void Awake()
	{
		if (this.hitResponder)
		{
			this.hitResponder.OnHit.AddListener(new UnityAction(this.OnHit));
		}
		this.Setup();
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.isActive;
			};
			this.persistent.OnSetSaveState += delegate(bool value)
			{
				this.isActive = value;
				this.Setup();
			};
		}
	}

	// Token: 0x060032AF RID: 12975 RVA: 0x000E1A88 File Offset: 0x000DFC88
	private void Setup()
	{
		if (this.activateWaitRoutine != null)
		{
			base.StopCoroutine(this.activateWaitRoutine);
		}
		if (this.isActive)
		{
			this.animator.enabled = true;
			this.animator.Play(WeaverLiftButton.IdleAnim);
			return;
		}
		this.animator.Play(WeaverLiftButton.AppearAnim, 0, 0f);
		this.animator.Update(0f);
		this.animator.enabled = false;
		this.activateWaitRoutine = base.StartCoroutine(this.ActivateWait());
	}

	// Token: 0x060032B0 RID: 12976 RVA: 0x000E1B12 File Offset: 0x000DFD12
	private void OnHit()
	{
		Debug.LogError("Direction button deprecated!", this);
	}

	// Token: 0x060032B1 RID: 12977 RVA: 0x000E1B1F File Offset: 0x000DFD1F
	private bool CanAppear()
	{
		return this.targetLift.IsAvailable && this.targetLift.HasDirections;
	}

	// Token: 0x060032B2 RID: 12978 RVA: 0x000E1B3B File Offset: 0x000DFD3B
	private IEnumerator ActivateWait()
	{
		while (!this.firstAppearRange.IsInside || !this.CanAppear())
		{
			yield return null;
		}
		yield return new WaitForSeconds(this.firstAppearDelay);
		this.isActive = true;
		this.animator.enabled = true;
		this.animator.Play(WeaverLiftButton.AppearAnim, 0, 0f);
		CaptureAnimationEvent component = this.animator.GetComponent<CaptureAnimationEvent>();
		if (component)
		{
			WeaverLiftButton.<>c__DisplayClass15_0 CS$<>8__locals1 = new WeaverLiftButton.<>c__DisplayClass15_0();
			CS$<>8__locals1.isWaiting = true;
			component.EventFiredTemp += delegate()
			{
				CS$<>8__locals1.isWaiting = false;
			};
			while (CS$<>8__locals1.isWaiting)
			{
				yield return null;
			}
			CS$<>8__locals1 = null;
		}
		yield break;
	}

	// Token: 0x04003695 RID: 13973
	private static readonly int IdleAnim = Animator.StringToHash("Idle");

	// Token: 0x04003696 RID: 13974
	private static readonly int AppearAnim = Animator.StringToHash("Appear");

	// Token: 0x04003697 RID: 13975
	private static readonly int HitAnim = Animator.StringToHash("Hit");

	// Token: 0x04003698 RID: 13976
	[SerializeField]
	private HitResponse hitResponder;

	// Token: 0x04003699 RID: 13977
	[SerializeField]
	private Animator animator;

	// Token: 0x0400369A RID: 13978
	[SerializeField]
	private WeaverLift targetLift;

	// Token: 0x0400369B RID: 13979
	[SerializeField]
	private TrackTriggerObjects firstAppearRange;

	// Token: 0x0400369C RID: 13980
	[SerializeField]
	private float firstAppearDelay;

	// Token: 0x0400369D RID: 13981
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x0400369E RID: 13982
	private bool isActive;

	// Token: 0x0400369F RID: 13983
	private Coroutine activateWaitRoutine;
}
