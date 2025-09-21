using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200012A RID: 298
public static class PersistentAudioManager
{
	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x0600093B RID: 2363 RVA: 0x0002AB12 File Offset: 0x00028D12
	// (set) Token: 0x0600093C RID: 2364 RVA: 0x0002AB19 File Offset: 0x00028D19
	public static bool Paused { get; set; }

	// Token: 0x0600093D RID: 2365 RVA: 0x0002AB24 File Offset: 0x00028D24
	public static void AddInstance(PersistentAudioInstance instance)
	{
		if (instance == null)
		{
			return;
		}
		if (string.IsNullOrEmpty(instance.Key))
		{
			return;
		}
		PersistentAudioManager.PersistentAudioData persistentAudioData;
		if (!PersistentAudioManager.dictionary.TryGetValue(instance.Key, out persistentAudioData))
		{
			persistentAudioData = (PersistentAudioManager.dictionary[instance.Key] = new PersistentAudioManager.PersistentAudioData());
		}
		persistentAudioData.AddInstance(instance);
		PersistentAudioManager.activeInstances.Add(instance);
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x0002AB88 File Offset: 0x00028D88
	public static void RemoveInstance(PersistentAudioInstance instance)
	{
		if (instance == null)
		{
			return;
		}
		PersistentAudioManager.PersistentAudioData persistentAudioData;
		if (PersistentAudioManager.dictionary.TryGetValue(instance.Key, out persistentAudioData))
		{
			persistentAudioData.instances.Remove(instance);
			PersistentAudioManager.activeInstances.Remove(instance);
			if (persistentAudioData.instances.Count == 0)
			{
				PersistentAudioManager.dictionary.Remove(instance.Key);
			}
		}
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x0002ABEC File Offset: 0x00028DEC
	public static void Update()
	{
		if (PersistentAudioManager.queuedEntryState >= 2 && PersistentAudioManager.queuedEntryState++ > 4)
		{
			PersistentAudioManager.OnEnteredNextScene();
			PersistentAudioManager.queuedEntryState = 0;
		}
		if (PersistentAudioManager.Paused)
		{
			return;
		}
		for (int i = PersistentAudioManager.activeInstances.Count - 1; i >= 0; i--)
		{
			PersistentAudioManager.activeInstances[i].UpdateVolume();
		}
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x0002AC4C File Offset: 0x00028E4C
	public static void MarkOldInstancesForRemoval()
	{
		foreach (PersistentAudioInstance persistentAudioInstance in PersistentAudioManager.activeInstances)
		{
			persistentAudioInstance.MarkForDestroy();
		}
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x0002AC9C File Offset: 0x00028E9C
	public static void AttachToObject(Transform parent)
	{
		foreach (PersistentAudioManager.PersistentAudioData persistentAudioData in PersistentAudioManager.dictionary.Values)
		{
			persistentAudioData.AttachToObject(parent);
		}
	}

	// Token: 0x06000942 RID: 2370 RVA: 0x0002ACF4 File Offset: 0x00028EF4
	public static void UpdateInstancePositions()
	{
		foreach (PersistentAudioManager.PersistentAudioData persistentAudioData in PersistentAudioManager.dictionary.Values)
		{
			persistentAudioData.UpdatePositions();
		}
	}

	// Token: 0x06000943 RID: 2371 RVA: 0x0002AD48 File Offset: 0x00028F48
	public static void MarkAsPreviousScene()
	{
		foreach (PersistentAudioInstance persistentAudioInstance in PersistentAudioManager.activeInstances)
		{
			persistentAudioInstance.IsFromPreviousScene = true;
		}
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x0002AD98 File Offset: 0x00028F98
	public static void QueueSceneEntry()
	{
		if (PersistentAudioManager.queuedEntryState <= 0)
		{
			PersistentAudioManager.queuedEntryState = 1;
		}
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x0002ADA8 File Offset: 0x00028FA8
	public static void OnLevelLoaded()
	{
		if (PersistentAudioManager.queuedEntryState == 1)
		{
			PersistentAudioManager.queuedEntryState = 2;
		}
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x0002ADB8 File Offset: 0x00028FB8
	public static void OnLeaveScene()
	{
		PersistentAudioManager.Paused = true;
		PersistentAudioManager.MarkAsPreviousScene();
		PersistentAudioManager.MarkOldInstancesForRemoval();
		GameManager instance = GameManager.instance;
		if (instance)
		{
			PersistentAudioManager.AttachToObject(instance.cameraCtrl.transform);
		}
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x0002ADF3 File Offset: 0x00028FF3
	public static void OnEnteredNextScene()
	{
		PersistentAudioManager.Paused = false;
		PersistentAudioManager.AttachToObject(null);
		PersistentAudioManager.UpdateInstancePositions();
		PersistentAudioManager.queuedEntryState = 0;
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x0002AE0C File Offset: 0x0002900C
	public static void ClearAndReset()
	{
		PersistentAudioManager.Paused = false;
		for (int i = PersistentAudioManager.activeInstances.Count - 1; i >= 0; i--)
		{
			Object.Destroy(PersistentAudioManager.activeInstances[i].gameObject);
		}
		PersistentAudioManager.activeInstances.Clear();
		PersistentAudioManager.dictionary.Clear();
	}

	// Token: 0x040008EA RID: 2282
	private static Dictionary<string, PersistentAudioManager.PersistentAudioData> dictionary = new Dictionary<string, PersistentAudioManager.PersistentAudioData>();

	// Token: 0x040008EB RID: 2283
	private static List<PersistentAudioInstance> activeInstances = new List<PersistentAudioInstance>();

	// Token: 0x040008ED RID: 2285
	private static int queuedEntryState;

	// Token: 0x02001469 RID: 5225
	public sealed class PersistentAudioData
	{
		// Token: 0x06008377 RID: 33655 RVA: 0x00268E8C File Offset: 0x0026708C
		public void AddInstance(PersistentAudioInstance instance)
		{
			this.instances.RemoveAll((PersistentAudioInstance o) => o == null);
			this.lastAddedPosition = instance.transform.position;
			if (this.instances.Count > 0)
			{
				foreach (PersistentAudioInstance persistentAudioInstance in this.instances)
				{
					if (instance.AlsoSetOtherChangeRate)
					{
						persistentAudioInstance.SetChangeRate(instance.FadeInRate);
					}
					persistentAudioInstance.MarkForDestroy();
				}
				bool adoptPreviousPlayingState = instance.AdoptPreviousPlayingState;
				foreach (PersistentAudioInstance persistentAudioInstance2 in this.instances)
				{
					if (persistentAudioInstance2.AudioSource.isPlaying)
					{
						AudioSource audioSource = persistentAudioInstance2.AudioSource;
						instance.QueueFadeUp();
						if (adoptPreviousPlayingState)
						{
							instance.AudioSource.clip = audioSource.clip;
							if (audioSource.isPlaying)
							{
								instance.AudioSource.Play();
							}
						}
						if (adoptPreviousPlayingState || (instance.AudioSource != null && audioSource.clip == instance.AudioSource.clip))
						{
							instance.AudioSource.timeSamples = audioSource.timeSamples;
							instance.SetSyncTarget(audioSource);
							break;
						}
						break;
					}
				}
			}
			this.instances.Add(instance);
		}

		// Token: 0x06008378 RID: 33656 RVA: 0x00269020 File Offset: 0x00267220
		public void UpdatePositions()
		{
			foreach (PersistentAudioInstance persistentAudioInstance in this.instances)
			{
				if (persistentAudioInstance.AdoptNewInstancePosition && persistentAudioInstance.IsFromPreviousScene)
				{
					persistentAudioInstance.transform.position = this.lastAddedPosition;
				}
			}
		}

		// Token: 0x06008379 RID: 33657 RVA: 0x00269090 File Offset: 0x00267290
		public void AttachToObject(Transform transform)
		{
			foreach (PersistentAudioInstance persistentAudioInstance in this.instances)
			{
				if (persistentAudioInstance.KeepRelativePositionInNewScene && persistentAudioInstance.IsFromPreviousScene)
				{
					persistentAudioInstance.transform.SetParent(transform);
				}
			}
		}

		// Token: 0x0400831B RID: 33563
		public List<PersistentAudioInstance> instances = new List<PersistentAudioInstance>();

		// Token: 0x0400831C RID: 33564
		private Vector3 lastAddedPosition;
	}
}
