using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001298 RID: 4760
	public class SetPoisonTintOverride : FsmStateAction
	{
		// Token: 0x06007CFE RID: 31998 RVA: 0x0025526F File Offset: 0x0025346F
		public override void Reset()
		{
			this.Target = null;
			this.IsPoison = null;
		}

		// Token: 0x06007CFF RID: 31999 RVA: 0x0025527F File Offset: 0x0025347F
		public override void OnEnter()
		{
			this.Target.GetSafe(this).GetComponent<PoisonTintBase>().SetPoisoned(this.IsPoison.Value);
			base.Finish();
		}

		// Token: 0x04007D10 RID: 32016
		[CheckForComponent(typeof(PoisonTintBase))]
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x04007D11 RID: 32017
		[RequiredField]
		public FsmBool IsPoison;
	}
}
