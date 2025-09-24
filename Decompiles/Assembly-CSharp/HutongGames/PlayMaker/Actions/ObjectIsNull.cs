using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC5 RID: 3269
	public class ObjectIsNull : FSMUtility.CheckFsmStateAction
	{
		// Token: 0x06006193 RID: 24979 RVA: 0x001EE7E2 File Offset: 0x001EC9E2
		public override void Reset()
		{
			base.Reset();
			this.Target = null;
		}

		// Token: 0x17000BC5 RID: 3013
		// (get) Token: 0x06006194 RID: 24980 RVA: 0x001EE7F1 File Offset: 0x001EC9F1
		public override bool IsTrue
		{
			get
			{
				return this.Target.Value == null;
			}
		}

		// Token: 0x04005FBF RID: 24511
		public FsmObject Target;
	}
}
