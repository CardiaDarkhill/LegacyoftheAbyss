using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020013A2 RID: 5026
	[ActionCategory("Hollow Knight")]
	public class StopVibration : FsmStateAction
	{
		// Token: 0x060080EF RID: 33007 RVA: 0x0025FC66 File Offset: 0x0025DE66
		public override void Reset()
		{
			base.Reset();
			this.fsmTag = new FsmString
			{
				UseVariable = true
			};
		}

		// Token: 0x060080F0 RID: 33008 RVA: 0x0025FC80 File Offset: 0x0025DE80
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.fsmTag == null || this.fsmTag.IsNone || string.IsNullOrEmpty(this.fsmTag.Value))
			{
				VibrationManager.StopAllVibration();
			}
			else
			{
				VibrationMixer mixer = VibrationManager.GetMixer();
				if (mixer != null)
				{
					mixer.StopAllEmissionsWithTag(this.fsmTag.Value);
				}
			}
			base.Finish();
		}

		// Token: 0x04008030 RID: 32816
		private FsmString fsmTag;
	}
}
