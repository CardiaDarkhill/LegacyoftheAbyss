using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012C4 RID: 4804
	[ActionCategory("Hollow Knight")]
	public class GetRecoilSpeed : FsmStateAction
	{
		// Token: 0x06007D96 RID: 32150 RVA: 0x00256C2B File Offset: 0x00254E2B
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.storeRecoilSpeed = new FsmFloat();
		}

		// Token: 0x06007D97 RID: 32151 RVA: 0x00256C44 File Offset: 0x00254E44
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				Recoil component = gameObject.GetComponent<Recoil>();
				if (component != null)
				{
					this.storeRecoilSpeed.Value = component.RecoilSpeedBase;
				}
			}
			base.Finish();
		}

		// Token: 0x04007D83 RID: 32131
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D84 RID: 32132
		public FsmFloat storeRecoilSpeed;
	}
}
