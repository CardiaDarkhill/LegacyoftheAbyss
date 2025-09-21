using System;
using System.Collections;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x0200086C RID: 2156
	public class ControllerProfileButton : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler, ISubmitHandler, IPointerClickHandler
	{
		// Token: 0x06004ADF RID: 19167 RVA: 0x00162AB8 File Offset: 0x00160CB8
		public void OnSelect(BaseEventData eventData)
		{
			this.leftCursor.ResetTrigger("hide");
			this.rightCursor.ResetTrigger("hide");
			this.leftCursor.SetTrigger("show");
			this.rightCursor.SetTrigger("show");
			if (!this.dontPlaySelectSound)
			{
				this.uiAudioPlayer.PlaySelect();
				return;
			}
			this.dontPlaySelectSound = false;
		}

		// Token: 0x06004AE0 RID: 19168 RVA: 0x00162B20 File Offset: 0x00160D20
		public void OnDeselect(BaseEventData eventData)
		{
			base.StartCoroutine(this.ValidateDeselect());
		}

		// Token: 0x06004AE1 RID: 19169 RVA: 0x00162B2F File Offset: 0x00160D2F
		public void OnSubmit(BaseEventData eventData)
		{
			this.highlightCursor.gameObject.SetActive(true);
		}

		// Token: 0x06004AE2 RID: 19170 RVA: 0x00162B42 File Offset: 0x00160D42
		public void OnPointerClick(PointerEventData eventData)
		{
			this.OnSubmit(eventData);
		}

		// Token: 0x06004AE3 RID: 19171 RVA: 0x00162B4B File Offset: 0x00160D4B
		private IEnumerator ValidateDeselect()
		{
			this.prevSelectedObject = EventSystem.current.currentSelectedGameObject;
			yield return new WaitForEndOfFrame();
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				this.leftCursor.ResetTrigger("show");
				this.rightCursor.ResetTrigger("show");
				this.leftCursor.SetTrigger("hide");
				this.rightCursor.SetTrigger("hide");
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
					this.leftCursor.ResetTrigger("show");
					this.rightCursor.ResetTrigger("show");
					this.leftCursor.SetTrigger("hide");
					this.rightCursor.SetTrigger("hide");
				}
				else if (this.prevSelectedObject != null && this.prevSelectedObject.activeInHierarchy)
				{
					this.dontPlaySelectSound = true;
					EventSystem.current.SetSelectedGameObject(this.prevSelectedObject);
				}
			}
			yield break;
		}

		// Token: 0x04004C89 RID: 19593
		public Animator leftCursor;

		// Token: 0x04004C8A RID: 19594
		public Animator rightCursor;

		// Token: 0x04004C8B RID: 19595
		public Image highlightCursor;

		// Token: 0x04004C8C RID: 19596
		public MenuAudioController uiAudioPlayer;

		// Token: 0x04004C8D RID: 19597
		private GameObject prevSelectedObject;

		// Token: 0x04004C8E RID: 19598
		private bool dontPlaySelectSound;
	}
}
