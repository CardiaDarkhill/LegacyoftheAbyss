using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF0 RID: 3312
	[ActionCategory("Physics 2d")]
	[Tooltip("Same as V2, but now uses FixedUpdate for performance.")]
	public class RayCast2dV3 : FsmStateAction
	{
		// Token: 0x06006256 RID: 25174 RVA: 0x001F1910 File Offset: 0x001EFB10
		public override void Reset()
		{
			this.FromGameObject = null;
			this.FromPosition = new FsmVector2
			{
				UseVariable = true
			};
			this.Direction = new FsmVector2
			{
				UseVariable = true
			};
			this.Space = Space.Self;
			this.MinDepth = new FsmInt
			{
				UseVariable = true
			};
			this.MaxDepth = new FsmInt
			{
				UseVariable = true
			};
			this.Distance = 100f;
			this.HitEvent = null;
			this.NoHitEvent = null;
			this.StoreDidHit = null;
			this.StoreHitObject = null;
			this.StoreHitPoint = null;
			this.StoreHitNormal = null;
			this.StoreHitFraction = null;
			this.StoreHitDistance = null;
			this.RepeatInterval = 1;
			this.LayerMask = Array.Empty<FsmInt>();
			this.InvertMask = false;
			this.IgnoreTriggers = false;
			this.DebugColor = Color.yellow;
			this.Debug = false;
		}

		// Token: 0x06006257 RID: 25175 RVA: 0x001F1A06 File Offset: 0x001EFC06
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006258 RID: 25176 RVA: 0x001F1A14 File Offset: 0x001EFC14
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.FromGameObject);
			if (ownerDefaultTarget != null)
			{
				this.trans = ownerDefaultTarget.transform;
			}
			this.DoRaycast();
			if (this.RepeatInterval.Value == 0)
			{
				base.Finish();
			}
		}

		// Token: 0x06006259 RID: 25177 RVA: 0x001F1A61 File Offset: 0x001EFC61
		public override void OnFixedUpdate()
		{
			this.repeat--;
			if (this.repeat == 0)
			{
				this.DoRaycast();
			}
		}

		// Token: 0x0600625A RID: 25178 RVA: 0x001F1A80 File Offset: 0x001EFC80
		private void DoRaycast()
		{
			this.repeat = this.RepeatInterval.Value;
			if (this.Distance.Value == 0f)
			{
				return;
			}
			Vector2 value = this.FromPosition.Value;
			if (this.trans != null)
			{
				Vector3 position = this.trans.position;
				value.x += position.x;
				value.y += position.y;
			}
			float num = float.PositiveInfinity;
			if (this.Distance.Value > 0f)
			{
				num = this.Distance.Value;
			}
			Vector2 normalized = this.Direction.Value.normalized;
			if (this.trans != null && this.Space == Space.Self)
			{
				Vector3 vector = this.trans.TransformDirection(new Vector3(this.Direction.Value.x, this.Direction.Value.y, 0f));
				normalized.x = vector.x;
				normalized.y = vector.y;
			}
			RaycastHit2D info;
			if (this.MinDepth.IsNone && this.MaxDepth.IsNone)
			{
				if (this.IgnoreTriggers.Value)
				{
					Helper.IsRayHittingNoTriggers(value, normalized, num, ActionHelpers.LayerArrayToLayerMask(this.LayerMask, this.InvertMask.Value), out info);
				}
				else
				{
					info = Helper.Raycast2D(value, normalized, num, ActionHelpers.LayerArrayToLayerMask(this.LayerMask, this.InvertMask.Value));
				}
			}
			else
			{
				float minDepth = this.MinDepth.IsNone ? float.NegativeInfinity : ((float)this.MinDepth.Value);
				float maxDepth = this.MaxDepth.IsNone ? float.PositiveInfinity : ((float)this.MaxDepth.Value);
				info = Helper.Raycast2D(value, normalized, num, ActionHelpers.LayerArrayToLayerMask(this.LayerMask, this.InvertMask.Value), minDepth, maxDepth);
			}
			if (info.collider != null && this.IgnoreTriggers.Value && info.collider.isTrigger)
			{
				info = default(RaycastHit2D);
			}
			bool flag = info.collider != null;
			PlayMakerUnity2d.RecordLastRaycastHitInfo(base.Fsm, info);
			this.StoreDidHit.Value = flag;
			if (flag)
			{
				this.StoreHitObject.Value = info.collider.gameObject;
				this.StoreHitPoint.Value = info.point;
				this.StoreHitNormal.Value = info.normal;
				this.StoreHitFraction.Value = info.fraction;
				this.StoreHitDistance.Value = info.distance;
				base.Fsm.Event(this.HitEvent);
			}
			else
			{
				base.Fsm.Event(this.NoHitEvent);
			}
			if (this.Debug.Value)
			{
				float d = Mathf.Min(num, 1000f);
				Vector3 vector2 = new Vector3(value.x, value.y, 0f);
				Vector3 a = new Vector3(normalized.x, normalized.y, 0f);
				Vector3 end = vector2 + a * d;
				UnityEngine.Debug.DrawLine(vector2, end, this.DebugColor.Value);
			}
		}

		// Token: 0x04006099 RID: 24729
		[ActionSection("Setup")]
		public FsmOwnerDefault FromGameObject;

		// Token: 0x0400609A RID: 24730
		public FsmVector2 FromPosition;

		// Token: 0x0400609B RID: 24731
		public FsmVector2 Direction;

		// Token: 0x0400609C RID: 24732
		public Space Space;

		// Token: 0x0400609D RID: 24733
		public FsmFloat Distance;

		// Token: 0x0400609E RID: 24734
		public FsmInt MinDepth;

		// Token: 0x0400609F RID: 24735
		public FsmInt MaxDepth;

		// Token: 0x040060A0 RID: 24736
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmEvent HitEvent;

		// Token: 0x040060A1 RID: 24737
		[UIHint(UIHint.Variable)]
		public FsmEvent NoHitEvent;

		// Token: 0x040060A2 RID: 24738
		[UIHint(UIHint.Variable)]
		public FsmBool StoreDidHit;

		// Token: 0x040060A3 RID: 24739
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreHitObject;

		// Token: 0x040060A4 RID: 24740
		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreHitPoint;

		// Token: 0x040060A5 RID: 24741
		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreHitNormal;

		// Token: 0x040060A6 RID: 24742
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreHitFraction;

		// Token: 0x040060A7 RID: 24743
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreHitDistance;

		// Token: 0x040060A8 RID: 24744
		[ActionSection("Filter")]
		[Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nSince raycasts can get expensive use the highest repeat interval you can get away with.")]
		public FsmInt RepeatInterval;

		// Token: 0x040060A9 RID: 24745
		[UIHint(UIHint.Layer)]
		public FsmInt[] LayerMask;

		// Token: 0x040060AA RID: 24746
		public FsmBool InvertMask;

		// Token: 0x040060AB RID: 24747
		public FsmBool IgnoreTriggers;

		// Token: 0x040060AC RID: 24748
		[ActionSection("Debug")]
		[Tooltip("The color to use for the debug line.")]
		public FsmColor DebugColor;

		// Token: 0x040060AD RID: 24749
		[Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
		public FsmBool Debug;

		// Token: 0x040060AE RID: 24750
		private Transform trans;

		// Token: 0x040060AF RID: 24751
		private int repeat;
	}
}
