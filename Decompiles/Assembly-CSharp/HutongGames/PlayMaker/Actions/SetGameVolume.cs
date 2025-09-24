using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E42 RID: 3650
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Sets the global sound volume.")]
	public class SetGameVolume : FsmStateAction
	{
		// Token: 0x0600687A RID: 26746 RVA: 0x0020D3F0 File Offset: 0x0020B5F0
		public override void Reset()
		{
			this.volume = 1f;
			this.everyFrame = false;
		}

		// Token: 0x0600687B RID: 26747 RVA: 0x0020D409 File Offset: 0x0020B609
		public override void OnEnter()
		{
			AudioListener.volume = this.volume.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600687C RID: 26748 RVA: 0x0020D429 File Offset: 0x0020B629
		public override void OnUpdate()
		{
			AudioListener.volume = this.volume.Value;
		}

		// Token: 0x040067A9 RID: 26537
		[RequiredField]
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Volume level (0-1).")]
		public FsmFloat volume;

		// Token: 0x040067AA RID: 26538
		[Tooltip("Perform this action every frame. Useful if Volume is changing e.g., to fade up/down.")]
		public bool everyFrame;
	}
}
