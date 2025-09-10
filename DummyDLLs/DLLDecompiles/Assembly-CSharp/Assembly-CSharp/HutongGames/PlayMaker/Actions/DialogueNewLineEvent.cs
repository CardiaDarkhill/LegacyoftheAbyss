using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001363 RID: 4963
	public sealed class DialogueNewLineEvent : FsmStateAction
	{
		// Token: 0x06008004 RID: 32772 RVA: 0x0025D206 File Offset: 0x0025B406
		public override void Reset()
		{
			this.Target = null;
			this.IgnoreFirstLine = null;
			this.ExpectedLineEvent = new FsmString
			{
				UseVariable = true
			};
		}

		// Token: 0x06008005 RID: 32773 RVA: 0x0025D228 File Offset: 0x0025B428
		public override void OnEnter()
		{
			this.target = this.Target.GetSafe(this);
			if (this.target != null)
			{
				this.seenFirstLine = false;
				this.RegisterEvents();
			}
			base.Finish();
		}

		// Token: 0x06008006 RID: 32774 RVA: 0x0025D25D File Offset: 0x0025B45D
		public override void OnExit()
		{
			base.OnExit();
			this.UnregisterEvents();
			this.target = null;
		}

		// Token: 0x06008007 RID: 32775 RVA: 0x0025D272 File Offset: 0x0025B472
		private void RegisterEvents()
		{
			if (!this.registeredEvent)
			{
				this.registeredEvent = true;
				this.target.StartedNewLine += this.TargetOnStartedNewLine;
			}
		}

		// Token: 0x06008008 RID: 32776 RVA: 0x0025D29A File Offset: 0x0025B49A
		private void UnregisterEvents()
		{
			if (this.registeredEvent)
			{
				this.registeredEvent = false;
				if (this.target != null)
				{
					this.target.StartedNewLine -= this.TargetOnStartedNewLine;
				}
			}
		}

		// Token: 0x06008009 RID: 32777 RVA: 0x0025D2D0 File Offset: 0x0025B4D0
		private void TargetOnStartedNewLine(DialogueBox.DialogueLine line)
		{
			bool flag = !this.seenFirstLine;
			this.seenFirstLine = true;
			if (flag && this.IgnoreFirstLine.Value)
			{
				return;
			}
			DialogueNewLineEvent.LineOwner lineOwner = (DialogueNewLineEvent.LineOwner)this.LineSpeaker.Value;
			if (lineOwner != DialogueNewLineEvent.LineOwner.Player)
			{
				if (lineOwner == DialogueNewLineEvent.LineOwner.Other)
				{
					if (line.IsPlayer)
					{
						return;
					}
				}
			}
			else if (!line.IsPlayer)
			{
				return;
			}
			if (!this.ExpectedLineEvent.IsNone)
			{
				string text = this.ExpectedLineEvent.Value;
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = line.Event;
					if (string.IsNullOrEmpty(text2))
					{
						return;
					}
					text = text.Trim();
					text2 = text2.Trim();
					if (text != text2)
					{
						return;
					}
				}
			}
			base.Fsm.Event(this.LineStartedEvent);
		}

		// Token: 0x04007F70 RID: 32624
		[RequiredField]
		[CheckForComponent(typeof(NPCControlBase))]
		public FsmOwnerDefault Target;

		// Token: 0x04007F71 RID: 32625
		public FsmBool IgnoreFirstLine;

		// Token: 0x04007F72 RID: 32626
		[ObjectType(typeof(DialogueNewLineEvent.LineOwner))]
		public FsmEnum LineSpeaker;

		// Token: 0x04007F73 RID: 32627
		public FsmString ExpectedLineEvent;

		// Token: 0x04007F74 RID: 32628
		public FsmEvent LineStartedEvent;

		// Token: 0x04007F75 RID: 32629
		private NPCControlBase target;

		// Token: 0x04007F76 RID: 32630
		private bool seenFirstLine;

		// Token: 0x04007F77 RID: 32631
		private bool registeredEvent;

		// Token: 0x02001BFB RID: 7163
		[Serializable]
		private enum LineOwner
		{
			// Token: 0x04009FBF RID: 40895
			Any,
			// Token: 0x04009FC0 RID: 40896
			Player,
			// Token: 0x04009FC1 RID: 40897
			Other
		}
	}
}
