using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E5E RID: 3678
	[ActionCategory(ActionCategory.Color)]
	[Tooltip("Interpolate through an array of Colors over a specified amount of Time.")]
	public class ColorInterpolate : FsmStateAction
	{
		// Token: 0x06006903 RID: 26883 RVA: 0x0020F844 File Offset: 0x0020DA44
		public override void Reset()
		{
			this.colors = new FsmColor[3];
			this.time = 1f;
			this.storeColor = null;
			this.finishEvent = null;
			this.realTime = false;
		}

		// Token: 0x06006904 RID: 26884 RVA: 0x0020F878 File Offset: 0x0020DA78
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			if (this.colors.Length < 2)
			{
				if (this.colors.Length == 1)
				{
					this.storeColor.Value = this.colors[0].Value;
				}
				base.Finish();
				return;
			}
			this.storeColor.Value = this.colors[0].Value;
		}

		// Token: 0x06006905 RID: 26885 RVA: 0x0020F8E8 File Offset: 0x0020DAE8
		public override void OnUpdate()
		{
			if (this.realTime)
			{
				this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.currentTime += Time.deltaTime;
			}
			if (this.currentTime > this.time.Value)
			{
				base.Finish();
				this.storeColor.Value = this.colors[this.colors.Length - 1].Value;
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
				return;
			}
			float num = (float)(this.colors.Length - 1) * this.currentTime / this.time.Value;
			Color value;
			if (num.Equals(0f))
			{
				value = this.colors[0].Value;
			}
			else if (num.Equals((float)(this.colors.Length - 1)))
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

		// Token: 0x06006906 RID: 26886 RVA: 0x0020FA23 File Offset: 0x0020DC23
		public override string ErrorCheck()
		{
			if (this.colors.Length >= 2)
			{
				return null;
			}
			return "Define at least 2 colors to make a gradient.";
		}

		// Token: 0x04006852 RID: 26706
		[RequiredField]
		[Tooltip("An array of colors. Set the number of colors, then set each color.")]
		public FsmColor[] colors;

		// Token: 0x04006853 RID: 26707
		[RequiredField]
		[Tooltip("How long it should take to interpolate through all the colors in the array.")]
		public FsmFloat time;

		// Token: 0x04006854 RID: 26708
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the interpolated color in a Color variable.")]
		public FsmColor storeColor;

		// Token: 0x04006855 RID: 26709
		[Tooltip("Event to send when the interpolation finishes.")]
		public FsmEvent finishEvent;

		// Token: 0x04006856 RID: 26710
		[Tooltip("Ignore TimeScale. Useful when the game is paused.")]
		public bool realTime;

		// Token: 0x04006857 RID: 26711
		private float startTime;

		// Token: 0x04006858 RID: 26712
		private float currentTime;
	}
}
