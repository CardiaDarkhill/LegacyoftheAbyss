using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000073 RID: 115
[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class CogRollThenFallOver : SpriteExtruder
{
	// Token: 0x14000009 RID: 9
	// (add) Token: 0x0600033E RID: 830 RVA: 0x00011374 File Offset: 0x0000F574
	// (remove) Token: 0x0600033F RID: 831 RVA: 0x000113AC File Offset: 0x0000F5AC
	public event Action Fallen;

	// Token: 0x1400000A RID: 10
	// (add) Token: 0x06000340 RID: 832 RVA: 0x000113E4 File Offset: 0x0000F5E4
	// (remove) Token: 0x06000341 RID: 833 RVA: 0x0001141C File Offset: 0x0000F61C
	public event Action FallenCleanup;

	// Token: 0x06000342 RID: 834 RVA: 0x00011454 File Offset: 0x0000F654
	protected override void Awake()
	{
		base.Awake();
		this.body = base.GetComponent<Rigidbody2D>();
		this.collider = (this.colliderOverride ? this.colliderOverride : base.GetComponent<CircleCollider2D>());
		this.selfCollider = base.GetComponent<Collider2D>();
		if (this.fallenObject)
		{
			this.fallenObjectTransform = this.fallenObject.transform;
		}
	}

	// Token: 0x06000343 RID: 835 RVA: 0x000114C0 File Offset: 0x0000F6C0
	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.fallenObject)
		{
			this.fallenObject.gameObject.SetActive(false);
			this.fallenObjectStartPos = this.fallenObjectTransform.localPosition;
			this.fallenObjectStartRot = this.fallenObjectTransform.localRotation;
			this.fallenObjectCollider = this.fallenObject.GetComponent<Collider2D>();
			if (this.fallenObjectCollider)
			{
				this.fallenObjectCollider.enabled = true;
			}
		}
		if (this.body.bodyType != RigidbodyType2D.Static)
		{
			this.body.bodyType = RigidbodyType2D.Dynamic;
		}
		this.SetFallOverAmount(0f);
		this.fallWaitTimeLeft = 0f;
		this.hasFallen = false;
		this.isActive = !this.startInactive;
		if (this.rollAudio)
		{
			this.rollAudio.Play();
		}
		this.markedForCleanup = false;
	}

	// Token: 0x06000344 RID: 836 RVA: 0x000115A8 File Offset: 0x0000F7A8
	private void OnDisable()
	{
		if (this.fallenObject)
		{
			this.fallenObjectTransform.localPosition = this.fallenObjectStartPos;
			this.fallenObjectTransform.localRotation = this.fallenObjectStartRot;
		}
		if (this.disableSelfColliderOnFall && this.selfCollider)
		{
			this.selfCollider.enabled = true;
		}
	}

	// Token: 0x06000345 RID: 837 RVA: 0x0001160C File Offset: 0x0000F80C
	private void FixedUpdate()
	{
		if (this.hasFallen)
		{
			return;
		}
		if (!this.isActive)
		{
			return;
		}
		if (this.IsBodyMoving())
		{
			this.fallWaitTimeLeft = this.fallWaitTime;
			return;
		}
		if (this.fallWaitTimeLeft > 0f)
		{
			if (this.rollAudio)
			{
				this.rollAudio.Stop();
			}
			this.fallWaitTimeLeft -= Time.deltaTime;
			if (this.fallWaitTimeLeft <= 0f)
			{
				this.DoFallOver();
			}
		}
	}

	// Token: 0x06000346 RID: 838 RVA: 0x0001168A File Offset: 0x0000F88A
	public void Activate()
	{
		this.isActive = true;
	}

	// Token: 0x06000347 RID: 839 RVA: 0x00011693 File Offset: 0x0000F893
	private void DoFallOver()
	{
		if (this.fallRoutine != null)
		{
			base.StopCoroutine(this.fallRoutine);
		}
		this.fallRoutine = base.StartCoroutine(this.FallOver());
	}

	// Token: 0x06000348 RID: 840 RVA: 0x000116BC File Offset: 0x0000F8BC
	private bool IsBodyMoving()
	{
		float magnitude = this.body.linearVelocity.magnitude;
		float num = Mathf.Abs(this.body.angularVelocity);
		return (this.velocityThreshold > 0f && magnitude > this.velocityThreshold) || (this.angularVelocityThreshold > 0f && num > this.angularVelocityThreshold);
	}

	// Token: 0x06000349 RID: 841 RVA: 0x0001171E File Offset: 0x0000F91E
	private IEnumerator FallOver()
	{
		if (this.waitUntilVisible && !base.OriginalDisplay.isVisible)
		{
			float waitTimeLeft = this.maxVisibleWaitTime;
			while (!base.OriginalDisplay.isVisible)
			{
				yield return null;
				waitTimeLeft -= Time.deltaTime;
				if (waitTimeLeft <= 0f)
				{
					break;
				}
			}
			if (this.visibleFallDelay > 0f)
			{
				yield return new WaitForSeconds(this.visibleFallDelay);
			}
		}
		this.hasFallen = true;
		if (this.Fallen != null)
		{
			this.Fallen();
		}
		if (this.body.bodyType != RigidbodyType2D.Static)
		{
			this.body.bodyType = RigidbodyType2D.Kinematic;
		}
		this.body.linearVelocity = Vector2.zero;
		this.body.angularVelocity = 0f;
		if (this.disableSelfColliderOnFall && this.selfCollider)
		{
			this.selfCollider.enabled = false;
		}
		Vector3 localScale = base.transform.localScale;
		float num = Mathf.Sign(localScale.x) * Mathf.Sign(localScale.y);
		base.OriginalDisplay.transform.SetLocalRotation2D(base.transform.localEulerAngles.z * num);
		base.transform.SetLocalRotation2D(0f);
		float frameTime = (this.fallFpsLimit > 0f) ? (1f / this.fallFpsLimit) : 0f;
		WaitForSeconds wait = new WaitForSeconds(frameTime);
		float elapsed;
		for (elapsed = 0f; elapsed < this.fallDuration; elapsed += Mathf.Max(Time.deltaTime, frameTime))
		{
			this.SetFallOverAmount(elapsed / this.fallDuration);
			yield return wait;
		}
		this.SetFallOverAmount(1f);
		base.OriginalDisplay.gameObject.SetActive(false);
		if (this.fallenObject)
		{
			this.fallenObject.gameObject.SetActive(true);
			this.fallenObject.linearVelocity = Vector2.zero;
			this.fallenObject.angularVelocity = 0f;
			this.fallenObject.AddForce(this.fallenObjectForce, ForceMode2D.Impulse);
			if (this.moveDownWithFall)
			{
				this.moveDownWithFall.transform.SetParent(this.fallenObject.transform);
				this.moveDownWithFall.transform.SetLocalPosition2D(Vector2.zero);
			}
		}
		if (this.preventCleanup || !this.fallenObjectCollider)
		{
			yield break;
		}
		elapsed = 0f;
		while (elapsed < 20f && !this.markedForCleanup)
		{
			yield return null;
			if (this.fallenObject.IsSleeping())
			{
				elapsed += Time.deltaTime;
			}
			else
			{
				elapsed = 0f;
			}
		}
		this.fallenObjectCollider.enabled = false;
		yield return new WaitForSeconds(2.5f);
		this.fallenObjectCollider.enabled = true;
		if (this.FallenCleanup != null)
		{
			this.FallenCleanup();
		}
		else
		{
			base.gameObject.Recycle();
		}
		yield break;
	}

	// Token: 0x0600034A RID: 842 RVA: 0x00011730 File Offset: 0x0000F930
	private void SetFallOverAmount(float amount)
	{
		amount = this.fallTimeCurve.Evaluate(amount);
		Transform transform = base.OriginalDisplay.transform;
		float b = -this.collider.radius + base.ExtrusionDepth;
		float x = Mathf.LerpUnclamped(0f, 90f, amount);
		Vector3 localEulerAngles = transform.localEulerAngles;
		localEulerAngles.x = x;
		transform.localEulerAngles = localEulerAngles;
		amount = this.fallPositionCurve.Evaluate(amount);
		float y = Mathf.LerpUnclamped(0f, b, amount);
		transform.localPosition = new Vector3(0f, y, 0f);
		if (this.moveDownWithFall)
		{
			this.moveDownWithFall.SetPosition2D(transform.position);
		}
	}

	// Token: 0x0600034B RID: 843 RVA: 0x000117E8 File Offset: 0x0000F9E8
	public void MarkForCleanup()
	{
	}

	// Token: 0x040002DC RID: 732
	[Space]
	[SerializeField]
	private AnimationCurve fallPositionCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040002DD RID: 733
	[SerializeField]
	private AnimationCurve fallTimeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040002DE RID: 734
	[SerializeField]
	private float fallDuration;

	// Token: 0x040002DF RID: 735
	[SerializeField]
	private float fallFpsLimit;

	// Token: 0x040002E0 RID: 736
	[Space]
	[SerializeField]
	private float velocityThreshold;

	// Token: 0x040002E1 RID: 737
	[SerializeField]
	private float angularVelocityThreshold;

	// Token: 0x040002E2 RID: 738
	[SerializeField]
	private float fallWaitTime;

	// Token: 0x040002E3 RID: 739
	[Space]
	[SerializeField]
	private Rigidbody2D fallenObject;

	// Token: 0x040002E4 RID: 740
	[SerializeField]
	private Vector2 fallenObjectForce;

	// Token: 0x040002E5 RID: 741
	[SerializeField]
	private CircleCollider2D colliderOverride;

	// Token: 0x040002E6 RID: 742
	[Space]
	[SerializeField]
	private AudioSource rollAudio;

	// Token: 0x040002E7 RID: 743
	[Space]
	[SerializeField]
	private Transform moveDownWithFall;

	// Token: 0x040002E8 RID: 744
	[Space]
	[SerializeField]
	private bool waitUntilVisible;

	// Token: 0x040002E9 RID: 745
	[SerializeField]
	[ModifiableProperty]
	[Conditional("waitUntilVisible", true, false, false)]
	private float maxVisibleWaitTime;

	// Token: 0x040002EA RID: 746
	[SerializeField]
	[ModifiableProperty]
	[Conditional("waitUntilVisible", true, false, false)]
	private float visibleFallDelay;

	// Token: 0x040002EB RID: 747
	[SerializeField]
	private bool startInactive;

	// Token: 0x040002EC RID: 748
	[SerializeField]
	private bool preventCleanup;

	// Token: 0x040002ED RID: 749
	[Space]
	[SerializeField]
	private bool disableSelfColliderOnFall;

	// Token: 0x040002EE RID: 750
	private bool isActive;

	// Token: 0x040002EF RID: 751
	private bool hasFallen;

	// Token: 0x040002F0 RID: 752
	private float fallWaitTimeLeft;

	// Token: 0x040002F1 RID: 753
	private Coroutine fallRoutine;

	// Token: 0x040002F2 RID: 754
	private bool markedForCleanup;

	// Token: 0x040002F3 RID: 755
	private Transform fallenObjectTransform;

	// Token: 0x040002F4 RID: 756
	private Vector2 fallenObjectStartPos;

	// Token: 0x040002F5 RID: 757
	private Quaternion fallenObjectStartRot;

	// Token: 0x040002F6 RID: 758
	private Collider2D fallenObjectCollider;

	// Token: 0x040002F7 RID: 759
	private Rigidbody2D body;

	// Token: 0x040002F8 RID: 760
	private CircleCollider2D collider;

	// Token: 0x040002F9 RID: 761
	private Collider2D selfCollider;
}
