using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FBE RID: 4030
	[ActionCategory(ActionCategory.Physics2D)]
	public class EdgeSlowdown : ComponentAction<Rigidbody2D>
	{
		// Token: 0x06006F3E RID: 28478 RVA: 0x002260F1 File Offset: 0x002242F1
		public override void Reset()
		{
			this.gameObject = null;
			this.rayRadius = 2f;
			this.rayDepth = 2f;
			this.deceleration = 0.9f;
		}

		// Token: 0x06006F3F RID: 28479 RVA: 0x0022612A File Offset: 0x0022432A
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006F40 RID: 28480 RVA: 0x00226138 File Offset: 0x00224338
		public override void OnFixedUpdate()
		{
			this.DoEdgeSlowdown();
		}

		// Token: 0x06006F41 RID: 28481 RVA: 0x00226140 File Offset: 0x00224340
		private void DoEdgeSlowdown()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Transform transform = ownerDefaultTarget.transform;
			RaycastHit2D raycastHit2D = Helper.Raycast2D(new Vector2(ownerDefaultTarget.transform.position.x - this.rayRadius.Value, ownerDefaultTarget.transform.position.y), Vector2.down, this.rayDepth.Value, EdgeSlowdown.TERRAIN_MASK);
			RaycastHit2D raycastHit2D2 = Helper.Raycast2D(new Vector2(ownerDefaultTarget.transform.position.x + this.rayRadius.Value, ownerDefaultTarget.transform.position.y), Vector2.down, this.rayDepth.Value, EdgeSlowdown.TERRAIN_MASK);
			bool flag = raycastHit2D.collider;
			bool flag2 = raycastHit2D2.collider;
			if (!flag && flag2)
			{
				Rigidbody2D component = ownerDefaultTarget.GetComponent<Rigidbody2D>();
				Vector2 linearVelocity = component.linearVelocity;
				if (linearVelocity.x < 0f)
				{
					linearVelocity.x *= this.deceleration.Value;
					component.linearVelocity = linearVelocity;
					return;
				}
			}
			else if (flag && !flag2)
			{
				Rigidbody2D component2 = ownerDefaultTarget.GetComponent<Rigidbody2D>();
				Vector2 linearVelocity2 = component2.linearVelocity;
				if (linearVelocity2.x > 0f)
				{
					linearVelocity2.x *= this.deceleration.Value;
					component2.linearVelocity = linearVelocity2;
				}
			}
		}

		// Token: 0x04006ED4 RID: 28372
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		[Tooltip("The GameObject with the Rigidbody2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006ED5 RID: 28373
		public FsmFloat rayRadius;

		// Token: 0x04006ED6 RID: 28374
		public FsmFloat rayDepth;

		// Token: 0x04006ED7 RID: 28375
		public FsmFloat deceleration;

		// Token: 0x04006ED8 RID: 28376
		private static readonly int TERRAIN_MASK = 256;
	}
}
