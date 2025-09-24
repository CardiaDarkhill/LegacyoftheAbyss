using System;
using HutongGames.PlayMaker.TweenEnums;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010FB RID: 4347
	[ActionCategory(ActionCategory.Tween)]
	public abstract class TweenPropertyBase<T> : TweenActionBase where T : NamedVariable
	{
		// Token: 0x06007589 RID: 30089 RVA: 0x0023EADB File Offset: 0x0023CCDB
		public override void Reset()
		{
			base.Reset();
			this.fromOption = TargetValueOptions.CurrentValue;
			this.fromValue = default(T);
			this.toOption = TargetValueOptions.Value;
			this.toValue = default(T);
		}

		// Token: 0x17000C07 RID: 3079
		// (get) Token: 0x0600758A RID: 30090 RVA: 0x0023EB09 File Offset: 0x0023CD09
		// (set) Token: 0x0600758B RID: 30091 RVA: 0x0023EB11 File Offset: 0x0023CD11
		public object StartValue { get; protected set; }

		// Token: 0x17000C08 RID: 3080
		// (get) Token: 0x0600758C RID: 30092 RVA: 0x0023EB1A File Offset: 0x0023CD1A
		// (set) Token: 0x0600758D RID: 30093 RVA: 0x0023EB22 File Offset: 0x0023CD22
		public object EndValue { get; protected set; }

		// Token: 0x0600758E RID: 30094 RVA: 0x0023EB2B File Offset: 0x0023CD2B
		public override void OnEnter()
		{
			base.OnEnter();
			this.InitTargets();
			this.DoTween();
		}

		// Token: 0x0600758F RID: 30095 RVA: 0x0023EB3F File Offset: 0x0023CD3F
		protected virtual void InitTargets()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06007590 RID: 30096 RVA: 0x0023EB46 File Offset: 0x0023CD46
		protected virtual object GetOffsetValue(object value, object offset)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06007591 RID: 30097 RVA: 0x0023EB4D File Offset: 0x0023CD4D
		protected override void DoTween()
		{
			throw new NotImplementedException();
		}

		// Token: 0x040075F1 RID: 30193
		[Title("From")]
		[Tooltip("Setup where to tween from.")]
		public TargetValueOptions fromOption;

		// Token: 0x040075F2 RID: 30194
		[Tooltip("Tween from this value.")]
		[HideIf("HideFromValue")]
		public T fromValue;

		// Token: 0x040075F3 RID: 30195
		[Title("To")]
		[Tooltip("Setup where to tween to.")]
		public TargetValueOptions toOption;

		// Token: 0x040075F4 RID: 30196
		[Tooltip("Tween to this value.")]
		[HideIf("HideToValue")]
		public T toValue;
	}
}
