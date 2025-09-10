using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001115 RID: 4373
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("The eventType will be executed on all components on the GameObject that can handle it.")]
	public class UiEventSystemExecuteEvent : FsmStateAction
	{
		// Token: 0x06007627 RID: 30247 RVA: 0x0024119B File Offset: 0x0023F39B
		public override void Reset()
		{
			this.gameObject = null;
			this.eventHandler = UiEventSystemExecuteEvent.EventHandlers.Submit;
			this.success = null;
			this.canNotHandleEvent = null;
		}

		// Token: 0x06007628 RID: 30248 RVA: 0x002411C3 File Offset: 0x0023F3C3
		public override void OnEnter()
		{
			base.Fsm.Event(this.ExecuteEvent() ? this.success : this.canNotHandleEvent);
			base.Finish();
		}

		// Token: 0x06007629 RID: 30249 RVA: 0x002411EC File Offset: 0x0023F3EC
		private bool ExecuteEvent()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				base.LogError("Missing GameObject ");
				return false;
			}
			switch ((UiEventSystemExecuteEvent.EventHandlers)this.eventHandler.Value)
			{
			case UiEventSystemExecuteEvent.EventHandlers.Submit:
				if (!ExecuteEvents.CanHandleEvent<ISubmitHandler>(this.go))
				{
					return false;
				}
				ExecuteEvents.Execute<ISubmitHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
				break;
			case UiEventSystemExecuteEvent.EventHandlers.beginDrag:
				if (!ExecuteEvents.CanHandleEvent<IBeginDragHandler>(this.go))
				{
					return false;
				}
				ExecuteEvents.Execute<IBeginDragHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.beginDragHandler);
				break;
			case UiEventSystemExecuteEvent.EventHandlers.cancel:
				if (!ExecuteEvents.CanHandleEvent<ICancelHandler>(this.go))
				{
					return false;
				}
				ExecuteEvents.Execute<ICancelHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.cancelHandler);
				break;
			case UiEventSystemExecuteEvent.EventHandlers.deselectHandler:
				if (!ExecuteEvents.CanHandleEvent<IDeselectHandler>(this.go))
				{
					return false;
				}
				ExecuteEvents.Execute<IDeselectHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.deselectHandler);
				break;
			case UiEventSystemExecuteEvent.EventHandlers.dragHandler:
				if (!ExecuteEvents.CanHandleEvent<IDragHandler>(this.go))
				{
					return false;
				}
				ExecuteEvents.Execute<IDragHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.dragHandler);
				break;
			case UiEventSystemExecuteEvent.EventHandlers.dropHandler:
				if (!ExecuteEvents.CanHandleEvent<IDropHandler>(this.go))
				{
					return false;
				}
				ExecuteEvents.Execute<IDropHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.dropHandler);
				break;
			case UiEventSystemExecuteEvent.EventHandlers.endDragHandler:
				if (!ExecuteEvents.CanHandleEvent<IEndDragHandler>(this.go))
				{
					return false;
				}
				ExecuteEvents.Execute<IEndDragHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.endDragHandler);
				break;
			case UiEventSystemExecuteEvent.EventHandlers.initializePotentialDrag:
				if (!ExecuteEvents.CanHandleEvent<IInitializePotentialDragHandler>(this.go))
				{
					return false;
				}
				ExecuteEvents.Execute<IInitializePotentialDragHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.initializePotentialDrag);
				break;
			case UiEventSystemExecuteEvent.EventHandlers.pointerClickHandler:
				if (!ExecuteEvents.CanHandleEvent<IPointerClickHandler>(this.go))
				{
					return false;
				}
				ExecuteEvents.Execute<IPointerClickHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
				break;
			case UiEventSystemExecuteEvent.EventHandlers.pointerDownHandler:
				if (!ExecuteEvents.CanHandleEvent<IPointerDownHandler>(this.go))
				{
					return false;
				}
				ExecuteEvents.Execute<IPointerDownHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
				break;
			case UiEventSystemExecuteEvent.EventHandlers.pointerEnterHandler:
				if (!ExecuteEvents.CanHandleEvent<IPointerEnterHandler>(this.go))
				{
					return false;
				}
				ExecuteEvents.Execute<IPointerEnterHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
				break;
			case UiEventSystemExecuteEvent.EventHandlers.pointerExitHandler:
				if (!ExecuteEvents.CanHandleEvent<IPointerExitHandler>(this.go))
				{
					return false;
				}
				ExecuteEvents.Execute<IPointerExitHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.pointerExitHandler);
				break;
			case UiEventSystemExecuteEvent.EventHandlers.pointerUpHandler:
				if (!ExecuteEvents.CanHandleEvent<IPointerUpHandler>(this.go))
				{
					return false;
				}
				ExecuteEvents.Execute<IPointerUpHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
				break;
			case UiEventSystemExecuteEvent.EventHandlers.scrollHandler:
				if (!ExecuteEvents.CanHandleEvent<IScrollHandler>(this.go))
				{
					return false;
				}
				ExecuteEvents.Execute<IScrollHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.scrollHandler);
				break;
			case UiEventSystemExecuteEvent.EventHandlers.updateSelectedHandler:
				if (!ExecuteEvents.CanHandleEvent<IUpdateSelectedHandler>(this.go))
				{
					return false;
				}
				ExecuteEvents.Execute<IUpdateSelectedHandler>(this.go, new BaseEventData(EventSystem.current), ExecuteEvents.updateSelectedHandler);
				break;
			}
			return true;
		}

		// Token: 0x04007690 RID: 30352
		[RequiredField]
		[Tooltip("The GameObject with  an IEventSystemHandler component (a UI button for example).")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007691 RID: 30353
		[Tooltip("The Type of handler to execute")]
		[ObjectType(typeof(UiEventSystemExecuteEvent.EventHandlers))]
		public FsmEnum eventHandler;

		// Token: 0x04007692 RID: 30354
		[Tooltip("Event Sent if execution was possible on GameObject")]
		public FsmEvent success;

		// Token: 0x04007693 RID: 30355
		[Tooltip("Event Sent if execution was NOT possible on GameObject because it can not handle the eventHandler selected")]
		public FsmEvent canNotHandleEvent;

		// Token: 0x04007694 RID: 30356
		private GameObject go;

		// Token: 0x02001BD2 RID: 7122
		public enum EventHandlers
		{
			// Token: 0x04009EEC RID: 40684
			Submit,
			// Token: 0x04009EED RID: 40685
			beginDrag,
			// Token: 0x04009EEE RID: 40686
			cancel,
			// Token: 0x04009EEF RID: 40687
			deselectHandler,
			// Token: 0x04009EF0 RID: 40688
			dragHandler,
			// Token: 0x04009EF1 RID: 40689
			dropHandler,
			// Token: 0x04009EF2 RID: 40690
			endDragHandler,
			// Token: 0x04009EF3 RID: 40691
			initializePotentialDrag,
			// Token: 0x04009EF4 RID: 40692
			pointerClickHandler,
			// Token: 0x04009EF5 RID: 40693
			pointerDownHandler,
			// Token: 0x04009EF6 RID: 40694
			pointerEnterHandler,
			// Token: 0x04009EF7 RID: 40695
			pointerExitHandler,
			// Token: 0x04009EF8 RID: 40696
			pointerUpHandler,
			// Token: 0x04009EF9 RID: 40697
			scrollHandler,
			// Token: 0x04009EFA RID: 40698
			submitHandler,
			// Token: 0x04009EFB RID: 40699
			updateSelectedHandler
		}
	}
}
