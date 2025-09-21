using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010D9 RID: 4313
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Gets the 2D Position of a GameObject and stores it in a Vector2 Variable or each Axis in a Float Variable")]
	public class GetPosition2d : ComponentAction<Transform>
	{
		// Token: 0x060074BF RID: 29887 RVA: 0x0023B76B File Offset: 0x0023996B
		public override void Reset()
		{
			this.gameObject = null;
			this.vector_2d = null;
			this.x = null;
			this.y = null;
			this.space = Space.World;
			this.everyFrame = false;
		}

		// Token: 0x060074C0 RID: 29888 RVA: 0x0023B797 File Offset: 0x00239997
		public override void OnEnter()
		{
			this.DoGetPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060074C1 RID: 29889 RVA: 0x0023B7AD File Offset: 0x002399AD
		public override void OnUpdate()
		{
			this.DoGetPosition();
		}

		// Token: 0x060074C2 RID: 29890 RVA: 0x0023B7B8 File Offset: 0x002399B8
		private void DoGetPosition()
		{
			if (!base.UpdateCachedTransform(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			Vector3 vector = (this.space == Space.World) ? base.cachedTransform.position : base.cachedTransform.localPosition;
			this.vector_2d.Value = vector;
			this.x.Value = vector.x;
			this.y.Value = vector.y;
		}

		// Token: 0x04007509 RID: 29961
		[RequiredField]
		[Tooltip("The game object to examine.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400750A RID: 29962
		[UIHint(UIHint.Variable)]
		[Title("Vector2")]
		[Tooltip("Store the position in a Vector2 Variable.")]
		public FsmVector2 vector_2d;

		// Token: 0x0400750B RID: 29963
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the X coordinate in a Float Variable.")]
		public FsmFloat x;

		// Token: 0x0400750C RID: 29964
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the Y coordinate in a Float Variable.")]
		public FsmFloat y;

		// Token: 0x0400750D RID: 29965
		[Tooltip("Use world or local coordinates.")]
		public Space space;

		// Token: 0x0400750E RID: 29966
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
