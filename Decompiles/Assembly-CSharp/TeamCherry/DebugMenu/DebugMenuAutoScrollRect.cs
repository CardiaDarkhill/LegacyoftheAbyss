using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TeamCherry.DebugMenu
{
	// Token: 0x020008BA RID: 2234
	[RequireComponent(typeof(ScrollRect))]
	public class DebugMenuAutoScrollRect : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		// Token: 0x06004D77 RID: 19831 RVA: 0x0016BF42 File Offset: 0x0016A142
		private void Start()
		{
			this.scrollRectTransform = base.GetComponent<RectTransform>();
			if (this.contentPanel == null)
			{
				this.contentPanel = base.GetComponent<ScrollRect>().content;
			}
			this.targetPos = this.contentPanel.anchoredPosition;
		}

		// Token: 0x06004D78 RID: 19832 RVA: 0x0016BF80 File Offset: 0x0016A180
		private void Update()
		{
			if (!this._mouseHover)
			{
				this.Autoscroll();
			}
		}

		// Token: 0x06004D79 RID: 19833 RVA: 0x0016BF90 File Offset: 0x0016A190
		public void Autoscroll()
		{
			if (this.contentPanel == null)
			{
				this.contentPanel = base.GetComponent<ScrollRect>().content;
			}
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			if (currentSelectedGameObject == null)
			{
				return;
			}
			if (currentSelectedGameObject.transform.parent != this.contentPanel.transform)
			{
				return;
			}
			if (currentSelectedGameObject == this.lastSelected)
			{
				return;
			}
			this.selectedRectTransform = (RectTransform)currentSelectedGameObject.transform;
			this.targetPos.x = this.contentPanel.anchoredPosition.x;
			this.targetPos.y = -this.selectedRectTransform.localPosition.y - this.selectedRectTransform.rect.height / 2f;
			this.targetPos.y = Mathf.Clamp(this.targetPos.y, 0f, this.contentPanel.sizeDelta.y - this.scrollRectTransform.sizeDelta.y);
			this.contentPanel.anchoredPosition = this.targetPos;
			this.lastSelected = currentSelectedGameObject;
		}

		// Token: 0x06004D7A RID: 19834 RVA: 0x0016C0B9 File Offset: 0x0016A2B9
		public void OnPointerEnter(PointerEventData eventData)
		{
			this._mouseHover = true;
		}

		// Token: 0x06004D7B RID: 19835 RVA: 0x0016C0C2 File Offset: 0x0016A2C2
		public void OnPointerExit(PointerEventData eventData)
		{
			this._mouseHover = false;
		}

		// Token: 0x04004E3B RID: 20027
		private RectTransform scrollRectTransform;

		// Token: 0x04004E3C RID: 20028
		private RectTransform contentPanel;

		// Token: 0x04004E3D RID: 20029
		private RectTransform selectedRectTransform;

		// Token: 0x04004E3E RID: 20030
		private GameObject lastSelected;

		// Token: 0x04004E3F RID: 20031
		private Vector2 targetPos;

		// Token: 0x04004E40 RID: 20032
		private bool _mouseHover;
	}
}
