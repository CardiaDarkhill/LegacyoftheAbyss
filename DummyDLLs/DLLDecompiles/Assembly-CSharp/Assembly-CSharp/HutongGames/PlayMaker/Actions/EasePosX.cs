using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC7 RID: 3527
	[ActionCategory(ActionCategory.AnimateVariables)]
	[Tooltip("Easing Animation - Float")]
	public class EasePosX : EaseFsmAction
	{
		// Token: 0x0600663F RID: 26175 RVA: 0x002063C3 File Offset: 0x002045C3
		public override void Reset()
		{
			base.Reset();
			this.floatVariable = null;
			this.toValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x06006640 RID: 26176 RVA: 0x002063E0 File Offset: 0x002045E0
		public override void OnEnter()
		{
			base.OnEnter();
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.GameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.tf = ownerDefaultTarget.transform;
			this.fromValue = this.tf.position.x;
			this.fromFloats = new float[1];
			this.fromFloats[0] = this.fromValue;
			this.toFloats = new float[1];
			this.toFloats[0] = this.toValue.Value;
			this.resultFloats = new float[1];
			this.finishInNextStep = false;
			this.floatVariable = this.fromValue;
		}

		// Token: 0x06006641 RID: 26177 RVA: 0x0020648E File Offset: 0x0020468E
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06006642 RID: 26178 RVA: 0x00206498 File Offset: 0x00204698
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (!this.floatVariable.IsNone && this.isRunning)
			{
				this.floatVariable.Value = this.resultFloats[0];
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
				if (!this.floatVariable.IsNone)
				{
					this.floatVariable.Value = (this.reverse.IsNone ? this.toValue.Value : (this.reverse.Value ? this.fromValue : this.toValue.Value));
				}
				this.finishInNextStep = true;
			}
			if (this.localSpace)
			{
				this.tf.localPosition = new Vector3(this.floatVariable.Value, this.tf.localPosition.y, this.tf.localPosition.z);
				return;
			}
			this.tf.position = new Vector3(this.floatVariable.Value, this.tf.position.y, this.tf.position.z);
		}

		// Token: 0x04006599 RID: 26009
		[RequiredField]
		public FsmOwnerDefault GameObject;

		// Token: 0x0400659A RID: 26010
		private float fromValue;

		// Token: 0x0400659B RID: 26011
		[RequiredField]
		public FsmFloat toValue;

		// Token: 0x0400659C RID: 26012
		public bool localSpace;

		// Token: 0x0400659D RID: 26013
		private FsmFloat floatVariable;

		// Token: 0x0400659E RID: 26014
		private bool finishInNextStep;

		// Token: 0x0400659F RID: 26015
		private Transform tf;
	}
}
