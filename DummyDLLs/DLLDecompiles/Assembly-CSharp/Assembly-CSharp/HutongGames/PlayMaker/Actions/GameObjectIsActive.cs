using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C54 RID: 3156
	public class GameObjectIsActive : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x06005F9A RID: 24474 RVA: 0x001E5525 File Offset: 0x001E3725
		public override void Reset()
		{
			base.Reset();
			this.Target = null;
			this.ActiveSpace = Space.Self;
		}

		// Token: 0x17000BC0 RID: 3008
		// (get) Token: 0x06005F9B RID: 24475 RVA: 0x001E553C File Offset: 0x001E373C
		public override bool IsTrue
		{
			get
			{
				GameObject safe = this.Target.GetSafe(this);
				if (!safe)
				{
					return false;
				}
				if (this.ActiveSpace != Space.Self)
				{
					return safe.activeInHierarchy;
				}
				return safe.activeSelf;
			}
		}

		// Token: 0x04005CF2 RID: 23794
		public FsmOwnerDefault Target;

		// Token: 0x04005CF3 RID: 23795
		public Space ActiveSpace;
	}
}
