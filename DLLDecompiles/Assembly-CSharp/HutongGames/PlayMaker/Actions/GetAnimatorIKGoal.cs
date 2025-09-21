using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DF2 RID: 3570
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Gets the position, rotation and weights of an IK goal. A GameObject can be set to use for the position and rotation")]
	public class GetAnimatorIKGoal : FsmStateActionAnimatorBase
	{
		// Token: 0x17000BD5 RID: 3029
		// (get) Token: 0x06006711 RID: 26385 RVA: 0x00209272 File Offset: 0x00207472
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x06006712 RID: 26386 RVA: 0x0020927A File Offset: 0x0020747A
		public override void Reset()
		{
			base.Reset();
			this.gameObject = null;
			this.iKGoal = null;
			this.goal = null;
			this.position = null;
			this.rotation = null;
			this.positionWeight = null;
			this.rotationWeight = null;
		}

		// Token: 0x06006713 RID: 26387 RVA: 0x002092B3 File Offset: 0x002074B3
		public override void OnEnter()
		{
		}

		// Token: 0x06006714 RID: 26388 RVA: 0x002092B5 File Offset: 0x002074B5
		public override void OnActionUpdate()
		{
			this.DoGetIKGoal();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006715 RID: 26389 RVA: 0x002092CC File Offset: 0x002074CC
		private void DoGetIKGoal()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			if (this.cachedGoal != this.goal.Value)
			{
				this.cachedGoal = this.goal.Value;
				this._transform = ((this.cachedGoal != null) ? this.cachedGoal.transform : null);
			}
			this._iKGoal = (AvatarIKGoal)this.iKGoal.Value;
			if (this._transform != null)
			{
				this._transform.position = this.animator.GetIKPosition(this._iKGoal);
				this._transform.rotation = this.animator.GetIKRotation(this._iKGoal);
			}
			if (!this.position.IsNone)
			{
				this.position.Value = this.animator.GetIKPosition(this._iKGoal);
			}
			if (!this.rotation.IsNone)
			{
				this.rotation.Value = this.animator.GetIKRotation(this._iKGoal);
			}
			if (!this.positionWeight.IsNone)
			{
				this.positionWeight.Value = this.animator.GetIKPositionWeight(this._iKGoal);
			}
			if (!this.rotationWeight.IsNone)
			{
				this.rotationWeight.Value = this.animator.GetIKRotationWeight(this._iKGoal);
			}
		}

		// Token: 0x04006660 RID: 26208
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006661 RID: 26209
		[Tooltip("The IK goal")]
		[ObjectType(typeof(AvatarIKGoal))]
		public FsmEnum iKGoal;

		// Token: 0x04006662 RID: 26210
		[ActionSection("Results")]
		[UIHint(UIHint.Variable)]
		[Tooltip("The gameObject to apply ik goal position and rotation to.")]
		public FsmGameObject goal;

		// Token: 0x04006663 RID: 26211
		[UIHint(UIHint.Variable)]
		[Tooltip("Gets The position of the ik goal. If Goal GameObject is defined, position is used as an offset from Goal")]
		public FsmVector3 position;

		// Token: 0x04006664 RID: 26212
		[UIHint(UIHint.Variable)]
		[Tooltip("Gets The rotation of the ik goal.If Goal GameObject define, rotation is used as an offset from Goal")]
		public FsmQuaternion rotation;

		// Token: 0x04006665 RID: 26213
		[UIHint(UIHint.Variable)]
		[Tooltip("Gets The translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal)")]
		public FsmFloat positionWeight;

		// Token: 0x04006666 RID: 26214
		[UIHint(UIHint.Variable)]
		[Tooltip("Gets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal)")]
		public FsmFloat rotationWeight;

		// Token: 0x04006667 RID: 26215
		private GameObject cachedGoal;

		// Token: 0x04006668 RID: 26216
		private Transform _transform;

		// Token: 0x04006669 RID: 26217
		private AvatarIKGoal _iKGoal;
	}
}
