using System;
using UnityEngine;

// Token: 0x020005C8 RID: 1480
public class SpellGetOrb : MonoBehaviour
{
	// Token: 0x060034C5 RID: 13509 RVA: 0x000EA394 File Offset: 0x000E8594
	private void Start()
	{
		base.transform.position = new Vector3(base.transform.position.x + Random.Range(-0.1f, 0.1f), base.transform.position.y + Random.Range(-0.2f, 0.2f), Random.Range(-6f, 6f));
		this.idleTime = Random.Range(2.5f, 4.5f);
		float num = Random.Range(0.3f, 0.7f);
		base.transform.localScale = new Vector3(num, num);
	}

	// Token: 0x060034C6 RID: 13510 RVA: 0x000EA438 File Offset: 0x000E8638
	private void OnEnable()
	{
		if (this.trackToHero)
		{
			GameManager instance = GameManager.instance;
			this.hero = instance.hero_ctrl.gameObject;
			if (this.hero == null)
			{
				this.hero = GameObject.FindWithTag("Player");
			}
		}
		this.startPosition = base.transform.position;
		float num = Random.Range(3f, 20f);
		float num2 = Random.Range(0f, 360f);
		float x = num * Mathf.Cos(num2 * 0.017453292f) * 1.25f;
		float y = num * Mathf.Sin(num2 * 0.017453292f);
		this.rb2d.linearVelocity = new Vector2(x, y);
	}

	// Token: 0x060034C7 RID: 13511 RVA: 0x000EA4E8 File Offset: 0x000E86E8
	private void Update()
	{
		if (this.state == 0)
		{
			if (this.timer < this.idleTime)
			{
				this.timer += Time.deltaTime;
				return;
			}
			this.rb2d.linearVelocity = new Vector3(0f, 0f, 0f);
			this.trailObject.SetActive(true);
			this.zoomPosition = base.transform.position;
			this.state = 1;
			this.ptIdle.Stop();
			this.ptZoom.Play();
			if (this.hero != null && this.trackToHero)
			{
				this.startPosition = new Vector3(this.hero.transform.position.x, this.hero.transform.position.y - 0.5f, this.hero.transform.position.z);
				return;
			}
		}
		else if (this.state == 1)
		{
			this.trailRenderer.startWidth = base.transform.localScale.x;
			base.transform.position = Vector3.Lerp(this.zoomPosition, this.startPosition, this.lerpTimer);
			this.lerpTimer += Time.deltaTime * this.accel;
			this.accel += Time.deltaTime * this.accelMultiplier;
			if (this.lerpTimer >= 1f)
			{
				this.Collect();
			}
		}
	}

	// Token: 0x060034C8 RID: 13512 RVA: 0x000EA680 File Offset: 0x000E8880
	private void FaceAngle()
	{
		Vector2 linearVelocity = this.rb2d.linearVelocity;
		float z = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * 57.295776f;
		base.transform.localEulerAngles = new Vector3(0f, 0f, z);
	}

	// Token: 0x060034C9 RID: 13513 RVA: 0x000EA6CC File Offset: 0x000E88CC
	private void ProjectileSquash()
	{
		float num = 1f - this.rb2d.linearVelocity.magnitude * this.stretchFactor * 0.01f;
		float num2 = 1f + this.rb2d.linearVelocity.magnitude * this.stretchFactor * 0.01f;
		if (num2 < this.stretchMinX)
		{
			num2 = this.stretchMinX;
		}
		if (num > this.stretchMaxY)
		{
			num = this.stretchMaxY;
		}
		num *= this.scaleModifier;
		num2 *= this.scaleModifier;
		base.transform.localScale = new Vector3(num2, num, base.transform.localScale.z);
	}

	// Token: 0x060034CA RID: 13514 RVA: 0x000EA77C File Offset: 0x000E897C
	private void Collect()
	{
		this.state = 2;
		this.rb2d.linearVelocity = new Vector3(0f, 0f, 0f);
		this.spriteRenderer.enabled = false;
		this.trailObject.SetActive(false);
		this.orbGetObject.SetActive(true);
		this.ptZoom.Stop();
	}

	// Token: 0x04003834 RID: 14388
	public SpriteRenderer spriteRenderer;

	// Token: 0x04003835 RID: 14389
	public Rigidbody2D rb2d;

	// Token: 0x04003836 RID: 14390
	public GameObject trailObject;

	// Token: 0x04003837 RID: 14391
	public TrailRenderer trailRenderer;

	// Token: 0x04003838 RID: 14392
	public GameObject orbGetObject;

	// Token: 0x04003839 RID: 14393
	public ParticleSystem ptIdle;

	// Token: 0x0400383A RID: 14394
	public ParticleSystem ptZoom;

	// Token: 0x0400383B RID: 14395
	public bool trackToHero;

	// Token: 0x0400383C RID: 14396
	private float accel = 0.5f;

	// Token: 0x0400383D RID: 14397
	private float accelMultiplier = 12f;

	// Token: 0x0400383E RID: 14398
	private float stretchFactor = 2f;

	// Token: 0x0400383F RID: 14399
	private float stretchMinX = 0.5f;

	// Token: 0x04003840 RID: 14400
	private float stretchMaxY = 2f;

	// Token: 0x04003841 RID: 14401
	private float scaleModifier;

	// Token: 0x04003842 RID: 14402
	private float timer;

	// Token: 0x04003843 RID: 14403
	private float idleTime;

	// Token: 0x04003844 RID: 14404
	private float lerpTimer;

	// Token: 0x04003845 RID: 14405
	private Vector3 startPosition;

	// Token: 0x04003846 RID: 14406
	private Vector3 zoomPosition;

	// Token: 0x04003847 RID: 14407
	private GameObject hero;

	// Token: 0x04003848 RID: 14408
	private int state;
}
