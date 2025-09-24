using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012C9 RID: 4809
	[ActionCategory("Hollow Knight")]
	public class GetIsRecoiling : FsmStateAction
	{
		// Token: 0x06007DA3 RID: 32163 RVA: 0x00256EF2 File Offset: 0x002550F2
		public override void OnEnter()
		{
			this.DoGetRecoiling();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06007DA4 RID: 32164 RVA: 0x00256F08 File Offset: 0x00255108
		public override void OnUpdate()
		{
			this.DoGetRecoiling();
		}

		// Token: 0x06007DA5 RID: 32165 RVA: 0x00256F10 File Offset: 0x00255110
		public void DoGetRecoiling()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject)
			{
				Recoil component = gameObject.GetComponent<Recoil>();
				this.storeIsRecoiling.Value = component.GetIsRecoiling();
			}
		}

		// Token: 0x04007D8F RID: 32143
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D90 RID: 32144
		[UIHint(UIHint.Variable)]
		public FsmBool storeIsRecoiling;

		// Token: 0x04007D91 RID: 32145
		public bool everyframe;
	}
}
