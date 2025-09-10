using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000584 RID: 1412
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(tk2dSpriteAnimator))]
[RequireComponent(typeof(AudioSource))]
public class WaterDrip : MonoBehaviour
{
	// Token: 0x06003287 RID: 12935 RVA: 0x000E0FEB File Offset: 0x000DF1EB
	private void Awake()
	{
		this.col = base.GetComponent<Collider2D>();
		this.body = base.GetComponent<Rigidbody2D>();
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
		this.source = base.GetComponent<AudioSource>();
	}

	// Token: 0x06003288 RID: 12936 RVA: 0x000E101D File Offset: 0x000DF21D
	private void Start()
	{
		this.startPos = base.transform.position;
		base.StartCoroutine(this.Drip());
	}

	// Token: 0x06003289 RID: 12937 RVA: 0x000E1042 File Offset: 0x000DF242
	private IEnumerator Drip()
	{
		for (;;)
		{
			this.anim.Play("Idle");
			this.body.gravityScale = 0f;
			this.body.linearVelocity = Vector2.zero;
			this.col.enabled = false;
			yield return new WaitForSeconds(Random.Range(this.idleTimeMin, this.idleTimeMax));
			this.col.enabled = true;
			yield return base.StartCoroutine(this.anim.PlayAnimWait("Drip", null));
			this.anim.Play("Fall");
			this.body.gravityScale = 1f;
			this.body.linearVelocity = new Vector2(0f, this.fallVelocity);
			this.impacted = false;
			while (!this.impacted)
			{
				yield return null;
			}
			this.body.gravityScale = 0f;
			this.body.linearVelocity = Vector2.zero;
			this.col.enabled = false;
			this.impactAudioClipTable.PlayOneShot(this.source, false);
			base.transform.position += new Vector3(0f, this.impactTranslation, 0f);
			yield return base.StartCoroutine(this.anim.PlayAnimWait("Impact", null));
			base.transform.position = this.startPos;
		}
		yield break;
	}

	// Token: 0x0600328A RID: 12938 RVA: 0x000E1051 File Offset: 0x000DF251
	private void OnCollisionEnter2D(Collision2D collision)
	{
		this.impacted = true;
	}

	// Token: 0x04003651 RID: 13905
	public float idleTimeMin = 2f;

	// Token: 0x04003652 RID: 13906
	public float idleTimeMax = 8f;

	// Token: 0x04003653 RID: 13907
	public float fallVelocity = -7f;

	// Token: 0x04003654 RID: 13908
	public RandomAudioClipTable impactAudioClipTable;

	// Token: 0x04003655 RID: 13909
	public float impactTranslation = -0.5f;

	// Token: 0x04003656 RID: 13910
	private bool impacted;

	// Token: 0x04003657 RID: 13911
	private Vector2 startPos;

	// Token: 0x04003658 RID: 13912
	private Collider2D col;

	// Token: 0x04003659 RID: 13913
	private Rigidbody2D body;

	// Token: 0x0400365A RID: 13914
	private tk2dSpriteAnimator anim;

	// Token: 0x0400365B RID: 13915
	private AudioSource source;
}
