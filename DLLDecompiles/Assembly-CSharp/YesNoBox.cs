using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000641 RID: 1601
public abstract class YesNoBox : MonoBehaviour
{
	// Token: 0x17000687 RID: 1671
	// (get) Token: 0x06003974 RID: 14708 RVA: 0x000FC97B File Offset: 0x000FAB7B
	protected virtual string InactiveYesText
	{
		get
		{
			return string.Empty;
		}
	}

	// Token: 0x17000688 RID: 1672
	// (get) Token: 0x06003975 RID: 14709 RVA: 0x000FC982 File Offset: 0x000FAB82
	protected virtual bool ShouldHideHud
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06003976 RID: 14710 RVA: 0x000FC985 File Offset: 0x000FAB85
	protected virtual void Awake()
	{
		this.pane.PaneClosedAnimEnd += delegate()
		{
			object obj = this.selectedState ? this.currentYes : this.currentNo;
			this.Clear();
			if (this.currentReturnHud)
			{
				this.hudFSM.SendEventSafe("IN");
			}
			object obj2 = obj;
			if (obj2 == null)
			{
				return;
			}
			obj2();
		};
		if (this.yesButton)
		{
			this.yesButton.InactiveConditionText = (() => this.InactiveYesText);
		}
	}

	// Token: 0x06003977 RID: 14711 RVA: 0x000FC9C2 File Offset: 0x000FABC2
	public void SelectYes()
	{
		if (!string.IsNullOrEmpty(this.InactiveYesText))
		{
			return;
		}
		this.selectedState = true;
		this.DoEnd();
	}

	// Token: 0x06003978 RID: 14712 RVA: 0x000FC9DF File Offset: 0x000FABDF
	public void SelectNo()
	{
		this.selectedState = false;
		this.DoEnd();
	}

	// Token: 0x06003979 RID: 14713 RVA: 0x000FC9EE File Offset: 0x000FABEE
	protected void DoEnd()
	{
		if (this.uiList)
		{
			this.uiList.SetActive(false);
		}
		this.pane.PaneEnd();
	}

	// Token: 0x0600397A RID: 14714 RVA: 0x000FCA14 File Offset: 0x000FAC14
	protected void OnAppearing()
	{
		if (this.uiList)
		{
			this.uiList.SetActive(false);
		}
	}

	// Token: 0x0600397B RID: 14715 RVA: 0x000FCA2F File Offset: 0x000FAC2F
	protected void OnAppeared()
	{
		if (this.uiList)
		{
			this.uiList.SetActive(true);
		}
	}

	// Token: 0x0600397C RID: 14716 RVA: 0x000FCA4A File Offset: 0x000FAC4A
	private void Clear()
	{
		this.currentYes = null;
		this.currentNo = null;
	}

	// Token: 0x0600397D RID: 14717 RVA: 0x000FCA5C File Offset: 0x000FAC5C
	protected void InternalOpen(Action yes, Action no, bool returnHud)
	{
		this.currentYes = yes;
		this.currentNo = no;
		this.currentReturnHud = returnHud;
		if (this.ShouldHideHud)
		{
			this.hudFSM.SendEventSafe("OUT");
		}
		DialogueBox.EndConversation(false, new Action(this.DoOpen));
	}

	// Token: 0x0600397E RID: 14718 RVA: 0x000FCAA8 File Offset: 0x000FACA8
	private void DoOpen()
	{
		this.pane.PaneStart();
		if (this.refreshLayoutGroup)
		{
			this.refreshLayoutGroup.ForceUpdateLayoutNoCanvas();
		}
	}

	// Token: 0x04003C36 RID: 15414
	[SerializeField]
	protected InventoryPaneStandalone pane;

	// Token: 0x04003C37 RID: 15415
	[SerializeField]
	private PlayMakerFSM hudFSM;

	// Token: 0x04003C38 RID: 15416
	[SerializeField]
	private UISelectionList uiList;

	// Token: 0x04003C39 RID: 15417
	[Space]
	[SerializeField]
	private UISelectionListItem yesButton;

	// Token: 0x04003C3A RID: 15418
	[Space]
	[SerializeField]
	private LayoutGroup refreshLayoutGroup;

	// Token: 0x04003C3B RID: 15419
	private Action currentYes;

	// Token: 0x04003C3C RID: 15420
	private Action currentNo;

	// Token: 0x04003C3D RID: 15421
	private bool currentReturnHud;

	// Token: 0x04003C3E RID: 15422
	private bool selectedState;
}
