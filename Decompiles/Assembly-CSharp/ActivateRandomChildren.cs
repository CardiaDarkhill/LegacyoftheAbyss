using System;
using UnityEngine;

// Token: 0x02000052 RID: 82
public class ActivateRandomChildren : MonoBehaviour
{
	// Token: 0x0600022D RID: 557 RVA: 0x0000DAAC File Offset: 0x0000BCAC
	private void OnEnable()
	{
		GameManager instance = GameManager.instance;
		Random random = null;
		if (instance)
		{
			random = instance.SceneSeededRandom;
		}
		if (random == null)
		{
			random = new Random();
		}
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(false);
		}
		for (float num = (float)random.Next(this.amountMin, this.amountMax); num > 0f; num -= 1f)
		{
			int index = random.Next(0, base.transform.childCount);
			Transform child = base.transform.GetChild(index);
			child.gameObject.SetActive(true);
			if (this.deparentAfterActivated)
			{
				child.parent = null;
			}
		}
	}

	// Token: 0x040001E1 RID: 481
	[SerializeField]
	private int amountMin = 1;

	// Token: 0x040001E2 RID: 482
	[SerializeField]
	private int amountMax = 1;

	// Token: 0x040001E3 RID: 483
	[SerializeField]
	private bool deparentAfterActivated = true;
}
