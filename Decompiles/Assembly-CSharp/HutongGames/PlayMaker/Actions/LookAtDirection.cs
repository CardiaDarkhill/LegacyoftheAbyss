using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010DF RID: 4319
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Rotates a Game Object so its forward vector points in the specified Direction.")]
	public class LookAtDirection : ComponentAction<Transform>
	{
		// Token: 0x060074E1 RID: 29921 RVA: 0x0023BDD2 File Offset: 0x00239FD2
		public override void Reset()
		{
			this.gameObject = null;
			this.targetDirection = new FsmVector3
			{
				UseVariable = true
			};
			this.upVector = new FsmVector3
			{
				UseVariable = true
			};
			this.everyFrame = true;
			this.lateUpdate = true;
		}

		// Token: 0x060074E2 RID: 29922 RVA: 0x0023BE0D File Offset: 0x0023A00D
		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = this.lateUpdate;
		}

		// Token: 0x060074E3 RID: 29923 RVA: 0x0023BE20 File Offset: 0x0023A020
		public override void OnEnter()
		{
			this.DoLookAtDirection();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060074E4 RID: 29924 RVA: 0x0023BE36 File Offset: 0x0023A036
		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoLookAtDirection();
			}
		}

		// Token: 0x060074E5 RID: 29925 RVA: 0x0023BE46 File Offset: 0x0023A046
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoLookAtDirection();
			}
		}

		// Token: 0x060074E6 RID: 29926 RVA: 0x0023BE58 File Offset: 0x0023A058
		private void DoLookAtDirection()
		{
			if (this.targetDirection.IsNone)
			{
				return;
			}
			if (!base.UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			base.cachedTransform.rotation = Quaternion.LookRotation(this.targetDirection.Value, this.upVector.IsNone ? Vector3.up : this.upVector.Value);
		}

		// Token: 0x04007532 RID: 30002
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007533 RID: 30003
		[RequiredField]
		[Tooltip("The direction to look at.")]
		public FsmVector3 targetDirection;

		// Token: 0x04007534 RID: 30004
		[Tooltip("Keep this vector pointing up as the GameObject rotates.")]
		public FsmVector3 upVector;

		// Token: 0x04007535 RID: 30005
		[Tooltip("Repeat every update.")]
		public bool everyFrame;

		// Token: 0x04007536 RID: 30006
		[Tooltip("Perform in LateUpdate. This can help eliminate jitters in some situations.")]
		public bool lateUpdate;
	}
}
