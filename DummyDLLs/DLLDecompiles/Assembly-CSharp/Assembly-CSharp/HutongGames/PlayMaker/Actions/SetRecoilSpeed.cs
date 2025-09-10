using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012C5 RID: 4805
	[ActionCategory("Hollow Knight")]
	public class SetRecoilSpeed : FsmStateAction
	{
		// Token: 0x06007D99 RID: 32153 RVA: 0x00256CAF File Offset: 0x00254EAF
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.newRecoilSpeed = new FsmFloat();
		}

		// Token: 0x06007D9A RID: 32154 RVA: 0x00256CC8 File Offset: 0x00254EC8
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				Recoil component = gameObject.GetComponent<Recoil>();
				if (component != null)
				{
					component.SetRecoilSpeed(this.newRecoilSpeed.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007D85 RID: 32133
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D86 RID: 32134
		public FsmFloat newRecoilSpeed;
	}
}
