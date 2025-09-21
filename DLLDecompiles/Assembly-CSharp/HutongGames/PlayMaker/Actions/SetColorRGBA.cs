using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E62 RID: 3682
	[ActionCategory(ActionCategory.Color)]
	[Tooltip("Sets the RGBA channels of a Color Variable. To leave any channel unchanged, set variable to 'None'.")]
	public class SetColorRGBA : FsmStateAction
	{
		// Token: 0x06006917 RID: 26903 RVA: 0x0020FD14 File Offset: 0x0020DF14
		public override void Reset()
		{
			this.colorVariable = null;
			this.red = 0f;
			this.green = 0f;
			this.blue = 0f;
			this.alpha = 1f;
			this.everyFrame = false;
		}

		// Token: 0x06006918 RID: 26904 RVA: 0x0020FD6F File Offset: 0x0020DF6F
		public override void OnEnter()
		{
			this.DoSetColorRGBA();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006919 RID: 26905 RVA: 0x0020FD85 File Offset: 0x0020DF85
		public override void OnUpdate()
		{
			this.DoSetColorRGBA();
		}

		// Token: 0x0600691A RID: 26906 RVA: 0x0020FD90 File Offset: 0x0020DF90
		private void DoSetColorRGBA()
		{
			if (this.colorVariable == null)
			{
				return;
			}
			Color value = this.colorVariable.Value;
			if (!this.red.IsNone)
			{
				value.r = this.red.Value;
			}
			if (!this.green.IsNone)
			{
				value.g = this.green.Value;
			}
			if (!this.blue.IsNone)
			{
				value.b = this.blue.Value;
			}
			if (!this.alpha.IsNone)
			{
				value.a = this.alpha.Value;
			}
			this.colorVariable.Value = value;
		}

		// Token: 0x04006866 RID: 26726
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Color Variable to set.")]
		public FsmColor colorVariable;

		// Token: 0x04006867 RID: 26727
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the red channel of the color variable.")]
		public FsmFloat red;

		// Token: 0x04006868 RID: 26728
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the green channel of the color variable.")]
		public FsmFloat green;

		// Token: 0x04006869 RID: 26729
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the blue channel of the color variable.")]
		public FsmFloat blue;

		// Token: 0x0400686A RID: 26730
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Set the alpha channel of the color variable.")]
		public FsmFloat alpha;

		// Token: 0x0400686B RID: 26731
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
