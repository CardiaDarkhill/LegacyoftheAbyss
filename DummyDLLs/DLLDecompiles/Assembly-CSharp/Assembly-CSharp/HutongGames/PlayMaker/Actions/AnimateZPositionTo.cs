using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BAD RID: 2989
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateZPositionTo : EaseFsmAction
	{
		// Token: 0x06005C40 RID: 23616 RVA: 0x001D0F13 File Offset: 0x001CF113
		public override void Reset()
		{
			base.Reset();
			this.fromValue = 0f;
			this.ToValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x06005C41 RID: 23617 RVA: 0x001D0F34 File Offset: 0x001CF134
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
				this.fromValue = this.tf.position.z;
			}
			else
			{
				this.fromValue = this.tf.localPosition.z;
			}
			this.fromFloats = new float[1];
			this.fromFloats[0] = this.fromValue;
			this.toFloats = new float[1];
			this.toFloats[0] = this.ToValue.Value;
			this.resultFloats = new float[1];
			this.finishInNextStep = false;
		}

		// Token: 0x06005C42 RID: 23618 RVA: 0x001D0FF4 File Offset: 0x001CF1F4
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06005C43 RID: 23619 RVA: 0x001D0FFC File Offset: 0x001CF1FC
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.isRunning)
			{
				if (!this.localSpace)
				{
					this.tf.position = new Vector3(this.tf.position.x, this.tf.position.y, this.resultFloats[0]);
				}
				else
				{
					this.tf.localPosition = new Vector3(this.tf.localPosition.x, this.tf.localPosition.y, this.resultFloats[0]);
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
					this.tf.position = new Vector3(this.tf.position.x, this.tf.position.y, this.reverse.IsNone ? this.ToValue.Value : (this.reverse.Value ? this.fromValue : this.ToValue.Value));
				}
				else
				{
					this.tf.localPosition = new Vector3(this.tf.localPosition.x, this.tf.localPosition.y, this.reverse.IsNone ? this.ToValue.Value : (this.reverse.Value ? this.fromValue : this.ToValue.Value));
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x040057B7 RID: 22455
		[RequiredField]
		public FsmOwnerDefault GameObject;

		// Token: 0x040057B8 RID: 22456
		[RequiredField]
		public FsmFloat ToValue;

		// Token: 0x040057B9 RID: 22457
		public bool localSpace;

		// Token: 0x040057BA RID: 22458
		private bool finishInNextStep;

		// Token: 0x040057BB RID: 22459
		private Transform tf;

		// Token: 0x040057BC RID: 22460
		private float fromValue;
	}
}
