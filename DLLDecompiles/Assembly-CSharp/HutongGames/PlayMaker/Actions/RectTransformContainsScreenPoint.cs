using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001016 RID: 4118
	[ActionCategory("RectTransform")]
	[Tooltip("Check if a RectTransform contains the screen point as seen from the given camera.")]
	public class RectTransformContainsScreenPoint : FsmStateAction
	{
		// Token: 0x0600712C RID: 28972 RVA: 0x0022D58C File Offset: 0x0022B78C
		public override void Reset()
		{
			this.gameObject = null;
			this.screenPointVector2 = null;
			this.orScreenPointVector3 = new FsmVector3
			{
				UseVariable = true
			};
			this.normalizedScreenPoint = false;
			this.camera = null;
			this.everyFrame = false;
			this.isContained = null;
			this.isContainedEvent = null;
			this.isNotContainedEvent = null;
		}

		// Token: 0x0600712D RID: 28973 RVA: 0x0022D5E4 File Offset: 0x0022B7E4
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

		// Token: 0x0600712E RID: 28974 RVA: 0x0022D661 File Offset: 0x0022B861
		public override void OnUpdate()
		{
			this.DoCheck();
		}

		// Token: 0x0600712F RID: 28975 RVA: 0x0022D66C File Offset: 0x0022B86C
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
			bool flag = RectTransformUtility.RectangleContainsScreenPoint(this._rt, value, this._camera);
			if (!this.isContained.IsNone)
			{
				this.isContained.Value = flag;
			}
			if (flag)
			{
				if (this.isContainedEvent != null)
				{
					base.Fsm.Event(this.isContainedEvent);
					return;
				}
			}
			else if (this.isNotContainedEvent != null)
			{
				base.Fsm.Event(this.isNotContainedEvent);
			}
		}

		// Token: 0x040070C6 RID: 28870
		[RequiredField]
		[CheckForComponent(typeof(RectTransform))]
		[Tooltip("The GameObject target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040070C7 RID: 28871
		[Tooltip("The screenPoint as a Vector2. Leave to none if you want to use the Vector3 alternative")]
		public FsmVector2 screenPointVector2;

		// Token: 0x040070C8 RID: 28872
		[Tooltip("The screenPoint as a Vector3. Leave to none if you want to use the Vector2 alternative")]
		public FsmVector3 orScreenPointVector3;

		// Token: 0x040070C9 RID: 28873
		[Tooltip("Define if screenPoint are expressed as normalized screen coordinates (0-1). Otherwise coordinates are in pixels.")]
		public bool normalizedScreenPoint;

		// Token: 0x040070CA RID: 28874
		[Tooltip("The Camera. For a RectTransform in a Canvas set to Screen Space - Overlay mode, the cam parameter should be set to null explicitly (default).\nLeave to none and the camera will be the one from EventSystem.current.camera")]
		[CheckForComponent(typeof(Camera))]
		public FsmGameObject camera;

		// Token: 0x040070CB RID: 28875
		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		// Token: 0x040070CC RID: 28876
		[ActionSection("Result")]
		[Tooltip("Store the result.")]
		[UIHint(UIHint.Variable)]
		public FsmBool isContained;

		// Token: 0x040070CD RID: 28877
		[Tooltip("Event sent if screenPoint is contained in RectTransform.")]
		public FsmEvent isContainedEvent;

		// Token: 0x040070CE RID: 28878
		[Tooltip("Event sent if screenPoint is NOT contained in RectTransform.")]
		public FsmEvent isNotContainedEvent;

		// Token: 0x040070CF RID: 28879
		private RectTransform _rt;

		// Token: 0x040070D0 RID: 28880
		private Camera _camera;
	}
}
