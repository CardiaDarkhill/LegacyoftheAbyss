using System;
using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x0200086B RID: 2155
	public class ClearSaveButton : MenuButton, ISubmitHandler, IEventSystemHandler, IPointerClickHandler, ISelectHandler
	{
		// Token: 0x06004AD6 RID: 19158 RVA: 0x00162944 File Offset: 0x00160B44
		public override void OnMove(AxisEventData eventData)
		{
			MoveDirection moveDir = eventData.moveDir;
			if (moveDir == MoveDirection.Left)
			{
				this.Navigate(eventData, this.FindSelectableOnLeft());
				return;
			}
			if (moveDir == MoveDirection.Right)
			{
				this.Navigate(eventData, this.FindSelectableOnRight());
				return;
			}
			base.OnMove(eventData);
		}

		// Token: 0x06004AD7 RID: 19159 RVA: 0x00162984 File Offset: 0x00160B84
		private void Navigate(AxisEventData eventData, Selectable sel)
		{
			if (sel != null)
			{
				if (sel.IsActive() && sel.IsInteractable())
				{
					eventData.selectedObject = sel.gameObject;
					return;
				}
				MoveDirection moveDir = eventData.moveDir;
				if (moveDir == MoveDirection.Left)
				{
					this.Navigate(eventData, sel.FindSelectableOnLeft());
					return;
				}
				if (moveDir == MoveDirection.Right)
				{
					this.Navigate(eventData, sel.FindSelectableOnRight());
					return;
				}
			}
		}

		// Token: 0x06004AD8 RID: 19160 RVA: 0x001629E1 File Offset: 0x00160BE1
		public new void OnSubmit(BaseEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			base.OnSubmit(eventData);
			base.ForceDeselect();
			this.saveSlotButton.ClearSavePrompt();
		}

		// Token: 0x06004AD9 RID: 19161 RVA: 0x00162A04 File Offset: 0x00160C04
		public new void OnPointerClick(PointerEventData eventData)
		{
			this.OnSubmit(eventData);
		}

		// Token: 0x06004ADA RID: 19162 RVA: 0x00162A10 File Offset: 0x00160C10
		public new void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			if (base.GetComponent<CanvasGroup>().interactable)
			{
				if (this.selectIcon)
				{
					this.selectIcon.SetBool(ClearSaveButton._isSelectedProp, true);
					return;
				}
			}
			else
			{
				base.StartCoroutine(this.SelectAfterFrame(base.navigation.selectOnUp.gameObject));
			}
		}

		// Token: 0x06004ADB RID: 19163 RVA: 0x00162A70 File Offset: 0x00160C70
		protected override void OnDeselected(BaseEventData eventData)
		{
			if (this.selectIcon)
			{
				this.selectIcon.SetBool(ClearSaveButton._isSelectedProp, false);
			}
		}

		// Token: 0x06004ADC RID: 19164 RVA: 0x00162A90 File Offset: 0x00160C90
		private IEnumerator SelectAfterFrame(GameObject obj)
		{
			yield return new WaitForEndOfFrame();
			EventSystem.current.SetSelectedGameObject(obj);
			yield break;
		}

		// Token: 0x04004C86 RID: 19590
		[Header("Clear Save Button")]
		[SerializeField]
		private SaveSlotButton saveSlotButton;

		// Token: 0x04004C87 RID: 19591
		[SerializeField]
		private Animator selectIcon;

		// Token: 0x04004C88 RID: 19592
		private static readonly int _isSelectedProp = Animator.StringToHash("Is Selected");
	}
}
