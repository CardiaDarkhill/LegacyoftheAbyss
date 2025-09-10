using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020000C2 RID: 194
public class TiltPlat : MonoBehaviour
{
	// Token: 0x0600061F RID: 1567 RVA: 0x0001F352 File Offset: 0x0001D552
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(this.originOffset, 0.1f);
	}

	// Token: 0x06000620 RID: 1568 RVA: 0x0001F379 File Offset: 0x0001D579
	private void Awake()
	{
		this.collider = base.GetComponent<Collider2D>();
		this.tinkEffect = base.GetComponent<TinkEffect>();
		if (this.tinkEffect)
		{
			this.tinkEffect.HitInDirection += this.OnHitInDirection;
		}
	}

	// Token: 0x06000621 RID: 1569 RVA: 0x0001F3B7 File Offset: 0x0001D5B7
	private void OnEnable()
	{
		if (!this.art)
		{
			base.enabled = false;
		}
	}

	// Token: 0x06000622 RID: 1570 RVA: 0x0001F3D0 File Offset: 0x0001D5D0
	private void Start()
	{
		this.initialArtPos = this.art.localPosition;
		this.initialArtRot = this.art.localRotation;
		Vector3 lossyScale = base.transform.lossyScale;
		this.sign = Mathf.Sign(lossyScale.x) * Mathf.Sign(lossyScale.y);
		if (this.startInactive)
		{
			this.collider.enabled = false;
			this.activeObjects.SetAllActive(false);
		}
	}

	// Token: 0x06000623 RID: 1571 RVA: 0x0001F450 File Offset: 0x0001D650
	private void OnCollisionEnter2D(Collision2D other)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		if (other.gameObject.layer != 9)
		{
			return;
		}
		if (!other.GetSafeContact().Normal.y.IsWithinTolerance(0.01f, -1f))
		{
			return;
		}
		this.heroTrans = other.transform;
		this.StopMoveRoutines();
		this.dropRoutine = base.StartCoroutine(this.ArtDrop());
		this.OnLand.Invoke();
		this.landAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
		if (this.tiltRoutine == null)
		{
			this.tiltRoutine = base.StartCoroutine(this.ArtTilt());
		}
	}

	// Token: 0x06000624 RID: 1572 RVA: 0x0001F4FC File Offset: 0x0001D6FC
	private void OnCollisionExit2D(Collision2D other)
	{
		if (!this.heroTrans || other.transform != this.heroTrans)
		{
			return;
		}
		this.heroTrans = null;
		this.StopMoveRoutines();
		this.dropRoutine = base.StartCoroutine(this.ArtRaise());
	}

	// Token: 0x06000625 RID: 1573 RVA: 0x0001F549 File Offset: 0x0001D749
	private void StopMoveRoutines()
	{
		if (this.dropRoutine != null)
		{
			base.StopCoroutine(this.dropRoutine);
		}
		if (this.tinkMoveRoutine != null)
		{
			base.StopCoroutine(this.tinkMoveRoutine);
		}
	}

	// Token: 0x06000626 RID: 1574 RVA: 0x0001F573 File Offset: 0x0001D773
	private IEnumerator ArtDrop()
	{
		Vector2 targetPos = this.initialArtPos + this.dropVector;
		for (float elapsed = 0f; elapsed < 0.075f; elapsed += Time.deltaTime)
		{
			Vector2 position = Vector2.Lerp(this.initialArtPos, targetPos, elapsed / 0.075f);
			this.art.SetLocalPosition2D(position);
			yield return null;
		}
		this.art.SetLocalPosition2D(targetPos);
		yield break;
	}

	// Token: 0x06000627 RID: 1575 RVA: 0x0001F582 File Offset: 0x0001D782
	private IEnumerator ArtRaise()
	{
		Vector2 startPos = this.art.localPosition;
		for (float elapsed = 0f; elapsed < 0.2f; elapsed += Time.deltaTime)
		{
			Vector2 position = Vector2.Lerp(startPos, this.initialArtPos, elapsed / 0.2f);
			this.art.SetLocalPosition2D(position);
			yield return null;
		}
		this.art.SetLocalPosition2D(this.initialArtPos);
		yield break;
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x0001F591 File Offset: 0x0001D791
	private IEnumerator ArtTilt()
	{
		this.SetArtTilt(0f);
		WaitForSeconds wait = new WaitForSeconds(0.055555556f);
		float previousTilt = 0f;
		double nextCreakTime = 0.0;
		for (;;)
		{
			float num;
			if (this.heroTrans)
			{
				float x = this.heroTrans.position.x;
				num = base.transform.TransformPoint(this.originOffset).x - x;
				num *= this.tiltFactor * this.sign;
			}
			else
			{
				num = Mathf.Lerp(previousTilt, 0f, 0.44444445f);
				if (Math.Abs(num) < 0.001f)
				{
					break;
				}
			}
			if (Math.Abs(num - previousTilt) > 0.001f && Time.timeAsDouble >= nextCreakTime)
			{
				this.creakAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
				nextCreakTime = Time.timeAsDouble + 1.5;
			}
			this.SetArtTilt(num);
			previousTilt = num;
			yield return wait;
		}
		this.SetArtTilt(0f);
		this.tiltRoutine = null;
		yield break;
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x0001F5A0 File Offset: 0x0001D7A0
	private void SetArtTilt(float tilt)
	{
		this.art.localRotation = this.initialArtRot * Quaternion.Euler(0f, 0f, tilt);
	}

	// Token: 0x0600062A RID: 1578 RVA: 0x0001F5C8 File Offset: 0x0001D7C8
	public void ActivateTiltPlat(bool isInstant)
	{
		this.startInactive = false;
		this.collider.enabled = true;
		this.activeObjects.SetAllActive(true);
		if (!isInstant)
		{
			this.OnLand.Invoke();
			this.art.SetLocalPosition2D(this.initialArtPos + this.dropVector);
			if (this.dropRoutine != null)
			{
				base.StopCoroutine(this.dropRoutine);
			}
			this.dropRoutine = base.StartCoroutine(this.ArtRaise());
		}
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x0001F644 File Offset: 0x0001D844
	private void OnHitInDirection(GameObject source, HitInstance.HitDirection direction)
	{
		this.StopMoveRoutines();
		switch (direction)
		{
		case HitInstance.HitDirection.Left:
			this.tinkMoveRoutine = base.StartCoroutine(this.TinkDirectionMove(new Vector2(-0.1f, 0f)));
			break;
		case HitInstance.HitDirection.Right:
			this.tinkMoveRoutine = base.StartCoroutine(this.TinkDirectionMove(new Vector2(0.1f, 0f)));
			break;
		case HitInstance.HitDirection.Up:
			this.tinkMoveRoutine = base.StartCoroutine(this.TinkDirectionMove(new Vector2(0f, 0.1f)));
			break;
		default:
			return;
		}
		if (this.platDust)
		{
			this.platDust.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
			this.platDust.Play(true);
		}
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x0001F6F7 File Offset: 0x0001D8F7
	private IEnumerator TinkDirectionMove(Vector2 moveVec)
	{
		Vector2 targetPos = this.initialArtPos + moveVec;
		for (float elapsed = 0f; elapsed < 0.05f; elapsed += Time.deltaTime)
		{
			Vector2 position = Vector2.Lerp(this.initialArtPos, targetPos, elapsed / 0.05f);
			this.art.SetLocalPosition2D(position);
			yield return null;
		}
		for (float elapsed = 0f; elapsed < 0.1f; elapsed += Time.deltaTime)
		{
			Vector2 position2 = Vector2.Lerp(targetPos, this.initialArtPos, elapsed / 0.1f);
			this.art.SetLocalPosition2D(position2);
			yield return null;
		}
		this.art.SetLocalPosition2D(this.initialArtPos);
		yield break;
	}

	// Token: 0x040005E6 RID: 1510
	[Header("Basic")]
	[SerializeField]
	private Vector2 originOffset;

	// Token: 0x040005E7 RID: 1511
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation]
	private Transform art;

	// Token: 0x040005E8 RID: 1512
	[SerializeField]
	private Vector2 dropVector;

	// Token: 0x040005E9 RID: 1513
	[SerializeField]
	private float tiltFactor;

	// Token: 0x040005EA RID: 1514
	[SerializeField]
	private ParticleSystem platDust;

	// Token: 0x040005EB RID: 1515
	[SerializeField]
	private RandomAudioClipTable landAudioClipTable;

	// Token: 0x040005EC RID: 1516
	[SerializeField]
	private RandomAudioClipTable creakAudioClipTable;

	// Token: 0x040005ED RID: 1517
	[Header("Extras")]
	[SerializeField]
	private GameObject[] activeObjects;

	// Token: 0x040005EE RID: 1518
	[SerializeField]
	private bool startInactive;

	// Token: 0x040005EF RID: 1519
	[Space]
	[SerializeField]
	private UnityEvent OnLand;

	// Token: 0x040005F0 RID: 1520
	private Collider2D collider;

	// Token: 0x040005F1 RID: 1521
	private Transform heroTrans;

	// Token: 0x040005F2 RID: 1522
	private Vector2 initialArtPos;

	// Token: 0x040005F3 RID: 1523
	private Quaternion initialArtRot;

	// Token: 0x040005F4 RID: 1524
	private float sign;

	// Token: 0x040005F5 RID: 1525
	private float nextUpdateTime;

	// Token: 0x040005F6 RID: 1526
	private Coroutine dropRoutine;

	// Token: 0x040005F7 RID: 1527
	private Coroutine tinkMoveRoutine;

	// Token: 0x040005F8 RID: 1528
	private Coroutine tiltRoutine;

	// Token: 0x040005F9 RID: 1529
	private TinkEffect tinkEffect;
}
