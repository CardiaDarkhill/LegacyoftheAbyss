using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMProOld
{
	// Token: 0x02000830 RID: 2096
	public class TMP_UpdateManager
	{
		// Token: 0x170008E4 RID: 2276
		// (get) Token: 0x06004A94 RID: 19092 RVA: 0x00161F20 File Offset: 0x00160120
		public static TMP_UpdateManager instance
		{
			get
			{
				if (TMP_UpdateManager.s_Instance == null)
				{
					TMP_UpdateManager.s_Instance = new TMP_UpdateManager();
				}
				return TMP_UpdateManager.s_Instance;
			}
		}

		// Token: 0x06004A95 RID: 19093 RVA: 0x00161F38 File Offset: 0x00160138
		protected TMP_UpdateManager()
		{
			Canvas.willRenderCanvases += this.DoRebuild;
		}

		// Token: 0x06004A96 RID: 19094 RVA: 0x00161F88 File Offset: 0x00160188
		public static void RegisterTextElementForLayoutRebuild(TMP_Text element)
		{
			TMP_UpdateManager.instance.InternalRegisterTextElementForLayoutRebuild(element);
		}

		// Token: 0x06004A97 RID: 19095 RVA: 0x00161F98 File Offset: 0x00160198
		private bool InternalRegisterTextElementForLayoutRebuild(TMP_Text element)
		{
			int instanceID = element.GetInstanceID();
			if (this.m_LayoutQueueLookup.ContainsKey(instanceID))
			{
				return false;
			}
			this.m_LayoutQueueLookup[instanceID] = instanceID;
			this.m_LayoutRebuildQueue.Add(element);
			return true;
		}

		// Token: 0x06004A98 RID: 19096 RVA: 0x00161FD6 File Offset: 0x001601D6
		public static void RegisterTextElementForGraphicRebuild(TMP_Text element)
		{
			TMP_UpdateManager.instance.InternalRegisterTextElementForGraphicRebuild(element);
		}

		// Token: 0x06004A99 RID: 19097 RVA: 0x00161FE4 File Offset: 0x001601E4
		private bool InternalRegisterTextElementForGraphicRebuild(TMP_Text element)
		{
			int instanceID = element.GetInstanceID();
			if (this.m_GraphicQueueLookup.ContainsKey(instanceID))
			{
				return false;
			}
			this.m_GraphicQueueLookup[instanceID] = instanceID;
			this.m_GraphicRebuildQueue.Add(element);
			return true;
		}

		// Token: 0x06004A9A RID: 19098 RVA: 0x00162022 File Offset: 0x00160222
		private void OnCameraPreRender(Camera cam)
		{
			this.DoRebuild();
		}

		// Token: 0x06004A9B RID: 19099 RVA: 0x0016202C File Offset: 0x0016022C
		private void DoRebuild()
		{
			for (int i = 0; i < this.m_LayoutRebuildQueue.Count; i++)
			{
				this.m_LayoutRebuildQueue[i].Rebuild(CanvasUpdate.Prelayout);
			}
			if (this.m_LayoutRebuildQueue.Count > 0)
			{
				this.m_LayoutRebuildQueue.Clear();
				this.m_LayoutQueueLookup.Clear();
			}
			for (int j = 0; j < this.m_GraphicRebuildQueue.Count; j++)
			{
				this.m_GraphicRebuildQueue[j].Rebuild(CanvasUpdate.PreRender);
			}
			if (this.m_GraphicRebuildQueue.Count > 0)
			{
				this.m_GraphicRebuildQueue.Clear();
				this.m_GraphicQueueLookup.Clear();
			}
		}

		// Token: 0x06004A9C RID: 19100 RVA: 0x001620D1 File Offset: 0x001602D1
		public static void UnRegisterTextElementForRebuild(TMP_Text element)
		{
			TMP_UpdateManager.instance.InternalUnRegisterTextElementForGraphicRebuild(element);
			TMP_UpdateManager.instance.InternalUnRegisterTextElementForLayoutRebuild(element);
		}

		// Token: 0x06004A9D RID: 19101 RVA: 0x001620EC File Offset: 0x001602EC
		private void InternalUnRegisterTextElementForGraphicRebuild(TMP_Text element)
		{
			int instanceID = element.GetInstanceID();
			TMP_UpdateManager.instance.m_GraphicRebuildQueue.Remove(element);
			this.m_GraphicQueueLookup.Remove(instanceID);
		}

		// Token: 0x06004A9E RID: 19102 RVA: 0x00162120 File Offset: 0x00160320
		private void InternalUnRegisterTextElementForLayoutRebuild(TMP_Text element)
		{
			int instanceID = element.GetInstanceID();
			TMP_UpdateManager.instance.m_LayoutRebuildQueue.Remove(element);
			this.m_LayoutQueueLookup.Remove(instanceID);
		}

		// Token: 0x04004A82 RID: 19074
		private static TMP_UpdateManager s_Instance;

		// Token: 0x04004A83 RID: 19075
		private readonly List<TMP_Text> m_LayoutRebuildQueue = new List<TMP_Text>();

		// Token: 0x04004A84 RID: 19076
		private Dictionary<int, int> m_LayoutQueueLookup = new Dictionary<int, int>();

		// Token: 0x04004A85 RID: 19077
		private readonly List<TMP_Text> m_GraphicRebuildQueue = new List<TMP_Text>();

		// Token: 0x04004A86 RID: 19078
		private Dictionary<int, int> m_GraphicQueueLookup = new Dictionary<int, int>();
	}
}
