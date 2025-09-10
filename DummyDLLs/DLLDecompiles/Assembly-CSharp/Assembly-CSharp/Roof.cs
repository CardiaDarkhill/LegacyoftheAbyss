using System;
using UnityEngine;

// Token: 0x02000542 RID: 1346
public class Roof : NonSlider
{
	// Token: 0x06003035 RID: 12341 RVA: 0x000D4CD4 File Offset: 0x000D2ED4
	protected void OnCollisionEnter2D(Collision2D collision)
	{
		GameObject gameObject = collision.gameObject;
		if (!gameObject.CompareTag("Player"))
		{
			return;
		}
		gameObject.SendMessage("CancelSuperDash", SendMessageOptions.DontRequireReceiver);
		gameObject.SendMessage("CancelHeroJump", SendMessageOptions.DontRequireReceiver);
	}

	// Token: 0x04003312 RID: 13074
	private const string PlayerTag = "Player";

	// Token: 0x04003313 RID: 13075
	private const string CancelSuperDashMethod = "CancelSuperDash";

	// Token: 0x04003314 RID: 13076
	private const string CancelHeroJumpMethod = "CancelHeroJump";
}
