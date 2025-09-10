using System;
using System.Runtime.CompilerServices;
using TMProOld;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200135D RID: 4957
	public abstract class RunDialogueBase : FSMUtility.GetComponentFsmStateAction<PlayMakerNPC>
	{
		// Token: 0x17000C3A RID: 3130
		// (get) Token: 0x06007FE0 RID: 32736
		protected abstract string DialogueText { get; }

		// Token: 0x06007FE1 RID: 32737 RVA: 0x0025CA3C File Offset: 0x0025AC3C
		public override void Reset()
		{
			base.Reset();
			this.OverrideContinue = null;
			this.PlayerVoiceTableOverride = null;
			this.PreventHeroAnimation = null;
			this.HideDecorators = null;
			this.TextAlignment = new FsmEnum(TextAlignmentOptions.TopLeft);
			this.OffsetY = null;
		}

		// Token: 0x06007FE2 RID: 32738 RVA: 0x0025CA88 File Offset: 0x0025AC88
		public override void Awake()
		{
			this.TextAlignment.Value = this.TextAlignment.Value;
		}

		// Token: 0x06007FE3 RID: 32739 RVA: 0x0025CAA0 File Offset: 0x0025ACA0
		protected override void DoAction(PlayMakerNPC npc)
		{
			this.setEventTarget = npc;
			npc.CustomEventTarget = base.Fsm.FsmComponent;
			if (!npc.IsRunningDialogue)
			{
				npc.SetAutoStarting();
				npc.StartDialogueImmediately();
			}
			this.StartDialogue(npc);
		}

		// Token: 0x06007FE4 RID: 32740 RVA: 0x0025CAD8 File Offset: 0x0025ACD8
		protected override void DoActionNoComponent(GameObject target)
		{
			PlayMakerNPC newTemp = PlayMakerNPC.GetNewTemp(base.Fsm.FsmComponent);
			newTemp.StartDialogueMove();
			this.StartDialogue(newTemp);
		}

		// Token: 0x06007FE5 RID: 32741 RVA: 0x0025CB03 File Offset: 0x0025AD03
		public override void OnExit()
		{
			if (this.setEventTarget != null)
			{
				if (this.setEventTarget.CustomEventTarget == base.Fsm.FsmComponent)
				{
					this.setEventTarget.CustomEventTarget = null;
				}
				this.setEventTarget = null;
			}
		}

		// Token: 0x06007FE6 RID: 32742 RVA: 0x0025CB44 File Offset: 0x0025AD44
		protected virtual void StartDialogue(PlayMakerNPC component)
		{
			Action action = null;
			RandomAudioClipTable randomAudioClipTable = this.PlayerVoiceTableOverride.Value as RandomAudioClipTable;
			if (randomAudioClipTable != null)
			{
				component.SetTalkTableOverride(randomAudioClipTable);
				action = new Action(component.RemoveTalkTableOverride);
			}
			if (this.PreventHeroAnimation.Value)
			{
				HeroTalkAnimation.SetBlocked(true);
				if (action == null)
				{
					action = new Action(RunDialogueBase.<StartDialogue>g__ResetBlocked|14_0);
				}
				else
				{
					action = (Action)Delegate.Combine(action, new Action(RunDialogueBase.<StartDialogue>g__ResetBlocked|14_0));
				}
			}
			DialogueBox.StartConversation(this.DialogueText, component, !this.OverrideContinue.IsNone && this.OverrideContinue.Value, this.GetDisplayOptions(new DialogueBox.DisplayOptions
			{
				ShowDecorators = !this.HideDecorators.Value,
				Alignment = (TextAlignmentOptions)this.TextAlignment.Value,
				OffsetY = this.OffsetY.Value,
				TextColor = Color.white
			}), action, action);
		}

		// Token: 0x06007FE7 RID: 32743 RVA: 0x0025CC40 File Offset: 0x0025AE40
		protected virtual DialogueBox.DisplayOptions GetDisplayOptions(DialogueBox.DisplayOptions defaultOptions)
		{
			return defaultOptions;
		}

		// Token: 0x06007FE9 RID: 32745 RVA: 0x0025CC4B File Offset: 0x0025AE4B
		[CompilerGenerated]
		internal static void <StartDialogue>g__ResetBlocked|14_0()
		{
			HeroTalkAnimation.SetBlocked(false);
		}

		// Token: 0x04007F51 RID: 32593
		public FsmBool OverrideContinue;

		// Token: 0x04007F52 RID: 32594
		[ObjectType(typeof(RandomAudioClipTable))]
		public FsmObject PlayerVoiceTableOverride;

		// Token: 0x04007F53 RID: 32595
		public FsmBool PreventHeroAnimation;

		// Token: 0x04007F54 RID: 32596
		[ActionSection("Display Options")]
		public FsmBool HideDecorators;

		// Token: 0x04007F55 RID: 32597
		[ObjectType(typeof(TextAlignmentOptions))]
		public FsmEnum TextAlignment;

		// Token: 0x04007F56 RID: 32598
		public FsmFloat OffsetY;

		// Token: 0x04007F57 RID: 32599
		private PlayMakerNPC setEventTarget;
	}
}
