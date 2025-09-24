using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001309 RID: 4873
	public class LookAnimNPCFaceTargetInstant : FSMUtility.GetComponentFsmStateAction<LookAnimNPC>
	{
		// Token: 0x06007E9B RID: 32411 RVA: 0x00259688 File Offset: 0x00257888
		public override void Reset()
		{
			base.Reset();
		}

		// Token: 0x06007E9C RID: 32412 RVA: 0x00259690 File Offset: 0x00257890
		protected override void DoAction(LookAnimNPC lookAnim)
		{
			lookAnim.FaceTargetInstant();
		}
	}
}
