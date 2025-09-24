using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004C9 RID: 1225
public class CrankPlat : MonoBehaviour, HeroPlatformStick.IMoveHooks
{
	// Token: 0x1400008B RID: 139
	// (add) Token: 0x06002C10 RID: 11280 RVA: 0x000C10C8 File Offset: 0x000BF2C8
	// (remove) Token: 0x06002C11 RID: 11281 RVA: 0x000C1100 File Offset: 0x000BF300
	public event Action OnStartMove;

	// Token: 0x1400008C RID: 140
	// (add) Token: 0x06002C12 RID: 11282 RVA: 0x000C1138 File Offset: 0x000BF338
	// (remove) Token: 0x06002C13 RID: 11283 RVA: 0x000C1170 File Offset: 0x000BF370
	public event Action OnStopMove;

	// Token: 0x06002C14 RID: 11284 RVA: 0x000C11A5 File Offset: 0x000BF3A5
	private void OnDrawGizmos()
	{
		if (this.endPoint)
		{
			Gizmos.DrawLine(base.transform.position, this.endPoint.position);
		}
	}

	// Token: 0x06002C15 RID: 11285 RVA: 0x000C11D0 File Offset: 0x000BF3D0
	private void Awake()
	{
		this.startPos = base.transform.position;
		this.endPos = this.endPoint.position;
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out bool value)
			{
				value = this.isComplete;
			};
			this.persistent.OnSetSaveState += delegate(bool value)
			{
				this.isComplete = value;
				if (this.isComplete)
				{
					this.SetComplete();
					this.UpdatePos();
				}
			};
		}
		if (this.crankHit)
		{
			this.crankHit.WasHitDirectional += delegate(HitInstance.HitDirection direction)
			{
				if (direction != HitInstance.HitDirection.Left)
				{
					if (direction != HitInstance.HitDirection.Right)
					{
						HeroController instance = HeroController.instance;
						this.crankHitDir = -Mathf.Sign(base.transform.position.x - instance.transform.position.x);
					}
					else
					{
						this.crankHitDir = -1f;
					}
				}
				else
				{
					this.crankHitDir = 1f;
				}
				this.DoHit();
			};
		}
	}

	// Token: 0x06002C16 RID: 11286 RVA: 0x000C1268 File Offset: 0x000BF468
	public void DoHit()
	{
		if (this.isComplete)
		{
			return;
		}
		if (this.speed < 0f)
		{
			this.speed = 0f;
		}
		this.speed += this.hitForce;
		this.PlayAudioEventOnSource(this.riseAudio);
		if (this.hitRoutine == null)
		{
			this.hitRoutine = base.StartCoroutine(this.HitRoutine());
		}
	}

	// Token: 0x06002C17 RID: 11287 RVA: 0x000C12D0 File Offset: 0x000BF4D0
	private void PlayAudioEventOnSource(AudioEventRandom audioEvent)
	{
		this.playOnSource.Stop();
		this.playOnSource.clip = audioEvent.GetClip();
		this.playOnSource.pitch = audioEvent.SelectPitch();
		this.playOnSource.volume = audioEvent.Volume;
		this.playOnSource.Play();
	}

	// Token: 0x06002C18 RID: 11288 RVA: 0x000C1328 File Offset: 0x000BF528
	private IEnumerator HitRoutine()
	{
		Action onStartMove = this.OnStartMove;
		if (onStartMove != null)
		{
			onStartMove();
		}
		float distance = Vector2.Distance(this.startPos, this.endPos);
		bool flag;
		do
		{
			float deltaTime = Time.deltaTime;
			float num = Mathf.Clamp(this.speed, -this.maxReturnSpeed, this.maxHitSpeed);
			this.posT += num / distance * deltaTime;
			float num2 = this.speed;
			this.speed -= this.returnForce * deltaTime;
			if (num2 >= 0f && this.speed < 0f)
			{
				this.PlayAudioEventOnSource(this.fallAudio);
			}
			this.UpdatePos();
			yield return null;
			flag = (this.posT >= 1f - Mathf.Epsilon);
		}
		while (!flag && this.posT > Mathf.Epsilon);
		this.UpdatePos();
		this.playOnSource.Stop();
		if (flag)
		{
			this.PlayAudioEventOnSource(this.endAudio);
			this.SetComplete();
			this.OnCompleted.Invoke();
		}
		else
		{
			this.PlayAudioEventOnSource(this.returnedAudio);
			this.posT = 0f;
			this.OnReturned.Invoke();
		}
		Action onStopMove = this.OnStopMove;
		if (onStopMove != null)
		{
			onStopMove();
		}
		this.speed = 0f;
		if (flag && this.returnOnHeroBelow)
		{
			HeroController hc = HeroController.instance;
			Vector3 pos = base.transform.position;
			pos.y -= 5f;
			while (hc.transform.position.y > pos.y)
			{
				yield return null;
			}
			this.posT -= 0.01f;
			this.isComplete = false;
			this.SetCrankEnabled(true);
			this.hitRoutine = base.StartCoroutine(this.HitRoutine());
			yield break;
		}
		this.hitRoutine = null;
		yield break;
	}

	// Token: 0x06002C19 RID: 11289 RVA: 0x000C1338 File Offset: 0x000BF538
	private void UpdatePos()
	{
		Vector2 vector = Vector2.Lerp(this.startPos, this.endPos, this.posT);
		base.transform.SetPosition2D(vector);
		if (this.isComplete)
		{
			return;
		}
		if (this.crank)
		{
			this.crank.SetRotation2D(Vector2.Distance(vector, this.startPos) * this.distanceRotation * this.crankHitDir);
		}
	}

	// Token: 0x06002C1A RID: 11290 RVA: 0x000C13A4 File Offset: 0x000BF5A4
	private void SetComplete()
	{
		this.posT = 1f;
		this.isComplete = true;
		if (this.crank)
		{
			this.crank.SetLocalPositionZ(this.crankInertZ);
			this.crank.SetLocalRotation2D(this.crankEndRotation);
		}
		this.SetCrankEnabled(false);
	}

	// Token: 0x06002C1B RID: 11291 RVA: 0x000C13FC File Offset: 0x000BF5FC
	private void SetCrankEnabled(bool value)
	{
		if (!this.crankHit)
		{
			return;
		}
		Collider2D component = this.crankHit.GetComponent<Collider2D>();
		if (component)
		{
			component.enabled = value;
		}
	}

	// Token: 0x06002C1C RID: 11292 RVA: 0x000C1432 File Offset: 0x000BF632
	public void AddMoveHooks(Action onStartMove, Action onStopMove)
	{
		this.OnStartMove += onStartMove;
		this.OnStopMove += onStopMove;
	}

	// Token: 0x04002D70 RID: 11632
	[SerializeField]
	private PersistentBoolItem persistent;

	// Token: 0x04002D71 RID: 11633
	[SerializeField]
	private bool returnOnHeroBelow;

	// Token: 0x04002D72 RID: 11634
	[Space]
	[SerializeField]
	private Transform crank;

	// Token: 0x04002D73 RID: 11635
	[SerializeField]
	private float distanceRotation;

	// Token: 0x04002D74 RID: 11636
	[SerializeField]
	private HitResponse crankHit;

	// Token: 0x04002D75 RID: 11637
	[SerializeField]
	private float crankInertZ;

	// Token: 0x04002D76 RID: 11638
	[SerializeField]
	private float crankEndRotation;

	// Token: 0x04002D77 RID: 11639
	[Space]
	[SerializeField]
	private Transform endPoint;

	// Token: 0x04002D78 RID: 11640
	[SerializeField]
	private float hitForce;

	// Token: 0x04002D79 RID: 11641
	[SerializeField]
	private float maxHitSpeed;

	// Token: 0x04002D7A RID: 11642
	[SerializeField]
	private float returnForce;

	// Token: 0x04002D7B RID: 11643
	[SerializeField]
	private float maxReturnSpeed;

	// Token: 0x04002D7C RID: 11644
	[Space]
	[SerializeField]
	private AudioEventRandom riseAudio;

	// Token: 0x04002D7D RID: 11645
	[SerializeField]
	private AudioEventRandom endAudio;

	// Token: 0x04002D7E RID: 11646
	[SerializeField]
	private AudioEventRandom fallAudio;

	// Token: 0x04002D7F RID: 11647
	[SerializeField]
	private AudioEventRandom returnedAudio;

	// Token: 0x04002D80 RID: 11648
	[SerializeField]
	private AudioSource playOnSource;

	// Token: 0x04002D81 RID: 11649
	[Space]
	public UnityEvent OnReturned;

	// Token: 0x04002D82 RID: 11650
	public UnityEvent OnCompleted;

	// Token: 0x04002D83 RID: 11651
	private bool isComplete;

	// Token: 0x04002D84 RID: 11652
	private Vector2 startPos;

	// Token: 0x04002D85 RID: 11653
	private Vector2 endPos;

	// Token: 0x04002D86 RID: 11654
	private float posT;

	// Token: 0x04002D87 RID: 11655
	private float speed;

	// Token: 0x04002D88 RID: 11656
	private Coroutine hitRoutine;

	// Token: 0x04002D89 RID: 11657
	private float crankHitDir;
}
