using System;
using UnityEngine;

// Token: 0x0200013A RID: 314
public sealed class tk2dFootstepPlayer : FootstepPlayer
{
	// Token: 0x060009A8 RID: 2472 RVA: 0x0002BF5A File Offset: 0x0002A15A
	protected override void Awake()
	{
		base.Awake();
		if (this.animator == null)
		{
			this.animator = base.GetComponent<tk2dSpriteAnimator>();
			this.animator == null;
			return;
		}
	}

	// Token: 0x060009A9 RID: 2473 RVA: 0x0002BF8A File Offset: 0x0002A18A
	protected override void OnValidate()
	{
		base.OnValidate();
		if (this.animator == null)
		{
			this.animator = base.GetComponent<tk2dSpriteAnimator>();
		}
	}

	// Token: 0x060009AA RID: 2474 RVA: 0x0002BFAC File Offset: 0x0002A1AC
	private void OnEnable()
	{
		this.RegisterEvents();
	}

	// Token: 0x060009AB RID: 2475 RVA: 0x0002BFB4 File Offset: 0x0002A1B4
	private void OnDisable()
	{
		this.UnregisterEvents();
	}

	// Token: 0x060009AC RID: 2476 RVA: 0x0002BFBC File Offset: 0x0002A1BC
	private void RegisterEvents()
	{
		if (!this.registeredEvent && this.animator)
		{
			this.registeredEvent = true;
			this.animator.AnimationEventTriggeredEvent += this.AnimationEventTriggered;
		}
	}

	// Token: 0x060009AD RID: 2477 RVA: 0x0002BFF1 File Offset: 0x0002A1F1
	private void UnregisterEvents()
	{
		if (this.registeredEvent && this.animator)
		{
			this.registeredEvent = false;
			this.animator.AnimationEventTriggeredEvent -= this.AnimationEventTriggered;
		}
	}

	// Token: 0x060009AE RID: 2478 RVA: 0x0002C026 File Offset: 0x0002A226
	private void AnimationEventTriggered(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip, int frame)
	{
		if (this.footstepTrigger.TriggerMatched(clip.frames[frame]))
		{
			base.PlayFootstep();
		}
	}

	// Token: 0x0400093D RID: 2365
	[SerializeField]
	private tk2dSpriteAnimator animator;

	// Token: 0x0400093E RID: 2366
	[SerializeField]
	private tk2dFootstepPlayer.tk2dEventTrigger footstepTrigger = new tk2dFootstepPlayer.tk2dEventTrigger
	{
		eventString = "Footstep"
	};

	// Token: 0x0400093F RID: 2367
	private bool registeredEvent;

	// Token: 0x02001472 RID: 5234
	[Serializable]
	public sealed class tk2dEventTrigger
	{
		// Token: 0x0600838D RID: 33677 RVA: 0x00269367 File Offset: 0x00267567
		private bool IsString()
		{
			return this.triggerType == tk2dFootstepPlayer.EventType.String;
		}

		// Token: 0x0600838E RID: 33678 RVA: 0x00269372 File Offset: 0x00267572
		private bool IsInt()
		{
			return this.triggerType == tk2dFootstepPlayer.EventType.Int;
		}

		// Token: 0x0600838F RID: 33679 RVA: 0x0026937D File Offset: 0x0026757D
		private bool IsFloat()
		{
			return this.triggerType == tk2dFootstepPlayer.EventType.Float;
		}

		// Token: 0x06008390 RID: 33680 RVA: 0x00269388 File Offset: 0x00267588
		public bool TriggerMatched(tk2dSpriteAnimationFrame frame)
		{
			switch (this.triggerType)
			{
			case tk2dFootstepPlayer.EventType.String:
				return frame.eventInfo == this.eventString;
			case tk2dFootstepPlayer.EventType.Int:
				return frame.eventInt == this.eventInt;
			case tk2dFootstepPlayer.EventType.Float:
				return frame.eventFloat == this.eventFloat;
			default:
				return false;
			}
		}

		// Token: 0x04008333 RID: 33587
		public tk2dFootstepPlayer.EventType triggerType;

		// Token: 0x04008334 RID: 33588
		[ModifiableProperty]
		[Conditional("IsString", true, true, true)]
		public string eventString;

		// Token: 0x04008335 RID: 33589
		[ModifiableProperty]
		[Conditional("IsInt", true, true, true)]
		public int eventInt;

		// Token: 0x04008336 RID: 33590
		[ModifiableProperty]
		[Conditional("IsFloat", true, true, true)]
		public float eventFloat;
	}

	// Token: 0x02001473 RID: 5235
	public enum EventType
	{
		// Token: 0x04008338 RID: 33592
		String,
		// Token: 0x04008339 RID: 33593
		Int,
		// Token: 0x0400833A RID: 33594
		Float
	}
}
