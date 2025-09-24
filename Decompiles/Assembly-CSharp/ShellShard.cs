using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020003EB RID: 1003
public class ShellShard : CurrencyObject<ShellShard>, IBreakOnContact, AntRegion.ICheck
{
	// Token: 0x06002242 RID: 8770 RVA: 0x0009DFC4 File Offset: 0x0009C1C4
	protected override bool Collected()
	{
		CurrencyManager.AddShards(this.value);
		return true;
	}

	// Token: 0x1700038F RID: 911
	// (get) Token: 0x06002243 RID: 8771 RVA: 0x0009DFD2 File Offset: 0x0009C1D2
	protected override CurrencyType? CurrencyType
	{
		get
		{
			return new CurrencyType?(global::CurrencyType.Shard);
		}
	}

	// Token: 0x06002244 RID: 8772 RVA: 0x0009DFDC File Offset: 0x0009C1DC
	protected override void Awake()
	{
		base.Awake();
		this.hasShineEffect = (this.shineEffectProfile != null);
		if (!this.hasShineEffect && this.shineEffectPrefab)
		{
			this.shineEffect = Object.Instantiate<GameObject>(this.shineEffectPrefab, Vector3.zero, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)), base.transform);
			this.shineEffect.transform.localPosition = new Vector3(0f, 0f, -0.0001f);
		}
		this.hasExtrRenderer = (this.extraRendererObjects != null);
	}

	// Token: 0x06002245 RID: 8773 RVA: 0x0009E088 File Offset: 0x0009C288
	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.hasShineEffect)
		{
			if (Random.Range(0f, 1f) <= this.shineSpawnPercentage)
			{
				this.shineRoutine = base.StartCoroutine(this.EnableShineEffect());
				return;
			}
		}
		else if (this.shineEffect)
		{
			this.shineEffect.SetActive(false);
			if (Random.Range(0f, 1f) <= this.shineSpawnPercentage)
			{
				this.shineRoutine = base.StartCoroutine(this.EnableShineEffect());
			}
		}
	}

	// Token: 0x06002246 RID: 8774 RVA: 0x0009E10F File Offset: 0x0009C30F
	protected override void OnDisable()
	{
		base.OnDisable();
		base.StopAllCoroutines();
	}

	// Token: 0x06002247 RID: 8775 RVA: 0x0009E11D File Offset: 0x0009C31D
	protected override void SetRendererActive(bool active)
	{
		base.SetRendererActive(active);
		if (this.hasExtrRenderer)
		{
			this.extraRendererObjects.SetActive(active);
		}
	}

	// Token: 0x06002248 RID: 8776 RVA: 0x0009E13C File Offset: 0x0009C33C
	public override void CollectPopup()
	{
		PlayerData instance = PlayerData.instance;
		if (instance.HasSeenShellShards)
		{
			return;
		}
		instance.HasSeenShellShards = true;
		if (!this.popupName.IsEmpty)
		{
			CollectableUIMsg.Spawn(new UIMsgDisplay
			{
				Name = this.popupName,
				Icon = this.popupSprite,
				IconScale = 1f
			}, null, false);
		}
	}

	// Token: 0x06002249 RID: 8777 RVA: 0x0009E1AD File Offset: 0x0009C3AD
	private IEnumerator EnableShineEffect()
	{
		yield return new WaitForSeconds(this.shineEffectStartDelay.GetRandomValue());
		if (!this.hasShineEffect)
		{
			this.shineEffect.SetActive(true);
			this.shineRoutine = null;
			yield break;
		}
		this.shineEffectProfile.SpawnEffect(new Vector3(0f, 0f, -0.0001f), Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)), base.transform);
		float t = this.shineEffectFrequency.GetRandomValue();
		for (;;)
		{
			t -= Time.deltaTime;
			if (t <= 0f)
			{
				t = this.shineEffectFrequency.GetRandomValue();
				this.shineEffectProfile.SpawnEffect(new Vector3(0f, 0f, -0.0001f), Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)), base.transform);
			}
			yield return null;
		}
	}

	// Token: 0x0600224A RID: 8778 RVA: 0x0009E1BC File Offset: 0x0009C3BC
	public override void BurnUp()
	{
		base.BurnUp();
		if (this.shineRoutine != null)
		{
			base.StopCoroutine(this.shineRoutine);
			this.shineRoutine = null;
		}
		if (this.shineEffect)
		{
			this.shineEffect.SetActive(false);
		}
	}

	// Token: 0x17000390 RID: 912
	// (get) Token: 0x0600224B RID: 8779 RVA: 0x0009E1F8 File Offset: 0x0009C3F8
	public bool CanEnterAntRegion
	{
		get
		{
			return true;
		}
	}

	// Token: 0x0600224C RID: 8780 RVA: 0x0009E1FB File Offset: 0x0009C3FB
	public bool TryGetRenderer(out Renderer renderer)
	{
		if (this.extraRendererObjects)
		{
			renderer = this.extraRendererObjects.GetComponentInChildren<Renderer>();
			if (renderer != null)
			{
				return true;
			}
		}
		renderer = null;
		return false;
	}

	// Token: 0x0400210F RID: 8463
	[Space]
	[SerializeField]
	private GameObject extraRendererObjects;

	// Token: 0x04002110 RID: 8464
	[Space]
	[SerializeField]
	private int value = 1;

	// Token: 0x04002111 RID: 8465
	[SerializeField]
	private PooledEffectProfile shineEffectProfile;

	// Token: 0x04002112 RID: 8466
	[SerializeField]
	private GameObject shineEffectPrefab;

	// Token: 0x04002113 RID: 8467
	[SerializeField]
	[Range(0f, 1f)]
	private float shineSpawnPercentage;

	// Token: 0x04002114 RID: 8468
	[SerializeField]
	private MinMaxFloat shineEffectStartDelay;

	// Token: 0x04002115 RID: 8469
	[SerializeField]
	private MinMaxFloat shineEffectFrequency = new MinMaxFloat(1f, 8f);

	// Token: 0x04002116 RID: 8470
	private GameObject shineEffect;

	// Token: 0x04002117 RID: 8471
	private bool hasShineEffect;

	// Token: 0x04002118 RID: 8472
	private Coroutine shineRoutine;

	// Token: 0x04002119 RID: 8473
	private bool hasExtrRenderer;
}
