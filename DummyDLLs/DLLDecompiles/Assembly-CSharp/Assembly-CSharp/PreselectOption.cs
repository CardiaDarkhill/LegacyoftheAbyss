using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020006F8 RID: 1784
public class PreselectOption : MonoBehaviour
{
	// Token: 0x06003FDF RID: 16351 RVA: 0x00119A60 File Offset: 0x00117C60
	public void HighlightDefault(bool deselect = false)
	{
		EventSystem current = EventSystem.current;
		if (deselect || current.currentSelectedGameObject == null || !current.currentSelectedGameObject.activeInHierarchy)
		{
			UIManager.HighlightSelectableNoSound(this.itemToHighlight);
			foreach (object obj in this.itemToHighlight.transform)
			{
				Animator component = ((Transform)obj).GetComponent<Animator>();
				if (component != null)
				{
					component.ResetTrigger("hide");
					component.SetTrigger("show");
					break;
				}
			}
		}
	}

	// Token: 0x06003FE0 RID: 16352 RVA: 0x00119B0C File Offset: 0x00117D0C
	public void SetDefaultHighlight(Button button)
	{
		this.itemToHighlight = button;
	}

	// Token: 0x06003FE1 RID: 16353 RVA: 0x00119B15 File Offset: 0x00117D15
	public void DeselectAll()
	{
		base.StartCoroutine(this.ForceDeselect());
	}

	// Token: 0x06003FE2 RID: 16354 RVA: 0x00119B24 File Offset: 0x00117D24
	private IEnumerator ForceDeselect()
	{
		yield return new WaitForSeconds(0.165f);
		UIManager.instance.eventSystem.SetSelectedGameObject(null);
		yield break;
	}

	// Token: 0x0400418C RID: 16780
	public Selectable itemToHighlight;
}
