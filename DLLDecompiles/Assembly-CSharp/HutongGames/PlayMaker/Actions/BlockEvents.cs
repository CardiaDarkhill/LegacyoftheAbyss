using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200107B RID: 4219
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Block events while this action is active.")]
	public class BlockEvents : FsmStateAction
	{
		// Token: 0x06007305 RID: 29445 RVA: 0x00235ACE File Offset: 0x00233CCE
		public override void Reset()
		{
			this.condition = BlockEvents.Options.Timeout;
			this.floatParam = null;
			this.boolParam = null;
			this.eventParam = null;
			this.logBlockedEvents = false;
		}

		// Token: 0x06007306 RID: 29446 RVA: 0x00235AF8 File Offset: 0x00233CF8
		public override void Awake()
		{
			base.HandlesOnEvent = true;
		}

		// Token: 0x06007307 RID: 29447 RVA: 0x00235B04 File Offset: 0x00233D04
		public override void OnUpdate()
		{
			switch (this.condition)
			{
			case BlockEvents.Options.Timeout:
				if (this.boolParam.Value)
				{
					if (FsmTime.RealtimeSinceStartup - base.State.RealStartTime > this.floatParam.Value)
					{
						base.Finish();
						return;
					}
				}
				else if (base.State.StateTime > this.floatParam.Value)
				{
					base.Finish();
					return;
				}
				break;
			case BlockEvents.Options.WhileTrue:
			case BlockEvents.Options.WhileFalse:
			case BlockEvents.Options.UntilEvent:
				break;
			case BlockEvents.Options.UntilTrue:
				if (this.boolParam.Value)
				{
					base.Finish();
					return;
				}
				break;
			case BlockEvents.Options.UntilFalse:
				if (!this.boolParam.Value)
				{
					base.Finish();
					return;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06007308 RID: 29448 RVA: 0x00235BB8 File Offset: 0x00233DB8
		public override bool Event(FsmEvent fsmEvent)
		{
			if (this.firstTime)
			{
				if (!this.Validate())
				{
					base.Finish();
				}
				this.firstTime = false;
			}
			if (base.Finished)
			{
				return false;
			}
			if (Fsm.EventData.SentByState == base.State || fsmEvent == FsmEvent.Finished)
			{
				return false;
			}
			bool flag = this.DoBlockEvent(fsmEvent);
			if (flag && this.logBlockedEvents.Value)
			{
				ActionHelpers.DebugLog(base.Fsm, LogLevel.Info, "Blocked: " + fsmEvent.Name, true);
			}
			return flag;
		}

		// Token: 0x06007309 RID: 29449 RVA: 0x00235C3C File Offset: 0x00233E3C
		private bool Validate()
		{
			switch (this.condition)
			{
			case BlockEvents.Options.Timeout:
				return !this.floatParam.IsNone;
			case BlockEvents.Options.WhileTrue:
			case BlockEvents.Options.WhileFalse:
			case BlockEvents.Options.UntilTrue:
			case BlockEvents.Options.UntilFalse:
				return !this.boolParam.IsNone;
			case BlockEvents.Options.UntilEvent:
				return !string.IsNullOrEmpty(this.eventParam.Name);
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x0600730A RID: 29450 RVA: 0x00235CA8 File Offset: 0x00233EA8
		private bool DoBlockEvent(FsmEvent fsmEvent)
		{
			switch (this.condition)
			{
			case BlockEvents.Options.Timeout:
				if (this.boolParam.Value)
				{
					if (FsmTime.RealtimeSinceStartup - base.State.RealStartTime < this.floatParam.Value)
					{
						return true;
					}
				}
				else if (base.State.StateTime < this.floatParam.Value)
				{
					return true;
				}
				return false;
			case BlockEvents.Options.WhileTrue:
				return this.boolParam.Value;
			case BlockEvents.Options.WhileFalse:
				return !this.boolParam.Value;
			case BlockEvents.Options.UntilTrue:
				return !this.boolParam.Value;
			case BlockEvents.Options.UntilFalse:
				return this.boolParam.Value;
			case BlockEvents.Options.UntilEvent:
				if (fsmEvent.Name == this.eventParam.Name)
				{
					base.Finish();
					if (this.boolParam.Value)
					{
						return false;
					}
				}
				return true;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x0400730B RID: 29451
		[Tooltip("When to block events.")]
		public BlockEvents.Options condition;

		// Token: 0x0400730C RID: 29452
		[Tooltip("Context sensitive parameter. Depends on Condition.")]
		public FsmFloat floatParam;

		// Token: 0x0400730D RID: 29453
		[Tooltip("Context sensitive parameter. Depends on Condition.")]
		public FsmBool boolParam;

		// Token: 0x0400730E RID: 29454
		[EventNotSent]
		[Tooltip("Context sensitive parameter. Depends on Condition.")]
		public FsmEvent eventParam;

		// Token: 0x0400730F RID: 29455
		[ActionSection("Debug")]
		[Tooltip("Log any events blocked by this action. Helpful for debugging.")]
		public FsmBool logBlockedEvents;

		// Token: 0x04007310 RID: 29456
		private bool firstTime = true;

		// Token: 0x02001BC6 RID: 7110
		public enum Options
		{
			// Token: 0x04009EA6 RID: 40614
			Timeout,
			// Token: 0x04009EA7 RID: 40615
			WhileTrue,
			// Token: 0x04009EA8 RID: 40616
			WhileFalse,
			// Token: 0x04009EA9 RID: 40617
			UntilTrue,
			// Token: 0x04009EAA RID: 40618
			UntilFalse,
			// Token: 0x04009EAB RID: 40619
			UntilEvent
		}
	}
}
