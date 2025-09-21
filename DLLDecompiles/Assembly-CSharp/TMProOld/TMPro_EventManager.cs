using System;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x020007EB RID: 2027
	public static class TMPro_EventManager
	{
		// Token: 0x060047AE RID: 18350 RVA: 0x0014D676 File Offset: 0x0014B876
		public static void ON_PRE_RENDER_OBJECT_CHANGED()
		{
			TMPro_EventManager.OnPreRenderObject_Event.Call();
		}

		// Token: 0x060047AF RID: 18351 RVA: 0x0014D682 File Offset: 0x0014B882
		public static void ON_MATERIAL_PROPERTY_CHANGED(bool isChanged, Material mat)
		{
			TMPro_EventManager.MATERIAL_PROPERTY_EVENT.Call(isChanged, mat);
		}

		// Token: 0x060047B0 RID: 18352 RVA: 0x0014D690 File Offset: 0x0014B890
		public static void ON_FONT_PROPERTY_CHANGED(bool isChanged, TMP_FontAsset font)
		{
			TMPro_EventManager.FONT_PROPERTY_EVENT.Call(isChanged, font);
		}

		// Token: 0x060047B1 RID: 18353 RVA: 0x0014D69E File Offset: 0x0014B89E
		public static void ON_SPRITE_ASSET_PROPERTY_CHANGED(bool isChanged, Object obj)
		{
			TMPro_EventManager.SPRITE_ASSET_PROPERTY_EVENT.Call(isChanged, obj);
		}

		// Token: 0x060047B2 RID: 18354 RVA: 0x0014D6AC File Offset: 0x0014B8AC
		public static void ON_TEXTMESHPRO_PROPERTY_CHANGED(bool isChanged, TextMeshPro obj)
		{
			TMPro_EventManager.TEXTMESHPRO_PROPERTY_EVENT.Call(isChanged, obj);
		}

		// Token: 0x060047B3 RID: 18355 RVA: 0x0014D6BA File Offset: 0x0014B8BA
		public static void ON_DRAG_AND_DROP_MATERIAL_CHANGED(GameObject sender, Material currentMaterial, Material newMaterial)
		{
			TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT.Call(sender, currentMaterial, newMaterial);
		}

		// Token: 0x060047B4 RID: 18356 RVA: 0x0014D6C9 File Offset: 0x0014B8C9
		public static void ON_TEXT_STYLE_PROPERTY_CHANGED(bool isChanged)
		{
			TMPro_EventManager.TEXT_STYLE_PROPERTY_EVENT.Call(isChanged);
		}

		// Token: 0x060047B5 RID: 18357 RVA: 0x0014D6D6 File Offset: 0x0014B8D6
		public static void ON_COLOR_GRAIDENT_PROPERTY_CHANGED(TMP_ColorGradient gradient)
		{
			TMPro_EventManager.COLOR_GRADIENT_PROPERTY_EVENT.Call(gradient);
		}

		// Token: 0x060047B6 RID: 18358 RVA: 0x0014D6E3 File Offset: 0x0014B8E3
		public static void ON_TEXT_CHANGED(Object obj)
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Call(obj);
		}

		// Token: 0x060047B7 RID: 18359 RVA: 0x0014D6F0 File Offset: 0x0014B8F0
		public static void ON_TMP_SETTINGS_CHANGED()
		{
			TMPro_EventManager.TMP_SETTINGS_PROPERTY_EVENT.Call();
		}

		// Token: 0x060047B8 RID: 18360 RVA: 0x0014D6FC File Offset: 0x0014B8FC
		public static void ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED(bool isChanged, TextMeshProUGUI obj)
		{
			TMPro_EventManager.TEXTMESHPRO_UGUI_PROPERTY_EVENT.Call(isChanged, obj);
		}

		// Token: 0x060047B9 RID: 18361 RVA: 0x0014D70A File Offset: 0x0014B90A
		public static void ON_COMPUTE_DT_EVENT(object Sender, Compute_DT_EventArgs e)
		{
			TMPro_EventManager.COMPUTE_DT_EVENT.Call(Sender, e);
		}

		// Token: 0x04004781 RID: 18305
		public static readonly FastAction<object, Compute_DT_EventArgs> COMPUTE_DT_EVENT = new FastAction<object, Compute_DT_EventArgs>();

		// Token: 0x04004782 RID: 18306
		public static readonly FastAction<bool, Material> MATERIAL_PROPERTY_EVENT = new FastAction<bool, Material>();

		// Token: 0x04004783 RID: 18307
		public static readonly FastAction<bool, TMP_FontAsset> FONT_PROPERTY_EVENT = new FastAction<bool, TMP_FontAsset>();

		// Token: 0x04004784 RID: 18308
		public static readonly FastAction<bool, Object> SPRITE_ASSET_PROPERTY_EVENT = new FastAction<bool, Object>();

		// Token: 0x04004785 RID: 18309
		public static readonly FastAction<bool, TextMeshPro> TEXTMESHPRO_PROPERTY_EVENT = new FastAction<bool, TextMeshPro>();

		// Token: 0x04004786 RID: 18310
		public static readonly FastAction<GameObject, Material, Material> DRAG_AND_DROP_MATERIAL_EVENT = new FastAction<GameObject, Material, Material>();

		// Token: 0x04004787 RID: 18311
		public static readonly FastAction<bool> TEXT_STYLE_PROPERTY_EVENT = new FastAction<bool>();

		// Token: 0x04004788 RID: 18312
		public static readonly FastAction<TMP_ColorGradient> COLOR_GRADIENT_PROPERTY_EVENT = new FastAction<TMP_ColorGradient>();

		// Token: 0x04004789 RID: 18313
		public static readonly FastAction TMP_SETTINGS_PROPERTY_EVENT = new FastAction();

		// Token: 0x0400478A RID: 18314
		public static readonly FastAction<bool, TextMeshProUGUI> TEXTMESHPRO_UGUI_PROPERTY_EVENT = new FastAction<bool, TextMeshProUGUI>();

		// Token: 0x0400478B RID: 18315
		public static readonly FastAction OnPreRenderObject_Event = new FastAction();

		// Token: 0x0400478C RID: 18316
		public static readonly FastAction<Object> TEXT_CHANGED_EVENT = new FastAction<Object>();
	}
}
