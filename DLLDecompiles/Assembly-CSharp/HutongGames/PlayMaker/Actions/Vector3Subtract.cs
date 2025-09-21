using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020011AD RID: 4525
	[ActionCategory(ActionCategory.Vector3)]
	[Tooltip("Subtracts a Vector3 value from a Vector3 variable.")]
	public class Vector3Subtract : FsmStateAction
	{
		// Token: 0x060078EC RID: 30956 RVA: 0x00249376 File Offset: 0x00247576
		public override void Reset()
		{
			this.vector3Variable = null;
			this.subtractVector = new FsmVector3
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x060078ED RID: 30957 RVA: 0x00249398 File Offset: 0x00247598
		public override void OnEnter()
		{
			this.vector3Variable.Value = this.vector3Variable.Value - this.subtractVector.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060078EE RID: 30958 RVA: 0x002493CE File Offset: 0x002475CE
		public override void OnUpdate()
		{
			this.vector3Variable.Value = this.vector3Variable.Value - this.subtractVector.Value;
		}

		// Token: 0x0400794A RID: 31050
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector3 variable to subtract from.")]
		public FsmVector3 vector3Variable;

		// Token: 0x0400794B RID: 31051
		[RequiredField]
		[Tooltip("The Vector3 to subtract.")]
		public FsmVector3 subtractVector;

		// Token: 0x0400794C RID: 31052
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
