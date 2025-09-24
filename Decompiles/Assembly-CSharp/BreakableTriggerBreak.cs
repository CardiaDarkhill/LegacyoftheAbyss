using System;
using UnityEngine;

// Token: 0x020004A9 RID: 1193
public class BreakableTriggerBreak : MonoBehaviour
{
	// Token: 0x06002B40 RID: 11072 RVA: 0x000BDB90 File Offset: 0x000BBD90
	private void OnTriggerEnter2D(Collider2D col)
	{
		BreakableTriggerBreak.TriggerConditions triggerConditions = this.triggerCondition;
		int num;
		if (triggerConditions != BreakableTriggerBreak.TriggerConditions.Hero)
		{
			if (triggerConditions != BreakableTriggerBreak.TriggerConditions.HeroSlide)
			{
				goto IL_3C;
			}
			num = 1;
		}
		else
		{
			num = 0;
		}
		int layer = col.gameObject.layer;
		if (layer == 9 || layer == 20)
		{
			if (num == 0)
			{
				goto IL_3C;
			}
			if (num == 1)
			{
				if (!SlideSurface.IsHeroSliding)
				{
					return;
				}
				goto IL_3C;
			}
		}
		return;
		IL_3C:
		this.breakable.BreakSelf();
	}

	// Token: 0x04002C8C RID: 11404
	[SerializeField]
	private Breakable breakable;

	// Token: 0x04002C8D RID: 11405
	[SerializeField]
	private BreakableTriggerBreak.TriggerConditions triggerCondition;

	// Token: 0x020017C0 RID: 6080
	private enum TriggerConditions
	{
		// Token: 0x04008F4A RID: 36682
		Hero,
		// Token: 0x04008F4B RID: 36683
		HeroSlide
	}
}
