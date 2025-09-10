using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TMProOld
{
	// Token: 0x02000813 RID: 2067
	public static class TMP_MaterialManager
	{
		// Token: 0x060048EE RID: 18670 RVA: 0x00154554 File Offset: 0x00152754
		public static Material GetStencilMaterial(Material baseMaterial, int stencilID)
		{
			if (!baseMaterial.HasProperty(ShaderUtilities.ID_StencilID))
			{
				Debug.LogWarning("Selected Shader does not support Stencil Masking. Please select the Distance Field or Mobile Distance Field Shader.");
				return baseMaterial;
			}
			int instanceID = baseMaterial.GetInstanceID();
			for (int i = 0; i < TMP_MaterialManager.m_materialList.Count; i++)
			{
				if (TMP_MaterialManager.m_materialList[i].baseMaterial.GetInstanceID() == instanceID && TMP_MaterialManager.m_materialList[i].stencilID == stencilID)
				{
					TMP_MaterialManager.m_materialList[i].count++;
					return TMP_MaterialManager.m_materialList[i].stencilMaterial;
				}
			}
			Material material = new Material(baseMaterial);
			material.hideFlags = HideFlags.HideAndDontSave;
			material.shaderKeywords = baseMaterial.shaderKeywords;
			ShaderUtilities.GetShaderPropertyIDs();
			material.SetFloat(ShaderUtilities.ID_StencilID, (float)stencilID);
			material.SetFloat(ShaderUtilities.ID_StencilComp, 4f);
			TMP_MaterialManager.MaskingMaterial maskingMaterial = new TMP_MaterialManager.MaskingMaterial();
			maskingMaterial.baseMaterial = baseMaterial;
			maskingMaterial.stencilMaterial = material;
			maskingMaterial.stencilID = stencilID;
			maskingMaterial.count = 1;
			TMP_MaterialManager.m_materialList.Add(maskingMaterial);
			return material;
		}

		// Token: 0x060048EF RID: 18671 RVA: 0x00154658 File Offset: 0x00152858
		public static void ReleaseStencilMaterial(Material stencilMaterial)
		{
			int instanceID = stencilMaterial.GetInstanceID();
			int i = 0;
			while (i < TMP_MaterialManager.m_materialList.Count)
			{
				if (TMP_MaterialManager.m_materialList[i].stencilMaterial.GetInstanceID() == instanceID)
				{
					if (TMP_MaterialManager.m_materialList[i].count > 1)
					{
						TMP_MaterialManager.m_materialList[i].count--;
						return;
					}
					Object.DestroyImmediate(TMP_MaterialManager.m_materialList[i].stencilMaterial);
					TMP_MaterialManager.m_materialList.RemoveAt(i);
					stencilMaterial = null;
					return;
				}
				else
				{
					i++;
				}
			}
		}

		// Token: 0x060048F0 RID: 18672 RVA: 0x001546EC File Offset: 0x001528EC
		public static Material GetBaseMaterial(Material stencilMaterial)
		{
			int num = TMP_MaterialManager.m_materialList.FindIndex((TMP_MaterialManager.MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num == -1)
			{
				return null;
			}
			return TMP_MaterialManager.m_materialList[num].baseMaterial;
		}

		// Token: 0x060048F1 RID: 18673 RVA: 0x00154733 File Offset: 0x00152933
		public static Material SetStencil(Material material, int stencilID)
		{
			material.SetFloat(ShaderUtilities.ID_StencilID, (float)stencilID);
			if (stencilID == 0)
			{
				material.SetFloat(ShaderUtilities.ID_StencilComp, 8f);
			}
			else
			{
				material.SetFloat(ShaderUtilities.ID_StencilComp, 4f);
			}
			return material;
		}

		// Token: 0x060048F2 RID: 18674 RVA: 0x00154768 File Offset: 0x00152968
		public static void AddMaskingMaterial(Material baseMaterial, Material stencilMaterial, int stencilID)
		{
			int num = TMP_MaterialManager.m_materialList.FindIndex((TMP_MaterialManager.MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num == -1)
			{
				TMP_MaterialManager.MaskingMaterial maskingMaterial = new TMP_MaterialManager.MaskingMaterial();
				maskingMaterial.baseMaterial = baseMaterial;
				maskingMaterial.stencilMaterial = stencilMaterial;
				maskingMaterial.stencilID = stencilID;
				maskingMaterial.count = 1;
				TMP_MaterialManager.m_materialList.Add(maskingMaterial);
				return;
			}
			stencilMaterial = TMP_MaterialManager.m_materialList[num].stencilMaterial;
			TMP_MaterialManager.m_materialList[num].count++;
		}

		// Token: 0x060048F3 RID: 18675 RVA: 0x00154800 File Offset: 0x00152A00
		public static void RemoveStencilMaterial(Material stencilMaterial)
		{
			int num = TMP_MaterialManager.m_materialList.FindIndex((TMP_MaterialManager.MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num != -1)
			{
				TMP_MaterialManager.m_materialList.RemoveAt(num);
			}
		}

		// Token: 0x060048F4 RID: 18676 RVA: 0x00154840 File Offset: 0x00152A40
		public static void ReleaseBaseMaterial(Material baseMaterial)
		{
			int num = TMP_MaterialManager.m_materialList.FindIndex((TMP_MaterialManager.MaskingMaterial item) => item.baseMaterial == baseMaterial);
			if (num == -1)
			{
				Debug.Log("No Masking Material exists for " + baseMaterial.name);
				return;
			}
			if (TMP_MaterialManager.m_materialList[num].count > 1)
			{
				TMP_MaterialManager.m_materialList[num].count--;
				Debug.Log(string.Concat(new string[]
				{
					"Removed (1) reference to ",
					TMP_MaterialManager.m_materialList[num].stencilMaterial.name,
					". There are ",
					TMP_MaterialManager.m_materialList[num].count.ToString(),
					" references left."
				}));
				return;
			}
			Debug.Log("Removed last reference to " + TMP_MaterialManager.m_materialList[num].stencilMaterial.name + " with ID " + TMP_MaterialManager.m_materialList[num].stencilMaterial.GetInstanceID().ToString());
			Object.DestroyImmediate(TMP_MaterialManager.m_materialList[num].stencilMaterial);
			TMP_MaterialManager.m_materialList.RemoveAt(num);
		}

		// Token: 0x060048F5 RID: 18677 RVA: 0x0015497C File Offset: 0x00152B7C
		public static void ClearMaterials()
		{
			if (TMP_MaterialManager.m_materialList.Count<TMP_MaterialManager.MaskingMaterial>() == 0)
			{
				Debug.Log("Material List has already been cleared.");
				return;
			}
			for (int i = 0; i < TMP_MaterialManager.m_materialList.Count<TMP_MaterialManager.MaskingMaterial>(); i++)
			{
				Object.DestroyImmediate(TMP_MaterialManager.m_materialList[i].stencilMaterial);
				TMP_MaterialManager.m_materialList.RemoveAt(i);
			}
		}

		// Token: 0x060048F6 RID: 18678 RVA: 0x001549D8 File Offset: 0x00152BD8
		public static int GetStencilID(GameObject obj)
		{
			int num = 0;
			List<Mask> list = TMP_ListPool<Mask>.Get();
			obj.GetComponentsInParent<Mask>(false, list);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].MaskEnabled())
				{
					num++;
				}
			}
			TMP_ListPool<Mask>.Release(list);
			return Mathf.Min((1 << num) - 1, 255);
		}

		// Token: 0x060048F7 RID: 18679 RVA: 0x00154A30 File Offset: 0x00152C30
		public static Material GetFallbackMaterial(Material sourceMaterial, Material targetMaterial)
		{
			int instanceID = sourceMaterial.GetInstanceID();
			Texture texture = targetMaterial.GetTexture(ShaderUtilities.ID_MainTex);
			int instanceID2 = texture.GetInstanceID();
			long num = (long)instanceID << 32 | (long)((ulong)instanceID2);
			TMP_MaterialManager.FallbackMaterial fallbackMaterial;
			if (TMP_MaterialManager.m_fallbackMaterials.TryGetValue(num, out fallbackMaterial))
			{
				return fallbackMaterial.fallbackMaterial;
			}
			Material material = new Material(sourceMaterial);
			material.hideFlags = HideFlags.HideAndDontSave;
			material.SetTexture(ShaderUtilities.ID_MainTex, texture);
			material.SetFloat(ShaderUtilities.ID_GradientScale, targetMaterial.GetFloat(ShaderUtilities.ID_GradientScale));
			material.SetFloat(ShaderUtilities.ID_TextureWidth, targetMaterial.GetFloat(ShaderUtilities.ID_TextureWidth));
			material.SetFloat(ShaderUtilities.ID_TextureHeight, targetMaterial.GetFloat(ShaderUtilities.ID_TextureHeight));
			material.SetFloat(ShaderUtilities.ID_WeightNormal, targetMaterial.GetFloat(ShaderUtilities.ID_WeightNormal));
			material.SetFloat(ShaderUtilities.ID_WeightBold, targetMaterial.GetFloat(ShaderUtilities.ID_WeightBold));
			fallbackMaterial = new TMP_MaterialManager.FallbackMaterial();
			fallbackMaterial.baseID = instanceID;
			fallbackMaterial.baseMaterial = sourceMaterial;
			fallbackMaterial.fallbackMaterial = material;
			fallbackMaterial.count = 0;
			TMP_MaterialManager.m_fallbackMaterials.Add(num, fallbackMaterial);
			TMP_MaterialManager.m_fallbackMaterialLookup.Add(material.GetInstanceID(), num);
			return material;
		}

		// Token: 0x060048F8 RID: 18680 RVA: 0x00154B54 File Offset: 0x00152D54
		public static void AddFallbackMaterialReference(Material targetMaterial)
		{
			if (targetMaterial == null)
			{
				return;
			}
			int instanceID = targetMaterial.GetInstanceID();
			long key;
			TMP_MaterialManager.FallbackMaterial fallbackMaterial;
			if (TMP_MaterialManager.m_fallbackMaterialLookup.TryGetValue(instanceID, out key) && TMP_MaterialManager.m_fallbackMaterials.TryGetValue(key, out fallbackMaterial))
			{
				fallbackMaterial.count++;
			}
		}

		// Token: 0x060048F9 RID: 18681 RVA: 0x00154BA0 File Offset: 0x00152DA0
		public static void RemoveFallbackMaterialReference(Material targetMaterial)
		{
			if (targetMaterial == null)
			{
				return;
			}
			int instanceID = targetMaterial.GetInstanceID();
			long num;
			TMP_MaterialManager.FallbackMaterial fallbackMaterial;
			if (TMP_MaterialManager.m_fallbackMaterialLookup.TryGetValue(instanceID, out num) && TMP_MaterialManager.m_fallbackMaterials.TryGetValue(num, out fallbackMaterial))
			{
				fallbackMaterial.count--;
				if (fallbackMaterial.count < 1)
				{
					TMP_MaterialManager.m_fallbackCleanupList.Add(num);
				}
			}
		}

		// Token: 0x060048FA RID: 18682 RVA: 0x00154C00 File Offset: 0x00152E00
		public static void CleanupFallbackMaterials()
		{
			for (int i = 0; i < TMP_MaterialManager.m_fallbackCleanupList.Count; i++)
			{
				long key = TMP_MaterialManager.m_fallbackCleanupList[i];
				TMP_MaterialManager.FallbackMaterial fallbackMaterial;
				if (TMP_MaterialManager.m_fallbackMaterials.TryGetValue(key, out fallbackMaterial) && fallbackMaterial.count < 1)
				{
					Material fallbackMaterial2 = fallbackMaterial.fallbackMaterial;
					Object.DestroyImmediate(fallbackMaterial2);
					TMP_MaterialManager.m_fallbackMaterials.Remove(key);
					TMP_MaterialManager.m_fallbackMaterialLookup.Remove(fallbackMaterial2.GetInstanceID());
				}
			}
		}

		// Token: 0x060048FB RID: 18683 RVA: 0x00154C74 File Offset: 0x00152E74
		public static void ReleaseFallbackMaterial(Material fallackMaterial)
		{
			if (fallackMaterial == null)
			{
				return;
			}
			int instanceID = fallackMaterial.GetInstanceID();
			long key;
			TMP_MaterialManager.FallbackMaterial fallbackMaterial;
			if (TMP_MaterialManager.m_fallbackMaterialLookup.TryGetValue(instanceID, out key) && TMP_MaterialManager.m_fallbackMaterials.TryGetValue(key, out fallbackMaterial))
			{
				if (fallbackMaterial.count > 1)
				{
					fallbackMaterial.count--;
					return;
				}
				Object.DestroyImmediate(fallbackMaterial.fallbackMaterial);
				TMP_MaterialManager.m_fallbackMaterials.Remove(key);
				TMP_MaterialManager.m_fallbackMaterialLookup.Remove(instanceID);
				fallackMaterial = null;
			}
		}

		// Token: 0x060048FC RID: 18684 RVA: 0x00154CF0 File Offset: 0x00152EF0
		public static void CopyMaterialPresetProperties(Material source, Material destination)
		{
			Texture texture = destination.GetTexture(ShaderUtilities.ID_MainTex);
			float @float = destination.GetFloat(ShaderUtilities.ID_GradientScale);
			float float2 = destination.GetFloat(ShaderUtilities.ID_TextureWidth);
			float float3 = destination.GetFloat(ShaderUtilities.ID_TextureHeight);
			float float4 = destination.GetFloat(ShaderUtilities.ID_WeightNormal);
			float float5 = destination.GetFloat(ShaderUtilities.ID_WeightBold);
			destination.CopyPropertiesFromMaterial(source);
			destination.shaderKeywords = source.shaderKeywords;
			destination.SetTexture(ShaderUtilities.ID_MainTex, texture);
			destination.SetFloat(ShaderUtilities.ID_GradientScale, @float);
			destination.SetFloat(ShaderUtilities.ID_TextureWidth, float2);
			destination.SetFloat(ShaderUtilities.ID_TextureHeight, float3);
			destination.SetFloat(ShaderUtilities.ID_WeightNormal, float4);
			destination.SetFloat(ShaderUtilities.ID_WeightBold, float5);
		}

		// Token: 0x04004902 RID: 18690
		private static List<TMP_MaterialManager.MaskingMaterial> m_materialList = new List<TMP_MaterialManager.MaskingMaterial>();

		// Token: 0x04004903 RID: 18691
		private static Dictionary<long, TMP_MaterialManager.FallbackMaterial> m_fallbackMaterials = new Dictionary<long, TMP_MaterialManager.FallbackMaterial>();

		// Token: 0x04004904 RID: 18692
		private static Dictionary<int, long> m_fallbackMaterialLookup = new Dictionary<int, long>();

		// Token: 0x04004905 RID: 18693
		private static List<long> m_fallbackCleanupList = new List<long>();

		// Token: 0x02001ACB RID: 6859
		private class FallbackMaterial
		{
			// Token: 0x04009A86 RID: 39558
			public int baseID;

			// Token: 0x04009A87 RID: 39559
			public Material baseMaterial;

			// Token: 0x04009A88 RID: 39560
			public Material fallbackMaterial;

			// Token: 0x04009A89 RID: 39561
			public int count;
		}

		// Token: 0x02001ACC RID: 6860
		private class MaskingMaterial
		{
			// Token: 0x04009A8A RID: 39562
			public Material baseMaterial;

			// Token: 0x04009A8B RID: 39563
			public Material stencilMaterial;

			// Token: 0x04009A8C RID: 39564
			public int count;

			// Token: 0x04009A8D RID: 39565
			public int stencilID;
		}
	}
}
