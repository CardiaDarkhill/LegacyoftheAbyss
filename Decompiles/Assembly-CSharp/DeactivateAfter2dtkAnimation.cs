using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200007E RID: 126
public class DeactivateAfter2dtkAnimation : MonoBehaviour
{
	// Token: 0x06000382 RID: 898 RVA: 0x00012038 File Offset: 0x00010238
	private void OnValidate()
	{
		if (this.spriteAnimator)
		{
			this.animators.Add(this.spriteAnimator);
			this.spriteAnimator = null;
		}
	}

	// Token: 0x06000383 RID: 899 RVA: 0x00012060 File Offset: 0x00010260
	private void Awake()
	{
		this.OnValidate();
		Transform transform = base.transform;
		this.startPos = transform.localPosition;
		this.startRotation = transform.localEulerAngles.z;
	}

	// Token: 0x06000384 RID: 900 RVA: 0x00012098 File Offset: 0x00010298
	private void OnEnable()
	{
		this.timer = 0f;
		if (this.animators.Count <= 0)
		{
			tk2dSpriteAnimator component = base.GetComponent<tk2dSpriteAnimator>();
			if (component)
			{
				this.animators.Add(component);
			}
		}
		foreach (tk2dSpriteAnimator tk2dSpriteAnimator in this.animators)
		{
			tk2dSpriteAnimator.PlayFromFrame(0);
		}
		Transform transform = base.transform;
		if (this.stayInPlace)
		{
			transform.localPosition = this.startPos;
			transform.localEulerAngles = new Vector3(0f, 0f, this.startRotation);
			this.queuedStayInPlaceUpdate = true;
		}
		bool flag = this.randomlyFlipXScale && (float)Random.Range(1, 100) > 50f;
		bool flag2 = this.randomlyFlipYScale && (float)Random.Range(1, 100) > 50f;
		if (flag || flag2)
		{
			transform.FlipLocalScale(flag, flag2, false);
		}
		if (this.keepScaleSign)
		{
			this.lossyScaleSign = transform.lossyScale.GetSign();
			this.localSign = transform.localScale.GetSign();
		}
	}

	// Token: 0x06000385 RID: 901 RVA: 0x000121CC File Offset: 0x000103CC
	private void OnDisable()
	{
		if (this.keepScaleSign)
		{
			this.localSign = this.localSign.GetSign();
			Transform transform = base.transform;
			Vector3 sign = transform.localScale.GetSign();
			Vector3 localScale = transform.localScale;
			if (sign.x != this.localSign.x)
			{
				localScale.x *= -1f;
			}
			if (sign.y != this.localSign.y)
			{
				localScale.y *= -1f;
			}
			if (sign.z != this.localSign.z)
			{
				localScale.z *= -1f;
			}
			transform.localScale = localScale;
		}
	}

	// Token: 0x06000386 RID: 902 RVA: 0x00012280 File Offset: 0x00010480
	private void Update()
	{
		Transform transform = base.transform;
		if (this.queuedStayInPlaceUpdate)
		{
			this.queuedStayInPlaceUpdate = false;
			this.worldPos = transform.position;
			this.worldRotation = transform.eulerAngles.z;
		}
		if (this.timer > 0.1f)
		{
			this.timer -= Time.deltaTime;
		}
		else if (this.animators.All((tk2dSpriteAnimator anim) => !anim.Playing))
		{
			if (!this.deactivateMeshRendererInstead)
			{
				base.gameObject.SetActive(false);
				return;
			}
			base.gameObject.GetComponent<MeshRenderer>().enabled = false;
			return;
		}
		if (this.stayInPlace)
		{
			transform.position = this.worldPos;
			transform.eulerAngles = new Vector3(0f, 0f, this.worldRotation);
		}
		if (this.matchParentScale)
		{
			this.parentXScale = this.myParent.localScale.x;
			if ((this.parentXScale < 0f && transform.lossyScale.x > 0f) || (this.parentXScale > 0f && transform.lossyScale.x < 0f))
			{
				transform.FlipLocalScale(true, false, false);
			}
		}
		if (this.keepScaleSign)
		{
			Vector3 sign = transform.lossyScale.GetSign();
			Vector3 localScale = transform.localScale;
			if (sign.x != this.lossyScaleSign.x)
			{
				localScale.x *= -1f;
			}
			if (sign.y != this.lossyScaleSign.y)
			{
				localScale.y *= -1f;
			}
			if (sign.z != this.lossyScaleSign.z)
			{
				localScale.z *= -1f;
			}
			transform.localScale = localScale;
		}
	}

	// Token: 0x04000325 RID: 805
	[SerializeField]
	private List<tk2dSpriteAnimator> animators;

	// Token: 0x04000326 RID: 806
	[SerializeField]
	private bool stayInPlace;

	// Token: 0x04000327 RID: 807
	[SerializeField]
	private bool matchParentScale;

	// Token: 0x04000328 RID: 808
	[SerializeField]
	private bool randomlyFlipXScale;

	// Token: 0x04000329 RID: 809
	[SerializeField]
	private bool randomlyFlipYScale;

	// Token: 0x0400032A RID: 810
	[SerializeField]
	private bool keepScaleSign;

	// Token: 0x0400032B RID: 811
	[SerializeField]
	private Transform myParent;

	// Token: 0x0400032C RID: 812
	[SerializeField]
	private bool deactivateMeshRendererInstead;

	// Token: 0x0400032D RID: 813
	[SerializeField]
	[HideInInspector]
	[Obsolete("Old serialized field, use \"animators\" instead.")]
	private tk2dSpriteAnimator spriteAnimator;

	// Token: 0x0400032E RID: 814
	private float timer;

	// Token: 0x0400032F RID: 815
	private Vector3 worldPos;

	// Token: 0x04000330 RID: 816
	private Vector3 startPos;

	// Token: 0x04000331 RID: 817
	private float startRotation;

	// Token: 0x04000332 RID: 818
	private float worldRotation;

	// Token: 0x04000333 RID: 819
	private float parentXScale;

	// Token: 0x04000334 RID: 820
	private bool queuedStayInPlaceUpdate;

	// Token: 0x04000335 RID: 821
	private Vector3 lossyScaleSign;

	// Token: 0x04000336 RID: 822
	private Vector3 localSign;
}
