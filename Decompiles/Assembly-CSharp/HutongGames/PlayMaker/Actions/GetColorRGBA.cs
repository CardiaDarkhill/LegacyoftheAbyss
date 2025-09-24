using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E60 RID: 3680
	[ActionCategory(ActionCategory.Color)]
	[Tooltip("Get the RGBA channels of a Color Variable and store them in Float Variables.")]
	public class GetColorRGBA : FsmStateAction
	{
		// Token: 0x0600690E RID: 26894 RVA: 0x0020FB7A File Offset: 0x0020DD7A
		public override void Reset()
		{
			this.color = null;
			this.storeRed = null;
			this.storeGreen = null;
			this.storeBlue = null;
			this.storeAlpha = null;
			this.everyFrame = false;
		}

		// Token: 0x0600690F RID: 26895 RVA: 0x0020FBA6 File Offset: 0x0020DDA6
		public override void OnEnter()
		{
			this.DoGetColorRGBA();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006910 RID: 26896 RVA: 0x0020FBBC File Offset: 0x0020DDBC
		public override void OnUpdate()
		{
			this.DoGetColorRGBA();
		}

		// Token: 0x06006911 RID: 26897 RVA: 0x0020FBC4 File Offset: 0x0020DDC4
		private void DoGetColorRGBA()
		{
			if (this.color.IsNone)
			{
				return;
			}
			this.storeRed.Value = this.color.Value.r;
			this.storeGreen.Value = this.color.Value.g;
			this.storeBlue.Value = this.color.Value.b;
			this.storeAlpha.Value = this.color.Value.a;
		}

		// Token: 0x0400685D RID: 26717
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Color variable.")]
		public FsmColor color;

		// Token: 0x0400685E RID: 26718
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the red channel in a float variable.")]
		public FsmFloat storeRed;

		// Token: 0x0400685F RID: 26719
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the green channel in a float variable.")]
		public FsmFloat storeGreen;

		// Token: 0x04006860 RID: 26720
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the blue channel in a float variable.")]
		public FsmFloat storeBlue;

		// Token: 0x04006861 RID: 26721
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the alpha channel in a float variable.")]
		public FsmFloat storeAlpha;

		// Token: 0x04006862 RID: 26722
		[Tooltip("Repeat every frame. Useful if the color variable is changing.")]
		public bool everyFrame;
	}
}
