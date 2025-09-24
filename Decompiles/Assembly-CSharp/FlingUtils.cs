using System;
using System.Collections.Generic;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000761 RID: 1889
public static class FlingUtils
{
	// Token: 0x06004349 RID: 17225 RVA: 0x00127F30 File Offset: 0x00126130
	public static void SpawnAndFling(FlingUtils.Config config, Transform spawnPoint, Vector3 positionOffset, List<GameObject> addToList = null, float blackThreadAmount = -1f)
	{
		if (config.Prefab == null)
		{
			return;
		}
		int num = Random.Range(config.AmountMin, config.AmountMax + 1);
		Vector3 a = (spawnPoint != null) ? spawnPoint.TransformPoint(positionOffset) : positionOffset;
		bool flag = addToList != null && num > 0;
		if (flag)
		{
			int capacity = addToList.Capacity;
			int num2 = capacity - addToList.Count;
			if (num > num2)
			{
				addToList.Capacity = capacity + (num - num2);
			}
		}
		for (int i = 0; i < num; i++)
		{
			Vector3 position = a + new Vector3(Random.Range(-config.OriginVariationX, config.OriginVariationX), Random.Range(-config.OriginVariationY, config.OriginVariationY), 0f);
			GameObject gameObject = config.Prefab.Spawn(position);
			if (blackThreadAmount > 0f)
			{
				BlackThreadEffectRendererGroup component = gameObject.GetComponent<BlackThreadEffectRendererGroup>();
				if (component != null)
				{
					component.SetBlackThreadAmount(blackThreadAmount);
				}
			}
			Rigidbody2D component2 = gameObject.GetComponent<Rigidbody2D>();
			if (component2 != null)
			{
				float d = Random.Range(config.SpeedMin, config.SpeedMax);
				float num3 = Random.Range(config.AngleMin, config.AngleMax);
				component2.linearVelocity = new Vector2(Mathf.Cos(num3 * 0.017453292f), Mathf.Sin(num3 * 0.017453292f)) * d;
			}
			if (flag)
			{
				addToList.Add(gameObject);
			}
		}
	}

	// Token: 0x0600434A RID: 17226 RVA: 0x00128098 File Offset: 0x00126298
	public static void SpawnAndFlingShellShards(FlingUtils.Config config, Transform spawnPoint, Vector3 positionOffset, List<GameObject> addToList = null)
	{
		if (config.Prefab == null)
		{
			return;
		}
		int num = Random.Range(config.AmountMin, config.AmountMax + 1);
		Vector3 a = (spawnPoint != null) ? spawnPoint.TransformPoint(positionOffset) : positionOffset;
		bool flag = addToList != null && num > 0;
		if (flag)
		{
			int capacity = addToList.Capacity;
			int num2 = capacity - addToList.Count;
			if (num > num2)
			{
				addToList.Capacity = capacity + (num - num2);
			}
		}
		int num3 = num / 5;
		int num4 = 0;
		while (num3 > 0 && num >= 5 + (num4 + 1) * 2)
		{
			if (Random.value > 0.5f)
			{
				num4++;
				num -= 5;
			}
			num3--;
		}
		if (num4 > 0)
		{
			GameObject shellShardMidPrefab = Gameplay.ShellShardMidPrefab;
			if (shellShardMidPrefab != null)
			{
				for (int i = 0; i < num4; i++)
				{
					Vector3 position = a + new Vector3(Random.Range(-config.OriginVariationX, config.OriginVariationX), Random.Range(-config.OriginVariationY, config.OriginVariationY), 0f);
					GameObject gameObject = shellShardMidPrefab.Spawn(position);
					Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
					if (component != null)
					{
						float d = Random.Range(config.SpeedMin, config.SpeedMax);
						float num5 = Random.Range(config.AngleMin, config.AngleMax);
						component.linearVelocity = new Vector2(Mathf.Cos(num5 * 0.017453292f), Mathf.Sin(num5 * 0.017453292f)) * d;
					}
					if (flag)
					{
						addToList.Add(gameObject);
					}
				}
			}
			else
			{
				num += num4 * 5;
			}
		}
		for (int j = 0; j < num; j++)
		{
			Vector3 position2 = a + new Vector3(Random.Range(-config.OriginVariationX, config.OriginVariationX), Random.Range(-config.OriginVariationY, config.OriginVariationY), 0f);
			GameObject gameObject2 = config.Prefab.Spawn(position2);
			Rigidbody2D component2 = gameObject2.GetComponent<Rigidbody2D>();
			if (component2 != null)
			{
				float d2 = Random.Range(config.SpeedMin, config.SpeedMax);
				float num6 = Random.Range(config.AngleMin, config.AngleMax);
				component2.linearVelocity = new Vector2(Mathf.Cos(num6 * 0.017453292f), Mathf.Sin(num6 * 0.017453292f)) * d2;
			}
			if (flag)
			{
				addToList.Add(gameObject2);
			}
		}
	}

	// Token: 0x0600434B RID: 17227 RVA: 0x001282FB File Offset: 0x001264FB
	public static void EnsurePersonalPool(FlingUtils.Config config, GameObject gameObject)
	{
		if (config.Prefab != null)
		{
			PersonalObjectPool.EnsurePooledInScene(gameObject, config.Prefab, config.AmountMax, false, false, false);
		}
	}

