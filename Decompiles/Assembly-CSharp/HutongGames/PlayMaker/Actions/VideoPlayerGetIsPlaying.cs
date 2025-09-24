using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011C5 RID: 4549
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check Whether content is being played. (Read Only)")]
	public class VideoPlayerGetIsPlaying : FsmStateAction
	{
		// Token: 0x06007979 RID: 31097 RVA: 0x0024A7DE File Offset: 0x002489DE
		public override void Reset()
		{
			this.gameObject = null;
			this.isPlaying = null;
			this.isPlayingEvent = null;
			this.isNotPlayingEvent = null;
		}

		// Token: 0x0600797A RID: 31098 RVA: 0x0024A7FC File Offset: 0x002489FC
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
		}

		// Token: 0x0600797B RID: 31099 RVA: 0x0024A80A File Offset: 0x00248A0A
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x0600797C RID: 31100 RVA: 0x0024A814 File Offset: 0x00248A14
		private void ExecuteAction()
		{
			if (this._vp == null)
			{
				return;
			}
			if (this._vp.isPlaying)
			{
				this.isPlaying.Value = true;
				if (this._isPlaying != 1)
				{
					base.Fsm.Event(this.isPlayingEvent);
				}
				this._isPlaying = 1;
				return;
			}
			this.isPlaying.Value = false;
			if (this._isPlaying != 0)
			{
				base.Fsm.Event(this.isNotPlayingEvent);
			}
			this._isPlaying = 0;
		}

		// Token: 0x0600797D RID: 31101 RVA: 0x0024A897 File Offset: 0x00248A97
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x040079D9 RID: 31193
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040079DA RID: 31194
		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isPlaying;

		// Token: 0x040079DB RID: 31195
		[Tooltip("Event sent if content is playing")]
		public FsmEvent isPlayingEvent;

		// Token: 0x040079DC RID: 31196
		[Tooltip("Event sent if content is not playing")]
		public FsmEvent isNotPlayingEvent;

		// Token: 0x040079DD RID: 31197
		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		// Token: 0x040079DE RID: 31198
		private GameObject go;

		// Token: 0x040079DF RID: 31199
		private VideoPlayer _vp;

		// Token: 0x040079E0 RID: 31200
		private int _isPlaying = -1;
	}
}
