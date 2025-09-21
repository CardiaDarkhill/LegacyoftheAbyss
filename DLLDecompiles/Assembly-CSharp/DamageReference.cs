using System;
using UnityEngine;

// Token: 0x02000629 RID: 1577
[CreateAssetMenu(fileName = "New Damage Reference", menuName = "Hornet/Damage Reference")]
public class DamageReference : IntReference
{
	// Token: 0x17000667 RID: 1639
	// (get) Token: 0x0600382F RID: 14383 RVA: 0x000F8441 File Offset: 0x000F6641
	public DamageReference.Targets Target
	{
		get
		{
			return this.target;
		}
	}

	// Token: 0x04003B27 RID: 15143
	public const string NEW_ASSET_LOC = "Data Assets/Damages";

	// Token: 0x04003B28 RID: 15144
	[SerializeField]
	private DamageReference.Targets target;

	// Token: 0x0200193A RID: 6458
	public enum Targets
	{
		// Token: 0x040094E3 RID: 38115
		DamageEnemies,
		// Token: 0x040094E4 RID: 38116
		DamageHero
	}
}
