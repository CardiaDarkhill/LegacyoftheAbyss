using System;
using UnityEngine;

// Token: 0x0200014C RID: 332
[RequireComponent(typeof(LightBlurredBackground))]
public class BlurManager : MonoBehaviour
{
	// Token: 0x06000A1F RID: 2591 RVA: 0x0002DD42 File Offset: 0x0002BF42
	protected void Awake()
	{
		this.appliedShaderQuality = ShaderQualities.High;
		this.lightBlurredBackground = base.GetComponent<LightBlurredBackground>();
		this.lightBlurredBackground.RenderTextureHeight = this.baseHeight;
	}

	// Token: 0x06000A20 RID: 2592 RVA: 0x0002DD68 File Offset: 0x0002BF68
	protected void Update()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (unsafeInstance != null)
		{
			ShaderQualities shaderQuality = unsafeInstance.gameSettings.shaderQuality;
			if (shaderQuality != this.appliedShaderQuality)
			{
				this.appliedShaderQuality = shaderQuality;
				if (shaderQuality <= ShaderQualities.High)
				{
					this.lightBlurredBackground.PassGroupCount = ((shaderQuality == ShaderQualities.Low) ? 1 : 2);
					this.lightBlurredBackground.enabled = true;
					return;
				}
				this.lightBlurredBackground.enabled = false;
			}
		}
	}

	// Token: 0x040009B3 RID: 2483
	private ShaderQualities appliedShaderQuality;

	// Token: 0x040009B4 RID: 2484
	private LightBlurredBackground lightBlurredBackground;

	// Token: 0x040009B5 RID: 2485
	[SerializeField]
	private int baseHeight;
}
