using System;
using System.Collections;
using GlobalEnums;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x02000081 RID: 129
public class EndingSequence : MonoBehaviour, GameManager.ISkippable
{
	// Token: 0x06000391 RID: 913 RVA: 0x000125AB File Offset: 0x000107AB
	private void Awake()
	{
		this.chainSequence.EndBlankerFade += this.OnEndBlankerFade;
		this.chainSequence.SequenceComplete += EndingSequence.OnSequenceComplete;
	}

	// Token: 0x06000392 RID: 914 RVA: 0x000125DB File Offset: 0x000107DB
	private void Start()
	{
		GameManager instance = GameManager.instance;
		instance.inputHandler.SetSkipMode(SkipPromptMode.SKIP_INSTANT);
		instance.RegisterSkippable(this);
		this.chainSequence.Begin();
	}

	// Token: 0x06000393 RID: 915 RVA: 0x00012600 File Offset: 0x00010800
	private void OnDestroy()
	{
		GameManager silentInstance = GameManager.SilentInstance;
		if (silentInstance)
		{
			silentInstance.DeregisterSkippable(this);
		}
	}

	// Token: 0x06000394 RID: 916 RVA: 0x00012622 File Offset: 0x00010822
	public IEnumerator Skip()
	{
		this.chainSequence.SkipSingle();
		while (this.chainSequence.IsCurrentSkipped)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x06000395 RID: 917 RVA: 0x00012631 File Offset: 0x00010831
	private void OnEndBlankerFade()
	{
		if (this.endBlankSnapshot)
		{
			this.endBlankSnapshot.TransitionTo(1f);
		}
	}

	// Token: 0x06000396 RID: 918 RVA: 0x00012650 File Offset: 0x00010850
	private static void OnSequenceComplete()
	{
		GameManager instance = GameManager.instance;
		instance.StartCoroutine(instance.ReturnToMainMenu(true, null, true, false));
	}

	// Token: 0x0400033E RID: 830
	[SerializeField]
	private ChainSequence chainSequence;

	// Token: 0x0400033F RID: 831
	[SerializeField]
	private AudioMixerSnapshot endBlankSnapshot;
}
