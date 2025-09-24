using System;
using UnityEngine;

// Token: 0x020004E3 RID: 1251
public class FollowRotation : MonoBehaviour
{
	// Token: 0x1700051E RID: 1310
	// (get) Token: 0x06002CDE RID: 11486 RVA: 0x000C43CC File Offset: 0x000C25CC
	// (set) Token: 0x06002CDF RID: 11487 RVA: 0x000C43D4 File Offset: 0x000C25D4
	public Transform Target
	{
		get
		{
			return this.target;
		}
		set
		{
			this.target = value;
			this.currentRotation = this.target.rotation;
		}
	}

	// Token: 0x1700051F RID: 1311
	// (get) Token: 0x06002CE0 RID: 11488 RVA: 0x000C43F0 File Offset: 0x000C25F0
	public Transform CurrentTarget
	{
		get
		{
			if (this.target && this.target.gameObject.activeInHierarchy)
			{
				return this.target;
			}
			if (this.fallbackTargets != null)
			{
				foreach (Transform transform in this.fallbackTargets)
				{
					if (transform && transform.gameObject.activeInHierarchy)
					{
						return transform;
					}
				}
			}
			return null;
		}
	}

	// Token: 0x06002CE1 RID: 11489 RVA: 0x000C445C File Offset: 0x000C265C
	private void OnValidate()
	{
		if (this.lerpSpeed < 0f)
		{
			this.lerpSpeed = 0f;
		}
		if (this.fpsLimit < 0f)
		{
			this.fpsLimit = 0f;
		}
	}

	// Token: 0x06002CE2 RID: 11490 RVA: 0x000C448E File Offset: 0x000C268E
	private void OnEnable()
	{
		this.currentRotation = base.transform.rotation;
	}

	// Token: 0x06002CE3 RID: 11491 RVA: 0x000C44A4 File Offset: 0x000C26A4
	private void LateUpdate()
	{
		Transform currentTarget = this.CurrentTarget;
		if (!currentTarget)
		{
			return;
		}
		Quaternion quaternion = currentTarget.rotation * Quaternion.Euler(0f, 0f, this.offset);
		this.currentRotation = ((this.lerpSpeed <= Mathf.Epsilon) ? quaternion : Quaternion.Lerp(this.currentRotation, quaternion, this.lerpSpeed * Time.deltaTime));
		if (this.fpsLimit > 0f)
		{
			if (Time.timeAsDouble < this.nextUpdateTime)
			{
				return;
			}
			this.nextUpdateTime = Time.timeAsDouble + (double)(1f / this.fpsLimit);
		}
		base.transform.rotation = this.currentRotation;
	}

	// Token: 0x04002E8B RID: 11915
	[SerializeField]
	private Transform target;

	// Token: 0x04002E8C RID: 11916
	[SerializeField]
	private Transform[] fallbackTargets;

	// Token: 0x04002E8D RID: 11917
	[SerializeField]
	private float offset;

	// Token: 0x04002E8E RID: 11918
	[SerializeField]
	private float lerpSpeed;

	// Token: 0x04002E8F RID: 11919
	[SerializeField]
	private float fpsLimit;

	// Token: 0x04002E90 RID: 11920
	private double nextUpdateTime;

	// Token: 0x04002E91 RID: 11921
	private Quaternion currentRotation;
}
