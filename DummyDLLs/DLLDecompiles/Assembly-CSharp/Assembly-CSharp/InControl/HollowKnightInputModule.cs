using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InControl
{
	// Token: 0x02000949 RID: 2377
	[AddComponentMenu("Event/Hollow Knight Input Module")]
	public class HollowKnightInputModule : StandaloneInputModule
	{
		// Token: 0x17000BA6 RID: 2982
		// (get) Token: 0x060054AF RID: 21679 RVA: 0x00181D53 File Offset: 0x0017FF53
		// (set) Token: 0x060054B0 RID: 21680 RVA: 0x00181D5B File Offset: 0x0017FF5B
		public PlayerAction SubmitAction { get; set; }

		// Token: 0x17000BA7 RID: 2983
		// (get) Token: 0x060054B1 RID: 21681 RVA: 0x00181D64 File Offset: 0x0017FF64
		// (set) Token: 0x060054B2 RID: 21682 RVA: 0x00181D6C File Offset: 0x0017FF6C
		public PlayerAction CancelAction { get; set; }

		// Token: 0x17000BA8 RID: 2984
		// (get) Token: 0x060054B3 RID: 21683 RVA: 0x00181D75 File Offset: 0x0017FF75
		// (set) Token: 0x060054B4 RID: 21684 RVA: 0x00181D7D File Offset: 0x0017FF7D
		public PlayerAction JumpAction { get; set; }

		// Token: 0x17000BA9 RID: 2985
		// (get) Token: 0x060054B5 RID: 21685 RVA: 0x00181D86 File Offset: 0x0017FF86
		// (set) Token: 0x060054B6 RID: 21686 RVA: 0x00181D8E File Offset: 0x0017FF8E
		public PlayerAction CastAction { get; set; }

		// Token: 0x17000BAA RID: 2986
		// (get) Token: 0x060054B7 RID: 21687 RVA: 0x00181D97 File Offset: 0x0017FF97
		// (set) Token: 0x060054B8 RID: 21688 RVA: 0x00181D9F File Offset: 0x0017FF9F
		public PlayerAction AttackAction { get; set; }

		// Token: 0x17000BAB RID: 2987
		// (get) Token: 0x060054B9 RID: 21689 RVA: 0x00181DA8 File Offset: 0x0017FFA8
		// (set) Token: 0x060054BA RID: 21690 RVA: 0x00181DB0 File Offset: 0x0017FFB0
		public PlayerTwoAxisAction MoveAction { get; set; }

		// Token: 0x060054BB RID: 21691 RVA: 0x00181DBC File Offset: 0x0017FFBC
		protected HollowKnightInputModule()
		{
			this.direction = new TwoAxisInputControl
			{
				StateThreshold = this.analogMoveThreshold
			};
		}

		// Token: 0x060054BC RID: 21692 RVA: 0x00181E0E File Offset: 0x0018000E
		public override void UpdateModule()
		{
			this.lastMousePosition = this.thisMousePosition;
			this.thisMousePosition = Input.mousePosition;
		}

		// Token: 0x060054BD RID: 21693 RVA: 0x00181E28 File Offset: 0x00180028
		public override bool ShouldActivateModule()
		{
			if (!base.enabled || !base.gameObject.activeInHierarchy)
			{
				return false;
			}
			this.UpdateInputState();
			bool flag = false;
			flag |= this.SubmitAction.WasPressed;
			flag |= this.CancelAction.WasPressed;
			flag |= this.JumpAction.WasPressed;
			flag |= this.CastAction.WasPressed;
			flag |= this.AttackAction.WasPressed;
			flag |= this.VectorWasPressed;
			if (this.allowMouseInput)
			{
				flag |= this.MouseHasMoved;
				flag |= this.MouseButtonIsPressed;
			}
			if (Input.touchCount > 0)
			{
				flag = true;
			}
			return flag;
		}

		// Token: 0x060054BE RID: 21694 RVA: 0x00181EC8 File Offset: 0x001800C8
		public override void ActivateModule()
		{
			base.ActivateModule();
			this.thisMousePosition = Input.mousePosition;
			this.lastMousePosition = Input.mousePosition;
			GameObject gameObject = base.eventSystem.currentSelectedGameObject;
			if (gameObject == null)
			{
				gameObject = base.eventSystem.firstSelectedGameObject;
			}
			base.eventSystem.SetSelectedGameObject(gameObject, this.GetBaseEventData());
		}

		// Token: 0x060054BF RID: 21695 RVA: 0x00181F24 File Offset: 0x00180124
		public override void Process()
		{
			bool flag = base.SendUpdateEventToSelectedObject();
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
			if (this.allowMouseInput && Cursor.visible)
			{
				base.ProcessMouseEvent();
			}
		}

		// Token: 0x060054C0 RID: 21696 RVA: 0x00181F70 File Offset: 0x00180170
		private void SendButtonEventToSelectedObject()
		{
			if (base.eventSystem.currentSelectedGameObject == null)
			{
				return;
			}
			if (UIManager.instance.IsFadingMenu)
			{
				return;
			}
			BaseEventData baseEventData = this.GetBaseEventData();
			Platform.MenuActions menuAction = Platform.Current.GetMenuAction(this.SubmitAction.WasPressed, this.CancelAction.WasPressed, this.JumpAction.WasPressed, this.AttackAction.WasPressed, this.CastAction.WasPressed, false, false, false, false);
			if (menuAction == Platform.MenuActions.Submit)
			{
				ExecuteEvents.Execute<ISubmitHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
				return;
			}
			if (menuAction != Platform.MenuActions.Cancel)
			{
				return;
			}
			PlayerAction playerAction = this.AttackAction.WasPressed ? this.AttackAction : this.CastAction;
			if (!playerAction.WasPressed || playerAction.FindBinding(new MouseBindingSource(Mouse.LeftButton)) == null)
			{
				ExecuteEvents.Execute<ICancelHandler>(base.eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
			}
		}

		// Token: 0x060054C1 RID: 21697 RVA: 0x00182060 File Offset: 0x00180260
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

		// Token: 0x060054C2 RID: 21698 RVA: 0x001820F8 File Offset: 0x001802F8
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

		// Token: 0x060054C3 RID: 21699 RVA: 0x00182142 File Offset: 0x00180342
		private void Update()
		{
			this.direction.Filter(this.Device.Direction, Time.deltaTime);
		}

		// Token: 0x060054C4 RID: 21700 RVA: 0x00182160 File Offset: 0x00180360
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
			if (this.VectorIsReleased)
			{
				this.nextMoveRepeatTime = 0f;
			}
			if (this.VectorIsPressed)
			{
				if (this.lastVectorState == Vector2.zero)
				{
					if (Time.realtimeSinceStartup > this.lastVectorPressedTime + 0.1f)
					{
						this.nextMoveRepeatTime = Time.realtimeSinceStartup + this.moveRepeatFirstDuration;
					}
					else
					{
						this.nextMoveRepeatTime = Time.realtimeSinceStartup + this.moveRepeatDelayDuration;
					}
				}
				this.lastVectorPressedTime = Time.realtimeSinceStartup;
			}
		}

		// Token: 0x17000BAC RID: 2988
		// (get) Token: 0x060054C6 RID: 21702 RVA: 0x00182261 File Offset: 0x00180461
		// (set) Token: 0x060054C5 RID: 21701 RVA: 0x00182258 File Offset: 0x00180458
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

		// Token: 0x060054C7 RID: 21703 RVA: 0x00182272 File Offset: 0x00180472
		private void SetVectorRepeatTimer()
		{
			this.nextMoveRepeatTime = Mathf.Max(this.nextMoveRepeatTime, Time.realtimeSinceStartup + this.moveRepeatDelayDuration);
		}

		// Token: 0x17000BAD RID: 2989
		// (get) Token: 0x060054C8 RID: 21704 RVA: 0x00182291 File Offset: 0x00180491
		private bool VectorIsPressed
		{
			get
			{
				return this.thisVectorState != Vector2.zero;
			}
		}

		// Token: 0x17000BAE RID: 2990
		// (get) Token: 0x060054C9 RID: 21705 RVA: 0x001822A3 File Offset: 0x001804A3
		private bool VectorIsReleased
		{
			get
			{
				return this.thisVectorState == Vector2.zero;
			}
		}

		// Token: 0x17000BAF RID: 2991
		// (get) Token: 0x060054CA RID: 21706 RVA: 0x001822B5 File Offset: 0x001804B5
		private bool VectorHasChanged
		{
			get
			{
				return this.thisVectorState != this.lastVectorState;
			}
		}

		// Token: 0x17000BB0 RID: 2992
		// (get) Token: 0x060054CB RID: 21707 RVA: 0x001822C8 File Offset: 0x001804C8
		private bool VectorWasPressed
		{
			get
			{
				return (this.VectorIsPressed && Time.realtimeSinceStartup > this.nextMoveRepeatTime) || (this.VectorIsPressed && this.lastVectorState == Vector2.zero);
			}
		}

		// Token: 0x17000BB1 RID: 2993
		// (get) Token: 0x060054CC RID: 21708 RVA: 0x001822FC File Offset: 0x001804FC
		private bool MouseHasMoved
		{
			get
			{
				return (this.thisMousePosition - this.lastMousePosition).sqrMagnitude > 0f;
			}
		}

		// Token: 0x17000BB2 RID: 2994
		// (get) Token: 0x060054CD RID: 21709 RVA: 0x00182329 File Offset: 0x00180529
		private bool MouseButtonIsPressed
		{
			get
			{
				return Input.GetMouseButtonDown(0);
			}
		}

		// Token: 0x040053BA RID: 21434
		[Range(0.1f, 0.9f)]
		public float analogMoveThreshold = 0.5f;

		// Token: 0x040053BB RID: 21435
		public float moveRepeatFirstDuration = 0.8f;

		// Token: 0x040053BC RID: 21436
		public float moveRepeatDelayDuration = 0.1f;

		// Token: 0x040053BD RID: 21437
		public bool allowMouseInput = true;

		// Token: 0x040053BE RID: 21438
		public bool focusOnMouseHover;

		// Token: 0x040053BF RID: 21439
		private InputDevice inputDevice;

		// Token: 0x040053C0 RID: 21440
		private Vector3 thisMousePosition;

		// Token: 0x040053C1 RID: 21441
		private Vector3 lastMousePosition;

		// Token: 0x040053C2 RID: 21442
		private Vector2 thisVectorState;

		// Token: 0x040053C3 RID: 21443
		private Vector2 lastVectorState;

		// Token: 0x040053C4 RID: 21444
		private float nextMoveRepeatTime;

		// Token: 0x040053C5 RID: 21445
		private float lastVectorPressedTime;

		// Token: 0x040053C6 RID: 21446
		private readonly TwoAxisInputControl direction;
	}
}
