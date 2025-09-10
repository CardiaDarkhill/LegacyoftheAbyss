using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AFD RID: 2813
	public class SetDamageEnemies : FsmStateAction
	{
		// Token: 0x06005916 RID: 22806 RVA: 0x001C3E0A File Offset: 0x001C200A
		public override void Reset()
		{
			this.Target = null;
			this.Enabled = null;
		}

		// Token: 0x06005917 RID: 22807 RVA: 0x001C3E1C File Offset: 0x001C201C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe != null)
			{
				DamageEnemies component = safe.GetComponent<DamageEnemies>();
				if (component != null)
				{
					component.enabled = this.Enabled.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04005432 RID: 21554
		[UIHint(UIHint.Variable)]
		[CheckForComponent(typeof(DamageEnemies))]
		public FsmOwnerDefault Target;

		// Token: 0x04005433 RID: 21555
		[RequiredField]
		public new FsmBool Enabled;
	}
}
