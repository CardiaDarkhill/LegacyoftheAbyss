using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace InControl
{
	// Token: 0x0200090D RID: 2317
	[AddComponentMenu("Event/InControl Input Module")]
	public class InControlInputModule : StandaloneInputModule
	{
		// Token: 0x17000B1B RID: 2843
		// (get) Token: 0x060051B9 RID: 20921 RVA: 0x001761D2 File Offset: 0x001743D2
		// (set) Token: 0x060051BA RID: 20922 RVA: 0x001761DA File Offset: 0x001743DA
		public PlayerAction SubmitAction { get; set; }

		// Token: 0x17000B1C RID: 2844
		// (get) Token: 0x060051BB RID: 20923 RVA: 0x001761E3 File Offset: 0x001743E3
		// (set) Token: 0x060051BC RID: 20924 RVA: 0x001761EB File Offset: 0x001743EB
		public PlayerAction CancelAction { get; set; }

		// Token: 0x17000B1D RID: 2845
		// (get) Token: 0x060051BD RID: 20925 RVA: 0x001761F4 File Offset: 0x001743F4
		// (set) Token: 0x060051BE RID: 20926 RVA: 0x001761FC File Offset: 0x001743FC
		public PlayerTwoAxisAction MoveAction { get; set; }

		// Token: 0x060051BF RID: 20927 RVA: 0x00176208 File Offset: 0x00174408
		protected InControlInputModule()
		{
			this.direction = new TwoAxisInputControl();
			this.direction.StateThreshold = this.analogMoveThreshold;
		}

		// Token: 0x060051C0 RID: 20928 RVA: 0x00176276 File Offset: 0x00174476
		public override void UpdateModule()
		{
			if (!InputManager.IsSetup)
			{
				return;
			}
			this.lastMousePosition = this.thisMousePosition;
			this.thisMousePosition = InputManager.MouseProvider.GetPosition();
		}

		// Token: 0x060051C1 RID: 20929 RVA: 0x001762A1 File Offset: 0x001744A1
		public override bool IsModuleSupported()
		{
			return InputManager.IsSetup && (this.forceModuleActive || InputManager.MouseProvider.HasMousePresent() || Input.touchSupported);
		}

		// Token: 0x060051C2 RID: 20930 RVA: 0x001762CC File Offset: 0x001744CC
		public override bool ShouldActivateModule()
		{
			if (!InputManager.IsSetup)
			{
				return false;
			}
			if (!base.enabled || !base.gameObject.activeInHierarchy)
			{
				return false;
			}
			this.UpdateInputState();
			bool flag = false;
			flag |= this.SubmitWasPressed;
			flag |= this.CancelWasPressed;
			flag |= this.VectorWasPressed;
			if (this.allowMouseInput)
			{
				flag |= this.MouseHasMoved;
				flag |= InControlInputModule.MouseButtonWasPressed;
			}
			if (this.allowTouchInput)
			{
				flag |= (Input.touchCount > 0);
			}
			return flag;
		}

		// Token: 0x060051C3 RID: 20931 RVA: 0x0017634C File Offset: 0x0017454C
		public override void ActivateModule()
		{
			base.ActivateModule();
			if (InputManager.IsSetup)
			{
				this.thisMousePosition = InputManager.MouseProvider.GetPosition();
			}
			this.lastMousePosition = this.thisMousePosition;
			GameObject gameObject = base.eventSystem.currentSelectedGameObject;
			if (gameObject == null)
			{
				gameObject = base.eventSystem.firstSelectedGameObject;
			}
			base.eventSystem.SetSelectedGameObject(gameObject, this.GetBaseEventData());
		}

		// Token: 0x060051C4 RID: 20932 RVA: 0x001763BC File Offset: 0x001745BC
		public override void Process()
		{
			bool flag = this.SendUpdateEventToSelectedObject();
			if (base.eventSystem.sendNavigationEvents)
			{
				if (!flag)
				{
					flag = this.SendVectorEventToSelectedObject();
				}
				if (!flag)
				{
					this.SendButtonEventToSelectedObject();
				}
			}
			if (this.allowTouchInput && this.ProcessTouchEvents())
			{
				return;
			}
			if (this.allowMouseInput)
			{
				this.ProcessMouseEvent();
			}
		}

		// Token: 0x060051C5 RID: 20933 RVA: 0x00176410 File Offset: 0x00174610
		private bool ProcessTouchEvents()
		{
			int touchCount = Input.touchCount;
			for (int i = 0; i < touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
				if (touch.type != TouchType.Indirect)
				{
					bool pressed;
					bool flag;
					PointerEventData touchPointerEventData = base.GetTouchPointerEventData(touch, out pressed, out flag);
					this.ProcessTouchPress(touchPointerEventData, pressed, flag);
					if (!flag)
					{
						this.ProcessMove(touchPointerEventData);
						this.ProcessDrag(touchPointerEventData);
					}
					else
					{
						base.RemovePointerData(touchPointerEventData);
					}
				}
			}
			return touchCount > 0;
		}

		// Token: 0x060051C6 RID: 20934 RVA: 0x0017647C File Offset: 0x0017467C
		private bool SendButtonEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				return false;
			}
			BaseEventData baseEventData = this.GetBaseEventData();
			if (this.SubmitWasPressed)
			{
				ExecuteEvents.Execute<ISubmitHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
			}
			else
			{
				bool submitWasReleased = this.SubmitWasReleased;
			}
			if (this.CancelWasPressed)
			{
				ExecuteEvents.Execute<ICancelHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
			}
			return baseEventData.used;
		}

		// Token: 0x060051C7 RID: 20935 RVA: 0x001764F4 File Offset: 0x001746F4
		private bool SendVectorEventToSelectedObject()
		{
			if (!this.VectorWasPressed)
			{
				return false;
			}
			AxisEventData axisEventData = this.GetAxisEventData(this.thisVectorState.x, this.thisVectorState.y, 0.5f);
			if (axisEventData.moveDir != MoveDirection.None)
			{
				if (base.eventSystem.currentSelectedGameObject == null)
				{
					base.eventSystem.SetSelectedGameObject(base.eventSystem.firstSelectedGameObject, this.GetBaseEventData());
				}
				else
				{
					ExecuteEvents.Execute<IMoveHandler>(base.eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
				}
				this.SetVectorRepeatTimer();
			}
			return axisEventData.used;
		}

		// Token: 0x060051C8 RID: 20936 RVA: 0x0017658C File Offset: 0x0017478C
		protected override void ProcessMove(PointerEventData pointerEvent)
		{
			GameObject pointerEnter = pointerEvent.pointerEnter;
			base.ProcessMove(pointerEvent);
			if (this.focusOnMouseHover && pointerEnter != pointerEvent.pointerEnter)
			{
				GameObject eventHandler = ExecuteEvents.GetEventHandler<ISelectHandler>(pointerEvent.pointerEnter);
				base.eventSystem.SetSelectedGameObject(eventHandler, pointerEvent);
			}
		}

		// Token: 0x060051C9 RID: 20937 RVA: 0x001765D6 File Offset: 0x001747D6
		private void Update()
		{
			this.direction.Filter(this.Device.Direction, Time.deltaTime);
		}

		// Token: 0x060051CA RID: 20938 RVA: 0x001765F4 File Offset: 0x001747F4
		private void UpdateInputState()
		{
			this.lastVectorState = this.thisVectorState;
			this.thisVectorState = Vector2.zero;
			TwoAxisInputControl twoAxisInputControl = this.MoveAction ?? this.direction;
			if (Utility.AbsoluteIsOverThreshold(twoAxisInputControl.X, this.analogMoveThreshold))
			{
				this.thisVectorState.x = Mathf.Sign(twoAxisInputControl.X);
			}
			if (Utility.AbsoluteIsOverThreshold(twoAxisInputControl.Y, this.analogMoveThreshold))
			{
				this.thisVectorState.y = Mathf.Sign(twoAxisInputControl.Y);
			}
			this.moveWasRepeated = false;
			if (this.VectorIsReleased)
			{
				this.nextMoveRepeatTime = 0f;
			}
			else if (this.VectorIsPressed)
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				if (this.lastVectorState == Vector2.zero)
				{
					this.nextMoveRepeatTime = realtimeSinceStartup + this.moveRepeatFirstDuration;
				}
				else if (realtimeSinceStartup >= this.nextMoveRepeatTime)
				{
					this.moveWasRepeated = true;
					this.nextMoveRepeatTime = realtimeSinceStartup + this.moveRepeatDelayDuration;
				}
			}
			this.lastSubmitState = this.thisSubmitState;
			this.thisSubmitState = ((this.SubmitAction == null) ? this.SubmitButton.IsPressed : this.SubmitAction.IsPressed);
			this.lastCancelState = this.thisCancelState;
			this.thisCancelState = ((this.CancelAction == null) ? this.CancelButton.IsPressed : this.CancelAction.IsPressed);
		}

		// Token: 0x17000B1E RID: 2846
		// (get) Token: 0x060051CC RID: 20940 RVA: 0x00176752 File Offset: 0x00174952
		// (set) Token: 0x060051CB RID: 20939 RVA: 0x00176749 File Offset: 0x00174949
		public InputDevice Device
		{
			get
			{
				return this.inputDevice ?? InputManager.ActiveDevice;
			}
			set
			{
				this.inputDevice = value;
			}
		}

		// Token: 0x17000B1F RID: 2847
		// (get) Token: 0x060051CD RID: 20941 RVA: 0x00176763 File Offset: 0x00174963
		private InputControl SubmitButton
		{
			get
			{
				return this.Device.GetControl((InputControlType)this.submitButton);
			}
		}

		// Token: 0x17000B20 RID: 2848
		// (get) Token: 0x060051CE RID: 20942 RVA: 0x00176776 File Offset: 0x00174976
		private InputControl CancelButton
		{
			get
			{
				return this.Device.GetControl((InputControlType)this.cancelButton);
			}
		}

		// Token: 0x060051CF RID: 20943 RVA: 0x00176789 File Offset: 0x00174989
		private void SetVectorRepeatTimer()
		{
			this.nextMoveRepeatTime = Mathf.Max(this.nextMoveRepeatTime, Time.realtimeSinceStartup + this.moveRepeatDelayDuration);
		}

		// Token: 0x17000B21 RID: 2849
		// (get) Token: 0x060051D0 RID: 20944 RVA: 0x001767A8 File Offset: 0x001749A8
		private bool VectorIsPressed
		{
			get
			{
				return this.thisVectorState != Vector2.zero;
			}
		}

		// Token: 0x17000B22 RID: 2850
		// (get) Token: 0x060051D1 RID: 20945 RVA: 0x001767BA File Offset: 0x001749BA
		private bool VectorIsReleased
		{
			get
			{
				return this.thisVectorState == Vector2.zero;
			}
		}

		// Token: 0x17000B23 RID: 2851
		// (get) Token: 0x060051D2 RID: 20946 RVA: 0x001767CC File Offset: 0x001749CC
		private bool VectorHasChanged
		{
			get
			{
				return this.thisVectorState != this.lastVectorState;
			}
		}

		// Token: 0x17000B24 RID: 2852
		// (get) Token: 0x060051D3 RID: 20947 RVA: 0x001767DF File Offset: 0x001749DF
		private bool VectorWasPressed
		{
			get
			{
				return this.moveWasRepeated || (this.VectorIsPressed && this.lastVectorState == Vector2.zero);
			}
		}

		// Token: 0x17000B25 RID: 2853
		// (get) Token: 0x060051D4 RID: 20948 RVA: 0x00176805 File Offset: 0x00174A05
		private bool SubmitWasPressed
		{
			get
			{
				return this.thisSubmitState && this.thisSubmitState != this.lastSubmitState;
			}
		}

		// Token: 0x17000B26 RID: 2854
		// (get) Token: 0x060051D5 RID: 20949 RVA: 0x00176822 File Offset: 0x00174A22
		private bool SubmitWasReleased
		{
			get
			{
				return !this.thisSubmitState && this.thisSubmitState != this.lastSubmitState;
			}
		}

		// Token: 0x17000B27 RID: 2855
		// (get) Token: 0x060051D6 RID: 20950 RVA: 0x0017683F File Offset: 0x00174A3F
		private bool CancelWasPressed
		{
			get
			{
				return this.thisCancelState && this.thisCancelState != this.lastCancelState;
			}
		}

		// Token: 0x17000B28 RID: 2856
		// (get) Token: 0x060051D7 RID: 20951 RVA: 0x0017685C File Offset: 0x00174A5C
		private bool MouseHasMoved
		{
			get
			{
				return (this.thisMousePosition - this.lastMousePosition).sqrMagnitude > 0f;
			}
		}

		// Token: 0x17000B29 RID: 2857
		// (get) Token: 0x060051D8 RID: 20952 RVA: 0x00176889 File Offset: 0x00174A89
		private static bool MouseButtonWasPressed
		{
			get
			{
				return InputManager.MouseProvider.GetButtonWasPressed(Mouse.LeftButton);
			}
		}

		// Token: 0x060051D9 RID: 20953 RVA: 0x00176898 File Offset: 0x00174A98
		protected new bool SendUpdateEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				return false;
			}
			BaseEventData baseEventData = this.GetBaseEventData();
			ExecuteEvents.Execute<IUpdateSelectedHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
			return baseEventData.used;
		}

		// Token: 0x060051DA RID: 20954 RVA: 0x001768DE File Offset: 0x00174ADE
		protected new void ProcessMouseEvent()
		{
			this.ProcessMouseEvent(0);
		}

		// Token: 0x060051DB RID: 20955 RVA: 0x001768E8 File Offset: 0x00174AE8
		protected new void ProcessMouseEvent(int id)
		{
			PointerInputModule.MouseState mousePointerEventData = this.GetMousePointerEventData(id);
			PointerInputModule.MouseButtonEventData eventData = mousePointerEventData.GetButtonState(PointerEventData.InputButton.Left).eventData;
			this.ProcessMousePress(eventData);
			this.ProcessMove(eventData.buttonData);
			this.ProcessDrag(eventData.buttonData);
			this.ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData);
			this.ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Right).eventData.buttonData);
			this.ProcessMousePress(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData);
			this.ProcessDrag(mousePointerEventData.GetButtonState(PointerEventData.InputButton.Middle).eventData.buttonData);
			if (!Mathf.Approximately(eventData.buttonData.scrollDelta.sqrMagnitude, 0f))
			{
				ExecuteEvents.ExecuteHierarchy<IScrollHandler>(ExecuteEvents.GetEventHandler<IScrollHandler>(eventData.buttonData.pointerCurrentRaycast.gameObject), eventData.buttonData, ExecuteEvents.scrollHandler);
			}
		}

		// Token: 0x060051DC RID: 20956 RVA: 0x001769C4 File Offset: 0x00174BC4
		protected new void ProcessMousePress(PointerInputModule.MouseButtonEventData data)
		{
			PointerEventData buttonData = data.buttonData;
			GameObject gameObject = buttonData.pointerCurrentRaycast.gameObject;
			if (data.PressedThisFrame())
			{
				buttonData.eligibleForClick = true;
				buttonData.delta = Vector2.zero;
				buttonData.dragging = false;
				buttonData.useDragThreshold = true;
				buttonData.pressPosition = buttonData.position;
				buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
				base.DeselectIfSelectionChanged(gameObject, buttonData);
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, buttonData, ExecuteEvents.pointerDownHandler);
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				}
				float unscaledTime = Time.unscaledTime;
				if (gameObject2 == buttonData.lastPress)
				{
					if (unscaledTime - buttonData.clickTime < 0.3f)
					{
						PointerEventData pointerEventData = buttonData;
						int clickCount = pointerEventData.clickCount + 1;
						pointerEventData.clickCount = clickCount;
					}
					else
					{
						buttonData.clickCount = 1;
					}
					buttonData.clickTime = unscaledTime;
				}
				else
				{
					buttonData.clickCount = 1;
				}
				buttonData.pointerPress = gameObject2;
				buttonData.rawPointerPress = gameObject;
				buttonData.clickTime = unscaledTime;
				buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				if (buttonData.pointerDrag != null)
				{
					ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.initializePotentialDrag);
				}
			}
			if (data.ReleasedThisFrame())
			{
				ExecuteEvents.Execute<IPointerUpHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerUpHandler);
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				if (buttonData.pointerPress == eventHandler && buttonData.eligibleForClick)
				{
					ExecuteEvents.Execute<IPointerClickHandler>(buttonData.pointerPress, buttonData, ExecuteEvents.pointerClickHandler);
				}
				else if (buttonData.pointerDrag != null && buttonData.dragging)
				{
					ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, buttonData, ExecuteEvents.dropHandler);
				}
				buttonData.eligibleForClick = false;
				buttonData.pointerPress = null;
				buttonData.rawPointerPress = null;
				if (buttonData.pointerDrag != null && buttonData.dragging)
				{
					ExecuteEvents.Execute<IEndDragHandler>(buttonData.pointerDrag, buttonData, ExecuteEvents.endDragHandler);
				}
				buttonData.dragging = false;
				buttonData.pointerDrag = null;
				if (gameObject != buttonData.pointerEnter)
				{
					base.HandlePointerExitAndEnter(buttonData, null);
					base.HandlePointerExitAndEnter(buttonData, gameObject);
				}
			}
		}

		// Token: 0x060051DD RID: 20957 RVA: 0x00176BC0 File Offset: 0x00174DC0
		protected new void ProcessTouchPress(PointerEventData pointerEvent, bool pressed, bool released)
		{
			GameObject gameObject = pointerEvent.pointerCurrentRaycast.gameObject;
			if (pressed)
			{
				pointerEvent.eligibleForClick = true;
				pointerEvent.delta = Vector2.zero;
				pointerEvent.dragging = false;
				pointerEvent.useDragThreshold = true;
				pointerEvent.pressPosition = pointerEvent.position;
				pointerEvent.pointerPressRaycast = pointerEvent.pointerCurrentRaycast;
				base.DeselectIfSelectionChanged(gameObject, pointerEvent);
				if (pointerEvent.pointerEnter != gameObject)
				{
					base.HandlePointerExitAndEnter(pointerEvent, gameObject);
					pointerEvent.pointerEnter = gameObject;
				}
				GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject, pointerEvent, ExecuteEvents.pointerDownHandler);
				if (gameObject2 == null)
				{
					gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				}
				float unscaledTime = Time.unscaledTime;
				if (gameObject2 == pointerEvent.lastPress)
				{
					if (unscaledTime - pointerEvent.clickTime < 0.3f)
					{
						int clickCount = pointerEvent.clickCount + 1;
						pointerEvent.clickCount = clickCount;
					}
					else
					{
						pointerEvent.clickCount = 1;
					}
					pointerEvent.clickTime = unscaledTime;
				}
				else
				{
					pointerEvent.clickCount = 1;
				}
				pointerEvent.pointerPress = gameObject2;
				pointerEvent.rawPointerPress = gameObject;
				pointerEvent.clickTime = unscaledTime;
				pointerEvent.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject);
				if (pointerEvent.pointerDrag != null)
				{
					ExecuteEvents.Execute<IInitializePotentialDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.initializePotentialDrag);
				}
			}
			if (released)
			{
				ExecuteEvents.Execute<IPointerUpHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerUpHandler);
				GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject);
				if (pointerEvent.pointerPress == eventHandler && pointerEvent.eligibleForClick)
				{
					ExecuteEvents.Execute<IPointerClickHandler>(pointerEvent.pointerPress, pointerEvent, ExecuteEvents.pointerClickHandler);
				}
				else if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
				{
					ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject, pointerEvent, ExecuteEvents.dropHandler);
				}
				pointerEvent.eligibleForClick = false;
				pointerEvent.pointerPress = null;
				pointerEvent.rawPointerPress = null;
				if (pointerEvent.pointerDrag != null && pointerEvent.dragging)
				{
					ExecuteEvents.Execute<IEndDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
				}
				pointerEvent.dragging = false;
				pointerEvent.pointerDrag = null;
				if (pointerEvent.pointerDrag != null)
				{
					ExecuteEvents.Execute<IEndDragHandler>(pointerEvent.pointerDrag, pointerEvent, ExecuteEvents.endDragHandler);
				}
				pointerEvent.pointerDrag = null;
				ExecuteEvents.ExecuteHierarchy<IPointerExitHandler>(pointerEvent.pointerEnter, pointerEvent, ExecuteEvents.pointerExitHandler);
				pointerEvent.pointerEnter = null;
			}
		}

		// Token: 0x0400524F RID: 21071
		public new InControlInputModule.Button submitButton = InControlInputModule.Button.Action1;

		// Token: 0x04005250 RID: 21072
		public new InControlInputModule.Button cancelButton = InControlInputModule.Button.Action2;

		// Token: 0x04005251 RID: 21073
		[Range(0.1f, 0.9f)]
		public float analogMoveThreshold = 0.5f;

		// Token: 0x04005252 RID: 21074
		public float moveRepeatFirstDuration = 0.8f;

		// Token: 0x04005253 RID: 21075
		public float moveRepeatDelayDuration = 0.1f;

		// Token: 0x04005254 RID: 21076
		[FormerlySerializedAs("allowMobileDevice")]
		public new bool forceModuleActive;

		// Token: 0x04005255 RID: 21077
		public bool allowMouseInput = true;

		// Token: 0x04005256 RID: 21078
		public bool focusOnMouseHover;

		// Token: 0x04005257 RID: 21079
		public bool allowTouchInput = true;

		// Token: 0x04005258 RID: 21080
		private InputDevice inputDevice;

		// Token: 0x04005259 RID: 21081
		private Vector3 thisMousePosition;

		// Token: 0x0400525A RID: 21082
		private Vector3 lastMousePosition;

		// Token: 0x0400525B RID: 21083
		private Vector2 thisVectorState;

		// Token: 0x0400525C RID: 21084
		private Vector2 lastVectorState;

		// Token: 0x0400525D RID: 21085
		private bool thisSubmitState;

		// Token: 0x0400525E RID: 21086
		private bool lastSubmitState;

		// Token: 0x0400525F RID: 21087
		private bool thisCancelState;

		// Token: 0x04005260 RID: 21088
		private bool lastCancelState;

		// Token: 0x04005261 RID: 21089
		private bool moveWasRepeated;

		// Token: 0x04005262 RID: 21090
		private float nextMoveRepeatTime;

		// Token: 0x04005263 RID: 21091
		private TwoAxisInputControl direction;

		// Token: 0x02001B5D RID: 7005
		public enum Button
		{
			// Token: 0x04009C7E RID: 40062
			Action1 = 19,
			// Token: 0x04009C7F RID: 40063
			Action2,
			// Token: 0x04009C80 RID: 40064
			Action3,
			// Token: 0x04009C81 RID: 40065
			Action4
		}
	}
}
