using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DEE RID: 3566
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns the feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
	public class GetAnimatorFeetPivotActive : ComponentAction<Animator>
	{
		// Token: 0x060066FF RID: 26367 RVA: 0x0020907D File Offset: 0x0020727D
		public override void Reset()
		{
			this.gameObject = null;
			this.feetPivotActive = null;
		}

		// Token: 0x06006700 RID: 26368 RVA: 0x0020908D File Offset: 0x0020728D
		public override void OnEnter()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.feetPivotActive.Value = this.cachedComponent.feetPivotActive;
			}
			base.Finish();
		}

		// Token: 0x04006655 RID: 26197
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006656 RID: 26198
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The feet pivot Blending. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
		public FsmFloat feetPivotActive;
	}
}
