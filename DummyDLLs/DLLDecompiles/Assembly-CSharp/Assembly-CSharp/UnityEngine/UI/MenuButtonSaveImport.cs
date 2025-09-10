using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x02000870 RID: 2160
	public sealed class MenuButtonSaveImport : MenuButton, ISubmitHandler, IEventSystemHandler, IPointerClickHandler
	{
		// Token: 0x170008E9 RID: 2281
		// (get) Token: 0x06004AF7 RID: 19191 RVA: 0x00162E44 File Offset: 0x00161044
		private string ButtonLabel
		{
			get
			{
				if (Platform.Current)
				{
					return Platform.Current.SaveImportLabel;
				}
				return string.Empty;
			}
		}

		// Token: 0x06004AF8 RID: 19192 RVA: 0x00162E62 File Offset: 0x00161062
		protected override void Awake()
		{
			base.Awake();
			this.textAligner = base.GetComponent<FixVerticalAlign>();
			if (this.textAligner)
			{
				this.hasTextAligner = true;
			}
		}

		// Token: 0x06004AF9 RID: 19193 RVA: 0x00162E8C File Offset: 0x0016108C
		protected override void OnEnable()
		{
			base.OnEnable();
			this.gm = GameManager.instance;
			if (this.gm)
			{
				this.gm.RefreshLanguageText += this.RefreshTextFromLocalization;
			}
			this.uiManager = UIManager.instance;
			this.RefreshTextFromLocalization();
		}

		// Token: 0x06004AFA RID: 19194 RVA: 0x00162EDF File Offset: 0x001610DF
		protected override void OnDisable()
		{
			base.OnDisable();
			if (this.gm != null)
			{
				this.gm.RefreshLanguageText -= this.RefreshTextFromLocalization;
			}
			this.showingConfirm = false;
			this.isCancelling = false;
		}

		// Token: 0x06004AFB RID: 19195 RVA: 0x00162F1A File Offset: 0x0016111A
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (this.detachedImportPrompt && this.importPromptGroup)
			{
				Object.Destroy(this.importPromptGroup.gameObject);
			}
		}

		// Token: 0x06004AFC RID: 19196 RVA: 0x00162F47 File Offset: 0x00161147
		public void RefreshTextFromLocalization()
		{
			this.textField.text = this.ButtonLabel;
			if (this.hasTextAligner)
			{
				this.textAligner.AlignText();
			}
		}

		// Token: 0x06004AFD RID: 19197 RVA: 0x00162F6D File Offset: 0x0016116D
		public new void OnSubmit(BaseEventData eventData)
		{
			if (base.interactable)
			{
				base.OnSubmit(eventData);
				this.ShowConfirmPrompt();
			}
		}

		// Token: 0x06004AFE RID: 19198 RVA: 0x00162F84 File Offset: 0x00161184
		public new void OnPointerClick(PointerEventData eventData)
		{
			if (base.interactable)
			{
				base.OnPointerClick(eventData);
				this.ShowConfirmPrompt();
			}
		}

		// Token: 0x06004AFF RID: 19199 RVA: 0x00162F9C File Offset: 0x0016119C
		public void ShowConfirmPrompt()
		{
			if (this.showingConfirm)
			{
				return;
			}
			this.showingConfirm = true;
			if (this.currentRoutine != null)
			{
				this.uiManager.StopCoroutine(this.currentRoutine);
			}
			this.currentRoutine = this.uiManager.StartCoroutine(this.ShowConfirmRoutine());
		}

		// Token: 0x06004B00 RID: 19200 RVA: 0x00162FE9 File Offset: 0x001611E9
		private IEnumerator ShowConfirmRoutine()
		{
			if (this.importPromptGroup == null || this.importPreselectOption == null || this.menuParent == null)
			{
				this.ConfirmPrompt();
				yield break;
			}
			if (this.parentGroup != null)
			{
				this.parentGroup.interactable = false;
				if (this.parentGroupCoroutine != null)
				{
					this.uiManager.StopCoroutine(this.parentGroupCoroutine);
				}
				this.parentGroupCoroutine = this.uiManager.StartCoroutine(this.uiManager.FadeOutCanvasGroup(this.parentGroup, true, false));
			}
			if (!this.detachedImportPrompt)
			{
				this.importPromptGroup.transform.SetParent(this.menuParent.parent);
			}
			if (this.importGroupCoroutine != null)
			{
				this.uiManager.StopCoroutine(this.importGroupCoroutine);
			}
			this.importGroupCoroutine = null;
			yield return this.uiManager.FadeInCanvasGroup(this.importPromptGroup, false);
			this.importPromptGroup.interactable = true;
			this.importPreselectOption.HighlightDefault(false);
			yield break;
		}

		// Token: 0x06004B01 RID: 19201 RVA: 0x00162FF8 File Offset: 0x001611F8
		public void ConfirmPrompt()
		{
			if (this.currentRoutine != null)
			{
				this.uiManager.StopCoroutine(this.currentRoutine);
			}
			this.currentRoutine = this.uiManager.StartCoroutine(this.ConfirmPromptRoutine());
		}

		// Token: 0x06004B02 RID: 19202 RVA: 0x0016302A File Offset: 0x0016122A
		private IEnumerator ConfirmPromptRoutine()
		{
			this.DoImport();
			if (this.importPromptGroup)
			{
				this.importPromptGroup.interactable = false;
				if (this.importGroupCoroutine != null)
				{
					this.uiManager.StopCoroutine(this.importGroupCoroutine);
				}
				this.importGroupCoroutine = this.uiManager.StartCoroutine(this.uiManager.FadeOutCanvasGroup(this.importPromptGroup, true, false));
			}
			this.showingConfirm = false;
			if (this.parentGroup != null)
			{
				if (this.parentGroupCoroutine != null)
				{
					this.uiManager.StopCoroutine(this.parentGroupCoroutine);
				}
				this.parentGroupCoroutine = null;
				yield return this.uiManager.FadeInCanvasGroup(this.parentGroup, false);
				this.parentGroup.interactable = true;
			}
			this.SelectGameObject();
			yield break;
		}

		// Token: 0x06004B03 RID: 19203 RVA: 0x0016303C File Offset: 0x0016123C
		public void CancelConfirmPrompt()
		{
			if (this.isCancelling)
			{
				return;
			}
			this.isCancelling = true;
			if (this.currentRoutine != null)
			{
				this.uiManager.StopCoroutine(this.currentRoutine);
			}
			this.currentRoutine = this.uiManager.StartCoroutine(this.CancelConfirmRoutine());
		}

		// Token: 0x06004B04 RID: 19204 RVA: 0x00163089 File Offset: 0x00161289
		private IEnumerator CancelConfirmRoutine()
		{
			if (this.importPromptGroup)
			{
				this.importPromptGroup.interactable = false;
				if (this.importGroupCoroutine != null)
				{
					this.uiManager.StopCoroutine(this.importGroupCoroutine);
				}
				this.importGroupCoroutine = this.uiManager.StartCoroutine(this.uiManager.FadeOutCanvasGroup(this.importPromptGroup, true, false));
			}
			if (this.parentGroup != null)
			{
				if (this.parentGroupCoroutine != null)
				{
					this.uiManager.StopCoroutine(this.parentGroupCoroutine);
				}
				this.parentGroupCoroutine = null;
				yield return this.uiManager.FadeInCanvasGroup(this.parentGroup, false);
				this.parentGroup.interactable = true;
			}
			this.showingConfirm = false;
			this.isCancelling = false;
			this.SelectGameObject();
			yield break;
		}

		// Token: 0x06004B05 RID: 19205 RVA: 0x00163098 File Offset: 0x00161298
		private void SelectGameObject()
		{
			EventSystem.current.SetSelectedGameObject(base.gameObject);
		}

		// Token: 0x06004B06 RID: 19206 RVA: 0x001630AC File Offset: 0x001612AC
		private void DoImport()
		{
			if (!Platform.Current.ShowSaveDataImport)
			{
				Debug.LogError(string.Format("{0} does not support save data import.", Platform.Current));
				this.showingConfirm = false;
				return;
			}
			this.DisplayImportInProgress();
			Platform.Current.FetchImportData(delegate(List<Platform.ImportDataInfo> importDataInfos)
			{
				if (importDataInfos == null)
				{
					this.HideImportInProgress();
					this.DisplayNoImportDataPrompt();
					return;
				}
				GameManager.instance.StartCoroutine(this.ProcessImportData(importDataInfos));
			});
		}

		// Token: 0x06004B07 RID: 19207 RVA: 0x001630FD File Offset: 0x001612FD
		private IEnumerator ProcessImportData(List<Platform.ImportDataInfo> importDataInfos)
		{
			int currentSlot = 1;
			int importedCount = 0;
			int importTotal = importDataInfos.Count;
			int num;
			for (int i = 0; i < importDataInfos.Count; i = num + 1)
			{
				MenuButtonSaveImport.<>c__DisplayClass32_0 CS$<>8__locals1 = new MenuButtonSaveImport.<>c__DisplayClass32_0();
				CS$<>8__locals1.wait = true;
				CS$<>8__locals1.isUsed = true;
				Platform.ImportDataInfo importDataInfo = importDataInfos[i];
				if (!importDataInfo.isSaveSlot)
				{
					importDataInfo.Save(0, delegate(bool success)
					{
						CS$<>8__locals1.wait = false;
					});
					yield return new WaitWhile(() => CS$<>8__locals1.wait);
					num = importTotal;
					importTotal = num - 1;
				}
				else
				{
					while (currentSlot <= 4)
					{
						CS$<>8__locals1.wait = true;
						Platform.Current.IsSaveSlotInUse(currentSlot, delegate(bool inUse)
						{
							CS$<>8__locals1.isUsed = inUse;
							CS$<>8__locals1.wait = false;
						});
						yield return new WaitWhile(() => CS$<>8__locals1.wait);
						if (!CS$<>8__locals1.isUsed)
						{
							break;
						}
						num = currentSlot;
						currentSlot = num + 1;
					}
					if (currentSlot <= 4)
					{
						CS$<>8__locals1.wait = true;
						importDataInfo.Save(currentSlot, delegate(bool success)
						{
							CS$<>8__locals1.wait = false;
						});
						yield return new WaitWhile(() => CS$<>8__locals1.wait);
						num = currentSlot;
						currentSlot = num + 1;
						num = importedCount;
						importedCount = num + 1;
						CS$<>8__locals1 = null;
						importDataInfo = null;
					}
				}
				num = i;
			}
			bool wait = true;
			bool success2 = importedCount == importTotal;
			Platform.Current.DisplayImportDataResultMessage(new Platform.ImportDataResult
			{
				importedCount = importedCount,
				importTotal = importTotal,
				success = success2
			}, delegate
			{
				wait = false;
			});
			yield return new WaitWhile(() => wait);
			this.uiManager.ReloadSaves();
			this.HideImportInProgress();
			yield break;
		}

		// Token: 0x06004B08 RID: 19208 RVA: 0x00163113 File Offset: 0x00161313
		private void DisplayImportInProgress()
		{
		}

		// Token: 0x06004B09 RID: 19209 RVA: 0x00163115 File Offset: 0x00161315
		private void HideImportInProgress()
		{
			Platform.Current.CloseSystemDialogs(null);
		}

		// Token: 0x06004B0A RID: 19210 RVA: 0x00163122 File Offset: 0x00161322
		private void DisplayNoImportDataPrompt()
		{
		}

		// Token: 0x04004C9D RID: 19613
		[Header("Save Import Button")]
		[SerializeField]
		private Text textField;

		// Token: 0x04004C9E RID: 19614
		[SerializeField]
		private CanvasGroup parentGroup;

		// Token: 0x04004C9F RID: 19615
		[SerializeField]
		private CanvasGroup importPromptGroup;

		// Token: 0x04004CA0 RID: 19616
		[SerializeField]
		private PreselectOption importPreselectOption;

		// Token: 0x04004CA1 RID: 19617
		[SerializeField]
		private Transform menuParent;

		// Token: 0x04004CA2 RID: 19618
		private bool detachedImportPrompt;

		// Token: 0x04004CA3 RID: 19619
		private GameManager gm;

		// Token: 0x04004CA4 RID: 19620
		private UIManager uiManager;

		// Token: 0x04004CA5 RID: 19621
		private FixVerticalAlign textAligner;

		// Token: 0x04004CA6 RID: 19622
		private bool hasTextAligner;

		// Token: 0x04004CA7 RID: 19623
		private bool showingConfirm;

		// Token: 0x04004CA8 RID: 19624
		private bool isCancelling;

		// Token: 0x04004CA9 RID: 19625
		private Coroutine parentGroupCoroutine;

		// Token: 0x04004CAA RID: 19626
		private Coroutine importGroupCoroutine;

		// Token: 0x04004CAB RID: 19627
		private Coroutine currentRoutine;
	}
}
