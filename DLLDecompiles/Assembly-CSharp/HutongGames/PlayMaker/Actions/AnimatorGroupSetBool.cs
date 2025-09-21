using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011F3 RID: 4595
	public class AnimatorGroupSetBool : FSMUtility.GetComponentFsmStateAction<AnimatorGroup>
	{
		// Token: 0x06007A76 RID: 31350 RVA: 0x0024C879 File Offset: 0x0024AA79
		public override void Reset()
		{
			base.Reset();
			this.BoolName = null;
			this.SetValue = null;
		}

		// Token: 0x06007A77 RID: 31351 RVA: 0x0024C88F File Offset: 0x0024AA8F
		protected override void DoAction(AnimatorGroup component)
		{
			if (this.BoolName.IsNone)
			{
				return;
			}
			component.SetBool(this.BoolName.Value, this.SetValue.Value);
		}

		// Token: 0x04007ABE RID: 31422
		[RequiredField]
		public FsmString BoolName;

		// Token: 0x04007ABF RID: 31423
		[RequiredField]
		public FsmBool SetValue;
	}
}
