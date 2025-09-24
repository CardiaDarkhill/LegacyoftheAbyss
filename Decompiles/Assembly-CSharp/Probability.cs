using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000481 RID: 1153
public static class Probability
{
	// Token: 0x060029A9 RID: 10665 RVA: 0x000B543C File Offset: 0x000B363C
	public static TVal GetRandomItemByProbability<TProb, TVal>(TProb[] array, float[] overrideProbabilities = null) where TProb : Probability.ProbabilityBase<TVal>
	{
		int num;
		return Probability.GetRandomItemByProbability<TProb, TVal>(array, out num, overrideProbabilities, null);
	}

	// Token: 0x060029AA RID: 10666 RVA: 0x000B5454 File Offset: 0x000B3654
	public static TVal GetRandomItemByProbability<TProb, TVal>(TProb[] array, out int chosenIndex, float[] overrideProbabilities = null, IReadOnlyList<bool> conditions = null) where TProb : Probability.ProbabilityBase<TVal>
	{
		Probability.ProbabilityBase<TVal> randomItemRootByProbability = Probability.GetRandomItemRootByProbability<TProb, TVal>(array, out chosenIndex, overrideProbabilities, conditions);
		if (randomItemRootByProbability == null)
		{
			return default(TVal);
		}
		return randomItemRootByProbability.Item;
	}

	// Token: 0x060029AB RID: 10667 RVA: 0x000B5480 File Offset: 0x000B3680
	public static Probability.ProbabilityBase<TVal> GetRandomItemRootByProbability<TProb, TVal>(TProb[] array, float[] overrideProbabilities = null) where TProb : Probability.ProbabilityBase<TVal>
	{
		int num;
		return Probability.GetRandomItemRootByProbability<TProb, TVal>(array, out num, overrideProbabilities, null);
	}

	// Token: 0x060029AC RID: 10668 RVA: 0x000B5498 File Offset: 0x000B3698
	private static Probability.ProbabilityBase<TVal> GetRandomItemRootByProbability<TProb, TVal>(IReadOnlyList<TProb> array, out int chosenIndex, float[] overrideProbabilities = null, IReadOnlyList<bool> conditions = null) where TProb : Probability.ProbabilityBase<TVal>
	{
		int index = -1;
		int count = array.Count;
		if (count <= 1)
		{
			if (count == 1)
			{
				chosenIndex = 0;
				return array[0];
			}
			chosenIndex = -1;
			return null;
		}
		else
		{
			int count2 = array.Count;
			if (overrideProbabilities != null)
			{
				foreach (float item in overrideProbabilities)
				{
					Probability._currentProbabilities.Add(item);
				}
				for (int j = Probability._currentProbabilities.Count; j < count2; j++)
				{
					Probability._currentProbabilities.Add(array[j].Probability);
				}
			}
			else
			{
				foreach (TProb tprob in array)
				{
					Probability._currentProbabilities.Add(tprob.Probability);
				}
			}
			if (Probability._currentPairs.Capacity < count2)
			{
				Probability._currentPairs.Capacity = count2;
			}
			for (int k = 0; k < count2; k++)
			{
				if (conditions == null || k >= conditions.Count || conditions[k])
				{
					Probability._currentPairs.Add(new Probability.SortedPair
					{
						Probability = Probability._currentProbabilities[k],
						Index = k
					});
				}
			}
			count2 = Probability._currentPairs.Count;
			if (count2 == 0)
			{
				chosenIndex = -1;
				return null;
			}
			for (int l = 0; l < count2; l++)
			{
				TProb tprob2 = array[l];
				Probability.ProbabilityBoxed key = new Probability.ProbabilityBoxed(tprob2.Item, tprob2.Probability);
				Probability._currentKvps.Add(new KeyValuePair<Probability.ProbabilityBoxed, Probability.SortedPair>(key, Probability._currentPairs[l]));
			}
			Probability._currentKvps.Sort((KeyValuePair<Probability.ProbabilityBoxed, Probability.SortedPair> x, KeyValuePair<Probability.ProbabilityBoxed, Probability.SortedPair> y) => x.Value.Probability.CompareTo(y.Value.Probability));
			float num = 0f;
			foreach (KeyValuePair<Probability.ProbabilityBoxed, Probability.SortedPair> keyValuePair in Probability._currentKvps)
			{
				num += ((Math.Abs(keyValuePair.Value.Probability) > 0.0001f) ? keyValuePair.Value.Probability : 1f);
			}
			float num2 = Random.Range(0f, num);
			float num3 = 0f;
			for (int m = 0; m < count2; m++)
			{
				if (num2 >= num3)
				{
					index = m;
				}
				num3 += Probability._currentKvps[m].Value.Probability;
			}
			chosenIndex = Probability._currentKvps[index].Value.Index;
			Probability._currentProbabilities.Clear();
			Probability._currentPairs.Clear();
			Probability._currentKvps.Clear();
			return array[chosenIndex];
		}
	}

