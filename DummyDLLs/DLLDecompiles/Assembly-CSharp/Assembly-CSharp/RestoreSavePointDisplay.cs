using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000708 RID: 1800
public sealed class RestoreSavePointDisplay : MonoBehaviour
{
	// Token: 0x06004038 RID: 16440 RVA: 0x0011AD26 File Offset: 0x00118F26
	private void Start()
	{
		this.Init();
	}

	// Token: 0x06004039 RID: 16441 RVA: 0x0011AD2E File Offset: 0x00118F2E
	private void OnEnable()
	{
		this.eventSystem = EventSystem.current;
	}

	// Token: 0x0600403A RID: 16442 RVA: 0x0011AD3B File Offset: 0x00118F3B
	private void OnDisable()
	{
		this.canvasGroup.interactable = false;
		this.canvasGroup.blocksRaycasts = false;
		this.RestoreBackButtonNavigation();
	}

	// Token: 0x0600403B RID: 16443 RVA: 0x0011AD5C File Offset: 0x00118F5C
	private void Update()
	{
		if (this.isActive && this.ih.acceptingInput)
		{
			if (this.eventSystem.currentSelectedGameObject == null && this.lastSelection != null)
			{
				this.eventSystem.SetSelectedGameObject(this.lastSelection.gameObject);
			}
			if (this.ih.inputActions.MenuCancel.WasPressed)
			{
				this.Cancel();
			}
		}
	}

	// Token: 0x0600403C RID: 16444 RVA: 0x0011ADD2 File Offset: 0x00118FD2
	private void LateUpdate()
	{
		if (this.isActive)
		{
			this.selector.Update();
		}
	}

	// Token: 0x0600403D RID: 16445 RVA: 0x0011ADE8 File Offset: 0x00118FE8
	public void Init()
	{
		if (!this.init)
		{
			this.init = true;
			this.ih = GameManager.instance.inputHandler;
			this.ui = UIManager.instance;
			this.buttons.RemoveAll((RestoreSaveSelectionButton o) => o == null);
			RestoreSaveSelectionButton restoreSaveSelectionButton = null;
			for (int i = 0; i < this.buttons.Count; i++)
			{
				RestoreSaveSelectionButton restoreSaveSelectionButton2 = this.buttons[i];
				restoreSaveSelectionButton2.transform.SetSiblingIndex(i);
				restoreSaveSelectionButton2.gameObject.SetActive(false);
				restoreSaveSelectionButton2.SetRestoreParent(this);
				if (i > 0)
				{
					Navigation navigation = restoreSaveSelectionButton.navigation;
					navigation.selectOnDown = restoreSaveSelectionButton2;
					restoreSaveSelectionButton.navigation = navigation;
					Navigation navigation2 = restoreSaveSelectionButton2.navigation;
					navigation2.selectOnUp = restoreSaveSelectionButton;
					restoreSaveSelectionButton2.navigation = navigation2;
				}
				restoreSaveSelectionButton = restoreSaveSelectionButton2;
			}
		}
	}

	// Token: 0x0600403E RID: 16446 RVA: 0x0011AEC8 File Offset: 0x001190C8
	private RestoreSaveSelectionButton CreateButton()
	{
		if (!this.template)
		{
			Debug.LogError(string.Format("{0} is missing button template", this), this);
			return null;
		}
		RestoreSaveSelectionButton restoreSaveSelectionButton = Object.Instantiate<RestoreSaveSelectionButton>(this.template);
		restoreSaveSelectionButton.transform.SetParent(this.scrollContent, false);
		restoreSaveSelectionButton.SetRestoreParent(this);
		return restoreSaveSelectionButton;
	}

