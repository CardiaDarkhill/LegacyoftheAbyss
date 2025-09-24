using System;
using UnityEngine;

// Token: 0x02000784 RID: 1924
[ExecuteInEditMode]
public class SetGlobalShaderTime : MonoBehaviour
{
	// Token: 0x0600445B RID: 17499 RVA: 0x0012B91B File Offset: 0x00129B1B
	private void Update()
	{
		this.UpdateTime(Time.unscaledTime);
	}

	// Token: 0x0600445C RID: 17500 RVA: 0x0012B928 File Offset: 0x00129B28
	private void UpdateTime(float time)
	{
		Shader.SetGlobalFloat(SetGlobalShaderTime._unscaledGlobalTime, time);
	}

	// Token: 0x04004576 RID: 17782
	private static readonly int _unscaledGlobalTime = Shader.PropertyToID("_UnscaledGlobalTime");
}
