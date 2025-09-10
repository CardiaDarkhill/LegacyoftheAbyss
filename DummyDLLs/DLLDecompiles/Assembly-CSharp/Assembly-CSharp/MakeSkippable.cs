using System;
using UnityEngine;

// Token: 0x02000358 RID: 856
public class MakeSkippable : MonoBehaviour
{
	// Token: 0x06001D98 RID: 7576 RVA: 0x0008894C File Offset: 0x00086B4C
	private void Awake()
	{
		this.cinematicPlayer = base.GetComponent<CinematicPlayer>();
		this.cutsceneHelper = base.GetComponent<CutsceneHelper>();
		if (this.cinematicPlayer != null)
		{
			this.skipType = MakeSkippable.SkipType.Cinematic;
			return;
		}
		if (this.cutsceneHelper != null)
		{
			this.skipType = MakeSkippable.SkipType.Cutscene;
			return;
		}
		this.skipType = MakeSkippable.SkipType.Inactive;
		Debug.LogError("MakeSkippable requires a Cinematic Player or Cutscene Helper component.");
	}

	// Token: 0x06001D99 RID: 7577 RVA: 0x000889AE File Offset: 0x00086BAE
	private void Start()
	{
		if (this.skipType != MakeSkippable.SkipType.Inactive)
		{
			if (this.unlockAfterSec <= 0f)
			{
				this.UnlockSkip();
				return;
			}
			base.Invoke("UnlockSkip", this.unlockAfterSec);
		}
	}

	// Token: 0x06001D9A RID: 7578 RVA: 0x000889E0 File Offset: 0x00086BE0
	private void UnlockSkip()
	{
		if (this.skipType != MakeSkippable.SkipType.Cinematic)
		{
			if (this.skipType == MakeSkippable.SkipType.Cutscene)
			{
				if (this.cutsceneHelper != null)
				{
					this.cutsceneHelper.UnlockSkip();
					return;
				}
				Debug.LogError("MakeSkippable - Cutscene Helper is null");
			}
			return;
		}
		if (this.cinematicPlayer != null)
		{
			this.cinematicPlayer.UnlockSkip();
			return;
		}
		Debug.LogError("MakeSkippable - Cinematic Player is null");
	}

	// Token: 0x04001CCF RID: 7375
	public float unlockAfterSec;

	// Token: 0x04001CD0 RID: 7376
	private CinematicPlayer cinematicPlayer;

	// Token: 0x04001CD1 RID: 7377
	private CutsceneHelper cutsceneHelper;

	// Token: 0x04001CD2 RID: 7378
	private MakeSkippable.SkipType skipType;

	// Token: 0x0200160E RID: 5646
	private enum SkipType
	{
		// Token: 0x04008992 RID: 35218
		Inactive,
		// Token: 0x04008993 RID: 35219
		Cinematic,
		// Token: 0x04008994 RID: 35220
		Cutscene
	}
}
