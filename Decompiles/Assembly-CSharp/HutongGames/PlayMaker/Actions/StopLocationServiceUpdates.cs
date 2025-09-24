using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E8F RID: 3727
	[ActionCategory(ActionCategory.Device)]
	[Tooltip("Stops location service updates. This could be useful for saving battery life.")]
	public class StopLocationServiceUpdates : FsmStateAction
	{
		// Token: 0x060069E0 RID: 27104 RVA: 0x00211CA7 File Offset: 0x0020FEA7
		public override void Reset()
		{
		}

		// Token: 0x060069E1 RID: 27105 RVA: 0x00211CA9 File Offset: 0x0020FEA9
		public override void OnEnter()
		{
			base.Finish();
		}
	}
}
