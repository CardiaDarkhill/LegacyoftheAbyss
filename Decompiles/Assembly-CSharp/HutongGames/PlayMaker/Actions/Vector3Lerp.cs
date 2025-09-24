using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011A5 RID: 4517
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Linearly interpolates between 2 vectors.")]
	public class Vector3Lerp : FsmStateAction
	{
		// Token: 0x060078CA RID: 30922 RVA: 0x00248C7D File Offset: 0x00246E7D
		public override void Reset()
		{
			this.fromVector = new FsmVector3
			{
				UseVariable = true
			};
			this.toVector = new FsmVector3
			{
				UseVariable = true
			};
			this.storeResult = null;
			this.everyFrame = true;
		}

		// Token: 0x060078CB RID: 30923 RVA: 0x00248CB1 File Offset: 0x00246EB1
		public override void OnEnter()
		{
			this.DoVector3Lerp();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078CC RID: 30924 RVA: 0x00248CC7 File Offset: 0x00246EC7
		public override void OnUpdate()
		{
			this.DoVector3Lerp();
		}

		// Token: 0x060078CD RID: 30925 RVA: 0x00248CCF File Offset: 0x00246ECF
		private void DoVector3Lerp()
		{
			this.storeResult.Value = Vector3.Lerp(this.fromVector.Value, this.toVector.Value, this.amount.Value);
		}

		// Token: 0x0400792E RID: 31022
		[RequiredField]
		[Tooltip("First Vector.")]
		public FsmVector3 fromVector;

		// Token: 0x0400792F RID: 31023
		[RequiredField]
		[Tooltip("Second Vector.")]
		public FsmVector3 toVector;

		// Token: 0x04007930 RID: 31024
		[RequiredField]
		[Tooltip("Interpolate between From Vector and ToVector by this amount. Value is clamped to 0-1 range. 0 = From Vector; 1 = To Vector; 0.5 = half way between.")]
		public FsmFloat amount;

		// Token: 0x04007931 RID: 31025
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in this vector variable.")]
		public FsmVector3 storeResult;

		// Token: 0x04007932 RID: 31026
		[Tooltip("Repeat every frame. Useful if any of the values are changing.")]
		public bool everyFrame;
	}
}
