using System;
using System.Collections;
using InControl;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

// Token: 0x02000440 RID: 1088
public class StartManager : MonoBehaviour
{
	// Token: 0x0600259F RID: 9631 RVA: 0x000AB8A3 File Offset: 0x000A9AA3
	private IEnumerator Start()
	{
		if (this.inControlManager != null)
		{
			Object.DontDestroyOnLoad(Object.Instantiate<InControlManager>(this.inControlManager).gameObject);
		}
		bool hasLoadedLanguageSelector = false;
		AsyncOperationHandle<GameObject> languageSelectorHandle = default(AsyncOperationHandle<GameObject>);
		LanguageSelector languageSelector = null;
		bool flag = !this.CheckIsLanguageSet();
		bool finished = true;
		bool isLoadingLanguageSelect = flag && Platform.Current.ShowLanguageSelect;
		if (isLoadingLanguageSelect)
		{
			finished = false;
			languageSelectorHandle = this.languageSelectorReference.InstantiateAsyncCustom(base.transform, delegate(bool s)
			{
				languageSelector = this.languageSelectorReference.Component;
				hasLoadedLanguageSelector = languageSelector;
				finished = true;
				if (hasLoadedLanguageSelector)
				{
					languageSelector.SetCamera(this.camera);
					return;
				}
				Debug.LogError(string.Format("Failed to load language selector. {0}", languageSelectorHandle.OperationException), this);
			});
			yield return null;
		}
		Platform.Current.SetSceneLoadState(true, false);
		QuitToMenu.StartLoadCoreManagers();
		yield return null;
		AsyncOperationHandle<SceneInstance> loadHandle = default(AsyncOperationHandle<SceneInstance>);
		bool startedLoadingMenu = false;
		AsyncLoadOrderingManager.DoActionAfterAllLoadsComplete(delegate
		{
			loadHandle = Addressables.LoadSceneAsync("Scenes/Menu_Title", LoadSceneMode.Single, false, 100, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded);
			startedLoadingMenu = true;
		});
		if (isLoadingLanguageSelect)
		{
			if (!languageSelectorHandle.IsValid())
			{
				Debug.Log("Language select handle is invalid");
			}
			if (languageSelectorHandle.IsValid() && !languageSelectorHandle.IsDone)
			{
				yield return languageSelectorHandle;
			}
			while (!finished)
			{
				yield return null;
			}
			if (hasLoadedLanguageSelector)
			{
				yield return languageSelector.DoLanguageSelect();
			}
		}
		bool showIntroSequence = true;
		RuntimePlatform platform = Application.platform;
		bool showLoadingIcon = platform == RuntimePlatform.PS4 || platform == RuntimePlatform.XboxOne || platform == RuntimePlatform.GameCoreXboxOne;
		while (!Platform.Current.IsSharedDataMounted)
		{
			yield return null;
		}
		bool flag2 = false;
		string langCode;
		if (TeamCherry.Localization.LocalizationProjectSettings.TryGetSavedLanguageCode(out langCode))
		{
			LanguageCode languageCode = Language.CurrentLanguage();
			LanguageCode languageEnum = LocalizationSettings.GetLanguageEnum(langCode);
			if (languageCode != languageEnum)
			{
				flag2 = true;
			}
		}
		if (flag2)
		{
			Language.LoadLanguage();
			ChangeFontByLanguage[] array = Object.FindObjectsByType<ChangeFontByLanguage>(FindObjectsSortMode.None);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetFont();
			}
			ChangePositionByLanguage[] array2 = Object.FindObjectsByType<ChangePositionByLanguage>(FindObjectsSortMode.None);
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].DoOffset();
			}
			ActivatePerLanguage[] array3 = Object.FindObjectsByType<ActivatePerLanguage>(FindObjectsSortMode.None);
			for (int i = 0; i < array3.Length; i++)
			{
				array3[i].UpdateLanguage();
			}
			ChangeByLanguageBase[] array4 = Object.FindObjectsByType<ChangeByLanguageBase>(FindObjectsSortMode.None);
			for (int i = 0; i < array4.Length; i++)
			{
				array4[i].DoUpdate();
			}
			SetTextMeshProGameText[] componentsInChildren = base.GetComponentsInChildren<SetTextMeshProGameText>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].UpdateText();
			}
		}
		if (showIntroSequence)
		{
			this.startManagerAnimator.SetBool("WillShowQuote", true);
			this.startManagerAnimator.SetTrigger("Start");
			int loadingIconNameHash = Animator.StringToHash("LoadingIcon");
			while (this.startManagerAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash != loadingIconNameHash)
			{
				yield return null;
			}
		}
		Platform.Current.SetSceneLoadState(true, true);
		if (showLoadingIcon)
		{
			Object.Instantiate<StandaloneLoadingSpinner>(this.loadSpinnerPrefab).Setup(null);
		}
		while (!startedLoadingMenu)
		{
			yield return null;
		}
		yield return loadHandle;
		yield return loadHandle.Result.ActivateAsync();
		yield break;
	}

	// Token: 0x060025A0 RID: 9632 RVA: 0x000AB8B2 File Offset: 0x000A9AB2
	private void OnDestroy()
	{
		AddressableReferenceGameObject<LanguageSelector> addressableReferenceGameObject = this.languageSelectorReference;
		if (addressableReferenceGameObject == null)
		{
			return;
		}
		addressableReferenceGameObject.Dispose();
	}

	// Token: 0x060025A1 RID: 9633 RVA: 0x000AB8C4 File Offset: 0x000A9AC4
	public void SwitchToMenuScene()
	{
		this.loadop.allowSceneActivation = true;
	}

	// Token: 0x060025A2 RID: 9634 RVA: 0x000AB8D2 File Offset: 0x000A9AD2
	public bool CheckIsLanguageSet()
	{
		return Platform.Current.LocalSharedData.GetBool("GameLangSet", false);
	}

	// Token: 0x0400232C RID: 9004
	public Animator startManagerAnimator;

	// Token: 0x0400232D RID: 9005
	[SerializeField]
	private StandaloneLoadingSpinner loadSpinnerPrefab;

	// Token: 0x0400232E RID: 9006
	[SerializeField]
	private InControlManager inControlManager;

	// Token: 0x0400232F RID: 9007
	[Header("Language Select")]
	[SerializeField]
	private Camera camera;

	// Token: 0x04002330 RID: 9008
	[SerializeField]
	private AddressableReferenceGameObject<LanguageSelector> languageSelectorReference;

	// Token: 0x04002331 RID: 9009
	private AsyncOperation loadop;

	// Token: 0x04002332 RID: 9010
	private const float FADE_SPEED = 1.6f;

	// Token: 0x04002333 RID: 9011
	private bool confirmedLanguage;
}
