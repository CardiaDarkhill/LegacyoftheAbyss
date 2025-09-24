using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200118C RID: 4492
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Reverses the direction of a Vector2 Variable. Same as multiplying by -1.")]
	public class Vector2Invert : FsmStateAction
	{
		// Token: 0x0600785B RID: 30811 RVA: 0x00247923 File Offset: 0x00245B23
		public override void Reset()
		{
			this.vector2Variable = null;
			this.everyFrame = false;
		}

		// Token: 0x0600785C RID: 30812 RVA: 0x00247933 File Offset: 0x00245B33
		public override void OnEnter()
		{
			this.vector2Variable.Value = this.vector2Variable.Value * -1f;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600785D RID: 30813 RVA: 0x00247963 File Offset: 0x00245B63
		public override void OnUpdate()
		{
			this.vector2Variable.Value = this.vector2Variable.Value * -1f;
		}

		// Token: 0x040078CF RID: 30927
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector to invert")]
		public FsmVector2 vector2Variable;

		// Token: 0x040078D0 RID: 30928
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
