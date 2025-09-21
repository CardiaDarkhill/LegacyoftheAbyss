using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BAC RID: 2988
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateYPositionTo : EaseFsmAction
	{
		// Token: 0x06005C3B RID: 23611 RVA: 0x001D0C3B File Offset: 0x001CEE3B
		public override void Reset()
		{
			base.Reset();
			this.fromValue = 0f;
			this.ToValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x06005C3C RID: 23612 RVA: 0x001D0C5C File Offset: 0x001CEE5C
		public override void OnEnter()
		{
			base.OnEnter();
			if (base.Fsm.GetOwnerDefaultTarget(this.GameObject) == null)
			{
				return;
			}
			this.tf = base.Fsm.GetOwnerDefaultTarget(this.GameObject).transform;
			if (this.tf == null)
			{
				return;
			}
			if (!this.localSpace)
			{
				this.fromValue = this.tf.position.y;
			}
			else
			{
				this.fromValue = this.tf.localPosition.y;
			}
			this.fromFloats = new float[1];
			this.fromFloats[0] = this.fromValue;
			this.toFloats = new float[1];
			this.toFloats[0] = this.ToValue.Value;
			this.resultFloats = new float[1];
			this.finishInNextStep = false;
		}

		// Token: 0x06005C3D RID: 23613 RVA: 0x001D0D36 File Offset: 0x001CEF36
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06005C3E RID: 23614 RVA: 0x001D0D40 File Offset: 0x001CEF40
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.isRunning)
			{
				if (!this.localSpace)
				{
					this.tf.position = new Vector3(this.tf.position.x, this.resultFloats[0], this.tf.position.z);
				}
				else
				{
					this.tf.localPosition = new Vector3(this.tf.localPosition.x, this.resultFloats[0], this.tf.localPosition.z);
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
					this.tf.position = new Vector3(this.tf.position.x, this.reverse.IsNone ? this.ToValue.Value : (this.reverse.Value ? this.fromValue : this.ToValue.Value), this.tf.position.z);
				}
				else
				{
					this.tf.localPosition = new Vector3(this.tf.localPosition.x, this.reverse.IsNone ? this.ToValue.Value : (this.reverse.Value ? this.fromValue : this.ToValue.Value), this.tf.localPosition.z);
				}
				this.finishInNextStep = true;
			}
			if (this.tf == null)
			{
				base.Finish();
			}
		}

		// Token: 0x040057B1 RID: 22449
		[RequiredField]
		public FsmOwnerDefault GameObject;

		// Token: 0x040057B2 RID: 22450
		[RequiredField]
		public FsmFloat ToValue;

		// Token: 0x040057B3 RID: 22451
		public bool localSpace;

		// Token: 0x040057B4 RID: 22452
		private bool finishInNextStep;

		// Token: 0x040057B5 RID: 22453
		private Transform tf;

		// Token: 0x040057B6 RID: 22454
		private float fromValue;
	}
}
