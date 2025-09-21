using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TMProOld
{
	// Token: 0x02000831 RID: 2097
	public class TMP_UpdateRegistry
	{
		// Token: 0x170008E5 RID: 2277
		// (get) Token: 0x06004A9F RID: 19103 RVA: 0x00162152 File Offset: 0x00160352
		public static TMP_UpdateRegistry instance
		{
			get
			{
				if (TMP_UpdateRegistry.s_Instance == null)
				{
					TMP_UpdateRegistry.s_Instance = new TMP_UpdateRegistry();
				}
				return TMP_UpdateRegistry.s_Instance;
			}
		}

		// Token: 0x06004AA0 RID: 19104 RVA: 0x0016216C File Offset: 0x0016036C
		protected TMP_UpdateRegistry()
		{
			Canvas.willRenderCanvases += this.PerformUpdateForCanvasRendererObjects;
		}

		// Token: 0x06004AA1 RID: 19105 RVA: 0x001621BC File Offset: 0x001603BC
		public static void RegisterCanvasElementForLayoutRebuild(ICanvasElement element)
		{
			TMP_UpdateRegistry.instance.InternalRegisterCanvasElementForLayoutRebuild(element);
		}

		// Token: 0x06004AA2 RID: 19106 RVA: 0x001621CC File Offset: 0x001603CC
		private bool InternalRegisterCanvasElementForLayoutRebuild(ICanvasElement element)
		{
			int instanceID = (element as Object).GetInstanceID();
			if (this.m_LayoutQueueLookup.ContainsKey(instanceID))
			{
				return false;
			}
			this.m_LayoutQueueLookup[instanceID] = instanceID;
			this.m_LayoutRebuildQueue.Add(element);
			return true;
		}

		// Token: 0x06004AA3 RID: 19107 RVA: 0x0016220F File Offset: 0x0016040F
		public static void RegisterCanvasElementForGraphicRebuild(ICanvasElement element)
		{
			TMP_UpdateRegistry.instance.InternalRegisterCanvasElementForGraphicRebuild(element);
		}

		// Token: 0x06004AA4 RID: 19108 RVA: 0x00162220 File Offset: 0x00160420
		private bool InternalRegisterCanvasElementForGraphicRebuild(ICanvasElement element)
		{
			int instanceID = (element as Object).GetInstanceID();
			if (this.m_GraphicQueueLookup.ContainsKey(instanceID))
			{
				return false;
			}
			this.m_GraphicQueueLookup[instanceID] = instanceID;
			this.m_GraphicRebuildQueue.Add(element);
			return true;
		}

		// Token: 0x06004AA5 RID: 19109 RVA: 0x00162264 File Offset: 0x00160464
		private void PerformUpdateForCanvasRendererObjects()
		{
			for (int i = 0; i < this.m_LayoutRebuildQueue.Count; i++)
			{
				TMP_UpdateRegistry.instance.m_LayoutRebuildQueue[i].Rebuild(CanvasUpdate.Prelayout);
			}
			if (this.m_LayoutRebuildQueue.Count > 0)
			{
				this.m_LayoutRebuildQueue.Clear();
				this.m_LayoutQueueLookup.Clear();
			}
			for (int j = 0; j < this.m_GraphicRebuildQueue.Count; j++)
			{
				TMP_UpdateRegistry.instance.m_GraphicRebuildQueue[j].Rebuild(CanvasUpdate.PreRender);
			}
			if (this.m_GraphicRebuildQueue.Count > 0)
			{
				this.m_GraphicRebuildQueue.Clear();
				this.m_GraphicQueueLookup.Clear();
			}
		}

		// Token: 0x06004AA6 RID: 19110 RVA: 0x00162311 File Offset: 0x00160511
		private void PerformUpdateForMeshRendererObjects()
		{
			Debug.Log("Perform update of MeshRenderer objects.");
		}

		// Token: 0x06004AA7 RID: 19111 RVA: 0x0016231D File Offset: 0x0016051D
		public static void UnRegisterCanvasElementForRebuild(ICanvasElement element)
		{
			TMP_UpdateRegistry.instance.InternalUnRegisterCanvasElementForLayoutRebuild(element);
			TMP_UpdateRegistry.instance.InternalUnRegisterCanvasElementForGraphicRebuild(element);
		}

		// Token: 0x06004AA8 RID: 19112 RVA: 0x00162338 File Offset: 0x00160538
		private void InternalUnRegisterCanvasElementForLayoutRebuild(ICanvasElement element)
		{
			int instanceID = (element as Object).GetInstanceID();
			TMP_UpdateRegistry.instance.m_LayoutRebuildQueue.Remove(element);
			this.m_GraphicQueueLookup.Remove(instanceID);
		}

		// Token: 0x06004AA9 RID: 19113 RVA: 0x00162370 File Offset: 0x00160570
		private void InternalUnRegisterCanvasElementForGraphicRebuild(ICanvasElement element)
		{
			int instanceID = (element as Object).GetInstanceID();
			TMP_UpdateRegistry.instance.m_GraphicRebuildQueue.Remove(element);
			this.m_LayoutQueueLookup.Remove(instanceID);
		}

		// Token: 0x04004A87 RID: 19079
		private static TMP_UpdateRegistry s_Instance;

		// Token: 0x04004A88 RID: 19080
		private readonly List<ICanvasElement> m_LayoutRebuildQueue = new List<ICanvasElement>();

		// Token: 0x04004A89 RID: 19081
		private Dictionary<int, int> m_LayoutQueueLookup = new Dictionary<int, int>();

		// Token: 0x04004A8A RID: 19082
		private readonly List<ICanvasElement> m_GraphicRebuildQueue = new List<ICanvasElement>();

		// Token: 0x04004A8B RID: 19083
		private Dictionary<int, int> m_GraphicQueueLookup = new Dictionary<int, int>();
	}
}
