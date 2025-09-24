using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200110B RID: 4363
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a Vector2 variable using a custom easing function.")]
	public class TweenVector2 : TweenVariableBase<FsmVector2>
	{
		// Token: 0x060075F9 RID: 30201 RVA: 0x0024088A File Offset: 0x0023EA8A
		protected override object GetOffsetValue(object value, object offset)
		{
			return (Vector2)value + (Vector2)offset;
		}

		// Token: 0x060075FA RID: 30202 RVA: 0x002408A4 File Offset: 0x0023EAA4
		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, this.normalizedTime);
			this.variable.Value = Vector2.Lerp((Vector2)base.StartValue, (Vector2)base.EndValue, t);
		}
	}
}
