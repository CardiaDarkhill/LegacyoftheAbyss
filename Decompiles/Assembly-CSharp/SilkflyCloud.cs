using System;
using UnityEngine;

// Token: 0x020003EC RID: 1004
public class SilkflyCloud : MemoryOrbSource
{
	// Token: 0x17000391 RID: 913
	// (get) Token: 0x0600224E RID: 8782 RVA: 0x0009E24B File Offset: 0x0009C44B
	public int OrbsCount
	{
		get
		{
			return this.orbsParent.childCount;
		}
	}

	// Token: 0x0600224F RID: 8783 RVA: 0x0009E258 File Offset: 0x0009C458
	private void OnDrawGizmos()
	{
		if (!this.orbsParent)
		{
			return;
		}
		Vector3 from = base.transform.position;
		for (int i = 0; i < this.orbsParent.childCount; i++)
		{
			Vector3 position = this.orbsParent.GetChild(i).position;
			Gizmos.DrawLine(from, position);
			from = position;
		}
	}

	// Token: 0x06002250 RID: 8784 RVA: 0x0009E2B0 File Offset: 0x0009C4B0
	private void Start()
	{
		for (int i = 0; i < this.orbsParent.childCount; i++)
		{
			this.orbsParent.GetChild(i).gameObject.SetActive(false);
		}
	}

	// Token: 0x06002251 RID: 8785 RVA: 0x0009E2EA File Offset: 0x0009C4EA
	public void StartTrail()
	{
		this.appearingOrbIndex = 0;
	}

	// Token: 0x06002252 RID: 8786 RVA: 0x0009E2F3 File Offset: 0x0009C4F3
	public GameObject GetNextOrb()
	{
		Component child = this.orbsParent.GetChild(this.appearingOrbIndex);
		this.appearingOrbIndex++;
		return child.gameObject;
	}

	// Token: 0x06002253 RID: 8787 RVA: 0x0009E319 File Offset: 0x0009C519
	public bool IsFinalOrb()
	{
		return this.appearingOrbIndex >= this.orbsParent.childCount;
	}

	// Token: 0x06002254 RID: 8788 RVA: 0x0009E334 File Offset: 0x0009C534
	public void StartTimeAlert()
	{
		for (int i = 0; i < this.orbsParent.childCount; i++)
		{
			this.orbsParent.GetChild(i).GetComponent<MemoryOrb>().StartTimeAlert();
		}
	}

	// Token: 0x06002255 RID: 8789 RVA: 0x0009E370 File Offset: 0x0009C570
	public void Dissipate()
	{
		for (int i = 0; i < this.orbsParent.childCount; i++)
		{
			this.orbsParent.GetChild(i).GetComponent<MemoryOrb>().Dissipate();
		}
	}

	// Token: 0x06002256 RID: 8790 RVA: 0x0009E3AC File Offset: 0x0009C5AC
	public void ActivateUncollectedOrbs(ulong bitmask, int startIndex, out int postIndex)
	{
		for (int i = 0; i < this.orbsParent.childCount; i++)
		{
			bool flag = bitmask.IsBitSet(startIndex + i);
			this.orbsParent.GetChild(i).gameObject.SetActive(!flag);
		}
		postIndex = startIndex + this.orbsParent.childCount;
	}

	// Token: 0x0400211A RID: 8474
	[Space]
	[SerializeField]
	private Transform orbsParent;

	// Token: 0x0400211B RID: 8475
	private int appearingOrbIndex;
}
