using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001190 RID: 4496
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Multiplies a Vector2 variable by a Float.")]
	public class Vector2Multiply : FsmStateAction
	{
		// Token: 0x0600786C RID: 30828 RVA: 0x00247BF7 File Offset: 0x00245DF7
		public override void Reset()
		{
			this.vector2Variable = null;
			this.multiplyBy = 1f;
			this.everyFrame = false;
		}

		// Token: 0x0600786D RID: 30829 RVA: 0x00247C17 File Offset: 0x00245E17
		public override void OnEnter()
		{
			this.vector2Variable.Value = this.vector2Variable.Value * this.multiplyBy.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600786E RID: 30830 RVA: 0x00247C4D File Offset: 0x00245E4D
		public override void OnUpdate()
		{
			this.vector2Variable.Value = this.vector2Variable.Value * this.multiplyBy.Value;
		}

		// Token: 0x040078DE RID: 30942
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector to Multiply")]
		public FsmVector2 vector2Variable;

		// Token: 0x040078DF RID: 30943
		[RequiredField]
		[Tooltip("The multiplication factor")]
		public FsmFloat multiplyBy;

		// Token: 0x040078E0 RID: 30944
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
