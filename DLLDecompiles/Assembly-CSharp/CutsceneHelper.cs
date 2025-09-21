using System;
using System.Collections;
using GlobalEnums;
using GlobalSettings;
using UnityEngine;

// Token: 0x0200007D RID: 125
public class CutsceneHelper : MonoBehaviour, GameManager.ISkippable
{
	// Token: 0x0600037B RID: 891 RVA: 0x00011EAB File Offset: 0x000100AB
	private IEnumerator Start()
	{
		this.gm = GameManager.instance;
		this.gm.RegisterSkippable(this);
		this.skipped = false;
		if (this.startSkipLocked)
		{
			this.gm.inputHandler.SetSkipMode(SkipPromptMode.NOT_SKIPPABLE);
		}
		else
		{
			this.gm.inputHandler.SetSkipMode(this.skipMode);
		}
		GameCameras.instance.DisableHUDCamIfAllowed();
		yield return new WaitForSeconds(this.waitBeforeFadeIn);
		if (this.fadeInSpeed == CameraFadeInType.SLOW)
		{
			GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE SCENE IN SLOWLY");
		}
		else if (this.fadeInSpeed == CameraFadeInType.NORMAL)
		{
			GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE SCENE IN");
		}
		else if (this.fadeInSpeed == CameraFadeInType.INSTANT)
		{
			GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE SCENE IN INSTANT");
		}
		yield break;
	}

	// Token: 0x0600037C RID: 892 RVA: 0x00011EBA File Offset: 0x000100BA
	private void OnDestroy()
	{
		if (this.gm)
		{
			this.gm.DeregisterSkippable(this);
		}
	}

	// Token: 0x0600037D RID: 893 RVA: 0x00011ED5 File Offset: 0x000100D5
	public void LoadNextScene()
	{
		GameCameras.instance.cameraFadeFSM.SendEventSafe("FADE INSTANT");
		this.DoSceneLoad();
	}

	// Token: 0x0600037E RID: 894 RVA: 0x00011EF1 File Offset: 0x000100F1
	public IEnumerator Skip()
	{
		if (this.skipped)
		{
			yield break;
		}
		this.skipped = true;
		Audio.StopConfirmSound.PlayOnSource(this.skipAudioSource);
		if (this.fadeTransitionAudioOnSkip)
		{
			TransitionAudioFader.FadeOutAllFaders();
		}
		PlayMakerFSM.BroadcastEvent("JUST FADE");
		yield return new WaitForSeconds(0.5f);
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		this.DoSceneLoad();
		yield break;
	}

	// Token: 0x0600037F RID: 895 RVA: 0x00011F00 File Offset: 0x00010100
	public void UnlockSkip()
	{
		this.gm.inputHandler.SetSkipMode(this.skipMode);
	}

	// Token: 0x06000380 RID: 896 RVA: 0x00011F18 File Offset: 0x00010118
	private void DoSceneLoad()
	{
		if (this.isLoadingScene)
		{
			return;
		}
		InputHandler instance = ManagerSingleton<InputHandler>.Instance;
		if (instance)
		{
			instance.StopAcceptingInput();
		}
		this.isLoadingScene = true;
		switch (this.nextSceneType)
		{
		case CutsceneHelper.NextScene.SpecifyScene:
			GameManager.instance.LoadScene(this.nextScene);
			return;
		case CutsceneHelper.NextScene.MainMenu:
			GameManager.instance.StartCoroutine(GameManager.instance.ReturnToMainMenu(true, null, true, false));
			return;
		case CutsceneHelper.NextScene.PermaDeathUnlock:
			GameManager.instance.LoadPermadeathUnlockScene();
			return;
		case CutsceneHelper.NextScene.GameCompletionScreen:
			GameManager.instance.LoadScene("End_Game_Completion");
			return;
		case CutsceneHelper.NextScene.EndCredits:
			GameManager.instance.LoadScene("End_Credits");
			return;
		case CutsceneHelper.NextScene.MrMushroomUnlock:
			break;
		case CutsceneHelper.NextScene.GGReturn:
			GameManager.instance.BeginSceneTransition(new GameManager.SceneLoadInfo
			{
				SceneName = this.nextScene,
				EntryGateName = GameManager.instance.playerData.bossReturnEntryGate,
				EntryDelay = 0f,
				PreventCameraFadeOut = true,
				WaitForSceneTransitionCameraFade = false
			});
			return;
		case CutsceneHelper.NextScene.MainMenuNoSave:
			GameManager.instance.StartCoroutine(GameManager.instance.ReturnToMainMenu(false, null, false, false));
			break;
		default:
			return;
		}
	}

	// Token: 0x0400031A RID: 794
	public float waitBeforeFadeIn;

	// Token: 0x0400031B RID: 795
	public CameraFadeInType fadeInSpeed;

	// Token: 0x0400031C RID: 796
	public SkipPromptMode skipMode;

	// Token: 0x0400031D RID: 797
	[SerializeField]
	private AudioSource skipAudioSource;

	// Token: 0x0400031E RID: 798
	[Tooltip("Prevents the skip action from taking place until the lock is released. Useful for animators delaying skip feature.")]
	public bool startSkipLocked;

	// Token: 0x0400031F RID: 799
	public CutsceneHelper.NextScene nextSceneType;

	// Token: 0x04000320 RID: 800
	public string nextScene;

	// Token: 0x04000321 RID: 801
	[SerializeField]
	private bool fadeTransitionAudioOnSkip;

	// Token: 0x04000322 RID: 802
	private GameManager gm;

	// Token: 0x04000323 RID: 803
	private bool isLoadingScene;

	// Token: 0x04000324 RID: 804
	private bool skipped;

	// Token: 0x020013F3 RID: 5107
	public enum NextScene
	{
		// Token: 0x0400814E RID: 33102
		SpecifyScene,
		// Token: 0x0400814F RID: 33103
		MainMenu,
		// Token: 0x04008150 RID: 33104
		PermaDeathUnlock,
		// Token: 0x04008151 RID: 33105
		GameCompletionScreen,
		// Token: 0x04008152 RID: 33106
		EndCredits,
		// Token: 0x04008153 RID: 33107
		MrMushroomUnlock,
		// Token: 0x04008154 RID: 33108
		GGReturn,
		// Token: 0x04008155 RID: 33109
		MainMenuNoSave
	}
}
