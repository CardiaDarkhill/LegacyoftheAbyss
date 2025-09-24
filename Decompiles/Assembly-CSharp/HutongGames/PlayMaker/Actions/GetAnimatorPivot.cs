using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DFE RID: 3582
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Returns the pivot weight and/or position. The pivot is the most stable point between the avatar's left and right foot.\n For a weight value of 0, the left foot is the most stable point For a value of 1, the right foot is the most stable point")]
	public class GetAnimatorPivot : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BDB RID: 3035
		// (get) Token: 0x0600674A RID: 26442 RVA: 0x00209C61 File Offset: 0x00207E61
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x0600674B RID: 26443 RVA: 0x00209C69 File Offset: 0x00207E69
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.pivotWeight = null;
			this.pivotPosition = null;
		}

		// Token: 0x0600674C RID: 26444 RVA: 0x00209C86 File Offset: 0x00207E86
		public override void OnEnter()
		{
			this.DoCheckPivot();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600674D RID: 26445 RVA: 0x00209C9C File Offset: 0x00207E9C
		public override void OnActionUpdate()
		{
			this.DoCheckPivot();
		}

		// Token: 0x0600674E RID: 26446 RVA: 0x00209CA4 File Offset: 0x00207EA4
		private void DoCheckPivot()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			if (!this.pivotWeight.IsNone)
			{
				this.pivotWeight.Value = this.animator.pivotWeight;
			}
			if (!this.pivotPosition.IsNone)
			{
				this.pivotPosition.Value = this.animator.pivotPosition;
			}
		}

		// Token: 0x0400669C RID: 26268
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400669D RID: 26269
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The pivot is the most stable point between the avatar's left and right foot.\n For a value of 0, the left foot is the most stable point For a value of 1, the right foot is the most stable point")]
		public FsmFloat pivotWeight;

		// Token: 0x0400669E RID: 26270
		[UIHint(UIHint.Variable)]
		[Tooltip("The pivot is the most stable point between the avatar's left and right foot.\n For a value of 0, the left foot is the most stable point For a value of 1, the right foot is the most stable point")]
		public FsmVector3 pivotPosition;
	}
}
