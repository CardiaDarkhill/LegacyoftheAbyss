using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005B3 RID: 1459
public class RecoilEnemiesToRadius : MonoBehaviour
{
	// Token: 0x170005D2 RID: 1490
	// (get) Token: 0x06003469 RID: 13417 RVA: 0x000E8E62 File Offset: 0x000E7062
	public Vector3 Position
	{
		get
		{
			return base.transform.TransformPoint(this.offset);
		}
	}

	// Token: 0x170005D3 RID: 1491
	// (get) Token: 0x0600346A RID: 13418 RVA: 0x000E8E7A File Offset: 0x000E707A
	public float ScaledInnerRadius
	{
		get
		{
			return base.transform.TransformRadius(this.innerRadius);
		}
	}

	// Token: 0x170005D4 RID: 1492
	// (get) Token: 0x0600346B RID: 13419 RVA: 0x000E8E8D File Offset: 0x000E708D
	public float ScaledOuterRadius
	{
		get
		{
			return base.transform.TransformRadius(this.outerRadius);
		}
	}

	// Token: 0x0600346C RID: 13420 RVA: 0x000E8EA0 File Offset: 0x000E70A0
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(this.Position, this.ScaledInnerRadius);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(this.Position, this.ScaledOuterRadius);
	}

	// Token: 0x0600346D RID: 13421 RVA: 0x000E8ED8 File Offset: 0x000E70D8
	private void OnValidate()
	{
		if (this.outerRadius < this.innerRadius)
		{
			this.outerRadius = this.innerRadius;
		}
	}

	// Token: 0x0600346E RID: 13422 RVA: 0x000E8EF4 File Offset: 0x000E70F4
	private void OnEnable()
	{
		this.enemies.Clear();
		int collidingLayerMaskForLayer = Helper.GetCollidingLayerMaskForLayer(base.gameObject.layer);
		foreach (Collider2D collider2D in base.GetComponents<Collider2D>())
		{
			Collider2D[] array = new Collider2D[10];
			if (collider2D.Overlap(new ContactFilter2D
			{
				useTriggers = true,
				useLayerMask = true,
				layerMask = collidingLayerMaskForLayer
			}, array) > 0)
			{
				foreach (Collider2D collider2D2 in array)
				{
					if (collider2D2)
					{
						this.OnTriggerEnter2D(collider2D2);
					}
				}
			}
		}
	}

	// Token: 0x0600346F RID: 13423 RVA: 0x000E8F9C File Offset: 0x000E719C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		GameObject gameObject = collision.gameObject;
		using (List<RecoilEnemiesToRadius.EnemyData>.Enumerator enumerator = this.enemies.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.Obj == gameObject)
				{
					return;
				}
			}
		}
		Recoil component = gameObject.GetComponent<Recoil>();
		if (!component)
		{
			return;
		}
		this.enemies.Add(new RecoilEnemiesToRadius.EnemyData
		{
			Obj = gameObject,
			Transform = gameObject.transform,
			Collider = collision,
			Recoil = component,
			Body = gameObject.GetComponent<Rigidbody2D>()
		});
	}

	// Token: 0x06003470 RID: 13424 RVA: 0x000E9054 File Offset: 0x000E7254
	private void OnTriggerExit2D(Collider2D collision)
	{
		int num = this.enemies.FindIndex((RecoilEnemiesToRadius.EnemyData e) => e.Obj == collision.gameObject);
		if (num < 0)
		{
			return;
		}
		this.enemies.RemoveAt(num);
	}

	// Token: 0x06003471 RID: 13425 RVA: 0x000E9098 File Offset: 0x000E7298
	private void FixedUpdate()
	{
		foreach (RecoilEnemiesToRadius.EnemyData enemy in this.enemies)
		{
			this.RecoilEnemy(enemy);
		}
	}

	// Token: 0x06003472 RID: 13426 RVA: 0x000E90EC File Offset: 0x000E72EC
	private void RecoilEnemy(RecoilEnemiesToRadius.EnemyData enemy)
	{
		if (enemy.Recoil.FreezeInPlace)
		{
			return;
		}
		float num = enemy.Recoil.RecoilSpeedBase * this.recoilMultipler;
		Vector2 vector = enemy.Transform.position - this.Position;
		Vector2 vector2 = vector.normalized * (num * Time.deltaTime);
		float magnitude = vector.magnitude;
		float scaledInnerRadius = this.ScaledInnerRadius;
		float scaledOuterRadius = this.ScaledOuterRadius;
		if (magnitude < scaledInnerRadius)
		{
			float time = Mathf.Clamp01(magnitude / scaledInnerRadius);
			vector2 *= this.innerForceCurve.Evaluate(time);
		}
		else
		{
			float num2 = scaledOuterRadius - scaledInnerRadius;
			float time2 = Mathf.Clamp01((magnitude - scaledInnerRadius) / num2);
			vector2 *= this.outerForceCurve.Evaluate(time2) * -1f;
		}
		Sweep sweepForwards = new Sweep(enemy.Collider, (vector2.x < 0f) ? 2 : 0, 3, 0.1f, 0.01f);
		Sweep sweepForwards2 = new Sweep(enemy.Collider, (vector2.y < 0f) ? 3 : 1, 3, 0.1f, 0.01f);
		Sweep sweepBackwards = new Sweep(enemy.Collider, (vector2.x > 0f) ? 2 : 0, 3, 0.1f, 0.01f);
		Sweep sweepBackwards2 = new Sweep(enemy.Collider, (vector2.y > 0f) ? 3 : 1, 3, 0.1f, 0.01f);
		RecoilEnemiesToRadius.CheckInDirection(sweepForwards, sweepBackwards, ref vector2.x);
		RecoilEnemiesToRadius.CheckInDirection(sweepForwards2, sweepBackwards2, ref vector2.y);
		if (enemy.Recoil.IsUpBlocked && vector2.y > 0f)
		{
			vector2.y = 0f;
		}
		if (enemy.Recoil.IsDownBlocked && vector2.y < 0f)
		{
			vector2.y = 0f;
		}
		if (enemy.Recoil.IsLeftBlocked && vector2.x < 0f)
		{
			vector2.x = 0f;
		}
		if (enemy.Recoil.IsRightBlocked && vector2.x > 0f)
		{
			vector2.x = 0f;
		}
		if (enemy.Body && Mathf.Abs(enemy.Body.gravityScale) > 0f && vector2.y > 0f)
		{
			vector2.y = 0f;
		}
		enemy.Transform.Translate(vector2);
	}

	// Token: 0x06003473 RID: 13427 RVA: 0x000E9360 File Offset: 0x000E7560
	private static void CheckInDirection(Sweep sweepForwards, Sweep sweepBackwards, ref float moveAmount)
	{
		float distance = Mathf.Abs(moveAmount);
		float num;
		if (!sweepForwards.Check(distance, 256, out num))
		{
			return;
		}
		float num2;
		if (!sweepBackwards.Check(distance, 256, out num2) || num2 < num)
		{
			moveAmount = num * Mathf.Sign(moveAmount);
		}
	}

	// Token: 0x040037E7 RID: 14311
	[SerializeField]
	private Vector2 offset;

	// Token: 0x040037E8 RID: 14312
	[SerializeField]
	private float innerRadius;

	// Token: 0x040037E9 RID: 14313
	[SerializeField]
	private float outerRadius;

	// Token: 0x040037EA RID: 14314
	[SerializeField]
	private AnimationCurve innerForceCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	// Token: 0x040037EB RID: 14315
	[SerializeField]
	private AnimationCurve outerForceCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040037EC RID: 14316
	[SerializeField]
	private float recoilMultipler = 1f;

	// Token: 0x040037ED RID: 14317
	private readonly List<RecoilEnemiesToRadius.EnemyData> enemies = new List<RecoilEnemiesToRadius.EnemyData>();

	// Token: 0x020018D6 RID: 6358
	private struct EnemyData
	{
		// Token: 0x04009376 RID: 37750
		public GameObject Obj;

		// Token: 0x04009377 RID: 37751
		public Transform Transform;

		// Token: 0x04009378 RID: 37752
		public Collider2D Collider;

		// Token: 0x04009379 RID: 37753
		public Recoil Recoil;

		// Token: 0x0400937A RID: 37754
		public Rigidbody2D Body;
	}
}
