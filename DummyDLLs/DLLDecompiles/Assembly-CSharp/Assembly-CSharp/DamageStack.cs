using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002C8 RID: 712
public class DamageStack
{
	// Token: 0x170002A1 RID: 673
	// (get) Token: 0x06001982 RID: 6530 RVA: 0x0007501E File Offset: 0x0007321E
	// (set) Token: 0x06001983 RID: 6531 RVA: 0x00075026 File Offset: 0x00073226
	public int BaseDamage { get; private set; }

	// Token: 0x06001984 RID: 6532 RVA: 0x0007502F File Offset: 0x0007322F
	public void SetupNew(int baseDamage)
	{
		this.BaseDamage = baseDamage;
		this.multipliers.Clear();
		this.totalOffset = 0f;
	}

	// Token: 0x06001985 RID: 6533 RVA: 0x0007504E File Offset: 0x0007324E
	public void AddMultiplier(float multiplier)
	{
		this.multipliers.Add(multiplier);
	}

	// Token: 0x06001986 RID: 6534 RVA: 0x0007505C File Offset: 0x0007325C
	public void AddOffset(float offset)
	{
		this.totalOffset += offset;
	}

	// Token: 0x06001987 RID: 6535 RVA: 0x0007506C File Offset: 0x0007326C
	public int PopDamage()
	{
		float num = (float)this.BaseDamage;
		for (int i = 0; i < this.multipliers.Count; i++)
		{
			float num2 = this.multipliers[i];
			float value = num * num2 - num;
			this.multipliers[i] = value;
		}
		foreach (float num3 in this.multipliers)
		{
			num += num3;
		}
		num += this.totalOffset;
		this.SetupNew(0);
		return Mathf.RoundToInt(num);
	}

	// Token: 0x04001886 RID: 6278
	private readonly List<float> multipliers = new List<float>();

	// Token: 0x04001887 RID: 6279
	private float totalOffset;
}
