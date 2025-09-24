using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE9 RID: 3049
	public class CheckForEnemyDamagerBelow : FSMUtility.CheckFsmStateEveryFrameAction
	{
		// Token: 0x06005D6C RID: 23916 RVA: 0x001D7286 File Offset: 0x001D5486
		public override void Reset()
		{
			base.Reset();
			this.Target = null;
		}

		// Token: 0x06005D6D RID: 23917 RVA: 0x001D7298 File Offset: 0x001D5498
		public override void OnEnter()
		{
			this.obj = this.Target.GetSafe(this);
			if (this.obj)
			{
				this.collider = this.obj.GetComponent<BoxCollider2D>();
				this.layerMask = Helper.GetCollidingLayerMaskForLayer(this.obj.layer);
			}
			base.OnEnter();
		}

		// Token: 0x17000BBC RID: 3004
		// (get) Token: 0x06005D6E RID: 23918 RVA: 0x001D72F4 File Offset: 0x001D54F4
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
				int a = Physics2D.BoxCastNonAlloc(vector, size, this.obj.transform.eulerAngles.z, Vector2.down, this.results, 100f, this.layerMask);
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

		// Token: 0x0400598E RID: 22926
		[CheckForComponent(typeof(BoxCollider2D))]
		public FsmOwnerDefault Target;

		// Token: 0x0400598F RID: 22927
		private GameObject obj;

		// Token: 0x04005990 RID: 22928
		private BoxCollider2D collider;

		// Token: 0x04005991 RID: 22929
		private int layerMask;

		// Token: 0x04005992 RID: 22930
		private readonly RaycastHit2D[] results = new RaycastHit2D[20];
	}
}
