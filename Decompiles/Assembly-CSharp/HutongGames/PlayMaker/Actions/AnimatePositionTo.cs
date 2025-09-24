using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA5 RID: 2981
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimatePositionTo : EaseFsmAction
	{
		// Token: 0x06005C1B RID: 23579 RVA: 0x001CF8C0 File Offset: 0x001CDAC0
		public override void Reset()
		{
			base.Reset();
			this.fromValue = new Vector3(0f, 0f, 0f);
			this.toValue = null;
			this.finishInNextStep = false;
			this.localSpace = false;
		}

		// Token: 0x06005C1C RID: 23580 RVA: 0x001CF8F8 File Offset: 0x001CDAF8
		public override void OnEnter()
		{
			base.OnEnter();
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!ownerDefaultTarget)
			{
				base.Finish();
				return;
			}
			this.objectTransform = ownerDefaultTarget.transform;
			if (this.localSpace)
			{
				this.fromValue = this.objectTransform.localPosition;
			}
			else
			{
				this.fromValue = this.objectTransform.position;
			}
			this.fromFloats = new float[3];
			this.fromFloats[0] = this.fromValue.x;
			this.fromFloats[1] = this.fromValue.y;
			this.fromFloats[2] = this.fromValue.z;
			this.toFloats = new float[3];
			this.toFloats[0] = this.toValue.Value.x;
			this.toFloats[1] = this.toValue.Value.y;
			this.toFloats[2] = this.toValue.Value.z;
			this.resultFloats = new float[3];
			this.finishInNextStep = false;
		}

		// Token: 0x06005C1D RID: 23581 RVA: 0x001CFA10 File Offset: 0x001CDC10
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.isRunning)
			{
				if (this.localSpace)
				{
					this.objectTransform.localPosition = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
				}
				else
				{
					this.objectTransform.position = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
				}
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
				if (this.localSpace)
				{
					this.objectTransform.localPosition = new Vector3(this.reverse.IsNone ? this.toValue.Value.x : (this.reverse.Value ? this.fromValue.x : this.toValue.Value.x), this.reverse.IsNone ? this.toValue.Value.y : (this.reverse.Value ? this.fromValue.y : this.toValue.Value.y), this.reverse.IsNone ? this.toValue.Value.z : (this.reverse.Value ? this.fromValue.z : this.toValue.Value.z));
				}
				else
				{
					this.objectTransform.position = new Vector3(this.reverse.IsNone ? this.toValue.Value.x : (this.reverse.Value ? this.fromValue.x : this.toValue.Value.x), this.reverse.IsNone ? this.toValue.Value.y : (this.reverse.Value ? this.fromValue.y : this.toValue.Value.y), this.reverse.IsNone ? this.toValue.Value.z : (this.reverse.Value ? this.fromValue.z : this.toValue.Value.z));
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x04005784 RID: 22404
		public FsmOwnerDefault gameObject;

		// Token: 0x04005785 RID: 22405
		[RequiredField]
		public FsmVector3 toValue;

		// Token: 0x04005786 RID: 22406
		public bool localSpace;

		// Token: 0x04005787 RID: 22407
		private bool finishInNextStep;

		// Token: 0x04005788 RID: 22408
		private Transform objectTransform;

		// Token: 0x04005789 RID: 22409
		private Vector3 fromValue;
	}
}
