using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C24 RID: 3108
	[ActionCategory(ActionCategory.Transform)]
	public class DockClamberCheck : FsmStateAction
	{
		// Token: 0x06005EA2 RID: 24226 RVA: 0x001DEE65 File Offset: 0x001DD065
		public override void Reset()
		{
			this.hero = null;
			this.clamberRayL = null;
			this.clamberRayR = null;
			this.clamberEvent = null;
		}

		// Token: 0x06005EA3 RID: 24227 RVA: 0x001DEE84 File Offset: 0x001DD084
		public override void OnEnter()
		{
			this.selfTransform = base.Owner.transform;
			this.heroTransform = this.hero.Value.transform;
			this.rayLTransform = this.clamberRayL.Value.transform;
			this.rayRTransform = this.clamberRayR.Value.transform;
			this.hc = this.hero.Value.GetComponent<HeroController>();
		}

		// Token: 0x06005EA4 RID: 24228 RVA: 0x001DEEFC File Offset: 0x001DD0FC
		public override void OnUpdate()
		{
			if (!this.noClamber.Value)
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				if (this.heroTransform.position.y > this.selfTransform.position.y)
				{
					flag = true;
				}
				if (flag)
				{
					flag2 = this.hc.GetState("onGround");
				}
				if (flag2)
				{
					Vector2 origin = this.rayLTransform.position;
					Vector2 origin2 = this.rayRTransform.position;
					int mask = LayerMask.GetMask(new string[]
					{
						"Terrain"
					});
					RaycastHit2D raycastHit2D = Helper.Raycast2D(origin, Vector2.up, 10f, mask);
					RaycastHit2D raycastHit2D2 = Helper.Raycast2D(origin2, Vector2.up, 10f, mask);
					bool flag4 = raycastHit2D.collider != null;
					bool flag5 = raycastHit2D2.collider != null;
					if (flag4 && flag5 && raycastHit2D.point.y == raycastHit2D2.point.y && raycastHit2D.collider.gameObject.tag == "Platform" && raycastHit2D2.collider.gameObject.tag == "Platform")
					{
						flag3 = true;
					}
				}
				if (flag3)
				{
					FSMUtility.SendEventToGameObject(base.Owner, this.clamberEvent, false);
				}
			}
		}

		// Token: 0x04005B29 RID: 23337
		[UIHint(UIHint.Variable)]
		public FsmGameObject hero;

		// Token: 0x04005B2A RID: 23338
		[UIHint(UIHint.Variable)]
		public FsmGameObject clamberRayL;

		// Token: 0x04005B2B RID: 23339
		[UIHint(UIHint.Variable)]
		public FsmGameObject clamberRayR;

		// Token: 0x04005B2C RID: 23340
		public FsmBool noClamber;

		// Token: 0x04005B2D RID: 23341
		public FsmEvent clamberEvent;

		// Token: 0x04005B2E RID: 23342
		private const float jumpHeightMax = 10f;

		// Token: 0x04005B2F RID: 23343
		private const float platThickMax = 10f;

		// Token: 0x04005B30 RID: 23344
		private const float clamberClearance = 4.5f;

		// Token: 0x04005B31 RID: 23345
		private Transform selfTransform;

		// Token: 0x04005B32 RID: 23346
		private Transform heroTransform;

		// Token: 0x04005B33 RID: 23347
		private Transform rayLTransform;

		// Token: 0x04005B34 RID: 23348
		private Transform rayRTransform;

		// Token: 0x04005B35 RID: 23349
		private HeroController hc;
	}
}
