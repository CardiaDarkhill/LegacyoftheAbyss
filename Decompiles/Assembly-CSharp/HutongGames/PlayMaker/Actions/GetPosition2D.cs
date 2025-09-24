using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C7A RID: 3194
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the 2D Position of a Game Object and stores it in a Vector2 Variable or each Axis in a Float Variable")]
	public class GetPosition2D : FsmStateAction
	{
		// Token: 0x0600603D RID: 24637 RVA: 0x001E7756 File Offset: 0x001E5956
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = null;
			this.y = null;
			this.space = Space.World;
			this.everyFrame = false;
		}

		// Token: 0x0600603E RID: 24638 RVA: 0x001E7782 File Offset: 0x001E5982
		public override void OnEnter()
		{
			this.DoGetPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600603F RID: 24639 RVA: 0x001E7798 File Offset: 0x001E5998
		public override void OnUpdate()
		{
			this.DoGetPosition();
		}

		// Token: 0x06006040 RID: 24640 RVA: 0x001E77A0 File Offset: 0x001E59A0
		private void DoGetPosition()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector = (this.space == Space.World) ? ownerDefaultTarget.transform.position : ownerDefaultTarget.transform.localPosition;
			this.vector.Value = vector;
			this.x.Value = vector.x;
			this.y.Value = vector.y;
		}

		// Token: 0x04005D91 RID: 23953
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005D92 RID: 23954
		[UIHint(UIHint.Variable)]
		public FsmVector2 vector;

		// Token: 0x04005D93 RID: 23955
		[UIHint(UIHint.Variable)]
		public FsmFloat x;

		// Token: 0x04005D94 RID: 23956
		[UIHint(UIHint.Variable)]
		public FsmFloat y;

		// Token: 0x04005D95 RID: 23957
		public Space space;

		// Token: 0x04005D96 RID: 23958
		public bool everyFrame;
	}
}
