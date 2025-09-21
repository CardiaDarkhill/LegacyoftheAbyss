using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC8 RID: 3272
	public class ObjectJitterOnRender : FsmStateAction
	{
		// Token: 0x060061A4 RID: 24996 RVA: 0x001EECE0 File Offset: 0x001ECEE0
		public override void Reset()
		{
			this.Target = null;
			this.X = new FsmFloat
			{
				UseVariable = true
			};
			this.Y = new FsmFloat
			{
				UseVariable = true
			};
			this.Z = new FsmFloat
			{
				UseVariable = true
			};
			this.LimitFps = null;
		}

		// Token: 0x060061A5 RID: 24997 RVA: 0x001EED34 File Offset: 0x001ECF34
		public override void OnEnter()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.Target);
			if (this.go == null)
			{
				base.Finish();
				return;
			}
			this.initialPos = this.go.transform.localPosition;
			this.OnUpdate();
			CameraRenderHooks.CameraPreCull += this.OnCameraPreCull;
			CameraRenderHooks.CameraPostRender += this.OnCameraPostRender;
		}

		// Token: 0x060061A6 RID: 24998 RVA: 0x001EEDAB File Offset: 0x001ECFAB
		public override void OnExit()
		{
			CameraRenderHooks.CameraPreCull -= this.OnCameraPreCull;
			CameraRenderHooks.CameraPostRender -= this.OnCameraPostRender;
		}

		// Token: 0x060061A7 RID: 24999 RVA: 0x001EEDD0 File Offset: 0x001ECFD0
		public override void OnUpdate()
		{
			if (this.LimitFps.Value > 0f)
			{
				double timeAsDouble = Time.timeAsDouble;
				if (timeAsDouble < this.nextUpdateTime)
				{
					return;
				}
				this.nextUpdateTime = timeAsDouble + (double)(1f / this.LimitFps.Value);
			}
			this.targetPos = this.initialPos + new Vector3(Random.Range(-this.X.Value, this.X.Value), Random.Range(-this.Y.Value, this.Y.Value), Random.Range(-this.Z.Value, this.Z.Value));
		}

		// Token: 0x060061A8 RID: 25000 RVA: 0x001EEE84 File Offset: 0x001ED084
		private void OnCameraPreCull(CameraRenderHooks.CameraSource cameraType)
		{
			if (cameraType != CameraRenderHooks.CameraSource.MainCamera)
			{
				return;
			}
			if (Time.timeScale <= Mathf.Epsilon)
			{
				return;
			}
			Transform transform = this.go.transform;
			this.preCullPos = transform.localPosition;
			transform.localPosition = this.targetPos;
		}

		// Token: 0x060061A9 RID: 25001 RVA: 0x001EEEC7 File Offset: 0x001ED0C7
		private void OnCameraPostRender(CameraRenderHooks.CameraSource cameraType)
		{
			if (cameraType != CameraRenderHooks.CameraSource.MainCamera)
			{
				return;
			}
			if (Time.timeScale <= Mathf.Epsilon)
			{
				return;
			}
			this.go.transform.localPosition = this.preCullPos;
			this.initialPos = this.preCullPos;
		}

		// Token: 0x04005FD4 RID: 24532
		[RequiredField]
		public FsmOwnerDefault Target;

		// Token: 0x04005FD5 RID: 24533
		public FsmFloat X;

		// Token: 0x04005FD6 RID: 24534
		public FsmFloat Y;

		// Token: 0x04005FD7 RID: 24535
		public FsmFloat Z;

		// Token: 0x04005FD8 RID: 24536
		public FsmFloat LimitFps;

		// Token: 0x04005FD9 RID: 24537
		private GameObject go;

		// Token: 0x04005FDA RID: 24538
		private Vector3 initialPos;

		// Token: 0x04005FDB RID: 24539
		private Vector3 preCullPos;

		// Token: 0x04005FDC RID: 24540
		private Vector3 targetPos;

		// Token: 0x04005FDD RID: 24541
		private double nextUpdateTime;
	}
}
