using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F13 RID: 3859
	[ActionCategory(ActionCategory.Input)]
	[Tooltip("Perform a raycast into the scene using screen coordinates and stores the results. Use Ray Distance to set how close the camera must be to pick the object. NOTE: Uses the MainCamera!")]
	public class ScreenPick : FsmStateAction
	{
		// Token: 0x06006BD5 RID: 27605 RVA: 0x00218544 File Offset: 0x00216744
		public override void Reset()
		{
			this.screenVector = new FsmVector3
			{
				UseVariable = true
			};
			this.screenX = new FsmFloat
			{
				UseVariable = true
			};
			this.screenY = new FsmFloat
			{
				UseVariable = true
			};
			this.normalized = false;
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

		// Token: 0x06006BD6 RID: 27606 RVA: 0x002185E5 File Offset: 0x002167E5
		public override void OnEnter()
		{
			this.DoScreenPick();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006BD7 RID: 27607 RVA: 0x002185FB File Offset: 0x002167FB
		public override void OnUpdate()
		{
			this.DoScreenPick();
		}

		// Token: 0x06006BD8 RID: 27608 RVA: 0x00218604 File Offset: 0x00216804
		private void DoScreenPick()
		{
			if (Camera.main == null)
			{
				base.LogError("No MainCamera defined!");
				base.Finish();
				return;
			}
			Vector3 pos = Vector3.zero;
			if (!this.screenVector.IsNone)
			{
				pos = this.screenVector.Value;
			}
			if (!this.screenX.IsNone)
			{
				pos.x = this.screenX.Value;
			}
			if (!this.screenY.IsNone)
			{
				pos.y = this.screenY.Value;
			}
			if (this.normalized.Value)
			{
				pos.x *= (float)Screen.width;
				pos.y *= (float)Screen.height;
			}
			RaycastHit raycastHit;
			Physics.Raycast(Camera.main.ScreenPointToRay(pos), out raycastHit, this.rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
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
			this.storeDistance = float.PositiveInfinity;
			this.storePoint.Value = Vector3.zero;
			this.storeNormal.Value = Vector3.zero;
		}

		// Token: 0x04006B28 RID: 27432
		[Tooltip("A Vector3 screen position. Commonly stored by other actions.")]
		public FsmVector3 screenVector;

		// Token: 0x04006B29 RID: 27433
		[Tooltip("X position on screen.")]
		public FsmFloat screenX;

		// Token: 0x04006B2A RID: 27434
		[Tooltip("Y position on screen.")]
		public FsmFloat screenY;

		// Token: 0x04006B2B RID: 27435
		[Tooltip("Are the supplied screen coordinates normalized (0-1), or in pixels.")]
		public FsmBool normalized;

		// Token: 0x04006B2C RID: 27436
		[RequiredField]
		[Tooltip("The length of the ray to use.")]
		public FsmFloat rayDistance = 100f;

		// Token: 0x04006B2D RID: 27437
		[UIHint(UIHint.Variable)]
		[Tooltip("Store whether the ray hit an object in a Bool Variable.")]
		public FsmBool storeDidPickObject;

		// Token: 0x04006B2E RID: 27438
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the hit Game Object in a Game Object Variable.")]
		public FsmGameObject storeGameObject;

		// Token: 0x04006B2F RID: 27439
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the world position of the hit point in a Vector3 Variable.")]
		public FsmVector3 storePoint;

		// Token: 0x04006B30 RID: 27440
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the normal of the hit point in a Vector3 Variable.\nNote, this is a direction vector not a rotation. Use Look At Direction to rotate a GameObject to this direction.")]
		public FsmVector3 storeNormal;

		// Token: 0x04006B31 RID: 27441
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the distance to the hit point.")]
		public FsmFloat storeDistance;

		// Token: 0x04006B32 RID: 27442
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers. Set a number then select layers.")]
		public FsmInt[] layerMask;

		// Token: 0x04006B33 RID: 27443
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04006B34 RID: 27444
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
