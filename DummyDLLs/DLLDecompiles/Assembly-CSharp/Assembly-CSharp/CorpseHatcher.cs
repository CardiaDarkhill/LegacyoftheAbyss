using System;
using UnityEngine;

// Token: 0x020002B0 RID: 688
public class CorpseHatcher : Corpse
{
	// Token: 0x06001876 RID: 6262 RVA: 0x0007056C File Offset: 0x0006E76C
	protected override void Smash()
	{
		if (!this.hitAcid)
		{
			BloodSpawner.SpawnBlood(base.transform.position, 40, 40, 15f, 20f, 75f, 105f, null, 0f);
			GameObject gameObject = GameObject.FindWithTag("Extra Tag");
			if (gameObject)
			{
				for (int i = 0; i < 2; i++)
				{
					int index = Random.Range(0, gameObject.transform.childCount);
					Transform child = gameObject.transform.GetChild(index);
					if (child)
					{
						child.SetParent(null);
						child.position = base.transform.position;
						FSMUtility.SendEventToGameObject(child.gameObject, "SPAWN", false);
					}
				}
			}
		}
		base.Smash();
	}
}
