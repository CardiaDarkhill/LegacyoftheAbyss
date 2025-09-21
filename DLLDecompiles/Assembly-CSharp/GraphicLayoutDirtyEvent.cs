using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000670 RID: 1648
[ExecuteInEditMode]
public sealed class GraphicLayoutDirtyEvent : MonoBehaviour
{
	// Token: 0x06003B23 RID: 15139 RVA: 0x00104A1A File Offset: 0x00102C1A
	private void OnEnable()
	{
		if (this.graphic != null)
		{
			this.graphic.RegisterDirtyLayoutCallback(new UnityAction(this.UpdateLayout));
			this.UpdateLayout();
		}
	}

	// Token: 0x06003B24 RID: 15140 RVA: 0x00104A47 File Offset: 0x00102C47
	private void OnDisable()
	{
		if (this.graphic != null)
		{
			this.graphic.UnregisterDirtyLayoutCallback(new UnityAction(this.UpdateLayout));
		}
	}

	// Token: 0x06003B25 RID: 15141 RVA: 0x00104A6E File Offset: 0x00102C6E
	private void UpdateLayout()
	{
		UnityEvent unityEvent = this.onLayoutDirty;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x04003D72 RID: 15730
	[SerializeField]
	private Graphic graphic;

	// Token: 0x04003D73 RID: 15731
	public UnityEvent onLayoutDirty;
}
