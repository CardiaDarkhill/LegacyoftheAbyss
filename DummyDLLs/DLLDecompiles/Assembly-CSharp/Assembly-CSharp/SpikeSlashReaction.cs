using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x0200055B RID: 1371
public class SpikeSlashReaction : MonoBehaviour, IHitResponder
{
	// Token: 0x06003107 RID: 12551 RVA: 0x000D95C4 File Offset: 0x000D77C4
	private void Awake()
	{
		TinkEffect component = base.GetComponent<TinkEffect>();
		this.hasTinkEffect = (component != null);
		if (this.hasTinkEffect)
		{
			component.OnSpawnedTink += this.OnSpawnedTink;
		}
		this.collider = base.GetComponent<Collider2D>();
	}

	// Token: 0x06003108 RID: 12552 RVA: 0x000D960B File Offset: 0x000D780B
	private void OnEnable()
	{
	}

	// Token: 0x06003109 RID: 12553 RVA: 0x000D960D File Offset: 0x000D780D
	private void OnSpawnedTink(Vector3 position, Quaternion rotation)
	{
		if (!base.enabled)
		{
			return;
		}
		this.SpawnSlashReaction(position, rotation);
	}

	// Token: 0x0600310A RID: 12554 RVA: 0x000D9620 File Offset: 0x000D7820
	public void SpawnSlashReaction(Vector3 position, Quaternion rotation)
	{
		Effects.SpikeSlashEffectPrefab.Spawn(position, rotation);
		if (this.profile != null)
		{
			this.profile.SpawnEffect(position, rotation);
		}
	}

	// Token: 0x0600310B RID: 12555 RVA: 0x000D964C File Offset: 0x000D784C
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (this.hasTinkEffect)
		{
			return IHitResponder.Response.None;
		}
		if (!base.enabled)
		{
			return IHitResponder.Response.None;
		}
		Vector3 euler = new Vector3(0f, 0f, 0f);
		bool flag = this.collider != null;
		bool flag2 = flag;
		if (this.useNailPosition)
		{
			flag2 = false;
		}
		Vector2 vector = Vector2.zero;
		float num = 0f;
		float num2 = 0f;
		if (flag2)
		{
			Bounds bounds = this.collider.bounds;
			vector = base.transform.TransformPoint(this.collider.offset);
			num = bounds.size.x * 0.5f;
			num2 = bounds.size.y * 0.5f;
		}
		GameObject source = damageInstance.Source;
		bool flag3 = source.CompareTag("Nail Attack");
		HeroController instance = HeroController.instance;
		Vector3 position = source.transform.position;
		Vector3 vector2 = flag3 ? instance.transform.position : position;
		Vector3 vector3;
		switch (DirectionUtils.GetCardinalDirection(this.GetActualHitDirection(damageInstance.Source.GetComponent<DamageEnemies>())))
		{
		case 0:
			if (flag2)
			{
				vector3 = new Vector3(vector.x - num, position.y, 0.002f);
			}
			else if (flag3)
			{
				vector3 = new Vector3(vector2.x + 2f, vector2.y, 0.002f);
			}
			else
			{
				vector3 = new Vector3(vector2.x, vector2.y, 0.002f);
			}
			break;
		case 1:
			if (flag2)
			{
				vector3 = new Vector3(position.x, Mathf.Max(vector.y - num2, position.y), 0.002f);
			}
			else if (flag3)
			{
				vector3 = new Vector3(vector2.x, vector2.y + 2f, 0.002f);
			}
			else
			{
				vector3 = new Vector3(vector2.x, vector2.y, 0.002f);
			}
			euler = new Vector3(0f, 0f, 90f);
			break;
		case 2:
			if (flag2)
			{
				vector3 = new Vector3(vector.x + num, position.y, 0.002f);
			}
			else if (flag3)
			{
				vector3 = new Vector3(vector2.x - 2f, vector2.y, 0.002f);
			}
			else
			{
				vector3 = new Vector3(vector2.x, vector2.y, 0.002f);
			}
			euler = new Vector3(0f, 0f, 180f);
			break;
		default:
			if (flag2)
			{
				float num3 = position.x;
				if (num3 < vector.x - num)
				{
					num3 = vector.x - num;
				}
				if (num3 > vector.x + num)
				{
					num3 = vector.x + num;
				}
				vector3 = new Vector3(num3, Mathf.Min(vector.y + num2, position.y), 0.002f);
			}
			else if (flag3)
			{
				vector3 = new Vector3(vector2.x, vector2.y - 2f, 0.002f);
			}
			else
			{
				vector3 = new Vector3(vector2.x, vector2.y, 0.002f);
			}
			euler = new Vector3(0f, 0f, 270f);
			break;
		}
		if (flag)
		{
			vector3 = this.collider.ClosestPoint(vector3);
		}
		this.SpawnSlashReaction(vector3, Quaternion.Euler(euler));
		return default(IHitResponder.HitResponse);
	}

	// Token: 0x0600310C RID: 12556 RVA: 0x000D99E4 File Offset: 0x000D7BE4
	private float GetActualHitDirection(DamageEnemies damager)
	{
		if (!damager)
		{
			return 0f;
		}
		if (!damager.CircleDirection)
		{
			return damager.GetDirection();
		}
		Vector2 vector = base.transform.position - damager.transform.position;
		return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	// Token: 0x04003455 RID: 13397
	[SerializeField]
	[AssetPickerDropdown]
	private SpikeSlashReactionProfile profile;

	// Token: 0x04003456 RID: 13398
	[Space]
	[SerializeField]
	private bool useNailPosition = true;

	// Token: 0x04003457 RID: 13399
	private Collider2D collider;

	// Token: 0x04003458 RID: 13400
	private bool hasTinkEffect;
}
