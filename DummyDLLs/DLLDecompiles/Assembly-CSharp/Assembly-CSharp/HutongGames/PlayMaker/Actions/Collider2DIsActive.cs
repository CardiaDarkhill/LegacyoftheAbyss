using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF8 RID: 3064
	public class Collider2DIsActive : FSMUtility.CheckFsmStateEveryFrameAction
	{
		// Token: 0x06005DB7 RID: 23991 RVA: 0x001D8D51 File Offset: 0x001D6F51
		public override void Reset()
		{
			base.Reset();
			this.Target = null;
		}

		// Token: 0x17000BBF RID: 3007
		// (get) Token: 0x06005DB8 RID: 23992 RVA: 0x001D8D60 File Offset: 0x001D6F60
		public override bool IsTrue
		{
			get
			{
				GameObject safe = this.Target.GetSafe(this);
				if (!safe)
				{
					return false;
				}
				Collider2D component = safe.GetComponent<Collider2D>();
				return component && component.isActiveAndEnabled;
			}
		}

		// Token: 0x04005A09 RID: 23049
		public FsmOwnerDefault Target;
	}
}
