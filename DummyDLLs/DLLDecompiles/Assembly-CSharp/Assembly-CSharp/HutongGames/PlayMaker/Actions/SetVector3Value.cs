using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200119C RID: 4508
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Sets the value of a Vector3 Variable.")]
	public class SetVector3Value : FsmStateAction
	{
		// Token: 0x060078A1 RID: 30881 RVA: 0x002484D5 File Offset: 0x002466D5
		public override void Reset()
		{
			this.vector3Variable = null;
			this.vector3Value = null;
			this.everyFrame = false;
		}

		// Token: 0x060078A2 RID: 30882 RVA: 0x002484EC File Offset: 0x002466EC
		public override void OnEnter()
		{
			this.vector3Variable.Value = this.vector3Value.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078A3 RID: 30883 RVA: 0x00248512 File Offset: 0x00246712
		public override void OnUpdate()
		{
			this.vector3Variable.Value = this.vector3Value.Value;
		}

		// Token: 0x04007907 RID: 30983
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Vector3 variable to set.")]
		public FsmVector3 vector3Variable;

		// Token: 0x04007908 RID: 30984
		[RequiredField]
		[Tooltip("Value to set variable to.")]
		public FsmVector3 vector3Value;

		// Token: 0x04007909 RID: 30985
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
