using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000113 RID: 275
public class AudioSourceMovingVolume : MonoBehaviour, IUpdateBatchableLateUpdate
{
	// Token: 0x170000B5 RID: 181
	// (get) Token: 0x06000893 RID: 2195 RVA: 0x00028490 File Offset: 0x00026690
	public bool ShouldUpdate
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x00028494 File Offset: 0x00026694
	private void OnEnable()
	{
		this.previousLocalPos = this.target.localPosition;
		this.audioSource.volume = 0f;
		this.curveAnimator = this.target.GetComponentInParent<VectorCurveAnimator>();
		if (this.curveAnimator)
		{
			VectorCurveAnimator vectorCurveAnimator = this.curveAnimator;
			vectorCurveAnimator.UpdatedPosition = (Action)Delegate.Combine(vectorCurveAnimator.UpdatedPosition, new Action(this.BatchedLateUpdate));
			return;
		}
		this.updateBatcher = GameManager.instance.GetComponent<UpdateBatcher>();
		this.updateBatcher.Add(this);
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x0002852C File Offset: 0x0002672C
	private void OnDisable()
	{
		if (this.curveAnimator)
		{
			VectorCurveAnimator vectorCurveAnimator = this.curveAnimator;
			vectorCurveAnimator.UpdatedPosition = (Action)Delegate.Remove(vectorCurveAnimator.UpdatedPosition, new Action(this.BatchedLateUpdate));
			this.curveAnimator = null;
		}
		if (this.updateBatcher)
		{
			this.updateBatcher.Remove(this);
			this.updateBatcher = null;
		}
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x00028598 File Offset: 0x00026798
	public void BatchedLateUpdate()
	{
		Vector2 a = this.target.localPosition;
		Vector2 vector = a - this.previousLocalPos;
		float num = Time.deltaTime;
		if (num == 0f)
		{
			num = Time.fixedDeltaTime;
		}
		float value = vector.magnitude / num;
		float tbetween = this.speedRange.GetTBetween(value);
		float num2 = this.volumeRange.GetLerpedValue(tbetween);
		if (float.IsNaN(num2))
		{
			num2 = this.volumeRange.Start;
		}
		this.audioSource.volume = ((this.lerpSpeed <= Mathf.Epsilon) ? num2 : Mathf.Lerp(this.audioSource.volume, num2, num * this.lerpSpeed));
		this.previousLocalPos = a;
	}

	// Token: 0x04000835 RID: 2101
	[SerializeField]
	private Transform target;

	// Token: 0x04000836 RID: 2102
	[SerializeField]
	private MinMaxFloat speedRange;

	// Token: 0x04000837 RID: 2103
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04000838 RID: 2104
	[SerializeField]
	private MinMaxFloat volumeRange = new MinMaxFloat(0f, 1f);

	// Token: 0x04000839 RID: 2105
	[SerializeField]
	private float lerpSpeed;

	// Token: 0x0400083A RID: 2106
	private Vector2 previousLocalPos;

	// Token: 0x0400083B RID: 2107
	private VectorCurveAnimator curveAnimator;

	// Token: 0x0400083C RID: 2108
	private UpdateBatcher updateBatcher;
}
