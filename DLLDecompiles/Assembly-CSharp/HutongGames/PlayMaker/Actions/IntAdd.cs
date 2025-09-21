using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F80 RID: 3968
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Adds a value to an Integer Variable.")]
	public class IntAdd : FsmStateAction
	{
		// Token: 0x06006DDD RID: 28125 RVA: 0x0022188C File Offset: 0x0021FA8C
		public override void Reset()
		{
			this.intVariable = null;
			this.add = null;
			this.everyFrame = false;
		}

		// Token: 0x06006DDE RID: 28126 RVA: 0x002218A3 File Offset: 0x0021FAA3
		public override void OnEnter()
		{
			this.intVariable.Value += this.add.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006DDF RID: 28127 RVA: 0x002218D0 File Offset: 0x0021FAD0
		public override void OnUpdate()
		{
			this.intVariable.Value += this.add.Value;
		}

		// Token: 0x04006D92 RID: 28050
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The variable to add to.")]
		public FsmInt intVariable;

		// Token: 0x04006D93 RID: 28051
		[RequiredField]
		[Tooltip("The value to add.")]
		public FsmInt add;

		// Token: 0x04006D94 RID: 28052
		[Tooltip("Repeat every frame. NOTE: This operation will NOT be frame rate independent!")]
		public bool everyFrame;
	}
}