	// Token: 0x0600434C RID: 17228 RVA: 0x00128320 File Offset: 0x00126520
	public static void FlingChildren(FlingUtils.ChildrenConfig config, Transform spawnPoint, Vector3 positionOffset, MinMaxFloat? randomiseZ = null)
	{
		if (config.Parent == null)
		{
			return;
		}
		Vector3 a = (spawnPoint != null) ? spawnPoint.TransformPoint(positionOffset) : positionOffset;
		int childCount = config.Parent.transform.childCount;
		int num = (config.AmountMax > 0) ? Random.Range(config.AmountMin, config.AmountMax) : childCount;
		if (num > childCount)
		{
			num = childCount;
		}
		for (int i = 0; i < num; i++)
		{
			Transform child = config.Parent.transform.GetChild(i);
			if (randomiseZ != null)
			{
				child.SetLocalPositionZ(randomiseZ.Value.GetRandomValue());
			}
			child.gameObject.SetActive(true);
			a + new Vector3(Random.Range(-config.OriginVariationX, config.OriginVariationX), Random.Range(-config.OriginVariationY, config.OriginVariationY), 0f);
			Rigidbody2D component = child.GetComponent<Rigidbody2D>();
			if (component != null)
			{
				float d = Random.Range(config.SpeedMin, config.SpeedMax);
				float num2 = Random.Range(config.AngleMin, config.AngleMax);
				component.linearVelocity = new Vector2(Mathf.Cos(num2 * 0.017453292f), Mathf.Sin(num2 * 0.017453292f)) * d;
			}
		}
	}

	// Token: 0x0600434D RID: 17229 RVA: 0x00128474 File Offset: 0x00126674
	public static void FlingObject(FlingUtils.SelfConfig config, Transform spawnPoint, Vector3 positionOffset)
	{
		if (config.Object == null)
		{
			return;
		}
		Vector3 position = (spawnPoint != null) ? spawnPoint.TransformPoint(positionOffset) : positionOffset;
		config.Object.transform.position = position;
		Rigidbody2D component = config.Object.GetComponent<Rigidbody2D>();
		if (component == null)
		{
			return;
		}
		float d = Random.Range(config.SpeedMin, config.SpeedMax);
		float num = Random.Range(config.AngleMin, config.AngleMax);
		component.linearVelocity = new Vector2(Mathf.Cos(num * 0.017453292f), Mathf.Sin(num * 0.017453292f)) * d;
	}

	// Token: 0x02001A30 RID: 6704
	[Serializable]
	public struct Config
	{
		// Token: 0x040098FC RID: 39164
		public GameObject Prefab;

		// Token: 0x040098FD RID: 39165
		public float SpeedMin;

		// Token: 0x040098FE RID: 39166
		public float SpeedMax;

		// Token: 0x040098FF RID: 39167
		public float AngleMin;

		// Token: 0x04009900 RID: 39168
		public float AngleMax;

		// Token: 0x04009901 RID: 39169
		public float OriginVariationX;

		// Token: 0x04009902 RID: 39170
		public float OriginVariationY;

		// Token: 0x04009903 RID: 39171
		public int AmountMin;

		// Token: 0x04009904 RID: 39172
		public int AmountMax;
	}

	// Token: 0x02001A31 RID: 6705
	[Serializable]
	public struct ChildrenConfig
	{
		// Token: 0x04009905 RID: 39173
		public GameObject Parent;

		// Token: 0x04009906 RID: 39174
		public int AmountMin;

		// Token: 0x04009907 RID: 39175
		public int AmountMax;

		// Token: 0x04009908 RID: 39176
		public float SpeedMin;

		// Token: 0x04009909 RID: 39177
		public float SpeedMax;

		// Token: 0x0400990A RID: 39178
		public float AngleMin;

		// Token: 0x0400990B RID: 39179
		public float AngleMax;

		// Token: 0x0400990C RID: 39180
		public float OriginVariationX;

		// Token: 0x0400990D RID: 39181
		public float OriginVariationY;
	}

	// Token: 0x02001A32 RID: 6706
	public struct SelfConfig
	{
		// Token: 0x0400990E RID: 39182
		public GameObject Object;

		// Token: 0x0400990F RID: 39183
		public float SpeedMin;

		// Token: 0x04009910 RID: 39184
		public float SpeedMax;

		// Token: 0x04009911 RID: 39185
		public float AngleMin;

		// Token: 0x04009912 RID: 39186
		public float AngleMax;
	}

	// Token: 0x02001A33 RID: 6707
	[Serializable]
	public struct ObjectFlingParams
	{
		// Token: 0x06009635 RID: 38453 RVA: 0x002A7C88 File Offset: 0x002A5E88
		public FlingUtils.SelfConfig GetSelfConfig(GameObject flingObject)
		{
			return new FlingUtils.SelfConfig
			{
				Object = flingObject,
				SpeedMin = this.SpeedMin,
				SpeedMax = this.SpeedMax,
				AngleMin = this.AngleMin,
				AngleMax = this.AngleMax
			};
		}

		// Token: 0x04009913 RID: 39187
		public float SpeedMin;

		// Token: 0x04009914 RID: 39188
		public float SpeedMax;

		// Token: 0x04009915 RID: 39189
		public float AngleMin;

		// Token: 0x04009916 RID: 39190
		public float AngleMax;
	}
}
