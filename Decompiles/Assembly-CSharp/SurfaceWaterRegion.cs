using System;
using System.Collections.Generic;
using GlobalSettings;
using UnityEngine;

// Token: 0x02000565 RID: 1381
public class SurfaceWaterRegion : MonoBehaviour
{
	// Token: 0x14000099 RID: 153
	// (add) Token: 0x06003151 RID: 12625 RVA: 0x000DADEC File Offset: 0x000D8FEC
	// (remove) Token: 0x06003152 RID: 12626 RVA: 0x000DAE24 File Offset: 0x000D9024
	public event Action<HeroController> HeroEntered;

	// Token: 0x1400009A RID: 154
	// (add) Token: 0x06003153 RID: 12627 RVA: 0x000DAE5C File Offset: 0x000D905C
	// (remove) Token: 0x06003154 RID: 12628 RVA: 0x000DAE94 File Offset: 0x000D9094
	public event Action<HeroController> HeroExited;

	// Token: 0x1400009B RID: 155
	// (add) Token: 0x06003155 RID: 12629 RVA: 0x000DAECC File Offset: 0x000D90CC
	// (remove) Token: 0x06003156 RID: 12630 RVA: 0x000DAF04 File Offset: 0x000D9104
	public event Action<Vector2> CorpseEntered;

	// Token: 0x1700055B RID: 1371
	// (get) Token: 0x06003157 RID: 12631 RVA: 0x000DAF39 File Offset: 0x000D9139
	// (set) Token: 0x06003158 RID: 12632 RVA: 0x000DAF41 File Offset: 0x000D9141
	public Color Color
	{
		get
		{
			return this.color;
		}
		set
		{
			this.color = value;
		}
	}

	// Token: 0x1700055C RID: 1372
	// (get) Token: 0x06003159 RID: 12633 RVA: 0x000DAF4A File Offset: 0x000D914A
	public float FlowSpeed
	{
		get
		{
			return this.flowSpeed;
		}
	}

	// Token: 0x1700055D RID: 1373
	// (get) Token: 0x0600315A RID: 12634 RVA: 0x000DAF52 File Offset: 0x000D9152
	public bool UseSpaAnims
	{
		get
		{
			return this.useSpaAnims;
		}
	}

	// Token: 0x1700055E RID: 1374
	// (get) Token: 0x0600315B RID: 12635 RVA: 0x000DAF5A File Offset: 0x000D915A
	private BoxCollider2D Col
	{
		get
		{
			if (this.col == null)
			{
				this.col = base.GetComponent<BoxCollider2D>();
			}
			return this.col;
		}
	}

	// Token: 0x1700055F RID: 1375
	// (get) Token: 0x0600315C RID: 12636 RVA: 0x000DAF7C File Offset: 0x000D917C
	public Bounds Bounds
	{
		get
		{
			return this.Col.bounds;
		}
	}

	// Token: 0x17000560 RID: 1376
	// (get) Token: 0x0600315D RID: 12637 RVA: 0x000DAF89 File Offset: 0x000D9189
	public static int InsideCount
	{
		get
		{
			return SurfaceWaterRegion.INSIDE_REGIONS.Count;
		}
	}

	// Token: 0x0600315E RID: 12638 RVA: 0x000DAF98 File Offset: 0x000D9198
	private void Start()
	{
		this.heroSurfaceY = base.transform.position.y + 0.4f;
		if (!base.transform.eulerAngles.z.IsWithinTolerance(0.1f, 0f))
		{
			this.angled = true;
		}
		this.cameraTarget = GameCameras.instance.cameraTarget;
	}

