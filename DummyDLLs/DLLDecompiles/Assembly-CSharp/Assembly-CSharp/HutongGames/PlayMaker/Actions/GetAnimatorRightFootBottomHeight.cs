using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E01 RID: 3585
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Get the right foot bottom height.")]
	public class GetAnimatorRightFootBottomHeight : ComponentAction<Animator>
	{
		// Token: 0x0600675A RID: 26458 RVA: 0x00209DFB File Offset: 0x00207FFB
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.rightFootHeight = null;
			this.everyFrame = false;
		}

		// Token: 0x0600675B RID: 26459 RVA: 0x00209E18 File Offset: 0x00208018
		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		// Token: 0x0600675C RID: 26460 RVA: 0x00209E26 File Offset: 0x00208026
		public override void OnEnter()
		{
			this.GetRightFootBottomHeight();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600675D RID: 26461 RVA: 0x00209E3C File Offset: 0x0020803C
		public override void OnLateUpdate()
		{
			this.GetRightFootBottomHeight();
		}

		// Token: 0x0600675E RID: 26462 RVA: 0x00209E44 File Offset: 0x00208044
		private void GetRightFootBottomHeight()
		{
			if (base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				this.rightFootHeight.Value = this.cachedComponent.rightFeetBottomHeight;
			}
		}

		// Token: 0x040066A5 RID: 26277
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066A6 RID: 26278
		[ActionSection("Result")]
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The right foot bottom height.")]
		public FsmFloat rightFootHeight;

		// Token: 0x040066A7 RID: 26279
		[Tooltip("Repeat every frame during LateUpdate. Useful when value is subject to change over time.")]
		public bool everyFrame;
	}
}
