using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000311 RID: 785
public class PlayMakerEventRange : MonoBehaviour
{
	// Token: 0x06001BBF RID: 7103 RVA: 0x00081739 File Offset: 0x0007F939
	private void OnDrawGizmosSelected()
	{
		if (this.sendRange <= 0f)
		{
			return;
		}
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, this.sendRange);
	}

	// Token: 0x06001BC0 RID: 7104 RVA: 0x00081769 File Offset: 0x0007F969
	private void OnEnable()
	{
		PlayMakerEventRange.allRanges.Add(this);
	}

	// Token: 0x06001BC1 RID: 7105 RVA: 0x00081776 File Offset: 0x0007F976
	private void OnDisable()
	{
		PlayMakerEventRange.allRanges.Remove(this);
	}

	// Token: 0x06001BC2 RID: 7106 RVA: 0x00081784 File Offset: 0x0007F984
	public void SendEvent(string eventName, bool excludeThis)
	{
		if (string.IsNullOrEmpty(eventName))
		{
			return;
		}
		this.SendEventRecursive(eventName, excludeThis);
		foreach (PlayMakerEventRange playMakerEventRange in PlayMakerEventRange.allRanges)
		{
			playMakerEventRange.handledEvents.Clear();
		}
	}

	// Token: 0x06001BC3 RID: 7107 RVA: 0x000817EC File Offset: 0x0007F9EC
	private void SendEventRecursive(string eventName, bool excludeThis)
	{
		if (this.sendRange <= 0f)
		{
			return;
		}
		if (this.handledEvents.Contains(eventName))
		{
			return;
		}
		this.handledEvents.Add(eventName);
		if (!excludeThis && this.targetFSM)
		{
			this.targetFSM.SendEvent(eventName);
		}
		foreach (PlayMakerEventRange playMakerEventRange in PlayMakerEventRange.allRanges)
		{
			if (!(playMakerEventRange == this) && Vector2.Distance(base.transform.position, playMakerEventRange.transform.position) <= this.sendRange)
			{
				playMakerEventRange.SendEventRecursive(eventName, false);
			}
		}
	}

	// Token: 0x04001ACC RID: 6860
	private static List<PlayMakerEventRange> allRanges = new List<PlayMakerEventRange>();

	// Token: 0x04001ACD RID: 6861
	[SerializeField]
	private float sendRange;

	// Token: 0x04001ACE RID: 6862
	[SerializeField]
	private PlayMakerFSM targetFSM;

	// Token: 0x04001ACF RID: 6863
	private List<string> handledEvents = new List<string>();
}
