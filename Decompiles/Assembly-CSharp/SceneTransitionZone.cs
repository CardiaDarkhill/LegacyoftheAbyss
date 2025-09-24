using System;
using UnityEngine;

// Token: 0x020007D2 RID: 2002
public class SceneTransitionZone : SceneTransitionZoneBase
{
	// Token: 0x170007FA RID: 2042
	// (get) Token: 0x0600468A RID: 18058 RVA: 0x00131C09 File Offset: 0x0012FE09
	protected override string TargetScene
	{
		get
		{
			return this.targetScene;
		}
	}

	// Token: 0x170007FB RID: 2043
	// (get) Token: 0x0600468B RID: 18059 RVA: 0x00131C11 File Offset: 0x0012FE11
	protected override string TargetGate
	{
		get
		{
			return this.targetGate;
		}
	}

	// Token: 0x040046F4 RID: 18164
	[SerializeField]
	private string targetScene;

	// Token: 0x040046F5 RID: 18165
	[SerializeField]
	private string targetGate;
}
