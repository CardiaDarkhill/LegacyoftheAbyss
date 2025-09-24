using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x020002DD RID: 733
[CreateAssetMenu(menuName = "Profiles/Enemy Hit Effects Profile")]
public class EnemyHitEffectsProfile : ScriptableObject
{
	// Token: 0x170002AB RID: 683
	// (get) Token: 0x060019F7 RID: 6647 RVA: 0x000778ED File Offset: 0x00075AED
	public bool DoHitFlash
	{
		get
		{
			return this.Get().doHitFlash;
		}
	}

	// Token: 0x170002AC RID: 684
	// (get) Token: 0x060019F8 RID: 6648 RVA: 0x000778FA File Offset: 0x00075AFA
	public bool OverrideHitFlashColor
	{
		get
		{
			return this.Get().overrideHitFlashColor;
		}
	}

	// Token: 0x170002AD RID: 685
	// (get) Token: 0x060019F9 RID: 6649 RVA: 0x00077907 File Offset: 0x00075B07
	public Color HitFlashColor
	{
		get
		{
			return this.Get().hitFlashColor;
		}
	}

	// Token: 0x060019FA RID: 6650 RVA: 0x00077914 File Offset: 0x00075B14
	private EnemyHitEffectsProfile Get()
	{
		if (this.conditionalAlts == null || this.conditionalAlts.Length == 0)
		{
			return this;
		}
		HeroControllerConfig config = HeroController.instance.Config;
		foreach (EnemyHitEffectsProfile.ConditionalAlt conditionalAlt in this.conditionalAlts)
		{
			if (conditionalAlt.HeroConfig == config)
			{
				return conditionalAlt.Profile;
			}
		}
		return this;
	}

	// Token: 0x060019FB RID: 6651 RVA: 0x00077970 File Offset: 0x00075B70
	public void SpawnEffects(Transform spawnPoint, Vector3 offset, HitInstance damageInstance, Color? bloodColorOverride = null, float blackThreadAmount = -1f)
	{
		EnemyHitEffectsProfile.ProfileSection profileSection;
		switch (damageInstance.HitEffectsType)
		{
		case EnemyHitEffectsProfile.EffectsTypes.Full:
			profileSection = this.Get().FullEffects;
			break;
		case EnemyHitEffectsProfile.EffectsTypes.Minimal:
			profileSection = this.Get().MinimalEffects;
			break;
		case EnemyHitEffectsProfile.EffectsTypes.LagHit:
			profileSection = this.Get().LagHitEffects;
			break;
		default:
			throw new NotImplementedException();
		}
		profileSection.SpawnEffects(spawnPoint, offset, damageInstance, bloodColorOverride, blackThreadAmount);
	}

	// Token: 0x060019FC RID: 6652 RVA: 0x000779D5 File Offset: 0x00075BD5
	public void EnsurePersonalPool(GameObject gameObject)
	{
		this.EnsurePersonalPool(gameObject, this.FullEffects);
		this.EnsurePersonalPool(gameObject, this.MinimalEffects);
		this.EnsurePersonalPool(gameObject, this.LagHitEffects);
		PersonalObjectPool.CreateIfRequired(gameObject, false);
	}

	// Token: 0x060019FD RID: 6653 RVA: 0x00077A08 File Offset: 0x00075C08
	private void EnsurePersonalPool(GameObject gameObject, EnemyHitEffectsProfile.ProfileSection profileSection)
	{
		foreach (EnemyHitEffectsProfile.HitFlingConfig hitFlingConfig in profileSection.SpawnFlings)
		{
			if (!(hitFlingConfig.Prefab == null))
			{
				PersonalObjectPool.EnsurePooledInScene(gameObject, hitFlingConfig.Prefab, 3, false, false, false);
			}
		}
		foreach (GameObject gameObject2 in profileSection.SpawnEffectPrefabs)
		{
			if (!(gameObject2 == null))
			{
				PersonalObjectPool.EnsurePooledInScene(gameObject, gameObject2, 3, false, false, false);
			}
		}
	}

