using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200118E RID: 4494
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Use a low pass filter to reduce the influence of sudden changes in a Vector2 Variable.")]
	public class Vector2LowPassFilter : FsmStateAction
	{
		// Token: 0x06007864 RID: 30820 RVA: 0x00247A1A File Offset: 0x00245C1A
		public override void Reset()
		{
			this.vector2Variable = null;
			this.filteringFactor = 0.1f;
		}

		// Token: 0x06007865 RID: 30821 RVA: 0x00247A33 File Offset: 0x00245C33
		public override void OnEnter()
		{
			this.filteredVector = new Vector2(this.vector2Variable.Value.x, this.vector2Variable.Value.y);
		}

		// Token: 0x06007866 RID: 30822 RVA: 0x00247A60 File Offset: 0x00245C60
		public override void OnUpdate()
		{
			this.filteredVector.x = this.vector2Variable.Value.x * this.filteringFactor.Value + this.filteredVector.x * (1f - this.filteringFactor.Value);
			this.filteredVector.y = this.vector2Variable.Value.y * this.filteringFactor.Value + this.filteredVector.y * (1f - this.filteringFactor.Value);
			this.vector2Variable.Value = new Vector2(this.filteredVector.x, this.filteredVector.y);
		}

		// Token: 0x040078D6 RID: 30934
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Vector2 Variable to filter. Should generally come from some constantly updated input")]
		public FsmVector2 vector2Variable;

		// Token: 0x040078D7 RID: 30935
		[Tooltip("Determines how much influence new changes have. E.g., 0.1 keeps 10 percent of the unfiltered vector and 90 percent of the previously filtered value")]
		public FsmFloat filteringFactor;

		// Token: 0x040078D8 RID: 30936
		private Vector2 filteredVector;
	}
}
