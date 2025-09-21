using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x020008FC RID: 2300
	public class TwoAxisInputControl : IInputControl
	{
		// Token: 0x17000AA5 RID: 2725
		// (get) Token: 0x060050A1 RID: 20641 RVA: 0x001734E1 File Offset: 0x001716E1
		// (set) Token: 0x060050A2 RID: 20642 RVA: 0x001734E9 File Offset: 0x001716E9
		public float X { get; protected set; }

		// Token: 0x17000AA6 RID: 2726
		// (get) Token: 0x060050A3 RID: 20643 RVA: 0x001734F2 File Offset: 0x001716F2
		// (set) Token: 0x060050A4 RID: 20644 RVA: 0x001734FA File Offset: 0x001716FA
		public float Y { get; protected set; }

		// Token: 0x17000AA7 RID: 2727
		// (get) Token: 0x060050A5 RID: 20645 RVA: 0x00173503 File Offset: 0x00171703
		// (set) Token: 0x060050A6 RID: 20646 RVA: 0x0017350B File Offset: 0x0017170B
		public OneAxisInputControl Left { get; protected set; }

		// Token: 0x17000AA8 RID: 2728
		// (get) Token: 0x060050A7 RID: 20647 RVA: 0x00173514 File Offset: 0x00171714
		// (set) Token: 0x060050A8 RID: 20648 RVA: 0x0017351C File Offset: 0x0017171C
		public OneAxisInputControl Right { get; protected set; }

		// Token: 0x17000AA9 RID: 2729
		// (get) Token: 0x060050A9 RID: 20649 RVA: 0x00173525 File Offset: 0x00171725
		// (set) Token: 0x060050AA RID: 20650 RVA: 0x0017352D File Offset: 0x0017172D
		public OneAxisInputControl Up { get; protected set; }

		// Token: 0x17000AAA RID: 2730
		// (get) Token: 0x060050AB RID: 20651 RVA: 0x00173536 File Offset: 0x00171736
		// (set) Token: 0x060050AC RID: 20652 RVA: 0x0017353E File Offset: 0x0017173E
		public OneAxisInputControl Down { get; protected set; }

		// Token: 0x17000AAB RID: 2731
		// (get) Token: 0x060050AD RID: 20653 RVA: 0x00173547 File Offset: 0x00171747
		// (set) Token: 0x060050AE RID: 20654 RVA: 0x0017354F File Offset: 0x0017174F
		public ulong UpdateTick { get; protected set; }

		// Token: 0x060050AF RID: 20655 RVA: 0x00173558 File Offset: 0x00171758
		public TwoAxisInputControl()
		{
			this.Left = new OneAxisInputControl();
			this.Right = new OneAxisInputControl();
			this.Up = new OneAxisInputControl();
			this.Down = new OneAxisInputControl();
		}

		// Token: 0x060050B0 RID: 20656 RVA: 0x001735C0 File Offset: 0x001717C0
		public void ClearInputState()
		{
			this.Left.ClearInputState();
			this.Right.ClearInputState();
			this.Up.ClearInputState();
			this.Down.ClearInputState();
			this.lastState = false;
			this.lastValue = Vector2.zero;
			this.thisState = false;
			this.thisValue = Vector2.zero;
			this.X = 0f;
			this.Y = 0f;
			this.clearInputState = true;
		}

		// Token: 0x060050B1 RID: 20657 RVA: 0x0017363A File Offset: 0x0017183A
		public void Filter(TwoAxisInputControl twoAxisInputControl, float deltaTime)
		{
			this.UpdateWithAxes(twoAxisInputControl.X, twoAxisInputControl.Y, InputManager.CurrentTick, deltaTime);
		}

		// Token: 0x060050B2 RID: 20658 RVA: 0x00173654 File Offset: 0x00171854
		internal void UpdateWithAxes(float x, float y, ulong updateTick, float deltaTime)
		{
			this.lastState = this.thisState;
			this.lastValue = this.thisValue;
			this.thisValue = (this.Raw ? new Vector2(x, y) : this.DeadZoneFunc(x, y, this.LowerDeadZone, this.UpperDeadZone));
			this.X = this.thisValue.x;
			this.Y = this.thisValue.y;
			this.Left.CommitWithValue(Mathf.Max(0f, -this.X), updateTick, deltaTime);
			this.Right.CommitWithValue(Mathf.Max(0f, this.X), updateTick, deltaTime);
			if (InputManager.InvertYAxis)
			{
				this.Up.CommitWithValue(Mathf.Max(0f, -this.Y), updateTick, deltaTime);
				this.Down.CommitWithValue(Mathf.Max(0f, this.Y), updateTick, deltaTime);
			}
			else
			{
				this.Up.CommitWithValue(Mathf.Max(0f, this.Y), updateTick, deltaTime);
				this.Down.CommitWithValue(Mathf.Max(0f, -this.Y), updateTick, deltaTime);
			}
			this.thisState = (this.Up.State || this.Down.State || this.Left.State || this.Right.State);
			if (this.clearInputState)
			{
				this.lastState = this.thisState;
				this.lastValue = this.thisValue;
				this.clearInputState = false;
				this.HasChanged = false;
				return;
			}
			if (this.thisValue != this.lastValue)
			{
				this.UpdateTick = updateTick;
				this.HasChanged = true;
				return;
			}
			this.HasChanged = false;
		}

		// Token: 0x17000AAC RID: 2732
		// (get) Token: 0x060050B3 RID: 20659 RVA: 0x0017381E File Offset: 0x00171A1E
		// (set) Token: 0x060050B4 RID: 20660 RVA: 0x00173828 File Offset: 0x00171A28
		public float Sensitivity
		{
			get
			{
				return this.sensitivity;
			}
			set
			{
				this.sensitivity = Mathf.Clamp01(value);
				this.Left.Sensitivity = this.sensitivity;
				this.Right.Sensitivity = this.sensitivity;
				this.Up.Sensitivity = this.sensitivity;
				this.Down.Sensitivity = this.sensitivity;
			}
		}

		// Token: 0x17000AAD RID: 2733
		// (get) Token: 0x060050B5 RID: 20661 RVA: 0x00173885 File Offset: 0x00171A85
		// (set) Token: 0x060050B6 RID: 20662 RVA: 0x00173890 File Offset: 0x00171A90
		public float StateThreshold
		{
			get
			{
				return this.stateThreshold;
			}
			set
			{
				this.stateThreshold = Mathf.Clamp01(value);
				this.Left.StateThreshold = this.stateThreshold;
				this.Right.StateThreshold = this.stateThreshold;
				this.Up.StateThreshold = this.stateThreshold;
				this.Down.StateThreshold = this.stateThreshold;
			}
		}

		// Token: 0x17000AAE RID: 2734
		// (get) Token: 0x060050B7 RID: 20663 RVA: 0x001738ED File Offset: 0x00171AED
		// (set) Token: 0x060050B8 RID: 20664 RVA: 0x001738F8 File Offset: 0x00171AF8
		public float LowerDeadZone
		{
			get
			{
				return this.lowerDeadZone;
			}
			set
			{
				this.lowerDeadZone = Mathf.Clamp01(value);
				this.Left.LowerDeadZone = this.lowerDeadZone;
				this.Right.LowerDeadZone = this.lowerDeadZone;
				this.Up.LowerDeadZone = this.lowerDeadZone;
				this.Down.LowerDeadZone = this.lowerDeadZone;
			}
		}

		// Token: 0x17000AAF RID: 2735
		// (get) Token: 0x060050B9 RID: 20665 RVA: 0x00173955 File Offset: 0x00171B55
		// (set) Token: 0x060050BA RID: 20666 RVA: 0x00173960 File Offset: 0x00171B60
		public float UpperDeadZone
		{
			get
			{
				return this.upperDeadZone;
			}
			set
			{
				this.upperDeadZone = Mathf.Clamp01(value);
				this.Left.UpperDeadZone = this.upperDeadZone;
				this.Right.UpperDeadZone = this.upperDeadZone;
				this.Up.UpperDeadZone = this.upperDeadZone;
				this.Down.UpperDeadZone = this.upperDeadZone;
			}
		}

		// Token: 0x17000AB0 RID: 2736
		// (get) Token: 0x060050BB RID: 20667 RVA: 0x001739BD File Offset: 0x00171BBD
		public bool State
		{
			get
			{
				return this.thisState;
			}
		}

		// Token: 0x17000AB1 RID: 2737
		// (get) Token: 0x060050BC RID: 20668 RVA: 0x001739C5 File Offset: 0x00171BC5
		public bool LastState
		{
			get
			{
				return this.lastState;
			}
		}

		// Token: 0x17000AB2 RID: 2738
		// (get) Token: 0x060050BD RID: 20669 RVA: 0x001739CD File Offset: 0x00171BCD
		public Vector2 Value
		{
			get
			{
				return this.thisValue;
			}
		}

		// Token: 0x17000AB3 RID: 2739
		// (get) Token: 0x060050BE RID: 20670 RVA: 0x001739D5 File Offset: 0x00171BD5
		public Vector2 LastValue
		{
			get
			{
				return this.lastValue;
			}
		}

		// Token: 0x17000AB4 RID: 2740
		// (get) Token: 0x060050BF RID: 20671 RVA: 0x001739DD File Offset: 0x00171BDD
		public Vector2 Vector
		{
			get
			{
				return this.thisValue;
			}
		}

		// Token: 0x17000AB5 RID: 2741
		// (get) Token: 0x060050C0 RID: 20672 RVA: 0x001739E5 File Offset: 0x00171BE5
		// (set) Token: 0x060050C1 RID: 20673 RVA: 0x001739ED File Offset: 0x00171BED
		public bool HasChanged { get; protected set; }

		// Token: 0x17000AB6 RID: 2742
		// (get) Token: 0x060050C2 RID: 20674 RVA: 0x001739F6 File Offset: 0x00171BF6
		public bool IsPressed
		{
			get
			{
				return this.thisState;
			}
		}

		// Token: 0x17000AB7 RID: 2743
		// (get) Token: 0x060050C3 RID: 20675 RVA: 0x001739FE File Offset: 0x00171BFE
		public bool WasPressed
		{
			get
			{
				return this.thisState && !this.lastState;
			}
		}

		// Token: 0x17000AB8 RID: 2744
		// (get) Token: 0x060050C4 RID: 20676 RVA: 0x00173A13 File Offset: 0x00171C13
		public bool WasReleased
		{
			get
			{
				return !this.thisState && this.lastState;
			}
		}

		// Token: 0x17000AB9 RID: 2745
		// (get) Token: 0x060050C5 RID: 20677 RVA: 0x00173A25 File Offset: 0x00171C25
		public float Angle
		{
			get
			{
				return Utility.VectorToAngle(this.thisValue);
			}
		}

		// Token: 0x060050C6 RID: 20678 RVA: 0x00173A32 File Offset: 0x00171C32
		public static implicit operator bool(TwoAxisInputControl instance)
		{
			return instance.thisState;
		}

		// Token: 0x060050C7 RID: 20679 RVA: 0x00173A3A File Offset: 0x00171C3A
		public static implicit operator Vector2(TwoAxisInputControl instance)
		{
			return instance.thisValue;
		}

		// Token: 0x060050C8 RID: 20680 RVA: 0x00173A42 File Offset: 0x00171C42
		public static implicit operator Vector3(TwoAxisInputControl instance)
		{
			return instance.thisValue;
		}

		// Token: 0x04005196 RID: 20886
		public static readonly TwoAxisInputControl Null = new TwoAxisInputControl();

		// Token: 0x0400519E RID: 20894
		public DeadZoneFunc DeadZoneFunc = new DeadZoneFunc(DeadZone.Circular);

		// Token: 0x0400519F RID: 20895
		private float sensitivity = 1f;

		// Token: 0x040051A0 RID: 20896
		private float lowerDeadZone;

		// Token: 0x040051A1 RID: 20897
		private float upperDeadZone = 1f;

		// Token: 0x040051A2 RID: 20898
		private float stateThreshold;

		// Token: 0x040051A3 RID: 20899
		public bool Raw;

		// Token: 0x040051A4 RID: 20900
		private bool thisState;

		// Token: 0x040051A5 RID: 20901
		private bool lastState;

		// Token: 0x040051A6 RID: 20902
		private Vector2 thisValue;

		// Token: 0x040051A7 RID: 20903
		private Vector2 lastValue;

		// Token: 0x040051A8 RID: 20904
		private bool clearInputState;
	}
}
