using System;
using UnityEngine;

// Token: 0x02000260 RID: 608
public class PoisonTintSpriteRenderer : PoisonTintBase
{
	// Token: 0x17000254 RID: 596
	// (get) Token: 0x060015DA RID: 5594 RVA: 0x0006272B File Offset: 0x0006092B
	// (set) Token: 0x060015DB RID: 5595 RVA: 0x00062738 File Offset: 0x00060938
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

	// Token: 0x060015DC RID: 5596 RVA: 0x00062746 File Offset: 0x00060946
	protected override void Awake()
	{
		this.sprite = base.GetComponent<SpriteRenderer>();
		base.Awake();
	}

	// Token: 0x060015DD RID: 5597 RVA: 0x0006275A File Offset: 0x0006095A
	protected override void EnableKeyword(string keyword)
	{
		this.sprite.material.EnableKeyword(keyword);
	}

	// Token: 0x060015DE RID: 5598 RVA: 0x0006276D File Offset: 0x0006096D
	protected override void DisableKeyword(string keyword)
	{
		this.sprite.material.DisableKeyword(keyword);
	}

	// Token: 0x060015DF RID: 5599 RVA: 0x00062780 File Offset: 0x00060980
	protected override void SetFloat(int propId, float value)
	{
		this.sprite.material.SetFloat(propId, value);
	}

	// Token: 0x0400146E RID: 5230
	private SpriteRenderer sprite;
}
