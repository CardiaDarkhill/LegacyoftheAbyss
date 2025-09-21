using System;
using System.Collections.Generic;
using UnityEngine;

namespace TMProOld
{
	// Token: 0x0200081B RID: 2075
	[Serializable]
	public class TMP_StyleSheet : ScriptableObject
	{
		// Token: 0x17000882 RID: 2178
		// (get) Token: 0x0600493F RID: 18751 RVA: 0x001565A4 File Offset: 0x001547A4
		public static TMP_StyleSheet instance
		{
			get
			{
				if (TMP_StyleSheet.s_Instance == null)
				{
					TMP_StyleSheet.s_Instance = TMP_Settings.defaultStyleSheet;
					if (TMP_StyleSheet.s_Instance == null)
					{
						TMP_StyleSheet.s_Instance = (Resources.Load("Style Sheets/TMP Default Style Sheet") as TMP_StyleSheet);
					}
					if (TMP_StyleSheet.s_Instance == null)
					{
						return null;
					}
					TMP_StyleSheet.s_Instance.LoadStyleDictionaryInternal();
				}
				return TMP_StyleSheet.s_Instance;
			}
		}

		// Token: 0x06004940 RID: 18752 RVA: 0x00156607 File Offset: 0x00154807
		public static TMP_StyleSheet LoadDefaultStyleSheet()
		{
			return TMP_StyleSheet.instance;
		}

		// Token: 0x06004941 RID: 18753 RVA: 0x0015660E File Offset: 0x0015480E
		public static TMP_Style GetStyle(int hashCode)
		{
			return TMP_StyleSheet.instance.GetStyleInternal(hashCode);
		}

		// Token: 0x06004942 RID: 18754 RVA: 0x0015661C File Offset: 0x0015481C
		private TMP_Style GetStyleInternal(int hashCode)
		{
			TMP_Style result;
			if (this.m_StyleDictionary.TryGetValue(hashCode, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06004943 RID: 18755 RVA: 0x0015663C File Offset: 0x0015483C
		public void UpdateStyleDictionaryKey(int old_key, int new_key)
		{
			if (this.m_StyleDictionary.ContainsKey(old_key))
			{
				TMP_Style value = this.m_StyleDictionary[old_key];
				this.m_StyleDictionary.Add(new_key, value);
				this.m_StyleDictionary.Remove(old_key);
			}
		}

		// Token: 0x06004944 RID: 18756 RVA: 0x0015667E File Offset: 0x0015487E
		public static void RefreshStyles()
		{
			TMP_StyleSheet.s_Instance.LoadStyleDictionaryInternal();
		}

		// Token: 0x06004945 RID: 18757 RVA: 0x0015668C File Offset: 0x0015488C
		private void LoadStyleDictionaryInternal()
		{
			this.m_StyleDictionary.Clear();
			for (int i = 0; i < this.m_StyleList.Count; i++)
			{
				this.m_StyleList[i].RefreshStyle();
				if (!this.m_StyleDictionary.ContainsKey(this.m_StyleList[i].hashCode))
				{
					this.m_StyleDictionary.Add(this.m_StyleList[i].hashCode, this.m_StyleList[i]);
				}
			}
		}

		// Token: 0x0400493D RID: 18749
		private static TMP_StyleSheet s_Instance;

		// Token: 0x0400493E RID: 18750
		[SerializeField]
		private List<TMP_Style> m_StyleList = new List<TMP_Style>(1);

		// Token: 0x0400493F RID: 18751
		private Dictionary<int, TMP_Style> m_StyleDictionary = new Dictionary<int, TMP_Style>();
	}
}
