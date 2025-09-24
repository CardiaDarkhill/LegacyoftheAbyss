using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000489 RID: 1161
public class AirZone : MonoBehaviour
{
	// Token: 0x060029F4 RID: 10740 RVA: 0x000B62C8 File Offset: 0x000B44C8
	private void OnTriggerEnter2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		ActiveCorpse activeCorpse;
		if (collision.gameObject.layer == 26 && ActiveCorpse.TryGetCorpse(collision.gameObject, out activeCorpse))
		{
			activeCorpse.SetInAirZone(true);
		}
		if (collision.gameObject.layer == 18)
		{
			Rigidbody2D component = gameObject.GetComponent<Rigidbody2D>();
			if (component && component.gravityScale <= 2f)
			{
				this.blowParticles.Add(gameObject.GetComponent<Rigidbody2D>());
			}
		}
	}

	// Token: 0x060029F5 RID: 10741 RVA: 0x000B6340 File Offset: 0x000B4540
	private void OnTriggerExit2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		ActiveCorpse activeCorpse;
		if (gameObject.layer == 26 && ActiveCorpse.TryGetCorpse(collision.gameObject, out activeCorpse))
		{
			activeCorpse.SetInAirZone(false);
		}
		if (collision.gameObject.layer == 18 && gameObject.GetComponent<Rigidbody2D>())
		{
			this.blowParticles.Remove(gameObject.GetComponent<Rigidbody2D>());
		}
	}

	// Token: 0x060029F6 RID: 10742 RVA: 0x000B63A4 File Offset: 0x000B45A4
	private void FixedUpdate()
	{
		foreach (Rigidbody2D rigidbody2D in this.blowParticles)
		{
			Vector2 force = new Vector2(Random.Range(-10f, 10f), Random.Range(60f, 125f));
			rigidbody2D.AddForce(force);
		}
	}

	// Token: 0x04002A6A RID: 10858
	private const float forceMinX = -10f;

	// Token: 0x04002A6B RID: 10859
	private const float forceMaxX = 10f;

	// Token: 0x04002A6C RID: 10860
	private const float forceMinY = 60f;

	// Token: 0x04002A6D RID: 10861
	private const float forceMaxY = 125f;

	// Token: 0x04002A6E RID: 10862
	private const int CORPSE_LAYER = 26;

	// Token: 0x04002A6F RID: 10863
	private const int PARTICLE_LAYER = 18;

	// Token: 0x04002A70 RID: 10864
	public List<Rigidbody2D> blowParticles;
}
