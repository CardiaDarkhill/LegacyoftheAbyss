using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012A6 RID: 4774
	public class SetEnemyDeathEffectsProfile : FsmStateAction
	{
		// Token: 0x06007D33 RID: 32051 RVA: 0x00255A9B File Offset: 0x00253C9B
		public override void Reset()
		{
			this.Target = null;
			this.Profile = null;
		}

		// Token: 0x06007D34 RID: 32052 RVA: 0x00255AAC File Offset: 0x00253CAC
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				EnemyDeathEffectsRegular component = safe.GetComponent<EnemyDeathEffectsRegular>();
				if (component)
				{
					component.Profile = (this.Profile.Value as EnemyDeathEffectsProfile);
				}
			}
			base.Finish();
		}

		// Token: 0x04007D34 RID: 32052
		public FsmOwnerDefault Target;

		// Token: 0x04007D35 RID: 32053
		[ObjectType(typeof(EnemyDeathEffectsProfile))]
		public FsmObject Profile;
	}
}
