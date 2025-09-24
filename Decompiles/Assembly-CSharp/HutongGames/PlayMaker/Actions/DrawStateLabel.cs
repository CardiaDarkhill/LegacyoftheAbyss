using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E83 RID: 3715
	[ActionCategory(ActionCategory.Debug)]
	[Tooltip("Draws a state label for this FSM in the Game View. The label is drawn on the game object that owns the FSM. Use this to override the global setting in the PlayMaker Debug menu.")]
	public class DrawStateLabel : FsmStateAction
	{
		// Token: 0x060069AB RID: 27051 RVA: 0x002112B5 File Offset: 0x0020F4B5
		public override void Reset()
		{
			this.showLabel = true;
		}

		// Token: 0x060069AC RID: 27052 RVA: 0x002112C3 File Offset: 0x0020F4C3
		public override void OnEnter()
		{
			base.Fsm.ShowStateLabel = this.showLabel.Value;
			base.Finish();
		}

		// Token: 0x040068D9 RID: 26841
		[RequiredField]
		[Tooltip("Set to True to show State labels, or False to hide them.")]
		public FsmBool showLabel;
	}
}
