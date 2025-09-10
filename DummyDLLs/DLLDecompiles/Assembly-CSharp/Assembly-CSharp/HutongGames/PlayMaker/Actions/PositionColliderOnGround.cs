using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CDE RID: 3294
	public class PositionColliderOnGround : FsmStateAction
	{
		// Token: 0x06006208 RID: 25096 RVA: 0x001EFCE1 File Offset: 0x001EDEE1
		public override void Reset()
		{
			this.Target = null;
		}

		// Token: 0x06006209 RID: 25097 RVA: 0x001EFCEC File Offset: 0x001EDEEC
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				Collider2D component = safe.GetComponent<Collider2D>();
				if (component)
				{
					Bounds bounds = component.bounds;
					Vector3 center = bounds.center;
					Vector2 vector = new Vector2(center.x, bounds.min.y);
					RaycastHit2D raycastHit2D;
					if (Helper.IsRayHittingNoTriggers(center, Vector2.down, 10f, 256, out raycastHit2D))
					{
						float num = safe.transform.position.y - vector.y;
						Vector2 point = raycastHit2D.point;
						point.y += num;
						safe.transform.SetPosition2D(point);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x0400601A RID: 24602
		[CheckForComponent(typeof(Collider2D))]
		public FsmOwnerDefault Target;
	}
}
