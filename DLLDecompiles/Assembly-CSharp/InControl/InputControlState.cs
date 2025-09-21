using System;

namespace InControl
{
	// Token: 0x020008F7 RID: 2295
	public struct InputControlState
	{
		// Token: 0x0600506A RID: 20586 RVA: 0x00172C4C File Offset: 0x00170E4C
		public void Reset()
		{
			this.State = false;
			this.Value = 0f;
			this.RawValue = 0f;
		}

		// Token: 0x0600506B RID: 20587 RVA: 0x00172C6B File Offset: 0x00170E6B
		public void Set(float value)
		{
			this.Value = value;
			this.State = Utility.IsNotZero(value);
		}

		// Token: 0x0600506C RID: 20588 RVA: 0x00172C80 File Offset: 0x00170E80
		public void Set(float value, float threshold)
		{
			this.Value = value;
			this.State = Utility.AbsoluteIsOverThreshold(value, threshold);
		}

		// Token: 0x0600506D RID: 20589 RVA: 0x00172C96 File Offset: 0x00170E96
		public void Set(bool state)
		{
			this.State = state;
			this.Value = (state ? 1f : 0f);
			this.RawValue = this.Value;
		}

		// Token: 0x0600506E RID: 20590 RVA: 0x00172CC0 File Offset: 0x00170EC0
		public static implicit operator bool(InputControlState state)
		{
			return state.State;
		}

		// Token: 0x0600506F RID: 20591 RVA: 0x00172CC8 File Offset: 0x00170EC8
		public static implicit operator float(InputControlState state)
		{
			return state.Value;
		}

		// Token: 0x06005070 RID: 20592 RVA: 0x00172CD0 File Offset: 0x00170ED0
		public static bool operator ==(InputControlState a, InputControlState b)
		{
			return a.State == b.State && Utility.Approximately(a.Value, b.Value);
		}

		// Token: 0x06005071 RID: 20593 RVA: 0x00172CF3 File Offset: 0x00170EF3
		public static bool operator !=(InputControlState a, InputControlState b)
		{
			return a.State != b.State || !Utility.Approximately(a.Value, b.Value);
		}

		// Token: 0x040050D3 RID: 20691
		public bool State;

		// Token: 0x040050D4 RID: 20692
		public float Value;

		// Token: 0x040050D5 RID: 20693
		public float RawValue;
	}
}
