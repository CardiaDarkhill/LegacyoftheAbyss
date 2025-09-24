using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001308 RID: 4872
	public class LookAnimNpcSetTargetEntered : FSMUtility.GetComponentFsmStateAction<LookAnimNPC>
	{
		// Token: 0x06007E98 RID: 32408 RVA: 0x00259650 File Offset: 0x00257850
		public override void Reset()
		{
			base.Reset();
			this.LookTarget = null;
		}

		// Token: 0x06007E99 RID: 32409 RVA: 0x00259660 File Offset: 0x00257860
		protected override void DoAction(LookAnimNPC lookAnim)
		{
			GameObject value = this.LookTarget.Value;
			lookAnim.TargetEntered(value);
		}

		// Token: 0x04007E52 RID: 32338
		public FsmGameObject LookTarget;
	}
}
