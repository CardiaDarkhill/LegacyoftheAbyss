using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C4A RID: 3146
	[ActionCategory(ActionCategory.Math)]
	[Tooltip("Divides one Float by another.")]
	public class FloatDivideV2 : FsmStateAction
	{
		// Token: 0x06005F6A RID: 24426 RVA: 0x001E4E18 File Offset: 0x001E3018
		public override void Reset()
		{
			this.floatVariable = null;
			this.divideBy = null;
			this.everyFrame = false;
			this.fixedUpdate = false;
		}

		// Token: 0x06005F6B RID: 24427 RVA: 0x001E4E36 File Offset: 0x001E3036
		public override void OnEnter()
		{
			this.floatVariable.Value /= this.divideBy.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005F6C RID: 24428 RVA: 0x001E4E63 File Offset: 0x001E3063
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005F6D RID: 24429 RVA: 0x001E4E71 File Offset: 0x001E3071
		public override void OnUpdate()
		{
			if (!this.fixedUpdate)
			{
				this.floatVariable.Value /= this.divideBy.Value;
			}
		}

		// Token: 0x06005F6E RID: 24430 RVA: 0x001E4E98 File Offset: 0x001E3098
		public override void OnFixedUpdate()
		{
			if (this.fixedUpdate)
			{
				this.floatVariable.Value /= this.divideBy.Value;
			}
		}

		// Token: 0x04005CC8 RID: 23752
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The float variable to divide.")]
		public FsmFloat floatVariable;

		// Token: 0x04005CC9 RID: 23753
		[RequiredField]
		[Tooltip("Divide the float variable by this value.")]
		public FsmFloat divideBy;

		// Token: 0x04005CCA RID: 23754
		[Tooltip("Repeate every frame. Useful if the variables are changing.")]
		public bool everyFrame;

		// Token: 0x04005CCB RID: 23755
		public bool fixedUpdate;
	}
}
