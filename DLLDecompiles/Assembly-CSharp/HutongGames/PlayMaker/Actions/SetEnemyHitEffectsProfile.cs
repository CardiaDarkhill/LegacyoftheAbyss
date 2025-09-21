using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012A7 RID: 4775
	public class SetEnemyHitEffectsProfile : FsmStateAction
	{
		// Token: 0x06007D36 RID: 32054 RVA: 0x00255B01 File Offset: 0x00253D01
		public override void Reset()
		{
			this.Target = null;
			this.Profile = null;
		}

		// Token: 0x06007D37 RID: 32055 RVA: 0x00255B14 File Offset: 0x00253D14
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				EnemyHitEffectsRegular component = safe.GetComponent<EnemyHitEffectsRegular>();
				if (component)
				{
					component.Profile = (this.Profile.Value as EnemyHitEffectsProfile);
				}
			}
			base.Finish();
		}

		// Token: 0x04007D36 RID: 32054
		public FsmOwnerDefault Target;

		// Token: 0x04007D37 RID: 32055
		[ObjectType(typeof(EnemyHitEffectsProfile))]
		public FsmObject Profile;
	}
}
