using System;
using UnityEngine;

namespace InControl
{
	// Token: 0x020008FB RID: 2299
	public class OneAxisInputControl : IInputControl
	{
		// Token: 0x17000A91 RID: 2705
		// (get) Token: 0x0600507A RID: 20602 RVA: 0x00172F2F File Offset: 0x0017112F
		// (set) Token: 0x0600507B RID: 20603 RVA: 0x00172F37 File Offset: 0x00171137
		public ulong UpdateTick { get; protected set; }

		// Token: 0x0600507C RID: 20604 RVA: 0x00172F40 File Offset: 0x00171140
		private void PrepareForUpdate(ulong updateTick)
		{
			if (this.isNullControl)
			{
				return;
			}
			if (updateTick < this.pendingTick)
			{
				throw new InvalidOperationException("Cannot be updated with an earlier tick.");
			}
			if (this.pendingCommit && updateTick != this.pendingTick)
			{
				throw new InvalidOperationException("Cannot be updated for a new tick until pending tick is committed.");
			}
			if (updateTick > this.pendingTick)
			{
				this.lastState = this.thisState;
				this.nextState.Reset();
				this.pendingTick = updateTick;
				this.pendingCommit = true;
			}
		}

		// Token: 0x0600507D RID: 20605 RVA: 0x00172FB4 File Offset: 0x001711B4
		public bool UpdateWithState(bool state, ulong updateTick, float deltaTime)
		{
			if (this.isNullControl)
			{
				return false;
			}
			this.PrepareForUpdate(updateTick);
			this.nextState.Set(state || this.nextState.State);
			return state;
		}

		// Token: 0x0600507E RID: 20606 RVA: 0x00172FE4 File Offset: 0x001711E4
		public bool UpdateWithValue(float value, ulong updateTick, float deltaTime)
		{
			if (this.isNullControl)
			{
				return false;
			}
			this.PrepareForUpdate(updateTick);
			if (Utility.Abs(value) > Utility.Abs(this.nextState.RawValue))
			{
				this.nextState.RawValue = value;
				if (!this.Raw)
				{
					value = Utility.ApplyDeadZone(value, this.lowerDeadZone, this.upperDeadZone);
				}
				this.nextState.Set(value, this.stateThreshold);
				return true;
			}
			return false;
		}

		// Token: 0x0600507F RID: 20607 RVA: 0x00173058 File Offset: 0x00171258
		internal bool UpdateWithRawValue(float value, ulong updateTick, float deltaTime)
		{
			if (this.isNullControl)
			{
				return false;
			}
			this.Raw = true;
			this.PrepareForUpdate(updateTick);
			if (Utility.Abs(value) > Utility.Abs(this.nextState.RawValue))
			{
				this.nextState.RawValue = value;
				this.nextState.Set(value, this.stateThreshold);
				return true;
			}
			return false;
		}

		// Token: 0x06005080 RID: 20608 RVA: 0x001730B8 File Offset: 0x001712B8
		internal void SetValue(float value, ulong updateTick)
		{
			if (this.isNullControl)
			{
				return;
			}
			if (updateTick > this.pendingTick)
			{
				this.lastState = this.thisState;
				this.nextState.Reset();
				this.pendingTick = updateTick;
				this.pendingCommit = true;
			}
			this.nextState.RawValue = value;
			this.nextState.Set(value, this.StateThreshold);
		}

		// Token: 0x06005081 RID: 20609 RVA: 0x0017311A File Offset: 0x0017131A
		public void ClearInputState()
		{
			this.lastState.Reset();
			this.thisState.Reset();
			this.nextState.Reset();
			this.wasRepeated = false;
			this.clearInputState = true;
		}

		// Token: 0x06005082 RID: 20610 RVA: 0x0017314C File Offset: 0x0017134C
		public void Commit()
		{
			if (this.isNullControl)
			{
				return;
			}
			this.pendingCommit = false;
			this.thisState = this.nextState;
			if (this.clearInputState)
			{
				this.lastState = this.nextState;
				this.UpdateTick = this.pendingTick;
				this.clearInputState = false;
				return;
			}
			bool state = this.lastState.State;
			bool state2 = this.thisState.State;
			this.wasRepeated = false;
			if (state && !state2)
			{
				this.nextRepeatTime = 0f;
			}
			else if (state2)
			{
				float realtimeSinceStartup = Time.realtimeSinceStartup;
				if (!state)
				{
					this.nextRepeatTime = realtimeSinceStartup + this.FirstRepeatDelay;
				}
				else if (realtimeSinceStartup >= this.nextRepeatTime)
				{
					this.wasRepeated = true;
					this.nextRepeatTime = realtimeSinceStartup + this.RepeatDelay;
				}
			}
			if (this.thisState != this.lastState)
			{
				this.UpdateTick = this.pendingTick;
			}
		}

