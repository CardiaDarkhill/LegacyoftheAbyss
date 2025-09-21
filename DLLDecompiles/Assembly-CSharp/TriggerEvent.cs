using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000580 RID: 1408
public class TriggerEvent : MonoBehaviour
{
	// Token: 0x17000566 RID: 1382
	// (get) Token: 0x06003272 RID: 12914 RVA: 0x000E08E0 File Offset: 0x000DEAE0
	public bool IsActive
	{
		get
		{
			Renderer[] array = this.activeRenderers;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].isVisible)
				{
					return false;
				}
			}
			return this.activeZWidth < Mathf.Epsilon || Mathf.Abs(base.transform.position.z) <= this.activeZWidth;
		}
	}

	// Token: 0x06003273 RID: 12915 RVA: 0x000E093D File Offset: 0x000DEB3D
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!this.IsActive)
		{
			return;
		}
		this.TriggerEntered.Invoke();
	}

	// Token: 0x06003274 RID: 12916 RVA: 0x000E0953 File Offset: 0x000DEB53
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!this.IsActive)
		{
			return;
		}
		this.TriggerExited.Invoke();
	}

	// Token: 0x04003630 RID: 13872
	public UnityEvent TriggerEntered;

	// Token: 0x04003631 RID: 13873
	public UnityEvent TriggerExited;

	// Token: 0x04003632 RID: 13874
	[SerializeField]
	[Tooltip("If not 0, will only work if z position is within +- this value.")]
	private float activeZWidth = 0.5f;

	// Token: 0x04003633 RID: 13875
	[SerializeField]
	[Tooltip("If set, will only work when assigned renderers are all visible.")]
	private Renderer[] activeRenderers;
}
