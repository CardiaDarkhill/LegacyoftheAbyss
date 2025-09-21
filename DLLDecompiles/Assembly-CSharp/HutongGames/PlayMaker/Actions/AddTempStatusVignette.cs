using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001392 RID: 5010
	public class AddTempStatusVignette : FsmStateAction
	{
		// Token: 0x060080AB RID: 32939 RVA: 0x0025EE7A File Offset: 0x0025D07A
		public override void Reset()
		{
			this.Status = null;
		}

		// Token: 0x060080AC RID: 32940 RVA: 0x0025EE83 File Offset: 0x0025D083
		public override void OnEnter()
		{
			StatusVignette.AddTempStatus((StatusVignette.TempStatusTypes)this.Status.Value);
			base.Finish();
		}

		// Token: 0x04007FF3 RID: 32755
		[ObjectType(typeof(StatusVignette.TempStatusTypes))]
		public FsmEnum Status;
	}
}
