using System;
using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012FD RID: 4861
	public class GetNPCEncounterState : FsmStateAction
	{
		// Token: 0x06007E72 RID: 32370 RVA: 0x00258F3B File Offset: 0x0025713B
		public override void Reset()
		{
			this.Target = null;
			this.State = null;
			this.responses = new GetNPCEncounterState.Response[0];
		}

		// Token: 0x06007E73 RID: 32371 RVA: 0x00258F58 File Offset: 0x00257158
		public override void OnEnter()
		{
			NPCEncounterStateController safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				NPCEncounterState currentState = safe.GetCurrentState();
				FsmEnum state = this.State;
				if (state != null && !state.IsNone)
				{
					this.State.Value = currentState;
				}
				foreach (GetNPCEncounterState.Response response in this.responses)
				{
					bool flag = response.state.Equals(currentState);
					FsmBool storeValue = response.storeValue;
					if (storeValue != null && !storeValue.IsNone)
					{
						response.storeValue.Value = flag;
					}
					if (flag)
					{
						base.Fsm.Event(response.trueEvent);
					}
					else
					{
						base.Fsm.Event(response.falseEvent);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007E2C RID: 32300
		[RequiredField]
		[CheckForComponent(typeof(NPCEncounterStateController))]
		public FsmOwnerDefault Target;

		// Token: 0x04007E2D RID: 32301
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(NPCEncounterState))]
		public new FsmEnum State;

		// Token: 0x04007E2E RID: 32302
		[Tooltip("Define which event to send for each possible state.")]
		public GetNPCEncounterState.Response[] responses;

		// Token: 0x02001BF1 RID: 7153
		[Serializable]
		public class Response
		{
			// Token: 0x04009FA4 RID: 40868
			public NPCEncounterState state;

			// Token: 0x04009FA5 RID: 40869
			public FsmEvent trueEvent;

			// Token: 0x04009FA6 RID: 40870
			public FsmEvent falseEvent;

			// Token: 0x04009FA7 RID: 40871
			[UIHint(UIHint.Variable)]
			public FsmBool storeValue;
		}
	}
}
