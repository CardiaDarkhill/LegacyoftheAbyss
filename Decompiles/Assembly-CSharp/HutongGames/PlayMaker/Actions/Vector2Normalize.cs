using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001191 RID: 4497
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Normalizes a Vector2 Variable.")]
	public class Vector2Normalize : FsmStateAction
	{
		// Token: 0x06007870 RID: 30832 RVA: 0x00247C7D File Offset: 0x00245E7D
		public override void Reset()
		{
			this.vector2Variable = null;
			this.everyFrame = false;
		}

		// Token: 0x06007871 RID: 30833 RVA: 0x00247C90 File Offset: 0x00245E90
		public override void OnEnter()
		{
			this.vector2Variable.Value = this.vector2Variable.Value.normalized;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007872 RID: 30834 RVA: 0x00247CCC File Offset: 0x00245ECC
		public override void OnUpdate()
		{
			this.vector2Variable.Value = this.vector2Variable.Value.normalized;
		}

		// Token: 0x040078E1 RID: 30945
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector to normalize")]
		public FsmVector2 vector2Variable;

		// Token: 0x040078E2 RID: 30946
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
