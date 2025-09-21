using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002E7 RID: 743
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(tk2dSpriteAnimator))]
public class FakeBat : MonoBehaviour
{
	// Token: 0x06001A37 RID: 6711 RVA: 0x00078AA9 File Offset: 0x00076CA9
	protected void Awake()
	{
		this.body = base.GetComponent<Rigidbody2D>();
		this.meshRenderer = base.GetComponent<MeshRenderer>();
		this.spriteAnimator = base.GetComponent<tk2dSpriteAnimator>();
		this.state = FakeBat.States.WaitingForBossAwake;
	}

	// Token: 0x06001A38 RID: 6712 RVA: 0x00078AD6 File Offset: 0x00076CD6
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	protected static void Init()
	{
		FakeBat.fakeBats = new List<FakeBat>();
	}

	// Token: 0x06001A39 RID: 6713 RVA: 0x00078AE2 File Offset: 0x00076CE2
	protected void OnEnable()
	{
		FakeBat.fakeBats.Add(this);
	}

	// Token: 0x06001A3A RID: 6714 RVA: 0x00078AEF File Offset: 0x00076CEF
	protected void OnDisable()
	{
		FakeBat.fakeBats.Remove(this);
	}

	// Token: 0x06001A3B RID: 6715 RVA: 0x00078B00 File Offset: 0x00076D00
	protected void Start()
	{
		float num = Random.Range(0.7f, 0.9f);
		base.transform.SetScaleX(num);
		base.transform.SetScaleY(num);
		base.transform.SetPositionZ(0f);
	}

	// Token: 0x06001A3C RID: 6716 RVA: 0x00078B45 File Offset: 0x00076D45
	protected void Update()
	{
		this.turnCooldown -= Time.deltaTime;
	}

	// Token: 0x06001A3D RID: 6717 RVA: 0x00078B5C File Offset: 0x00076D5C
	public static void NotifyAllBossAwake()
	{
		foreach (FakeBat fakeBat in FakeBat.fakeBats)
		{
			if (!(fakeBat == null))
			{
				fakeBat.NotifyBossAwake();
			}
		}
	}

	// Token: 0x06001A3E RID: 6718 RVA: 0x00078BB8 File Offset: 0x00076DB8
	public void NotifyBossAwake()
	{
		this.state = FakeBat.States.Dormant;
	}

	// Token: 0x06001A3F RID: 6719 RVA: 0x00078BC4 File Offset: 0x00076DC4
	public static void SendAllOut()
	{
		foreach (FakeBat fakeBat in FakeBat.fakeBats)
		{
			if (!(fakeBat == null))
			{
				fakeBat.SendOut();
			}
		}
	}

	// Token: 0x06001A40 RID: 6720 RVA: 0x00078C20 File Offset: 0x00076E20
	public void SendOut()
	{
		if (this.state != FakeBat.States.Dormant)
		{
			return;
		}
		base.StartCoroutine("SendOutRoutine");
		base.StopCoroutine("BringInRoutine");
	}

	// Token: 0x06001A41 RID: 6721 RVA: 0x00078C43 File Offset: 0x00076E43
	protected IEnumerator SendOutRoutine()
	{
		this.state = FakeBat.States.Out;
		base.transform.SetPosition2D(this.grimm.transform.position);
		base.transform.SetPositionZ(0f);
		this.meshRenderer.enabled = true;
		this.spriteAnimator.Play("Bat Fly");
		int? selectedDirection = null;
		for (;;)
		{
			int num = selectedDirection ?? Random.Range(0, 4);
			float minInclusive;
			float maxInclusive;
			float minInclusive2;
			float maxInclusive2;
			int num2;
			float num3;
			if (num == 1)
			{
				minInclusive = 1f;
				maxInclusive = 4f;
				minInclusive2 = 2f;
				maxInclusive2 = 3f;
				num2 = 1;
				num3 = 0.3f;
			}
			else if (num == 3)
			{
				minInclusive = 1f;
				maxInclusive = 4f;
				minInclusive2 = -3f;
				maxInclusive2 = -2f;
				num2 = 1;
				num3 = 0.3f;
			}
			else if (num == 2)
			{
				minInclusive = -5f;
				maxInclusive = -3f;
				minInclusive2 = 0.5f;
				maxInclusive2 = 2f;
				num2 = 0;
				num3 = 0.5f;
			}
			else
			{
				minInclusive = 3f;
				maxInclusive = 5f;
				minInclusive2 = 0.5f;
				maxInclusive2 = 2f;
				num2 = 0;
				num3 = 0.5f;
			}
			int index = (num2 + 1) % 2;
			Vector2 accel = new Vector2(Random.Range(minInclusive, maxInclusive), Random.Range(minInclusive2, maxInclusive2));
			if (Random.Range(0, 1) == 0)
			{
				accel[index] = -accel[index];
			}
			Vector2 linearVelocity = this.body.linearVelocity;
			ref Vector2 ptr = ref linearVelocity;
			int index2 = num2;
			ptr[index2] *= num3;
			this.body.linearVelocity = linearVelocity;
			accel *= 0.5f;
			Vector2 maxSpeed = accel * 10f;
			maxSpeed.x = Mathf.Abs(maxSpeed.x);
			maxSpeed.y = Mathf.Abs(maxSpeed.y);
			float timer;
			for (timer = 0.2f; timer > 0f; timer -= Time.deltaTime)
			{
				this.FaceDirection((this.body.linearVelocity.x > 0f) ? 1 : -1, false);
				this.Accelerate(accel, new Vector2(15f, 10f));
				yield return null;
			}
			selectedDirection = null;
			timer = Random.Range(0.5f, 1.5f);
			while (selectedDirection == null && timer > 0f)
			{
				this.FaceDirection((this.body.linearVelocity.x > 0f) ? 1 : -1, false);
				this.Accelerate(accel, maxSpeed);
				Vector2 vector = base.transform.position;
				if (vector.x < 73f)
				{
					selectedDirection = new int?(0);
					break;
				}
				if (vector.x > 99f)
				{
					selectedDirection = new int?(2);
					break;
				}
				if (vector.y < 8f)
				{
					selectedDirection = new int?(1);
					break;
				}
				if (vector.y > 15f)
				{
					selectedDirection = new int?(3);
					break;
				}
				yield return null;
				timer -= Time.deltaTime;
			}
			accel = default(Vector2);
			maxSpeed = default(Vector2);
		}
		yield break;
	}

