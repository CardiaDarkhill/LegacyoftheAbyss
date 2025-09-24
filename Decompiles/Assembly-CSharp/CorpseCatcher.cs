using System;
using UnityEngine;

// Token: 0x020004C8 RID: 1224
public class CorpseCatcher : MonoBehaviour
{
	// Token: 0x06002C0D RID: 11277 RVA: 0x000C0FD8 File Offset: 0x000BF1D8
	private void OnTriggerEnter2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		if (gameObject.layer != 26 && gameObject.layer != 18)
		{
			return;
		}
		Transform parent = collision.transform.parent;
		while (parent != null)
		{
			if (parent.gameObject.layer == 26)
			{
				return;
			}
			parent = parent.parent;
		}
		gameObject.transform.SetParent(base.transform);
		ActiveCorpse activeCorpse;
		if (ActiveCorpse.TryGetCorpse(gameObject, out activeCorpse))
		{
			activeCorpse.SetInert(true);
		}
	}

	// Token: 0x06002C0E RID: 11278 RVA: 0x000C1054 File Offset: 0x000BF254
	private void OnTriggerExit2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		if ((gameObject.layer == 26 || gameObject.layer == 18) && gameObject.transform.parent == base.transform)
		{
			gameObject.transform.SetParent(null);
			if (ObjectPool.ObjectWasSpawned(gameObject))
			{
				Object.DontDestroyOnLoad(gameObject);
			}
		}
		ActiveCorpse activeCorpse;
		if (ActiveCorpse.TryGetCorpse(gameObject, out activeCorpse))
		{
			activeCorpse.SetInert(false);
		}
	}

	// Token: 0x04002D6C RID: 11628
	private const int CORPSE_LAYER = 26;

	// Token: 0x04002D6D RID: 11629
	private const int PARTICLE_LAYER = 18;
}
