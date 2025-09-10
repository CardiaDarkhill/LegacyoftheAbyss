using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200048A RID: 1162
public class AnimateRigidBody2DProperties : BaseAnimator
{
	// Token: 0x060029F8 RID: 10744 RVA: 0x000B6424 File Offset: 0x000B4624
	public override void StartAnimation()
	{
		if (this.animationRoutine != null)
		{
			base.StopCoroutine(this.animationRoutine);
		}
		if (!this.body)
		{
			return;
		}
		this.animationRoutine = this.StartTimerRoutine(0f, this.duration, delegate(float time)
		{
			time = this.curve.Evaluate(time);
			this.body.linearDamping = Mathf.Lerp(this.linearDragRange.Start, this.linearDragRange.End, time);
			this.body.angularDamping = Mathf.Lerp(this.angularDragRange.Start, this.angularDragRange.End, time);
		}, null, null, false);
	}

	// Token: 0x04002A71 RID: 10865
	[SerializeField]
	private Rigidbody2D body;

	// Token: 0x04002A72 RID: 10866
	[SerializeField]
	private MinMaxFloat linearDragRange;

	// Token: 0x04002A73 RID: 10867
	[SerializeField]
	private MinMaxFloat angularDragRange;

	// Token: 0x04002A74 RID: 10868
	[SerializeField]
	private AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002A75 RID: 10869
	[SerializeField]
	private float duration;

	// Token: 0x04002A76 RID: 10870
	private Coroutine animationRoutine;
}
