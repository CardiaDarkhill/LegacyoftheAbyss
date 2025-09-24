using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C16 RID: 3094
	[ActionCategory("Hollow Knight")]
	public class DisplayBossTitle : FsmStateAction
	{
		// Token: 0x06005E4C RID: 24140 RVA: 0x001DB80D File Offset: 0x001D9A0D
		public override void Reset()
		{
			this.areaTitleObject = null;
			this.displayRight = null;
			this.bossTitle = null;
		}

		// Token: 0x06005E4D RID: 24141 RVA: 0x001DB824 File Offset: 0x001D9A24
		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(this.bossTitle.Value))
			{
				base.Finish();
				return;
			}
			GameObject gameObject = ManagerSingleton<AreaTitle>.Instance.gameObject;
			this.areaTitleObject.Value = gameObject;
			PlayMakerFSM gameObjectFsm = ActionHelpers.GetGameObjectFsm(gameObject, "Area Title Control");
			gameObject.SetActive(false);
			gameObjectFsm.FsmVariables.FindFsmBool("Visited").Value = true;
			gameObjectFsm.FsmVariables.FindFsmBool("Display Right").Value = this.displayRight.Value;
			gameObjectFsm.FsmVariables.FindFsmString("Area Event").Value = this.bossTitle.Value;
			gameObjectFsm.FsmVariables.FindFsmBool("NPC Title").Value = false;
			gameObject.SetActive(true);
			base.Finish();
		}

		// Token: 0x04005A9B RID: 23195
		public FsmGameObject areaTitleObject;

		// Token: 0x04005A9C RID: 23196
		public FsmBool displayRight;

		// Token: 0x04005A9D RID: 23197
		public FsmString bossTitle;
	}
}
