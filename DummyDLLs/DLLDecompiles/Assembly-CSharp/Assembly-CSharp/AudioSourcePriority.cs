using System;
using UnityEngine;

// Token: 0x02000117 RID: 279
public sealed class AudioSourcePriority : MonoBehaviour
{
	// Token: 0x060008A3 RID: 2211 RVA: 0x0002881C File Offset: 0x00026A1C
	private void Awake()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
			if (this.audioSource == null)
			{
				Debug.LogError("AudioSourcePriorityAssigner requires an AudioSource component on " + base.gameObject.name + ".", this);
				base.enabled = false;
				return;
			}
		}
		this.UpdatePriority();
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x0002887F File Offset: 0x00026A7F
	private void OnValidate()
	{
		if (this.audioSource == null)
		{
			this.audioSource = base.GetComponent<AudioSource>();
		}
		if (this.audioSource != null)
		{
			this.UpdatePriority();
		}
	}

	// Token: 0x060008A5 RID: 2213 RVA: 0x000288AF File Offset: 0x00026AAF
	private void UpdatePriority()
	{
		this.audioSource.priority = this.InternalGetPriority(this.sourceType);
	}

	// Token: 0x060008A6 RID: 2214 RVA: 0x000288C8 File Offset: 0x00026AC8
	private int InternalGetPriority(AudioSourcePriority.SourceType sourceType)
	{
		int num = 128;
		if (sourceType >= AudioSourcePriority.SourceType.Music && sourceType < (AudioSourcePriority.SourceType)AudioSourcePriority.PRIORITY_TABLE.Length)
		{
			num = AudioSourcePriority.PRIORITY_TABLE[(int)sourceType];
		}
		else
		{
			Debug.LogWarning(string.Format("Priority level {0} is out of range for the priority table on {1}.", sourceType, base.gameObject.name), this);
		}
		return Mathf.Clamp(num + this.offset, 0, 256);
	}

	// Token: 0x060008A7 RID: 2215 RVA: 0x0002892C File Offset: 0x00026B2C
	public static int GetPriority(AudioSourcePriority.SourceType sourceType)
	{
		int value = 128;
		if (sourceType >= AudioSourcePriority.SourceType.Music && sourceType < (AudioSourcePriority.SourceType)AudioSourcePriority.PRIORITY_TABLE.Length)
		{
			value = AudioSourcePriority.PRIORITY_TABLE[(int)sourceType];
		}
		else
		{
			Debug.LogWarning(string.Format("Priority level {0} is out of range for the priority table.", sourceType));
		}
		return Mathf.Clamp(value, 0, 256);
	}

	// Token: 0x04000848 RID: 2120
	private const int DEFAULT_PRIORITY = 128;

	// Token: 0x04000849 RID: 2121
	private const int MIN_PRIORITY = 0;

	// Token: 0x0400084A RID: 2122
	private const int MAX_PRIORITY = 256;

	// Token: 0x0400084B RID: 2123
	private static readonly int[] PRIORITY_TABLE = new int[]
	{
		0,
		10,
		100,
		128,
		118
	};

	// Token: 0x0400084C RID: 2124
	public static readonly int SPAWNED_ACTOR_PRIORITY = AudioSourcePriority.GetPriority(AudioSourcePriority.SourceType.SpawnedActor);

	// Token: 0x0400084D RID: 2125
	[Header("Audio Source Priority Settings")]
	[SerializeField]
	private AudioSourcePriority.SourceType sourceType;

	// Token: 0x0400084E RID: 2126
	[SerializeField]
	private int offset;

	// Token: 0x0400084F RID: 2127
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x02001460 RID: 5216
	[Serializable]
	public enum SourceType
	{
		// Token: 0x040082FB RID: 33531
		Music,
		// Token: 0x040082FC RID: 33532
		Atmos,
		// Token: 0x040082FD RID: 33533
		Hero,
		// Token: 0x040082FE RID: 33534
		BackgroundLoop,
		// Token: 0x040082FF RID: 33535
		SpawnedActor
	}
}
