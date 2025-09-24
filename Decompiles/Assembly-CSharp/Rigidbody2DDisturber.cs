using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200053F RID: 1343
public class Rigidbody2DDisturber : Rigidbody2DDisturberBase
{
	// Token: 0x06003028 RID: 12328 RVA: 0x000D485B File Offset: 0x000D2A5B
	public void StartRumble()
	{
		this.StopRumble();
		this.rumbleRoutine = base.StartCoroutine(this.Rumble());
	}

	// Token: 0x06003029 RID: 12329 RVA: 0x000D4875 File Offset: 0x000D2A75
	public void StopRumble()
	{
		if (this.rumbleRoutine != null)
		{
			base.StopCoroutine(this.rumbleRoutine);
			this.rumbleRoutine = null;
		}
	}

	// Token: 0x0600302A RID: 12330 RVA: 0x000D4892 File Offset: 0x000D2A92
	private IEnumerator Rumble()
	{
		WaitForSeconds wait = new WaitForSeconds(1f / this.frequency);
		for (;;)
		{
			foreach (Rigidbody2D rigidbody2D in this.bodies)
			{
				Vector2 vector = new Vector2(Random.Range(-this.force.x, this.force.x), Random.Range(-this.force.y, this.force.y));
				rigidbody2D.AddForce(vector, ForceMode2D.Impulse);
			}
			yield return wait;
		}
		yield break;
	}

	// Token: 0x040032FA RID: 13050
	[SerializeField]
	private Vector2 force;

	// Token: 0x040032FB RID: 13051
	[SerializeField]
	private float frequency = 60f;

	// Token: 0x040032FC RID: 13052
	private Coroutine rumbleRoutine;
}
