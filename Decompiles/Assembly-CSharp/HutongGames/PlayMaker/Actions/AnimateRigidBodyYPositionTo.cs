using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA9 RID: 2985
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateRigidBodyYPositionTo : EaseFsmAction
	{
		// Token: 0x06005C2D RID: 23597 RVA: 0x001D0531 File Offset: 0x001CE731
		public override void Reset()
		{
			base.Reset();
			this.fromValue = 0f;
			this.ToValue = null;
			this.finishInNextStep = false;
		}

		// Token: 0x06005C2E RID: 23598 RVA: 0x001D0554 File Offset: 0x001CE754
		public override void OnEnter()
		{
			base.OnEnter();
			this.body = base.Fsm.GetOwnerDefaultTarget(this.GameObject).GetComponent<Rigidbody2D>();
			if (this.body == null)
			{
				return;
			}
			this.fromValue = this.body.position.y;
			this.fromFloats = new float[1];
			this.fromFloats[0] = this.fromValue;
			this.toFloats = new float[1];
			this.toFloats[0] = this.ToValue.Value;
			this.resultFloats = new float[1];
			this.finishInNextStep = false;
		}

		// Token: 0x06005C2F RID: 23599 RVA: 0x001D05F4 File Offset: 0x001CE7F4
		public override void OnExit()
		{
			base.OnExit();
		}

		// Token: 0x06005C30 RID: 23600 RVA: 0x001D05FC File Offset: 0x001CE7FC
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.isRunning)
			{
				this.body.MovePosition(new Vector2(this.body.position.x, this.resultFloats[0]));
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
				this.body.MovePosition(new Vector2(this.body.position.x, this.reverse.IsNone ? this.ToValue.Value : (this.reverse.Value ? this.fromValue : this.ToValue.Value)));
				this.finishInNextStep = true;
			}
		}

		// Token: 0x040057A1 RID: 22433
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault GameObject;

		// Token: 0x040057A2 RID: 22434
		[RequiredField]
		public FsmFloat ToValue;

		// Token: 0x040057A3 RID: 22435
		private bool finishInNextStep;

		// Token: 0x040057A4 RID: 22436
		private Rigidbody2D body;

		// Token: 0x040057A5 RID: 22437
		private float fromValue;
	}
}
