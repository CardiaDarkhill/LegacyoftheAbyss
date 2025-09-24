using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200124D RID: 4685
	public class CheckHeroClamberLedge : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x06007BD9 RID: 31705 RVA: 0x002509CE File Offset: 0x0024EBCE
		public override void Reset()
		{
			base.Reset();
			this.StoreY = null;
			this.StoreCollider = null;
		}

		// Token: 0x17000C19 RID: 3097
		// (get) Token: 0x06007BDA RID: 31706 RVA: 0x002509E4 File Offset: 0x0024EBE4
		public override bool IsTrue
		{
			get
			{
				float value;
				Collider2D collider2D;
				if (HeroController.instance.CheckClamberLedge(out value, out collider2D))
				{
					this.StoreY.Value = value;
					this.StoreCollider.Value = (collider2D ? collider2D.gameObject : null);
					return true;
				}
				this.StoreY.Value = 0f;
				this.StoreCollider.Value = null;
				return false;
			}
		}

		// Token: 0x04007C07 RID: 31751
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreY;

		// Token: 0x04007C08 RID: 31752
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreCollider;
	}
}
