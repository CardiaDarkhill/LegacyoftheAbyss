using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001304 RID: 4868
	public class CogEnergyTimelineSetEnergy : FsmStateAction
	{
		// Token: 0x06007E89 RID: 32393 RVA: 0x00259434 File Offset: 0x00257634
		public override void Reset()
		{
			this.Target = null;
			this.Energy = null;
			this.Animate = null;
			this.EveryFrame = false;
		}

		// Token: 0x06007E8A RID: 32394 RVA: 0x00259454 File Offset: 0x00257654
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (!safe)
			{
				base.Finish();
				return;
			}
			this.timeline = safe.GetComponent<CogEnergyTimeline>();
			if (!this.timeline)
			{
				base.Finish();
				return;
			}
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007E8B RID: 32395 RVA: 0x002594B1 File Offset: 0x002576B1
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007E8C RID: 32396 RVA: 0x002594B9 File Offset: 0x002576B9
		private void DoAction()
		{
			this.timeline.SetEnergy(this.Energy.Value, this.Animate.Value);
		}

		// Token: 0x04007E47 RID: 32327
		[CheckForComponent(typeof(CogEnergyTimeline))]
		public FsmOwnerDefault Target;

		// Token: 0x04007E48 RID: 32328
		public FsmFloat Energy;

		// Token: 0x04007E49 RID: 32329
		public FsmBool Animate;

		// Token: 0x04007E4A RID: 32330
		public bool EveryFrame;

		// Token: 0x04007E4B RID: 32331
		private CogEnergyTimeline timeline;
	}
}
