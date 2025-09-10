using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200110C RID: 4364
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a Vector3 variable using a custom easing function.")]
	public class TweenVector3 : TweenVariableBase<FsmVector3>
	{
		// Token: 0x060075FC RID: 30204 RVA: 0x002408FC File Offset: 0x0023EAFC
		protected override object GetOffsetValue(object value, object offset)
		{
			return (Vector3)value + (Vector3)offset;
		}

		// Token: 0x060075FD RID: 30205 RVA: 0x00240914 File Offset: 0x0023EB14
		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, this.normalizedTime);
			this.variable.Value = Vector3.Lerp((Vector3)base.StartValue, (Vector3)base.EndValue, t);
		}
	}
}
