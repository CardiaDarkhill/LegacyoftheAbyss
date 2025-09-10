using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D07 RID: 3335
	public class ScreenFader : FsmStateAction
	{
		// Token: 0x060062B1 RID: 25265 RVA: 0x001F3423 File Offset: 0x001F1623
		public override void Reset()
		{
			this.startColour = null;
			this.endColour = null;
			this.duration = null;
		}

		// Token: 0x060062B2 RID: 25266 RVA: 0x001F343C File Offset: 0x001F163C
		public override void OnEnter()
		{
			ScreenFaderUtils.Fade(this.startColour.IsNone ? ScreenFaderUtils.GetColour() : this.startColour.Value, this.endColour.Value, this.duration.Value);
			base.Finish();
		}

		// Token: 0x0400611E RID: 24862
		public FsmColor startColour;

		// Token: 0x0400611F RID: 24863
		[RequiredField]
		public FsmColor endColour;

		// Token: 0x04006120 RID: 24864
		public FsmFloat duration;
	}
}
