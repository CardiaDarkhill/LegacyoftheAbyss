using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D4D RID: 3405
	public class SetRigidBody2dInterpolation : FsmStateAction
	{
		// Token: 0x060063D0 RID: 25552 RVA: 0x001F7729 File Offset: 0x001F5929
		public override void Reset()
		{
			this.Target = null;
			this.SetValue = null;
			this.SaveCurrentValue = null;
			this.ResetOnExit = false;
		}

		// Token: 0x060063D1 RID: 25553 RVA: 0x001F774C File Offset: 0x001F594C
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			this.body = safe.GetComponent<Rigidbody2D>();
			this.currentValue = this.body.interpolation;
			this.SaveCurrentValue.Value = this.currentValue;
			this.body.interpolation = (RigidbodyInterpolation2D)this.SetValue.Value;
			if (!this.ResetOnExit.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x060063D2 RID: 25554 RVA: 0x001F77C7 File Offset: 0x001F59C7
		public override void OnExit()
		{
			if (this.ResetOnExit.Value)
			{
				this.body.interpolation = this.currentValue;
			}
		}

		// Token: 0x04006226 RID: 25126
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault Target;

		// Token: 0x04006227 RID: 25127
		[ObjectType(typeof(RigidbodyInterpolation2D))]
		public FsmEnum SetValue;

		// Token: 0x04006228 RID: 25128
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(RigidbodyInterpolation2D))]
		public FsmEnum SaveCurrentValue;

		// Token: 0x04006229 RID: 25129
		public FsmBool ResetOnExit;

		// Token: 0x0400622A RID: 25130
		private Rigidbody2D body;

		// Token: 0x0400622B RID: 25131
		private RigidbodyInterpolation2D currentValue;
	}
}
