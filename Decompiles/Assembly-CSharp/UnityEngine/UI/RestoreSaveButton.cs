using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	// Token: 0x0200087D RID: 2173
	public sealed class RestoreSaveButton : MenuButton, ISubmitHandler, IEventSystemHandler, IPointerClickHandler, ISelectHandler
	{
		// Token: 0x06004B9B RID: 19355 RVA: 0x00165242 File Offset: 0x00163442
		public void RestoreSavePromptYes()
		{
			this.InternalEnterState(RestoreSaveButton.RestoreState.Confirm);
		}

		// Token: 0x06004B9C RID: 19356 RVA: 0x0016524B File Offset: 0x0016344B
		public void CancelSelection()
		{
			this.saveSlotButton.ShowRelevantModeForSaveFileState();
		}

		// Token: 0x06004B9D RID: 19357 RVA: 0x00165258 File Offset: 0x00163458
		public void RestoreSavePromptNo()
		{
			this.saveSlotButton.ShowRelevantModeForSaveFileState();
		}

		// Token: 0x06004B9E RID: 19358 RVA: 0x00165265 File Offset: 0x00163465
		public void RestoreSaveConfirmYes()
		{
			this.saveSlotButton.StartCoroutine(this.RestoreConfirmed());
		}

		// Token: 0x06004B9F RID: 19359 RVA: 0x00165279 File Offset: 0x00163479
		public void RestoreSaveConfirmNo()
		{
			this.saveSlotButton.ShowRelevantModeForSaveFileState();
		}

		// Token: 0x170008F0 RID: 2288
		// (get) Token: 0x06004BA0 RID: 19360 RVA: 0x00165286 File Offset: 0x00163486
		public CanvasGroup CanvasGroup
		{
			get
			{
				return this.restoreSaveButton;
			}
		}

		// Token: 0x06004BA1 RID: 19361 RVA: 0x00165290 File Offset: 0x00163490
		protected override void Start()
		{
			base.Start();
			base.HookUpAudioPlayer();
			base.HookUpEventTrigger();
			this.ui = UIManager.instance;
			this.ih = GameManager.instance.inputHandler;
			this.restoreSavePromptHighlight = this.restoreSavePrompt.GetComponent<PreselectOption>();
			this.restoreSaveConfirmPromptHighlight = this.restoreSaveConfirmPrompt.GetComponent<PreselectOption>();
			this.restoreSaveSelection.Init();
			Navigation navigation = base.navigation;
			if (navigation.selectOnDown == null)
			{
				navigation.selectOnDown = this.saveSlotButton.backButton;
			}
			base.navigation = navigation;
			if (!this.isVisible)
			{
				this.SetHidden();
			}
		}

		// Token: 0x06004BA2 RID: 19362 RVA: 0x00165334 File Offset: 0x00163534
		protected override void OnDisable()
		{
			base.OnDisable();
			if (this.isVisible)
			{
				this.saveSlotButton.interactable = true;
				this.saveSlotButton.myCanvasGroup.interactable = true;
				this.saveSlotButton.myCanvasGroup.blocksRaycasts = true;
				this.saveSlotButton.controlsGroupBlocker.ignoreParentGroups = false;
			}
			this.isVisible = false;
		}

		// Token: 0x06004BA3 RID: 19363 RVA: 0x00165395 File Offset: 0x00163595
		public new void OnSubmit(BaseEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			base.OnSubmit(eventData);
			base.ForceDeselect();
			this.saveSlotButton.RestoreSaveMenu();
		}

		// Token: 0x06004BA4 RID: 19364 RVA: 0x001653B8 File Offset: 0x001635B8
		public new void OnPointerClick(PointerEventData eventData)
		{
			this.OnSubmit(eventData);
		}

		// Token: 0x06004BA5 RID: 19365 RVA: 0x001653C4 File Offset: 0x001635C4
		public new void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			if (this.restoreSaveButton && this.restoreSaveButton.interactable)
			{
				if (this.selectIcon)
				{
					this.selectIcon.SetBool(RestoreSaveButton._isSelectedProp, true);
					return;
				}
			}
			else
			{
				base.StartCoroutine(this.SelectAfterFrame(base.navigation.selectOnUp.gameObject));
			}
		}

		// Token: 0x06004BA6 RID: 19366 RVA: 0x00165431 File Offset: 0x00163631
		protected override void OnDeselected(BaseEventData eventData)
		{
			if (this.selectIcon)
			{
				this.selectIcon.SetBool(RestoreSaveButton._isSelectedProp, false);
			}
		}

		// Token: 0x06004BA7 RID: 19367 RVA: 0x00165451 File Offset: 0x00163651
		private IEnumerator SelectAfterFrame(GameObject obj)
		{
			yield return new WaitForEndOfFrame();
			EventSystem.current.SetSelectedGameObject(obj);
			yield break;
		}

		// Token: 0x06004BA8 RID: 19368 RVA: 0x00165460 File Offset: 0x00163660
		public Selectable GetBackButton()
		{
			return this.saveSlotButton.backButton;
		}

		// Token: 0x06004BA9 RID: 19369 RVA: 0x0016546D File Offset: 0x0016366D
		private void SetHidden()
		{
			RestoreSaveButton.SetGroupHidden(this.restoreSaveButton);
			RestoreSaveButton.SetGroupHidden(this.restoreSavePrompt);
			RestoreSaveButton.SetGroupHidden(this.restoreSaveConfirmPrompt);
			this.restoreSaveSelection.SetHidden();
		}

		// Token: 0x06004BAA RID: 19370 RVA: 0x0016549B File Offset: 0x0016369B
		private static void SetGroupHidden(CanvasGroup canvasGroup)
		{
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}

		// Token: 0x06004BAB RID: 19371 RVA: 0x001654B6 File Offset: 0x001636B6
		public IEnumerator ShowRestoreSaveButton()
		{
			base.gameObject.SetActive(true);
			yield return this.EnterState(RestoreSaveButton.RestoreState.Button);
			yield break;
		}

		// Token: 0x06004BAC RID: 19372 RVA: 0x001654C5 File Offset: 0x001636C5
		public IEnumerator ShowSaveSelection()
		{
			base.gameObject.SetActive(true);
			yield return this.EnterState(RestoreSaveButton.RestoreState.Selection);
			yield break;
		}

		// Token: 0x06004BAD RID: 19373 RVA: 0x001654D4 File Offset: 0x001636D4
		private IEnumerator EnterState(RestoreSaveButton.RestoreState restoreState)
		{
			yield return this.ExitState();
			if (this.currentTransition != null)
			{
				UIManager.instance.StopCoroutine(this.currentTransition);
			}
			this.currentTransition = UIManager.instance.StartCoroutine(this.TransitionToState(restoreState));
			yield return this.currentTransition;
			this.currentTransition = null;
			yield break;
		}

		// Token: 0x06004BAE RID: 19374 RVA: 0x001654EA File Offset: 0x001636EA
		public IEnumerator Hide()
		{
			if (!this.isVisible)
			{
				yield break;
			}
			if (this.currentTransition != null)
			{
				UIManager.instance.StopCoroutine(this.currentTransition);
				this.currentTransition = null;
			}
			if (this.isLoading)
			{
				this.isLoading = false;
				UIManager.instance.StartCoroutine(this.saveSlotButton.HideLoadingPrompt());
			}
			this.isVisible = false;
			yield return this.ExitState();
			yield break;
		}

		// Token: 0x06004BAF RID: 19375 RVA: 0x001654F9 File Offset: 0x001636F9
		private IEnumerator TransitionToState(RestoreSaveButton.RestoreState newState)
		{
			if (!this.isVisible || newState != this.state)
			{
				this.isVisible = true;
				this.state = newState;
				switch (newState)
				{
				case RestoreSaveButton.RestoreState.Button:
					this.saveSlotButton.StartCoroutine(this.ui.FadeInCanvasGroup(this.restoreSaveButton, false));
					this.restoreSaveButton.blocksRaycasts = true;
					break;
				case RestoreSaveButton.RestoreState.Selection:
				{
					if (this.fetchRequest == null)
					{
						this.LoadRestorePoints(this.saveSlotButton.SaveSlotIndex, false);
						yield return null;
						if (this.fetchRequest == null)
						{
							this.HandleNoRequestError();
						}
					}
					bool isSaveIncompatible = this.saveSlotButton.IsSaveIncompatible();
					if (isSaveIncompatible && this.versionDataRequest == null)
					{
						this.LoadVersionRestoresPoints(this.saveSlotButton.SaveSlotIndex);
					}
					if (this.fetchRequest != null)
					{
						FetchDataRequest.Status status = this.fetchRequest.State;
						if (status != FetchDataRequest.Status.InProgress)
						{
							if (status != FetchDataRequest.Status.Completed)
							{
								this.HandleUnexpectedStateError();
							}
						}
						else
						{
							this.restoreSaveSelection.ToggleLoadIcon(true);
							this.isLoading = true;
							float timer = 0f;
							while (timer < 10f)
							{
								yield return null;
								timer += Time.deltaTime;
								if (this.fetchRequest.State == FetchDataRequest.Status.Completed)
								{
									timer = 0f;
									break;
								}
							}
							this.isLoading = false;
							if (timer >= 10f)
							{
								this.HandleTimeoutError();
							}
						}
						List<RestorePointData> restorePoints = new List<RestorePointData>();
						if (isSaveIncompatible && this.versionDataRequest != null)
						{
							Debug.Log(string.Format("Processing Version Backups. : Slot {0}", this.saveSlotButton.SaveSlotIndex), this);
							if (this.versionDataRequest.State != FetchDataRequest.Status.Completed)
							{
								this.restoreSaveSelection.ToggleLoadIcon(true);
								this.isLoading = true;
								float timer = 0f;
								while (timer < 10f)
								{
									yield return null;
									timer += Time.deltaTime;
									if (this.versionDataRequest.State == FetchDataRequest.Status.Completed)
									{
										timer = 0f;
										break;
									}
								}
								this.isLoading = false;
								if (timer >= 10f)
								{
									this.HandleTimeoutError();
								}
							}
							if (this.versionDataRequest.State == FetchDataRequest.Status.Completed && this.versionDataRequest.results != null)
							{
								this.restoreSaveSelection.ToggleLoadIcon(true);
								Debug.Log(string.Format("Found {0} version backups. : Slot {1}", this.versionDataRequest.results.Count, this.saveSlotButton.SaveSlotIndex), this);
								for (int i = 0; i < this.versionDataRequest.results.Count; i++)
								{
									FetchDataRequest<SaveGameData>.FetchResult fetchResult = this.versionDataRequest.results[i];
									if (fetchResult.loadedObject != null && !SaveDataUtility.IsVersionIncompatible("1.0.28324", fetchResult.loadedObject.playerData.version, fetchResult.loadedObject.playerData.RevisionBreak, 28104))
									{
										restorePoints.Add(RestorePointData.CreateVersionBackup(fetchResult.loadedObject));
									}
								}
							}
						}
						if (this.fetchRequest.results != null)
						{
							this.restoreSaveSelection.ToggleLoadIcon(true);
							Debug.Log(string.Format("Found {0} restore points. : Slot {1}", this.fetchRequest.results.Count, this.saveSlotButton.SaveSlotIndex), this);
							for (int j = 0; j < this.fetchRequest.results.Count; j++)
							{
								FetchDataRequest<RestorePointData>.FetchResult fetchResult2 = this.fetchRequest.results[j];
								if (fetchResult2.loadedObject.saveGameData != null && !SaveDataUtility.IsVersionIncompatible("1.0.28324", fetchResult2.loadedObject.saveGameData.playerData.version, fetchResult2.loadedObject.saveGameData.playerData.RevisionBreak, 28104))
								{
									restorePoints.Add(fetchResult2.loadedObject);
								}
							}
						}
						this.restoreSaveSelection.SetRestorePoints(restorePoints);
						restorePoints = null;
					}
					else
					{
						Debug.Log(string.Format("Failed to create fetch request for slot {0}", this.saveSlotButton.SaveSlotIndex), this);
						this.restoreSaveSelection.ClearRestorePoints();
					}
					this.saveSlotButton.parentBlocker.blocksRaycasts = false;
					yield return this.restoreSaveSelection.Show();
					this.ih.StartUIInput();
					RestoreSaveButton.activeRestoreSaveButton = this;
					this.saveSlotButton.controlsGroupBlocker.ignoreParentGroups = true;
					break;
				}
				case RestoreSaveButton.RestoreState.Prompt:
					yield return this.ui.FadeInCanvasGroup(this.restoreSavePrompt, false);
					this.restoreSavePrompt.blocksRaycasts = true;
					this.restoreSavePromptHighlight.HighlightDefault(false);
					this.ih.StartUIInput();
					this.saveSlotButton.controlsGroupBlocker.ignoreParentGroups = true;
					break;
				case RestoreSaveButton.RestoreState.Confirm:
					yield return this.ui.FadeInCanvasGroup(this.restoreSaveConfirmPrompt, false);
					this.restoreSaveConfirmPrompt.blocksRaycasts = true;
					this.restoreSaveConfirmPromptHighlight.HighlightDefault(false);
					this.ih.StartUIInput();
					this.saveSlotButton.controlsGroupBlocker.ignoreParentGroups = true;
					break;
				}
			}
			yield break;
		}

		// Token: 0x06004BB0 RID: 19376 RVA: 0x0016550F File Offset: 0x0016370F
		private IEnumerator ExitState()
		{
			this.isVisible = false;
			this.saveSlotButton.controlsGroupBlocker.ignoreParentGroups = false;
			switch (this.state)
			{
			case RestoreSaveButton.RestoreState.None:
				break;
			case RestoreSaveButton.RestoreState.Button:
				yield return this.ui.FadeOutCanvasGroup(this.restoreSaveButton, false, false);
				this.restoreSaveButton.interactable = false;
				this.restoreSaveButton.blocksRaycasts = false;
				break;
			case RestoreSaveButton.RestoreState.Selection:
				this.ih.StopUIInput();
				if (RestoreSaveButton.activeRestoreSaveButton)
				{
					RestoreSaveButton.activeRestoreSaveButton = null;
				}
				if (this.isLoading)
				{
					this.isLoading = false;
					UIManager.instance.StartCoroutine(this.saveSlotButton.HideLoadingPrompt());
				}
				yield return this.restoreSaveSelection.Hide();
				break;
			case RestoreSaveButton.RestoreState.Prompt:
				this.ih.StopUIInput();
				yield return this.ui.FadeOutCanvasGroup(this.restoreSavePrompt, true, false);
				this.restoreSavePrompt.interactable = false;
				this.restoreSavePrompt.blocksRaycasts = false;
				break;
			case RestoreSaveButton.RestoreState.Confirm:
				this.ih.StopUIInput();
				yield return this.ui.FadeOutCanvasGroup(this.restoreSaveConfirmPrompt, true, false);
				this.restoreSaveConfirmPrompt.interactable = false;
				this.restoreSaveConfirmPrompt.blocksRaycasts = false;
				break;
			default:
				Debug.LogError(string.Format("Exiting unsupported state {0}", this.state), this);
				break;
			}
			yield break;
		}

		// Token: 0x06004BB1 RID: 19377 RVA: 0x0016551E File Offset: 0x0016371E
		public void SaveSelected(RestorePointData restorePointData)
		{
			if (this.state != RestoreSaveButton.RestoreState.Selection)
			{
				Debug.LogWarning("Restore point was selected while in an unexpected state", this);
			}
			if (restorePointData == null)
			{
				Debug.LogError("Restore Point Data is null", this);
				this.saveSlotButton.ShowRelevantModeForSaveFileState();
				return;
			}
			this.selectedRestorePoint = restorePointData;
			this.InternalEnterState(RestoreSaveButton.RestoreState.Prompt);
		}

		// Token: 0x06004BB2 RID: 19378 RVA: 0x0016555C File Offset: 0x0016375C
		private void InternalEnterState(RestoreSaveButton.RestoreState restoreState)
		{
			this.saveSlotButton.StartCoroutine(this.EnterState(restoreState));
		}

		// Token: 0x06004BB3 RID: 19379 RVA: 0x00165571 File Offset: 0x00163771
		private IEnumerator RestoreConfirmed()
		{
			yield return this.Hide();
			this.saveSlotButton.OverrideSaveData(this.selectedRestorePoint);
			this.selectedRestorePoint = null;
			yield break;
		}

		// Token: 0x06004BB4 RID: 19380 RVA: 0x00165580 File Offset: 0x00163780
		private void HandleNoRequestError()
		{
			Debug.LogError(string.Format("Failed fetch restore points Slot {0}", this.saveSlotButton.SaveSlotIndex), this);
		}

		// Token: 0x06004BB5 RID: 19381 RVA: 0x001655A2 File Offset: 0x001637A2
		private void HandleTimeoutError()
		{
			Debug.LogError(string.Format("Timed out while trying to fetch restore points {0}", this.saveSlotButton.SaveSlotIndex), this);
		}

		// Token: 0x06004BB6 RID: 19382 RVA: 0x001655C4 File Offset: 0x001637C4
		private void HandleUnexpectedStateError()
		{
			Debug.LogError(string.Format("Encountered unexpected state while trying to restore save {0}", this.saveSlotButton.SaveSlotIndex), this);
		}

		// Token: 0x06004BB7 RID: 19383 RVA: 0x001655E6 File Offset: 0x001637E6
		public void ResetButton()
		{
			this.fetchRequest = null;
			this.versionDataRequest = null;
		}

		// Token: 0x06004BB8 RID: 19384 RVA: 0x001655F6 File Offset: 0x001637F6
		public void PreloadRestorePoints(int slot, bool isIncompatible)
		{
		}

		// Token: 0x06004BB9 RID: 19385 RVA: 0x001655F8 File Offset: 0x001637F8
		private void LoadRestorePoints(int slot, bool isIncompatible)
		{
			this.fetchRequest = new FetchDataRequest<RestorePointData>(Platform.Current.FetchRestorePoints(slot));
			if (isIncompatible)
			{
				this.LoadVersionRestoresPoints(slot);
			}
		}

		// Token: 0x06004BBA RID: 19386 RVA: 0x0016561A File Offset: 0x0016381A
		private void LoadVersionRestoresPoints(int slot)
		{
			this.versionDataRequest = new FetchDataRequest<SaveGameData>(Platform.Current.FetchVersionRestorePoints(slot));
		}

		// Token: 0x06004BBB RID: 19387 RVA: 0x00165632 File Offset: 0x00163832
		public static bool GoBack()
		{
			if (RestoreSaveButton.activeRestoreSaveButton && RestoreSaveButton.activeRestoreSaveButton.state == RestoreSaveButton.RestoreState.Selection)
			{
				RestoreSaveButton.activeRestoreSaveButton.CancelSelection();
				return true;
			}
			return false;
		}

		// Token: 0x04004CFC RID: 19708
		[Header("Restore Save Button")]
		[SerializeField]
		private SaveSlotButton saveSlotButton;

		// Token: 0x04004CFD RID: 19709
		[SerializeField]
		private Animator selectIcon;

		// Token: 0x04004CFE RID: 19710
		[Space]
		[SerializeField]
		private CanvasGroup restoreSaveButton;

		// Token: 0x04004CFF RID: 19711
		[FormerlySerializedAs("selectionDisplay")]
		[SerializeField]
		private RestoreSavePointDisplay restoreSaveSelection;

		// Token: 0x04004D00 RID: 19712
		[SerializeField]
		private CanvasGroup restoreSavePrompt;

		// Token: 0x04004D01 RID: 19713
		[SerializeField]
		private CanvasGroup restoreSaveConfirmPrompt;

		// Token: 0x04004D02 RID: 19714
		[Header("Debug")]
		[SerializeField]
		private bool fakeLoadingState;

		// Token: 0x04004D03 RID: 19715
		[SerializeField]
		private bool fakeIncompatibleFiles;

		// Token: 0x04004D04 RID: 19716
		[SerializeField]
		private bool fakeEmptyList;

		// Token: 0x04004D05 RID: 19717
		private static readonly int _isSelectedProp = Animator.StringToHash("Is Selected");

		// Token: 0x04004D06 RID: 19718
		private UIManager ui;

		// Token: 0x04004D07 RID: 19719
		private InputHandler ih;

		// Token: 0x04004D08 RID: 19720
		private FetchDataRequest<RestorePointData> fetchRequest;

		// Token: 0x04004D09 RID: 19721
		private FetchDataRequest<SaveGameData> versionDataRequest;

		// Token: 0x04004D0A RID: 19722
		private bool isVisible;

		// Token: 0x04004D0B RID: 19723
		private RestoreSaveButton.RestoreState state;

		// Token: 0x04004D0C RID: 19724
		private PreselectOption restoreSavePromptHighlight;

		// Token: 0x04004D0D RID: 19725
		private PreselectOption restoreSaveConfirmPromptHighlight;

		// Token: 0x04004D0E RID: 19726
		private RestorePointData selectedRestorePoint;

		// Token: 0x04004D0F RID: 19727
		private const float SHOW_LOADING_DELAY = 0f;

		// Token: 0x04004D10 RID: 19728
		private const float LOADING_TIMEOUT = 10f;

		// Token: 0x04004D11 RID: 19729
		private Selectable previousBackSelectable;

		// Token: 0x04004D12 RID: 19730
		private bool isLoading;

		// Token: 0x04004D13 RID: 19731
		private Coroutine currentTransition;

		// Token: 0x04004D14 RID: 19732
		private static RestoreSaveButton activeRestoreSaveButton;

		// Token: 0x02001AE6 RID: 6886
		private enum RestoreState
		{
			// Token: 0x04009ADB RID: 39643
			None,
			// Token: 0x04009ADC RID: 39644
			Button,
			// Token: 0x04009ADD RID: 39645
			Selection,
			// Token: 0x04009ADE RID: 39646
			Prompt,
			// Token: 0x04009ADF RID: 39647
			Confirm
		}
	}
}
