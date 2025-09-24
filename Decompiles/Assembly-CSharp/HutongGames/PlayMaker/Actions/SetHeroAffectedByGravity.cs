using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001247 RID: 4679
	public class SetHeroAffectedByGravity : FsmStateAction
	{
		// Token: 0x06007BB5 RID: 31669 RVA: 0x00250498 File Offset: 0x0024E698
		public override void Reset()
		{
			this.SetTime = null;
			this.CanSet = new FsmBool
			{
				UseVariable = true
			};
			this.SetAffectedByGravity = null;
		}

		// Token: 0x06007BB6 RID: 31670 RVA: 0x002504BA File Offset: 0x0024E6BA
		public override void OnEnter()
		{
			if (this.SetTime.Value <= 0f)
			{
				this.Set();
			}
		}

		// Token: 0x06007BB7 RID: 31671 RVA: 0x002504D4 File Offset: 0x0024E6D4
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x06007BB8 RID: 31672 RVA: 0x002504DC File Offset: 0x0024E6DC
		public override void OnExit()
		{
			if (!base.Finished)
			{
				this.Set();
			}
		}

		// Token: 0x06007BB9 RID: 31673 RVA: 0x002504EC File Offset: 0x0024E6EC
		private void DoAction()
		{
			if (Time.time >= this.SetTime.Value)
			{
				this.Set();
			}
		}

		// Token: 0x06007BBA RID: 31674 RVA: 0x00250506 File Offset: 0x0024E706
		private void Set()
		{
			if (!this.CanSet.IsNone && !this.CanSet.Value)
			{
				return;
			}
			HeroController.instance.AffectedByGravity(this.SetAffectedByGravity.Value);
			base.Finish();
		}

		// Token: 0x04007BEF RID: 31727
		public FsmFloat SetTime;

		// Token: 0x04007BF0 RID: 31728
		public FsmBool CanSet;

		// Token: 0x04007BF1 RID: 31729
		[RequiredField]
		public FsmBool SetAffectedByGravity;
	}
}
