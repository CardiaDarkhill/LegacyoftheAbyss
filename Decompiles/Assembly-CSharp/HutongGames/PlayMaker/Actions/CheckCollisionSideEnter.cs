using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE6 RID: 3046
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Detect additional collisions between the Owner of this FSM and other object with additional raycasting.")]
	public class CheckCollisionSideEnter : FsmStateAction
	{
		// Token: 0x06005D59 RID: 23897 RVA: 0x001D639C File Offset: 0x001D459C
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
		}

		// Token: 0x06005D5A RID: 23898 RVA: 0x001D63D6 File Offset: 0x001D45D6
		public override void OnPreprocess()
		{
			base.Fsm.HandleCollisionEnter2D = true;
		}

		// Token: 0x06005D5B RID: 23899 RVA: 0x001D63E4 File Offset: 0x001D45E4
		public override void OnEnter()
		{
			this.col2d = base.Fsm.GameObject.GetComponent<Collider2D>();
			this.body = base.Fsm.GameObject.GetComponent<Rigidbody2D>();
			this.hasBody = (this.body != null);
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

		// Token: 0x06005D5C RID: 23900 RVA: 0x001D6578 File Offset: 0x001D4778
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

		// Token: 0x06005D5D RID: 23901 RVA: 0x001D65A4 File Offset: 0x001D47A4
		private void RecordTrigger()
		{
			this.lastUpdateCycle = CustomPlayerLoop.FixedUpdateCycle;
		}

		// Token: 0x06005D5E RID: 23902 RVA: 0x001D65B4 File Offset: 0x001D47B4
		private void CheckTouching(int layerMask)
		{
			if (this.lastUpdateCycle == CustomPlayerLoop.FixedUpdateCycle)
			{
				return;
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
			if (this.checkUp && (!this.hasBody || this.body.linearVelocity.y >= -0.001f))
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
			if (this.checkRight && (!this.hasBody || this.body.linearVelocity.x >= -0.001f))
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
			if (this.checkDown && (!this.hasBody || this.body.linearVelocity.y <= 0.001f))
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
			if (this.checkLeft && (!this.hasBody || this.body.linearVelocity.x <= 0.001f))
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

		// Token: 0x0400594D RID: 22861
		[UIHint(UIHint.Variable)]
		public FsmBool topHit;

		// Token: 0x0400594E RID: 22862
		[UIHint(UIHint.Variable)]
		public FsmBool rightHit;

		// Token: 0x0400594F RID: 22863
		[UIHint(UIHint.Variable)]
		public FsmBool bottomHit;

		// Token: 0x04005950 RID: 22864
		[UIHint(UIHint.Variable)]
		public FsmBool leftHit;

		// Token: 0x04005951 RID: 22865
		public FsmEvent topHitEvent;

		// Token: 0x04005952 RID: 22866
		public FsmEvent rightHitEvent;

		// Token: 0x04005953 RID: 22867
		public FsmEvent bottomHitEvent;

		// Token: 0x04005954 RID: 22868
		public FsmEvent leftHitEvent;

		// Token: 0x04005955 RID: 22869
		public bool otherLayer;

		// Token: 0x04005956 RID: 22870
		public int otherLayerNumber;

		// Token: 0x04005957 RID: 22871
		public FsmBool ignoreTriggers;

		// Token: 0x04005958 RID: 22872
		private PlayMakerUnity2DProxy _proxy;

		// Token: 0x04005959 RID: 22873
		private Collider2D col2d;

		// Token: 0x0400595A RID: 22874
		public const float RAYCAST_LENGTH = 0.08f;

		// Token: 0x0400595B RID: 22875
		public const float SMALL_VALUE = 0.001f;

		// Token: 0x0400595C RID: 22876
		private List<Vector2> topRays;

		// Token: 0x0400595D RID: 22877
		private List<Vector2> rightRays;

		// Token: 0x0400595E RID: 22878
		private List<Vector2> bottomRays;

		// Token: 0x0400595F RID: 22879
		private List<Vector2> leftRays;

		// Token: 0x04005960 RID: 22880
		private bool checkUp;

		// Token: 0x04005961 RID: 22881
		private bool checkDown;

		// Token: 0x04005962 RID: 22882
		private bool checkLeft;

		// Token: 0x04005963 RID: 22883
		private bool checkRight;

		// Token: 0x04005964 RID: 22884
		private int lastUpdateCycle;

		// Token: 0x04005965 RID: 22885
		private int enterCount;

		// Token: 0x04005966 RID: 22886
		private Rigidbody2D body;

		// Token: 0x04005967 RID: 22887
		private bool hasBody;

		// Token: 0x02001B7E RID: 7038
		public enum CollisionSide
		{
			// Token: 0x04009D60 RID: 40288
			top,
			// Token: 0x04009D61 RID: 40289
			left,
			// Token: 0x04009D62 RID: 40290
			right,
			// Token: 0x04009D63 RID: 40291
			bottom,
			// Token: 0x04009D64 RID: 40292
			other
		}
	}
}