	// Token: 0x0600403F RID: 16447 RVA: 0x0011AF1C File Offset: 0x0011911C
	private void CreateMore(int count)
	{
		if (count <= this.buttons.Count)
		{
			return;
		}
		count -= this.buttons.Count;
		for (int i = 0; i < count; i++)
		{
			RestoreSaveSelectionButton restoreSaveSelectionButton = this.CreateButton();
			if (!restoreSaveSelectionButton)
			{
				Debug.LogError("Failed to create buttons", this);
				return;
			}
			restoreSaveSelectionButton.name = restoreSaveSelectionButton.name + " (" + (i + 2).ToString() + ")";
			this.buttons.Add(restoreSaveSelectionButton);
		}
	}

	// Token: 0x06004040 RID: 16448 RVA: 0x0011AFA2 File Offset: 0x001191A2
	public void SetHidden()
	{
		this.canvasGroup.alpha = 0f;
		this.canvasGroup.interactable = false;
		this.canvasGroup.blocksRaycasts = false;
		this.selector.Hide();
		this.HideLoadIconInstant();
	}

	// Token: 0x06004041 RID: 16449 RVA: 0x0011AFDD File Offset: 0x001191DD
	public void ToggleLoadIcon(bool show)
	{
		if (this.loadIcon)
		{
			if (show)
			{
				base.gameObject.SetActive(true);
				this.loadIcon.StartFadeIn();
				return;
			}
			this.loadIcon.StartFadeOut();
		}
	}

	// Token: 0x06004042 RID: 16450 RVA: 0x0011B012 File Offset: 0x00119212
	private void HideLoadIconInstant()
	{
		if (this.loadIcon)
		{
			this.loadIcon.HideInstant();
		}
	}

