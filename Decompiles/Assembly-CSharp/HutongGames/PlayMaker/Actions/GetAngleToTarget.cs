using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010D7 RID: 4311
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the Angle between a GameObject's forward axis and a Target. The Target can be defined as a GameObject or a world Position. If you specify both, then the Position will be used as a local offset from the Target Object's position.")]
	public class GetAngleToTarget : FsmStateAction
	{
		// Token: 0x060074B5 RID: 29877 RVA: 0x0023B53A File Offset: 0x0023973A
		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.targetPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.ignoreHeight = true;
			this.storeAngle = null;
			this.everyFrame = false;
		}

		// Token: 0x060074B6 RID: 29878 RVA: 0x0023B576 File Offset: 0x00239776
		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		// Token: 0x060074B7 RID: 29879 RVA: 0x0023B584 File Offset: 0x00239784
		public override void OnLateUpdate()
		{
			this.DoGetAngleToTarget();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060074B8 RID: 29880 RVA: 0x0023B59C File Offset: 0x0023979C
		private void DoGetAngleToTarget()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			GameObject value = this.targetObject.Value;
			if (value == null && this.targetPosition.IsNone)
			{
				return;
			}
			Vector3 a;
			if (value != null)
			{
				a = ((!this.targetPosition.IsNone) ? value.transform.TransformPoint(this.targetPosition.Value) : value.transform.position);
			}
			else
			{
				a = this.targetPosition.Value;
			}
			if (this.ignoreHeight.Value)
			{
				a.y = ownerDefaultTarget.transform.position.y;
			}
			Vector3 from = a - ownerDefaultTarget.transform.position;
			this.storeAngle.Value = Vector3.Angle(from, ownerDefaultTarget.transform.forward);
		}

		// Token: 0x040074FC RID: 29948
		[RequiredField]
		[Tooltip("The game object whose forward axis we measure from. If the target is dead ahead the angle will be 0.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040074FD RID: 29949
		[Tooltip("The target object to measure the angle to. Or use target position.")]
		public FsmGameObject targetObject;

		// Token: 0x040074FE RID: 29950
		[Tooltip("The world position to measure an angle to. If Target Object is also specified, this vector is used as an offset from that object's position.")]
		public FsmVector3 targetPosition;

		// Token: 0x040074FF RID: 29951
		[Tooltip("Ignore height differences when calculating the angle.")]
		public FsmBool ignoreHeight;

		// Token: 0x04007500 RID: 29952
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the angle in a float variable.")]
		public FsmFloat storeAngle;

		// Token: 0x04007501 RID: 29953
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
