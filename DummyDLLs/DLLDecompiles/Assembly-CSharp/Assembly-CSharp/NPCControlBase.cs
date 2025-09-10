using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000636 RID: 1590
public abstract class NPCControlBase : InteractableBase
{
	// Token: 0x140000B7 RID: 183
	// (add) Token: 0x060038C1 RID: 14529 RVA: 0x000FAA94 File Offset: 0x000F8C94
	// (remove) Token: 0x060038C2 RID: 14530 RVA: 0x000FAACC File Offset: 0x000F8CCC
	public event Action StartingDialogue;

	// Token: 0x140000B8 RID: 184
	// (add) Token: 0x060038C3 RID: 14531 RVA: 0x000FAB04 File Offset: 0x000F8D04
	// (remove) Token: 0x060038C4 RID: 14532 RVA: 0x000FAB3C File Offset: 0x000F8D3C
	public event Action StartedDialogue;

	// Token: 0x140000B9 RID: 185
	// (add) Token: 0x060038C5 RID: 14533 RVA: 0x000FAB74 File Offset: 0x000F8D74
	// (remove) Token: 0x060038C6 RID: 14534 RVA: 0x000FABAC File Offset: 0x000F8DAC
	public event Action<DialogueBox.DialogueLine> OpeningDialogueBox;

	// Token: 0x140000BA RID: 186
	// (add) Token: 0x060038C7 RID: 14535 RVA: 0x000FABE4 File Offset: 0x000F8DE4
	// (remove) Token: 0x060038C8 RID: 14536 RVA: 0x000FAC1C File Offset: 0x000F8E1C
	public event Action<DialogueBox.DialogueLine> StartedNewLine;

	// Token: 0x140000BB RID: 187
	// (add) Token: 0x060038C9 RID: 14537 RVA: 0x000FAC54 File Offset: 0x000F8E54
	// (remove) Token: 0x060038CA RID: 14538 RVA: 0x000FAC8C File Offset: 0x000F8E8C
	public event Action<DialogueBox.DialogueLine> LineEnded;

	// Token: 0x140000BC RID: 188
	// (add) Token: 0x060038CB RID: 14539 RVA: 0x000FACC4 File Offset: 0x000F8EC4
	// (remove) Token: 0x060038CC RID: 14540 RVA: 0x000FACFC File Offset: 0x000F8EFC
	public event Action EndingDialogue;

	// Token: 0x140000BD RID: 189
	// (add) Token: 0x060038CD RID: 14541 RVA: 0x000FAD34 File Offset: 0x000F8F34
	// (remove) Token: 0x060038CE RID: 14542 RVA: 0x000FAD6C File Offset: 0x000F8F6C
	public event Action EndedDialogue;

	// Token: 0x17000671 RID: 1649
	// (get) Token: 0x060038CF RID: 14543 RVA: 0x000FADA1 File Offset: 0x000F8FA1
	public virtual bool AutoEnd
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000672 RID: 1650
	// (get) Token: 0x060038D0 RID: 14544 RVA: 0x000FADA4 File Offset: 0x000F8FA4
	protected virtual bool AutoCallEndAction
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000673 RID: 1651
	// (get) Token: 0x060038D1 RID: 14545 RVA: 0x000FADA7 File Offset: 0x000F8FA7
	protected override bool IsQueueingHandled
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000674 RID: 1652
	// (get) Token: 0x060038D2 RID: 14546 RVA: 0x000FADAA File Offset: 0x000F8FAA
	protected override bool AutoQueueOnDeactivate
	{
		get
		{
			return this.isWaitingToBegin;
		}
	}

