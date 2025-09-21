using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001066 RID: 4198
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Sets the value of a Float Variable.")]
	public class SetFloatValueV2 : FsmStateAction
	{
		// Token: 0x060072B4 RID: 29364 RVA: 0x00234B6F File Offset: 0x00232D6F
		public override void Reset()
		{
			this.floatVariable = null;
			this.floatValue = null;
			this.everyFrame = false;
			this.activeBool = null;
		}

		// Token: 0x060072B5 RID: 29365 RVA: 0x00234B8D File Offset: 0x00232D8D
		public override void OnEnter()
		{
			if (this.activeBool.IsNone || this.activeBool.Value)
			{
				this.floatVariable.Value = this.floatValue.Value;
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060072B6 RID: 29366 RVA: 0x00234BCD File Offset: 0x00232DCD
		public override void OnUpdate()
		{
			if (this.activeBool.IsNone || this.activeBool.Value)
			{
				this.floatVariable.Value = this.floatValue.Value;
			}
		}

		// Token: 0x040072B6 RID: 29366
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmFloat floatVariable;

		// Token: 0x040072B7 RID: 29367
		[RequiredField]
		public FsmFloat floatValue;

		// Token: 0x040072B8 RID: 29368
		public bool everyFrame;

		// Token: 0x040072B9 RID: 29369
		public FsmBool activeBool;
	}
}
