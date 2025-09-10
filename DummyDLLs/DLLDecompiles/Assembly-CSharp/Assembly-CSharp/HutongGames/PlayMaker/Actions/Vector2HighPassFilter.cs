using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200118A RID: 4490
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Use a high pass filter to isolate sudden changes in a Vector2 Variable.")]
	public class Vector2HighPassFilter : FsmStateAction
	{
		// Token: 0x06007853 RID: 30803 RVA: 0x00247694 File Offset: 0x00245894
		public override void Reset()
		{
			this.vector2Variable = null;
			this.filteringFactor = 0.1f;
		}

		// Token: 0x06007854 RID: 30804 RVA: 0x002476AD File Offset: 0x002458AD
		public override void OnEnter()
		{
			this.filteredVector = new Vector2(this.vector2Variable.Value.x, this.vector2Variable.Value.y);
		}

		// Token: 0x06007855 RID: 30805 RVA: 0x002476DC File Offset: 0x002458DC
		public override void OnUpdate()
		{
			this.filteredVector.x = this.vector2Variable.Value.x - (this.vector2Variable.Value.x * this.filteringFactor.Value + this.filteredVector.x * (1f - this.filteringFactor.Value));
			this.filteredVector.y = this.vector2Variable.Value.y - (this.vector2Variable.Value.y * this.filteringFactor.Value + this.filteredVector.y * (1f - this.filteringFactor.Value));
			this.vector2Variable.Value = new Vector2(this.filteredVector.x, this.filteredVector.y);
		}

		// Token: 0x040078C3 RID: 30915
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("Vector2 Variable to filter. Should generally come from some constantly updated input.")]
		public FsmVector2 vector2Variable;

		// Token: 0x040078C4 RID: 30916
		[Tooltip("Determines how much influence new changes have.")]
		public FsmFloat filteringFactor;

		// Token: 0x040078C5 RID: 30917
		private Vector2 filteredVector;
	}
}
