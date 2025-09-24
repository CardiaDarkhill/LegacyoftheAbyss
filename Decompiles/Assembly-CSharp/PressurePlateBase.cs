using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000530 RID: 1328
public abstract class PressurePlateBase : EventBase
{
	// Token: 0x17000546 RID: 1350
	// (get) Token: 0x06002FA2 RID: 12194
	protected abstract bool CanDepress { get; }

	// Token: 0x17000547 RID: 1351
	// (get) Token: 0x06002FA3 RID: 12195 RVA: 0x000D1C0F File Offset: 0x000CFE0F
	public float GateOpenDelay
	{
		get
		{
			return this.gateOpenDelay;
		}
	}

	// Token: 0x17000548 RID: 1352
	// (get) Token: 0x06002FA4 RID: 12196 RVA: 0x000D1C17 File Offset: 0x000CFE17
	public override string InspectorInfo
	{
		get
		{
			return string.Format("Pressure Plate ({0})", base.gameObject.name);
		}
	}

	// Token: 0x06002FA5 RID: 12197 RVA: 0x000D1C30 File Offset: 0x000CFE30
	protected override void Awake()
	{
		base.Awake();
		this.col = base.GetComponent<Collider2D>();
		this.source = base.GetComponent<AudioSource>();
		this.initialPos = (this.plateGraphic ? this.plateGraphic.transform.localPosition : Vector3.zero);
		if (this.plateUpMaterial && this.plateGraphic)
		{
			this.plateGraphic.sharedMaterial = this.plateUpMaterial;
		}
		if (this.touchParticles)
		{
			this.touchParticles.SetActive(false);
		}
		if (this.endParticles)
		{
			this.endParticles.SetActive(false);
		}
		if (this.dropOnTrigger)
		{
			this.dropOnTrigger.OnTriggerEntered += delegate(Collider2D other, GameObject sender)
			{
				this.OnTouchStart(other.gameObject);
			};
			this.dropOnTrigger.OnTriggerExited += delegate(Collider2D other, GameObject sender)
			{
				this.OnTouchEnd(other.gameObject);
			};
		}
	}

