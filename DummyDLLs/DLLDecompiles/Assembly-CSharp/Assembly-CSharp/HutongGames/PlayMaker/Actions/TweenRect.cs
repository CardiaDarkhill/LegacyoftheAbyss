using System;
using HutongGames.Extensions;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001106 RID: 4358
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a Rect variable using a custom easing function.")]
	public class TweenRect : TweenVariableBase<FsmRect>
	{
		// Token: 0x060075CE RID: 30158 RVA: 0x002400EC File Offset: 0x0023E2EC
		protected override object GetOffsetValue(object value, object offset)
		{
			Rect rect = (Rect)value;
			Rect rect2 = (Rect)offset;
			return new Rect(rect.x + rect2.x, rect.y + rect2.y, rect.width + rect2.width, rect.height + rect2.height);
		}

		// Token: 0x060075CF RID: 30159 RVA: 0x00240150 File Offset: 0x0023E350
		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, this.normalizedTime);
			this.variable.Value = this.variable.Value.Lerp((Rect)base.StartValue, (Rect)base.EndValue, t);
		}
	}
}
