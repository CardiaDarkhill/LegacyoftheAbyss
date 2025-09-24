using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000518 RID: 1304
public abstract class LookAnimNPC : MonoBehaviour
{
	// Token: 0x17000531 RID: 1329
	// (get) Token: 0x06002EA3 RID: 11939 RVA: 0x000CD84F File Offset: 0x000CBA4F
	protected bool DefaultLeft
	{
		get
		{
			if (this.facingIgnoresScale && base.transform.lossyScale.x < 0f)
			{
				return !this.defaultLeft;
			}
			return this.defaultLeft;
		}
	}

	// Token: 0x17000532 RID: 1330
	// (get) Token: 0x06002EA4 RID: 11940 RVA: 0x000CD880 File Offset: 0x000CBA80
	// (set) Token: 0x06002EA5 RID: 11941 RVA: 0x000CD888 File Offset: 0x000CBA88
	public Transform TargetOverride { get; set; }

	// Token: 0x17000533 RID: 1331
	// (get) Token: 0x06002EA6 RID: 11942 RVA: 0x000CD894 File Offset: 0x000CBA94
	public Transform CurrentTarget
	{
		get
		{
			if (this.TargetOverride)
			{
				return this.TargetOverride;
			}
			if (this.target)
			{
				return this.target;
			}
			if (this.turnOnInteract && this.startedConversationCount > 0)
			{
				return HeroController.instance.transform;
			}
			return null;
		}
	}

	// Token: 0x17000534 RID: 1332
	// (get) Token: 0x06002EA7 RID: 11943 RVA: 0x000CD8E6 File Offset: 0x000CBAE6
	// (set) Token: 0x06002EA8 RID: 11944 RVA: 0x000CD8EE File Offset: 0x000CBAEE
	public LookAnimNPC.AnimState State
	{
		get
		{
			return this.state;
		}
		protected set
		{
			this.PreviousState = this.state;
			this.state = value;
		}
	}

	// Token: 0x17000535 RID: 1333
	// (get) Token: 0x06002EA9 RID: 11945 RVA: 0x000CD903 File Offset: 0x000CBB03
	// (set) Token: 0x06002EAA RID: 11946 RVA: 0x000CD90B File Offset: 0x000CBB0B
	public LookAnimNPC.AnimState PreviousState { get; private set; }

	// Token: 0x17000536 RID: 1334
	// (get) Token: 0x06002EAB RID: 11947 RVA: 0x000CD914 File Offset: 0x000CBB14
	// (set) Token: 0x06002EAC RID: 11948 RVA: 0x000CD91C File Offset: 0x000CBB1C
	public bool WasFacingLeft { get; private set; }

	// Token: 0x17000537 RID: 1335
	// (get) Token: 0x06002EAD RID: 11949 RVA: 0x000CD925 File Offset: 0x000CBB25
	public bool IsNPCInConversation
	{
		get
		{
			return this.startingConversationCount > 0;
		}
	}

	// Token: 0x17000538 RID: 1336
	// (get) Token: 0x06002EAE RID: 11950 RVA: 0x000CD930 File Offset: 0x000CBB30
	public bool IsNPCTalking
	{
		get
		{
			return this.IsNPCInConversation && this.isCurrentLineNpc;
		}
	}

	// Token: 0x17000539 RID: 1337
	// (get) Token: 0x06002EAF RID: 11951 RVA: 0x000CD942 File Offset: 0x000CBB42
	public bool IsTurning
	{
		get
		{
			return this.state == LookAnimNPC.AnimState.TurningLeft || this.state == LookAnimNPC.AnimState.TurningRight;
		}
	}

	// Token: 0x1700053A RID: 1338
	// (get) Token: 0x06002EB0 RID: 11952 RVA: 0x000CD958 File Offset: 0x000CBB58
	// (set) Token: 0x06002EB1 RID: 11953 RVA: 0x000CD960 File Offset: 0x000CBB60
	public int CurrentLineNumber { get; private set; }

	// Token: 0x1700053B RID: 1339
	// (get) Token: 0x06002EB2 RID: 11954 RVA: 0x000CD969 File Offset: 0x000CBB69
	// (set) Token: 0x06002EB3 RID: 11955 RVA: 0x000CD971 File Offset: 0x000CBB71
	public bool ForceShouldTurnChecking { get; set; }

	// Token: 0x06002EB4 RID: 11956 RVA: 0x000CD97A File Offset: 0x000CBB7A
	private bool? IsFsmBoolValid(string boolName)
	{
		return this.restAnimFSM.IsVariableValid(boolName, true);
	}

	// Token: 0x06002EB5 RID: 11957 RVA: 0x000CD989 File Offset: 0x000CBB89
	private bool DoesIdleTurn()
	{
		return (!string.IsNullOrEmpty(this.leftAnim) || !string.IsNullOrEmpty(this.turnLeftAnim)) && (!string.IsNullOrEmpty(this.rightAnim) || !string.IsNullOrEmpty(this.turnRightAnim));
	}

	// Token: 0x06002EB6 RID: 11958 RVA: 0x000CD9C4 File Offset: 0x000CBBC4
	protected virtual void OnValidate()
	{
		if (!string.IsNullOrEmpty(this.talkLeftAnim))
		{
			this.talkLeftAnims = new LookAnimNPC.TalkAnims
			{
				Talk = this.talkLeftAnim,
				Listen = this.leftAnim
			};
			this.talkLeftAnim = null;
		}
		if (!string.IsNullOrEmpty(this.talkRightAnim))
		{
			this.talkRightAnims = new LookAnimNPC.TalkAnims
			{
				Talk = this.talkRightAnim,
				Listen = this.rightAnim
			};
			this.talkRightAnim = null;
		}
		if (this.flipAfterTurn)
		{
			this.turnFlipType = LookAnimNPC.TurnFlipTypes.AfterTurn;
			this.flipAfterTurn = false;
		}
	}

