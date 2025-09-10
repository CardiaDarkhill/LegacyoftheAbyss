using System;
using UnityEngine;

// Token: 0x02000214 RID: 532
public class AmbientLightAnimator : MonoBehaviour
{
	// Token: 0x060013BD RID: 5053 RVA: 0x00059F57 File Offset: 0x00058157
	private void Start()
	{
		this.sm = GameCameras.instance.sceneColorManager;
		this.initialValue = this.sm.AmbientIntensityA;
		this.sm.AmbientIntensityA = this.value;
		this.oldValue = this.value;
	}

	// Token: 0x060013BE RID: 5054 RVA: 0x00059F97 File Offset: 0x00058197
	private void Update()
	{
		if (this.value != this.oldValue)
		{
			this.sm.AmbientIntensityA = this.value;
			this.oldValue = this.value;
		}
	}

	// Token: 0x04001228 RID: 4648
	[SerializeField]
	private float value;

	// Token: 0x04001229 RID: 4649
	private float initialValue;

	// Token: 0x0400122A RID: 4650
	private float oldValue;

	// Token: 0x0400122B RID: 4651
	private SceneColorManager sm;
}
