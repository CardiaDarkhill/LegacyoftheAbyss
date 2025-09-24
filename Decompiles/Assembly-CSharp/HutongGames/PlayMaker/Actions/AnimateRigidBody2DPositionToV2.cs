using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA8 RID: 2984
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimateRigidBody2DPositionToV2 : EaseFsmAction
	{
		// Token: 0x06005C28 RID: 23592 RVA: 0x001D02D1 File Offset: 0x001CE4D1
		[UsedImplicitly]
		public bool HasVectorValue()
		{
			return !this.ToValue.IsNone;
		}

		// Token: 0x06005C29 RID: 23593 RVA: 0x001D02E1 File Offset: 0x001CE4E1
		public override void Reset()
		{
			base.Reset();
			this.GameObject = null;
			this.ToValue = new FsmVector2
			{
				UseVariable = true
			};
			this.ToX = null;
			this.ToY = null;
		}

		// Token: 0x06005C2A RID: 23594 RVA: 0x001D0310 File Offset: 0x001CE510
		public override void OnEnter()
		{
			base.OnEnter();
			this.body = base.Fsm.GetOwnerDefaultTarget(this.GameObject).GetComponent<Rigidbody2D>();
			if (this.body == null)
			{
				return;
			}
			this.fromValue = this.body.position;
			this.toValue = ((!this.ToValue.IsNone) ? this.ToValue.Value : new Vector2(this.ToX.Value, this.ToY.Value));
			this.fromFloats = new float[2];
			this.fromFloats[0] = this.fromValue.x;
			this.fromFloats[1] = this.fromValue.y;
			this.toFloats = new float[2];
			this.toFloats[0] = this.toValue.x;
			this.toFloats[1] = this.toValue.y;
			this.resultFloats = new float[2];
			this.finishInNextStep = false;
		}

		// Token: 0x06005C2B RID: 23595 RVA: 0x001D041C File Offset: 0x001CE61C
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
				this.body.MovePosition(new Vector2(this.reverse.IsNone ? this.toValue.x : (this.reverse.Value ? this.fromValue.x : this.toValue.x), this.reverse.IsNone ? this.toValue.y : (this.reverse.Value ? this.fromValue.y : this.toValue.y)));
				this.finishInNextStep = true;
			}
		}

		// Token: 0x04005799 RID: 22425
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault GameObject;

		// Token: 0x0400579A RID: 22426
		public FsmVector2 ToValue;

		// Token: 0x0400579B RID: 22427
		[HideIf("HasVectorValue")]
		public FsmFloat ToX;

		// Token: 0x0400579C RID: 22428
		[HideIf("HasVectorValue")]
		public FsmFloat ToY;

		// Token: 0x0400579D RID: 22429
		private bool finishInNextStep;

		// Token: 0x0400579E RID: 22430
		private Rigidbody2D body;

		// Token: 0x0400579F RID: 22431
		private Vector3 fromValue;

		// Token: 0x040057A0 RID: 22432
		private Vector3 toValue;
	}
}
