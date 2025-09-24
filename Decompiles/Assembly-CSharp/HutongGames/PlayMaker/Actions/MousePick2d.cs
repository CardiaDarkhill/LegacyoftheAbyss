using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FD2 RID: 4050
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Perform a Mouse Pick on a 2d scene and stores the results. Use Ray Distance to set how close the camera must be to pick the 2d object.")]
	public class MousePick2d : FsmStateAction
	{
		// Token: 0x06006FA1 RID: 28577 RVA: 0x0022822F File Offset: 0x0022642F
		public override void Reset()
		{
			this.storeDidPickObject = null;
			this.storeGameObject = null;
			this.storePoint = null;
			this.layerMask = new FsmInt[0];
			this.invertMask = false;
			this.everyFrame = false;
		}

		// Token: 0x06006FA2 RID: 28578 RVA: 0x00228265 File Offset: 0x00226465
		public override void OnEnter()
		{
			this.DoMousePick2d();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006FA3 RID: 28579 RVA: 0x0022827B File Offset: 0x0022647B
		public override void OnUpdate()
		{
			this.DoMousePick2d();
		}

		// Token: 0x06006FA4 RID: 28580 RVA: 0x00228284 File Offset: 0x00226484
		private void DoMousePick2d()
		{
			Vector3 mousePosition = Input.mousePosition;
			RaycastHit2D rayIntersection = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(mousePosition), float.PositiveInfinity, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
			bool flag = rayIntersection.collider != null;
			this.storeDidPickObject.Value = flag;
			if (flag)
			{
				this.storeGameObject.Value = rayIntersection.collider.gameObject;
				this.storePoint.Value = rayIntersection.point;
				return;
			}
			this.storeGameObject.Value = null;
			this.storePoint.Value = Vector3.zero;
		}

		// Token: 0x04006F81 RID: 28545
		[UIHint(UIHint.Variable)]
		[Tooltip("Store if a GameObject was picked in a Bool variable. True if a GameObject was picked, otherwise false.")]
		public FsmBool storeDidPickObject;

		// Token: 0x04006F82 RID: 28546
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the picked GameObject in a variable.")]
		public FsmGameObject storeGameObject;

		// Token: 0x04006F83 RID: 28547
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the picked point in a variable.")]
		public FsmVector2 storePoint;

		// Token: 0x04006F84 RID: 28548
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		// Token: 0x04006F85 RID: 28549
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04006F86 RID: 28550
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
