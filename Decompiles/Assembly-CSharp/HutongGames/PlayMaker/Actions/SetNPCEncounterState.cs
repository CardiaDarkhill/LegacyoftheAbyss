using System;
using GlobalEnums;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012FC RID: 4860
	public class SetNPCEncounterState : FsmStateAction
	{
		// Token: 0x06007E6F RID: 32367 RVA: 0x00258EE3 File Offset: 0x002570E3
		public override void Reset()
		{
			this.Target = null;
			this.State = null;
		}

		// Token: 0x06007E70 RID: 32368 RVA: 0x00258EF4 File Offset: 0x002570F4
		public override void OnEnter()
		{
			NPCEncounterStateController safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				safe.SetState((NPCEncounterState)this.State.Value);
			}
			base.Finish();
		}

		// Token: 0x04007E2A RID: 32298
		[RequiredField]
		[CheckForComponent(typeof(NPCEncounterStateController))]
		public FsmOwnerDefault Target;

		// Token: 0x04007E2B RID: 32299
		[ObjectType(typeof(NPCEncounterState))]
		public new FsmEnum State;
	}
}
