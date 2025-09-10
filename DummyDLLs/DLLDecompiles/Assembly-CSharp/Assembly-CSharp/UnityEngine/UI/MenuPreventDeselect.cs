using System;
using System.Collections;
using GlobalEnums;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x02000875 RID: 2165
	public class MenuPreventDeselect : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler, ICancelHandler, IPlaySelectSound
	{
		// Token: 0x170008EA RID: 2282
		// (get) Token: 0x06004B4D RID: 19277 RVA: 0x00163FE6 File Offset: 0x001621E6
		// (set) Token: 0x06004B4E RID: 19278 RVA: 0x00163FEE File Offset: 0x001621EE
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

		// Token: 0x06004B4F RID: 19279 RVA: 0x00163FF7 File Offset: 0x001621F7
		private void Start()
		{
			this.HookUpAudioPlayer();
		}

		// Token: 0x06004B50 RID: 19280 RVA: 0x00164000 File Offset: 0x00162200
		public void OnSelect(BaseEventData eventData)
		{
			if (this.leftCursor != null)
			{
				this.leftCursor.SetTrigger(MenuPreventDeselect._showProp);
				this.leftCursor.ResetTrigger(MenuPreventDeselect._hideProp);
			}
			if (this.rightCursor != null)
			{
				this.rightCursor.SetTrigger(MenuPreventDeselect._showProp);
				this.rightCursor.ResetTrigger(MenuPreventDeselect._hideProp);
			}
			if (this.playSelectSound && !this.dontPlaySelectSound)
			{
				this.uiAudioPlayer.PlaySelect();
				return;
			}
			this.dontPlaySelectSound = false;
		}

		// Token: 0x06004B51 RID: 19281 RVA: 0x0016408C File Offset: 0x0016228C
		public void OnDeselect(BaseEventData eventData)
		{
			base.StartCoroutine(this.ValidateDeselect());
		}

		// Token: 0x06004B52 RID: 19282 RVA: 0x0016409C File Offset: 0x0016229C
		public void OnCancel(BaseEventData eventData)
		{
			bool flag = true;
			if (this.cancelAction != CancelAction.DoNothing)
			{
				this.ForceDeselect();
			}
			UIManager instance = UIManager.instance;
			switch (this.cancelAction)
			{
			case CancelAction.DoNothing:
				flag = false;
				goto IL_183;
			case CancelAction.GoToMainMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToMainMenu();
					goto IL_183;
				}
				goto IL_183;
			case CancelAction.GoToOptionsMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToOptionsMenu();
					goto IL_183;
				}
				goto IL_183;
			case CancelAction.GoToVideoMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToVideoMenu(false);
					goto IL_183;
				}
				goto IL_183;
			case CancelAction.GoToPauseMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToPauseMenu();
					goto IL_183;
				}
				goto IL_183;
			case CancelAction.LeaveOptionsMenu:
				if (!instance.UIGoBack())
				{
					instance.UILeaveOptionsMenu();
					goto IL_183;
				}
				goto IL_183;
			case CancelAction.GoToExitPrompt:
				if (!instance.UIGoBack())
				{
					instance.UIShowQuitGamePrompt();
					goto IL_183;
				}
				goto IL_183;
			case CancelAction.GoToProfileMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToProfileMenu();
					goto IL_183;
				}
				goto IL_183;
			case CancelAction.GoToControllerMenu:
				if (!instance.UIGoBack())
				{
					instance.UIGoToControllerMenu();
					goto IL_183;
				}
				goto IL_183;
			case CancelAction.ApplyRemapGamepadSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyRemapGamepadMenuSettings();
					goto IL_183;
				}
				goto IL_183;
			case CancelAction.ApplyAudioSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyAudioMenuSettings();
					goto IL_183;
				}
				goto IL_183;
			case CancelAction.ApplyVideoSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyVideoMenuSettings();
					goto IL_183;
				}
				goto IL_183;
			case CancelAction.ApplyGameSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyGameMenuSettings();
					goto IL_183;
				}
				goto IL_183;
			case CancelAction.ApplyKeyboardSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyKeyboardMenuSettings();
					goto IL_183;
				}
				goto IL_183;
			case CancelAction.ApplyControllerSettings:
				if (!instance.UIGoBack())
				{
					instance.ApplyControllerMenuSettings();
					goto IL_183;
				}
				goto IL_183;
			}
			Debug.LogError("CancelAction not implemented for this control");
			flag = false;
			IL_183:
			if (flag)
			{
				this.uiAudioPlayer.PlayCancel();
			}
		}

		// Token: 0x06004B53 RID: 19283 RVA: 0x0016423A File Offset: 0x0016243A
		private IEnumerator ValidateDeselect()
		{
			this.prevSelectedObject = EventSystem.current.currentSelectedGameObject;
			yield return new WaitForEndOfFrame();
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				if (this.leftCursor != null)
				{
					this.leftCursor.ResetTrigger(MenuPreventDeselect._showProp);
					this.leftCursor.SetTrigger(MenuPreventDeselect._hideProp);
				}
				if (this.rightCursor != null)
				{
					this.rightCursor.ResetTrigger(MenuPreventDeselect._showProp);
					this.rightCursor.SetTrigger(MenuPreventDeselect._hideProp);
				}
			}
			else if (this.deselectWasForced)
			{
				if (this.leftCursor != null)
				{
					this.leftCursor.ResetTrigger(MenuPreventDeselect._showProp);
					this.leftCursor.SetTrigger(MenuPreventDeselect._hideProp);
				}
				if (this.rightCursor != null)
				{
					this.rightCursor.ResetTrigger(MenuPreventDeselect._showProp);
					this.rightCursor.SetTrigger(MenuPreventDeselect._hideProp);
				}
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
					if (this.leftCursor != null)
					{
						this.leftCursor.ResetTrigger(MenuPreventDeselect._showProp);
						this.leftCursor.SetTrigger(MenuPreventDeselect._hideProp);
					}
					if (this.rightCursor != null)
					{
						this.rightCursor.ResetTrigger(MenuPreventDeselect._showProp);
						this.rightCursor.SetTrigger(MenuPreventDeselect._hideProp);
					}
				}
				else if (this.deselectWasForced)
				{
					if (this.leftCursor != null)
					{
						this.leftCursor.ResetTrigger(MenuPreventDeselect._showProp);
						this.leftCursor.SetTrigger(MenuPreventDeselect._hideProp);
					}
					if (this.rightCursor != null)
					{
						this.rightCursor.ResetTrigger(MenuPreventDeselect._showProp);
						this.rightCursor.SetTrigger(MenuPreventDeselect._hideProp);
					}
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

		// Token: 0x06004B54 RID: 19284 RVA: 0x00164249 File Offset: 0x00162449
		protected void HookUpAudioPlayer()
		{
			if (this.uiAudioPlayer)
			{
				return;
			}
			this.uiAudioPlayer = UIManager.instance.uiAudioPlayer;
		}

		// Token: 0x06004B55 RID: 19285 RVA: 0x00164269 File Offset: 0x00162469
		public void ForceDeselect()
		{
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				this.deselectWasForced = true;
				EventSystem.current.SetSelectedGameObject(null);
			}
		}

		// Token: 0x06004B56 RID: 19286 RVA: 0x0016428F File Offset: 0x0016248F
		public void SimulateSubmit()
		{
			this.ForceDeselect();
			UIManager.instance.uiAudioPlayer.PlaySubmit();
		}

		// Token: 0x04004CC5 RID: 19653
		[Header("On Cancel")]
		public CancelAction cancelAction;

		// Token: 0x04004CC6 RID: 19654
		[Header("Fleurs")]
		public Animator leftCursor;

		// Token: 0x04004CC7 RID: 19655
		public Animator rightCursor;

		// Token: 0x04004CC8 RID: 19656
		[Space]
		[SerializeField]
		private bool playSelectSound = true;

		// Token: 0x04004CC9 RID: 19657
		private MenuAudioController uiAudioPlayer;

		// Token: 0x04004CCA RID: 19658
		private GameObject prevSelectedObject;

		// Token: 0x04004CCB RID: 19659
		private bool dontPlaySelectSound;

		// Token: 0x04004CCC RID: 19660
		private bool deselectWasForced;

		// Token: 0x04004CCD RID: 19661
		private static readonly int _showProp = Animator.StringToHash("show");

		// Token: 0x04004CCE RID: 19662
		private static readonly int _hideProp = Animator.StringToHash("hide");
	}
}
