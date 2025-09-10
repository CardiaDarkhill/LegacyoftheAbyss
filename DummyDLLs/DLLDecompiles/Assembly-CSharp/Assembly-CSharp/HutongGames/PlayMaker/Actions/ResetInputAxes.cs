using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F12 RID: 3858
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Resets all Input. After ResetInputAxes all axes return to 0 and all buttons return to 0 for one frame")]
	public class ResetInputAxes : FsmStateAction
	{
		// Token: 0x06006BD2 RID: 27602 RVA: 0x0021852B File Offset: 0x0021672B
		public override void Reset()
		{
		}

		// Token: 0x06006BD3 RID: 27603 RVA: 0x0021852D File Offset: 0x0021672D
		public override void OnEnter()
		{
			Input.ResetInputAxes();
			base.Finish();
		}
	}
}
