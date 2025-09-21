using System;
using UnityEngine;

// Token: 0x0200043F RID: 1087
public sealed class ScreenFaderState : MonoBehaviour
{
	// Token: 0x170003FE RID: 1022
	// (get) Token: 0x0600259A RID: 9626 RVA: 0x000AB7F5 File Offset: 0x000A99F5
	public static float Alpha
	{
		get
		{
			if (!ScreenFaderState.hasInstance)
			{
				return 0f;
			}
			if (!ScreenFaderState.instance.spriteRenderer.enabled)
			{
				return 0f;
			}
			return ScreenFaderState.instance.spriteRenderer.color.a;
		}
	}

	// Token: 0x0600259B RID: 9627 RVA: 0x000AB82F File Offset: 0x000A9A2F
	private void Awake()
	{
		if (ScreenFaderState.instance == null)
		{
			ScreenFaderState.instance = this;
			ScreenFaderState.hasInstance = true;
			if (this.spriteRenderer == null)
			{
				this.spriteRenderer = base.GetComponent<SpriteRenderer>();
			}
		}
	}

	// Token: 0x0600259C RID: 9628 RVA: 0x000AB864 File Offset: 0x000A9A64
	private void OnValidate()
	{
		if (this.spriteRenderer == null)
		{
			this.spriteRenderer = base.GetComponent<SpriteRenderer>();
		}
	}

	// Token: 0x0600259D RID: 9629 RVA: 0x000AB880 File Offset: 0x000A9A80
	private void OnDestroy()
	{
		if (ScreenFaderState.instance == this)
		{
			ScreenFaderState.hasInstance = false;
			ScreenFaderState.instance = null;
		}
	}

	// Token: 0x04002329 RID: 9001
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	// Token: 0x0400232A RID: 9002
	private static ScreenFaderState instance;

	// Token: 0x0400232B RID: 9003
	private static bool hasInstance;
}
