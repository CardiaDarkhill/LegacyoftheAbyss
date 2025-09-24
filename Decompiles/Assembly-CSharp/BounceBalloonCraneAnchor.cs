using System;
using UnityEngine;

// Token: 0x0200049A RID: 1178
public class BounceBalloonCraneAnchor : MonoBehaviour
{
	// Token: 0x06002A99 RID: 10905 RVA: 0x000B8AE8 File Offset: 0x000B6CE8
	public void SetInactive()
	{
		this.ResetMove();
		if (this.fakeBalloon)
		{
			this.fakeBalloon.enabled = true;
		}
		if (this.realBalloon)
		{
			this.realBalloon.SetDeflated();
			this.realBalloon.gameObject.SetActive(false);
		}
		if (this.realBalloonJoiner)
		{
			this.realBalloonJoiner.SetActive(false);
			if (this.originalJoinerParent)
			{
				this.realBalloonJoiner.transform.SetParent(this.originalJoinerParent, true);
				this.realBalloonJoiner.transform.localPosition = this.originalJoinerPosition;
			}
		}
	}

	// Token: 0x06002A9A RID: 10906 RVA: 0x000B8B90 File Offset: 0x000B6D90
	public void SetActive(bool isInstant)
	{
		this.ResetMove();
		Vector3 ropeScale = Vector3.zero;
		Transform rope = null;
		if (this.realBalloonJoiner)
		{
			this.realBalloonJoiner.SetActive(true);
			if (this.realBalloon)
			{
				if (this.originalJoinerParent == null)
				{
					this.originalJoinerPosition = this.realBalloonJoiner.transform.localPosition;
					this.originalJoinerParent = this.realBalloonJoiner.transform.parent;
				}
				this.realBalloonJoiner.transform.SetParent(this.realBalloon.transform, true);
			}
			rope = this.realBalloonJoiner.transform.GetChild(0);
			if (rope)
			{
				Transform transform = new GameObject("Pivot").transform;
				transform.SetParentReset(this.realBalloonJoiner.transform);
				rope.SetParent(transform, true);
				rope = transform;
				ropeScale = rope.localScale;
			}
		}
		if (isInstant || this.fakeBalloonLerpTime <= 0f || !this.fakeBalloon || !this.realBalloon)
		{
			if (this.realBalloon)
			{
				this.realBalloon.gameObject.SetActive(true);
				if (isInstant)
				{
					this.realBalloon.Opened();
				}
				else
				{
					this.realBalloon.Open();
				}
			}
			if (this.fakeBalloon)
			{
				this.fakeBalloon.enabled = false;
				return;
			}
		}
		else
		{
			Transform transform2 = this.fakeBalloon.transform;
			this.originalFakeBalloonPos = transform2.localPosition;
			Transform realBalloonTrans = this.realBalloon.transform;
			Vector3 fromPos = transform2.position;
			Vector3 toPos = realBalloonTrans.position;
			this.fakeBalloon.enabled = false;
			AmbientFloat floater = this.realBalloon.GetComponentInChildren<AmbientFloat>();
			if (floater)
			{
				floater.enabled = false;
			}
			realBalloonTrans.position = fromPos;
			this.realBalloon.gameObject.SetActive(true);
			this.realBalloon.Open();
			this.moveRoutine = this.StartTimerRoutine(0f, this.fakeBalloonLerpTime, delegate(float time)
			{
				float x = Mathf.LerpUnclamped(fromPos.x, toPos.x, this.fakeBalloonLerpCurveX.Evaluate(time));
				float y = Mathf.LerpUnclamped(fromPos.y, toPos.y, this.fakeBalloonLerpCurveY.Evaluate(time));
				float z = Mathf.LerpUnclamped(fromPos.z, toPos.z, time);
				realBalloonTrans.position = new Vector3(x, y, z);
				if (rope)
				{
					rope.transform.localScale = ropeScale * time;
				}
			}, null, delegate
			{
				realBalloonTrans.position = toPos;
				if (floater)
				{
					floater.enabled = true;
				}
			}, false);
		}
	}

	// Token: 0x06002A9B RID: 10907 RVA: 0x000B8E05 File Offset: 0x000B7005
	private void ResetMove()
	{
		if (this.moveRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this.moveRoutine);
		this.fakeBalloon.transform.localPosition = this.originalFakeBalloonPos;
	}

	// Token: 0x04002B51 RID: 11089
	[SerializeField]
	private SpriteRenderer fakeBalloon;

	// Token: 0x04002B52 RID: 11090
	[SerializeField]
	private BounceBalloon realBalloon;

	// Token: 0x04002B53 RID: 11091
	[SerializeField]
	private GameObject realBalloonJoiner;

	// Token: 0x04002B54 RID: 11092
	[Space]
	[SerializeField]
	private float fakeBalloonLerpTime;

	// Token: 0x04002B55 RID: 11093
	[SerializeField]
	private AnimationCurve fakeBalloonLerpCurveX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002B56 RID: 11094
	[SerializeField]
	private AnimationCurve fakeBalloonLerpCurveY = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002B57 RID: 11095
	private Transform originalJoinerParent;

	// Token: 0x04002B58 RID: 11096
	private Vector3 originalJoinerPosition;

	// Token: 0x04002B59 RID: 11097
	private Vector3 originalFakeBalloonPos;

	// Token: 0x04002B5A RID: 11098
	private Coroutine moveRoutine;
}
