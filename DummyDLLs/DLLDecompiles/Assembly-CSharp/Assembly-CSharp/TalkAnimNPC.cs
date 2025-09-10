using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200063F RID: 1599
public abstract class TalkAnimNPC : MonoBehaviour
{
	// Token: 0x140000BE RID: 190
	// (add) Token: 0x06003957 RID: 14679 RVA: 0x000FC4FC File Offset: 0x000FA6FC
	// (remove) Token: 0x06003958 RID: 14680 RVA: 0x000FC534 File Offset: 0x000FA734
	public event Action Stopped;

	// Token: 0x17000685 RID: 1669
	// (get) Token: 0x06003959 RID: 14681 RVA: 0x000FC569 File Offset: 0x000FA769
	// (set) Token: 0x0600395A RID: 14682 RVA: 0x000FC571 File Offset: 0x000FA771
	public string IdleAnim
	{
		get
		{
			return this.idleAnim;
		}
		set
		{
			this.idleAnim = value;
		}
	}

	// Token: 0x17000686 RID: 1670
	// (get) Token: 0x0600395B RID: 14683 RVA: 0x000FC57A File Offset: 0x000FA77A
	// (set) Token: 0x0600395C RID: 14684 RVA: 0x000FC582 File Offset: 0x000FA782
	public bool IsInConversation
	{
		get
		{
			return this.isInConversation;
		}
		set
		{
			this.isInConversation = value;
		}
	}

	// Token: 0x0600395D RID: 14685 RVA: 0x000FC58C File Offset: 0x000FA78C
	private void OnValidate()
	{
		if (!string.IsNullOrEmpty(this.talkEnterAnim))
		{
			this.talkLeftAnims.TalkEnterAnim = this.talkEnterAnim;
			this.talkRightAnims.TalkEnterAnim = this.talkEnterAnim;
			this.talkEnterAnim = null;
		}
		if (!string.IsNullOrEmpty(this.talkAnim))
		{
			this.talkLeftAnims.TalkAnim = this.talkAnim;
			this.talkRightAnims.TalkAnim = this.talkAnim;
			this.talkAnim = null;
		}
		if (!string.IsNullOrEmpty(this.talkToListenAnim))
		{
			this.talkLeftAnims.TalkToListenAnim = this.talkToListenAnim;
			this.talkRightAnims.TalkToListenAnim = this.talkToListenAnim;
			this.talkToListenAnim = null;
		}
		if (!string.IsNullOrEmpty(this.listenAnim))
		{
			this.talkLeftAnims.ListenAnim = this.listenAnim;
			this.talkRightAnims.ListenAnim = this.listenAnim;
			this.listenAnim = null;
		}
		if (!string.IsNullOrEmpty(this.listenToTalkAnim))
		{
			this.talkLeftAnims.ListenToTalkAnim = this.listenToTalkAnim;
			this.talkRightAnims.ListenToTalkAnim = this.listenToTalkAnim;
			this.listenToTalkAnim = null;
		}
		if (!string.IsNullOrEmpty(this.talkExitAnim))
		{
			this.talkLeftAnims.TalkExitAnim = this.talkExitAnim;
			this.talkRightAnims.TalkExitAnim = this.talkExitAnim;
			this.talkExitAnim = null;
		}
	}

	// Token: 0x0600395E RID: 14686 RVA: 0x000FC6E0 File Offset: 0x000FA8E0
	protected virtual void Awake()
	{
		this.OnValidate();
		if (string.IsNullOrEmpty(this.talkLeftAnims.TalkAnim))
		{
			this.talkLeftAnims.TalkAnim = this.talkLeftAnims.ListenAnim;
		}
		if (string.IsNullOrEmpty(this.talkRightAnims.TalkAnim))
		{
			this.talkRightAnims.TalkAnim = this.talkRightAnims.ListenAnim;
		}
	}

	// Token: 0x0600395F RID: 14687 RVA: 0x000FC743 File Offset: 0x000FA943
	private void OnEnable()
	{
		if (!this.hasStarted)
		{
			return;
		}
		if (!this.StopIfRequested())
		{
			this.StartAnimation();
		}
	}

	// Token: 0x06003960 RID: 14688 RVA: 0x000FC75C File Offset: 0x000FA95C
	private void OnDisable()
	{
		if (this.animationRoutine == null)
		{
			return;
		}
		base.StopCoroutine(this.animationRoutine);
		this.animationRoutine = null;
	}

