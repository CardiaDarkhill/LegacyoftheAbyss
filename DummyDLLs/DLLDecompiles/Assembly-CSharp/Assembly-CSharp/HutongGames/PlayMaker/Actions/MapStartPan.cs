using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200136F RID: 4975
	[ActionCategory("Inventory")]
	public class MapStartPan : FsmStateAction
	{
		// Token: 0x06008037 RID: 32823 RVA: 0x0025DBB8 File Offset: 0x0025BDB8
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
		}

		// Token: 0x06008038 RID: 32824 RVA: 0x0025DBC8 File Offset: 0x0025BDC8
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				GameMap component = gameObject.GetComponent<GameMap>();
				if (component != null)
				{
					component.StartPan();
				}
			}
			base.Finish();
		}

		// Token: 0x04007F9D RID: 32669
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;
	}
}
