using System;
using UnityEngine;

// Token: 0x02000615 RID: 1557
public class CharmItem : MonoBehaviour
{
	// Token: 0x06003780 RID: 14208 RVA: 0x000F4BD8 File Offset: 0x000F2DD8
	public int GetListNumber()
	{
		return this.listNumber;
	}

	// Token: 0x06003781 RID: 14209 RVA: 0x000F4BE0 File Offset: 0x000F2DE0
	public void SetListNumber(int newNumber)
	{
		this.listNumber = newNumber;
	}

	// Token: 0x04003A79 RID: 14969
	public int listNumber;
}
