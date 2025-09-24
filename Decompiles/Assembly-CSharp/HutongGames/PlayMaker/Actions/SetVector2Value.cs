using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001185 RID: 4485
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Sets the value of a Vector2 Variable.")]
	public class SetVector2Value : FsmStateAction
	{
		// Token: 0x0600783B RID: 30779 RVA: 0x00247365 File Offset: 0x00245565
		public override void Reset()
		{
			this.vector2Variable = null;
			this.vector2Value = null;
			this.everyFrame = false;
		}

		// Token: 0x0600783C RID: 30780 RVA: 0x0024737C File Offset: 0x0024557C
		public override void OnEnter()
		{
			this.vector2Variable.Value = this.vector2Value.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600783D RID: 30781 RVA: 0x002473A2 File Offset: 0x002455A2
		public override void OnUpdate()
		{
			this.vector2Variable.Value = this.vector2Value.Value;
		}

		// Token: 0x040078AF RID: 30895
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector2 target")]
		public FsmVector2 vector2Variable;

		// Token: 0x040078B0 RID: 30896
		[RequiredField]
		[Tooltip("The vector2 source")]
		public FsmVector2 vector2Value;

		// Token: 0x040078B1 RID: 30897
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
