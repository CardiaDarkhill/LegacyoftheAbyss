using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DBC RID: 3516
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateRotationTo : EaseFsmAction
	{
		// Token: 0x060065E8 RID: 26088 RVA: 0x00203009 File Offset: 0x00201209
		public override void Reset()
		{
			base.Reset();
			this.fromValue = null;
			this.toValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x060065E9 RID: 26089 RVA: 0x00203028 File Offset: 0x00201228
		public override void OnEnter()
		{
			base.OnEnter();
			this.objectTransform = base.Fsm.GetOwnerDefaultTarget(this.gameObject).transform;
			if (this.objectTransform == null)
			{
				return;
			}
			if (this.fromValue.IsNone)
			{
				this.fromValue.Value = this.objectTransform.localEulerAngles.z;
				if (this.negativeSpace)
				{
					while (this.fromValue.Value > 0f)
					{
						this.fromValue.Value -= 360f;
					}
				}
			}
			this.fromFloats = new float[1];
			this.fromFloats[0] = this.fromValue.Value;
			this.toFloats = new float[1];
			this.toFloats[0] = this.toValue.Value;
			this.resultFloats = new float[1];
			this.finishInNextStep = false;
		}

		// Token: 0x060065EA RID: 26090 RVA: 0x00203112 File Offset: 0x00201312
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060065EB RID: 26091 RVA: 0x0020311C File Offset: 0x0020131C
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.isRunning)
			{
				if (this.worldSpace)
				{
					this.objectTransform.eulerAngles = new Vector3(0f, 0f, this.resultFloats[0]);
				}
				else
				{
					this.objectTransform.localEulerAngles = new Vector3(0f, 0f, this.resultFloats[0]);
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
				if (this.worldSpace)
				{
					this.objectTransform.eulerAngles = new Vector3(0f, 0f, this.reverse.IsNone ? this.toValue.Value : (this.reverse.Value ? this.fromValue.Value : this.toValue.Value));
				}
				else
				{
					this.objectTransform.localEulerAngles = new Vector3(0f, 0f, this.reverse.IsNone ? this.toValue.Value : (this.reverse.Value ? this.fromValue.Value : this.toValue.Value));
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x04006523 RID: 25891
		public FsmOwnerDefault gameObject;

		// Token: 0x04006524 RID: 25892
		[RequiredField]
		public FsmFloat fromValue;

		// Token: 0x04006525 RID: 25893
		[RequiredField]
		public FsmFloat toValue;

		// Token: 0x04006526 RID: 25894
		public bool worldSpace;

		// Token: 0x04006527 RID: 25895
		public bool negativeSpace;

		// Token: 0x04006528 RID: 25896
		private Transform objectTransform;

		// Token: 0x04006529 RID: 25897
		private bool finishInNextStep;
	}
}
