using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010D8 RID: 4312
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the Position of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
	public class GetPosition : ComponentAction<Transform>
	{
		// Token: 0x060074BA RID: 29882 RVA: 0x0023B68A File Offset: 0x0023988A
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = null;
			this.y = null;
			this.z = null;
			this.space = Space.World;
			this.everyFrame = false;
		}

		// Token: 0x060074BB RID: 29883 RVA: 0x0023B6BD File Offset: 0x002398BD
		public override void OnEnter()
		{
			this.DoGetPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060074BC RID: 29884 RVA: 0x0023B6D3 File Offset: 0x002398D3
		public override void OnUpdate()
		{
			this.DoGetPosition();
		}

		// Token: 0x060074BD RID: 29885 RVA: 0x0023B6DC File Offset: 0x002398DC
		private void DoGetPosition()
		{
			if (!base.UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			Vector3 vector = (this.space == Space.World) ? base.cachedTransform.position : base.cachedTransform.localPosition;
			this.vector.Value = vector;
			this.x.Value = vector.x;
			this.y.Value = vector.y;
			this.z.Value = vector.z;
		}

		// Token: 0x04007502 RID: 29954
		[RequiredField]
		[Tooltip("The game object to examine.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007503 RID: 29955
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the position in a Vector3 Variable.")]
		public FsmVector3 vector;

		// Token: 0x04007504 RID: 29956
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the X coordinate in a Float Variable.")]
		public FsmFloat x;

		// Token: 0x04007505 RID: 29957
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Y coordinate in a Float Variable.")]
		public FsmFloat y;

		// Token: 0x04007506 RID: 29958
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Z coordinate in a Float Variable.")]
		public FsmFloat z;

		// Token: 0x04007507 RID: 29959
		[Tooltip("Use world or local coordinates.")]
		public Space space;

		// Token: 0x04007508 RID: 29960
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
