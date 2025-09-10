using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000289 RID: 649
[ExecuteInEditMode]
[AddComponentMenu("Image effects/Custom/Uber Postprocess")]
public class UberPostprocess : MonoBehaviour
{
	// Token: 0x060016D4 RID: 5844 RVA: 0x00066CA3 File Offset: 0x00064EA3
	private void OnEnable()
	{
		this.material = new Material(this.postprocessShader);
		this.material.name = "UberPostprocess_Material";
		this.material.hideFlags = HideFlags.HideAndDontSave;
		this.modules = new List<IPostprocessModule>();
	}

	// Token: 0x060016D5 RID: 5845 RVA: 0x00066CDE File Offset: 0x00064EDE
	private void OnDisable()
	{
		Object.DestroyImmediate(this.material);
		this.material = null;
		this.modules.Clear();
		this.modules = null;
	}

	// Token: 0x060016D6 RID: 5846 RVA: 0x00066D04 File Offset: 0x00064F04
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (this.modules == null)
		{
			this.modules = new List<IPostprocessModule>();
		}
		this.modules.Clear();
		base.GetComponents<IPostprocessModule>(this.modules);
		for (int i = 0; i < this.modules.Count; i++)
		{
			IPostprocessModule postprocessModule = this.modules[i];
			if ((postprocessModule as MonoBehaviour).enabled)
			{
				this.material.EnableKeyword(postprocessModule.EffectKeyword);
				this.modules[i].UpdateProperties(this.material);
			}
			else
			{
				this.material.DisableKeyword(postprocessModule.EffectKeyword);
			}
		}
		Graphics.Blit(source, destination, this.material, 0);
		if (GameCameraTextureDisplay.Instance)
		{
			GameCameraTextureDisplay.Instance.UpdateDisplay(source, this.material);
		}
	}

	// Token: 0x04001555 RID: 5461
	[SerializeField]
	[Tooltip("Shader that implements all of the postprocesses")]
	private Shader postprocessShader;

	// Token: 0x04001556 RID: 5462
	private List<IPostprocessModule> modules;

	// Token: 0x04001557 RID: 5463
	private Material material;
}
