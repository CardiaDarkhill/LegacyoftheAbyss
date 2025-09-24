using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001102 RID: 4354
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween an integer variable using a custom easing function. NOTE: Tweening is performed on float values and then rounded to the integer value.")]
	public class TweenInt : TweenVariableBase<FsmInt>
	{
		// Token: 0x060075BA RID: 30138 RVA: 0x0023FBD9 File Offset: 0x0023DDD9
		protected override object GetOffsetValue(object value, object offset)
		{
			return (int)value + (int)offset;
		}

		// Token: 0x060075BB RID: 30139 RVA: 0x0023FBED File Offset: 0x0023DDED
		protected override void DoTween()
		{
			this.variable.Value = (int)base.easingFunction((float)((int)base.StartValue), (float)((int)base.EndValue), this.normalizedTime);
		}
	}
}