		// Token: 0x06005083 RID: 20611 RVA: 0x00173228 File Offset: 0x00171428
		public void CommitWithState(bool state, ulong updateTick, float deltaTime)
		{
			this.UpdateWithState(state, updateTick, deltaTime);
			this.Commit();
		}

		// Token: 0x06005084 RID: 20612 RVA: 0x0017323A File Offset: 0x0017143A
		public void CommitWithValue(float value, ulong updateTick, float deltaTime)
		{
			this.UpdateWithValue(value, updateTick, deltaTime);
			this.Commit();
		}

		// Token: 0x06005085 RID: 20613 RVA: 0x0017324C File Offset: 0x0017144C
		internal void CommitWithSides(InputControl negativeSide, InputControl positiveSide, ulong updateTick, float deltaTime)
		{
			this.LowerDeadZone = Mathf.Max(negativeSide.LowerDeadZone, positiveSide.LowerDeadZone);
			this.UpperDeadZone = Mathf.Min(negativeSide.UpperDeadZone, positiveSide.UpperDeadZone);
			this.Raw = (negativeSide.Raw || positiveSide.Raw);
			float value = Utility.ValueFromSides(negativeSide.RawValue, positiveSide.RawValue);
			this.CommitWithValue(value, updateTick, deltaTime);
		}

		// Token: 0x17000A92 RID: 2706
		// (get) Token: 0x06005086 RID: 20614 RVA: 0x001732BA File Offset: 0x001714BA
		public bool State
		{
			get
			{
				return this.EnabledInHierarchy && this.thisState.State;
			}
		}

		// Token: 0x17000A93 RID: 2707
		// (get) Token: 0x06005087 RID: 20615 RVA: 0x001732D1 File Offset: 0x001714D1
		public bool LastState
		{
			get
			{
				return this.EnabledInHierarchy && this.lastState.State;
			}
		}

		// Token: 0x17000A94 RID: 2708
		// (get) Token: 0x06005088 RID: 20616 RVA: 0x001732E8 File Offset: 0x001714E8
		public float Value
		{
			get
			{
				if (!this.EnabledInHierarchy)
				{
					return 0f;
				}
				return this.thisState.Value;
			}
		}

		// Token: 0x17000A95 RID: 2709
		// (get) Token: 0x06005089 RID: 20617 RVA: 0x00173303 File Offset: 0x00171503
		public float LastValue
		{
			get
			{
				if (!this.EnabledInHierarchy)
				{
					return 0f;
				}
				return this.lastState.Value;
			}
		}

		// Token: 0x17000A96 RID: 2710
		// (get) Token: 0x0600508A RID: 20618 RVA: 0x0017331E File Offset: 0x0017151E
		public float RawValue
		{
			get
			{
				if (!this.EnabledInHierarchy)
				{
					return 0f;
				}
				return this.thisState.RawValue;
			}
		}

		// Token: 0x17000A97 RID: 2711
		// (get) Token: 0x0600508B RID: 20619 RVA: 0x00173339 File Offset: 0x00171539
		internal float NextRawValue
		{
			get
			{
				if (!this.EnabledInHierarchy)
				{
					return 0f;
				}
				return this.nextState.RawValue;
			}
		}

		// Token: 0x17000A98 RID: 2712
		// (get) Token: 0x0600508C RID: 20620 RVA: 0x00173354 File Offset: 0x00171554
		internal bool HasInput
		{
			get
			{
				return this.EnabledInHierarchy && Utility.IsNotZero(this.thisState.Value);
			}
		}

		// Token: 0x17000A99 RID: 2713
		// (get) Token: 0x0600508D RID: 20621 RVA: 0x00173370 File Offset: 0x00171570
		public bool HasChanged
		{
			get
			{
				return this.EnabledInHierarchy && this.thisState != this.lastState;
			}
		}

		// Token: 0x17000A9A RID: 2714
		// (get) Token: 0x0600508E RID: 20622 RVA: 0x0017338D File Offset: 0x0017158D
		public bool IsPressed
		{
			get
			{
				return this.EnabledInHierarchy && this.thisState.State;
			}
		}

		// Token: 0x17000A9B RID: 2715
		// (get) Token: 0x0600508F RID: 20623 RVA: 0x001733A4 File Offset: 0x001715A4
		public bool WasPressed
		{
			get
			{
				return this.EnabledInHierarchy && this.thisState && !this.lastState;
			}
		}

		// Token: 0x17000A9C RID: 2716
		// (get) Token: 0x06005090 RID: 20624 RVA: 0x001733CB File Offset: 0x001715CB
		public bool WasReleased
		{
			get
			{
				return this.EnabledInHierarchy && !this.thisState && this.lastState;
			}
		}

