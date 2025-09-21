using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011C8 RID: 4552
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check Whether the content will start playing back as soon as the component awakes.")]
	public class VideoPlayerGetPlayOnAwake : FsmStateAction
	{
		// Token: 0x0600798B RID: 31115 RVA: 0x0024AA7F File Offset: 0x00248C7F
		public override void Reset()
		{
			this.gameObject = null;
			this.isPlayingOnAwake = null;
			this.isPlayingOnAwakeEvent = null;
			this.isNotPlayingOnAwakeEvent = null;
			this.everyframe = false;
		}

		// Token: 0x0600798C RID: 31116 RVA: 0x0024AAA4 File Offset: 0x00248CA4
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x0600798D RID: 31117 RVA: 0x0024AAC0 File Offset: 0x00248CC0
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x0600798E RID: 31118 RVA: 0x0024AAC8 File Offset: 0x00248CC8
		private void ExecuteAction()
		{
			if (this._vp == null)
			{
				return;
			}
			if (this._vp.playOnAwake)
			{
				this.isPlayingOnAwake.Value = true;
				if (this._isPlayingOnAwake != 1)
				{
					base.Fsm.Event(this.isPlayingOnAwakeEvent);
				}
				this._isPlayingOnAwake = 1;
				return;
			}
			this.isPlayingOnAwake.Value = false;
			if (this._isPlayingOnAwake != 0)
			{
				base.Fsm.Event(this.isNotPlayingOnAwakeEvent);
			}
			this._isPlayingOnAwake = 0;
		}

		// Token: 0x0600798F RID: 31119 RVA: 0x0024AB4B File Offset: 0x00248D4B
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x040079EE RID: 31214
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040079EF RID: 31215
		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isPlayingOnAwake;

		// Token: 0x040079F0 RID: 31216
		[Tooltip("Event sent if content content will start playing back as soon as the component awakes")]
		public FsmEvent isPlayingOnAwakeEvent;

		// Token: 0x040079F1 RID: 31217
		[Tooltip("Event sent if content will not start playing back as soon as the component awakes")]
		public FsmEvent isNotPlayingOnAwakeEvent;

		// Token: 0x040079F2 RID: 31218
		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		// Token: 0x040079F3 RID: 31219
		private GameObject go;

		// Token: 0x040079F4 RID: 31220
		private VideoPlayer _vp;

		// Token: 0x040079F5 RID: 31221
		private int _isPlayingOnAwake = -1;
	}
}
