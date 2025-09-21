using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000A7 RID: 167
public class RemapTk2DSpriteAnimator : MonoBehaviour
{
	// Token: 0x0600050A RID: 1290 RVA: 0x0001A268 File Offset: 0x00018468
	private void Start()
	{
		this.shouldAnimate = true;
		if (!this.sourceAnimator)
		{
			this.shouldAnimate = false;
		}
		if (this.targetAnimator)
		{
			this.targetSprite = this.targetAnimator.GetComponent<tk2dSprite>();
			this.targetRenderer = this.targetAnimator.GetComponent<MeshRenderer>();
		}
		else
		{
			this.shouldAnimate = false;
		}
		if (this.syncFrames && this.targetSprite == null)
		{
			this.shouldAnimate = false;
		}
		foreach (RemapTk2DSpriteAnimator.AnimationRemap animationRemap in this.animationsList)
		{
			this.animations[animationRemap.sourceClip] = animationRemap.targetClip;
		}
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x0001A33C File Offset: 0x0001853C
	private void LateUpdate()
	{
		if (!this.shouldAnimate)
		{
			return;
		}
		string text = (this.sourceAnimator.CurrentClip != null) ? this.sourceAnimator.CurrentClip.name : string.Empty;
		if (this.syncFrames)
		{
			if (this.targetAnimator.enabled)
			{
				this.targetAnimator.enabled = false;
			}
			if (!string.IsNullOrEmpty(text) && this.animations.ContainsKey(text))
			{
				if (this.targetRenderer)
				{
					this.targetRenderer.enabled = true;
				}
				this.MatchFrame(text);
				return;
			}
			if (this.targetRenderer)
			{
				this.targetRenderer.enabled = false;
				return;
			}
		}
		else
		{
			if (!this.targetAnimator.enabled)
			{
				this.targetAnimator.enabled = true;
				this.lastSourceClip = null;
			}
			if (text != this.lastSourceClip)
			{
				this.lastSourceClip = text;
				if (!string.IsNullOrEmpty(text) && this.animations.ContainsKey(text))
				{
					this.targetAnimator.PlayFromFrame(this.animations[text], this.syncFrames ? this.sourceAnimator.CurrentFrame : 0);
				}
			}
		}
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x0001A464 File Offset: 0x00018664
	private void MatchFrame(string sourceClipName)
	{
		tk2dSpriteAnimationClip clipByName = this.targetAnimator.GetClipByName(this.animations[sourceClipName]);
		if (clipByName == null)
		{
			Debug.LogError("targetAnimator does not have clip: " + sourceClipName, this);
			return;
		}
		int num = this.sourceAnimator.CurrentFrame;
		int num2 = clipByName.frames.Length;
		RemapTk2DSpriteAnimator.MatchSyncTypes matchSyncTypes = this.matchSyncType;
		if (matchSyncTypes != RemapTk2DSpriteAnimator.MatchSyncTypes.Wrap)
		{
			if (matchSyncTypes != RemapTk2DSpriteAnimator.MatchSyncTypes.Clamp)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (num > num2 - 1)
			{
				num = num2 - 1;
			}
		}
		else
		{
			num %= num2;
		}
		tk2dSpriteAnimationFrame frame = clipByName.GetFrame(num);
		this.targetSprite.SetSprite(frame.spriteId);
	}

	// Token: 0x040004E4 RID: 1252
	[SerializeField]
	private tk2dSpriteAnimator sourceAnimator;

	// Token: 0x040004E5 RID: 1253
	[SerializeField]
	private tk2dSpriteAnimator targetAnimator;

	// Token: 0x040004E6 RID: 1254
	[SerializeField]
	private bool syncFrames = true;

	// Token: 0x040004E7 RID: 1255
	[SerializeField]
	[ModifiableProperty]
	[Conditional("syncFrames", true, false, false)]
	private RemapTk2DSpriteAnimator.MatchSyncTypes matchSyncType;

	// Token: 0x040004E8 RID: 1256
	[Space]
	[SerializeField]
	private List<RemapTk2DSpriteAnimator.AnimationRemap> animationsList = new List<RemapTk2DSpriteAnimator.AnimationRemap>();

	// Token: 0x040004E9 RID: 1257
	private readonly Dictionary<string, string> animations = new Dictionary<string, string>();

	// Token: 0x040004EA RID: 1258
	private tk2dSprite targetSprite;

	// Token: 0x040004EB RID: 1259
	private MeshRenderer targetRenderer;

	// Token: 0x040004EC RID: 1260
	private bool shouldAnimate;

	// Token: 0x040004ED RID: 1261
	private string lastSourceClip;

	// Token: 0x02001418 RID: 5144
	[Serializable]
	private struct AnimationRemap
	{
		// Token: 0x040081E5 RID: 33253
		public string sourceClip;

		// Token: 0x040081E6 RID: 33254
		public string targetClip;
	}

	// Token: 0x02001419 RID: 5145
	private enum MatchSyncTypes
	{
		// Token: 0x040081E8 RID: 33256
		Wrap,
		// Token: 0x040081E9 RID: 33257
		Clamp
	}
}
