using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200125C RID: 4700
	public class HarpoonDashRayCheck : FsmStateAction
	{
		// Token: 0x06007C12 RID: 31762 RVA: 0x00251540 File Offset: 0x0024F740
		public override void Reset()
		{
			this.Hero = null;
			this.Direction = null;
			this.StoreHitObject = null;
			this.StoreHitPoint = null;
			this.NoHitEvent = null;
			this.TerrainEvent = null;
			this.EnemyEvent = null;
			this.RingEvent = null;
			this.BouncePodEvent = null;
			this.TinkEvent = null;
		}

		// Token: 0x06007C13 RID: 31763 RVA: 0x00251594 File Offset: 0x0024F794
		public override void OnEnter()
		{
			this.StoreHitObject.Value = null;
			this.StoreHitPoint.Value = Vector2.zero;
			GameObject safe = this.Hero.GetSafe(this);
			if (!safe)
			{
				base.Finish();
				return;
			}
			Vector2 a = safe.transform.position;
			this.hitChecks[0] = this.CheckRay(a + new Vector2(0f, 0.425f), false);
			this.hitChecks[1] = this.CheckRay(a + new Vector2(0f, -1.05f), false);
			this.hitChecks[2] = this.CheckRay(a + new Vector2(0f, -0.31249997f), false);
			this.hitChecks[3] = this.CheckRay(a + new Vector2(0f, 0.05625002f), true);
			this.hitChecks[4] = this.CheckRay(a + new Vector2(0f, -0.68125f), true);
			this.hitChecks[5] = this.CheckRay(a + new Vector2(0f, -0.31249997f), true);
			HarpoonDashRayCheck.HitTypes hitTypes = HarpoonDashRayCheck.HitTypes.None;
			RaycastHit2D hit = default(RaycastHit2D);
			foreach (HarpoonDashRayCheck.HitCheck hitCheck in this.hitChecks)
			{
				if (hitCheck.HitType >= hitTypes)
				{
					hitTypes = hitCheck.HitType;
					hit = hitCheck.Hit;
				}
			}
			float num = float.PositiveInfinity;
			RaycastHit2D raycastHit2D = default(RaycastHit2D);
			foreach (HarpoonDashRayCheck.HitCheck hitCheck2 in this.hitChecks)
			{
				if (hitCheck2.HitType == HarpoonDashRayCheck.HitTypes.Terrain)
				{
					RaycastHit2D hit2 = hitCheck2.Hit;
					if (hit2.distance <= num)
					{
						hit2 = hitCheck2.Hit;
						num = hit2.distance;
						raycastHit2D = hitCheck2.Hit;
					}
				}
			}
			if (num < hit.distance)
			{
				hitTypes = HarpoonDashRayCheck.HitTypes.Terrain;
				hit = raycastHit2D;
			}
			switch (hitTypes)
			{
			case HarpoonDashRayCheck.HitTypes.None:
				base.Fsm.Event(this.NoHitEvent);
				base.Finish();
				return;
			case HarpoonDashRayCheck.HitTypes.Terrain:
				this.RecordSuccess(hit);
				base.Fsm.Event(this.TerrainEvent);
				base.Finish();
				return;
			case HarpoonDashRayCheck.HitTypes.Tinker:
				this.RecordSuccess(hit);
				base.Fsm.Event(this.TinkEvent);
				base.Finish();
				return;
			case HarpoonDashRayCheck.HitTypes.HarpoonRing:
				this.RecordSuccess(hit);
				base.Fsm.Event(this.RingEvent);
				base.Finish();
				return;
			case HarpoonDashRayCheck.HitTypes.BouncePod:
				this.RecordSuccess(hit);
				base.Fsm.Event(this.BouncePodEvent);
				base.Finish();
				return;
			case HarpoonDashRayCheck.HitTypes.Enemy:
				this.RecordSuccess(hit);
				base.Fsm.Event(this.EnemyEvent);
				base.Finish();
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06007C14 RID: 31764 RVA: 0x00251880 File Offset: 0x0024FA80
		private HarpoonDashRayCheck.HitCheck CheckRay(Vector2 origin, bool isTerrainCheck)
		{
			Vector2 direction = new Vector2(Mathf.Sign(this.Direction.Value), 0f);
			float distance = 10.5f;
			int a = Physics2D.Raycast(origin, direction, new ContactFilter2D
			{
				useLayerMask = true,
				layerMask = (isTerrainCheck ? 256 : 657408),
				useTriggers = true
			}, this.results, distance);
			HarpoonDashRayCheck.HitCheck result;
			for (int i = 0; i < Mathf.Min(a, this.results.Length); i++)
			{
				RaycastHit2D hit = this.results[i];
				Collider2D collider = hit.collider;
				if (collider.gameObject.layer == 11)
				{
					HealthManager healthManager;
					if (!HitTaker.TryGetHealthManager(collider.gameObject, out healthManager) || !healthManager.IsInvincible || !healthManager.PreventInvincibleEffect || healthManager.InvincibleFromDirection == 2 || healthManager.InvincibleFromDirection == 4 || healthManager.InvincibleFromDirection == 7)
					{
						result = new HarpoonDashRayCheck.HitCheck
						{
							Hit = hit,
							HitType = HarpoonDashRayCheck.HitTypes.Enemy
						};
						return result;
					}
				}
				else
				{
					if (collider.CompareTag("Bounce Pod") || collider.GetComponent<BouncePod>() || collider.GetComponent<HarpoonHook>())
					{
						result = new HarpoonDashRayCheck.HitCheck
						{
							Hit = hit,
							HitType = HarpoonDashRayCheck.HitTypes.BouncePod
						};
						return result;
					}
					if (collider.CompareTag("Harpoon Ring"))
					{
						result = new HarpoonDashRayCheck.HitCheck
						{
							Hit = hit,
							HitType = HarpoonDashRayCheck.HitTypes.HarpoonRing
						};
						return result;
					}
					if (collider.gameObject.layer == 17 && collider.GetComponent<TinkEffect>() && !collider.GetComponent<TinkEffect>().noHarpoonHook)
					{
						result = new HarpoonDashRayCheck.HitCheck
						{
							Hit = hit,
							HitType = HarpoonDashRayCheck.HitTypes.Tinker
						};
						return result;
					}
					if (collider.gameObject.layer == 8)
					{
						result = new HarpoonDashRayCheck.HitCheck
						{
							Hit = hit,
							HitType = HarpoonDashRayCheck.HitTypes.Terrain
						};
						return result;
					}
				}
			}
			result = new HarpoonDashRayCheck.HitCheck
			{
				Hit = default(RaycastHit2D),
				HitType = HarpoonDashRayCheck.HitTypes.None
			};
			return result;
		}

		// Token: 0x06007C15 RID: 31765 RVA: 0x00251A9E File Offset: 0x0024FC9E
		private void RecordSuccess(RaycastHit2D hit)
		{
			this.StoreHitObject.Value = hit.collider.gameObject;
			this.StoreHitPoint.Value = hit.point;
		}

		// Token: 0x04007C2A RID: 31786
		public FsmOwnerDefault Hero;

		// Token: 0x04007C2B RID: 31787
		public FsmFloat Direction;

		// Token: 0x04007C2C RID: 31788
		[UIHint(UIHint.Variable)]
		public FsmGameObject StoreHitObject;

		// Token: 0x04007C2D RID: 31789
		[UIHint(UIHint.Variable)]
		public FsmVector2 StoreHitPoint;

		// Token: 0x04007C2E RID: 31790
		public FsmEvent NoHitEvent;

		// Token: 0x04007C2F RID: 31791
		public FsmEvent TerrainEvent;

		// Token: 0x04007C30 RID: 31792
		public FsmEvent EnemyEvent;

		// Token: 0x04007C31 RID: 31793
		public FsmEvent RingEvent;

		// Token: 0x04007C32 RID: 31794
		public FsmEvent BouncePodEvent;

		// Token: 0x04007C33 RID: 31795
		public FsmEvent TinkEvent;

		// Token: 0x04007C34 RID: 31796
		private readonly RaycastHit2D[] results = new RaycastHit2D[10];

		// Token: 0x04007C35 RID: 31797
		private readonly HarpoonDashRayCheck.HitCheck[] hitChecks = new HarpoonDashRayCheck.HitCheck[6];

		// Token: 0x02001BDE RID: 7134
		private enum HitTypes
		{
			// Token: 0x04009F44 RID: 40772
			None,
			// Token: 0x04009F45 RID: 40773
			Terrain,
			// Token: 0x04009F46 RID: 40774
			Tinker,
			// Token: 0x04009F47 RID: 40775
			HarpoonRing,
			// Token: 0x04009F48 RID: 40776
			BouncePod,
			// Token: 0x04009F49 RID: 40777
			Enemy
		}

		// Token: 0x02001BDF RID: 7135
		private struct HitCheck
		{
			// Token: 0x04009F4A RID: 40778
			public HarpoonDashRayCheck.HitTypes HitType;

			// Token: 0x04009F4B RID: 40779
			public RaycastHit2D Hit;
		}
	}
}
