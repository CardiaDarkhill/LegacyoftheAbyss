using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200118D RID: 4493
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Linearly interpolates between 2 vectors.")]
	public class Vector2Lerp : FsmStateAction
	{
		// Token: 0x0600785F RID: 30815 RVA: 0x0024798D File Offset: 0x00245B8D
		public override void Reset()
		{
			this.fromVector = new FsmVector2
			{
				UseVariable = true
			};
			this.toVector = new FsmVector2
			{
				UseVariable = true
			};
			this.storeResult = null;
			this.everyFrame = true;
		}

		// Token: 0x06007860 RID: 30816 RVA: 0x002479C1 File Offset: 0x00245BC1
		public override void OnEnter()
		{
			this.DoVector2Lerp();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007861 RID: 30817 RVA: 0x002479D7 File Offset: 0x00245BD7
		public override void OnUpdate()
		{
			this.DoVector2Lerp();
		}

		// Token: 0x06007862 RID: 30818 RVA: 0x002479DF File Offset: 0x00245BDF
		private void DoVector2Lerp()
		{
			this.storeResult.Value = Vector2.Lerp(this.fromVector.Value, this.toVector.Value, this.amount.Value);
		}

		// Token: 0x040078D1 RID: 30929
		[RequiredField]
		[Tooltip("First Vector.")]
		public FsmVector2 fromVector;

		// Token: 0x040078D2 RID: 30930
		[RequiredField]
		[Tooltip("Second Vector.")]
		public FsmVector2 toVector;

		// Token: 0x040078D3 RID: 30931
		[RequiredField]
		[Tooltip("Interpolate between From Vector and ToVector by this amount. Value is clamped to 0-1 range. 0 = From Vector; 1 = To Vector; 0.5 = half way between.")]
		public FsmFloat amount;

		// Token: 0x040078D4 RID: 30932
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in this vector variable.")]
		public FsmVector2 storeResult;

		// Token: 0x040078D5 RID: 30933
		[Tooltip("Repeat every frame. Useful if any of the values are changing.")]
		public bool everyFrame;
	}
}
