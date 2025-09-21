using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C5C RID: 3164
	[ActionCategory(ActionCategory.Audio)]
	[Tooltip("Sets the Volume of the Audio Clip played by the AudioSource component on a Game Object.")]
	public class GetAudioVolume : ComponentAction<AudioSource>
	{
		// Token: 0x06005FBE RID: 24510 RVA: 0x001E5AF8 File Offset: 0x001E3CF8
		public override void Reset()
		{
			this.gameObject = null;
			this.storeVolume = null;
			this.everyFrame = false;
		}

		// Token: 0x06005FBF RID: 24511 RVA: 0x001E5B0F File Offset: 0x001E3D0F
		public override void OnEnter()
		{
			this.DoGetAudioVolume();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005FC0 RID: 24512 RVA: 0x001E5B25 File Offset: 0x001E3D25
		public override void OnUpdate()
		{
			this.DoGetAudioVolume();
		}

		// Token: 0x06005FC1 RID: 24513 RVA: 0x001E5B30 File Offset: 0x001E3D30
		private void DoGetAudioVolume()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.storeVolume.Value = base.audio.volume;
			}
		}

		// Token: 0x04005D17 RID: 23831
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005D18 RID: 23832
		public FsmFloat storeVolume;

		// Token: 0x04005D19 RID: 23833
		public bool everyFrame;
	}
}
