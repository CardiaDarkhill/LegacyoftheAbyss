using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E18 RID: 3608
	[ActionCategory(ActionCategory.Application)]
	[Tooltip("Quits the player application.")]
	public class ApplicationQuit : FsmStateAction
	{
		// Token: 0x060067CF RID: 26575 RVA: 0x0020B147 File Offset: 0x00209347
		public override void Reset()
		{
			this.exitCode = 0;
		}

		// Token: 0x060067D0 RID: 26576 RVA: 0x0020B155 File Offset: 0x00209355
		public override void OnEnter()
		{
			Application.Quit(this.exitCode.Value);
			base.Finish();
		}

		// Token: 0x04006707 RID: 26375
		[Tooltip("An optional exit code to return when the player application terminates on Windows, Mac and Linux. Defaults to 0.")]
		public FsmInt exitCode;
	}
}