	// Token: 0x06002FA6 RID: 12198 RVA: 0x000D1D20 File Offset: 0x000CFF20
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (this.dropOnTrigger)
		{
			return;
		}
		if (!this.CanDepress || !collision.collider.CompareTag("Player"))
		{
			return;
		}
		if (Time.fixedTime <= this.forceTouchedTime)
		{
			HeroController.instance.BounceShort();
			return;
		}
		Collision2DUtils.Collision2DSafeContact safeContact = collision.GetSafeContact();
		Vector2 point = safeContact.Point;
		Bounds bounds = this.col.bounds;
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		if (point.y < max.y || point.x < min.x || point.x > max.x)
		{
			bool flag = false;
			if (safeContact.IsLegitimate)
			{
				flag = (safeContact.Normal == Vector2.up);
				if (!flag)
				{
					Bounds bounds2 = collision.collider.bounds;
					Vector3 max2 = bounds2.max;
					Vector3 min2 = bounds2.min;
					flag = (min2.y >= max.y && ((max2.x > min.x && max2.x < max.x) || (min2.x > min.x && min2.x < max.x)));
				}
			}
			if (!flag)
			{
				return;
			}
		}
		if (!safeContact.IsLegitimate)
		{
			Debug.LogWarning("Pressure Plate contact point was not legitimate! (dang it, Unity D:)", this);
		}
		this.OnTouchStart(collision.gameObject);
	}

	// Token: 0x06002FA7 RID: 12199 RVA: 0x000D1E88 File Offset: 0x000D0088
	private void OnCollisionExit2D(Collision2D collision)
	{
		if (this.dropOnTrigger)
		{
			return;
		}
		this.OnTouchEnd(collision.gameObject);
	}

	// Token: 0x06002FA8 RID: 12200 RVA: 0x000D1EA4 File Offset: 0x000D00A4
	private void OnTouchStart(GameObject touchingPlayer)
	{
		this.player = touchingPlayer;
		this.ResetTouched();
		this.isTouched = true;
		if (this.plateGraphic)
		{
			this.plateGraphic.transform.localPosition += new Vector3(0f, -this.touchOffset, 0f);
		}
		if (this.touchParticles)
		{
			this.touchParticles.SetActive(true);
		}
		this.PlaySound(this.landSound);
		this.PlayVibration(this.landVibration);
		this.StartDrop(false);
	}

	// Token: 0x06002FA9 RID: 12201 RVA: 0x000D1F3B File Offset: 0x000D013B
	private void OnTouchEnd(GameObject touchingPlayer)
	{
		if (this.player == touchingPlayer)
		{
			this.player = null;
		}
	}

	// Token: 0x06002FAA RID: 12202 RVA: 0x000D1F52 File Offset: 0x000D0152
	protected void StartDrop(bool force)
	{
		if (this.moveRoutine != null)
		{
			base.StopCoroutine(this.moveRoutine);
		}
		this.moveRoutine = base.StartCoroutine(this.Drop(force));
	}

	// Token: 0x06002FAB RID: 12203 RVA: 0x000D1F7B File Offset: 0x000D017B
	protected void StartRaise(float raiseDelay)
	{
		if (this.moveRoutine != null)
		{
			base.StopCoroutine(this.moveRoutine);
		}
		this.moveRoutine = base.StartCoroutine(this.Raise(raiseDelay, false));
	}

	// Token: 0x06002FAC RID: 12204 RVA: 0x000D1FA5 File Offset: 0x000D01A5
	protected void StartRaiseSilent(float raiseDelay)
	{
		if (this.moveRoutine != null)
		{
			base.StopCoroutine(this.moveRoutine);
		}
		this.moveRoutine = base.StartCoroutine(this.Raise(raiseDelay, true));
	}

	// Token: 0x06002FAD RID: 12205 RVA: 0x000D1FD0 File Offset: 0x000D01D0
	protected void SetDepressed()
	{
		if (this.plateGraphic)
		{
			this.plateGraphic.transform.localPosition = this.initialPos + new Vector3(0f, -this.dropDistance, 0f);
			if (this.plateDownMaterial)
			{
				this.plateGraphic.sharedMaterial = this.plateDownMaterial;
			}
		}
		this.col.enabled = false;
	}

	// Token: 0x06002FAE RID: 12206 RVA: 0x000D2048 File Offset: 0x000D0248
	private void ResetTouched()
	{
		if (!this.isTouched)
		{
			return;
		}
		this.isTouched = false;
		if (this.plateGraphic)
		{
			this.plateGraphic.transform.localPosition -= Vector3.down * this.touchOffset;
		}
		if (this.touchParticles)
		{
			this.touchParticles.SetActive(false);
		}
		if (this.source.clip == this.landSound)
		{
			this.source.Stop();
			this.source.clip = null;
		}
	}

	// Token: 0x06002FAF RID: 12207 RVA: 0x000D20E5 File Offset: 0x000D02E5
	private IEnumerator Drop(bool force)
	{
		if (!force)
		{
			yield return new WaitForSeconds(this.waitTime);
			if (!this.player)
			{
				this.ResetTouched();
				yield break;
			}
			this.PlaySound(this.dropSound);
			this.PlayVibration(this.dropVibration);
		}
		this.col.enabled = false;
		if (this.plateGraphic)
		{
			Vector3 targetPos = this.initialPos + new Vector3(0f, -this.dropDistance, 0f);
			for (float elapsed = 0f; elapsed <= this.dropTime; elapsed += Time.deltaTime)
			{
				this.plateGraphic.transform.localPosition = Vector3.Lerp(this.initialPos, targetPos, elapsed / this.dropTime);
				yield return null;
			}
			this.plateGraphic.transform.localPosition = targetPos;
			if (this.plateDownMaterial)
			{
				this.plateGraphic.sharedMaterial = this.plateDownMaterial;
			}
			targetPos = default(Vector3);
		}
		if (!force)
		{
			this.dropCameraShake.DoShake(this, true);
			if (this.endParticles)
			{
				this.endParticles.SetActive(true);
			}
		}
		this.PreActivate();
		yield return new WaitForSeconds(this.gateOpenDelay);
		this.Activate();
		base.CallReceivedEvent();
		if (!string.IsNullOrWhiteSpace(this.sendEventToRegister))
		{
			EventRegister.SendEvent(this.sendEventToRegister, null);
		}
		yield break;
	}

	// Token: 0x06002FB0 RID: 12208 RVA: 0x000D20FB File Offset: 0x000D02FB
	private IEnumerator Raise(float raiseDelay, bool silent)
	{
		if (raiseDelay > 0f)
		{
			yield return new WaitForSeconds(raiseDelay);
		}
		this.forceTouchedTime = Time.fixedTime + 0.1f;
		this.col.enabled = true;
		if (!silent)
		{
			this.PlaySound(this.raiseSound);
			this.PlayVibration(this.raiseVibration);
		}
		if (this.plateGraphic)
		{
			Vector3 targetPos = this.initialPos;
			Vector3 startPos = this.plateGraphic.transform.localPosition;
			if (this.plateUpMaterial)
			{
				this.plateGraphic.sharedMaterial = this.plateUpMaterial;
			}
			for (float elapsed = 0f; elapsed <= this.dropTime; elapsed += Time.deltaTime)
			{
				this.plateGraphic.transform.localPosition = Vector3.Lerp(startPos, targetPos, elapsed / this.dropTime);
				yield return null;
			}
			this.plateGraphic.transform.localPosition = targetPos;
			targetPos = default(Vector3);
			startPos = default(Vector3);
		}
		this.isTouched = false;
		this.Raised();
		yield break;
	}

	// Token: 0x06002FB1 RID: 12209 RVA: 0x000D2118 File Offset: 0x000D0318
	protected void PlaySound(AudioClip clip)
	{
		if (this.source)
		{
			this.source.Stop();
			this.source.clip = clip;
			this.source.Play();
		}
	}

	// Token: 0x06002FB2 RID: 12210 RVA: 0x000D214C File Offset: 0x000D034C
	protected void PlayVibration(VibrationDataAsset vibrationDataAsset)
	{
		if (vibrationDataAsset)
		{
			VibrationManager.PlayVibrationClipOneShot(vibrationDataAsset.VibrationData, null, false, "", false);
		}
	}

	// Token: 0x06002FB3 RID: 12211 RVA: 0x000D217D File Offset: 0x000D037D
	protected virtual void PreActivate()
	{
	}

	// Token: 0x06002FB4 RID: 12212
	protected abstract void Activate();

	// Token: 0x06002FB5 RID: 12213 RVA: 0x000D217F File Offset: 0x000D037F
	protected virtual void Raised()
	{
	}

	// Token: 0x04003263 RID: 12899
	[SerializeField]
	private TriggerEnterEvent dropOnTrigger;

	// Token: 0x04003264 RID: 12900
	[SerializeField]
	private SpriteRenderer plateGraphic;

	// Token: 0x04003265 RID: 12901
	[SerializeField]
	private Material plateUpMaterial;

	// Token: 0x04003266 RID: 12902
	[SerializeField]
	private Material plateDownMaterial;

	// Token: 0x04003267 RID: 12903
	[Space]
	[SerializeField]
	private float touchOffset = 0.1f;

	// Token: 0x04003268 RID: 12904
	[SerializeField]
	private GameObject touchParticles;

	// Token: 0x04003269 RID: 12905
	[SerializeField]
	private GameObject endParticles;

	// Token: 0x0400326A RID: 12906
	[SerializeField]
	private AudioClip landSound;

	// Token: 0x0400326B RID: 12907
	[SerializeField]
	private VibrationDataAsset landVibration;

	// Token: 0x0400326C RID: 12908
	[Space]
	[SerializeField]
	private float waitTime = 1f;

	// Token: 0x0400326D RID: 12909
	[SerializeField]
	private float dropDistance = 1f;

	// Token: 0x0400326E RID: 12910
	[SerializeField]
	private float dropTime = 0.1f;

	// Token: 0x0400326F RID: 12911
	[SerializeField]
	private AudioClip dropSound;

	// Token: 0x04003270 RID: 12912
	[SerializeField]
	private VibrationDataAsset dropVibration;

	// Token: 0x04003271 RID: 12913
	[SerializeField]
	private AudioClip raiseSound;

	// Token: 0x04003272 RID: 12914
	[SerializeField]
	private VibrationDataAsset raiseVibration;

	// Token: 0x04003273 RID: 12915
	[SerializeField]
	private CameraShakeTarget dropCameraShake;

	// Token: 0x04003274 RID: 12916
	[SerializeField]
	private float gateOpenDelay = 1f;

	// Token: 0x04003275 RID: 12917
	[SerializeField]
	private string sendEventToRegister;

	// Token: 0x04003276 RID: 12918
	private Collider2D col;

	// Token: 0x04003277 RID: 12919
	private AudioSource source;

	// Token: 0x04003278 RID: 12920
	private GameObject player;

	// Token: 0x04003279 RID: 12921
	private bool isTouched;

	// Token: 0x0400327A RID: 12922
	private float forceTouchedTime;

	// Token: 0x0400327B RID: 12923
	private Vector3 initialPos;

	// Token: 0x0400327C RID: 12924
	private Coroutine moveRoutine;
}
