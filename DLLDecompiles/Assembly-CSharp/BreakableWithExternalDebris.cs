using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020004AB RID: 1195
public class BreakableWithExternalDebris : Breakable
{
	// Token: 0x06002B43 RID: 11075 RVA: 0x000BDBEC File Offset: 0x000BBDEC
	protected override void CreateAdditionalDebrisParts(List<GameObject> sourceDebrisParts)
	{
		base.CreateAdditionalDebrisParts(sourceDebrisParts);
		foreach (BreakableWithExternalDebris.ExternalDebris externalDebrisPart in this.externalDebris)
		{
			this.Spawn(externalDebrisPart, sourceDebrisParts);
		}
		BreakableWithExternalDebris.WeightedExternalDebrisItem weightedExternalDebrisItem = this.externalDebrisVariants.SelectValue<BreakableWithExternalDebris.WeightedExternalDebrisItem>();
		if (weightedExternalDebrisItem != null)
		{
			this.Spawn(weightedExternalDebrisItem.Value, sourceDebrisParts);
		}
	}

	// Token: 0x06002B44 RID: 11076 RVA: 0x000BDC44 File Offset: 0x000BBE44
	private void Spawn(BreakableWithExternalDebris.ExternalDebris externalDebrisPart, List<GameObject> debrisParts)
	{
		for (int i = 0; i < externalDebrisPart.Count; i++)
		{
			if (!(externalDebrisPart.Prefab == null))
			{
				GameObject gameObject = Object.Instantiate<GameObject>(externalDebrisPart.Prefab);
				gameObject.GetComponents<IExternalDebris>(BreakableWithExternalDebris._externalDebrisResponders);
				foreach (IExternalDebris externalDebris in BreakableWithExternalDebris._externalDebrisResponders)
				{
					externalDebris.InitExternalDebris();
				}
				BreakableWithExternalDebris._externalDebrisResponders.Clear();
				gameObject.transform.position = base.transform.position + new Vector3(Random.Range(-this.debrisPrefabPositionVariance, this.debrisPrefabPositionVariance), Random.Range(-this.debrisPrefabPositionVariance, this.debrisPrefabPositionVariance), 0f);
				gameObject.SetActive(false);
				debrisParts.Add(gameObject);
			}
		}
	}

	// Token: 0x04002C8E RID: 11406
	[SerializeField]
	private float debrisPrefabPositionVariance;

	// Token: 0x04002C8F RID: 11407
	[SerializeField]
	private BreakableWithExternalDebris.ExternalDebris[] externalDebris;

	// Token: 0x04002C90 RID: 11408
	[SerializeField]
	private BreakableWithExternalDebris.WeightedExternalDebrisItem[] externalDebrisVariants;

	// Token: 0x04002C91 RID: 11409
	private static readonly List<IExternalDebris> _externalDebrisResponders = new List<IExternalDebris>();

	// Token: 0x020017C1 RID: 6081
	[Serializable]
	public struct ExternalDebris
	{
		// Token: 0x04008F4C RID: 36684
		public GameObject Prefab;

		// Token: 0x04008F4D RID: 36685
		public int Count;
	}

	// Token: 0x020017C2 RID: 6082
	[Serializable]
	public class WeightedExternalDebrisItem : WeightedItem
	{
		// Token: 0x04008F4E RID: 36686
		public BreakableWithExternalDebris.ExternalDebris Value;
	}
}
