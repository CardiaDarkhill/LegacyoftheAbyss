using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x020003FD RID: 1021
public class LavaCorpseSplash : MonoBehaviour
{
	// Token: 0x060022BE RID: 8894 RVA: 0x0009F9D0 File Offset: 0x0009DBD0
	private void OnTriggerEnter2D(Collider2D collision)
	{
		global::Corpse corpse;
		if (global::Corpse.TryGetCorpse(collision.gameObject, out corpse))
		{
			if (GlobalSettings.Corpse.EnemyLavaDeath)
			{
				GlobalSettings.Corpse.EnemyLavaDeath.Spawn().transform.SetPosition2D(corpse.transform.position);
			}
			corpse.gameObject.SetActive(false);
		}
		ActiveCorpse activeCorpse;
		if (ActiveCorpse.TryGetCorpse(collision.gameObject, out activeCorpse))
		{
			if (GlobalSettings.Corpse.EnemyLavaDeath)
			{
				GlobalSettings.Corpse.EnemyLavaDeath.Spawn().transform.SetPosition2D(activeCorpse.transform.position);
			}
			activeCorpse.gameObject.SetActive(false);
		}
	}
}
