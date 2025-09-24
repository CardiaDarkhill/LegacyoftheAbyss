using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010EB RID: 4331
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Smoothly Rotates a Game Object so its forward vector points in the specified Direction. Lets you fire an event when minmagnitude is reached")]
	public class SmoothLookAtDirection : FsmStateAction
	{
		// Token: 0x0600752D RID: 29997 RVA: 0x0023D620 File Offset: 0x0023B820
		public override void Reset()
		{
			this.gameObject = null;
			this.targetDirection = new FsmVector3
			{
				UseVariable = true
			};
			this.minMagnitude = 0.1f;
			this.upVector = new FsmVector3
			{
				UseVariable = true
			};
			this.keepVertical = true;
			this.speed = 5f;
			this.lateUpdate = true;
			this.finishEvent = null;
		}

		// Token: 0x0600752E RID: 29998 RVA: 0x0023D692 File Offset: 0x0023B892
		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		// Token: 0x0600752F RID: 29999 RVA: 0x0023D6A0 File Offset: 0x0023B8A0
		public override void OnEnter()
		{
			this.previousGo = null;
		}

		// Token: 0x06007530 RID: 30000 RVA: 0x0023D6A9 File Offset: 0x0023B8A9
		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoSmoothLookAtDirection();
			}
		}

		// Token: 0x06007531 RID: 30001 RVA: 0x0023D6B9 File Offset: 0x0023B8B9
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoSmoothLookAtDirection();
			}
		}

		// Token: 0x06007532 RID: 30002 RVA: 0x0023D6CC File Offset: 0x0023B8CC
		private void DoSmoothLookAtDirection()
		{
			if (this.targetDirection.IsNone)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.previousGo != ownerDefaultTarget)
			{
				this.lastRotation = ownerDefaultTarget.transform.rotation;
				this.desiredRotation = this.lastRotation;
				this.previousGo = ownerDefaultTarget;
			}
			Vector3 value = this.targetDirection.Value;
			if (this.keepVertical.Value)
			{
				value.y = 0f;
			}
			bool flag = false;
			if (value.sqrMagnitude > this.minMagnitude.Value)
			{
				this.desiredRotation = Quaternion.LookRotation(value, this.upVector.IsNone ? Vector3.up : this.upVector.Value);
			}
			else
			{
				flag = true;
			}
			this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, this.speed.Value * Time.deltaTime);
			ownerDefaultTarget.transform.rotation = this.lastRotation;
			if (flag)
			{
				base.Fsm.Event(this.finishEvent);
				if (this.finish.Value)
				{
					base.Finish();
				}
			}
		}

		// Token: 0x04007599 RID: 30105
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400759A RID: 30106
		[RequiredField]
		[Tooltip("The direction to smoothly rotate towards.")]
		public FsmVector3 targetDirection;

		// Token: 0x0400759B RID: 30107
		[Tooltip("Only rotate if Target Direction Vector length is greater than this threshold.")]
		public FsmFloat minMagnitude;

		// Token: 0x0400759C RID: 30108
		[Tooltip("Keep this vector pointing up as the GameObject rotates.")]
		public FsmVector3 upVector;

		// Token: 0x0400759D RID: 30109
		[RequiredField]
		[Tooltip("Eliminate any tilt up/down as the GameObject rotates.")]
		public FsmBool keepVertical;

		// Token: 0x0400759E RID: 30110
		[RequiredField]
		[HasFloatSlider(0.5f, 15f)]
		[Tooltip("How quickly to rotate.")]
		public FsmFloat speed;

		// Token: 0x0400759F RID: 30111
		[Tooltip("Perform in LateUpdate. This can help eliminate jitters in some situations.")]
		public bool lateUpdate;

		// Token: 0x040075A0 RID: 30112
		[Tooltip("Event to send if the direction difference is less than Min Magnitude.")]
		public FsmEvent finishEvent;

		// Token: 0x040075A1 RID: 30113
		[Tooltip("Stop running the action if the direction difference is less than Min Magnitude.")]
		public FsmBool finish;

		// Token: 0x040075A2 RID: 30114
		private GameObject previousGo;

		// Token: 0x040075A3 RID: 30115
		private Quaternion lastRotation;

		// Token: 0x040075A4 RID: 30116
		private Quaternion desiredRotation;
	}
}
