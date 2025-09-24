using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FA4 RID: 4004
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Gets info on the last Raycast and store in variables.")]
	public class GetRaycastHitInfo : FsmStateAction
	{
		// Token: 0x06006EA2 RID: 28322 RVA: 0x00223E33 File Offset: 0x00222033
		public override void Reset()
		{
			this.gameObjectHit = null;
			this.point = null;
			this.normal = null;
			this.distance = null;
			this.everyFrame = false;
		}

		// Token: 0x06006EA3 RID: 28323 RVA: 0x00223E58 File Offset: 0x00222058
		private void StoreRaycastInfo()
		{
			if (base.Fsm.RaycastHitInfo.collider != null)
			{
				this.gameObjectHit.Value = base.Fsm.RaycastHitInfo.collider.gameObject;
				this.point.Value = base.Fsm.RaycastHitInfo.point;
				this.normal.Value = base.Fsm.RaycastHitInfo.normal;
				this.distance.Value = base.Fsm.RaycastHitInfo.distance;
			}
		}

		// Token: 0x06006EA4 RID: 28324 RVA: 0x00223EFD File Offset: 0x002220FD
		public override void OnEnter()
		{
			this.StoreRaycastInfo();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006EA5 RID: 28325 RVA: 0x00223F13 File Offset: 0x00222113
		public override void OnUpdate()
		{
			this.StoreRaycastInfo();
		}

		// Token: 0x04006E41 RID: 28225
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the GameObject hit by the last Raycast and store it in a variable.")]
		public FsmGameObject gameObjectHit;

		// Token: 0x04006E42 RID: 28226
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the world position of the ray hit point and store it in a variable.")]
		[Title("Hit Point")]
		public FsmVector3 point;

		// Token: 0x04006E43 RID: 28227
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the normal at the hit point and store it in a variable.")]
		public FsmVector3 normal;

		// Token: 0x04006E44 RID: 28228
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
		public FsmFloat distance;

		// Token: 0x04006E45 RID: 28229
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
