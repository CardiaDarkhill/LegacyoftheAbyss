using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012C6 RID: 4806
	[ActionCategory("Hollow Knight")]
	public class SetRecoilFreeze : FsmStateAction
	{
		// Token: 0x06007D9C RID: 32156 RVA: 0x00256D34 File Offset: 0x00254F34
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject)
			{
				Recoil component = gameObject.GetComponent<Recoil>();
				if (component)
				{
					component.FreezeInPlace = this.freeze.Value;
					if (this.freeze.Value)
					{
						component.CancelRecoil();
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04007D87 RID: 32135
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;

		// Token: 0x04007D88 RID: 32136
		public FsmBool freeze;
	}
}
