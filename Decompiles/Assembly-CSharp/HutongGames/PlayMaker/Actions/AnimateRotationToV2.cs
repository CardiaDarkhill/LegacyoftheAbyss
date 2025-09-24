using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DBD RID: 3517
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateRotationToV2 : EaseFsmAction
	{
		// Token: 0x060065ED RID: 26093 RVA: 0x0020328D File Offset: 0x0020148D
		public override void Reset()
		{
			base.Reset();
			this.fromValue = null;
			this.toValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x060065EE RID: 26094 RVA: 0x002032AC File Offset: 0x002014AC
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
			}
			if (Mathf.Abs(this.toValue.Value - this.fromValue.Value) > 180f)
			{
				if (this.toValue.Value < this.fromValue.Value)
				{
					this.toValue.Value += 360f;
				}
				else
				{
					this.fromValue.Value += 360f;
				}
			}
			this.fromFloats = new float[1];
			this.fromFloats[0] = this.fromValue.Value;
			this.toFloats = new float[1];
			this.toFloats[0] = this.toValue.Value;
			this.resultFloats = new float[1];
			this.finishInNextStep = false;
		}

		// Token: 0x060065EF RID: 26095 RVA: 0x002033CE File Offset: 0x002015CE
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x060065F0 RID: 26096 RVA: 0x002033D8 File Offset: 0x002015D8
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

		// Token: 0x0400652A RID: 25898
		public FsmOwnerDefault gameObject;

		// Token: 0x0400652B RID: 25899
		[RequiredField]
		public FsmFloat fromValue;

		// Token: 0x0400652C RID: 25900
		[RequiredField]
		public FsmFloat toValue;

		// Token: 0x0400652D RID: 25901
		public bool worldSpace;

		// Token: 0x0400652E RID: 25902
		private Transform objectTransform;

		// Token: 0x0400652F RID: 25903
		private bool finishInNextStep;
	}
}
