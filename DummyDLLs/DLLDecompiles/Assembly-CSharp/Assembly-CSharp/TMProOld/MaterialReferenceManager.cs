using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x020007E4 RID: 2020
	public class MaterialReferenceManager
	{
		// Token: 0x17000805 RID: 2053
		// (get) Token: 0x060046E1 RID: 18145 RVA: 0x0013BC93 File Offset: 0x00139E93
		public static MaterialReferenceManager instance
		{
			get
			{
				if (MaterialReferenceManager.s_Instance == null)
				{
					MaterialReferenceManager.s_Instance = new MaterialReferenceManager();
				}
				return MaterialReferenceManager.s_Instance;
			}
		}

		// Token: 0x060046E2 RID: 18146 RVA: 0x0013BCAB File Offset: 0x00139EAB
		public static void AddFontAsset(TMP_FontAsset fontAsset)
		{
			MaterialReferenceManager.instance.AddFontAssetInternal(fontAsset);
		}

		// Token: 0x060046E3 RID: 18147 RVA: 0x0013BCB8 File Offset: 0x00139EB8
		private void AddFontAssetInternal(TMP_FontAsset fontAsset)
		{
			if (this.m_FontAssetReferenceLookup.ContainsKey(fontAsset.hashCode))
			{
				return;
			}
			this.m_FontAssetReferenceLookup.Add(fontAsset.hashCode, fontAsset);
			this.m_FontMaterialReferenceLookup.Add(fontAsset.materialHashCode, fontAsset.material);
		}

		// Token: 0x060046E4 RID: 18148 RVA: 0x0013BCF7 File Offset: 0x00139EF7
		public static void AddSpriteAsset(TMP_SpriteAsset spriteAsset)
		{
			MaterialReferenceManager.instance.AddSpriteAssetInternal(spriteAsset);
		}

		// Token: 0x060046E5 RID: 18149 RVA: 0x0013BD04 File Offset: 0x00139F04
		private void AddSpriteAssetInternal(TMP_SpriteAsset spriteAsset)
		{
			if (this.m_SpriteAssetReferenceLookup.ContainsKey(spriteAsset.hashCode))
			{
				return;
			}
			this.m_SpriteAssetReferenceLookup.Add(spriteAsset.hashCode, spriteAsset);
			this.m_FontMaterialReferenceLookup.Add(spriteAsset.hashCode, spriteAsset.material);
		}

		// Token: 0x060046E6 RID: 18150 RVA: 0x0013BD43 File Offset: 0x00139F43
		public static void AddSpriteAsset(int hashCode, TMP_SpriteAsset spriteAsset)
		{
			MaterialReferenceManager.instance.AddSpriteAssetInternal(hashCode, spriteAsset);
		}

		// Token: 0x060046E7 RID: 18151 RVA: 0x0013BD51 File Offset: 0x00139F51
		private void AddSpriteAssetInternal(int hashCode, TMP_SpriteAsset spriteAsset)
		{
			if (this.m_SpriteAssetReferenceLookup.ContainsKey(hashCode))
			{
				return;
			}
			this.m_SpriteAssetReferenceLookup.Add(hashCode, spriteAsset);
			this.m_FontMaterialReferenceLookup.Add(hashCode, spriteAsset.material);
			if (spriteAsset.hashCode == 0)
			{
				spriteAsset.hashCode = hashCode;
			}
		}

		// Token: 0x060046E8 RID: 18152 RVA: 0x0013BD90 File Offset: 0x00139F90
		public static void AddFontMaterial(int hashCode, Material material)
		{
			MaterialReferenceManager.instance.AddFontMaterialInternal(hashCode, material);
		}

		// Token: 0x060046E9 RID: 18153 RVA: 0x0013BD9E File Offset: 0x00139F9E
		private void AddFontMaterialInternal(int hashCode, Material material)
		{
			this.m_FontMaterialReferenceLookup.Add(hashCode, material);
		}

		// Token: 0x060046EA RID: 18154 RVA: 0x0013BDAD File Offset: 0x00139FAD
		public bool Contains(TMP_FontAsset font)
		{
			return this.m_FontAssetReferenceLookup.ContainsKey(font.hashCode);
		}

		// Token: 0x060046EB RID: 18155 RVA: 0x0013BDC5 File Offset: 0x00139FC5
		public bool Contains(TMP_SpriteAsset sprite)
		{
			return this.m_FontAssetReferenceLookup.ContainsKey(sprite.hashCode);
		}

		// Token: 0x060046EC RID: 18156 RVA: 0x0013BDDD File Offset: 0x00139FDD
		public static bool TryGetFontAsset(int hashCode, out TMP_FontAsset fontAsset)
		{
			return MaterialReferenceManager.instance.TryGetFontAssetInternal(hashCode, out fontAsset);
		}

		// Token: 0x060046ED RID: 18157 RVA: 0x0013BDEB File Offset: 0x00139FEB
		private bool TryGetFontAssetInternal(int hashCode, out TMP_FontAsset fontAsset)
		{
			fontAsset = null;
			return this.m_FontAssetReferenceLookup.TryGetValue(hashCode, out fontAsset);
		}

		// Token: 0x060046EE RID: 18158 RVA: 0x0013BE02 File Offset: 0x0013A002
		public static bool TryGetSpriteAsset(int hashCode, out TMP_SpriteAsset spriteAsset)
		{
			return MaterialReferenceManager.instance.TryGetSpriteAssetInternal(hashCode, out spriteAsset);
		}

		// Token: 0x060046EF RID: 18159 RVA: 0x0013BE10 File Offset: 0x0013A010
		private bool TryGetSpriteAssetInternal(int hashCode, out TMP_SpriteAsset spriteAsset)
		{
			spriteAsset = null;
			return this.m_SpriteAssetReferenceLookup.TryGetValue(hashCode, out spriteAsset);
		}

		// Token: 0x060046F0 RID: 18160 RVA: 0x0013BE27 File Offset: 0x0013A027
		public static bool TryGetMaterial(int hashCode, out Material material)
		{
			return MaterialReferenceManager.instance.TryGetMaterialInternal(hashCode, out material);
		}

		// Token: 0x060046F1 RID: 18161 RVA: 0x0013BE35 File Offset: 0x0013A035
		private bool TryGetMaterialInternal(int hashCode, out Material material)
		{
			material = null;
			return this.m_FontMaterialReferenceLookup.TryGetValue(hashCode, out material);
		}

		// Token: 0x04004729 RID: 18217
		private static MaterialReferenceManager s_Instance;

		// Token: 0x0400472A RID: 18218
		private Dictionary<int, Material> m_FontMaterialReferenceLookup = new Dictionary<int, Material>();

		// Token: 0x0400472B RID: 18219
		private Dictionary<int, TMP_FontAsset> m_FontAssetReferenceLookup = new Dictionary<int, TMP_FontAsset>();

		// Token: 0x0400472C RID: 18220
		private Dictionary<int, TMP_SpriteAsset> m_SpriteAssetReferenceLookup = new Dictionary<int, TMP_SpriteAsset>();
	}
}
