using System;
using System.Collections;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020001CF RID: 463
public class CollectionGramaphone : MonoBehaviour
{
	// Token: 0x170001ED RID: 493
	// (get) Token: 0x0600120B RID: 4619 RVA: 0x00053FE8 File Offset: 0x000521E8
	// (set) Token: 0x0600120C RID: 4620 RVA: 0x00053FF0 File Offset: 0x000521F0
	public CollectableRelic PlayingRelic { get; private set; }

	// Token: 0x0600120D RID: 4621 RVA: 0x00053FF9 File Offset: 0x000521F9
	private void Awake()
	{
		if (this.activeWhilePlaying)
		{
			this.activeWhilePlaying.SetActive(false);
		}
		this.defaultMixer = this.source.outputAudioMixerGroup;
		this.TryLoadRelic();
	}

	// Token: 0x0600120E RID: 4622 RVA: 0x0005402C File Offset: 0x0005222C
	private IEnumerator Start()
	{
		if (!this.TryLoadRelic())
		{
			yield break;
		}
		HeroController hc = HeroController.instance;
		if (hc != null && !hc.isHeroInPosition)
		{
			while (!hc.isHeroInPosition)
			{
				yield return null;
			}
		}
		hc = null;
		while (this.loadedRelic.IsLoading)
		{
			yield return null;
		}
		this.Play(this.loadedRelic, true, null);
		yield break;
	}

	// Token: 0x0600120F RID: 4623 RVA: 0x0005403B File Offset: 0x0005223B
	private void OnDestroy()
	{
		if (this.loadedRelic)
		{
			this.loadedRelic.FreeClips();
			this.loadedRelic = null;
		}
	}

	// Token: 0x06001210 RID: 4624 RVA: 0x0005405C File Offset: 0x0005225C
	private bool TryLoadRelic()
	{
		if (this.loadedRelic)
		{
			return true;
		}
		if (string.IsNullOrEmpty(this.playingPdField))
		{
			return false;
		}
		CollectionGramaphone.PlayingInfo variable = PlayerData.instance.GetVariable(this.playingPdField);
		if (string.IsNullOrEmpty(variable.RelicName))
		{
			return false;
		}
		this.loadedRelic = CollectableRelicManager.GetRelic(variable.RelicName);
		if (!this.loadedRelic)
		{
			return false;
		}
		this.loadedRelic.LoadClips();
		return true;
	}

	// Token: 0x06001211 RID: 4625 RVA: 0x000540D4 File Offset: 0x000522D4
	public void Play(CollectableRelic playingRelicAudio, bool alreadyPlaying, RelicBoardOwner owner)
	{
		if (this.snapshot)
		{
			this.snapshot.TransitionTo(alreadyPlaying ? this.alreadyFadingTransitionTime : 0f);
		}
		if (this.behaviourFsm)
		{
			this.behaviourFsm.SendEventSafe(alreadyPlaying ? "ALREADY PLAYING" : "START PLAYING");
		}
		if (!alreadyPlaying)
		{
			this.sourcePreStartAudio.PlayOnSource(this.eventSource);
		}
		if (this.behaviourFsm)
		{
			AudioMixerGroup mixerOverride = playingRelicAudio.MixerOverride;
			this.source.outputAudioMixerGroup = (mixerOverride ? mixerOverride : this.defaultMixer);
		}
		if (this.source.clip != playingRelicAudio.GramaphoneClip)
		{
			this.source.clip = playingRelicAudio.GramaphoneClip;
			this.source.PlayDelayed(this.sourceStartDelay);
		}
		else if (!this.source.isPlaying)
		{
			this.source.PlayDelayed(this.sourceStartDelay);
		}
		this.PlayingRelic = playingRelicAudio;
		if (this.activeWhilePlaying)
		{
			this.activeWhilePlaying.SetActive(true);
		}
		if (playingRelicAudio.PlaySyncedAudioSource && this.needolinSyncedAudioPlayer)
		{
			this.needolinSyncedAudioPlayer.Play();
		}
		if (this.needolinLoop)
		{
			AudioClip needolinClip = playingRelicAudio.NeedolinClip;
			this.needolinLoop.DoSync = (needolinClip != null);
			if (!this.needolinLoop.DoSync)
			{
				needolinClip = this.defaultNeedolinClip;
			}
			this.needolinLoop.NeedolinClip = needolinClip;
		}
		if (this.reportDelayRoutine != null)
		{
			base.StopCoroutine(this.reportDelayRoutine);
		}
		if (playingRelicAudio.WillSendPlayEvent)
		{
			this.reportDelayRoutine = base.StartCoroutine(this.ReportPlayedDelayed(playingRelicAudio, owner));
		}
		if (string.IsNullOrEmpty(this.playingPdField))
		{
			return;
		}
		PlayerData.instance.SetVariable(this.playingPdField, new CollectionGramaphone.PlayingInfo
		{
			RelicName = playingRelicAudio.name,
			StartTime = GameManager.instance.PlayTime
		});
	}

