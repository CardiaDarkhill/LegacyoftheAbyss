using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DE7 RID: 3559
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the current State information on a specified layer")]
	public class GetAnimatorCurrentStateInfo : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BCC RID: 3020
		// (get) Token: 0x060066D5 RID: 26325 RVA: 0x0020888B File Offset: 0x00206A8B
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060066D6 RID: 26326 RVA: 0x00208894 File Offset: 0x00206A94
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
			this.everyFrame = false;
		}

		// Token: 0x060066D7 RID: 26327 RVA: 0x00208902 File Offset: 0x00206B02
		public override void OnEnter()
		{
			this.GetLayerInfo();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066D8 RID: 26328 RVA: 0x00208918 File Offset: 0x00206B18
		public override void OnActionUpdate()
		{
			this.GetLayerInfo();
		}

		// Token: 0x060066D9 RID: 26329 RVA: 0x00208920 File Offset: 0x00206B20
		private void GetLayerInfo()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = this.animator.GetCurrentAnimatorStateInfo(this.layerIndex.Value);
			if (!this.fullPathHash.IsNone)
			{
				this.fullPathHash.Value = currentAnimatorStateInfo.fullPathHash;
			}
			if (!this.shortPathHash.IsNone)
			{
				this.shortPathHash.Value = currentAnimatorStateInfo.shortNameHash;
			}
			if (!this.nameHash.IsNone)
			{
				this.nameHash.Value = currentAnimatorStateInfo.fullPathHash;
			}
			if (!this.name.IsNone)
			{
				this.name.Value = this.animator.GetLayerName(this.layerIndex.Value);
			}
			if (!this.tagHash.IsNone)
			{
				this.tagHash.Value = currentAnimatorStateInfo.tagHash;
			}
			if (!this.length.IsNone)
			{
				this.length.Value = currentAnimatorStateInfo.length;
			}
			if (!this.isStateLooping.IsNone)
			{
				this.isStateLooping.Value = currentAnimatorStateInfo.loop;
			}
			if (!this.normalizedTime.IsNone)
			{
				this.normalizedTime.Value = currentAnimatorStateInfo.normalizedTime;
			}
			if (!this.loopCount.IsNone || !this.currentLoopProgress.IsNone)
			{
				this.loopCount.Value = (int)Math.Truncate((double)currentAnimatorStateInfo.normalizedTime);
				this.currentLoopProgress.Value = currentAnimatorStateInfo.normalizedTime - (float)this.loopCount.Value;
			}
		}

		// Token: 0x04006628 RID: 26152
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006629 RID: 26153
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		// Token: 0x0400662A RID: 26154
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The layer's name.")]
		public FsmString name;

		// Token: 0x0400662B RID: 26155
		[UIHint(UIHint.Variable)]
		[Tooltip("The layer's name Hash. Obsolete in Unity 5, use fullPathHash or shortPathHash instead, nameHash will be the same as shortNameHash for legacy")]
		public FsmInt nameHash;

		// Token: 0x0400662C RID: 26156
		[UIHint(UIHint.Variable)]
		[Tooltip("The full path hash for this state.")]
		public FsmInt fullPathHash;

		// Token: 0x0400662D RID: 26157
		[UIHint(UIHint.Variable)]
		[Tooltip("The name Hash. Does not include the parent layer's name")]
		public FsmInt shortPathHash;

		// Token: 0x0400662E RID: 26158
		[UIHint(UIHint.Variable)]
		[Tooltip("The layer's tag hash")]
		public FsmInt tagHash;

		// Token: 0x0400662F RID: 26159
		[UIHint(UIHint.Variable)]
		[Tooltip("Is the state looping. All animations in the state must be looping")]
		public FsmBool isStateLooping;

		// Token: 0x04006630 RID: 26160
		[UIHint(UIHint.Variable)]
		[Tooltip("The Current duration of the state. In seconds, can vary when the State contains a Blend Tree ")]
		public FsmFloat length;

		// Token: 0x04006631 RID: 26161
		[UIHint(UIHint.Variable)]
		[Tooltip("The integer part is the number of time a state has been looped. The fractional part is the % (0-1) of progress in the current loop")]
		public FsmFloat normalizedTime;

		// Token: 0x04006632 RID: 26162
		[UIHint(UIHint.Variable)]
		[Tooltip("The integer part is the number of time a state has been looped. This is extracted from the normalizedTime")]
		public FsmInt loopCount;

		// Token: 0x04006633 RID: 26163
		[UIHint(UIHint.Variable)]
		[Tooltip("The progress in the current loop. This is extracted from the normalizedTime")]
		public FsmFloat currentLoopProgress;
	}
}
