using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000448 RID: 1096
[RequireComponent(typeof(Rigidbody2D))]
public class PaintBullet : MonoBehaviour
{
	// Token: 0x0600268F RID: 9871 RVA: 0x000AE780 File Offset: 0x000AC980
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.col = base.GetComponent<Collider2D>();
		this.sprite = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x06002690 RID: 9872 RVA: 0x000AE7A8 File Offset: 0x000AC9A8
	private void OnEnable()
	{
		this.active = true;
		this.scale = Random.Range(this.scaleMin, this.scaleMax);
		this.sprite.enabled = true;
		this.col.enabled = true;
		this.trailParticle.Play();
		this.body.isKinematic = false;
		this.body.linearVelocity = Vector2.zero;
		this.body.angularVelocity = 0f;
		this.splatList = new List<SpriteRenderer>();
		if (this.colour == 0)
		{
			this.SetBlue();
			return;
		}
		if (this.colour == 1)
		{
			this.SetRed();
			return;
		}
		if (this.colour == 2)
		{
			this.SetPink();
		}
	}

	// Token: 0x06002691 RID: 9873 RVA: 0x000AE85C File Offset: 0x000ACA5C
	private void Update()
	{
		if (this.active)
		{
			float rotation = Mathf.Atan2(this.body.linearVelocity.y, this.body.linearVelocity.x) * 57.295776f;
			base.transform.SetRotation2D(rotation);
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

	// Token: 0x06002692 RID: 9874 RVA: 0x000AE950 File Offset: 0x000ACB50
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (this.active && collision.tag == "HeroBox")
		{
			this.active = false;
			base.StartCoroutine(this.Collision(Vector2.zero, false));
		}
		if (this.active && collision.tag == "Extra Tag")
		{
			this.splatList.Add(collision.gameObject.GetComponent<SpriteRenderer>());
		}
	}

	// Token: 0x06002693 RID: 9875 RVA: 0x000AE9C1 File Offset: 0x000ACBC1
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (this.active && collision.tag == "Extra Tag")
		{
			this.splatList.Remove(collision.gameObject.GetComponent<SpriteRenderer>());
		}
	}

	// Token: 0x06002694 RID: 9876 RVA: 0x000AE9F4 File Offset: 0x000ACBF4
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (this.active)
		{
			this.active = false;
			base.StartCoroutine(this.Collision(collision.GetSafeContact().Normal, true));
		}
	}

	// Token: 0x06002695 RID: 9877 RVA: 0x000AEA20 File Offset: 0x000ACC20
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

	// Token: 0x06002696 RID: 9878 RVA: 0x000AEA6E File Offset: 0x000ACC6E
	private IEnumerator Collision(Vector2 normal, bool doRotation)
	{
		base.transform.localScale = new Vector3(this.scale, this.scale, base.transform.localScale.z);
		this.body.isKinematic = true;
		this.body.linearVelocity = Vector2.zero;
		this.body.angularVelocity = 0f;
		this.sprite.enabled = false;
		this.impactParticle.Play();
		this.trailParticle.Stop();
		this.splatEffect.SetActive(true);
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
		AudioClip clip = this.splatClips[Random.Range(0, this.splatClips.Count - 1)];
		Random.Range(0.9f, 1.1f);
		this.audioSourcePrefab.PlayOneShot(clip);
		this.chance = 100;
		this.painting = true;
		foreach (SpriteRenderer spriteRenderer in this.splatList)
		{
			if (this.painting)
			{
				if (Random.Range(1, 100) <= this.chance)
				{
					spriteRenderer.color = this.sprite.color;
					this.chance /= 2;
				}
				else
				{
					this.painting = false;
				}
			}
		}
		yield return null;
		this.col.enabled = false;
		yield return new WaitForSeconds(1f);
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x06002697 RID: 9879 RVA: 0x000AEA8B File Offset: 0x000ACC8B
	public void SetColourBlue()
	{
		this.colour = 0;
		this.SetBlue();
	}

	// Token: 0x06002698 RID: 9880 RVA: 0x000AEA9A File Offset: 0x000ACC9A
	public void SetColourRed()
	{
		this.colour = 1;
		this.SetRed();
	}

	// Token: 0x06002699 RID: 9881 RVA: 0x000AEAAC File Offset: 0x000ACCAC
	public void SetBlue()
	{
		this.sprite.color = this.colourBlue;
		this.splatEffectSprite.color = this.colourBlue;
		this.impactParticle.main.startColor = this.colourBlue;
		this.trailParticle.main.startColor = this.colourBlue;
	}

	// Token: 0x0600269A RID: 9882 RVA: 0x000AEB18 File Offset: 0x000ACD18
	public void SetRed()
	{
		this.sprite.color = this.colourRed;
		this.splatEffectSprite.color = this.colourRed;
		this.impactParticle.main.startColor = this.colourRed;
		this.trailParticle.main.startColor = this.colourRed;
	}

	// Token: 0x0600269B RID: 9883 RVA: 0x000AEB84 File Offset: 0x000ACD84
	public void SetPink()
	{
		this.sprite.color = this.colourPink;
		this.splatEffectSprite.color = this.colourPink;
		this.impactParticle.main.startColor = this.colourPink;
		this.trailParticle.main.startColor = this.colourPink;
	}

	// Token: 0x040023E5 RID: 9189
	public float scaleMin = 1.15f;

	// Token: 0x040023E6 RID: 9190
	public float scaleMax = 1.45f;

	// Token: 0x040023E7 RID: 9191
	private float scale;

	// Token: 0x040023E8 RID: 9192
	[Space]
	public float stretchFactor = 1.2f;

	// Token: 0x040023E9 RID: 9193
	public float stretchMinX = 0.75f;

	// Token: 0x040023EA RID: 9194
	public float stretchMaxY = 1.75f;

	// Token: 0x040023EB RID: 9195
	[Space]
	public AudioSource audioSourcePrefab;

	// Token: 0x040023EC RID: 9196
	public List<AudioClip> splatClips;

	// Token: 0x040023ED RID: 9197
	[Space]
	public ParticleSystem impactParticle;

	// Token: 0x040023EE RID: 9198
	public ParticleSystem trailParticle;

	// Token: 0x040023EF RID: 9199
	public GameObject splatEffect;

	// Token: 0x040023F0 RID: 9200
	public tk2dSprite splatEffectSprite;

	// Token: 0x040023F1 RID: 9201
	[Space]
	public Color colourBlue;

	// Token: 0x040023F2 RID: 9202
	public Color colourRed;

	// Token: 0x040023F3 RID: 9203
	public Color colourPink;

	// Token: 0x040023F4 RID: 9204
	private bool active;

	// Token: 0x040023F5 RID: 9205
	public int colour;

	// Token: 0x040023F6 RID: 9206
	[Space]
	private List<SpriteRenderer> splatList;

	// Token: 0x040023F7 RID: 9207
	private int chance;

	// Token: 0x040023F8 RID: 9208
	private bool painting;

	// Token: 0x040023F9 RID: 9209
	private Rigidbody2D body;

	// Token: 0x040023FA RID: 9210
	private Collider2D col;

	// Token: 0x040023FB RID: 9211
	private SpriteRenderer sprite;
}
