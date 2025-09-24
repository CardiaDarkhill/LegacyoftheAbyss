using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA7 RID: 2983
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateRigidBody2DPositionTo : EaseFsmAction
	{
		// Token: 0x06005C24 RID: 23588 RVA: 0x001D00B7 File Offset: 0x001CE2B7
		public override void Reset()
		{
			base.Reset();
			this.GameObject = null;
			this.ToValue = null;
		}

		// Token: 0x06005C25 RID: 23589 RVA: 0x001D00D0 File Offset: 0x001CE2D0
		public override void OnEnter()
		{
			base.OnEnter();
			this.body = base.Fsm.GetOwnerDefaultTarget(this.GameObject).GetComponent<Rigidbody2D>();
			if (this.body == null)
			{
				return;
			}
			this.fromValue = this.body.position;
			this.fromFloats = new float[2];
			this.fromFloats[0] = this.fromValue.x;
			this.fromFloats[1] = this.fromValue.y;
			this.toFloats = new float[2];
			this.toFloats[0] = this.ToValue.Value.x;
			this.toFloats[1] = this.ToValue.Value.y;
			this.resultFloats = new float[2];
			this.finishInNextStep = false;
		}

		// Token: 0x06005C26 RID: 23590 RVA: 0x001D01A8 File Offset: 0x001CE3A8
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.isRunning)
			{
				this.body.MovePosition(new Vector2(this.resultFloats[0], this.resultFloats[1]));
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
				this.body.MovePosition(new Vector2(this.reverse.IsNone ? this.ToValue.Value.x : (this.reverse.Value ? this.fromValue.x : this.ToValue.Value.x), this.reverse.IsNone ? this.ToValue.Value.y : (this.reverse.Value ? this.fromValue.y : this.ToValue.Value.y)));
				this.finishInNextStep = true;
			}
		}

		// Token: 0x04005794 RID: 22420
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault GameObject;

		// Token: 0x04005795 RID: 22421
		[RequiredField]
		public FsmVector2 ToValue;

		// Token: 0x04005796 RID: 22422
		private bool finishInNextStep;

		// Token: 0x04005797 RID: 22423
		private Rigidbody2D body;

		// Token: 0x04005798 RID: 22424
		private Vector3 fromValue;
	}
}
