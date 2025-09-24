using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012C8 RID: 4808
	[ActionCategory("Hollow Knight")]
	public class CancelRecoil : FsmStateAction
	{
		// Token: 0x06007DA1 RID: 32161 RVA: 0x00256E94 File Offset: 0x00255094
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject)
			{
				Recoil component = gameObject.GetComponent<Recoil>();
				if (component)
				{
					component.CancelRecoil();
				}
			}
			base.Finish();
		}

		// Token: 0x04007D8E RID: 32142
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;
	}
}
