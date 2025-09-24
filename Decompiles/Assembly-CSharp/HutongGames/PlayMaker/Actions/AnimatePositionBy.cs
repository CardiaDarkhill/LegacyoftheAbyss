using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA4 RID: 2980
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimatePositionBy : EaseFsmAction
	{
		// Token: 0x06005C17 RID: 23575 RVA: 0x001CF4C4 File Offset: 0x001CD6C4
		public override void Reset()
		{
			base.Reset();
			this.shiftBy = null;
			this.finishInNextStep = false;
			this.gotFromValue = false;
		}

		// Token: 0x06005C18 RID: 23576 RVA: 0x001CF4E4 File Offset: 0x001CD6E4
		public override void OnEnter()
		{
			base.OnEnter();
			if (this.shiftBy.Value.magnitude <= Mathf.Epsilon)
			{
				base.Finish();
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!ownerDefaultTarget)
			{
				base.Finish();
				return;
			}
			this.objectTransform = ownerDefaultTarget.transform;
			if (!this.objectTransform)
			{
				base.Finish();
				return;
			}
			if (this.delay.Value == 0f || this.delay.IsNone)
			{
				this.fromValue = this.objectTransform.position;
				this.toValue = this.objectTransform.position + this.shiftBy.Value;
				this.fromFloats = new float[3];
				this.fromFloats[0] = this.fromValue.x;
				this.fromFloats[1] = this.fromValue.y;
				this.fromFloats[2] = this.fromValue.z;
				this.toFloats = new float[3];
				this.toFloats[0] = this.toValue.x;
				this.toFloats[1] = this.toValue.y;
				this.toFloats[2] = this.toValue.z;
				this.resultFloats = new float[3];
				this.finishInNextStep = false;
			}
		}

		// Token: 0x06005C19 RID: 23577 RVA: 0x001CF64C File Offset: 0x001CD84C
		public override void OnUpdate()
		{
			if (this.isRunning)
			{
				if (!this.gotFromValue)
				{
					this.fromValue = this.objectTransform.position;
					this.toValue = this.objectTransform.position + this.shiftBy.Value;
					this.fromFloats = new float[3];
					this.fromFloats[0] = this.fromValue.x;
					this.fromFloats[1] = this.fromValue.y;
					this.fromFloats[2] = this.fromValue.z;
					this.toFloats = new float[3];
					this.toFloats[0] = this.toValue.x;
					this.toFloats[1] = this.toValue.y;
					this.toFloats[2] = this.toValue.z;
					this.resultFloats = new float[3];
					this.finishInNextStep = false;
					this.gotFromValue = true;
				}
				if (this.resultFloats[0] != 0f || this.resultFloats[1] != 0f || this.resultFloats[2] != 0f)
				{
					this.objectTransform.position = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
				}
			}
			if (this.finishInNextStep)
			{
				this.finished = true;
				base.Finish();
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
			}
			if (this.finishAction && !this.finishInNextStep)
			{
				this.objectTransform.position = new Vector3(this.reverse.IsNone ? this.toValue.x : (this.reverse.Value ? this.fromValue.x : this.toValue.x), this.reverse.IsNone ? this.toValue.y : (this.reverse.Value ? this.fromValue.y : this.toValue.y), this.reverse.IsNone ? this.toValue.z : (this.reverse.Value ? this.fromValue.z : this.toValue.z));
				this.finishInNextStep = true;
			}
			base.OnUpdate();
		}

		// Token: 0x0400577D RID: 22397
		public FsmOwnerDefault gameObject;

		// Token: 0x0400577E RID: 22398
		[RequiredField]
		public FsmVector3 shiftBy;

		// Token: 0x0400577F RID: 22399
		private Vector3 fromValue;

		// Token: 0x04005780 RID: 22400
		private Vector3 toValue;

		// Token: 0x04005781 RID: 22401
		private bool finishInNextStep;

		// Token: 0x04005782 RID: 22402
		private Transform objectTransform;

		// Token: 0x04005783 RID: 22403
		private bool gotFromValue;
	}
}
