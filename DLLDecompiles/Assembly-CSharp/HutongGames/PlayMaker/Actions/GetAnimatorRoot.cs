using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E02 RID: 3586
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the avatar body mass center position and rotation.Optionally accept a GameObject to get the body transform. \nThe position and rotation are local to the gameobject")]
	public class GetAnimatorRoot : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BDC RID: 3036
		// (get) Token: 0x06006760 RID: 26464 RVA: 0x00209E7D File Offset: 0x0020807D
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x06006761 RID: 26465 RVA: 0x00209E85 File Offset: 0x00208085
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.rootPosition = null;
			this.rootRotation = null;
			this.bodyGameObject = null;
		}

		// Token: 0x06006762 RID: 26466 RVA: 0x00209EA9 File Offset: 0x002080A9
		public override void OnEnter()
		{
			this.DoGetBodyPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006763 RID: 26467 RVA: 0x00209EBF File Offset: 0x002080BF
		public override void OnActionUpdate()
		{
			this.DoGetBodyPosition();
		}

		// Token: 0x06006764 RID: 26468 RVA: 0x00209EC8 File Offset: 0x002080C8
		private void DoGetBodyPosition()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			if (this.cachedBodyGameObject != this.bodyGameObject.Value)
			{
				this.cachedBodyGameObject = this.bodyGameObject.Value;
				this._transform = ((this.cachedBodyGameObject != null) ? this.cachedBodyGameObject.transform : null);
			}
			this.rootPosition.Value = this.animator.rootPosition;
			this.rootRotation.Value = this.animator.rootRotation;
			if (this._transform != null)
			{
				this._transform.position = this.animator.rootPosition;
				this._transform.rotation = this.animator.rootRotation;
			}
		}

		// Token: 0x040066A8 RID: 26280
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066A9 RID: 26281
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The avatar body mass center")]
		public FsmVector3 rootPosition;

		// Token: 0x040066AA RID: 26282
		[UIHint(UIHint.Variable)]
		[Tooltip("The avatar body mass center")]
		public FsmQuaternion rootRotation;

		// Token: 0x040066AB RID: 26283
		[Tooltip("If set, apply the body mass center position and rotation to this gameObject")]
		public FsmGameObject bodyGameObject;

		// Token: 0x040066AC RID: 26284
		private GameObject cachedBodyGameObject;

		// Token: 0x040066AD RID: 26285
		private Transform _transform;
	}
}
