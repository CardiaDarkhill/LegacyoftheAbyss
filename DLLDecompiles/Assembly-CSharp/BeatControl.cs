using System;
using UnityEngine;

// Token: 0x02000216 RID: 534
public class BeatControl : MonoBehaviour
{
	// Token: 0x060013CB RID: 5067 RVA: 0x0005A1EC File Offset: 0x000583EC
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.B))
		{
			this.beatIncrease += 0.25f;
		}
		if (this.beatIncrease != this.oldBeatValue)
		{
			this.oldBeatValue = this.beatIncrease;
			Shader.SetGlobalFloat("_BeatSpeedIncrease", this.beatIncrease);
			Shader.SetGlobalFloat("_BeatMagnitudeIncrease", this.beatIncrease);
		}
	}

	// Token: 0x060013CC RID: 5068 RVA: 0x0005A24E File Offset: 0x0005844E
	private void OnDestroy()
	{
		Shader.SetGlobalFloat("_BeatSpeedIncrease", 0f);
		Shader.SetGlobalFloat("_BeatMagnitudeIncrease", 0f);
	}

	// Token: 0x04001231 RID: 4657
	public float beatIncrease;

	// Token: 0x04001232 RID: 4658
	private float oldBeatValue;
}
