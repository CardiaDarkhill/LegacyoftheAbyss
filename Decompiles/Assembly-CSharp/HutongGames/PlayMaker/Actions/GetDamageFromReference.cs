using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001346 RID: 4934
	public class GetDamageFromReference : FsmStateAction
	{
		// Token: 0x06007F79 RID: 32633 RVA: 0x0025B7FE File Offset: 0x002599FE
		public override void Reset()
		{
			this.Reference = null;
			this.StoreValue = null;
		}

		// Token: 0x06007F7A RID: 32634 RVA: 0x0025B810 File Offset: 0x00259A10
		public override void OnEnter()
		{
			DamageReference damageReference = this.Reference.Value as DamageReference;
			if (damageReference != null)
			{
				this.StoreValue.Value = damageReference.Value;
			}
			else
			{
				Debug.LogError("Damage reference not assigned!", base.Owner);
			}
			base.Finish();
		}

		// Token: 0x04007F00 RID: 32512
		[ObjectType(typeof(DamageReference))]
		public FsmObject Reference;

		// Token: 0x04007F01 RID: 32513
		[UIHint(UIHint.Variable)]
		public FsmInt StoreValue;
	}
}
