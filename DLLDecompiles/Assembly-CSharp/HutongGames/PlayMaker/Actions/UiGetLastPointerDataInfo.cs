using System;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001116 RID: 4374
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets pointer data on the last System Event.\nHINT: Use {{Hide Unused}} in the {{State Inspector}} to hide the unused parameters after selecting the ones you need.")]
	public class UiGetLastPointerDataInfo : FsmStateAction
	{
		// Token: 0x0600762B RID: 30251 RVA: 0x00241548 File Offset: 0x0023F748
		public override void Reset()
		{
			this.clickCount = null;
			this.clickTime = null;
			this.delta = null;
			this.dragging = null;
			this.inputButton = PointerEventData.InputButton.Left;
			this.eligibleForClick = null;
			this.enterEventCamera = null;
			this.pressEventCamera = null;
			this.isPointerMoving = null;
			this.isScrolling = null;
			this.lastPress = null;
			this.pointerDrag = null;
			this.pointerEnter = null;
			this.pointerId = null;
			this.pointerPress = null;
			this.position = null;
			this.pressPosition = null;
			this.rawPointerPress = null;
			this.scrollDelta = null;
			this.used = null;
			this.useDragThreshold = null;
			this.worldNormal = null;
			this.worldPosition = null;
		}

		// Token: 0x0600762C RID: 30252 RVA: 0x00241600 File Offset: 0x0023F800
		public override void OnEnter()
		{
			if (UiGetLastPointerDataInfo.lastPointerEventData == null)
			{
				base.Finish();
				return;
			}
			if (!this.clickCount.IsNone)
			{
				this.clickCount.Value = UiGetLastPointerDataInfo.lastPointerEventData.clickCount;
			}
			if (!this.clickTime.IsNone)
			{
				this.clickTime.Value = UiGetLastPointerDataInfo.lastPointerEventData.clickTime;
			}
			if (!this.delta.IsNone)
			{
				this.delta.Value = UiGetLastPointerDataInfo.lastPointerEventData.delta;
			}
			if (!this.dragging.IsNone)
			{
				this.dragging.Value = UiGetLastPointerDataInfo.lastPointerEventData.dragging;
			}
			if (!this.inputButton.IsNone)
			{
				this.inputButton.Value = UiGetLastPointerDataInfo.lastPointerEventData.button;
			}
			if (!this.eligibleForClick.IsNone)
			{
				this.eligibleForClick.Value = UiGetLastPointerDataInfo.lastPointerEventData.eligibleForClick;
			}
			if (!this.enterEventCamera.IsNone)
			{
				this.enterEventCamera.Value = UiGetLastPointerDataInfo.lastPointerEventData.enterEventCamera.gameObject;
			}
			if (!this.isPointerMoving.IsNone)
			{
				this.isPointerMoving.Value = UiGetLastPointerDataInfo.lastPointerEventData.IsPointerMoving();
			}
			if (!this.isScrolling.IsNone)
			{
				this.isScrolling.Value = UiGetLastPointerDataInfo.lastPointerEventData.IsScrolling();
			}
			if (!this.lastPress.IsNone)
			{
				this.lastPress.Value = UiGetLastPointerDataInfo.lastPointerEventData.lastPress;
			}
			if (!this.pointerDrag.IsNone)
			{
				this.pointerDrag.Value = UiGetLastPointerDataInfo.lastPointerEventData.pointerDrag;
			}
			if (!this.pointerEnter.IsNone)
			{
				this.pointerEnter.Value = UiGetLastPointerDataInfo.lastPointerEventData.pointerEnter;
			}
			if (!this.pointerId.IsNone)
			{
				this.pointerId.Value = UiGetLastPointerDataInfo.lastPointerEventData.pointerId;
			}
			if (!this.pointerPress.IsNone)
			{
				this.pointerPress.Value = UiGetLastPointerDataInfo.lastPointerEventData.pointerPress;
			}
			if (!this.position.IsNone)
			{
				this.position.Value = UiGetLastPointerDataInfo.lastPointerEventData.position;
			}
			if (!this.pressEventCamera.IsNone)
			{
				this.pressEventCamera.Value = UiGetLastPointerDataInfo.lastPointerEventData.pressEventCamera.gameObject;
			}
			if (!this.pressPosition.IsNone)
			{
				this.pressPosition.Value = UiGetLastPointerDataInfo.lastPointerEventData.pressPosition;
			}
			if (!this.rawPointerPress.IsNone)
			{
				this.rawPointerPress.Value = UiGetLastPointerDataInfo.lastPointerEventData.rawPointerPress;
			}
			if (!this.scrollDelta.IsNone)
			{
				this.scrollDelta.Value = UiGetLastPointerDataInfo.lastPointerEventData.scrollDelta;
			}
			if (!this.used.IsNone)
			{
				this.used.Value = UiGetLastPointerDataInfo.lastPointerEventData.used;
			}
			if (!this.useDragThreshold.IsNone)
			{
				this.useDragThreshold.Value = UiGetLastPointerDataInfo.lastPointerEventData.useDragThreshold;
			}
			if (!this.worldNormal.IsNone)
			{
				this.worldNormal.Value = UiGetLastPointerDataInfo.lastPointerEventData.pointerCurrentRaycast.worldNormal;
			}
			if (!this.worldPosition.IsNone)
			{
				this.worldPosition.Value = UiGetLastPointerDataInfo.lastPointerEventData.pointerCurrentRaycast.worldPosition;
			}
			base.Finish();
		}

		// Token: 0x04007695 RID: 30357
		public static PointerEventData lastPointerEventData;

		// Token: 0x04007696 RID: 30358
		[Tooltip("Number of clicks in a row.")]
		[UIHint(UIHint.Variable)]
		public FsmInt clickCount;

		// Token: 0x04007697 RID: 30359
		[Tooltip("The last time a click event was sent.")]
		[UIHint(UIHint.Variable)]
		public FsmFloat clickTime;

		// Token: 0x04007698 RID: 30360
		[Tooltip("Pointer delta since last update.")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 delta;

		// Token: 0x04007699 RID: 30361
		[Tooltip("Is a drag operation currently occuring.")]
		[UIHint(UIHint.Variable)]
		public FsmBool dragging;

		// Token: 0x0400769A RID: 30362
		[Tooltip("The InputButton for this event.")]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(PointerEventData.InputButton))]
		public FsmEnum inputButton;

		// Token: 0x0400769B RID: 30363
		[Tooltip("Is the pointer being pressed? (Not documented by Unity)")]
		[UIHint(UIHint.Variable)]
		public FsmBool eligibleForClick;

		// Token: 0x0400769C RID: 30364
		[Tooltip("The camera associated with the last OnPointerEnter event.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject enterEventCamera;

		// Token: 0x0400769D RID: 30365
		[Tooltip("The camera associated with the last OnPointerPress event.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject pressEventCamera;

		// Token: 0x0400769E RID: 30366
		[Tooltip("Is the pointer moving.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isPointerMoving;

		// Token: 0x0400769F RID: 30367
		[Tooltip("Is scroll being used on the input device.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isScrolling;

		// Token: 0x040076A0 RID: 30368
		[Tooltip("The GameObject for the last press event.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject lastPress;

		// Token: 0x040076A1 RID: 30369
		[Tooltip("The object that is receiving OnDrag.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject pointerDrag;

		// Token: 0x040076A2 RID: 30370
		[Tooltip("The object that received 'OnPointerEnter'.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject pointerEnter;

		// Token: 0x040076A3 RID: 30371
		[Tooltip("Id of the pointer (touch id).")]
		[UIHint(UIHint.Variable)]
		public FsmInt pointerId;

		// Token: 0x040076A4 RID: 30372
		[Tooltip("The GameObject that received the OnPointerDown.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject pointerPress;

		// Token: 0x040076A5 RID: 30373
		[Tooltip("Current pointer position.")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 position;

		// Token: 0x040076A6 RID: 30374
		[Tooltip("Position of the press.")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 pressPosition;

		// Token: 0x040076A7 RID: 30375
		[Tooltip("The object that the press happened on even if it can not handle the press event.")]
		[UIHint(UIHint.Variable)]
		public FsmGameObject rawPointerPress;

		// Token: 0x040076A8 RID: 30376
		[Tooltip("The amount of scroll since the last update.")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 scrollDelta;

		// Token: 0x040076A9 RID: 30377
		[Tooltip("Is the event used?")]
		[UIHint(UIHint.Variable)]
		public FsmBool used;

		// Token: 0x040076AA RID: 30378
		[Tooltip("Should a drag threshold be used?")]
		[UIHint(UIHint.Variable)]
		public FsmBool useDragThreshold;

		// Token: 0x040076AB RID: 30379
		[Tooltip("The normal of the last raycast in world coordinates.")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 worldNormal;

		// Token: 0x040076AC RID: 30380
		[Tooltip("The world position of the last raycast.")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 worldPosition;
	}
}
