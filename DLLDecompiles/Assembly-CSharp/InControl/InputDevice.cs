using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000901 RID: 2305
	public class InputDevice
	{
		// Token: 0x17000ABA RID: 2746
		// (get) Token: 0x060050D2 RID: 20690 RVA: 0x00173B91 File Offset: 0x00171D91
		// (set) Token: 0x060050D3 RID: 20691 RVA: 0x00173B99 File Offset: 0x00171D99
		public int CustomPlayerID { get; set; } = -1;

		// Token: 0x17000ABB RID: 2747
		// (get) Token: 0x060050D4 RID: 20692 RVA: 0x00173BA2 File Offset: 0x00171DA2
		// (set) Token: 0x060050D5 RID: 20693 RVA: 0x00173BAA File Offset: 0x00171DAA
		public string Name { get; protected set; }

		// Token: 0x17000ABC RID: 2748
		// (get) Token: 0x060050D6 RID: 20694 RVA: 0x00173BB3 File Offset: 0x00171DB3
		// (set) Token: 0x060050D7 RID: 20695 RVA: 0x00173BBB File Offset: 0x00171DBB
		public string Meta { get; protected set; }

		// Token: 0x17000ABD RID: 2749
		// (get) Token: 0x060050D8 RID: 20696 RVA: 0x00173BC4 File Offset: 0x00171DC4
		// (set) Token: 0x060050D9 RID: 20697 RVA: 0x00173BCC File Offset: 0x00171DCC
		public int SortOrder { get; protected set; }

		// Token: 0x17000ABE RID: 2750
		// (get) Token: 0x060050DA RID: 20698 RVA: 0x00173BD5 File Offset: 0x00171DD5
		// (set) Token: 0x060050DB RID: 20699 RVA: 0x00173BDD File Offset: 0x00171DDD
		public InputDeviceClass DeviceClass { get; protected set; }

		// Token: 0x17000ABF RID: 2751
		// (get) Token: 0x060050DC RID: 20700 RVA: 0x00173BE6 File Offset: 0x00171DE6
		// (set) Token: 0x060050DD RID: 20701 RVA: 0x00173BEE File Offset: 0x00171DEE
		public InputDeviceStyle DeviceStyle { get; protected set; }

		// Token: 0x17000AC0 RID: 2752
		// (get) Token: 0x060050DE RID: 20702 RVA: 0x00173BF7 File Offset: 0x00171DF7
		// (set) Token: 0x060050DF RID: 20703 RVA: 0x00173BFF File Offset: 0x00171DFF
		public Guid GUID { get; private set; }

		// Token: 0x17000AC1 RID: 2753
		// (get) Token: 0x060050E0 RID: 20704 RVA: 0x00173C08 File Offset: 0x00171E08
		// (set) Token: 0x060050E1 RID: 20705 RVA: 0x00173C10 File Offset: 0x00171E10
		public ulong LastInputTick { get; private set; }

		// Token: 0x17000AC2 RID: 2754
		// (get) Token: 0x060050E2 RID: 20706 RVA: 0x00173C19 File Offset: 0x00171E19
		// (set) Token: 0x060050E3 RID: 20707 RVA: 0x00173C21 File Offset: 0x00171E21
		public bool IsActive { get; private set; }

		// Token: 0x17000AC3 RID: 2755
		// (get) Token: 0x060050E4 RID: 20708 RVA: 0x00173C2A File Offset: 0x00171E2A
		// (set) Token: 0x060050E5 RID: 20709 RVA: 0x00173C32 File Offset: 0x00171E32
		public bool IsAttached { get; private set; }

		// Token: 0x17000AC4 RID: 2756
		// (get) Token: 0x060050E6 RID: 20710 RVA: 0x00173C3B File Offset: 0x00171E3B
		// (set) Token: 0x060050E7 RID: 20711 RVA: 0x00173C43 File Offset: 0x00171E43
		private protected bool RawSticks { protected get; private set; }

		// Token: 0x17000AC5 RID: 2757
		// (get) Token: 0x060050E8 RID: 20712 RVA: 0x00173C4C File Offset: 0x00171E4C
		// (set) Token: 0x060050E9 RID: 20713 RVA: 0x00173C54 File Offset: 0x00171E54
		public ReadOnlyCollection<InputControl> Controls { get; protected set; }

		// Token: 0x17000AC6 RID: 2758
		// (get) Token: 0x060050EA RID: 20714 RVA: 0x00173C5D File Offset: 0x00171E5D
		// (set) Token: 0x060050EB RID: 20715 RVA: 0x00173C65 File Offset: 0x00171E65
		private protected InputControl[] ControlsByTarget { protected get; private set; }

		// Token: 0x17000AC7 RID: 2759
		// (get) Token: 0x060050EC RID: 20716 RVA: 0x00173C6E File Offset: 0x00171E6E
		// (set) Token: 0x060050ED RID: 20717 RVA: 0x00173C76 File Offset: 0x00171E76
		public TwoAxisInputControl LeftStick { get; private set; }

		// Token: 0x17000AC8 RID: 2760
		// (get) Token: 0x060050EE RID: 20718 RVA: 0x00173C7F File Offset: 0x00171E7F
		// (set) Token: 0x060050EF RID: 20719 RVA: 0x00173C87 File Offset: 0x00171E87
		public TwoAxisInputControl RightStick { get; private set; }

		// Token: 0x17000AC9 RID: 2761
		// (get) Token: 0x060050F0 RID: 20720 RVA: 0x00173C90 File Offset: 0x00171E90
		// (set) Token: 0x060050F1 RID: 20721 RVA: 0x00173C98 File Offset: 0x00171E98
		public TwoAxisInputControl DPad { get; private set; }

		// Token: 0x17000ACA RID: 2762
		// (get) Token: 0x060050F2 RID: 20722 RVA: 0x00173CA1 File Offset: 0x00171EA1
		// (set) Token: 0x060050F3 RID: 20723 RVA: 0x00173CA9 File Offset: 0x00171EA9
		public InputControlType LeftCommandControl { get; private set; }

		// Token: 0x17000ACB RID: 2763
		// (get) Token: 0x060050F4 RID: 20724 RVA: 0x00173CB2 File Offset: 0x00171EB2
		// (set) Token: 0x060050F5 RID: 20725 RVA: 0x00173CBA File Offset: 0x00171EBA
		public InputControlType RightCommandControl { get; private set; }

		// Token: 0x17000ACC RID: 2764
		// (get) Token: 0x060050F6 RID: 20726 RVA: 0x00173CC3 File Offset: 0x00171EC3
		// (set) Token: 0x060050F7 RID: 20727 RVA: 0x00173CCB File Offset: 0x00171ECB
		protected InputDevice.AnalogSnapshotEntry[] AnalogSnapshot { get; set; }

		// Token: 0x060050F8 RID: 20728 RVA: 0x00173CD4 File Offset: 0x00171ED4
		public InputDevice() : this("")
		{
		}

		// Token: 0x060050F9 RID: 20729 RVA: 0x00173CE1 File Offset: 0x00171EE1
		public InputDevice(string name) : this(name, false)
		{
		}

		// Token: 0x060050FA RID: 20730 RVA: 0x00173CEC File Offset: 0x00171EEC
		public InputDevice(string name, bool rawSticks)
		{
			this.Name = name;
			this.RawSticks = rawSticks;
			this.Meta = "";
			this.GUID = Guid.NewGuid();
			this.LastInputTick = 0UL;
			this.SortOrder = int.MaxValue;
			this.DeviceClass = InputDeviceClass.Unknown;
			this.DeviceStyle = InputDeviceStyle.Unknown;
			this.Passive = false;
			this.ControlsByTarget = new InputControl[531];
			this.controls = new List<InputControl>(32);
			this.Controls = new ReadOnlyCollection<InputControl>(this.controls);
			this.RemoveAliasControls();
		}

		// Token: 0x060050FB RID: 20731 RVA: 0x00173D86 File Offset: 0x00171F86
		internal void OnAttached()
		{
			this.IsAttached = true;
			this.AddAliasControls();
		}

		// Token: 0x060050FC RID: 20732 RVA: 0x00173D95 File Offset: 0x00171F95
		internal void OnDetached()
		{
			this.IsAttached = false;
			this.StopVibration();
			this.RemoveAliasControls();
		}

		// Token: 0x060050FD RID: 20733 RVA: 0x00173DAC File Offset: 0x00171FAC
		private void AddAliasControls()
		{
			this.RemoveAliasControls();
			if (this.IsKnown)
			{
				this.LeftStick = new TwoAxisInputControl();
				this.RightStick = new TwoAxisInputControl();
				this.DPad = new TwoAxisInputControl();
				this.DPad.DeadZoneFunc = new DeadZoneFunc(DeadZone.Separate);
				this.AddControl(InputControlType.LeftStickX, "Left Stick X");
				this.AddControl(InputControlType.LeftStickY, "Left Stick Y");
				this.AddControl(InputControlType.RightStickX, "Right Stick X");
				this.AddControl(InputControlType.RightStickY, "Right Stick Y");
				this.AddControl(InputControlType.DPadX, "DPad X");
				this.AddControl(InputControlType.DPadY, "DPad Y");
				this.AddControl(InputControlType.Command, "Command");
				this.LeftCommandControl = this.DeviceStyle.LeftCommandControl();
				this.leftCommandSource = this.GetControl(this.LeftCommandControl);
				this.hasLeftCommandControl = !this.leftCommandSource.IsNullControl;
				if (this.hasLeftCommandControl)
				{
					this.AddControl(InputControlType.LeftCommand, this.leftCommandSource.Handle);
				}
				this.RightCommandControl = this.DeviceStyle.RightCommandControl();
				this.rightCommandSource = this.GetControl(this.RightCommandControl);
				this.hasRightCommandControl = !this.rightCommandSource.IsNullControl;
				if (this.hasRightCommandControl)
				{
					this.AddControl(InputControlType.RightCommand, this.rightCommandSource.Handle);
				}
				this.ExpireControlCache();
			}
		}

		// Token: 0x060050FE RID: 20734 RVA: 0x00173F2C File Offset: 0x0017212C
		private void RemoveAliasControls()
		{
			this.LeftStick = TwoAxisInputControl.Null;
			this.RightStick = TwoAxisInputControl.Null;
			this.DPad = TwoAxisInputControl.Null;
			this.RemoveControl(InputControlType.LeftStickX);
			this.RemoveControl(InputControlType.LeftStickY);
			this.RemoveControl(InputControlType.RightStickX);
			this.RemoveControl(InputControlType.RightStickY);
			this.RemoveControl(InputControlType.DPadX);
			this.RemoveControl(InputControlType.DPadY);
			this.RemoveControl(InputControlType.Command);
			this.RemoveControl(InputControlType.LeftCommand);
			this.RemoveControl(InputControlType.RightCommand);
			this.leftCommandSource = null;
			this.hasLeftCommandControl = false;
			this.rightCommandSource = null;
			this.hasRightCommandControl = false;
			this.ExpireControlCache();
		}

		// Token: 0x060050FF RID: 20735 RVA: 0x00173FDF File Offset: 0x001721DF
		protected void ClearControls()
		{
			Array.Clear(this.ControlsByTarget, 0, this.ControlsByTarget.Length);
			this.controls.Clear();
			this.ExpireControlCache();
		}

		// Token: 0x06005100 RID: 20736 RVA: 0x00174006 File Offset: 0x00172206
		public bool HasControl(InputControlType controlType)
		{
			return this.ControlsByTarget[(int)controlType] != null;
		}

		// Token: 0x06005101 RID: 20737 RVA: 0x00174013 File Offset: 0x00172213
		public InputControl GetControl(InputControlType controlType)
		{
			return this.ControlsByTarget[(int)controlType] ?? InputControl.Null;
		}

		// Token: 0x17000ACD RID: 2765
		public InputControl this[InputControlType controlType]
		{
			get
			{
				return this.GetControl(controlType);
			}
		}

		// Token: 0x06005103 RID: 20739 RVA: 0x0017402F File Offset: 0x0017222F
		public static InputControlType GetInputControlTypeByName(string inputControlName)
		{
			return (InputControlType)Enum.Parse(typeof(InputControlType), inputControlName);
		}

		// Token: 0x06005104 RID: 20740 RVA: 0x00174048 File Offset: 0x00172248
		public InputControl GetControlByName(string controlName)
		{
			InputControlType inputControlTypeByName = InputDevice.GetInputControlTypeByName(controlName);
			return this.GetControl(inputControlTypeByName);
		}

		// Token: 0x06005105 RID: 20741 RVA: 0x00174064 File Offset: 0x00172264
		public InputControl AddControl(InputControlType controlType, string handle)
		{
			InputControl inputControl = this.ControlsByTarget[(int)controlType];
			if (inputControl == null)
			{
				inputControl = new InputControl(handle, controlType);
				this.ControlsByTarget[(int)controlType] = inputControl;
				this.controls.Add(inputControl);
				this.ExpireControlCache();
			}
			return inputControl;
		}

		// Token: 0x06005106 RID: 20742 RVA: 0x001740A1 File Offset: 0x001722A1
		public InputControl AddControl(InputControlType controlType, string handle, float lowerDeadZone, float upperDeadZone)
		{
			InputControl inputControl = this.AddControl(controlType, handle);
			inputControl.LowerDeadZone = lowerDeadZone;
			inputControl.UpperDeadZone = upperDeadZone;
			return inputControl;
		}

		// Token: 0x06005107 RID: 20743 RVA: 0x001740BC File Offset: 0x001722BC
		private void RemoveControl(InputControlType controlType)
		{
			InputControl inputControl = this.ControlsByTarget[(int)controlType];
			if (inputControl != null)
			{
				this.ControlsByTarget[(int)controlType] = null;
				this.controls.Remove(inputControl);
				this.ExpireControlCache();
			}
		}

		// Token: 0x06005108 RID: 20744 RVA: 0x001740F4 File Offset: 0x001722F4
		public void ClearInputState()
		{
			this.LeftStick.ClearInputState();
			this.RightStick.ClearInputState();
			this.DPad.ClearInputState();
			int count = this.Controls.Count;
			for (int i = 0; i < count; i++)
			{
				InputControl inputControl = this.Controls[i];
				if (inputControl != null)
				{
					inputControl.ClearInputState();
				}
			}
		}

		// Token: 0x06005109 RID: 20745 RVA: 0x00174150 File Offset: 0x00172350
		protected void UpdateWithState(InputControlType controlType, bool state, ulong updateTick, float deltaTime)
		{
			this.GetControl(controlType).UpdateWithState(state, updateTick, deltaTime);
		}

		// Token: 0x0600510A RID: 20746 RVA: 0x00174163 File Offset: 0x00172363
		protected void UpdateWithValue(InputControlType controlType, float value, ulong updateTick, float deltaTime)
		{
			this.GetControl(controlType).UpdateWithValue(value, updateTick, deltaTime);
		}

		// Token: 0x0600510B RID: 20747 RVA: 0x00174176 File Offset: 0x00172376
		protected void UpdateWithRawValue(InputControlType controlType, float value, ulong updateTick, float deltaTime)
		{
			this.GetControl(controlType).UpdateWithRawValue(value, updateTick, deltaTime);
		}

		// Token: 0x0600510C RID: 20748 RVA: 0x0017418C File Offset: 0x0017238C
		public void UpdateLeftStickWithValue(Vector2 value, ulong updateTick, float deltaTime)
		{
			this.LeftStickLeft.UpdateWithValue(Mathf.Max(0f, -value.x), updateTick, deltaTime);
			this.LeftStickRight.UpdateWithValue(Mathf.Max(0f, value.x), updateTick, deltaTime);
			if (InputManager.InvertYAxis)
			{
				this.LeftStickUp.UpdateWithValue(Mathf.Max(0f, -value.y), updateTick, deltaTime);
				this.LeftStickDown.UpdateWithValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
				return;
			}
			this.LeftStickUp.UpdateWithValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
			this.LeftStickDown.UpdateWithValue(Mathf.Max(0f, -value.y), updateTick, deltaTime);
		}

		// Token: 0x0600510D RID: 20749 RVA: 0x00174258 File Offset: 0x00172458
		public void UpdateLeftStickWithRawValue(Vector2 value, ulong updateTick, float deltaTime)
		{
			this.LeftStickLeft.UpdateWithRawValue(Mathf.Max(0f, -value.x), updateTick, deltaTime);
			this.LeftStickRight.UpdateWithRawValue(Mathf.Max(0f, value.x), updateTick, deltaTime);
			if (InputManager.InvertYAxis)
			{
				this.LeftStickUp.UpdateWithRawValue(Mathf.Max(0f, -value.y), updateTick, deltaTime);
				this.LeftStickDown.UpdateWithRawValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
				return;
			}
			this.LeftStickUp.UpdateWithRawValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
			this.LeftStickDown.UpdateWithRawValue(Mathf.Max(0f, -value.y), updateTick, deltaTime);
		}

		// Token: 0x0600510E RID: 20750 RVA: 0x00174324 File Offset: 0x00172524
		public void CommitLeftStick()
		{
			this.LeftStickUp.Commit();
			this.LeftStickDown.Commit();
			this.LeftStickLeft.Commit();
			this.LeftStickRight.Commit();
		}

		// Token: 0x0600510F RID: 20751 RVA: 0x00174354 File Offset: 0x00172554
		public void UpdateRightStickWithValue(Vector2 value, ulong updateTick, float deltaTime)
		{
			this.RightStickLeft.UpdateWithValue(Mathf.Max(0f, -value.x), updateTick, deltaTime);
			this.RightStickRight.UpdateWithValue(Mathf.Max(0f, value.x), updateTick, deltaTime);
			if (InputManager.InvertYAxis)
			{
				this.RightStickUp.UpdateWithValue(Mathf.Max(0f, -value.y), updateTick, deltaTime);
				this.RightStickDown.UpdateWithValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
				return;
			}
			this.RightStickUp.UpdateWithValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
			this.RightStickDown.UpdateWithValue(Mathf.Max(0f, -value.y), updateTick, deltaTime);
		}

		// Token: 0x06005110 RID: 20752 RVA: 0x00174420 File Offset: 0x00172620
		public void UpdateRightStickWithRawValue(Vector2 value, ulong updateTick, float deltaTime)
		{
			this.RightStickLeft.UpdateWithRawValue(Mathf.Max(0f, -value.x), updateTick, deltaTime);
			this.RightStickRight.UpdateWithRawValue(Mathf.Max(0f, value.x), updateTick, deltaTime);
			if (InputManager.InvertYAxis)
			{
				this.RightStickUp.UpdateWithRawValue(Mathf.Max(0f, -value.y), updateTick, deltaTime);
				this.RightStickDown.UpdateWithRawValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
				return;
			}
			this.RightStickUp.UpdateWithRawValue(Mathf.Max(0f, value.y), updateTick, deltaTime);
			this.RightStickDown.UpdateWithRawValue(Mathf.Max(0f, -value.y), updateTick, deltaTime);
		}

		// Token: 0x06005111 RID: 20753 RVA: 0x001744EC File Offset: 0x001726EC
		public void CommitRightStick()
		{
			this.RightStickUp.Commit();
			this.RightStickDown.Commit();
			this.RightStickLeft.Commit();
			this.RightStickRight.Commit();
		}

		// Token: 0x06005112 RID: 20754 RVA: 0x0017451A File Offset: 0x0017271A
		public virtual void Update(ulong updateTick, float deltaTime)
		{
		}

		// Token: 0x06005113 RID: 20755 RVA: 0x0017451C File Offset: 0x0017271C
		private void ProcessLeftStick(ulong updateTick, float deltaTime)
		{
			float x = Utility.ValueFromSides(this.LeftStickLeft.NextRawValue, this.LeftStickRight.NextRawValue);
			float y = Utility.ValueFromSides(this.LeftStickDown.NextRawValue, this.LeftStickUp.NextRawValue, InputManager.InvertYAxis);
			Vector2 vector;
			if (this.RawSticks || this.LeftStickLeft.Raw || this.LeftStickRight.Raw || this.LeftStickUp.Raw || this.LeftStickDown.Raw)
			{
				vector = new Vector2(x, y);
			}
			else
			{
				float lowerDeadZone = Utility.Max(this.LeftStickLeft.LowerDeadZone, this.LeftStickRight.LowerDeadZone, this.LeftStickUp.LowerDeadZone, this.LeftStickDown.LowerDeadZone);
				float upperDeadZone = Utility.Min(this.LeftStickLeft.UpperDeadZone, this.LeftStickRight.UpperDeadZone, this.LeftStickUp.UpperDeadZone, this.LeftStickDown.UpperDeadZone);
				vector = this.LeftStick.DeadZoneFunc(x, y, lowerDeadZone, upperDeadZone);
			}
			this.LeftStick.Raw = true;
			this.LeftStick.UpdateWithAxes(vector.x, vector.y, updateTick, deltaTime);
			this.LeftStickX.Raw = true;
			this.LeftStickX.CommitWithValue(vector.x, updateTick, deltaTime);
			this.LeftStickY.Raw = true;
			this.LeftStickY.CommitWithValue(vector.y, updateTick, deltaTime);
			this.LeftStickLeft.SetValue(this.LeftStick.Left.Value, updateTick);
			this.LeftStickRight.SetValue(this.LeftStick.Right.Value, updateTick);
			this.LeftStickUp.SetValue(this.LeftStick.Up.Value, updateTick);
			this.LeftStickDown.SetValue(this.LeftStick.Down.Value, updateTick);
		}

		// Token: 0x06005114 RID: 20756 RVA: 0x001746FC File Offset: 0x001728FC
		private void ProcessRightStick(ulong updateTick, float deltaTime)
		{
			float x = Utility.ValueFromSides(this.RightStickLeft.NextRawValue, this.RightStickRight.NextRawValue);
			float y = Utility.ValueFromSides(this.RightStickDown.NextRawValue, this.RightStickUp.NextRawValue, InputManager.InvertYAxis);
			Vector2 vector;
			if (this.RawSticks || this.RightStickLeft.Raw || this.RightStickRight.Raw || this.RightStickUp.Raw || this.RightStickDown.Raw)
			{
				vector = new Vector2(x, y);
			}
			else
			{
				float lowerDeadZone = Utility.Max(this.RightStickLeft.LowerDeadZone, this.RightStickRight.LowerDeadZone, this.RightStickUp.LowerDeadZone, this.RightStickDown.LowerDeadZone);
				float upperDeadZone = Utility.Min(this.RightStickLeft.UpperDeadZone, this.RightStickRight.UpperDeadZone, this.RightStickUp.UpperDeadZone, this.RightStickDown.UpperDeadZone);
				vector = this.RightStick.DeadZoneFunc(x, y, lowerDeadZone, upperDeadZone);
			}
			this.RightStick.Raw = true;
			this.RightStick.UpdateWithAxes(vector.x, vector.y, updateTick, deltaTime);
			this.RightStickX.Raw = true;
			this.RightStickX.CommitWithValue(vector.x, updateTick, deltaTime);
			this.RightStickY.Raw = true;
			this.RightStickY.CommitWithValue(vector.y, updateTick, deltaTime);
			this.RightStickLeft.SetValue(this.RightStick.Left.Value, updateTick);
			this.RightStickRight.SetValue(this.RightStick.Right.Value, updateTick);
			this.RightStickUp.SetValue(this.RightStick.Up.Value, updateTick);
			this.RightStickDown.SetValue(this.RightStick.Down.Value, updateTick);
		}

		// Token: 0x06005115 RID: 20757 RVA: 0x001748DC File Offset: 0x00172ADC
		private void ProcessDPad(ulong updateTick, float deltaTime)
		{
			float x = Utility.ValueFromSides(this.DPadLeft.NextRawValue, this.DPadRight.NextRawValue);
			float y = Utility.ValueFromSides(this.DPadDown.NextRawValue, this.DPadUp.NextRawValue, InputManager.InvertYAxis);
			Vector2 vector;
			if (this.RawSticks || this.DPadLeft.Raw || this.DPadRight.Raw || this.DPadUp.Raw || this.DPadDown.Raw)
			{
				vector = new Vector2(x, y);
			}
			else
			{
				float lowerDeadZone = Utility.Max(this.DPadLeft.LowerDeadZone, this.DPadRight.LowerDeadZone, this.DPadUp.LowerDeadZone, this.DPadDown.LowerDeadZone);
				float upperDeadZone = Utility.Min(this.DPadLeft.UpperDeadZone, this.DPadRight.UpperDeadZone, this.DPadUp.UpperDeadZone, this.DPadDown.UpperDeadZone);
				vector = this.DPad.DeadZoneFunc(x, y, lowerDeadZone, upperDeadZone);
			}
			this.DPad.Raw = true;
			this.DPad.UpdateWithAxes(vector.x, vector.y, updateTick, deltaTime);
			this.DPadX.Raw = true;
			this.DPadX.CommitWithValue(vector.x, updateTick, deltaTime);
			this.DPadY.Raw = true;
			this.DPadY.CommitWithValue(vector.y, updateTick, deltaTime);
			this.DPadLeft.SetValue(this.DPad.Left.Value, updateTick);
			this.DPadRight.SetValue(this.DPad.Right.Value, updateTick);
			this.DPadUp.SetValue(this.DPad.Up.Value, updateTick);
			this.DPadDown.SetValue(this.DPad.Down.Value, updateTick);
		}

		// Token: 0x06005116 RID: 20758 RVA: 0x00174ABC File Offset: 0x00172CBC
		public void Commit(ulong updateTick, float deltaTime)
		{
			if (this.IsKnown)
			{
				this.ProcessLeftStick(updateTick, deltaTime);
				this.ProcessRightStick(updateTick, deltaTime);
				this.ProcessDPad(updateTick, deltaTime);
			}
			int count = this.Controls.Count;
			for (int i = 0; i < count; i++)
			{
				InputControl inputControl = this.Controls[i];
				if (inputControl != null)
				{
					inputControl.Commit();
				}
			}
			if (this.IsKnown)
			{
				bool passive = true;
				bool state = false;
				for (int j = 100; j <= 116; j++)
				{
					InputControl inputControl2 = this.ControlsByTarget[j];
					if (inputControl2 != null && inputControl2.IsPressed)
					{
						state = true;
						if (!inputControl2.Passive)
						{
							passive = false;
						}
					}
				}
				this.Command.Passive = passive;
				this.Command.CommitWithState(state, updateTick, deltaTime);
				if (this.hasLeftCommandControl)
				{
					this.LeftCommand.Passive = this.leftCommandSource.Passive;
					this.LeftCommand.CommitWithState(this.leftCommandSource.IsPressed, updateTick, deltaTime);
				}
				if (this.hasRightCommandControl)
				{
					this.RightCommand.Passive = this.rightCommandSource.Passive;
					this.RightCommand.CommitWithState(this.rightCommandSource.IsPressed, updateTick, deltaTime);
				}
			}
			this.IsActive = false;
			for (int k = 0; k < count; k++)
			{
				InputControl inputControl3 = this.Controls[k];
				if (inputControl3 != null && inputControl3.HasInput && !inputControl3.Passive)
				{
					this.LastInputTick = updateTick;
					this.IsActive = true;
				}
			}
		}

		// Token: 0x06005117 RID: 20759 RVA: 0x00174C30 File Offset: 0x00172E30
		public bool LastInputAfter(InputDevice device)
		{
			return device == null || this.LastInputTick > device.LastInputTick;
		}

		// Token: 0x06005118 RID: 20760 RVA: 0x00174C45 File Offset: 0x00172E45
		public void RequestActivation()
		{
			this.LastInputTick = InputManager.CurrentTick;
			this.IsActive = true;
		}

		// Token: 0x06005119 RID: 20761 RVA: 0x00174C59 File Offset: 0x00172E59
		public virtual void Vibrate(float leftSpeed, float rightSpeed)
		{
		}

		// Token: 0x0600511A RID: 20762 RVA: 0x00174C5B File Offset: 0x00172E5B
		public void Vibrate(float intensity)
		{
			this.Vibrate(intensity, intensity);
		}

		// Token: 0x0600511B RID: 20763 RVA: 0x00174C65 File Offset: 0x00172E65
		public virtual void VibrateTriggers(float leftTriggerSpeed, float rightTriggerSpeed)
		{
		}

		// Token: 0x0600511C RID: 20764 RVA: 0x00174C67 File Offset: 0x00172E67
		public void StopVibration()
		{
			this.Vibrate(0f);
		}

		// Token: 0x0600511D RID: 20765 RVA: 0x00174C74 File Offset: 0x00172E74
		public virtual void SetLightColor(float red, float green, float blue)
		{
		}

		// Token: 0x0600511E RID: 20766 RVA: 0x00174C76 File Offset: 0x00172E76
		public void SetLightColor(Color color)
		{
			this.SetLightColor(color.r * color.a, color.g * color.a, color.b * color.a);
		}

		// Token: 0x0600511F RID: 20767 RVA: 0x00174CA5 File Offset: 0x00172EA5
		public virtual void SetLightFlash(float flashOnDuration, float flashOffDuration)
		{
		}

		// Token: 0x06005120 RID: 20768 RVA: 0x00174CA7 File Offset: 0x00172EA7
		public void StopLightFlash()
		{
			this.SetLightFlash(1f, 0f);
		}

		// Token: 0x17000ACE RID: 2766
		// (get) Token: 0x06005121 RID: 20769 RVA: 0x00174CB9 File Offset: 0x00172EB9
		public virtual bool IsSupportedOnThisPlatform
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000ACF RID: 2767
		// (get) Token: 0x06005122 RID: 20770 RVA: 0x00174CBC File Offset: 0x00172EBC
		public virtual bool IsKnown
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000AD0 RID: 2768
		// (get) Token: 0x06005123 RID: 20771 RVA: 0x00174CBF File Offset: 0x00172EBF
		public bool IsUnknown
		{
			get
			{
				return !this.IsKnown;
			}
		}

		// Token: 0x17000AD1 RID: 2769
		// (get) Token: 0x06005124 RID: 20772 RVA: 0x00174CCA File Offset: 0x00172ECA
		[Obsolete("Use InputDevice.CommandIsPressed instead.", false)]
		public bool MenuIsPressed
		{
			get
			{
				return this.IsKnown && this.Command.IsPressed;
			}
		}

		// Token: 0x17000AD2 RID: 2770
		// (get) Token: 0x06005125 RID: 20773 RVA: 0x00174CE1 File Offset: 0x00172EE1
		[Obsolete("Use InputDevice.CommandWasPressed instead.", false)]
		public bool MenuWasPressed
		{
			get
			{
				return this.IsKnown && this.Command.WasPressed;
			}
		}

		// Token: 0x17000AD3 RID: 2771
		// (get) Token: 0x06005126 RID: 20774 RVA: 0x00174CF8 File Offset: 0x00172EF8
		[Obsolete("Use InputDevice.CommandWasReleased instead.", false)]
		public bool MenuWasReleased
		{
			get
			{
				return this.IsKnown && this.Command.WasReleased;
			}
		}

		// Token: 0x17000AD4 RID: 2772
		// (get) Token: 0x06005127 RID: 20775 RVA: 0x00174D0F File Offset: 0x00172F0F
		public bool CommandIsPressed
		{
			get
			{
				return this.IsKnown && this.Command.IsPressed;
			}
		}

		// Token: 0x17000AD5 RID: 2773
		// (get) Token: 0x06005128 RID: 20776 RVA: 0x00174D26 File Offset: 0x00172F26
		public bool CommandWasPressed
		{
			get
			{
				return this.IsKnown && this.Command.WasPressed;
			}
		}

		// Token: 0x17000AD6 RID: 2774
		// (get) Token: 0x06005129 RID: 20777 RVA: 0x00174D3D File Offset: 0x00172F3D
		public bool CommandWasReleased
		{
			get
			{
				return this.IsKnown && this.Command.WasReleased;
			}
		}

		// Token: 0x17000AD7 RID: 2775
		// (get) Token: 0x0600512A RID: 20778 RVA: 0x00174D54 File Offset: 0x00172F54
		public InputControl AnyButton
		{
			get
			{
				int count = this.Controls.Count;
				for (int i = 0; i < count; i++)
				{
					InputControl inputControl = this.Controls[i];
					if (inputControl != null && inputControl.IsButton && inputControl.IsPressed)
					{
						return inputControl;
					}
				}
				return InputControl.Null;
			}
		}

		// Token: 0x17000AD8 RID: 2776
		// (get) Token: 0x0600512B RID: 20779 RVA: 0x00174DA0 File Offset: 0x00172FA0
		public bool AnyButtonIsPressed
		{
			get
			{
				int count = this.Controls.Count;
				for (int i = 0; i < count; i++)
				{
					InputControl inputControl = this.Controls[i];
					if (inputControl != null && inputControl.IsButton && inputControl.IsPressed)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x17000AD9 RID: 2777
		// (get) Token: 0x0600512C RID: 20780 RVA: 0x00174DE8 File Offset: 0x00172FE8
		public bool AnyButtonWasPressed
		{
			get
			{
				int count = this.Controls.Count;
				for (int i = 0; i < count; i++)
				{
					InputControl inputControl = this.Controls[i];
					if (inputControl != null && inputControl.IsButton && inputControl.WasPressed)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x17000ADA RID: 2778
		// (get) Token: 0x0600512D RID: 20781 RVA: 0x00174E30 File Offset: 0x00173030
		public bool AnyButtonWasReleased
		{
			get
			{
				int count = this.Controls.Count;
				for (int i = 0; i < count; i++)
				{
					InputControl inputControl = this.Controls[i];
					if (inputControl != null && inputControl.IsButton && inputControl.WasReleased)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x17000ADB RID: 2779
		// (get) Token: 0x0600512E RID: 20782 RVA: 0x00174E78 File Offset: 0x00173078
		public TwoAxisInputControl Direction
		{
			get
			{
				if (this.DPad.UpdateTick <= this.LeftStick.UpdateTick)
				{
					return this.LeftStick;
				}
				return this.DPad;
			}
		}

		// Token: 0x17000ADC RID: 2780
		// (get) Token: 0x0600512F RID: 20783 RVA: 0x00174EA0 File Offset: 0x001730A0
		public InputControl LeftStickUp
		{
			get
			{
				InputControl result;
				if ((result = this.cachedLeftStickUp) == null)
				{
					result = (this.cachedLeftStickUp = this.GetControl(InputControlType.LeftStickUp));
				}
				return result;
			}
		}

		// Token: 0x17000ADD RID: 2781
		// (get) Token: 0x06005130 RID: 20784 RVA: 0x00174EC8 File Offset: 0x001730C8
		public InputControl LeftStickDown
		{
			get
			{
				InputControl result;
				if ((result = this.cachedLeftStickDown) == null)
				{
					result = (this.cachedLeftStickDown = this.GetControl(InputControlType.LeftStickDown));
				}
				return result;
			}
		}

		// Token: 0x17000ADE RID: 2782
		// (get) Token: 0x06005131 RID: 20785 RVA: 0x00174EF0 File Offset: 0x001730F0
		public InputControl LeftStickLeft
		{
			get
			{
				InputControl result;
				if ((result = this.cachedLeftStickLeft) == null)
				{
					result = (this.cachedLeftStickLeft = this.GetControl(InputControlType.LeftStickLeft));
				}
				return result;
			}
		}

		// Token: 0x17000ADF RID: 2783
		// (get) Token: 0x06005132 RID: 20786 RVA: 0x00174F18 File Offset: 0x00173118
		public InputControl LeftStickRight
		{
			get
			{
				InputControl result;
				if ((result = this.cachedLeftStickRight) == null)
				{
					result = (this.cachedLeftStickRight = this.GetControl(InputControlType.LeftStickRight));
				}
				return result;
			}
		}

		// Token: 0x17000AE0 RID: 2784
		// (get) Token: 0x06005133 RID: 20787 RVA: 0x00174F40 File Offset: 0x00173140
		public InputControl RightStickUp
		{
			get
			{
				InputControl result;
				if ((result = this.cachedRightStickUp) == null)
				{
					result = (this.cachedRightStickUp = this.GetControl(InputControlType.RightStickUp));
				}
				return result;
			}
		}

		// Token: 0x17000AE1 RID: 2785
		// (get) Token: 0x06005134 RID: 20788 RVA: 0x00174F68 File Offset: 0x00173168
		public InputControl RightStickDown
		{
			get
			{
				InputControl result;
				if ((result = this.cachedRightStickDown) == null)
				{
					result = (this.cachedRightStickDown = this.GetControl(InputControlType.RightStickDown));
				}
				return result;
			}
		}

		// Token: 0x17000AE2 RID: 2786
		// (get) Token: 0x06005135 RID: 20789 RVA: 0x00174F90 File Offset: 0x00173190
		public InputControl RightStickLeft
		{
			get
			{
				InputControl result;
				if ((result = this.cachedRightStickLeft) == null)
				{
					result = (this.cachedRightStickLeft = this.GetControl(InputControlType.RightStickLeft));
				}
				return result;
			}
		}

		// Token: 0x17000AE3 RID: 2787
		// (get) Token: 0x06005136 RID: 20790 RVA: 0x00174FB8 File Offset: 0x001731B8
		public InputControl RightStickRight
		{
			get
			{
				InputControl result;
				if ((result = this.cachedRightStickRight) == null)
				{
					result = (this.cachedRightStickRight = this.GetControl(InputControlType.RightStickRight));
				}
				return result;
			}
		}

		// Token: 0x17000AE4 RID: 2788
		// (get) Token: 0x06005137 RID: 20791 RVA: 0x00174FE0 File Offset: 0x001731E0
		public InputControl DPadUp
		{
			get
			{
				InputControl result;
				if ((result = this.cachedDPadUp) == null)
				{
					result = (this.cachedDPadUp = this.GetControl(InputControlType.DPadUp));
				}
				return result;
			}
		}

		// Token: 0x17000AE5 RID: 2789
		// (get) Token: 0x06005138 RID: 20792 RVA: 0x00175008 File Offset: 0x00173208
		public InputControl DPadDown
		{
			get
			{
				InputControl result;
				if ((result = this.cachedDPadDown) == null)
				{
					result = (this.cachedDPadDown = this.GetControl(InputControlType.DPadDown));
				}
				return result;
			}
		}

		// Token: 0x17000AE6 RID: 2790
		// (get) Token: 0x06005139 RID: 20793 RVA: 0x00175030 File Offset: 0x00173230
		public InputControl DPadLeft
		{
			get
			{
				InputControl result;
				if ((result = this.cachedDPadLeft) == null)
				{
					result = (this.cachedDPadLeft = this.GetControl(InputControlType.DPadLeft));
				}
				return result;
			}
		}

		// Token: 0x17000AE7 RID: 2791
		// (get) Token: 0x0600513A RID: 20794 RVA: 0x00175058 File Offset: 0x00173258
		public InputControl DPadRight
		{
			get
			{
				InputControl result;
				if ((result = this.cachedDPadRight) == null)
				{
					result = (this.cachedDPadRight = this.GetControl(InputControlType.DPadRight));
				}
				return result;
			}
		}

		// Token: 0x17000AE8 RID: 2792
		// (get) Token: 0x0600513B RID: 20795 RVA: 0x00175080 File Offset: 0x00173280
		public InputControl Action1
		{
			get
			{
				InputControl result;
				if ((result = this.cachedAction1) == null)
				{
					result = (this.cachedAction1 = this.GetControl(InputControlType.Action1));
				}
				return result;
			}
		}

		// Token: 0x17000AE9 RID: 2793
		// (get) Token: 0x0600513C RID: 20796 RVA: 0x001750A8 File Offset: 0x001732A8
		public InputControl Action2
		{
			get
			{
				InputControl result;
				if ((result = this.cachedAction2) == null)
				{
					result = (this.cachedAction2 = this.GetControl(InputControlType.Action2));
				}
				return result;
			}
		}

		// Token: 0x17000AEA RID: 2794
		// (get) Token: 0x0600513D RID: 20797 RVA: 0x001750D0 File Offset: 0x001732D0
		public InputControl Action3
		{
			get
			{
				InputControl result;
				if ((result = this.cachedAction3) == null)
				{
					result = (this.cachedAction3 = this.GetControl(InputControlType.Action3));
				}
				return result;
			}
		}

		// Token: 0x17000AEB RID: 2795
		// (get) Token: 0x0600513E RID: 20798 RVA: 0x001750F8 File Offset: 0x001732F8
		public InputControl Action4
		{
			get
			{
				InputControl result;
				if ((result = this.cachedAction4) == null)
				{
					result = (this.cachedAction4 = this.GetControl(InputControlType.Action4));
				}
				return result;
			}
		}

		// Token: 0x17000AEC RID: 2796
		// (get) Token: 0x0600513F RID: 20799 RVA: 0x00175120 File Offset: 0x00173320
		public InputControl LeftTrigger
		{
			get
			{
				InputControl result;
				if ((result = this.cachedLeftTrigger) == null)
				{
					result = (this.cachedLeftTrigger = this.GetControl(InputControlType.LeftTrigger));
				}
				return result;
			}
		}

		// Token: 0x17000AED RID: 2797
		// (get) Token: 0x06005140 RID: 20800 RVA: 0x00175148 File Offset: 0x00173348
		public InputControl RightTrigger
		{
			get
			{
				InputControl result;
				if ((result = this.cachedRightTrigger) == null)
				{
					result = (this.cachedRightTrigger = this.GetControl(InputControlType.RightTrigger));
				}
				return result;
			}
		}

		// Token: 0x17000AEE RID: 2798
		// (get) Token: 0x06005141 RID: 20801 RVA: 0x00175170 File Offset: 0x00173370
		public InputControl LeftBumper
		{
			get
			{
				InputControl result;
				if ((result = this.cachedLeftBumper) == null)
				{
					result = (this.cachedLeftBumper = this.GetControl(InputControlType.LeftBumper));
				}
				return result;
			}
		}

		// Token: 0x17000AEF RID: 2799
		// (get) Token: 0x06005142 RID: 20802 RVA: 0x00175198 File Offset: 0x00173398
		public InputControl RightBumper
		{
			get
			{
				InputControl result;
				if ((result = this.cachedRightBumper) == null)
				{
					result = (this.cachedRightBumper = this.GetControl(InputControlType.RightBumper));
				}
				return result;
			}
		}

		// Token: 0x17000AF0 RID: 2800
		// (get) Token: 0x06005143 RID: 20803 RVA: 0x001751C0 File Offset: 0x001733C0
		public InputControl LeftStickButton
		{
			get
			{
				InputControl result;
				if ((result = this.cachedLeftStickButton) == null)
				{
					result = (this.cachedLeftStickButton = this.GetControl(InputControlType.LeftStickButton));
				}
				return result;
			}
		}

		// Token: 0x17000AF1 RID: 2801
		// (get) Token: 0x06005144 RID: 20804 RVA: 0x001751E8 File Offset: 0x001733E8
		public InputControl RightStickButton
		{
			get
			{
				InputControl result;
				if ((result = this.cachedRightStickButton) == null)
				{
					result = (this.cachedRightStickButton = this.GetControl(InputControlType.RightStickButton));
				}
				return result;
			}
		}

		// Token: 0x17000AF2 RID: 2802
		// (get) Token: 0x06005145 RID: 20805 RVA: 0x00175210 File Offset: 0x00173410
		public InputControl LeftStickX
		{
			get
			{
				InputControl result;
				if ((result = this.cachedLeftStickX) == null)
				{
					result = (this.cachedLeftStickX = this.GetControl(InputControlType.LeftStickX));
				}
				return result;
			}
		}

		// Token: 0x17000AF3 RID: 2803
		// (get) Token: 0x06005146 RID: 20806 RVA: 0x0017523C File Offset: 0x0017343C
		public InputControl LeftStickY
		{
			get
			{
				InputControl result;
				if ((result = this.cachedLeftStickY) == null)
				{
					result = (this.cachedLeftStickY = this.GetControl(InputControlType.LeftStickY));
				}
				return result;
			}
		}

		// Token: 0x17000AF4 RID: 2804
		// (get) Token: 0x06005147 RID: 20807 RVA: 0x00175268 File Offset: 0x00173468
		public InputControl RightStickX
		{
			get
			{
				InputControl result;
				if ((result = this.cachedRightStickX) == null)
				{
					result = (this.cachedRightStickX = this.GetControl(InputControlType.RightStickX));
				}
				return result;
			}
		}

		// Token: 0x17000AF5 RID: 2805
		// (get) Token: 0x06005148 RID: 20808 RVA: 0x00175294 File Offset: 0x00173494
		public InputControl RightStickY
		{
			get
			{
				InputControl result;
				if ((result = this.cachedRightStickY) == null)
				{
					result = (this.cachedRightStickY = this.GetControl(InputControlType.RightStickY));
				}
				return result;
			}
		}

		// Token: 0x17000AF6 RID: 2806
		// (get) Token: 0x06005149 RID: 20809 RVA: 0x001752C0 File Offset: 0x001734C0
		public InputControl DPadX
		{
			get
			{
				InputControl result;
				if ((result = this.cachedDPadX) == null)
				{
					result = (this.cachedDPadX = this.GetControl(InputControlType.DPadX));
				}
				return result;
			}
		}

		// Token: 0x17000AF7 RID: 2807
		// (get) Token: 0x0600514A RID: 20810 RVA: 0x001752EC File Offset: 0x001734EC
		public InputControl DPadY
		{
			get
			{
				InputControl result;
				if ((result = this.cachedDPadY) == null)
				{
					result = (this.cachedDPadY = this.GetControl(InputControlType.DPadY));
				}
				return result;
			}
		}

		// Token: 0x17000AF8 RID: 2808
		// (get) Token: 0x0600514B RID: 20811 RVA: 0x00175318 File Offset: 0x00173518
		public InputControl Command
		{
			get
			{
				InputControl result;
				if ((result = this.cachedCommand) == null)
				{
					result = (this.cachedCommand = this.GetControl(InputControlType.Command));
				}
				return result;
			}
		}

		// Token: 0x17000AF9 RID: 2809
		// (get) Token: 0x0600514C RID: 20812 RVA: 0x00175344 File Offset: 0x00173544
		public InputControl LeftCommand
		{
			get
			{
				InputControl result;
				if ((result = this.cachedLeftCommand) == null)
				{
					result = (this.cachedLeftCommand = this.GetControl(InputControlType.LeftCommand));
				}
				return result;
			}
		}

		// Token: 0x17000AFA RID: 2810
		// (get) Token: 0x0600514D RID: 20813 RVA: 0x00175370 File Offset: 0x00173570
		public InputControl RightCommand
		{
			get
			{
				InputControl result;
				if ((result = this.cachedRightCommand) == null)
				{
					result = (this.cachedRightCommand = this.GetControl(InputControlType.RightCommand));
				}
				return result;
			}
		}

		// Token: 0x0600514E RID: 20814 RVA: 0x0017539C File Offset: 0x0017359C
		private void ExpireControlCache()
		{
			this.cachedLeftStickUp = null;
			this.cachedLeftStickDown = null;
			this.cachedLeftStickLeft = null;
			this.cachedLeftStickRight = null;
			this.cachedRightStickUp = null;
			this.cachedRightStickDown = null;
			this.cachedRightStickLeft = null;
			this.cachedRightStickRight = null;
			this.cachedDPadUp = null;
			this.cachedDPadDown = null;
			this.cachedDPadLeft = null;
			this.cachedDPadRight = null;
			this.cachedAction1 = null;
			this.cachedAction2 = null;
			this.cachedAction3 = null;
			this.cachedAction4 = null;
			this.cachedLeftTrigger = null;
			this.cachedRightTrigger = null;
			this.cachedLeftBumper = null;
			this.cachedRightBumper = null;
			this.cachedLeftStickButton = null;
			this.cachedRightStickButton = null;
			this.cachedLeftStickX = null;
			this.cachedLeftStickY = null;
			this.cachedRightStickX = null;
			this.cachedRightStickY = null;
			this.cachedDPadX = null;
			this.cachedDPadY = null;
			this.cachedCommand = null;
			this.cachedLeftCommand = null;
			this.cachedRightCommand = null;
		}

		// Token: 0x17000AFB RID: 2811
		// (get) Token: 0x0600514F RID: 20815 RVA: 0x00175482 File Offset: 0x00173682
		public virtual int NumUnknownAnalogs
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000AFC RID: 2812
		// (get) Token: 0x06005150 RID: 20816 RVA: 0x00175485 File Offset: 0x00173685
		public virtual int NumUnknownButtons
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x06005151 RID: 20817 RVA: 0x00175488 File Offset: 0x00173688
		public virtual bool ReadRawButtonState(int index)
		{
			return false;
		}

		// Token: 0x06005152 RID: 20818 RVA: 0x0017548B File Offset: 0x0017368B
		public virtual float ReadRawAnalogValue(int index)
		{
			return 0f;
		}

		// Token: 0x06005153 RID: 20819 RVA: 0x00175494 File Offset: 0x00173694
		public void TakeSnapshot()
		{
			if (this.AnalogSnapshot == null)
			{
				this.AnalogSnapshot = new InputDevice.AnalogSnapshotEntry[this.NumUnknownAnalogs];
			}
			for (int i = 0; i < this.NumUnknownAnalogs; i++)
			{
				float value = Utility.ApplySnapping(this.ReadRawAnalogValue(i), 0.5f);
				this.AnalogSnapshot[i].value = value;
			}
		}

		// Token: 0x06005154 RID: 20820 RVA: 0x001754F0 File Offset: 0x001736F0
		public UnknownDeviceControl GetFirstPressedAnalog()
		{
			if (this.AnalogSnapshot != null)
			{
				for (int i = 0; i < this.NumUnknownAnalogs; i++)
				{
					InputControlType control = InputControlType.Analog0 + i;
					float num = Utility.ApplySnapping(this.ReadRawAnalogValue(i), 0.5f);
					float num2 = num - this.AnalogSnapshot[i].value;
					this.AnalogSnapshot[i].TrackMinMaxValue(num);
					if (num2 > 0.1f)
					{
						num2 = this.AnalogSnapshot[i].maxValue - this.AnalogSnapshot[i].value;
					}
					if (num2 < -0.1f)
					{
						num2 = this.AnalogSnapshot[i].minValue - this.AnalogSnapshot[i].value;
					}
					if (num2 > 1.9f)
					{
						return new UnknownDeviceControl(control, InputRangeType.MinusOneToOne);
					}
					if (num2 < -0.9f)
					{
						return new UnknownDeviceControl(control, InputRangeType.ZeroToMinusOne);
					}
					if (num2 > 0.9f)
					{
						return new UnknownDeviceControl(control, InputRangeType.ZeroToOne);
					}
				}
			}
			return UnknownDeviceControl.None;
		}

		// Token: 0x06005155 RID: 20821 RVA: 0x001755EC File Offset: 0x001737EC
		public UnknownDeviceControl GetFirstPressedButton()
		{
			for (int i = 0; i < this.NumUnknownButtons; i++)
			{
				if (this.ReadRawButtonState(i))
				{
					return new UnknownDeviceControl(InputControlType.Button0 + i, InputRangeType.ZeroToOne);
				}
			}
			return UnknownDeviceControl.None;
		}

		// Token: 0x040051B2 RID: 20914
		public static readonly InputDevice Null = new InputDevice("None");

		// Token: 0x040051BD RID: 20925
		private readonly List<InputControl> controls;

		// Token: 0x040051C3 RID: 20931
		private bool hasLeftCommandControl;

		// Token: 0x040051C4 RID: 20932
		private InputControl leftCommandSource;

		// Token: 0x040051C6 RID: 20934
		private bool hasRightCommandControl;

		// Token: 0x040051C7 RID: 20935
		private InputControl rightCommandSource;

		// Token: 0x040051C9 RID: 20937
		public bool Passive;

		// Token: 0x040051CB RID: 20939
		private InputControl cachedLeftStickUp;

		// Token: 0x040051CC RID: 20940
		private InputControl cachedLeftStickDown;

		// Token: 0x040051CD RID: 20941
		private InputControl cachedLeftStickLeft;

		// Token: 0x040051CE RID: 20942
		private InputControl cachedLeftStickRight;

		// Token: 0x040051CF RID: 20943
		private InputControl cachedRightStickUp;

		// Token: 0x040051D0 RID: 20944
		private InputControl cachedRightStickDown;

		// Token: 0x040051D1 RID: 20945
		private InputControl cachedRightStickLeft;

		// Token: 0x040051D2 RID: 20946
		private InputControl cachedRightStickRight;

		// Token: 0x040051D3 RID: 20947
		private InputControl cachedDPadUp;

		// Token: 0x040051D4 RID: 20948
		private InputControl cachedDPadDown;

		// Token: 0x040051D5 RID: 20949
		private InputControl cachedDPadLeft;

		// Token: 0x040051D6 RID: 20950
		private InputControl cachedDPadRight;

		// Token: 0x040051D7 RID: 20951
		private InputControl cachedAction1;

		// Token: 0x040051D8 RID: 20952
		private InputControl cachedAction2;

		// Token: 0x040051D9 RID: 20953
		private InputControl cachedAction3;

		// Token: 0x040051DA RID: 20954
		private InputControl cachedAction4;

		// Token: 0x040051DB RID: 20955
		private InputControl cachedLeftTrigger;

		// Token: 0x040051DC RID: 20956
		private InputControl cachedRightTrigger;

		// Token: 0x040051DD RID: 20957
		private InputControl cachedLeftBumper;

		// Token: 0x040051DE RID: 20958
		private InputControl cachedRightBumper;

		// Token: 0x040051DF RID: 20959
		private InputControl cachedLeftStickButton;

		// Token: 0x040051E0 RID: 20960
		private InputControl cachedRightStickButton;

		// Token: 0x040051E1 RID: 20961
		private InputControl cachedLeftStickX;

		// Token: 0x040051E2 RID: 20962
		private InputControl cachedLeftStickY;

		// Token: 0x040051E3 RID: 20963
		private InputControl cachedRightStickX;

		// Token: 0x040051E4 RID: 20964
		private InputControl cachedRightStickY;

		// Token: 0x040051E5 RID: 20965
		private InputControl cachedDPadX;

		// Token: 0x040051E6 RID: 20966
		private InputControl cachedDPadY;

		// Token: 0x040051E7 RID: 20967
		private InputControl cachedCommand;

		// Token: 0x040051E8 RID: 20968
		private InputControl cachedLeftCommand;

		// Token: 0x040051E9 RID: 20969
		private InputControl cachedRightCommand;

		// Token: 0x02001B5C RID: 7004
		protected struct AnalogSnapshotEntry
		{
			// Token: 0x060099F6 RID: 39414 RVA: 0x002B2878 File Offset: 0x002B0A78
			public void TrackMinMaxValue(float currentValue)
			{
				this.maxValue = Mathf.Max(this.maxValue, currentValue);
				this.minValue = Mathf.Min(this.minValue, currentValue);
			}

			// Token: 0x04009C7A RID: 40058
			public float value;

			// Token: 0x04009C7B RID: 40059
			public float maxValue;

			// Token: 0x04009C7C RID: 40060
			public float minValue;
		}
	}
}
