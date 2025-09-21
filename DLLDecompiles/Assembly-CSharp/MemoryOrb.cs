using System;
using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x020003E2 RID: 994
public class MemoryOrb : MonoBehaviour
{
	// Token: 0x17000387 RID: 903
	// (get) Token: 0x06002205 RID: 8709 RVA: 0x0009CC12 File Offset: 0x0009AE12
	// (set) Token: 0x06002206 RID: 8710 RVA: 0x0009CC1A File Offset: 0x0009AE1A
	public bool InstantOrb { get; set; }

	// Token: 0x17000388 RID: 904
	// (get) Token: 0x06002207 RID: 8711 RVA: 0x0009CC23 File Offset: 0x0009AE23
	// (set) Token: 0x06002208 RID: 8712 RVA: 0x0009CC2B File Offset: 0x0009AE2B
	public bool SkipAppear { get; set; }

	// Token: 0x06002209 RID: 8713 RVA: 0x0009CC34 File Offset: 0x0009AE34
	private void Awake()
	{
		this.circleCollider = base.GetComponent<CircleCollider2D>();
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x0600220A RID: 8714 RVA: 0x0009CC50 File Offset: 0x0009AE50
	private void OnEnable()
	{
		if (!this.nextOrb && !this.InstantOrb)
		{
			if (base.transform.parent != null && base.transform.parent.parent != null)
			{
				Transform transform = base.transform.parent.parent.Find("Silkfly Cloud");
				if (transform != null)
				{
					this.silkflyCloud = transform.gameObject;
				}
			}
		}
		else if (!this.InstantOrb)
		{
			this.isSmall = true;
		}
		this.circleCollider.enabled = true;
		this.activateFx.SetActive(false);
		this.idleFx.SetActive(true);
		this.collectFx.SetActive(false);
		this.collectFxLarge.SetActive(false);
		this.dissipateFx.SetActive(false);
		this.returnObj.SetActive(false);
		this.idleFx.transform.localPosition = new Vector3(0f, 0f, 0f);
		this.collected = false;
		if (this.collectRoutine != null)
		{
			base.StopCoroutine(this.collectRoutine);
			this.collectRoutine = null;
		}
		this.collectableTime = Time.timeAsDouble + 0.4000000059604645;
		this.active = false;
		this.collected = false;
		if (this.isSmall)
		{
			this.memoryOrbSmall.SetActive(false);
			this.memoryOrbSmallInactive.SetActive(true);
		}
		else
		{
			this.memoryOrbLarge.SetActive(false);
			this.memoryOrbLargeInactive.SetActive(true);
		}
		this.timeAlertAnimator.ForceStop();
		this.timeAlertFadeGroup.FadeTo(1f, 0f, null, false, null);
		if (!this.InstantOrb)
		{
			return;
		}
		if (this.SkipAppear)
		{
			this.SetActive();
		}
		else
		{
			base.StartCoroutine(this.InstantOrbSpawn());
		}
		this.appearFx.SetActive(false);
	}

	// Token: 0x0600220B RID: 8715 RVA: 0x0009CE2B File Offset: 0x0009B02B
	private void OnTriggerEnter2D(Collider2D collision)
	{
		this.Collect();
	}

	// Token: 0x0600220C RID: 8716 RVA: 0x0009CE33 File Offset: 0x0009B033
	private void OnTriggerStay2D(Collider2D collision)
	{
		this.Collect();
	}

	// Token: 0x0600220D RID: 8717 RVA: 0x0009CE3C File Offset: 0x0009B03C
	private void Collect()
	{
		if (!this.active)
		{
			return;
		}
		if (this.collected)
		{
			return;
		}
		if (Time.timeAsDouble < this.collectableTime)
		{
			return;
		}
		this.collected = true;
		this.circleCollider.enabled = false;
		this.idleFx.SetActive(false);
		if (this.isSmall)
		{
			this.collectFx.SetActive(true);
		}
		else
		{
			this.collectFxLarge.SetActive(true);
		}
		if (this.isSmall)
		{
			this.audioSource.volume = 1f;
			this.audioSource.PlayOneShot(this.collectSmall);
		}
		else
		{
			this.audioSource.volume = 1f;
			this.audioSource.PlayOneShot(this.collectLarge);
		}
		if (this.nextOrb)
		{
			this.nextOrb.SetActive();
			Transform transform = this.nextOrb.transform;
			float y = transform.position.y - base.transform.position.y;
			float x = transform.position.x - base.transform.position.x;
			float num;
			for (num = Mathf.Atan2(y, x) * 57.295776f; num < 0f; num += 360f)
			{
			}
			this.nextOrbTrail.transform.localEulerAngles = new Vector3(0f, 0f, num);
			this.nextOrbTrail.SetActive(true);
		}
		if (GameManager.instance && GameManager.instance.cameraCtrl)
		{
			GameManager.instance.cameraCtrl.ScreenFlash(this.flashColour);
		}
		if (this.orbGroup)
		{
			this.orbGroup.CollectedOrb(this.orbIndex);
		}
		if (!this.isSmall && !this.InstantOrb && this.silkflyCloud != null)
		{
			this.silkflyCloud.GetComponent<PlayMakerFSM>().SendEvent("COMPLETED");
		}
		base.StartCoroutine(this.CollectRoutine());
	}

	// Token: 0x0600220E RID: 8718 RVA: 0x0009D031 File Offset: 0x0009B231
	private IEnumerator CollectRoutine()
	{
		PlayerData instance = PlayerData.instance;
		int orbCount = instance.CollectedCloverMemoryOrbs;
		yield return new WaitForSeconds(0.3f);
		if (!this.isSmall)
		{
			this.returnObj.SetActive(true);
			Transform returnObjTrans = this.returnObj.transform;
			Vector2 startPos = returnObjTrans.position;
			Vector2 endPos = this.returnTarget.position;
			float num = Vector2.Distance(startPos, endPos);
			float duration = num / this.returnSpeed;
			Vector2 to = endPos - startPos;
			float rotation = Vector2.SignedAngle(Vector2.right, to);
			returnObjTrans.SetRotation2D(rotation);
			float elapsed = 0f;
			while (elapsed < duration && elapsed < 0.75f)
			{
				Vector2 position = Vector2.Lerp(startPos, endPos, elapsed / duration);
				returnObjTrans.SetPosition2D(position);
				yield return null;
				elapsed += Time.deltaTime;
			}
			if (this.orbGroup)
			{
				this.orbGroup.LargeOrbReturned();
			}
			returnObjTrans = null;
			startPos = default(Vector2);
			endPos = default(Vector2);
		}
		else
		{
			yield return new WaitForSeconds(0.75f);
		}
		yield return new WaitForSeconds(0.5f);
		if (this.orbGroup)
		{
			this.orbGroup.OrbReturned(orbCount);
		}
		base.gameObject.SetActive(false);
		this.collectRoutine = null;
		yield break;
	}

	// Token: 0x0600220F RID: 8719 RVA: 0x0009D040 File Offset: 0x0009B240
	private IEnumerator InstantOrbSpawn()
	{
		Vector3 destination = base.transform.position;
		base.transform.position = new Vector3(this.spawnPoint.transform.position.x, this.spawnPoint.transform.position.y, base.transform.position.z);
		Vector2 startPos = base.transform.position;
		Vector2 endPos = destination;
		float num = Vector2.Distance(startPos, endPos);
		float duration = num / this.spawnSpeed;
		endPos - startPos;
		for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
		{
			Vector2 position = Vector2.Lerp(startPos, endPos, elapsed / duration);
			base.transform.SetPosition2D(position);
			yield return null;
		}
		base.transform.position = destination;
		this.SetActive();
		yield break;
	}

	// Token: 0x06002210 RID: 8720 RVA: 0x0009D050 File Offset: 0x0009B250
	public void SetActive()
	{
		if (this.isSmall)
		{
			this.memoryOrbSmall.SetActive(true);
			this.memoryOrbSmallInactive.SetActive(false);
		}
		else
		{
			this.memoryOrbLarge.SetActive(true);
			this.memoryOrbLargeInactive.SetActive(false);
		}
		this.activateFx.SetActive(true);
		this.active = true;
	}

	// Token: 0x06002211 RID: 8721 RVA: 0x0009D0AA File Offset: 0x0009B2AA
	public void Setup(MemoryOrbGroup group, int index)
	{
		this.orbGroup = group;
		this.orbIndex = index;
	}

	// Token: 0x06002212 RID: 8722 RVA: 0x0009D0BA File Offset: 0x0009B2BA
	public void StartTimeAlert()
	{
		if (this.timeAlertAnimator.gameObject.activeInHierarchy)
		{
			this.timeAlertAnimator.StartAnimation();
			this.audioSource.volume = 0.25f;
			this.audioSource.PlayOneShot(this.dissipateAntic);
		}
	}

	// Token: 0x06002213 RID: 8723 RVA: 0x0009D0FA File Offset: 0x0009B2FA
	public void StopTimeAlert()
	{
		if (this.timeAlertAnimator.gameObject.activeInHierarchy)
		{
			this.timeAlertAnimator.ForceStop();
			this.timeAlertFadeGroup.FadeTo(1f, 0f, null, false, null);
		}
	}

	// Token: 0x06002214 RID: 8724 RVA: 0x0009D132 File Offset: 0x0009B332
	public void Dissipate()
	{
		if (!this.collected)
		{
			base.StartCoroutine(this.DoDissipate());
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x06002215 RID: 8725 RVA: 0x0009D156 File Offset: 0x0009B356
	private IEnumerator DoDissipate()
	{
		this.circleCollider.enabled = false;
		this.audioSource.volume = 0.3f;
		this.audioSource.pitch = Random.Range(0.8f, 1.2f);
		this.dissipateFx.SetActive(true);
		this.audioSource.PlayOneShot(this.dissipate);
		this.idleFx.SetActive(false);
		yield return new WaitForSeconds(1f);
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x040020C8 RID: 8392
	[SerializeField]
	private Color flashColour;

	// Token: 0x040020C9 RID: 8393
	[SerializeField]
	private GameObject idleFx;

	// Token: 0x040020CA RID: 8394
	[SerializeField]
	private NestedFadeGroup idleFxFadeGroup;

	// Token: 0x040020CB RID: 8395
	[SerializeField]
	private GameObject memoryOrbSmall;

	// Token: 0x040020CC RID: 8396
	[SerializeField]
	private GameObject memoryOrbSmallInactive;

	// Token: 0x040020CD RID: 8397
	[SerializeField]
	private GameObject memoryOrbLarge;

	// Token: 0x040020CE RID: 8398
	[SerializeField]
	private GameObject memoryOrbLargeInactive;

	// Token: 0x040020CF RID: 8399
	[SerializeField]
	private NestedFadeGroup timeAlertFadeGroup;

	// Token: 0x040020D0 RID: 8400
	[SerializeField]
	private NestedFadeGroupCurveAnimator timeAlertAnimator;

	// Token: 0x040020D1 RID: 8401
	[SerializeField]
	private GameObject appearFx;

	// Token: 0x040020D2 RID: 8402
	[SerializeField]
	private GameObject activateFx;

	// Token: 0x040020D3 RID: 8403
	[SerializeField]
	private GameObject collectFx;

	// Token: 0x040020D4 RID: 8404
	[SerializeField]
	private GameObject collectFxLarge;

	// Token: 0x040020D5 RID: 8405
	[SerializeField]
	private GameObject dissipateFx;

	// Token: 0x040020D6 RID: 8406
	[SerializeField]
	private GameObject returnObj;

	// Token: 0x040020D7 RID: 8407
	[SerializeField]
	private MemoryOrb nextOrb;

	// Token: 0x040020D8 RID: 8408
	[SerializeField]
	private GameObject nextOrbTrail;

	// Token: 0x040020D9 RID: 8409
	[SerializeField]
	private Transform spawnPoint;

	// Token: 0x040020DA RID: 8410
	[Space]
	[SerializeField]
	private Transform returnTarget;

	// Token: 0x040020DB RID: 8411
	[SerializeField]
	private float returnSpeed;

	// Token: 0x040020DC RID: 8412
	[SerializeField]
	private float spawnSpeed;

	// Token: 0x040020DD RID: 8413
	[SerializeField]
	private AudioClip collectSmall;

	// Token: 0x040020DE RID: 8414
	[SerializeField]
	private AudioClip collectLarge;

	// Token: 0x040020DF RID: 8415
	[SerializeField]
	private AudioClip dissipateAntic;

	// Token: 0x040020E0 RID: 8416
	[SerializeField]
	private AudioClip dissipate;

	// Token: 0x040020E1 RID: 8417
	private bool active;

	// Token: 0x040020E2 RID: 8418
	private bool collected;

	// Token: 0x040020E3 RID: 8419
	private bool isSmall;

	// Token: 0x040020E4 RID: 8420
	private Coroutine collectRoutine;

	// Token: 0x040020E5 RID: 8421
	private GameObject silkflyCloud;

	// Token: 0x040020E6 RID: 8422
	private MemoryOrbGroup orbGroup;

	// Token: 0x040020E7 RID: 8423
	private int orbIndex;

	// Token: 0x040020E8 RID: 8424
	private CircleCollider2D circleCollider;

	// Token: 0x040020E9 RID: 8425
	private AudioSource audioSource;

	// Token: 0x040020EA RID: 8426
	private double collectableTime;

	// Token: 0x040020EB RID: 8427
	private const float TIME_UNTIL_COLLECTABLE = 0.4f;
}
