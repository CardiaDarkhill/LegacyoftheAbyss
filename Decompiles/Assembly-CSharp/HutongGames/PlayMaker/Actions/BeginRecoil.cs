using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012C3 RID: 4803
	[ActionCategory("Hollow Knight")]
	public class BeginRecoil : FsmStateAction
	{
		// Token: 0x06007D93 RID: 32147 RVA: 0x00256B81 File Offset: 0x00254D81
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.attackDirection = new FsmFloat();
			this.attackType = new FsmInt();
			this.attackMagnitude = new FsmFloat();
		}

		// Token: 0x06007D94 RID: 32148 RVA: 0x00256BB0 File Offset: 0x00254DB0
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				Recoil component = gameObject.GetComponent<Recoil>();
				if (component != null)
				{
					component.RecoilByDirection(DirectionUtils.GetCardinalDirection(this.attackDirection.Value), this.attackMagnitude.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04007D7F RID: 32127
		public FsmOwnerDefault target;

		// Token: 0x04007D80 RID: 32128
		public FsmFloat attackDirection;

		// Token: 0x04007D81 RID: 32129
		public FsmInt attackType;

		// Token: 0x04007D82 RID: 32130
		public FsmFloat attackMagnitude;
	}
}
