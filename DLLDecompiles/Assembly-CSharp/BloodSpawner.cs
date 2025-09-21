using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x02000144 RID: 324
public static class BloodSpawner
{
	// Token: 0x060009FA RID: 2554 RVA: 0x0002D2EC File Offset: 0x0002B4EC
	public static GameObject SpawnBlood(BloodSpawner.Config config, Transform spawnPoint, Color? colorOverride = null)
	{
		return BloodSpawner.SpawnBlood(config.Position + spawnPoint.position, config.MinCount, config.MaxCount, config.MinSpeed, config.MaxSpeed, config.AngleMin, config.AngleMax, colorOverride, config.SizeMultiplier);
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x0002D33C File Offset: 0x0002B53C
	public static GameObject SpawnBlood(BloodSpawner.GeneralConfig config, Vector3 position)
	{
		return BloodSpawner.SpawnBlood(position, config.MinCount, config.MaxCount, config.MinSpeed, config.MaxSpeed, config.AngleMin, config.AngleMax, new Color?(config.Colour), config.SizeMultiplier);
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x0002D384 File Offset: 0x0002B584
	public static GameObject SpawnBlood(Vector3 position, short minCount, short maxCount, float minSpeed, float maxSpeed, float angleMin = 0f, float angleMax = 360f, Color? colorOverride = null, float sizeMultiplier = 0f)
	{
		GameObject bloodParticlePrefab = Effects.BloodParticlePrefab;
		if (bloodParticlePrefab && maxCount > 0)
		{
			GameObject gameObject = bloodParticlePrefab.Spawn();
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			if (component)
			{
				BloodSpawner.<>c__DisplayClass4_0 CS$<>8__locals1 = new BloodSpawner.<>c__DisplayClass4_0();
				component.Stop();
				component.emission.SetBursts(new ParticleSystem.Burst[]
				{
					new ParticleSystem.Burst(0f, minCount, maxCount)
				});
				CS$<>8__locals1.main = component.main;
				CS$<>8__locals1.main.maxParticles = Mathf.RoundToInt((float)maxCount);
				CS$<>8__locals1.main.startSpeed = new ParticleSystem.MinMaxCurve(minSpeed, maxSpeed);
				if (colorOverride != null)
				{
					ParticleSystem.MinMaxGradient initialColor = CS$<>8__locals1.main.startColor;
					CS$<>8__locals1.main.startColor = new ParticleSystem.MinMaxGradient(colorOverride.Value);
					RecycleResetHandler.Add(component.gameObject, delegate()
					{
						CS$<>8__locals1.main.startColor = initialColor;
					});
				}
				if (sizeMultiplier != 0f)
				{
					float initialMultiplier = CS$<>8__locals1.main.startSizeMultiplier;
					BloodSpawner.<>c__DisplayClass4_0 CS$<>8__locals4 = CS$<>8__locals1;
					CS$<>8__locals4.main.startSizeMultiplier = CS$<>8__locals4.main.startSizeMultiplier * sizeMultiplier;
					RecycleResetHandler.Add(component.gameObject, delegate()
					{
						CS$<>8__locals1.main.startSizeMultiplier = initialMultiplier;
					});
				}
				component.shape.arc = angleMax - angleMin;
				component.transform.SetRotation2D(angleMin);
				component.transform.position = position;
				component.Play();
			}
			return gameObject;
		}
		return null;
	}

	// Token: 0x02001476 RID: 5238
	[Serializable]
	public struct Config
	{
		// Token: 0x0400834E RID: 33614
		public Vector3 Position;

		// Token: 0x0400834F RID: 33615
		public short MinCount;

		// Token: 0x04008350 RID: 33616
		public short MaxCount;

		// Token: 0x04008351 RID: 33617
		public float MinSpeed;

		// Token: 0x04008352 RID: 33618
		public float MaxSpeed;

		// Token: 0x04008353 RID: 33619
		public float AngleMin;

		// Token: 0x04008354 RID: 33620
		public float AngleMax;

		// Token: 0x04008355 RID: 33621
		public float SizeMultiplier;
	}

	// Token: 0x02001477 RID: 5239
	[Serializable]
	public struct GeneralConfig
	{
		// Token: 0x04008356 RID: 33622
		public Color Colour;

		// Token: 0x04008357 RID: 33623
		public short MinCount;

		// Token: 0x04008358 RID: 33624
		public short MaxCount;

		// Token: 0x04008359 RID: 33625
		public float MinSpeed;

		// Token: 0x0400835A RID: 33626
		public float MaxSpeed;

		// Token: 0x0400835B RID: 33627
		public float AngleMin;

		// Token: 0x0400835C RID: 33628
		public float AngleMax;

		// Token: 0x0400835D RID: 33629
		public float SizeMultiplier;
	}
}
