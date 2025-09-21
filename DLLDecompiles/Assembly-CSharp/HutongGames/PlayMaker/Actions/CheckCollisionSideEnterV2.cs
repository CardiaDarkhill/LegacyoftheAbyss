using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE7 RID: 3047
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Detect additional collisions between the Owner of this FSM and other object with additional raycasting.")]
	public sealed class CheckCollisionSideEnterV2 : FsmStateAction
	{
		// Token: 0x06005D60 RID: 23904 RVA: 0x001D6A34 File Offset: 0x001D4C34
		public override void Reset()
		{
			this.topHit = null;
			this.rightHit = null;
			this.bottomHit = null;
			this.leftHit = null;
			this.topHitEvent = null;
			this.rightHitEvent = null;
			this.bottomHitEvent = null;
			this.leftHitEvent = null;
			this.velocity2D = null;
		}

		// Token: 0x06005D61 RID: 23905 RVA: 0x001D6A80 File Offset: 0x001D4C80
		public override void OnPreprocess()
		{
			base.Fsm.HandleCollisionEnter2D = true;
		}

		// Token: 0x06005D62 RID: 23906 RVA: 0x001D6A90 File Offset: 0x001D4C90
		public override void OnEnter()
		{
			this.col2d = base.Fsm.GameObject.GetComponent<Collider2D>();
			this.checkVelocity = !this.velocity2D.IsNone;
			if (this.checkVelocity)
			{
				this.velocityVector = this.velocity2D.Value;
			}
			this._proxy = base.Owner.GetComponent<PlayMakerUnity2DProxy>();
			this.topRays = new List<Vector2>(3);
			this.rightRays = new List<Vector2>(3);
			this.bottomRays = new List<Vector2>(3);
			this.leftRays = new List<Vector2>(3);
			if (this._proxy == null)
			{
				this._proxy = base.Owner.AddComponent<PlayMakerUnity2DProxy>();
			}
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
					this.CheckTouching(8);
					return;
				}
				this.CheckTouching(1 << this.otherLayerNumber);
			}
		}

		// Token: 0x06005D63 RID: 23907 RVA: 0x001D6C29 File Offset: 0x001D4E29
		public override void DoCollisionEnter2D(Collision2D collision)
		{
			if (!this.otherLayer)
			{
				if (collision.gameObject.layer == 8)
				{
					this.CheckTouching(8);
					return;
				}
			}
			else
			{
				this.CheckTouching(this.otherLayerNumber);
			}
		}

		// Token: 0x06005D64 RID: 23908 RVA: 0x001D6C55 File Offset: 0x001D4E55
		private void RecordTrigger()
		{
			this.lastUpdateCycle = CustomPlayerLoop.FixedUpdateCycle;
		}

		// Token: 0x06005D65 RID: 23909 RVA: 0x001D6C64 File Offset: 0x001D4E64
		private void CheckTouching(int layerMask)
		{
			if (this.lastUpdateCycle == CustomPlayerLoop.FixedUpdateCycle)
			{
				return;
			}
			if (this.checkVelocity)
			{
				this.velocityVector = this.velocity2D.Value;
			}
			layerMask = 1 << layerMask;
			Bounds bounds = this.col2d.bounds;
			Vector3 max = bounds.max;
			Vector3 min = bounds.min;
			Vector3 center = bounds.center;
			this.topHit.Value = false;
			this.rightHit.Value = false;
			this.bottomHit.Value = false;
			this.leftHit.Value = false;
			if (this.checkUp && (!this.checkVelocity || this.velocityVector.y >= -0.001f))
			{
				this.topRays.Clear();
				this.topRays.Add(new Vector2(min.x, max.y));
				this.topRays.Add(new Vector2(center.x, max.y));
				this.topRays.Add(max);
				for (int i = 0; i < 3; i++)
				{
					Collider2D collider = Helper.Raycast2D(this.topRays[i], Vector2.up, 0.08f, layerMask).collider;
					if (!(collider == null) && (!this.ignoreTriggers.Value || !collider.isTrigger))
					{
						this.RecordTrigger();
						this.topHit.Value = true;
						base.Fsm.Event(this.topHitEvent);
						break;
					}
				}
			}
			if (this.checkRight && (!this.checkVelocity || this.velocityVector.x >= -0.001f))
			{
				this.rightRays.Clear();
				this.rightRays.Add(max);
				this.rightRays.Add(new Vector2(max.x, center.y));
				this.rightRays.Add(new Vector2(max.x, min.y));
				for (int j = 0; j < 3; j++)
				{
					Collider2D collider2 = Helper.Raycast2D(this.rightRays[j], Vector2.right, 0.08f, layerMask).collider;
					if (collider2 != null && (!this.ignoreTriggers.Value || !collider2.isTrigger))
					{
						this.RecordTrigger();
						this.rightHit.Value = true;
						base.Fsm.Event(this.rightHitEvent);
						break;
					}
				}
			}
			if (this.checkDown && (!this.checkVelocity || this.velocityVector.y <= 0.001f))
			{
				this.bottomRays.Clear();
				this.bottomRays.Add(new Vector2(max.x, min.y));
				this.bottomRays.Add(new Vector2(center.x, min.y));
				this.bottomRays.Add(min);
				for (int k = 0; k < 3; k++)
				{
					Collider2D collider3 = Helper.Raycast2D(this.bottomRays[k], -Vector2.up, 0.08f, layerMask).collider;
					if (collider3 != null && (!this.ignoreTriggers.Value || !collider3.isTrigger))
					{
						this.RecordTrigger();
						this.bottomHit.Value = true;
						base.Fsm.Event(this.bottomHitEvent);
						break;
					}
				}
			}
			if (this.checkLeft && (!this.checkVelocity || this.velocityVector.x <= 0.001f))
			{
				this.leftRays.Clear();
				this.leftRays.Add(min);
				this.leftRays.Add(new Vector2(min.x, center.y));
				this.leftRays.Add(new Vector2(min.x, max.y));
				for (int l = 0; l < 3; l++)
				{
					Collider2D collider4 = Helper.Raycast2D(this.leftRays[l], -Vector2.right, 0.08f, layerMask).collider;
					if (collider4 != null && (!this.ignoreTriggers.Value || !collider4.isTrigger))
					{
						this.RecordTrigger();
						this.leftHit.Value = true;
						base.Fsm.Event(this.leftHitEvent);
						return;
					}
				}
			}
		}

		// Token: 0x04005968 RID: 22888
		[UIHint(UIHint.Variable)]
		public FsmBool topHit;

		// Token: 0x04005969 RID: 22889
		[UIHint(UIHint.Variable)]
		public FsmBool rightHit;

		// Token: 0x0400596A RID: 22890
		[UIHint(UIHint.Variable)]
		public FsmBool bottomHit;

		// Token: 0x0400596B RID: 22891
		[UIHint(UIHint.Variable)]
		public FsmBool leftHit;

		// Token: 0x0400596C RID: 22892
		public FsmEvent topHitEvent;

		// Token: 0x0400596D RID: 22893
		public FsmEvent rightHitEvent;

		// Token: 0x0400596E RID: 22894
		public FsmEvent bottomHitEvent;

		// Token: 0x0400596F RID: 22895
		public FsmEvent leftHitEvent;

		// Token: 0x04005970 RID: 22896
		public bool otherLayer;

		// Token: 0x04005971 RID: 22897
		public int otherLayerNumber;

		// Token: 0x04005972 RID: 22898
		public FsmBool ignoreTriggers;

		// Token: 0x04005973 RID: 22899
		[UIHint(UIHint.Variable)]
		public FsmVector2 velocity2D;

		// Token: 0x04005974 RID: 22900
		private PlayMakerUnity2DProxy _proxy;

		// Token: 0x04005975 RID: 22901
		private Collider2D col2d;

		// Token: 0x04005976 RID: 22902
		public const float RAYCAST_LENGTH = 0.08f;

		// Token: 0x04005977 RID: 22903
		public const float SMALL_VALUE = 0.001f;

		// Token: 0x04005978 RID: 22904
		private List<Vector2> topRays;

		// Token: 0x04005979 RID: 22905
		private List<Vector2> rightRays;

		// Token: 0x0400597A RID: 22906
		private List<Vector2> bottomRays;

		// Token: 0x0400597B RID: 22907
		private List<Vector2> leftRays;

		// Token: 0x0400597C RID: 22908
		private bool checkUp;

		// Token: 0x0400597D RID: 22909
		private bool checkDown;

		// Token: 0x0400597E RID: 22910
		private bool checkLeft;

		// Token: 0x0400597F RID: 22911
		private bool checkRight;

		// Token: 0x04005980 RID: 22912
		private int lastUpdateCycle;

		// Token: 0x04005981 RID: 22913
		private int enterCount;

		// Token: 0x04005982 RID: 22914
		private bool checkVelocity;

		// Token: 0x04005983 RID: 22915
		private Vector2 velocityVector;

		// Token: 0x02001B7F RID: 7039
		public enum CollisionSide
		{
			// Token: 0x04009D66 RID: 40294
			top,
			// Token: 0x04009D67 RID: 40295
			left,
			// Token: 0x04009D68 RID: 40296
			right,
			// Token: 0x04009D69 RID: 40297
			bottom,
			// Token: 0x04009D6A RID: 40298
			other
		}
	}
}
