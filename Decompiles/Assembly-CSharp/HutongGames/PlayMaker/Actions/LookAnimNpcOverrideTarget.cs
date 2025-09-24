using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001307 RID: 4871
	public class LookAnimNpcOverrideTarget : FSMUtility.GetComponentFsmStateAction<LookAnimNPC>
	{
		// Token: 0x06007E95 RID: 32405 RVA: 0x00259608 File Offset: 0x00257808
		public override void Reset()
		{
			base.Reset();
			this.OverrideTarget = null;
		}

		// Token: 0x06007E96 RID: 32406 RVA: 0x00259618 File Offset: 0x00257818
		protected override void DoAction(LookAnimNPC lookAnim)
		{
			GameObject value = this.OverrideTarget.Value;
			lookAnim.TargetOverride = (value ? value.transform : null);
		}

		// Token: 0x04007E51 RID: 32337
		public FsmGameObject OverrideTarget;
	}
}
