using System;
using UnityEngine;
using UnityEngine.UI;

namespace GUIBlendModes
{
	// Token: 0x02000AE0 RID: 2784
	[AddComponentMenu("UI/Effects/Blend Mode")]
	[RequireComponent(typeof(MaskableGraphic))]
	[ExecuteInEditMode]
	public class UIBlendMode : MonoBehaviour
	{
		// Token: 0x17000BB3 RID: 2995
		// (get) Token: 0x06005826 RID: 22566 RVA: 0x001BFCD8 File Offset: 0x001BDED8
		// (set) Token: 0x06005827 RID: 22567 RVA: 0x001BFCE0 File Offset: 0x001BDEE0
		public BlendMode BlendMode
		{
			get
			{
				return this.blendMode;
			}
			set
			{
				this.SetBlendMode(value, this.ShaderOptimization);
			}
		}

		// Token: 0x17000BB4 RID: 2996
		// (get) Token: 0x06005828 RID: 22568 RVA: 0x001BFCEF File Offset: 0x001BDEEF
		// (set) Token: 0x06005829 RID: 22569 RVA: 0x001BFCF7 File Offset: 0x001BDEF7
		public bool ShaderOptimization
		{
			get
			{
				return this.shaderOptimization;
			}
			set
			{
				this.SetBlendMode(this.BlendMode, value);
			}
		}

		// Token: 0x0600582A RID: 22570 RVA: 0x001BFD06 File Offset: 0x001BDF06
		private void OnEnable()
		{
			this.isDisabled = false;
			this.SetBlendMode(this.editorBlendMode, this.editorShaderOptimization);
		}

		// Token: 0x0600582B RID: 22571 RVA: 0x001BFD21 File Offset: 0x001BDF21
		private void OnDisable()
		{
			this.isDisabled = true;
			this.SetBlendMode(BlendMode.Normal, false);
		}

		// Token: 0x0600582C RID: 22572 RVA: 0x001BFD34 File Offset: 0x001BDF34
		public void SetBlendMode(BlendMode blendMode, bool shaderOptimization = false)
		{
			if (this.blendMode == blendMode && this.shaderOptimization == shaderOptimization)
			{
				return;
			}
			if (!this.source)
			{
				this.source = base.GetComponent<MaskableGraphic>();
			}
			this.source.material = BlendMaterials.GetMaterial(blendMode, this.source is Text, shaderOptimization);
			this.blendMode = blendMode;
			this.shaderOptimization = shaderOptimization;
			if (!this.isDisabled)
			{
				this.editorBlendMode = blendMode;
				this.editorShaderOptimization = shaderOptimization;
			}
		}

		// Token: 0x0600582D RID: 22573 RVA: 0x001BFDB1 File Offset: 0x001BDFB1
		public void SyncEditor()
		{
			if (Application.isEditor && !this.isDisabled && (this.BlendMode != this.editorBlendMode || this.ShaderOptimization != this.editorShaderOptimization))
			{
				this.SetBlendMode(this.editorBlendMode, this.editorShaderOptimization);
			}
		}

		// Token: 0x040053EE RID: 21486
		[SerializeField]
		private BlendMode editorBlendMode;

		// Token: 0x040053EF RID: 21487
		private BlendMode blendMode;

		// Token: 0x040053F0 RID: 21488
		[SerializeField]
		private bool editorShaderOptimization;

		// Token: 0x040053F1 RID: 21489
		private bool shaderOptimization;

		// Token: 0x040053F2 RID: 21490
		private MaskableGraphic source;

		// Token: 0x040053F3 RID: 21491
		private bool isDisabled;
	}
}
