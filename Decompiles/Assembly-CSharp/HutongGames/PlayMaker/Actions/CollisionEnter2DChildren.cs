using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BFA RID: 3066
	public class CollisionEnter2DChildren : FsmStateAction
	{
		// Token: 0x06005DC4 RID: 24004 RVA: 0x001D92BC File Offset: 0x001D74BC
		public override void Reset()
		{
			this.Target = null;
			this.CollisionType = CollisionEnter2DChildren.CollisionTypes.Enter;
			this.CollideTag = new FsmString
			{
				UseVariable = true
			};
			this.CollideLayer = new FsmInt
			{
				UseVariable = true
			};
			this.StoreNormal = null;
			this.SendEvent = null;
		}

		// Token: 0x06005DC5 RID: 24005 RVA: 0x001D930C File Offset: 0x001D750C
		public override void OnEnter()
		{
			foreach (Collider2D collider2D in this.Target.GetSafe(this).GetComponentsInChildren<Collider2D>())
			{
				CollisionEnterEvent collisionEnterEvent = collider2D.GetComponent<CollisionEnterEvent>();
				if (!collisionEnterEvent)
				{
					collisionEnterEvent = collider2D.gameObject.AddComponent<CollisionEnterEvent>();
				}
				this.collisionTrackers.Add(collisionEnterEvent);
			}
			CollisionEnter2DChildren.CollisionTypes collisionType = this.CollisionType;
			if (collisionType != CollisionEnter2DChildren.CollisionTypes.Enter)
			{
				if (collisionType != CollisionEnter2DChildren.CollisionTypes.Exit)
				{
					goto IL_E2;
				}
			}
			else
			{
				using (HashSet<CollisionEnterEvent>.Enumerator enumerator = this.collisionTrackers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CollisionEnterEvent collisionEnterEvent2 = enumerator.Current;
						collisionEnterEvent2.CollisionEntered += this.TrackerOnCollision;
					}
					return;
				}
			}
			using (HashSet<CollisionEnterEvent>.Enumerator enumerator = this.collisionTrackers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CollisionEnterEvent collisionEnterEvent3 = enumerator.Current;
					collisionEnterEvent3.CollisionExited += this.TrackerOnCollision;
				}
				return;
			}
			IL_E2:
			throw new ArgumentOutOfRangeException();
		}

		// Token: 0x06005DC6 RID: 24006 RVA: 0x001D9420 File Offset: 0x001D7620
		public override void OnExit()
		{
			CollisionEnter2DChildren.CollisionTypes collisionType = this.CollisionType;
			if (collisionType != CollisionEnter2DChildren.CollisionTypes.Enter)
			{
				if (collisionType != CollisionEnter2DChildren.CollisionTypes.Exit)
				{
					goto IL_8E;
				}
			}
			else
			{
				using (HashSet<CollisionEnterEvent>.Enumerator enumerator = this.collisionTrackers.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						CollisionEnterEvent collisionEnterEvent = enumerator.Current;
						collisionEnterEvent.CollisionEntered -= this.TrackerOnCollision;
					}
					goto IL_94;
				}
			}
			using (HashSet<CollisionEnterEvent>.Enumerator enumerator = this.collisionTrackers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CollisionEnterEvent collisionEnterEvent2 = enumerator.Current;
					collisionEnterEvent2.CollisionExited -= this.TrackerOnCollision;
				}
				goto IL_94;
			}
			IL_8E:
			throw new ArgumentOutOfRangeException();
			IL_94:
			this.collisionTrackers.Clear();
		}

		// Token: 0x06005DC7 RID: 24007 RVA: 0x001D94E8 File Offset: 0x001D76E8
		private void TrackerOnCollision(Collision2D collision)
		{
			if (!this.CollideLayer.IsNone && collision.gameObject.layer != this.CollideLayer.Value)
			{
				return;
			}
			if (!this.CollideTag.IsNone && !collision.gameObject.CompareTag(this.CollideTag.Value))
			{
				return;
			}
			this.StoreNormal.Value = collision.GetSafeContact().Normal;
			base.Fsm.Event(this.SendEvent);
		}

		// Token: 0x04005A18 RID: 23064
		public FsmOwnerDefault Target;

		// Token: 0x04005A19 RID: 23065
		public CollisionEnter2DChildren.CollisionTypes CollisionType;

		// Token: 0x04005A1A RID: 23066
		[UIHint(UIHint.Tag)]
		[Tooltip("Filter by Tag.")]
		public FsmString CollideTag;

		// Token: 0x04005A1B RID: 23067
		[UIHint(UIHint.Layer)]
		[Tooltip("Filter by Layer.")]
		public FsmInt CollideLayer;

		// Token: 0x04005A1C RID: 23068
		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreNormal;

		// Token: 0x04005A1D RID: 23069
		[RequiredField]
		[Tooltip("Event to send if a collision is detected.")]
		public FsmEvent SendEvent;

		// Token: 0x04005A1E RID: 23070
		private readonly HashSet<CollisionEnterEvent> collisionTrackers = new HashSet<CollisionEnterEvent>();

		// Token: 0x02001B80 RID: 7040
		public enum CollisionTypes
		{
			// Token: 0x04009D6C RID: 40300
			Enter,
			// Token: 0x04009D6D RID: 40301
			Exit
		}
	}
}
