using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001256 RID: 4694
	public class HeroGetWarriorCrestState : FsmStateAction
	{
		// Token: 0x06007BFE RID: 31742 RVA: 0x00251357 File Offset: 0x0024F557
		public override void Reset()
		{
			this.IsInRageMode = null;
		}

		// Token: 0x06007BFF RID: 31743 RVA: 0x00251360 File Offset: 0x0024F560
		public override void OnEnter()
		{
			HeroController.WarriorCrestStateInfo warriorState = HeroController.instance.WarriorState;
			this.IsInRageMode.Value = warriorState.IsInRageMode;
			base.Finish();
		}

		// Token: 0x04007C24 RID: 31780
		[UIHint(UIHint.Variable)]
		public FsmBool IsInRageMode;
	}
}
