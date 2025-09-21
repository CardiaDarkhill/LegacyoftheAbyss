using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F0C RID: 3852
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Gets the X Position of the mouse and stores it in a Float Variable.")]
	public class GetMouseX : FsmStateAction
	{
		// Token: 0x06006BAC RID: 27564 RVA: 0x00217B55 File Offset: 0x00215D55
		public override void Reset()
		{
			this.storeResult = null;
			this.normalize = true;
			this.everyFrame = true;
		}

		// Token: 0x06006BAD RID: 27565 RVA: 0x00217B6C File Offset: 0x00215D6C
		public override void OnEnter()
		{
			this.DoGetMouseX();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006BAE RID: 27566 RVA: 0x00217B82 File Offset: 0x00215D82
		public override void OnUpdate()
		{
			this.DoGetMouseX();
		}

		// Token: 0x06006BAF RID: 27567 RVA: 0x00217B8C File Offset: 0x00215D8C
		private void DoGetMouseX()
		{
			if (this.storeResult != null)
			{
				float num = Input.mousePosition.x;
				if (this.normalize)
				{
					num /= (float)Screen.width;
				}
				this.storeResult.Value = num;
			}
		}

		// Token: 0x04006AFA RID: 27386
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store in a float variable.")]
		public FsmFloat storeResult;

		// Token: 0x04006AFB RID: 27387
		[Tooltip("Normalized coordinates are in the range 0 to 1 (0 = left, 1 = right). Otherwise the coordinate is in pixels. Normalized coordinates are useful for resolution independent functions.")]
		public bool normalize;

		// Token: 0x04006AFC RID: 27388
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
