using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

// Token: 0x02000575 RID: 1397
public class TimelineGate : UnlockablePropBase
{
	// Token: 0x0600320B RID: 12811 RVA: 0x000DEDE0 File Offset: 0x000DCFE0
	private void Start()
	{
		PersistentBoolItem component = base.GetComponent<PersistentBoolItem>();
		if (component)
		{
			component.OnGetSaveState += delegate(out bool value)
			{
				value = this.activated;
			};
			component.OnSetSaveState += delegate(bool value)
			{
				this.activated = value;
				if (this.activated)
				{
					this.Opened();
				}
			};
		}
		this.timeline.Stop();
		this.timeline.time = 0.0;
		this.timeline.Evaluate();
	}

	// Token: 0x0600320C RID: 12812 RVA: 0x000DEE4C File Offset: 0x000DD04C
	public override void Open()
	{
		if (!this.activated)
		{
			this.activated = true;
			float randomValue = this.openDelay.GetRandomValue();
			if (randomValue <= 0f)
			{
				this.onBeforeDelay.Invoke();
				this.DoOpen();
				return;
			}
			this.onBeforeDelay.Invoke();
			this.ExecuteDelayed(randomValue, new Action(this.DoOpen));
		}
	}

	// Token: 0x0600320D RID: 12813 RVA: 0x000DEEB0 File Offset: 0x000DD0B0
	public override void Opened()
	{
		this.onOpened.Invoke();
		if (this.deactivateIfOpened)
		{
			base.gameObject.SetActive(false);
			return;
		}
		this.timeline.Stop();
		this.timeline.time = this.timeline.duration;
		this.timeline.Evaluate();
	}

	// Token: 0x0600320E RID: 12814 RVA: 0x000DEF09 File Offset: 0x000DD109
	private void DoOpen()
	{
		this.onOpen.Invoke();
		this.timeline.Play();
	}

	// Token: 0x040035A2 RID: 13730
	[SerializeField]
	private PlayableDirector timeline;

	// Token: 0x040035A3 RID: 13731
	[SerializeField]
	private bool deactivateIfOpened;

	// Token: 0x040035A4 RID: 13732
	[SerializeField]
	private MinMaxFloat openDelay;

	// Token: 0x040035A5 RID: 13733
	[Space]
	[SerializeField]
	private UnityEvent onBeforeDelay;

	// Token: 0x040035A6 RID: 13734
	[SerializeField]
	private UnityEvent onOpen;

	// Token: 0x040035A7 RID: 13735
	[SerializeField]
	private UnityEvent onOpened;

	// Token: 0x040035A8 RID: 13736
	private bool activated;
}
