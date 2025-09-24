using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013AC RID: 5036
	public class SetPersistentInt : FsmStateAction
	{
		// Token: 0x06008112 RID: 33042 RVA: 0x002603BF File Offset: 0x0025E5BF
		public override void Reset()
		{
			this.target = new FsmOwnerDefault();
			this.setValue = null;
		}

		// Token: 0x06008113 RID: 33043 RVA: 0x002603D4 File Offset: 0x0025E5D4
		public override void OnEnter()
		{
			GameObject gameObject = (this.target.OwnerOption == OwnerDefaultOption.UseOwner) ? base.Owner : this.target.GameObject.Value;
			if (gameObject != null)
			{
				gameObject.GetComponent<PersistentIntItem>().SetValueOverride(this.setValue.Value);
			}
			base.Finish();
		}

		// Token: 0x0400804E RID: 32846
		public FsmOwnerDefault target;

		// Token: 0x0400804F RID: 32847
		public FsmInt setValue;
	}
}
