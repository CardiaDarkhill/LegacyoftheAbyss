using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C04 RID: 3076
	[ActionCategory(ActionCategory.Vector2)]
	public class ConvertVector2ToAngle : FsmStateAction
	{
		// Token: 0x06005DF8 RID: 24056 RVA: 0x001D9DC6 File Offset: 0x001D7FC6
		public override void Reset()
		{
			this.vector = null;
			this.storeAngle = null;
			this.everyFrame = false;
		}

		// Token: 0x06005DF9 RID: 24057 RVA: 0x001D9DDD File Offset: 0x001D7FDD
		public override void OnEnter()
		{
			this.DoCalculate();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005DFA RID: 24058 RVA: 0x001D9DF3 File Offset: 0x001D7FF3
		public override void OnUpdate()
		{
			this.DoCalculate();
		}

		// Token: 0x06005DFB RID: 24059 RVA: 0x001D9DFB File Offset: 0x001D7FFB
		private void DoCalculate()
		{
			this.storeAngle.Value = this.vector.Value.DirectionToAngle();
		}

		// Token: 0x04005A52 RID: 23122
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmVector2 vector;

		// Token: 0x04005A53 RID: 23123
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat storeAngle;

		// Token: 0x04005A54 RID: 23124
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
