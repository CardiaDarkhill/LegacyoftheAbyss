using System;
using HutongGames.PlayMaker.TweenEnums;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001105 RID: 4357
	[ActionCategory(ActionCategory.Tween)]
	[Tooltip("Tween a Quaternion variable using a custom easing function.")]
	public class TweenQuaternion : TweenVariableBase<FsmQuaternion>
	{
		// Token: 0x060075CB RID: 30155 RVA: 0x00240052 File Offset: 0x0023E252
		protected override object GetOffsetValue(object value, object offset)
		{
			return (Quaternion)value * (Quaternion)offset;
		}

		// Token: 0x060075CC RID: 30156 RVA: 0x0024006C File Offset: 0x0023E26C
		protected override void DoTween()
		{
			float t = base.easingFunction(0f, 1f, this.normalizedTime);
			this.variable.Value = ((this.interpolation == RotationInterpolation.Linear) ? Quaternion.Lerp(this.fromValue.Value, this.toValue.Value, t) : Quaternion.Slerp(this.fromValue.Value, this.toValue.Value, t));
		}

		// Token: 0x0400763D RID: 30269
		[Tooltip("Type of interpolation. Linear is faster but looks worse if the rotations are far apart.")]
		[DisplayOrder(1)]
		public RotationInterpolation interpolation;
	}
}
