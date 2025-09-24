using System;
using UnityEngine;

// Token: 0x0200009D RID: 157
[RequireComponent(typeof(tk2dSpriteAnimator))]
public class PlayFromRandomFrame : MonoBehaviour
{
	// Token: 0x060004E3 RID: 1251 RVA: 0x00019A27 File Offset: 0x00017C27
	private void Start()
	{
		this.DoRandomFrame();
	}

	// Token: 0x060004E4 RID: 1252 RVA: 0x00019A2F File Offset: 0x00017C2F
	private void OnEnable()
	{
		if (this.onEnable)
		{
			this.DoRandomFrame();
		}
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x00019A40 File Offset: 0x00017C40
	private void DoRandomFrame()
	{
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		if (!this.animator)
		{
			return;
		}
		if (this.getFromCurrentClip)
		{
			tk2dSpriteAnimationClip tk2dSpriteAnimationClip = this.animator.CurrentClip ?? this.animator.DefaultClip;
			if (tk2dSpriteAnimationClip == null)
			{
				Debug.LogError("Clip is null", this);
				return;
			}
			this.frameCount = tk2dSpriteAnimationClip.frames.Length;
		}
		int frame = Random.Range(0, this.frameCount);
		this.animator.PlayFromFrame(frame);
	}

	// Token: 0x040004B8 RID: 1208
	[SerializeField]
	[ModifiableProperty]
	[Conditional("getFromCurrentClip", false, false, false)]
	private int frameCount;

	// Token: 0x040004B9 RID: 1209
	[SerializeField]
	private bool getFromCurrentClip;

	// Token: 0x040004BA RID: 1210
	[SerializeField]
	private bool onEnable;

	// Token: 0x040004BB RID: 1211
	private tk2dSpriteAnimator animator;
}
