using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C55 RID: 3157
	public class GameObjectIsActiveEveryFrame : FSMUtility.CheckFsmStateEveryFrameAction
	{
		// Token: 0x06005F9D RID: 24477 RVA: 0x001E557E File Offset: 0x001E377E
		public override void Reset()
		{
			base.Reset();
			this.Target = null;
			this.ActiveSpace = Space.Self;
		}

		// Token: 0x17000BC1 RID: 3009
		// (get) Token: 0x06005F9E RID: 24478 RVA: 0x001E5594 File Offset: 0x001E3794
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

		// Token: 0x04005CF4 RID: 23796
		public FsmOwnerDefault Target;

		// Token: 0x04005CF5 RID: 23797
		public Space ActiveSpace;
	}
}
