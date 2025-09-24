using System;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011C0 RID: 4544
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Static property. Maximum number of audio tracks that can be controlled. When playing audio from a URL, the number of audio tracks is not known in advance. It is up to the user to specify the number of controlled audio tracks through VideoPlayer.controlledAudioTrackCount. Other tracks will be ignored and silenced. In this scenario, VideoPlayer.audioTrackCount will be set to the actual number of tracks during playback, after prepration is complete. See VideoPlayer.Prepare.")]
	public class VideoPlayerGetControlledAudioTrackMaxCount : FsmStateAction
	{
		// Token: 0x0600795C RID: 31068 RVA: 0x0024A4AF File Offset: 0x002486AF
		public override void Reset()
		{
			this.controlledAudioTrackMaxCount = null;
			this.everyFrame = false;
		}

		// Token: 0x0600795D RID: 31069 RVA: 0x0024A4BF File Offset: 0x002486BF
		public override void OnEnter()
		{
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600795E RID: 31070 RVA: 0x0024A4D5 File Offset: 0x002486D5
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x0600795F RID: 31071 RVA: 0x0024A4DD File Offset: 0x002486DD
		private void ExecuteAction()
		{
			this.controlledAudioTrackMaxCount.Value = (int)VideoPlayer.controlledAudioTrackMaxCount;
		}

		// Token: 0x040079C0 RID: 31168
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The maximum number of audio tracks that can be controlled")]
		public FsmInt controlledAudioTrackMaxCount;

		// Token: 0x040079C1 RID: 31169
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
