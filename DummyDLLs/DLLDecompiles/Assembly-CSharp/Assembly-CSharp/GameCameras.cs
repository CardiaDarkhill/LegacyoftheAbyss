using System;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

// Token: 0x0200016B RID: 363
public class GameCameras : MonoBehaviour
{
	// Token: 0x17000104 RID: 260
	// (get) Token: 0x06000B75 RID: 2933 RVA: 0x00034814 File Offset: 0x00032A14
	public static GameCameras instance
	{
		get
		{
			GameCameras silentInstance = GameCameras.SilentInstance;
			if (!silentInstance)
			{
				Debug.LogError("Couldn't find GameCameras, make sure one exists in the scene.");
			}
			return silentInstance;
		}
	}

	// Token: 0x17000105 RID: 261
	// (get) Token: 0x06000B76 RID: 2934 RVA: 0x0003482D File Offset: 0x00032A2D
	public static GameCameras SilentInstance
	{
		get
		{
			if (GameCameras._instance == null)
			{
				GameCameras._instance = Object.FindObjectOfType<GameCameras>();
				if (GameCameras._instance && Application.isPlaying)
				{
					Object.DontDestroyOnLoad(GameCameras._instance.gameObject);
				}
			}
			return GameCameras._instance;
		}
	}

	// Token: 0x17000106 RID: 262
	// (get) Token: 0x06000B77 RID: 2935 RVA: 0x00034870 File Offset: 0x00032A70
	public bool IsHudVisible
	{
		get
		{
			if (!this.hudCanvasSlideOut)
			{
				return false;
			}
			FsmBool fsmBool = this.hudCanvasSlideOut.FsmVariables.FindFsmBool("Is Visible");
			return fsmBool != null && fsmBool.Value;
		}
	}

	// Token: 0x17000107 RID: 263
	// (get) Token: 0x06000B78 RID: 2936 RVA: 0x000348AD File Offset: 0x00032AAD
	// (set) Token: 0x06000B79 RID: 2937 RVA: 0x000348B5 File Offset: 0x00032AB5
	public bool IsInCinematic { get; private set; }

