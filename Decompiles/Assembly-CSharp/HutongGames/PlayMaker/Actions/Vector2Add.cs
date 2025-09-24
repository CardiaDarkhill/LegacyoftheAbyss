using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001187 RID: 4487
	[ActionCategory(ActionCategory.Vector2)]
	[Tooltip("Adds a value to Vector2 Variable.")]
	public class Vector2Add : FsmStateAction
	{
		// Token: 0x06007844 RID: 30788 RVA: 0x002474A9 File Offset: 0x002456A9
		public override void Reset()
		{
			this.vector2Variable = null;
			this.addVector = new FsmVector2
			{
				UseVariable = true
			};
			this.everyFrame = false;
			this.perSecond = false;
		}

		// Token: 0x06007845 RID: 30789 RVA: 0x002474D2 File Offset: 0x002456D2
		public override void OnEnter()
		{
			this.DoVector2Add();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007846 RID: 30790 RVA: 0x002474E8 File Offset: 0x002456E8
		public override void OnUpdate()
		{
			this.DoVector2Add();
		}

		// Token: 0x06007847 RID: 30791 RVA: 0x002474F0 File Offset: 0x002456F0
		private void DoVector2Add()
		{
			if (this.perSecond)
			{
				this.vector2Variable.Value = this.vector2Variable.Value + this.addVector.Value * Time.deltaTime;
				return;
			}
			this.vector2Variable.Value = this.vector2Variable.Value + this.addVector.Value;
		}

		// Token: 0x040078B7 RID: 30903
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The vector2 target")]
		public FsmVector2 vector2Variable;

		// Token: 0x040078B8 RID: 30904
		[RequiredField]
		[Tooltip("The vector2 to add")]
		public FsmVector2 addVector;

		// Token: 0x040078B9 RID: 30905
		[Tooltip("Repeat every frame")]
		public bool everyFrame;

		// Token: 0x040078BA RID: 30906
		[Tooltip("Add the value on a per second bases.")]
		public bool perSecond;
	}
}
