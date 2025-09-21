using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ED1 RID: 3793
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("GUI Vertical Slider connected to a Float Variable.")]
	public class GUIVerticalSlider : GUIAction
	{
		// Token: 0x06006AEE RID: 27374 RVA: 0x002156B8 File Offset: 0x002138B8
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.topValue = 100f;
			this.bottomValue = 0f;
			this.sliderStyle = "verticalslider";
			this.thumbStyle = "verticalsliderthumb";
			this.width = 0.1f;
		}

		// Token: 0x06006AEF RID: 27375 RVA: 0x00215724 File Offset: 0x00213924
		public override void OnGUI()
		{
			base.OnGUI();
			if (this.floatVariable != null)
			{
				this.floatVariable.Value = GUI.VerticalSlider(this.rect, this.floatVariable.Value, this.topValue.Value, this.bottomValue.Value, (this.sliderStyle.Value != "") ? this.sliderStyle.Value : "verticalslider", (this.thumbStyle.Value != "") ? this.thumbStyle.Value : "verticalsliderthumb");
			}
		}

		// Token: 0x04006A3A RID: 27194
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Float Variable linked to the slider value.")]
		public FsmFloat floatVariable;

		// Token: 0x04006A3B RID: 27195
		[RequiredField]
		[Tooltip("The value of the variable at the top of the slider.")]
		public FsmFloat topValue;

		// Token: 0x04006A3C RID: 27196
		[RequiredField]
		[Tooltip("The value of the variable at the bottom of the slider.")]
		public FsmFloat bottomValue;

		// Token: 0x04006A3D RID: 27197
		[Tooltip("Optional GUIStyle for the slider track.")]
		public FsmString sliderStyle;

		// Token: 0x04006A3E RID: 27198
		[Tooltip("Optional GUIStyle for the slider thumb.")]
		public FsmString thumbStyle;
	}
}
