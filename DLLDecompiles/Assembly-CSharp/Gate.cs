using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004E9 RID: 1257
public class Gate : UnlockablePropBase
{
	// Token: 0x06002D09 RID: 11529 RVA: 0x000C4A56 File Offset: 0x000C2C56
	private void OnValidate()
	{
		if (this.openClip)
		{
			this.openAnim = this.openClip.name;
			this.openClip = null;
		}
		if (this.startAtOpenStart)
		{
			this.startState = Gate.StartStates.FreezeAtOpenStart;
			this.startAtOpenStart = false;
		}
	}

	// Token: 0x06002D0A RID: 11530 RVA: 0x000C4A93 File Offset: 0x000C2C93
	private void Reset()
	{
		this.animator = base.GetComponent<Animator>();
	}

	// Token: 0x06002D0B RID: 11531 RVA: 0x000C4AA4 File Offset: 0x000C2CA4
	private void Awake()
	{
		this.OnValidate();
		if (!this.animator)
		{
			this.animator = base.GetComponent<Animator>();
		}
		if (this.animator)
		{
			this.initialCullingMode = this.animator.cullingMode;
		}
		this.source = base.GetComponent<AudioSource>();
	}

	// Token: 0x06002D0C RID: 11532 RVA: 0x000C4AFC File Offset: 0x000C2CFC
	private void Start()
	{
		PersistentBoolItem component = base.GetComponent<PersistentBoolItem>();
		if (component)
		{
			component.OnGetSaveState += delegate(out bool value)
			{
				value = this.activated;
			};
			component.OnSetSaveState += delegate(bool value)
			{
				this.activated = value;
				if (this.activated)
				{
					this.Opened();
				}
			};
		}
		if (!this.activated && this.animator)
		{
			switch (this.startState)
			{
			case Gate.StartStates.Default:
				break;
			case Gate.StartStates.FreezeAtOpenStart:
				this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
				this.animator.Play(this.openAnim);
				this.animator.enabled = false;
				this.animator.Update(0f);
				this.animator.playbackTime = 0f;
				this.animator.cullingMode = this.initialCullingMode;
				return;
			case Gate.StartStates.StartOpen:
				if (!string.IsNullOrEmpty(this.openedAnim))
				{
					this.animator.Play(this.openedAnim);
					return;
				}
				this.animator.Play(this.openAnim, 0, 1f);
				return;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}

	// Token: 0x06002D0D RID: 11533 RVA: 0x000C4C10 File Offset: 0x000C2E10
	public override void Open()
	{
		if (this.startState == Gate.StartStates.StartOpen && !this.isClosed)
		{
			return;
		}
		if (this.activated)
		{
			return;
		}
		this.activated = true;
		float randomValue = this.openDelay.GetRandomValue();
		this.OpenWithDelay(randomValue);
	}

	// Token: 0x06002D0E RID: 11534 RVA: 0x000C4C52 File Offset: 0x000C2E52
	public void OpenWithDelay(float delay)
	{
		if (delay <= 0f)
		{
			this.onBeforeDelay.Invoke();
			this.DoOpen();
			return;
		}
		this.onBeforeDelay.Invoke();
		this.ExecuteDelayed(delay, new Action(this.DoOpen));
	}

	// Token: 0x06002D0F RID: 11535 RVA: 0x000C4C90 File Offset: 0x000C2E90
	private void DoOpen()
	{
		this.onOpen.Invoke();
		this.isClosed = false;
		if (this.animator)
		{
			this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			this.animator.enabled = true;
			this.animator.Play(this.openAnim);
		}
		if (this.openChildDelay <= 0f)
		{
			foreach (UnlockablePropBase unlockablePropBase in this.openChildren)
			{
				if (unlockablePropBase)
				{
					unlockablePropBase.Open();
				}
			}
			return;
		}
		foreach (UnlockablePropBase unlockablePropBase2 in this.openChildren)
		{
			if (unlockablePropBase2)
			{
				unlockablePropBase2.ExecuteDelayed(this.openChildDelay, new Action(unlockablePropBase2.Open));
			}
		}
	}

	// Token: 0x06002D10 RID: 11536 RVA: 0x000C4D58 File Offset: 0x000C2F58
	public override void Opened()
	{
		this.activated = true;
		this.isClosed = false;
		this.onOpened.Invoke();
		if (this.deactivateIfOpened)
		{
			base.gameObject.SetActive(false);
		}
		else if (this.animator)
		{
			this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			this.animator.enabled = true;
			if (!string.IsNullOrEmpty(this.openedAnim))
			{
				this.animator.Play(this.openedAnim);
			}
			else
			{
				this.animator.Play(this.openAnim, 0, 1f);
			}
			this.animator.Update(0f);
			this.animator.cullingMode = this.initialCullingMode;
		}
		foreach (UnlockablePropBase unlockablePropBase in this.openChildren)
		{
			if (unlockablePropBase)
			{
				unlockablePropBase.Opened();
			}
		}
	}

	// Token: 0x06002D11 RID: 11537 RVA: 0x000C4E38 File Offset: 0x000C3038
	public void Close()
	{
		if (this.startState == Gate.StartStates.StartOpen)
		{
			this.ForceClose();
			return;
		}
		this.DoClose();
	}

	// Token: 0x06002D12 RID: 11538 RVA: 0x000C4E50 File Offset: 0x000C3050
	public void ForceClose()
	{
		this.onClose.Invoke();
		this.isClosed = true;
		if (this.animator)
		{
			this.animator.enabled = true;
			this.animator.Play(this.closeAnim);
		}
	}

	// Token: 0x06002D13 RID: 11539 RVA: 0x000C4E8E File Offset: 0x000C308E
	private void DoClose()
	{
		if (!this.activated)
		{
			return;
		}
		this.ForceClose();
		this.activated = false;
	}

	// Token: 0x06002D14 RID: 11540 RVA: 0x000C4EA6 File Offset: 0x000C30A6
	public void PlaySound()
	{
		if (this.source)
		{
			this.source.Play();
		}
	}

	// Token: 0x06002D15 RID: 11541 RVA: 0x000C4EC0 File Offset: 0x000C30C0
	public void StartRumble()
	{
		GameCameras gameCameras = Object.FindObjectOfType<GameCameras>();
		if (gameCameras)
		{
			gameCameras.cameraShakeFSM.SendEvent("AverageShake");
		}
	}

	// Token: 0x04002EA9 RID: 11945
	[SerializeField]
	private Animator animator;

	// Token: 0x04002EAA RID: 11946
	[SerializeField]
	private string openAnim;

	// Token: 0x04002EAB RID: 11947
	[SerializeField]
	private string closeAnim;

	// Token: 0x04002EAC RID: 11948
	[SerializeField]
	private Gate.StartStates startState;

	// Token: 0x04002EAD RID: 11949
	[SerializeField]
	private string openedAnim;

	// Token: 0x04002EAE RID: 11950
	[SerializeField]
	private bool deactivateIfOpened;

	// Token: 0x04002EAF RID: 11951
	[SerializeField]
	private MinMaxFloat openDelay;

	// Token: 0x04002EB0 RID: 11952
	[Space]
	[SerializeField]
	private UnityEvent onBeforeDelay;

	// Token: 0x04002EB1 RID: 11953
	[SerializeField]
	private UnityEvent onOpen;

	// Token: 0x04002EB2 RID: 11954
	[SerializeField]
	private UnityEvent onOpened;

	// Token: 0x04002EB3 RID: 11955
	[SerializeField]
	private UnityEvent onClose;

	// Token: 0x04002EB4 RID: 11956
	[Space]
	[SerializeField]
	private UnlockablePropBase[] openChildren;

	// Token: 0x04002EB5 RID: 11957
	[SerializeField]
	private float openChildDelay;

	// Token: 0x04002EB6 RID: 11958
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private AnimationClip openClip;

	// Token: 0x04002EB7 RID: 11959
	[SerializeField]
	[HideInInspector]
	[Obsolete]
	private bool startAtOpenStart;

	// Token: 0x04002EB8 RID: 11960
	private AnimatorCullingMode initialCullingMode;

	// Token: 0x04002EB9 RID: 11961
	private AudioSource source;

	// Token: 0x04002EBA RID: 11962
	private bool activated;

	// Token: 0x04002EBB RID: 11963
	private bool isClosed;

	// Token: 0x020017EE RID: 6126
	private enum StartStates
	{
		// Token: 0x04008FFA RID: 36858
		Default,
		// Token: 0x04008FFB RID: 36859
		FreezeAtOpenStart,
		// Token: 0x04008FFC RID: 36860
		StartOpen
	}
}
