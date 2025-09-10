using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BFD RID: 3069
	[ActionCategory(ActionCategory.Logic)]
	public class CompareHPBool : FsmStateAction
	{
		// Token: 0x06005DD5 RID: 24021 RVA: 0x001D9792 File Offset: 0x001D7992
		public override void Reset()
		{
			this.hp = 0;
			this.compareTo = 0;
			this.equalBool = null;
			this.lessThanBool = null;
			this.greaterThanBool = null;
			this.everyFrame = false;
		}

		// Token: 0x06005DD6 RID: 24022 RVA: 0x001D97C3 File Offset: 0x001D79C3
		public override void OnEnter()
		{
			this.healthManager = this.enemy.Value.GetComponent<HealthManager>();
			this.DoCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005DD7 RID: 24023 RVA: 0x001D97EF File Offset: 0x001D79EF
		public override void OnUpdate()
		{
			this.DoCompare();
		}

		// Token: 0x06005DD8 RID: 24024 RVA: 0x001D97F8 File Offset: 0x001D79F8
		private void DoCompare()
		{
			if (this.healthManager != null)
			{
				this.hp = this.healthManager.hp;
			}
			if (this.hp == this.compareTo.Value)
			{
				this.equalBool.Value = true;
			}
			else
			{
				this.equalBool.Value = false;
			}
			if (this.hp < this.compareTo.Value)
			{
				this.lessThanBool.Value = true;
			}
			else
			{
				this.lessThanBool.Value = false;
			}
			if (this.hp > this.compareTo.Value)
			{
				this.greaterThanBool.Value = true;
				return;
			}
			this.greaterThanBool.Value = false;
		}

		// Token: 0x04005A2C RID: 23084
		[RequiredField]
		public FsmGameObject enemy;

		// Token: 0x04005A2D RID: 23085
		public FsmInt compareTo;

		// Token: 0x04005A2E RID: 23086
		public FsmBool equalBool;

		// Token: 0x04005A2F RID: 23087
		public FsmBool lessThanBool;

		// Token: 0x04005A30 RID: 23088
		public FsmBool greaterThanBool;

		// Token: 0x04005A31 RID: 23089
		public bool everyFrame;

		// Token: 0x04005A32 RID: 23090
		private int hp;

		// Token: 0x04005A33 RID: 23091
		private HealthManager healthManager;
	}
}
