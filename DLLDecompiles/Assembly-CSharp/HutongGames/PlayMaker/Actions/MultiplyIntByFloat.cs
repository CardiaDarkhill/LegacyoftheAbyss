using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F97 RID: 3991
	[ActionCategory(ActionCategory.Math)]
	public class MultiplyIntByFloat : FsmStateAction
	{
		// Token: 0x06006E41 RID: 28225 RVA: 0x002228F3 File Offset: 0x00220AF3
		public override void Reset()
		{
			this.integer = null;
			this.multiplyFloat = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006E42 RID: 28226 RVA: 0x00222911 File Offset: 0x00220B11
		public override void OnEnter()
		{
			this.DoMultiply();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E43 RID: 28227 RVA: 0x00222927 File Offset: 0x00220B27
		public override void OnUpdate()
		{
			this.DoMultiply();
		}

		// Token: 0x06006E44 RID: 28228 RVA: 0x00222930 File Offset: 0x00220B30
		private void DoMultiply()
		{
			if (this.forceRoundUp)
			{
				this.storeResult.Value = (int)Mathf.Ceil((float)this.integer.Value * this.multiplyFloat.Value);
				return;
			}
			this.storeResult.Value = (int)((float)this.integer.Value * this.multiplyFloat.Value);
		}

		// Token: 0x04006DE7 RID: 28135
		[RequiredField]
		public FsmInt integer;

		// Token: 0x04006DE8 RID: 28136
		[RequiredField]
		public FsmFloat multiplyFloat;

		// Token: 0x04006DE9 RID: 28137
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt storeResult;

		// Token: 0x04006DEA RID: 28138
		public bool everyFrame;

		// Token: 0x04006DEB RID: 28139
		public bool forceRoundUp;
	}
}
