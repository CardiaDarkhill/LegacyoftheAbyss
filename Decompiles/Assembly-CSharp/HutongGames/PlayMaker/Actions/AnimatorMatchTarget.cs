using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DDB RID: 3547
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Automatically adjust the GameObject position and rotation so that the AvatarTarget reaches the Match Position when the current animation state is at the specified progress.")]
	public class AnimatorMatchTarget : ComponentAction<Animator>
	{
		// Token: 0x17000BC8 RID: 3016
		// (get) Token: 0x060066A0 RID: 26272 RVA: 0x00207FD4 File Offset: 0x002061D4
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060066A1 RID: 26273 RVA: 0x00207FDC File Offset: 0x002061DC
		public override void Reset()
		{
			this.gameObject = null;
			this.bodyPart = AvatarTarget.Root;
			this.target = null;
			this.targetPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.targetRotation = new FsmQuaternion
			{
				UseVariable = true
			};
			this.positionWeight = Vector3.one;
			this.rotationWeight = 0f;
			this.startNormalizedTime = null;
			this.targetNormalizedTime = null;
			this.everyFrame = true;
		}

		// Token: 0x060066A2 RID: 26274 RVA: 0x00208058 File Offset: 0x00206258
		public override void OnEnter()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			if (this.cachedTarget != this.target.Value)
			{
				this.cachedTarget = this.target.Value;
				this.targetTransform = ((this.cachedTarget != null) ? this.cachedTarget.transform : null);
			}
			this.weightMask = default(MatchTargetWeightMask);
			this.DoMatchTarget();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066A3 RID: 26275 RVA: 0x002080F0 File Offset: 0x002062F0
		public override void OnUpdate()
		{
			this.DoMatchTarget();
		}

		// Token: 0x060066A4 RID: 26276 RVA: 0x002080F8 File Offset: 0x002062F8
		private void DoMatchTarget()
		{
			if (this.animator == null)
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			Quaternion quaternion = Quaternion.identity;
			if (this.targetTransform != null)
			{
				vector = this.targetTransform.position;
				quaternion = this.targetTransform.rotation;
			}
			if (!this.targetPosition.IsNone)
			{
				vector += this.targetPosition.Value;
			}
			if (!this.targetRotation.IsNone)
			{
				quaternion *= this.targetRotation.Value;
			}
			this.weightMask.positionXYZWeight = this.positionWeight.Value;
			this.weightMask.rotationWeight = this.rotationWeight.Value;
			this.animator.MatchTarget(vector, quaternion, this.bodyPart, this.weightMask, this.startNormalizedTime.Value, this.targetNormalizedTime.Value);
		}

		// Token: 0x040065F6 RID: 26102
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040065F7 RID: 26103
		[Tooltip("The body part that is used to match the target.")]
		public AvatarTarget bodyPart;

		// Token: 0x040065F8 RID: 26104
		[Tooltip("A GameObject target to match. Leave empty to use position instead.")]
		public FsmGameObject target;

		// Token: 0x040065F9 RID: 26105
		[Tooltip("A target position to match. If Target GameObject is set, this is used as an offset from the Target's position.")]
		public FsmVector3 targetPosition;

		// Token: 0x040065FA RID: 26106
		[Tooltip("A rotation to match. If Target GameObject is set, this is used as an offset from the Target's rotation.")]
		public FsmQuaternion targetRotation;

		// Token: 0x040065FB RID: 26107
		[Tooltip("The MatchTargetWeightMask Position XYZ weight")]
		public FsmVector3 positionWeight;

		// Token: 0x040065FC RID: 26108
		[Tooltip("The MatchTargetWeightMask Rotation weight")]
		public FsmFloat rotationWeight;

		// Token: 0x040065FD RID: 26109
		[Tooltip("Start time within the animation clip (0 - beginning of clip, 1 - end of clip)")]
		public FsmFloat startNormalizedTime;

		// Token: 0x040065FE RID: 26110
		[Tooltip("End time within the animation clip (0 - beginning of clip, 1 - end of clip). Values greater than 1 trigger a match after a certain number of loops. Example: 2.3 means at 30% of 2nd loop.")]
		public FsmFloat targetNormalizedTime;

		// Token: 0x040065FF RID: 26111
		[Tooltip("Should always be true")]
		public bool everyFrame;

		// Token: 0x04006600 RID: 26112
		private GameObject cachedTarget;

		// Token: 0x04006601 RID: 26113
		private Transform targetTransform;

		// Token: 0x04006602 RID: 26114
		private MatchTargetWeightMask weightMask;
	}
}
