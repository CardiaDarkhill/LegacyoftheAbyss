using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D85 RID: 3461
	public class TakeDamage : FsmStateAction
	{
		// Token: 0x060064CA RID: 25802 RVA: 0x001FCF30 File Offset: 0x001FB130
		public override void Reset()
		{
			base.Reset();
			this.Target = new FsmGameObject
			{
				UseVariable = true
			};
			this.AttackType = new FsmInt
			{
				UseVariable = true
			};
			this.CircleDirection = new FsmBool
			{
				UseVariable = true
			};
			this.DamageDealt = new FsmInt
			{
				UseVariable = true
			};
			this.Direction = new FsmFloat
			{
				UseVariable = true
			};
			this.IgnoreInvulnerable = new FsmBool
			{
				UseVariable = true
			};
			this.MagnitudeMultiplier = new FsmFloat
			{
				UseVariable = true
			};
			this.MoveAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.MoveDirection = new FsmBool
			{
				UseVariable = true
			};
			this.Multiplier = new FsmFloat
			{
				UseVariable = true
			};
			this.SpecialType = new FsmInt
			{
				UseVariable = true
			};
		}

		// Token: 0x060064CB RID: 25803 RVA: 0x001FD00C File Offset: 0x001FB20C
		public override void OnEnter()
		{
			base.OnEnter();
			HitTaker.Hit(this.Target.Value, new HitInstance
			{
				Source = base.Owner,
				AttackType = (AttackTypes)this.AttackType.Value,
				CircleDirection = this.CircleDirection.Value,
				DamageDealt = this.DamageDealt.Value,
				Direction = this.Direction.Value,
				IgnoreInvulnerable = this.IgnoreInvulnerable.Value,
				MagnitudeMultiplier = this.MagnitudeMultiplier.Value,
				MoveAngle = this.MoveAngle.Value,
				MoveDirection = this.MoveDirection.Value,
				Multiplier = (this.Multiplier.IsNone ? 1f : this.Multiplier.Value),
				SpecialType = (SpecialTypes)this.SpecialType.Value
			}, 3);
			base.Finish();
		}

		// Token: 0x040063C5 RID: 25541
		public FsmGameObject Target;

		// Token: 0x040063C6 RID: 25542
		public FsmInt AttackType;

		// Token: 0x040063C7 RID: 25543
		public FsmBool CircleDirection;

		// Token: 0x040063C8 RID: 25544
		public FsmInt DamageDealt;

		// Token: 0x040063C9 RID: 25545
		public FsmFloat Direction;

		// Token: 0x040063CA RID: 25546
		public FsmBool IgnoreInvulnerable;

		// Token: 0x040063CB RID: 25547
		public FsmFloat MagnitudeMultiplier;

		// Token: 0x040063CC RID: 25548
		public FsmFloat MoveAngle;

		// Token: 0x040063CD RID: 25549
		public FsmBool MoveDirection;

		// Token: 0x040063CE RID: 25550
		public FsmFloat Multiplier;

		// Token: 0x040063CF RID: 25551
		public FsmInt SpecialType;
	}
}
