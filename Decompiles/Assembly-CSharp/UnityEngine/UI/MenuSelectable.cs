using System;
using System.Collections;
using GlobalEnums;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UnityEngine.UI
{
	// Token: 0x02000878 RID: 2168
	public class MenuSelectable : Selectable, ISelectHandler, IEventSystemHandler, IDeselectHandler, ICancelHandler, IPlaySelectSound
	{
		// Token: 0x140000F2 RID: 242
		// (add) Token: 0x06004B75 RID: 19317 RVA: 0x00164884 File Offset: 0x00162A84
		// (remove) Token: 0x06004B76 RID: 19318 RVA: 0x001648BC File Offset: 0x00162ABC
		public event MenuSelectable.OnSelectedEvent OnSelected;

		// Token: 0x170008ED RID: 2285
		// (get) Token: 0x06004B77 RID: 19319 RVA: 0x001648F1 File Offset: 0x00162AF1
		// (set) Token: 0x06004B78 RID: 19320 RVA: 0x001648F9 File Offset: 0x00162AF9
		public bool DontPlaySelectSound
		{
			get
			{
				return this.dontPlaySelectSound;
			}
			set
			{
				this.dontPlaySelectSound = value;
			}
		}

		// Token: 0x06004B79 RID: 19321 RVA: 0x00164904 File Offset: 0x00162B04
		protected override void Awake()
		{
			base.Awake();
			base.transition = Selectable.Transition.None;
			if (base.navigation.mode != Navigation.Mode.Explicit)
			{
				Navigation navigation = new Navigation
				{
					mode = Navigation.Mode.Explicit
				};
				base.navigation = navigation;
			}
		}

		// Token: 0x06004B7A RID: 19322 RVA: 0x00164948 File Offset: 0x00162B48
		protected override void Start()
		{
			base.Start();
			this.HookUpAudioPlayer();
			this.HookUpEventTrigger();
		}

		// Token: 0x06004B7B RID: 19323 RVA: 0x0016495C File Offset: 0x00162B5C
		protected override void OnEnable()
		{
			base.OnEnable();
			this.CacheCancelEvent();
		}

		// Token: 0x06004B7C RID: 19324 RVA: 0x0016496C File Offset: 0x00162B6C
		public new void OnSelect(BaseEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			if (this.OnSelected != null)
			{
				this.OnSelected(this);
			}
			if (this.leftCursor != null)
			{
				this.leftCursor.ResetTrigger(MenuSelectable._hidePropId);
				this.leftCursor.SetTrigger(MenuSelectable._showPropId);
			}
			if (this.rightCursor != null)
			{
				this.rightCursor.ResetTrigger(MenuSelectable._hidePropId);
				this.rightCursor.SetTrigger(MenuSelectable._showPropId);
			}
			if (this.selectHighlight != null)
			{
				this.selectHighlight.ResetTrigger(MenuSelectable._hidePropId);
				this.selectHighlight.SetTrigger(MenuSelectable._showPropId);
			}
			if (this.descriptionText != null)
			{
				this.descriptionText.ResetTrigger(MenuSelectable._hidePropId);
				this.descriptionText.SetTrigger(MenuSelectable._showPropId);
			}
			if (!this.DontPlaySelectSound)
			{
				try
				{
					this.uiAudioPlayer.PlaySelect();
					return;
				}
				catch (Exception ex)
				{
					string name = base.name;
					string str = " doesn't have a select sound specified. ";
					Exception ex2 = ex;
					Debug.LogError(name + str + ((ex2 != null) ? ex2.ToString() : null));
					return;
				}
			}
			this.dontPlaySelectSound = false;
		}

		// Token: 0x06004B7D RID: 19325 RVA: 0x00164AA0 File Offset: 0x00162CA0
		public new void OnDeselect(BaseEventData eventData)
		{
			base.StartCoroutine(this.ValidateDeselect(eventData));
		}

		// Token: 0x06004B7E RID: 19326 RVA: 0x00164AB0 File Offset: 0x00162CB0
		protected virtual void OnDeselected(BaseEventData eventData)
		{
		}

		// Token: 0x06004B7F RID: 19327 RVA: 0x00164AB2 File Offset: 0x00162CB2
		public void ForceDeselect()
		{
			if (EventSystem.current.currentSelectedGameObject == base.gameObject)
			{
				this.deselectWasForced = true;
				EventSystem.current.SetSelectedGameObject(null);
			}
		}

		// Token: 0x06004B80 RID: 19328 RVA: 0x00164AE0 File Offset: 0x00162CE0
		public void OnCancel(BaseEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			if (this.cancelAction != CancelAction.DoNothing || this.hasCancelEventTrigger)
			{
				this.ForceDeselect();
			}
			if (!this.parentList)
			{
				this.parentList = base.GetComponentInParent<MenuButtonList>();
			}
			if (this.parentList)
			{
				this.parentList.ClearLastSelected();
			}
			UIManager instance = UIManager.instance;
			switch (this.cancelAction)
			{
			case CancelAction.DoNothing:
				break;
			case CancelAction.GoToMainMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToMainMenu();
				}
				break;
			case CancelAction.GoToOptionsMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToOptionsMenu();
				}
				break;
			case CancelAction.GoToVideoMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToVideoMenu(false);
				}
				break;
			case CancelAction.GoToPauseMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToPauseMenu();
				}
				break;
			case CancelAction.LeaveOptionsMenu:
				if (!instance.UIGoBack())
				{
					instance.UILeaveOptionsMenu();
				}
				break;
			case CancelAction.GoToExitPrompt:
				if (!instance.UIGoBack())
				{
					instance.UIShowQuitGamePrompt();
				}
				break;
			case CancelAction.GoToProfileMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToProfileMenu();
				}
				break;
			case CancelAction.GoToControllerMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToControllerMenu();
				}
				break;
			case CancelAction.ApplyRemapGamepadSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyRemapGamepadMenuSettings();
				}
				break;
			case CancelAction.ApplyAudioSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyAudioMenuSettings();
				}
				break;
			case CancelAction.ApplyVideoSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyVideoMenuSettings();
				}
				break;
			case CancelAction.ApplyGameSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyGameMenuSettings();
				}
				break;
			case CancelAction.ApplyKeyboardSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyKeyboardMenuSettings();
				}
				break;
			case CancelAction.GoToExtrasMenu:
				if (ContentPackDetailsUI.Instance)
				{
					ContentPackDetailsUI.Instance.UndoMenuStyle();
				}
				if (!instance.UIGoBack())
				{
					instance.UIGoToExtrasMenu();
				}
				break;
			case CancelAction.ApplyControllerSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyControllerMenuSettings();
				}
				break;
			default:
				Debug.LogError("CancelAction not implemented for this control");
				break;
			}
			if (this.cancelAction != CancelAction.DoNothing || this.hasCancelEventTrigger)
			{
				this.PlayCancelSound();
			}
		}

		// Token: 0x06004B81 RID: 19329 RVA: 0x00164CF2 File Offset: 0x00162EF2
		private IEnumerator ValidateDeselect(BaseEventData eventData)
		{
			this.prevSelectedObject = EventSystem.current.currentSelectedGameObject;
			yield return new WaitForEndOfFrame();
			if (EventSystem.current.currentSelectedGameObject != null || this.deselectWasForced)
			{
				if (this.leftCursor != null)
				{
					this.leftCursor.ResetTrigger(MenuSelectable._showPropId);
					this.leftCursor.SetTrigger(MenuSelectable._hidePropId);
				}
				if (this.rightCursor != null)
				{
					this.rightCursor.ResetTrigger(MenuSelectable._showPropId);
					this.rightCursor.SetTrigger(MenuSelectable._hidePropId);
				}
				if (this.selectHighlight != null)
				{
					this.selectHighlight.ResetTrigger(MenuSelectable._showPropId);
					this.selectHighlight.SetTrigger(MenuSelectable._hidePropId);
				}
				if (this.descriptionText != null)
				{
					this.descriptionText.ResetTrigger(MenuSelectable._showPropId);
					this.descriptionText.SetTrigger(MenuSelectable._hidePropId);
				}
				this.OnDeselected(eventData);
				this.deselectWasForced = false;
			}
			else
			{
				InputHandler ih = ManagerSingleton<InputHandler>.Instance;
				if (ih && !ih.acceptingInput)
				{
					while (!ih.acceptingInput)
					{
						yield return null;
					}
				}
				yield return null;
				if (EventSystem.current.currentSelectedGameObject != null || this.deselectWasForced)
				{
					if (this.leftCursor != null)
					{
						this.leftCursor.ResetTrigger(MenuSelectable._showPropId);
						this.leftCursor.SetTrigger(MenuSelectable._hidePropId);
					}
					if (this.rightCursor != null)
					{
						this.rightCursor.ResetTrigger(MenuSelectable._showPropId);
						this.rightCursor.SetTrigger(MenuSelectable._hidePropId);
					}
					if (this.selectHighlight != null)
					{
						this.selectHighlight.ResetTrigger(MenuSelectable._showPropId);
						this.selectHighlight.SetTrigger(MenuSelectable._hidePropId);
					}
					if (this.descriptionText != null)
					{
						this.descriptionText.ResetTrigger(MenuSelectable._showPropId);
						this.descriptionText.SetTrigger(MenuSelectable._hidePropId);
					}
					this.OnDeselected(eventData);
					this.deselectWasForced = false;
				}
				else if (this.prevSelectedObject != null && this.prevSelectedObject.activeInHierarchy)
				{
					this.deselectWasForced = false;
					this.dontPlaySelectSound = true;
					EventSystem.current.SetSelectedGameObject(this.prevSelectedObject);
				}
				ih = null;
			}
			yield break;
		}

		// Token: 0x06004B82 RID: 19330 RVA: 0x00164D08 File Offset: 0x00162F08
		protected void HookUpAudioPlayer()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			UIManager instance = UIManager.instance;
			this.uiAudioPlayer = ((SceneManager.GetActiveScene().name == "Pre_Menu_Intro") ? Object.FindObjectOfType<MenuAudioController>() : instance.uiAudioPlayer);
			if (instance)
			{
				if (this.menuSubmitVibration == null)
				{
					this.menuSubmitVibration = instance.menuSubmitVibration;
				}
				if (this.menuCancelVibration == null)
				{
					this.menuCancelVibration = instance.menuCancelVibration;
				}
			}
		}

		// Token: 0x06004B83 RID: 19331 RVA: 0x00164D8B File Offset: 0x00162F8B
		protected void HookUpEventTrigger()
		{
			this.eventTrigger = base.GetComponent<EventTrigger>();
			this.CacheCancelEvent();
		}

		// Token: 0x06004B84 RID: 19332 RVA: 0x00164DA0 File Offset: 0x00162FA0
		protected void CacheCancelEvent()
		{
			if (this.eventTrigger)
			{
				this.hasCancelEventTrigger = this.eventTrigger.triggers.Exists((EventTrigger.Entry trigger) => trigger.eventID == EventTriggerType.Cancel);
			}
		}

		// Token: 0x06004B85 RID: 19333 RVA: 0x00164DF0 File Offset: 0x00162FF0
		protected void PlaySubmitSound()
		{
			if (this.playSubmitSound)
			{
				this.uiAudioPlayer.PlaySubmit();
			}
			if (this.menuSubmitVibration)
			{
				VibrationManager.PlayVibrationClipOneShot(this.menuSubmitVibration.VibrationData, null, false, "", false);
			}
		}

		// Token: 0x06004B86 RID: 19334 RVA: 0x00164E40 File Offset: 0x00163040
		protected void PlayCancelSound()
		{
			this.uiAudioPlayer.PlayCancel();
			if (this.menuCancelVibration)
			{
				VibrationManager.PlayVibrationClipOneShot(this.menuCancelVibration.VibrationData, null, false, "", false);
			}
		}

		// Token: 0x06004B87 RID: 19335 RVA: 0x00164E86 File Offset: 0x00163086
		protected void PlaySelectSound()
		{
			this.uiAudioPlayer.PlaySelect();
		}

		// Token: 0x04004CE2 RID: 19682
		[Header("On Cancel")]
		public CancelAction cancelAction;

		// Token: 0x04004CE3 RID: 19683
		[Header("Fleurs")]
		public Animator leftCursor;

		// Token: 0x04004CE4 RID: 19684
		public Animator rightCursor;

		// Token: 0x04004CE5 RID: 19685
		[Header("Highlight")]
		public Animator selectHighlight;

		// Token: 0x04004CE6 RID: 19686
		public bool playSubmitSound = true;

		// Token: 0x04004CE7 RID: 19687
		[Header("Description Text")]
		public Animator descriptionText;

		// Token: 0x04004CE8 RID: 19688
		[Header("Vibrations")]
		public VibrationDataAsset menuSubmitVibration;

		// Token: 0x04004CE9 RID: 19689
		public VibrationDataAsset menuCancelVibration;

		// Token: 0x04004CEA RID: 19690
		protected MenuAudioController uiAudioPlayer;

		// Token: 0x04004CEB RID: 19691
		protected GameObject prevSelectedObject;

		// Token: 0x04004CEC RID: 19692
		protected bool dontPlaySelectSound;

		// Token: 0x04004CED RID: 19693
		protected bool deselectWasForced;

		// Token: 0x04004CEE RID: 19694
		private MenuButtonList parentList;

		// Token: 0x04004CEF RID: 19695
		private static readonly int _showPropId = Animator.StringToHash("show");

		// Token: 0x04004CF0 RID: 19696
		private static readonly int _hidePropId = Animator.StringToHash("hide");

		// Token: 0x04004CF1 RID: 19697
		private EventTrigger eventTrigger;

		// Token: 0x04004CF2 RID: 19698
		private bool hasCancelEventTrigger;

		// Token: 0x04004CF3 RID: 19699
		private Action<bool> customGoBack;

		// Token: 0x02001AE2 RID: 6882
		// (Invoke) Token: 0x06009847 RID: 38983
		public delegate void OnSelectedEvent(MenuSelectable self);
	}
}
