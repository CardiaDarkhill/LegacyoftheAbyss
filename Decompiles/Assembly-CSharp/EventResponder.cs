using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000334 RID: 820
public class EventResponder : MonoBehaviour
{
	// Token: 0x170002EC RID: 748
	// (get) Token: 0x06001CB8 RID: 7352 RVA: 0x00085D51 File Offset: 0x00083F51
	// (set) Token: 0x06001CB9 RID: 7353 RVA: 0x00085D5C File Offset: 0x00083F5C
	public EventBase Event
	{
		get
		{
			return this.eventReceiver;
		}
		set
		{
			if (this.eventReceiver)
			{
				this.eventReceiver.ReceivedEvent -= this.Respond;
			}
			this.eventReceiver = value;
			if (this.eventReceiver)
			{
				this.eventReceiver.ReceivedEvent += this.Respond;
			}
		}
	}

	// Token: 0x06001CBA RID: 7354 RVA: 0x00085DB8 File Offset: 0x00083FB8
	private void Awake()
	{
		this.Event = this.eventReceiver;
	}

	// Token: 0x06001CBB RID: 7355 RVA: 0x00085DC8 File Offset: 0x00083FC8
	public void Respond()
	{
		if (this.requireActive && !base.isActiveAndEnabled)
		{
			return;
		}
		this.CancelRespond();
		float randomValue = this.delay.GetRandomValue();
		if (randomValue > 0f)
		{
			this.delayedRoutine = base.StartCoroutine(this.RespondDelayed(randomValue));
			return;
		}
		this.DoResponses();
	}

	// Token: 0x06001CBC RID: 7356 RVA: 0x00085E1A File Offset: 0x0008401A
	public void CancelRespond()
	{
		if (this.delayedRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this.delayedRoutine);
		this.delayedRoutine = null;
	}

	// Token: 0x06001CBD RID: 7357 RVA: 0x00085E38 File Offset: 0x00084038
	private IEnumerator RespondDelayed(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		this.DoResponses();
		yield break;
	}

	// Token: 0x06001CBE RID: 7358 RVA: 0x00085E50 File Offset: 0x00084050
	private void DoResponses()
	{
		this.OnRespond.Invoke();
		if (!string.IsNullOrEmpty(this.sendEventRegister))
		{
			EventRegister.SendEvent(this.sendEventRegister, null);
		}
		foreach (EventResponder.PlaymakerEventTarget playmakerEventTarget in this.playmakerEventTargets)
		{
			if (!playmakerEventTarget.EventTarget || string.IsNullOrEmpty(playmakerEventTarget.EventName))
			{
				return;
			}
			playmakerEventTarget.EventTarget.SendEvent(playmakerEventTarget.EventName);
		}
	}

	// Token: 0x04001C1A RID: 7194
	[Header("Receive")]
	[SerializeField]
	[FormerlySerializedAs("eventRegister")]
	private EventBase eventReceiver;

	// Token: 0x04001C1B RID: 7195
	[SerializeField]
	private bool requireActive;

	// Token: 0x04001C1C RID: 7196
	[Header("Response")]
	[SerializeField]
	private MinMaxFloat delay;

	// Token: 0x04001C1D RID: 7197
	[SerializeField]
	private string sendEventRegister;

	// Token: 0x04001C1E RID: 7198
	[Space]
	public UnityEvent OnRespond;

	// Token: 0x04001C1F RID: 7199
	[Space]
	[SerializeField]
	private EventResponder.PlaymakerEventTarget[] playmakerEventTargets;

	// Token: 0x04001C20 RID: 7200
	private Coroutine delayedRoutine;

	// Token: 0x020015FE RID: 5630
	[Serializable]
	private struct PlaymakerEventTarget
	{
		// Token: 0x0600889F RID: 34975 RVA: 0x0027B10C File Offset: 0x0027930C
		private bool? IsEventValid(string eventName)
		{
			return this.EventTarget.IsEventValid(eventName, true);
		}

		// Token: 0x04008961 RID: 35169
		public PlayMakerFSM EventTarget;

		// Token: 0x04008962 RID: 35170
		[ModifiableProperty]
		[InspectorValidation("IsEventValid")]
		public string EventName;
	}
}
