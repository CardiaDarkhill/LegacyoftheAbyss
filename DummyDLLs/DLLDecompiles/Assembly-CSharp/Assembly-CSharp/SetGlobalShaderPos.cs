using System;
using UnityEngine;

// Token: 0x02000276 RID: 630
[ExecuteInEditMode]
public class SetGlobalShaderPos : MonoBehaviour
{
	// Token: 0x06001672 RID: 5746 RVA: 0x00065111 File Offset: 0x00063311
	private void OnEnable()
	{
		if (!string.IsNullOrEmpty(this.playModeVariable))
		{
			Shader.SetGlobalFloat(this.playModeVariable, (float)(Application.isPlaying ? 1 : 0));
		}
		this.UpdateValue();
	}

	// Token: 0x06001673 RID: 5747 RVA: 0x0006513D File Offset: 0x0006333D
	private void Update()
	{
		this.UpdateValue();
	}

	// Token: 0x06001674 RID: 5748 RVA: 0x00065148 File Offset: 0x00063348
	private void UpdateValue()
	{
		if (string.IsNullOrEmpty(this.variableName))
		{
			return;
		}
		Vector3 v = base.transform.position + this.offset;
		Shader.SetGlobalVector(this.variableName, v);
	}

	// Token: 0x040014E4 RID: 5348
	[SerializeField]
	private string variableName;

	// Token: 0x040014E5 RID: 5349
	[SerializeField]
	private Vector3 offset;

	// Token: 0x040014E6 RID: 5350
	[SerializeField]
	private string playModeVariable;
}
