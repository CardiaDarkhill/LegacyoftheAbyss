using System;
using System.Collections;
using UnityEngine;

// Token: 0x020003E1 RID: 993
public class HeartPieceOrb : MonoBehaviour
{
	// Token: 0x060021FA RID: 8698 RVA: 0x0009C826 File Offset: 0x0009AA26
	private void Awake()
	{
		this.sprite = base.GetComponent<SpriteRenderer>();
		this.trail = base.GetComponent<TrailRenderer>();
		this.body = base.GetComponent<Rigidbody2D>();
		this.source = base.GetComponent<AudioSource>();
	}

	// Token: 0x060021FB RID: 8699 RVA: 0x0009C858 File Offset: 0x0009AA58
	private void Start()
	{
		base.transform.SetPositionZ(Random.Range(-0.001f, -0.1f));
		this.target = HeroController.instance.transform;
	}

	// Token: 0x060021FC RID: 8700 RVA: 0x0009C884 File Offset: 0x0009AA84
	private void OnDisable()
	{
		if (this.sprite)
		{
			this.sprite.enabled = false;
		}
		if (this.trail)
		{
			this.trail.enabled = false;
		}
		if (this.body)
		{
			this.body.isKinematic = true;
		}
		GameManager silentInstance = GameManager.SilentInstance;
		if (silentInstance)
		{
			silentInstance.UnloadingLevel -= this.SceneLoading;
		}
	}

	// Token: 0x060021FD RID: 8701 RVA: 0x0009C8FC File Offset: 0x0009AAFC
	private void OnEnable()
	{
		if (this.sprite)
		{
			this.sprite.enabled = true;
		}
		if (this.trail)
		{
			this.trail.enabled = true;
		}
		if (this.body)
		{
			this.body.isKinematic = false;
		}
		if (this.zoomRoutine != null)
		{
			base.StopCoroutine(this.zoomRoutine);
		}
		this.zoomRoutine = null;
		GameManager.instance.UnloadingLevel += this.SceneLoading;
		this.scaleModifier = Random.Range(this.scaleModifierMin, this.scaleModifierMax);
	}

	// Token: 0x060021FE RID: 8702 RVA: 0x0009C99C File Offset: 0x0009AB9C
	private void Update()
	{
		if (this.body && this.body.linearVelocity.magnitude < 2.5f && this.zoomRoutine == null)
		{
			this.zoomRoutine = base.StartCoroutine(this.Zoom(true));
		}
		this.FaceAngle();
		this.ProjectileSquash();
	}

	// Token: 0x060021FF RID: 8703 RVA: 0x0009C9F7 File Offset: 0x0009ABF7
	private void SceneLoading()
	{
		if (this.zoomRoutine != null)
		{
			base.StopCoroutine(this.zoomRoutine);
		}
		this.zoomRoutine = base.StartCoroutine(this.Zoom(false));
	}

	// Token: 0x06002200 RID: 8704 RVA: 0x0009CA20 File Offset: 0x0009AC20
	private IEnumerator Zoom(bool doZoom = true)
	{
		if (doZoom)
		{
			this.speed = 0f;
			while (this.target)
			{
				this.speed += this.acceleration;
				this.speed = Mathf.Clamp(this.speed, 0f, 30f);
				this.acceleration += 0.07f;
				this.FireAtTarget();
				if (Vector2.Distance(this.target.position, base.transform.position) < 0.8f)
				{
					goto IL_E8;
				}
				yield return null;
			}
			Debug.LogError("Soul orb could not get player target!");
		}
		IL_E8:
		this.body.linearVelocity = Vector2.zero;
		if (this.soulOrbCollectSounds)
		{
			this.soulOrbCollectSounds.PlayOneShot(this.source, false);
		}
		if (this.getParticles)
		{
			this.getParticles.Play();
		}
		if (this.sprite)
		{
			this.sprite.enabled = false;
		}
		SpriteFlash component = HeroController.instance.gameObject.GetComponent<SpriteFlash>();
		if (component)
		{
			component.flashSoulGet();
		}
		yield return new WaitForSeconds(0.4f);
		if (this.dontRecycle)
		{
			base.gameObject.SetActive(false);
		}
		else
		{
			base.gameObject.Recycle();
		}
		yield break;
	}

	// Token: 0x06002201 RID: 8705 RVA: 0x0009CA38 File Offset: 0x0009AC38
	private void FireAtTarget()
	{
		float y = this.target.position.y - base.transform.position.y;
		float x = this.target.position.x - base.transform.position.x;
		float num = Mathf.Atan2(y, x) * 57.295776f;
		Vector2 linearVelocity;
		linearVelocity.x = this.speed * Mathf.Cos(num * 0.017453292f);
		linearVelocity.y = this.speed * Mathf.Sin(num * 0.017453292f);
		this.body.linearVelocity = linearVelocity;
	}

	// Token: 0x06002202 RID: 8706 RVA: 0x0009CAD8 File Offset: 0x0009ACD8
	private void FaceAngle()
	{
		Vector2 linearVelocity = this.body.linearVelocity;
		float z = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * 57.295776f;
		base.transform.localEulerAngles = new Vector3(0f, 0f, z);
	}

	// Token: 0x06002203 RID: 8707 RVA: 0x0009CB24 File Offset: 0x0009AD24
	private void ProjectileSquash()
	{
		float num = 1f - this.body.linearVelocity.magnitude * this.stretchFactor * 0.01f;
		float num2 = 1f + this.body.linearVelocity.magnitude * this.stretchFactor * 0.01f;
		if (num2 > this.stretchMaxX)
		{
			num2 = this.stretchMaxX;
		}
		if (num < this.stretchMinY)
		{
			num = this.stretchMinY;
		}
		num *= this.scaleModifier;
		num2 *= this.scaleModifier;
		base.transform.localScale = new Vector3(num2, num, base.transform.localScale.z);
	}

	// Token: 0x040020B7 RID: 8375
	public RandomAudioClipTable soulOrbCollectSounds;

	// Token: 0x040020B8 RID: 8376
	public ParticleSystem getParticles;

	// Token: 0x040020B9 RID: 8377
	public bool dontRecycle;

	// Token: 0x040020BA RID: 8378
	private Transform target;

	// Token: 0x040020BB RID: 8379
	private float speed;

	// Token: 0x040020BC RID: 8380
	private float acceleration;

	// Token: 0x040020BD RID: 8381
	private SpriteRenderer sprite;

	// Token: 0x040020BE RID: 8382
	private TrailRenderer trail;

	// Token: 0x040020BF RID: 8383
	private Rigidbody2D body;

	// Token: 0x040020C0 RID: 8384
	private AudioSource source;

	// Token: 0x040020C1 RID: 8385
	private Coroutine zoomRoutine;

	// Token: 0x040020C2 RID: 8386
	public float stretchFactor = 2f;

	// Token: 0x040020C3 RID: 8387
	public float stretchMinY = 1f;

	// Token: 0x040020C4 RID: 8388
	public float stretchMaxX = 2f;

	// Token: 0x040020C5 RID: 8389
	public float scaleModifier;

	// Token: 0x040020C6 RID: 8390
	public float scaleModifierMin = 1f;

	// Token: 0x040020C7 RID: 8391
	public float scaleModifierMax = 2f;
}
