using System;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000016 RID: 22
public class ObjectBounce : MonoBehaviour
{
	// Token: 0x14000002 RID: 2
	// (add) Token: 0x0600009A RID: 154 RVA: 0x000049E8 File Offset: 0x00002BE8
	// (remove) Token: 0x0600009B RID: 155 RVA: 0x00004A20 File Offset: 0x00002C20
	public event ObjectBounce.BounceEvent Bounced;

	// Token: 0x14000003 RID: 3
	// (add) Token: 0x0600009C RID: 156 RVA: 0x00004A58 File Offset: 0x00002C58
	// (remove) Token: 0x0600009D RID: 157 RVA: 0x00004A90 File Offset: 0x00002C90
	public event ObjectBounce.BounceEvent StartedMoving;

	// Token: 0x14000004 RID: 4
	// (add) Token: 0x0600009E RID: 158 RVA: 0x00004AC8 File Offset: 0x00002CC8
	// (remove) Token: 0x0600009F RID: 159 RVA: 0x00004B00 File Offset: 0x00002D00
	public event ObjectBounce.BounceEvent StoppedMoving;

	// Token: 0x060000A0 RID: 160 RVA: 0x00004B38 File Offset: 0x00002D38
	private void Start()
	{
		this.rb = base.GetComponent<Rigidbody2D>();
		this.audio = base.GetComponent<AudioSource>();
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.hasRB2D = (this.rb != null);
		if (this.sendFSMEvent)
		{
			this.fsm = base.GetComponent<PlayMakerFSM>();
		}
		this.started = true;
		ComponentSingleton<ObjectBounceCallbackHooks>.Instance.OnUpdate += this.OnUpdate;
		ComponentSingleton<ObjectBounceCallbackHooks>.Instance.OnFixedUpdate += this.OnFixedUpdate;
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x00004BC4 File Offset: 0x00002DC4
	private void OnEnable()
	{
		if (this.started)
		{
			ComponentSingleton<ObjectBounceCallbackHooks>.Instance.OnUpdate += this.OnUpdate;
			ComponentSingleton<ObjectBounceCallbackHooks>.Instance.OnFixedUpdate += this.OnFixedUpdate;
		}
		this.isMoving = false;
		this.bounces = 0;
		this.recycleTimer = 0f;
		this.bouncing = true;
		this.stoppedMovingTimer = 0f;
		this.enteredWater = false;
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x00004C37 File Offset: 0x00002E37
	private void OnDisable()
	{
		ComponentSingleton<ObjectBounceCallbackHooks>.Instance.OnUpdate -= this.OnUpdate;
		ComponentSingleton<ObjectBounceCallbackHooks>.Instance.OnFixedUpdate -= this.OnFixedUpdate;
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x00004C68 File Offset: 0x00002E68
	private void OnUpdate()
	{
		if (this.stoppedMovingTimer > 0f)
		{
			this.stoppedMovingTimer -= Time.deltaTime;
			if (this.stoppedMovingTimer <= 0f)
			{
				if (this.StoppedMoving != null)
				{
					this.StoppedMoving();
				}
				if (this.recycleAfterBounces > 0)
				{
					this.DropAndRecycle();
				}
			}
		}
		bool flag = this.isMoving;
		this.isMoving = (this.speed > 0.1f);
		if (!this.isMoving && flag)
		{
			this.stoppedMovingTimer = 0.5f;
		}
		else if (this.isMoving && !flag)
		{
			if (this.stoppedMovingTimer <= 0f && this.StartedMoving != null)
			{
				this.StartedMoving();
			}
			this.stoppedMovingTimer = 0f;
		}
		if (this.animTimer > 0f)
		{
			this.animTimer -= Time.deltaTime;
		}
		if (this.recycleTimer <= 0f)
		{
			return;
		}
		this.recycleTimer -= Time.deltaTime;
		if (this.recycleTimer > 0f)
		{
			return;
		}
		PolygonCollider2D component = base.GetComponent<PolygonCollider2D>();
		if (component)
		{
			component.enabled = true;
		}
		BoxCollider2D component2 = base.GetComponent<BoxCollider2D>();
		if (component2)
		{
			component2.enabled = true;
		}
		CircleCollider2D component3 = base.GetComponent<CircleCollider2D>();
		if (component3)
		{
			component3.enabled = true;
		}
		this.StartBounce();
		base.gameObject.Recycle();
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x00004DD0 File Offset: 0x00002FD0
	private void OnFixedUpdate()
	{
		if (!this.bouncing)
		{
			return;
		}
		Vector2 a = base.transform.position;
		this.velocity = a - this.lastPos;
		this.lastPos = a;
		if (!this.hasRB2D || !this.rb.IsAwake())
		{
			this.speed = 0f;
			return;
		}
		this.speed = this.rb.linearVelocity.magnitude;
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x00004E4C File Offset: 0x0000304C
	private void OnCollisionEnter2D(Collision2D col)
	{
		if (!this.hasRB2D || this.rb.isKinematic)
		{
			return;
		}
		if (!this.bouncing)
		{
			return;
		}
		if (this.speed >= this.speedThreshold)
		{
			if (Math.Abs(this.bounceFactor) > Mathf.Epsilon)
			{
				Vector3 inNormal = col.GetSafeContact().Normal;
				Vector3 normalized = Vector3.Reflect(this.velocity.normalized, inNormal).normalized;
				Vector2 vector = new Vector2(normalized.x, normalized.y) * (this.speed * (this.bounceFactor * Random.Range(0.8f, 1.2f)));
				if (this.doNotAffectX)
				{
					this.rb.linearVelocity = new Vector2(this.rb.linearVelocity.x, vector.y);
				}
				else if (this.doNotAffectY)
				{
					this.rb.linearVelocity = new Vector2(vector.x, this.rb.linearVelocity.y);
				}
				else
				{
					this.rb.linearVelocity = vector;
				}
			}
			if (!GameManager.IsWaitingForSceneReady && !this.enteredWater)
			{
				if (this.randomAudioClipTable)
				{
					this.randomAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
				}
				else if (this.playSound && Time.timeAsDouble >= this.playSoundCooldownTime)
				{
					this.playSoundCooldownTime = Time.timeAsDouble + (double)this.playSoundCooldown;
					this.chooser = Random.Range(1, 100);
					if (this.chooser <= this.chanceToPlay)
					{
						int num = Random.Range(0, this.clips.Length);
						AudioClip audioClip = this.clips[num];
						if (audioClip != null)
						{
							if (this.audio != null)
							{
								float pitch = Random.Range(this.pitchMin, this.pitchMax);
								this.audio.pitch = pitch;
								this.audio.PlayOneShot(audioClip);
							}
							else if (audioClip != null)
							{
								new AudioEvent
								{
									Clip = audioClip,
									PitchMin = this.pitchMin,
									PitchMax = this.pitchMax
								}.SpawnAndPlayOneShot(Audio.Default2DAudioSourcePrefab, base.transform.position, null);
							}
						}
					}
				}
			}
			if (this.playAnimationOnBounce && this.animTimer <= 0f)
			{
				this.animator.Play(this.animationName);
				this.animator.PlayFromFrame(0);
				this.animTimer = this.animPause;
			}
			if (this.sendFSMEvent && this.fsm)
			{
				this.fsm.SendEvent("BOUNCE");
			}
			if (this.Bounced != null)
			{
				this.Bounced();
			}
			if (this.recycleAfterBounces > 0)
			{
				this.bounces++;
				if (this.bounces >= this.recycleAfterBounces)
				{
					this.DropAndRecycle();
				}
			}
			this.OnBounced.Invoke();
			if (this.speed >= (float)this.heavyBounceSpeed)
			{
				this.OnHeavyBounce.Invoke();
				return;
			}
		}
		else if (this.recycleAfterBounces > 0)
		{
			this.DropAndRecycle();
		}
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x00005187 File Offset: 0x00003387
	public void StopBounce()
	{
		this.bouncing = false;
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x00005190 File Offset: 0x00003390
	public void StartBounce()
	{
		this.bouncing = true;
		this.lastPos = base.transform.position;
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x000051AF File Offset: 0x000033AF
	public void SetBounceFactor(float value)
	{
		this.bounceFactor = value;
	}

	// Token: 0x060000A9 RID: 169 RVA: 0x000051B8 File Offset: 0x000033B8
	public void SetBounceAnimation(bool set)
	{
		this.playAnimationOnBounce = set;
	}

	// Token: 0x060000AA RID: 170 RVA: 0x000051C4 File Offset: 0x000033C4
	public void DropAndRecycle()
	{
		this.StopBounce();
		PolygonCollider2D component = base.GetComponent<PolygonCollider2D>();
		if (component)
		{
			component.enabled = false;
		}
		BoxCollider2D component2 = base.GetComponent<BoxCollider2D>();
		if (component2)
		{
			component2.enabled = false;
		}
		CircleCollider2D component3 = base.GetComponent<CircleCollider2D>();
		if (component3)
		{
			component3.enabled = false;
		}
		this.recycleTimer = 1f;
	}

	// Token: 0x060000AB RID: 171 RVA: 0x00005224 File Offset: 0x00003424
	public void SetEnteredWater()
	{
		this.enteredWater = true;
	}

	// Token: 0x04000078 RID: 120
	[Tooltip("1.0 = full bounce, 0.5 = half bounce, >1 makes the bounce increase speed")]
	public float bounceFactor;

	// Token: 0x04000079 RID: 121
	[Tooltip("If object's speed is below this, don't bounce")]
	public float speedThreshold = 1f;

	// Token: 0x0400007A RID: 122
	public bool doNotAffectX;

	// Token: 0x0400007B RID: 123
	public bool doNotAffectY;

	// Token: 0x0400007C RID: 124
	public bool playSound;

	// Token: 0x0400007D RID: 125
	public float playSoundCooldown;

	// Token: 0x0400007E RID: 126
	[Range(0f, 100f)]
	public int chanceToPlay = 100;

	// Token: 0x0400007F RID: 127
	public AudioClip[] clips;

	// Token: 0x04000080 RID: 128
	public RandomAudioClipTable randomAudioClipTable;

	// Token: 0x04000081 RID: 129
	public float pitchMin = 1f;

	// Token: 0x04000082 RID: 130
	public float pitchMax = 1f;

	// Token: 0x04000083 RID: 131
	public bool playAnimationOnBounce;

	// Token: 0x04000084 RID: 132
	public string animationName;

	// Token: 0x04000085 RID: 133
	public float animPause = 0.5f;

	// Token: 0x04000086 RID: 134
	public bool sendFSMEvent;

	// Token: 0x04000087 RID: 135
	[Space]
	public int recycleAfterBounces;

	// Token: 0x04000088 RID: 136
	[Space]
	public int heavyBounceSpeed;

	// Token: 0x04000089 RID: 137
	[Space]
	public UnityEvent OnBounced;

	// Token: 0x0400008A RID: 138
	[Space]
	public UnityEvent OnHeavyBounce;

	// Token: 0x0400008B RID: 139
	private float speed;

	// Token: 0x0400008C RID: 140
	private float animTimer;

	// Token: 0x0400008D RID: 141
	private tk2dSpriteAnimator animator;

	// Token: 0x0400008E RID: 142
	private PlayMakerFSM fsm;

	// Token: 0x0400008F RID: 143
	private Vector2 velocity;

	// Token: 0x04000090 RID: 144
	private Vector2 lastPos;

	// Token: 0x04000091 RID: 145
	private Rigidbody2D rb;

	// Token: 0x04000092 RID: 146
	private AudioSource audio;

	// Token: 0x04000093 RID: 147
	private int chooser;

	// Token: 0x04000094 RID: 148
	private bool isMoving;

	// Token: 0x04000095 RID: 149
	private int bounces;

	// Token: 0x04000096 RID: 150
	private float stoppedMovingTimer;

	// Token: 0x04000097 RID: 151
	private double playSoundCooldownTime;

	// Token: 0x04000098 RID: 152
	private float recycleTimer;

	// Token: 0x04000099 RID: 153
	private bool enteredWater;

	// Token: 0x0400009A RID: 154
	private bool bouncing = true;

	// Token: 0x0400009B RID: 155
	private bool hasRB2D;

	// Token: 0x0400009C RID: 156
	private bool started;

	// Token: 0x020013BD RID: 5053
	// (Invoke) Token: 0x06008153 RID: 33107
	public delegate void BounceEvent();
}
