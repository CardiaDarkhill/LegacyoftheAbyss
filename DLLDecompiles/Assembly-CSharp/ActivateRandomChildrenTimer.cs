using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000053 RID: 83
public class ActivateRandomChildrenTimer : MonoBehaviour
{
	// Token: 0x0600022F RID: 559 RVA: 0x0000DBB4 File Offset: 0x0000BDB4
	private void OnEnable()
	{
		foreach (object obj in base.transform)
		{
			((Transform)obj).gameObject.SetActive(false);
		}
		base.StartCoroutine(this.Routine());
	}

	// Token: 0x06000230 RID: 560 RVA: 0x0000DC20 File Offset: 0x0000BE20
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06000231 RID: 561 RVA: 0x0000DC28 File Offset: 0x0000BE28
	private IEnumerator Routine()
	{
		for (;;)
		{
			if (this.isEnabled)
			{
				yield return new WaitForSeconds(this.delayBetween.GetRandomValue());
				this.temp.Clear();
				this.temp.Capacity = base.transform.childCount;
				foreach (object obj in base.transform)
				{
					Transform item = (Transform)obj;
					this.temp.Add(item);
				}
				this.temp.Shuffle<Transform>();
				int randomValue = this.activateCount.GetRandomValue(true);
				for (int i = 0; i < Mathf.Min(randomValue, this.temp.Count); i++)
				{
					this.temp[i].gameObject.SetActive(true);
				}
			}
		}
		yield break;
	}

	// Token: 0x06000232 RID: 562 RVA: 0x0000DC37 File Offset: 0x0000BE37
	public void Disable()
	{
		base.StopAllCoroutines();
		this.isEnabled = false;
	}

	// Token: 0x040001E4 RID: 484
	[SerializeField]
	private MinMaxFloat delayBetween;

	// Token: 0x040001E5 RID: 485
	[SerializeField]
	private MinMaxInt activateCount;

	// Token: 0x040001E6 RID: 486
	private bool isEnabled = true;

	// Token: 0x040001E7 RID: 487
	private readonly List<Transform> temp = new List<Transform>();
}
