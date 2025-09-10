using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000547 RID: 1351
public class RuinsLift : MonoBehaviour
{
	// Token: 0x06003054 RID: 12372 RVA: 0x000D5764 File Offset: 0x000D3964
	private void Start()
	{
		if (this.chainsParent)
		{
			int childCount = this.chainsParent.childCount;
			this.chains = new List<Transform>(childCount);
			for (int i = 0; i < childCount; i++)
			{
				Transform child = this.chainsParent.GetChild(i);
				if (child.gameObject.activeSelf)
				{
					SpriteRenderer component = child.GetComponent<SpriteRenderer>();
					if (!component || (component.enabled && !(component.sprite == null)))
					{
						this.chains.Add(child);
					}
				}
			}
			this.chains.Sort((Transform a, Transform b) => a.transform.position.y.CompareTo(b.transform.position.y));
			this.chains.Reverse();
			base.StartCoroutine(this.HideChains());
		}
	}

	// Token: 0x06003055 RID: 12373 RVA: 0x000D5832 File Offset: 0x000D3A32
	private IEnumerator HideChains()
	{
		List<float> list = new List<float>(this.stopPositions);
		list.Sort();
		float minYPos = list[0];
		float maxYPos = list[list.Count - 1] - minYPos;
		float lastYPos = 0f;
		for (;;)
		{
			yield return null;
			if (base.transform.position.y != lastYPos)
			{
				lastYPos = base.transform.position.y;
				int num = Mathf.FloorToInt((base.transform.position.y - minYPos) / maxYPos * (float)this.chains.Count);
				for (int i = 0; i < this.chains.Count; i++)
				{
					this.chains[i].gameObject.SetActive(i >= num);
				}
			}
		}
		yield break;
	}

	// Token: 0x06003056 RID: 12374 RVA: 0x000D5841 File Offset: 0x000D3A41
	public float GetPositionY(int position)
	{
		position--;
		if (position < 0 || position + 1 > this.stopPositions.Length)
		{
			position = 0;
		}
		return this.stopPositions[position];
	}

	// Token: 0x06003057 RID: 12375 RVA: 0x000D5864 File Offset: 0x000D3A64
	public bool IsCurrentPositionTerminus(int position)
	{
		bool result = false;
		if (position == 1 || position == this.stopPositions.Length)
		{
			result = true;
		}
		return result;
	}

	// Token: 0x06003058 RID: 12376 RVA: 0x000D5885 File Offset: 0x000D3A85
	public int KeepInBounds(int position)
	{
		position--;
		if (position < 0 || position + 1 > this.stopPositions.Length)
		{
			position = 0;
		}
		return position + 1;
	}

	// Token: 0x04003338 RID: 13112
	public float[] stopPositions;

	// Token: 0x04003339 RID: 13113
	public Transform chainsParent;

	// Token: 0x0400333A RID: 13114
	private List<Transform> chains;
}
