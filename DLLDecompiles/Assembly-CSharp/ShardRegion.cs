using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class ShardRegion : DebugDrawColliderRuntimeAdder, ICurrencyLimitRegion
{
	// Token: 0x17000098 RID: 152
	// (get) Token: 0x06000742 RID: 1858 RVA: 0x00023AAE File Offset: 0x00021CAE
	public CurrencyType CurrencyType
	{
		get
		{
			return CurrencyType.Shard;
		}
	}

	// Token: 0x17000099 RID: 153
	// (get) Token: 0x06000743 RID: 1859 RVA: 0x00023AB1 File Offset: 0x00021CB1
	public int Limit
	{
		get
		{
			return this.limit;
		}
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x00023ABC File Offset: 0x00021CBC
	protected override void Awake()
	{
		base.Awake();
		this.box = base.GetComponent<BoxCollider2D>();
		this.hasBox = this.box;
		this.architectProbabilities = new float[this.dropChances.Length];
		for (int i = 0; i < this.dropChances.Length; i++)
		{
			ShardRegion.ProbabilityShardDrop probabilityShardDrop = this.dropChances[i];
			float num = probabilityShardDrop.Probability;
			if (probabilityShardDrop.Item > 0)
			{
				num *= this.architectCrestChanceMultiplier;
			}
			this.architectProbabilities[i] = num;
		}
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x00023B3B File Offset: 0x00021D3B
	private void Start()
	{
		this.hc = HeroController.instance;
		this.hasHC = (this.hc != null);
	}

	// Token: 0x06000746 RID: 1862 RVA: 0x00023B5A File Offset: 0x00021D5A
	private void OnEnable()
	{
		NailSlashTerrainThunk.AnyThunked += this.OnNailSlashTerrainThunked;
		CurrencyObjectLimitRegion.AddRegion(this);
	}

	// Token: 0x06000747 RID: 1863 RVA: 0x00023B73 File Offset: 0x00021D73
	private void OnDisable()
	{
		NailSlashTerrainThunk.AnyThunked -= this.OnNailSlashTerrainThunked;
		CurrencyObjectLimitRegion.RemoveRegion(this);
	}

	// Token: 0x06000748 RID: 1864 RVA: 0x00023B8C File Offset: 0x00021D8C
	private void OnNailSlashTerrainThunked(Vector2 thunkPos, int surfaceDir)
	{
		if (!this.hasBox)
		{
			return;
		}
		if (!this.box.OverlapPoint(thunkPos))
		{
			return;
		}
		float[] overrideProbabilities = null;
		if (this.hasHC && this.hc.IsArchitectCrestEquipped())
		{
			overrideProbabilities = this.architectProbabilities;
		}
		int randomItemByProbability = Probability.GetRandomItemByProbability<ShardRegion.ProbabilityShardDrop, int>(this.dropChances, overrideProbabilities);
		if (this.dropEffectPrefab)
		{
			this.dropEffectPrefab.Spawn(thunkPos);
		}
		this.hitSound.SpawnAndPlayOneShot(thunkPos, null);
		if (randomItemByProbability <= 0)
		{
			return;
		}
		FlingUtils.ObjectFlingParams objectFlingParams = this.shardFling;
		switch (surfaceDir)
		{
		case 0:
			thunkPos += new Vector2(0.5f, 0f);
			break;
		case 1:
			thunkPos += new Vector2(0f, 0.5f);
			objectFlingParams.AngleMin -= 90f;
			objectFlingParams.AngleMax -= 90f;
			break;
		case 2:
			thunkPos += new Vector2(-0.5f, 0f);
			objectFlingParams.AngleMin += 180f;
			objectFlingParams.AngleMax += 180f;
			break;
		case 3:
			thunkPos += new Vector2(0f, -0.5f);
			objectFlingParams.AngleMin += 90f;
			objectFlingParams.AngleMax += 90f;
			break;
		}
		for (int i = 0; i < randomItemByProbability; i++)
		{
			GameObject flingObject = Gameplay.ShellShardPrefab.Spawn(thunkPos);
			FlingUtils.FlingObject(objectFlingParams.GetSelfConfig(flingObject), null, thunkPos);
		}
	}

	// Token: 0x06000749 RID: 1865 RVA: 0x00023D2D File Offset: 0x00021F2D
	public bool IsInsideLimitRegion(Vector2 point)
	{
		return this.hasBox && this.box.OverlapPoint(point);
	}

	// Token: 0x0600074A RID: 1866 RVA: 0x00023D45 File Offset: 0x00021F45
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.ShardRegion, false);
	}

	// Token: 0x04000709 RID: 1801
	[SerializeField]
	private ShardRegion.ProbabilityShardDrop[] dropChances;

	// Token: 0x0400070A RID: 1802
	[SerializeField]
	private float architectCrestChanceMultiplier = 0.33333334f;

	// Token: 0x0400070B RID: 1803
	[SerializeField]
	private FlingUtils.ObjectFlingParams shardFling;

	// Token: 0x0400070C RID: 1804
	[SerializeField]
	private GameObject dropEffectPrefab;

	// Token: 0x0400070D RID: 1805
	[SerializeField]
	private AudioEventRandom hitSound;

	// Token: 0x0400070E RID: 1806
	[Space]
	[SerializeField]
	private int limit = 30;

	// Token: 0x0400070F RID: 1807
	private BoxCollider2D box;

	// Token: 0x04000710 RID: 1808
	private bool hasBox;

	// Token: 0x04000711 RID: 1809
	private float[] architectProbabilities;

	// Token: 0x04000712 RID: 1810
	private bool hasHC;

	// Token: 0x04000713 RID: 1811
	private HeroController hc;

	// Token: 0x0200144C RID: 5196
	[Serializable]
	private class ProbabilityShardDrop : Probability.ProbabilityBase<int>
	{
		// Token: 0x17000CB9 RID: 3257
		// (get) Token: 0x06008320 RID: 33568 RVA: 0x00268356 File Offset: 0x00266556
		public override int Item
		{
			get
			{
				return this.dropAmount;
			}
		}

		// Token: 0x040082BF RID: 33471
		[SerializeField]
		private int dropAmount;
	}
}
