using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001025 RID: 4133
	[ActionCategory("RectTransform")]
	[Tooltip("Transform a screen space point to a local position that is on the plane of the given RectTransform. Also check if the plane of the RectTransform is hit, regardless of whether the point is inside the rectangle.")]
	public class RectTransformScreenPointToLocalPointInRectangle : FsmStateAction
	{
		// Token: 0x06007176 RID: 29046 RVA: 0x0022E768 File Offset: 0x0022C968
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
			this.localPosition = null;
			this.localPosition2d = null;
			this.isHit = null;
			this.hitEvent = null;
			this.noHitEvent = null;
		}

		// Token: 0x06007177 RID: 29047 RVA: 0x0022E7D8 File Offset: 0x0022C9D8
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != this.cachedGameObject)
			{
				this.cachedGameObject = ownerDefaultTarget;
				this._rt = ownerDefaultTarget.GetComponent<RectTransform>();
			}
			this._camera = ((!this.camera.IsNone) ? this.camera.Value.GetComponent<Camera>() : EventSystem.current.GetComponent<Camera>());
			this.DoCheck();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007178 RID: 29048 RVA: 0x0022E85B File Offset: 0x0022CA5B
		public override void OnUpdate()
		{
			this.DoCheck();
		}

		// Token: 0x06007179 RID: 29049 RVA: 0x0022E864 File Offset: 0x0022CA64
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
			Vector2 vector;
			bool flag = RectTransformUtility.ScreenPointToLocalPointInRectangle(this._rt, value, this._camera, out vector);
			if (!this.localPosition2d.IsNone)
			{
				this.localPosition2d.Value = vector;
			}
			if (!this.localPosition.IsNone)
			{
				this.localPosition.Value = new Vector3(vector.x, vector.y, 0f);
			}
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

		// Token: 0x04007120 RID: 28960
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007121 RID: 28961
		[Tooltip("The screenPoint as a Vector2. Leave as none if you want to use the Vector3 alternative")]
		public FsmVector2 screenPointVector2;

		// Token: 0x04007122 RID: 28962
		[Tooltip("The screenPoint as a Vector3. Leave as none if you want to use the Vector2 alternative")]
		public FsmVector3 orScreenPointVector3;

		// Token: 0x04007123 RID: 28963
		[Tooltip("Define if screenPoint are normalized screen coordinates (0-1). Otherwise coordinates are in pixels.")]
		public bool normalizedScreenPoint;

		// Token: 0x04007124 RID: 28964
		[Tooltip("The Camera. For a RectTransform in a Canvas set to Screen Space - Overlay mode, the cam parameter should be set to null explicitly (default).\nLeave to none and the camera will be the one from EventSystem.current.camera")]
		[CheckForComponent(typeof(Camera))]
		public FsmGameObject camera;

		// Token: 0x04007125 RID: 28965
		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		// Token: 0x04007126 RID: 28966
		[ActionSection("Result")]
		[Tooltip("Store the local Position as a vector3 of the screenPoint on the RectTransform Plane.")]
		[UIHint(UIHint.Variable)]
		public FsmVector3 localPosition;

		// Token: 0x04007127 RID: 28967
		[Tooltip("Store the local Position as a vector2 of the screenPoint on the RectTransform Plane.")]
		[UIHint(UIHint.Variable)]
		public FsmVector2 localPosition2d;

		// Token: 0x04007128 RID: 28968
		[Tooltip("True if the plane of the RectTransform is hit, regardless of whether the point is inside the rectangle.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isHit;

		// Token: 0x04007129 RID: 28969
		[Tooltip("Event sent if the plane of the RectTransform is hit, regardless of whether the point is inside the rectangle.")]
		public FsmEvent hitEvent;

		// Token: 0x0400712A RID: 28970
		[Tooltip("Event sent if the plane of the RectTransform is NOT hit, regardless of whether the point is inside the rectangle.")]
		public FsmEvent noHitEvent;

		// Token: 0x0400712B RID: 28971
		private GameObject cachedGameObject;

		// Token: 0x0400712C RID: 28972
		private RectTransform _rt;

		// Token: 0x0400712D RID: 28973
		private Camera _camera;
	}
}
