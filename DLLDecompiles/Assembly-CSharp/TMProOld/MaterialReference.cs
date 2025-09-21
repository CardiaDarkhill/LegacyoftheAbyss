using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x020007E5 RID: 2021
	public struct MaterialReference
	{
		// Token: 0x060046F3 RID: 18163 RVA: 0x0013BE78 File Offset: 0x0013A078
		public MaterialReference(int index, TMP_FontAsset fontAsset, TMP_SpriteAsset spriteAsset, Material material, float padding)
		{
			this.index = index;
			this.fontAsset = fontAsset;
			this.spriteAsset = spriteAsset;
			this.material = material;
			this.isDefaultMaterial = (material.GetInstanceID() == fontAsset.material.GetInstanceID());
			this.isFallbackMaterial = false;
			this.fallbackMaterial = null;
			this.padding = padding;
			this.referenceCount = 0;
		}

		// Token: 0x060046F4 RID: 18164 RVA: 0x0013BEE0 File Offset: 0x0013A0E0
		public static bool Contains(MaterialReference[] materialReferences, TMP_FontAsset fontAsset)
		{
			int instanceID = fontAsset.GetInstanceID();
			int num = 0;
			while (num < materialReferences.Length && materialReferences[num].fontAsset != null)
			{
				if (materialReferences[num].fontAsset.GetInstanceID() == instanceID)
				{
					return true;
				}
				num++;
			}
			return false;
		}

		// Token: 0x060046F5 RID: 18165 RVA: 0x0013BF30 File Offset: 0x0013A130
		public static int AddMaterialReference(Material material, TMP_FontAsset fontAsset, MaterialReference[] materialReferences, Dictionary<int, int> materialReferenceIndexLookup)
		{
			int instanceID = material.GetInstanceID();
			int num = 0;
			if (materialReferenceIndexLookup.TryGetValue(instanceID, out num))
			{
				return num;
			}
			num = materialReferenceIndexLookup.Count;
			materialReferenceIndexLookup[instanceID] = num;
			materialReferences[num].index = num;
			materialReferences[num].fontAsset = fontAsset;
			materialReferences[num].spriteAsset = null;
			materialReferences[num].material = material;
			materialReferences[num].isDefaultMaterial = (instanceID == fontAsset.material.GetInstanceID());
			materialReferences[num].referenceCount = 0;
			return num;
		}

		// Token: 0x060046F6 RID: 18166 RVA: 0x0013BFC4 File Offset: 0x0013A1C4
		public static int AddMaterialReference(Material material, TMP_SpriteAsset spriteAsset, MaterialReference[] materialReferences, Dictionary<int, int> materialReferenceIndexLookup)
		{
			int instanceID = material.GetInstanceID();
			int num = 0;
			if (materialReferenceIndexLookup.TryGetValue(instanceID, out num))
			{
				return num;
			}
			num = materialReferenceIndexLookup.Count;
			materialReferenceIndexLookup[instanceID] = num;
			materialReferences[num].index = num;
			materialReferences[num].fontAsset = materialReferences[0].fontAsset;
			materialReferences[num].spriteAsset = spriteAsset;
			materialReferences[num].material = material;
			materialReferences[num].isDefaultMaterial = true;
			materialReferences[num].referenceCount = 0;
			return num;
		}

		// Token: 0x0400472D RID: 18221
		public int index;

		// Token: 0x0400472E RID: 18222
		public TMP_FontAsset fontAsset;

		// Token: 0x0400472F RID: 18223
		public TMP_SpriteAsset spriteAsset;

		// Token: 0x04004730 RID: 18224
		public Material material;

		// Token: 0x04004731 RID: 18225
		public bool isDefaultMaterial;

		// Token: 0x04004732 RID: 18226
		public bool isFallbackMaterial;

		// Token: 0x04004733 RID: 18227
		public Material fallbackMaterial;

		// Token: 0x04004734 RID: 18228
		public float padding;

		// Token: 0x04004735 RID: 18229
		public int referenceCount;
	}
}
