using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001026 RID: 4134
	[ActionCategory("RectTransform")]
	[Tooltip("Transform a screen space point to a world position that is on the plane of the given RectTransform. Also check if the plane of the RectTransform is hit, regardless of whether the point is inside the rectangle.")]
	public class RectTransformScreenPointToWorldPointInRectangle : FsmStateAction
	{
		// Token: 0x0600717B RID: 29051 RVA: 0x0022E9A8 File Offset: 0x0022CBA8
		public override void Reset()
		{
			this.gameObject = null;
			this.screenPointVector2 = null;
			this.orScreenPointVector3 = new FsmVector3
			{
				UseVariable = true
			};
			this.normalizedScreenPoint = false;
			this.camera = new FsmGameObject
			{
				UseVariable = true
			};
			this.everyFrame = false;
			this.worldPosition = null;
			this.isHit = null;
			this.hitEvent = null;
			this.noHitEvent = null;
		}

		// Token: 0x0600717C RID: 29052 RVA: 0x0022EA14 File Offset: 0x0022CC14
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			if (!this.camera.IsNone)
			{
				this._camera = this.camera.Value.GetComponent<Camera>();
			}
			else
			{
				this._camera = EventSystem.current.GetComponent<Camera>();
			}
			this.DoCheck();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600717D RID: 29053 RVA: 0x0022EA91 File Offset: 0x0022CC91
		public override void OnUpdate()
		{
			this.DoCheck();
		}

		// Token: 0x0600717E RID: 29054 RVA: 0x0022EA9C File Offset: 0x0022CC9C
		private void DoCheck()
		{
			if (this._rt == null)
			{
				return;
			}
			Vector2 value = this.screenPointVector2.Value;
			if (!this.orScreenPointVector3.IsNone)
			{
				value.x = this.orScreenPointVector3.Value.x;
				value.y = this.orScreenPointVector3.Value.y;
			}
			if (this.normalizedScreenPoint)
			{
				value.x *= (float)Screen.width;
				value.y *= (float)Screen.height;
			}
			Vector3 value2;
			bool flag = RectTransformUtility.ScreenPointToWorldPointInRectangle(this._rt, value, this._camera, out value2);
			this.worldPosition.Value = value2;
			if (!this.isHit.IsNone)
			{
				this.isHit.Value = flag;
			}
			if (flag)
			{
				if (this.hitEvent != null)
				{
					base.Fsm.Event(this.hitEvent);
					return;
				}
			}
			else if (this.noHitEvent != null)
			{
				base.Fsm.Event(this.noHitEvent);
			}
		}

		// Token: 0x0400712E RID: 28974
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400712F RID: 28975
		[Tooltip("The screenPoint as a Vector2. Leave to none if you want to use the Vector3 alternative")]
		public FsmVector2 screenPointVector2;

		// Token: 0x04007130 RID: 28976
		[Tooltip("The screenPoint as a Vector3. Leave to none if you want to use the Vector2 alternative")]
		public FsmVector3 orScreenPointVector3;

		// Token: 0x04007131 RID: 28977
		[Tooltip("Define if screenPoint are expressed as normalized screen coordinates (0-1). Otherwise coordinates are in pixels.")]
		public bool normalizedScreenPoint;

		// Token: 0x04007132 RID: 28978
		[Tooltip("The Camera. For a RectTransform in a Canvas set to Screen Space - Overlay mode, the cam parameter should be set to null explicitly (default).\nLeave to none and the camera will be the one from EventSystem.current.camera")]
		[CheckForComponent(typeof(Camera))]
		public FsmGameObject camera;

		// Token: 0x04007133 RID: 28979
		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		// Token: 0x04007134 RID: 28980
		[ActionSection("Result")]
		[Tooltip("Store the world Position of the screenPoint on the RectTransform Plane.")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 worldPosition;

		// Token: 0x04007135 RID: 28981
		[Tooltip("True if the plane of the RectTransform is hit, regardless of whether the point is inside the rectangle.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isHit;

		// Token: 0x04007136 RID: 28982
		[Tooltip("Event sent if the plane of the RectTransform is hit, regardless of whether the point is inside the rectangle.")]
		public FsmEvent hitEvent;

		// Token: 0x04007137 RID: 28983
		[Tooltip("Event sent if the plane of the RectTransform is NOT hit, regardless of whether the point is inside the rectangle.")]
		public FsmEvent noHitEvent;

		// Token: 0x04007138 RID: 28984
		private RectTransform _rt;

		// Token: 0x04007139 RID: 28985
		private Camera _camera;
	}
}
