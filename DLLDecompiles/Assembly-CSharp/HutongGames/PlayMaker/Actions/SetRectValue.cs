using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001015 RID: 4117
	[ActionCategory(ActionCategory.Rect)]
	[Tooltip("Sets the value of a Rect Variable.")]
	public class SetRectValue : FsmStateAction
	{
		// Token: 0x06007128 RID: 28968 RVA: 0x0022D52C File Offset: 0x0022B72C
		public override void Reset()
		{
			this.rectVariable = null;
			this.rectValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06007129 RID: 28969 RVA: 0x0022D543 File Offset: 0x0022B743
		public override void OnEnter()
		{
			this.rectVariable.Value = this.rectValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600712A RID: 28970 RVA: 0x0022D569 File Offset: 0x0022B769
		public override void OnUpdate()
		{
			this.rectVariable.Value = this.rectValue.Value;
		}

		// Token: 0x040070C3 RID: 28867
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Rect Variable to set.")]
		public FsmRect rectVariable;

		// Token: 0x040070C4 RID: 28868
		[RequiredField]
		[Tooltip("The value to set it to.")]
		public FsmRect rectValue;

		// Token: 0x040070C5 RID: 28869
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
