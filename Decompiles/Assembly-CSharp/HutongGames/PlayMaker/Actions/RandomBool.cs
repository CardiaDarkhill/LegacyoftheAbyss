using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F85 RID: 3973
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets a Bool Variable to True or False randomly.")]
	public class RandomBool : FsmStateAction
	{
		// Token: 0x06006DF5 RID: 28149 RVA: 0x00221BDA File Offset: 0x0021FDDA
		public override void Reset()
		{
			this.storeResult = null;
		}

		// Token: 0x06006DF6 RID: 28150 RVA: 0x00221BE3 File Offset: 0x0021FDE3
		public override void OnEnter()
		{
			this.storeResult.Value = (Random.Range(0, 100) < 50);
			base.Finish();
		}

		// Token: 0x04006DA7 RID: 28071
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a bool variable. Hint: Use a {{Bool Test}} action to branch based on this \"coin toss\"")]
		public FsmBool storeResult;
	}
}
