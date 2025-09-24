using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200120F RID: 4623
	public class FadeOutTransitionAudioFaders : FsmStateAction
	{
		// Token: 0x06007AD6 RID: 31446 RVA: 0x0024DC84 File Offset: 0x0024BE84
		public override void Reset()
		{
		}

		// Token: 0x06007AD7 RID: 31447 RVA: 0x0024DC86 File Offset: 0x0024BE86
		public override void OnEnter()
		{
			TransitionAudioFader.FadeOutAllFaders();
			base.Finish();
		}
	}
}
