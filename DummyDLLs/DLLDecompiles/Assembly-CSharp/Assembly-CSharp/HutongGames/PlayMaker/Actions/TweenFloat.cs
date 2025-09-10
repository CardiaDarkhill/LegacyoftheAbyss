using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001101 RID: 4353
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a float variable using a custom easing function.")]
	public class TweenFloat : TweenVariableBase<FsmFloat>
	{
		// Token: 0x060075B7 RID: 30135 RVA: 0x0023FB89 File Offset: 0x0023DD89
		protected override object GetOffsetValue(object value, object offset)
		{
			return (float)value + (float)offset;
		}

		// Token: 0x060075B8 RID: 30136 RVA: 0x0023FB9D File Offset: 0x0023DD9D
		protected override void DoTween()
		{
			this.variable.Value = base.easingFunction((float)base.StartValue, (float)base.EndValue, this.normalizedTime);
		}
	}
}
