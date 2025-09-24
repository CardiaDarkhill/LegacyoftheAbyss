using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001068 RID: 4200
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Sets the value of a Game Object Variable.")]
	public class SetGameObjectSelf : FsmStateAction
	{
		// Token: 0x060072BC RID: 29372 RVA: 0x00234D31 File Offset: 0x00232F31
		public override void Reset()
		{
			this.variable = null;
			this.gameObject = new FsmOwnerDefault();
			this.everyFrame = false;
		}

		// Token: 0x060072BD RID: 29373 RVA: 0x00234D4C File Offset: 0x00232F4C
		public override void OnEnter()
		{
			GameObject safe = this.gameObject.GetSafe(this);
			if (safe != null)
			{
				this.variable.Value = safe;
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060072BE RID: 29374 RVA: 0x00234D8C File Offset: 0x00232F8C
		public override void OnUpdate()
		{
			GameObject safe = this.gameObject.GetSafe(this);
			if (safe != null)
			{
				this.variable.Value = safe;
			}
		}

		// Token: 0x040072C1 RID: 29377
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmGameObject variable;

		// Token: 0x040072C2 RID: 29378
		public FsmOwnerDefault gameObject;

		// Token: 0x040072C3 RID: 29379
		public bool everyFrame;
	}
}
