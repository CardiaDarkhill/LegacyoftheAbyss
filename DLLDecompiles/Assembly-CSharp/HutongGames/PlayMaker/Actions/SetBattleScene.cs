using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012A9 RID: 4777
	[ActionCategory("Hollow Knight")]
	public class SetBattleScene : FsmStateAction
	{
		// Token: 0x06007D3E RID: 32062 RVA: 0x00255C39 File Offset: 0x00253E39
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.battleScene = new FsmGameObject();
		}

		// Token: 0x06007D3F RID: 32063 RVA: 0x00255C54 File Offset: 0x00253E54
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				HealthManager component = gameObject.GetComponent<HealthManager>();
				if (component != null)
				{
					component.SetBattleScene(this.battleScene.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007D3D RID: 32061
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D3E RID: 32062
		[UIHint(UIHint.Variable)]
		public FsmGameObject battleScene;
	}
}
