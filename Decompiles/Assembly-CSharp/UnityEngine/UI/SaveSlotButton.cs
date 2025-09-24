using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using GlobalEnums;
using TeamCherry.SharedUtils;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x0200087E RID: 2174
	[Serializable]
	public class SaveSlotButton : MenuButton, ISelectHandler, IEventSystemHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler
	{
		// Token: 0x170008F1 RID: 2289
		// (get) Token: 0x06004BBE RID: 19390 RVA: 0x00165673 File Offset: 0x00163873
		// (set) Token: 0x06004BBF RID: 19391 RVA: 0x0016567B File Offset: 0x0016387B
		public SaveSlotButton.SlotState State { get; private set; }

		// Token: 0x170008F2 RID: 2290
		// (get) Token: 0x06004BC0 RID: 19392 RVA: 0x00165684 File Offset: 0x00163884
		public bool IsBlackThreaded
		{
			get
			{
				return this.State == SaveSlotButton.SlotState.BlackThreadInfected && this.blackThreadImpactsLeft > 0;
			}
		}

		// Token: 0x170008F3 RID: 2291
		// (get) Token: 0x06004BC1 RID: 19393 RVA: 0x0016569B File Offset: 0x0016389B
		public bool HasPreloaded
		{
			get
			{
				return this.preloadOperation != null;
			}
		}

		// Token: 0x170008F4 RID: 2292
		// (get) Token: 0x06004BC2 RID: 19394 RVA: 0x001656A8 File Offset: 0x001638A8
		public int SaveSlotIndex
		{
			get
			{
				switch (this.saveSlot)
				{
				case SaveSlotButton.SaveSlot.Slot1:
					return 1;
				case SaveSlotButton.SaveSlot.Slot2:
					return 2;
				case SaveSlotButton.SaveSlot.Slot3:
					return 3;
				case SaveSlotButton.SaveSlot.Slot4:
					return 4;
				default:
					return 0;
				}
			}
		}

		// Token: 0x06004BC3 RID: 19395 RVA: 0x001656E0 File Offset: 0x001638E0
		private new void Awake()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			this.gm = GameManager.instance;
			this.ui = UIManager.instance;
			this.ih = this.gm.inputHandler;
			this.blackThreads = this.blackThreadsParent.GetComponentsInChildren<BlackThreadStrand>();
			this.clearSavePromptHighlight = this.clearSavePrompt.GetComponent<PreselectOption>();
			this.clearSaveConfirmPromptHighlight = this.clearSaveConfirmPrompt.GetComponent<PreselectOption>();
			this.coroutineQueue = new CoroutineQueue(this);
			this.SetupNavs();
			this.strandsRemovedPerHit = this.blackThreadInfectedGroup.TotalStrands / 5;
			this.clearSaveButtonX = this.clearSaveButton.transform.localPosition.x;
		}

		// Token: 0x06004BC4 RID: 19396 RVA: 0x0016578F File Offset: 0x0016398F
		private new void OnEnable()
		{
			if (this.saveStats != null && this.saveFileState == SaveSlotButton.SaveFileStates.LoadedStats)
			{
				this.PresentSaveSlot(this.saveStats);
			}
		}

		// Token: 0x06004BC5 RID: 19397 RVA: 0x001657AE File Offset: 0x001639AE
		protected override void OnDisable()
		{
			base.OnDisable();
			if (this.coroutineQueue != null)
			{
				this.coroutineQueue.Clear();
				base.StopAllCoroutines();
			}
			if (this.blackThreadImpactsLeft > 0)
			{
				this.BlackThreadOnDisable.Invoke();
			}
		}

		// Token: 0x06004BC6 RID: 19398 RVA: 0x001657E3 File Offset: 0x001639E3
		private new void Start()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			base.HookUpAudioPlayer();
		}

		// Token: 0x06004BC7 RID: 19399 RVA: 0x001657F4 File Offset: 0x001639F4
		public void ResetButton(GameManager gameManager, bool doAnimate = true, bool fetchRestorePoints = true)
		{
			this.ResetFluerTriggersReversed();
			UIManager.FadeOutCanvasGroupInstant(this.slotNumberText, true, false);
			UIManager.FadeOutCanvasGroupInstant(this.backgroundCg, true, false);
			this.backgroundCg.gameObject.SetActive(true);
			UIManager.FadeOutCanvasGroupInstant(this.activeSaveSlot, true, false);
			this.ui.StartCoroutine(this.restoreSaveButton.Hide());
			UIManager.FadeOutCanvasGroupInstant(this.clearSaveButton, true, true);
			UIManager.FadeOutCanvasGroupInstant(this.saveCorruptedText, true, false);
			UIManager.FadeOutCanvasGroupInstant(this.saveIncompatibleText, true, false);
			UIManager.FadeOutCanvasGroupInstant(this.loadingText, true, false);
			base.interactable = true;
			this.myCanvasGroup.blocksRaycasts = true;
			UIManager.FadeOutCanvasGroupInstant(this.defeatedBackground, true, false);
			UIManager.FadeOutCanvasGroupInstant(this.defeatedText, true, false);
			UIManager.FadeOutCanvasGroupInstant(this.brokenSteelOrb, true, false);
			UIManager.FadeOutCanvasGroupInstant(this.newGameText, true, false);
			UIManager.FadeOutCanvasGroupInstant(this.clearSavePrompt, true, true);
			UIManager.FadeOutCanvasGroupInstant(this.clearSaveConfirmPrompt, true, true);
			this.restoreSaveButton.ResetButton();
			this.saveFileState = SaveSlotButton.SaveFileStates.NotStarted;
			this.State = SaveSlotButton.SlotState.Hidden;
			if (doAnimate)
			{
				this.preloadOperation = null;
				this.Prepare(gameManager, true, true, fetchRestorePoints);
				return;
			}
			this.preloadOperation = null;
			this.PreloadSave(gameManager);
		}

		// Token: 0x06004BC8 RID: 19400 RVA: 0x00165927 File Offset: 0x00163B27
		public void ForcePreloadSave(GameManager gameManager)
		{
			this.preloadOperation = new SaveSlotButton.PreloadOperation(this.SaveSlotIndex, gameManager);
		}

		// Token: 0x06004BC9 RID: 19401 RVA: 0x0016593B File Offset: 0x00163B3B
		public void PreloadSave(GameManager gameManager)
		{
			if (this.preloadOperation != null)
			{
				return;
			}
			this.preloadOperation = new SaveSlotButton.PreloadOperation(this.SaveSlotIndex, gameManager);
		}

		// Token: 0x06004BCA RID: 19402 RVA: 0x00165958 File Offset: 0x00163B58
		public void Prepare(GameManager gameManager, bool isReload = false, bool doAnimate = true, bool fetchRestorePoints = false)
		{
			if (isReload || this.saveFileState == SaveSlotButton.SaveFileStates.NotStarted || this.saveFileState == SaveSlotButton.SaveFileStates.Corrupted)
			{
				this.ChangeSaveFileState(SaveSlotButton.SaveFileStates.OperationInProgress, doAnimate);
				if (isReload)
				{
					this.preloadOperation = null;
				}
				else if (this.preloadOperation != null)
				{
					if (doAnimate)
					{
						SaveSlotButton.PreloadOperation currentOperation = this.preloadOperation;
						currentOperation.WaitForComplete(delegate(SaveSlotButton.PreloadOperation.PreloadState state)
						{
							if (currentOperation != this.preloadOperation)
							{
								return;
							}
							this.preloadOperation = null;
							if (state == SaveSlotButton.PreloadOperation.PreloadState.Complete)
							{
								if (currentOperation.IsEmpty)
								{
									this.ChangeSaveFileState(SaveSlotButton.SaveFileStates.Empty, true);
									return;
								}
								this.ProcessSaveStats(true, currentOperation.Message, currentOperation.SaveStats);
							}
						});
					}
					return;
				}
				if (DemoHelper.IsDemoMode)
				{
					if (DemoHelper.HasSaveFile(this.SaveSlotIndex))
					{
						this.PrepareInternal(gameManager, true, doAnimate, false);
						return;
					}
					base.gameObject.SetActive(false);
					return;
				}
				else
				{
					Platform.Current.IsSaveSlotInUse(this.SaveSlotIndex, delegate(bool fileExists)
					{
						this.PrepareInternal(gameManager, fileExists, doAnimate, fetchRestorePoints);
					});
				}
			}
		}

		// Token: 0x06004BCB RID: 19403 RVA: 0x00165A4C File Offset: 0x00163C4C
		private void PrepareInternal(GameManager gameManager, bool fileExists, bool doAnimate, bool fetchRestorePoints)
		{
			if (!fileExists)
			{
				this.ChangeSaveFileState(SaveSlotButton.SaveFileStates.Empty, doAnimate);
				return;
			}
			bool isIncompatible = false;
			gameManager.GetSaveStatsForSlot(this.SaveSlotIndex, delegate(SaveStats newSaveStats, string errorInfo)
			{
				isIncompatible = this.ProcessSaveStats(doAnimate, errorInfo, newSaveStats);
				if (fetchRestorePoints)
				{
					this.FetchRestorePoints(isIncompatible);
				}
			});
		}

		// Token: 0x06004BCC RID: 19404 RVA: 0x00165AA8 File Offset: 0x00163CA8
		private bool ProcessSaveStats(bool doAnimate, string errorInfo, SaveStats newSaveStats)
		{
			bool result = false;
			CheatManager.LastErrorText = errorInfo;
			if (newSaveStats == null)
			{
				this.ChangeSaveFileState(SaveSlotButton.SaveFileStates.Corrupted, doAnimate);
			}
			else if (newSaveStats.IsBlank)
			{
				this.ChangeSaveFileState(SaveSlotButton.SaveFileStates.Empty, doAnimate);
			}
			else if (SaveSlotButton.IsVersionIncompatible(newSaveStats.Version, newSaveStats.RevisionBreak))
			{
				this.ChangeSaveFileState(SaveSlotButton.SaveFileStates.Incompatible, doAnimate);
				result = true;
			}
			else
			{
				this.saveStats = newSaveStats;
				this.ChangeSaveFileState(SaveSlotButton.SaveFileStates.LoadedStats, doAnimate);
			}
			return result;
		}

		// Token: 0x06004BCD RID: 19405 RVA: 0x00165B0B File Offset: 0x00163D0B
		public bool IsSaveIncompatible()
		{
			return this.saveStats == null || SaveSlotButton.IsVersionIncompatible(this.saveStats.Version, this.saveStats.RevisionBreak);
		}

		// Token: 0x06004BCE RID: 19406 RVA: 0x00165B32 File Offset: 0x00163D32
		public void FetchRestorePoints(bool isIncompatible)
		{
			if (this.restoreSaveButton)
			{
				this.restoreSaveButton.PreloadRestorePoints(this.SaveSlotIndex, isIncompatible);
			}
		}

		// Token: 0x06004BCF RID: 19407 RVA: 0x00165B53 File Offset: 0x00163D53
		private static bool IsVersionIncompatible(string fileVersionText, int fileRevisionBreak)
		{
			return SaveDataUtility.IsVersionIncompatible("1.0.28324", fileVersionText, fileRevisionBreak, 28104);
		}

		// Token: 0x06004BD0 RID: 19408 RVA: 0x00165B66 File Offset: 0x00163D66
		public void ClearCache()
		{
			this.saveFileState = SaveSlotButton.SaveFileStates.NotStarted;
			this.saveStats = null;
		}

		// Token: 0x06004BD1 RID: 19409 RVA: 0x00165B76 File Offset: 0x00163D76
		private void ChangeSaveFileState(SaveSlotButton.SaveFileStates nextSaveFileState, bool doAnimate = true)
		{
			this.saveFileState = nextSaveFileState;
			if (doAnimate && base.isActiveAndEnabled)
			{
				this.ShowRelevantModeForSaveFileState();
			}
		}

		// Token: 0x06004BD2 RID: 19410 RVA: 0x00165B90 File Offset: 0x00163D90
		private bool TryEnterSave()
		{
			if (!this.saveStats.IsBlackThreadInfected)
			{
				return true;
			}
			if (Time.timeAsDouble < this.nextImpactTime)
			{
				return false;
			}
			this.nextImpactTime = Time.timeAsDouble + 0.5;
			this.blackThreadImpactsLeft--;
			this.ui.UpdateBlackThreadAudio();
			if (this.blackThreadImpactsLeft > 0)
			{
				this.OnBlackThreadImpact.Invoke();
				this.blackThreadInfectedGroup.HideStrands(Random.Range(this.strandsRemovedPerHit - 1, this.strandsRemovedPerHit + 1));
				MinMaxFloat minMaxFloat = new MinMaxFloat(0.35f, 0.4f);
				BlackThreadStrand[] array = this.blackThreads;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].RageForTime(minMaxFloat.GetRandomValue(), false);
				}
				return false;
			}
			if (this.blackThreadImpactsLeft == 0)
			{
				this.OnBlackThreadBurst.Invoke();
				this.PresentSaveSlot(this.saveStats);
				base.StartCoroutine(this.FadeUpSlotInfoUninfected());
				this.saveStats.HasClearedBlackThreads = true;
				return false;
			}
			return true;
		}

		// Token: 0x06004BD3 RID: 19411 RVA: 0x00165C90 File Offset: 0x00163E90
		private IEnumerator FadeUpSlotInfoUninfected()
		{
			this.ih.StopUIInput();
			if (this.leftCursor != null)
			{
				this.leftCursor.ResetTrigger(SaveSlotButton._showAnimProp);
				this.leftCursor.SetTrigger(SaveSlotButton._hideAnimProp);
			}
			if (this.rightCursor != null)
			{
				this.rightCursor.ResetTrigger(SaveSlotButton._showAnimProp);
				this.rightCursor.SetTrigger(SaveSlotButton._hideAnimProp);
			}
			this.<FadeUpSlotInfoUninfected>g__SetAlpha|95_0(0f);
			yield return new WaitForSeconds(1.5f);
			float elapsed;
			for (elapsed = 0f; elapsed < 2f; elapsed += Time.deltaTime)
			{
				float t = elapsed / 2.5f;
				this.<FadeUpSlotInfoUninfected>g__SetAlpha|95_0(t);
				yield return null;
			}
			this.ih.StartUIInput();
			if (this.leftCursor != null)
			{
				this.leftCursor.ResetTrigger(SaveSlotButton._hideAnimProp);
				this.leftCursor.SetTrigger(SaveSlotButton._showAnimProp);
			}
			if (this.rightCursor != null)
			{
				this.rightCursor.ResetTrigger(SaveSlotButton._hideAnimProp);
				this.rightCursor.SetTrigger(SaveSlotButton._showAnimProp);
			}
			while (elapsed < 2.5f)
			{
				float t2 = elapsed / 2.5f;
				this.<FadeUpSlotInfoUninfected>g__SetAlpha|95_0(t2);
				yield return null;
				elapsed += Time.deltaTime;
			}
			this.<FadeUpSlotInfoUninfected>g__SetAlpha|95_0(1f);
			yield break;
		}

		// Token: 0x06004BD4 RID: 19412 RVA: 0x00165CA0 File Offset: 0x00163EA0
		public new void OnSubmit(BaseEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			Platform.Current.LocalSharedData.SetInt("lastProfileIndex", this.SaveSlotIndex);
			if (this.saveFileState == SaveSlotButton.SaveFileStates.LoadedStats)
			{
				if (this.saveStats.PermadeathMode == PermadeathModes.Dead)
				{
					base.ForceDeselect();
					this.ClearSavePrompt();
				}
				else
				{
					if (!this.TryEnterSave())
					{
						return;
					}
					if (DemoHelper.IsDemoMode && !DemoHelper.IsExhibitionMode && DemoHelper.IsDummySaveFile(this.SaveSlotIndex))
					{
						this.gm.profileID = this.SaveSlotIndex;
						this.ui.StartNewGame(false, false);
					}
					else
					{
						UIManager uimanager = this.ui;
						int saveSlotIndex = this.SaveSlotIndex;
						SaveStats saveStats = this.saveStats;
						uimanager.UIContinueGame(saveSlotIndex, (saveStats != null) ? saveStats.saveGameData : null);
					}
				}
				base.OnSubmit(eventData);
				return;
			}
			if (this.saveFileState == SaveSlotButton.SaveFileStates.Empty)
			{
				this.gm.profileID = this.SaveSlotIndex;
				if (this.gm.GetStatusRecordInt("RecPermadeathMode") == 1 || this.gm.GetStatusRecordInt("RecBossRushMode") == 1)
				{
					this.ui.UIGoToPlayModeMenu();
				}
				else
				{
					this.ui.StartNewGame(false, false);
				}
				base.OnSubmit(eventData);
				return;
			}
			if (this.saveFileState == SaveSlotButton.SaveFileStates.Corrupted)
			{
				GenericMessageCanvas.Show("SAVE_CORRUPTED", null);
			}
		}

		// Token: 0x06004BD5 RID: 19413 RVA: 0x00165DDD File Offset: 0x00163FDD
		protected IEnumerator ReloadCorrupted()
		{
			this.ih.StopUIInput();
			this.Prepare(this.gm, true, true, false);
			while (this.saveFileState == SaveSlotButton.SaveFileStates.OperationInProgress)
			{
				yield return null;
			}
			this.ih.StartUIInput();
			yield break;
		}

		// Token: 0x06004BD6 RID: 19414 RVA: 0x00165DEC File Offset: 0x00163FEC
		public new void OnPointerClick(PointerEventData eventData)
		{
			this.OnSubmit(eventData);
		}

		// Token: 0x06004BD7 RID: 19415 RVA: 0x00165DF8 File Offset: 0x00163FF8
		public new void OnSelect(BaseEventData eventData)
		{
			this.highlight.ResetTrigger(SaveSlotButton._hideAnimProp);
			this.highlight.SetTrigger(SaveSlotButton._showAnimProp);
			if (this.leftCursor != null)
			{
				this.leftCursor.ResetTrigger(SaveSlotButton._hideAnimProp);
				this.leftCursor.SetTrigger(SaveSlotButton._showAnimProp);
			}
			if (this.rightCursor != null)
			{
				this.rightCursor.ResetTrigger(SaveSlotButton._hideAnimProp);
				this.rightCursor.SetTrigger(SaveSlotButton._showAnimProp);
			}
			base.OnSelect(eventData);
			if (!base.interactable)
			{
				try
				{
					this.uiAudioPlayer.PlaySelect();
				}
				catch (Exception ex)
				{
					string name = base.name;
					string str = " doesn't have a select sound specified. ";
					Exception ex2 = ex;
					Debug.LogError(name + str + ((ex2 != null) ? ex2.ToString() : null));
				}
			}
		}

		// Token: 0x06004BD8 RID: 19416 RVA: 0x00165ED4 File Offset: 0x001640D4
		public new void OnDeselect(BaseEventData eventData)
		{
			base.StartCoroutine(this.ValidateDeselect());
		}

		// Token: 0x06004BD9 RID: 19417 RVA: 0x00165EE4 File Offset: 0x001640E4
		public void UpdateSaveFileState()
		{
			if (this.saveFileState == SaveSlotButton.SaveFileStates.Empty && this.preloadOperation != null)
			{
				SaveSlotButton.PreloadOperation.PreloadState state = this.preloadOperation.State;
				if (state == SaveSlotButton.PreloadOperation.PreloadState.Loading)
				{
					this.saveFileState = SaveSlotButton.SaveFileStates.OperationInProgress;
					return;
				}
				if (state != SaveSlotButton.PreloadOperation.PreloadState.Complete)
				{
					return;
				}
				if (this.preloadOperation.IsEmpty)
				{
					this.saveFileState = SaveSlotButton.SaveFileStates.Empty;
					return;
				}
				this.ProcessSaveStats(false, this.preloadOperation.Message, this.preloadOperation.SaveStats);
			}
		}

		// Token: 0x06004BDA RID: 19418 RVA: 0x00165F54 File Offset: 0x00164154
		public void ShowRelevantModeForSaveFileState()
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			switch (this.saveFileState)
			{
			case SaveSlotButton.SaveFileStates.NotStarted:
			case SaveSlotButton.SaveFileStates.OperationInProgress:
				this.coroutineQueue.Enqueue(this.AnimateToSlotState(SaveSlotButton.SlotState.OperationInProgress, true));
				return;
			case SaveSlotButton.SaveFileStates.Empty:
				this.coroutineQueue.Enqueue(this.AnimateToSlotState(SaveSlotButton.SlotState.EmptySlot, true));
				return;
			case SaveSlotButton.SaveFileStates.LoadedStats:
			{
				SaveSlotButton.SlotState nextState;
				if (this.saveStats.IsBlackThreadInfected)
				{
					nextState = SaveSlotButton.SlotState.BlackThreadInfected;
				}
				else if (this.saveStats.PermadeathMode == PermadeathModes.Dead)
				{
					nextState = SaveSlotButton.SlotState.Defeated;
				}
				else
				{
					nextState = SaveSlotButton.SlotState.SavePresent;
				}
				this.coroutineQueue.Enqueue(this.AnimateToSlotState(nextState, true));
				return;
			}
			case SaveSlotButton.SaveFileStates.Corrupted:
				this.coroutineQueue.Enqueue(this.AnimateToSlotState(SaveSlotButton.SlotState.Corrupted, true));
				return;
			case SaveSlotButton.SaveFileStates.Incompatible:
				this.coroutineQueue.Enqueue(this.AnimateToSlotState(SaveSlotButton.SlotState.Incompatible, true));
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06004BDB RID: 19419 RVA: 0x00166028 File Offset: 0x00164228
		public void HideSaveSlot(bool updateBlackThread)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			this.coroutineQueue.Enqueue(this.AnimateToSlotState(SaveSlotButton.SlotState.Hidden, updateBlackThread));
		}

		// Token: 0x06004BDC RID: 19420 RVA: 0x0016604B File Offset: 0x0016424B
		public void ClearSavePrompt()
		{
			this.coroutineQueue.Enqueue(this.AnimateToSlotState(SaveSlotButton.SlotState.ClearPrompt, true));
		}

		// Token: 0x06004BDD RID: 19421 RVA: 0x00166060 File Offset: 0x00164260
		public void ClearSaveConfirmPrompt()
		{
			if (this.saveFileState == SaveSlotButton.SaveFileStates.LoadedStats && this.saveStats != null && this.saveStats.PermadeathMode == PermadeathModes.Dead)
			{
				this.ClearSaveFile();
				return;
			}
			this.coroutineQueue.Enqueue(this.AnimateToSlotState(SaveSlotButton.SlotState.ClearConfirm, true));
		}

		// Token: 0x06004BDE RID: 19422 RVA: 0x0016609B File Offset: 0x0016429B
		public void CancelClearSave()
		{
			if (this.State == SaveSlotButton.SlotState.ClearPrompt || this.State == SaveSlotButton.SlotState.ClearConfirm)
			{
				this.parentBlocker.blocksRaycasts = true;
				this.ShowRelevantModeForSaveFileState();
			}
		}

		// Token: 0x06004BDF RID: 19423 RVA: 0x001660C1 File Offset: 0x001642C1
		public void ClearSaveFile()
		{
			this.gm.ClearSaveFile(this.SaveSlotIndex, delegate(bool didClear)
			{
				if (didClear)
				{
					this.saveStats = null;
					this.ChangeSaveFileState(SaveSlotButton.SaveFileStates.Empty, true);
					return;
				}
				this.coroutineQueue.Enqueue(this.AnimateToSlotState(SaveSlotButton.SlotState.SavePresent, true));
			});
		}

		// Token: 0x06004BE0 RID: 19424 RVA: 0x001660E0 File Offset: 0x001642E0
		public IEnumerator ShowLoadingPrompt(float delay)
		{
			if (this.State != SaveSlotButton.SlotState.RestoreSave)
			{
				yield break;
			}
			this.ResetFluerTriggers();
			yield return new WaitForSeconds(0.1f);
			base.StartCoroutine(this.currentLoadingTextFadeIn = this.FadeInCanvasGroupAfterDelay(delay, this.loadingText));
			yield break;
		}

		// Token: 0x06004BE1 RID: 19425 RVA: 0x001660F6 File Offset: 0x001642F6
		public IEnumerator HideLoadingPrompt()
		{
			if (this.State != SaveSlotButton.SlotState.RestoreSave)
			{
				Debug.LogError(string.Format("Unexpected call during {0} state detected", this.State), this);
				yield break;
			}
			if (this.currentLoadingTextFadeIn != null)
			{
				base.StopCoroutine(this.currentLoadingTextFadeIn);
			}
			yield return this.ui.FadeOutCanvasGroup(this.loadingText, true, false);
			yield break;
		}

		// Token: 0x06004BE2 RID: 19426 RVA: 0x00166105 File Offset: 0x00164305
		public void RestoreSaveMenu()
		{
			this.coroutineQueue.Enqueue(this.AnimateToSlotState(SaveSlotButton.SlotState.RestoreSave, true));
		}

		// Token: 0x06004BE3 RID: 19427 RVA: 0x0016611C File Offset: 0x0016431C
		public void OverrideSaveData(RestorePointData data)
		{
			if (this.State != SaveSlotButton.SlotState.RestoreSave)
			{
				Debug.LogError("Unable to override data while not in restore save state", this);
				return;
			}
			this.isRestoringSave = true;
			this.coroutineQueue.Enqueue(this.AnimateToSlotState(SaveSlotButton.SlotState.OperationInProgress, true));
			Platform.Current.WriteSaveSlot(this.SaveSlotIndex, GameManager.instance.GetBytesForSaveData(data.saveGameData), delegate(bool result)
			{
				if (result)
				{
					this.saveStats = GameManager.GetSaveStatsFromData(data.saveGameData);
				}
				else
				{
					Debug.LogError("Failed to override data", this);
				}
				this.preloadOperation = null;
				this.ProcessSaveStats(true, null, this.saveStats);
			});
		}

		// Token: 0x06004BE4 RID: 19428 RVA: 0x0016619E File Offset: 0x0016439E
		private IEnumerator FadeInCanvasGroupAfterDelay(float delay, CanvasGroup cg)
		{
			for (float timer = 0f; timer < delay; timer += Time.unscaledDeltaTime)
			{
				yield return null;
			}
			yield return this.ui.FadeInCanvasGroup(cg, false);
			yield break;
		}

		// Token: 0x06004BE5 RID: 19429 RVA: 0x001661BB File Offset: 0x001643BB
		private IEnumerator AnimateToSlotState(SaveSlotButton.SlotState nextState, bool updateBlackThread = true)
		{
			SaveSlotButton.SlotState state = this.State;
			if (state == nextState)
			{
				yield break;
			}
			if (this.currentLoadingTextFadeIn != null)
			{
				base.StopCoroutine(this.currentLoadingTextFadeIn);
				this.currentLoadingTextFadeIn = null;
			}
			this.State = nextState;
			if (updateBlackThread)
			{
				if (nextState == SaveSlotButton.SlotState.BlackThreadInfected && !this.saveStats.HasClearedBlackThreads)
				{
					this.blackThreadImpactsLeft = 5;
				}
				this.ui.UpdateBlackThreadAudio();
			}
			if (!DemoHelper.IsDemoMode)
			{
				switch (nextState)
				{
				case SaveSlotButton.SlotState.Hidden:
				case SaveSlotButton.SlotState.OperationInProgress:
					base.navigation = this.noNav;
					break;
				case SaveSlotButton.SlotState.EmptySlot:
				case SaveSlotButton.SlotState.BlackThreadInfected:
					base.navigation = this.emptySlotNav;
					break;
				case SaveSlotButton.SlotState.SavePresent:
				case SaveSlotButton.SlotState.Corrupted:
				case SaveSlotButton.SlotState.Incompatible:
				case SaveSlotButton.SlotState.RestoreSave:
					base.navigation = this.fullSlotNav;
					break;
				case SaveSlotButton.SlotState.Defeated:
					base.navigation = this.defeatedSlotNav;
					break;
				}
			}
			if (this.highlightImage)
			{
				this.highlightImage.material = ((nextState == SaveSlotButton.SlotState.SavePresent || nextState == SaveSlotButton.SlotState.BlackThreadInfected) ? this.highlightMatFull : this.highlightMatEmpty);
			}
			if (nextState != SaveSlotButton.SlotState.ClearPrompt && nextState != SaveSlotButton.SlotState.ClearConfirm)
			{
				this.clearSaveButton.transform.SetLocalPositionX(this.clearSaveButtonX);
			}
			if (state == SaveSlotButton.SlotState.Hidden)
			{
				if (nextState == SaveSlotButton.SlotState.OperationInProgress)
				{
					this.ResetFluerTriggers();
					yield return new WaitForSeconds(0.1f);
					base.StartCoroutine(this.currentLoadingTextFadeIn = this.FadeInCanvasGroupAfterDelay(5f, this.loadingText));
				}
				else if (nextState == SaveSlotButton.SlotState.EmptySlot)
				{
					this.backgroundCg.alpha = 0f;
					this.backgroundCg.gameObject.SetActive(true);
					this.ResetFluerTriggers();
					yield return new WaitForSeconds(0.1f);
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.slotNumberText, false));
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.newGameText, false));
				}
				else if (nextState == SaveSlotButton.SlotState.SavePresent || nextState == SaveSlotButton.SlotState.BlackThreadInfected)
				{
					if (nextState == SaveSlotButton.SlotState.BlackThreadInfected && !this.saveStats.HasClearedBlackThreads)
					{
						this.blackThreadImpactsLeft = 5;
					}
					this.ResetFluerTriggers();
					yield return new WaitForSeconds(0.1f);
					this.PresentSaveSlot(this.saveStats);
					this.SavePresentFadeIn(this.saveStats);
				}
				else if (nextState == SaveSlotButton.SlotState.Defeated)
				{
					this.ResetFluerTriggers();
					yield return new WaitForSeconds(0.1f);
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.defeatedBackground, false));
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.defeatedText, false));
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.brokenSteelOrb, false));
					this.clearSaveButton.transform.SetLocalPositionX(0f);
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.clearSaveButton, false));
					base.StartCoroutine(this.restoreSaveButton.Hide());
					this.clearSaveButton.blocksRaycasts = true;
					this.myCanvasGroup.blocksRaycasts = true;
				}
				else if (nextState == SaveSlotButton.SlotState.Corrupted || nextState == SaveSlotButton.SlotState.Incompatible)
				{
					this.backgroundCg.alpha = 0f;
					this.backgroundCg.gameObject.SetActive(true);
					this.ResetFluerTriggers();
					yield return new WaitForSeconds(0.1f);
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.slotNumberText, false));
					if (nextState != SaveSlotButton.SlotState.Corrupted)
					{
						if (nextState == SaveSlotButton.SlotState.Incompatible)
						{
							base.StartCoroutine(this.ui.FadeInCanvasGroup(this.saveIncompatibleText, false));
						}
					}
					else
					{
						base.StartCoroutine(this.ui.FadeInCanvasGroup(this.saveCorruptedText, false));
					}
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.clearSaveButton, false));
					base.StartCoroutine(this.restoreSaveButton.ShowRestoreSaveButton());
					this.clearSaveButton.blocksRaycasts = true;
					this.myCanvasGroup.blocksRaycasts = true;
				}
			}
			else if (state == SaveSlotButton.SlotState.OperationInProgress)
			{
				if (nextState == SaveSlotButton.SlotState.Hidden)
				{
					this.ResetFluerTriggersReversed();
					yield return new WaitForSeconds(0.1f);
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.loadingText, true, false));
				}
				else if (nextState == SaveSlotButton.SlotState.EmptySlot)
				{
					yield return base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.loadingText, true, false));
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.slotNumberText, false));
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.newGameText, false));
				}
				else if (nextState == SaveSlotButton.SlotState.SavePresent || nextState == SaveSlotButton.SlotState.BlackThreadInfected)
				{
					if (!this.saveStats.HasClearedBlackThreads)
					{
						this.blackThreadImpactsLeft = 5;
					}
					yield return base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.loadingText, true, false));
					this.PresentSaveSlot(this.saveStats);
					this.SavePresentFadeIn(this.saveStats);
					if (this.isRestoringSave)
					{
						this.isRestoringSave = false;
						base.StartCoroutine(this.ui.FadeInCanvasGroup(this.backgroundCg, false));
						base.StartCoroutine(this.ui.FadeInCanvasGroup(this.activeSaveSlot, false));
						if (!DemoHelper.IsDemoMode)
						{
							if (!this.saveStats.IsBlackThreadInfected || this.blackThreadImpactsLeft <= 0)
							{
								base.StartCoroutine(this.ui.FadeInCanvasGroup(this.slotNumberText, false));
							}
							if (!this.saveStats.IsBlackThreadInfected)
							{
								base.StartCoroutine(this.restoreSaveButton.ShowRestoreSaveButton());
								yield return this.ui.FadeInCanvasGroup(this.clearSaveButton, false);
								this.clearSaveButton.blocksRaycasts = true;
							}
						}
						this.ih.StartUIInput();
						this.Select();
					}
					else
					{
						this.SavePresentFadeIn(this.saveStats);
					}
				}
				else if (nextState == SaveSlotButton.SlotState.Defeated)
				{
					yield return base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.loadingText, true, false));
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.defeatedBackground, false));
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.defeatedText, false));
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.brokenSteelOrb, false));
					this.clearSaveButton.transform.SetLocalPositionX(0f);
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.clearSaveButton, false));
					base.StartCoroutine(this.restoreSaveButton.Hide());
					this.clearSaveButton.blocksRaycasts = true;
					this.myCanvasGroup.blocksRaycasts = true;
				}
				else if (nextState == SaveSlotButton.SlotState.Corrupted || nextState == SaveSlotButton.SlotState.Incompatible)
				{
					yield return base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.loadingText, true, false));
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.slotNumberText, false));
					if (nextState != SaveSlotButton.SlotState.Corrupted)
					{
						if (nextState == SaveSlotButton.SlotState.Incompatible)
						{
							base.StartCoroutine(this.ui.FadeInCanvasGroup(this.saveIncompatibleText, false));
						}
					}
					else
					{
						base.StartCoroutine(this.ui.FadeInCanvasGroup(this.saveCorruptedText, false));
					}
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.clearSaveButton, false));
					base.StartCoroutine(this.restoreSaveButton.ShowRestoreSaveButton());
					this.clearSaveButton.blocksRaycasts = true;
					this.myCanvasGroup.blocksRaycasts = true;
				}
			}
			else if (state == SaveSlotButton.SlotState.SavePresent || state == SaveSlotButton.SlotState.BlackThreadInfected)
			{
				if (nextState == SaveSlotButton.SlotState.ClearPrompt)
				{
					this.ih.StopUIInput();
					base.interactable = false;
					this.myCanvasGroup.blocksRaycasts = false;
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.slotNumberText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.activeSaveSlot, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.backgroundCg, true, false));
					base.StartCoroutine(this.restoreSaveButton.Hide());
					yield return base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.clearSaveButton, false, false));
					this.clearSaveButton.blocksRaycasts = false;
					this.parentBlocker.blocksRaycasts = false;
					yield return base.StartCoroutine(this.ui.FadeInCanvasGroup(this.clearSavePrompt, false));
					this.clearSavePrompt.interactable = true;
					this.clearSavePrompt.blocksRaycasts = true;
					this.clearSavePromptHighlight.HighlightDefault(false);
					this.ih.StartUIInput();
				}
				else if (nextState == SaveSlotButton.SlotState.Hidden)
				{
					this.ResetFluerTriggersReversed();
					yield return new WaitForSeconds(0.1f);
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.slotNumberText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.backgroundCg, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.activeSaveSlot, true, false));
					base.StartCoroutine(this.restoreSaveButton.Hide());
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.clearSaveButton, false, false));
					this.clearSaveButton.blocksRaycasts = false;
				}
				else if (nextState == SaveSlotButton.SlotState.RestoreSave)
				{
					this.ih.StopUIInput();
					base.interactable = false;
					this.myCanvasGroup.blocksRaycasts = false;
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.slotNumberText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.activeSaveSlot, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.backgroundCg, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.clearSaveButton, false, false));
					this.clearSaveButton.blocksRaycasts = false;
					this.parentBlocker.blocksRaycasts = false;
					yield return this.restoreSaveButton.ShowSaveSelection();
				}
			}
			else if (state == SaveSlotButton.SlotState.ClearPrompt || state == SaveSlotButton.SlotState.ClearConfirm)
			{
				CanvasGroup currentFadePrompt = (state == SaveSlotButton.SlotState.ClearPrompt) ? this.clearSavePrompt : this.clearSaveConfirmPrompt;
				if (nextState == SaveSlotButton.SlotState.ClearConfirm)
				{
					this.ih.StopUIInput();
					yield return base.StartCoroutine(this.ui.FadeOutCanvasGroup(currentFadePrompt, true, false));
					currentFadePrompt.interactable = false;
					yield return base.StartCoroutine(this.ui.FadeInCanvasGroup(this.clearSaveConfirmPrompt, false));
					this.clearSaveConfirmPrompt.interactable = true;
					this.clearSaveConfirmPrompt.blocksRaycasts = true;
					this.clearSaveConfirmPromptHighlight.HighlightDefault(false);
					this.ih.StartUIInput();
				}
				else if (nextState == SaveSlotButton.SlotState.SavePresent || nextState == SaveSlotButton.SlotState.BlackThreadInfected)
				{
					this.ih.StopUIInput();
					yield return base.StartCoroutine(this.ui.FadeOutCanvasGroup(currentFadePrompt, true, false));
					this.parentBlocker.blocksRaycasts = true;
					currentFadePrompt.interactable = false;
					currentFadePrompt.blocksRaycasts = false;
					if (this.saveStats != null)
					{
						this.PresentSaveSlot(this.saveStats);
					}
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.slotNumberText, false));
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.activeSaveSlot, false));
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.backgroundCg, false));
					base.StartCoroutine(this.restoreSaveButton.ShowRestoreSaveButton());
					yield return base.StartCoroutine(this.ui.FadeInCanvasGroup(this.clearSaveButton, false));
					this.clearSaveButton.blocksRaycasts = true;
					base.interactable = true;
					this.myCanvasGroup.blocksRaycasts = true;
					this.Select();
					this.ih.StartUIInput();
				}
				else if (nextState == SaveSlotButton.SlotState.EmptySlot)
				{
					this.ih.StopUIInput();
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.backgroundCg, true, false));
					yield return base.StartCoroutine(this.ui.FadeOutCanvasGroup(currentFadePrompt, true, false));
					currentFadePrompt.interactable = false;
					currentFadePrompt.blocksRaycasts = false;
					this.parentBlocker.blocksRaycasts = true;
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.slotNumberText, false));
					yield return base.StartCoroutine(this.ui.FadeInCanvasGroup(this.newGameText, false));
					this.myCanvasGroup.blocksRaycasts = true;
					base.interactable = true;
					this.Select();
					this.ih.StartUIInput();
				}
				else if (nextState == SaveSlotButton.SlotState.Defeated)
				{
					this.ih.StopUIInput();
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.backgroundCg, true, false));
					yield return base.StartCoroutine(this.ui.FadeOutCanvasGroup(currentFadePrompt, true, false));
					currentFadePrompt.interactable = false;
					currentFadePrompt.blocksRaycasts = false;
					this.parentBlocker.blocksRaycasts = true;
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.defeatedBackground, false));
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.defeatedText, false));
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.brokenSteelOrb, false));
					base.StartCoroutine(this.restoreSaveButton.Hide());
					this.clearSaveButton.transform.SetLocalPositionX(0f);
					yield return base.StartCoroutine(this.ui.FadeInCanvasGroup(this.clearSaveButton, false));
					base.interactable = true;
					this.clearSaveButton.blocksRaycasts = true;
					this.myCanvasGroup.blocksRaycasts = true;
					this.Select();
					this.ih.StartUIInput();
				}
				else if (nextState == SaveSlotButton.SlotState.Hidden)
				{
					yield return base.StartCoroutine(this.ui.FadeOutCanvasGroup(currentFadePrompt, true, false));
				}
				else if (nextState == SaveSlotButton.SlotState.Corrupted || nextState == SaveSlotButton.SlotState.Incompatible)
				{
					this.ih.StopUIInput();
					yield return base.StartCoroutine(this.ui.FadeOutCanvasGroup(currentFadePrompt, true, false));
					currentFadePrompt.interactable = false;
					currentFadePrompt.blocksRaycasts = false;
					this.parentBlocker.blocksRaycasts = true;
					base.StartCoroutine(this.ui.FadeInCanvasGroup(this.slotNumberText, false));
					if (nextState != SaveSlotButton.SlotState.Corrupted)
					{
						if (nextState == SaveSlotButton.SlotState.Incompatible)
						{
							base.StartCoroutine(this.ui.FadeInCanvasGroup(this.saveIncompatibleText, false));
						}
					}
					else
					{
						base.StartCoroutine(this.ui.FadeInCanvasGroup(this.saveCorruptedText, false));
					}
					base.StartCoroutine(this.restoreSaveButton.ShowRestoreSaveButton());
					yield return base.StartCoroutine(this.ui.FadeInCanvasGroup(this.clearSaveButton, false));
					this.clearSaveButton.blocksRaycasts = true;
					this.myCanvasGroup.blocksRaycasts = true;
					this.Select();
					this.ih.StartUIInput();
				}
				currentFadePrompt = null;
			}
			else if (state == SaveSlotButton.SlotState.EmptySlot)
			{
				if (nextState == SaveSlotButton.SlotState.Hidden)
				{
					this.ResetFluerTriggersReversed();
					yield return new WaitForSeconds(0.1f);
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.slotNumberText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.backgroundCg, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.newGameText, true, false));
				}
			}
			else if (state == SaveSlotButton.SlotState.Defeated)
			{
				if (nextState == SaveSlotButton.SlotState.ClearPrompt)
				{
					this.ih.StopUIInput();
					base.interactable = false;
					this.myCanvasGroup.blocksRaycasts = false;
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.defeatedBackground, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.defeatedText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.brokenSteelOrb, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.restoreSaveButton.CanvasGroup, false, false));
					yield return base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.clearSaveButton, false, false));
					this.clearSaveButton.blocksRaycasts = false;
					this.parentBlocker.blocksRaycasts = false;
					yield return base.StartCoroutine(this.ui.FadeInCanvasGroup(this.clearSavePrompt, false));
					this.clearSavePrompt.interactable = true;
					this.clearSavePrompt.blocksRaycasts = true;
					this.clearSavePromptHighlight.HighlightDefault(false);
					base.interactable = false;
					this.myCanvasGroup.blocksRaycasts = false;
					this.ih.StartUIInput();
				}
				else if (nextState == SaveSlotButton.SlotState.Hidden)
				{
					this.ResetFluerTriggersReversed();
					yield return new WaitForSeconds(0.1f);
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.slotNumberText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.backgroundCg, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.activeSaveSlot, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.defeatedBackground, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.defeatedText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.brokenSteelOrb, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.restoreSaveButton.CanvasGroup, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.clearSaveButton, false, false));
					this.clearSaveButton.blocksRaycasts = false;
				}
				else if (nextState == SaveSlotButton.SlotState.RestoreSave)
				{
					this.ih.StopUIInput();
					base.interactable = false;
					this.myCanvasGroup.blocksRaycasts = false;
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.defeatedBackground, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.defeatedText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.brokenSteelOrb, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.restoreSaveButton.CanvasGroup, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.slotNumberText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.activeSaveSlot, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.backgroundCg, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.clearSaveButton, false, false));
					this.clearSaveButton.blocksRaycasts = false;
					this.parentBlocker.blocksRaycasts = false;
					yield return this.restoreSaveButton.ShowSaveSelection();
				}
			}
			else if (state == SaveSlotButton.SlotState.Corrupted || state == SaveSlotButton.SlotState.Incompatible)
			{
				if (nextState == SaveSlotButton.SlotState.ClearPrompt)
				{
					this.ih.StopUIInput();
					base.interactable = false;
					this.myCanvasGroup.blocksRaycasts = false;
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.slotNumberText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.saveCorruptedText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.saveIncompatibleText, true, false));
					base.StartCoroutine(this.restoreSaveButton.Hide());
					yield return base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.clearSaveButton, false, false));
					this.clearSaveButton.blocksRaycasts = false;
					this.parentBlocker.blocksRaycasts = false;
					yield return base.StartCoroutine(this.ui.FadeInCanvasGroup(this.clearSavePrompt, false));
					this.clearSavePrompt.interactable = true;
					this.clearSavePrompt.blocksRaycasts = true;
					this.clearSavePromptHighlight.HighlightDefault(false);
					base.interactable = false;
					this.myCanvasGroup.blocksRaycasts = false;
					this.ih.StartUIInput();
				}
				else if (nextState == SaveSlotButton.SlotState.Hidden)
				{
					this.ResetFluerTriggersReversed();
					yield return new WaitForSeconds(0.1f);
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.slotNumberText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.saveCorruptedText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.saveIncompatibleText, true, false));
					base.StartCoroutine(this.restoreSaveButton.Hide());
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.clearSaveButton, false, false));
					this.clearSaveButton.blocksRaycasts = false;
				}
				else if (nextState == SaveSlotButton.SlotState.OperationInProgress)
				{
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.slotNumberText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.saveCorruptedText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.saveIncompatibleText, true, false));
					base.StartCoroutine(this.restoreSaveButton.Hide());
					yield return base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.clearSaveButton, false, false));
					base.StartCoroutine(this.currentLoadingTextFadeIn = this.FadeInCanvasGroupAfterDelay(5f, this.loadingText));
				}
				else if (nextState == SaveSlotButton.SlotState.RestoreSave)
				{
					this.ih.StopUIInput();
					base.interactable = false;
					this.myCanvasGroup.blocksRaycasts = false;
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.slotNumberText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.saveCorruptedText, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.saveIncompatibleText, true, false));
					base.StartCoroutine(this.restoreSaveButton.Hide());
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.backgroundCg, true, false));
					base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.clearSaveButton, false, false));
					this.clearSaveButton.blocksRaycasts = false;
					this.parentBlocker.blocksRaycasts = false;
					yield return this.restoreSaveButton.ShowSaveSelection();
				}
			}
			else if (state == SaveSlotButton.SlotState.RestoreSave)
			{
				yield return this.ProcessRestoreSaveState(nextState);
			}
			yield break;
		}

		// Token: 0x06004BE6 RID: 19430 RVA: 0x001661D8 File Offset: 0x001643D8
		private IEnumerator ProcessRestoreSaveState(SaveSlotButton.SlotState nextState)
		{
			switch (nextState)
			{
			case SaveSlotButton.SlotState.Hidden:
				this.ResetFluerTriggersReversed();
				yield return new WaitForSeconds(0.1f);
				base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.slotNumberText, true, false));
				base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.backgroundCg, true, false));
				base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.activeSaveSlot, true, false));
				base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.clearSaveButton, false, false));
				base.StartCoroutine(this.restoreSaveButton.Hide());
				this.clearSaveButton.blocksRaycasts = false;
				base.interactable = true;
				this.myCanvasGroup.blocksRaycasts = true;
				goto IL_595;
			case SaveSlotButton.SlotState.OperationInProgress:
				yield return this.restoreSaveButton.Hide();
				this.ih.StopUIInput();
				base.interactable = true;
				this.parentBlocker.blocksRaycasts = true;
				this.clearSaveButton.blocksRaycasts = false;
				this.myCanvasGroup.blocksRaycasts = true;
				base.StartCoroutine(this.currentLoadingTextFadeIn = this.FadeInCanvasGroupAfterDelay(5f, this.loadingText));
				goto IL_595;
			case SaveSlotButton.SlotState.SavePresent:
				yield return this.restoreSaveButton.Hide();
				this.blackThreadImpactsLeft = 5;
				this.parentBlocker.blocksRaycasts = true;
				this.PresentSaveSlot(this.saveStats);
				base.StartCoroutine(this.ui.FadeInCanvasGroup(this.backgroundCg, false));
				base.StartCoroutine(this.ui.FadeInCanvasGroup(this.activeSaveSlot, false));
				base.StartCoroutine(this.ui.FadeInCanvasGroup(this.slotNumberText, false));
				base.StartCoroutine(this.restoreSaveButton.ShowRestoreSaveButton());
				yield return this.ui.FadeInCanvasGroup(this.clearSaveButton, false);
				this.clearSaveButton.blocksRaycasts = true;
				this.myCanvasGroup.blocksRaycasts = true;
				base.interactable = true;
				this.Select();
				this.ih.StartUIInput();
				goto IL_595;
			case SaveSlotButton.SlotState.Defeated:
				this.ih.StopUIInput();
				base.StartCoroutine(this.ui.FadeOutCanvasGroup(this.backgroundCg, true, false));
				yield return this.restoreSaveButton.Hide();
				this.parentBlocker.blocksRaycasts = true;
				base.StartCoroutine(this.ui.FadeInCanvasGroup(this.defeatedBackground, false));
				base.StartCoroutine(this.ui.FadeInCanvasGroup(this.defeatedText, false));
				base.StartCoroutine(this.ui.FadeInCanvasGroup(this.brokenSteelOrb, false));
				base.StartCoroutine(this.restoreSaveButton.Hide());
				yield return base.StartCoroutine(this.ui.FadeInCanvasGroup(this.clearSaveButton, false));
				this.clearSaveButton.blocksRaycasts = true;
				this.myCanvasGroup.blocksRaycasts = true;
				this.Select();
				this.ih.StartUIInput();
				goto IL_595;
			case SaveSlotButton.SlotState.Corrupted:
				this.ih.StopUIInput();
				yield return this.restoreSaveButton.Hide();
				this.parentBlocker.blocksRaycasts = true;
				base.StartCoroutine(this.ui.FadeInCanvasGroup(this.slotNumberText, false));
				base.StartCoroutine(this.ui.FadeInCanvasGroup(this.saveCorruptedText, false));
				base.StartCoroutine(this.restoreSaveButton.ShowRestoreSaveButton());
				yield return base.StartCoroutine(this.ui.FadeInCanvasGroup(this.clearSaveButton, false));
				base.interactable = true;
				this.clearSaveButton.blocksRaycasts = true;
				this.myCanvasGroup.blocksRaycasts = true;
				this.Select();
				this.ih.StartUIInput();
				goto IL_595;
			case SaveSlotButton.SlotState.Incompatible:
				this.ih.StopUIInput();
				yield return this.restoreSaveButton.Hide();
				this.parentBlocker.blocksRaycasts = true;
				base.StartCoroutine(this.ui.FadeInCanvasGroup(this.slotNumberText, false));
				base.StartCoroutine(this.ui.FadeInCanvasGroup(this.saveIncompatibleText, false));
				base.StartCoroutine(this.restoreSaveButton.ShowRestoreSaveButton());
				yield return base.StartCoroutine(this.ui.FadeInCanvasGroup(this.clearSaveButton, false));
				this.clearSaveButton.blocksRaycasts = true;
				this.myCanvasGroup.blocksRaycasts = true;
				this.Select();
				this.ih.StartUIInput();
				goto IL_595;
			}
			Debug.LogError(string.Format("Unhandled state transition from {0} to {1} detected", SaveSlotButton.SlotState.RestoreSave, nextState), this);
			IL_595:
			yield break;
		}

		// Token: 0x06004BE7 RID: 19431 RVA: 0x001661F0 File Offset: 0x001643F0
		private void SavePresentFadeIn(SaveStats currentSaveStats)
		{
			base.StartCoroutine(this.ui.FadeInCanvasGroup(this.backgroundCg, false));
			base.StartCoroutine(this.ui.FadeInCanvasGroup(this.activeSaveSlot, false));
			if (DemoHelper.IsDemoMode)
			{
				return;
			}
			if (!currentSaveStats.IsBlackThreadInfected || this.blackThreadImpactsLeft <= 0)
			{
				base.StartCoroutine(this.ui.FadeInCanvasGroup(this.slotNumberText, false));
			}
			if (currentSaveStats.IsBlackThreadInfected)
			{
				return;
			}
			base.StartCoroutine(this.restoreSaveButton.ShowRestoreSaveButton());
			base.StartCoroutine(this.ui.FadeInCanvasGroup(this.clearSaveButton, false));
			this.clearSaveButton.blocksRaycasts = true;
		}

		// Token: 0x06004BE8 RID: 19432 RVA: 0x001662A4 File Offset: 0x001644A4
		private void ResetFluerTriggers()
		{
			foreach (Animator animator in this.frameFluers)
			{
				animator.ResetTrigger(SaveSlotButton._hideAnimProp);
				animator.SetTrigger(SaveSlotButton._showAnimProp);
			}
		}

		// Token: 0x06004BE9 RID: 19433 RVA: 0x001662E0 File Offset: 0x001644E0
		private void ResetFluerTriggersReversed()
		{
			foreach (Animator animator in this.frameFluers)
			{
				animator.ResetTrigger(SaveSlotButton._showAnimProp);
				animator.SetTrigger(SaveSlotButton._hideAnimProp);
			}
		}

		// Token: 0x06004BEA RID: 19434 RVA: 0x0016631C File Offset: 0x0016451C
		private void PresentSaveSlot(SaveStats currentSaveStats)
		{
			this.rosaryGroup.SetActive(true);
			this.shardGroup.SetActive(true);
			this.completionText.gameObject.SetActive(true);
			this.background.enabled = true;
			this.blackThreadInfectedGroup.gameObject.SetActive(false);
			if (this.saveSlotCompletionIcons)
			{
				this.saveSlotCompletionIcons.SetCompletionIconState(currentSaveStats);
			}
			SaveSlotBackgrounds.AreaBackground areaBackground = null;
			string text = null;
			if (currentSaveStats.BossRushMode)
			{
				this.healthSlots.ShowHealth(currentSaveStats.MaxHealth, false, currentSaveStats.CrestId);
				this.SetSilkDisplay(currentSaveStats);
				this.playTimeText.text = currentSaveStats.GetPlaytimeHHMM();
				this.rosaryGroup.SetActive(false);
				this.shardGroup.SetActive(false);
				this.completionText.gameObject.SetActive(false);
				areaBackground = this.saveSlots.GetBackground(currentSaveStats);
			}
			else if (currentSaveStats.IsBlackThreadInfected)
			{
				this.healthSlots.gameObject.SetActive(false);
				this.silkBar.gameObject.SetActive(false);
				this.rosaryGroup.gameObject.SetActive(false);
				this.shardGroup.gameObject.SetActive(false);
				this.completionText.gameObject.SetActive(false);
				this.playTimeText.gameObject.SetActive(false);
				this.background.enabled = false;
				this.blackThreadOverlay.SetActive(false);
				if (this.saveSlotCompletionIcons)
				{
					this.saveSlotCompletionIcons.gameObject.SetActive(false);
				}
				if (this.blackThreadImpactsLeft > 0)
				{
					this.locationText.gameObject.SetActive(false);
					this.slotNumberText.gameObject.SetActive(false);
					this.blackThreadInfectedGroup.gameObject.SetActive(true);
					this.blackThreadInfectedGroup.ResetVisibleStrands();
				}
				else
				{
					text = "???";
					this.locationText.gameObject.SetActive(true);
					this.slotNumberText.gameObject.SetActive(true);
					this.blackThreadInfectedGroup.gameObject.SetActive(false);
					this.background.enabled = true;
				}
				areaBackground = this.saveSlots.GetBackground(MapZone.CRADLE);
			}
			else if (currentSaveStats.PermadeathMode == PermadeathModes.Off)
			{
				if (!DemoHelper.IsDemoMode)
				{
					this.healthSlots.ShowHealth(currentSaveStats.MaxHealth, false, currentSaveStats.CrestId);
					this.SetSilkDisplay(currentSaveStats);
					this.rosaryText.text = currentSaveStats.Geo.ToString();
					this.shardText.text = currentSaveStats.Shards.ToString();
					if (currentSaveStats.UnlockedCompletionRate)
					{
						this.completionText.text = currentSaveStats.CompletionPercentage.ToString() + "%";
					}
					else
					{
						this.completionText.gameObject.SetActive(false);
					}
					this.playTimeText.text = currentSaveStats.GetPlaytimeHHMM();
				}
				else
				{
					this.healthSlots.gameObject.SetActive(false);
					this.silkBar.gameObject.SetActive(false);
					this.rosaryGroup.gameObject.SetActive(false);
					this.shardGroup.gameObject.SetActive(false);
					this.completionText.gameObject.SetActive(false);
					this.playTimeText.gameObject.SetActive(false);
					this.slotNumberText.gameObject.SetActive(false);
				}
				areaBackground = this.saveSlots.GetBackground(currentSaveStats);
			}
			else if (currentSaveStats.PermadeathMode == PermadeathModes.On)
			{
				this.healthSlots.ShowHealth(currentSaveStats.MaxHealth, true, currentSaveStats.CrestId);
				this.SetSilkDisplay(currentSaveStats);
				this.rosaryText.text = currentSaveStats.Geo.ToString();
				this.shardText.text = currentSaveStats.Shards.ToString();
				if (currentSaveStats.UnlockedCompletionRate)
				{
					this.completionText.text = currentSaveStats.CompletionPercentage.ToString(CultureInfo.InvariantCulture) + "%";
				}
				else
				{
					this.completionText.gameObject.SetActive(false);
				}
				this.playTimeText.text = currentSaveStats.GetPlaytimeHHMM();
				areaBackground = this.saveSlots.GetBackground(currentSaveStats);
			}
			string text2;
			if (text != null)
			{
				text2 = text;
				if (areaBackground != null)
				{
					if (this.saveStats.IsAct3 && areaBackground.Act3BackgroundImage != null)
					{
						this.background.sprite = areaBackground.Act3BackgroundImage;
					}
					else
					{
						this.background.sprite = areaBackground.BackgroundImage;
					}
				}
			}
			else if (areaBackground != null)
			{
				if (this.saveStats.IsAct3 && areaBackground.Act3BackgroundImage != null)
				{
					this.background.sprite = areaBackground.Act3BackgroundImage;
				}
				else
				{
					this.background.sprite = areaBackground.BackgroundImage;
				}
				text2 = ((!areaBackground.NameOverride.IsEmpty) ? areaBackground.NameOverride : this.gm.GetFormattedMapZoneString(currentSaveStats.MapZone));
			}
			else
			{
				text2 = this.gm.GetFormattedMapZoneString(currentSaveStats.MapZone);
			}
			this.locationText.text = text2.Replace("<br>", Environment.NewLine);
			this.blackThreadOverlay.SetActive(currentSaveStats.IsAct3IntroCompleted && areaBackground != null && !areaBackground.Act3OverlayOptOut);
		}

		// Token: 0x06004BEB RID: 19435 RVA: 0x0016685B File Offset: 0x00164A5B
		private void SetSilkDisplay(SaveStats stats)
		{
			this.silkBar.ShowSilk(stats.IsSpoolBroken, stats.MaxSilk, stats.CrestId == "Cursed");
		}

		// Token: 0x06004BEC RID: 19436 RVA: 0x00166884 File Offset: 0x00164A84
		private void SetupNavs()
		{
			Navigation navigation = base.navigation;
			this.noNav = new Navigation
			{
				mode = Navigation.Mode.Explicit,
				selectOnLeft = navigation.selectOnLeft,
				selectOnRight = navigation.selectOnRight,
				selectOnUp = null,
				selectOnDown = null
			};
			this.emptySlotNav = new Navigation
			{
				mode = Navigation.Mode.Explicit,
				selectOnLeft = navigation.selectOnLeft,
				selectOnRight = navigation.selectOnRight,
				selectOnUp = navigation.selectOnUp,
				selectOnDown = this.backButton
			};
			if (this.restoreSaveButton)
			{
				navigation.selectOnDown = this.restoreSaveButton;
			}
			this.fullSlotNav = navigation;
			navigation.selectOnDown = this.clearSaveButton.GetComponent<Selectable>();
			this.defeatedSlotNav = navigation;
		}

		// Token: 0x06004BED RID: 19437 RVA: 0x00166963 File Offset: 0x00164B63
		private IEnumerator ValidateDeselect()
		{
			this.prevSelectedObject = EventSystem.current.currentSelectedGameObject;
			yield return new WaitForEndOfFrame();
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				this.leftCursor.ResetTrigger(SaveSlotButton._showAnimProp);
				this.rightCursor.ResetTrigger(SaveSlotButton._showAnimProp);
				this.highlight.ResetTrigger(SaveSlotButton._showAnimProp);
				this.leftCursor.SetTrigger(SaveSlotButton._hideAnimProp);
				this.rightCursor.SetTrigger(SaveSlotButton._hideAnimProp);
				this.highlight.SetTrigger(SaveSlotButton._hideAnimProp);
				this.deselectWasForced = false;
			}
			else if (this.deselectWasForced)
			{
				this.leftCursor.ResetTrigger(SaveSlotButton._showAnimProp);
				this.rightCursor.ResetTrigger(SaveSlotButton._showAnimProp);
				this.highlight.ResetTrigger(SaveSlotButton._showAnimProp);
				this.leftCursor.SetTrigger(SaveSlotButton._hideAnimProp);
				this.rightCursor.SetTrigger(SaveSlotButton._hideAnimProp);
				this.highlight.SetTrigger(SaveSlotButton._hideAnimProp);
				this.deselectWasForced = false;
			}
			else
			{
				if (!ManagerSingleton<InputHandler>.Instance.acceptingInput)
				{
					while (!ManagerSingleton<InputHandler>.Instance.acceptingInput)
					{
						yield return null;
					}
				}
				yield return null;
				if (EventSystem.current.currentSelectedGameObject != null)
				{
					this.leftCursor.ResetTrigger(SaveSlotButton._showAnimProp);
					this.rightCursor.ResetTrigger(SaveSlotButton._showAnimProp);
					this.highlight.ResetTrigger(SaveSlotButton._showAnimProp);
					this.leftCursor.SetTrigger(SaveSlotButton._hideAnimProp);
					this.rightCursor.SetTrigger(SaveSlotButton._hideAnimProp);
					this.highlight.SetTrigger(SaveSlotButton._hideAnimProp);
					this.deselectWasForced = false;
				}
				else if (this.deselectWasForced)
				{
					this.leftCursor.ResetTrigger(SaveSlotButton._showAnimProp);
					this.rightCursor.ResetTrigger(SaveSlotButton._showAnimProp);
					this.highlight.ResetTrigger(SaveSlotButton._showAnimProp);
					this.leftCursor.SetTrigger(SaveSlotButton._hideAnimProp);
					this.rightCursor.SetTrigger(SaveSlotButton._hideAnimProp);
					this.highlight.SetTrigger(SaveSlotButton._hideAnimProp);
					this.deselectWasForced = false;
				}
				else if (this.prevSelectedObject != null && this.prevSelectedObject.activeInHierarchy)
				{
					this.deselectWasForced = false;
					this.dontPlaySelectSound = true;
					EventSystem.current.SetSelectedGameObject(this.prevSelectedObject);
				}
			}
			yield break;
		}

		// Token: 0x06004BF0 RID: 19440 RVA: 0x0016699A File Offset: 0x00164B9A
		[CompilerGenerated]
		private void <FadeUpSlotInfoUninfected>g__SetAlpha|95_0(float t)
		{
			this.activeSaveSlot.alpha = t;
			this.slotNumberText.alpha = t;
		}

		// Token: 0x04004D15 RID: 19733
		[Header("SaveSlotButton")]
		public Selectable backButton;

		// Token: 0x04004D16 RID: 19734
		public SaveSlotButton.SaveSlot saveSlot;

		// Token: 0x04004D17 RID: 19735
		[SerializeField]
		private RestoreSaveButton restoreSaveButton;

		// Token: 0x04004D18 RID: 19736
		[Header("Animation")]
		public Animator[] frameFluers;

		// Token: 0x04004D19 RID: 19737
		public Animator highlight;

		// Token: 0x04004D1A RID: 19738
		[SerializeField]
		private Image highlightImage;

		// Token: 0x04004D1B RID: 19739
		[SerializeField]
		private Material highlightMatEmpty;

		// Token: 0x04004D1C RID: 19740
		[SerializeField]
		private Material highlightMatFull;

		// Token: 0x04004D1D RID: 19741
		[SerializeField]
		private BlackThreadStrandGroup blackThreadInfectedGroup;

		// Token: 0x04004D1E RID: 19742
		[SerializeField]
		private Transform blackThreadsParent;

		// Token: 0x04004D1F RID: 19743
		public UnityEvent OnBlackThreadImpact;

		// Token: 0x04004D20 RID: 19744
		public UnityEvent OnBlackThreadBurst;

		// Token: 0x04004D21 RID: 19745
		public UnityEvent BlackThreadOnDisable;

		// Token: 0x04004D22 RID: 19746
		[Header("Canvas Groups")]
		public CanvasGroup newGameText;

		// Token: 0x04004D23 RID: 19747
		public CanvasGroup saveCorruptedText;

		// Token: 0x04004D24 RID: 19748
		public CanvasGroup saveIncompatibleText;

		// Token: 0x04004D25 RID: 19749
		public CanvasGroup loadingText;

		// Token: 0x04004D26 RID: 19750
		public CanvasGroup activeSaveSlot;

		// Token: 0x04004D27 RID: 19751
		public CanvasGroup clearSaveButton;

		// Token: 0x04004D28 RID: 19752
		public CanvasGroup clearSavePrompt;

		// Token: 0x04004D29 RID: 19753
		public CanvasGroup clearSaveConfirmPrompt;

		// Token: 0x04004D2A RID: 19754
		public CanvasGroup backgroundCg;

		// Token: 0x04004D2B RID: 19755
		public CanvasGroup slotNumberText;

		// Token: 0x04004D2C RID: 19756
		public CanvasGroup myCanvasGroup;

		// Token: 0x04004D2D RID: 19757
		public CanvasGroup defeatedText;

		// Token: 0x04004D2E RID: 19758
		public CanvasGroup defeatedBackground;

		// Token: 0x04004D2F RID: 19759
		public CanvasGroup brokenSteelOrb;

		// Token: 0x04004D30 RID: 19760
		public CanvasGroup parentBlocker;

		// Token: 0x04004D31 RID: 19761
		public CanvasGroup controlsGroupBlocker;

		// Token: 0x04004D32 RID: 19762
		[Header("Text Elements")]
		public Text locationText;

		// Token: 0x04004D33 RID: 19763
		public Text playTimeText;

		// Token: 0x04004D34 RID: 19764
		public Text completionText;

		// Token: 0x04004D35 RID: 19765
		[Space]
		public GameObject rosaryGroup;

		// Token: 0x04004D36 RID: 19766
		public Text rosaryText;

		// Token: 0x04004D37 RID: 19767
		public GameObject shardGroup;

		// Token: 0x04004D38 RID: 19768
		public Text shardText;

		// Token: 0x04004D39 RID: 19769
		[Header("Visual Elements")]
		public Image background;

		// Token: 0x04004D3A RID: 19770
		public GameObject blackThreadOverlay;

		// Token: 0x04004D3B RID: 19771
		public SaveProfileHealthBar healthSlots;

		// Token: 0x04004D3C RID: 19772
		public SaveProfileSilkBar silkBar;

		// Token: 0x04004D3D RID: 19773
		public SaveSlotBackgrounds saveSlots;

		// Token: 0x04004D3E RID: 19774
		private GameManager gm;

		// Token: 0x04004D3F RID: 19775
		private UIManager ui;

		// Token: 0x04004D40 RID: 19776
		private InputHandler ih;

		// Token: 0x04004D41 RID: 19777
		public SaveSlotButton.SaveFileStates saveFileState;

		// Token: 0x04004D43 RID: 19779
		private PreselectOption clearSavePromptHighlight;

		// Token: 0x04004D44 RID: 19780
		private PreselectOption clearSaveConfirmPromptHighlight;

		// Token: 0x04004D45 RID: 19781
		[SerializeField]
		private SaveStats saveStats;

		// Token: 0x04004D46 RID: 19782
		[SerializeField]
		private SaveSlotCompletionIcons saveSlotCompletionIcons;

		// Token: 0x04004D47 RID: 19783
		[Header("Debug")]
		[SerializeField]
		private bool fakeCorruptedState;

		// Token: 0x04004D48 RID: 19784
		private BlackThreadStrand[] blackThreads;

		// Token: 0x04004D49 RID: 19785
		private const int BLACK_THREAD_IMPACTS = 5;

		// Token: 0x04004D4A RID: 19786
		private int blackThreadImpactsLeft;

		// Token: 0x04004D4B RID: 19787
		private const float BLACK_THREAD_IMPACT_COOLDOWN = 0.5f;

		// Token: 0x04004D4C RID: 19788
		private int strandsRemovedPerHit;

		// Token: 0x04004D4D RID: 19789
		private double nextImpactTime;

		// Token: 0x04004D4E RID: 19790
		private Navigation noNav;

		// Token: 0x04004D4F RID: 19791
		private Navigation fullSlotNav;

		// Token: 0x04004D50 RID: 19792
		private Navigation emptySlotNav;

		// Token: 0x04004D51 RID: 19793
		private Navigation defeatedSlotNav;

		// Token: 0x04004D52 RID: 19794
		private float clearSaveButtonX;

		// Token: 0x04004D53 RID: 19795
		private IEnumerator currentLoadingTextFadeIn;

		// Token: 0x04004D54 RID: 19796
		private CoroutineQueue coroutineQueue;

		// Token: 0x04004D55 RID: 19797
		private SaveSlotButton.PreloadOperation preloadOperation;

		// Token: 0x04004D56 RID: 19798
		private static readonly int _showAnimProp = Animator.StringToHash("show");

		// Token: 0x04004D57 RID: 19799
		private static readonly int _hideAnimProp = Animator.StringToHash("hide");

		// Token: 0x04004D58 RID: 19800
		private bool isRestoringSave;

		// Token: 0x02001AEF RID: 6895
		public enum SaveFileStates
		{
			// Token: 0x04009AFE RID: 39678
			NotStarted,
			// Token: 0x04009AFF RID: 39679
			OperationInProgress,
			// Token: 0x04009B00 RID: 39680
			Empty,
			// Token: 0x04009B01 RID: 39681
			LoadedStats,
			// Token: 0x04009B02 RID: 39682
			Corrupted,
			// Token: 0x04009B03 RID: 39683
			Incompatible
		}

		// Token: 0x02001AF0 RID: 6896
		public enum SaveSlot
		{
			// Token: 0x04009B05 RID: 39685
			Slot1,
			// Token: 0x04009B06 RID: 39686
			Slot2,
			// Token: 0x04009B07 RID: 39687
			Slot3,
			// Token: 0x04009B08 RID: 39688
			Slot4
		}

		// Token: 0x02001AF1 RID: 6897
		public enum SlotState
		{
			// Token: 0x04009B0A RID: 39690
			Hidden,
			// Token: 0x04009B0B RID: 39691
			OperationInProgress,
			// Token: 0x04009B0C RID: 39692
			EmptySlot,
			// Token: 0x04009B0D RID: 39693
			SavePresent,
			// Token: 0x04009B0E RID: 39694
			Defeated,
			// Token: 0x04009B0F RID: 39695
			Corrupted,
			// Token: 0x04009B10 RID: 39696
			Incompatible,
			// Token: 0x04009B11 RID: 39697
			ClearPrompt,
			// Token: 0x04009B12 RID: 39698
			ClearConfirm,
			// Token: 0x04009B13 RID: 39699
			BlackThreadInfected,
			// Token: 0x04009B14 RID: 39700
			RestoreSave
		}

		// Token: 0x02001AF2 RID: 6898
		private class PreloadOperation
		{
			// Token: 0x17001178 RID: 4472
			// (get) Token: 0x06009883 RID: 39043 RVA: 0x002AD578 File Offset: 0x002AB778
			// (set) Token: 0x06009884 RID: 39044 RVA: 0x002AD5BC File Offset: 0x002AB7BC
			public SaveSlotButton.PreloadOperation.PreloadState State
			{
				get
				{
					object obj = this.stateLock;
					SaveSlotButton.PreloadOperation.PreloadState result;
					lock (obj)
					{
						result = this.state;
					}
					return result;
				}
				private set
				{
					object obj = this.stateLock;
					lock (obj)
					{
						this.state = value;
					}
				}
			}

			// Token: 0x17001179 RID: 4473
			// (get) Token: 0x06009885 RID: 39045 RVA: 0x002AD600 File Offset: 0x002AB800
			// (set) Token: 0x06009886 RID: 39046 RVA: 0x002AD608 File Offset: 0x002AB808
			public bool IsEmpty { get; private set; } = true;

			// Token: 0x1700117A RID: 4474
			// (get) Token: 0x06009887 RID: 39047 RVA: 0x002AD611 File Offset: 0x002AB811
			// (set) Token: 0x06009888 RID: 39048 RVA: 0x002AD619 File Offset: 0x002AB819
			public SaveStats SaveStats { get; private set; }

			// Token: 0x1700117B RID: 4475
			// (get) Token: 0x06009889 RID: 39049 RVA: 0x002AD622 File Offset: 0x002AB822
			// (set) Token: 0x0600988A RID: 39050 RVA: 0x002AD62A File Offset: 0x002AB82A
			public string Message { get; private set; }

			// Token: 0x0600988B RID: 39051 RVA: 0x002AD633 File Offset: 0x002AB833
			public PreloadOperation(int saveSlot, GameManager gm)
			{
				this.SaveSlot = saveSlot;
				this.gm = gm;
				this.GetSaveStatsForSlot();
			}

			// Token: 0x0600988C RID: 39052 RVA: 0x002AD661 File Offset: 0x002AB861
			private void GetSaveStatsForSlot()
			{
				if (this.State != SaveSlotButton.PreloadOperation.PreloadState.NotStarted)
				{
					return;
				}
				this.State = SaveSlotButton.PreloadOperation.PreloadState.Loading;
				this.gm.HasSaveFile(this.SaveSlot, delegate(bool inUse)
				{
					if (this.killed)
					{
						return;
					}
					if (!inUse)
					{
						this.IsEmpty = true;
						this.SetComplete();
						return;
					}
					this.IsEmpty = false;
					this.gm.GetSaveStatsForSlot(this.SaveSlot, delegate(SaveStats stats, string message)
					{
						if (this.killed)
						{
							return;
						}
						this.Message = message;
						if (stats != null)
						{
							this.SaveStats = stats;
						}
						this.SetComplete();
					});
				});
			}

			// Token: 0x0600988D RID: 39053 RVA: 0x002AD690 File Offset: 0x002AB890
			public void WaitForComplete(Action<SaveSlotButton.PreloadOperation.PreloadState> onComplete)
			{
				if (this.killed)
				{
					if (onComplete != null)
					{
						onComplete(SaveSlotButton.PreloadOperation.PreloadState.Complete);
					}
					return;
				}
				object obj = this.stateLock;
				lock (obj)
				{
					if (this.state != SaveSlotButton.PreloadOperation.PreloadState.Complete)
					{
						this.callback = (Action<SaveSlotButton.PreloadOperation.PreloadState>)Delegate.Combine(this.callback, onComplete);
						return;
					}
				}
				if (onComplete != null)
				{
					onComplete(this.State);
				}
			}

			// Token: 0x0600988E RID: 39054 RVA: 0x002AD710 File Offset: 0x002AB910
			private void SetComplete()
			{
				if (this.killed)
				{
					return;
				}
				object obj = this.stateLock;
				lock (obj)
				{
					this.state = SaveSlotButton.PreloadOperation.PreloadState.Complete;
				}
				if (this.callback != null)
				{
					CoreLoop.InvokeSafe(delegate
					{
						this.callback(SaveSlotButton.PreloadOperation.PreloadState.Complete);
					});
				}
			}

			// Token: 0x0600988F RID: 39055 RVA: 0x002AD774 File Offset: 0x002AB974
			public void Kill()
			{
				this.killed = true;
			}

			// Token: 0x04009B15 RID: 39701
			public readonly int SaveSlot;

			// Token: 0x04009B19 RID: 39705
			private SaveSlotButton.PreloadOperation.PreloadState state;

			// Token: 0x04009B1A RID: 39706
			private object stateLock = new object();

			// Token: 0x04009B1B RID: 39707
			private GameManager gm;

			// Token: 0x04009B1C RID: 39708
			private Action<SaveSlotButton.PreloadOperation.PreloadState> callback;

			// Token: 0x04009B1D RID: 39709
			private bool killed;

			// Token: 0x02001C2A RID: 7210
			public enum PreloadState
			{
				// Token: 0x0400A034 RID: 41012
				NotStarted,
				// Token: 0x0400A035 RID: 41013
				Loading,
				// Token: 0x0400A036 RID: 41014
				Complete
			}
		}
	}
}
