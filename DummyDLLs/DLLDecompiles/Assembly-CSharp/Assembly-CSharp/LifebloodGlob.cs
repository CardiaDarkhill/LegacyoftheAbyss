using System;
using System.Collections;
using GlobalSettings;
using UnityEngine;

// Token: 0x02000513 RID: 1299
[RequireComponent(typeof(Collider2D))]
public class LifebloodGlob : MonoBehaviour
{
	// Token: 0x14000092 RID: 146
	// (add) Token: 0x06002E5C RID: 11868 RVA: 0x000CBBD4 File Offset: 0x000C9DD4
	// (remove) Token: 0x06002E5D RID: 11869 RVA: 0x000CBC0C File Offset: 0x000C9E0C
	public event Action PickedUp;

	// Token: 0x06002E5E RID: 11870 RVA: 0x000CBC41 File Offset: 0x000C9E41
	private void Awake()
	{
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
	}

	// Token: 0x06002E5F RID: 11871 RVA: 0x000CBC50 File Offset: 0x000C9E50
	private void OnEnable()
	{
		float num = Random.Range(this.minScale, this.maxScale);
		base.transform.localScale = new Vector3(num, num, 1f);
		if (this.splatChild)
		{
			this.splatChild.SetActive(false);
		}
		if (this.rend)
		{
			this.rend.enabled = true;
		}
		this.anim.Play(this.fallAnim);
		this.landed = false;
		this.broken = false;
		this.ShowPlink(false);
	}

	// Token: 0x06002E60 RID: 11872 RVA: 0x000CBCE0 File Offset: 0x000C9EE0
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
						this.anim.Play(this.landAnim);
						return;
					}
					collision.DoCollisionStay = true;
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
				}
			};
		}
	}

	// Token: 0x06002E61 RID: 11873 RVA: 0x000CBD4C File Offset: 0x000C9F4C
	private void OnTriggerEnter2D(Collider2D col)
	{
		if (!this.landed || this.broken)
		{
			return;
		}
		if (col.tag == "Nail Attack")
		{
			this.DoBreak();
			return;
		}
		if (col.tag == "HeroBox")
		{
			if (this.isPlinkActive)
			{
				this.DoBreak();
				return;
			}
			this.anim.Play(this.wobbleAnim);
		}
	}

	// Token: 0x06002E62 RID: 11874 RVA: 0x000CBDB5 File Offset: 0x000C9FB5
	private IEnumerator Break()
	{
		this.broken = true;
		this.breakSound.SpawnAndPlayOneShot(this.audioPlayerPrefab, base.transform.position, null);
		BloodSpawner.SpawnBlood(base.transform.position, 4, 5, 5f, 20f, 80f, 100f, new Color?(this.bloodColorOverride), 0f);
		this.ShowPlink(false);
		if (this.splatChild)
		{
			this.splatChild.SetActive(true);
		}
		yield return this.anim.PlayAnimWait(this.breakAnim, null);
		if (this.rend)
		{
			this.rend.enabled = false;
		}
		if (this.recycleTime > 0f)
		{
			yield return new WaitForSeconds(this.recycleTime);
		}
		base.gameObject.Recycle();
		yield break;
	}

	// Token: 0x06002E63 RID: 11875 RVA: 0x000CBDC4 File Offset: 0x000C9FC4
	private void DoBreak()
	{
		if (this.broken)
		{
			return;
		}
		if (this.isPlinkActive && this.PickedUp != null)
		{
			this.PickedUp();
		}
		base.StartCoroutine(this.Break());
	}

	// Token: 0x06002E64 RID: 11876 RVA: 0x000CBDF7 File Offset: 0x000C9FF7
	public void ShowPlink(bool value)
	{
		if (this.pickupPlink)
		{
			this.pickupPlink.SetActive(value);
		}
		this.isPlinkActive = value;
	}

	// Token: 0x06002E65 RID: 11877 RVA: 0x000CBE1C File Offset: 0x000CA01C
	public void SetTempQuestHandler(Action questHandler)
	{
		this.ShowPlink(this.IsQuestActive());
		if (this.isPlinkActive)
		{
			Action temp = null;
			temp = delegate()
			{
				if (this.pickup)
				{
					this.pickup.Get(true);
				}
				if (questHandler != null)
				{
					questHandler();
				}
				this.PickedUp -= temp;
			};
			this.PickedUp += temp;
			return;
		}
		if (questHandler != null)
		{
			questHandler();
		}
	}

	// Token: 0x06002E66 RID: 11878 RVA: 0x000CBE8C File Offset: 0x000CA08C
	private bool IsQuestActive()
	{
		Quest lifeBloodQuest = Effects.LifeBloodQuest;
		return lifeBloodQuest && lifeBloodQuest.IsAccepted && !lifeBloodQuest.CanComplete;
	}

	// Token: 0x040030BB RID: 12475
	public Renderer rend;

	// Token: 0x040030BC RID: 12476
	public float recycleTime;

	// Token: 0x040030BD RID: 12477
	[Space]
	public float minScale = 0.6f;

	// Token: 0x040030BE RID: 12478
	public float maxScale = 1.6f;

	// Token: 0x040030BF RID: 12479
	[Space]
	public string fallAnim = "Glob Fall";

	// Token: 0x040030C0 RID: 12480
	public string landAnim = "Glob Land";

	// Token: 0x040030C1 RID: 12481
	public string wobbleAnim = "Glob Wobble";

	// Token: 0x040030C2 RID: 12482
	public string breakAnim = "Glob Break";

	// Token: 0x040030C3 RID: 12483
	[Space]
	public AudioSource audioPlayerPrefab;

	// Token: 0x040030C4 RID: 12484
	public AudioEvent breakSound;

	// Token: 0x040030C5 RID: 12485
	public Color bloodColorOverride = new Color(1f, 0.537f, 0.188f);

	// Token: 0x040030C6 RID: 12486
	[Space]
	public GameObject splatChild;

	// Token: 0x040030C7 RID: 12487
	[Space]
	public Collider2D groundCollider;

	// Token: 0x040030C8 RID: 12488
	[Space]
	public GameObject pickupPlink;

	// Token: 0x040030C9 RID: 12489
	public CollectableItem pickup;

	// Token: 0x040030CA RID: 12490
	private bool landed;

	// Token: 0x040030CB RID: 12491
	private bool broken;

	// Token: 0x040030CC RID: 12492
	private bool isPlinkActive;

	// Token: 0x040030CD RID: 12493
	private tk2dSpriteAnimator anim;
}
