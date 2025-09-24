using System;
using UnityEngine;

// Token: 0x02000360 RID: 864
public sealed class PinChallenge : MonoBehaviour
{
	// Token: 0x06001DE6 RID: 7654 RVA: 0x0008A610 File Offset: 0x00088810
	private void Awake()
	{
		EventRegister.GetRegisterGuaranteed(base.gameObject, "PIN CHALLENGE START").ReceivedEvent += this.OnPinChallengeStart;
		EventRegister.GetRegisterGuaranteed(base.gameObject, "PIN CHALLENGE END").ReceivedEvent += this.OnPinChallengeEnd;
		ToolItemManager.BoundAttackToolUsed += this.ToolItemManagerOnBoundAttackToolUsed;
		this.hasRestBench = this.restBenchInteractable;
		base.enabled = false;
	}

	// Token: 0x06001DE7 RID: 7655 RVA: 0x0008A688 File Offset: 0x00088888
	private void OnValidate()
	{
		this.hasRestBench = this.restBenchInteractable;
	}

	// Token: 0x06001DE8 RID: 7656 RVA: 0x0008A69B File Offset: 0x0008889B
	private void OnDestroy()
	{
		ToolItemManager.BoundAttackToolUsed -= this.ToolItemManagerOnBoundAttackToolUsed;
	}

	// Token: 0x06001DE9 RID: 7657 RVA: 0x0008A6AE File Offset: 0x000888AE
	private void Update()
	{
		this.timer -= Time.deltaTime;
		if (this.timer <= 0f)
		{
			this.ToggleBenchPause(false);
		}
	}

	// Token: 0x06001DEA RID: 7658 RVA: 0x0008A6D6 File Offset: 0x000888D6
	private void OnPinChallengeStart()
	{
		this.isActive = true;
	}

	// Token: 0x06001DEB RID: 7659 RVA: 0x0008A6DF File Offset: 0x000888DF
	private void OnPinChallengeEnd()
	{
		this.isActive = false;
		this.ToggleBenchPause(false);
	}

	// Token: 0x06001DEC RID: 7660 RVA: 0x0008A6EF File Offset: 0x000888EF
	private void ToolItemManagerOnBoundAttackToolUsed(AttackToolBinding binding)
	{
		if (!this.isActive || !this.hasRestBench)
		{
			return;
		}
		this.ToggleBenchPause(true);
	}

	// Token: 0x06001DED RID: 7661 RVA: 0x0008A70C File Offset: 0x0008890C
	private void ToggleBenchPause(bool active)
	{
		if (this.pausedInteractable == active)
		{
			return;
		}
		this.pausedInteractable = active;
		if (this.pausedInteractable)
		{
			this.restBenchInteractable.Deactivate(false);
			this.timer = this.pauseDuration;
			base.enabled = true;
			return;
		}
		this.restBenchInteractable.Activate();
		base.enabled = false;
	}

	// Token: 0x04001D0E RID: 7438
	[SerializeField]
	private InteractableBase restBenchInteractable;

	// Token: 0x04001D0F RID: 7439
	[SerializeField]
	private float pauseDuration = 2f;

	// Token: 0x04001D10 RID: 7440
	private bool isActive;

	// Token: 0x04001D11 RID: 7441
	private bool hasRestBench;

	// Token: 0x04001D12 RID: 7442
	private bool pausedInteractable;

	// Token: 0x04001D13 RID: 7443
	private float timer;
}
