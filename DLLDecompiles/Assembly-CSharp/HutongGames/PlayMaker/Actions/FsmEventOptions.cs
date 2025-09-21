using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E9B RID: 3739
	[ActionCategory(ActionCategory.StateMachine)]
	[Tooltip("Sets how subsequent events sent in this state are handled.")]
	public class FsmEventOptions : FsmStateAction
	{
		// Token: 0x06006A19 RID: 27161 RVA: 0x00212C01 File Offset: 0x00210E01
		public override void Reset()
		{
			this.sendToFsmComponent = null;
			this.sendToGameObject = null;
			this.fsmName = "";
			this.sendToChildren = false;
			this.broadcastToAll = false;
		}

		// Token: 0x06006A1A RID: 27162 RVA: 0x00212C39 File Offset: 0x00210E39
		public override void OnUpdate()
		{
		}

		// Token: 0x04006970 RID: 26992
		public PlayMakerFSM sendToFsmComponent;

		// Token: 0x04006971 RID: 26993
		public FsmGameObject sendToGameObject;

		// Token: 0x04006972 RID: 26994
		public FsmString fsmName;

		// Token: 0x04006973 RID: 26995
		public FsmBool sendToChildren;

		// Token: 0x04006974 RID: 26996
		public FsmBool broadcastToAll;
	}
}
