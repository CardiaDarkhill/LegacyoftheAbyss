using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010EA RID: 4330
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Smoothly Rotates a Game Object so its forward vector points at a Target. The target can be defined as a Game Object or a world Position. If you specify both, then the position will be used as a local offset from the object's position.")]
	public class SmoothLookAt : FsmStateAction
	{
		// Token: 0x06007527 RID: 29991 RVA: 0x0023D370 File Offset: 0x0023B570
		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.targetPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.upVector = new FsmVector3
			{
				UseVariable = true
			};
			this.keepVertical = true;
			this.debug = false;
			this.speed = 5f;
			this.finishTolerance = 1f;
			this.finishEvent = null;
		}

		// Token: 0x06007528 RID: 29992 RVA: 0x0023D3EE File Offset: 0x0023B5EE
		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		// Token: 0x06007529 RID: 29993 RVA: 0x0023D3FC File Offset: 0x0023B5FC
		public override void OnEnter()
		{
			this.previousGo = null;
		}

		// Token: 0x0600752A RID: 29994 RVA: 0x0023D405 File Offset: 0x0023B605
		public override void OnLateUpdate()
		{
			this.DoSmoothLookAt();
		}

		// Token: 0x0600752B RID: 29995 RVA: 0x0023D410 File Offset: 0x0023B610
		private void DoSmoothLookAt()
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
			if (this.previousGo != ownerDefaultTarget)
			{
				this.lastRotation = ownerDefaultTarget.transform.rotation;
				this.desiredRotation = this.lastRotation;
				this.previousGo = ownerDefaultTarget;
			}
			if (value != null)
			{
				this.lookAtPos = ((!this.targetPosition.IsNone) ? value.transform.TransformPoint(this.targetPosition.Value) : value.transform.position);
			}
			else
			{
				this.lookAtPos = this.targetPosition.Value;
			}
			if (this.keepVertical.Value)
			{
				this.lookAtPos.y = ownerDefaultTarget.transform.position.y;
			}
			Vector3 vector = this.lookAtPos - ownerDefaultTarget.transform.position;
			if (vector != Vector3.zero && vector.sqrMagnitude > 0f)
			{
				this.desiredRotation = Quaternion.LookRotation(vector, this.upVector.IsNone ? Vector3.up : this.upVector.Value);
			}
			this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, this.speed.Value * Time.deltaTime);
			ownerDefaultTarget.transform.rotation = this.lastRotation;
			if (this.debug.Value)
			{
				Debug.DrawLine(ownerDefaultTarget.transform.position, this.lookAtPos, Color.grey);
			}
			if (this.finishEvent != null && Mathf.Abs(Vector3.Angle(this.lookAtPos - ownerDefaultTarget.transform.position, ownerDefaultTarget.transform.forward)) <= this.finishTolerance.Value)
			{
				base.Fsm.Event(this.finishEvent);
			}
		}

		// Token: 0x0400758C RID: 30092
		[RequiredField]
		[Tooltip("The GameObject to rotate to face a target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400758D RID: 30093
		[Tooltip("A target GameObject.")]
		public FsmGameObject targetObject;

		// Token: 0x0400758E RID: 30094
		[Tooltip("A world position, or local offset if a Target Object is defined.")]
		public FsmVector3 targetPosition;

		// Token: 0x0400758F RID: 30095
		[Tooltip("Used to keep the game object generally upright. If left undefined the world y axis is used.")]
		public FsmVector3 upVector;

		// Token: 0x04007590 RID: 30096
		[Tooltip("Force the game object to remain vertical. Useful for characters.")]
		public FsmBool keepVertical;

		// Token: 0x04007591 RID: 30097
		[HasFloatSlider(0.5f, 15f)]
		[Tooltip("How fast the look at moves.")]
		public FsmFloat speed;

		// Token: 0x04007592 RID: 30098
		[Tooltip("Draw a line in the Scene View to the look at position.")]
		public FsmBool debug;

		// Token: 0x04007593 RID: 30099
		[Tooltip("If the angle to the target is less than this, send the Finish Event below. Measured in degrees.")]
		public FsmFloat finishTolerance;

		// Token: 0x04007594 RID: 30100
		[Tooltip("Event to send if the angle to target is less than the Finish Tolerance.")]
		public FsmEvent finishEvent;

		// Token: 0x04007595 RID: 30101
		private GameObject previousGo;

		// Token: 0x04007596 RID: 30102
		private Quaternion lastRotation;

		// Token: 0x04007597 RID: 30103
		private Quaternion desiredRotation;

		// Token: 0x04007598 RID: 30104
		private Vector3 lookAtPos;
	}
}
