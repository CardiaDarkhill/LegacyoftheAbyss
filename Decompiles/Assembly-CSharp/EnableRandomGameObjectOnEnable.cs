using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200028F RID: 655
public class EnableRandomGameObjectOnEnable : MonoBehaviour
{
	// Token: 0x060016F0 RID: 5872 RVA: 0x00067D1C File Offset: 0x00065F1C
	private void Awake()
	{
		if (this.fairnessKey != string.Empty)
		{
			this.fairnessKey = string.Format("{0}_{1}", this.fairnessKey, this.gameObjects.Length);
		}
	}

	// Token: 0x060016F1 RID: 5873 RVA: 0x00067D54 File Offset: 0x00065F54
	private void OnEnable()
	{
		foreach (EnableRandomGameObjectOnEnable.ProbabilityGameObject probabilityGameObject in this.gameObjects)
		{
			if (probabilityGameObject.Child)
			{
				probabilityGameObject.Child.SetActive(false);
			}
		}
		GameObject gameObject;
		if (this.fairnessKey == string.Empty)
		{
			gameObject = Probability.GetRandomItemByProbability<EnableRandomGameObjectOnEnable.ProbabilityGameObject, GameObject>(this.gameObjects, null);
		}
		else
		{
			float[] value = null;
			float[] array2;
			if (EnableRandomGameObjectOnEnable.fairness.TryGetValue(this.fairnessKey, out array2))
			{
				value = array2;
			}
			gameObject = Probability.GetRandomItemByProbabilityFair<EnableRandomGameObjectOnEnable.ProbabilityGameObject, GameObject>(this.gameObjects, ref value, 2f);
			EnableRandomGameObjectOnEnable.fairness[this.fairnessKey] = value;
		}
		if (gameObject)
		{
			gameObject.SetActive(true);
		}
	}

	// Token: 0x04001586 RID: 5510
	[SerializeField]
	private EnableRandomGameObjectOnEnable.ProbabilityGameObject[] gameObjects;

	// Token: 0x04001587 RID: 5511
	[SerializeField]
	[Tooltip("Others using the same fairness key, with the same amount of gameObjects, will share a fairness tracking array.")]
	private string fairnessKey;

	// Token: 0x04001588 RID: 5512
	private static Dictionary<string, float[]> fairness = new Dictionary<string, float[]>();

	// Token: 0x02001560 RID: 5472
	[Serializable]
	private class ProbabilityGameObject : Probability.ProbabilityBase<GameObject>
	{
		// Token: 0x17000D85 RID: 3461
		// (get) Token: 0x06008695 RID: 34453 RVA: 0x002732D7 File Offset: 0x002714D7
		public override GameObject Item
		{
			get
			{
				return this.Child;
			}
		}

		// Token: 0x040086EE RID: 34542
		public GameObject Child;
	}
}