	// Token: 0x06001A42 RID: 6722 RVA: 0x00078C54 File Offset: 0x00076E54
	public static void BringAllIn()
	{
		foreach (FakeBat fakeBat in FakeBat.fakeBats)
		{
			if (!(fakeBat == null))
			{
				fakeBat.BringIn();
			}
		}
	}

	// Token: 0x06001A43 RID: 6723 RVA: 0x00078CB0 File Offset: 0x00076EB0
	public void BringIn()
	{
		base.StartCoroutine("BringInRoutine");
		base.StopCoroutine("SendOutRoutine");
	}

	// Token: 0x06001A44 RID: 6724 RVA: 0x00078CC9 File Offset: 0x00076EC9
	protected IEnumerator BringInRoutine()
	{
		this.state = FakeBat.States.In;
		int sign = (this.grimm.transform.position.x - this.body.linearVelocity.x > 0f) ? 1 : -1;
		this.FaceDirection(sign, true);
		this.body.linearVelocity = Vector2.zero;
		for (;;)
		{
			Vector2 current = base.transform.position;
			Vector2 vector = this.grimm.transform.position;
			Vector2 vector2 = Vector2.MoveTowards(current, vector, 25f * Time.deltaTime);
			base.transform.SetPosition2D(vector2);
			if (Vector2.Distance(vector2, vector) < Mathf.Epsilon)
			{
				break;
			}
			yield return null;
		}
		this.spriteAnimator.Play("Bat End");
		while (this.spriteAnimator.ClipTimeSeconds < this.spriteAnimator.CurrentClip.Duration - Mathf.Epsilon)
		{
			yield return null;
		}
		this.meshRenderer.enabled = false;
		base.transform.SetPositionY(-50f);
		this.state = FakeBat.States.Dormant;
		yield break;
	}

	// Token: 0x06001A45 RID: 6725 RVA: 0x00078CD8 File Offset: 0x00076ED8
	private void FaceDirection(int sign, bool snap)
	{
		float num = Mathf.Abs(base.transform.localScale.x) * (float)sign;
		if (!Mathf.Approximately(base.transform.localScale.x, num) && (snap || this.turnCooldown <= 0f))
		{
			if (!snap)
			{
				this.spriteAnimator.Play("Bat TurnToFly");
				this.spriteAnimator.PlayFromFrame(0);
				this.turnCooldown = 0.5f;
			}
			base.transform.SetScaleX(num);
		}
	}

	// Token: 0x06001A46 RID: 6726 RVA: 0x00078D5C File Offset: 0x00076F5C
	private void Accelerate(Vector2 fixedAcceleration, Vector2 speedLimit)
	{
		Vector2 a = fixedAcceleration / Time.fixedDeltaTime;
		Vector2 vector = this.body.linearVelocity;
		vector += a * Time.deltaTime;
		vector.x = Mathf.Clamp(vector.x, -speedLimit.x, speedLimit.x);
		vector.y = Mathf.Clamp(vector.y, -speedLimit.y, speedLimit.y);
		this.body.linearVelocity = vector;
	}

	// Token: 0x04001928 RID: 6440
	private Rigidbody2D body;

	// Token: 0x04001929 RID: 6441
	private MeshRenderer meshRenderer;

	// Token: 0x0400192A RID: 6442
	private tk2dSpriteAnimator spriteAnimator;

	// Token: 0x0400192B RID: 6443
	private FakeBat.States state;

	// Token: 0x0400192C RID: 6444
	private float turnCooldown;

	// Token: 0x0400192D RID: 6445
	[SerializeField]
	private Transform grimm;

	// Token: 0x0400192E RID: 6446
	private const float Z = 0f;

	// Token: 0x0400192F RID: 6447
	private static List<FakeBat> fakeBats;

	// Token: 0x020015C1 RID: 5569
	private enum States
	{
		// Token: 0x04008870 RID: 34928
		WaitingForBossAwake,
		// Token: 0x04008871 RID: 34929
		Dormant,
		// Token: 0x04008872 RID: 34930
		In,
		// Token: 0x04008873 RID: 34931
		Out
	}
}
