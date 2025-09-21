using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005FB RID: 1531
public class TrackTriggerResponder : MonoBehaviour
{
	// Token: 0x060036A2 RID: 13986 RVA: 0x000F1093 File Offset: 0x000EF293
	private void OnEnable()
	{
		this.wasInside = this.trigger.IsInside;
		this.trigger.InsideStateChanged += this.OnInsideStateChanged;
	}

	// Token: 0x060036A3 RID: 13987 RVA: 0x000F10BD File Offset: 0x000EF2BD
	private void OnDisable()
	{
		if (this.trigger)
		{
			this.trigger.InsideStateChanged -= this.OnInsideStateChanged;
		}
	}

	// Token: 0x060036A4 RID: 13988 RVA: 0x000F10E4 File Offset: 0x000EF2E4
	private void OnInsideStateChanged(bool isInside)
	{
		if (isInside == this.wasInside)
		{
			return;
		}
		this.wasInside = isInside;
		if (this.intersectingTracker && !this.intersectingTracker.IsTargetIntersecting)
		{
			return;
		}
		if (isInside)
		{
			this.OnEntered.Invoke();
			return;
		}
		this.OnExited.Invoke();
	}

	// Token: 0x0400396E RID: 14702
	[SerializeField]
	private TrackTriggerObjects trigger;

	// Token: 0x0400396F RID: 14703
	[SerializeField]
	private FadeUpWhileIntersecting intersectingTracker;

	// Token: 0x04003970 RID: 14704
	[Space]
	public UnityEvent OnEntered;

	// Token: 0x04003971 RID: 14705
	public UnityEvent OnExited;

	// Token: 0x04003972 RID: 14706
	private bool wasInside;
}
