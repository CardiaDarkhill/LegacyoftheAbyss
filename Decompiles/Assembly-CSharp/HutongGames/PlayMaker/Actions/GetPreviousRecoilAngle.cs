using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012CA RID: 4810
	[ActionCategory("Hollow Knight")]
	public class GetPreviousRecoilAngle : FsmStateAction
	{
		// Token: 0x06007DA7 RID: 32167 RVA: 0x00256F6B File Offset: 0x0025516B
		public override void OnEnter()
		{
			this.DoGetAngle();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06007DA8 RID: 32168 RVA: 0x00256F81 File Offset: 0x00255181
		public override void OnUpdate()
		{
			this.DoGetAngle();
		}

		// Token: 0x06007DA9 RID: 32169 RVA: 0x00256F8C File Offset: 0x0025518C
		public void DoGetAngle()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject)
			{
				Recoil component = gameObject.GetComponent<Recoil>();
				this.storeRecoilAngle.Value = component.GetPreviousRecoilAngle();
			}
		}

		// Token: 0x04007D92 RID: 32146
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D93 RID: 32147
		[UIHint(UIHint.Variable)]
		public FsmFloat storeRecoilAngle;

		// Token: 0x04007D94 RID: 32148
		public bool everyframe;
	}
}
