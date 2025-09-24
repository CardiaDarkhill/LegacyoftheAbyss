using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001391 RID: 5009
	public class RemoveStatusVignetteStatus : FsmStateAction
	{
		// Token: 0x060080A8 RID: 32936 RVA: 0x0025EE0C File Offset: 0x0025D00C
		public override void Reset()
		{
			this.Status = null;
			this.StoredSet = false;
		}

		// Token: 0x060080A9 RID: 32937 RVA: 0x0025EE24 File Offset: 0x0025D024
		public override void OnEnter()
		{
			if (this.StoredSet.IsNone || this.StoredSet.Value)
			{
				StatusVignette.RemoveStatus((StatusVignette.StatusTypes)this.Status.Value);
				this.StoredSet.Value = false;
			}
			base.Finish();
		}

		// Token: 0x04007FF1 RID: 32753
		[ObjectType(typeof(StatusVignette.StatusTypes))]
		public FsmEnum Status;

		// Token: 0x04007FF2 RID: 32754
		[UIHint(UIHint.Variable)]
		public FsmBool StoredSet;
	}
}
