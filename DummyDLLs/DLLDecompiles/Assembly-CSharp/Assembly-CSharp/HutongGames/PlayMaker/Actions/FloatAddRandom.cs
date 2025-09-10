using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C48 RID: 3144
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Adds a value to a Float Variable.")]
	public class FloatAddRandom : FsmStateAction
	{
		// Token: 0x06005F5E RID: 24414 RVA: 0x001E4CB9 File Offset: 0x001E2EB9
		public override void Reset()
		{
			this.floatVariable = null;
			this.addMin = null;
			this.addMax = null;
		}

		// Token: 0x06005F5F RID: 24415 RVA: 0x001E4CD0 File Offset: 0x001E2ED0
		public override void OnEnter()
		{
			this.DoFloatAdd();
		}

		// Token: 0x06005F60 RID: 24416 RVA: 0x001E4CD8 File Offset: 0x001E2ED8
		private void DoFloatAdd()
		{
			this.floatVariable.Value += Random.Range(this.addMin.Value, this.addMax.Value);
			base.Finish();
		}

		// Token: 0x04005CBF RID: 23743
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Float variable to add to.")]
		public FsmFloat floatVariable;

		// Token: 0x04005CC0 RID: 23744
		[RequiredField]
		public FsmFloat addMin;

		// Token: 0x04005CC1 RID: 23745
		[RequiredField]
		public FsmFloat addMax;
	}
}
