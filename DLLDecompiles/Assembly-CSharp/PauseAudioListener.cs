using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000437 RID: 1079
public sealed class PauseAudioListener : FsmStateAction
{
	// Token: 0x06002534 RID: 9524 RVA: 0x000AAE33 File Offset: 0x000A9033
	public override void Reset()
	{
		this.pause = null;
		this.unpauseOnExit = null;
	}

	// Token: 0x06002535 RID: 9525 RVA: 0x000AAE43 File Offset: 0x000A9043
	public override void OnEnter()
	{
		AudioListener.pause = this.pause.Value;
		base.Finish();
	}

	// Token: 0x06002536 RID: 9526 RVA: 0x000AAE5B File Offset: 0x000A905B
	public override void OnExit()
	{
		if (this.unpauseOnExit.Value)
		{
			AudioListener.pause = false;
		}
	}

	// Token: 0x040022F8 RID: 8952
	public FsmBool pause;

	// Token: 0x040022F9 RID: 8953
	public FsmBool unpauseOnExit;
}
