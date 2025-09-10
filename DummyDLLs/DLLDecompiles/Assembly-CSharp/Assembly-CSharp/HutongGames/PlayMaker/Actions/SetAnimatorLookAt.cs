using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E10 RID: 3600
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets look at position and weights. You can use a target GameObject or position.")]
	public class SetAnimatorLookAt : ComponentAction<Animator>
	{
		// Token: 0x17000BE4 RID: 3044
		// (get) Token: 0x060067A7 RID: 26535 RVA: 0x0020AB3B File Offset: 0x00208D3B
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060067A8 RID: 26536 RVA: 0x0020AB44 File Offset: 0x00208D44
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.targetPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.weight = 1f;
			this.bodyWeight = 0.3f;
			this.headWeight = 0.6f;
			this.eyesWeight = 1f;
			this.clampWeight = 0.5f;
			this.everyFrame = false;
		}

		// Token: 0x060067A9 RID: 26537 RVA: 0x0020ABC8 File Offset: 0x00208DC8
		public override void OnPreprocess()
		{
			base.Fsm.HandleAnimatorIK = true;
		}

		// Token: 0x060067AA RID: 26538 RVA: 0x0020ABD6 File Offset: 0x00208DD6
		public override void OnEnter()
		{
		}

		// Token: 0x060067AB RID: 26539 RVA: 0x0020ABD8 File Offset: 0x00208DD8
		public override void DoAnimatorIK(int layerIndex)
		{
			this.DoSetLookAt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067AC RID: 26540 RVA: 0x0020ABF0 File Offset: 0x00208DF0
		private void DoSetLookAt()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				base.Finish();
				return;
			}
			if (this.cachedTarget != this.target.Value)
			{
				this.cachedTarget = this.target.Value;
				this._transform = ((this.cachedTarget != null) ? this.cachedTarget.transform : null);
			}
			if (this._transform != null)
			{
				if (this.targetPosition.IsNone)
				{
					this.animator.SetLookAtPosition(this._transform.position);
				}
				else
				{
					this.animator.SetLookAtPosition(this._transform.position + this.targetPosition.Value);
				}
			}
			else if (!this.targetPosition.IsNone)
			{
				this.animator.SetLookAtPosition(this.targetPosition.Value);
			}
			if (!this.clampWeight.IsNone)
			{
				this.animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value, this.headWeight.Value, this.eyesWeight.Value, this.clampWeight.Value);
				return;
			}
			if (!this.eyesWeight.IsNone)
			{
				this.animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value, this.headWeight.Value, this.eyesWeight.Value);
				return;
			}
			if (!this.headWeight.IsNone)
			{
				this.animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value, this.headWeight.Value);
				return;
			}
			if (!this.bodyWeight.IsNone)
			{
				this.animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value);
				return;
			}
			if (!this.weight.IsNone)
			{
				this.animator.SetLookAtWeight(this.weight.Value);
			}
		}

		// Token: 0x040066E8 RID: 26344
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066E9 RID: 26345
		[Tooltip("The GameObject to look at. Set to None to use a position instead.")]
		public FsmGameObject target;

		// Token: 0x040066EA RID: 26346
		[Tooltip("The look-at position. If Target GameObject is set, this is used as an offset from the Target's position.")]
		public FsmVector3 targetPosition;

		// Token: 0x040066EB RID: 26347
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The global weight of the LookAt, multiplier for other parameters. Range from 0 to 1")]
		public FsmFloat weight;

		// Token: 0x040066EC RID: 26348
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Determines how much the body is involved in the LookAt. Range from 0 to 1")]
		public FsmFloat bodyWeight;

		// Token: 0x040066ED RID: 26349
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Determines how much the head is involved in the LookAt. Range from 0 to 1")]
		public FsmFloat headWeight;

		// Token: 0x040066EE RID: 26350
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Determines how much the eyes are involved in the LookAt. Range from 0 to 1")]
		public FsmFloat eyesWeight;

		// Token: 0x040066EF RID: 26351
		[HasFloatSlider(0f, 1f)]
		[Tooltip("0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).")]
		public FsmFloat clampWeight;

		// Token: 0x040066F0 RID: 26352
		[Tooltip("Repeat every frame during OnAnimatorIK(). This would normally be true.")]
		public bool everyFrame;

		// Token: 0x040066F1 RID: 26353
		private GameObject cachedTarget;

		// Token: 0x040066F2 RID: 26354
		private Transform _transform;
	}
}
