using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CFA RID: 3322
	public class RendererFloatRamp : FsmStateAction
	{
		// Token: 0x06006279 RID: 25209 RVA: 0x001F26D4 File Offset: 0x001F08D4
		public override void Reset()
		{
			this.Target = null;
			this.PropertyName = null;
			this.From = null;
			this.To = null;
			this.Duration = null;
			this.Curve = new FsmAnimationCurve
			{
				curve = AnimationCurve.Linear(0f, 0f, 1f, 1f)
			};
			this.UseChildren = null;
		}

		// Token: 0x0600627A RID: 25210 RVA: 0x001F2738 File Offset: 0x001F0938
		public override void Awake()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (this.UseChildren.Value)
			{
				if (safe)
				{
					this.renderers = safe.GetComponentsInChildren<Renderer>(true);
					this.propBlocks = new MaterialPropertyBlock[this.renderers.Length];
					for (int i = 0; i < this.renderers.Length; i++)
					{
						MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
						this.propBlocks[i] = materialPropertyBlock;
					}
					return;
				}
			}
			else
			{
				Renderer component = safe.GetComponent<Renderer>();
				if (component)
				{
					this.renderers = new Renderer[]
					{
						component
					};
					MaterialPropertyBlock materialPropertyBlock2 = new MaterialPropertyBlock();
					this.propBlocks = new MaterialPropertyBlock[]
					{
						materialPropertyBlock2
					};
					return;
				}
			}
			this.renderers = Array.Empty<Renderer>();
			this.propBlocks = Array.Empty<MaterialPropertyBlock>();
		}

		// Token: 0x0600627B RID: 25211 RVA: 0x001F27F8 File Offset: 0x001F09F8
		public override void OnEnter()
		{
			this.elapsed = 0f;
			this.propId = Shader.PropertyToID(this.PropertyName.Value);
			this.UpdateValues(0f);
		}

		// Token: 0x0600627C RID: 25212 RVA: 0x001F2828 File Offset: 0x001F0A28
		public override void OnUpdate()
		{
			this.elapsed += Time.deltaTime;
			float num = this.elapsed / this.Duration.Value;
			bool flag = num >= 1f;
			if (flag)
			{
				num = 1f;
			}
			this.UpdateValues(this.Curve.curve.Evaluate(num));
			if (flag)
			{
				base.Finish();
			}
		}

		// Token: 0x0600627D RID: 25213 RVA: 0x001F2890 File Offset: 0x001F0A90
		private void UpdateValues(float t)
		{
			float value = Mathf.Lerp(this.From.Value, this.To.Value, t);
			for (int i = 0; i < this.renderers.Length; i++)
			{
				MaterialPropertyBlock materialPropertyBlock = this.propBlocks[i];
				Renderer renderer = this.renderers[i];
				renderer.GetPropertyBlock(materialPropertyBlock);
				materialPropertyBlock.SetFloat(this.propId, value);
				renderer.SetPropertyBlock(materialPropertyBlock);
			}
		}

		// Token: 0x040060DF RID: 24799
		public FsmOwnerDefault Target;

		// Token: 0x040060E0 RID: 24800
		public FsmString PropertyName;

		// Token: 0x040060E1 RID: 24801
		public FsmFloat From;

		// Token: 0x040060E2 RID: 24802
		public FsmFloat To;

		// Token: 0x040060E3 RID: 24803
		public FsmFloat Duration;

		// Token: 0x040060E4 RID: 24804
		public FsmAnimationCurve Curve;

		// Token: 0x040060E5 RID: 24805
		public FsmBool UseChildren;

		// Token: 0x040060E6 RID: 24806
		private int propId;

		// Token: 0x040060E7 RID: 24807
		private float elapsed;

		// Token: 0x040060E8 RID: 24808
		private Renderer[] renderers;

		// Token: 0x040060E9 RID: 24809
		private MaterialPropertyBlock[] propBlocks;
	}
}
