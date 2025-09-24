using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000639 RID: 1593
public class NPCSpeakingAudio : MonoBehaviour
{
	// Token: 0x1700067E RID: 1662
	// (get) Token: 0x06003917 RID: 14615 RVA: 0x000FB884 File Offset: 0x000F9A84
	// (set) Token: 0x06003918 RID: 14616 RVA: 0x000FB89A File Offset: 0x000F9A9A
	public RandomAudioClipTable Table
	{
		get
		{
			AudioSource audioSource;
			return this.GetTableForSpeaker(null, out audioSource);
		}
		set
		{
			this.SetTableForSpeaker(null, value);
		}
	}

	// Token: 0x06003919 RID: 14617 RVA: 0x000FB8A4 File Offset: 0x000F9AA4
	private void Reset()
	{
		this.npc = base.GetComponent<NPCControlBase>();
	}

	// Token: 0x0600391A RID: 14618 RVA: 0x000FB8B2 File Offset: 0x000F9AB2
	private void OnValidate()
	{
		if (this.audioTable)
		{
			this.speakers = new List<NPCSpeakingAudio.SpeakerAudio>
			{
				new NPCSpeakingAudio.SpeakerAudio
				{
					SpeakerEvent = null,
					AudioTable = this.audioTable
				}
			};
			this.audioTable = null;
		}
	}

	// Token: 0x0600391B RID: 14619 RVA: 0x000FB8F4 File Offset: 0x000F9AF4
	private void Awake()
	{
		if (!this.npc)
		{
			this.npc = base.GetComponent<NPCControlBase>();
		}
		if (this.npc)
		{
			this.npc.StartedDialogue += this.OnDialogueStarted;
			this.npc.StartedNewLine += this.OnNewLineStarted;
			this.npc.EndingDialogue += this.OnDialogueEnding;
		}
	}

	// Token: 0x0600391C RID: 14620 RVA: 0x000FB96C File Offset: 0x000F9B6C
	private void OnDialogueStarted()
	{
		this.wasPlayer = null;
		this.hasSpokenOnce = false;
	}

	// Token: 0x0600391D RID: 14621 RVA: 0x000FB981 File Offset: 0x000F9B81
	private void OnDialogueEnding()
	{
		this.wasPlayer = null;
	}

	// Token: 0x0600391E RID: 14622 RVA: 0x000FB990 File Offset: 0x000F9B90
	public RandomAudioClipTable GetTableForSpeaker(string speakerEvent, out AudioSource playOnSource)
	{
		if (this.speakers == null)
		{
			playOnSource = null;
			return null;
		}
		NPCSpeakingAudio.SpeakerAudio speakerAudio = null;
		foreach (NPCSpeakingAudio.SpeakerAudio speakerAudio2 in this.speakers)
		{
			if (speakerAudio == null || speakerAudio2.SpeakerEvent == speakerEvent || (string.IsNullOrEmpty(speakerAudio2.SpeakerEvent) && string.IsNullOrEmpty(speakerEvent)))
			{
				speakerAudio = speakerAudio2;
			}
		}
		if (speakerAudio != null)
		{
			playOnSource = speakerAudio.PlayOnSource;
			return speakerAudio.AudioTable;
		}
		playOnSource = null;
		Debug.LogError("No NPC dialogue table found for speaker event: " + speakerEvent, this);
		return null;
	}

	// Token: 0x0600391F RID: 14623 RVA: 0x000FBA3C File Offset: 0x000F9C3C
	public void SetTableForSpeaker(string speakerEvent, RandomAudioClipTable table)
	{
		if (this.speakers == null)
		{
			this.speakers = new List<NPCSpeakingAudio.SpeakerAudio>();
		}
		foreach (NPCSpeakingAudio.SpeakerAudio speakerAudio in this.speakers)
		{
			if (speakerAudio.SpeakerEvent == speakerEvent || (string.IsNullOrEmpty(speakerAudio.SpeakerEvent) && string.IsNullOrEmpty(speakerEvent)))
			{
				speakerAudio.AudioTable = table;
				return;
			}
		}
		this.speakers.Add(new NPCSpeakingAudio.SpeakerAudio
		{
			AudioTable = table
		});
	}

	// Token: 0x06003920 RID: 14624 RVA: 0x000FBAE0 File Offset: 0x000F9CE0
	private void OnNewLineStarted(DialogueBox.DialogueLine line)
	{
		if (!line.IsPlayer)
		{
			this.Speak(line);
		}
		else
		{
			this.linesSinceLastSpeak = 0;
		}
		this.wasPlayer = new bool?(line.IsPlayer);
	}

	// Token: 0x06003921 RID: 14625 RVA: 0x000FBB0C File Offset: 0x000F9D0C
	private void Speak(DialogueBox.DialogueLine line)
	{
		AudioSource playOnSource;
		RandomAudioClipTable tableForSpeaker = this.GetTableForSpeaker(line.Event, out playOnSource);
		if (this.linesSinceLastSpeak < 1 && this.wasPlayer != null && !this.wasPlayer.Value && tableForSpeaker == this.lastSpeakerTable)
		{
			this.linesSinceLastSpeak++;
			return;
		}
		this.linesSinceLastSpeak = 0;
		if (this.skipNextSpeak)
		{
			this.skipNextSpeak = false;
			return;
		}
		this.lastSpeakerTable = tableForSpeaker;
		this.lastDialogueLine = line;
		NPCSpeakingAudio.PlayVoice(tableForSpeaker, base.transform.position, playOnSource, this);
		this.hasSpokenOnce = true;
	}

