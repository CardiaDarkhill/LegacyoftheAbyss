using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004A8 RID: 1192
public class BreakablePoleTopLand : MonoBehaviour, MetronomePlat.INotify
{
	// Token: 0x06002B3B RID: 11067 RVA: 0x000BD98C File Offset: 0x000BBB8C
	private void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
	}

	// Token: 0x06002B3C RID: 11068 RVA: 0x000BD99C File Offset: 0x000BBB9C
	private void OnCollisionEnter2D(Collision2D collision)
	{
		Collider2D otherCollider = collision.otherCollider;
		if (collision.gameObject.layer != 8)
		{
			return;
		}
		Vector2 point = collision.GetSafeContact().Point;
		if (point.y > otherCollider.bounds.center.y)
		{
			return;
		}
		float num = BreakablePoleTopLand.<OnCollisionEnter2D>g__NormalizeAngle|8_0(base.transform.eulerAngles.z);
		float num2 = this.angleMin;
		float num3 = this.angleMax;
		if (base.transform.lossyScale.x < 0f)
		{
			num2 = BreakablePoleTopLand.<OnCollisionEnter2D>g__NormalizeAngle|8_0(360f - this.angleMax);
			num3 = BreakablePoleTopLand.<OnCollisionEnter2D>g__NormalizeAngle|8_0(360f - this.angleMin);
		}
		if (num < num2 || num > num3)
		{
			return;
		}
		GameObject[] array = this.effects;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Spawn(point);
		}
		this.stickAudioTable.SpawnAndPlayOneShot(base.transform.position, false);
		if (this.body)
		{
			this.body.isKinematic = true;
			if (!this.keepSimulating)
			{
				this.body.simulated = false;
			}
			this.body.linearVelocity = Vector2.zero;
			this.body.angularVelocity = 0f;
			MetronomePlat componentInParent = collision.gameObject.GetComponentInParent<MetronomePlat>();
			if (componentInParent)
			{
				componentInParent.RegisterNotifier(this);
			}
		}
		this.OnStick.Invoke();
	}

	// Token: 0x06002B3D RID: 11069 RVA: 0x000BDB10 File Offset: 0x000BBD10
	public void PlatRetracted(MetronomePlat plat)
	{
		plat.UnregisterNotifier(this);
		this.body.isKinematic = false;
		this.body.simulated = true;
		CollectableItemPickup componentInChildren = base.GetComponentInChildren<CollectableItemPickup>();
		if (componentInChildren)
		{
			componentInChildren.CancelPickup();
		}
	}

	// Token: 0x06002B3F RID: 11071 RVA: 0x000BDB70 File Offset: 0x000BBD70
	[CompilerGenerated]
	internal static float <OnCollisionEnter2D>g__NormalizeAngle|8_0(float angle)
	{
		angle %= 360f;
		if (angle < 0f)
		{
			angle += 360f;
		}
		return angle;
	}

	// Token: 0x04002C85 RID: 11397
	[SerializeField]
	private float angleMin = 165f;

	// Token: 0x04002C86 RID: 11398
	[SerializeField]
	private float angleMax = 195f;

	// Token: 0x04002C87 RID: 11399
	[SerializeField]
	private bool keepSimulating;

	// Token: 0x04002C88 RID: 11400
	[Space]
	[SerializeField]
	private GameObject[] effects;

	// Token: 0x04002C89 RID: 11401
	[SerializeField]
	private RandomAudioClipTable stickAudioTable;

	// Token: 0x04002C8A RID: 11402
	[Space]
	public UnityEvent OnStick;

	// Token: 0x04002C8B RID: 11403
	private Rigidbody2D body;
}