	// Token: 0x0600315F RID: 12639 RVA: 0x000DAFFC File Offset: 0x000D91FC
	private void Awake()
	{
		PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.spatterPrefab, 100, false, false, false);
		PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.splashInPrefab, 5, false, false, false);
		PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.splashOutPrefab, 5, false, false, false);
		if (this.fireNailExtinguishPrefab)
		{
			PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.fireNailExtinguishPrefab, 2, false, false, false);
		}
		PersonalObjectPool.EnsurePooledInScene(base.gameObject, this.dripperPrefab.gameObject, 5, true, false, false);
	}

	// Token: 0x06003160 RID: 12640 RVA: 0x000DB085 File Offset: 0x000D9285
	private void OnDisable()
	{
		if (this.isHeroInside)
		{
			SurfaceWaterRegion.INSIDE_REGIONS.Remove(this);
		}
		this.isHeroInside = false;
	}

	// Token: 0x06003161 RID: 12641 RVA: 0x000DB0A4 File Offset: 0x000D92A4
	private void OnTriggerEnter2D(Collider2D collision)
	{
		HeroController hero = collision.gameObject.GetComponent<HeroController>();
		if (!hero)
		{
			return;
		}
		if (!hero.isHeroInPosition)
		{
			return;
		}
		if (hero.cState.hazardDeath || hero.cState.hazardRespawning || hero.playerData.isInvincible)
		{
			return;
		}
		if (hero.cState.isBinding && !Gameplay.SpellCrest.IsEquipped)
		{
			Rigidbody2D component = hero.GetComponent<Rigidbody2D>();
			if (component)
			{
				Rigidbody2D body = component;
				float? y = new float?(0f);
				body.SetVelocity(null, y);
			}
			hero.transform.Translate(new Vector3(0f, 0.2f), Space.World);
			return;
		}
		this.heroColliderInside = collision;
		if (GameManager.instance.HasFinishedEnteringScene)
		{
			this.DoTriggerEnter(hero);
			return;
		}
		GameManager.EnterSceneEvent temp = null;
		temp = delegate()
		{
			if (this.heroColliderInside)
			{
				this.DoTriggerEnter(hero);
			}
			GameManager.instance.OnFinishedEnteringScene -= temp;
		};
		GameManager.instance.OnFinishedEnteringScene += temp;
	}

	// Token: 0x06003162 RID: 12642 RVA: 0x000DB1DC File Offset: 0x000D93DC
	private void DoTriggerEnter(HeroController hero)
	{
		HeroWaterController component = hero.GetComponent<HeroWaterController>();
		if (component.IsInWater)
		{
			return;
		}
		if (!this.angled && this.CheckBelowWaterSurface(hero))
		{
			component.Rejected();
			return;
		}
		this.DoSplashHeroIn(hero, component);
	}

	// Token: 0x06003163 RID: 12643 RVA: 0x000DB21C File Offset: 0x000D941C
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (this.heroColliderInside == collision)
		{
			this.heroColliderInside = null;
		}
		if (!this.isHeroInside)
		{
			return;
		}
		HeroController component = collision.gameObject.GetComponent<HeroController>();
		if (!component)
		{
			return;
		}
		this.DoSplashHeroOut(component);
	}

	// Token: 0x06003164 RID: 12644 RVA: 0x000DB264 File Offset: 0x000D9464
	public bool CheckBelowWaterSurface(HeroController hero)
	{
		float num = this.heroSurfaceY - 0.5f;
		Vector3 position = hero.transform.position;
		if (position.y >= num)
		{
			return false;
		}
		Vector2[] positionHistory = hero.PositionHistory;
		for (int i = 0; i < positionHistory.Length; i++)
		{
			if (positionHistory[i].y >= num)
			{
				return false;
			}
		}
		Bounds bounds = this.Bounds;
		return position.x <= bounds.min.x + 0.5f || position.x >= bounds.max.x - 0.5f;
	}

	// Token: 0x06003165 RID: 12645 RVA: 0x000DB300 File Offset: 0x000D9500
	private void DoSplashHeroIn(HeroController hero, HeroWaterController waterControl)
	{
		if (!this.isHeroInside)
		{
			SurfaceWaterRegion.INSIDE_REGIONS.Add(this);
			this.isHeroInside = true;
		}
		bool isBigSplash = hero.cState.willHardLand || hero.cState.isScrewDownAttacking;
		Transform transform = hero.transform;
		if (!this.angled)
		{
			transform.SetPositionY(this.heroSurfaceY);
		}
		if (this.angled)
		{
			this.cameraTarget.SetUpdraft(true);
		}
		this.DoSplashIn(transform, isBigSplash);
		FSMUtility.SendEventToGlobalGameObject("Inventory", "INVENTORY CANCEL");
		if (waterControl)
		{
			waterControl.EnterWaterRegion(this);
		}
		NailElements currentElement = hero.NailImbuement.CurrentElement;
		if (currentElement != NailElements.None && currentElement != NailElements.Poison)
		{
			hero.NailImbuement.SetElement(NailElements.None);
			if (currentElement == NailElements.Fire && this.fireNailExtinguishPrefab)
			{
				this.fireNailExtinguishPrefab.Spawn(hero.transform.position);
			}
		}
		Action<HeroController> heroEntered = this.HeroEntered;
		if (heroEntered == null)
		{
			return;
		}
		heroEntered(hero);
	}

	// Token: 0x06003166 RID: 12646 RVA: 0x000DB3F4 File Offset: 0x000D95F4
	private void DoSplashHeroOut(HeroController hero)
	{
		if (this.isHeroInside)
		{
			SurfaceWaterRegion.INSIDE_REGIONS.Remove(this);
		}
		this.isHeroInside = false;
		this.DoSplashOut(hero.transform, new Vector3(0f, -0.5f, -0.001f));
		HeroWaterController component = hero.GetComponent<HeroWaterController>();
		if (component)
		{
			component.ExitWaterRegion();
		}
		if (this.angled)
		{
			this.cameraTarget.SetUpdraft(false);
		}
		if (this.HeroExited != null)
		{
			this.HeroExited(hero);
		}
	}

	// Token: 0x06003167 RID: 12647 RVA: 0x000DB47E File Offset: 0x000D967E
	public void DoSplashIn(Transform obj, bool isBigSplash)
	{
		this.DoSplashIn(obj, Vector3.zero, isBigSplash);
	}

	// Token: 0x06003168 RID: 12648 RVA: 0x000DB490 File Offset: 0x000D9690
	public void DoSplashIn(Transform obj, Vector3 localPosition, bool isBigSplash)
	{
		this.spawnedObjects.Clear();
		Vector3 vector = obj.TransformPoint(localPosition);
		if (!isBigSplash)
		{
			vector = new Vector3(vector.x, vector.y + 0.3f, vector.z - 0.001f);
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.spatterPrefab,
				AmountMin = 12,
				AmountMax = 15,
				SpeedMin = 8f,
				SpeedMax = 22f,
				AngleMin = 80f,
				AngleMax = 100f,
				OriginVariationX = 0.75f,
				OriginVariationY = 0f
			}, obj, localPosition, this.spawnedObjects, -1f);
			this.splashSound.SpawnAndPlayOneShot(vector, null);
			GameObject gameObject = this.splashInPrefab.Spawn(vector);
			gameObject.transform.SetScaleMatching(2f);
			this.spawnedObjects.Add(gameObject);
		}
		else
		{
			this.bigSplashShake.DoShake(this, true);
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.spatterPrefab,
				AmountMin = 60,
				AmountMax = 60,
				SpeedMin = 10f,
				SpeedMax = 30f,
				AngleMin = 80f,
				AngleMax = 100f,
				OriginVariationX = 1f,
				OriginVariationY = 0f
			}, obj, localPosition + new Vector3(0f, 0f, 0f), this.spawnedObjects, -1f);
			this.bigSplashSound.SpawnAndPlayOneShot(vector, null);
			GameObject gameObject2 = this.splashInPrefab.Spawn(vector);
			gameObject2.transform.SetScaleMatching(2.5f);
			this.spawnedObjects.Add(gameObject2);
			GameObject gameObject3 = this.splashOutPrefab.Spawn(vector);
			gameObject3.transform.SetScaleMatching(2f);
			this.spawnedObjects.Add(gameObject3);
		}
		this.SetSpawnedGameObjectColorsTemp(this.spawnedObjects);
		this.spawnedObjects.Clear();
	}

	// Token: 0x06003169 RID: 12649 RVA: 0x000DB6BC File Offset: 0x000D98BC
	public void DoSplashInSmall(Transform obj, Vector3 localPosition)
	{
		this.spawnedObjects.Clear();
		Vector3 position = obj.TransformPoint(localPosition);
		FlingUtils.SpawnAndFling(new FlingUtils.Config
		{
			Prefab = this.spatterPrefab,
			AmountMin = 3,
			AmountMax = 4,
			SpeedMin = 6f,
			SpeedMax = 15f,
			AngleMin = 80f,
			AngleMax = 100f,
			OriginVariationX = 0.75f,
			OriginVariationY = 0f
		}, obj, localPosition, this.spawnedObjects, -1f);
		this.splashSound.SpawnAndPlayOneShot(position, null);
		GameObject gameObject = this.splashInPrefab.Spawn(position);
		gameObject.transform.SetScaleMatching(1.25f);
		this.spawnedObjects.Add(gameObject);
		this.SetSpawnedGameObjectColorsTemp(this.spawnedObjects);
		this.spawnedObjects.Clear();
	}

	// Token: 0x0600316A RID: 12650 RVA: 0x000DB7AC File Offset: 0x000D99AC
	public void DoSplashOut(Transform obj, Vector2 effectsOffset)
	{
		this.spawnedObjects.Clear();
		this.exitSound.SpawnAndPlayOneShot(obj.position, null);
		if (this.dripperPrefab)
		{
			Dripper dripper = this.dripperPrefab.Spawn(obj.transform.position);
			dripper.StartDripper(obj);
			this.spawnedObjects.Add(dripper.gameObject);
		}
		this.spawnedObjects.Add(this.splashOutPrefab.Spawn(obj.position + effectsOffset));
		this.SetSpawnedGameObjectColorsTemp(this.spawnedObjects);
	}

	// Token: 0x0600316B RID: 12651 RVA: 0x000DB846 File Offset: 0x000D9A46
	public void CorpseSplashedIn(Vector2 splashPos)
	{
		Action<Vector2> corpseEntered = this.CorpseEntered;
		if (corpseEntered == null)
		{
			return;
		}
		corpseEntered(splashPos);
	}

	// Token: 0x0600316C RID: 12652 RVA: 0x000DB85C File Offset: 0x000D9A5C
	private void SetSpawnedGameObjectColorsTemp(List<GameObject> gameObjects)
	{
		foreach (GameObject gameObject in gameObjects)
		{
			RecycleResetHandler recycleResetHandler = gameObject.GetComponent<RecycleResetHandler>() ?? gameObject.AddComponent<RecycleResetHandler>();
			SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer>();
			if (sprite)
			{
				Color initialColor = sprite.color;
				sprite.color = this.color;
				recycleResetHandler.AddTempAction(delegate()
				{
					sprite.color = initialColor;
				});
			}
			tk2dSprite tk2dSprite = gameObject.GetComponent<tk2dSprite>();
			if (tk2dSprite)
			{
				Color initialColor = tk2dSprite.color;
				tk2dSprite.color = this.color;
				recycleResetHandler.AddTempAction(delegate()
				{
					tk2dSprite.color = initialColor;
				});
			}
			Dripper dripper = gameObject.GetComponent<Dripper>();
			if (dripper)
			{
				dripper.OnSpawned += this.SetSpawnedGameObjectColorsTemp;
				recycleResetHandler.AddTempAction(delegate()
				{
					dripper.OnSpawned -= this.SetSpawnedGameObjectColorsTemp;
				});
			}
		}
	}

	// Token: 0x0600316D RID: 12653 RVA: 0x000DB9EC File Offset: 0x000D9BEC
	private bool TryReentryInternal(HeroWaterController waterControl, HeroController hero)
	{
		if (!this.angled && this.CheckBelowWaterSurface(hero))
		{
			waterControl.Rejected();
			return false;
		}
		this.DoSplashHeroIn(hero, waterControl);
		return true;
	}

	// Token: 0x0600316E RID: 12654 RVA: 0x000DBA10 File Offset: 0x000D9C10
	public static void TryReentry(HeroWaterController waterControl, HeroController hero)
	{
		SurfaceWaterRegion.INSIDE_REGIONS.ReserveListUsage();
		using (List<SurfaceWaterRegion>.Enumerator enumerator = SurfaceWaterRegion.INSIDE_REGIONS.List.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.TryReentryInternal(waterControl, hero))
				{
					return;
				}
			}
		}
		SurfaceWaterRegion.INSIDE_REGIONS.ReleaseListUsage();
	}

	// Token: 0x0600316F RID: 12655 RVA: 0x000DBA80 File Offset: 0x000D9C80
	private void DrawGizmos()
	{
		float y = base.transform.position.y + 0.4f;
		Bounds bounds = this.Bounds;
		Gizmos.color = Color.blue;
		GizmoUtility.DrawCollider2D(this.col);
		Gizmos.color = Color.green;
		Vector3 min = bounds.min;
		Vector3 max = bounds.max;
		min.y = y;
		max.y = y;
		Gizmos.DrawLine(min, max);
		Gizmos.color = Color.red;
		min.y -= 0.5f;
		max.y -= 0.5f;
		Gizmos.DrawLine(min, max);
	}

	// Token: 0x06003170 RID: 12656 RVA: 0x000DBB22 File Offset: 0x000D9D22
	private void OnDrawGizmosSelected()
	{
		this.DrawGizmos();
	}

	// Token: 0x040034AF RID: 13487
	private const float ORIGINAL_HERO_OFFSET = 0.4f;

	// Token: 0x040034B3 RID: 13491
	[Header("Main Config")]
	[SerializeField]
	private Color color;

	// Token: 0x040034B4 RID: 13492
	[Space]
	[SerializeField]
	private float flowSpeed;

	// Token: 0x040034B5 RID: 13493
	[SerializeField]
	private bool useSpaAnims;

	// Token: 0x040034B6 RID: 13494
	[Header("Prefab")]
	[SerializeField]
	private GameObject splashInPrefab;

	// Token: 0x040034B7 RID: 13495
	[SerializeField]
	private GameObject spatterPrefab;

	// Token: 0x040034B8 RID: 13496
	[SerializeField]
	private Dripper dripperPrefab;

	// Token: 0x040034B9 RID: 13497
	[SerializeField]
	private AudioEvent splashSound;

	// Token: 0x040034BA RID: 13498
	[SerializeField]
	private AudioEvent bigSplashSound;

	// Token: 0x040034BB RID: 13499
	[SerializeField]
	private CameraShakeTarget bigSplashShake;

	// Token: 0x040034BC RID: 13500
	[SerializeField]
	private GameObject splashOutPrefab;

	// Token: 0x040034BD RID: 13501
	[SerializeField]
	private AudioEvent exitSound;

	// Token: 0x040034BE RID: 13502
	[SerializeField]
	private GameObject fireNailExtinguishPrefab;

	// Token: 0x040034BF RID: 13503
	private Collider2D heroColliderInside;

	// Token: 0x040034C0 RID: 13504
	private CameraTarget cameraTarget;

	// Token: 0x040034C1 RID: 13505
	private bool isHeroInside;

	// Token: 0x040034C2 RID: 13506
	private float heroSurfaceY;

	// Token: 0x040034C3 RID: 13507
	private readonly List<GameObject> spawnedObjects = new List<GameObject>();

	// Token: 0x040034C4 RID: 13508
	private bool angled;

	// Token: 0x040034C5 RID: 13509
	private BoxCollider2D col;

	// Token: 0x040034C6 RID: 13510
	private static readonly UniqueList<SurfaceWaterRegion> INSIDE_REGIONS = new UniqueList<SurfaceWaterRegion>();

	// Token: 0x040034C7 RID: 13511
	private const float enterTopThreshold = 0.5f;
}
