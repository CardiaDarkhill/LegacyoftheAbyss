using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000525 RID: 1317
public class NonSlider : MonoBehaviour
{
	// Token: 0x17000541 RID: 1345
	// (get) Token: 0x06002F5A RID: 12122 RVA: 0x000D0C5F File Offset: 0x000CEE5F
	// (set) Token: 0x06002F5B RID: 12123 RVA: 0x000D0C67 File Offset: 0x000CEE67
	public bool IsActive
	{
		get
		{
			return this.isActive;
		}
		set
		{
			this.isActive = value;
		}
	}

	// Token: 0x06002F5C RID: 12124 RVA: 0x000D0C70 File Offset: 0x000CEE70
	private void Awake()
	{
		List<NonSlider> list;
		if (!NonSlider.NON_SLIDERS.TryGetValue(base.gameObject, out list))
		{
			list = new List<NonSlider>();
			NonSlider.NON_SLIDERS.Add(base.gameObject, list);
		}
		list.Add(this);
	}

	// Token: 0x06002F5D RID: 12125 RVA: 0x000D0CB0 File Offset: 0x000CEEB0
	private void OnDestroy()
	{
		List<NonSlider> list;
		if (NonSlider.NON_SLIDERS.TryGetValue(base.gameObject, out list) && list.Remove(this) && list.Count == 0)
		{
			NonSlider.NON_SLIDERS.Remove(base.gameObject);
		}
	}

	// Token: 0x06002F5E RID: 12126 RVA: 0x000D0CF3 File Offset: 0x000CEEF3
	public static bool IsNonSlider(Collider2D collider2D)
	{
		return NonSlider.NON_SLIDERS.ContainsKey(collider2D.gameObject);
	}

	// Token: 0x06002F5F RID: 12127 RVA: 0x000D0D05 File Offset: 0x000CEF05
	public static bool IsNonSlider(GameObject gameObject)
	{
		return NonSlider.NON_SLIDERS.ContainsKey(gameObject);
	}

	// Token: 0x06002F60 RID: 12128 RVA: 0x000D0D12 File Offset: 0x000CEF12
	public static bool TryGetNonSlider(Collider2D collider2D, out NonSlider nonSlider)
	{
		return NonSlider.TryGetNonSlider(collider2D.gameObject, out nonSlider);
	}

	// Token: 0x06002F61 RID: 12129 RVA: 0x000D0D20 File Offset: 0x000CEF20
	public static bool TryGetNonSlider(GameObject gameObject, out NonSlider nonSlider)
	{
		List<NonSlider> list;
		if (NonSlider.NON_SLIDERS.TryGetValue(gameObject, out list) && list.Count > 0)
		{
			nonSlider = list[0];
			return true;
		}
		nonSlider = null;
		return false;
	}

	// Token: 0x0400321F RID: 12831
	[SerializeField]
	private bool isActive = true;

	// Token: 0x04003220 RID: 12832
	private static readonly Dictionary<GameObject, List<NonSlider>> NON_SLIDERS = new Dictionary<GameObject, List<NonSlider>>();
}
