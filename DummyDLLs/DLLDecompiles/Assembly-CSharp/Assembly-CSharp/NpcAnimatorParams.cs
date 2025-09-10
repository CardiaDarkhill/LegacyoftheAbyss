using System;
using UnityEngine;

// Token: 0x02000635 RID: 1589
public class NpcAnimatorParams : MonoBehaviour
{
	// Token: 0x060038BA RID: 14522 RVA: 0x000FA994 File Offset: 0x000F8B94
	private void Awake()
	{
		if (!this.control || !this.animator)
		{
			return;
		}
		this.control.StartedDialogue += delegate()
		{
			this.animator.SetBool(NpcAnimatorParams.InConvoParam, true);
		};
		this.control.StartedNewLine += delegate(DialogueBox.DialogueLine line)
		{
			this.animator.SetBool(NpcAnimatorParams.TalkingParam, !line.IsPlayer);
		};
		this.control.EndingDialogue += delegate()
		{
			this.animator.SetBool(NpcAnimatorParams.TalkingParam, false);
		};
		this.control.EndedDialogue += delegate()
		{
			this.animator.SetBool(NpcAnimatorParams.InConvoParam, false);
		};
	}

	// Token: 0x04003BC2 RID: 15298
	private static readonly int InConvoParam = Animator.StringToHash("In Conversation");

	// Token: 0x04003BC3 RID: 15299
	private static readonly int TalkingParam = Animator.StringToHash("Is Talking");

	// Token: 0x04003BC4 RID: 15300
	[SerializeField]
	private NPCControlBase control;

	// Token: 0x04003BC5 RID: 15301
	[SerializeField]
	private Animator animator;
}
