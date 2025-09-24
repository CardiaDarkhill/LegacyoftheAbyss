using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE5 RID: 3045
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Detect additional collisions between objects with additional raycasting.")]
	public class CheckCollisionSideV2 : FsmStateAction
	{
		// Token: 0x06005D4D RID: 23885 RVA: 0x001D5A60 File Offset: 0x001D3C60
		public override void Reset()
		{
			this.checkUp = false;
			this.checkDown = false;
			this.checkLeft = false;
			this.checkRight = false;
		}

		// Token: 0x06005D4E RID: 23886 RVA: 0x001D5A7E File Offset: 0x001D3C7E
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
			base.Fsm.HandleCollisionExit2D = true;
		}

		// Token: 0x06005D4F RID: 23887 RVA: 0x001D5A98 File Offset: 0x001D3C98
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
			base.Fsm.HandleCollisionExit2D = true;
		}

		// Token: 0x06005D50 RID: 23888 RVA: 0x001D5AB4 File Offset: 0x001D3CB4
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

		// Token: 0x06005D51 RID: 23889 RVA: 0x001D5B20 File Offset: 0x001D3D20
		public override void OnEnter()
		{
			this.isActive = true;
			this.eventProxy = CustomPlayMakerCollisionStay2D.GetEventSender(base.Fsm.Owner.gameObject);
			this.responder = this.eventProxy.Add(this, new Action<Collision2D>(this.DoCollisionStay2D));
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.collidingObject);
			this.terrainLayerMask = ((this.ignoreTriggers.Value && ownerDefaultTarget.layer == 9) ? 33562880 : 33554688);
			this.col2d = ownerDefaultTarget.GetComponent<Collider2D>();
			this.body = ownerDefaultTarget.GetComponent<Rigidbody2D>();
			this.hasBody = (this.body != null);
			this.lastPosition = this.col2d.transform.position;
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

		// Token: 0x06005D52 RID: 23890 RVA: 0x001D5D07 File Offset: 0x001D3F07
		public override void OnExit()
		{
			this.isActive = false;
			this.RemoveEventProxy();
		}

		// Token: 0x06005D53 RID: 23891 RVA: 0x001D5D18 File Offset: 0x001D3F18
		public override void OnFixedUpdate()
		{
			if (!this.isActive)
			{
				return;
			}
			if (this.lastUpdateCycle == CustomPlayerLoop.FixedUpdateCycle)
			{
				return;
			}
			bool flag = false;
			if (this.hasBody && CustomPlayerLoop.FixedUpdateCycle - this.lastUpdateCycle > 1)
			{
				flag = (this.body.GetContacts(CheckCollisionSideV2.contactPoint2Ds) > 0);
			}
			if (flag || this.topHit.Value || this.bottomHit.Value || this.rightHit.Value || this.leftHit.Value)
			{
				if (!this.otherLayer)
				{
					this.CheckTouching(this.terrainLayerMask);
					return;
				}
				this.CheckTouching(1 << this.otherLayerNumber);
			}
		}

		// Token: 0x06005D54 RID: 23892 RVA: 0x001D5DC8 File Offset: 0x001D3FC8
		public override void DoCollisionStay2D(Collision2D collision)
		{
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

		// Token: 0x06005D55 RID: 23893 RVA: 0x001D5E14 File Offset: 0x001D4014
		public override void DoCollisionExit2D(Collision2D collision)
		{
			this.topHit.Value = false;
			this.rightHit.Value = false;
			this.bottomHit.Value = false;
			this.leftHit.Value = false;
		}

		// Token: 0x06005D56 RID: 23894 RVA: 0x001D5E48 File Offset: 0x001D4048
		private void CheckTouching(int layerMask)
		{
			if (this.lastUpdateCycle == CustomPlayerLoop.FixedUpdateCycle)
			{
				return;
			}
			if (this.lastUpdateCycle == CustomPlayerLoop.FixedUpdateCycle)
			{
				int num = this.enterCount;
				this.enterCount = num + 1;
				if (num > 1)
				{
					return;
				}
			}
			else
			{
				this.enterCount = 0;
				this.lastUpdateCycle = CustomPlayerLoop.FixedUpdateCycle;
			}
			Bounds bounds = this.col2d.bounds;
			Vector3 max = bounds.max;
			Vector3 min = bounds.min;
			Vector3 center = bounds.center;
			Vector3 position = this.col2d.transform.position;
			Vector3 vector = position - this.lastPosition;
			this.lastPosition = position;
			bool value = this.ignoreBodyVelocity.Value;
			if (this.checkUp && (value || !this.hasBody || this.body.linearVelocity.y >= -0.001f || vector.y >= 0f))
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
						this.lastTriggerCycle = CustomPlayerLoop.FixedUpdateCycle;
						this.topHit.Value = true;
						base.Fsm.Event(this.topHitEvent);
						break;
					}
				}
			}
			if (this.checkRight && (value || !this.hasBody || this.body.linearVelocity.x >= -0.001f || vector.x >= 0f))
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
						this.lastTriggerCycle = CustomPlayerLoop.FixedUpdateCycle;
						this.rightHit.Value = true;
						base.Fsm.Event(this.rightHitEvent);
						break;
					}
				}
			}
			if (this.checkDown && (value || !this.hasBody || this.body.linearVelocity.y <= 0.001f || vector.y <= 0f))
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
						this.lastTriggerCycle = CustomPlayerLoop.FixedUpdateCycle;
						this.bottomHit.Value = true;
						base.Fsm.Event(this.bottomHitEvent);
						break;
					}
				}
			}
			if (this.checkLeft && (value || !this.hasBody || this.body.linearVelocity.x <= 0.001f || vector.x <= 0f))
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
						this.lastTriggerCycle = CustomPlayerLoop.FixedUpdateCycle;
						this.leftHit.Value = true;
						base.Fsm.Event(this.leftHitEvent);
						return;
					}
				}
			}
		}

		// Token: 0x0400592B RID: 22827
		private const int TERRAIN_LAYER_MASK = 33554688;

		// Token: 0x0400592C RID: 22828
		public FsmOwnerDefault collidingObject;

		// Token: 0x0400592D RID: 22829
		[UIHint(UIHint.Variable)]
		public FsmBool topHit;

		// Token: 0x0400592E RID: 22830
		[UIHint(UIHint.Variable)]
		public FsmBool rightHit;

		// Token: 0x0400592F RID: 22831
		[UIHint(UIHint.Variable)]
		public FsmBool bottomHit;

		// Token: 0x04005930 RID: 22832
		[UIHint(UIHint.Variable)]
		public FsmBool leftHit;

		// Token: 0x04005931 RID: 22833
		public FsmEvent topHitEvent;

		// Token: 0x04005932 RID: 22834
		public FsmEvent rightHitEvent;

		// Token: 0x04005933 RID: 22835
		public FsmEvent bottomHitEvent;

		// Token: 0x04005934 RID: 22836
		public FsmEvent leftHitEvent;

		// Token: 0x04005935 RID: 22837
		public bool otherLayer;

		// Token: 0x04005936 RID: 22838
		public int otherLayerNumber;

		// Token: 0x04005937 RID: 22839
		public FsmBool ignoreTriggers;

		// Token: 0x04005938 RID: 22840
		[Space]
		public FsmBool ignoreBodyVelocity;

		// Token: 0x04005939 RID: 22841
		private Collider2D col2d;

		// Token: 0x0400593A RID: 22842
		private Rigidbody2D body;

		// Token: 0x0400593B RID: 22843
		private int terrainLayerMask;

		// Token: 0x0400593C RID: 22844
		private List<Vector2> topRays;

		// Token: 0x0400593D RID: 22845
		private List<Vector2> rightRays;

		// Token: 0x0400593E RID: 22846
		private List<Vector2> bottomRays;

		// Token: 0x0400593F RID: 22847
		private List<Vector2> leftRays;

		// Token: 0x04005940 RID: 22848
		private bool checkUp;

		// Token: 0x04005941 RID: 22849
		private bool checkDown;

		// Token: 0x04005942 RID: 22850
		private bool checkLeft;

		// Token: 0x04005943 RID: 22851
		private bool checkRight;

		// Token: 0x04005944 RID: 22852
		private static ContactPoint2D[] contactPoint2Ds = new ContactPoint2D[1];

		// Token: 0x04005945 RID: 22853
		private CustomPlayMakerCollisionStay2D eventProxy;

		// Token: 0x04005946 RID: 22854
		private CustomPlayMakerPhysicsEvent<Collision2D>.EventResponder responder;

		// Token: 0x04005947 RID: 22855
		private bool hasBody;

		// Token: 0x04005948 RID: 22856
		private Vector3 lastPosition;

		// Token: 0x04005949 RID: 22857
		private bool isActive;

		// Token: 0x0400594A RID: 22858
		private int lastUpdateCycle;

		// Token: 0x0400594B RID: 22859
		private int lastTriggerCycle;

		// Token: 0x0400594C RID: 22860
		private int enterCount;

		// Token: 0x02001B7D RID: 7037
		public enum CollisionSide
		{
			// Token: 0x04009D5A RID: 40282
			top,
			// Token: 0x04009D5B RID: 40283
			left,
			// Token: 0x04009D5C RID: 40284
			right,
			// Token: 0x04009D5D RID: 40285
			bottom,
			// Token: 0x04009D5E RID: 40286
			other
		}
	}
}
