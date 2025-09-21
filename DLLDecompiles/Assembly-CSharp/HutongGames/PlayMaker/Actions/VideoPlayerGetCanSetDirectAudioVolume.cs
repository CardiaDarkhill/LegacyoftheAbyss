using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011BA RID: 4538
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check whether direct-output volume controls are supported for the current platform and video format on a VideoPlayer. (Read Only)")]
	public class VideoPlayerGetCanSetDirectAudioVolume : FsmStateAction
	{
		// Token: 0x06007938 RID: 31032 RVA: 0x00249F49 File Offset: 0x00248149
		public override void Reset()
		{
			this.gameObject = null;
			this.canSetDirectAudioVolume = null;
			this.canSetDirectAudioVolumeEvent = null;
			this.canNotSetDirectAudioVolumeEvent = null;
			this.everyFrame = false;
		}

		// Token: 0x06007939 RID: 31033 RVA: 0x00249F6E File Offset: 0x0024816E
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600793A RID: 31034 RVA: 0x00249F8A File Offset: 0x0024818A
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x0600793B RID: 31035 RVA: 0x00249F94 File Offset: 0x00248194
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.canSetDirectAudioVolume.Value = this._vp.canSetDirectAudioVolume;
				base.Fsm.Event(this._vp.canSetTime ? this.canSetDirectAudioVolumeEvent : this.canNotSetDirectAudioVolumeEvent);
			}
		}

		// Token: 0x0600793C RID: 31036 RVA: 0x00249FEB File Offset: 0x002481EB
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007995 RID: 31125
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with a VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007996 RID: 31126
		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool canSetDirectAudioVolume;

		// Token: 0x04007997 RID: 31127
		[Tooltip("Event sent if DirectAudioVolume can be set")]
		public FsmEvent canSetDirectAudioVolumeEvent;

		// Token: 0x04007998 RID: 31128
		[Tooltip("Event sent if DirectAudioVolume can not be set")]
		public FsmEvent canNotSetDirectAudioVolumeEvent;

		// Token: 0x04007999 RID: 31129
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x0400799A RID: 31130
		private GameObject go;

		// Token: 0x0400799B RID: 31131
		private VideoPlayer _vp;
	}
}
