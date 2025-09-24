using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020012A0 RID: 4768
	[ActionCategory("Enemy AI")]
	public class StartClimber : FsmStateAction
	{
		// Token: 0x06007D1C RID: 32028 RVA: 0x002557A2 File Offset: 0x002539A2
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06007D1D RID: 32029 RVA: 0x002557AC File Offset: 0x002539AC
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				Climber component = safe.GetComponent<Climber>();
				if (component)
				{
					component.enabled = true;
				}
			}
			base.Finish();
		}

		// Token: 0x04007D27 RID: 32039
		public FsmOwnerDefault Target;
	}
}
