using System;
using System.Linq;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020005B6 RID: 1462
public class SavedFleaActivator : MonoBehaviour
{
	// Token: 0x0600347B RID: 13435 RVA: 0x000E94CC File Offset: 0x000E76CC
	private void Start()
	{
		int activeCount = PlayerData.instance.GetVariables(this.pdBoolTemplate).Count((bool val) => val);
		Transform[] array = this.fleaParents;
		for (int i = 0; i < array.Length; i++)
		{
			SavedFleaActivator.ActivateFleas(array[i], activeCount, out activeCount);
		}
	}

	// Token: 0x0600347C RID: 13436 RVA: 0x000E9530 File Offset: 0x000E7730
	private static void ActivateFleas(Transform fleaParent, int activeCount, out int remaining)
	{
		remaining = activeCount;
		if (activeCount > fleaParent.childCount)
		{
			activeCount = fleaParent.childCount;
		}
		foreach (object obj in fleaParent)
		{
			((Transform)obj).gameObject.SetActive(false);
		}
		int num = 0;
		while (activeCount > 0)
		{
			num++;
			if (num > 100)
			{
				break;
			}
			GameObject gameObject = fleaParent.GetChild(Random.Range(0, fleaParent.childCount)).gameObject;
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
				activeCount--;
				remaining--;
			}
		}
	}

	// Token: 0x040037F2 RID: 14322
	[SerializeField]
	private Transform[] fleaParents;

	// Token: 0x040037F3 RID: 14323
	[SerializeField]
	private string pdBoolTemplate;
}
