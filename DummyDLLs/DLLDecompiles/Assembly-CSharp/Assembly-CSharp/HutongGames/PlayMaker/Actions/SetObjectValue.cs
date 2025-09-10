using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200117F RID: 4479
	[ActionCategory(ActionCategory.UnityObject)]
	[Tooltip("Sets the value of an Object Variable.")]
	public class SetObjectValue : FsmStateAction
	{
		// Token: 0x06007822 RID: 30754 RVA: 0x0024705F File Offset: 0x0024525F
		public override void Reset()
		{
			this.objectVariable = null;
			this.objectValue = null;
			this.everyFrame = false;
		}

		// Token: 0x06007823 RID: 30755 RVA: 0x00247076 File Offset: 0x00245276
		public override void OnEnter()
		{
			this.objectVariable.Value = this.objectValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007824 RID: 30756 RVA: 0x0024709C File Offset: 0x0024529C
		public override void OnUpdate()
		{
			this.objectVariable.Value = this.objectValue.Value;
		}

		// Token: 0x0400789E RID: 30878
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Object Variable to set.")]
		public FsmObject objectVariable;

		// Token: 0x0400789F RID: 30879
		[RequiredField]
		[Tooltip("The value.")]
		public FsmObject objectValue;

		// Token: 0x040078A0 RID: 30880
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
