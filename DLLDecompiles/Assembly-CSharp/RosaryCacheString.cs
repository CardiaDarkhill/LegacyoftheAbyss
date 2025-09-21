using System;
using System.Collections.Generic;
using System.Linq;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000546 RID: 1350
public class RosaryCacheString : RosaryCacheHanging
{
	// Token: 0x17000551 RID: 1361
	// (get) Token: 0x0600304F RID: 12367 RVA: 0x000D554C File Offset: 0x000D374C
	protected override int HitCount
	{
		get
		{
			return this.maxHits;
		}
	}

	// Token: 0x06003050 RID: 12368 RVA: 0x000D5554 File Offset: 0x000D3754
	private void OnValidate()
	{
		if (this.maxHits <= 0)
		{
			this.maxHits = 1;
		}
	}

	// Token: 0x06003051 RID: 12369 RVA: 0x000D5568 File Offset: 0x000D3768
	protected override void Awake()
	{
		base.Awake();
		RosaryCache.HitState hitState = this.hitStates[0];
		for (int i = 0; i < this.maxHits - 1; i++)
		{
			this.hitStates.Insert(0, hitState.Duplicate());
		}
		this.flingObjects = this.rosaryGroups.SelectMany((RosaryCacheString.RosaryGroup group) => from obj in @group.RepresentingObjects
		select new RosaryCacheString.FlintObject(obj, @group)).ToList<RosaryCacheString.FlintObject>();
		for (int j = 0; j < this.maxHits; j++)
		{
			bool isLast = j == this.maxHits - 1;
			this.hitStates[j].OnActivated.AddListener(delegate()
			{
				this.FlingRosaries(isLast);
			});
		}
	}

	// Token: 0x06003052 RID: 12370 RVA: 0x000D5634 File Offset: 0x000D3834
	private void FlingRosaries(bool isLast)
	{
		int num = Mathf.FloorToInt((float)this.flingObjects.Count / (float)this.maxHits);
		num -= Random.Range(-1, 2);
		if (!isLast)
		{
			for (int i = 0; i < num; i++)
			{
				num = Mathf.Min(num, this.flingObjects.Count);
				if (num == 0)
				{
					return;
				}
				int index = Random.Range(0, num);
				RosaryCacheString.FlintObject flintObject = this.flingObjects[index];
				this.flingObjects.RemoveAt(index);
				flintObject.Obj.SetActive(false);
				if (!base.IsReactivating)
				{
					flintObject.Group.FlingPrefab(flintObject.Obj.transform, Vector3.zero);
				}
			}
			return;
		}
		foreach (RosaryCacheString.FlintObject flintObject2 in this.flingObjects)
		{
			flintObject2.Obj.SetActive(false);
			if (!base.IsReactivating)
			{
				flintObject2.Group.FlingPrefab(flintObject2.Obj.transform, Vector3.zero);
			}
		}
	}

	// Token: 0x04003335 RID: 13109
	[Header("String")]
	[SerializeField]
	private RosaryCacheString.RosaryGroup[] rosaryGroups;

	// Token: 0x04003336 RID: 13110
	[SerializeField]
	private int maxHits;

	// Token: 0x04003337 RID: 13111
	private List<RosaryCacheString.FlintObject> flingObjects = new List<RosaryCacheString.FlintObject>();

	// Token: 0x0200184C RID: 6220
	[Serializable]
	private class RosaryGroup
	{
		// Token: 0x060090A9 RID: 37033 RVA: 0x00295E0C File Offset: 0x0029400C
		public void FlingPrefab(Transform spawnPoint, Vector3 offset)
		{
			FlingUtils.SpawnAndFling(new FlingUtils.Config
			{
				Prefab = this.SpawnPrefab,
				AmountMin = 1,
				AmountMax = 1,
				SpeedMin = this.Speed.Start,
				SpeedMax = this.Speed.End,
				AngleMin = this.Angle.Start,
				AngleMax = this.Angle.End
			}, spawnPoint, offset, null, -1f);
		}

		// Token: 0x04009180 RID: 37248
		public GameObject[] RepresentingObjects;

		// Token: 0x04009181 RID: 37249
		public GameObject SpawnPrefab;

		// Token: 0x04009182 RID: 37250
		public MinMaxFloat Speed;

		// Token: 0x04009183 RID: 37251
		public MinMaxFloat Angle;
	}

	// Token: 0x0200184D RID: 6221
	private struct FlintObject
	{
		// Token: 0x060090AB RID: 37035 RVA: 0x00295E9C File Offset: 0x0029409C
		public FlintObject(GameObject obj, RosaryCacheString.RosaryGroup group)
		{
			this.Obj = obj;
			this.Group = group;
		}

		// Token: 0x04009184 RID: 37252
		public GameObject Obj;

		// Token: 0x04009185 RID: 37253
		public RosaryCacheString.RosaryGroup Group;
	}
}
