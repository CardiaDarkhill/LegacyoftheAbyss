using System;
using UnityEngine;

// Token: 0x02000009 RID: 9
public class BombBounce : MonoBehaviour
{
	// Token: 0x14000001 RID: 1
	// (add) Token: 0x06000045 RID: 69 RVA: 0x000032A0 File Offset: 0x000014A0
	// (remove) Token: 0x06000046 RID: 70 RVA: 0x000032D8 File Offset: 0x000014D8
	public event BombBounce.BounceEvent OnBounce;

	// Token: 0x06000047 RID: 71 RVA: 0x0000330D File Offset: 0x0000150D
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody2D>();
		this.audio = base.GetComponent<AudioSource>();
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00003334 File Offset: 0x00001534
	private void FixedUpdate()
	{
		if (this.bouncing)
		{
			Vector2 a = new Vector2(base.transform.position.x, base.transform.position.y);
			this.velocity = a - this.lastPos;
			this.lastPos = a;
			this.speed = (this.rb ? this.rb.linearVelocity.magnitude : 0f);
		}
	}

	// Token: 0x06000049 RID: 73 RVA: 0x000033B8 File Offset: 0x000015B8
	private void LateUpdate()
	{
		if (this.animTimer > 0f)
		{
			this.animTimer -= Time.deltaTime;
		}
		this.prevXSpeed = this.xSpeed;
		this.xSpeed = this.rb.linearVelocity.x;
		if (this.rb.linearVelocity.x < 0.01f && this.rb.linearVelocity.x > -0.01f)
		{
			if (this.prevXSpeed > 0f)
			{
				this.rb.linearVelocity = new Vector2(-3f, this.rb.linearVelocity.y);
			}
			else
			{
				this.rb.linearVelocity = new Vector2(3f, this.rb.linearVelocity.y);
			}
		}
		this.prevYSpeed = this.ySpeed;
		this.ySpeed = this.rb.linearVelocity.y;
		if (this.rb.linearVelocity.y < 0.01f && this.rb.linearVelocity.y > -0.01f)
		{
			if (this.prevYSpeed > 0f)
			{
				this.rb.linearVelocity.Set(this.rb.linearVelocity.x, 0f);
				return;
			}
			this.rb.linearVelocity = new Vector2(this.rb.linearVelocity.x, 10f);
		}
	}

	// Token: 0x0600004A RID: 74 RVA: 0x00003538 File Offset: 0x00001738
	private void OnCollisionEnter2D(Collision2D col)
	{
		if (!this.rb || this.rb.isKinematic)
		{
			return;
		}
		if (this.bouncing && this.speed > this.speedThreshold)
		{
			Vector3 inNormal = col.GetSafeContact().Normal;
			Vector3 normalized = Vector3.Reflect(this.velocity.normalized, inNormal).normalized;
			Vector2 linearVelocity = new Vector2(normalized.x, normalized.y) * (this.speed * this.bounceFactor);
			this.rb.linearVelocity = linearVelocity;
			if (this.playSound)
			{
				this.chooser = Random.Range(1, 100);
				int num = Random.Range(0, this.clips.Length - 1);
				AudioClip clip = this.clips[num];
				if (this.chooser <= this.chanceToPlay)
				{
					float pitch = Random.Range(this.pitchMin, this.pitchMax);
					this.audio.pitch = pitch;
					this.audio.PlayOneShot(clip);
				}
			}
			if (this.playAnimationOnBounce && this.animTimer <= 0f)
			{
				this.animator.Play(this.animationName);
				this.animator.PlayFromFrame(0);
				this.animTimer = this.animPause;
			}
			if (this.OnBounce != null)
			{
				this.OnBounce();
			}
		}
	}

	// Token: 0x0600004B RID: 75 RVA: 0x0000369B File Offset: 0x0000189B
	private void OnCollisionStay2D(Collision2D col)
	{
	}

	// Token: 0x0600004C RID: 76 RVA: 0x0000369D File Offset: 0x0000189D
	public void StopBounce()
	{
		this.bouncing = false;
	}

	// Token: 0x0600004D RID: 77 RVA: 0x000036A6 File Offset: 0x000018A6
	public void StartBounce()
	{
		this.bouncing = true;
	}

	// Token: 0x0600004E RID: 78 RVA: 0x000036AF File Offset: 0x000018AF
	public void SetBounceFactor(float value)
	{
		this.bounceFactor = value;
	}

	// Token: 0x0600004F RID: 79 RVA: 0x000036B8 File Offset: 0x000018B8
	public void SetBounceAnimation(bool set)
	{
		this.playAnimationOnBounce = set;
	}

	// Token: 0x04000018 RID: 24
	public float bounceFactor;

	// Token: 0x04000019 RID: 25
	public float speedThreshold = 1f;

	// Token: 0x0400001A RID: 26
	public bool playSound;

	// Token: 0x0400001B RID: 27
	public float minBounceSpeed;

	// Token: 0x0400001C RID: 28
	public float maxBounceSpeed;

	// Token: 0x0400001D RID: 29
	public AudioClip[] clips;

	// Token: 0x0400001E RID: 30
	public int chanceToPlay = 100;

	// Token: 0x0400001F RID: 31
	public float pitchMin = 1f;

	// Token: 0x04000020 RID: 32
	public float pitchMax = 1f;

	// Token: 0x04000021 RID: 33
	public bool playAnimationOnBounce;

	// Token: 0x04000022 RID: 34
	public string animationName;

	// Token: 0x04000023 RID: 35
	public float animPause = 0.5f;

	// Token: 0x04000024 RID: 36
	private float speed;

	// Token: 0x04000025 RID: 37
	private float animTimer;

	// Token: 0x04000026 RID: 38
	private tk2dSpriteAnimator animator;

	// Token: 0x04000027 RID: 39
	private Vector2 velocity;

	// Token: 0x04000028 RID: 40
	private Vector2 lastPos;

	// Token: 0x04000029 RID: 41
	private Rigidbody2D rb;

	// Token: 0x0400002A RID: 42
	private AudioSource audio;

	// Token: 0x0400002B RID: 43
	private int chooser;

	// Token: 0x0400002C RID: 44
	private float xSpeed;

	// Token: 0x0400002D RID: 45
	private float prevXSpeed;

	// Token: 0x0400002E RID: 46
	private float ySpeed;

	// Token: 0x0400002F RID: 47
	private float prevYSpeed;

	// Token: 0x04000030 RID: 48
	private bool bouncing = true;

	// Token: 0x020013B6 RID: 5046
	// (Invoke) Token: 0x06008144 RID: 33092
	public delegate void BounceEvent();
}
