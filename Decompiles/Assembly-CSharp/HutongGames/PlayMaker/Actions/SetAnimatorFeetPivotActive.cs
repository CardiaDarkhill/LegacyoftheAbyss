using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E0A RID: 3594
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Activates feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
	public class SetAnimatorFeetPivotActive : ComponentAction<Animator>
	{
		// Token: 0x06006789 RID: 26505 RVA: 0x0020A5A5 File Offset: 0x002087A5
		public override void Reset()
		{
			this.gameObject = null;
			this.feetPivotActive = null;
		}

		// Token: 0x0600678A RID: 26506 RVA: 0x0020A5B5 File Offset: 0x002087B5
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.cachedComponent.feetPivotActive = this.feetPivotActive.Value;
			}
			base.Finish();
		}

		// Token: 0x040066CB RID: 26315
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066CC RID: 26316
		[Tooltip("Activates feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
		public FsmFloat feetPivotActive;
	}
}
