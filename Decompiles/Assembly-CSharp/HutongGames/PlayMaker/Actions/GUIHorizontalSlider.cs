using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000ECE RID: 3790
	[ActionCategory(ActionCategory.GUI)]
	[Tooltip("GUI Horizontal Slider connected to a Float Variable.")]
	public class GUIHorizontalSlider : GUIAction
	{
		// Token: 0x06006AE6 RID: 27366 RVA: 0x0021551C File Offset: 0x0021371C
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.leftValue = 0f;
			this.rightValue = 100f;
			this.sliderStyle = "horizontalslider";
			this.thumbStyle = "horizontalsliderthumb";
		}

		// Token: 0x06006AE7 RID: 27367 RVA: 0x00215578 File Offset: 0x00213778
		public override void OnGUI()
		{
			base.OnGUI();
			if (this.floatVariable != null)
			{
				this.floatVariable.Value = GUI.HorizontalSlider(this.rect, this.floatVariable.Value, this.leftValue.Value, this.rightValue.Value, (this.sliderStyle.Value != "") ? this.sliderStyle.Value : "horizontalslider", (this.thumbStyle.Value != "") ? this.thumbStyle.Value : "horizontalsliderthumb");
			}
		}

		// Token: 0x04006A34 RID: 27188
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to link the slider to. Moving the slider changes the value, and changes in the value move the slider.")]
		public FsmFloat floatVariable;

		// Token: 0x04006A35 RID: 27189
		[RequiredField]
		[Tooltip("The value of the float variable when slider is all the way to the left.")]
		public FsmFloat leftValue;

		// Token: 0x04006A36 RID: 27190
		[RequiredField]
		[Tooltip("The value of the float variable when slider is all the way to the right.")]
		public FsmFloat rightValue;

		// Token: 0x04006A37 RID: 27191
		[Tooltip("Optional GUIStyle for the slider track.")]
		public FsmString sliderStyle;

		// Token: 0x04006A38 RID: 27192
		[Tooltip("Optional GUIStyle for the slider thumb.")]
		public FsmString thumbStyle;
	}
}
