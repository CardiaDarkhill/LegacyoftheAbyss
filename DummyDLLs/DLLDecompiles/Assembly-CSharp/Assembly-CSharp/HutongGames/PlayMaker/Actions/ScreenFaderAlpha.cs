using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D08 RID: 3336
	public class ScreenFaderAlpha : FsmStateAction
	{
		// Token: 0x060062B4 RID: 25268 RVA: 0x001F3491 File Offset: 0x001F1691
		public override void Reset()
		{
			this.EndAlpha = null;
			this.Duration = null;
		}

		// Token: 0x060062B5 RID: 25269 RVA: 0x001F34A4 File Offset: 0x001F16A4
		public override void OnEnter()
		{
			Color colour;
			Color startColour = colour = ScreenFaderUtils.GetColour();
			colour.a = this.EndAlpha.Value;
			ScreenFaderUtils.Fade(startColour, colour, this.Duration.Value);
			base.Finish();
		}

		// Token: 0x04006121 RID: 24865
		[RequiredField]
		public FsmFloat EndAlpha;

		// Token: 0x04006122 RID: 24866
		public FsmFloat Duration;
	}
}