	// Token: 0x17000675 RID: 1653
	// (get) Token: 0x060038D3 RID: 14547 RVA: 0x000FADB2 File Offset: 0x000F8FB2
	protected virtual bool AllowMovePlayer
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000676 RID: 1654
	// (get) Token: 0x060038D4 RID: 14548 RVA: 0x000FADB5 File Offset: 0x000F8FB5
	private bool DoMovePlayer
	{
		get
		{
			return (this.AllowMovePlayer && Math.Abs(this.targetDistance) > 0.01f) || this.outsideRangeBehaviour > NPCControlBase.OutsideRangeBehaviours.None;
		}
	}

	// Token: 0x17000677 RID: 1655
	// (get) Token: 0x060038D5 RID: 14549 RVA: 0x000FADDC File Offset: 0x000F8FDC
	public bool OverrideHeroHurtAnim
	{
		get
		{
			if (!this.overrideHeroHurtAnim)
			{
				HeroTalkAnimation.AnimationTypes animationTypes = this.HeroAnimation;
				return animationTypes == HeroTalkAnimation.AnimationTypes.Kneeling || animationTypes == HeroTalkAnimation.AnimationTypes.Custom;
			}
			return true;
		}
	}

	// Token: 0x17000678 RID: 1656
	// (get) Token: 0x060038D6 RID: 14550 RVA: 0x000FAE0B File Offset: 0x000F900B
	// (set) Token: 0x060038D7 RID: 14551 RVA: 0x000FAE13 File Offset: 0x000F9013
	public float CentreOffset
	{
		get
		{
			return this.centreOffset;
		}
		set
		{
			this.centreOffset = value;
		}
	}

	// Token: 0x17000679 RID: 1657
	// (get) Token: 0x060038D8 RID: 14552 RVA: 0x000FAE1C File Offset: 0x000F901C
	// (set) Token: 0x060038D9 RID: 14553 RVA: 0x000FAE24 File Offset: 0x000F9024
	public float TargetDistance
	{
		get
		{
			return this.targetDistance;
		}
		set
		{
			this.targetDistance = value;
		}
	}

	// Token: 0x1700067A RID: 1658
	// (get) Token: 0x060038DA RID: 14554 RVA: 0x000FAE2D File Offset: 0x000F902D
	// (set) Token: 0x060038DB RID: 14555 RVA: 0x000FAE35 File Offset: 0x000F9035
	public NPCControlBase.TalkPositions TalkPosition
	{
		get
		{
			return this.talkPosition;
		}
		set
		{
			this.talkPosition = value;
		}
	}

	// Token: 0x1700067B RID: 1659
	// (get) Token: 0x060038DC RID: 14556 RVA: 0x000FAE3E File Offset: 0x000F903E
	// (set) Token: 0x060038DD RID: 14557 RVA: 0x000FAE46 File Offset: 0x000F9046
	public HeroTalkAnimation.AnimationTypes HeroAnimation
	{
		get
		{
			return this.heroAnimation;
		}
		set
		{
			this.heroAnimation = value;
		}
	}

	// Token: 0x1700067C RID: 1660
	// (get) Token: 0x060038DE RID: 14558 RVA: 0x000FAE4F File Offset: 0x000F904F
	// (set) Token: 0x060038DF RID: 14559 RVA: 0x000FAE57 File Offset: 0x000F9057
	public NPCControlBase.OutsideRangeBehaviours OutsideRangeBehaviour
	{
		get
		{
			return this.outsideRangeBehaviour;
		}
		set
		{
			this.outsideRangeBehaviour = value;
		}
	}

	// Token: 0x1700067D RID: 1661
	// (get) Token: 0x060038E0 RID: 14560 RVA: 0x000FAE60 File Offset: 0x000F9060
	// (set) Token: 0x060038E1 RID: 14561 RVA: 0x000FAE68 File Offset: 0x000F9068
	public NPCControlBase.HeroAnimBeginTypes HeroAnimBegin
	{
		get
		{
			return this.heroAnimBegin;
		}
		set
		{
			this.heroAnimBegin = value;
		}
	}

	// Token: 0x060038E2 RID: 14562 RVA: 0x000FAE71 File Offset: 0x000F9071
	protected virtual void OnEnable()
	{
		this.RegisterSceneEvents();
	}

