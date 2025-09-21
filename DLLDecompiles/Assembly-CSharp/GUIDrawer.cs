using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000768 RID: 1896
public sealed class GUIDrawer : MonoBehaviour
{
	// Token: 0x060043A1 RID: 17313 RVA: 0x00129470 File Offset: 0x00127670
	private void Awake()
	{
		if (GUIDrawer.instance && GUIDrawer.instance != this)
		{
			Object.Destroy(this);
			return;
		}
		GUIDrawer.instance = this;
		GUIDrawer.hasInstance = true;
		Object.DontDestroyOnLoad(base.gameObject);
		GUIDrawer.ToggleDrawer(GUIDrawer.drawers.Count > 0);
	}

	// Token: 0x060043A2 RID: 17314 RVA: 0x001294C6 File Offset: 0x001276C6
	private void OnDestroy()
	{
		if (GUIDrawer.instance == this)
		{
			GUIDrawer.instance = null;
			GUIDrawer.hasInstance = false;
		}
	}

	// Token: 0x060043A3 RID: 17315 RVA: 0x001294E4 File Offset: 0x001276E4
	public static void InsertInOrder(List<IOnGUI> list, IOnGUI item)
	{
		int num = list.BinarySearch(item, GUIDrawer.comparer);
		if (num < 0)
		{
			num = ~num;
		}
		list.Insert(num, item);
	}

	// Token: 0x060043A4 RID: 17316 RVA: 0x00129510 File Offset: 0x00127710
	private void OnGUI()
	{
		if (GUIDrawer.isDirty)
		{
			GUIDrawer.isDirty = false;
			GUIDrawer.drawers.RemoveWhere((IOnGUI o) => o == null);
			GUIDrawer.runList.Clear();
			if (GUIDrawer.runList.Capacity < GUIDrawer.drawers.Count)
			{
				GUIDrawer.runList.Capacity = GUIDrawer.drawers.Count;
			}
			using (HashSet<IOnGUI>.Enumerator enumerator = GUIDrawer.drawers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					IOnGUI item = enumerator.Current;
					GUIDrawer.InsertInOrder(GUIDrawer.runList, item);
				}
				goto IL_D1;
			}
		}
		GUIDrawer.runList.RemoveAll((IOnGUI o) => o == null);
		IL_D1:
		foreach (IOnGUI onGUI in GUIDrawer.runList)
		{
			onGUI.DrawGUI();
		}
		if (GUIDrawer.runList.Count == 0)
		{
			GUIDrawer.ToggleDrawer(false);
		}
	}

	// Token: 0x060043A5 RID: 17317 RVA: 0x00129650 File Offset: 0x00127850
	public static void AddDrawer(IOnGUI drawer)
	{
		if (!GUIDrawer.hasInstance)
		{
			new GameObject("GUI Drawer", new Type[]
			{
				typeof(GUIDrawer)
			});
		}
		if (GUIDrawer.drawers.Add(drawer))
		{
			GUIDrawer.isDirty = true;
			if (GUIDrawer.hasInstance)
			{
				GUIDrawer.ToggleDrawer(true);
			}
		}
	}

	// Token: 0x060043A6 RID: 17318 RVA: 0x001296A2 File Offset: 0x001278A2
	public static void RemoveDrawer(IOnGUI drawer)
	{
		if (GUIDrawer.drawers.Remove(drawer))
		{
			GUIDrawer.isDirty = true;
			if (GUIDrawer.drawers.Count == 0)
			{
				GUIDrawer.ToggleDrawer(false);
			}
		}
	}

	// Token: 0x060043A7 RID: 17319 RVA: 0x001296C9 File Offset: 0x001278C9
	private static void ToggleDrawer(bool enabled)
	{
		if (GUIDrawer.hasInstance)
		{
			GUIDrawer.instance.enabled = enabled;
		}
	}

	// Token: 0x04004522 RID: 17698
	private static bool hasInstance;

	// Token: 0x04004523 RID: 17699
	private static GUIDrawer instance;

	// Token: 0x04004524 RID: 17700
	private static bool isDirty;

	// Token: 0x04004525 RID: 17701
	private static List<IOnGUI> runList = new List<IOnGUI>();

	// Token: 0x04004526 RID: 17702
	private static HashSet<IOnGUI> drawers = new HashSet<IOnGUI>();

	// Token: 0x04004527 RID: 17703
	private static Comparer<IOnGUI> comparer = Comparer<IOnGUI>.Create((IOnGUI a, IOnGUI b) => a.GUIDepth.CompareTo(b.GUIDepth));
}