	// Token: 0x040018EE RID: 6382
	[SerializeField]
	private EnemyHitEffectsProfile.ProfileSection FullEffects;

	// Token: 0x040018EF RID: 6383
	[Space]
	[SerializeField]
	private EnemyHitEffectsProfile.ProfileSection MinimalEffects;

	// Token: 0x040018F0 RID: 6384
	[Space]
	[SerializeField]
	private EnemyHitEffectsProfile.ProfileSection LagHitEffects;

	// Token: 0x040018F1 RID: 6385
	[Space]
	[SerializeField]
	private bool doHitFlash = true;

	// Token: 0x040018F2 RID: 6386
	[SerializeField]
	private bool overrideHitFlashColor;

	// Token: 0x040018F3 RID: 6387
	[SerializeField]
	[ModifiableProperty]
	[Conditional("overrideHitFlashColor", true, false, false)]
	private Color hitFlashColor;

	// Token: 0x040018F4 RID: 6388
	[Space]
	[SerializeField]
	private EnemyHitEffectsProfile.ConditionalAlt[] conditionalAlts;

	// Token: 0x020015BA RID: 5562
	public enum EffectsTypes
	{
		// Token: 0x04008850 RID: 34896
		Full,
		// Token: 0x04008851 RID: 34897
		Minimal,
		// Token: 0x04008852 RID: 34898
		LagHit
	}

	// Token: 0x020015BB RID: 5563
	[Serializable]
	public struct HitFlingConfig
	{
		// Token: 0x04008853 RID: 34899
		public GameObject Prefab;

		// Token: 0x04008854 RID: 34900
		public float SpeedMin;

		// Token: 0x04008855 RID: 34901
		public float SpeedMax;

		// Token: 0x04008856 RID: 34902
		public float OriginVariationX;

		// Token: 0x04008857 RID: 34903
		public float OriginVariationY;

		// Token: 0x04008858 RID: 34904
		public int AmountMin;

		// Token: 0x04008859 RID: 34905
		public int AmountMax;

		// Token: 0x0400885A RID: 34906
		public float AngleWidth;

		// Token: 0x0400885B RID: 34907
		[Range(0f, 1f)]
		public float ProbabilitySelf;
	}

	// Token: 0x020015BC RID: 5564
	[Serializable]
	public struct HitBloodConfig
	{
		// Token: 0x0400885C RID: 34908
		public short MinCount;

		// Token: 0x0400885D RID: 34909
		public short MaxCount;

		// Token: 0x0400885E RID: 34910
		public float MinSpeed;

		// Token: 0x0400885F RID: 34911
		public float MaxSpeed;

		// Token: 0x04008860 RID: 34912
		public float AngleWidth;
	}