	// Token: 0x06004043 RID: 16451 RVA: 0x0011B02C File Offset: 0x0011922C
	public void SetRestorePoints(List<RestorePointData> restorePoints)
	{
		if (restorePoints == null || restorePoints.Count == 0)
		{
			this.ClearRestorePoints();
			return;
		}
		if (restorePoints.Count > this.buttons.Count)
		{
			this.CreateMore(restorePoints.Count);
		}
		this.buttonCount = restorePoints.Count;
		for (int i = 0; i < this.buttons.Count; i++)
		{
			RestoreSaveSelectionButton restoreSaveSelectionButton = this.buttons[i];
			if (i < restorePoints.Count)
			{
				RestorePointData restorePoint = restorePoints[i];
				restoreSaveSelectionButton.SetRestorePoint(restorePoint);
				restoreSaveSelectionButton.gameObject.SetActive(true);
				this.lastActive = i;
			}
			else
			{
				restoreSaveSelectionButton.ClearRestorePoint();
				restoreSaveSelectionButton.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06004044 RID: 16452 RVA: 0x0011B0D8 File Offset: 0x001192D8
	public void ClearRestorePoints()
	{
		this.buttonCount = 0;
		for (int i = 0; i < this.buttons.Count; i++)
		{
			RestoreSaveSelectionButton restoreSaveSelectionButton = this.buttons[i];
			restoreSaveSelectionButton.ClearRestorePoint();
			restoreSaveSelectionButton.gameObject.SetActive(false);
		}
	}

	// Token: 0x06004045 RID: 16453 RVA: 0x0011B11F File Offset: 0x0011931F
	public bool IsEmpty()
	{
		return this.buttonCount == 0;
	}

	// Token: 0x06004046 RID: 16454 RVA: 0x0011B12C File Offset: 0x0011932C
	private Selectable GetFirstSelectable()
	{
		if (this.buttons.Count > 0)
		{
			RestoreSaveSelectionButton restoreSaveSelectionButton = this.buttons[0];
			if (restoreSaveSelectionButton.ButtonIsActive)
			{
				return restoreSaveSelectionButton;
			}
		}
		return this.GetBackButton();
	}

	// Token: 0x06004047 RID: 16455 RVA: 0x0011B164 File Offset: 0x00119364
	private void Cancel()
	{
		this.isActive = false;
	}

	// Token: 0x06004048 RID: 16456 RVA: 0x0011B16D File Offset: 0x0011936D
	public void OnSelectButton(RestoreSaveSelectionButton button, bool isMouse = false)
	{
		this.scrollRectController.SetScrollTarget(button.transform, isMouse);
		this.selector.SetTargetAndShow(button.transform);
		this.lastSelection = button;
	}

	// Token: 0x06004049 RID: 16457 RVA: 0x0011B199 File Offset: 0x00119399
	public void OnDeselectButton(RestoreSaveSelectionButton button)
	{
		this.selector.Hide();
	}

	// Token: 0x0600404A RID: 16458 RVA: 0x0011B1A6 File Offset: 0x001193A6
	public void SetSelectedData(RestorePointData data)
	{
		this.isActive = false;
		this.restoreSaveButton.SaveSelected(data);
	}

	// Token: 0x0600404B RID: 16459 RVA: 0x0011B1BB File Offset: 0x001193BB
	public Selectable GetBackButton()
	{
		return this.restoreSaveButton.GetBackButton();
	}

	// Token: 0x0600404C RID: 16460 RVA: 0x0011B1C8 File Offset: 0x001193C8
	public void DoCancel()
	{
		this.restoreSaveButton.CancelSelection();
	}

	// Token: 0x0600404D RID: 16461 RVA: 0x0011B1D5 File Offset: 0x001193D5
	public IEnumerator Show()
	{
		this.ToggleLoadIcon(false);
		this.SetBackButtonNavigation();
		this.scrollRectController.ResetScroll();
		yield return this.ui.FadeInCanvasGroup(this.canvasGroup, false);
		this.HideLoadIconInstant();
		this.ih.StartUIInput();
		this.canvasGroup.blocksRaycasts = true;
		yield return null;
		this.ih.StopMouseInput();
		this.selectionHighlight.itemToHighlight = this.GetFirstSelectable();
		this.selectionHighlight.HighlightDefault(true);
		this.isActive = true;
		yield return null;
		this.ih.EnableMouseInput();
		yield break;
	}

	// Token: 0x0600404E RID: 16462 RVA: 0x0011B1E4 File Offset: 0x001193E4
	public IEnumerator Hide()
	{
		this.isActive = false;
		this.lastSelection = null;
		this.RestoreBackButtonNavigation();
		yield return this.ui.FadeOutCanvasGroup(this.canvasGroup, true, false);
		this.canvasGroup.interactable = false;
		this.canvasGroup.blocksRaycasts = false;
		yield break;
	}

	// Token: 0x0600404F RID: 16463 RVA: 0x0011B1F4 File Offset: 0x001193F4
	private void SetBackButtonNavigation()
	{
		if (!this.hasSetBackSelectable)
		{
			Selectable backButton = this.GetBackButton();
			if (backButton)
			{
				this.hasSetBackSelectable = true;
				Navigation navigation = backButton.navigation;
				this.backButtonNav = navigation;
				navigation.selectOnUp = null;
				for (int i = this.lastActive; i >= 0; i--)
				{
					RestoreSaveSelectionButton restoreSaveSelectionButton = this.buttons[i];
					if (restoreSaveSelectionButton.ButtonIsActive)
					{
						navigation.selectOnUp = restoreSaveSelectionButton;
						break;
					}
				}
				backButton.navigation = navigation;
			}
		}
	}

	// Token: 0x06004050 RID: 16464 RVA: 0x0011B26C File Offset: 0x0011946C
	private void RestoreBackButtonNavigation()
	{
		if (this.hasSetBackSelectable)
		{
			this.GetBackButton().navigation = this.backButtonNav;
			this.hasSetBackSelectable = false;
		}
	}

	// Token: 0x040041E0 RID: 16864
	[SerializeField]
	private RestoreSaveButton restoreSaveButton;

	// Token: 0x040041E1 RID: 16865
	[SerializeField]
	private CanvasGroup canvasGroup;

	// Token: 0x040041E2 RID: 16866
	[SerializeField]
	private PreselectOption selectionHighlight;

	// Token: 0x040041E3 RID: 16867
	[SerializeField]
	private VerticalScrollRectController scrollRectController;

	// Token: 0x040041E4 RID: 16868
	[Space]
	[SerializeField]
	private RestoreSaveLoadIcon loadIcon;

	// Token: 0x040041E5 RID: 16869
	[Space]
	[SerializeField]
	private RestoreSaveSelectionButton template;

	// Token: 0x040041E6 RID: 16870
	[SerializeField]
	private List<RestoreSaveSelectionButton> buttons = new List<RestoreSaveSelectionButton>();

	// Token: 0x040041E7 RID: 16871
	[SerializeField]
	private Transform scrollContent;

	// Token: 0x040041E8 RID: 16872
	[SerializeField]
	private RestoreSavePointDisplay.Selector selector;

	// Token: 0x040041E9 RID: 16873
	[Space]
	[SerializeField]
	private float scrollAmount = 130f;

	// Token: 0x040041EA RID: 16874
	private bool init;

	// Token: 0x040041EB RID: 16875
	private RestoreSaveSelectionButton lastSelection;

	// Token: 0x040041EC RID: 16876
	private RestorePointFileWrapper selectedData;

	// Token: 0x040041ED RID: 16877
	private UIManager ui;

	// Token: 0x040041EE RID: 16878
	private InputHandler ih;

	// Token: 0x040041EF RID: 16879
	private bool isActive;

	// Token: 0x040041F0 RID: 16880
	private bool hasSelector;

	// Token: 0x040041F1 RID: 16881
	private Navigation backButtonNav;

	// Token: 0x040041F2 RID: 16882
	private bool hasSetBackSelectable;

	// Token: 0x040041F3 RID: 16883
	private int lastActive = -1;

	// Token: 0x040041F4 RID: 16884
	private int buttonCount;

	// Token: 0x040041F5 RID: 16885
	private EventSystem eventSystem;

	// Token: 0x020019F6 RID: 6646
	[Serializable]
	private class Selector
	{
		// Token: 0x0600959B RID: 38299 RVA: 0x002A5E37 File Offset: 0x002A4037
		public void SetTargetAndShow(Transform target)
		{
			this.SetTarget(target);
			this.Show();
		}

		// Token: 0x0600959C RID: 38300 RVA: 0x002A5E46 File Offset: 0x002A4046
		public void SetTarget(Transform target)
		{
			this.target = target;
			this.hasTarget = target;
			if (!this.hasTarget)
			{
				this.Hide();
			}
			this.Update();
		}

		// Token: 0x0600959D RID: 38301 RVA: 0x002A5E70 File Offset: 0x002A4070
		public void Update()
		{
			if (this.hideCalled)
			{
				if (!this.waitedFrame)
				{
					this.waitedFrame = true;
					return;
				}
				this.hideCalled = false;
				this.Hide();
			}
			if (this.hasTarget)
			{
				this.selector.transform.position = this.target.transform.position;
			}
		}

		// Token: 0x0600959E RID: 38302 RVA: 0x002A5ECA File Offset: 0x002A40CA
		public void Show()
		{
			this.selector.gameObject.SetActive(true);
		}

		// Token: 0x0600959F RID: 38303 RVA: 0x002A5EDD File Offset: 0x002A40DD
		public void Hide()
		{
			this.selector.gameObject.SetActive(false);
		}

		// Token: 0x060095A0 RID: 38304 RVA: 0x002A5EF0 File Offset: 0x002A40F0
		public void DelayedHide()
		{
			this.hideCalled = true;
			this.waitedFrame = false;
		}

		// Token: 0x040097E6 RID: 38886
		[SerializeField]
		private RectTransform selector;

		// Token: 0x040097E7 RID: 38887
		private bool hasTarget;

		// Token: 0x040097E8 RID: 38888
		private Transform target;

		// Token: 0x040097E9 RID: 38889
		private bool hideCalled;

		// Token: 0x040097EA RID: 38890
		private bool waitedFrame;
	}
}
