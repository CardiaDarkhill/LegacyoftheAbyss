using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001197 RID: 4503
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Subtracts a Vector2 value from a Vector2 variable.")]
	public class Vector2Subtract : FsmStateAction
	{
		// Token: 0x0600788B RID: 30859 RVA: 0x002481DF File Offset: 0x002463DF
		public override void Reset()
		{
			this.vector2Variable = null;
			this.subtractVector = new FsmVector2
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x0600788C RID: 30860 RVA: 0x00248201 File Offset: 0x00246401
		public override void OnEnter()
		{
			this.vector2Variable.Value = this.vector2Variable.Value - this.subtractVector.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600788D RID: 30861 RVA: 0x00248237 File Offset: 0x00246437
		public override void OnUpdate()
		{
			this.vector2Variable.Value = this.vector2Variable.Value - this.subtractVector.Value;
		}

		// Token: 0x040078F7 RID: 30967
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector2 operand")]
		public FsmVector2 vector2Variable;

		// Token: 0x040078F8 RID: 30968
		[RequiredField]
		[Tooltip("The vector2 to subtract with")]
		public FsmVector2 subtractVector;

		// Token: 0x040078F9 RID: 30969
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
