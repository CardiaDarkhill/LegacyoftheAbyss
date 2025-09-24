using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010D4 RID: 4308
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Align a GameObject to the specified Direction.")]
	public class AlignToDirection : ComponentAction<Transform>
	{
		// Token: 0x060074A0 RID: 29856 RVA: 0x0023AE0D File Offset: 0x0023900D
		public override void Reset()
		{
			this.gameObject = null;
			this.targetDirection = new FsmVector3
			{
				UseVariable = true
			};
			this.alignAxis = null;
			this.flipAxis = null;
			this.everyFrame = false;
			this.lateUpdate = false;
		}

		// Token: 0x060074A1 RID: 29857 RVA: 0x0023AE44 File Offset: 0x00239044
		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = this.lateUpdate;
		}

		// Token: 0x060074A2 RID: 29858 RVA: 0x0023AE57 File Offset: 0x00239057
		public override void OnEnter()
		{
			this.DoAlignToDirection();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060074A3 RID: 29859 RVA: 0x0023AE6D File Offset: 0x0023906D
		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoAlignToDirection();
			}
		}

		// Token: 0x060074A4 RID: 29860 RVA: 0x0023AE7D File Offset: 0x0023907D
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoAlignToDirection();
			}
		}

		// Token: 0x060074A5 RID: 29861 RVA: 0x0023AE90 File Offset: 0x00239090
		private void DoAlignToDirection()
		{
			if (this.targetDirection.IsNone)
			{
				return;
			}
			if (!base.UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			Vector3 vector = default(Vector3);
			switch ((AlignToDirection.AlignAxis)this.alignAxis.Value)
			{
			case AlignToDirection.AlignAxis.x:
				vector = base.cachedTransform.right;
				break;
			case AlignToDirection.AlignAxis.y:
				vector = base.cachedTransform.up;
				break;
			case AlignToDirection.AlignAxis.z:
				vector = base.cachedTransform.forward;
				break;
			}
			if (this.flipAxis.Value)
			{
				vector *= -1f;
			}
			base.cachedTransform.rotation = Quaternion.FromToRotation(vector, this.targetDirection.Value) * base.cachedTransform.rotation;
		}

		// Token: 0x040074DB RID: 29915
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040074DC RID: 29916
		[RequiredField]
		[Tooltip("The direction to look at. E.g. the Hit Normal from a Raycast.")]
		public FsmVector3 targetDirection;

		// Token: 0x040074DD RID: 29917
		[RequiredField]
		[Tooltip("The GameObject axis to align to the direction.")]
		[ObjectType(typeof(AlignToDirection.AlignAxis))]
		public FsmEnum alignAxis;

		// Token: 0x040074DE RID: 29918
		[Tooltip("Flip the alignment axis. So x becomes -x.")]
		public FsmBool flipAxis;

		// Token: 0x040074DF RID: 29919
		[Tooltip("Repeat every update.")]
		public bool everyFrame;

		// Token: 0x040074E0 RID: 29920
		[Tooltip("Perform in LateUpdate. This can help eliminate jitters in some situations.")]
		public bool lateUpdate;

		// Token: 0x02001BC9 RID: 7113
		public enum AlignAxis
		{
			// Token: 0x04009EBF RID: 40639
			x,
			// Token: 0x04009EC0 RID: 40640
			y,
			// Token: 0x04009EC1 RID: 40641
			z
		}
	}
}
