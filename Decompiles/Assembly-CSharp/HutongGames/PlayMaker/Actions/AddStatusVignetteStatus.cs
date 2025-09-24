using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001390 RID: 5008
	public class AddStatusVignetteStatus : FsmStateAction
	{
		// Token: 0x060080A4 RID: 32932 RVA: 0x0025ED5F File Offset: 0x0025CF5F
		public override void Reset()
		{
			this.Status = null;
			this.StoreSet = null;
			this.InState = null;
		}

		// Token: 0x060080A5 RID: 32933 RVA: 0x0025ED78 File Offset: 0x0025CF78
		public override void OnEnter()
		{
			if (this.StoreSet.IsNone || !this.StoreSet.Value)
			{
				StatusVignette.AddStatus((StatusVignette.StatusTypes)this.Status.Value);
				this.StoreSet.Value = true;
			}
			if (!this.InState.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x060080A6 RID: 32934 RVA: 0x0025EDD3 File Offset: 0x0025CFD3
		public override void OnExit()
		{
			if (!this.InState.Value)
			{
				return;
			}
			StatusVignette.RemoveStatus((StatusVignette.StatusTypes)this.Status.Value);
			this.StoreSet.Value = false;
		}

		// Token: 0x04007FEE RID: 32750
		[ObjectType(typeof(StatusVignette.StatusTypes))]
		public FsmEnum Status;

		// Token: 0x04007FEF RID: 32751
		[UIHint(UIHint.Variable)]
		public FsmBool StoreSet;

		// Token: 0x04007FF0 RID: 32752
		public FsmBool InState;
	}
}
