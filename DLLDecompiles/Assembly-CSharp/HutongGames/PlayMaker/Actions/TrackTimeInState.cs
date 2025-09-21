using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D98 RID: 3480
	[ActionCategory(ActionCategory.Debug)]
	public class TrackTimeInState : FsmStateAction
	{
		// Token: 0x06006524 RID: 25892 RVA: 0x001FE5BF File Offset: 0x001FC7BF
		public override void OnEnter()
		{
			this.enterTime = Time.timeAsDouble;
		}

		// Token: 0x06006525 RID: 25893 RVA: 0x001FE5CC File Offset: 0x001FC7CC
		public override void OnExit()
		{
			double timeAsDouble = Time.timeAsDouble;
			double num = this.enterTime;
		}

		// Token: 0x04006421 RID: 25633
		private double enterTime;
	}
}
