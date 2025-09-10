using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001189 RID: 4489
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Clamps the Magnitude of Vector2 Variable.")]
	public class Vector2ClampMagnitude : FsmStateAction
	{
		// Token: 0x0600784E RID: 30798 RVA: 0x0024762F File Offset: 0x0024582F
		public override void Reset()
		{
			this.vector2Variable = null;
			this.maxLength = null;
			this.everyFrame = false;
		}

		// Token: 0x0600784F RID: 30799 RVA: 0x00247646 File Offset: 0x00245846
		public override void OnEnter()
		{
			this.DoVector2ClampMagnitude();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007850 RID: 30800 RVA: 0x0024765C File Offset: 0x0024585C
		public override void OnUpdate()
		{
			this.DoVector2ClampMagnitude();
		}

		// Token: 0x06007851 RID: 30801 RVA: 0x00247664 File Offset: 0x00245864
		private void DoVector2ClampMagnitude()
		{
			this.vector2Variable.Value = Vector2.ClampMagnitude(this.vector2Variable.Value, this.maxLength.Value);
		}

		// Token: 0x040078C0 RID: 30912
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The Vector2")]
		public FsmVector2 vector2Variable;

		// Token: 0x040078C1 RID: 30913
		[RequiredField]
		[Tooltip("The maximum Magnitude")]
		public FsmFloat maxLength;

		// Token: 0x040078C2 RID: 30914
		[Tooltip("Repeat every frame")]
		public bool everyFrame;
	}
}
