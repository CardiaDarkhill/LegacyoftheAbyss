using System;
using System.Collections;
using UnityEngine;

// Token: 0x020003DF RID: 991
public class GrimmballControl : MonoBehaviour
{
	// Token: 0x17000385 RID: 901
	// (get) Token: 0x060021EA RID: 8682 RVA: 0x0009C636 File Offset: 0x0009A836
	// (set) Token: 0x060021EB RID: 8683 RVA: 0x0009C63E File Offset: 0x0009A83E
	public float Force
	{
		get
		{
			return this.force;
		}
		set
		{
			this.force = value;
		}
	}

	// Token: 0x17000386 RID: 902
	// (get) Token: 0x060021EC RID: 8684 RVA: 0x0009C647 File Offset: 0x0009A847
	// (set) Token: 0x060021ED RID: 8685 RVA: 0x0009C64F File Offset: 0x0009A84F
	public float TweenY
	{
		get
		{
			return this.tweenY;
		}
		set
		{
			this.tweenY = value;
		}
	}

	// Token: 0x060021EE RID: 8686 RVA: 0x0009C658 File Offset: 0x0009A858
	private void Awake()
	{
		this.col = base.GetComponent<Collider2D>();
		this.body = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x060021EF RID: 8687 RVA: 0x0009C674 File Offset: 0x0009A874
	private void OnEnable()
	{
		this.force = 0f;
		this.tweenY = 0f;
		this.col.enabled = true;
		this.hit = false;
		this.ptFlame.Play();
		this.ptSmoke.Play();
		base.transform.localScale = Vector3.one;
	}

	// Token: 0x060021F0 RID: 8688 RVA: 0x0009C6D0 File Offset: 0x0009A8D0
	private void OnDisable()
	{
		iTween.Stop(base.gameObject);
		if (this.fireRoutine != null)
		{
			base.StopCoroutine(this.fireRoutine);
			this.fireRoutine = null;
		}
	}

	// Token: 0x060021F1 RID: 8689 RVA: 0x0009C6F8 File Offset: 0x0009A8F8
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 8 && this.shrinkRoutine == null && !this.hit)
		{
			this.DoHit();
		}
	}

	// Token: 0x060021F2 RID: 8690 RVA: 0x0009C71E File Offset: 0x0009A91E
	public void DoHit()
	{
		this.hit = true;
		if (this.fireRoutine != null)
		{
			base.StopCoroutine(this.fireRoutine);
			this.fireRoutine = null;
		}
		this.shrinkRoutine = base.StartCoroutine(this.Shrink());
	}

	// Token: 0x060021F3 RID: 8691 RVA: 0x0009C754 File Offset: 0x0009A954
	public void Fire()
	{
		if (this.fireRoutine == null)
		{
			this.fireRoutine = base.StartCoroutine(this.DoFire());
		}
	}

	// Token: 0x060021F4 RID: 8692 RVA: 0x0009C770 File Offset: 0x0009A970
	private IEnumerator DoFire()
	{
		Vector3 vector = new Vector3(0f, this.tweenY + Random.Range(-0.2f, 0.2f), 0f);
		iTween.MoveBy(base.gameObject, iTween.Hash(new object[]
		{
			"amount",
			vector,
			"time",
			0.7f,
			"easetype",
			iTween.EaseType.easeOutSine,
			"space",
			Space.World
		}));
		for (float elapsed = 0f; elapsed < this.maxLifeTime; elapsed += Time.fixedDeltaTime)
		{
			this.body.AddForce(new Vector2(this.force, 0f), ForceMode2D.Force);
			yield return new WaitForFixedUpdate();
		}
		this.DoHit();
		yield break;
	}

	// Token: 0x060021F5 RID: 8693 RVA: 0x0009C77F File Offset: 0x0009A97F
	private IEnumerator Shrink()
	{
		this.ptFlame.Stop();
		this.ptSmoke.Stop();
		this.col.enabled = false;
		float time = 0.5f;
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"scale",
			Vector3.zero,
			"time",
			time,
			"easetype",
			iTween.EaseType.linear
		}));
		for (float elapsed = 0f; elapsed < time; elapsed += Time.deltaTime)
		{
			this.body.linearVelocity *= 0.85f;
			yield return null;
		}
		this.shrinkRoutine = null;
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x040020AA RID: 8362
	public ParticleSystem ptFlame;

	// Token: 0x040020AB RID: 8363
	public ParticleSystem ptSmoke;

	// Token: 0x040020AC RID: 8364
	public float maxLifeTime = 10f;

	// Token: 0x040020AD RID: 8365
	private Collider2D col;

	// Token: 0x040020AE RID: 8366
	private Rigidbody2D body;

	// Token: 0x040020AF RID: 8367
	private Coroutine fireRoutine;

	// Token: 0x040020B0 RID: 8368
	private Coroutine shrinkRoutine;

	// Token: 0x040020B1 RID: 8369
	private float force;

	// Token: 0x040020B2 RID: 8370
	private float tweenY;

	// Token: 0x040020B3 RID: 8371
	private bool hit;
}
