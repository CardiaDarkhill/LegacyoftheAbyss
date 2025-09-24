using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010D2 RID: 4306
	[ActionCategory(ActionCategory.Time)]
	[Tooltip("Scales time: 1 = normal, 0.5 = half speed, 2 = double speed.")]
	public class ScaleTime : FsmStateAction
	{
		// Token: 0x06007497 RID: 29847 RVA: 0x0023ACAC File Offset: 0x00238EAC
		public override void Reset()
		{
			this.timeScale = 1f;
			this.adjustFixedDeltaTime = true;
			this.everyFrame = false;
		}

		// Token: 0x06007498 RID: 29848 RVA: 0x0023ACD1 File Offset: 0x00238ED1
		public override void OnEnter()
		{
			this.DoTimeScale();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007499 RID: 29849 RVA: 0x0023ACE7 File Offset: 0x00238EE7
		public override void OnUpdate()
		{
			this.DoTimeScale();
		}

		// Token: 0x0600749A RID: 29850 RVA: 0x0023ACEF File Offset: 0x00238EEF
		private void DoTimeScale()
		{
			Time.timeScale = this.timeScale.Value;
			if (this.adjustFixedDeltaTime.Value)
			{
				Time.fixedDeltaTime = 0.02f * Time.timeScale;
			}
		}

		// Token: 0x040074D3 RID: 29907
		[RequiredField]
		[HasFloatSlider(0f, 4f)]
		[Tooltip("Scales time: 1 = normal, 0.5 = half speed, 2 = double speed.")]
		public FsmFloat timeScale;

		// Token: 0x040074D4 RID: 29908
		[Tooltip("Adjust the fixed physics time step to match the time scale.")]
		public FsmBool adjustFixedDeltaTime;

		// Token: 0x040074D5 RID: 29909
		[Tooltip("Repeat every frame. Useful when animating the value.")]
		public bool everyFrame;
	}
}
