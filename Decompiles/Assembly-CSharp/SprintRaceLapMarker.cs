using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200019C RID: 412
public class SprintRaceLapMarker : MonoBehaviour
{
	// Token: 0x0600101A RID: 4122 RVA: 0x0004DCEC File Offset: 0x0004BEEC
	private void OnEnable()
	{
		if (this.trigger)
		{
			this.trigger.OnTriggerEntered += this.OnTriggerEntered;
		}
	}

	// Token: 0x0600101B RID: 4123 RVA: 0x0004DD12 File Offset: 0x0004BF12
	private void OnDisable()
	{
		if (this.trigger)
		{
			this.trigger.OnTriggerEntered -= this.OnTriggerEntered;
		}
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x0004DD38 File Offset: 0x0004BF38
	private void OnTriggerEntered(Collider2D col, GameObject sender)
	{
		if (!this.controller)
		{
			return;
		}
		HeroController component = col.GetComponent<HeroController>();
		SplineRunner component2 = col.GetComponent<SplineRunner>();
		if (!component && !component2)
		{
			return;
		}
		if (!this.controller.IsTracking)
		{
			return;
		}
		if (this.isEndMarker && component && !this.controller.IsNextLapMarkerEnd)
		{
			return;
		}
		this.OnHeroEnteredAny.Invoke();
		if (!component)
		{
			return;
		}
		if (this.heroEnteredCorrectFunc != null && this.heroEnteredCorrectFunc(!this.isEndMarker))
		{
			this.OnHeroEnteredCorrect.Invoke();
			if (this.hazardRespawnMarker)
			{
				PlayerData.instance.SetHazardRespawn(this.hazardRespawnMarker);
				return;
			}
		}
		else
		{
			this.OnHeroEnteredIncorrect.Invoke();
		}
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x0004DE07 File Offset: 0x0004C007
	public void RegisterController(SprintRaceController newController, SprintRaceLapMarker.HeroEnteredCorrectDelegate newHeroEnteredCorrectFunc)
	{
		this.controller = newController;
		this.heroEnteredCorrectFunc = newHeroEnteredCorrectFunc;
	}

	// Token: 0x04000FAF RID: 4015
	[SerializeField]
	private TriggerEnterEvent trigger;

	// Token: 0x04000FB0 RID: 4016
	[SerializeField]
	private bool isEndMarker;

	// Token: 0x04000FB1 RID: 4017
	[SerializeField]
	private HazardRespawnMarker hazardRespawnMarker;

	// Token: 0x04000FB2 RID: 4018
	[Space]
	public UnityEvent OnHeroEnteredIncorrect;

	// Token: 0x04000FB3 RID: 4019
	public UnityEvent OnHeroEnteredCorrect;

	// Token: 0x04000FB4 RID: 4020
	public UnityEvent OnHeroEnteredAny;

	// Token: 0x04000FB5 RID: 4021
	private SprintRaceController controller;

	// Token: 0x04000FB6 RID: 4022
	private SprintRaceLapMarker.HeroEnteredCorrectDelegate heroEnteredCorrectFunc;

	// Token: 0x020014DD RID: 5341
	// (Invoke) Token: 0x06008517 RID: 34071
	public delegate bool HeroEnteredCorrectDelegate(bool canDisqualify);
}