	// Token: 0x06003961 RID: 14689 RVA: 0x000FC77C File Offset: 0x000FA97C
	private void Start()
	{
		this.hasStarted = true;
		this.OnEnable();
		if (!this.control)
		{
			return;
		}
		this.control.StartedDialogue += delegate()
		{
			this.isInConversation = true;
		};
		this.control.StartedNewLine += delegate(DialogueBox.DialogueLine line)
		{
			this.isTalking = line.IsNpcEvent(this.talkingPageEvent);
		};
		this.control.EndingDialogue += delegate()
		{
			this.isTalking = false;
		};
		this.control.EndedDialogue += delegate()
		{
			this.isInConversation = false;
		};
	}

	// Token: 0x06003962 RID: 14690 RVA: 0x000FC800 File Offset: 0x000FAA00
	public void SetForceTalkEnter()
	{
		this.forceTalkEnter = true;
	}

	// Token: 0x06003963 RID: 14691 RVA: 0x000FC809 File Offset: 0x000FAA09
	public void StartAnimation()
	{
		if (this.animationRoutine != null)
		{
			this.StopAnimation(true);
		}
		this.animationRoutine = base.StartCoroutine(this.Animation());
	}

	// Token: 0x06003964 RID: 14692 RVA: 0x000FC82C File Offset: 0x000FAA2C
	public void StopAnimation(bool instant)
	{
		this.shouldStop = true;
		if (!instant)
		{
			if (this.animationRoutine == null)
			{
				Action stopped = this.Stopped;
				if (stopped == null)
				{
					return;
				}
				stopped();
			}
			return;
		}
		if (this.animationRoutine != null)
		{
			base.StopCoroutine(this.animationRoutine);
			this.animationRoutine = null;
		}
		Action stopped2 = this.Stopped;
		if (stopped2 == null)
		{
			return;
		}
		stopped2();
	}

	// Token: 0x06003965 RID: 14693 RVA: 0x000FC887 File Offset: 0x000FAA87
	private IEnumerator Animation()
	{
		this.shouldStop = false;
		bool wasInConversation = false;
		while (!this.StopIfRequested())
		{
			if (!wasInConversation)
			{
				this.PlayIdle();
				while (!this.StopIfRequested())
				{
					yield return null;
					if (this.isInConversation)
					{
						goto IL_87;
					}
				}
				yield break;
			}
			IL_87:
			this.StopIdle();
			Vector3 pos = base.transform.position;
			bool flag = HeroController.instance.transform.position.x > pos.x;
			if (base.transform.localScale.x < 0f)
			{
				flag = !flag;
			}
			TalkAnimNPC.TalkAnims talkAnims = flag ? this.talkRightAnims : this.talkLeftAnims;
			if (!wasInConversation || this.forceTalkEnter || string.IsNullOrEmpty(talkAnims.TalkAnim))
			{
				this.talkEnterAudio.SpawnAndPlayOneShot(pos, null);
				this.talkEnterAudioTable.SpawnAndPlayOneShot(pos, false);
				if (!string.IsNullOrEmpty(talkAnims.TalkEnterAnim))
				{
					yield return base.StartCoroutine(this.PlayAnimWait(talkAnims.TalkEnterAnim));
				}
			}
			wasInConversation = true;
			bool wasTalking = false;
			bool firstLoop = true;
			bool hasTalkAnim = !string.IsNullOrEmpty(talkAnims.TalkAnim);
			bool hasListenAnim = !string.IsNullOrEmpty(talkAnims.ListenAnim);
			while (this.isInConversation && !this.StopIfRequested())
			{
				if (!firstLoop)
				{
					if (this.isTalking && !wasTalking)
					{
						if (!string.IsNullOrEmpty(talkAnims.ListenToTalkAnim))
						{
							yield return base.StartCoroutine(this.PlayAnimWait(talkAnims.ListenToTalkAnim));
						}
					}
					else if (!this.isTalking && wasTalking && !string.IsNullOrEmpty(talkAnims.TalkToListenAnim))
					{
						yield return base.StartCoroutine(this.PlayAnimWait(talkAnims.TalkToListenAnim));
					}
				}
				string text = this.isTalking ? talkAnims.TalkAnim : talkAnims.ListenAnim;
				bool flag2 = !string.IsNullOrEmpty(text);
				if (!flag2)
				{
					if (!this.isTalking)
					{
						if (hasTalkAnim)
						{
							text = talkAnims.TalkAnim;
							flag2 = true;
						}
					}
					else if (hasListenAnim)
					{
						text = talkAnims.ListenAnim;
						flag2 = true;
					}
				}
				if (flag2)
				{
					this.PlayAnim(text);
				}
				wasTalking = this.isTalking;
				firstLoop = false;
				yield return null;
			}
			float delay = this.talkExitDelay.GetRandomValue();
			if (delay > 0f)
			{
				if (wasTalking && !string.IsNullOrEmpty(talkAnims.ListenAnim))
				{
					this.PlayAnim(talkAnims.ListenAnim);
				}
				float elapsed = 0f;
				while (elapsed < delay && !this.shouldStop)
				{
					yield return null;
					elapsed += Time.deltaTime;
				}
			}
			if (!this.isInConversation)
			{
				wasInConversation = false;
				this.talkExitAudio.SpawnAndPlayOneShot(pos, null);
				this.talkExitAudioTable.SpawnAndPlayOneShot(pos, false);
				if (!string.IsNullOrEmpty(talkAnims.TalkExitAnim))
				{
					yield return base.StartCoroutine(this.PlayAnimWait(talkAnims.TalkExitAnim));
				}
				this.PlayIdle();
				pos = default(Vector3);
				talkAnims = default(TalkAnimNPC.TalkAnims);
			}
		}
		this.animationRoutine = null;
		yield break;
	}

