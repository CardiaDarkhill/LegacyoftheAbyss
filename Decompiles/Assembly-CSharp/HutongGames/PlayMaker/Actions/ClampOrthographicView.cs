using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E4A RID: 3658
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Clamps an orthographic camera's position to keep the view inside min/max ranges. Set any limit to None to leave that axis un-clamped.")]
	public class ClampOrthographicView : ComponentAction<Camera>
	{
		// Token: 0x0600689C RID: 26780 RVA: 0x0020DB48 File Offset: 0x0020BD48
		public override void Reset()
		{
			this.gameObject = null;
			this.minX = new FsmFloat
			{
				UseVariable = true
			};
			this.maxX = new FsmFloat
			{
				UseVariable = true
			};
			this.minY = new FsmFloat
			{
				UseVariable = true
			};
			this.maxY = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
			this.lateUpdate = false;
		}

		// Token: 0x0600689D RID: 26781 RVA: 0x0020DBB2 File Offset: 0x0020BDB2
		public override void OnPreprocess()
		{
			if (this.lateUpdate)
			{
				base.Fsm.HandleLateUpdate = true;
			}
		}

		// Token: 0x0600689E RID: 26782 RVA: 0x0020DBC8 File Offset: 0x0020BDC8
		public override void OnEnter()
		{
			if (!this.everyFrame && !this.lateUpdate)
			{
				this.DoClampPosition();
				base.Finish();
			}
		}

		// Token: 0x0600689F RID: 26783 RVA: 0x0020DBE6 File Offset: 0x0020BDE6
		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoClampPosition();
			}
		}

		// Token: 0x060068A0 RID: 26784 RVA: 0x0020DBF6 File Offset: 0x0020BDF6
		public override void OnLateUpdate()
		{
			this.DoClampPosition();
			if (this.lateUpdate)
			{
				this.DoClampPosition();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068A1 RID: 26785 RVA: 0x0020DC1C File Offset: 0x0020BE1C
		private void DoClampPosition()
		{
			if (!base.UpdateCacheAndTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			Vector3 position = base.cachedTransform.position;
			float orthographicSize = base.camera.orthographicSize;
			float num = base.camera.orthographicSize * (float)Screen.width / (float)Screen.height;
			if (!this.minX.IsNone)
			{
				position.x = Mathf.Max(this.minX.Value + num, position.x);
			}
			if (!this.maxX.IsNone)
			{
				position.x = Mathf.Min(this.maxX.Value - num, position.x);
			}
			if (this.view == ClampOrthographicView.ScreenPlane.XY)
			{
				if (!this.minY.IsNone)
				{
					position.y = Mathf.Max(this.minY.Value + orthographicSize, position.y);
				}
				if (!this.maxY.IsNone)
				{
					position.y = Mathf.Min(this.maxY.Value - orthographicSize, position.y);
				}
			}
			else
			{
				if (!this.minY.IsNone)
				{
					position.z = Mathf.Max(this.minY.Value + orthographicSize, position.z);
				}
				if (!this.maxY.IsNone)
				{
					position.z = Mathf.Min(this.maxY.Value - orthographicSize, position.z);
				}
			}
			base.camera.transform.position = position;
		}

		// Token: 0x040067CC RID: 26572
		[RequiredField]
		[Tooltip("The GameObject with a Camera component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040067CD RID: 26573
		[Tooltip("Orientation of the view.")]
		public ClampOrthographicView.ScreenPlane view;

		// Token: 0x040067CE RID: 26574
		[Tooltip("The left edge of the view to stay inside.")]
		public FsmFloat minX;

		// Token: 0x040067CF RID: 26575
		[Tooltip("The right edge of the view to stay inside.")]
		public FsmFloat maxX;

		// Token: 0x040067D0 RID: 26576
		[Tooltip("The bottom edge of the view to stay inside.")]
		public FsmFloat minY;

		// Token: 0x040067D1 RID: 26577
		[Tooltip("The top edge of the view to stay inside.")]
		public FsmFloat maxY;

		// Token: 0x040067D2 RID: 26578
		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		// Token: 0x040067D3 RID: 26579
		[Tooltip("Perform in LateUpdate. This is useful if you want to clamp the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;

		// Token: 0x02001BA1 RID: 7073
		public enum ScreenPlane
		{
			// Token: 0x04009DFA RID: 40442
			XY,
			// Token: 0x04009DFB RID: 40443
			XZ
		}
	}
}
