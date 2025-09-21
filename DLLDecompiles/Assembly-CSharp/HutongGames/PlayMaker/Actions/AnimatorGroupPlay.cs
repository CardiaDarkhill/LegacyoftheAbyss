using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011F2 RID: 4594
	public class AnimatorGroupPlay : FSMUtility.GetComponentFsmStateAction<AnimatorGroup>
	{
		// Token: 0x06007A73 RID: 31347 RVA: 0x0024C841 File Offset: 0x0024AA41
		public override void Reset()
		{
			base.Reset();
			this.StateName = null;
		}

		// Token: 0x06007A74 RID: 31348 RVA: 0x0024C850 File Offset: 0x0024AA50
		protected override void DoAction(AnimatorGroup component)
		{
			if (this.StateName.IsNone)
			{
				return;
			}
			component.Play(this.StateName.Value);
		}

		// Token: 0x04007ABD RID: 31421
		[RequiredField]
		public FsmString StateName;
	}
}
