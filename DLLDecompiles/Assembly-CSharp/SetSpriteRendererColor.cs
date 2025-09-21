using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200002D RID: 45
public class SetSpriteRendererColor : FSMUtility.GetComponentFsmStateAction<SpriteRenderer>
{
	// Token: 0x06000169 RID: 361 RVA: 0x00008343 File Offset: 0x00006543
	public override void Reset()
	{
		base.Reset();
		this.TintColor = null;
	}

	// Token: 0x0600016A RID: 362 RVA: 0x00008352 File Offset: 0x00006552
	protected override void DoAction(SpriteRenderer spriteRenderer)
	{
		spriteRenderer.color = this.TintColor.Value;
	}

	// Token: 0x04000108 RID: 264
	public FsmColor TintColor;
}
