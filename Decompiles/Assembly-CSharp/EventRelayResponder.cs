using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002BA RID: 698
public sealed class EventRelayResponder : MonoBehaviour
{
	// Token: 0x060018B3 RID: 6323 RVA: 0x00071300 File Offset: 0x0006F500
	private void Awake()
	{
		this.responses.RemoveAll((EventRelayResponder.EventResponse o) => o.response == null);
		foreach (EventRelayResponder.EventResponse eventResponse in this.responses)
		{
			this.eventResponses.Add(eventResponse.eventName, eventResponse.response);
		}
	}

	// Token: 0x060018B4 RID: 6324 RVA: 0x00071390 File Offset: 0x0006F590
	public void ReceiveEvent(string eventName)
	{
		UnityEvent unityEvent;
		if (this.eventResponses.TryGetValue(eventName, out unityEvent) && unityEvent != null)
		{
			unityEvent.Invoke();
		}
	}

	// Token: 0x040017A6 RID: 6054
	[SerializeField]
	private List<EventRelayResponder.EventResponse> responses = new List<EventRelayResponder.EventResponse>();

	// Token: 0x040017A7 RID: 6055
	[SerializeField]
	private bool sendToChildren;

	// Token: 0x040017A8 RID: 6056
	private List<EventRelayResponder> childRepsonders = new List<EventRelayResponder>();

	// Token: 0x040017A9 RID: 6057
	private Dictionary<string, UnityEvent> eventResponses = new Dictionary<string, UnityEvent>();

	// Token: 0x020015A2 RID: 5538
	[Serializable]
	public class EventResponse
	{
		// Token: 0x04008815 RID: 34837
		public string eventName;

		// Token: 0x04008816 RID: 34838
		public UnityEvent response;
	}
}
