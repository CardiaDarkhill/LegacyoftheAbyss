using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
	// Token: 0x020008C4 RID: 2244
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/Noise/Fast Noise")]
	public class FastNoise : PostEffectsBase
	{
		// Token: 0x17000920 RID: 2336
		// (get) Token: 0x06004D9B RID: 19867 RVA: 0x0016C4A4 File Offset: 0x0016A6A4
		// (set) Token: 0x06004D9C RID: 19868 RVA: 0x0016C4AC File Offset: 0x0016A6AC
		public bool ForceRender { get; set; }

		// Token: 0x17000921 RID: 2337
		// (get) Token: 0x06004D9D RID: 19869 RVA: 0x0016C4B5 File Offset: 0x0016A6B5
		// (set) Token: 0x06004D9E RID: 19870 RVA: 0x0016C4BC File Offset: 0x0016A6BC
		public static FastNoise Instance { get; private set; }

		// Token: 0x06004D9F RID: 19871 RVA: 0x0016C4C4 File Offset: 0x0016A6C4
		public void Init()
		{
			FastNoise.Instance = this;
		}

		// Token: 0x06004DA0 RID: 19872 RVA: 0x0016C4CC File Offset: 0x0016A6CC
		protected override void OnEnable()
		{
			base.OnEnable();
			this.effectIsSupported = (this.CheckResources() && this.hasMaterial);
		}

		// Token: 0x06004DA1 RID: 19873 RVA: 0x0016C4EC File Offset: 0x0016A6EC
		public override bool CheckResources()
		{
			base.CheckSupport(false);
			this.noiseMaterial = base.CheckShaderAndCreateMaterial(this.noiseShader, this.noiseMaterial);
			this.hasMaterial = this.noiseMaterial;
			if (!this.isSupported)
			{
				base.ReportAutoDisable();
			}
			return this.isSupported;
		}

		// Token: 0x06004DA2 RID: 19874 RVA: 0x0016C540 File Offset: 0x0016A740
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (!this.effectIsSupported)
			{
				Graphics.Blit(source, destination);
				if (!this.hasMaterial)
				{
					Debug.LogWarning("FastNoise effect failing as noise texture is not assigned. please assign.", base.transform);
				}
				return;
			}
			this.softness = Mathf.Clamp(this.softness, 0f, 0.99f);
			if (this.noiseTexture)
			{
				this.noiseTexture.wrapMode = TextureWrapMode.Repeat;
				this.noiseTexture.filterMode = this.filterMode;
			}
			this.noiseMaterial.SetTexture(FastNoise._noiseTex, this.noiseTexture);
			this.noiseMaterial.SetVector(FastNoise._noisePerChannel, this.monochrome ? Vector3.one : this.intensities);
			Vector2 one = Vector2.one;
			this.noiseMaterial.SetVector(FastNoise._noiseTilingPerChannel, one * this.monochromeTiling);
			this.noiseMaterial.SetVector(FastNoise._midGrey, new Vector3(this.midGrey, 1f / (1f - this.midGrey), -1f / this.midGrey));
			this.noiseMaterial.SetVector(FastNoise._noiseAmount, new Vector3(this.generalIntensity, this.blackIntensity, this.whiteIntensity) * this.intensityMultiplier);
			int frameMultiple = (int)(this.ForceRender ? FastNoise.FrameMultiple.Always : this.frameRateMultiplier);
			if (this.softness > Mathf.Epsilon)
			{
				RenderTexture temporary = RenderTexture.GetTemporary((int)((float)source.width * (1f - this.softness)), (int)((float)source.height * (1f - this.softness)));
				this.DrawNoiseQuadGrid(source, temporary, this.noiseMaterial, this.noiseTexture, 2, frameMultiple);
				this.noiseMaterial.SetTexture(FastNoise._noiseTex, temporary);
				Graphics.Blit(source, destination, this.noiseMaterial, 1);
				this.noiseMaterial.SetTexture(FastNoise._noiseTex, null);
				RenderTexture.ReleaseTemporary(temporary);
			}
			else
			{
				this.DrawNoiseQuadGrid(source, destination, this.noiseMaterial, this.noiseTexture, 0, frameMultiple);
			}
			this.frameCount += 1;
		}

		// Token: 0x06004DA3 RID: 19875 RVA: 0x0016C758 File Offset: 0x0016A958
		private void DrawNoiseQuadGrid(RenderTexture source, RenderTexture dest, Material fxMaterial, Texture2D noise, int passNr, int frameMultiple)
		{
			FastNoise.<>c__DisplayClass42_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			float num = (float)noise.width * 1f;
			float x = FastNoise.ReferenceRes.x;
			float num2 = 1f * x / FastNoise.TILE_AMOUNT;
			float num3 = 1f * (float)source.width / (1f * (float)source.height);
			float num4 = 1f / num2;
			float num5 = num4 * num3;
			float num6 = num / ((float)noise.width * 1f);
			if (Time.frameCount % frameMultiple == 0 || this.tcStartRandoms.Count == 0 || source.width != this.previousWidth || source.height != this.previousHeight)
			{
				this.tcStartRandoms.Clear();
				for (float num7 = 0f; num7 < 1f; num7 += num4)
				{
					for (float num8 = 0f; num8 < 1f; num8 += num5)
					{
						float item = Random.Range(0f, 1f);
						this.tcStartRandoms.Add(item);
						float item2 = Random.Range(0f, 1f);
						this.tcStartRandoms.Add(item2);
					}
				}
			}
			this.previousWidth = source.width;
			this.previousHeight = source.height;
			RenderTexture active = RenderTexture.active;
			RenderTexture.active = dest;
			fxMaterial.SetTexture(FastNoise._mainTex, source);
			GL.PushMatrix();
			GL.LoadOrtho();
			fxMaterial.SetPass(passNr);
			GL.Begin(7);
			CS$<>8__locals1.tcStartIndex = 0;
			for (float num9 = 0f; num9 < 1f; num9 += num4)
			{
				for (float num10 = 0f; num10 < 1f; num10 += num5)
				{
					float num11 = this.<DrawNoiseQuadGrid>g__GetNextTcStart|42_0(ref CS$<>8__locals1);
					float num12 = this.<DrawNoiseQuadGrid>g__GetNextTcStart|42_0(ref CS$<>8__locals1);
					num11 = Mathf.Floor(num11 * num) / num;
					num12 = Mathf.Floor(num12 * num) / num;
					float num13 = 1f / num;
					GL.MultiTexCoord2(0, num11, num12);
					GL.MultiTexCoord2(1, 0f, 0f);
					GL.Vertex3(num9, num10, 0.1f);
					GL.MultiTexCoord2(0, num11 + num6 * num13, num12);
					GL.MultiTexCoord2(1, 1f, 0f);
					GL.Vertex3(num9 + num4, num10, 0.1f);
					GL.MultiTexCoord2(0, num11 + num6 * num13, num12 + num6 * num13);
					GL.MultiTexCoord2(1, 1f, 1f);
					GL.Vertex3(num9 + num4, num10 + num5, 0.1f);
					GL.MultiTexCoord2(0, num11, num12 + num6 * num13);
					GL.MultiTexCoord2(1, 0f, 1f);
					GL.Vertex3(num9, num10 + num5, 0.1f);
				}
			}
			GL.End();
			GL.PopMatrix();
			fxMaterial.SetTexture(FastNoise._mainTex, null);
			RenderTexture.active = active;
		}

		// Token: 0x06004DA6 RID: 19878 RVA: 0x0016CB74 File Offset: 0x0016AD74
		[CompilerGenerated]
		private float <DrawNoiseQuadGrid>g__GetNextTcStart|42_0(ref FastNoise.<>c__DisplayClass42_0 A_1)
		{
			float result = this.tcStartRandoms[A_1.tcStartIndex];
			int tcStartIndex = A_1.tcStartIndex;
			A_1.tcStartIndex = tcStartIndex + 1;
			return result;
		}

		// Token: 0x04004E53 RID: 20051
		public static readonly Vector2 ReferenceRes = new Vector2(1920f, 1080f);

		// Token: 0x04004E54 RID: 20052
		public static readonly Vector2 InverseRef = new Vector2(1f / FastNoise.ReferenceRes.x, 1f / FastNoise.ReferenceRes.y);

		// Token: 0x04004E55 RID: 20053
		private bool monochrome = true;

		// Token: 0x04004E56 RID: 20054
		[Header("Update Rate")]
		public FastNoise.FrameMultiple frameRateMultiplier = FastNoise.FrameMultiple.Always;

		// Token: 0x04004E57 RID: 20055
		[Header("Intensity")]
		public float intensityMultiplier = 0.25f;

		// Token: 0x04004E58 RID: 20056
		public float generalIntensity = 0.5f;

		// Token: 0x04004E59 RID: 20057
		public float blackIntensity = 1f;

		// Token: 0x04004E5A RID: 20058
		public float whiteIntensity = 1f;

		// Token: 0x04004E5B RID: 20059
		[Range(0f, 1f)]
		public float midGrey = 0.2f;

		// Token: 0x04004E5C RID: 20060
		[Header("Noise Shape")]
		public Texture2D noiseTexture;

		// Token: 0x04004E5D RID: 20061
		public FilterMode filterMode = FilterMode.Bilinear;

		// Token: 0x04004E5E RID: 20062
		[Range(0f, 0.99f)]
		private Vector3 intensities = new Vector3(1f, 1f, 1f);

		// Token: 0x04004E5F RID: 20063
		[Range(0f, 0.99f)]
		public float softness = 0.052f;

		// Token: 0x04004E60 RID: 20064
		[Header("Advanced")]
		public float monochromeTiling = 64f;

		// Token: 0x04004E61 RID: 20065
		public Shader noiseShader;

		// Token: 0x04004E62 RID: 20066
		private Material noiseMaterial;

		// Token: 0x04004E63 RID: 20067
		private static float TILE_AMOUNT = 64f;

		// Token: 0x04004E64 RID: 20068
		private byte frameCount;

		// Token: 0x04004E65 RID: 20069
		private int previousWidth;

		// Token: 0x04004E66 RID: 20070
		private int previousHeight;

		// Token: 0x04004E67 RID: 20071
		private readonly List<float> tcStartRandoms = new List<float>();

		// Token: 0x04004E69 RID: 20073
		private bool effectIsSupported;

		// Token: 0x04004E6A RID: 20074
		private bool hasMaterial;

		// Token: 0x04004E6B RID: 20075
		private static readonly int _noiseTex = Shader.PropertyToID("_NoiseTex");

		// Token: 0x04004E6C RID: 20076
		private static readonly int _noisePerChannel = Shader.PropertyToID("_NoisePerChannel");

		// Token: 0x04004E6D RID: 20077
		private static readonly int _noiseTilingPerChannel = Shader.PropertyToID("_NoiseTilingPerChannel");

		// Token: 0x04004E6E RID: 20078
		private static readonly int _midGrey = Shader.PropertyToID("_MidGrey");

		// Token: 0x04004E6F RID: 20079
		private static readonly int _noiseAmount = Shader.PropertyToID("_NoiseAmount");

		// Token: 0x04004E70 RID: 20080
		private static readonly int _mainTex = Shader.PropertyToID("_MainTex");

		// Token: 0x02001B4F RID: 6991
		public enum FrameMultiple
		{
			// Token: 0x04009C25 RID: 39973
			Always = 1,
			// Token: 0x04009C26 RID: 39974
			Half,
			// Token: 0x04009C27 RID: 39975
			Third,
			// Token: 0x04009C28 RID: 39976
			Quarter,
			// Token: 0x04009C29 RID: 39977
			Fifth,
			// Token: 0x04009C2A RID: 39978
			Sixth,
			// Token: 0x04009C2B RID: 39979
			Eighth = 8,
			// Token: 0x04009C2C RID: 39980
			Tenth = 10
		}
	}
}
