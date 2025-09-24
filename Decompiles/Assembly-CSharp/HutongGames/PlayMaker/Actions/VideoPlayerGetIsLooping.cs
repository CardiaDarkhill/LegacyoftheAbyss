using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011C4 RID: 4548
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check Whether the player restarts from the beginning without when it reaches the end of the clip.")]
	public class VideoPlayerGetIsLooping : FsmStateAction
	{
		// Token: 0x06007973 RID: 31091 RVA: 0x0024A6DF File Offset: 0x002488DF
		public override void Reset()
		{
			this.gameObject = null;
			this.isLooping = null;
			this.isLoopingEvent = null;
			this.isNotLoopingEvent = null;
		}

		// Token: 0x06007974 RID: 31092 RVA: 0x0024A6FD File Offset: 0x002488FD
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
		}

		// Token: 0x06007975 RID: 31093 RVA: 0x0024A70B File Offset: 0x0024890B
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007976 RID: 31094 RVA: 0x0024A714 File Offset: 0x00248914
		private void ExecuteAction()
		{
			if (this._vp == null)
			{
				return;
			}
			if (this._vp.isLooping)
			{
				this.isLooping.Value = true;
				if (this._isLooping != 1)
				{
					base.Fsm.Event(this.isLoopingEvent);
				}
				this._isLooping = 1;
				return;
			}
			this.isLooping.Value = false;
			if (this._isLooping != 0)
			{
				base.Fsm.Event(this.isNotLoopingEvent);
			}
			this._isLooping = 0;
		}

		// Token: 0x06007977 RID: 31095 RVA: 0x0024A797 File Offset: 0x00248997
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x040079D1 RID: 31185
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040079D2 RID: 31186
		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isLooping;

		// Token: 0x040079D3 RID: 31187
		[Tooltip("Event sent if content is looping")]
		public FsmEvent isLoopingEvent;

		// Token: 0x040079D4 RID: 31188
		[Tooltip("Event sent if content is not looping")]
		public FsmEvent isNotLoopingEvent;

		// Token: 0x040079D5 RID: 31189
		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		// Token: 0x040079D6 RID: 31190
		private GameObject go;

		// Token: 0x040079D7 RID: 31191
		private VideoPlayer _vp;

		// Token: 0x040079D8 RID: 31192
		private int _isLooping = -1;
	}
}
