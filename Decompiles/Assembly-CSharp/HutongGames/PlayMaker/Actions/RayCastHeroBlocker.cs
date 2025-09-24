using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF1 RID: 3313
	public class RayCastHeroBlocker : FsmStateAction
	{
		// Token: 0x0600625C RID: 25180 RVA: 0x001F1DC8 File Offset: 0x001EFFC8
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
			this.Distance = 100f;
			this.HitEvent = null;
			this.NoHitEvent = null;
			this.StoreDidHit = null;
			this.StoreHitObject = null;
			this.StoreHitPoint = null;
			this.StoreHitNormal = null;
			this.StoreHitDistance = null;
			this.RepeatInterval = 1;
			this.DebugColor = Color.yellow;
			this.Debug = false;
		}

		// Token: 0x0600625D RID: 25181 RVA: 0x001F1E70 File Offset: 0x001F0070
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

		// Token: 0x0600625E RID: 25182 RVA: 0x001F1EBD File Offset: 0x001F00BD
		public override void OnUpdate()
		{
			this.repeat--;
			if (this.repeat == 0)
			{
				this.DoRaycast();
			}
		}

		// Token: 0x0600625F RID: 25183 RVA: 0x001F1EDC File Offset: 0x001F00DC
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
			int num2 = Physics2D.RaycastNonAlloc(value, normalized, this.storeHits, num, 8448);
			if (num2 > 10)
			{
				UnityEngine.Debug.LogWarning("Raycast hit count exceeded allocated buffer", base.Owner);
				num2 = 10;
			}
			RaycastHit2D raycastHit2D = default(RaycastHit2D);
			bool flag = false;
			for (int i = 0; i < num2; i++)
			{
				RaycastHit2D raycastHit2D2 = this.storeHits[i];
				GameObject gameObject = raycastHit2D2.collider.gameObject;
				if ((gameObject.layer != 13 || raycastHit2D2.collider.GetComponent<SlideSurface>()) && !gameObject.CompareTag("Piercable Terrain"))
				{
					raycastHit2D = raycastHit2D2;
					flag = true;
					break;
				}
			}
			this.StoreDidHit.Value = flag;
			if (flag)
			{
				this.StoreHitObject.Value = raycastHit2D.collider.gameObject;
				this.StoreHitPoint.Value = raycastHit2D.point;
				this.StoreHitNormal.Value = raycastHit2D.normal;
				this.StoreHitDistance.Value = raycastHit2D.fraction;
				this.StoreDistance.Value = raycastHit2D.distance;
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

		// Token: 0x040060B0 RID: 24752
		private const int MAX_RAY_HITS = 10;

		// Token: 0x040060B1 RID: 24753
		[ActionSection("Setup")]
		public FsmOwnerDefault FromGameObject;

		// Token: 0x040060B2 RID: 24754
		public FsmVector2 FromPosition;

		// Token: 0x040060B3 RID: 24755
		public FsmVector2 Direction;

		// Token: 0x040060B4 RID: 24756
		public Space Space;

		// Token: 0x040060B5 RID: 24757
		public FsmFloat Distance;

		// Token: 0x040060B6 RID: 24758
		[ActionSection("Result")]
		[UIHint(UIHint.Variable)]
		public FsmEvent HitEvent;

		// Token: 0x040060B7 RID: 24759
		[UIHint(UIHint.Variable)]
		public FsmEvent NoHitEvent;

		// Token: 0x040060B8 RID: 24760
		[UIHint(UIHint.Variable)]
		public FsmBool StoreDidHit;

		// Token: 0x040060B9 RID: 24761
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreHitObject;

		// Token: 0x040060BA RID: 24762
		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreHitPoint;

		// Token: 0x040060BB RID: 24763
		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreHitNormal;

		// Token: 0x040060BC RID: 24764
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreHitDistance;

		// Token: 0x040060BD RID: 24765
		[UIHint(UIHint.Variable)]
		public FsmFloat StoreDistance;

		// Token: 0x040060BE RID: 24766
		[ActionSection("Filter")]
		public FsmInt RepeatInterval;

		// Token: 0x040060BF RID: 24767
		[ActionSection("Debug")]
		public FsmColor DebugColor;

		// Token: 0x040060C0 RID: 24768
		public FsmBool Debug;

		// Token: 0x040060C1 RID: 24769
		private Transform trans;

		// Token: 0x040060C2 RID: 24770
		private int repeat;

		// Token: 0x040060C3 RID: 24771
		private readonly RaycastHit2D[] storeHits = new RaycastHit2D[10];
	}
}
