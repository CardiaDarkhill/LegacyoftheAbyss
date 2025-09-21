using System;
using UnityEngine;

// Token: 0x020004A6 RID: 1190
public class BreakablePole : MonoBehaviour, IHitResponder
{
	// Token: 0x06002B33 RID: 11059 RVA: 0x000BD56B File Offset: 0x000BB76B
	protected void Reset()
	{
		this.inertBackgroundThreshold = -1f;
		this.inertForegroundThreshold = 1f;
	}

	// Token: 0x06002B34 RID: 11060 RVA: 0x000BD584 File Offset: 0x000BB784
	protected void Start()
	{
		float z = base.transform.position.z;
		if (z < this.inertBackgroundThreshold || z > this.inertForegroundThreshold)
		{
			base.enabled = false;
			return;
		}
	}

	// Token: 0x06002B35 RID: 11061 RVA: 0x000BD5BC File Offset: 0x000BB7BC
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		int cardinalDirection = DirectionUtils.GetCardinalDirection(damageInstance.Direction);
		if (cardinalDirection != 2 && cardinalDirection != 0)
		{
			return IHitResponder.Response.None;
		}
		this.spriteRenderer.sprite = this.brokenSprite;
		Transform transform = this.slashImpactPrefab.Spawn().transform;
		transform.eulerAngles = new Vector3(0f, 0f, Random.Range(340f, 380f));
		Vector3 localScale = transform.localScale;
		localScale.x = ((cardinalDirection == 2) ? -1f : 1f);
		localScale.y = 1f;
		this.hitClip.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, false, 1f, null);
		this.top.gameObject.SetActive(true);
		float num = (float)((cardinalDirection == 2) ? Random.Range(120, 140) : Random.Range(40, 60));
		this.top.linearVelocity = new Vector2(Mathf.Cos(num * 0.017453292f), Mathf.Sin(num * 0.017453292f)) * 17f;
		base.enabled = false;
		return IHitResponder.Response.GenericHit;
	}

	// Token: 0x04002C71 RID: 11377
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	// Token: 0x04002C72 RID: 11378
	[SerializeField]
	private Sprite brokenSprite;

	// Token: 0x04002C73 RID: 11379
	[SerializeField]
	private float inertBackgroundThreshold;

	// Token: 0x04002C74 RID: 11380
	[SerializeField]
	private float inertForegroundThreshold;

	// Token: 0x04002C75 RID: 11381
	[SerializeField]
	private AudioSource audioSourcePrefab;

	// Token: 0x04002C76 RID: 11382
	[SerializeField]
	private RandomAudioClipTable hitClip;

	// Token: 0x04002C77 RID: 11383
	[SerializeField]
	private GameObject slashImpactPrefab;

	// Token: 0x04002C78 RID: 11384
	[SerializeField]
	private Rigidbody2D top;
}
