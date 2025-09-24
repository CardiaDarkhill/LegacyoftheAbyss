using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010CF RID: 4303
	[ActionCategory(ActionCategory.Time)]
	[Tooltip("Gets various useful Time measurements.")]
	public class GetTimeInfo : FsmStateAction
	{
		// Token: 0x06007489 RID: 29833 RVA: 0x0023AA10 File Offset: 0x00238C10
		public override void Reset()
		{
			this.getInfo = GetTimeInfo.TimeInfo.TimeSinceLevelLoad;
			this.storeValue = null;
			this.everyFrame = false;
		}

		// Token: 0x0600748A RID: 29834 RVA: 0x0023AA27 File Offset: 0x00238C27
		public override void OnEnter()
		{
			this.DoGetTimeInfo();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600748B RID: 29835 RVA: 0x0023AA3D File Offset: 0x00238C3D
		public override void OnUpdate()
		{
			this.DoGetTimeInfo();
		}

		// Token: 0x0600748C RID: 29836 RVA: 0x0023AA48 File Offset: 0x00238C48
		private void DoGetTimeInfo()
		{
			switch (this.getInfo)
			{
			case GetTimeInfo.TimeInfo.DeltaTime:
				this.storeValue.Value = Time.deltaTime;
				return;
			case GetTimeInfo.TimeInfo.TimeScale:
				this.storeValue.Value = Time.timeScale;
				return;
			case GetTimeInfo.TimeInfo.SmoothDeltaTime:
				this.storeValue.Value = Time.smoothDeltaTime;
				return;
			case GetTimeInfo.TimeInfo.TimeInCurrentState:
				this.storeValue.Value = base.State.StateTime;
				return;
			case GetTimeInfo.TimeInfo.TimeSinceStartup:
				this.storeValue.Value = Time.time;
				return;
			case GetTimeInfo.TimeInfo.TimeSinceLevelLoad:
				this.storeValue.Value = Time.timeSinceLevelLoad;
				return;
			case GetTimeInfo.TimeInfo.RealTimeSinceStartup:
				this.storeValue.Value = FsmTime.RealtimeSinceStartup;
				return;
			case GetTimeInfo.TimeInfo.RealTimeInCurrentState:
				this.storeValue.Value = FsmTime.RealtimeSinceStartup - base.State.RealStartTime;
				return;
			default:
				this.storeValue.Value = 0f;
				return;
			}
		}

		// Token: 0x040074C6 RID: 29894
		[Tooltip("Info to get.")]
		public GetTimeInfo.TimeInfo getInfo;

		// Token: 0x040074C7 RID: 29895
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the time info in a float variable.")]
		public FsmFloat storeValue;

		// Token: 0x040074C8 RID: 29896
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x02001BC8 RID: 7112
		public enum TimeInfo
		{
			// Token: 0x04009EB6 RID: 40630
			DeltaTime,
			// Token: 0x04009EB7 RID: 40631
			TimeScale,
			// Token: 0x04009EB8 RID: 40632
			SmoothDeltaTime,
			// Token: 0x04009EB9 RID: 40633
			TimeInCurrentState,
			// Token: 0x04009EBA RID: 40634
			TimeSinceStartup,
			// Token: 0x04009EBB RID: 40635
			TimeSinceLevelLoad,
			// Token: 0x04009EBC RID: 40636
			RealTimeSinceStartup,
			// Token: 0x04009EBD RID: 40637
			RealTimeInCurrentState
		}
	}
}
