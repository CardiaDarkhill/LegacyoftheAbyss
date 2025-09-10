using System;
using System.Collections;
using System.Collections.Generic;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

// Token: 0x020007CC RID: 1996
public class SceneAdditiveLoadConditional : MonoBehaviour, IApplyExtraLoadSettings
{
	// Token: 0x140000F1 RID: 241
	// (add) Token: 0x0600464C RID: 17996 RVA: 0x001310E8 File Offset: 0x0012F2E8
	// (remove) Token: 0x0600464D RID: 17997 RVA: 0x00131120 File Offset: 0x0012F320
	public event Action ApplyExtraLoadSettings;

	// Token: 0x170007EF RID: 2031
	// (get) Token: 0x0600464E RID: 17998 RVA: 0x00131155 File Offset: 0x0012F355
	private string SceneNameToLoad
	{
		get
		{
			if (!this.loadAlt)
			{
				return this.sceneNameToLoad;
			}
			return this.altSceneNameToLoad;
		}
	}

	// Token: 0x170007F0 RID: 2032
	// (get) Token: 0x0600464F RID: 17999 RVA: 0x0013116C File Offset: 0x0012F36C
	public static bool ShouldLoadBoss
	{
		get
		{
			return SceneAdditiveLoadConditional._additiveSceneLoads != null && SceneAdditiveLoadConditional._additiveSceneLoads.Count > 0;
		}
	}

