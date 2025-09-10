using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E07 RID: 3591
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Sets the position and rotation of the body. A GameObject can be set to control the position and rotation, or it can be manually expressed.")]
	public class SetAnimatorBody : ComponentAction<Animator>
	{
		// Token: 0x17000BDF RID: 3039
		// (get) Token: 0x0600677A RID: 26490 RVA: 0x0020A27B File Offset: 0x0020847B
		private Animator animator
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x0600677B RID: 26491 RVA: 0x0020A283 File Offset: 0x00208483
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.rotation = new FsmQuaternion
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x0600677C RID: 26492 RVA: 0x0020A2BE File Offset: 0x002084BE
		public override void OnPreprocess()
		{
			base.Fsm.HandleAnimatorIK = true;
		}

		// Token: 0x0600677D RID: 26493 RVA: 0x0020A2CC File Offset: 0x002084CC
		public override void DoAnimatorIK(int layerIndex)
		{
			this.DoSetBody();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600677E RID: 26494 RVA: 0x0020A2E4 File Offset: 0x002084E4
		private void DoSetBody()
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
			if (!(this._transform != null))
			{
				if (!this.position.IsNone)
				{
					this.animator.bodyPosition = this.position.Value;
				}
				if (!this.rotation.IsNone)
				{
					this.animator.bodyRotation = this.rotation.Value;
				}
				return;
			}
			if (this.position.IsNone)
			{
				this.animator.bodyPosition = this._transform.position;
			}
			else
			{
				this.animator.bodyPosition = this._transform.position + this.position.Value;
			}
			if (this.rotation.IsNone)
			{
				this.animator.bodyRotation = this._transform.rotation;
				return;
			}
			this.animator.bodyRotation = this._transform.rotation * this.rotation.Value;
		}

		// Token: 0x040066BC RID: 26300
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The GameObject with an Animator Component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066BD RID: 26301
		[Tooltip("The gameObject target of the ik goal")]
		public FsmGameObject target;

		// Token: 0x040066BE RID: 26302
		[Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
		public FsmVector3 position;

		// Token: 0x040066BF RID: 26303
		[Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
		public FsmQuaternion rotation;

		// Token: 0x040066C0 RID: 26304
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040066C1 RID: 26305
		private GameObject cachedTarget;

		// Token: 0x040066C2 RID: 26306
		private Transform _transform;
	}
}