	// Token: 0x020015BD RID: 5565
	[Serializable]
	public class ProfileSection
	{
		// Token: 0x060087E0 RID: 34784 RVA: 0x00278208 File Offset: 0x00276408
		public void SpawnEffects(Transform spawnPoint, Vector3 offset, HitInstance hitInstance, Color? bloodColorOverride, float blackThreadAmount = -1f)
		{
			Vector3 position = spawnPoint.TransformPoint(offset);
			float num;
			for (num = hitInstance.GetActualDirection(spawnPoint, HitInstance.TargetType.Regular); num < 0f; num += 360f)
			{
			}
			if (num.IsWithinTolerance(40f, 270f))
			{
				num = 90f;
			}
			foreach (EnemyHitEffectsProfile.HitBloodConfig hitBloodConfig in this.Blood)
			{
				float num2 = hitBloodConfig.AngleWidth / 2f;
				GameObject gameObject = BloodSpawner.SpawnBlood(new BloodSpawner.Config
				{
					Position = offset,
					MinCount = hitBloodConfig.MinCount,
					MaxCount = hitBloodConfig.MaxCount,
					MinSpeed = hitBloodConfig.MinSpeed,
					MaxSpeed = hitBloodConfig.MaxSpeed,
					AngleMin = num - num2,
					AngleMax = num + num2
				}, spawnPoint, bloodColorOverride);
				if (gameObject)
				{
					FollowTransform follow = gameObject.GetComponent<FollowTransform>() ?? gameObject.AddComponent<FollowTransform>();
					follow.Target = spawnPoint;
					follow.Offset = offset;
					RecycleResetHandler.Add(gameObject, delegate()
					{
						follow.Target = null;
					});
				}
			}
			foreach (AudioEvent audioEvent in this.DamageSounds)
			{
				audioEvent.SpawnAndPlayOneShot(position, null);
			}
			this.PlayRandomVibration();
			foreach (GameObject gameObject2 in this.SpawnEffectPrefabs)
			{
				if (gameObject2)
				{
					GameObject gameObject3 = gameObject2.Spawn(position);
					if (blackThreadAmount > 0f)
					{
						BlackThreadEffectRendererGroup component = gameObject3.GetComponent<BlackThreadEffectRendererGroup>();
						if (component != null)
						{
							component.SetBlackThreadAmount(blackThreadAmount);
						}
					}
					gameObject3.transform.SetRotation2D(num);
				}
			}
			foreach (EnemyHitEffectsProfile.HitFlingConfig hitFlingConfig in this.SpawnFlings)
			{
				if (Random.Range(0f, 1f) <= hitFlingConfig.ProbabilitySelf)
				{
					float num3 = hitFlingConfig.AngleWidth / 2f;
					float angleMin = num - num3;
					float angleMax = num + num3;
					FlingUtils.SpawnAndFling(new FlingUtils.Config
					{
						Prefab = hitFlingConfig.Prefab,
						AmountMin = hitFlingConfig.AmountMin,
						AmountMax = hitFlingConfig.AmountMax,
						SpeedMin = hitFlingConfig.SpeedMin,
						SpeedMax = hitFlingConfig.SpeedMax,
						AngleMin = angleMin,
						AngleMax = angleMax,
						OriginVariationX = hitFlingConfig.OriginVariationX,
						OriginVariationY = hitFlingConfig.OriginVariationY
					}, spawnPoint, offset, null, -1f);
				}
			}
			if (hitInstance.RageHit)
			{
				GameObject rageHitEffectPrefab = Effects.RageHitEffectPrefab;
				if (rageHitEffectPrefab)
				{
					rageHitEffectPrefab.Spawn(position).transform.SetRotation2D(num);
				}
			}
		}

		// Token: 0x060087E1 RID: 34785 RVA: 0x002784EC File Offset: 0x002766EC
		private void PlayRandomVibration()
		{
			if (this.Vibrations.Length != 0)
			{
				int num = Random.Range(0, this.Vibrations.Length);
				VibrationManager.PlayVibrationClipOneShot(this.Vibrations[num % this.Vibrations.Length], null, false, "", false);
			}
		}

		// Token: 0x04008861 RID: 34913
		public EnemyHitEffectsProfile.HitBloodConfig[] Blood = new EnemyHitEffectsProfile.HitBloodConfig[0];

		// Token: 0x04008862 RID: 34914
		public GameObject[] SpawnEffectPrefabs = new GameObject[0];

		// Token: 0x04008863 RID: 34915
		public EnemyHitEffectsProfile.HitFlingConfig[] SpawnFlings = new EnemyHitEffectsProfile.HitFlingConfig[0];

		// Token: 0x04008864 RID: 34916
		public AudioEvent[] DamageSounds = new AudioEvent[0];

		// Token: 0x04008865 RID: 34917
		public VibrationDataAsset[] Vibrations = new VibrationDataAsset[0];
	}

	// Token: 0x020015BE RID: 5566
	[Serializable]
	private class ConditionalAlt
	{
		// Token: 0x04008866 RID: 34918
		public HeroControllerConfig HeroConfig;

		// Token: 0x04008867 RID: 34919
		public EnemyHitEffectsProfile Profile;
	}
}
