using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E1B RID: 3611
	[ActionCategory(ActionCategory.Application)]
	[Tooltip("Gets the Width of the Screen in pixels.")]
	public class GetScreenWidth : FsmStateAction
	{
		// Token: 0x060067D9 RID: 26585 RVA: 0x0020B1EF File Offset: 0x002093EF
		public override void Reset()
		{
			this.storeScreenWidth = null;
			this.everyFrame = false;
		}

		// Token: 0x060067DA RID: 26586 RVA: 0x0020B1FF File Offset: 0x002093FF
		public override void OnEnter()
		{
			this.storeScreenWidth.Value = (float)Screen.width;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067DB RID: 26587 RVA: 0x0020B220 File Offset: 0x00209420
		public override void OnUpdate()
		{
			this.storeScreenWidth.Value = (float)Screen.width;
		}

		// Token: 0x0400670B RID: 26379
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the screen width in a Float Variable")]
		public FsmFloat storeScreenWidth;

		// Token: 0x0400670C RID: 26380
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
