using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000709 RID: 1801
public class RestoreSaveSelectionButton : MenuButton, ISubmitHandler, IEventSystemHandler, IPointerClickHandler, ISelectHandler, ICancelHandler
{
	// Token: 0x17000750 RID: 1872
	// (get) Token: 0x06004052 RID: 16466 RVA: 0x0011B2B3 File Offset: 0x001194B3
	// (set) Token: 0x06004053 RID: 16467 RVA: 0x0011B2BB File Offset: 0x001194BB
	public bool ButtonIsActive { get; private set; }

	// Token: 0x06004054 RID: 16468 RVA: 0x0011B2C4 File Offset: 0x001194C4
	public new void OnSubmit(BaseEventData eventData)
	{
		if (!base.interactable)
		{
			return;
		}
		if (!this.ButtonIsActive)
		{
			return;
		}
		if (!this.isValid)
		{
			return;
		}
		base.OnSubmit(eventData);
		if (this.SubmitEventToParent())
		{
			base.ForceDeselect();
		}
	}

	// Token: 0x06004055 RID: 16469 RVA: 0x0011B2F6 File Offset: 0x001194F6
	public new void OnCancel(BaseEventData eventData)
	{
		if (!base.interactable)
		{
			return;
		}
		base.OnCancel(eventData);
		this.parentDisplay.DoCancel();
		base.PlayCancelSound();
	}

	// Token: 0x06004056 RID: 16470 RVA: 0x0011B319 File Offset: 0x00119519
	public new void OnPointerClick(PointerEventData eventData)
	{
		this.OnSubmit(eventData);
	}

	// Token: 0x06004057 RID: 16471 RVA: 0x0011B324 File Offset: 0x00119524
	public new void OnSelect(BaseEventData eventData)
	{
		if (!base.interactable)
		{
			eventData.selectedObject = this.GetBackButton();
			return;
		}
		if (!this.ButtonIsActive || !base.gameObject.activeSelf)
		{
			eventData.selectedObject = this.GetBackButton();
			return;
		}
		base.OnSelect(eventData);
		this.parentDisplay.OnSelectButton(this, eventData is PointerEventData);
		eventData.Use();
	}

	// Token: 0x06004058 RID: 16472 RVA: 0x0011B38C File Offset: 0x0011958C
	public override void OnMove(AxisEventData eventData)
	{
		if (eventData.moveDir == MoveDirection.Down && (base.navigation.selectOnDown == null || !base.navigation.selectOnDown.gameObject.activeInHierarchy))
		{
			eventData.selectedObject = this.GetBackButton();
			this.parentDisplay.OnDeselectButton(this);
			return;
		}
		base.OnMove(eventData);
	}

	// Token: 0x06004059 RID: 16473 RVA: 0x0011B3F2 File Offset: 0x001195F2
	public void SetRestorePoint(RestorePointData restorePointData)
	{
		this.restorePointData = restorePointData;
		this.ButtonIsActive = (restorePointData != null);
		this.UpdateDisplay();
	}

	// Token: 0x0600405A RID: 16474 RVA: 0x0011B40B File Offset: 0x0011960B
	public void ClearRestorePoint()
	{
		this.ButtonIsActive = false;
		this.restorePointData = null;
	}

	// Token: 0x0600405B RID: 16475 RVA: 0x0011B41C File Offset: 0x0011961C
	private void UpdateDisplay()
	{
		if (this.ButtonIsActive)
		{
			this.nameText.text = this.restorePointData.GetName();
			this.dateTimeText.text = this.restorePointData.GetDateTime();
			this.isValid = this.restorePointData.IsValid();
		}
	}

	// Token: 0x0600405C RID: 16476 RVA: 0x0011B46E File Offset: 0x0011966E
	public void PrependNumber(int number)
	{
		this.nameText.text = string.Format("{0}. {1}", number, this.nameText.text);
	}

	// Token: 0x0600405D RID: 16477 RVA: 0x0011B496 File Offset: 0x00119696
	public void SetRestoreParent(RestoreSavePointDisplay restoreSavePointDisplay)
	{
		this.parentDisplay = restoreSavePointDisplay;
	}

	// Token: 0x0600405E RID: 16478 RVA: 0x0011B4A0 File Offset: 0x001196A0
	private GameObject GetBackButton()
	{
		if (this.parentDisplay)
		{
			Selectable backButton = this.parentDisplay.GetBackButton();
			if (backButton)
			{
				return backButton.gameObject;
			}
		}
		return null;
	}

	// Token: 0x0600405F RID: 16479 RVA: 0x0011B4D6 File Offset: 0x001196D6
	private bool SubmitEventToParent()
	{
		if (this.parentDisplay)
		{
			this.parentDisplay.SetSelectedData(this.restorePointData);
			return true;
		}
		Debug.LogError(string.Format("{0} is missing parent display", this), this);
		return false;
	}

	// Token: 0x040041F6 RID: 16886
	[SerializeField]
	private Text nameText;

	// Token: 0x040041F7 RID: 16887
	[SerializeField]
	private Text dateTimeText;

	// Token: 0x040041F8 RID: 16888
	private RestorePointData restorePointData;

	// Token: 0x040041F9 RID: 16889
	private RestoreSavePointDisplay parentDisplay;

	// Token: 0x040041FB RID: 16891
	private bool isValid;
}
