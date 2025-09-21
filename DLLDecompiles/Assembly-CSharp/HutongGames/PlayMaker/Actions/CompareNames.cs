using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E64 RID: 3684
	public class CompareNames : FsmStateAction
	{
		// Token: 0x06006921 RID: 26913 RVA: 0x0020FE9F File Offset: 0x0020E09F
		public override void Reset()
		{
			this.name = new FsmString();
			this.target = new FsmEventTarget();
			this.strings = new FsmArray();
			this.trueEvent = null;
			this.falseEvent = null;
		}

		// Token: 0x06006922 RID: 26914 RVA: 0x0020FED0 File Offset: 0x0020E0D0
		public override void OnEnter()
		{
			if (!this.name.IsNone && this.name.Value != "")
			{
				foreach (string value in this.strings.stringValues)
				{
					if (this.name.Value.Contains(value))
					{
						base.Fsm.Event(this.target, this.trueEvent);
						base.Finish();
						return;
					}
				}
				base.Fsm.Event(this.target, this.falseEvent);
			}
			base.Finish();
		}

		// Token: 0x0400686F RID: 26735
		public FsmString name;

		// Token: 0x04006870 RID: 26736
		[ArrayEditor(VariableType.String, "", 0, 0, 65536)]
		public FsmArray strings;

		// Token: 0x04006871 RID: 26737
		public FsmEventTarget target;

		// Token: 0x04006872 RID: 26738
		public FsmEvent trueEvent;

		// Token: 0x04006873 RID: 26739
		public FsmEvent falseEvent;
	}
}
