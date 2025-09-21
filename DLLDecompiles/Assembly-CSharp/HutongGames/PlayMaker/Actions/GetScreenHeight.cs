using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E1A RID: 3610
	[ActionCategory(ActionCategory.Application)]
	[Tooltip("Gets the Height of the Screen in pixels.")]
	public class GetScreenHeight : FsmStateAction
	{
		// Token: 0x060067D5 RID: 26581 RVA: 0x0020B1A3 File Offset: 0x002093A3
		public override void Reset()
		{
			this.storeScreenHeight = null;
			this.everyFrame = false;
		}

		// Token: 0x060067D6 RID: 26582 RVA: 0x0020B1B3 File Offset: 0x002093B3
		public override void OnEnter()
		{
			this.storeScreenHeight.Value = (float)Screen.height;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067D7 RID: 26583 RVA: 0x0020B1D4 File Offset: 0x002093D4
		public override void OnUpdate()
		{
			this.storeScreenHeight.Value = (float)Screen.height;
		}

		// Token: 0x04006709 RID: 26377
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen height in a Float Variable")]
		public FsmFloat storeScreenHeight;

		// Token: 0x0400670A RID: 26378
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
