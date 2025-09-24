using System;
using UnityEngine;

// Token: 0x020002CA RID: 714
public class EnemyDamagerGroup : MonoBehaviour
{
	// Token: 0x0600198B RID: 6539 RVA: 0x00075154 File Offset: 0x00073354
	private void Awake()
	{
		foreach (DamageEnemies damageEnemies in this.damagers)
		{
			DamageEnemies[] array2 = this.damagers;
			for (int j = 0; j < array2.Length; j++)
			{
				DamageEnemies toDamager = array2[j];
				if (!(damageEnemies == null) && !(toDamager == null) && !(damageEnemies == toDamager))
				{
					damageEnemies.WillDamageEnemyCollider += delegate(Collider2D collider)
					{
						toDamager.PreventDamage(collider);
					};
				}
			}
		}
	}

	// Token: 0x04001889 RID: 6281
	[SerializeField]
	private DamageEnemies[] damagers;
}
