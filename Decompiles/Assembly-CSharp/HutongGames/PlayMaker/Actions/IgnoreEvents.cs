using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200109E RID: 4254
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Ignore specified events while this action is active.")]
	public class IgnoreEvents : FsmStateAction
	{
		// Token: 0x0600739B RID: 29595 RVA: 0x00237ABD File Offset: 0x00235CBD
		public override void Reset()
		{
			this.eventTypes = new IgnoreEvents.EventType[0];
			this.events = new FsmString[0];
			this.logIgnoredEvents = false;
		}

		// Token: 0x0600739C RID: 29596 RVA: 0x00237AE3 File Offset: 0x00235CE3
		public override void Awake()
		{
			base.HandlesOnEvent = true;
			base.BlocksFinish = false;
		}

		// Token: 0x0600739D RID: 29597 RVA: 0x00237AF3 File Offset: 0x00235CF3
		public override bool Event(FsmEvent fsmEvent)
		{
			bool flag = this.DoIgnoreEvent(fsmEvent);
			if (flag && this.logIgnoredEvents.Value)
			{
				ActionHelpers.DebugLog(base.Fsm, LogLevel.Info, "Ignored: " + fsmEvent.Name, true);
			}
			return flag;
		}

		// Token: 0x0600739E RID: 29598 RVA: 0x00237B2C File Offset: 0x00235D2C
		private bool DoIgnoreEvent(FsmEvent fsmEvent)
		{
			if (fsmEvent == null)
			{
				return false;
			}
			IgnoreEvents.EventType[] array = this.eventTypes;
			for (int i = 0; i < array.Length; i++)
			{
				switch (array[i])
				{
				case IgnoreEvents.EventType.mouse:
					if (fsmEvent.IsMouseEvent)
					{
						return true;
					}
					break;
				case IgnoreEvents.EventType.application:
					if (fsmEvent.IsApplicationEvent)
					{
						return true;
					}
					break;
				case IgnoreEvents.EventType.collision:
					if (fsmEvent.IsCollisionEvent)
					{
						return true;
					}
					break;
				case IgnoreEvents.EventType.collision2d:
					if (fsmEvent.IsCollision2DEvent)
					{
						return true;
					}
					break;
				case IgnoreEvents.EventType.trigger:
					if (fsmEvent.IsTriggerEvent)
					{
						return true;
					}
					break;
				case IgnoreEvents.EventType.trigger2d:
					if (fsmEvent.IsTrigger2DEvent)
					{
						return true;
					}
					break;
				case IgnoreEvents.EventType.UI:
					if (fsmEvent.IsUIEvent)
					{
						return true;
					}
					break;
				case IgnoreEvents.EventType.anyUnityEvent:
					if (fsmEvent.IsUnityEvent)
					{
						return true;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
			string name = fsmEvent.Name;
			for (int j = 0; j < this.events.Length; j++)
			{
				if (this.events[j].Value == name)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x040073CA RID: 29642
		[Tooltip("Type of events to ignore.")]
		public IgnoreEvents.EventType[] eventTypes;

		// Token: 0x040073CB RID: 29643
		[Tooltip("Event names to ignore.")]
		[UIHint(UIHint.FsmEvent)]
		public FsmString[] events;

		// Token: 0x040073CC RID: 29644
		[ActionSection("Debug")]
		[Tooltip("Log any events blocked by this action. Helpful for debugging.")]
		public FsmBool logIgnoredEvents;

		// Token: 0x02001BC7 RID: 7111
		[Serializable]
		public enum EventType
		{
			// Token: 0x04009EAD RID: 40621
			mouse,
			// Token: 0x04009EAE RID: 40622
			application,
			// Token: 0x04009EAF RID: 40623
			collision,
			// Token: 0x04009EB0 RID: 40624
			collision2d,
			// Token: 0x04009EB1 RID: 40625
			trigger,
			// Token: 0x04009EB2 RID: 40626
			trigger2d,
			// Token: 0x04009EB3 RID: 40627
			UI,
			// Token: 0x04009EB4 RID: 40628
			anyUnityEvent
		}
	}
}
