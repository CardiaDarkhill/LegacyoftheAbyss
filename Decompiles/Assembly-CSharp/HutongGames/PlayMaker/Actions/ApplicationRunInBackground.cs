using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E19 RID: 3609
	[ActionCategory(ActionCategory.Application)]
	[Tooltip("Sets if the Application should play in the background. Useful for servers or testing network games on one machine.")]
	public class ApplicationRunInBackground : FsmStateAction
	{
		// Token: 0x060067D2 RID: 26578 RVA: 0x0020B175 File Offset: 0x00209375
		public override void Reset()
		{
			this.runInBackground = true;
		}

		// Token: 0x060067D3 RID: 26579 RVA: 0x0020B183 File Offset: 0x00209383
		public override void OnEnter()
		{
			Application.runInBackground = this.runInBackground.Value;
			base.Finish();
		}

		// Token: 0x04006708 RID: 26376
		[Tooltip("Should the Application play in the background.")]
		public FsmBool runInBackground;
	}
}
