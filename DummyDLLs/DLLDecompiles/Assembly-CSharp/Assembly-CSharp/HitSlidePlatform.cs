using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000504 RID: 1284
public class HitSlidePlatform : MonoBehaviour
{
	// Token: 0x06002DEF RID: 11759 RVA: 0x000C9367 File Offset: 0x000C7567
	private void OnValidate()
	{
		if (this.tinker)
		{
			this.tinkers = new TinkEffect[]
			{
				this.tinker
			};
			this.tinker = null;
		}
	}

	// Token: 0x06002DF0 RID: 11760 RVA: 0x000C9394 File Offset: 0x000C7594
	private void Awake()
	{
		this.OnValidate();
		foreach (HitSlidePlatformNode hitSlidePlatformNode in this.nodes)
		{
			if (hitSlidePlatformNode.transform.IsChildOf(base.transform))
			{
				hitSlidePlatformNode.transform.SetParent(null, true);
			}
		}
		foreach (TinkEffect tinkEffect in this.tinkers)
		{
			if (tinkEffect)
			{
				tinkEffect.HitInDirection += this.OnHit;
			}
		}
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out int value)
			{
				value = this.currentNodeIndex;
			};
			this.persistent.OnSetSaveState += delegate(int value)
			{
				if (value < 0 && this.initialNode)
				{
					this.SetAtNode(this.initialNode);
					return;
				}
				this.SetAtNode(value);
			};
		}
		if (this.enemyKiller)
		{
			this.enemyKiller.gameObject.SetActive(false);
		}
		if (this.heroDamager)
		{
			this.heroDamageInitialPosition = this.heroDamager.transform.localPosition;
			this.heroDamager.gameObject.SetActive(false);
		}
		if (this.initialNode)
		{
			this.initialNode.transform.SetParent(null, true);
			this.currentNodeIndex = -1;
			this.SetAtNode(this.initialNode);
			return;
		}
		this.SetAtNode(-1);
	}

	// Token: 0x06002DF1 RID: 11761 RVA: 0x000C9508 File Offset: 0x000C7708
	private void Start()
	{
		BoxCollider2D component = this.terrainCollider.GetComponent<BoxCollider2D>();
		Vector2 size = component.size;
		Vector2 offset = component.offset;
		float x = size.x;
		float y = size.y;
		float x2 = offset.x;
		float y2 = offset.y;
		float num = x / 2f;
		float num2 = y / 2f;
		this.colliderOffsetL = -num + x2;
		this.colliderOffsetR = num + x2;
		this.colliderOffsetD = -num2 + y2;
		this.colliderOffsetU = num2 + y2;
	}

	// Token: 0x06002DF2 RID: 11762 RVA: 0x000C9588 File Offset: 0x000C7788
	private void SetAtNode(int nodeIndex)
	{
		if (nodeIndex < 0)
		{
			float num = float.MaxValue;
			Vector2 a = base.transform.position;
			for (int i = 0; i < this.nodes.Count; i++)
			{
				HitSlidePlatformNode hitSlidePlatformNode = this.nodes[i];
				if (hitSlidePlatformNode && hitSlidePlatformNode.gameObject.activeSelf)
				{
					float num2 = Vector2.Distance(a, hitSlidePlatformNode.transform.position);
					if (num2 < num)
					{
						num = num2;
						nodeIndex = i;
					}
				}
			}
		}
		this.currentNodeIndex = nodeIndex;
		this.SetAtNode(this.nodes[this.currentNodeIndex]);
	}

	// Token: 0x06002DF3 RID: 11763 RVA: 0x000C9629 File Offset: 0x000C7829
	private void SetAtNode(HitSlidePlatformNode node)
	{
		this.currentNode = node;
		base.transform.SetPosition2D(this.currentNode.transform.position);
	}

	// Token: 0x06002DF4 RID: 11764 RVA: 0x000C9654 File Offset: 0x000C7854
	private void OnHit(GameObject source, HitInstance.HitDirection hitDirection)
	{
		if (this.moveRoutine != null)
		{
			return;
		}
		if (this.hitParticles)
		{
			this.hitParticles.PlayParticleSystems();
		}
		if (this.jitter)
		{
			this.jitter.StartTimedJitter();
		}
		if (this.currentNode.IsEndNode)
		{
			return;
		}
		if (this.directionFailMoveRoutine != null)
		{
			this.jitter.transform.localPosition = this.initialGraphicOffset;
			base.StopCoroutine(this.directionFailMoveRoutine);
			this.directionFailMoveRoutine = null;
		}
		Vector3 position = source.gameObject.transform.position;
		HitInstance.HitDirection validDirection = this.GetValidDirection(position, hitDirection);
		if (validDirection < HitInstance.HitDirection.Left)
		{
			validDirection = this.GetValidDirection(source.transform.root.position, hitDirection);
		}
		float angle = DirectionUtils.GetAngle(validDirection);
		Vector2 vector = new Vector2(Mathf.Cos(angle * 0.017453292f), Mathf.Sin(angle * 0.017453292f));
		HitSlidePlatformNode connectedNode = this.currentNode.GetConnectedNode(validDirection);
		if (!connectedNode)
		{
			this.directionFailMoveRoutine = base.StartCoroutine(this.DirectionFailMove(vector));
			return;
		}
		if (this.trailParticles)
		{
			this.trailParticles.Play(true);
		}
		if (this.enemyKiller)
		{
			this.enemyKiller.direction = angle;
			this.enemyKiller.gameObject.SetActive(true);
		}
		if (this.heroDamager)
		{
			Vector2 b = this.heroDamagerDirectionalOffset.MultiplyElements(vector);
			this.heroDamager.transform.SetLocalPosition2D(this.heroDamageInitialPosition + b);
			this.heroDamager.gameObject.SetActive(true);
		}
		if (this.audioEventPlayer)
		{
			this.hitSound.SpawnAndPlayOneShot(this.audioEventPlayer, base.transform.position, null);
		}
		if (this.slideLoopSource)
		{
			this.slideLoopSource.Play();
		}
		this.currentNodeIndex = this.nodes.IndexOf(connectedNode);
		this.currentNode = connectedNode;
		this.moveRoutine = null;
		this.moveStartPosition = base.transform.position;
		this.moveEndPosition = connectedNode.transform.position;
		this.moveDuration = Vector2.Distance(this.moveStartPosition, this.moveEndPosition) / this.moveSpeed;
		this.moveRoutine = this.StartTimerRoutine(0f, this.moveDuration, delegate(float t)
		{
			base.transform.SetPosition2D(Vector2.Lerp(this.moveStartPosition, this.moveEndPosition, t));
		}, null, new Action(this.OnMoveEnd), false);
		if (!this.didFirstHit)
		{
			if (this.FirstHit != null)
			{
				this.FirstHit.Invoke();
			}
			this.didFirstHit = true;
		}
	}

	// Token: 0x06002DF5 RID: 11765 RVA: 0x000C98F0 File Offset: 0x000C7AF0
	private HitInstance.HitDirection GetValidDirection(Vector3 sourcePos, HitInstance.HitDirection hitDirection)
	{
		Vector3 position = base.transform.position;
		bool flag = sourcePos.y <= position.y + this.colliderOffsetD;
		bool flag2 = sourcePos.y >= position.y + this.colliderOffsetU;
		bool flag3 = sourcePos.x <= position.x + this.colliderOffsetL;
		bool flag4 = sourcePos.x >= position.x + this.colliderOffsetR;
		switch (hitDirection)
		{
		case HitInstance.HitDirection.Left:
			if (flag4)
			{
				return hitDirection;
			}
			break;
		case HitInstance.HitDirection.Right:
			if (flag3)
			{
				return hitDirection;
			}
			break;
		case HitInstance.HitDirection.Up:
			if (flag)
			{
				return hitDirection;
			}
			break;
		case HitInstance.HitDirection.Down:
			if (flag2)
			{
				return hitDirection;
			}
			break;
		}
		if (flag)
		{
			return HitInstance.HitDirection.Up;
		}
		if (flag2)
		{
			return HitInstance.HitDirection.Down;
		}
		if (flag3)
		{
			return HitInstance.HitDirection.Right;
		}
		if (flag4)
		{
			return HitInstance.HitDirection.Left;
		}
		Debug.LogError("Couldn't determine valid hit direction!");
		return (HitInstance.HitDirection)(-1);
	}

	// Token: 0x06002DF6 RID: 11766 RVA: 0x000C99BC File Offset: 0x000C7BBC
	private void OnMoveEnd()
	{
		this.moveRoutine = null;
		if (this.slamParticles)
		{
			this.slamParticles.PlayParticleSystems();
		}
		if (this.jitter)
		{
			this.jitter.StartTimedJitter();
		}
		if (this.trailParticles)
		{
			this.trailParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
		this.arriveShake.DoShake(this, true);
		if (this.enemyKiller)
		{
			this.enemyKiller.gameObject.SetActive(false);
		}
		if (this.heroDamager)
		{
			this.heroDamager.gameObject.SetActive(false);
		}
		this.arriveSound.SpawnAndPlayOneShot(base.transform.position, null);
		if (this.slideLoopSource)
		{
			this.slideLoopSource.Stop();
		}
	}

	// Token: 0x06002DF7 RID: 11767 RVA: 0x000C9A93 File Offset: 0x000C7C93
	private IEnumerator DirectionFailMove(Vector2 hitDirectionVector)
	{
		Transform trans = this.jitter.transform;
		this.initialGraphicOffset = trans.localPosition;
		if (this.audioEventPlayer)
		{
			this.arriveSound.SpawnAndPlayOneShot(this.audioEventPlayer, base.transform.position, null);
		}
		for (float elapsed = 0f; elapsed < this.directionFailMoveDuration; elapsed += Time.deltaTime)
		{
			float time = elapsed / this.directionFailMoveDuration;
			Vector2 v = hitDirectionVector * (this.directionFailMove.Evaluate(time) * this.directionFailMoveMagnitude);
			trans.localPosition = this.initialGraphicOffset + v;
			yield return null;
		}
		trans.localPosition = this.initialGraphicOffset;
		this.directionFailMoveRoutine = null;
		yield break;
	}

	// Token: 0x04003008 RID: 12296
	[SerializeField]
	private PersistentIntItem persistent;

	// Token: 0x04003009 RID: 12297
	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private TinkEffect tinker;

	// Token: 0x0400300A RID: 12298
	[SerializeField]
	private TinkEffect[] tinkers;

	// Token: 0x0400300B RID: 12299
	[SerializeField]
	private float moveSpeed;

	// Token: 0x0400300C RID: 12300
	[SerializeField]
	private CameraShakeTarget arriveShake;

	// Token: 0x0400300D RID: 12301
	[SerializeField]
	private PlayParticleEffects hitParticles;

	// Token: 0x0400300E RID: 12302
	[SerializeField]
	private PlayParticleEffects slamParticles;

	// Token: 0x0400300F RID: 12303
	[SerializeField]
	private ParticleSystem trailParticles;

	// Token: 0x04003010 RID: 12304
	[SerializeField]
	private JitterSelfForTime jitter;

	// Token: 0x04003011 RID: 12305
	[SerializeField]
	private DamageEnemies enemyKiller;

	// Token: 0x04003012 RID: 12306
	[SerializeField]
	private DamageHero heroDamager;

	// Token: 0x04003013 RID: 12307
	[SerializeField]
	private Vector2 heroDamagerDirectionalOffset;

	// Token: 0x04003014 RID: 12308
	[SerializeField]
	private GameObject terrainCollider;

	// Token: 0x04003015 RID: 12309
	[SerializeField]
	private AudioSource slideLoopSource;

	// Token: 0x04003016 RID: 12310
	[SerializeField]
	private AudioEvent hitSound;

	// Token: 0x04003017 RID: 12311
	[SerializeField]
	private AudioEvent arriveSound;

	// Token: 0x04003018 RID: 12312
	[SerializeField]
	private AudioSource audioEventPlayer;

	// Token: 0x04003019 RID: 12313
	[SerializeField]
	private AnimationCurve directionFailMove;

	// Token: 0x0400301A RID: 12314
	[SerializeField]
	private float directionFailMoveDuration;

	// Token: 0x0400301B RID: 12315
	[SerializeField]
	private float directionFailMoveMagnitude;

	// Token: 0x0400301C RID: 12316
	[Space]
	[SerializeField]
	private HitSlidePlatformNode initialNode;

	// Token: 0x0400301D RID: 12317
	[SerializeField]
	private List<HitSlidePlatformNode> nodes;

	// Token: 0x0400301E RID: 12318
	private Vector2 heroDamageInitialPosition;

	// Token: 0x0400301F RID: 12319
	private int currentNodeIndex;

	// Token: 0x04003020 RID: 12320
	private HitSlidePlatformNode currentNode;

	// Token: 0x04003021 RID: 12321
	private Coroutine moveRoutine;

	// Token: 0x04003022 RID: 12322
	private float moveDuration;

	// Token: 0x04003023 RID: 12323
	private Vector2 moveStartPosition;

	// Token: 0x04003024 RID: 12324
	private Vector2 moveEndPosition;

	// Token: 0x04003025 RID: 12325
	private Coroutine directionFailMoveRoutine;

	// Token: 0x04003026 RID: 12326
	private Vector3 initialGraphicOffset;

	// Token: 0x04003027 RID: 12327
	private float colliderOffsetL;

	// Token: 0x04003028 RID: 12328
	private float colliderOffsetR;

	// Token: 0x04003029 RID: 12329
	private float colliderOffsetU;

	// Token: 0x0400302A RID: 12330
	private float colliderOffsetD;

	// Token: 0x0400302B RID: 12331
	private bool didFirstHit;

	// Token: 0x0400302C RID: 12332
	public UnityEvent FirstHit;
}
