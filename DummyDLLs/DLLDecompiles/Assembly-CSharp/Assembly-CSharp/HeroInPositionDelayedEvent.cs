using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000353 RID: 851
public sealed class HeroInPositionDelayedEvent : MonoBehaviour
{
	// Token: 0x06001D85 RID: 7557 RVA: 0x0008862E File Offset: 0x0008682E
	private void Start()
	{
		this.heroController = HeroController.instance;
		if (this.heroController)
		{
			if (this.heroController.isHeroInPosition)
			{
				this.TriggerEvents();
				return;
			}
			this.RegisterEvents();
		}
	}

	// Token: 0x06001D86 RID: 7558 RVA: 0x00088662 File Offset: 0x00086862
	private void OnDisable()
	{
		this.UnregisterEvents();
	}

	// Token: 0x06001D87 RID: 7559 RVA: 0x0008866A File Offset: 0x0008686A
	private void RegisterEvents()
	{
		if (!this.registeredEvent)
		{
			this.registeredEvent = true;
			this.heroController.heroInPositionDelayed += this.OnHeroSetPosition;
		}
	}

	// Token: 0x06001D88 RID: 7560 RVA: 0x00088692 File Offset: 0x00086892
	private void UnregisterEvents()
	{
		if (this.registeredEvent)
		{
			this.registeredEvent = false;
			this.heroController.heroInPositionDelayed -= this.OnHeroSetPosition;
		}
	}

	// Token: 0x06001D89 RID: 7561 RVA: 0x000886BA File Offset: 0x000868BA
	private void OnHeroSetPosition(bool forceDirect)
	{
		this.TriggerEvents();
		this.UnregisterEvents();
	}

	// Token: 0x06001D8A RID: 7562 RVA: 0x000886C8 File Offset: 0x000868C8
	private void TriggerEvents()
	{
		UnityEvent unityEvent = this.onHeroInPosition;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04001CC0 RID: 7360
	public UnityEvent onHeroInPosition;

	// Token: 0x04001CC1 RID: 7361
	private HeroController heroController;

	// Token: 0x04001CC2 RID: 7362
	private bool registeredEvent;
}
