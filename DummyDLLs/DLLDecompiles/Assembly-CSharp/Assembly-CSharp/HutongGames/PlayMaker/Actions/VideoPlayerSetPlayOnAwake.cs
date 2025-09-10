using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011E0 RID: 4576
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Set Whether the content will start playing back as soon as the component awakes.")]
	public class VideoPlayerSetPlayOnAwake : FsmStateAction
	{
		// Token: 0x06007A13 RID: 31251 RVA: 0x0024BB62 File Offset: 0x00249D62
		public override void Reset()
		{
			this.gameObject = null;
			this.playOnAwake = null;
			this.everyframe = false;
		}

		// Token: 0x06007A14 RID: 31252 RVA: 0x0024BB79 File Offset: 0x00249D79
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyframe)
			{
				base.Finish();
			}
		}

		// Token: 0x06007A15 RID: 31253 RVA: 0x0024BB95 File Offset: 0x00249D95
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x06007A16 RID: 31254 RVA: 0x0024BB9D File Offset: 0x00249D9D
		private void ExecuteAction()
		{
			if (this._vp == null)
			{
				return;
			}
			this._vp.playOnAwake = this.playOnAwake.Value;
		}

		// Token: 0x06007A17 RID: 31255 RVA: 0x0024BBC4 File Offset: 0x00249DC4
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A67 RID: 31335
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A68 RID: 31336
		[Tooltip("The Value")]
		public FsmBool playOnAwake;

		// Token: 0x04007A69 RID: 31337
		[Tooltip("Execute action everyframe. Events are however sent discretly, only when changes occurs")]
		public bool everyframe;

		// Token: 0x04007A6A RID: 31338
		private GameObject go;

		// Token: 0x04007A6B RID: 31339
		private VideoPlayer _vp;
	}
}
