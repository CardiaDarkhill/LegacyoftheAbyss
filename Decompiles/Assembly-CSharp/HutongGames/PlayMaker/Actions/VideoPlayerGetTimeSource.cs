using System;
using UnityEngine;
using UnityEngine.Video;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011D3 RID: 4563
	[ActionCategory(ActionCategory.Video)]
	[Tooltip("Get The clock that the player follows to derive its current time")]
	public class VideoPlayerGetTimeSource : FsmStateAction
	{
		// Token: 0x060079CD RID: 31181 RVA: 0x0024B2EC File Offset: 0x002494EC
		public override void Reset()
		{
			this.gameObject = null;
			this.timeSource = null;
			this.everyFrame = false;
		}

		// Token: 0x060079CE RID: 31182 RVA: 0x0024B303 File Offset: 0x00249503
		public override void OnEnter()
		{
			this.GetVideoPlayer();
			this.ExecuteAction();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060079CF RID: 31183 RVA: 0x0024B31F File Offset: 0x0024951F
		public override void OnUpdate()
		{
			this.ExecuteAction();
		}

		// Token: 0x060079D0 RID: 31184 RVA: 0x0024B327 File Offset: 0x00249527
		private void ExecuteAction()
		{
			if (this._vp != null)
			{
				this.timeSource.Value = this._vp.timeUpdateMode;
			}
		}

		// Token: 0x060079D1 RID: 31185 RVA: 0x0024B352 File Offset: 0x00249552
		private void GetVideoPlayer()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go != null)
			{
				this._vp = this.go.GetComponent<VideoPlayer>();
			}
		}

		// Token: 0x04007A2E RID: 31278
		[RequiredField]
		[CheckForComponent(typeof(VideoPlayer))]
		[Tooltip("The GameObject with as VideoPlayer component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007A2F RID: 31279
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The time source type")]
		[ObjectType(typeof(VideoTimeUpdateMode))]
		public FsmEnum timeSource;

		// Token: 0x04007A30 RID: 31280
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04007A31 RID: 31281
		private GameObject go;

		// Token: 0x04007A32 RID: 31282
		private VideoPlayer _vp;
	}
}