	// Token: 0x06000B7A RID: 2938 RVA: 0x000348C0 File Offset: 0x00032AC0
	private void Awake()
	{
		if (GameCameras._instance == null)
		{
			GameCameras._instance = this;
			Object.DontDestroyOnLoad(this);
			return;
		}
		if (!(this != GameCameras._instance))
		{
			return;
		}
		if (Application.isPlaying)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x00034913 File Offset: 0x00032B13
	private void Start()
	{
		this.gs.LoadOverscanSettings();
		this.SetOverscan(this.gs.overScanAdjustment);
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x00034931 File Offset: 0x00032B31
	public void SceneInit()
	{
		if (this == GameCameras._instance)
		{
			this.StartScene();
		}
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x00034946 File Offset: 0x00032B46
	private void OnDestroy()
	{
		if (GameCameras._instance == this)
		{
			GameCameras._instance = null;
		}
		if (Application.isPlaying)
		{
			Object.Destroy(this.sceneParticles);
			return;
		}
		Object.DestroyImmediate(this.sceneParticles);
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x0003497C File Offset: 0x00032B7C
	private void SetupGameRefs()
	{
		this.gm = GameManager.instance;
		this.gs = this.gm.gameSettings;
		this.canvasScaler = UIManager.instance.canvasScaler;
		if (this.cameraController != null)
		{
			this.cameraController.GameInit();
		}
		else
		{
			Debug.LogError("CameraController not set in inspector.");
		}
		if (this.cameraTarget != null)
		{
			this.cameraTarget.GameInit();
		}
		else
		{
			Debug.LogError("CameraTarget not set in inspector.");
		}
		if (this.sceneParticlesPrefab != null)
		{
			this.sceneParticles = Object.Instantiate<SceneParticlesController>(this.sceneParticlesPrefab);
			this.sceneParticles.name = "SceneParticlesController";
			this.sceneParticles.transform.position = new Vector3(this.tk2dCam.transform.position.x, this.tk2dCam.transform.position.y, 0f);
			this.sceneParticles.transform.SetParent(this.tk2dCam.transform);
		}
		else
		{
			Debug.LogError("Scene Particles Prefab not set in inspector.");
		}
		if (this.sceneColorManager != null)
		{
			this.sceneColorManager.GameInit();
		}
		else
		{
			Debug.LogError("SceneColorManager not set in inspector.");
		}
		this.init = true;
	}

	// Token: 0x06000B7F RID: 2943 RVA: 0x00034AC8 File Offset: 0x00032CC8
	private void StartScene()
	{
		if (!this.init)
		{
			this.SetupGameRefs();
		}
		if (this.gm.IsMenuScene())
		{
			this.MoveMenuToHUDCamera();
			this.hudCamera.GetComponent<HUDCamera>().SetIsGameplayMode(false);
			if (!this.hudCamera.gameObject.activeSelf)
			{
				this.hudCamera.gameObject.SetActive(true);
			}
		}
		else if (this.gm.IsGameplayScene() || this.gm.ShouldKeepHUDCameraActive())
		{
			this.MoveMenuToHUDCamera();
			HUDCamera component = this.hudCamera.GetComponent<HUDCamera>();
			component.EnsureGameMapSpawned();
			component.SetIsGameplayMode(true);
			if (!this.hudCamera.gameObject.activeSelf)
			{
				this.hudCamera.gameObject.SetActive(true);
			}
			if (this.gm.gameMap)
			{
				this.gm.gameMap.InitPinUpdate();
			}
		}
		else
		{
			this.DisableHUDCamIfAllowed();
			if (!this.hudCamera.gameObject.activeSelf && this.gm.IsCinematicScene())
			{
				this.MoveMenuToMainCamera();
			}
		}
		if (this.gm.IsMenuScene())
		{
			this.cameraController.transform.SetPosition2D(14.6f, 8.5f);
		}
		else if (this.gm.IsCinematicScene())
		{
			this.cameraController.transform.SetPosition2D(14.6f, 8.5f);
		}
		else if (this.gm.IsNonGameplayScene())
		{
			if (this.gm.IsBossDoorScene())
			{
				this.cameraController.transform.SetPosition2D(17.5f, 17.5f);
			}
			else if (InGameCutsceneInfo.IsInCutscene)
			{
				this.cameraController.transform.SetPosition2D(InGameCutsceneInfo.CameraPosition);
			}
			else
			{
				this.cameraController.transform.SetPosition2D(14.6f, 8.5f);
			}
		}
		this.cameraController.SceneInit();
		this.cameraTarget.SceneInit();
		this.sceneColorManager.SceneInit();
		this.sceneParticles.SceneInit();
	}

	// Token: 0x06000B80 RID: 2944 RVA: 0x00034CCC File Offset: 0x00032ECC
	public void MoveMenuToHUDCamera()
	{
		UIManager.instance.UICanvas.worldCamera = this.hudCamera;
		UIManager.instance.UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
		UIManager.instance.GenericMessageCanvas.worldCamera = this.hudCamera;
		UIManager.instance.GenericMessageCanvas.renderMode = RenderMode.ScreenSpaceCamera;
		this.hudCamera.cullingMask = 32;
		this.mainCamera.cullingMask &= ~this.hudCamera.cullingMask;
	}

	// Token: 0x06000B81 RID: 2945 RVA: 0x00034D50 File Offset: 0x00032F50
	private void MoveMenuToMainCamera()
	{
		UIManager.instance.UICanvas.worldCamera = this.mainCamera;
		UIManager.instance.UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
		UIManager.instance.GenericMessageCanvas.worldCamera = this.mainCamera;
		UIManager.instance.GenericMessageCanvas.renderMode = RenderMode.ScreenSpaceCamera;
		this.hudCamera.cullingMask = 0;
		this.mainCamera.cullingMask |= 32;
	}

	// Token: 0x06000B82 RID: 2946 RVA: 0x00034DC7 File Offset: 0x00032FC7
	public void SetMainCameraActive(bool value)
	{
		this.mainCamera.enabled = value;
		if (value)
		{
			this.hudCamera.clearFlags = CameraClearFlags.Depth;
			return;
		}
		this.hudCamera.clearFlags = CameraClearFlags.Color;
		this.hudCamera.backgroundColor = Color.black;
	}

	// Token: 0x06000B83 RID: 2947 RVA: 0x00034E04 File Offset: 0x00033004
	public void DisableHUDCamIfAllowed()
	{
		if (this.gm.IsNonGameplayScene() && !this.gm.IsStagTravelScene() && !this.gm.IsBossDoorScene() && !this.gm.ShouldKeepHUDCameraActive())
		{
			this.hudCamera.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000B84 RID: 2948 RVA: 0x00034E56 File Offset: 0x00033056
	public void StopCameraShake()
	{
		this.cameraShakeFSM.Fsm.Event("CANCEL SHAKE");
	}

	// Token: 0x06000B85 RID: 2949 RVA: 0x00034E6D File Offset: 0x0003306D
	public void ResumeCameraShake()
	{
		this.cameraShakeFSM.Fsm.Event("RESUME SHAKE");
	}

	// Token: 0x06000B86 RID: 2950 RVA: 0x00034E84 File Offset: 0x00033084
	public void OnCinematicBegin()
	{
		this.IsInCinematic = true;
		this.cameraController.ApplyEffectConfiguration();
		this.mainCamera.GetComponent<BloomOptimized>().enabled = false;
		this.mainCamera.GetComponent<ColorCorrectionCurves>().enabled = false;
	}

	// Token: 0x06000B87 RID: 2951 RVA: 0x00034EBA File Offset: 0x000330BA
	public void OnCinematicEnd()
	{
		this.IsInCinematic = false;
		this.cameraController.ApplyEffectConfiguration();
	}

	// Token: 0x06000B88 RID: 2952 RVA: 0x00034ED0 File Offset: 0x000330D0
	public void SetOverscan(float value)
	{
		if (!this.init)
		{
			this.SetupGameRefs();
		}
		base.Invoke("TestParentForPosition", 0.33f * Time.timeScale);
		float num = (float)Screen.width / (float)Screen.height;
		if (this.canvasScaler == null)
		{
			this.canvasScaler = UIManager.instance.canvasScaler;
		}
		this.canvasScaler.referenceResolution = new Vector2(1920f * (1f - value) + -220f * value, 1080f * (1f - value) + 1f / num * (-220f * value));
		this.forceCameraAspect.SetOverscanViewport(value);
		this.gs.overScanAdjustment = value;
	}

	// Token: 0x06000B89 RID: 2953 RVA: 0x00034F88 File Offset: 0x00033188
	public void TestParentForPosition()
	{
		if (this.cameraParent.transform.localPosition.z != 0f)
		{
			this.cameraParent.transform.localPosition = new Vector3(this.cameraParent.transform.localPosition.x, this.cameraParent.transform.localPosition.y, 0f);
		}
	}

	// Token: 0x06000B8A RID: 2954 RVA: 0x00034FF5 File Offset: 0x000331F5
	public void HUDOut()
	{
		this.hudCanvasSlideOut.SendEventSafe("OUT");
	}

	// Token: 0x06000B8B RID: 2955 RVA: 0x00035007 File Offset: 0x00033207
	public void HUDIn()
	{
		this.hudCanvasSlideOut.SendEventSafe("IN");
	}

	// Token: 0x04000B18 RID: 2840
	[Header("Cameras")]
	public Camera hudCamera;

	// Token: 0x04000B19 RID: 2841
	public Camera mainCamera;

	// Token: 0x04000B1A RID: 2842
	[Header("Controllers")]
	public CameraController cameraController;

	// Token: 0x04000B1B RID: 2843
	public CameraTarget cameraTarget;

	// Token: 0x04000B1C RID: 2844
	public ForceCameraAspect forceCameraAspect;

	// Token: 0x04000B1D RID: 2845
	public WorldRumbleManager worldRumbleManager;

	// Token: 0x04000B1E RID: 2846
	[Header("FSMs")]
	public PlayMakerFSM cameraFadeFSM;

	// Token: 0x04000B1F RID: 2847
	public PlayMakerFSM cameraShakeFSM;

	// Token: 0x04000B20 RID: 2848
	public PlayMakerFSM soulOrbFSM;

	// Token: 0x04000B21 RID: 2849
	public PlayMakerFSM soulVesselFSM;

	// Token: 0x04000B22 RID: 2850
	public PlayMakerFSM openStagFSM;

	// Token: 0x04000B23 RID: 2851
	public PlayMakerFSM hudCanvasSlideOut;

	// Token: 0x04000B24 RID: 2852
	[Header("Camera Effects")]
	public ColorCorrectionCurves colorCorrectionCurves;

	// Token: 0x04000B25 RID: 2853
	public SceneColorManager sceneColorManager;

	// Token: 0x04000B26 RID: 2854
	public BrightnessEffect brightnessEffect;

	// Token: 0x04000B27 RID: 2855
	public SceneParticlesController sceneParticlesPrefab;

	// Token: 0x04000B28 RID: 2856
	private SceneParticlesController sceneParticles;

	// Token: 0x04000B29 RID: 2857
	[Header("Other")]
	public tk2dCamera tk2dCam;

	// Token: 0x04000B2A RID: 2858
	public Transform cameraParent;

	// Token: 0x04000B2B RID: 2859
	public SilkSpool silkSpool;

	// Token: 0x04000B2C RID: 2860
	private GameManager gm;

	// Token: 0x04000B2D RID: 2861
	private GameSettings gs;

	// Token: 0x04000B2E RID: 2862
	private CanvasScaler canvasScaler;

	// Token: 0x04000B2F RID: 2863
	private bool init;

	// Token: 0x04000B30 RID: 2864
	private static GameCameras _instance;
}
