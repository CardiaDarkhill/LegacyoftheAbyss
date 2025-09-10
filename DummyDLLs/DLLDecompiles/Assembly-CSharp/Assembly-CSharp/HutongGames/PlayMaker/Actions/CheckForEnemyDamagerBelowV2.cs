using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BEA RID: 3050
	public class CheckForEnemyDamagerBelowV2 : FSMUtility.CheckFsmStateEveryFrameAction
	{
		// Token: 0x06005D70 RID: 23920 RVA: 0x001D745C File Offset: 0x001D565C
		public override void Reset()
		{
			base.Reset();
			this.Target = null;
			this.MaxDistance = null;
		}

		// Token: 0x06005D71 RID: 23921 RVA: 0x001D7474 File Offset: 0x001D5674
		public override void OnEnter()
		{
			this.obj = this.Target.GetSafe(this);
			if (this.obj)
			{
				this.collider = this.obj.GetComponent<BoxCollider2D>();
				this.layerMask = Helper.GetCollidingLayerMaskForLayer(this.obj.layer);
				this.maxDistance = this.MaxDistance.Value;
				if (this.maxDistance <= 0f)
				{
					this.maxDistance = 100f;
				}
			}
			base.OnEnter();
		}

		// Token: 0x17000BBD RID: 3005
		// (get) Token: 0x06005D72 RID: 23922 RVA: 0x001D74F8 File Offset: 0x001D56F8
		public override bool IsTrue
		{
			get
			{
				if (!this.collider)
				{
					return false;
				}
				Vector2 vector = this.obj.transform.TransformPoint(this.collider.offset);
				Vector2 size = this.collider.size.MultiplyElements(this.obj.transform.lossyScale).Abs();
				int a = Physics2D.BoxCastNonAlloc(vector, size, this.obj.transform.eulerAngles.z, Vector2.down, this.results, this.maxDistance, this.layerMask);
				float num = float.MaxValue;
				float num2 = float.MaxValue;
				bool flag = false;
				for (int i = 0; i < Mathf.Min(a, this.results.Length); i++)
				{
					RaycastHit2D raycastHit2D = this.results[i];
					GameObject gameObject = raycastHit2D.collider.gameObject;
					float num3 = vector.y - raycastHit2D.point.y;
					if (num3 >= 0f)
					{
						if (gameObject.layer == 17 || gameObject.GetComponent<DamageEnemies>())
						{
							if (num3 < num2)
							{
								num2 = num3;
							}
							flag = true;
						}
						else if (gameObject.layer == 8)
						{
							if (num3 < num)
							{
								num = num3;
							}
							flag = true;
						}
					}
				}
				return flag && num2 < num;
			}
		}

		// Token: 0x04005993 RID: 22931
		[CheckForComponent(typeof(BoxCollider2D))]
		public FsmOwnerDefault Target;

		// Token: 0x04005994 RID: 22932
		public FsmFloat MaxDistance;

		// Token: 0x04005995 RID: 22933
		private GameObject obj;

		// Token: 0x04005996 RID: 22934
		private BoxCollider2D collider;

		// Token: 0x04005997 RID: 22935
		private int layerMask;

		// Token: 0x04005998 RID: 22936
		private readonly RaycastHit2D[] results = new RaycastHit2D[20];

		// Token: 0x04005999 RID: 22937
		private float maxDistance;
	}
}
