using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001252 RID: 4690
	public class HeroFaceInstant : FsmStateAction
	{
		// Token: 0x06007BEE RID: 31726 RVA: 0x002510B0 File Offset: 0x0024F2B0
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06007BEF RID: 31727 RVA: 0x002510BC File Offset: 0x0024F2BC
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (!safe)
			{
				base.Finish();
				return;
			}
			this.hc = HeroController.instance;
			Transform transform = this.hc.transform;
			if (safe.transform.position.x - transform.position.x > 0f)
			{
				this.hc.FaceRight();
			}
			else
			{
				this.hc.FaceLeft();
			}
			base.Finish();
		}

		// Token: 0x04007C1A RID: 31770
		public FsmOwnerDefault Target;

		// Token: 0x04007C1B RID: 31771
		private HeroController hc;
	}
}