	// Token: 0x06002EB7 RID: 11959 RVA: 0x000CDA61 File Offset: 0x000CBC61
	protected virtual void Awake()
	{
		this.OnValidate();
		this.noiseResponder = (base.GetComponent<NoiseResponder>() ?? base.gameObject.AddComponent<NoiseResponder>());
	}

	// Token: 0x06002EB8 RID: 11960 RVA: 0x000CDA84 File Offset: 0x000CBC84
	private void OnEnable()
	{
		this.noiseResponder.NoiseStarted += this.OnNoiseStarted;
		if (this.isTryingToRest)
		{
			if (this.talkRoutine != null)
			{
				base.StopCoroutine(this.talkRoutine);
				this.talkRoutine = null;
			}
			this.talkRoutine = base.StartCoroutine(this.Rest());
		}
	}

	// Token: 0x06002EB9 RID: 11961 RVA: 0x000CDADD File Offset: 0x000CBCDD
	private void OnDisable()
	{
		this.noiseResponder.NoiseStarted -= this.OnNoiseStarted;
		this.talkRoutine = null;
	}

	// Token: 0x06002EBA RID: 11962 RVA: 0x000CDB00 File Offset: 0x000CBD00
	private void Start()
	{
		if (this.limitZ > 0f && Mathf.Abs(base.transform.position.z - 0.004f) > this.limitZ)
		{
			Object.Destroy(this);
			return;
		}
		this.faceLeft = this.DefaultLeft;
		this.WasFacingLeft = this.GetWasFacingLeft();
		this.lastTalkedLeft = this.WasFacingLeft;
		if (this.State != LookAnimNPC.AnimState.Disabled)
		{
			if (this.WasFacingLeft)
			{
				if (!string.IsNullOrEmpty(this.leftAnim))
				{
					this.PlayAnim(this.leftAnim);
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(this.rightAnim))
				{
					this.PlayAnim(this.rightAnim);
				}
				this.State = LookAnimNPC.AnimState.Right;
			}
		}
		bool flag = false;
		if (this.DoesIdleTurn())
		{
			if (this.enterDetector)
			{
				flag = true;
				this.enterDetector.OnTriggerEntered += delegate(Collider2D col, GameObject _)
				{
					this.TargetEntered(col.gameObject);
				};
			}
			TriggerEnterEvent triggerEnterEvent = this.exitDetector ? this.exitDetector : this.enterDetector;
			if (triggerEnterEvent)
			{
				flag = true;
				triggerEnterEvent.OnTriggerExited += delegate(Collider2D _, GameObject _)
				{
					this.StartExitDelay();
				};
				triggerEnterEvent.DelayUpdateGrounded = true;
			}
		}
		if (!flag && !this.turnOnInteract)
		{
			this.target = HeroController.instance.transform;
		}
		List<NPCControlBase> list = new List<NPCControlBase>(this.extraConvoTrackers.Length + 1);
		if (this.npcControl)
		{
			list.Add(this.npcControl);
		}
		foreach (NPCControlBase npccontrolBase in this.extraConvoTrackers)
		{
			if (npccontrolBase)
			{
				list.Add(npccontrolBase);
			}
		}
		foreach (NPCControlBase npccontrolBase2 in list)
		{
			if (npccontrolBase2)
			{
				npccontrolBase2.StartingDialogue += delegate()
				{
					this.startingConversationCount++;
					if (this.startingConversationCount == 1)
					{
						this.CurrentLineNumber = 0;
					}
				};
				npccontrolBase2.StartedDialogue += delegate()
				{
					this.isCurrentLineNpc = false;
					this.preventMoveAttention = false;
					this.startedConversationCount++;
					if (this.State == LookAnimNPC.AnimState.Disabled)
					{
						return;
					}
					if (!base.isActiveAndEnabled)
					{
						return;
					}
					if (this.State != LookAnimNPC.AnimState.Resting && !this.turnOnInteract)
					{
						this.StartTalk();
					}
				};
				npccontrolBase2.EndedDialogue += delegate()
				{
					if (this.turnOnInteract)
					{
						this.StartExitDelay();
					}
					this.startingConversationCount--;
					this.startedConversationCount--;
				};
				npccontrolBase2.StartedNewLine += delegate(DialogueBox.DialogueLine line)
				{
					this.isCurrentLineNpc = line.IsNpcEvent(this.talkingPageEvent);
					int currentLineNumber = this.CurrentLineNumber;
					this.CurrentLineNumber = currentLineNumber + 1;
				};
			}
		}
		if (this.State == LookAnimNPC.AnimState.Disabled)
		{
			return;
		}
		if (this.restAnimFSM)
		{
			this.ResetRestTimer();
			this.isTryingToRest = true;
			this.talkRoutine = base.StartCoroutine(this.Rest());
		}
		else
		{
			bool flag2 = this.DefaultLeft;
			if (base.transform.lossyScale.x < 0f)
			{
				flag2 = !flag2;
			}
			this.State = (flag2 ? LookAnimNPC.AnimState.Left : LookAnimNPC.AnimState.Right);
			this.PlayAnim(flag2 ? this.leftAnim : this.rightAnim);
			this.faceLeft = (this.WasFacingLeft = flag2);
		}
		if (this.waitForHeroInPosition)
		{
			LookAnimNPC.<>c__DisplayClass101_0 CS$<>8__locals1 = new LookAnimNPC.<>c__DisplayClass101_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.hc = HeroController.instance;
			if (CS$<>8__locals1.hc != null && !CS$<>8__locals1.hc.isHeroInPosition)
			{
				CS$<>8__locals1.hc.heroInPosition += CS$<>8__locals1.<Start>g__OnHeroInPosition|6;
				this.waitingForHero = true;
			}
		}
	}

	// Token: 0x06002EBB RID: 11963 RVA: 0x000CDE20 File Offset: 0x000CC020
	private void OnDrawGizmosSelected()
	{
		Vector3 position = base.transform.position;
		float? z = new float?(0f);
		Gizmos.DrawWireSphere(position.Where(null, null, z) + new Vector3(this.centreOffset, 0f, 0f), 0.25f);
	}

