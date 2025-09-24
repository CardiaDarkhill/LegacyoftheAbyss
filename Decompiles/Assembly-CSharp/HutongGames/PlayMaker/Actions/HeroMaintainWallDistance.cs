using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001266 RID: 4710
	public sealed class HeroMaintainWallDistance : FsmStateAction
	{
		// Token: 0x06007C47 RID: 31815 RVA: 0x0025295C File Offset: 0x00250B5C
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06007C48 RID: 31816 RVA: 0x0025296A File Offset: 0x00250B6A
		public override void Reset()
		{
			this.lowPointX = null;
			this.highPointX = null;
			this.direction = 1f;
			this.distance = 0.005f;
			this.everyFixedUpdate = false;
		}

		// Token: 0x06007C49 RID: 31817 RVA: 0x002529A8 File Offset: 0x00250BA8
		public override void OnEnter()
		{
			this.hc = HeroController.instance;
			if (this.hc != null)
			{
				this.MaintainWallDistance();
			}
			if (!this.everyFixedUpdate.Value || this.hc == null)
			{
				base.Finish();
			}
		}

		// Token: 0x06007C4A RID: 31818 RVA: 0x002529F5 File Offset: 0x00250BF5
		public override void OnFixedUpdate()
		{
			if (this.hc != null)
			{
				this.MaintainWallDistance();
				return;
			}
			base.Finish();
		}

		// Token: 0x06007C4B RID: 31819 RVA: 0x00252A14 File Offset: 0x00250C14
		private void MaintainWallDistance()
		{
			float num = Mathf.Sign(this.direction.Value);
			float num2 = this.lowPointX.Value;
			float num3 = this.highPointX.Value;
			if (this.lowPointX.IsNone && this.highPointX.IsNone)
			{
				Vector2 origin = this.hc.transform.position;
				Vector2 vector = new Vector2(num, 0f);
				RaycastHit2D hit = Physics2D.Raycast(origin, vector);
				if (!hit)
				{
					return;
				}
				num2 = hit.point.x;
				num3 = hit.point.x;
			}
			Bounds bounds = this.hc.Bounds;
			if (num > 0f)
			{
				float num4 = float.MinValue;
				if (!this.lowPointX.IsNone)
				{
					num4 = num2;
				}
				if (!this.highPointX.IsNone && num4 > num3)
				{
					num4 = num3;
				}
				num4 -= this.distance.Value;
				if (bounds.max.x > num4)
				{
					float x = num4 - bounds.max.x;
					this.hc.Body.MovePosition(this.hc.transform.position + new Vector3(x, 0f));
					return;
				}
			}
			else
			{
				float num5 = float.MaxValue;
				if (!this.lowPointX.IsNone)
				{
					num5 = num2;
				}
				if (!this.highPointX.IsNone && num5 < num3)
				{
					num5 = num3;
				}
				num5 += this.distance.Value;
				if (bounds.min.x < num5)
				{
					float x2 = num5 - bounds.min.x;
					this.hc.Body.MovePosition(this.hc.transform.position + new Vector3(x2, 0f));
				}
			}
		}

		// Token: 0x04007C57 RID: 31831
		public FsmFloat lowPointX;

		// Token: 0x04007C58 RID: 31832
		public FsmFloat highPointX;

		// Token: 0x04007C59 RID: 31833
		public FsmFloat direction;

		// Token: 0x04007C5A RID: 31834
		public FsmFloat distance;

		// Token: 0x04007C5B RID: 31835
		public FsmBool everyFixedUpdate;

		// Token: 0x04007C5C RID: 31836
		private HeroController hc;
	}
}
