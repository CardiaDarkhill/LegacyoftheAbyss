using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002B4 RID: 692
[CreateAssetMenu(menuName = "Profiles/Corpse Effects Profile")]
public class CorpseRegularEffectsProfile : ScriptableObject
{
	// Token: 0x17000280 RID: 640
	// (get) Token: 0x06001896 RID: 6294 RVA: 0x00070EA1 File Offset: 0x0006F0A1
	public GameObject[] SpawnOnStart
	{
		get
		{
			return this.spawnOnStart;
		}
	}

	// Token: 0x17000281 RID: 641
	// (get) Token: 0x06001897 RID: 6295 RVA: 0x00070EA9 File Offset: 0x0006F0A9
	public float StunTime
	{
		get
		{
			return this.stunTime;
		}
	}

	// Token: 0x17000282 RID: 642
	// (get) Token: 0x06001898 RID: 6296 RVA: 0x00070EB1 File Offset: 0x0006F0B1
	public GameObject LoopingStunEffectPrefab
	{
		get
		{
			return this.loopingStunEffectPrefab;
		}
	}

	// Token: 0x17000283 RID: 643
	// (get) Token: 0x06001899 RID: 6297 RVA: 0x00070EB9 File Offset: 0x0006F0B9
	public CameraShakeTarget StunEndShake
	{
		get
		{
			return this.stunEndShake;
		}
	}

	// Token: 0x17000284 RID: 644
	// (get) Token: 0x0600189A RID: 6298 RVA: 0x00070EC1 File Offset: 0x0006F0C1
	public GameObject[] SpawnOnStunEnd
	{
		get
		{
			return this.spawnOnStunEnd;
		}
	}

	// Token: 0x17000285 RID: 645
	// (get) Token: 0x0600189B RID: 6299 RVA: 0x00070EC9 File Offset: 0x0006F0C9
	public GameObject[] SpawnOnLand
	{
		get
		{
			return this.spawnOnLand;
		}
	}

	// Token: 0x17000286 RID: 646
	// (get) Token: 0x0600189C RID: 6300 RVA: 0x00070ED1 File Offset: 0x0006F0D1
	public BloodSpawner.Config ExplodeBlood
	{
		get
		{
			return this.explodeBlood;
		}
	}

	// Token: 0x17000287 RID: 647
	// (get) Token: 0x0600189D RID: 6301 RVA: 0x00070ED9 File Offset: 0x0006F0D9
	public GameObject[] SpawnOnExplode
	{
		get
		{
			return this.spawnOnExplode;
		}
	}

	// Token: 0x17000288 RID: 648
	// (get) Token: 0x0600189E RID: 6302 RVA: 0x00070EE1 File Offset: 0x0006F0E1
	// (set) Token: 0x0600189F RID: 6303 RVA: 0x00070EE9 File Offset: 0x0006F0E9
	public CorpseRegularEffectsProfile.EffectList[] ElementalEffects
	{
		get
		{
			return this.elementalEffects;
		}
		set
		{
			this.elementalEffects = value;
		}
	}

	// Token: 0x060018A0 RID: 6304 RVA: 0x00070EF4 File Offset: 0x0006F0F4
	public void EnsurePersonalPool(GameObject gameObject)
	{
		bool flag = false;
		if (this.EnsureArray(this.spawnOnStart, gameObject))
		{
			flag = true;
		}
		if (this.loopingStunEffectPrefab != null)
		{
			PersonalObjectPool.EnsurePooledInScene(gameObject, this.loopingStunEffectPrefab, 1, false, false, false);
			flag = true;
		}
		if (this.EnsureArray(this.spawnOnStunEnd, gameObject))
		{
			flag = true;
		}
		if (this.EnsureArray(this.spawnOnLand, gameObject))
		{
			flag = true;
		}
		if (this.EnsureArray(this.spawnOnExplode, gameObject))
		{
			flag = true;
		}
		if (this.ElementalEffects != null)
		{
			foreach (CorpseRegularEffectsProfile.EffectList effectList in this.ElementalEffects)
			{
				if (effectList != null)
				{
					foreach (GameObject prefab in effectList.effects)
					{
						PersonalObjectPool.EnsurePooledInScene(gameObject, prefab, 1, false, false, false);
						flag = true;
					}
				}
			}
		}
		if (flag)
		{
			PersonalObjectPool.EnsurePooledInSceneFinished(gameObject);
		}
	}

	// Token: 0x060018A1 RID: 6305 RVA: 0x00070FE4 File Offset: 0x0006F1E4
	private bool EnsureArray(GameObject[] prefabArray, GameObject gameObject)
	{
		bool result = false;
		if (prefabArray != null)
		{
			foreach (GameObject prefab in prefabArray)
			{
				PersonalObjectPool.EnsurePooledInScene(gameObject, prefab, 1, false, false, false);
				result = true;
			}
		}
		return result;
	}

	// Token: 0x060018A2 RID: 6306 RVA: 0x00071018 File Offset: 0x0006F218
	private void OnValidate()
	{
		if (this.ElementalEffects == null)
		{
			this.ElementalEffects = new CorpseRegularEffectsProfile.EffectList[0];
		}
		ArrayForEnumAttribute.EnsureArraySize<CorpseRegularEffectsProfile.EffectList>(ref this.elementalEffects, typeof(ElementalEffectType));
	}

	// Token: 0x04001790 RID: 6032
	[SerializeField]
	private GameObject[] spawnOnStart;

	// Token: 0x04001791 RID: 6033
	[SerializeField]
	private float stunTime;

	// Token: 0x04001792 RID: 6034
	[SerializeField]
	private GameObject loopingStunEffectPrefab;

	// Token: 0x04001793 RID: 6035
	[SerializeField]
	private CameraShakeTarget stunEndShake;

	// Token: 0x04001794 RID: 6036
	[SerializeField]
	private GameObject[] spawnOnStunEnd;

	// Token: 0x04001795 RID: 6037
	[SerializeField]
	private GameObject[] spawnOnLand;

	// Token: 0x04001796 RID: 6038
	[SerializeField]
	private BloodSpawner.Config explodeBlood;

	// Token: 0x04001797 RID: 6039
	[SerializeField]
	private GameObject[] spawnOnExplode;

	// Token: 0x04001798 RID: 6040
	[ArrayForEnum(typeof(ElementalEffectType))]
	[SerializeField]
	private CorpseRegularEffectsProfile.EffectList[] elementalEffects = new CorpseRegularEffectsProfile.EffectList[0];

	// Token: 0x020015A0 RID: 5536
	[Serializable]
	public class EffectList
	{
		// Token: 0x04008810 RID: 34832
		public List<GameObject> effects = new List<GameObject>();
	}
}
