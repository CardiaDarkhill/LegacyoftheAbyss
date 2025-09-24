using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x02000819 RID: 2073
	public class TMP_SpriteAsset : TMP_Asset
	{
		// Token: 0x1700087B RID: 2171
		// (get) Token: 0x06004930 RID: 18736 RVA: 0x0015640D File Offset: 0x0015460D
		public static TMP_SpriteAsset defaultSpriteAsset
		{
			get
			{
				if (TMP_SpriteAsset.m_defaultSpriteAsset == null)
				{
					TMP_SpriteAsset.m_defaultSpriteAsset = Resources.Load<TMP_SpriteAsset>("Sprite Assets/Default Sprite Asset");
				}
				return TMP_SpriteAsset.m_defaultSpriteAsset;
			}
		}

		// Token: 0x06004931 RID: 18737 RVA: 0x00156430 File Offset: 0x00154630
		private void OnEnable()
		{
		}

		// Token: 0x06004932 RID: 18738 RVA: 0x00156432 File Offset: 0x00154632
		private Material GetDefaultSpriteMaterial()
		{
			ShaderUtilities.GetShaderPropertyIDs();
			Material material = new Material(Shader.Find("TextMeshPro/Sprite"));
			material.SetTexture(ShaderUtilities.ID_MainTex, this.spriteSheet);
			material.hideFlags = HideFlags.HideInHierarchy;
			return material;
		}

		// Token: 0x06004933 RID: 18739 RVA: 0x00156460 File Offset: 0x00154660
		public int GetSpriteIndex(int hashCode)
		{
			for (int i = 0; i < this.spriteInfoList.Count; i++)
			{
				if (this.spriteInfoList[i].hashCode == hashCode)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x04004933 RID: 18739
		public static TMP_SpriteAsset m_defaultSpriteAsset;

		// Token: 0x04004934 RID: 18740
		public Texture spriteSheet;

		// Token: 0x04004935 RID: 18741
		public List<TMP_Sprite> spriteInfoList;

		// Token: 0x04004936 RID: 18742
		private List<Sprite> m_sprites;
	}
}
