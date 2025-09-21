using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BAA RID: 2986
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateScaleTo : EaseFsmAction
	{
		// Token: 0x06005C32 RID: 23602 RVA: 0x001D06DF File Offset: 0x001CE8DF
		public override void Reset()
		{
			base.Reset();
			this.Target = null;
			this.fromValue = new Vector3(0f, 0f, 0f);
			this.ToLocalScale = null;
			this.finishInNextStep = false;
		}

		// Token: 0x06005C33 RID: 23603 RVA: 0x001D0718 File Offset: 0x001CE918
		public override void OnEnter()
		{
			base.OnEnter();
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.Target);
			if (!ownerDefaultTarget)
			{
				base.Finish();
				return;
			}
			this.objectTransform = ownerDefaultTarget.transform;
			this.fromValue = this.objectTransform.localScale;
			this.fromFloats = new float[3];
			this.fromFloats[0] = this.fromValue.x;
			this.fromFloats[1] = this.fromValue.y;
			this.fromFloats[2] = this.fromValue.z;
			this.toFloats = new float[3];
			this.toFloats[0] = this.ToLocalScale.Value.x;
			this.toFloats[1] = this.ToLocalScale.Value.y;
			this.toFloats[2] = this.ToLocalScale.Value.z;
			this.resultFloats = new float[3];
			this.finishInNextStep = false;
		}

		// Token: 0x06005C34 RID: 23604 RVA: 0x001D0818 File Offset: 0x001CEA18
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.isRunning)
			{
				this.objectTransform.localScale = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
			}
			if (this.finishInNextStep)
			{
				base.Finish();
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
			}
			if (this.finishAction && !this.finishInNextStep)
			{
				this.objectTransform.localScale = new Vector3(this.reverse.IsNone ? this.ToLocalScale.Value.x : (this.reverse.Value ? this.fromValue.x : this.ToLocalScale.Value.x), this.reverse.IsNone ? this.ToLocalScale.Value.y : (this.reverse.Value ? this.fromValue.y : this.ToLocalScale.Value.y), this.reverse.IsNone ? this.ToLocalScale.Value.z : (this.reverse.Value ? this.fromValue.z : this.ToLocalScale.Value.z));
				this.finishInNextStep = true;
			}
		}

		// Token: 0x040057A6 RID: 22438
		public FsmOwnerDefault Target;

		// Token: 0x040057A7 RID: 22439
		[RequiredField]
		public FsmVector3 ToLocalScale;

		// Token: 0x040057A8 RID: 22440
		private bool finishInNextStep;

		// Token: 0x040057A9 RID: 22441
		private Transform objectTransform;

		// Token: 0x040057AA RID: 22442
		private Vector3 fromValue;
	}
}
