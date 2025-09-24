using System;
using UnityEngine;

// Token: 0x02000261 RID: 609
public class PoisonTintTk2dSprite : PoisonTintBase
{
	// Token: 0x17000255 RID: 597
	// (get) Token: 0x060015E1 RID: 5601 RVA: 0x0006279C File Offset: 0x0006099C
	// (set) Token: 0x060015E2 RID: 5602 RVA: 0x000627A9 File Offset: 0x000609A9
	protected override Color Colour
	{
		get
		{
			return this.sprite.color;
		}
		set
		{
			this.sprite.color = value;
		}
	}

	// Token: 0x060015E3 RID: 5603 RVA: 0x000627B7 File Offset: 0x000609B7
	protected override void Awake()
	{
		this.sprite = base.GetComponent<tk2dSprite>();
		base.Awake();
	}

	// Token: 0x060015E4 RID: 5604 RVA: 0x000627CB File Offset: 0x000609CB
	protected override void EnableKeyword(string keyword)
	{
		if (this.sprite)
		{
			this.sprite.EnableKeyword(keyword);
		}
	}

	// Token: 0x060015E5 RID: 5605 RVA: 0x000627E6 File Offset: 0x000609E6
	protected override void DisableKeyword(string keyword)
	{
		if (this.sprite)
		{
			this.sprite.DisableKeyword(keyword);
		}
	}

	// Token: 0x060015E6 RID: 5606 RVA: 0x00062801 File Offset: 0x00060A01
	protected override void SetFloat(int propId, float value)
	{
		if (this.sprite)
		{
			this.sprite.SetFloat(propId, value);
		}
	}

	// Token: 0x0400146F RID: 5231
	private tk2dSprite sprite;
}
