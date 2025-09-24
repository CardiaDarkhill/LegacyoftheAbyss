using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C17 RID: 3095
	[ActionCategory("Hollow Knight")]
	public class DisplayNPCTitle : FsmStateAction
	{
		// Token: 0x06005E4F RID: 24143 RVA: 0x001DB8F3 File Offset: 0x001D9AF3
		public override void Reset()
		{
			this.areaTitleObject = null;
			this.displayRight = null;
			this.npcTitle = null;
		}

		// Token: 0x06005E50 RID: 24144 RVA: 0x001DB90C File Offset: 0x001D9B0C
		public override void OnEnter()
		{
			try
			{
				this.areaTitleObject.Value = ManagerSingleton<AreaTitle>.Instance.gameObject;
				PlayMakerFSM gameObjectFsm = ActionHelpers.GetGameObjectFsm(this.areaTitleObject.Value, "Area Title Control");
				this.areaTitleObject.Value.SetActive(false);
				gameObjectFsm.FsmVariables.FindFsmBool("Visited").Value = true;
				gameObjectFsm.FsmVariables.FindFsmBool("NPC Title").Value = true;
				gameObjectFsm.FsmVariables.FindFsmBool("Display Right").Value = this.displayRight.Value;
				gameObjectFsm.FsmVariables.FindFsmString("Area Event").Value = this.npcTitle.Value;
				this.areaTitleObject.Value.SetActive(true);
			}
			finally
			{
				base.Finish();
			}
		}

		// Token: 0x04005A9E RID: 23198
		public FsmGameObject areaTitleObject;

		// Token: 0x04005A9F RID: 23199
		public FsmBool displayRight;

		// Token: 0x04005AA0 RID: 23200
		public FsmString npcTitle;
	}
}
