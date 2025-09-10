using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AFE RID: 2814
	public class ResetDamageEnemiesLists : FsmStateAction
	{
		// Token: 0x06005919 RID: 22809 RVA: 0x001C3E6E File Offset: 0x001C206E
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x0600591A RID: 22810 RVA: 0x001C3E78 File Offset: 0x001C2078
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				DamageEnemies component = safe.GetComponent<DamageEnemies>();
				if (component != null)
				{
					try
					{
						component.EndDamage();
						component.StartDamage();
					}
					catch (Exception)
					{
					}
				}
			}
			base.Finish();
		}

		// Token: 0x04005434 RID: 21556
		[UIHint(UIHint.Variable)]
		[CheckForComponent(typeof(DamageEnemies))]
		public FsmOwnerDefault Target;
	}
}
