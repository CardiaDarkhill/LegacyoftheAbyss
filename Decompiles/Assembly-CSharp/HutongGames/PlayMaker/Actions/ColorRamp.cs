using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E5F RID: 3679
	[ActionCategory(ActionCategory.Color)]
	[Tooltip("Sample a color on a continuous color gradient. Define the gradient with a color array. Use Sample At to get an interpolated color.\n\nFor example:\nIf Element 1 is black and Element 2 is red:\n<ul><li>Sample At 1 = black</li><li>Sample At 2 = red</li><li>Sample At 1.5 = dark red</li></ul>")]
	public class ColorRamp : FsmStateAction
	{
		// Token: 0x06006908 RID: 26888 RVA: 0x0020FA3F File Offset: 0x0020DC3F
		public override void Reset()
		{
			this.colors = new FsmColor[3];
			this.sampleAt = 0f;
			this.storeColor = null;
			this.everyFrame = false;
		}

		// Token: 0x06006909 RID: 26889 RVA: 0x0020FA6B File Offset: 0x0020DC6B
		public override void OnEnter()
		{
			this.DoColorRamp();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600690A RID: 26890 RVA: 0x0020FA81 File Offset: 0x0020DC81
		public override void OnUpdate()
		{
			this.DoColorRamp();
		}

		// Token: 0x0600690B RID: 26891 RVA: 0x0020FA8C File Offset: 0x0020DC8C
		private void DoColorRamp()
		{
			if (this.colors == null)
			{
				return;
			}
			if (this.colors.Length == 0)
			{
				return;
			}
			if (this.sampleAt == null)
			{
				return;
			}
			if (this.storeColor == null)
			{
				return;
			}
			float num = Mathf.Clamp(this.sampleAt.Value, 0f, (float)(this.colors.Length - 1));
			Color value;
			if (num == 0f)
			{
				value = this.colors[0].Value;
			}
			else if (num == (float)this.colors.Length)
			{
				value = this.colors[this.colors.Length - 1].Value;
			}
			else
			{
				Color value2 = this.colors[Mathf.FloorToInt(num)].Value;
				Color value3 = this.colors[Mathf.CeilToInt(num)].Value;
				num -= Mathf.Floor(num);
				value = Color.Lerp(value2, value3, num);
			}
			this.storeColor.Value = value;
		}

		// Token: 0x0600690C RID: 26892 RVA: 0x0020FB5E File Offset: 0x0020DD5E
		public override string ErrorCheck()
		{
			if (this.colors.Length < 2)
			{
				return "Define at least 2 colors to make a gradient.";
			}
			return null;
		}

		// Token: 0x04006859 RID: 26713
		[RequiredField]
		[Tooltip("Array of colors to defining the gradient.")]
		public FsmColor[] colors;

		// Token: 0x0400685A RID: 26714
		[RequiredField]
		[Tooltip("Point on the gradient to sample. Should be between 0 and the number of colors in the gradient.")]
		public FsmFloat sampleAt;

		// Token: 0x0400685B RID: 26715
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the sampled color in a Color variable.")]
		public FsmColor storeColor;

		// Token: 0x0400685C RID: 26716
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
