using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BAB RID: 2987
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateXPositionTo : EaseFsmAction
	{
		// Token: 0x06005C36 RID: 23606 RVA: 0x001D0992 File Offset: 0x001CEB92
		public override void Reset()
		{
			base.Reset();
			this.fromValue = 0f;
			this.ToValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x06005C37 RID: 23607 RVA: 0x001D09B4 File Offset: 0x001CEBB4
		public override void OnEnter()
		{
			base.OnEnter();
			this.tf = base.Fsm.GetOwnerDefaultTarget(this.GameObject).transform;
			if (this.tf == null)
			{
				return;
			}
			if (!this.localSpace)
			{
				this.fromValue = this.tf.position.x;
			}
			else
			{
				this.fromValue = this.tf.localPosition.x;
			}
			this.fromFloats = new float[1];
			this.fromFloats[0] = this.fromValue;
			this.toFloats = new float[1];
			this.toFloats[0] = this.ToValue.Value;
			this.resultFloats = new float[1];
			this.finishInNextStep = false;
		}

		// Token: 0x06005C38 RID: 23608 RVA: 0x001D0A74 File Offset: 0x001CEC74
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06005C39 RID: 23609 RVA: 0x001D0A7C File Offset: 0x001CEC7C
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.isRunning)
			{
				if (!this.localSpace)
				{
					this.tf.position = new Vector3(this.resultFloats[0], this.tf.position.y, this.tf.position.z);
				}
				else
				{
					this.tf.localPosition = new Vector3(this.resultFloats[0], this.tf.position.y, this.tf.localPosition.z);
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
				if (!this.localSpace)
				{
					this.tf.position = new Vector3(this.reverse.IsNone ? this.ToValue.Value : (this.reverse.Value ? this.fromValue : this.ToValue.Value), this.tf.position.y, this.tf.position.z);
				}
				else
				{
					this.tf.localPosition = new Vector3(this.reverse.IsNone ? this.ToValue.Value : (this.reverse.Value ? this.fromValue : this.ToValue.Value), this.tf.position.y, this.tf.localPosition.z);
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x040057AB RID: 22443
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault GameObject;

		// Token: 0x040057AC RID: 22444
		[RequiredField]
		public FsmFloat ToValue;

		// Token: 0x040057AD RID: 22445
		public bool localSpace;

		// Token: 0x040057AE RID: 22446
		private bool finishInNextStep;

		// Token: 0x040057AF RID: 22447
		private Transform tf;

		// Token: 0x040057B0 RID: 22448
		private float fromValue;
	}
}
