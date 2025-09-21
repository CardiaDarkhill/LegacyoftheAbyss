using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E1C RID: 3612
	[ActionCategory(ActionCategory.Application)]
	[Tooltip("Sets the Screen Width and Height.")]
	public class SetScreenResolution : FsmStateAction
	{
		// Token: 0x060067DD RID: 26589 RVA: 0x0020B23B File Offset: 0x0020943B
		public override void Reset()
		{
			this.width = new FsmInt
			{
				Value = 800
			};
			this.height = new FsmInt
			{
				Value = 600
			};
			this.fullscreen = null;
		}

		// Token: 0x060067DE RID: 26590 RVA: 0x0020B270 File Offset: 0x00209470
		public override void OnEnter()
		{
			Screen.SetResolution(this.width.Value, this.height.Value, this.fullscreen.Value);
			base.Finish();
		}

		// Token: 0x0400670D RID: 26381
		[RequiredField]
		[Tooltip("Screen Width")]
		public FsmInt width;

		// Token: 0x0400670E RID: 26382
		[RequiredField]
		[Tooltip("Screen Height")]
		public FsmInt height;

		// Token: 0x0400670F RID: 26383
		[Tooltip("Show Fullscreen")]
		public FsmBool fullscreen;
	}
}