	// Token: 0x060029AD RID: 10669 RVA: 0x000B5790 File Offset: 0x000B3990
	public static GameObject GetRandomGameObjectByProbability(Probability.ProbabilityGameObject[] array)
	{
		return Probability.GetRandomItemByProbability<Probability.ProbabilityGameObject, GameObject>(array, null);
	}

	// Token: 0x060029AE RID: 10670 RVA: 0x000B579C File Offset: 0x000B399C
	public static TVal GetRandomItemByProbabilityFair<TProb, TVal>(TProb[] items, ref float[] storeProbabilities, float multiplier = 2f) where TProb : Probability.ProbabilityBase<TVal>
	{
		int num;
		return Probability.GetRandomItemByProbabilityFair<TProb, TVal>(items, out num, ref storeProbabilities, multiplier, null);
	}

	// Token: 0x060029AF RID: 10671 RVA: 0x000B57B4 File Offset: 0x000B39B4
	public static TVal GetRandomItemByProbabilityFair<TProb, TVal>(TProb[] items, out int chosenIndex, ref float[] storeProbabilities, float multiplier = 2f, IReadOnlyList<bool> conditions = null) where TProb : Probability.ProbabilityBase<TVal>
	{
		if (storeProbabilities == null || storeProbabilities.Length != items.Length)
		{
			storeProbabilities = new float[items.Length];
			for (int i = 0; i < items.Length; i++)
			{
				storeProbabilities[i] = items[i].Probability;
			}
		}
		TVal randomItemByProbability = Probability.GetRandomItemByProbability<TProb, TVal>(items, out chosenIndex, storeProbabilities, conditions);
		for (int j = 0; j < storeProbabilities.Length; j++)
		{
			storeProbabilities[j] = ((j == chosenIndex) ? items[j].Probability : (storeProbabilities[j] * multiplier));
		}
		return randomItemByProbability;
	}

	// Token: 0x04002A3A RID: 10810
	private const int TEMP_ARRAY_START_CAPACITY = 100;

	// Token: 0x04002A3B RID: 10811
	private static readonly List<float> _currentProbabilities = new List<float>(100);

	// Token: 0x04002A3C RID: 10812
	private static readonly List<Probability.SortedPair> _currentPairs = new List<Probability.SortedPair>(100);

	// Token: 0x04002A3D RID: 10813
	private static readonly List<KeyValuePair<Probability.ProbabilityBoxed, Probability.SortedPair>> _currentKvps = new List<KeyValuePair<Probability.ProbabilityBoxed, Probability.SortedPair>>(100);

	// Token: 0x0200178D RID: 6029
	[Serializable]
	public abstract class ProbabilityBase<T>
	{
		// Token: 0x17000F1D RID: 3869
		// (get) Token: 0x06008DDE RID: 36318
		public abstract T Item { get; }

		// Token: 0x04008E6C RID: 36460
		[Tooltip("If probability = 0, it will be considered 1.")]
		[FormerlySerializedAs("probability")]
		public float Probability = 1f;
	}

	// Token: 0x0200178E RID: 6030
	[Serializable]
	public class ProbabilityGameObject : Probability.ProbabilityBase<GameObject>
	{
		// Token: 0x17000F1E RID: 3870
		// (get) Token: 0x06008DE0 RID: 36320 RVA: 0x0028A96D File Offset: 0x00288B6D
		public override GameObject Item
		{
			get
			{
				return this.Prefab;
			}
		}

		// Token: 0x04008E6D RID: 36461
		[FormerlySerializedAs("prefab")]
		public GameObject Prefab;
	}

	// Token: 0x0200178F RID: 6031
	[Serializable]
	public class ProbabilityInt : Probability.ProbabilityBase<int>
	{
		// Token: 0x17000F1F RID: 3871
		// (get) Token: 0x06008DE2 RID: 36322 RVA: 0x0028A97D File Offset: 0x00288B7D
		public override int Item
		{
			get
			{
				return this.Value;
			}
		}

		// Token: 0x04008E6E RID: 36462
		public int Value;
	}

	// Token: 0x02001790 RID: 6032
	private struct SortedPair
	{
		// Token: 0x04008E6F RID: 36463
		public float Probability;

		// Token: 0x04008E70 RID: 36464
		public int Index;
	}

	// Token: 0x02001791 RID: 6033
	private class ProbabilityBoxed : Probability.ProbabilityBase<object>
	{
		// Token: 0x06008DE4 RID: 36324 RVA: 0x0028A98D File Offset: 0x00288B8D
		public ProbabilityBoxed(object value, float probability)
		{
			this.Item = value;
			this.Probability = probability;
		}

		// Token: 0x17000F20 RID: 3872
		// (get) Token: 0x06008DE5 RID: 36325 RVA: 0x0028A9A3 File Offset: 0x00288BA3
		public override object Item { get; }
	}
}