		// Token: 0x17000A9D RID: 2717
		// (get) Token: 0x06005091 RID: 20625 RVA: 0x001733EF File Offset: 0x001715EF
		public bool WasRepeated
		{
			get
			{
				return this.EnabledInHierarchy && this.wasRepeated;
			}
		}

		// Token: 0x17000A9E RID: 2718
		// (get) Token: 0x06005092 RID: 20626 RVA: 0x00173401 File Offset: 0x00171601
		// (set) Token: 0x06005093 RID: 20627 RVA: 0x00173409 File Offset: 0x00171609
		public float Sensitivity
		{
			get
			{
				return this.sensitivity;
			}
			set
			{
				this.sensitivity = Mathf.Clamp01(value);
			}
		}

		// Token: 0x17000A9F RID: 2719
		// (get) Token: 0x06005094 RID: 20628 RVA: 0x00173417 File Offset: 0x00171617
		// (set) Token: 0x06005095 RID: 20629 RVA: 0x0017341F File Offset: 0x0017161F
		public float LowerDeadZone
		{
			get
			{
				return this.lowerDeadZone;
			}
			set
			{
				this.lowerDeadZone = Mathf.Clamp01(value);
			}
		}

		// Token: 0x17000AA0 RID: 2720
		// (get) Token: 0x06005096 RID: 20630 RVA: 0x0017342D File Offset: 0x0017162D
		// (set) Token: 0x06005097 RID: 20631 RVA: 0x00173435 File Offset: 0x00171635
		public float UpperDeadZone
		{
			get
			{
				return this.upperDeadZone;
			}
			set
			{
				this.upperDeadZone = Mathf.Clamp01(value);
			}
		}

		// Token: 0x17000AA1 RID: 2721
		// (get) Token: 0x06005098 RID: 20632 RVA: 0x00173443 File Offset: 0x00171643
		// (set) Token: 0x06005099 RID: 20633 RVA: 0x0017344B File Offset: 0x0017164B
		public float StateThreshold
		{
			get
			{
				return this.stateThreshold;
			}
			set
			{
				this.stateThreshold = Mathf.Clamp01(value);
			}
		}

		// Token: 0x17000AA2 RID: 2722
		// (get) Token: 0x0600509A RID: 20634 RVA: 0x00173459 File Offset: 0x00171659
		public bool IsNullControl
		{
			get
			{
				return this.isNullControl;
			}
		}

		// Token: 0x17000AA3 RID: 2723
		// (get) Token: 0x0600509B RID: 20635 RVA: 0x00173461 File Offset: 0x00171661
		// (set) Token: 0x0600509C RID: 20636 RVA: 0x00173469 File Offset: 0x00171669
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				this.enabled = value;
			}
		}

		// Token: 0x17000AA4 RID: 2724
		// (get) Token: 0x0600509D RID: 20637 RVA: 0x00173472 File Offset: 0x00171672
		public bool EnabledInHierarchy
		{
			get
			{
				return this.enabled && this.ownerEnabled;
			}
		}

		// Token: 0x0600509E RID: 20638 RVA: 0x00173484 File Offset: 0x00171684
		public static implicit operator bool(OneAxisInputControl instance)
		{
			return instance.State;
		}

		// Token: 0x0600509F RID: 20639 RVA: 0x0017348C File Offset: 0x0017168C
		public static implicit operator float(OneAxisInputControl instance)
		{
			return instance.Value;
		}

		// Token: 0x04005184 RID: 20868
		private float sensitivity = 1f;

		// Token: 0x04005185 RID: 20869
		private float lowerDeadZone;

		// Token: 0x04005186 RID: 20870
		private float upperDeadZone = 1f;

		// Token: 0x04005187 RID: 20871
		private float stateThreshold;

		// Token: 0x04005188 RID: 20872
		protected bool isNullControl;

		// Token: 0x04005189 RID: 20873
		public float FirstRepeatDelay = 0.8f;

		// Token: 0x0400518A RID: 20874
		public float RepeatDelay = 0.1f;

		// Token: 0x0400518B RID: 20875
		public bool Raw;

		// Token: 0x0400518C RID: 20876
		private bool enabled = true;

		// Token: 0x0400518D RID: 20877
		protected bool ownerEnabled = true;

		// Token: 0x0400518E RID: 20878
		private ulong pendingTick;

		// Token: 0x0400518F RID: 20879
		private bool pendingCommit;

		// Token: 0x04005190 RID: 20880
		private float nextRepeatTime;

		// Token: 0x04005191 RID: 20881
		private bool wasRepeated;

		// Token: 0x04005192 RID: 20882
		private bool clearInputState;

		// Token: 0x04005193 RID: 20883
		private InputControlState lastState;

		// Token: 0x04005194 RID: 20884
		private InputControlState nextState;

		// Token: 0x04005195 RID: 20885
		private InputControlState thisState;
	}
}
