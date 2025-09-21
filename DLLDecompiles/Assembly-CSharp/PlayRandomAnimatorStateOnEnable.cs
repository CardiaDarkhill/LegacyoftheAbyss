using System;
using System.Linq;
using UnityEngine;

// Token: 0x020000A0 RID: 160
public class PlayRandomAnimatorStateOnEnable : MonoBehaviour
{
	// Token: 0x060004F2 RID: 1266 RVA: 0x00019C0D File Offset: 0x00017E0D
	private void Awake()
	{
		this.stateNameHashes = (from stateName in this.stateNames
		select Animator.StringToHash(stateName)).ToArray<int>();
	}

	// Token: 0x060004F3 RID: 1267 RVA: 0x00019C44 File Offset: 0x00017E44
	private void OnEnable()
	{
		if (!this.animator)
		{
			return;
		}
		int num = this.stateNameHashes.Length;
		if (num == 0)
		{
			return;
		}
		this.animator.Play(this.stateNameHashes[Random.Range(0, num)]);
	}

	// Token: 0x040004C5 RID: 1221
	[SerializeField]
	private Animator animator;

	// Token: 0x040004C6 RID: 1222
	[SerializeField]
	private string[] stateNames;

	// Token: 0x040004C7 RID: 1223
	private int[] stateNameHashes;
}
