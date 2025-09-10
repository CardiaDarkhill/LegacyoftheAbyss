using System;
using TeamCherry.NestedFadeGroup;
using UnityEngine;

// Token: 0x0200006E RID: 110
public class ChainSequence : SkippableSequence
{
	// Token: 0x17000026 RID: 38
	// (get) Token: 0x060002DF RID: 735 RVA: 0x0000FB45 File Offset: 0x0000DD45
	public SkippableSequence CurrentSequence
	{
		get
		{
			if (this.currentSequenceIndex < 0 || this.currentSequenceIndex >= this.sequences.Length)
			{
				return null;
			}
			return this.sequences[this.currentSequenceIndex];
		}
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x060002E0 RID: 736 RVA: 0x0000FB6F File Offset: 0x0000DD6F
	public bool IsCurrentSkipped
	{
		get
		{
			return this.CurrentSequence != null && this.CurrentSequence.IsSkipped;
		}
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x060002E1 RID: 737 RVA: 0x0000FB8C File Offset: 0x0000DD8C
	public bool CanSkipCurrent
	{
		get
		{
			return this.CurrentSequence != null && this.CurrentSequence.CanSkip;
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x060002E2 RID: 738 RVA: 0x0000FBA9 File Offset: 0x0000DDA9
	public override bool IsSkipped
	{
		get
		{
			return this.isSkipped;
		}
	}

	// Token: 0x14000005 RID: 5
	// (add) Token: 0x060002E3 RID: 739 RVA: 0x0000FBB4 File Offset: 0x0000DDB4
	// (remove) Token: 0x060002E4 RID: 740 RVA: 0x0000FBEC File Offset: 0x0000DDEC
	public event Action TransitionedToNextSequence;

	// Token: 0x14000006 RID: 6
	// (add) Token: 0x060002E5 RID: 741 RVA: 0x0000FC24 File Offset: 0x0000DE24
	// (remove) Token: 0x060002E6 RID: 742 RVA: 0x0000FC5C File Offset: 0x0000DE5C
	public event Action EndBlankerFade;

	// Token: 0x14000007 RID: 7
	// (add) Token: 0x060002E7 RID: 743 RVA: 0x0000FC94 File Offset: 0x0000DE94
	// (remove) Token: 0x060002E8 RID: 744 RVA: 0x0000FCCC File Offset: 0x0000DECC
	public event Action SequenceComplete;

	// Token: 0x060002E9 RID: 745 RVA: 0x0000FD01 File Offset: 0x0000DF01
	protected void Awake()
	{
		this.fadeByController = 1f;
		if (this.endBlanker)
		{
			this.endBlanker.AlphaSelf = 0f;
		}
	}

	// Token: 0x060002EA RID: 746 RVA: 0x0000FD2C File Offset: 0x0000DF2C
	protected void Update()
	{
		if (this.isSkipped || this.CurrentSequence == null)
		{
			return;
		}
		if (this.CurrentSequence.IsPlaying)
		{
			return;
		}
		if (!this.CurrentSequence.WaitForSkip || this.CurrentSequence.IsSkipped)
		{
			this.Next();
		}
	}

	// Token: 0x060002EB RID: 747 RVA: 0x0000FD7E File Offset: 0x0000DF7E
	public override void Begin()
	{
		this.isSkipped = false;
		this.currentSequenceIndex = -1;
		this.Next();
	}

	// Token: 0x060002EC RID: 748 RVA: 0x0000FD94 File Offset: 0x0000DF94
	private void Next()
	{
		SkippableSequence currentSequence = this.CurrentSequence;
		this.currentSequenceIndex++;
		if (currentSequence != null && (!this.keepLastActive || this.CurrentSequence))
		{
			currentSequence.SetActive(false);
		}
		if (this.isSkipped)
		{
			return;
		}
		if (this.CurrentSequence != null)
		{
			if (!this.CurrentSequence.ShouldShow)
			{
				this.Next();
				return;
			}
			this.CurrentSequence.SetActive(true);
			this.CurrentSequence.Begin();
			Action transitionedToNextSequence = this.TransitionedToNextSequence;
			if (transitionedToNextSequence == null)
			{
				return;
			}
			transitionedToNextSequence();
			return;
		}
		else
		{
			if (this.endBlanker)
			{
				Action endBlankerFade = this.EndBlankerFade;
				if (endBlankerFade != null)
				{
					endBlankerFade();
				}
				this.StartTimerRoutine(0f, 1f, delegate(float t)
				{
					this.endBlanker.AlphaSelf = t;
				}, null, this.SequenceComplete, false);
				return;
			}
			Action sequenceComplete = this.SequenceComplete;
			if (sequenceComplete == null)
			{
				return;
			}
			sequenceComplete();
			return;
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x060002ED RID: 749 RVA: 0x0000FE82 File Offset: 0x0000E082
	public override bool IsPlaying
	{
		get
		{
			return this.currentSequenceIndex < this.sequences.Length - 1 || (this.CurrentSequence != null && this.CurrentSequence.IsPlaying);
		}
	}

	// Token: 0x060002EE RID: 750 RVA: 0x0000FEB4 File Offset: 0x0000E0B4
	public override void Skip()
	{
		this.isSkipped = true;
		SkippableSequence[] array = this.sequences;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Skip();
		}
	}

	// Token: 0x060002EF RID: 751 RVA: 0x0000FEE5 File Offset: 0x0000E0E5
	public void SkipSingle()
	{
		if (this.CurrentSequence == null)
		{
			return;
		}
		if (!this.CurrentSequence.CanSkip)
		{
			return;
		}
		this.CurrentSequence.Skip();
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x060002F0 RID: 752 RVA: 0x0000FF0F File Offset: 0x0000E10F
	// (set) Token: 0x060002F1 RID: 753 RVA: 0x0000FF18 File Offset: 0x0000E118
	public override float FadeByController
	{
		get
		{
			return this.fadeByController;
		}
		set
		{
			this.fadeByController = Mathf.Clamp01(value);
			SkippableSequence[] array = this.sequences;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].FadeByController = this.fadeByController;
			}
		}
	}

	// Token: 0x04000273 RID: 627
	[SerializeField]
	private SkippableSequence[] sequences;

	// Token: 0x04000274 RID: 628
	[SerializeField]
	private bool keepLastActive;

	// Token: 0x04000275 RID: 629
	[SerializeField]
	private NestedFadeGroupBase endBlanker;

	// Token: 0x04000276 RID: 630
	private int currentSequenceIndex = -1;

	// Token: 0x04000277 RID: 631
	private float fadeByController;

	// Token: 0x04000278 RID: 632
	private bool isSkipped;
}
