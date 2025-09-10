using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001342 RID: 4930
	public class GetCostFromReference : FsmStateAction
	{
		// Token: 0x06007F6B RID: 32619 RVA: 0x0025B5E7 File Offset: 0x002597E7
		public override void Reset()
		{
			this.Reference = null;
			this.StoreValue = null;
		}

		// Token: 0x06007F6C RID: 32620 RVA: 0x0025B5F8 File Offset: 0x002597F8
		public override void OnEnter()
		{
			CostReference costReference = this.Reference.Value as CostReference;
			if (costReference != null)
			{
				this.StoreValue.Value = costReference.Value;
			}
			else
			{
				Debug.LogError("Cost reference not assigned!", base.Owner);
			}
			base.Finish();
		}

		// Token: 0x04007EF8 RID: 32504
		[ObjectType(typeof(CostReference))]
		public FsmObject Reference;

		// Token: 0x04007EF9 RID: 32505
		[UIHint(UIHint.Variable)]
		public FsmInt StoreValue;
	}
}