	// Token: 0x06003966 RID: 14694 RVA: 0x000FC896 File Offset: 0x000FAA96
	private void PlayIdle()
	{
		if (!string.IsNullOrEmpty(this.idleAnim))
		{
			this.PlayAnim(this.idleAnim);
		}
		if (this.idleAudioLoop)
		{
			this.idleAudioLoop.Play();
		}
	}

	// Token: 0x06003967 RID: 14695 RVA: 0x000FC8C9 File Offset: 0x000FAAC9
	private void StopIdle()
	{
		if (this.idleAudioLoop)
		{
			this.idleAudioLoop.Stop();
		}
	}

	// Token: 0x06003968 RID: 14696 RVA: 0x000FC8E3 File Offset: 0x000FAAE3
	private bool StopIfRequested()
	{
		if (!this.shouldStop)
		{
			return false;
		}
		if (this.Stopped != null)
		{
			this.Stopped();
		}
		this.animationRoutine = null;
		return true;
	}

	// Token: 0x06003969 RID: 14697
	protected abstract void PlayAnim(string animName);

	// Token: 0x0600396A RID: 14698
	protected abstract IEnumerator PlayAnimWait(string animName);

	// Token: 0x04003C1E RID: 15390
	[SerializeField]
	private NPCControlBase control;

	// Token: 0x04003C1F RID: 15391
	[SerializeField]
	private string talkingPageEvent;

	// Token: 0x04003C20 RID: 15392
	[SerializeField]
	private string idleAnim;

	// Token: 0x04003C21 RID: 15393
	[SerializeField]
	private AudioSource idleAudioLoop;

	// Token: 0x04003C22 RID: 15394
	[SerializeField]
	private AudioEventRandom talkEnterAudio;

	// Token: 0x04003C23 RID: 15395
	[SerializeField]
	private RandomAudioClipTable talkEnterAudioTable;

	// Token: 0x04003C24 RID: 15396
	[SerializeField]
	private AudioEventRandom talkExitAudio;

	// Token: 0x04003C25 RID: 15397
	[SerializeField]
	private RandomAudioClipTable talkExitAudioTable;

	// Token: 0x04003C26 RID: 15398
	[SerializeField]
	private TalkAnimNPC.TalkAnims talkLeftAnims;

	// Token: 0x04003C27 RID: 15399
	[SerializeField]
	private TalkAnimNPC.TalkAnims talkRightAnims;

	// Token: 0x04003C28 RID: 15400
	[SerializeField]
	private MinMaxFloat talkExitDelay;

	// Token: 0x04003C29 RID: 15401
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string talkEnterAnim;

	// Token: 0x04003C2A RID: 15402
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string talkAnim;

	// Token: 0x04003C2B RID: 15403
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string talkToListenAnim;

	// Token: 0x04003C2C RID: 15404
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string listenAnim;

	// Token: 0x04003C2D RID: 15405
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string listenToTalkAnim;

	// Token: 0x04003C2E RID: 15406
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private string talkExitAnim;

	// Token: 0x04003C2F RID: 15407
	private bool hasStarted;

	// Token: 0x04003C30 RID: 15408
	private bool isInConversation;

	// Token: 0x04003C31 RID: 15409
	private bool isTalking;

	// Token: 0x04003C32 RID: 15410
	private bool shouldStop;

	// Token: 0x04003C33 RID: 15411
	private bool forceTalkEnter;

	// Token: 0x04003C34 RID: 15412
	private Coroutine animationRoutine;

	// Token: 0x02001960 RID: 6496
	[Serializable]
	private struct TalkAnims
	{
		// Token: 0x04009580 RID: 38272
		public string TalkEnterAnim;

		// Token: 0x04009581 RID: 38273
		public string TalkAnim;

		// Token: 0x04009582 RID: 38274
		public string TalkToListenAnim;

		// Token: 0x04009583 RID: 38275
		public string ListenAnim;

		// Token: 0x04009584 RID: 38276
		public string ListenToTalkAnim;

		// Token: 0x04009585 RID: 38277
		public string TalkExitAnim;
	}
}
