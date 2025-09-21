using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020003ED RID: 1005
public class SilkRationObject : CurrencyObject<SilkRationObject>, IInitialisable, IBreakOnContact
{
	// Token: 0x17000392 RID: 914
	// (get) Token: 0x06002258 RID: 8792 RVA: 0x0009E40C File Offset: 0x0009C60C
	protected override CurrencyType? CurrencyType
	{
		get
		{
			return null;
		}
	}

	// Token: 0x06002259 RID: 8793 RVA: 0x0009E422 File Offset: 0x0009C622
	private void OnValidate()
	{
		if (this.silkPerHit > 0)
		{
			this.hitSilk = new int[]
			{
				this.silkPerHit
			};
			this.silkPerHit = 0;
		}
	}

	// Token: 0x0600225A RID: 8794 RVA: 0x0009E44C File Offset: 0x0009C64C
	public bool OnAwake()
	{
		if (this.hasAwaken)
		{
			return false;
		}
		this.hasAwaken = true;
		GameObject gameObject = base.gameObject;
		if (this.corePrefab != null)
		{
			this.ensureSpawn = true;
			PersonalObjectPool.EnsurePooledInScene(gameObject, this.corePrefab, 1, false, true, false);
		}
		if (this.collectSpawnEffects != null)
		{
			foreach (GameObject gameObject2 in this.collectSpawnEffects)
			{
				if (!(gameObject2 == null))
				{
					this.ensureSpawn = true;
					PersonalObjectPool.EnsurePooledInScene(gameObject, gameObject2, 1, false, true, false);
				}
			}
		}
		if (this.silkGetEffect != null)
		{
			this.ensureSpawn = true;
			PersonalObjectPool.EnsurePooledInScene(gameObject, this.silkGetEffect, 1, false, true, false);
		}
		return true;
	}

	// Token: 0x0600225B RID: 8795 RVA: 0x0009E4F8 File Offset: 0x0009C6F8
	public bool OnStart()
	{
		this.OnAwake();
		if (this.hasOnStarted)
		{
			return false;
		}
		this.hasOnStarted = true;
		if (this.ensureSpawn)
		{
			PersonalObjectPool.EnsurePooledInSceneFinished(base.gameObject);
		}
		return true;
	}

	// Token: 0x0600225C RID: 8796 RVA: 0x0009E526 File Offset: 0x0009C726
	protected override void Awake()
	{
		base.Awake();
		this.OnValidate();
		this.OnAwake();
		this.anim = base.GetComponent<tk2dSpriteAnimator>();
		this.spin = base.GetComponent<SpinSelf>();
	}

	// Token: 0x0600225D RID: 8797 RVA: 0x0009E553 File Offset: 0x0009C753
	private new void Start()
	{
		this.OnStart();
	}

	// Token: 0x0600225E RID: 8798 RVA: 0x0009E55C File Offset: 0x0009C75C
	protected override void OnEnable()
	{
		base.OnEnable();
		this.anim.Play(this.airAnim);
	}

	// Token: 0x0600225F RID: 8799 RVA: 0x0009E578 File Offset: 0x0009C778
	protected override bool Collected()
	{
		Transform transform = base.transform;
		this.AddSilk();
		this.hits++;
		bool flag = this.hits >= this.hitSilk.Length;
		if (this.silkGetEffect != null)
		{
			this.silkGetEffect.Spawn(transform.position, transform.rotation);
		}
		this.collectSpawnEffects.SpawnAll(base.transform.position);
		if (!flag)
		{
			this.anim.Play(this.hitAnim);
			FlingUtils.FlingObject(this.coreFlingParams.GetSelfConfig(base.gameObject), transform, Vector3.zero);
			this.spin.ReSpin();
			return false;
		}
		if (!this.corePrefab)
		{
			return true;
		}
		GameObject flingObject = this.corePrefab.Spawn(transform.position, transform.rotation * this.corePrefab.transform.rotation);
		FlingUtils.FlingObject(this.coreFlingParams.GetSelfConfig(flingObject), transform, Vector3.zero);
		return true;
	}

	// Token: 0x06002260 RID: 8800 RVA: 0x0009E688 File Offset: 0x0009C888
	public void AddSilk()
	{
		int num = Mathf.Clamp(this.hits, 0, this.hitSilk.Length - 1);
		int num2 = this.hitSilk[num];
		HeroController.instance.AddSilk(1, true);
	}

	// Token: 0x06002261 RID: 8801 RVA: 0x0009E6C4 File Offset: 0x0009C8C4
	protected override void Land()
	{
		base.Land();
		if (this.hits > 0)
		{
			return;
		}
		tk2dSpriteAnimationClip clipByName = this.anim.GetClipByName(this.idleAnim);
		if (clipByName != null)
		{
			this.anim.PlayFromFrame(clipByName, Random.Range(0, clipByName.frames.Length));
		}
	}

	// Token: 0x06002262 RID: 8802 RVA: 0x0009E710 File Offset: 0x0009C910
	protected override void HandleHeroEnter(Collider2D collision, GameObject sender)
	{
	}

	// Token: 0x06002264 RID: 8804 RVA: 0x0009E731 File Offset: 0x0009C931
	GameObject IInitialisable.get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0400211C RID: 8476
	[Space]
	[SerializeField]
	[FormerlySerializedAs("value")]
	[Obsolete]
	[HideInInspector]
	private int silkPerHit = 1;

	// Token: 0x0400211D RID: 8477
	[SerializeField]
	private int[] hitSilk = new int[]
	{
		1
	};

	// Token: 0x0400211E RID: 8478
	[Space]
	[SerializeField]
	private string idleAnim;

	// Token: 0x0400211F RID: 8479
	[SerializeField]
	private string airAnim;

	// Token: 0x04002120 RID: 8480
	[SerializeField]
	private string hitAnim;

	// Token: 0x04002121 RID: 8481
	[SerializeField]
	private GameObject corePrefab;

	// Token: 0x04002122 RID: 8482
	[SerializeField]
	private FlingUtils.ObjectFlingParams coreFlingParams;

	// Token: 0x04002123 RID: 8483
	[SerializeField]
	private GameObject[] collectSpawnEffects;

	// Token: 0x04002124 RID: 8484
	[SerializeField]
	private GameObject silkGetEffect;

	// Token: 0x04002125 RID: 8485
	private int hits;

	// Token: 0x04002126 RID: 8486
	private tk2dSpriteAnimator anim;

	// Token: 0x04002127 RID: 8487
	private SpinSelf spin;

	// Token: 0x04002128 RID: 8488
	private bool hasAwaken;

	// Token: 0x04002129 RID: 8489
	private bool hasOnStarted;

	// Token: 0x0400212A RID: 8490
	private bool ensureSpawn;
}
