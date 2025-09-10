using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001379 RID: 4985
	[ActionCategory("Inventory")]
	public class CloseMarkerMenu : FsmStateAction
	{
		// Token: 0x06008056 RID: 32854 RVA: 0x0025E360 File Offset: 0x0025C560
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
		}

		// Token: 0x06008057 RID: 32855 RVA: 0x0025E370 File Offset: 0x0025C570
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				MapMarkerMenu component = gameObject.GetComponent<MapMarkerMenu>();
				if (component != null)
				{
					component.Close();
				}
			}
			base.Finish();
		}

		// Token: 0x04007FC3 RID: 32707
		[UIHint(UIHint.Variable)]
		public FsmOwnerDefault target;
	}
}
