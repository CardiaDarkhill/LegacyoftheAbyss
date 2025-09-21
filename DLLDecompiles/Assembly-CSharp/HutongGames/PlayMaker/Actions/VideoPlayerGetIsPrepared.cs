using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011C6 RID: 4550
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Check Whether the player has successfully prepared the content to be played. (Read Only)")]
	public class VideoPlayerGetIsPrepared : FsmStateAction
	{
		// Token: 0x0600797F RID: 31103 RVA: 0x0024A8DE File Offset: 0x00248ADE
		public override void Reset()
		{
			this.gameObject = null;
			this.isPrepared = null;
			this.isPreparedEvent = null;
			this.isNotPreparedEvent = null;
		}

		// Token: 0x06007980 RID: 31104 RVA: 0x0024A8FC File Offset: 0x00248AFC
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
		}

		// Token: 0x06007981 RID: 31105 RVA: 0x0024A90A File Offset: 0x00248B0A
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007982 RID: 31106 RVA: 0x0024A914 File Offset: 0x00248B14
		private void ExecuteAction()
		{
			if (this._vp == null)
			{
				return;
			}
			if (this._vp.isPrepared)
			{
				this.isPrepared.Value = true;
				if (this._isPrepared != 1)
				{
					base.Fsm.Event(this.isPreparedEvent);
				}
				this._isPrepared = 1;
				return;
			}
			this.isPrepared.Value = false;
			if (this._isPrepared != 0)
			{
				base.Fsm.Event(this.isNotPreparedEvent);
			}
			this._isPrepared = 0;
		}

		// Token: 0x06007983 RID: 31107 RVA: 0x0024A997 File Offset: 0x00248B97
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x040079E1 RID: 31201
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040079E2 RID: 31202
		[Tooltip("The Value")]
		[UIHint(UIHint.Variable)]
		public FsmBool isPrepared;

		// Token: 0x040079E3 RID: 31203
		[Tooltip("Event sent if content is prepared")]
		public FsmEvent isPreparedEvent;

		// Token: 0x040079E4 RID: 31204
		[Tooltip("Event sent if content is not yet prepared")]
		public FsmEvent isNotPreparedEvent;

		// Token: 0x040079E5 RID: 31205
		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		// Token: 0x040079E6 RID: 31206
		private GameObject go;

		// Token: 0x040079E7 RID: 31207
		private VideoPlayer _vp;

		// Token: 0x040079E8 RID: 31208
		private int _isPrepared = -1;
	}
}
