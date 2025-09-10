using System;
using UnityEngine;

// Token: 0x020004A2 RID: 1186
public class BreakableGenerateCorpse : MonoBehaviour
{
	// Token: 0x06002B10 RID: 11024 RVA: 0x000BC0F1 File Offset: 0x000BA2F1
	private bool HasCorpseObject()
	{
		return this.corpseObject;
	}

	// Token: 0x06002B11 RID: 11025 RVA: 0x000BC100 File Offset: 0x000BA300
	private void Awake()
	{
		if (!this.corpseObject)
		{
			this.corpseObject = Object.Instantiate<GameObject>(this.corpsePrefab);
		}
		ActiveCorpse component = this.corpseObject.GetComponent<ActiveCorpse>();
		if (component)
		{
			component.SetBlockAudio(true);
			this.corpseObject.SetActive(true);
			component.SetBlockAudio(false);
		}
		else
		{
			this.corpseObject.SetActive(true);
		}
		this.corpseObject.SetActive(false);
		if (this.placedCorpseObject)
		{
			this.placedCorpseObject.SetActive(true);
			this.placedCorpseObject.SetActive(false);
		}
		this.breakable.BrokenHit += this.FlingCorpse;
		this.breakable.AlreadyBroken += this.PlaceCorpse;
	}

	// Token: 0x06002B12 RID: 11026 RVA: 0x000BC1C8 File Offset: 0x000BA3C8
	private void FlingCorpse(HitInstance hit)
	{
		Vector3 position = base.transform.TransformPoint(this.corpseOffset);
		position.z = Random.Range(0.008f, 0.009f);
		this.corpseObject.transform.position = position;
		this.corpseObject.SetActive(true);
		float num = this.corpseFlingSpeed;
		float num2 = hit.GetMagnitudeMultForType(HitInstance.TargetType.Corpse);
		float num3 = this.corpseObject.transform.localScale.x * (this.corpseFacesRight ? 1f : -1f);
		float num4 = 60f;
		float num5 = 120f;
		if (num2 > 1.25f)
		{
			num4 = 45f;
			num5 = 135f;
		}
		float num6;
		switch (hit.GetActualHitDirection(base.transform, HitInstance.TargetType.Corpse))
		{
		case HitInstance.HitDirection.Left:
			num6 = num5;
			this.corpseObject.transform.SetScaleX(num3 * Mathf.Sign(base.transform.localScale.x));
			break;
		case HitInstance.HitDirection.Right:
			num6 = num4;
			this.corpseObject.transform.SetScaleX(-num3 * Mathf.Sign(base.transform.localScale.x));
			break;
		case HitInstance.HitDirection.Up:
			num6 = Random.Range(75f, 105f);
			num *= 1.3f;
			break;
		case HitInstance.HitDirection.Down:
			num6 = 270f;
			break;
		default:
			num6 = 90f;
			break;
		}
		if (num2 < 0.5f)
		{
			num2 = 0.5f;
		}
		this.corpseObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Mathf.Cos(num6 * 0.017453292f), Mathf.Sin(num6 * 0.017453292f)) * (num * num2);
	}

	// Token: 0x06002B13 RID: 11027 RVA: 0x000BC370 File Offset: 0x000BA570
	private void PlaceCorpse()
	{
		if (this.placedCorpseObject)
		{
			this.placedCorpseObject.SetActive(true);
			return;
		}
		ActiveCorpse component = this.corpseObject.GetComponent<ActiveCorpse>();
		if (component)
		{
			component.SetBlockAudio(true);
			this.corpseObject.SetActive(true);
			component.SetOnGround();
			component.SetBlockAudio(false);
			return;
		}
		this.corpseObject.SetActive(true);
	}

	// Token: 0x04002C2B RID: 11307
	[SerializeField]
	private Breakable breakable;

	// Token: 0x04002C2C RID: 11308
	[SerializeField]
	[ModifiableProperty]
	[Conditional("HasCorpseObject", false, true, false)]
	private GameObject corpsePrefab;

	// Token: 0x04002C2D RID: 11309
	[SerializeField]
	private GameObject corpseObject;

	// Token: 0x04002C2E RID: 11310
	[SerializeField]
	private bool corpseFacesRight;

	// Token: 0x04002C2F RID: 11311
	[SerializeField]
	private Vector2 corpseOffset;

	// Token: 0x04002C30 RID: 11312
	[SerializeField]
	private float corpseFlingSpeed;

	// Token: 0x04002C31 RID: 11313
	[SerializeField]
	private GameObject placedCorpseObject;
}
