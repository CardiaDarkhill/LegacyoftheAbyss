using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x020004A1 RID: 1185
public class BreakableBreaker : MonoBehaviour
{
	// Token: 0x06002B0C RID: 11020 RVA: 0x000BC081 File Offset: 0x000BA281
	private void OnTriggerEnter2D(Collider2D collision)
	{
		this.HandleCollider(collision);
	}

	// Token: 0x06002B0D RID: 11021 RVA: 0x000BC08A File Offset: 0x000BA28A
	private void OnCollisionEnter2D(Collision2D collision)
	{
		this.HandleCollider(collision.collider);
	}

	// Token: 0x06002B0E RID: 11022 RVA: 0x000BC098 File Offset: 0x000BA298
	private void HandleCollider(Collider2D collision)
	{
		IBreakerBreakable component = collision.GetComponent<IBreakerBreakable>();
		if (component == null)
		{
			return;
		}
		if (!this.breakTypeMask.IsBitSet((int)component.BreakableType))
		{
			return;
		}
		if (this.breakInstantly)
		{
			component.BreakFromBreaker(this);
			return;
		}
		component.HitFromBreaker(this);
	}

	// Token: 0x04002C29 RID: 11305
	[SerializeField]
	[EnumPickerBitmask(typeof(BreakableBreaker.BreakableTypes))]
	private int breakTypeMask = -1;

	// Token: 0x04002C2A RID: 11306
	[SerializeField]
	private bool breakInstantly = true;

	// Token: 0x020017BA RID: 6074
	public enum BreakableTypes
	{
		// Token: 0x04008F2E RID: 36654
		Basic,
		// Token: 0x04008F2F RID: 36655
		Grass
	}
}