	// Token: 0x06003922 RID: 14626 RVA: 0x000FBBA7 File Offset: 0x000F9DA7
	public static void PlayVoice(RandomAudioClipTable audioTable, Vector3 position)
	{
		NPCSpeakingAudio.PlayVoice(audioTable, position, null, null);
	}

	// Token: 0x06003923 RID: 14627 RVA: 0x000FBBB4 File Offset: 0x000F9DB4
	private static void PlayVoice(RandomAudioClipTable audioTable, Vector3 position, AudioSource playOnSource, NPCSpeakingAudio runner)
	{
		NPCSpeakingAudio.<>c__DisplayClass28_0 CS$<>8__locals1 = new NPCSpeakingAudio.<>c__DisplayClass28_0();
		CS$<>8__locals1.runner = runner;
		if (NPCSpeakingAudio._currentPlayingSource)
		{
			NPCSpeakingAudio._currentPlayingSource.Stop();
			CS$<>8__locals1.<PlayVoice>g__OnRecycle|0();
		}
		if (!audioTable)
		{
			return;
		}
		if (CS$<>8__locals1.runner)
		{
			UnityEvent onPlayedVoiceClip = CS$<>8__locals1.runner.OnPlayedVoiceClip;
			if (onPlayedVoiceClip != null)
			{
				onPlayedVoiceClip.Invoke();
			}
		}
		AudioClip audioClip;
		if (CS$<>8__locals1.runner && !string.IsNullOrEmpty(CS$<>8__locals1.runner.lastDialogueLine.Text))
		{
			NPCSpeakingAudio runner2 = CS$<>8__locals1.runner;
			if (runner2.spokenRecord == null)
			{
				runner2.spokenRecord = new Dictionary<string, AudioClip>(audioTable.ClipCount);
			}
			if (CS$<>8__locals1.runner.spokenRecord.TryGetValue(CS$<>8__locals1.runner.lastDialogueLine.Text, out audioClip))
			{
				if (!audioTable.CanPlay(true))
				{
					audioClip = null;
				}
			}
			else
			{
				audioClip = audioTable.SelectClip(true);
				CS$<>8__locals1.runner.spokenRecord[CS$<>8__locals1.runner.lastDialogueLine.Text] = audioClip;
			}
		}
		else
		{
			audioClip = audioTable.SelectClip(true);
		}
		if (playOnSource)
		{
			NPCSpeakingAudio._currentPlayingSource = playOnSource;
			playOnSource.clip = audioClip;
			playOnSource.pitch = audioTable.SelectPitch();
			playOnSource.volume = audioTable.SelectVolume();
			playOnSource.Play();
			return;
		}
		float num = audioTable.SelectPitch();
		NPCSpeakingAudio._currentPlayingSource = new AudioEvent
		{
			Clip = audioClip,
			PitchMin = num,
			PitchMax = num,
			Volume = audioTable.SelectVolume()
		}.SpawnAndPlayOneShot(position, new Action(CS$<>8__locals1.<PlayVoice>g__OnRecycle|0));
		audioTable.ReportPlayed(audioClip, NPCSpeakingAudio._currentPlayingSource);
	}

	// Token: 0x06003924 RID: 14628 RVA: 0x000FBD54 File Offset: 0x000F9F54
	public void TriggerFirstSpeak()
	{
		if (!this.hasSpokenOnce)
		{
			this.Speak(default(DialogueBox.DialogueLine));
		}
	}

	// Token: 0x06003925 RID: 14629 RVA: 0x000FBD78 File Offset: 0x000F9F78
	public void SkipNextSpeak()
	{
		this.skipNextSpeak = true;
	}

	// Token: 0x04003BEE RID: 15342
	public const int RE_SPEAK_AFTER_LINES = 2;

	// Token: 0x04003BEF RID: 15343
	[SerializeField]
	private NPCControlBase npc;

	// Token: 0x04003BF0 RID: 15344
	[SerializeField]
	[Obsolete]
	[HideInInspector]
	private RandomAudioClipTable audioTable;

	// Token: 0x04003BF1 RID: 15345
	[SerializeField]
	private List<NPCSpeakingAudio.SpeakerAudio> speakers;

	// Token: 0x04003BF2 RID: 15346
	[Space]
	public UnityEvent OnPlayedVoiceClip;

	// Token: 0x04003BF3 RID: 15347
	public UnityEvent OnEndedVoiceClip;

	// Token: 0x04003BF4 RID: 15348
	private bool? wasPlayer;

	// Token: 0x04003BF5 RID: 15349
	private bool hasSpokenOnce;

	// Token: 0x04003BF6 RID: 15350
	private bool skipNextSpeak;

	// Token: 0x04003BF7 RID: 15351
	private int linesSinceLastSpeak;

	// Token: 0x04003BF8 RID: 15352
	private RandomAudioClipTable lastSpeakerTable;

	// Token: 0x04003BF9 RID: 15353
	private DialogueBox.DialogueLine lastDialogueLine;

	// Token: 0x04003BFA RID: 15354
	private Dictionary<string, AudioClip> spokenRecord;

	// Token: 0x04003BFB RID: 15355
	private static AudioSource _currentPlayingSource;

	// Token: 0x02001959 RID: 6489
	[Serializable]
	private class SpeakerAudio
	{
		// Token: 0x04009573 RID: 38259
		public RandomAudioClipTable AudioTable;

		// Token: 0x04009574 RID: 38260
		public AudioSource PlayOnSource;

		// Token: 0x04009575 RID: 38261
		public string SpeakerEvent;
	}
}
