using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x02000626 RID: 1574
public class CurrencyCounterStack : MonoBehaviour
{
	// Token: 0x0600381F RID: 14367 RVA: 0x000F8048 File Offset: 0x000F6248
	private void Awake()
	{
		Transform transform = base.transform;
		this.initialParent = transform.parent;
		this.initialPosition = transform.localPosition;
		this.initialScale = transform.localScale;
	}

	// Token: 0x06003820 RID: 14368 RVA: 0x000F8080 File Offset: 0x000F6280
	[UsedImplicitly]
	public void ReportVisibleChange(bool isVisible)
	{
		if (this.wasVisible == isVisible)
		{
			return;
		}
		Transform transform = base.transform;
		if (isVisible)
		{
			if (this.inPositioner)
			{
				this.inPositioner.enabled = true;
			}
			transform.SetParent(this.initialParent);
			transform.localPosition = this.initialPosition;
			transform.localScale = this.initialScale;
			CurrencyCounterBase.FadeInIfActive();
		}
		else
		{
			if (this.inPositioner)
			{
				this.inPositioner.enabled = false;
			}
			transform.SetParent(this.outAppearParent);
			transform.SetLocalPosition2D(this.outAppearPosition);
			transform.localScale = this.initialScale;
			CurrencyCounterBase.HideAllInstant();
		}
		this.wasVisible = isVisible;
	}

	// Token: 0x06003821 RID: 14369 RVA: 0x000F8130 File Offset: 0x000F6330
	public void AddNewCounter(CurrencyCounterBase counter)
	{
		CurrencyCounterBase counter2 = counter;
		counter2.Appeared = (Action)Delegate.Combine(counter2.Appeared, new Action(delegate()
		{
			if (this.activeCounters.Add(counter))
			{
				int num = -1;
				for (int i = this.currentCounters.Count - 1; i >= 0; i--)
				{
					if (this.currentCounters[i].StackOrder <= counter.StackOrder)
					{
						num = i + 1;
						break;
					}
					num = i;
				}
				if (num < 0 || num >= this.currentCounters.Count)
				{
					this.currentCounters.Add(counter);
					this.SetPosition(counter);
					return;
				}
				this.currentCounters.Insert(num, counter);
				for (int j = 0; j < this.currentCounters.Count; j++)
				{
					Transform transform = this.currentCounters[j].transform;
					Vector3 localPosition = transform.localPosition;
					float? y = new float?(this.yOffset * (float)j);
					transform.localPosition = localPosition.Where(null, y, null);
				}
			}
		}));
		CurrencyCounterBase counter3 = counter;
		counter3.Disappeared = (Action)Delegate.Combine(counter3.Disappeared, new Action(delegate()
		{
			if (this.activeCounters.Remove(counter) && this.activeCounters.Count == 0)
			{
				this.currentCounters.Clear();
			}
		}));
	}

	// Token: 0x06003822 RID: 14370 RVA: 0x000F81A0 File Offset: 0x000F63A0
	private void SetPosition(CurrencyCounterBase counter)
	{
		Transform transform = counter.transform;
		Vector3 localPosition = transform.localPosition;
		float? y = new float?(this.yOffset * (float)this.currentCounters.IndexOf(counter));
		transform.localPosition = localPosition.Where(null, y, null);
	}

	// Token: 0x04003B13 RID: 15123
	[SerializeField]
	private float yOffset;

	// Token: 0x04003B14 RID: 15124
	[Space]
	[SerializeField]
	private Transform outAppearParent;

	// Token: 0x04003B15 RID: 15125
	[SerializeField]
	private Vector2 outAppearPosition;

	// Token: 0x04003B16 RID: 15126
	[Space]
	[SerializeField]
	private PositionRelativeTo inPositioner;

	// Token: 0x04003B17 RID: 15127
	private Transform initialParent;

	// Token: 0x04003B18 RID: 15128
	private Vector3 initialPosition;

	// Token: 0x04003B19 RID: 15129
	private Vector3 initialScale;

	// Token: 0x04003B1A RID: 15130
	private bool isVisibleTargetNull;

	// Token: 0x04003B1B RID: 15131
	private bool wasVisible = true;

	// Token: 0x04003B1C RID: 15132
	private readonly HashSet<CurrencyCounterBase> activeCounters = new HashSet<CurrencyCounterBase>();

	// Token: 0x04003B1D RID: 15133
	private readonly List<CurrencyCounterBase> currentCounters = new List<CurrencyCounterBase>();
}
