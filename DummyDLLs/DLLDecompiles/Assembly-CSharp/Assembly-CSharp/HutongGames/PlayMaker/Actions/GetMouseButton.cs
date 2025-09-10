using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F09 RID: 3849
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Gets the pressed state of the specified Mouse Button and stores it in a Bool Variable. See Unity Input Manager doc.")]
	public class GetMouseButton : FsmStateAction
	{
		// Token: 0x06006B9D RID: 27549 RVA: 0x00217A11 File Offset: 0x00215C11
		public override void Reset()
		{
			this.button = MouseButton.Left;
			this.storeResult = null;
			this.everyFrame = true;
		}

		// Token: 0x06006B9E RID: 27550 RVA: 0x00217A28 File Offset: 0x00215C28
		public override void OnEnter()
		{
			this.DoGetMouseButton();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B9F RID: 27551 RVA: 0x00217A3E File Offset: 0x00215C3E
		public override void OnUpdate()
		{
			this.DoGetMouseButton();
		}

		// Token: 0x06006BA0 RID: 27552 RVA: 0x00217A46 File Offset: 0x00215C46
		private void DoGetMouseButton()
		{
			this.storeResult.Value = Input.GetMouseButton((int)this.button);
		}

		// Token: 0x04006AEF RID: 27375
		[RequiredField]
		[Tooltip("The mouse button to test.")]
		public MouseButton button;

		// Token: 0x04006AF0 RID: 27376
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the pressed state in a Bool Variable.")]
		public FsmBool storeResult;

		// Token: 0x04006AF1 RID: 27377
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
