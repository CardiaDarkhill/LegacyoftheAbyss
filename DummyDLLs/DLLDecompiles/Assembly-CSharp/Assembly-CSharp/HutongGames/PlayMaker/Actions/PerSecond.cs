using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010D0 RID: 4304
	[ActionCategory(ActionCategory.Time)]
	[Tooltip("Multiplies a Float by Time.deltaTime to use in frame-rate independent operations. E.g., 10 becomes 10 units per second.")]
	public class PerSecond : FsmStateAction
	{
		// Token: 0x0600748E RID: 29838 RVA: 0x0023AB39 File Offset: 0x00238D39
		public override void Reset()
		{
			this.floatValue = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x0600748F RID: 29839 RVA: 0x0023AB50 File Offset: 0x00238D50
		public override void OnEnter()
		{
			this.DoPerSecond();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007490 RID: 29840 RVA: 0x0023AB66 File Offset: 0x00238D66
		public override void OnUpdate()
		{
			this.DoPerSecond();
		}

		// Token: 0x06007491 RID: 29841 RVA: 0x0023AB6E File Offset: 0x00238D6E
		private void DoPerSecond()
		{
			if (this.storeResult == null)
			{
				return;
			}
			this.storeResult.Value = this.floatValue.Value * Time.deltaTime;
		}

		// Token: 0x040074C9 RID: 29897
		[RequiredField]
		[Tooltip("The float value to multiply be Time.deltaTime.")]
		public FsmFloat floatValue;

		// Token: 0x040074CA RID: 29898
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a float variable.")]
		public FsmFloat storeResult;

		// Token: 0x040074CB RID: 29899
		[Tooltip("Do it every frame.")]
		public bool everyFrame;
	}
}
