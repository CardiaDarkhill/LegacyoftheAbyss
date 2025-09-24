using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class AnimSpeedLerp : FsmStateAction
{
	// Token: 0x0600013C RID: 316 RVA: 0x000077AD File Offset: 0x000059AD
	public override void Reset()
	{
		this.target = null;
		this.duration = null;
		this.toSpeed = null;
	}

	// Token: 0x0600013D RID: 317 RVA: 0x000077C4 File Offset: 0x000059C4
	public override void OnEnter()
	{
		this.elapsed = 0f;
		GameObject safe = this.target.GetSafe(this);
		if (safe)
		{
			this.animator = safe.GetComponent<Animator>();
			if (this.animator)
			{
				this.fromSpeed = this.animator.speed;
				return;
			}
		}
		base.Finish();
	}

	// Token: 0x0600013E RID: 318 RVA: 0x00007824 File Offset: 0x00005A24
	public override void OnUpdate()
	{
		if (this.animator)
		{
			this.animator.speed = Mathf.Lerp(this.fromSpeed, this.toSpeed.Value, Mathf.Min(1f, this.elapsed / this.duration.Value));
			this.elapsed += Time.deltaTime;
		}
	}

	// Token: 0x0600013F RID: 319 RVA: 0x0000788D File Offset: 0x00005A8D
	public override void OnExit()
	{
		if (this.animator)
		{
			this.animator.speed = 0f;
		}
	}

	// Token: 0x040000D5 RID: 213
	public FsmOwnerDefault target;

	// Token: 0x040000D6 RID: 214
	public FsmFloat duration;

	// Token: 0x040000D7 RID: 215
	public FsmFloat toSpeed;

	// Token: 0x040000D8 RID: 216
	private float elapsed;

	// Token: 0x040000D9 RID: 217
	private float fromSpeed;

	// Token: 0x040000DA RID: 218
	private Animator animator;
}
