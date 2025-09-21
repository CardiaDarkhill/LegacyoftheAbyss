using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013AB RID: 5035
	public class GetPersistentInt : FsmStateAction
	{
		// Token: 0x0600810F RID: 33039 RVA: 0x00260343 File Offset: 0x0025E543
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.storeValue = null;
		}

		// Token: 0x06008110 RID: 33040 RVA: 0x00260358 File Offset: 0x0025E558
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				PersistentIntItem component = gameObject.GetComponent<PersistentIntItem>();
				this.storeValue.Value = component.ItemData.Value;
			}
			base.Finish();
		}

		// Token: 0x0400804C RID: 32844
		public FsmOwnerDefault target;

		// Token: 0x0400804D RID: 32845
		[UIHint(UIHint.Variable)]
		public FsmInt storeValue;
	}
}
