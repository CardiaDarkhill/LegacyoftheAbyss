using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010CE RID: 4302
	[ActionCategory(ActionCategory.Time)]
	[Tooltip("Gets system date and time info and stores it in a string variable. An optional format string gives you a lot of control over the formatting (see online docs for format syntax).")]
	public class GetSystemDateTime : FsmStateAction
	{
		// Token: 0x06007485 RID: 29829 RVA: 0x0023A97F File Offset: 0x00238B7F
		public override void Reset()
		{
			this.storeString = null;
			this.format = "MM/dd/yyyy HH:mm";
		}

		// Token: 0x06007486 RID: 29830 RVA: 0x0023A998 File Offset: 0x00238B98
		public override void OnEnter()
		{
			this.storeString.Value = DateTime.Now.ToString(this.format.Value);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007487 RID: 29831 RVA: 0x0023A9D8 File Offset: 0x00238BD8
		public override void OnUpdate()
		{
			this.storeString.Value = DateTime.Now.ToString(this.format.Value);
		}

		// Token: 0x040074C3 RID: 29891
		[UIHint(UIHint.Variable)]
		[Tooltip("Store System DateTime as a string.")]
		public FsmString storeString;

		// Token: 0x040074C4 RID: 29892
		[Tooltip("Optional format string. E.g., MM/dd/yyyy HH:mm")]
		public FsmString format;

		// Token: 0x040074C5 RID: 29893
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
