using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200032C RID: 812
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBullet : MonoBehaviour
{
	// Token: 0x06001C7C RID: 7292 RVA: 0x00084CC0 File Offset: 0x00082EC0
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
		this.col = base.GetComponent<Collider2D>();
		if (this.anim)
		{
			this.impactAnim = this.anim.GetClipByName("Impact");
		}
	}

	// Token: 0x06001C7D RID: 7293 RVA: 0x00084D14 File Offset: 0x00082F14
	private void OnEnable()
	{
		this.active = true;
		this.scale = Random.Range(this.scaleMin, this.scaleMax);
		this.col.enabled = true;
		if (this.damageCollider)
		{
			this.damageCollider.SetActive(true);
		}
		this.body.isKinematic = false;
		this.body.linearVelocity = Vector2.zero;
		this.body.angularVelocity = 0f;
		if (this.idleParticle)
		{
			this.idleParticle.Play();
		}
		if (!this.dontUseAnimator && this.anim)
		{
			this.anim.Play("Idle");
		}
		MeshRenderer component = base.GetComponent<MeshRenderer>();
		if (component)
		{
			component.enabled = true;
		}
		if (this.idleEffects)
		{
			this.idleEffects.SetActive(true);
		}
		if (this.impactEffects)
		{
			this.impactEffects.SetActive(false);
		}
	}

	// Token: 0x06001C7E RID: 7294 RVA: 0x00084E18 File Offset: 0x00083018
	private void Update()
	{
		if (this.active)
		{
			float rotation = Mathf.Atan2(this.body.linearVelocity.y, this.body.linearVelocity.x) * 57.295776f;
			base.transform.SetRotation2D(rotation);
			if (this.stretchFactor != 1f)
			{
				float num = 1f - this.body.linearVelocity.magnitude * this.stretchFactor * 0.01f;
				float num2 = 1f + this.body.linearVelocity.magnitude * this.stretchFactor * 0.01f;
				if (num2 < this.stretchMinX)
				{
					num2 = this.stretchMinX;
				}
				if (num > this.stretchMaxY)
				{
					num = this.stretchMaxY;
				}
				num *= this.scale;
				num2 *= this.scale;
				base.transform.localScale = new Vector3(num2, num, base.transform.localScale.z);
			}
		}
	}

	// Token: 0x06001C7F RID: 7295 RVA: 0x00084F1A File Offset: 0x0008311A
	private void OnCollisionEnter2D(Collision2D collision)
	{
		GameObject gameObject = collision.gameObject;
		if (this.active)
		{
			this.active = false;
			base.StartCoroutine(this.Collision(collision.GetSafeContact().Normal, true));
		}
	}

	// Token: 0x06001C80 RID: 7296 RVA: 0x00084F4C File Offset: 0x0008314C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.active && !this.heroBullet && collision.tag == "HeroBox")
		{
			this.active = false;
			base.StartCoroutine(this.Collision(Vector2.zero, false));
		}
		if (this.active)
		{
			GameObject gameObject = collision.gameObject;
			if (gameObject.layer == 25 && gameObject.CompareTag("Water Surface"))
			{
				this.active = false;
				this.Break();
			}
		}
	}

	// Token: 0x06001C81 RID: 7297 RVA: 0x00084FC8 File Offset: 0x000831C8
	public void OrbitShieldHit(Transform shield)
	{
		if (this.active)
		{
			this.active = false;
			Vector2 normal = base.transform.position - shield.position;
			normal.Normalize();
			base.StartCoroutine(this.Collision(normal, true));
		}
	}

	// Token: 0x06001C82 RID: 7298 RVA: 0x00085016 File Offset: 0x00083216
	public void Break()
	{
		base.StartCoroutine(this.Collision(Vector2.zero, false));
	}

	// Token: 0x06001C83 RID: 7299 RVA: 0x0008502B File Offset: 0x0008322B
	public void TornadoEffect()
	{
		this.Break();
	}

	// Token: 0x06001C84 RID: 7300 RVA: 0x00085033 File Offset: 0x00083233
	private IEnumerator Collision(Vector2 normal, bool doRotation)
	{
		MeshRenderer meshRenderer = base.GetComponent<MeshRenderer>();
		base.transform.localScale = new Vector3(this.scale, this.scale, base.transform.localScale.z);
		this.body.isKinematic = true;
		this.body.linearVelocity = Vector2.zero;
		this.body.angularVelocity = 0f;
		float animTime = 0f;
		if (this.impactAnim != null && !this.dontUseAnimator)
		{
			this.anim.Play(this.impactAnim);
			animTime = (float)(this.impactAnim.frames.Length - 1) / this.impactAnim.fps;
		}
		else if (meshRenderer)
		{
			meshRenderer.enabled = false;
		}
		if (!this.heroBullet && !this.noImpactRotation)
		{
			if (!doRotation || (normal.y >= 0.75f && Mathf.Abs(normal.x) < 0.5f))
			{
				base.transform.SetRotation2D(0f);
			}
			else if (normal.y <= 0.75f && Mathf.Abs(normal.x) < 0.5f)
			{
				base.transform.SetRotation2D(180f);
			}
			else if (normal.x >= 0.75f && Mathf.Abs(normal.y) < 0.5f)
			{
				base.transform.SetRotation2D(270f);
			}
			else if (normal.x <= 0.75f && Mathf.Abs(normal.y) < 0.5f)
			{
				base.transform.SetRotation2D(90f);
			}
		}
		if (this.impactAudioClipTable)
		{
			this.impactAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		else
		{
			this.impactSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		}
		if (this.idleParticle)
		{
			this.idleParticle.Stop();
		}
		if (this.idleEffects)
		{
			this.idleEffects.SetActive(false);
		}
		if (this.impactEffects)
		{
			this.impactEffects.SetActive(true);
		}
		yield return null;
		this.col.enabled = false;
		if (this.damageCollider)
		{
			this.damageCollider.SetActive(false);
		}
		yield return new WaitForSeconds(animTime);
		if (meshRenderer)
		{
			meshRenderer.enabled = false;
		}
		yield return new WaitForSeconds(this.lingerTime);
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x04001BA5 RID: 7077
	public float scaleMin = 1.15f;

	// Token: 0x04001BA6 RID: 7078
	public float scaleMax = 1.45f;

	// Token: 0x04001BA7 RID: 7079
	private float scale;

	// Token: 0x04001BA8 RID: 7080
	[Space]
	public float stretchFactor = 1.2f;

	// Token: 0x04001BA9 RID: 7081
	public float stretchMinX = 0.75f;

	// Token: 0x04001BAA RID: 7082
	public float stretchMaxY = 1.75f;

	// Token: 0x04001BAB RID: 7083
	[Space]
	public float lingerTime;

	// Token: 0x04001BAC RID: 7084
	public GameObject idleEffects;

	// Token: 0x04001BAD RID: 7085
	public GameObject impactEffects;

	// Token: 0x04001BAE RID: 7086
	public ParticleSystem idleParticle;

	// Token: 0x04001BAF RID: 7087
	[Space]
	public AudioSource audioSourcePrefab;

	// Token: 0x04001BB0 RID: 7088
	public AudioEvent impactSound;

	// Token: 0x04001BB1 RID: 7089
	public RandomAudioClipTable impactAudioClipTable;

	// Token: 0x04001BB2 RID: 7090
	[Space]
	public bool heroBullet;

	// Token: 0x04001BB3 RID: 7091
	public GameObject damageCollider;

	// Token: 0x04001BB4 RID: 7092
	public bool noImpactRotation;

	// Token: 0x04001BB5 RID: 7093
	public bool dontUseAnimator;

	// Token: 0x04001BB6 RID: 7094
	private bool active;

	// Token: 0x04001BB7 RID: 7095
	private Rigidbody2D body;

	// Token: 0x04001BB8 RID: 7096
	private tk2dSpriteAnimator anim;

	// Token: 0x04001BB9 RID: 7097
	private tk2dSpriteAnimationClip impactAnim;

	// Token: 0x04001BBA RID: 7098
	private Collider2D col;
}