	// Token: 0x06001212 RID: 4626 RVA: 0x000542C9 File Offset: 0x000524C9
	private IEnumerator ReportPlayedDelayed(CollectableRelic playingRelic, RelicBoardOwner owner)
	{
		if (owner)
		{
			owner.RelicBoard.IsActionsBlocked = true;
		}
		yield return new WaitForSeconds(0.5f);
		playingRelic.OnPlayedEvent();
		yield break;
	}

	// Token: 0x06001213 RID: 4627 RVA: 0x000542E0 File Offset: 0x000524E0
	public void Stop()
	{
		if (this.reportDelayRoutine != null)
		{
			base.StopCoroutine(this.reportDelayRoutine);
			this.reportDelayRoutine = null;
		}
		if (this.behaviourFsm)
		{
			this.behaviourFsm.SendEventSafe("STOP PLAYING");
		}
		this.source.Stop();
		if (this.PlayingRelic != null && this.PlayingRelic.PlaySyncedAudioSource && this.needolinSyncedAudioPlayer)
		{
			this.needolinSyncedAudioPlayer.Stop();
		}
		this.PlayingRelic = null;
		this.sourceEndAudio.PlayOnSource(this.eventSource);
		if (this.activeWhilePlaying)
		{
			this.activeWhilePlaying.SetActive(false);
		}
		if (!string.IsNullOrEmpty(this.playingPdField))
		{
			PlayerData.instance.SetVariable(this.playingPdField, default(CollectionGramaphone.PlayingInfo));
		}
	}

	// Token: 0x040010DF RID: 4319
	[SerializeField]
	private AudioSource source;

	// Token: 0x040010E0 RID: 4320
	[SerializeField]
	private float sourceStartDelay;

	// Token: 0x040010E1 RID: 4321
	[SerializeField]
	private AudioSource eventSource;

	// Token: 0x040010E2 RID: 4322
	[SerializeField]
	private AudioEvent sourcePreStartAudio;

	// Token: 0x040010E3 RID: 4323
	[SerializeField]
	private AudioEvent sourceEndAudio;

	// Token: 0x040010E4 RID: 4324
	[SerializeField]
	private OverrideNeedolinLoop needolinLoop;

	// Token: 0x040010E5 RID: 4325
	[SerializeField]
	private NeedolinSyncedAudioPlayer needolinSyncedAudioPlayer;

	// Token: 0x040010E6 RID: 4326
	[SerializeField]
	private AudioClip defaultNeedolinClip;

	// Token: 0x040010E7 RID: 4327
	[SerializeField]
	private AudioMixerSnapshot snapshot;

	// Token: 0x040010E8 RID: 4328
	[SerializeField]
	private float alreadyFadingTransitionTime = 1f;

	// Token: 0x040010E9 RID: 4329
	[SerializeField]
	private PlayMakerFSM behaviourFsm;

	// Token: 0x040010EA RID: 4330
	[SerializeField]
	private GameObject activeWhilePlaying;

	// Token: 0x040010EB RID: 4331
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(CollectionGramaphone.PlayingInfo), false)]
	private string playingPdField;

	// Token: 0x040010EC RID: 4332
	private Coroutine reportDelayRoutine;

	// Token: 0x040010ED RID: 4333
	private AudioMixerGroup defaultMixer;

	// Token: 0x040010EE RID: 4334
	private CollectableRelic loadedRelic;

	// Token: 0x0200150D RID: 5389
	[Serializable]
	public struct PlayingInfo
	{
		// Token: 0x040085BA RID: 34234
		public string RelicName;

		// Token: 0x040085BB RID: 34235
		public float StartTime;
	}
}
