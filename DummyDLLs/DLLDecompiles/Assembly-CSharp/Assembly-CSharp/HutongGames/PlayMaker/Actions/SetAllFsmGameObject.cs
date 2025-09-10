using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001065 RID: 4197
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Set the value of a Game Object Variable in another All FSM. Accept null reference")]
	public class SetAllFsmGameObject : FsmStateAction
	{
		// Token: 0x060072B0 RID: 29360 RVA: 0x00234B53 File Offset: 0x00232D53
		public override void Reset()
		{
		}

		// Token: 0x060072B1 RID: 29361 RVA: 0x00234B55 File Offset: 0x00232D55
		public override void OnEnter()
		{
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060072B2 RID: 29362 RVA: 0x00234B65 File Offset: 0x00232D65
		private void DoSetFsmGameObject()
		{
		}

		// Token: 0x040072B4 RID: 29364
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040072B5 RID: 29365
		public bool everyFrame;
	}
}
