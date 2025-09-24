using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004EC RID: 1260
[RequireComponent(typeof(Collider2D))]
public class GlobControl : MonoBehaviour
{
	// Token: 0x06002D27 RID: 11559 RVA: 0x000C516F File Offset: 0x000C336F
	private void Awake()
	{
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
	}

	// Token: 0x06002D28 RID: 11560 RVA: 0x000C5180 File Offset: 0x000C3380
	private void OnEnable()
	{
		float num = Random.Range(this.minScale, this.maxScale);
		base.transform.localScale = new Vector3(num, num, 1f);
		if (this.splatChild)
		{
			this.splatChild.SetActive(false);
		}
		this.landed = false;
		this.broken = false;
	}

	// Token: 0x06002D29 RID: 11561 RVA: 0x000C51E0 File Offset: 0x000C33E0
	private void Start()
	{
		CollisionEnterEvent collision = base.GetComponent<CollisionEnterEvent>();
		if (collision)
		{
			collision.CollisionEnteredDirectional += delegate(CollisionEnterEvent.Direction direction, Collision2D col)
			{
				if (!this.landed)
				{
					if (direction == CollisionEnterEvent.Direction.Bottom)
					{
						this.landed = true;
						collision.DoCollisionStay = false;
						if (this.CheckForGround())
						{
							this.anim.Play(this.landAnim);
							this.landSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, this.transform.position, null);
							return;
						}
						this.StartCoroutine(this.Break());
						return;
					}
					else
					{
						collision.DoCollisionStay = true;
					}
				}
			};
		}
		TriggerEnterEvent componentInChildren = base.GetComponentInChildren<TriggerEnterEvent>();
		if (componentInChildren)
		{
			componentInChildren.OnTriggerEntered += delegate(Collider2D col, GameObject sender)
			{
				if (!this.landed || this.broken)
				{
					return;
				}
				if (col.gameObject.layer == 11)
				{
					this.anim.Play(this.wobbleAnim);
					this.wobbleSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, this.transform.position, null);
				}
			};
		}
	}

	// Token: 0x06002D2A RID: 11562 RVA: 0x000C524C File Offset: 0x000C344C
	private void OnTriggerEnter2D(Collider2D col)
	{
		if (!this.landed || this.broken)
		{
			return;
		}
		if (col.tag == "Nail Attack")
		{
			base.StartCoroutine(this.Break());
			return;
		}
		if (col.tag == "HeroBox")
		{
			this.wobbleSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
			this.anim.Play(this.wobbleAnim);
		}
	}

	// Token: 0x06002D2B RID: 11563 RVA: 0x000C52CB File Offset: 0x000C34CB
	private IEnumerator Break()
	{
		this.broken = true;
		this.breakSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		BloodSpawner.SpawnBlood(base.transform.position, 4, 5, 5f, 20f, 80f, 100f, new Color?(this.bloodColorOverride), 0f);
		if (this.splatChild)
		{
			this.splatChild.SetActive(true);
		}
		yield return this.anim.PlayAnimWait(this.breakAnim, null);
		if (this.rend)
		{
			this.rend.enabled = false;
		}
		yield break;
	}

	// Token: 0x06002D2C RID: 11564 RVA: 0x000C52DC File Offset: 0x000C34DC
	private bool CheckForGround()
	{
		if (!this.groundCollider)
		{
			return true;
		}
		Vector2 vector = this.groundCollider.bounds.min;
		Vector2 vector2 = this.groundCollider.bounds.max;
		float num = vector2.y - vector.y;
		vector.y = vector2.y;
		vector.x += 0.1f;
		vector2.x -= 0.1f;
		RaycastHit2D raycastHit2D = Helper.Raycast2D(vector, Vector2.down, num + 0.25f, 256);
		RaycastHit2D raycastHit2D2 = Helper.Raycast2D(vector2, Vector2.down, num + 0.25f, 256);
		return raycastHit2D.collider != null && raycastHit2D2.collider != null;
	}

	// Token: 0x04002EC4 RID: 11972
	public Renderer rend;

	// Token: 0x04002EC5 RID: 11973
	[Space]
	public float minScale = 0.6f;

	// Token: 0x04002EC6 RID: 11974
	public float maxScale = 1.6f;

	// Token: 0x04002EC7 RID: 11975
	[Space]
	public string landAnim = "Glob Land";

	// Token: 0x04002EC8 RID: 11976
	public string wobbleAnim = "Glob Wobble";

	// Token: 0x04002EC9 RID: 11977
	public string breakAnim = "Glob Break";

	// Token: 0x04002ECA RID: 11978
	[Space]
	public AudioSource audioPlayerPrefab;

	// Token: 0x04002ECB RID: 11979
	public AudioEvent breakSound;

	// Token: 0x04002ECC RID: 11980
	public AudioEvent landSound;

	// Token: 0x04002ECD RID: 11981
	public AudioEvent wobbleSound;

	// Token: 0x04002ECE RID: 11982
	public Color bloodColorOverride = new Color(1f, 0.537f, 0.188f);

	// Token: 0x04002ECF RID: 11983
	[Space]
	public GameObject splatChild;

	// Token: 0x04002ED0 RID: 11984
	[Space]
	public Collider2D groundCollider;

	// Token: 0x04002ED1 RID: 11985
	private bool landed;

	// Token: 0x04002ED2 RID: 11986
	private bool broken;

	// Token: 0x04002ED3 RID: 11987
	private tk2dSpriteAnimator anim;
}
