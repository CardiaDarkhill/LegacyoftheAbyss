using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001113 RID: 4371
	public abstract class EventTriggerActionBase : ComponentAction<EventTrigger>
	{
		// Token: 0x0600761E RID: 30238 RVA: 0x00240F1A File Offset: 0x0023F11A
		public override void Reset()
		{
			this.gameObject = null;
			this.eventTarget = null;
		}

		// Token: 0x0600761F RID: 30239 RVA: 0x00240F2C File Offset: 0x0023F12C
		protected void Init(EventTriggerType eventTriggerType, UnityAction<BaseEventData> call)
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCacheAddComponent(ownerDefaultTarget))
			{
				this.trigger = this.cachedComponent;
				if (this.entry == null)
				{
					this.entry = new EventTrigger.Entry();
				}
				this.entry.eventID = eventTriggerType;
				this.entry.callback.AddListener(call);
				this.trigger.triggers.Add(this.entry);
			}
		}

		// Token: 0x06007620 RID: 30240 RVA: 0x00240FA8 File Offset: 0x0023F1A8
		public override void OnExit()
		{
			if (this.entry != null && this.entry.callback != null)
			{
				this.entry.callback.RemoveAllListeners();
			}
			if (this.trigger != null && this.trigger.triggers != null)
			{
				this.trigger.triggers.Remove(this.entry);
			}
		}

		// Token: 0x04007685 RID: 30341
		[DisplayOrder(0)]
		[RequiredField]
		[Tooltip("The GameObject with the UI component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007686 RID: 30342
		[DisplayOrder(1)]
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		// Token: 0x04007687 RID: 30343
		protected EventTrigger trigger;

		// Token: 0x04007688 RID: 30344
		protected EventTrigger.Entry entry;
	}
}