	// Token: 0x170007F1 RID: 2033
	// (get) Token: 0x06004650 RID: 18000 RVA: 0x00131184 File Offset: 0x0012F384
	public static bool IsAnyLoaded
	{
		get
		{
			using (List<SceneAdditiveLoadConditional>.Enumerator enumerator = SceneAdditiveLoadConditional._additiveSceneLoads.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.sceneLoaded)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	// Token: 0x06004651 RID: 18001 RVA: 0x001311DC File Offset: 0x0012F3DC
	private bool? IsSceneNameValid(string sceneName)
	{
		return null;
	}

	// Token: 0x06004652 RID: 18002 RVA: 0x001311F2 File Offset: 0x0012F3F2
	private void OnEnable()
	{
		if (!SceneAdditiveLoadConditional.LoadInSequence)
		{
			return;
		}
		if (this.sceneLoaded)
		{
			return;
		}
		if (this.TryTestLoad())
		{
			SceneAdditiveLoadConditional._additiveSceneLoads.Add(this);
		}
	}

	// Token: 0x06004653 RID: 18003 RVA: 0x00131218 File Offset: 0x0012F418
	private void Start()
	{
		if (SceneAdditiveLoadConditional.LoadInSequence)
		{
			return;
		}
		if (this.sceneLoaded)
		{
			return;
		}
		if (this.TryTestLoad())
		{
			SceneAdditiveLoadConditional._additiveSceneLoads.Add(this);
			this.ApplySettings();
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				if (SceneManager.GetSceneAt(i).name == this.SceneNameToLoad)
				{
					this.sceneLoaded = true;
					return;
				}
			}
			base.StartCoroutine(this.LoadRoutine(true, this));
		}
	}

	// Token: 0x06004654 RID: 18004 RVA: 0x00131291 File Offset: 0x0012F491
	private void ApplySettings()
	{
		if (this.appliedSettings)
		{
			return;
		}
		this.appliedSettings = true;
		Action applyExtraLoadSettings = this.ApplyExtraLoadSettings;
		if (applyExtraLoadSettings == null)
		{
			return;
		}
		applyExtraLoadSettings();
	}

	// Token: 0x06004655 RID: 18005 RVA: 0x001312B4 File Offset: 0x0012F4B4
	private bool TryTestLoad()
	{
		bool flag = !this.tests.IsFulfilled;
		foreach (QuestTest questTest in this.questTests)
		{
			if (!questTest.IsFulfilled)
			{
				flag = true;
				break;
			}
		}
		string entryGateName = GameManager.instance.entryGateName;
		if (!flag && this.doorWhiteList.Length != 0)
		{
			bool flag2 = false;
			string[] array2 = this.doorWhiteList;
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i].Equals(entryGateName))
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				flag = true;
			}
		}
		if (!flag && this.doorBlackList.Length != 0)
		{
			string[] array2 = this.doorBlackList;
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i].Equals(entryGateName))
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag && this.otherLoaderBlacklist.Length != 0)
		{
			SceneAdditiveLoadConditional[] array3 = this.otherLoaderBlacklist;
			for (int i = 0; i < array3.Length; i++)
			{
				if (array3[i].TryTestLoad())
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			this.loadAlt = true;
		}
		return !string.IsNullOrEmpty(this.SceneNameToLoad);
	}

	// Token: 0x06004656 RID: 18006 RVA: 0x001313C0 File Offset: 0x0012F5C0
	private void OnDisable()
	{
		this.Unload();
	}

	// Token: 0x06004657 RID: 18007 RVA: 0x001313CC File Offset: 0x0012F5CC
	private AsyncOperationHandle<SceneInstance>? Unload()
	{
		if (!this.sceneLoaded)
		{
			return null;
		}
		this.sceneLoaded = false;
		SceneAdditiveLoadConditional._additiveSceneLoads.Remove(this);
		if (this.loadOp != null)
		{
			return new AsyncOperationHandle<SceneInstance>?(Addressables.UnloadSceneAsync(this.loadOp.Value, true));
		}
		return null;
	}

	// Token: 0x06004658 RID: 18008 RVA: 0x0013142C File Offset: 0x0012F62C
	public static void Unload(Scene owningScene, List<AsyncOperationHandle<SceneInstance>> storeOperations)
	{
		for (int i = SceneAdditiveLoadConditional._additiveSceneLoads.Count - 1; i >= 0; i--)
		{
			SceneAdditiveLoadConditional sceneAdditiveLoadConditional = SceneAdditiveLoadConditional._additiveSceneLoads[i];
			if (!(sceneAdditiveLoadConditional.gameObject.scene != owningScene))
			{
				AsyncOperationHandle<SceneInstance>? asyncOperationHandle = sceneAdditiveLoadConditional.Unload();
				if (asyncOperationHandle != null)
				{
					storeOperations.Add(asyncOperationHandle.Value);
				}
			}
		}
	}

	// Token: 0x06004659 RID: 18009 RVA: 0x0013148C File Offset: 0x0012F68C
	public static IEnumerator LoadAll()
	{
		if (SceneAdditiveLoadConditional._additiveSceneLoads != null)
		{
			foreach (SceneAdditiveLoadConditional sceneAdditiveLoadConditional in SceneAdditiveLoadConditional._additiveSceneLoads)
			{
				if (sceneAdditiveLoadConditional)
				{
					yield return sceneAdditiveLoadConditional.StartCoroutine(sceneAdditiveLoadConditional.LoadRoutine(false, sceneAdditiveLoadConditional));
				}
			}
			List<SceneAdditiveLoadConditional>.Enumerator enumerator = default(List<SceneAdditiveLoadConditional>.Enumerator);
		}
		SceneAdditiveLoadConditional.LoadInSequence = false;
		yield break;
		yield break;
	}

	// Token: 0x0600465A RID: 18010 RVA: 0x00131494 File Offset: 0x0012F694
	private IEnumerator LoadRoutine(bool callEvent, SceneAdditiveLoadConditional sceneLoader)
	{
		this.ApplySettings();
		bool loadInSequence = SceneAdditiveLoadConditional.LoadInSequence;
		yield return null;
		string text = "Scenes/" + this.SceneNameToLoad;
		this.loadOp = new AsyncOperationHandle<SceneInstance>?(ScenePreloader.TakeSceneLoadOperation(text, LoadSceneMode.Additive) ?? Addressables.LoadSceneAsync(text, LoadSceneMode.Additive, true, 100, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded));
		yield return this.loadOp;
		if (this.loadOp.Value.OperationException != null)
		{
			Debug.LogError("Additive scene load for " + this.SceneNameToLoad + " failed with exception:");
			Debug.LogException(this.loadOp.Value.OperationException, this);
		}
		else
		{
			this.sceneLoaded = true;
			if (this.repositionScene)
			{
				GameObject[] rootGameObjects = SceneManager.GetSceneByName(this.SceneNameToLoad).GetRootGameObjects();
				Vector3 position = base.transform.position;
				GameObject[] array = rootGameObjects;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].transform.position += position;
				}
			}
		}
		if (callEvent && GameManager.instance)
		{
			GameManager.instance.LoadedBoss();
		}
		sceneLoader.OnWasLoaded();
		yield break;
	}

	// Token: 0x0600465B RID: 18011 RVA: 0x001314B1 File Offset: 0x0012F6B1
	private void OnWasLoaded()
	{
		if (!string.IsNullOrEmpty(this.setPdBoolOnLoad))
		{
			PlayerData.instance.SetVariable(this.setPdBoolOnLoad, true);
		}
	}

	// Token: 0x040046BE RID: 18110
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsSceneNameValid")]
	private string sceneNameToLoad;

	// Token: 0x040046BF RID: 18111
	[SerializeField]
	[ModifiableProperty]
	[InspectorValidation("IsSceneNameValid")]
	private string altSceneNameToLoad;

	// Token: 0x040046C0 RID: 18112
	[SerializeField]
	private PlayerDataTest tests;

	// Token: 0x040046C1 RID: 18113
	[SerializeField]
	private QuestTest[] questTests;

	// Token: 0x040046C2 RID: 18114
	[Space]
	[SerializeField]
	private string[] doorWhiteList;

	// Token: 0x040046C3 RID: 18115
	[SerializeField]
	private string[] doorBlackList;

	// Token: 0x040046C4 RID: 18116
	[SerializeField]
	private SceneAdditiveLoadConditional[] otherLoaderBlacklist;

	// Token: 0x040046C5 RID: 18117
	[Space]
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string setPdBoolOnLoad;

	// Token: 0x040046C6 RID: 18118
	[Space]
	[SerializeField]
	private bool repositionScene;

	// Token: 0x040046C8 RID: 18120
	private bool appliedSettings;

	// Token: 0x040046C9 RID: 18121
	private bool loadAlt;

	// Token: 0x040046CA RID: 18122
	private bool sceneLoaded;

	// Token: 0x040046CB RID: 18123
	private AsyncOperationHandle<SceneInstance>? loadOp;

	// Token: 0x040046CC RID: 18124
	private static readonly List<SceneAdditiveLoadConditional> _additiveSceneLoads = new List<SceneAdditiveLoadConditional>();

	// Token: 0x040046CD RID: 18125
	public static bool LoadInSequence;
}
