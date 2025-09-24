using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.NestedFadeGroup;
using TeamCherry.Splines;
using UnityEngine;

// Token: 0x020000B5 RID: 181
public class SplineDepress : MonoBehaviour
{
	// Token: 0x06000549 RID: 1353 RVA: 0x0001ADAF File Offset: 0x00018FAF
	private void Awake()
	{
		this.spline.UpdateCondition = SplineBase.UpdateConditions.Manual;
		this.initialFollowerPos = this.follower.localPosition;
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x0001ADD4 File Offset: 0x00018FD4
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.GetSafeContact().Normal.y >= 0f)
		{
			return;
		}
		this.touching.AddIfNotPresent(other.transform);
		if (this.touching.Count == 1)
		{
			if (this.followRoutine != null)
			{
				base.StopCoroutine(this.followRoutine);
			}
			this.followRoutine = base.StartCoroutine(this.FollowRoutine());
		}
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x0001AE3F File Offset: 0x0001903F
	private void OnCollisionExit2D(Collision2D other)
	{
		this.touching.Remove(other.transform);
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x0001AE53 File Offset: 0x00019053
	private IEnumerator FollowRoutine()
	{
		this.follower.SetLocalPosition2D(this.initialFollowerPos + this.depressDistance);
		float walkCreakTimeLeft = this.walkCreakHoldTime;
		float previousCreakX = 0f;
		bool isCreaking = true;
		this.walkCreakLoop.AlphaSelf = 1f;
		do
		{
			Vector2 vector = Vector2.zero;
			foreach (Transform transform in this.touching)
			{
				vector += transform.position;
			}
			vector /= (float)this.touching.Count;
			this.follower.SetPositionX(vector.x);
			this.spline.UpdateSpline();
			if (Mathf.Abs(vector.x - previousCreakX) > 0.001f)
			{
				previousCreakX = vector.x;
				walkCreakTimeLeft = this.walkCreakHoldTime;
				if (!isCreaking)
				{
					isCreaking = true;
					this.walkCreakLoop.FadeToOne(this.walkCreakFadeUpTime);
				}
			}
			else if (isCreaking)
			{
				walkCreakTimeLeft -= Time.deltaTime;
				if (walkCreakTimeLeft < 0f)
				{
					isCreaking = false;
					this.walkCreakLoop.FadeToZero(this.walkCreakFadeDownTime);
				}
			}
			yield return null;
		}
		while (this.touching.Count != 0);
		this.walkCreakLoop.FadeToZero(this.walkCreakFadeDownTime);
		float elapsed = 0f;
		Vector2 fromPos = this.follower.localPosition;
		while (elapsed < this.releaseDuration)
		{
			float t = this.releaseCurve.Evaluate(elapsed / this.releaseDuration);
			Vector2 position = Vector2.Lerp(fromPos, this.initialFollowerPos, t);
			this.follower.SetLocalPosition2D(position);
			this.spline.UpdateSpline();
			yield return null;
			elapsed += Time.deltaTime;
		}
		this.follower.SetLocalPosition2D(this.initialFollowerPos);
		this.spline.UpdateSpline();
		this.followRoutine = null;
		yield break;
	}

	// Token: 0x0400051F RID: 1311
	[SerializeField]
	private SplineBase spline;

	// Token: 0x04000520 RID: 1312
	[SerializeField]
	private Transform follower;

	// Token: 0x04000521 RID: 1313
	[SerializeField]
	private Vector2 depressDistance;

	// Token: 0x04000522 RID: 1314
	[SerializeField]
	private float releaseDuration;

	// Token: 0x04000523 RID: 1315
	[SerializeField]
	private AnimationCurve releaseCurve;

	// Token: 0x04000524 RID: 1316
	[Space]
	[SerializeField]
	private NestedFadeGroupFloatEvent walkCreakLoop;

	// Token: 0x04000525 RID: 1317
	[SerializeField]
	private float walkCreakFadeUpTime;

	// Token: 0x04000526 RID: 1318
	[SerializeField]
	private float walkCreakHoldTime;

	// Token: 0x04000527 RID: 1319
	[SerializeField]
	private float walkCreakFadeDownTime;

	// Token: 0x04000528 RID: 1320
	private Vector2 initialFollowerPos;

	// Token: 0x04000529 RID: 1321
	private Coroutine followRoutine;

	// Token: 0x0400052A RID: 1322
	private readonly List<Transform> touching = new List<Transform>();
}
