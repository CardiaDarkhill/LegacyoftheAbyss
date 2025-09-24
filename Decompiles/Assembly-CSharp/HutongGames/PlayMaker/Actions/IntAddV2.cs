using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C8D RID: 3213
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Adds a value to an Integer Variable. Uses FixedUpdate")]
	public class IntAddV2 : FsmStateAction
	{
		// Token: 0x06006099 RID: 24729 RVA: 0x001EA19D File Offset: 0x001E839D
		public override void Reset()
		{
			this.intVariable = null;
			this.add = null;
			this.everyFrame = false;
		}

		// Token: 0x0600609A RID: 24730 RVA: 0x001EA1B4 File Offset: 0x001E83B4
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x0600609B RID: 24731 RVA: 0x001EA1C2 File Offset: 0x001E83C2
		public override void OnEnter()
		{
			this.intVariable.Value += this.add.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600609C RID: 24732 RVA: 0x001EA1EF File Offset: 0x001E83EF
		public override void OnFixedUpdate()
		{
			this.intVariable.Value += this.add.Value;
		}

		// Token: 0x04005E1A RID: 24090
		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmInt intVariable;

		// Token: 0x04005E1B RID: 24091
		[RequiredField]
		public FsmInt add;

		// Token: 0x04005E1C RID: 24092
		public bool everyFrame;
	}
}
