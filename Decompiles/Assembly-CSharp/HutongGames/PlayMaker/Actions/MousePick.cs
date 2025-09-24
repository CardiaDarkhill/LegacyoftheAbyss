using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F10 RID: 3856
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Perform a Mouse Pick on the scene from the Main Camera and stores the results. Use Ray Distance to set how close the camera must be to pick the object.")]
	public class MousePick : FsmStateAction
	{
		// Token: 0x06006BC6 RID: 27590 RVA: 0x002181F4 File Offset: 0x002163F4
		public override void Reset()
		{
			this.rayDistance = 100f;
			this.storeDidPickObject = null;
			this.storeGameObject = null;
			this.storePoint = null;
			this.storeNormal = null;
			this.storeDistance = null;
			this.layerMask = new FsmInt[0];
			this.invertMask = false;
			this.everyFrame = false;
		}

		// Token: 0x06006BC7 RID: 27591 RVA: 0x00218253 File Offset: 0x00216453
		public override void OnEnter()
		{
			this.DoMousePick();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006BC8 RID: 27592 RVA: 0x00218269 File Offset: 0x00216469
		public override void OnUpdate()
		{
			this.DoMousePick();
		}

		// Token: 0x06006BC9 RID: 27593 RVA: 0x00218274 File Offset: 0x00216474
		private void DoMousePick()
		{
			RaycastHit raycastHit = ActionHelpers.MousePick(this.rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
			bool flag = raycastHit.collider != null;
			this.storeDidPickObject.Value = flag;
			if (flag)
			{
				this.storeGameObject.Value = raycastHit.collider.gameObject;
				this.storeDistance.Value = raycastHit.distance;
				this.storePoint.Value = raycastHit.point;
				this.storeNormal.Value = raycastHit.normal;
				return;
			}
			this.storeGameObject.Value = null;
			this.storeDistance.Value = float.PositiveInfinity;
			this.storePoint.Value = Vector3.zero;
			this.storeNormal.Value = Vector3.zero;
		}

		// Token: 0x04006B16 RID: 27414
		[RequiredField]
		[Tooltip("Set the length of the ray to cast from the Main Camera.")]
		public FsmFloat rayDistance = 100f;

		// Token: 0x04006B17 RID: 27415
		[UIHint(UIHint.Variable)]
		[Tooltip("Set Bool variable true if an object was picked, false if not.")]
		public FsmBool storeDidPickObject;

		// Token: 0x04006B18 RID: 27416
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the picked GameObject.")]
		public FsmGameObject storeGameObject;

		// Token: 0x04006B19 RID: 27417
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the point of contact.")]
		public FsmVector3 storePoint;

		// Token: 0x04006B1A RID: 27418
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the normal at the point of contact.\nNote, this is a direction vector not a rotation. Use Look At Direction to rotate a GameObject to this direction.")]
		public FsmVector3 storeNormal;

		// Token: 0x04006B1B RID: 27419
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance to the point of contact.")]
		public FsmFloat storeDistance;

		// Token: 0x04006B1C RID: 27420
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		// Token: 0x04006B1D RID: 27421
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04006B1E RID: 27422
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
