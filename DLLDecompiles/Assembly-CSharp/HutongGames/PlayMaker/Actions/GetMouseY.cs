using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F0D RID: 3853
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Gets the Y Position of the mouse and stores it in a Float Variable.")]
	public class GetMouseY : FsmStateAction
	{
		// Token: 0x06006BB1 RID: 27569 RVA: 0x00217BD1 File Offset: 0x00215DD1
		public override void Reset()
		{
			this.storeResult = null;
			this.normalize = true;
			this.everyFrame = true;
		}

		// Token: 0x06006BB2 RID: 27570 RVA: 0x00217BE8 File Offset: 0x00215DE8
		public override void OnEnter()
		{
			this.DoGetMouseY();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006BB3 RID: 27571 RVA: 0x00217BFE File Offset: 0x00215DFE
		public override void OnUpdate()
		{
			this.DoGetMouseY();
		}

		// Token: 0x06006BB4 RID: 27572 RVA: 0x00217C08 File Offset: 0x00215E08
		private void DoGetMouseY()
		{
			if (this.storeResult != null)
			{
				float num = Input.mousePosition.y;
				if (this.normalize)
				{
					num /= (float)Screen.height;
				}
				this.storeResult.Value = num;
			}
		}

		// Token: 0x04006AFD RID: 27389
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store in a float variable.")]
		public FsmFloat storeResult;

		// Token: 0x04006AFE RID: 27390
		[Tooltip("Normalized coordinates are in the range 0 to 1 (0 = left, 1 = right). Otherwise the coordinate is in pixels. Normalized coordinates are useful for resolution independent functions.")]
		public bool normalize;

		// Token: 0x04006AFF RID: 27391
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
