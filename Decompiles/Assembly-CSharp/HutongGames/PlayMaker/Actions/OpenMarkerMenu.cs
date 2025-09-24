using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001378 RID: 4984
	[ActionCategory("Inventory")]
	public class OpenMarkerMenu : FsmStateAction
	{
		// Token: 0x06008053 RID: 32851 RVA: 0x0025E2F1 File Offset: 0x0025C4F1
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
		}

		// Token: 0x06008054 RID: 32852 RVA: 0x0025E300 File Offset: 0x0025C500
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				MapMarkerMenu component = gameObject.GetComponent<MapMarkerMenu>();
				if (component != null)
				{
					component.Open();
				}
			}
			base.Finish();
		}

		// Token: 0x04007FC2 RID: 32706
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;
	}
}
