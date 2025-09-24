using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE4 RID: 3044
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Detect additional collisions between objects with additional raycasting.")]
	public class CheckCollisionSide : FsmStateAction
	{
		// Token: 0x06005D41 RID: 23873 RVA: 0x001D523B File Offset: 0x001D343B
		public override void Reset()
		{
			this.checkUp = false;
			this.checkDown = false;
			this.checkLeft = false;
			this.checkRight = false;
		}

		// Token: 0x06005D42 RID: 23874 RVA: 0x001D5259 File Offset: 0x001D3459
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
			base.Fsm.HandleCollisionExit2D = true;
		}

		// Token: 0x06005D43 RID: 23875 RVA: 0x001D5273 File Offset: 0x001D3473
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
			base.Fsm.HandleCollisionExit2D = true;
		}

		// Token: 0x06005D44 RID: 23876 RVA: 0x001D5290 File Offset: 0x001D3490
		private void RemoveEventProxy()
		{
			if (this.eventProxy)
			{
				if (this.responder != null)
				{
					this.eventProxy.Remove(this.responder);
					this.responder = null;
				}
				else
				{
					this.eventProxy.Remove(this);
				}
				this.eventProxy = null;
				return;
			}
			if (this.responder != null)
			{
				this.responder.pendingRemoval = true;
				this.responder = null;
			}
		}

		// Token: 0x06005D45 RID: 23877 RVA: 0x001D52FC File Offset: 0x001D34FC
		public override void OnEnter()
		{
			this.eventProxy = CustomPlayMakerCollisionStay2D.GetEventSender(base.Fsm.Owner.gameObject);
			this.responder = this.eventProxy.Add(this, new Action<Collision2D>(this.DoCollisionStay2D));
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.collidingObject);
			if (ownerDefaultTarget == null)
			{
				this.hasBody = false;
				this.col2d = null;
				this.body = null;
				base.Finish();
				return;
			}
			this.terrainLayerMask = ((this.ignoreTriggers.Value && ownerDefaultTarget.layer == 9) ? 33562880 : 33554688);
			this.col2d = ownerDefaultTarget.GetComponent<Collider2D>();
			this.body = ownerDefaultTarget.GetComponent<Rigidbody2D>();
			this.hasBody = (this.body != null);
			this.topRays = new List<Vector2>(3);
			this.rightRays = new List<Vector2>(3);
			this.bottomRays = new List<Vector2>(3);
			this.leftRays = new List<Vector2>(3);
			if (!this.topHit.IsNone || this.topHitEvent != null)
			{
				this.checkUp = true;
			}
			else
			{
				this.checkUp = false;
			}
			if (!this.rightHit.IsNone || this.rightHitEvent != null)
			{
				this.checkRight = true;
			}
			else
			{
				this.checkRight = false;
			}
			if (!this.bottomHit.IsNone || this.bottomHitEvent != null)
			{
				this.checkDown = true;
			}
			else
			{
				this.checkDown = false;
			}
			if (!this.leftHit.IsNone || this.leftHitEvent != null)
			{
				this.checkLeft = true;
			}
			else
			{
				this.checkLeft = false;
			}
			if (this.topHit.Value || this.bottomHit.Value || this.rightHit.Value || this.leftHit.Value)
			{
				if (!this.otherLayer)
				{
					this.CheckTouching(this.terrainLayerMask);
					return;
				}
				this.CheckTouching(1 << this.otherLayerNumber);
			}
		}

		// Token: 0x06005D46 RID: 23878 RVA: 0x001D54EB File Offset: 0x001D36EB
		public override void OnExit()
		{
			this.RemoveEventProxy();
		}

		// Token: 0x06005D47 RID: 23879 RVA: 0x001D54F4 File Offset: 0x001D36F4
		public override void OnFixedUpdate()
		{
			if (this.topHit.Value || this.bottomHit.Value || this.rightHit.Value || this.leftHit.Value)
			{
				if (!this.otherLayer)
				{
					this.CheckTouching(this.terrainLayerMask);
					return;
				}
				this.CheckTouching(1 << this.otherLayerNumber);
			}
		}

		// Token: 0x06005D48 RID: 23880 RVA: 0x001D555C File Offset: 0x001D375C
		public override void DoCollisionStay2D(Collision2D collision)
		{
			if (base.Fsm.Finished)
			{
				return;
			}
			if (!this.otherLayer)
			{
				if ((1 << collision.gameObject.layer & this.terrainLayerMask) != 0)
				{
					this.CheckTouching(this.terrainLayerMask);
					return;
				}
			}
			else
			{
				this.CheckTouching(1 << this.otherLayerNumber);
			}
		}

		// Token: 0x06005D49 RID: 23881 RVA: 0x001D55B6 File Offset: 0x001D37B6
		public override void DoCollisionExit2D(Collision2D collision)
		{
			this.topHit.Value = false;
			this.rightHit.Value = false;
			this.bottomHit.Value = false;
			this.leftHit.Value = false;
		}

		// Token: 0x06005D4A RID: 23882 RVA: 0x001D55E8 File Offset: 0x001D37E8
		private void CheckTouching(int layerMask)
		{
			if (this.lastUpdateCycle == CustomPlayerLoop.FixedUpdateCycle)
			{
				return;
			}
			this.lastUpdateCycle = CustomPlayerLoop.FixedUpdateCycle;
			Bounds bounds = this.col2d.bounds;
			Vector3 max = bounds.max;
			Vector3 min = bounds.min;
			Vector3 center = bounds.center;
			if (this.checkUp && (!this.hasBody || this.body.linearVelocity.y >= -0.001f))
			{
				this.topRays.Clear();
				this.topRays.Add(new Vector2(min.x, max.y));
				this.topRays.Add(new Vector2(center.x, max.y));
				this.topRays.Add(max);
				this.topHit.Value = false;
				for (int i = 0; i < 3; i++)
				{
					Collider2D collider = Helper.Raycast2D(this.topRays[i], Vector2.up, 0.08f, layerMask).collider;
					if (!(collider == null) && (!this.ignoreTriggers.Value || !collider.isTrigger))
					{
						this.topHit.Value = true;
						base.Fsm.Event(this.topHitEvent);
						break;
					}
				}
			}
			if (this.checkRight && (!this.hasBody || this.body.linearVelocity.x >= -0.001f))
			{
				this.rightRays.Clear();
				this.rightRays.Add(max);
				this.rightRays.Add(new Vector2(max.x, center.y));
				this.rightRays.Add(new Vector2(max.x, min.y));
				this.rightHit.Value = false;
				for (int j = 0; j < 3; j++)
				{
					Collider2D collider2 = Helper.Raycast2D(this.rightRays[j], Vector2.right, 0.08f, layerMask).collider;
					if (collider2 != null && (!this.ignoreTriggers.Value || !collider2.isTrigger))
					{
						this.rightHit.Value = true;
						base.Fsm.Event(this.rightHitEvent);
						break;
					}
				}
			}
			if (this.checkDown && (!this.hasBody || this.body.linearVelocity.y <= 0.001f))
			{
				this.bottomRays.Clear();
				this.bottomRays.Add(new Vector2(max.x, min.y));
				this.bottomRays.Add(new Vector2(center.x, min.y));
				this.bottomRays.Add(min);
				this.bottomHit.Value = false;
				for (int k = 0; k < 3; k++)
				{
					Collider2D collider3 = Helper.Raycast2D(this.bottomRays[k], -Vector2.up, 0.08f, layerMask).collider;
					if (collider3 != null && (!this.ignoreTriggers.Value || !collider3.isTrigger))
					{
						this.bottomHit.Value = true;
						base.Fsm.Event(this.bottomHitEvent);
						break;
					}
				}
			}
			if (this.checkLeft && (!this.hasBody || this.body.linearVelocity.x <= 0.001f))
			{
				this.leftRays.Clear();
				this.leftRays.Add(min);
				this.leftRays.Add(new Vector2(min.x, center.y));
				this.leftRays.Add(new Vector2(min.x, max.y));
				this.leftHit.Value = false;
				for (int l = 0; l < 3; l++)
				{
					Collider2D collider4 = Helper.Raycast2D(this.leftRays[l], -Vector2.right, 0.08f, layerMask).collider;
					if (collider4 != null && (!this.ignoreTriggers.Value || !collider4.isTrigger))
					{
						this.leftHit.Value = true;
						base.Fsm.Event(this.leftHitEvent);
						return;
					}
				}
			}
		}

		// Token: 0x0400590B RID: 22795
		private const int TERRAIN_LAYER_MASK = 33554688;

		// Token: 0x0400590C RID: 22796
		public FsmOwnerDefault collidingObject;

		// Token: 0x0400590D RID: 22797
		[UIHint(UIHint.Variable)]
		public FsmBool topHit;

		// Token: 0x0400590E RID: 22798
		[UIHint(UIHint.Variable)]
		public FsmBool rightHit;

		// Token: 0x0400590F RID: 22799
		[UIHint(UIHint.Variable)]
		public FsmBool bottomHit;

		// Token: 0x04005910 RID: 22800
		[UIHint(UIHint.Variable)]
		public FsmBool leftHit;

		// Token: 0x04005911 RID: 22801
		public FsmEvent topHitEvent;

		// Token: 0x04005912 RID: 22802
		public FsmEvent rightHitEvent;

		// Token: 0x04005913 RID: 22803
		public FsmEvent bottomHitEvent;

		// Token: 0x04005914 RID: 22804
		public FsmEvent leftHitEvent;

		// Token: 0x04005915 RID: 22805
		public bool otherLayer;

		// Token: 0x04005916 RID: 22806
		public int otherLayerNumber;

		// Token: 0x04005917 RID: 22807
		public FsmBool ignoreTriggers;

		// Token: 0x04005918 RID: 22808
		private Collider2D col2d;

		// Token: 0x04005919 RID: 22809
		private Rigidbody2D body;

		// Token: 0x0400591A RID: 22810
		private int terrainLayerMask;

		// Token: 0x0400591B RID: 22811
		public const float RAYCAST_LENGTH = 0.08f;

		// Token: 0x0400591C RID: 22812
		public const float SMALL_VALUE = 0.001f;

		// Token: 0x0400591D RID: 22813
		private List<Vector2> topRays;

		// Token: 0x0400591E RID: 22814
		private List<Vector2> rightRays;

		// Token: 0x0400591F RID: 22815
		private List<Vector2> bottomRays;

		// Token: 0x04005920 RID: 22816
		private List<Vector2> leftRays;

		// Token: 0x04005921 RID: 22817
		private bool checkUp;

		// Token: 0x04005922 RID: 22818
		private bool checkDown;

		// Token: 0x04005923 RID: 22819
		private bool checkLeft;

		// Token: 0x04005924 RID: 22820
		private bool checkRight;

		// Token: 0x04005925 RID: 22821
		private CustomPlayMakerCollisionStay2D eventProxy;

		// Token: 0x04005926 RID: 22822
		private CustomPlayMakerPhysicsEvent<Collision2D>.EventResponder responder;

		// Token: 0x04005927 RID: 22823
		private bool hasBody;

		// Token: 0x04005928 RID: 22824
		private static ContactPoint2D[] contactPoint2Ds = new ContactPoint2D[1];

		// Token: 0x04005929 RID: 22825
		private int lastUpdateCycle;

		// Token: 0x0400592A RID: 22826
		private int enterCount;

		// Token: 0x02001B7C RID: 7036
		public enum CollisionSide
		{
			// Token: 0x04009D54 RID: 40276
			top,
			// Token: 0x04009D55 RID: 40277
			left,
			// Token: 0x04009D56 RID: 40278
			right,
			// Token: 0x04009D57 RID: 40279
			bottom,
			// Token: 0x04009D58 RID: 40280
			other
		}
	}
}
