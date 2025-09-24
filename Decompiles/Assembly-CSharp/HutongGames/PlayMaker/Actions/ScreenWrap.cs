using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001057 RID: 4183
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Wraps a GameObject's position across screen edges. For example, a GameObject that moves off the left side of the screen wraps to the right side. This is often used in 2d arcade style games like Asteroids.")]
	public class ScreenWrap : ComponentAction<Transform, Camera>
	{
		// Token: 0x17000C03 RID: 3075
		// (get) Token: 0x06007265 RID: 29285 RVA: 0x0023246B File Offset: 0x0023066B
		private Camera cameraComponent
		{
			get
			{
				return this.cachedComponent2;
			}
		}

		// Token: 0x17000C04 RID: 3076
		// (get) Token: 0x06007266 RID: 29286 RVA: 0x00232473 File Offset: 0x00230673
		private Transform cameraTransform
		{
			get
			{
				return this.cachedTransform2;
			}
		}

		// Token: 0x17000C05 RID: 3077
		// (get) Token: 0x06007267 RID: 29287 RVA: 0x0023247B File Offset: 0x0023067B
		private Transform gameObjectTransform
		{
			get
			{
				return this.cachedComponent1;
			}
		}

		// Token: 0x06007268 RID: 29288 RVA: 0x00232484 File Offset: 0x00230684
		public override void Reset()
		{
			this.gameObject = null;
			this.wrapLeft = new FsmBool
			{
				Value = true
			};
			this.wrapRight = new FsmBool
			{
				Value = true
			};
			this.wrapTop = new FsmBool
			{
				Value = true
			};
			this.wrapBottom = new FsmBool
			{
				Value = true
			};
			this.everyFrame = true;
			this.lateUpdate = true;
		}

		// Token: 0x06007269 RID: 29289 RVA: 0x002324EE File Offset: 0x002306EE
		public override void OnPreprocess()
		{
			if (this.lateUpdate)
			{
				base.Fsm.HandleLateUpdate = true;
			}
		}

		// Token: 0x0600726A RID: 29290 RVA: 0x00232504 File Offset: 0x00230704
		public override void OnEnter()
		{
			if (!this.everyFrame && !this.lateUpdate)
			{
				this.DoScreenWrap();
				base.Finish();
			}
		}

		// Token: 0x0600726B RID: 29291 RVA: 0x00232522 File Offset: 0x00230722
		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoScreenWrap();
			}
		}

		// Token: 0x0600726C RID: 29292 RVA: 0x00232532 File Offset: 0x00230732
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoScreenWrap();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600726D RID: 29293 RVA: 0x00232550 File Offset: 0x00230750
		private void DoScreenWrap()
		{
			if (this.camera.Value == null)
			{
				this.camera.Value = ((Camera.main != null) ? Camera.main.gameObject : null);
			}
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.camera.Value))
			{
				return;
			}
			Vector3 vector = this.cameraComponent.WorldToViewportPoint(this.gameObjectTransform.position);
			bool flag = false;
			if ((this.wrapLeft.Value && vector.x < 0f) || (this.wrapRight.Value && vector.x >= 1f))
			{
				vector.x = ScreenWrap.Wrap01(vector.x);
				flag = true;
			}
			if ((this.wrapTop.Value && vector.y >= 1f) || (this.wrapBottom.Value && vector.y < 0f))
			{
				vector.y = ScreenWrap.Wrap01(vector.y);
				flag = true;
			}
			if (flag)
			{
				vector.z = this.cameraTransform.InverseTransformPoint(this.gameObjectTransform.position).z;
				this.gameObjectTransform.position = this.cameraComponent.ViewportToWorldPoint(vector);
			}
		}

		// Token: 0x0600726E RID: 29294 RVA: 0x0023269F File Offset: 0x0023089F
		private static float Wrap01(float x)
		{
			return ScreenWrap.Wrap(x, 0f, 1f);
		}

		// Token: 0x0600726F RID: 29295 RVA: 0x002326B1 File Offset: 0x002308B1
		private static float Wrap(float x, float xMin, float xMax)
		{
			if (x < xMin)
			{
				x = xMax - (xMin - x) % (xMax - xMin);
			}
			else
			{
				x = xMin + (x - xMin) % (xMax - xMin);
			}
			return x;
		}

		// Token: 0x04007256 RID: 29270
		[RequiredField]
		[Tooltip("The GameObject to wrap.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007257 RID: 29271
		[CheckForComponent(typeof(Camera))]
		[Tooltip("GameObject with a Camera component used to render the view (or MainCamera if not set). The Viewport Rect is used for wrapping.")]
		public FsmGameObject camera;

		// Token: 0x04007258 RID: 29272
		[Tooltip("Wrap the position of the GameObject if it moves off the left side of the screen.")]
		public FsmBool wrapLeft;

		// Token: 0x04007259 RID: 29273
		[Tooltip("Wrap the position of the GameObject if it moves off the right side of the screen.")]
		public FsmBool wrapRight;

		// Token: 0x0400725A RID: 29274
		[Tooltip("Wrap the position of the GameObject if it moves off the top of the screen.")]
		public FsmBool wrapTop;

		// Token: 0x0400725B RID: 29275
		[Tooltip("Wrap the position of the GameObject if it moves off the top of the screen.")]
		public FsmBool wrapBottom;

		// Token: 0x0400725C RID: 29276
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x0400725D RID: 29277
		[Tooltip("Use LateUpdate. Useful if you want to wrap after any other operations in Update.")]
		public bool lateUpdate;
	}
}
