using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FC8 RID: 4040
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Gets info on the last 2d Raycast or LineCast and store in variables.")]
	public class GetRayCastHit2dInfo : FsmStateAction
	{
		// Token: 0x06006F6E RID: 28526 RVA: 0x002276B9 File Offset: 0x002258B9
		public override void Reset()
		{
			this.gameObjectHit = null;
			this.point = null;
			this.normal = null;
			this.distance = null;
			this.everyFrame = false;
		}

		// Token: 0x06006F6F RID: 28527 RVA: 0x002276DE File Offset: 0x002258DE
		public override void OnEnter()
		{
			this.StoreRaycastInfo();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F70 RID: 28528 RVA: 0x002276F4 File Offset: 0x002258F4
		public override void OnUpdate()
		{
			this.StoreRaycastInfo();
		}

		// Token: 0x06006F71 RID: 28529 RVA: 0x002276FC File Offset: 0x002258FC
		private void StoreRaycastInfo()
		{
			RaycastHit2D lastRaycastHit2DInfo = Fsm.GetLastRaycastHit2DInfo(base.Fsm);
			if (lastRaycastHit2DInfo.collider != null)
			{
				this.gameObjectHit.Value = lastRaycastHit2DInfo.collider.gameObject;
				this.point.Value = lastRaycastHit2DInfo.point;
				this.normal.Value = lastRaycastHit2DInfo.normal;
				this.distance.Value = lastRaycastHit2DInfo.fraction;
			}
		}

		// Token: 0x04006F3E RID: 28478
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the GameObject hit by the last Raycast and store it in a variable.")]
		public FsmGameObject gameObjectHit;

		// Token: 0x04006F3F RID: 28479
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the world position of the ray hit point and store it in a variable.")]
		[Title("Hit Point")]
		public FsmVector2 point;

		// Token: 0x04006F40 RID: 28480
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the normal at the hit point and store it in a variable.")]
		public FsmVector3 normal;

		// Token: 0x04006F41 RID: 28481
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
		public FsmFloat distance;

		// Token: 0x04006F42 RID: 28482
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
