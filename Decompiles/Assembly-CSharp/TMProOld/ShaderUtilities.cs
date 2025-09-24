using System;
using System.Linq;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x02000803 RID: 2051
	public static class ShaderUtilities
	{
		// Token: 0x060047DF RID: 18399 RVA: 0x0014E13C File Offset: 0x0014C33C
		public static void GetShaderPropertyIDs()
		{
			if (!ShaderUtilities.isInitialized)
			{
				ShaderUtilities.isInitialized = true;
				ShaderUtilities.ID_MainTex = Shader.PropertyToID("_MainTex");
				ShaderUtilities.ID_FaceTex = Shader.PropertyToID("_FaceTex");
				ShaderUtilities.ID_FaceColor = Shader.PropertyToID("_FaceColor");
				ShaderUtilities.ID_FaceDilate = Shader.PropertyToID("_FaceDilate");
				ShaderUtilities.ID_Shininess = Shader.PropertyToID("_FaceShininess");
				ShaderUtilities.ID_UnderlayColor = Shader.PropertyToID("_UnderlayColor");
				ShaderUtilities.ID_UnderlayOffsetX = Shader.PropertyToID("_UnderlayOffsetX");
				ShaderUtilities.ID_UnderlayOffsetY = Shader.PropertyToID("_UnderlayOffsetY");
				ShaderUtilities.ID_UnderlayDilate = Shader.PropertyToID("_UnderlayDilate");
				ShaderUtilities.ID_UnderlaySoftness = Shader.PropertyToID("_UnderlaySoftness");
				ShaderUtilities.ID_WeightNormal = Shader.PropertyToID("_WeightNormal");
				ShaderUtilities.ID_WeightBold = Shader.PropertyToID("_WeightBold");
				ShaderUtilities.ID_OutlineTex = Shader.PropertyToID("_OutlineTex");
				ShaderUtilities.ID_OutlineWidth = Shader.PropertyToID("_OutlineWidth");
				ShaderUtilities.ID_OutlineSoftness = Shader.PropertyToID("_OutlineSoftness");
				ShaderUtilities.ID_OutlineColor = Shader.PropertyToID("_OutlineColor");
				ShaderUtilities.ID_GradientScale = Shader.PropertyToID("_GradientScale");
				ShaderUtilities.ID_ScaleX = Shader.PropertyToID("_ScaleX");
				ShaderUtilities.ID_ScaleY = Shader.PropertyToID("_ScaleY");
				ShaderUtilities.ID_PerspectiveFilter = Shader.PropertyToID("_PerspectiveFilter");
				ShaderUtilities.ID_TextureWidth = Shader.PropertyToID("_TextureWidth");
				ShaderUtilities.ID_TextureHeight = Shader.PropertyToID("_TextureHeight");
				ShaderUtilities.ID_BevelAmount = Shader.PropertyToID("_Bevel");
				ShaderUtilities.ID_LightAngle = Shader.PropertyToID("_LightAngle");
				ShaderUtilities.ID_EnvMap = Shader.PropertyToID("_Cube");
				ShaderUtilities.ID_EnvMatrix = Shader.PropertyToID("_EnvMatrix");
				ShaderUtilities.ID_EnvMatrixRotation = Shader.PropertyToID("_EnvMatrixRotation");
				ShaderUtilities.ID_GlowColor = Shader.PropertyToID("_GlowColor");
				ShaderUtilities.ID_GlowOffset = Shader.PropertyToID("_GlowOffset");
				ShaderUtilities.ID_GlowPower = Shader.PropertyToID("_GlowPower");
				ShaderUtilities.ID_GlowOuter = Shader.PropertyToID("_GlowOuter");
				ShaderUtilities.ID_MaskCoord = Shader.PropertyToID("_MaskCoord");
				ShaderUtilities.ID_ClipRect = Shader.PropertyToID("_ClipRect");
				ShaderUtilities.ID_UseClipRect = Shader.PropertyToID("_UseClipRect");
				ShaderUtilities.ID_MaskSoftnessX = Shader.PropertyToID("_MaskSoftnessX");
				ShaderUtilities.ID_MaskSoftnessY = Shader.PropertyToID("_MaskSoftnessY");
				ShaderUtilities.ID_VertexOffsetX = Shader.PropertyToID("_VertexOffsetX");
				ShaderUtilities.ID_VertexOffsetY = Shader.PropertyToID("_VertexOffsetY");
				ShaderUtilities.ID_StencilID = Shader.PropertyToID("_Stencil");
				ShaderUtilities.ID_StencilOp = Shader.PropertyToID("_StencilOp");
				ShaderUtilities.ID_StencilComp = Shader.PropertyToID("_StencilComp");
				ShaderUtilities.ID_StencilReadMask = Shader.PropertyToID("_StencilReadMask");
				ShaderUtilities.ID_StencilWriteMask = Shader.PropertyToID("_StencilWriteMask");
				ShaderUtilities.ID_ShaderFlags = Shader.PropertyToID("_ShaderFlags");
				ShaderUtilities.ID_ScaleRatio_A = Shader.PropertyToID("_ScaleRatioA");
				ShaderUtilities.ID_ScaleRatio_B = Shader.PropertyToID("_ScaleRatioB");
				ShaderUtilities.ID_ScaleRatio_C = Shader.PropertyToID("_ScaleRatioC");
			}
		}

		// Token: 0x060047E0 RID: 18400 RVA: 0x0014E41C File Offset: 0x0014C61C
		public static void UpdateShaderRatios(Material mat, bool isBold)
		{
			bool flag = !mat.shaderKeywords.Contains(ShaderUtilities.Keyword_Ratios);
			float @float = mat.GetFloat(ShaderUtilities.ID_GradientScale);
			float float2 = mat.GetFloat(ShaderUtilities.ID_FaceDilate);
			float float3 = mat.GetFloat(ShaderUtilities.ID_OutlineWidth);
			float float4 = mat.GetFloat(ShaderUtilities.ID_OutlineSoftness);
			float num = (!isBold) ? (mat.GetFloat(ShaderUtilities.ID_WeightNormal) * 2f / @float) : (mat.GetFloat(ShaderUtilities.ID_WeightBold) * 2f / @float);
			float num2 = Mathf.Max(1f, num + float2 + float3 + float4);
			float value = flag ? ((@float - ShaderUtilities.m_clamp) / (@float * num2)) : 1f;
			mat.SetFloat(ShaderUtilities.ID_ScaleRatio_A, value);
			if (mat.HasProperty(ShaderUtilities.ID_GlowOffset))
			{
				float float5 = mat.GetFloat(ShaderUtilities.ID_GlowOffset);
				float float6 = mat.GetFloat(ShaderUtilities.ID_GlowOuter);
				float num3 = (num + float2) * (@float - ShaderUtilities.m_clamp);
				num2 = Mathf.Max(1f, float5 + float6);
				float value2 = flag ? (Mathf.Max(0f, @float - ShaderUtilities.m_clamp - num3) / (@float * num2)) : 1f;
				mat.SetFloat(ShaderUtilities.ID_ScaleRatio_B, value2);
			}
			if (mat.HasProperty(ShaderUtilities.ID_UnderlayOffsetX))
			{
				float float7 = mat.GetFloat(ShaderUtilities.ID_UnderlayOffsetX);
				float float8 = mat.GetFloat(ShaderUtilities.ID_UnderlayOffsetY);
				float float9 = mat.GetFloat(ShaderUtilities.ID_UnderlayDilate);
				float float10 = mat.GetFloat(ShaderUtilities.ID_UnderlaySoftness);
				float num4 = (num + float2) * (@float - ShaderUtilities.m_clamp);
				num2 = Mathf.Max(1f, Mathf.Max(Mathf.Abs(float7), Mathf.Abs(float8)) + float9 + float10);
				float value3 = flag ? (Mathf.Max(0f, @float - ShaderUtilities.m_clamp - num4) / (@float * num2)) : 1f;
				mat.SetFloat(ShaderUtilities.ID_ScaleRatio_C, value3);
			}
		}

		// Token: 0x060047E1 RID: 18401 RVA: 0x0014E613 File Offset: 0x0014C813
		public static Vector4 GetFontExtent(Material material)
		{
			return Vector4.zero;
		}

		// Token: 0x060047E2 RID: 18402 RVA: 0x0014E61C File Offset: 0x0014C81C
		public static bool IsMaskingEnabled(Material material)
		{
			return !(material == null) && material.HasProperty(ShaderUtilities.ID_ClipRect) && (material.shaderKeywords.Contains(ShaderUtilities.Keyword_MASK_SOFT) || material.shaderKeywords.Contains(ShaderUtilities.Keyword_MASK_HARD) || material.shaderKeywords.Contains(ShaderUtilities.Keyword_MASK_TEX));
		}

		// Token: 0x060047E3 RID: 18403 RVA: 0x0014E67C File Offset: 0x0014C87C
		public static float GetPadding(Material material, bool enableExtraPadding, bool isBold)
		{
			if (!ShaderUtilities.isInitialized)
			{
				ShaderUtilities.GetShaderPropertyIDs();
			}
			if (material == null)
			{
				return 0f;
			}
			int num = enableExtraPadding ? 4 : 0;
			if (!material.HasProperty(ShaderUtilities.ID_GradientScale))
			{
				return (float)num;
			}
			Vector4 vector = Vector4.zero;
			Vector4 zero = Vector4.zero;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			ShaderUtilities.UpdateShaderRatios(material, isBold);
			string[] shaderKeywords = material.shaderKeywords;
			if (material.HasProperty(ShaderUtilities.ID_ScaleRatio_A))
			{
				num5 = material.GetFloat(ShaderUtilities.ID_ScaleRatio_A);
			}
			if (material.HasProperty(ShaderUtilities.ID_FaceDilate))
			{
				num2 = material.GetFloat(ShaderUtilities.ID_FaceDilate) * num5;
			}
			if (material.HasProperty(ShaderUtilities.ID_OutlineSoftness))
			{
				num3 = material.GetFloat(ShaderUtilities.ID_OutlineSoftness) * num5;
			}
			if (material.HasProperty(ShaderUtilities.ID_OutlineWidth))
			{
				num4 = material.GetFloat(ShaderUtilities.ID_OutlineWidth) * num5;
			}
			float num10 = num4 + num3 + num2;
			if (material.HasProperty(ShaderUtilities.ID_GlowOffset) && shaderKeywords.Contains(ShaderUtilities.Keyword_Glow))
			{
				if (material.HasProperty(ShaderUtilities.ID_ScaleRatio_B))
				{
					num6 = material.GetFloat(ShaderUtilities.ID_ScaleRatio_B);
				}
				num8 = material.GetFloat(ShaderUtilities.ID_GlowOffset) * num6;
				num9 = material.GetFloat(ShaderUtilities.ID_GlowOuter) * num6;
			}
			num10 = Mathf.Max(num10, num2 + num8 + num9);
			if (material.HasProperty(ShaderUtilities.ID_UnderlaySoftness) && shaderKeywords.Contains(ShaderUtilities.Keyword_Underlay))
			{
				if (material.HasProperty(ShaderUtilities.ID_ScaleRatio_C))
				{
					num7 = material.GetFloat(ShaderUtilities.ID_ScaleRatio_C);
				}
				float num11 = material.GetFloat(ShaderUtilities.ID_UnderlayOffsetX) * num7;
				float num12 = material.GetFloat(ShaderUtilities.ID_UnderlayOffsetY) * num7;
				float num13 = material.GetFloat(ShaderUtilities.ID_UnderlayDilate) * num7;
				float num14 = material.GetFloat(ShaderUtilities.ID_UnderlaySoftness) * num7;
				vector.x = Mathf.Max(vector.x, num2 + num13 + num14 - num11);
				vector.y = Mathf.Max(vector.y, num2 + num13 + num14 - num12);
				vector.z = Mathf.Max(vector.z, num2 + num13 + num14 + num11);
				vector.w = Mathf.Max(vector.w, num2 + num13 + num14 + num12);
			}
			vector.x = Mathf.Max(vector.x, num10);
			vector.y = Mathf.Max(vector.y, num10);
			vector.z = Mathf.Max(vector.z, num10);
			vector.w = Mathf.Max(vector.w, num10);
			vector.x += (float)num;
			vector.y += (float)num;
			vector.z += (float)num;
			vector.w += (float)num;
			vector.x = Mathf.Min(vector.x, 1f);
			vector.y = Mathf.Min(vector.y, 1f);
			vector.z = Mathf.Min(vector.z, 1f);
			vector.w = Mathf.Min(vector.w, 1f);
			zero.x = ((zero.x < vector.x) ? vector.x : zero.x);
			zero.y = ((zero.y < vector.y) ? vector.y : zero.y);
			zero.z = ((zero.z < vector.z) ? vector.z : zero.z);
			zero.w = ((zero.w < vector.w) ? vector.w : zero.w);
			float @float = material.GetFloat(ShaderUtilities.ID_GradientScale);
			vector *= @float;
			num10 = Mathf.Max(vector.x, vector.y);
			num10 = Mathf.Max(vector.z, num10);
			num10 = Mathf.Max(vector.w, num10);
			return num10 + 0.5f;
		}

		// Token: 0x060047E4 RID: 18404 RVA: 0x0014EA9C File Offset: 0x0014CC9C
		public static float GetPadding(Material[] materials, bool enableExtraPadding, bool isBold)
		{
			if (!ShaderUtilities.isInitialized)
			{
				ShaderUtilities.GetShaderPropertyIDs();
			}
			if (materials == null)
			{
				return 0f;
			}
			int num = enableExtraPadding ? 4 : 0;
			if (!materials[0].HasProperty(ShaderUtilities.ID_GradientScale))
			{
				return (float)num;
			}
			Vector4 vector = Vector4.zero;
			Vector4 zero = Vector4.zero;
			float num2 = 0f;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 0f;
			float num8 = 0f;
			float num9 = 0f;
			float num10;
			for (int i = 0; i < materials.Length; i++)
			{
				ShaderUtilities.UpdateShaderRatios(materials[i], isBold);
				string[] shaderKeywords = materials[i].shaderKeywords;
				if (materials[i].HasProperty(ShaderUtilities.ID_ScaleRatio_A))
				{
					num5 = materials[i].GetFloat(ShaderUtilities.ID_ScaleRatio_A);
				}
				if (materials[i].HasProperty(ShaderUtilities.ID_FaceDilate))
				{
					num2 = materials[i].GetFloat(ShaderUtilities.ID_FaceDilate) * num5;
				}
				if (materials[i].HasProperty(ShaderUtilities.ID_OutlineSoftness))
				{
					num3 = materials[i].GetFloat(ShaderUtilities.ID_OutlineSoftness) * num5;
				}
				if (materials[i].HasProperty(ShaderUtilities.ID_OutlineWidth))
				{
					num4 = materials[i].GetFloat(ShaderUtilities.ID_OutlineWidth) * num5;
				}
				num10 = num4 + num3 + num2;
				if (materials[i].HasProperty(ShaderUtilities.ID_GlowOffset) && shaderKeywords.Contains(ShaderUtilities.Keyword_Glow))
				{
					if (materials[i].HasProperty(ShaderUtilities.ID_ScaleRatio_B))
					{
						num6 = materials[i].GetFloat(ShaderUtilities.ID_ScaleRatio_B);
					}
					num8 = materials[i].GetFloat(ShaderUtilities.ID_GlowOffset) * num6;
					num9 = materials[i].GetFloat(ShaderUtilities.ID_GlowOuter) * num6;
				}
				num10 = Mathf.Max(num10, num2 + num8 + num9);
				if (materials[i].HasProperty(ShaderUtilities.ID_UnderlaySoftness) && shaderKeywords.Contains(ShaderUtilities.Keyword_Underlay))
				{
					if (materials[i].HasProperty(ShaderUtilities.ID_ScaleRatio_C))
					{
						num7 = materials[i].GetFloat(ShaderUtilities.ID_ScaleRatio_C);
					}
					float num11 = materials[i].GetFloat(ShaderUtilities.ID_UnderlayOffsetX) * num7;
					float num12 = materials[i].GetFloat(ShaderUtilities.ID_UnderlayOffsetY) * num7;
					float num13 = materials[i].GetFloat(ShaderUtilities.ID_UnderlayDilate) * num7;
					float num14 = materials[i].GetFloat(ShaderUtilities.ID_UnderlaySoftness) * num7;
					vector.x = Mathf.Max(vector.x, num2 + num13 + num14 - num11);
					vector.y = Mathf.Max(vector.y, num2 + num13 + num14 - num12);
					vector.z = Mathf.Max(vector.z, num2 + num13 + num14 + num11);
					vector.w = Mathf.Max(vector.w, num2 + num13 + num14 + num12);
				}
				vector.x = Mathf.Max(vector.x, num10);
				vector.y = Mathf.Max(vector.y, num10);
				vector.z = Mathf.Max(vector.z, num10);
				vector.w = Mathf.Max(vector.w, num10);
				vector.x += (float)num;
				vector.y += (float)num;
				vector.z += (float)num;
				vector.w += (float)num;
				vector.x = Mathf.Min(vector.x, 1f);
				vector.y = Mathf.Min(vector.y, 1f);
				vector.z = Mathf.Min(vector.z, 1f);
				vector.w = Mathf.Min(vector.w, 1f);
				zero.x = ((zero.x < vector.x) ? vector.x : zero.x);
				zero.y = ((zero.y < vector.y) ? vector.y : zero.y);
				zero.z = ((zero.z < vector.z) ? vector.z : zero.z);
				zero.w = ((zero.w < vector.w) ? vector.w : zero.w);
			}
			float @float = materials[0].GetFloat(ShaderUtilities.ID_GradientScale);
			vector *= @float;
			num10 = Mathf.Max(vector.x, vector.y);
			num10 = Mathf.Max(vector.z, num10);
			num10 = Mathf.Max(vector.w, num10);
			return num10 + 0.25f;
		}

		// Token: 0x0400483D RID: 18493
		public static int ID_MainTex;

		// Token: 0x0400483E RID: 18494
		public static int ID_FaceTex;

		// Token: 0x0400483F RID: 18495
		public static int ID_FaceColor;

		// Token: 0x04004840 RID: 18496
		public static int ID_FaceDilate;

		// Token: 0x04004841 RID: 18497
		public static int ID_Shininess;

		// Token: 0x04004842 RID: 18498
		public static int ID_UnderlayColor;

		// Token: 0x04004843 RID: 18499
		public static int ID_UnderlayOffsetX;

		// Token: 0x04004844 RID: 18500
		public static int ID_UnderlayOffsetY;

		// Token: 0x04004845 RID: 18501
		public static int ID_UnderlayDilate;

		// Token: 0x04004846 RID: 18502
		public static int ID_UnderlaySoftness;

		// Token: 0x04004847 RID: 18503
		public static int ID_WeightNormal;

		// Token: 0x04004848 RID: 18504
		public static int ID_WeightBold;

		// Token: 0x04004849 RID: 18505
		public static int ID_OutlineTex;

		// Token: 0x0400484A RID: 18506
		public static int ID_OutlineWidth;

		// Token: 0x0400484B RID: 18507
		public static int ID_OutlineSoftness;

		// Token: 0x0400484C RID: 18508
		public static int ID_OutlineColor;

		// Token: 0x0400484D RID: 18509
		public static int ID_GradientScale;

		// Token: 0x0400484E RID: 18510
		public static int ID_ScaleX;

		// Token: 0x0400484F RID: 18511
		public static int ID_ScaleY;

		// Token: 0x04004850 RID: 18512
		public static int ID_PerspectiveFilter;

		// Token: 0x04004851 RID: 18513
		public static int ID_TextureWidth;

		// Token: 0x04004852 RID: 18514
		public static int ID_TextureHeight;

		// Token: 0x04004853 RID: 18515
		public static int ID_BevelAmount;

		// Token: 0x04004854 RID: 18516
		public static int ID_GlowColor;

		// Token: 0x04004855 RID: 18517
		public static int ID_GlowOffset;

		// Token: 0x04004856 RID: 18518
		public static int ID_GlowPower;

		// Token: 0x04004857 RID: 18519
		public static int ID_GlowOuter;

		// Token: 0x04004858 RID: 18520
		public static int ID_LightAngle;

		// Token: 0x04004859 RID: 18521
		public static int ID_EnvMap;

		// Token: 0x0400485A RID: 18522
		public static int ID_EnvMatrix;

		// Token: 0x0400485B RID: 18523
		public static int ID_EnvMatrixRotation;

		// Token: 0x0400485C RID: 18524
		public static int ID_MaskCoord;

		// Token: 0x0400485D RID: 18525
		public static int ID_ClipRect;

		// Token: 0x0400485E RID: 18526
		public static int ID_MaskSoftnessX;

		// Token: 0x0400485F RID: 18527
		public static int ID_MaskSoftnessY;

		// Token: 0x04004860 RID: 18528
		public static int ID_VertexOffsetX;

		// Token: 0x04004861 RID: 18529
		public static int ID_VertexOffsetY;

		// Token: 0x04004862 RID: 18530
		public static int ID_UseClipRect;

		// Token: 0x04004863 RID: 18531
		public static int ID_StencilID;

		// Token: 0x04004864 RID: 18532
		public static int ID_StencilOp;

		// Token: 0x04004865 RID: 18533
		public static int ID_StencilComp;

		// Token: 0x04004866 RID: 18534
		public static int ID_StencilReadMask;

		// Token: 0x04004867 RID: 18535
		public static int ID_StencilWriteMask;

		// Token: 0x04004868 RID: 18536
		public static int ID_ShaderFlags;

		// Token: 0x04004869 RID: 18537
		public static int ID_ScaleRatio_A;

		// Token: 0x0400486A RID: 18538
		public static int ID_ScaleRatio_B;

		// Token: 0x0400486B RID: 18539
		public static int ID_ScaleRatio_C;

		// Token: 0x0400486C RID: 18540
		public static string Keyword_Bevel = "BEVEL_ON";

		// Token: 0x0400486D RID: 18541
		public static string Keyword_Glow = "GLOW_ON";

		// Token: 0x0400486E RID: 18542
		public static string Keyword_Underlay = "UNDERLAY_ON";

		// Token: 0x0400486F RID: 18543
		public static string Keyword_Ratios = "RATIOS_OFF";

		// Token: 0x04004870 RID: 18544
		public static string Keyword_MASK_SOFT = "MASK_SOFT";

		// Token: 0x04004871 RID: 18545
		public static string Keyword_MASK_HARD = "MASK_HARD";

		// Token: 0x04004872 RID: 18546
		public static string Keyword_MASK_TEX = "MASK_TEX";

		// Token: 0x04004873 RID: 18547
		public static string Keyword_Outline = "OUTLINE_ON";

		// Token: 0x04004874 RID: 18548
		public static string ShaderTag_ZTestMode = "unity_GUIZTestMode";

		// Token: 0x04004875 RID: 18549
		public static string ShaderTag_CullMode = "_CullMode";

		// Token: 0x04004876 RID: 18550
		private static float m_clamp = 1f;

		// Token: 0x04004877 RID: 18551
		public static bool isInitialized = false;
	}
}
