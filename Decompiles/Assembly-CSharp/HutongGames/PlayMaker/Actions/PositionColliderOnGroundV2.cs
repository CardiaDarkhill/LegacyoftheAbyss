using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CDF RID: 3295
	public class PositionColliderOnGroundV2 : FsmStateAction
	{
		// Token: 0x0600620B RID: 25099 RVA: 0x001EFDB4 File Offset: 0x001EDFB4
		public override void Reset()
		{
			this.Target = null;
			this.GroundPos = new FsmVector2
			{
				UseVariable = true
			};
		}

		// Token: 0x0600620C RID: 25100 RVA: 0x001EFDD0 File Offset: 0x001EDFD0
		public override void OnEnter()
		{
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				Collider2D component = safe.GetComponent<Collider2D>();
				if (component)
				{
					Bounds bounds = component.bounds;
					Vector2 vector = bounds.center;
					Vector2 vector2 = new Vector2(vector.x, bounds.min.y);
					RaycastHit2D raycastHit2D;
					if (Helper.IsRayHittingNoTriggers((!this.GroundPos.IsNone) ? this.GroundPos.Value : vector, Vector2.down, 10f, 256, out raycastHit2D))
					{
						float num = safe.transform.position.y - vector2.y;
						Vector2 point = raycastHit2D.point;
						point.y += num;
						safe.transform.SetPosition2D(point);
					}
				}
			}
			base.Finish();
		}

		// Token: 0x0400601B RID: 24603
		[CheckForComponent(typeof(Collider2D))]
		public FsmOwnerDefault Target;

		// Token: 0x0400601C RID: 24604
		public FsmVector2 GroundPos;
	}
}
