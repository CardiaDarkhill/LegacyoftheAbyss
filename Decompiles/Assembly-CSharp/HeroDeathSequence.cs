using System;
using UnityEngine;

// Token: 0x0200018F RID: 399
public sealed class HeroDeathSequence : MonoBehaviour
{
	// Token: 0x17000183 RID: 387
	// (get) Token: 0x06000F85 RID: 3973 RVA: 0x0004AD72 File Offset: 0x00048F72
	public float DeathWait
	{
		get
		{
			return this.deathWait;
		}
	}

	// Token: 0x04000F20 RID: 3872
	[Tooltip("Amount of time to wait before beginning transition to next scene.")]
	[SerializeField]
	private float deathWait = 4.1f;
}
