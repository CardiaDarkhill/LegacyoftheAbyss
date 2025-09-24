using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E0C RID: 3596
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the position, rotation and weights of an IK goal. A GameObject can be set to control the position and rotation, or it can be manually expressed.")]
	public class SetAnimatorIKGoal : ComponentAction<Animator>
	{
		// Token: 0x17000BE2 RID: 3042
		// (get) Token: 0x06006792 RID: 26514 RVA: 0x0020A71B File Offset: 0x0020891B
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x06006793 RID: 26515 RVA: 0x0020A724 File Offset: 0x00208924
		public override void Reset()
		{
			this.gameObject = null;
			this.goal = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.rotation = new FsmQuaternion
			{
				UseVariable = true
			};
			this.positionWeight = 1f;
			this.rotationWeight = 1f;
			this.everyFrame = false;
		}

		// Token: 0x06006794 RID: 26516 RVA: 0x0020A78A File Offset: 0x0020898A
		public override void OnPreprocess()
		{
			base.Fsm.HandleAnimatorIK = true;
		}

		// Token: 0x06006795 RID: 26517 RVA: 0x0020A798 File Offset: 0x00208998
		public override void OnEnter()
		{
		}

		// Token: 0x06006796 RID: 26518 RVA: 0x0020A79A File Offset: 0x0020899A
		public override void DoAnimatorIK(int layerIndex)
		{
			this.DoSetIKGoal();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006797 RID: 26519 RVA: 0x0020A7B0 File Offset: 0x002089B0
		private void DoSetIKGoal()
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
			if (this._transform != null)
			{
				if (this.position.IsNone)
				{
					this.animator.SetIKPosition(this.iKGoal, this._transform.position);
				}
				else
				{
					this.animator.SetIKPosition(this.iKGoal, this._transform.position + this.position.Value);
				}
				if (this.rotation.IsNone)
				{
					this.animator.SetIKRotation(this.iKGoal, this._transform.rotation);
				}
				else
				{
					this.animator.SetIKRotation(this.iKGoal, this._transform.rotation * this.rotation.Value);
				}
			}
			else
			{
				if (!this.position.IsNone)
				{
					this.animator.SetIKPosition(this.iKGoal, this.position.Value);
				}
				if (!this.rotation.IsNone)
				{
					this.animator.SetIKRotation(this.iKGoal, this.rotation.Value);
				}
			}
			if (!this.positionWeight.IsNone)
			{
				this.animator.SetIKPositionWeight(this.iKGoal, this.positionWeight.Value);
			}
			if (!this.rotationWeight.IsNone)
			{
				this.animator.SetIKRotationWeight(this.iKGoal, this.rotationWeight.Value);
			}
		}

		// Token: 0x040066D3 RID: 26323
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066D4 RID: 26324
		[Tooltip("The IK goal")]
		public AvatarIKGoal iKGoal;

		// Token: 0x040066D5 RID: 26325
		[Tooltip("The gameObject target of the ik goal")]
		public FsmGameObject goal;

		// Token: 0x040066D6 RID: 26326
		[Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
		public FsmVector3 position;

		// Token: 0x040066D7 RID: 26327
		[Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
		public FsmQuaternion rotation;

		// Token: 0x040066D8 RID: 26328
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The translative weight of an IK goal (0 = at the original animation before IK, 1 = at the goal)")]
		public FsmFloat positionWeight;

		// Token: 0x040066D9 RID: 26329
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Sets the rotational weight of an IK goal (0 = rotation before IK, 1 = rotation at the IK goal)")]
		public FsmFloat rotationWeight;

		// Token: 0x040066DA RID: 26330
		[Tooltip("Repeat every frame. Useful when changing over time.")]
		public bool everyFrame;

		// Token: 0x040066DB RID: 26331
		private GameObject cachedGoal;

		// Token: 0x040066DC RID: 26332
		private Transform _transform;
	}
}