	// Token: 0x060038E3 RID: 14563 RVA: 0x000FAE7C File Offset: 0x000F907C
	protected override void OnDisable()
	{
		this.UnregisterSceneEvents();
		base.OnDisable();
		if (this.blockedHeroInput || this.moveHeroRoutine != null)
		{
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				if (this.moveHeroRoutine != null)
				{
					this.moveHeroRoutine = null;
					base.EnableInteraction();
					if (!instance.HasAnimationControl)
					{
						instance.StartAnimationControl();
					}
				}
				if (this.blockedHeroInput)
				{
					instance.RemoveInputBlocker(this);
					this.blockedHeroInput = false;
				}
			}
		}
	}

	// Token: 0x060038E4 RID: 14564 RVA: 0x000FAEF0 File Offset: 0x000F90F0
	private void OnDrawGizmosSelected()
	{
		if (!this.DoMovePlayer)
		{
			return;
		}
		Gizmos.DrawWireSphere(base.transform.position + new Vector3(this.centreOffset, 0f, 0f), 0.25f);
		if (this.talkPosition != NPCControlBase.TalkPositions.Left)
		{
			Gizmos.DrawWireSphere(base.transform.position + new Vector3(this.centreOffset + this.targetDistance, 0f, 0f), 0.1f);
		}
		if (this.talkPosition != NPCControlBase.TalkPositions.Right)
		{
			Gizmos.DrawWireSphere(base.transform.position + new Vector3(this.centreOffset - this.targetDistance, 0f, 0f), 0.1f);
		}
	}

	// Token: 0x060038E5 RID: 14565 RVA: 0x000FAFB3 File Offset: 0x000F91B3
	protected override void OnValidate()
	{
		base.OnValidate();
		if (this.forceToDistance)
		{
			this.outsideRangeBehaviour = NPCControlBase.OutsideRangeBehaviours.ForceToDistance;
			this.forceToDistance = false;
		}
	}

	// Token: 0x060038E6 RID: 14566 RVA: 0x000FAFD1 File Offset: 0x000F91D1
	protected virtual void Start()
	{
		this.CheckTalkSide();
	}

	// Token: 0x060038E7 RID: 14567 RVA: 0x000FAFDC File Offset: 0x000F91DC
	private void RegisterSceneEvents()
	{
		if (!this.nextSceneRegistered)
		{
			HeroController instance = HeroController.instance;
			if (instance)
			{
				this.nextSceneRegistered = true;
				instance.HeroLeavingScene += this.OnNextSceneWillActivate;
			}
		}
	}

	// Token: 0x060038E8 RID: 14568 RVA: 0x000FB018 File Offset: 0x000F9218
	private void UnregisterSceneEvents()
	{
		if (this.nextSceneRegistered)
		{
			this.nextSceneRegistered = false;
			HeroController silentInstance = HeroController.SilentInstance;
			if (silentInstance)
			{
				silentInstance.HeroLeavingScene -= this.OnNextSceneWillActivate;
			}
		}
	}

	// Token: 0x060038E9 RID: 14569 RVA: 0x000FB054 File Offset: 0x000F9254
	private void OnNextSceneWillActivate()
	{
		this.CancelHeroMove();
	}

	// Token: 0x060038EA RID: 14570 RVA: 0x000FB05C File Offset: 0x000F925C
	public void StartDialogueMove()
	{
		this.SendStartingDialogue();
		this.isWaitingToBegin = true;
		if (this.DoMovePlayer)
		{
			this.moveHeroRoutine = base.StartCoroutine(this.MovePlayer(new Action(this.StartDialogue)));
			return;
		}
		this.StartDialogue();
	}

	// Token: 0x060038EB RID: 14571 RVA: 0x000FB098 File Offset: 0x000F9298
	protected void CancelHeroMove()
	{
		if (this.moveHeroRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this.moveHeroRoutine);
		this.moveHeroRoutine = null;
		HeroController instance = HeroController.instance;
		if (instance == null)
		{
			return;
		}
		if (this.didChangedSpriteDirection)
		{
			this.didChangedSpriteDirection = false;
			if (this.originalFacingDirection)
			{
				instance.FaceRight();
			}
			else
			{
				instance.FaceLeft();
			}
		}
		if (this.blockedHeroInput)
		{
			instance.RemoveInputBlocker(this);
			this.blockedHeroInput = false;
		}
	}

	// Token: 0x060038EC RID: 14572 RVA: 0x000FB10C File Offset: 0x000F930C
	public void StartDialogueImmediately()
	{
		this.SendStartingDialogue();
		this.StartDialogue();
	}

	// Token: 0x060038ED RID: 14573 RVA: 0x000FB11A File Offset: 0x000F931A
	private void SendStartingDialogue()
	{
		Action startingDialogue = this.StartingDialogue;
		if (startingDialogue != null)
		{
			startingDialogue();
		}
		this.OnStartingDialogue();
	}

	// Token: 0x060038EE RID: 14574 RVA: 0x000FB133 File Offset: 0x000F9333
	private void StartDialogue()
	{
		this.isWaitingToBegin = false;
		if (this.heroAnimBegin == NPCControlBase.HeroAnimBeginTypes.Auto)
		{
			HeroTalkAnimation.EnterConversation(this);
		}
		Action startedDialogue = this.StartedDialogue;
		if (startedDialogue != null)
		{
			startedDialogue();
		}
		this.OnStartDialogue();
	}

	// Token: 0x060038EF RID: 14575 RVA: 0x000FB161 File Offset: 0x000F9361
	public void BeginHeroAnimManual()
	{
		if (this.heroAnimBegin != NPCControlBase.HeroAnimBeginTypes.Manual)
		{
			return;
		}
		HeroTalkAnimation.EnterConversation(this);
	}

	// Token: 0x060038F0 RID: 14576 RVA: 0x000FB173 File Offset: 0x000F9373
	public void BeginHeroTalkAnimation()
	{
		HeroTalkAnimation.EnterConversation(this);
	}

	// Token: 0x060038F1 RID: 14577 RVA: 0x000FB17B File Offset: 0x000F937B
	public void NewLineStarted(DialogueBox.DialogueLine line)
	{
		HeroTalkAnimation.SetTalking(line.IsPlayer, line.IsPlayer);
		Action<DialogueBox.DialogueLine> startedNewLine = this.StartedNewLine;
		if (startedNewLine != null)
		{
			startedNewLine(line);
		}
		this.OnNewLineStarted(line);
	}

	// Token: 0x060038F2 RID: 14578 RVA: 0x000FB1A7 File Offset: 0x000F93A7
	public void NewLineEnded(DialogueBox.DialogueLine line)
	{
		if (line.IsPlayer)
		{
			HeroTalkAnimation.SetTalking(true, false);
		}
		Action<DialogueBox.DialogueLine> lineEnded = this.LineEnded;
		if (lineEnded != null)
		{
			lineEnded(line);
		}
		this.OnLineEnded(line);
	}

	// Token: 0x060038F3 RID: 14579 RVA: 0x000FB1D1 File Offset: 0x000F93D1
	public void EndDialogue()
	{
		this.CancelHeroMove();
		HeroTalkAnimation.SetTalking(false, false);
		Action endingDialogue = this.EndingDialogue;
		if (endingDialogue != null)
		{
			endingDialogue();
		}
		if (this.AutoCallEndAction)
		{
			this.CallEndAction();
		}
		this.OnEndDialogue();
	}

	// Token: 0x060038F4 RID: 14580 RVA: 0x000FB205 File Offset: 0x000F9405
	protected void CallEndAction()
	{
		HeroTalkAnimation.ExitConversation();
		Action endedDialogue = this.EndedDialogue;
		if (endedDialogue != null)
		{
			endedDialogue();
		}
		GameManager.instance.DoQueuedSaveGame();
	}

	// Token: 0x060038F5 RID: 14581 RVA: 0x000FB227 File Offset: 0x000F9427
	public void OnOpeningDialogueBox(DialogueBox.DialogueLine firstLine)
	{
		Action<DialogueBox.DialogueLine> openingDialogueBox = this.OpeningDialogueBox;
		if (openingDialogueBox == null)
		{
			return;
		}
		openingDialogueBox(firstLine);
	}

	// Token: 0x060038F6 RID: 14582 RVA: 0x000FB23A File Offset: 0x000F943A
	protected virtual void OnStartingDialogue()
	{
	}

	// Token: 0x060038F7 RID: 14583 RVA: 0x000FB23C File Offset: 0x000F943C
	protected virtual void OnStartDialogue()
	{
	}

	// Token: 0x060038F8 RID: 14584 RVA: 0x000FB23E File Offset: 0x000F943E
	protected virtual void OnNewLineStarted(DialogueBox.DialogueLine line)
	{
	}

	// Token: 0x060038F9 RID: 14585 RVA: 0x000FB240 File Offset: 0x000F9440
	protected virtual void OnLineEnded(DialogueBox.DialogueLine line)
	{
	}

	// Token: 0x060038FA RID: 14586 RVA: 0x000FB242 File Offset: 0x000F9442
	public virtual void OnDialogueBoxEnded()
	{
	}

	// Token: 0x060038FB RID: 14587 RVA: 0x000FB244 File Offset: 0x000F9444
	protected virtual void OnEndDialogue()
	{
	}

	// Token: 0x060038FC RID: 14588 RVA: 0x000FB246 File Offset: 0x000F9446
	public override void Interact()
	{
		this.StartDialogueMove();
	}

	// Token: 0x060038FD RID: 14589 RVA: 0x000FB24E File Offset: 0x000F944E
	private IEnumerator MovePlayer(Action onEnd)
	{
		NPCControlBase.<>c__DisplayClass100_0 CS$<>8__locals1 = new NPCControlBase.<>c__DisplayClass100_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.hc = HeroController.instance;
		if (CS$<>8__locals1.hc)
		{
			NPCControlBase.<>c__DisplayClass100_1 CS$<>8__locals2 = new NPCControlBase.<>c__DisplayClass100_1();
			CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
			base.DisableInteraction();
			NPCControlBase.TalkPositions talkPositions = this.talkPosition;
			if (!this.isLeftValid)
			{
				talkPositions = NPCControlBase.TalkPositions.Right;
			}
			else if (!this.isRightValid)
			{
				talkPositions = NPCControlBase.TalkPositions.Left;
			}
			float num = base.transform.position.x + this.centreOffset;
			CS$<>8__locals2.dir = 0;
			switch (talkPositions)
			{
			case NPCControlBase.TalkPositions.Any:
				CS$<>8__locals2.dir = (int)Mathf.Sign(CS$<>8__locals2.CS$<>8__locals1.hc.transform.position.x - num);
				break;
			case NPCControlBase.TalkPositions.Left:
				CS$<>8__locals2.dir = -1;
				break;
			case NPCControlBase.TalkPositions.Right:
				CS$<>8__locals2.dir = 1;
				break;
			}
			CS$<>8__locals2.targetX = num + this.targetDistance * (float)CS$<>8__locals2.dir;
			float f = CS$<>8__locals2.targetX - CS$<>8__locals2.CS$<>8__locals1.hc.transform.position.x;
			bool didForce = false;
			if (this.outsideRangeBehaviour == NPCControlBase.OutsideRangeBehaviours.ForceToDistance)
			{
				int num2 = (int)Mathf.Sign(f);
				if (num2 != CS$<>8__locals2.dir)
				{
					CS$<>8__locals2.dir = num2;
					didForce = true;
				}
			}
			Func<bool> shouldMove;
			switch (this.outsideRangeBehaviour)
			{
			case NPCControlBase.OutsideRangeBehaviours.None:
				if (Mathf.Abs(f) < 0.3f)
				{
					shouldMove = (() => false);
					goto IL_25D;
				}
				break;
			case NPCControlBase.OutsideRangeBehaviours.ForceToDistance:
				break;
			case NPCControlBase.OutsideRangeBehaviours.FaceDirection:
				shouldMove = (() => false);
				goto IL_25D;
			default:
				throw new ArgumentOutOfRangeException();
			}
			shouldMove = delegate()
			{
				if (CS$<>8__locals2.dir < 0)
				{
					if (CS$<>8__locals2.CS$<>8__locals1.hc.transform.position.x <= CS$<>8__locals2.targetX)
					{
						return false;
					}
				}
				else if (CS$<>8__locals2.CS$<>8__locals1.hc.transform.position.x >= CS$<>8__locals2.targetX)
				{
					return false;
				}
				return true;
			};
			IL_25D:
			tk2dSpriteAnimator animator = CS$<>8__locals2.CS$<>8__locals1.hc.GetComponent<tk2dSpriteAnimator>();
			HeroAnimationController heroAnim = CS$<>8__locals2.CS$<>8__locals1.hc.GetComponent<HeroAnimationController>();
			this.originalFacingDirection = CS$<>8__locals2.CS$<>8__locals1.hc.cState.facingRight;
			if (shouldMove())
			{
				CS$<>8__locals2.CS$<>8__locals1.hc.transform.SetScaleX((float)(-(float)CS$<>8__locals2.dir));
				this.didChangedSpriteDirection = true;
				Rigidbody2D body = CS$<>8__locals2.CS$<>8__locals1.hc.GetComponent<Rigidbody2D>();
				if (body)
				{
					body.linearVelocity = body.linearVelocity.Where(new float?(CS$<>8__locals2.CS$<>8__locals1.hc.GetWalkSpeed() * (float)CS$<>8__locals2.dir), null);
				}
				if (animator)
				{
					CS$<>8__locals2.CS$<>8__locals1.hc.StopAnimationControl();
					if ((CS$<>8__locals2.CS$<>8__locals1.hc.cState.facingRight && CS$<>8__locals2.dir < 0) || (!CS$<>8__locals2.CS$<>8__locals1.hc.cState.facingRight && CS$<>8__locals2.dir > 0))
					{
						float timeLeft = animator.PlayAnimGetTime(heroAnim.GetClip("TurnWalk"));
						while (timeLeft > 0f && shouldMove())
						{
							yield return null;
							timeLeft -= Time.deltaTime;
						}
					}
					animator.Play(heroAnim.GetClip("Walk"));
					CS$<>8__locals2.CS$<>8__locals1.hc.ForceWalkingSound = true;
				}
				Vector3 lastPosition = CS$<>8__locals2.CS$<>8__locals1.hc.transform.position;
				lastPosition.x += 100f;
				int unchangedFrames = 0;
				float elapsed = 0f;
				WaitForFixedUpdate fixedUpdate = new WaitForFixedUpdate();
				while (elapsed < 3f && shouldMove())
				{
					yield return fixedUpdate;
					elapsed += Time.deltaTime;
					Vector3 position = CS$<>8__locals2.CS$<>8__locals1.hc.transform.position;
					if (lastPosition == position)
					{
						int num3 = unchangedFrames;
						unchangedFrames = num3 + 1;
						if (unchangedFrames > 1)
						{
							break;
						}
					}
					else
					{
						unchangedFrames = 0;
						lastPosition = position;
					}
				}
				if (body)
				{
					body.linearVelocity = body.linearVelocity.Where(new float?(0f), null);
				}
				if (this.outsideRangeBehaviour == NPCControlBase.OutsideRangeBehaviours.ForceToDistance)
				{
					CS$<>8__locals2.CS$<>8__locals1.hc.transform.SetPositionX(CS$<>8__locals2.targetX);
				}
				body = null;
				lastPosition = default(Vector3);
				fixedUpdate = null;
			}
			int num4 = (int)Mathf.Sign(CS$<>8__locals2.CS$<>8__locals1.hc.transform.localScale.x);
			this.didChangedSpriteDirection = false;
			if ((didForce && CS$<>8__locals2.dir < 0) || (!didForce && CS$<>8__locals2.dir > 0))
			{
				CS$<>8__locals2.CS$<>8__locals1.hc.FaceLeft();
			}
			else
			{
				CS$<>8__locals2.CS$<>8__locals1.hc.FaceRight();
			}
			if ((int)Mathf.Sign(CS$<>8__locals2.CS$<>8__locals1.hc.transform.localScale.x) != num4 && animator && !didForce)
			{
				CS$<>8__locals2.CS$<>8__locals1.hc.StopAnimationControl();
				yield return base.StartCoroutine(animator.PlayAnimWait(heroAnim.GetClip("TurnWalk"), null));
			}
			if (!CS$<>8__locals2.CS$<>8__locals1.hc.HasAnimationControl)
			{
				CS$<>8__locals2.CS$<>8__locals1.hc.AnimCtrl.PlayIdle();
			}
			CS$<>8__locals2.CS$<>8__locals1.hc.ForceWalkingSound = false;
			CS$<>8__locals2.CS$<>8__locals1.hc.AddInputBlocker(this);
			this.blockedHeroInput = true;
			yield return null;
			base.EnableInteraction();
			if (!CS$<>8__locals2.CS$<>8__locals1.hc.HasAnimationControl)
			{
				CS$<>8__locals2.CS$<>8__locals1.hc.StartAnimationControl();
			}
			CS$<>8__locals2.CS$<>8__locals1.hc.RemoveInputBlocker(this);
			this.blockedHeroInput = false;
			CS$<>8__locals2 = null;
			shouldMove = null;
			animator = null;
			heroAnim = null;
		}
		if (base.IsQueued)
		{
			base.DisableInteraction();
			yield return new WaitUntil(() => !CS$<>8__locals1.<>4__this.IsQueued);
			base.EnableInteraction();
		}
		this.moveHeroRoutine = null;
		onEnd();
		yield break;
	}

	// Token: 0x060038FE RID: 14590 RVA: 0x000FB264 File Offset: 0x000F9464
	public void SetTalkPositionLeft()
	{
		this.talkPosition = NPCControlBase.TalkPositions.Left;
	}

	// Token: 0x060038FF RID: 14591 RVA: 0x000FB26D File Offset: 0x000F946D
	public void SetTalkPositionRight()
	{
		this.talkPosition = NPCControlBase.TalkPositions.Right;
	}

	// Token: 0x06003900 RID: 14592 RVA: 0x000FB276 File Offset: 0x000F9476
	public void SetTalkPositionAny()
	{
		this.talkPosition = NPCControlBase.TalkPositions.Any;
	}

	// Token: 0x06003901 RID: 14593 RVA: 0x000FB27F File Offset: 0x000F947F
	protected override void OnActivated()
	{
		this.CheckTalkSide();
	}

	// Token: 0x06003902 RID: 14594 RVA: 0x000FB288 File Offset: 0x000F9488
	private void CheckTalkSide()
	{
		this.isLeftValid = true;
		this.isRightValid = true;
		if (!this.checkGround)
		{
			return;
		}
		if (!this.DoMovePlayer)
		{
			return;
		}
		this.isRightValid = this.IsGroundSideValid(true);
		this.isLeftValid = this.IsGroundSideValid(false);
		if (!this.isLeftValid && !this.isRightValid)
		{
			base.Deactivate(false);
		}
	}

	// Token: 0x06003903 RID: 14595 RVA: 0x000FB2E8 File Offset: 0x000F94E8
	private bool IsGroundSideValid(bool isRight)
	{
		RaycastHit2D raycastHit2D;
		if (!Helper.IsRayHittingNoTriggers(base.transform.position + Vector3.up * 2f, Vector2.down, 5f, 256, out raycastHit2D))
		{
			return false;
		}
		Vector2 vector = raycastHit2D.point;
		vector.y += 0.1f;
		if (Helper.IsRayHittingNoTriggers(vector, isRight ? Vector2.right : Vector2.left, this.targetDistance, 256, out raycastHit2D))
		{
			vector = raycastHit2D.point;
		}
		else
		{
			vector += (isRight ? Vector2.right : Vector2.left) * this.targetDistance;
		}
		vector.x += (isRight ? -0.1f : 0.1f);
		return Helper.IsRayHittingNoTriggers(vector, Vector2.down, 0.2f, 256);
	}

	// Token: 0x04003BCD RID: 15309
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("AllowMovePlayer", true, false, false)]
	private NPCControlBase.TalkPositions talkPosition;

	// Token: 0x04003BCE RID: 15310
	[SerializeField]
	[ModifiableProperty]
	[Conditional("AllowMovePlayer", true, false, false)]
	private float centreOffset;

	// Token: 0x04003BCF RID: 15311
	[SerializeField]
	[ModifiableProperty]
	[Conditional("AllowMovePlayer", true, false, false)]
	private float targetDistance = 2f;

	// Token: 0x04003BD0 RID: 15312
	[SerializeField]
	[ModifiableProperty]
	[Conditional("AllowMovePlayer", true, false, false)]
	private NPCControlBase.OutsideRangeBehaviours outsideRangeBehaviour;

	// Token: 0x04003BD1 RID: 15313
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool forceToDistance;

	// Token: 0x04003BD2 RID: 15314
	[SerializeField]
	private bool checkGround;

	// Token: 0x04003BD3 RID: 15315
	[SerializeField]
	private HeroTalkAnimation.AnimationTypes heroAnimation;

	// Token: 0x04003BD4 RID: 15316
	[SerializeField]
	private NPCControlBase.HeroAnimBeginTypes heroAnimBegin;

	// Token: 0x04003BD5 RID: 15317
	[SerializeField]
	private bool overrideHeroHurtAnim;

	// Token: 0x04003BD6 RID: 15318
	private bool isWaitingToBegin;

	// Token: 0x04003BD7 RID: 15319
	private Coroutine moveHeroRoutine;

	// Token: 0x04003BD8 RID: 15320
	private bool isLeftValid = true;

	// Token: 0x04003BD9 RID: 15321
	private bool isRightValid = true;

	// Token: 0x04003BDA RID: 15322
	private bool blockedHeroInput;

	// Token: 0x04003BDB RID: 15323
	private bool nextSceneRegistered;

	// Token: 0x04003BDC RID: 15324
	private bool didChangedSpriteDirection;

	// Token: 0x04003BDD RID: 15325
	private bool originalFacingDirection;

	// Token: 0x02001951 RID: 6481
	public enum TalkPositions
	{
		// Token: 0x0400954F RID: 38223
		Any,
		// Token: 0x04009550 RID: 38224
		Left,
		// Token: 0x04009551 RID: 38225
		Right
	}

	// Token: 0x02001952 RID: 6482
	public enum OutsideRangeBehaviours
	{
		// Token: 0x04009553 RID: 38227
		None,
		// Token: 0x04009554 RID: 38228
		ForceToDistance,
		// Token: 0x04009555 RID: 38229
		FaceDirection
	}

	// Token: 0x02001953 RID: 6483
	public enum HeroAnimBeginTypes
	{
		// Token: 0x04009557 RID: 38231
		Auto,
		// Token: 0x04009558 RID: 38232
		Manual
	}
}
