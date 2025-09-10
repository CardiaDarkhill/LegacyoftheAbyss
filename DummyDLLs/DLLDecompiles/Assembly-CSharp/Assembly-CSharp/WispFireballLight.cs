using System;
using UnityEngine;

// Token: 0x0200032A RID: 810
public class WispFireballLight : MonoBehaviour
{
	// Token: 0x06001C6A RID: 7274 RVA: 0x0008454C File Offset: 0x0008274C
	private void Awake()
	{
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.collider = base.GetComponent<Collider2D>();
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.body = base.GetComponent<Rigidbody2D>();
		this.damager = base.GetComponent<DamageHero>();
		this.damager.HeroDamaged += this.Explode;
		this.wispsEndEventRegister.ReceivedEvent += delegate()
		{
			this.End();
			this.meshRenderer.enabled = false;
			this.recycleTimer = 3f;
		};
	}

	// Token: 0x06001C6B RID: 7275 RVA: 0x000845C4 File Offset: 0x000827C4
	private void OnEnable()
	{
		this.meshRenderer.enabled = false;
		this.collider.enabled = false;
		this.isActive = false;
		this.explodeTimer = 0f;
		this.recycleTimer = 0f;
		this.explosion.SetActive(false);
		this.haze.SetActive(true);
	}

	// Token: 0x06001C6C RID: 7276 RVA: 0x00084620 File Offset: 0x00082820
	private void FixedUpdate()
	{
		if (!this.isActive)
		{
			return;
		}
		this.body.AddForce(this.curveForce, ForceMode2D.Force);
		Vector2 linearVelocity = this.body.linearVelocity;
		float rotation = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * 57.295776f;
		this.body.rotation = rotation;
	}

	// Token: 0x06001C6D RID: 7277 RVA: 0x00084678 File Offset: 0x00082878
	private void Update()
	{
		if (this.recycleTimer > 0f)
		{
			this.recycleTimer -= Time.deltaTime;
			if (this.recycleTimer <= 0f)
			{
				base.gameObject.Recycle();
				return;
			}
		}
		else if (this.explodeTimer > 0f)
		{
			this.explodeTimer -= Time.deltaTime;
			if (this.explodeTimer <= 0f)
			{
				this.Explode();
			}
		}
	}

	// Token: 0x06001C6E RID: 7278 RVA: 0x000846EF File Offset: 0x000828EF
	private void OnCollisionEnter2D()
	{
		this.Explode();
	}

	// Token: 0x06001C6F RID: 7279 RVA: 0x000846F8 File Offset: 0x000828F8
	public void Fire(Vector2 initialVelocity, Vector2 newCurveForce)
	{
		this.body.linearVelocity = initialVelocity;
		this.curveForce = newCurveForce;
		this.shootAudio.SpawnAndPlayOneShot(base.transform.position, null);
		this.collider.enabled = true;
		this.meshRenderer.enabled = true;
		this.ptFire.Play(true);
		this.animator.Play("Fire Instant");
		this.animator.AnimationCompleted = null;
		this.isActive = true;
		this.explodeTimer = 2f;
	}

	// Token: 0x06001C70 RID: 7280 RVA: 0x00084784 File Offset: 0x00082984
	private void Explode()
	{
		this.End();
		this.explodeCamShake.DoShake(this, true);
		this.explodeAudio.SpawnAndPlayOneShot(base.transform.position, null);
		this.explosion.SetActive(true);
		this.animator.Play("Impact");
		this.animator.AnimationCompleted = new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted);
	}

	// Token: 0x06001C71 RID: 7281 RVA: 0x000847EF File Offset: 0x000829EF
	private void End()
	{
		this.isActive = false;
		this.body.linearVelocity = Vector2.zero;
		this.collider.enabled = false;
		this.haze.SetActive(false);
		this.ptFire.Stop(true);
	}

	// Token: 0x06001C72 RID: 7282 RVA: 0x0008482C File Offset: 0x00082A2C
	private void OnAnimationCompleted(tk2dSpriteAnimator arg1, tk2dSpriteAnimationClip arg2)
	{
		this.meshRenderer.enabled = false;
		this.recycleTimer = 3f;
	}

	// Token: 0x04001B89 RID: 7049
	[SerializeField]
	private GameObject haze;

	// Token: 0x04001B8A RID: 7050
	[SerializeField]
	private ParticleSystem ptFire;

	// Token: 0x04001B8B RID: 7051
	[SerializeField]
	private AudioEvent shootAudio;

	// Token: 0x04001B8C RID: 7052
	[Space]
	[SerializeField]
	private GameObject explosion;

	// Token: 0x04001B8D RID: 7053
	[SerializeField]
	private CameraShakeTarget explodeCamShake;

	// Token: 0x04001B8E RID: 7054
	[SerializeField]
	private AudioEvent explodeAudio;

	// Token: 0x04001B8F RID: 7055
	[Space]
	[SerializeField]
	private EventRegister wispsEndEventRegister;

	// Token: 0x04001B90 RID: 7056
	private bool isActive;

	// Token: 0x04001B91 RID: 7057
	private Vector2 curveForce;

	// Token: 0x04001B92 RID: 7058
	private float explodeTimer;

	// Token: 0x04001B93 RID: 7059
	private float recycleTimer;

	// Token: 0x04001B94 RID: 7060
	private MeshRenderer meshRenderer;

	// Token: 0x04001B95 RID: 7061
	private Collider2D collider;

	// Token: 0x04001B96 RID: 7062
	private tk2dSpriteAnimator animator;

	// Token: 0x04001B97 RID: 7063
	private Rigidbody2D body;

	// Token: 0x04001B98 RID: 7064
	private DamageHero damager;
}
