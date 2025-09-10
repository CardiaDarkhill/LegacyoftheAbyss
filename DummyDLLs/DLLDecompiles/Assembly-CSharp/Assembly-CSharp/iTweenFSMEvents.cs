using System;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

// Token: 0x02000047 RID: 71
public class iTweenFSMEvents : MonoBehaviour
{
	// Token: 0x06000201 RID: 513 RVA: 0x0000C8DA File Offset: 0x0000AADA
	private void iTweenOnStart(int aniTweenID)
	{
		if (this.itweenID == aniTweenID)
		{
			this.itweenFSMAction.Fsm.Event(this.itweenFSMAction.startEvent);
		}
	}

	// Token: 0x06000202 RID: 514 RVA: 0x0000C900 File Offset: 0x0000AB00
	private void iTweenOnComplete(int aniTweenID)
	{
		if (this.itweenID == aniTweenID)
		{
			if (this.islooping)
			{
				if (!this.donotfinish)
				{
					this.itweenFSMAction.Fsm.Event(this.itweenFSMAction.finishEvent);
					this.itweenFSMAction.Finish();
					return;
				}
			}
			else
			{
				this.itweenFSMAction.Fsm.Event(this.itweenFSMAction.finishEvent);
				this.itweenFSMAction.Finish();
			}
		}
	}

	// Token: 0x06000203 RID: 515 RVA: 0x0000C973 File Offset: 0x0000AB73
	private void OnDestroy()
	{
		Action<iTweenFSMEvents> action = this.onDestroy;
		if (action != null)
		{
			action(this);
		}
		this.onDestroy = null;
		this.itweenFSMAction = null;
	}

	// Token: 0x0400019D RID: 413
	public static int itweenIDCount;

	// Token: 0x0400019E RID: 414
	public int itweenID;

	// Token: 0x0400019F RID: 415
	public iTweenFsmAction itweenFSMAction;

	// Token: 0x040001A0 RID: 416
	public bool donotfinish;

	// Token: 0x040001A1 RID: 417
	public bool islooping;

	// Token: 0x040001A2 RID: 418
	public Action<iTweenFSMEvents> onDestroy;
}
