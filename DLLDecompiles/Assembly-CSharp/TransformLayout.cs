using System;
using UnityEngine;

// Token: 0x0200073A RID: 1850
public abstract class TransformLayout : MonoBehaviour
{
	// Token: 0x06004208 RID: 16904 RVA: 0x00122706 File Offset: 0x00120906
	private void OnDestroy()
	{
	}

	// Token: 0x06004209 RID: 16905 RVA: 0x00122708 File Offset: 0x00120908
	private void OnTransformChildrenChanged()
	{
		this.UpdatePositions();
		this.UpdateListeners();
	}

	// Token: 0x0600420A RID: 16906 RVA: 0x00122716 File Offset: 0x00120916
	private void OnEnable()
	{
		this.UpdateListeners();
		this.UpdatePositions();
	}

	// Token: 0x0600420B RID: 16907 RVA: 0x00122724 File Offset: 0x00120924
	private void OnDisable()
	{
		this.UpdateListeners(false);
	}

	// Token: 0x0600420C RID: 16908 RVA: 0x0012272D File Offset: 0x0012092D
	private void UpdateListeners()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		this.UpdateListeners(this.trackChildState);
	}

	// Token: 0x0600420D RID: 16909 RVA: 0x00122744 File Offset: 0x00120944
	private void UpdateListeners(bool trackChildState)
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			UnityMessageListener listener = transform.GetComponent<UnityMessageListener>();
			if (!listener && trackChildState)
			{
				listener = transform.gameObject.AddComponent<UnityMessageListener>();
				listener.ExecuteInEditMode = true;
				listener.TransformParentChanged += delegate()
				{
					if (listener.transform.parent != this.transform)
					{
						this.DestroySafe(listener);
					}
				};
			}
			else
			{
				if (listener && !trackChildState)
				{
					this.DestroySafe(listener);
					continue;
				}
				if (!listener)
				{
					continue;
				}
			}
			listener.Enabled -= this.UpdatePositions;
			listener.Disabled -= this.UpdatePositions;
			listener.Enabled += this.UpdatePositions;
			listener.Disabled += this.UpdatePositions;
		}
	}

	// Token: 0x0600420E RID: 16910 RVA: 0x00122894 File Offset: 0x00120A94
	private void DestroySafe(Object obj)
	{
		if (Application.isPlaying)
		{
			Object.Destroy(obj);
			return;
		}
		Object.DestroyImmediate(obj);
	}

	// Token: 0x0600420F RID: 16911
	public abstract void UpdatePositions();

	// Token: 0x0400438C RID: 17292
	[SerializeField]
	private bool trackChildState;
}
