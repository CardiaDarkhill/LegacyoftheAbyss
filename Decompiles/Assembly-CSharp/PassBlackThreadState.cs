using System;
using UnityEngine;

// Token: 0x0200030E RID: 782
public class PassBlackThreadState : MonoBehaviour
{
	// Token: 0x170002DD RID: 733
	// (get) Token: 0x06001BAF RID: 7087 RVA: 0x0008112B File Offset: 0x0007F32B
	// (set) Token: 0x06001BB0 RID: 7088 RVA: 0x00081133 File Offset: 0x0007F333
	public bool IsBlackThreaded { get; set; }

	// Token: 0x170002DE RID: 734
	// (get) Token: 0x06001BB1 RID: 7089 RVA: 0x0008113C File Offset: 0x0007F33C
	// (set) Token: 0x06001BB2 RID: 7090 RVA: 0x00081144 File Offset: 0x0007F344
	public BlackThreadAttack ChosenAttack { get; set; }
}
