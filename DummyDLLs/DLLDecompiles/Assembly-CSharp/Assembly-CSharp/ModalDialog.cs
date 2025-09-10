using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020006F0 RID: 1776
public class ModalDialog : MonoBehaviour
{
	// Token: 0x1700074A RID: 1866
	// (get) Token: 0x06003FB9 RID: 16313 RVA: 0x00119145 File Offset: 0x00117345
	public CanvasGroup modalWindow
	{
		get
		{
			return base.GetComponent<CanvasGroup>();
		}
	}

	// Token: 0x06003FBA RID: 16314 RVA: 0x00119150 File Offset: 0x00117350
	public void HighlightDefault()
	{
		if (this.defaultHighlight != null)
		{
			this.defaultHighlight.Select();
			using (IEnumerator enumerator = this.defaultHighlight.transform.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					Animator component = ((Transform)obj).GetComponent<Animator>();
					if (component != null)
					{
						component.ResetTrigger("hide");
						component.SetTrigger("show");
						break;
					}
				}
				return;
			}
		}
		Debug.LogError("No default highlight item defined.");
	}

	// Token: 0x0400415E RID: 16734
	public CanvasGroup content;

	// Token: 0x0400415F RID: 16735
	public Selectable defaultHighlight;
}
