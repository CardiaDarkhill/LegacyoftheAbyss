using System;
using HutongGames.PlayMaker.TweenEnums;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010FC RID: 4348
	[ActionCategory(ActionCategory.Tween)]
	public abstract class TweenVariableBase<T> : TweenPropertyBase<T> where T : NamedVariable
	{
		// Token: 0x06007593 RID: 30099 RVA: 0x0023EB5C File Offset: 0x0023CD5C
		public override void Reset()
		{
			base.Reset();
			this.variable = default(T);
			this.fromOption = TargetValueOptions.CurrentValue;
			this.fromValue = default(T);
			this.toOption = TargetValueOptions.Value;
			this.toValue = default(T);
		}

		// Token: 0x06007594 RID: 30100 RVA: 0x0023EB96 File Offset: 0x0023CD96
		public override void OnEnter()
		{
			base.OnEnter();
			this.DoTween();
		}

		// Token: 0x06007595 RID: 30101 RVA: 0x0023EBA4 File Offset: 0x0023CDA4
		protected override void InitTargets()
		{
			switch (this.fromOption)
			{
			case TargetValueOptions.CurrentValue:
				base.StartValue = this.variable.RawValue;
				break;
			case TargetValueOptions.Offset:
				base.StartValue = this.GetOffsetValue(this.variable.RawValue, this.fromValue.RawValue);
				break;
			case TargetValueOptions.Value:
				base.StartValue = this.fromValue.RawValue;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			switch (this.toOption)
			{
			case TargetValueOptions.CurrentValue:
				base.EndValue = this.variable.RawValue;
				return;
			case TargetValueOptions.Offset:
				base.EndValue = this.GetOffsetValue(this.variable.RawValue, this.toValue.RawValue);
				return;
			case TargetValueOptions.Value:
				base.EndValue = this.toValue.RawValue;
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x040075F7 RID: 30199
		[RequiredField]
		[Tooltip("The variable to tween.")]
		[UIHint(UIHint.Variable)]
		public T variable;
	}
}
