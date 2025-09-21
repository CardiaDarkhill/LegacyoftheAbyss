using System;
using UnityEngine;

namespace GUIBlendModes
{
	// Token: 0x02000ADE RID: 2782
	public static class BlendMaterials
	{
		// Token: 0x06005824 RID: 22564 RVA: 0x001BFB84 File Offset: 0x001BDD84
		public static void Initialize()
		{
			BlendMaterials.Materials = new Material[84];
			for (int i = 0; i < 21; i++)
			{
				BlendMaterials.Materials[i] = Resources.Load<Material>("UIBlend" + (i + BlendMode.Darken).ToString());
			}
			for (int j = 21; j < 42; j++)
			{
				BlendMaterials.Materials[j] = Resources.Load<Material>("UIBlend" + ((BlendMode)(j - 20)).ToString() + "Optimized");
			}
			for (int k = 42; k < 63; k++)
			{
				BlendMaterials.Materials[k] = Resources.Load<Material>("UIFontBlend" + ((BlendMode)(k - 41)).ToString());
			}
			for (int l = 63; l < 84; l++)
			{
				BlendMaterials.Materials[l] = Resources.Load<Material>("UIFontBlend" + ((BlendMode)(l - 62)).ToString() + "Optimized");
			}
			BlendMaterials.Initialized = true;
		}

		// Token: 0x06005825 RID: 22565 RVA: 0x001BFC8C File Offset: 0x001BDE8C
		public static Material GetMaterial(BlendMode mode, bool font, bool optimized)
		{
			if (!BlendMaterials.Initialized)
			{
				BlendMaterials.Initialize();
			}
			if (font)
			{
				if (mode != BlendMode.Normal)
				{
					return BlendMaterials.Materials[mode - BlendMode.Darken + (optimized ? 63 : 42)];
				}
				return null;
			}
			else
			{
				if (mode != BlendMode.Normal)
				{
					return BlendMaterials.Materials[mode - BlendMode.Darken + (optimized ? 21 : 0)];
				}
				return null;
			}
		}

		// Token: 0x040053D5 RID: 21461
		public static Material[] Materials;

		// Token: 0x040053D6 RID: 21462
		public static bool Initialized;
	}
}
