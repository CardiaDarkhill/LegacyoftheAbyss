using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200055F RID: 1375
public class StaffFallOver : MonoBehaviour
{
	// Token: 0x0600311C RID: 12572 RVA: 0x000D9F09 File Offset: 0x000D8109
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x0600311D RID: 12573 RVA: 0x000D9F18 File Offset: 0x000D8118
	private void OnEnable()
	{
		this.front.SetActive(true);
		this.frontCollider.SetActive(true);
		this.side.SetActive(false);
		this.sideCollider.SetActive(false);
		this.frontRotator.localEulerAngles = Vector3.zero;
	}

	// Token: 0x0600311E RID: 12574 RVA: 0x000D9F65 File Offset: 0x000D8165
	private void OnDisable()
	{
		if (this.fallRoutine != null)
		{
			base.StopCoroutine(this.fallRoutine);
			this.fallRoutine = null;
		}
	}

	// Token: 0x0600311F RID: 12575 RVA: 0x000D9F82 File Offset: 0x000D8182
	private void OnCollisionEnter2D()
	{
		if (this.fallRoutine == null)
		{
			this.fallRoutine = base.StartCoroutine(this.Fall());
		}
	}

	// Token: 0x06003120 RID: 12576 RVA: 0x000D9F9E File Offset: 0x000D819E
	private IEnumerator Fall()
	{
		WaitForFixedUpdate fixedWait = new WaitForFixedUpdate();
		float elapsedStill = 0f;
		while (elapsedStill < this.landWaitTime)
		{
			yield return fixedWait;
			float magnitude = this.body.linearVelocity.magnitude;
			if (Mathf.Abs(this.body.angularVelocity) > this.landAngularThreshold || magnitude > this.landSpeedThreshold)
			{
				elapsedStill = 0f;
			}
			else
			{
				elapsedStill += Time.fixedDeltaTime;
			}
		}
		this.frontCollider.SetActive(false);
		this.sideCollider.SetActive(true);
		float targetRotation = (base.transform.localEulerAngles.z < 0f) ? 90f : -90f;
		for (float elapsed = 0f; elapsed < this.fallDuration; elapsed += Time.deltaTime)
		{
			float t = this.fallCurve.Evaluate(elapsed / this.fallDuration);
			float y = Mathf.Lerp(0f, targetRotation, t);
			this.frontRotator.localEulerAngles = new Vector3(0f, y, 0f);
			yield return null;
		}
		this.front.SetActive(false);
		this.side.SetActive(true);
		this.body.AddForce(new Vector2(0f, this.bounceUpForce), ForceMode2D.Impulse);
		yield break;
	}

	// Token: 0x0400346B RID: 13419
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private GameObject front;

	// Token: 0x0400346C RID: 13420
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Transform frontRotator;

	// Token: 0x0400346D RID: 13421
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private GameObject frontCollider;

	// Token: 0x0400346E RID: 13422
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private GameObject side;

	// Token: 0x0400346F RID: 13423
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private GameObject sideCollider;

	// Token: 0x04003470 RID: 13424
	[SerializeField]
	private float landSpeedThreshold;

	// Token: 0x04003471 RID: 13425
	[SerializeField]
	private float landAngularThreshold;

	// Token: 0x04003472 RID: 13426
	[SerializeField]
	private float landWaitTime;

	// Token: 0x04003473 RID: 13427
	[SerializeField]
	private float fallDuration;

	// Token: 0x04003474 RID: 13428
	[SerializeField]
	private AnimationCurve fallCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04003475 RID: 13429
	[SerializeField]
	private float bounceUpForce;

	// Token: 0x04003476 RID: 13430
	private Coroutine fallRoutine;

	// Token: 0x04003477 RID: 13431
	private Rigidbody2D body;
}
