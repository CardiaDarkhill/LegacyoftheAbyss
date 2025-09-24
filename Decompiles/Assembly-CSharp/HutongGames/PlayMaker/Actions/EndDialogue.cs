using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001365 RID: 4965
	[ActionCategory("Dialogue")]
	public class EndDialogue : FsmStateAction
	{
		// Token: 0x0600800E RID: 32782 RVA: 0x0025D3F6 File Offset: 0x0025B5F6
		public override void Reset()
		{
			this.ReturnControl = new FsmBool(true);
			this.ReturnHUD = new FsmBool(true);
			this.Target = null;
			this.UseChildren = null;
		}

		// Token: 0x0600800F RID: 32783 RVA: 0x0025D428 File Offset: 0x0025B628
		public override void OnEnter()
		{
			this.runningDialogues = 0;
			GameObject safe = this.Target.GetSafe(this);
			if (safe)
			{
				if (!this.UseChildren.Value)
				{
					PlayMakerNPC component = safe.GetComponent<PlayMakerNPC>();
					if (component != null)
					{
						component.CloseDialogueBox(this.ReturnControl.Value, this.ReturnHUD.Value, new Action(base.Finish));
						return;
					}
					base.Finish();
					return;
				}
				else
				{
					PlayMakerNPC[] componentsInChildren = safe.GetComponentsInChildren<PlayMakerNPC>();
					if (componentsInChildren.Length == 0)
					{
						base.Finish();
						return;
					}
					foreach (PlayMakerNPC playMakerNPC in componentsInChildren)
					{
						if (playMakerNPC.IsRunningDialogue)
						{
							this.runningDialogues++;
							playMakerNPC.CloseDialogueBox(this.ReturnControl.Value, this.ReturnHUD.Value, new Action(this.EndChild));
						}
					}
					if (this.runningDialogues <= 0)
					{
						base.Finish();
						return;
					}
				}
			}
			else
			{
				base.Finish();
			}
		}

		// Token: 0x06008010 RID: 32784 RVA: 0x0025D523 File Offset: 0x0025B723
		private void EndChild()
		{
			this.runningDialogues--;
			if (this.runningDialogues <= 0)
			{
				base.Finish();
			}
		}

		// Token: 0x04007F7A RID: 32634
		public FsmBool ReturnControl;

		// Token: 0x04007F7B RID: 32635
		public FsmBool ReturnHUD;

		// Token: 0x04007F7C RID: 32636
		public FsmOwnerDefault Target;

		// Token: 0x04007F7D RID: 32637
		public FsmBool UseChildren;

		// Token: 0x04007F7E RID: 32638
		private int runningDialogues;
	}
}
