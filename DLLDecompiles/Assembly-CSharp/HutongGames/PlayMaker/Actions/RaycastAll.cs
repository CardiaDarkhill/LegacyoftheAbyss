using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FAC RID: 4012
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Casts a Ray against all Colliders in the scene. Use either a GameObject or Vector3 world position as the origin of the ray. Use GetRaycastAllInfo to get more detailed info.")]
	public class RaycastAll : FsmStateAction
	{
		// Token: 0x06006ECB RID: 28363 RVA: 0x00224764 File Offset: 0x00222964
		public override void Reset()
		{
			this.fromGameObject = null;
			this.fromPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.direction = new FsmVector3
			{
				UseVariable = true
			};
			this.space = Space.Self;
			this.distance = new FsmFloat
			{
				Value = 100f
			};
			this.hitEvent = null;
			this.storeDidHit = null;
			this.storeHitObjects = null;
			this.storeHitPoint = null;
			this.storeHitNormal = null;
			this.storeHitDistance = null;
			this.repeatInterval = new FsmInt
			{
				Value = 1
			};
			this.layerMask = new FsmInt[0];
			this.invertMask = null;
			this.debugColor = new FsmColor
			{
				Value = Color.yellow
			};
			this.debug = null;
		}

		// Token: 0x06006ECC RID: 28364 RVA: 0x00224825 File Offset: 0x00222A25
		public override void OnEnter()
		{
			this.DoRaycast();
			if (this.repeatInterval.Value == 0)
			{
				base.Finish();
			}
		}

		// Token: 0x06006ECD RID: 28365 RVA: 0x00224840 File Offset: 0x00222A40
		public override void OnUpdate()
		{
			this.repeat--;
			if (this.repeat == 0)
			{
				this.DoRaycast();
			}
		}

		// Token: 0x06006ECE RID: 28366 RVA: 0x00224860 File Offset: 0x00222A60
		private void DoRaycast()
		{
			this.repeat = this.repeatInterval.Value;
			if (this.distance.Value == 0f)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
			Vector3 vector = (ownerDefaultTarget != null) ? ownerDefaultTarget.transform.position : this.fromPosition.Value;
			float num = float.PositiveInfinity;
			if (this.distance.Value > 0f)
			{
				num = this.distance.Value;
			}
			Vector3 a = this.direction.Value;
			if (ownerDefaultTarget != null && this.space == Space.Self)
			{
				a = ownerDefaultTarget.transform.TransformDirection(this.direction.Value);
			}
			RaycastAll.RaycastAllHitInfo = Physics.RaycastAll(vector, a, num, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
			bool flag = RaycastAll.RaycastAllHitInfo.Length != 0;
			this.storeDidHit.Value = flag;
			if (flag)
			{
				GameObject[] array = new GameObject[RaycastAll.RaycastAllHitInfo.Length];
				for (int i = 0; i < RaycastAll.RaycastAllHitInfo.Length; i++)
				{
					RaycastHit raycastHit = RaycastAll.RaycastAllHitInfo[i];
					array[i] = raycastHit.collider.gameObject;
				}
				FsmArray fsmArray = this.storeHitObjects;
				object[] values = array;
				fsmArray.Values = values;
				this.storeHitPoint.Value = base.Fsm.RaycastHitInfo.point;
				this.storeHitNormal.Value = base.Fsm.RaycastHitInfo.normal;
				this.storeHitDistance.Value = base.Fsm.RaycastHitInfo.distance;
				base.Fsm.Event(this.hitEvent);
			}
			if (this.debug.Value)
			{
				float d = Mathf.Min(num, 1000f);
				Debug.DrawLine(vector, vector + a * d, this.debugColor.Value);
			}
		}

		// Token: 0x04006E76 RID: 28278
		public static RaycastHit[] RaycastAllHitInfo;

		// Token: 0x04006E77 RID: 28279
		[Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
		public FsmOwnerDefault fromGameObject;

		// Token: 0x04006E78 RID: 28280
		[Tooltip("Start ray at a vector3 world position. \nOr use Game Object parameter.")]
		public FsmVector3 fromPosition;

		// Token: 0x04006E79 RID: 28281
		[Tooltip("A vector3 direction vector")]
		public FsmVector3 direction;

		// Token: 0x04006E7A RID: 28282
		[Tooltip("Cast the ray in world or local space. Note if no Game Object is specified, the direction is in world space.")]
		public Space space;

		// Token: 0x04006E7B RID: 28283
		[Tooltip("The length of the ray. Set to -1 for infinity.")]
		public FsmFloat distance;

		// Token: 0x04006E7C RID: 28284
		[ActionSection("Result")]
		[Tooltip("Event to send if the ray hits an object.")]
		[UIHint(UIHint.Variable)]
		public FsmEvent hitEvent;

		// Token: 0x04006E7D RID: 28285
		[Tooltip("Set a bool variable to true if hit something, otherwise false.")]
		[UIHint(UIHint.Variable)]
		public FsmBool storeDidHit;

		// Token: 0x04006E7E RID: 28286
		[Tooltip("Store the GameObjects hit in an array variable.")]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray storeHitObjects;

		// Token: 0x04006E7F RID: 28287
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the world position of the ray hit point and store it in a variable.")]
		public FsmVector3 storeHitPoint;

		// Token: 0x04006E80 RID: 28288
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the normal at the hit point and store it in a variable.\nNote, this is a direction vector not a rotation. Use Look At Direction to rotate a GameObject to this direction.")]
		public FsmVector3 storeHitNormal;

		// Token: 0x04006E81 RID: 28289
		[UIHint(UIHint.Variable)]
		[Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
		public FsmFloat storeHitDistance;

		// Token: 0x04006E82 RID: 28290
		[ActionSection("Filter")]
		[Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nBecause raycasts can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt repeatInterval;

		// Token: 0x04006E83 RID: 28291
		[UIHint(UIHint.Layer)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		// Token: 0x04006E84 RID: 28292
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04006E85 RID: 28293
		[ActionSection("Debug")]
		[Tooltip("The color to use for the debug line.")]
		public FsmColor debugColor;

		// Token: 0x04006E86 RID: 28294
		[Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
		public FsmBool debug;

		// Token: 0x04006E87 RID: 28295
		private int repeat;
	}
}
