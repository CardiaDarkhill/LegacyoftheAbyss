using System;
using UnityEngine;

// Token: 0x020000A3 RID: 163
public class RandomisePlaybackOffset : StateMachineBehaviour
{
	// Token: 0x060004FD RID: 1277 RVA: 0x00019F84 File Offset: 0x00018184
	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (this.lastFrame < Time.frameCount)
		{
			if (this.minOffset > this.maxOffset)
			{
				this.minOffset = this.maxOffset;
			}
			if (this.randomizeEveryEntry || stateInfo.normalizedTime == 0f)
			{
				this.lastOffset = Random.Range(this.minOffset, this.maxOffset);
			}
			this.lastFrame = Time.frameCount + 5;
			animator.Play(stateInfo.fullPathHash, layerIndex, this.lastOffset);
		}
	}

	// Token: 0x040004D3 RID: 1235
	[Range(0f, 1f)]
	[SerializeField]
	private float minOffset;

	// Token: 0x040004D4 RID: 1236
	[Range(0f, 1f)]
	[SerializeField]
	private float maxOffset = 1f;

	// Token: 0x040004D5 RID: 1237
	[SerializeField]
	private bool randomizeEveryEntry = true;

	// Token: 0x040004D6 RID: 1238
	private float lastOffset;

	// Token: 0x040004D7 RID: 1239
	private int lastFrame = -1;
}
