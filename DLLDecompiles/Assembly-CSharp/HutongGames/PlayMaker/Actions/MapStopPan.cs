using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200136E RID: 4974
	[ActionCategory("Inventory")]
	public class MapStopPan : FsmStateAction
	{
		// Token: 0x06008034 RID: 32820 RVA: 0x0025DB4B File Offset: 0x0025BD4B
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
		}

		// Token: 0x06008035 RID: 32821 RVA: 0x0025DB58 File Offset: 0x0025BD58
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				GameMap component = gameObject.GetComponent<GameMap>();
				if (component != null)
				{
					component.StopPan();
				}
			}
			base.Finish();
		}

		// Token: 0x04007F9C RID: 32668
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;
	}
}
