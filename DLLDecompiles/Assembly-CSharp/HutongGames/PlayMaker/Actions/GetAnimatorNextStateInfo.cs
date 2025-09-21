using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DFD RID: 3581
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the next State information on a specified layer")]
	public class GetAnimatorNextStateInfo : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BDA RID: 3034
		// (get) Token: 0x06006744 RID: 26436 RVA: 0x00209A2A File Offset: 0x00207C2A
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x06006745 RID: 26437 RVA: 0x00209A34 File Offset: 0x00207C34
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.layerIndex = null;
			this.name = null;
			this.nameHash = null;
			this.fullPathHash = null;
			this.shortPathHash = null;
			this.tagHash = null;
			this.length = null;
			this.normalizedTime = null;
			this.isStateLooping = null;
			this.loopCount = null;
			this.currentLoopProgress = null;
		}

		// Token: 0x06006746 RID: 26438 RVA: 0x00209A9B File Offset: 0x00207C9B
		public override void OnEnter()
		{
			this.GetLayerInfo();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006747 RID: 26439 RVA: 0x00209AB1 File Offset: 0x00207CB1
		public override void OnActionUpdate()
		{
			this.GetLayerInfo();
		}

		// Token: 0x06006748 RID: 26440 RVA: 0x00209ABC File Offset: 0x00207CBC
		private void GetLayerInfo()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			AnimatorStateInfo nextAnimatorStateInfo = this.animator.GetNextAnimatorStateInfo(this.layerIndex.Value);
			if (!this.fullPathHash.IsNone)
			{
				this.fullPathHash.Value = nextAnimatorStateInfo.fullPathHash;
			}
			if (!this.shortPathHash.IsNone)
			{
				this.shortPathHash.Value = nextAnimatorStateInfo.shortNameHash;
			}
			if (!this.nameHash.IsNone)
			{
				this.nameHash.Value = nextAnimatorStateInfo.shortNameHash;
			}
			if (!this.name.IsNone)
			{
				this.name.Value = this.animator.GetLayerName(this.layerIndex.Value);
			}
			if (!this.tagHash.IsNone)
			{
				this.tagHash.Value = nextAnimatorStateInfo.tagHash;
			}
			if (!this.length.IsNone)
			{
				this.length.Value = nextAnimatorStateInfo.length;
			}
			if (!this.isStateLooping.IsNone)
			{
				this.isStateLooping.Value = nextAnimatorStateInfo.loop;
			}
			if (!this.normalizedTime.IsNone)
			{
				this.normalizedTime.Value = nextAnimatorStateInfo.normalizedTime;
			}
			if (!this.loopCount.IsNone || !this.currentLoopProgress.IsNone)
			{
				this.loopCount.Value = (int)Math.Truncate((double)nextAnimatorStateInfo.normalizedTime);
				this.currentLoopProgress.Value = nextAnimatorStateInfo.normalizedTime - (float)this.loopCount.Value;
			}
		}

		// Token: 0x04006690 RID: 26256
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006691 RID: 26257
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		// Token: 0x04006692 RID: 26258
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The layer's name.")]
		public FsmString name;

		// Token: 0x04006693 RID: 26259
		[UIHint(UIHint.Variable)]
		[Tooltip("The layer's name Hash. Obsolete in Unity 5, use fullPathHash or shortPathHash instead, nameHash will be the same as shortNameHash for legacy")]
		public FsmInt nameHash;

		// Token: 0x04006694 RID: 26260
		[UIHint(UIHint.Variable)]
		[Tooltip("The full path hash for this state.")]
		public FsmInt fullPathHash;

		// Token: 0x04006695 RID: 26261
		[UIHint(UIHint.Variable)]
		[Tooltip("The name Hash. Does not include the parent layer's name")]
		public FsmInt shortPathHash;

		// Token: 0x04006696 RID: 26262
		[UIHint(UIHint.Variable)]
		[Tooltip("The layer's tag hash")]
		public FsmInt tagHash;

		// Token: 0x04006697 RID: 26263
		[UIHint(UIHint.Variable)]
		[Tooltip("Is the state looping. All animations in the state must be looping")]
		public FsmBool isStateLooping;

		// Token: 0x04006698 RID: 26264
		[UIHint(UIHint.Variable)]
		[Tooltip("The Current duration of the state. In seconds, can vary when the State contains a Blend Tree ")]
		public FsmFloat length;

		// Token: 0x04006699 RID: 26265
		[UIHint(UIHint.Variable)]
		[Tooltip("The integer part is the number of time a state has been looped. The fractional part is the % (0-1) of progress in the current loop")]
		public FsmFloat normalizedTime;

		// Token: 0x0400669A RID: 26266
		[UIHint(UIHint.Variable)]
		[Tooltip("The integer part is the number of time a state has been looped. This is extracted from the normalizedTime")]
		public FsmInt loopCount;

		// Token: 0x0400669B RID: 26267
		[UIHint(UIHint.Variable)]
		[Tooltip("The progress in the current loop. This is extracted from the normalizedTime")]
		public FsmFloat currentLoopProgress;
	}
}
