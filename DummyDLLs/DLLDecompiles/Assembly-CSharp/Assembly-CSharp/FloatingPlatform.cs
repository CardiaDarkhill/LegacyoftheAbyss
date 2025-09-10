using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004E1 RID: 1249
public class FloatingPlatform : MonoBehaviour
{
	// Token: 0x06002CD2 RID: 11474 RVA: 0x000C40BC File Offset: 0x000C22BC
	private void Start()
	{
		this.overallSinOffset = Random.Range(0f, 1f);
		this.sinBlend = 1f;
		this.initialObjectPositions = new Vector3[this.moveObjects.Length];
		for (int i = 0; i < this.moveObjects.Length; i++)
		{
			if (!(this.moveObjects[i] == null))
			{
				this.initialObjectPositions[i] = this.moveObjects[i].localPosition;
			}
		}
	}

	// Token: 0x06002CD3 RID: 11475 RVA: 0x000C4138 File Offset: 0x000C2338
	private void Update()
	{
		if (Mathf.Abs(this.moveAmount) <= 0f)
		{
			return;
		}
		for (int i = 0; i < this.moveObjects.Length; i++)
		{
			Transform transform = this.moveObjects[i];
			if (!(transform == null))
			{
				float num = this.maxSinOffset * ((float)(i + 1) / (float)this.moveObjects.Length) + this.overallSinOffset;
				float num2 = this.moveAmount * this.sinBlend;
				transform.localPosition = this.initialObjectPositions[i] + (Vector3.up * (num2 * Mathf.Sin((Time.time + num) * this.moveSpeed)) + Vector3.down * this.landOffset) + Vector3.up * num2;
			}
		}
	}

	// Token: 0x06002CD4 RID: 11476 RVA: 0x000C420C File Offset: 0x000C240C
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!collision.collider.CompareTag("Player"))
		{
			return;
		}
		Collision2DUtils.Collision2DSafeContact safeContact = collision.GetSafeContact();
		if (safeContact.Point.y < collision.otherCollider.bounds.max.y)
		{
			return;
		}
		if (!safeContact.IsLegitimate)
		{
			Debug.LogWarning("Platform contact point was not legitimate! (dang it, Unity D:)", this);
		}
		this.isHeroOnTop = true;
		this.OnLanded.Invoke();
		if (this.landReactRoutine != null)
		{
			base.StopCoroutine(this.landReactRoutine);
		}
		this.landReactRoutine = base.StartCoroutine(this.LandPush());
		if (this.landSinRoutine != null)
		{
			base.StopCoroutine(this.landSinRoutine);
		}
		this.landSinRoutine = base.StartCoroutine(this.LandSinState(false));
	}

	// Token: 0x06002CD5 RID: 11477 RVA: 0x000C42CC File Offset: 0x000C24CC
	private void OnCollisionExit2D(Collision2D collision)
	{
		if (!this.isHeroOnTop || !collision.collider.CompareTag("Player"))
		{
			return;
		}
		if (this.landSinRoutine != null)
		{
			base.StopCoroutine(this.landSinRoutine);
		}
		this.landSinRoutine = base.StartCoroutine(this.LandSinState(true));
	}

	// Token: 0x06002CD6 RID: 11478 RVA: 0x000C431B File Offset: 0x000C251B
	private IEnumerator LandPush()
	{
		float elapsed = 0f;
		ParticleSystem[] array = this.landParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		while (elapsed < this.landMoveLength)
		{
			this.landOffset = this.landCurve.Evaluate(elapsed / this.landMoveLength) * this.landMoveAmount;
			yield return null;
			elapsed += Time.deltaTime;
		}
		this.landOffset = 0f;
		yield break;
	}

	// Token: 0x06002CD7 RID: 11479 RVA: 0x000C432A File Offset: 0x000C252A
	private IEnumerator LandSinState(bool enableSinFloat)
	{
		float elapsed = 0f;
		float startSinBlend = this.sinBlend;
		float targetSinBlend;
		float duration;
		if (enableSinFloat)
		{
			targetSinBlend = 1f;
			duration = this.landSinStartTime;
		}
		else
		{
			targetSinBlend = 0f;
			duration = this.landSinStopTime;
		}
		while (elapsed < duration)
		{
			this.sinBlend = Mathf.Lerp(startSinBlend, targetSinBlend, elapsed / duration);
			yield return null;
			elapsed += Time.deltaTime;
		}
		this.sinBlend = targetSinBlend;
		yield break;
	}

	// Token: 0x04002E77 RID: 11895
	[SerializeField]
	private Transform[] moveObjects;

	// Token: 0x04002E78 RID: 11896
	[Space]
	[SerializeField]
	private float moveAmount = 0.1f;

	// Token: 0x04002E79 RID: 11897
	[SerializeField]
	private float moveSpeed = 0.05f;

	// Token: 0x04002E7A RID: 11898
	[SerializeField]
	private float maxSinOffset = 0.1f;

	// Token: 0x04002E7B RID: 11899
	[Space]
	[SerializeField]
	private float landSinStopTime;

	// Token: 0x04002E7C RID: 11900
	[SerializeField]
	private float landSinStartTime;

	// Token: 0x04002E7D RID: 11901
	[Space]
	[SerializeField]
	private float landMoveAmount = 0.1f;

	// Token: 0x04002E7E RID: 11902
	[SerializeField]
	private float landMoveLength = 0.5f;

	// Token: 0x04002E7F RID: 11903
	[SerializeField]
	private AnimationCurve landCurve;

	// Token: 0x04002E80 RID: 11904
	[SerializeField]
	private ParticleSystem[] landParticles;

	// Token: 0x04002E81 RID: 11905
	[Space]
	[SerializeField]
	private UnityEvent OnLanded;

	// Token: 0x04002E82 RID: 11906
	private float overallSinOffset;

	// Token: 0x04002E83 RID: 11907
	private float sinBlend;

	// Token: 0x04002E84 RID: 11908
	private float landOffset;

	// Token: 0x04002E85 RID: 11909
	private Vector3[] initialObjectPositions;

	// Token: 0x04002E86 RID: 11910
	private bool isHeroOnTop;

	// Token: 0x04002E87 RID: 11911
	private Coroutine landReactRoutine;

	// Token: 0x04002E88 RID: 11912
	private Coroutine landSinRoutine;
}
