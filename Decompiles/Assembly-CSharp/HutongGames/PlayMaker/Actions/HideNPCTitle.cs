using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C18 RID: 3096
	public class HideNPCTitle : FsmStateAction
	{
		// Token: 0x06005E52 RID: 24146 RVA: 0x001DB9F4 File Offset: 0x001D9BF4
		public override void Reset()
		{
			this.AreaTitleObject = null;
		}

		// Token: 0x06005E53 RID: 24147 RVA: 0x001DBA00 File Offset: 0x001D9C00
		public override void OnEnter()
		{
			try
			{
				this.AreaTitleObject.Value = ManagerSingleton<AreaTitle>.Instance.gameObject;
				ActionHelpers.GetGameObjectFsm(this.AreaTitleObject.Value, "Area Title Control").SendEventSafe("NPC TITLE DOWN");
			}
			finally
			{
				base.Finish();
			}
		}

		// Token: 0x04005AA1 RID: 23201
		public FsmGameObject AreaTitleObject;
	}
}
