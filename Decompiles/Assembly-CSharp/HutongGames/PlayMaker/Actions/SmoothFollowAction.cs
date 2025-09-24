using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010E9 RID: 4329
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Action version of Unity's Smooth Follow script.")]
	public class SmoothFollowAction : FsmStateAction
	{
		// Token: 0x06007523 RID: 29987 RVA: 0x0023D140 File Offset: 0x0023B340
		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.distance = 10f;
			this.height = 5f;
			this.heightDamping = 2f;
			this.rotationDamping = 3f;
		}

		// Token: 0x06007524 RID: 29988 RVA: 0x0023D19B File Offset: 0x0023B39B
		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		// Token: 0x06007525 RID: 29989 RVA: 0x0023D1AC File Offset: 0x0023B3AC
		public override void OnLateUpdate()
		{
			if (this.targetObject.Value == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.cachedObject != ownerDefaultTarget)
			{
				this.cachedObject = ownerDefaultTarget;
				this.myTransform = ownerDefaultTarget.transform;
			}
			if (this.cachedTarget != this.targetObject.Value)
			{
				this.cachedTarget = this.targetObject.Value;
				this.targetTransform = this.cachedTarget.transform;
			}
			float y = this.targetTransform.eulerAngles.y;
			float b = this.targetTransform.position.y + this.height.Value;
			float num = this.myTransform.eulerAngles.y;
			float num2 = this.myTransform.position.y;
			num = Mathf.LerpAngle(num, y, this.rotationDamping.Value * Time.deltaTime);
			num2 = Mathf.Lerp(num2, b, this.heightDamping.Value * Time.deltaTime);
			Quaternion rotation = Quaternion.Euler(0f, num, 0f);
			this.myTransform.position = this.targetTransform.position;
			this.myTransform.position -= rotation * Vector3.forward * this.distance.Value;
			this.myTransform.position = new Vector3(this.myTransform.position.x, num2, this.myTransform.position.z);
			this.myTransform.LookAt(this.targetTransform);
		}

		// Token: 0x04007582 RID: 30082
		[RequiredField]
		[Tooltip("The game object to control. E.g. The camera.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007583 RID: 30083
		[Tooltip("The GameObject to follow.")]
		public FsmGameObject targetObject;

		// Token: 0x04007584 RID: 30084
		[RequiredField]
		[Tooltip("The distance in the x-z plane to the target.")]
		public FsmFloat distance;

		// Token: 0x04007585 RID: 30085
		[RequiredField]
		[Tooltip("The height we want the camera to be above the target")]
		public FsmFloat height;

		// Token: 0x04007586 RID: 30086
		[RequiredField]
		[Tooltip("How much to dampen height movement.")]
		public FsmFloat heightDamping;

		// Token: 0x04007587 RID: 30087
		[RequiredField]
		[Tooltip("How much to dampen rotation changes.")]
		public FsmFloat rotationDamping;

		// Token: 0x04007588 RID: 30088
		private GameObject cachedObject;

		// Token: 0x04007589 RID: 30089
		private Transform myTransform;

		// Token: 0x0400758A RID: 30090
		private GameObject cachedTarget;

		// Token: 0x0400758B RID: 30091
		private Transform targetTransform;
	}
}