	// Token: 0x06002EBC RID: 11964 RVA: 0x000CDE80 File Offset: 0x000CC080
	private void Update()
	{
		if (this.waitingForHero)
		{
			return;
		}
		if (this.waitingForBench)
		{
			if (PlayerData.instance.atBench)
			{
				return;
			}
			this.waitingForBench = false;
			this.skipNextDelay = true;
		}
		if (this.disabled)
		{
			return;
		}
		if (this.target && this.keepTargetUntil > (double)Mathf.Epsilon && Time.timeAsDouble >= this.keepTargetUntil)
		{
			this.target = null;
		}
		if (this.State < LookAnimNPC.AnimState.Talking)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (!this.isTurning && !this.justFinishedTurning)
			{
				this.WasFacingLeft = this.GetWasFacingLeft();
				this.faceLeft = this.ShouldFaceLeft(false);
				bool flag4 = this.faceLeft;
				if (this.WasFacingLeft != this.faceLeft)
				{
					if (this.skipNextDelay)
					{
						this.skipNextDelay = false;
					}
					else if (this.turnDelay <= 0f)
					{
						this.turnDelay = Random.Range(this.minReactDelay, this.maxReactDelay);
					}
				}
				if (this.turnDelay > 0f)
				{
					this.turnDelay -= Time.deltaTime;
					if (this.turnDelay > 0f)
					{
						flag4 = !flag4;
					}
				}
				LookAnimNPC.AnimState animState = this.State;
				if (animState != LookAnimNPC.AnimState.Left)
				{
					if (animState == LookAnimNPC.AnimState.Right)
					{
						if (flag4)
						{
							this.State = LookAnimNPC.AnimState.TurningLeft;
							this.isTurning = true;
							if (!string.IsNullOrEmpty(this.turnLeftAnim))
							{
								this.PlayAnim(this.turnLeftAnim);
								this.waitingForTurnStart = this.turnLeftAnim;
								flag2 = true;
							}
							else
							{
								flag3 = true;
							}
						}
						flag = true;
					}
				}
				else
				{
					if (!flag4)
					{
						this.State = LookAnimNPC.AnimState.TurningRight;
						this.isTurning = true;
						if (!string.IsNullOrEmpty(this.turnRightAnim))
						{
							this.PlayAnim(this.turnRightAnim);
							this.waitingForTurnStart = this.turnRightAnim;
							flag2 = true;
						}
						else
						{
							flag3 = true;
						}
					}
					flag = true;
				}
				if (this.isTurning)
				{
					if (this.turnFlipType == LookAnimNPC.TurnFlipTypes.BeforeTurn)
					{
						this.DoFlip();
					}
					this.DidTurn();
				}
			}
			else
			{
				this.justFinishedTurning = false;
			}
			bool flag5 = false;
			if (!flag2 && !string.IsNullOrEmpty(this.waitingForTurnStart) && !this.IsAnimPlaying(this.waitingForTurnStart))
			{
				flag5 = true;
				this.waitingForTurnStart = null;
			}
			if ((this.isTurning && flag5) || flag3)
			{
				this.isTurning = false;
				this.turnDelay = 0f;
				switch (this.State)
				{
				case LookAnimNPC.AnimState.Left:
					if (!string.IsNullOrEmpty(this.leftAnim))
					{
						this.PlayAnim(this.leftAnim);
					}
					break;
				case LookAnimNPC.AnimState.TurningLeft:
					this.State = LookAnimNPC.AnimState.Left;
					if (!string.IsNullOrEmpty(this.leftAnim))
					{
						this.PlayAnim(this.leftAnim);
					}
					break;
				case LookAnimNPC.AnimState.Right:
					if (!string.IsNullOrEmpty(this.rightAnim))
					{
						this.PlayAnim(this.rightAnim);
					}
					break;
				case LookAnimNPC.AnimState.TurningRight:
					this.State = LookAnimNPC.AnimState.Right;
					if (!string.IsNullOrEmpty(this.rightAnim))
					{
						this.PlayAnim(this.rightAnim);
					}
					break;
				}
				if (this.turnFlipType == LookAnimNPC.TurnFlipTypes.AfterTurn)
				{
					this.DoFlip();
				}
				this.EnsureCorrectFacing();
			}
			else if (flag && this.restAnimFSM)
			{
				if (this.HasAttention())
				{
					this.ResetRestTimer();
				}
				else
				{
					this.restTimer -= Time.deltaTime;
					if (this.restTimer <= 0f)
					{
						this.preventMoveAttention = true;
						base.StartCoroutine(this.Rest());
					}
				}
			}
			if (!this.isTurning && this.turnDelay <= 0f && this.talkRoutine == null && this.startedConversationCount > 0)
			{
				this.StartTalk();
			}
			Transform currentTarget = this.CurrentTarget;
			if (currentTarget != null)
			{
				this.previousTargetPosition = currentTarget.position;
				return;
			}
		}
		else
		{
			this.ResetRestTimer();
		}
	}

	// Token: 0x06002EBD RID: 11965 RVA: 0x000CE210 File Offset: 0x000CC410
	public void SuppressTurnAudio(float duration)
	{
		this.turnAudioSuppressor = Time.timeAsDouble + (double)duration;
	}

	// Token: 0x06002EBE RID: 11966 RVA: 0x000CE220 File Offset: 0x000CC420
	public void ResetRestTimer()
	{
		this.restTimer = this.restEnterTime;
	}

	// Token: 0x06002EBF RID: 11967 RVA: 0x000CE22E File Offset: 0x000CC42E
	public void ClearTurnDelaySkip()
	{
		this.skipNextDelay = false;
	}

	// Token: 0x06002EC0 RID: 11968 RVA: 0x000CE237 File Offset: 0x000CC437
	private void StartExitDelay()
	{
		this.keepTargetUntil = Time.timeAsDouble + (double)this.exitDelay.GetRandomValue();
		if (Time.timeAsDouble >= this.keepTargetUntil)
		{
			this.target = null;
		}
	}

	// Token: 0x06002EC1 RID: 11969 RVA: 0x000CE265 File Offset: 0x000CC465
	public void TargetEntered(GameObject obj)
	{
		this.target = obj.transform;
		this.keepTargetUntil = 0.0;
		this.preventMoveAttention = false;
	}

	// Token: 0x06002EC2 RID: 11970 RVA: 0x000CE289 File Offset: 0x000CC489
	public bool ShouldFaceLeft()
	{
		return this.ShouldFaceLeft(false);
	}

	// Token: 0x06002EC3 RID: 11971 RVA: 0x000CE292 File Offset: 0x000CC492
	protected virtual float GetXScale()
	{
		return base.transform.lossyScale.x;
	}

	// Token: 0x06002EC4 RID: 11972 RVA: 0x000CE2A4 File Offset: 0x000CC4A4
	public bool ShouldFaceLeft(bool isTalking)
	{
		if (!this.ForceShouldTurnChecking && !this.DoesIdleTurn() && !isTalking)
		{
			if (base.transform.lossyScale.x < 0f)
			{
				return !this.DefaultLeft;
			}
			return this.DefaultLeft;
		}
		else
		{
			Transform currentTarget = this.CurrentTarget;
			if (currentTarget)
			{
				Transform transform = base.transform;
				float num = (this.turnFlipType == LookAnimNPC.TurnFlipTypes.NoFlip) ? base.transform.lossyScale.x : 1f;
				float x = currentTarget.transform.position.x;
				float num2 = transform.position.x + this.centreOffset;
				float num3 = x - num2;
				if (this.isForcedDirection)
				{
					if (this.forcedLeft)
					{
						num3 = -1f;
					}
					else
					{
						num3 = 1f;
					}
				}
				return num3 * num < 0f;
			}
			if (this.turnOnInteract)
			{
				return this.lastTalkedLeft;
			}
			if (base.transform.lossyScale.x < 0f)
			{
				return !this.DefaultLeft;
			}
			return this.DefaultLeft;
		}
	}

	// Token: 0x06002EC5 RID: 11973 RVA: 0x000CE3AC File Offset: 0x000CC5AC
	public bool ForceTurn(bool left)
	{
		this.isForcedDirection = true;
		this.forcedLeft = left;
		bool flag = this.ShouldFaceLeft();
		bool wasFacingLeft = this.GetWasFacingLeft();
		if (!this.isTurning && flag != wasFacingLeft)
		{
			this.skipNextDelay = true;
			this.isTurning = true;
			this.StartTurning(wasFacingLeft, flag);
		}
		return this.isTurning;
	}

	// Token: 0x06002EC6 RID: 11974 RVA: 0x000CE400 File Offset: 0x000CC600
	public bool UnlockTurn()
	{
		this.isForcedDirection = false;
		bool flag = this.ShouldFaceLeft();
		bool wasFacingLeft = this.GetWasFacingLeft();
		if (!this.isTurning && flag != wasFacingLeft)
		{
			this.skipNextDelay = true;
			this.isTurning = true;
			this.StartTurning(wasFacingLeft, flag);
		}
		return this.isTurning;
	}

	// Token: 0x06002EC7 RID: 11975 RVA: 0x000CE44C File Offset: 0x000CC64C
	private void StartTurning(bool wasFacingLeft, bool lookLeft)
	{
		this.WasFacingLeft = wasFacingLeft;
		this.faceLeft = lookLeft;
		LookAnimNPC.AnimState animState = this.State;
		if (animState != LookAnimNPC.AnimState.Left)
		{
			if (animState == LookAnimNPC.AnimState.Right)
			{
				if (lookLeft)
				{
					this.State = LookAnimNPC.AnimState.TurningLeft;
					this.isTurning = true;
					if (!string.IsNullOrEmpty(this.turnLeftAnim))
					{
						this.PlayAnim(this.turnLeftAnim);
						this.waitingForTurnStart = this.turnLeftAnim;
					}
				}
			}
		}
		else if (!lookLeft)
		{
			this.State = LookAnimNPC.AnimState.TurningRight;
			this.isTurning = true;
			if (!string.IsNullOrEmpty(this.turnRightAnim))
			{
				this.PlayAnim(this.turnRightAnim);
				this.waitingForTurnStart = this.turnRightAnim;
			}
		}
		if (this.isTurning)
		{
			if (this.turnFlipType == LookAnimNPC.TurnFlipTypes.BeforeTurn)
			{
				this.DoFlip();
			}
			this.DidTurn();
		}
	}

	// Token: 0x06002EC8 RID: 11976 RVA: 0x000CE502 File Offset: 0x000CC702
	private void DidTurn()
	{
		if (Time.timeAsDouble >= this.turnAudioSuppressor)
		{
			this.turnAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		UnityEvent onStartTurn = this.OnStartTurn;
		if (onStartTurn == null)
		{
			return;
		}
		onStartTurn.Invoke();
	}

	// Token: 0x06002EC9 RID: 11977 RVA: 0x000CE53C File Offset: 0x000CC73C
	public bool HasAttention()
	{
		if (this.startingConversationCount > 0)
		{
			return true;
		}
		if (this.preventMoveAttention)
		{
			return false;
		}
		Transform currentTarget = this.CurrentTarget;
		return currentTarget && (this.restEnterTime <= 0f || (currentTarget.position - this.previousTargetPosition).magnitude > 0.001f);
	}

	// Token: 0x06002ECA RID: 11978 RVA: 0x000CE5A3 File Offset: 0x000CC7A3
	private IEnumerator Rest()
	{
		LookAnimNPC.AnimState previousState = this.State;
		this.State = LookAnimNPC.AnimState.Resting;
		this.isTryingToRest = true;
		HeroController hc = HeroController.instance;
		if (hc != null && !hc.isHeroInPosition)
		{
			while (!hc.isHeroInPosition)
			{
				yield return null;
			}
		}
		bool didRest = false;
		if (this.waitingForBench)
		{
			if (this.restAnimFSM)
			{
				this.restAnimFSM.SetFsmBoolIfExists(this.restingFSMBool, true);
				didRest = true;
			}
			while (this.waitingForBench)
			{
				yield return null;
			}
		}
		bool flag = this.HasAttention();
		if (!flag && !this.forceWake)
		{
			if (this.restAnimFSM)
			{
				this.restAnimFSM.SetFsmBoolIfExists(this.restingFSMBool, true);
				didRest = true;
			}
			while (!flag && !this.forceWake)
			{
				yield return null;
				flag = this.HasAttention();
			}
		}
		if (!this.forceWake)
		{
			yield return new WaitForSeconds(Random.Range(this.minReactDelay, this.maxReactDelay));
		}
		this.forceWake = false;
		if (!didRest)
		{
			this.State = previousState;
		}
		if (this.restAnimFSM)
		{
			if (didRest && (this.startingConversationCount > 0 || this.startedConversationCount > 0))
			{
				this.WasFacingLeft = (this.faceLeft = this.ShouldFaceLeft(true));
			}
			this.restAnimFSM.SetFsmBoolIfExists(this.restingFSMBool, false);
		}
		this.isTryingToRest = false;
		yield break;
	}

	// Token: 0x06002ECB RID: 11979 RVA: 0x000CE5B2 File Offset: 0x000CC7B2
	public void ClearRestBehaviour()
	{
		this.restAnimFSM = null;
	}

	// Token: 0x06002ECC RID: 11980 RVA: 0x000CE5BB File Offset: 0x000CC7BB
	private void OnNoiseStarted()
	{
		if (this.State != LookAnimNPC.AnimState.Resting)
		{
			return;
		}
		this.ForceWake();
	}

	// Token: 0x06002ECD RID: 11981 RVA: 0x000CE5CD File Offset: 0x000CC7CD
	public void ForceWake()
	{
		this.forceWake = true;
	}

	// Token: 0x06002ECE RID: 11982 RVA: 0x000CE5D6 File Offset: 0x000CC7D6
	public void SetForceWake(bool value)
	{
		this.forceWake = value;
	}

	// Token: 0x06002ECF RID: 11983 RVA: 0x000CE5DF File Offset: 0x000CC7DF
	public void EndRest(bool facingLeft)
	{
		this.EndRestInternal(new bool?(facingLeft));
	}

	// Token: 0x06002ED0 RID: 11984 RVA: 0x000CE5F0 File Offset: 0x000CC7F0
	public void EndRest()
	{
		this.EndRestInternal(null);
	}

	// Token: 0x06002ED1 RID: 11985 RVA: 0x000CE60C File Offset: 0x000CC80C
	private void EndRestInternal(bool? facingLeft)
	{
		bool flag = this.DefaultLeft;
		if (base.transform.lossyScale.x < 0f)
		{
			flag = !flag;
		}
		if (this.startingConversationCount > 0)
		{
			if (facingLeft != null)
			{
				this.PlayIdle(facingLeft.Value);
			}
			this.WasFacingLeft = (this.faceLeft = (facingLeft ?? flag));
			if (this.target == null)
			{
				this.target = HeroController.instance.transform;
				this.keepTargetUntil = 0.0;
			}
			this.StartTalk();
			return;
		}
		int num = (int)this.State;
		this.ResetState(facingLeft ?? flag);
		if (num == 6)
		{
			this.State = LookAnimNPC.AnimState.Disabled;
		}
	}

	// Token: 0x06002ED2 RID: 11986 RVA: 0x000CE6E0 File Offset: 0x000CC8E0
	public void ResetState(bool facingLeft)
	{
		this.State = (facingLeft ? LookAnimNPC.AnimState.Left : LookAnimNPC.AnimState.Right);
		this.faceLeft = facingLeft;
		this.WasFacingLeft = facingLeft;
		this.skipNextDelay = true;
		this.turnDelay = 0f;
		bool flag = this.DefaultLeft;
		if (base.transform.lossyScale.x < 0f)
		{
			flag = !flag;
		}
		this.PlayIdle(this.DoesIdleTurn() ? facingLeft : flag);
	}

	// Token: 0x06002ED3 RID: 11987 RVA: 0x000CE754 File Offset: 0x000CC954
	private void PlayIdle(bool facingLeft)
	{
		string text = facingLeft ? this.leftAnim : this.rightAnim;
		if (!string.IsNullOrEmpty(text))
		{
			this.PlayAnim(text);
		}
	}

	// Token: 0x06002ED4 RID: 11988 RVA: 0x000CE784 File Offset: 0x000CC984
	[ContextMenu("Activate")]
	public void Activate()
	{
		this.Activate(this.GetIsSpriteFlipped() ? (!this.DefaultLeft) : this.DefaultLeft);
	}

	// Token: 0x06002ED5 RID: 11989 RVA: 0x000CE7B4 File Offset: 0x000CC9B4
	public void Activate(bool facingLeft)
	{
		if (this.wasFlippedOnDeactivate)
		{
			this.wasFlippedOnDeactivate = false;
			this.FlipSprite();
			base.transform.FlipLocalScale(true, false, false);
			this.isSpriteFlipped = true;
		}
		bool flag = this.State == LookAnimNPC.AnimState.Resting;
		this.ResetState(facingLeft);
		if (flag && this.restAnimFSM)
		{
			this.restAnimFSM.SetFsmBoolIfExists(this.restingFSMBool, false);
			this.restAnimFSM.SendEvent("DEACTIVATED");
		}
	}

	// Token: 0x06002ED6 RID: 11990 RVA: 0x000CE82C File Offset: 0x000CCA2C
	public void Enable()
	{
		this.disabled = false;
	}

	// Token: 0x06002ED7 RID: 11991 RVA: 0x000CE835 File Offset: 0x000CCA35
	public void Disable()
	{
		this.disabled = true;
	}

	// Token: 0x06002ED8 RID: 11992 RVA: 0x000CE840 File Offset: 0x000CCA40
	[ContextMenu("Deactivate")]
	public void Deactivate()
	{
		this.State = LookAnimNPC.AnimState.Disabled;
		if (this.restAnimFSM)
		{
			this.restAnimFSM.SetFsmBoolIfExists(this.restingFSMBool, false);
			this.restAnimFSM.SendEvent("DEACTIVATED");
		}
		if (this.talkRoutine != null)
		{
			base.StopCoroutine(this.talkRoutine);
			this.talkRoutine = null;
		}
		if (this.turnFlipType != LookAnimNPC.TurnFlipTypes.NoFlip && this.isSpriteFlipped)
		{
			this.FlipSprite();
			this.isSpriteFlipped = false;
			base.transform.FlipLocalScale(true, false, false);
			this.wasFlippedOnDeactivate = true;
		}
	}

	// Token: 0x06002ED9 RID: 11993 RVA: 0x000CE8D0 File Offset: 0x000CCAD0
	public void DeactivateInstant()
	{
		this.State = LookAnimNPC.AnimState.Disabled;
		if (this.restAnimFSM)
		{
			this.restAnimFSM.SetFsmBoolIfExists(this.restingFSMBool, false);
			this.restAnimFSM.SendEvent("DEACTIVATED_INSTANT");
			this.EndRest();
		}
		if (this.talkRoutine != null)
		{
			base.StopCoroutine(this.talkRoutine);
			this.talkRoutine = null;
		}
		if (this.turnFlipType != LookAnimNPC.TurnFlipTypes.NoFlip && this.isSpriteFlipped)
		{
			this.FlipSprite();
			this.isSpriteFlipped = false;
			base.transform.FlipLocalScale(true, false, false);
			this.wasFlippedOnDeactivate = true;
		}
	}

	// Token: 0x06002EDA RID: 11994 RVA: 0x000CE966 File Offset: 0x000CCB66
	private void StartTalk()
	{
		if (this.talkRoutine != null)
		{
			base.StopCoroutine(this.talkRoutine);
			this.talkRoutine = null;
		}
		this.talkRoutine = base.StartCoroutine(this.Talk());
	}

	// Token: 0x06002EDB RID: 11995 RVA: 0x000CE995 File Offset: 0x000CCB95
	private IEnumerator Talk()
	{
		LookAnimNPC.<>c__DisplayClass134_0 CS$<>8__locals1 = new LookAnimNPC.<>c__DisplayClass134_0();
		CS$<>8__locals1.<>4__this = this;
		while (this.turnDelay > 0f || this.IsTurning)
		{
			yield return null;
		}
		this.State = LookAnimNPC.AnimState.Talking;
		bool shouldFaceLeft;
		do
		{
			shouldFaceLeft = this.ShouldFaceLeft(true);
			bool wasFacingLeft = this.GetWasFacingLeft();
			if (shouldFaceLeft != wasFacingLeft)
			{
				LookAnimNPC.<>c__DisplayClass134_1 CS$<>8__locals2 = new LookAnimNPC.<>c__DisplayClass134_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				CS$<>8__locals2.turnAnim = (shouldFaceLeft ? this.turnLeftAnim : this.turnRightAnim);
				if (!string.IsNullOrEmpty(CS$<>8__locals2.turnAnim))
				{
					this.PlayAnim(CS$<>8__locals2.turnAnim);
					if (this.turnFlipType == LookAnimNPC.TurnFlipTypes.BeforeTurn)
					{
						this.DoFlip();
					}
					this.DidTurn();
					yield return new WaitUntil(() => CS$<>8__locals2.CS$<>8__locals1.<>4__this.IsAnimPlaying(CS$<>8__locals2.turnAnim));
					yield return new WaitUntil(() => !CS$<>8__locals2.CS$<>8__locals1.<>4__this.IsAnimPlaying(CS$<>8__locals2.turnAnim));
					if (this.turnFlipType == LookAnimNPC.TurnFlipTypes.AfterTurn)
					{
						this.DoFlip();
					}
				}
				this.WasFacingLeft = (this.faceLeft = shouldFaceLeft);
				CS$<>8__locals2 = null;
			}
			this.PlayIdle(this.faceLeft);
			while (this.startedConversationCount == 0)
			{
				yield return null;
				if (this.startingConversationCount == 0)
				{
					goto IL_433;
				}
			}
		}
		while (this.ShouldFaceLeft(true) != shouldFaceLeft);
		CS$<>8__locals1.anims = (shouldFaceLeft ? this.talkLeftAnims : this.talkRightAnims);
		if (!string.IsNullOrEmpty(CS$<>8__locals1.anims.Enter))
		{
			this.PlayAnim(CS$<>8__locals1.anims.Enter);
			yield return new WaitUntil(() => CS$<>8__locals1.<>4__this.IsAnimPlaying(CS$<>8__locals1.anims.Enter));
			yield return new WaitUntil(() => !CS$<>8__locals1.<>4__this.IsAnimPlaying(CS$<>8__locals1.anims.Enter));
		}
		if (this.playTurnAudioOnTalkStart && this.turnAudioClipTable)
		{
			this.turnAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		bool wasNpcTalking = !this.isCurrentLineNpc;
		do
		{
			if (this.isCurrentLineNpc)
			{
				if (!wasNpcTalking && !string.IsNullOrEmpty(CS$<>8__locals1.anims.Talk))
				{
					this.PlayAnim(CS$<>8__locals1.anims.Talk);
				}
			}
			else if (wasNpcTalking && !string.IsNullOrEmpty(CS$<>8__locals1.anims.Listen))
			{
				this.PlayAnim(CS$<>8__locals1.anims.Listen);
			}
			wasNpcTalking = this.isCurrentLineNpc;
			yield return null;
		}
		while (this.startingConversationCount > 0);
		if (!string.IsNullOrEmpty(CS$<>8__locals1.anims.Exit))
		{
			this.PlayAnim(CS$<>8__locals1.anims.Exit);
			yield return new WaitUntil(() => CS$<>8__locals1.<>4__this.IsAnimPlaying(CS$<>8__locals1.anims.Exit));
			yield return new WaitUntil(() => !CS$<>8__locals1.<>4__this.IsAnimPlaying(CS$<>8__locals1.anims.Exit));
		}
		if (this.playTurnAudioOnTalkStart && this.turnAudioClipTable)
		{
			this.turnAudioClipTable.SpawnAndPlayOneShot(base.transform.position, false);
		}
		IL_433:
		this.lastTalkedLeft = this.faceLeft;
		this.talkRoutine = null;
		if (this.State != LookAnimNPC.AnimState.Disabled)
		{
			this.ResetState(this.faceLeft);
		}
		yield break;
	}

	// Token: 0x06002EDC RID: 11996 RVA: 0x000CE9A4 File Offset: 0x000CCBA4
	[UsedImplicitly]
	public bool IsFacingLeft()
	{
		LookAnimNPC.AnimState animState = this.State;
		return animState <= LookAnimNPC.AnimState.TurningLeft;
	}

	// Token: 0x06002EDD RID: 11997 RVA: 0x000CE9C0 File Offset: 0x000CCBC0
	[UsedImplicitly]
	public bool IsFacingLeftScaled()
	{
		bool flag = base.transform.lossyScale.x < 0f;
		LookAnimNPC.AnimState animState = this.State;
		if (animState <= LookAnimNPC.AnimState.TurningLeft)
		{
			return !flag;
		}
		return flag;
	}

	// Token: 0x06002EDE RID: 11998 RVA: 0x000CE9F6 File Offset: 0x000CCBF6
	private void DoFlip()
	{
		this.FlipSprite();
		this.isSpriteFlipped = !this.isSpriteFlipped;
	}

	// Token: 0x06002EDF RID: 11999
	protected abstract void PlayAnim(string animName);

	// Token: 0x06002EE0 RID: 12000
	protected abstract bool IsAnimPlaying(string animName);

	// Token: 0x06002EE1 RID: 12001
	protected abstract void FlipSprite();

	// Token: 0x06002EE2 RID: 12002
	protected abstract bool GetIsSpriteFlipped();

	// Token: 0x06002EE3 RID: 12003 RVA: 0x000CEA0D File Offset: 0x000CCC0D
	protected virtual bool GetWasFacingLeft()
	{
		return this.faceLeft;
	}

	// Token: 0x06002EE4 RID: 12004 RVA: 0x000CEA15 File Offset: 0x000CCC15
	protected virtual void EnsureCorrectFacing()
	{
	}

	// Token: 0x06002EE5 RID: 12005 RVA: 0x000CEA17 File Offset: 0x000CCC17
	public void SetDefaultFacingLeft(bool set)
	{
		this.defaultLeft = set;
	}

	// Token: 0x06002EE6 RID: 12006 RVA: 0x000CEA20 File Offset: 0x000CCC20
	public void FaceTargetInstant()
	{
		if (this.isTurning)
		{
			return;
		}
		this.WasFacingLeft = this.GetWasFacingLeft();
		this.faceLeft = this.ShouldFaceLeft(false);
		if (this.WasFacingLeft == this.faceLeft)
		{
			return;
		}
		bool flag = this.faceLeft;
		LookAnimNPC.AnimState animState = this.State;
		if (animState != LookAnimNPC.AnimState.Left)
		{
			if (animState == LookAnimNPC.AnimState.Right)
			{
				if (flag)
				{
					this.State = LookAnimNPC.AnimState.TurningLeft;
					if (!string.IsNullOrEmpty(this.turnLeftAnim))
					{
						this.PlayAnim(this.turnLeftAnim);
						this.waitingForTurnStart = this.turnLeftAnim;
					}
				}
			}
		}
		else if (!flag)
		{
			this.State = LookAnimNPC.AnimState.TurningRight;
			if (!string.IsNullOrEmpty(this.turnRightAnim))
			{
				this.PlayAnim(this.turnRightAnim);
				this.waitingForTurnStart = this.turnRightAnim;
			}
		}
		if (this.turnFlipType == LookAnimNPC.TurnFlipTypes.BeforeTurn)
		{
			this.DoFlip();
		}
		this.DidTurn();
		this.isTurning = false;
		this.turnDelay = 0f;
		animState = this.State;
		if (animState != LookAnimNPC.AnimState.TurningLeft)
		{
			if (animState == LookAnimNPC.AnimState.TurningRight)
			{
				this.State = LookAnimNPC.AnimState.Right;
				if (!string.IsNullOrEmpty(this.rightAnim))
				{
					this.PlayAnim(this.rightAnim);
				}
			}
		}
		else
		{
			this.State = LookAnimNPC.AnimState.Left;
			if (!string.IsNullOrEmpty(this.leftAnim))
			{
				this.PlayAnim(this.leftAnim);
			}
		}
		if (this.turnFlipType == LookAnimNPC.TurnFlipTypes.AfterTurn)
		{
			this.DoFlip();
		}
		this.EnsureCorrectFacing();
	}

	// Token: 0x04003137 RID: 12599
	[Tooltip("Automatically remove behaviour on Start if further than this from the hero's Z. Leave 0 for no limit.")]
	[SerializeField]
	private float limitZ;

	// Token: 0x04003138 RID: 12600
	[Space]
	[SerializeField]
	private string leftAnim;

	// Token: 0x04003139 RID: 12601
	[SerializeField]
	private string rightAnim;

	// Token: 0x0400313A RID: 12602
	[SerializeField]
	private string turnLeftAnim;

	// Token: 0x0400313B RID: 12603
	[SerializeField]
	private string turnRightAnim;

	// Token: 0x0400313C RID: 12604
	[SerializeField]
	protected LookAnimNPC.TurnFlipTypes turnFlipType;

	// Token: 0x0400313D RID: 12605
	[SerializeField]
	private RandomAudioClipTable turnAudioClipTable;

	// Token: 0x0400313E RID: 12606
	[SerializeField]
	private bool playTurnAudioOnTalkStart;

	// Token: 0x0400313F RID: 12607
	[SerializeField]
	protected bool defaultLeft = true;

	// Token: 0x04003140 RID: 12608
	[SerializeField]
	private bool facingIgnoresScale;

	// Token: 0x04003141 RID: 12609
	[Space]
	[SerializeField]
	private float centreOffset;

	// Token: 0x04003142 RID: 12610
	[SerializeField]
	[ModifiableProperty]
	[Conditional("DoesIdleTurn", true, true, false)]
	private TriggerEnterEvent enterDetector;

	// Token: 0x04003143 RID: 12611
	[SerializeField]
	[ModifiableProperty]
	[Conditional("DoesIdleTurn", true, true, false)]
	private TriggerEnterEvent exitDetector;

	// Token: 0x04003144 RID: 12612
	[SerializeField]
	private MinMaxFloat exitDelay;

	// Token: 0x04003145 RID: 12613
	[SerializeField]
	private bool turnOnInteract;

	// Token: 0x04003146 RID: 12614
	[Space]
	[SerializeField]
	private float minReactDelay = 0.3f;

	// Token: 0x04003147 RID: 12615
	[SerializeField]
	private float maxReactDelay = 0.5f;

	// Token: 0x04003148 RID: 12616
	[SerializeField]
	private bool waitForHeroInPosition;

	// Token: 0x04003149 RID: 12617
	[Space]
	public UnityEvent OnStartTurn;

	// Token: 0x0400314A RID: 12618
	[Header("Talking")]
	[SerializeField]
	private NPCControlBase npcControl;

	// Token: 0x0400314B RID: 12619
	[SerializeField]
	private string talkingPageEvent;

	// Token: 0x0400314C RID: 12620
	[SerializeField]
	private LookAnimNPC.TalkAnims talkLeftAnims;

	// Token: 0x0400314D RID: 12621
	[SerializeField]
	private LookAnimNPC.TalkAnims talkRightAnims;

	// Token: 0x0400314E RID: 12622
	[Space]
	[SerializeField]
	private NPCControlBase[] extraConvoTrackers;

	// Token: 0x0400314F RID: 12623
	[Header("Resting")]
	[SerializeField]
	private float restEnterTime = 3f;

	// Token: 0x04003150 RID: 12624
	[Space]
	[SerializeField]
	private PlayMakerFSM restAnimFSM;

	// Token: 0x04003151 RID: 12625
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsFsmBoolValid")]
	private string restingFSMBool = "Resting";

	// Token: 0x04003152 RID: 12626
	[SerializeField]
	private bool debugMe;

	// Token: 0x04003153 RID: 12627
	private bool disabled;

	// Token: 0x04003154 RID: 12628
	private float restTimer;

	// Token: 0x04003155 RID: 12629
	private int startingConversationCount;

	// Token: 0x04003156 RID: 12630
	private int startedConversationCount;

	// Token: 0x04003157 RID: 12631
	private bool isCurrentLineNpc;

	// Token: 0x04003158 RID: 12632
	private bool isTurning;

	// Token: 0x04003159 RID: 12633
	private string waitingForTurnStart;

	// Token: 0x0400315A RID: 12634
	private Vector2 previousTargetPosition;

	// Token: 0x0400315B RID: 12635
	private bool preventMoveAttention;

	// Token: 0x0400315C RID: 12636
	private bool forceWake;

	// Token: 0x0400315D RID: 12637
	private bool isSpriteFlipped;

	// Token: 0x0400315E RID: 12638
	private bool faceLeft;

	// Token: 0x0400315F RID: 12639
	protected float turnDelay;

	// Token: 0x04003160 RID: 12640
	private bool skipNextDelay;

	// Token: 0x04003161 RID: 12641
	private bool lastTalkedLeft;

	// Token: 0x04003162 RID: 12642
	private bool waitingForHero;

	// Token: 0x04003163 RID: 12643
	private double turnAudioSuppressor;

	// Token: 0x04003164 RID: 12644
	private Coroutine talkRoutine;

	// Token: 0x04003165 RID: 12645
	private NoiseResponder noiseResponder;

	// Token: 0x04003166 RID: 12646
	private Transform target;

	// Token: 0x04003167 RID: 12647
	private double keepTargetUntil;

	// Token: 0x04003168 RID: 12648
	private LookAnimNPC.AnimState state;

	// Token: 0x0400316E RID: 12654
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string talkLeftAnim;

	// Token: 0x0400316F RID: 12655
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string talkRightAnim;

	// Token: 0x04003170 RID: 12656
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool flipAfterTurn;

	// Token: 0x04003171 RID: 12657
	private bool justFinishedTurning;

	// Token: 0x04003172 RID: 12658
	private bool isTryingToRest;

	// Token: 0x04003173 RID: 12659
	private bool wasFlippedOnDeactivate;

	// Token: 0x04003174 RID: 12660
	private bool isForcedDirection;

	// Token: 0x04003175 RID: 12661
	private bool forcedLeft;

	// Token: 0x04003176 RID: 12662
	private bool waitingForBench;

	// Token: 0x0200181B RID: 6171
	[Serializable]
	private struct TalkAnims
	{
		// Token: 0x040090BC RID: 37052
		public string Enter;

		// Token: 0x040090BD RID: 37053
		public string Talk;

		// Token: 0x040090BE RID: 37054
		public string Listen;

		// Token: 0x040090BF RID: 37055
		public string Exit;
	}

	// Token: 0x0200181C RID: 6172
	public enum AnimState
	{
		// Token: 0x040090C1 RID: 37057
		Left,
		// Token: 0x040090C2 RID: 37058
		TurningLeft,
		// Token: 0x040090C3 RID: 37059
		Right,
		// Token: 0x040090C4 RID: 37060
		TurningRight,
		// Token: 0x040090C5 RID: 37061
		Talking,
		// Token: 0x040090C6 RID: 37062
		Resting,
		// Token: 0x040090C7 RID: 37063
		Disabled
	}

	// Token: 0x0200181D RID: 6173
	protected enum TurnFlipTypes
	{
		// Token: 0x040090C9 RID: 37065
		NoFlip,
		// Token: 0x040090CA RID: 37066
		AfterTurn,
		// Token: 0x040090CB RID: 37067
		BeforeTurn
	}
}
