using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using GenericVariableExtension;
using GlobalEnums;
using GlobalSettings;
using InControl;
using TeamCherry.Localization;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

// Token: 0x02000418 RID: 1048
public class GameManager : MonoBehaviour
{
	// Token: 0x170003AE RID: 942
	// (get) Token: 0x06002398 RID: 9112 RVA: 0x000A2CBA File Offset: 0x000A0EBA
	// (set) Token: 0x06002399 RID: 9113 RVA: 0x000A2CC2 File Offset: 0x000A0EC2
	public GameState GameState { get; private set; }

	// Token: 0x1400006D RID: 109
	// (add) Token: 0x0600239A RID: 9114 RVA: 0x000A2CCC File Offset: 0x000A0ECC
	// (remove) Token: 0x0600239B RID: 9115 RVA: 0x000A2D04 File Offset: 0x000A0F04
	public event GameManager.PausedEvent GamePausedChange;

	// Token: 0x1400006E RID: 110
	// (add) Token: 0x0600239C RID: 9116 RVA: 0x000A2D3C File Offset: 0x000A0F3C
	// (remove) Token: 0x0600239D RID: 9117 RVA: 0x000A2D74 File Offset: 0x000A0F74
	public event GameManager.GameStateEvent GameStateChange;

	// Token: 0x1400006F RID: 111
	// (add) Token: 0x0600239E RID: 9118 RVA: 0x000A2DAC File Offset: 0x000A0FAC
	// (remove) Token: 0x0600239F RID: 9119 RVA: 0x000A2DE4 File Offset: 0x000A0FE4
	public event Action SceneInit;

	// Token: 0x170003AF RID: 943
	// (get) Token: 0x060023A0 RID: 9120 RVA: 0x000A2E19 File Offset: 0x000A1019
	public bool TimeSlowed
	{
		get
		{
			return this.timeSlowedCount > 0;
		}
	}

	// Token: 0x170003B0 RID: 944
	// (get) Token: 0x060023A1 RID: 9121 RVA: 0x000A2E24 File Offset: 0x000A1024
	// (set) Token: 0x060023A2 RID: 9122 RVA: 0x000A2E2C File Offset: 0x000A102C
	public Random SceneSeededRandom { get; private set; }

	// Token: 0x170003B1 RID: 945
	// (get) Token: 0x060023A3 RID: 9123 RVA: 0x000A2E35 File Offset: 0x000A1035
	// (set) Token: 0x060023A4 RID: 9124 RVA: 0x000A2E3D File Offset: 0x000A103D
	public InputHandler inputHandler { get; private set; }

	// Token: 0x170003B2 RID: 946
	// (get) Token: 0x060023A5 RID: 9125 RVA: 0x000A2E46 File Offset: 0x000A1046
	// (set) Token: 0x060023A6 RID: 9126 RVA: 0x000A2E4E File Offset: 0x000A104E
	public AchievementHandler achievementHandler { get; private set; }

	// Token: 0x170003B3 RID: 947
	// (get) Token: 0x060023A7 RID: 9127 RVA: 0x000A2E57 File Offset: 0x000A1057
	public AudioManager AudioManager
	{
		get
		{
			return this.audioManager;
		}
	}

	// Token: 0x170003B4 RID: 948
	// (get) Token: 0x060023A8 RID: 9128 RVA: 0x000A2E5F File Offset: 0x000A105F
	// (set) Token: 0x060023A9 RID: 9129 RVA: 0x000A2E66 File Offset: 0x000A1066
	public static bool IsCollectingGarbage { get; set; }

	// Token: 0x060023AA RID: 9130 RVA: 0x000A2E70 File Offset: 0x000A1070
	public void SpawnInControlManager()
	{
		bool flag = false;
		try
		{
			flag = (SingletonMonoBehavior<InControlManager>.Instance != null);
		}
		catch
		{
		}
		if (!flag)
		{
			Object.DontDestroyOnLoad(Object.Instantiate<InControlManager>(this.inControlManagerPrefab).gameObject);
		}
	}

	// Token: 0x170003B5 RID: 949
	// (get) Token: 0x060023AB RID: 9131 RVA: 0x000A2EB8 File Offset: 0x000A10B8
	// (set) Token: 0x060023AC RID: 9132 RVA: 0x000A2EC0 File Offset: 0x000A10C0
	public CameraController cameraCtrl { get; private set; }

	// Token: 0x170003B6 RID: 950
	// (get) Token: 0x060023AD RID: 9133 RVA: 0x000A2EC9 File Offset: 0x000A10C9
	// (set) Token: 0x060023AE RID: 9134 RVA: 0x000A2ED1 File Offset: 0x000A10D1
	public HeroController hero_ctrl { get; private set; }

	// Token: 0x170003B7 RID: 951
	// (get) Token: 0x060023AF RID: 9135 RVA: 0x000A2EDA File Offset: 0x000A10DA
	// (set) Token: 0x060023B0 RID: 9136 RVA: 0x000A2EE2 File Offset: 0x000A10E2
	public SpriteRenderer heroLight { get; private set; }

	// Token: 0x170003B8 RID: 952
	// (get) Token: 0x060023B1 RID: 9137 RVA: 0x000A2EEB File Offset: 0x000A10EB
	// (set) Token: 0x060023B2 RID: 9138 RVA: 0x000A2EF3 File Offset: 0x000A10F3
	public CustomSceneManager sm { get; private set; }

	// Token: 0x170003B9 RID: 953
	// (get) Token: 0x060023B3 RID: 9139 RVA: 0x000A2EFC File Offset: 0x000A10FC
	// (set) Token: 0x060023B4 RID: 9140 RVA: 0x000A2F04 File Offset: 0x000A1104
	public UIManager ui { get; private set; }

	// Token: 0x170003BA RID: 954
	// (get) Token: 0x060023B5 RID: 9141 RVA: 0x000A2F0D File Offset: 0x000A110D
	// (set) Token: 0x060023B6 RID: 9142 RVA: 0x000A2F15 File Offset: 0x000A1115
	public tk2dTileMap tilemap { get; private set; }

	// Token: 0x170003BB RID: 955
	// (get) Token: 0x060023B7 RID: 9143 RVA: 0x000A2F1E File Offset: 0x000A111E
	// (set) Token: 0x060023B8 RID: 9144 RVA: 0x000A2F26 File Offset: 0x000A1126
	public PlayMakerFSM soulOrb_fsm { get; private set; }

	// Token: 0x170003BC RID: 956
	// (get) Token: 0x060023B9 RID: 9145 RVA: 0x000A2F2F File Offset: 0x000A112F
	// (set) Token: 0x060023BA RID: 9146 RVA: 0x000A2F37 File Offset: 0x000A1137
	public PlayMakerFSM soulVessel_fsm { get; private set; }

	// Token: 0x170003BD RID: 957
	// (get) Token: 0x060023BB RID: 9147 RVA: 0x000A2F40 File Offset: 0x000A1140
	// (set) Token: 0x060023BC RID: 9148 RVA: 0x000A2F48 File Offset: 0x000A1148
	public PlayMakerFSM inventoryFSM { get; private set; }

	// Token: 0x170003BE RID: 958
	// (get) Token: 0x060023BD RID: 9149 RVA: 0x000A2F51 File Offset: 0x000A1151
	// (set) Token: 0x060023BE RID: 9150 RVA: 0x000A2F59 File Offset: 0x000A1159
	public PlayMakerFSM screenFader_fsm { get; private set; }

	// Token: 0x170003BF RID: 959
	// (get) Token: 0x060023BF RID: 9151 RVA: 0x000A2F62 File Offset: 0x000A1162
	public float PlayTime
	{
		get
		{
			return this.sessionStartTime + this.sessionPlayTimer;
		}
	}

	// Token: 0x170003C0 RID: 960
	// (get) Token: 0x060023C0 RID: 9152 RVA: 0x000A2F71 File Offset: 0x000A1171
	// (set) Token: 0x060023C1 RID: 9153 RVA: 0x000A2F79 File Offset: 0x000A1179
	public bool RespawningHero { get; set; }

	// Token: 0x14000070 RID: 112
	// (add) Token: 0x060023C2 RID: 9154 RVA: 0x000A2F84 File Offset: 0x000A1184
	// (remove) Token: 0x060023C3 RID: 9155 RVA: 0x000A2FBC File Offset: 0x000A11BC
	public event Action SavePersistentObjects;

	// Token: 0x14000071 RID: 113
	// (add) Token: 0x060023C4 RID: 9156 RVA: 0x000A2FF4 File Offset: 0x000A11F4
	// (remove) Token: 0x060023C5 RID: 9157 RVA: 0x000A302C File Offset: 0x000A122C
	public event Action ResetSemiPersistentObjects;

	// Token: 0x14000072 RID: 114
	// (add) Token: 0x060023C6 RID: 9158 RVA: 0x000A3064 File Offset: 0x000A1264
	// (remove) Token: 0x060023C7 RID: 9159 RVA: 0x000A309C File Offset: 0x000A129C
	public event Action NextSceneWillActivate;

	// Token: 0x14000073 RID: 115
	// (add) Token: 0x060023C8 RID: 9160 RVA: 0x000A30D4 File Offset: 0x000A12D4
	// (remove) Token: 0x060023C9 RID: 9161 RVA: 0x000A310C File Offset: 0x000A130C
	public event Action UnloadingLevel;

	// Token: 0x14000074 RID: 116
	// (add) Token: 0x060023CA RID: 9162 RVA: 0x000A3144 File Offset: 0x000A1344
	// (remove) Token: 0x060023CB RID: 9163 RVA: 0x000A317C File Offset: 0x000A137C
	public event Action RefreshLanguageText;

	// Token: 0x14000075 RID: 117
	// (add) Token: 0x060023CC RID: 9164 RVA: 0x000A31B4 File Offset: 0x000A13B4
	// (remove) Token: 0x060023CD RID: 9165 RVA: 0x000A31EC File Offset: 0x000A13EC
	public event Action RefreshParticleLevel;

	// Token: 0x14000076 RID: 118
	// (add) Token: 0x060023CE RID: 9166 RVA: 0x000A3224 File Offset: 0x000A1424
	// (remove) Token: 0x060023CF RID: 9167 RVA: 0x000A325C File Offset: 0x000A145C
	public event GameManager.BossLoad OnLoadedBoss;

	// Token: 0x14000077 RID: 119
	// (add) Token: 0x060023D0 RID: 9168 RVA: 0x000A3294 File Offset: 0x000A1494
	// (remove) Token: 0x060023D1 RID: 9169 RVA: 0x000A32CC File Offset: 0x000A14CC
	public event GameManager.EnterSceneEvent OnFinishedEnteringScene;

	// Token: 0x14000078 RID: 120
	// (add) Token: 0x060023D2 RID: 9170 RVA: 0x000A3304 File Offset: 0x000A1504
	// (remove) Token: 0x060023D3 RID: 9171 RVA: 0x000A333C File Offset: 0x000A153C
	public event GameManager.SceneTransitionFinishEvent OnBeforeFinishedSceneTransition;

	// Token: 0x14000079 RID: 121
	// (add) Token: 0x060023D4 RID: 9172 RVA: 0x000A3374 File Offset: 0x000A1574
	// (remove) Token: 0x060023D5 RID: 9173 RVA: 0x000A33AC File Offset: 0x000A15AC
	public event GameManager.SceneTransitionFinishEvent OnFinishedSceneTransition;

	// Token: 0x170003C1 RID: 961
	// (get) Token: 0x060023D6 RID: 9174 RVA: 0x000A33E1 File Offset: 0x000A15E1
	// (set) Token: 0x060023D7 RID: 9175 RVA: 0x000A33E9 File Offset: 0x000A15E9
	public bool IsInSceneTransition { get; private set; }

	// Token: 0x170003C2 RID: 962
	// (get) Token: 0x060023D8 RID: 9176 RVA: 0x000A33F2 File Offset: 0x000A15F2
	// (set) Token: 0x060023D9 RID: 9177 RVA: 0x000A33F9 File Offset: 0x000A15F9
	public static bool SuppressRegainControl { get; set; }

	// Token: 0x170003C3 RID: 963
	// (get) Token: 0x060023DA RID: 9178 RVA: 0x000A3401 File Offset: 0x000A1601
	public bool HasFinishedEnteringScene
	{
		get
		{
			return this.hasFinishedEnteringScene;
		}
	}

	// Token: 0x170003C4 RID: 964
	// (get) Token: 0x060023DB RID: 9179 RVA: 0x000A3409 File Offset: 0x000A1609
	public bool IsLoadingSceneTransition
	{
		get
		{
			return (this.sceneLoad == null || this.sceneLoad.SceneLoadInfo.IsReadyToActivate()) && this.isLoading;
		}
	}

	// Token: 0x170003C5 RID: 965
	// (get) Token: 0x060023DC RID: 9180 RVA: 0x000A342D File Offset: 0x000A162D
	public GameManager.SceneLoadVisualizations LoadVisualization
	{
		get
		{
			return this.loadVisualization;
		}
	}

	// Token: 0x170003C6 RID: 966
	// (get) Token: 0x060023DD RID: 9181 RVA: 0x000A3435 File Offset: 0x000A1635
	public float CurrentLoadDuration
	{
		get
		{
			if (!this.isLoading)
			{
				return 0f;
			}
			return this.currentLoadDuration;
		}
	}

	// Token: 0x170003C7 RID: 967
	// (get) Token: 0x060023DE RID: 9182 RVA: 0x000A344B File Offset: 0x000A164B
	// (set) Token: 0x060023DF RID: 9183 RVA: 0x000A3453 File Offset: 0x000A1653
	public int QueuedBlueHealth
	{
		get
		{
			return this.queuedBlueHealth;
		}
		set
		{
			this.queuedBlueHealth = Mathf.Max(value, 0);
		}
	}

	// Token: 0x170003C8 RID: 968
	// (get) Token: 0x060023E0 RID: 9184 RVA: 0x000A3462 File Offset: 0x000A1662
	// (set) Token: 0x060023E1 RID: 9185 RVA: 0x000A346A File Offset: 0x000A166A
	public bool IsFirstLevelForPlayer { get; private set; }

	// Token: 0x170003C9 RID: 969
	// (get) Token: 0x060023E2 RID: 9186 RVA: 0x000A3473 File Offset: 0x000A1673
	// (set) Token: 0x060023E3 RID: 9187 RVA: 0x000A347A File Offset: 0x000A167A
	public static bool IsWaitingForSceneReady { get; private set; }

	// Token: 0x170003CA RID: 970
	// (get) Token: 0x060023E4 RID: 9188 RVA: 0x000A3482 File Offset: 0x000A1682
	public static GameManager instance
	{
		get
		{
			GameManager silentInstance = GameManager.SilentInstance;
			if (!silentInstance)
			{
				Debug.LogError("Couldn't find a Game Manager, make sure one exists in the scene.");
			}
			return silentInstance;
		}
	}

	// Token: 0x170003CB RID: 971
	// (get) Token: 0x060023E5 RID: 9189 RVA: 0x000A349C File Offset: 0x000A169C
	public static GameManager SilentInstance
	{
		get
		{
			if (GameManager._instance != null)
			{
				return GameManager._instance;
			}
			GameManager._instance = Object.FindObjectOfType<GameManager>();
			if (GameManager._instance && Application.isPlaying)
			{
				Object.DontDestroyOnLoad(GameManager._instance.gameObject);
			}
			return GameManager._instance;
		}
	}

	// Token: 0x170003CC RID: 972
	// (get) Token: 0x060023E6 RID: 9190 RVA: 0x000A34ED File Offset: 0x000A16ED
	public static GameManager UnsafeInstance
	{
		get
		{
			return GameManager._instance;
		}
	}

	// Token: 0x060023E7 RID: 9191 RVA: 0x000A34F4 File Offset: 0x000A16F4
	private void Awake()
	{
		PlayMakerPrefs.LogPerformanceWarnings = false;
		if (GameManager._instance == null)
		{
			GameManager._instance = this;
			Object.DontDestroyOnLoad(this);
			this.SetupGameRefs();
		}
		else
		{
			if (this != GameManager._instance)
			{
				Object.Destroy(base.gameObject);
				return;
			}
			this.SetupGameRefs();
		}
		PlayMakerGlobals.Instance.Variables.FindFsmGameObject("GameManager").Value = base.gameObject;
	}

	// Token: 0x060023E8 RID: 9192 RVA: 0x000A3566 File Offset: 0x000A1766
	private void Start()
	{
		if (this == GameManager._instance)
		{
			this.SetupStatusModifiers();
			if (Platform.Current)
			{
				Platform.Current.SetGameManagerReady();
			}
		}
	}

	// Token: 0x060023E9 RID: 9193 RVA: 0x000A3591 File Offset: 0x000A1791
	private void OnDestroy()
	{
		if (GameManager._instance == this)
		{
			GameManager._instance = null;
			PlayerData.ClearOptimisers();
			VariableExtensions.ClearCache();
			VariableExtensionsGeneric.ClearCache();
			GameManager.IsCollectingGarbage = false;
		}
		this.UnregisterEvents();
	}

	// Token: 0x060023EA RID: 9194 RVA: 0x000A35C4 File Offset: 0x000A17C4
	protected void Update()
	{
		AudioGroupManager.UpdateAudioGroups();
		if (this.isLoading)
		{
			this.currentLoadDuration += Time.unscaledDeltaTime;
		}
		else
		{
			this.currentLoadDuration = 0f;
		}
		this.IncreaseGameTimer(ref this.sessionPlayTimer);
		this.IncreaseGameTimer(ref this.timeInScene);
		if (this.timeInScene < 300f)
		{
			this.IncreaseGameTimer(ref this.timeSinceLastTimePasses);
		}
		CrossSceneWalker.Tick();
		this.UpdateEngagement();
	}

	// Token: 0x060023EB RID: 9195 RVA: 0x000A363C File Offset: 0x000A183C
	private void UpdateEngagement()
	{
		if (this.GameState == GameState.MAIN_MENU)
		{
			if (!this.ui.didLeaveEngageMenu)
			{
				if (this.ui.menuState == MainMenuState.ENGAGE_MENU)
				{
					Platform.EngagementStates engagementState = Platform.Current.EngagementState;
					if (engagementState != Platform.EngagementStates.Engaged)
					{
						if (engagementState != Platform.EngagementStates.EngagePending && Input.anyKeyDown)
						{
							Platform.Current.BeginEngagement();
						}
						Platform.Current.UpdateWaitingForEngagement();
						return;
					}
					if (!Platform.Current.IsSavingAllowedByEngagement || Platform.Current.IsSaveStoreMounted)
					{
						this.ui.didLeaveEngageMenu = true;
						this.ui.UIGoToMainMenu();
						return;
					}
				}
			}
			else if (Platform.Current.EngagementState != Platform.EngagementStates.Engaged && this.inputHandler.acceptingInput && this.ui.menuState != MainMenuState.ENGAGE_MENU)
			{
				this.ui.UIGoToEngageMenu();
				this.ui.slotOne.ClearCache();
				this.ui.slotTwo.ClearCache();
				this.ui.slotThree.ClearCache();
				this.ui.slotFour.ClearCache();
				return;
			}
		}
		else if ((this.GameState == GameState.PLAYING || this.GameState == GameState.PAUSED) && Platform.Current.EngagementState == Platform.EngagementStates.NotEngaged && !this.didEmergencyQuit)
		{
			this.didEmergencyQuit = true;
			this.EmergencyReturnToMenu();
		}
	}

	// Token: 0x060023EC RID: 9196 RVA: 0x000A3784 File Offset: 0x000A1984
	private void LevelActivated(Scene sceneFrom, Scene sceneTo)
	{
		if (this != GameManager._instance)
		{
			return;
		}
		PersistentAudioManager.OnLevelLoaded();
		if (!this.waitForManualLevelStart)
		{
			this.SetupSceneRefs(true);
			this.BeginScene();
			this.OnNextLevelReady();
		}
	}

	// Token: 0x060023ED RID: 9197 RVA: 0x000A37B4 File Offset: 0x000A19B4
	private void OnDisable()
	{
		SceneManager.activeSceneChanged -= this.LevelActivated;
	}

	// Token: 0x060023EE RID: 9198 RVA: 0x000A37C7 File Offset: 0x000A19C7
	private void OnApplicationQuit()
	{
		if (this.startedLanguageDisabled)
		{
			this.gameConfig.hideLanguageOption = true;
		}
	}

	// Token: 0x060023EF RID: 9199 RVA: 0x000A37DD File Offset: 0x000A19DD
	public void BeginSceneTransition(GameManager.SceneLoadInfo info)
	{
		this.inventoryFSM.SendEvent("INVENTORY CANCEL");
		if (info.IsFirstLevelForPlayer)
		{
			this.ResetGameTimer();
			this.LoadedFromMenu();
		}
		base.StartCoroutine(this.BeginSceneTransitionRoutine(info));
	}

	// Token: 0x170003CD RID: 973
	// (get) Token: 0x060023F0 RID: 9200 RVA: 0x000A3811 File Offset: 0x000A1A11
	// (set) Token: 0x060023F1 RID: 9201 RVA: 0x000A3819 File Offset: 0x000A1A19
	public SceneLoad LastSceneLoad { get; set; }

	// Token: 0x1400007A RID: 122
	// (add) Token: 0x060023F2 RID: 9202 RVA: 0x000A3824 File Offset: 0x000A1A24
	// (remove) Token: 0x060023F3 RID: 9203 RVA: 0x000A3858 File Offset: 0x000A1A58
	public static event GameManager.SceneTransitionBeganDelegate SceneTransitionBegan;

	// Token: 0x060023F4 RID: 9204 RVA: 0x000A388B File Offset: 0x000A1A8B
	private IEnumerator BeginSceneTransitionRoutine(GameManager.SceneLoadInfo info)
	{
		GameManager.<>c__DisplayClass237_0 CS$<>8__locals1 = new GameManager.<>c__DisplayClass237_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.info = info;
		GameManager.SuppressRegainControl = false;
		if (this.sceneLoad != null)
		{
			yield break;
		}
		this.cameraCtrl.ResetPositionedAtHero();
		this.IsInSceneTransition = true;
		this.hasFinishedEnteringScene = false;
		StaticVariableList.ReportSceneTransition();
		this.sceneLoad = new SceneLoad(this, CS$<>8__locals1.info);
		this.isLoading = true;
		this.loadVisualization = CS$<>8__locals1.info.Visualization;
		CS$<>8__locals1.unloadingSceneLoad = this.LastSceneLoad;
		this.LastSceneLoad = this.sceneLoad;
		NonTinter.ClearNonTinters();
		if (this.hero_ctrl != null)
		{
			FSMUtility.SendEventToGameObject(this.hero_ctrl.gameObject, "ROAR EXIT", false);
			this.hero_ctrl.LeavingScene();
			this.hero_ctrl.SetHeroParent(null);
		}
		if (!CS$<>8__locals1.info.IsFirstLevelForPlayer)
		{
			this.NoLongerFirstGame();
		}
		else
		{
			this.IsFirstLevelForPlayer = true;
		}
		this.SaveLevelState();
		if (this.GameState != GameState.CUTSCENE)
		{
			this.SetState(GameState.EXITING_LEVEL);
		}
		this.entryGateName = (CS$<>8__locals1.info.EntryGateName ?? "");
		this.targetScene = CS$<>8__locals1.info.SceneName;
		if (this.hero_ctrl != null)
		{
			this.hero_ctrl.LeaveScene(CS$<>8__locals1.info.HeroLeaveDirection);
		}
		if (!CS$<>8__locals1.info.PreventCameraFadeOut)
		{
			this.cameraCtrl.FreezeInPlace(true);
			this.screenFader_fsm.SendEvent("SCENE FADE OUT");
		}
		EventRegister.SendEvent(EventRegisterEvents.SceneTransitionBegan, null);
		this.startedOnThisScene = false;
		this.nextSceneName = CS$<>8__locals1.info.SceneName;
		this.waitForManualLevelStart = true;
		GameManager.IsWaitingForSceneReady = true;
		CS$<>8__locals1.previousMapZone = this.sm.mapZone;
		CS$<>8__locals1.forcedNotMemory = this.sm.ForceNotMemory;
		this.actorSnapshotPaused.TransitionToSafe(this.sceneTransitionActorFadeDown);
		SilkSpool.EndSilkAudio();
		Action unloadingLevel = this.UnloadingLevel;
		if (unloadingLevel != null)
		{
			unloadingLevel();
		}
		CS$<>8__locals1.unloadingSceneName = SceneManager.GetActiveScene().name;
		CS$<>8__locals1.didBlankScreen = false;
		this.sceneLoad.FetchComplete += CS$<>8__locals1.<BeginSceneTransitionRoutine>g__FetchCompleteAction|0;
		this.sceneLoad.WillActivate += CS$<>8__locals1.<BeginSceneTransitionRoutine>g__WillActivateAction|1;
		this.sceneLoad.ActivationComplete += CS$<>8__locals1.<BeginSceneTransitionRoutine>g__ActivationCompleteAction|2;
		this.sceneLoad.Complete += CS$<>8__locals1.<BeginSceneTransitionRoutine>g__CompleteAction|3;
		this.sceneLoad.Finish += CS$<>8__locals1.<BeginSceneTransitionRoutine>g__FinishAction|4;
		if (GameManager.SceneTransitionBegan != null)
		{
			try
			{
				GameManager.SceneTransitionBegan(this.sceneLoad);
			}
			catch (Exception ex)
			{
				Debug.LogError("Exception in responders to GameManager.SceneTransitionBegan. Attempting to continue load regardless.");
				CheatManager.LastErrorText = ex.ToString();
				Debug.LogException(ex);
			}
		}
		this.sceneLoad.IsFetchAllowed = (!CS$<>8__locals1.info.ForceWaitFetch && (Platform.Current.FetchScenesBeforeFade || CS$<>8__locals1.info.PreventCameraFadeOut));
		this.sceneLoad.IsActivationAllowed = false;
		this.sceneLoad.WaitForFade = CS$<>8__locals1.info.WaitForSceneTransitionCameraFade;
		float cameraFadeTimer;
		if (CS$<>8__locals1.info.WaitForSceneTransitionCameraFade)
		{
			cameraFadeTimer = 0.34f;
		}
		else
		{
			cameraFadeTimer = 0f;
			CS$<>8__locals1.info.NotifyFadedOut();
			this.sceneLoad.WaitForFade = false;
		}
		Platform.Current.SetSceneLoadState(true, false);
		if (CS$<>8__locals1.info.SceneName == CS$<>8__locals1.unloadingSceneName)
		{
			Debug.Log("Reloading same scene! Destroying GuidComponents to prevent collisions...");
			foreach (GuidComponent guidComponent in Object.FindObjectsByType<GuidComponent>(FindObjectsInactive.Include, FindObjectsSortMode.None))
			{
				if (guidComponent.gameObject.scene.name == CS$<>8__locals1.info.SceneName)
				{
					Object.Destroy(guidComponent);
				}
			}
		}
		this.sceneLoad.Begin();
		for (;;)
		{
			bool flag = false;
			if (cameraFadeTimer > 0f)
			{
				cameraFadeTimer -= Time.deltaTime;
				if (cameraFadeTimer > 0f)
				{
					flag = true;
				}
				else
				{
					CS$<>8__locals1.info.NotifyFadedOut();
					this.sceneLoad.WaitForFade = false;
				}
			}
			if (!CS$<>8__locals1.info.IsReadyToActivate())
			{
				flag = true;
			}
			if (!flag)
			{
				break;
			}
			yield return null;
		}
		VibrationManager.StopAllVibration();
		Platform.Current.SetSceneLoadState(true, true);
		Platform.Current.OnScreenFaded();
		this.sceneLoad.IsFetchAllowed = true;
		this.sceneLoad.IsActivationAllowed = true;
		yield break;
	}

	// Token: 0x060023F5 RID: 9205 RVA: 0x000A38A1 File Offset: 0x000A1AA1
	private static void ReportUnload(string sn)
	{
		PersonalObjectPool.PreUnloadingScene(sn);
		PersonalObjectPool.UnloadingScene(sn);
	}

	// Token: 0x060023F6 RID: 9206 RVA: 0x000A38AF File Offset: 0x000A1AAF
	private void UnloadScene(string unloadingSceneName, SceneLoad unloadingSceneLoad)
	{
		GameManager.ReportUnload(unloadingSceneName);
		if (unloadingSceneLoad == null && this.LastSceneLoad != null && this.LastSceneLoad.TargetSceneName == unloadingSceneName)
		{
			unloadingSceneLoad = this.LastSceneLoad;
		}
		SceneManager.UnloadScene(unloadingSceneName);
	}

	// Token: 0x060023F7 RID: 9207 RVA: 0x000A38E4 File Offset: 0x000A1AE4
	public IEnumerator TransitionScene(TransitionPoint gate)
	{
		Debug.LogError("TransitionScene(TransitionPoint) is no longer supported");
		this.callingGate = gate;
		this.hero_ctrl.LeavingScene();
		this.NoLongerFirstGame();
		this.SaveLevelState();
		this.SetState(GameState.EXITING_LEVEL);
		this.entryGateName = gate.entryPoint;
		this.targetScene = gate.targetScene;
		this.hero_ctrl.LeaveScene(new GatePosition?(gate.GetGatePosition()));
		this.cameraCtrl.FreezeInPlace(true);
		this.screenFader_fsm.SendEvent("SCENE FADE OUT");
		this.hasFinishedEnteringScene = false;
		yield return new WaitForSeconds(0.34f);
		this.LeftScene(true);
		yield break;
	}

	// Token: 0x060023F8 RID: 9208 RVA: 0x000A38FC File Offset: 0x000A1AFC
	public void ChangeToScene(string targetScene, string entryGateName, float pauseBeforeEnter)
	{
		if (this.hero_ctrl != null)
		{
			this.hero_ctrl.LeavingScene();
			this.hero_ctrl.transform.SetParent(null);
		}
		this.NoLongerFirstGame();
		this.SaveLevelState();
		this.SetState(GameState.EXITING_LEVEL);
		this.entryGateName = entryGateName;
		this.targetScene = targetScene;
		this.entryDelay = pauseBeforeEnter;
		this.cameraCtrl.FreezeInPlace(false);
		if (this.hero_ctrl != null)
		{
			this.hero_ctrl.ResetState();
		}
		this.LeftScene(false);
	}

	// Token: 0x060023F9 RID: 9209 RVA: 0x000A3987 File Offset: 0x000A1B87
	public void LeftScene(bool doAdditiveLoad = false)
	{
		if (doAdditiveLoad)
		{
			base.StartCoroutine(this.LoadSceneAdditive(this.targetScene));
			return;
		}
		this.LoadScene(this.targetScene);
	}

	// Token: 0x060023FA RID: 9210 RVA: 0x000A39AC File Offset: 0x000A1BAC
	public IEnumerator PlayerDead(float waitTime)
	{
		this.cameraCtrl.FreezeInPlace(true);
		this.NoLongerFirstGame();
		this.ResetSemiPersistentItems();
		MazeController.ResetSaveData();
		bool isPermaDead = this.playerData.permadeathMode == PermadeathModes.Dead;
		bool finishedSaving;
		bool willDemoEnd;
		if (DemoHelper.IsDemoMode)
		{
			this.heroDeathCount++;
			int maxDeathCount = Demo.MaxDeathCount;
			willDemoEnd = (this.heroDeathCount >= maxDeathCount && maxDeathCount > 0);
			finishedSaving = true;
		}
		else
		{
			willDemoEnd = false;
			finishedSaving = false;
			this.SaveGame(this.profileID, delegate(bool _)
			{
				finishedSaving = true;
			}, false, AutoSaveName.NONE);
			if (!isPermaDead)
			{
				string text;
				string text2;
				this.GetRespawnInfo(out text, out text2);
				ScenePreloader.SpawnPreloader(text, LoadSceneMode.Additive);
			}
		}
		yield return new WaitForSeconds(waitTime);
		while (!finishedSaving)
		{
			yield return null;
		}
		if (willDemoEnd)
		{
			this.BeginSceneTransition(new GameManager.SceneLoadInfo
			{
				SceneName = "Demo End",
				PreventCameraFadeOut = true,
				WaitForSceneTransitionCameraFade = false,
				Visualization = GameManager.SceneLoadVisualizations.Default
			});
			yield break;
		}
		this.TimePasses();
		if (!isPermaDead)
		{
			this.ReadyForRespawn(false);
		}
		else
		{
			this.LoadScene("PermaDeath");
		}
		this.ResetSemiPersistentItems();
		yield break;
	}

	// Token: 0x060023FB RID: 9211 RVA: 0x000A39C2 File Offset: 0x000A1BC2
	public IEnumerator PlayerDeadFromHazard(float waitTime)
	{
		this.cameraCtrl.FreezeInPlace(true);
		this.NoLongerFirstGame();
		this.SaveLevelState();
		yield return new WaitForSeconds(waitTime);
		this.screenFader_fsm.SendEventSafe("HAZARD FADE");
		EventRegister.SendEvent(EventRegisterEvents.HazardFade, null);
		yield return new WaitForSeconds(0.65f);
		PlayMakerFSM.BroadcastEvent("HAZARD RELOAD");
		EventRegister.SendEvent(EventRegisterEvents.HazardReload, null);
		if (!this.hero_ctrl.cState.dead)
		{
			this.HazardRespawn();
		}
		yield break;
	}

	// Token: 0x060023FC RID: 9212 RVA: 0x000A39D8 File Offset: 0x000A1BD8
	private void GetRespawnInfo(out string scene, out string marker)
	{
		string savedRespawnScene;
		string savedRespawnMarker;
		if (!string.IsNullOrEmpty(this.playerData.tempRespawnScene))
		{
			savedRespawnScene = this.playerData.tempRespawnScene;
			savedRespawnMarker = this.playerData.tempRespawnMarker;
		}
		else
		{
			savedRespawnScene = this.playerData.respawnScene;
			savedRespawnMarker = this.playerData.respawnMarkerName;
		}
		Dictionary<string, SceneTeleportMap.SceneInfo> teleportMap = SceneTeleportMap.GetTeleportMap();
		if (!Application.isEditor)
		{
			SceneTeleportMap.SceneInfo sceneInfo;
			if (teleportMap.TryGetValue(savedRespawnScene, out sceneInfo))
			{
				if (sceneInfo.RespawnPoints.Contains(savedRespawnMarker))
				{
					scene = savedRespawnScene;
					marker = savedRespawnMarker;
					return;
				}
				if ((from kvp in teleportMap
				where kvp.Key.StartsWith(savedRespawnScene)
				select kvp).Any((KeyValuePair<string, SceneTeleportMap.SceneInfo> kvp) => kvp.Value.RespawnPoints.Contains(savedRespawnMarker)))
				{
					scene = savedRespawnScene;
					marker = savedRespawnMarker;
					return;
				}
			}
			scene = "Tut_01";
			marker = "Death Respawn Marker Init";
			this.playerData.ResetTempRespawn();
			return;
		}
		scene = savedRespawnScene;
		marker = savedRespawnMarker;
		SceneTeleportMap.SceneInfo sceneInfo2;
		if (!teleportMap.TryGetValue(savedRespawnScene, out sceneInfo2))
		{
			return;
		}
		if (sceneInfo2.RespawnPoints.Contains(savedRespawnMarker))
		{
			return;
		}
		(from kvp in teleportMap
		where kvp.Key.StartsWith(savedRespawnScene)
		select kvp).Any((KeyValuePair<string, SceneTeleportMap.SceneInfo> kvp) => kvp.Value.RespawnPoints.Contains(savedRespawnMarker));
	}

	// Token: 0x060023FD RID: 9213 RVA: 0x000A3B30 File Offset: 0x000A1D30
	public void ReadyForRespawn(bool isFirstLevelForPlayer)
	{
		this.RespawningHero = true;
		string text;
		string text2;
		this.GetRespawnInfo(out text, out text2);
		this.BeginSceneTransition(new GameManager.SceneLoadInfo
		{
			PreventCameraFadeOut = true,
			WaitForSceneTransitionCameraFade = false,
			EntryGateName = text2,
			SceneName = text,
			Visualization = (isFirstLevelForPlayer ? GameManager.SceneLoadVisualizations.ContinueFromSave : GameManager.SceneLoadVisualizations.Default),
			AlwaysUnloadUnusedAssets = true,
			IsFirstLevelForPlayer = isFirstLevelForPlayer
		});
	}

	// Token: 0x060023FE RID: 9214 RVA: 0x000A3B90 File Offset: 0x000A1D90
	public void HazardRespawn()
	{
		this.hazardRespawningHero = true;
		this.cameraCtrl.ResetStartTimer();
		if (!this.cameraCtrl.camTarget.IsFreeModeManual)
		{
			this.cameraCtrl.camTarget.mode = CameraTarget.TargetMode.FOLLOW_HERO;
		}
		this.EnterHero();
	}

	// Token: 0x060023FF RID: 9215 RVA: 0x000A3BD0 File Offset: 0x000A1DD0
	public void TimePasses()
	{
		StaticVariableList.ClearSceneTransitions();
		string sceneNameString = this.GetSceneNameString();
		MapZone currentMapZoneEnum = this.GetCurrentMapZoneEnum();
		if (this.playerData.seenBellBeast && this.playerData.shermaPos == 0)
		{
			this.playerData.shermaPos = 1;
		}
		if (this.playerData.shermaPos == 1 && sceneNameString != "Bone_East_10_Room" && (this.playerData.scenesVisited.Contains("Belltown") || this.playerData.scenesVisited.Contains("Halfway_01")))
		{
			this.playerData.shermaPos = 2;
		}
		if (this.playerData.spinnerDefeated && (this.playerData.encounteredLastJudge || (this.playerData.activatedStepsUpperBellbench && this.playerData.defeatedCoralDrillers) || (this.playerData.visitedCoral && currentMapZoneEnum == MapZone.DUSTPENS) || currentMapZoneEnum == MapZone.SWAMP))
		{
			this.playerData.shermaAtSteps = true;
		}
		if (this.playerData.SeenLastJudgeGateOpen && !this.playerData.shermaCitadelEntrance_Visiting && this.playerData.enteredCoral_10 && this.playerData.enteredSong_19 && this.playerData.citadelWoken)
		{
			this.playerData.shermaCitadelEntrance_Visiting = true;
		}
		if ((this.playerData.shermaCitadelEntrance_Seen || this.playerData.citadelHalfwayComplete) && !this.playerData.shermaCitadelEntrance_Left)
		{
			this.playerData.shermaCitadelEntrance_Left = true;
		}
		if (this.playerData.shermaCitadelSpa_Seen && !this.playerData.shermaCitadelSpa_Left)
		{
			this.playerData.shermaCitadelSpa_Left = true;
			this.playerData.shermaCitadelEntrance_Left = true;
		}
		if (this.playerData.enclaveLevel >= 2 && sceneNameString != "Song_Enclave" && sceneNameString != "Bellshrine_Enclave")
		{
			this.playerData.shermaInEnclave = true;
		}
		if (!this.playerData.shermaHealerActive && QuestManager.GetQuest("Save Sherma").IsCompleted && sceneNameString != "Song_Enclave" && sceneNameString != "Bellshrine_Enclave")
		{
			this.playerData.shermaHealerActive = true;
		}
		if (this.playerData.shermaHealerActive && sceneNameString != "Song_Enclave" && sceneNameString != "Bellshrine_Enclave")
		{
			this.playerData.shermaWoundedPilgrim = Random.Range(1, 4);
		}
		if (sceneNameString != "Bonetown" && sceneNameString != "Belltown")
		{
			this.playerData.mapperAway = (Random.Range(1, 100) > 50);
		}
		if (this.playerData.killedRoostingCrowman)
		{
			this.playerData.killedRoostingCrowman = false;
		}
		if (this.playerData.spinnerDefeated)
		{
			this.playerData.MapperAppearInBellhart = true;
		}
		if (this.playerData.defeatedCoralDrillers)
		{
			this.playerData.coralDrillerSoloReady = true;
		}
		if (this.playerData.dust01_battleCompleted)
		{
			this.playerData.dust01_returnReady = true;
		}
		if (sceneNameString != "Bonetown" && this.playerData.visitedBellhart)
		{
			this.playerData.BonebottomBellwayPilgrimLeft = true;
		}
		if (this.playerData.BoneBottomShopKeepWillLeave && !this.playerData.BoneBottomShopKeepLeft && sceneNameString != "Bonetown")
		{
			this.playerData.BoneBottomShopKeepLeft = true;
		}
		if (currentMapZoneEnum != MapZone.BONETOWN && currentMapZoneEnum != MapZone.PATH_OF_BONE && currentMapZoneEnum != MapZone.MOSS_CAVE)
		{
			if (this.playerData.seenPilbyLeft && !this.playerData.bonetownPilgrimRoundActive)
			{
				this.playerData.bonetownPilgrimRoundActive = true;
			}
			if (this.playerData.seenPebbLeft && !this.playerData.bonetownPilgrimHornedActive)
			{
				this.playerData.bonetownPilgrimHornedActive = true;
			}
		}
		if (this.playerData.hasChargeSlash && sceneNameString != "Room_Pinstress")
		{
			this.playerData.pinstressStoppedResting = true;
			if (Random.Range(0, 100) >= 50)
			{
				this.playerData.pinstressInsideSitting = true;
			}
			else
			{
				this.playerData.pinstressInsideSitting = false;
			}
			if (this.playerData.blackThreadWorld)
			{
				this.playerData.pinstressQuestReady = true;
			}
		}
		if (!this.playerData.IsPinGallerySetup && this.playerData.spinnerDefeated && sceneNameString != "Bone_12")
		{
			this.playerData.IsPinGallerySetup = true;
		}
		if (!this.playerData.PinGalleryLastChallengeOpen && this.playerData.pinGalleriesCompleted == 2 && this.playerData.visitedCitadel && sceneNameString != "Bone_12")
		{
			this.playerData.PinGalleryLastChallengeOpen = true;
		}
		if (this.playerData.bone03_openedTrapdoor && !QuestManager.GetQuest("Rock Rollers").IsAccepted)
		{
			this.playerData.bone03_openedTrapdoorForRockRoller = true;
		}
		if (this.playerData.rhinoChurchUnlocked)
		{
			this.playerData.rhinoRampageCompleted = true;
		}
		if (this.playerData.rhinoChurchUnlocked && !this.playerData.churchRhinoKilled && this.playerData.visitedCitadel && this.playerData.PilgrimsRestDoorBroken && currentMapZoneEnum != MapZone.WILDS && this.playerData.respawnScene != "Bone_East_10_Room")
		{
			this.playerData.rhinoRuckus = true;
			this.playerData.didRhinoRuckus = true;
		}
		if (sceneNameString != "Bone_East_10_Room")
		{
			this.playerData.pilgrimRestCrowd = Random.Range(1, 6);
		}
		if (sceneNameString != "Halfway_01" && this.playerData.MetHalfwayHunterFan)
		{
			if (Random.Range(1, 100) > 50)
			{
				this.playerData.nuuIsHome = true;
			}
			else
			{
				this.playerData.nuuIsHome = false;
			}
		}
		if (this.playerData.MetHalfwayHunterFan && this.playerData.defeatedSplinterQueen && currentMapZoneEnum != MapZone.SHELLWOOD_THICKET && !this.playerData.nuuVisiting_splinterQueen)
		{
			this.playerData.nuuVisiting_splinterQueen = true;
		}
		if (this.playerData.MetHalfwayHunterFan && this.playerData.defeatedCoralDrillers && currentMapZoneEnum != MapZone.JUDGE_STEPS && !this.playerData.nuuVisiting_coralDrillers)
		{
			this.playerData.nuuVisiting_coralDrillers = true;
		}
		if (this.playerData.MetHalfwayHunterFan && this.playerData.skullKingDefeated && currentMapZoneEnum != MapZone.PATH_OF_BONE && !this.playerData.nuuVisiting_skullKing)
		{
			this.playerData.nuuVisiting_skullKing = true;
		}
		if (this.playerData.MetHalfwayHunterFan && this.playerData.defeatedZapCoreEnemy && currentMapZoneEnum != MapZone.RED_CORAL_GORGE && !this.playerData.nuuVisiting_zapNest)
		{
			this.playerData.nuuVisiting_zapNest = true;
		}
		if (sceneNameString != "Halfway_01")
		{
			this.playerData.halfwayCrowd = Random.Range(1, 5);
			if (this.playerData.MetHalfwayBartender)
			{
				this.playerData.HalfwayPatronsCanVisit = true;
			}
			if (this.playerData.SeenHalfwayPatronLeft)
			{
				this.playerData.HalfwayPatronLeftGone = true;
			}
			if (this.playerData.SeenHalfwayPatronRight)
			{
				this.playerData.HalfwayPatronRightGone = true;
			}
		}
		if (!this.playerData.greymoor_05_centipedeArrives && (this.playerData.greymoor_04_battleCompleted || this.playerData.greymoor_10_entered))
		{
			this.playerData.greymoor_05_centipedeArrives = true;
		}
		if (this.playerData.garmondMoorwingConvo)
		{
			this.playerData.garmondMoorwingConvoReady = true;
		}
		if (this.playerData.metGarmond && this.playerData.vampireGnatDeaths > 1)
		{
			this.playerData.garmondMoorwingConvoReady = true;
		}
		if (this.playerData.openedDust05Gate)
		{
			this.playerData.garmondInDust05 = true;
		}
		if (this.playerData.dust05EnemyClearedOut)
		{
			this.playerData.dust05EnemyClearedOut = true;
		}
		if (this.playerData.bellShrineEnclave || this.playerData.defeatedCogworkDancers || this.playerData.hasHarpoonDash)
		{
			if (!this.playerData.garmondInSong01 && !this.playerData.garmondSeenInSong01 && this.playerData.enteredSong_01)
			{
				this.playerData.garmondInSong01 = true;
			}
			if (!this.playerData.garmondInSong02 && !this.playerData.garmondSeenInSong02 && this.playerData.enteredSong_02)
			{
				this.playerData.garmondInSong02 = true;
			}
			if (!this.playerData.garmondInSong13 && !this.playerData.garmondSeenInSong13 && this.playerData.enteredSong_13)
			{
				this.playerData.garmondInSong13 = true;
			}
			if (!this.playerData.garmondInSong17 && !this.playerData.garmondSeenInSong17 && this.playerData.enteredSong_17 && this.playerData.marionettesMet)
			{
				this.playerData.garmondInSong17 = true;
			}
		}
		if (!this.playerData.garmondInLibrary && this.playerData.libraryRoofShortcut && this.playerData.metGarmond)
		{
			this.playerData.garmondInLibrary = true;
		}
		if (this.playerData.HasMelodyArchitect && this.playerData.HasMelodyConductor && this.playerData.HasMelodyLibrarian && QuestManager.GetQuest("Fine Pins").IsCompleted && (this.playerData.metGarmond || (this.playerData.garmondSeenInSong13 && this.playerData.garmondSeenInSong17)))
		{
			this.playerData.garmondInEnclave = true;
		}
		if ((this.playerData.garmondEncounters_act3 >= 3 || this.playerData.HasWhiteFlower) && !this.playerData.garmondFinalQuestReady)
		{
			this.playerData.garmondFinalQuestReady = true;
		}
		this.playerData.pilgrimGroupBonegrave = Random.Range(1, 4);
		if (this.playerData.savedPlinney)
		{
			this.playerData.shellGravePopulated = true;
		}
		this.playerData.pilgrimGroupShellgrave = Random.Range(1, 4);
		this.playerData.pilgrimGroupGreymoorField = Random.Range(1, 4);
		this.playerData.enemyGroupAnt04 = Random.Range(1, 4);
		float num = (float)Random.Range(1, 100);
		if (num <= 60f)
		{
			this.playerData.halfwayCrowEnemyGroup = 2;
		}
		else if (num <= 90f)
		{
			this.playerData.halfwayCrowEnemyGroup = 1;
		}
		else
		{
			this.playerData.halfwayCrowEnemyGroup = 3;
		}
		if (this.playerData.bonegraveRosaryPilgrimDefeated)
		{
			this.playerData.bonegravePilgrimCrowdsCanReturn = true;
		}
		if (this.playerData.wokeSongChevalier && !this.playerData.songChevalierActiveInSong_25)
		{
			this.playerData.songChevalierActiveInSong_25 = true;
		}
		if (this.playerData.wokeSongChevalier && !this.playerData.songChevalierActiveInSong_27)
		{
			this.playerData.songChevalierActiveInSong_27 = true;
		}
		if (this.playerData.wokeSongChevalier && !this.playerData.songChevalierActiveInSong_04 && this.playerData.song_04_battleCompleted)
		{
			this.playerData.songChevalierActiveInSong_04 = true;
		}
		if (this.playerData.blackThreadWorld && this.playerData.wokeSongChevalier)
		{
			if (!this.playerData.songChevalierActiveInSong_02)
			{
				this.playerData.songChevalierActiveInSong_02 = true;
			}
			if (!this.playerData.songChevalierActiveInSong_24)
			{
				this.playerData.songChevalierActiveInSong_24 = true;
			}
			if (!this.playerData.songChevalierActiveInHang_02)
			{
				this.playerData.songChevalierActiveInHang_02 = true;
			}
			if (!this.playerData.songChevalierActiveInSong_07 && QuestManager.GetQuest("Save City Merchant").IsCompleted)
			{
				this.playerData.songChevalierActiveInSong_07 = true;
			}
		}
		if (this.playerData.songChevalierEncounterCooldown)
		{
			this.playerData.songChevalierEncounterCooldown = false;
		}
		if (this.playerData.songChevalierEncounters >= 2 && this.playerData.HasMelodyConductor && this.playerData.enclaveLevel >= 2 && !this.playerData.songChevalierQuestReady && !QuestManager.GetQuest("Song Knight").IsCompleted)
		{
			this.playerData.songChevalierQuestReady = true;
		}
		if (this.playerData.songChevalierEncounters >= 1 && this.playerData.HasWhiteFlower && !this.playerData.songChevalierQuestReady && !QuestManager.GetQuest("Song Knight").IsCompleted)
		{
			this.playerData.songChevalierQuestReady = true;
		}
		if (this.playerData.blackThreadWorld && QuestManager.GetQuest("Song Knight").IsCompleted)
		{
			this.playerData.songChevalierActiveInSong_25 = false;
			this.playerData.songChevalierSeenInSong_25 = false;
			this.playerData.songChevalierActiveInSong_27 = false;
			this.playerData.songChevalierSeenInSong_27 = false;
			this.playerData.songChevalierActiveInSong_02 = false;
			this.playerData.songChevalierSeenInSong_02 = false;
			this.playerData.songChevalierActiveInSong_24 = false;
			this.playerData.songChevalierSeenInSong_24 = false;
			this.playerData.songChevalierActiveInSong_07 = false;
			this.playerData.songChevalierSeenInSong_07 = false;
			this.playerData.songChevalierActiveInHang_02 = false;
			this.playerData.songChevalierSeenInHang_02 = false;
			switch (Random.Range(1, 7))
			{
			case 1:
				this.playerData.songChevalierActiveInSong_25 = true;
				break;
			case 2:
				this.playerData.songChevalierActiveInSong_27 = true;
				break;
			case 3:
				this.playerData.songChevalierActiveInSong_02 = true;
				break;
			case 4:
				this.playerData.songChevalierActiveInSong_24 = true;
				break;
			case 5:
				if (QuestManager.GetQuest("Save City Merchant").IsCompleted)
				{
					this.playerData.songChevalierActiveInSong_07 = true;
				}
				else
				{
					this.playerData.songChevalierActiveInSong_25 = true;
				}
				break;
			case 6:
				this.playerData.songChevalierActiveInHang_02 = true;
				break;
			}
		}
		if (!this.playerData.enclaveNPC_songKnightFan && this.playerData.blackThreadWorld && this.playerData.wokeSongChevalier && this.playerData.hasSuperJump && currentMapZoneEnum != MapZone.CITY_OF_SONG)
		{
			this.playerData.enclaveNPC_songKnightFan = true;
		}
		if (this.playerData.boneEastJailerKilled || this.playerData.CurseKilledFlyBoneEast)
		{
			this.playerData.boneEastJailerClearedOut = true;
		}
		if (this.playerData.enteredGreymoor05)
		{
			this.playerData.previouslyVisitedGreymoor_05 = true;
		}
		if (this.playerData.greymoor05_clearedOut)
		{
			this.playerData.greymoor05_clearedOut = false;
		}
		if (this.playerData.seenEmptyShellwood16)
		{
			this.playerData.slabFlyInShellwood16 = true;
		}
		if (this.playerData.completedLibraryEntryBattle)
		{
			this.playerData.scholarAmbushReady = true;
		}
		if (this.playerData.ant04_battleCompleted)
		{
			this.playerData.ant04_enemiesReturn = true;
		}
		if (this.playerData.dicePilgrimDefeated && this.playerData.dicePilgrimState == 0 && (this.playerData.defeatedCogworkDancers || this.playerData.hasHarpoonDash) && currentMapZoneEnum != MapZone.JUDGE_STEPS)
		{
			this.playerData.dicePilgrimState = 2;
		}
		else if (this.playerData.dicePilgrimState == 0 && (this.playerData.defeatedCogworkDancers || this.playerData.hasHarpoonDash) && currentMapZoneEnum != MapZone.JUDGE_STEPS)
		{
			this.playerData.dicePilgrimState = 1;
		}
		if (this.playerData.coral19_clearedOut)
		{
			this.playerData.coral19_clearedOut = false;
		}
		if (this.playerData.defeatedCoralDrillerSolo && currentMapZoneEnum != MapZone.RED_CORAL_GORGE && currentMapZoneEnum != MapZone.JUDGE_STEPS)
		{
			this.playerData.coralDrillerSoloEnemiesReturned = true;
		}
		if (this.playerData.defeatedWispPyreEffigy)
		{
			this.playerData.wisp02_enemiesReturned = true;
		}
		if (!this.playerData.HalfwayScarecrawAppeared && sceneNameString != "Halfway_01" && QuestManager.GetQuest("Crow Feathers").IsCompleted)
		{
			this.playerData.HalfwayScarecrawAppeared = true;
		}
		if (this.playerData.metGrubFarmer && !this.playerData.farmer_grewFirstGrub && sceneNameString != "Dust_11")
		{
			this.playerData.farmer_grubGrown_1 = true;
			this.playerData.farmer_grewFirstGrub = true;
		}
		if (this.playerData.grubFarmLevel >= 1 && this.playerData.farmer_grewFirstGrub && (!this.playerData.blackThreadWorld || this.playerData.silkFarmAbyssCoresCleared))
		{
			if (this.timeSinceLastTimePasses > 0f)
			{
				this.playerData.grubFarmerTimer += this.timeSinceLastTimePasses;
			}
			while (this.playerData.grubFarmerTimer >= 1800f)
			{
				this.playerData.grubFarmerTimer -= 1800f;
				if (!this.playerData.farmer_grubGrowing_1 && !this.playerData.farmer_grubGrown_1)
				{
					this.playerData.farmer_grubGrowing_1 = true;
				}
				else if (!this.playerData.farmer_grubGrown_1)
				{
					this.playerData.farmer_grubGrown_1 = true;
					this.playerData.farmer_grubGrowing_1 = false;
				}
				else
				{
					if (this.playerData.grubFarmLevel >= 2 && this.playerData.farmer_grubGrown_1)
					{
						if (!this.playerData.farmer_grubGrowing_2 && !this.playerData.farmer_grubGrown_2)
						{
							this.playerData.farmer_grubGrowing_2 = true;
							continue;
						}
						if (!this.playerData.farmer_grubGrown_2)
						{
							this.playerData.farmer_grubGrown_2 = true;
							this.playerData.farmer_grubGrowing_2 = false;
							continue;
						}
					}
					if (this.playerData.grubFarmLevel >= 3 && this.playerData.farmer_grubGrown_2)
					{
						if (!this.playerData.farmer_grubGrowing_3 && !this.playerData.farmer_grubGrown_3)
						{
							this.playerData.farmer_grubGrowing_3 = true;
						}
						else if (!this.playerData.farmer_grubGrown_3)
						{
							this.playerData.farmer_grubGrown_3 = true;
							this.playerData.farmer_grubGrowing_3 = false;
						}
					}
				}
			}
		}
		if (this.playerData.hitCrowCourtSwitch && !this.playerData.CrowCourtInSession && !this.playerData.defeatedCrowCourt && currentMapZoneEnum != MapZone.GREYMOOR && this.playerData.blackThreadWorld)
		{
			this.playerData.CrowCourtInSession = true;
		}
		if (this.playerData.defeatedCrowCourt && this.playerData.CrowCourtInSession && currentMapZoneEnum != MapZone.GREYMOOR)
		{
			this.playerData.CrowCourtInSession = false;
		}
		if (this.playerData.CrawbellInstalled)
		{
			if (this.timeSinceLastTimePasses > 0f)
			{
				this.playerData.CrawbellTimer += this.timeSinceLastTimePasses;
			}
			if (!(sceneNameString == "Belltown") && !(sceneNameString == "Belltown_Room_Spare") && !(sceneNameString == "Belltown_basement") && !(sceneNameString == "Belltown_Room_pinsmith") && !(sceneNameString == "Belltown_Room_Relic"))
			{
				this.playerData.CrawbellCrawsInside = (Random.Range(0, 2) == 0);
				ArrayForEnumAttribute.EnsureArraySize<int>(ref this.playerData.CrawbellCurrency, typeof(CurrencyType));
				ArrayForEnumAttribute.EnsureArraySize<int>(ref this.playerData.CrawbellCurrencyCaps, typeof(CurrencyType));
				for (int i = 0; i < this.playerData.CrawbellCurrencyCaps.Length; i++)
				{
					if (this.playerData.CrawbellCurrencyCaps[i] <= 0)
					{
						this.playerData.CrawbellCurrencyCaps[i] = Random.Range(300, 500);
					}
				}
				while (this.playerData.CrawbellTimer >= 300f)
				{
					this.playerData.CrawbellTimer -= 300f;
					for (int j = 0; j < this.playerData.CrawbellCurrency.Length; j++)
					{
						int num2 = this.playerData.CrawbellCurrency[j];
						if (num2 < this.playerData.CrawbellCurrencyCaps[j])
						{
							this.playerData.CrawbellCurrency[j] = num2 + Random.Range(5, 20);
						}
					}
				}
			}
		}
		if (this.playerData.Collectables.GetData("Growstone").Amount > 0)
		{
			if (this.timeSinceLastTimePasses > 0f)
			{
				this.playerData.GrowstoneTimer += this.timeSinceLastTimePasses;
			}
			while (this.playerData.GrowstoneTimer >= 1200f)
			{
				if (this.playerData.GrowstoneState >= 3)
				{
					this.playerData.GrowstoneTimer = 0f;
					break;
				}
				this.playerData.GrowstoneTimer -= 1200f;
				this.playerData.GrowstoneState++;
			}
		}
		if (this.playerData.blackThreadWorld && !this.playerData.HuntressRuntAppeared && !Gameplay.HuntressQuest.IsCompleted)
		{
			this.playerData.HuntressRuntAppeared = true;
		}
		if (this.playerData.defeatedSplinterQueen && !this.playerData.splinterQueenSproutCut && currentMapZoneEnum != MapZone.SHELLWOOD_THICKET && this.playerData.splinterQueenSproutTimer < 50)
		{
			this.playerData.splinterQueenSproutTimer++;
		}
		if (this.playerData.roofCrabDefeated && this.playerData.citadelWoken && currentMapZoneEnum != MapZone.CRAWLSPACE && currentMapZoneEnum != MapZone.BONETOWN && currentMapZoneEnum != MapZone.PATH_OF_BONE && currentMapZoneEnum != MapZone.MOSSTOWN)
		{
			this.playerData.littleCrabsAppeared = true;
		}
		if ((this.playerData.BonebottomBellwayPilgrimScared || this.playerData.skullKingInvaded) && !this.playerData.BonebottomBellwayPilgrimLeft && (!this.playerData.skullKingInvaded || this.playerData.skullKingKilled) && currentMapZoneEnum != MapZone.BONETOWN && currentMapZoneEnum != MapZone.PATH_OF_BONE)
		{
			this.playerData.BonebottomBellwayPilgrimLeft = true;
		}
		if (this.playerData.spinnerDefeated)
		{
			this.playerData.BonebottomBellwayPilgrimLeft = true;
		}
		if (this.playerData.skullKingDefeated && !this.playerData.skullKingWillInvade && currentMapZoneEnum != MapZone.BONETOWN && currentMapZoneEnum != MapZone.PATH_OF_BONE && currentMapZoneEnum != MapZone.MOSS_CAVE && !this.playerData.blackThreadWorld && (this.playerData.visitedCitadel || this.playerData.visitedCoral || this.playerData.visitedDustpens) && Random.Range(1, 100) <= 20)
		{
			this.playerData.skullKingWillInvade = true;
		}
		if (this.playerData.skullKingKilled && !this.playerData.skullKingBenchMended)
		{
			this.playerData.skullKingBenchMended = true;
		}
		if (this.playerData.skullKingBenchMended && this.playerData.pilbyKilled && !this.playerData.boneBottomFuneral && !this.playerData.blackThreadWorld && currentMapZoneEnum != MapZone.BONETOWN && currentMapZoneEnum != MapZone.PATH_OF_BONE && currentMapZoneEnum != MapZone.MOSS_CAVE)
		{
			this.playerData.boneBottomFuneral = true;
			this.playerData.skullKingPlatMended = true;
		}
		if ((this.playerData.UnlockedMelodyLift || this.playerData.hasDoubleJump) && !this.playerData.pilbyKilled && this.playerData.pilbyMeetConvo && !this.playerData.pilbyAtPilgrimsRest && !this.playerData.rhinoRuckus)
		{
			this.playerData.pilbyAtPilgrimsRest = true;
			if (this.playerData.PilgrimsRestDoorBroken)
			{
				this.playerData.pilbyInsidePilgrimsRest = true;
			}
		}
		if (this.playerData.pilbySeenAtPilgrimsRest && !this.playerData.pilbyLeftPilgrimsRest && currentMapZoneEnum != MapZone.WILDS)
		{
			this.playerData.pilbyLeftPilgrimsRest = true;
		}
		if (currentMapZoneEnum != MapZone.BONETOWN && currentMapZoneEnum != MapZone.PATH_OF_BONE && currentMapZoneEnum != MapZone.MOSS_CAVE && this.playerData.visitedBoneForest)
		{
			this.playerData.bonetownCrowd = Random.Range(1, 8);
		}
		if (sceneNameString != "Bellway_01" && sceneNameString != "Bonetown")
		{
			if (QuestManager.GetQuest("Building Materials (Statue)").IsCompleted)
			{
				this.playerData.fixerStatueConstructed = true;
			}
			if (QuestManager.GetQuest("Building Materials (Bridge)").IsCompleted)
			{
				this.playerData.fixerBridgeConstructed = true;
			}
			if (QuestManager.GetQuest("Pilgrim Rags").IsCompleted)
			{
				this.playerData.boneBottomAddition_RagLine = true;
			}
		}
		PersistentItemData<int> persistentItemData;
		if (!this.playerData.ChurchKeeperLeftBasement && this.sceneName != "Tut_03" && this.sceneName != "Bonetown" && this.sceneData.PersistentInts.TryGetValue("Tut_03", "Churchkeeper Basement", out persistentItemData) && persistentItemData.Value > 0)
		{
			this.playerData.ChurchKeeperLeftBasement = true;
		}
		if (this.playerData.CaravanLechSaved && !this.playerData.CaravanLechReturnedToCaravan && this.sceneName != "Bone_10" && this.sceneName != "Greymoor_08" && this.sceneName != "Coral_Judge_Arena" && this.sceneName != "Aqueduct_05")
		{
			this.playerData.CaravanLechReturnedToCaravan = true;
		}
		if ((this.playerData.encounteredVampireGnat_05 || this.playerData.encounteredVampireGnatBoss) && currentMapZoneEnum != MapZone.GREYMOOR)
		{
			this.playerData.allowVampireGnatInAltLoc = true;
		}
		if (!this.playerData.VampireGnatCorpseOnCaravan && this.playerData.VampireGnatDefeatedBeforeCaravanArrived && !this.playerData.VampireGnatCorpseInWater && this.playerData.CaravanTroupeLocation > CaravanTroupeLocations.Bone && this.sceneName != "Greymoor_08")
		{
			this.playerData.VampireGnatCorpseOnCaravan = true;
		}
		if (this.playerData.CaravanTroupeLocation > CaravanTroupeLocations.Bone && !this.playerData.creaturesReturnedToBone10 && this.playerData.respawnScene != "Bone_10")
		{
			this.playerData.creaturesReturnedToBone10 = true;
		}
		CaravanTroupeLocations caravanTroupeLocation = this.playerData.CaravanTroupeLocation;
		if (caravanTroupeLocation != CaravanTroupeLocations.Greymoor)
		{
			if (caravanTroupeLocation == CaravanTroupeLocations.CoralJudge)
			{
				if (this.playerData.MetCaravanTroupeLeaderJudge && !this.playerData.CaravanTroupeLeaderCanLeaveJudge && this.sceneName != "Coral_Judge_Arena")
				{
					this.playerData.CaravanTroupeLeaderCanLeaveJudge = true;
				}
			}
		}
		else if (this.playerData.MetCaravanTroupeLeaderGreymoor && !this.playerData.CaravanTroupeLeaderCanLeaveGreymoor && this.sceneName != "Greymoor_08")
		{
			this.playerData.CaravanTroupeLeaderCanLeaveGreymoor = true;
		}
		if (!this.playerData.SpinnerDefeatedTimePassed && this.sceneName != "Belltown" && this.sceneName != "Belltown_Boss")
		{
			this.playerData.SpinnerDefeatedTimePassed = true;
		}
		if (!this.playerData.antMerchantKilled && currentMapZoneEnum != MapZone.HUNTERS_NEST && (this.playerData.defeatedCogworkDancers || this.playerData.bellShrineEnclave || this.playerData.hasHarpoonDash))
		{
			this.playerData.antMerchantKilled = true;
			if (this.playerData.ant21_InitBattleCompleted && !Gameplay.CurveclawTool.IsUnlocked && !Gameplay.CurveclawUpgradedTool.IsUnlocked)
			{
				this.playerData.ant21_ExtraBattleAdded = true;
			}
		}
		if (this.playerData.MottledChildGivenTool && !this.playerData.MottledChildNewTool)
		{
			this.playerData.MottledChildNewTool = true;
		}
		if (this.playerData.bellShrineBellhart && currentMapZoneEnum != MapZone.BELLTOWN && currentMapZoneEnum != MapZone.MEMORY)
		{
			this.playerData.belltownCrowdsReady = true;
			this.playerData.belltownCrowd = Random.Range(1, 7);
		}
		if (this.playerData.visitedBellhartSaved && currentMapZoneEnum != MapZone.BELLTOWN)
		{
			this.playerData.shermaInBellhart = true;
		}
		if (currentMapZoneEnum != MapZone.BELLTOWN)
		{
			BelltownHouseStates belltownHouseState = this.playerData.BelltownHouseState;
			if (belltownHouseState != BelltownHouseStates.None)
			{
				if (belltownHouseState == BelltownHouseStates.Half)
				{
					if (QuestManager.GetQuest("Belltown House Mid").IsCompleted)
					{
						this.playerData.BelltownHouseState = BelltownHouseStates.Full;
					}
				}
			}
			else if (QuestManager.GetQuest("Belltown House Start").IsCompleted)
			{
				this.playerData.BelltownHouseState = BelltownHouseStates.Half;
			}
			if (this.playerData.BelltownGreeterConvo > 0)
			{
				this.playerData.BelltownGreeterMetTimePassed = true;
			}
		}
		if (this.playerData.gotPastDockSpearThrower)
		{
			this.playerData.gotPastDockSpearThrower = false;
		}
		if (this.playerData.wardBossDefeated && currentMapZoneEnum != MapZone.WARD && currentMapZoneEnum != MapZone.MEMORY && this.playerData.respawnScene != "Ward_02")
		{
			this.playerData.wardWoken = true;
		}
		PersistentItemData<bool> persistentItemData2;
		if (this.sceneData.PersistentBools.TryGetValue("Dock_10", "dock_pressure_plate_lock", out persistentItemData2) && persistentItemData2.Value && currentMapZoneEnum != MapZone.DOCKS && this.playerData.respawnScene != "Dock_10" && !this.playerData.blackThreadWorld && !this.playerData.BallowInSauna)
		{
			this.playerData.BallowInSauna = true;
		}
		if (this.playerData.BallowSeenInSauna && !this.playerData.BallowLeftSauna && sceneNameString != "Dock_10")
		{
			this.playerData.BallowLeftSauna = true;
		}
		if (this.playerData.defeatedRoachkeeperChef && currentMapZoneEnum != MapZone.DUSTPENS)
		{
			this.playerData.roachkeeperChefCorpsePrepared = true;
		}
		if (sceneNameString != "Song_Enclave" && sceneNameString != "Bellshrine_Enclave")
		{
			switch (this.playerData.enclaveLevel)
			{
			case 0:
				if (this.playerData.metCaretaker)
				{
					this.playerData.enclaveLevel = 1;
				}
				break;
			case 1:
				if (QuestManager.GetQuest("Songclave Donation 1").IsCompleted && Gameplay.EnclaveQuestBoard.CompletedQuestCount >= 2)
				{
					this.playerData.enclaveLevel = 2;
				}
				break;
			case 2:
				if (QuestManager.GetQuest("Songclave Donation 2").IsCompleted && Gameplay.EnclaveQuestBoard.CompletedQuestCount >= 5)
				{
					this.playerData.enclaveLevel = 3;
				}
				break;
			}
			if (QuestManager.GetQuest("Fine Pins").IsCompleted)
			{
				this.playerData.enclaveAddition_PinRack = true;
			}
			if (QuestManager.GetQuest("Song Pilgrim Cloaks").IsCompleted)
			{
				this.playerData.enclaveAddition_CloakLine = true;
			}
			if (Gameplay.EnclaveQuestBoard.CompletedQuestCount >= 4)
			{
				this.playerData.enclaveDonation2_Available = true;
			}
		}
		if (!this.playerData.citadelHalfwayComplete && (this.playerData.defeatedCogworkDancers || this.playerData.hasHarpoonDash || this.playerData.visitedEnclave))
		{
			this.playerData.citadelHalfwayComplete = true;
		}
		if (this.playerData.citadelWoken && currentMapZoneEnum != MapZone.WARD && currentMapZoneEnum != MapZone.CITY_OF_SONG)
		{
			this.playerData.song05MarchGroupReady = true;
		}
		if (this.playerData.under07_battleCompleted && this.playerData.hasHarpoonDash && currentMapZoneEnum != MapZone.UNDERSTORE && currentMapZoneEnum != MapZone.CITY_OF_SONG)
		{
			this.playerData.under07_heavyWorkerReturned = true;
		}
		if (this.playerData.grindleInSong_08)
		{
			this.playerData.grindleInSong_08 = false;
		}
		if (this.playerData.seenGrindleInSong_08 && currentMapZoneEnum != MapZone.CITY_OF_SONG)
		{
			this.playerData.savedGrindleInCitadel = true;
			this.playerData.grindleInSong_08 = false;
		}
		if (this.playerData.savedGrindleInCitadel && this.playerData.grindleChestLocation == 0)
		{
			this.playerData.grindleChestLocation = Random.Range(1, 4);
		}
		if (this.playerData.hang04Battle)
		{
			this.playerData.leftTheGrandForum = true;
		}
		if (this.playerData.song_17_clearedOut)
		{
			this.playerData.song_17_clearedOut = false;
		}
		PersistentItemData<bool> persistentItemData3;
		if (this.sceneData.PersistentBools.TryGetValue("Under_07c", "Bot Blocker", out persistentItemData3) && persistentItemData3.Value && currentMapZoneEnum != MapZone.UNDERSTORE && currentMapZoneEnum != MapZone.CITY_OF_SONG && currentMapZoneEnum != MapZone.COG_CORE)
		{
			this.playerData.rosaryThievesInUnder07 = true;
		}
		if (this.playerData.bankOpened && currentMapZoneEnum != MapZone.HANG && currentMapZoneEnum != MapZone.CITY_OF_SONG && currentMapZoneEnum != MapZone.COG_CORE)
		{
			this.playerData.rosaryThievesInBank = true;
		}
		if (this.playerData.cog7_automaton_defeated)
		{
			this.playerData.cog7_automatonRepairing = true;
		}
		if (this.playerData.cog7_automatonRepairingComplete)
		{
			this.playerData.cog7_automaton_defeated = false;
			this.playerData.cog7_automatonRepairing = false;
			this.playerData.cog7_automatonRepairingComplete = false;
		}
		if (!this.playerData.cityMerchantCanLeaveForBridge && this.playerData.enclaveLevel > 2 && this.playerData.hasDoubleJump && this.playerData.MetCityMerchantEnclave)
		{
			this.playerData.cityMerchantCanLeaveForBridge = true;
		}
		if (this.playerData.MetCityMerchantEnclave)
		{
			this.playerData.cityMerchantInLibrary03 = true;
		}
		if (this.playerData.cityMerchantInGrandForumSeen)
		{
			this.playerData.cityMerchantInGrandForumLeft = true;
		}
		if (this.playerData.cityMerchantInLibrary03Seen)
		{
			this.playerData.cityMerchantInLibrary03Left = true;
		}
		if (this.playerData.cityMerchantRecentlySeenInEnclave && currentMapZoneEnum != MapZone.LIBRARY && currentMapZoneEnum != MapZone.CITY_OF_SONG && currentMapZoneEnum != MapZone.HANG)
		{
			this.playerData.cityMerchantRecentlySeenInEnclave = false;
		}
		if (this.playerData.ArchitectWillLeave && this.sceneName != "Under_17" && !this.playerData.ArchitectLeft)
		{
			this.playerData.ArchitectLeft = true;
		}
		if (!this.playerData.trobbioCleanedUp && this.playerData.defeatedTrobbio && currentMapZoneEnum != MapZone.LIBRARY && currentMapZoneEnum != MapZone.CITY_OF_SONG && currentMapZoneEnum != MapZone.UNDERSTORE)
		{
			this.playerData.trobbioCleanedUp = true;
		}
		if (this.playerData.visitedCradle && currentMapZoneEnum != MapZone.CITY_OF_SONG && currentMapZoneEnum != MapZone.MEMORY && currentMapZoneEnum != MapZone.CRADLE)
		{
			this.playerData.laceCorpseAddedEffects = true;
		}
		if (QuestManager.GetQuest("Broodmother Hunt").IsCompleted && !this.playerData.tinyBroodMotherAppeared && currentMapZoneEnum != MapZone.THE_SLAB)
		{
			this.playerData.tinyBroodMotherAppeared = true;
		}
		if (!this.playerData.BlueScientistDead && currentMapZoneEnum != MapZone.CRAWLSPACE && QuestManager.GetQuest("Extractor Blue").IsCompleted && QuestManager.GetQuest("Extractor Blue Worms").IsCompleted)
		{
			this.playerData.BlueScientistDead = true;
		}
		if (!this.playerData.BlueScientistSceneryPustulesGrown && currentMapZoneEnum != MapZone.CRAWLSPACE && QuestManager.GetQuest("Extractor Blue").IsCompleted)
		{
			this.playerData.BlueScientistSceneryPustulesGrown = true;
		}
		if (this.playerData.UnlockedDustCage)
		{
			if (this.playerData.GreenPrinceLocation == GreenPrinceLocations.DustCage && this.playerData.song_04_battleCompleted)
			{
				this.playerData.GreenPrinceLocation = GreenPrinceLocations.Song04;
			}
			else if (this.playerData.GreenPrinceLocation < GreenPrinceLocations.CogDancers && this.playerData.defeatedCogworkDancers && this.playerData.bellShrineEnclave && (this.playerData.GreenPrinceSeenSong04 || this.playerData.blackThreadWorld))
			{
				this.playerData.GreenPrinceLocation = GreenPrinceLocations.CogDancers;
			}
		}
		if (!this.playerData.ShakraFinalQuestAppear && sceneNameString != "Belltown")
		{
			ShopItemList mapperStock = Gameplay.MapperStock;
			bool flag = false;
			foreach (ShopItem shopItem in mapperStock.ShopItems)
			{
				if ((shopItem.GetTypeFlags() & ShopItem.TypeFlags.Map) != ShopItem.TypeFlags.None && !shopItem.IsPurchased)
				{
					flag = true;
					break;
				}
			}
			int num3 = 0;
			if (this.playerData.HasMelodyArchitect)
			{
				num3++;
			}
			if (this.playerData.HasMelodyConductor)
			{
				num3++;
			}
			if (this.playerData.HasMelodyLibrarian)
			{
				num3++;
			}
			bool flag2 = false;
			if (num3 >= 2)
			{
				flag2 = true;
			}
			if (!flag && (this.playerData.DefeatedSwampShaman || flag2))
			{
				this.playerData.ShakraFinalQuestAppear = true;
				this.playerData.MapperLeaveAll();
			}
		}
		if (!this.playerData.swampMuckmanTallInvades && this.playerData.DefeatedSwampShaman && currentMapZoneEnum != MapZone.SWAMP && currentMapZoneEnum != MapZone.AQUEDUCT)
		{
			this.playerData.swampMuckmanTallInvades = true;
		}
		if (this.playerData.blackThreadWorld && this.playerData.seenMapperAct3 && currentMapZoneEnum != MapZone.BELLTOWN)
		{
			this.playerData.mapperLocationAct3 = Random.Range(1, 6);
			if (this.playerData.mapperLocationAct3 == 4 || this.playerData.mapperLocationAct3 == 5)
			{
				this.playerData.mapperLocationAct3 = 2;
			}
			if (Random.Range(1, 100) <= 50)
			{
				this.playerData.mapperIsFightingAct3 = true;
			}
			else
			{
				this.playerData.mapperIsFightingAct3 = false;
			}
			this.playerData.mapperFightGroup = Random.Range(1, 3);
		}
		if (!this.playerData.thievesReturnedToShadow28 && this.playerData.SavedFlea_Shadow_28)
		{
			this.playerData.thievesReturnedToShadow28 = true;
		}
		if (!this.playerData.FleaGamesCanStart && this.playerData.blackThreadWorld && this.playerData.CaravanTroupeLocation == CaravanTroupeLocations.Aqueduct && Gameplay.FleaCharmTool.IsUnlocked && currentMapZoneEnum != MapZone.AQUEDUCT && this.playerData.respawnScene != "Aqueduct_05")
		{
			this.playerData.FleaGamesCanStart = true;
		}
		if (this.playerData.SethNpcLocation == SethNpcLocations.Absent && this.playerData.HasWhiteFlower)
		{
			this.playerData.SethNpcLocation = SethNpcLocations.Greymoor;
		}
		if (!this.playerData.SethJoinedFleatopia && this.playerData.FleaGamesEnded && (this.playerData.SethNpcLocation == SethNpcLocations.Fleatopia || (this.playerData.MetSethNPC & this.playerData.HasWhiteFlower)) && sceneNameString != "Aqueduct_05")
		{
			this.playerData.SethNpcLocation = SethNpcLocations.Fleatopia;
			this.playerData.SethJoinedFleatopia = true;
		}
		if (this.playerData.gillyQueueMovingOn && this.playerData.blackThreadWorld)
		{
			this.playerData.gillyLocationAct3++;
			this.playerData.gillyQueueMovingOn = false;
		}
		if (this.playerData.gillyLocationAct3 > 0 && this.playerData.Collectables.GetData("Hunter Heart").Amount > 0 && currentMapZoneEnum != MapZone.HUNTERS_NEST && currentMapZoneEnum != MapZone.WILDS)
		{
			this.playerData.gillyLocationAct3 = 3;
		}
		if (this.GetCurrentMapZone() != this.lastTimePassesMapZone)
		{
			this.TimePassesElsewhere();
		}
		this.lastTimePassesMapZone = this.GetCurrentMapZone();
		this.timeSinceLastTimePasses = 0f;
	}

	// Token: 0x06002400 RID: 9216 RVA: 0x000A602C File Offset: 0x000A422C
	public void TimePassesElsewhere()
	{
		string sceneNameString = this.GetSceneNameString();
		if (this.playerData.muchTimePassed)
		{
			this.sceneData.MimicShuffle();
		}
		if (this.playerData.hang04Battle && this.playerData.MetCityMerchantEnclave)
		{
			this.playerData.cityMerchantInGrandForum = true;
		}
		if (this.playerData.scholarAcolytesReleased)
		{
			this.playerData.scholarAcolytesInLibrary_02 = true;
		}
		if (this.playerData.completedLavaChallenge)
		{
			this.playerData.lavaSpittersEmerge = true;
		}
		if (this.playerData.garmondEncounterCooldown)
		{
			this.playerData.garmondEncounterCooldown = false;
		}
		if (!(sceneNameString == "Belltown") && !(sceneNameString == "Belltown_Room_Spare"))
		{
			if (this.playerData.mapperBellhartConvo)
			{
				this.playerData.mapperBellhartConvoTimePassed = true;
			}
			if (!this.playerData.mapperSellingTubePins && this.playerData.mapperTubeConvo)
			{
				this.playerData.mapperSellingTubePins = true;
			}
			if (this.playerData.mapperConvo_Act3Intro)
			{
				this.playerData.mapperConvo_Act3IntroTimePassed = true;
			}
			if (!this.playerData.BelltownFurnishingSpaAvailable)
			{
				int num = 0;
				if (this.playerData.BelltownHouseColour != BellhomePaintColours.None)
				{
					num++;
				}
				if (this.playerData.BelltownFurnishingDesk)
				{
					num++;
				}
				if (this.playerData.BelltownFurnishingFairyLights)
				{
					num++;
				}
				if (this.playerData.BelltownFurnishingGramaphone)
				{
					num++;
				}
				if (num >= 2)
				{
					this.playerData.BelltownFurnishingSpaAvailable = true;
				}
			}
			if (this.playerData.BelltownHouseColour != BellhomePaintColours.None)
			{
				this.playerData.BelltownHousePaintComplete = true;
			}
		}
		if (this.playerData.BelltownHermitConvoCooldown)
		{
			this.playerData.BelltownHermitConvoCooldown = false;
		}
		if (this.playerData.HasMelodyConductor)
		{
			this.playerData.ConductorWeaverDlgQueued = true;
		}
		if (this.playerData.bonetownPilgrimHornedSeen && this.playerData.bonetownPilgrimHornedCount > 0)
		{
			this.playerData.bonetownPilgrimHornedCount--;
		}
		if (this.playerData.bonetownPilgrimRoundSeen && this.playerData.bonetownPilgrimRoundCount > 0)
		{
			this.playerData.bonetownPilgrimRoundCount--;
		}
		if (!this.playerData.ArchitectMelodyReturnSeen && this.playerData.ArchitectMentionedMelody)
		{
			this.playerData.ArchitectMelodyReturnQueued = true;
		}
		if (!this.playerData.MaskMakerTalkedUnmasked2 && this.playerData.MaskMakerTalkedUnmasked1 && !this.playerData.MaskMakerQueuedUnmasked2 && this.playerData.UnlockedMelodyLift)
		{
			this.playerData.MaskMakerQueuedUnmasked2 = true;
		}
		if (this.playerData.BonebottomBellwayPilgrimState == 1)
		{
			this.playerData.BonebottomBellwayPilgrimState = 2;
		}
		if (!(sceneNameString == "Song_Enclave"))
		{
			GameManager.<TimePassesElsewhere>g__CheckReadyToLeave|249_0(ref this.playerData.EnclaveStatePilgrimSmall);
			GameManager.<TimePassesElsewhere>g__CheckReadyToLeave|249_0(ref this.playerData.EnclaveStateNPCShortHorned);
			GameManager.<TimePassesElsewhere>g__CheckReadyToLeave|249_0(ref this.playerData.EnclaveStateNPCTall);
			GameManager.<TimePassesElsewhere>g__CheckReadyToLeave|249_0(ref this.playerData.EnclaveStateNPCStandard);
			GameManager.<TimePassesElsewhere>g__CheckReadyToLeave|249_0(ref this.playerData.EnclaveState_songKnightFan);
		}
		if (this.playerData.metShermaEnclave && this.playerData.visitedWard && !this.playerData.shermaQuestActive)
		{
			this.playerData.shermaQuestActive = true;
		}
		if (this.playerData.gillyQueueMovingOn)
		{
			this.playerData.gillyLocation++;
			this.playerData.gillyQueueMovingOn = false;
		}
		if (!this.playerData.SprintMasterExtraRaceAvailable && QuestManager.GetQuest("Sprintmaster Race").IsCompleted && this.playerData.HasWhiteFlower && !(sceneNameString == "Sprintmaster_Cave"))
		{
			this.playerData.SprintMasterExtraRaceAvailable = true;
		}
		if (this.playerData.defeatedTormentedTrobbio)
		{
			this.playerData.tormentedTrobbioLurking = true;
		}
		if (!this.playerData.CrestPreUpgradeTalked && this.playerData.MetCrestUpgrader)
		{
			this.playerData.CrestPurposeQueued = true;
		}
	}

	// Token: 0x06002401 RID: 9217 RVA: 0x000A640C File Offset: 0x000A460C
	public void TimePassesLoadedIn()
	{
		string respawnScene = this.playerData.respawnScene;
		if ((respawnScene == "Belltown" || respawnScene == "Belltown_Room_Spare") && this.playerData.BelltownHouseColour != BellhomePaintColours.None)
		{
			this.playerData.BelltownHousePaintComplete = true;
		}
	}

	// Token: 0x06002402 RID: 9218 RVA: 0x000A6458 File Offset: 0x000A4658
	public void MuchTimePasses()
	{
		if (!this.playerData.muchTimePassed)
		{
			this.playerData.muchTimePassed = true;
		}
	}

	// Token: 0x06002403 RID: 9219 RVA: 0x000A6473 File Offset: 0x000A4673
	public void StartBlackThreadWorld()
	{
		this.playerData.blackThreadWorld = true;
		this.playerData.act3_wokeUp = false;
	}

	// Token: 0x06002404 RID: 9220 RVA: 0x000A6490 File Offset: 0x000A4690
	private void StartAct3()
	{
		this.playerData.respawnScene = "Song_Tower_Destroyed";
		this.playerData.respawnMarkerName = "Death Respawn Marker Init";
		this.playerData.respawnType = 0;
		this.playerData.mapZone = MapZone.CRADLE;
		this.playerData.extraRestZone = ExtraRestZones.None;
		QuestManager.GetQuest("Silk Defeat Snare").TryEndQuest(null, false, true, false);
		this.playerData.bindCutscenePlayed = false;
		if (!this.playerData.churchRhinoKilled && !this.playerData.rhinoChurchUnlocked)
		{
			this.playerData.churchRhinoBlackThreadCorpse = true;
		}
		if (!Gameplay.WeightedAnkletTool.IsUnlocked)
		{
			this.playerData.mortKeptWeightedAnklet = true;
		}
		DeliveryQuestItem.BreakAllNoEffects();
		ToolCrest crest;
		if (Gameplay.HunterCrest3.IsUnlocked)
		{
			crest = Gameplay.HunterCrest3;
		}
		else if (Gameplay.HunterCrest2.IsUnlocked)
		{
			crest = Gameplay.HunterCrest2;
		}
		else
		{
			crest = Gameplay.HunterCrest;
		}
		ToolItemManager.AutoEquip(crest, false);
		this.playerData.ExtraToolEquips = new FloatingCrestSlotsData();
		ToolItemManager.AutoEquip(Gameplay.DefaultSkillTool);
		if (this.playerData.fixerBridgeConstructed)
		{
			this.playerData.fixerBridgeBroken = true;
		}
		this.playerData.scholarAcolytesReleased = true;
		this.playerData.spinnerDefeated = true;
		this.playerData.marionettesBurned = true;
		EnemyJournalRecord record = EnemyJournalManager.GetRecord("Song Handmaiden");
		EnemyJournalKillData.KillData killData = this.playerData.EnemyJournalKillData.GetKillData(record.name);
		if (killData.Kills < record.KillsRequired)
		{
			killData.Kills = record.KillsRequired;
			this.playerData.EnemyJournalKillData.RecordKillData(record.name, killData);
		}
		this.playerData.encounteredSongGolem = true;
		this.playerData.defeatedSongGolem = true;
		EnemyJournalRecord record2 = EnemyJournalManager.GetRecord("Song Golem");
		EnemyJournalKillData.KillData killData2 = this.playerData.EnemyJournalKillData.GetKillData(record2.name);
		if (killData2.Kills < record2.KillsRequired)
		{
			killData2.Kills = record2.KillsRequired;
			this.playerData.EnemyJournalKillData.RecordKillData(record2.name, killData2);
		}
		this.playerData.defeatedTrobbio = true;
		this.playerData.encounteredTrobbio = true;
		if (!this.playerData.PinsmithMetBelltown)
		{
			this.playerData.savedPlinney = true;
		}
		this.playerData.SeenLastJudgeGateOpen = true;
		QuestManager.GetQuest("Grand Gate Bellshrines").TryEndQuest(null, false, true, false);
		if (this.playerData.defeatedSplinterQueen && !this.playerData.splinterQueenSproutCut)
		{
			this.playerData.splinterQueenSproutGrewLarge = true;
		}
		this.playerData.garmondInEnclave = true;
		if (this.playerData.hitCrowCourtSwitch)
		{
			this.playerData.CrowCourtInSession = true;
		}
		GameManager.RemoveIncompleteActiveQuest(this.playerData, "Pilgrim Rags", "Pilgrim Rag");
		GameManager.RemoveIncompleteActiveQuest(this.playerData, "Skull King", "Skull King Fragment");
		GameManager.RemoveIncompleteActiveQuest(this.playerData, "Roach Killing", "Roach Corpse Item");
		GameManager.RemoveIncompleteActiveQuest(this.playerData, "Rock Rollers", "Rock Roller Item");
		this.playerData.grubFarmerTimer = 0f;
		this.playerData.farmer_grubGrowing_1 = false;
		this.playerData.farmer_grubGrown_1 = false;
		this.playerData.farmer_grubGrowing_2 = false;
		this.playerData.farmer_grubGrown_2 = false;
		this.playerData.farmer_grubGrowing_3 = false;
		this.playerData.farmer_grubGrown_3 = false;
		QuestManager.IncrementVersion();
		CollectableItemManager.IncrementVersion();
		if (!this.playerData.UnlockedEnclaveTube)
		{
			this.sceneData.PersistentBools.SetValue(new PersistentItemData<bool>
			{
				SceneName = "Song_Enclave_Tube",
				ID = "One Way Wall",
				Value = true
			});
		}
		this.playerData.mapperSellingTubePins = true;
	}

	// Token: 0x06002405 RID: 9221 RVA: 0x000A6834 File Offset: 0x000A4A34
	private static void RemoveIncompleteActiveQuest(PlayerData playerData, string questName, string itemName)
	{
		QuestCompletionData.Completion data = playerData.QuestCompletionData.GetData(questName);
		if (data.IsAccepted && !data.IsCompleted)
		{
			playerData.QuestCompletionData.SetData(questName, default(QuestCompletionData.Completion));
		}
		CollectableItemsData.Data data2 = playerData.Collectables.GetData(itemName);
		if (data2.Amount > 0)
		{
			MateriumItemsData.Data data3 = playerData.MateriumCollected.GetData(itemName);
			if (!data3.IsCollected)
			{
				data3.IsCollected = true;
				playerData.MateriumCollected.SetData(itemName, data3);
			}
			data2.Amount = 0;
			playerData.Collectables.SetData(itemName, data2);
		}
	}

	// Token: 0x06002406 RID: 9222 RVA: 0x000A68C8 File Offset: 0x000A4AC8
	public void SethTravelCheck()
	{
		if (this.playerData.SethNpcLocation == SethNpcLocations.Absent && this.playerData.defeatedFlowerQueen)
		{
			this.playerData.SethNpcLocation = SethNpcLocations.Greymoor;
		}
	}

	// Token: 0x06002407 RID: 9223 RVA: 0x000A68F0 File Offset: 0x000A4AF0
	private void EnteredNewMapZone(MapZone previousMapZone, MapZone currentMapZone, bool forcedNotMemory)
	{
		string location = currentMapZone.ToString();
		if (this.playerData.hang04Battle)
		{
			this.playerData.leftTheGrandForum = true;
		}
		Platform.Current.UpdateLocation(location);
		Platform.Current.UpdatePlayTime(this.PlayTime);
		if (!forcedNotMemory && GameManager.IsMemoryScene(currentMapZone))
		{
			if (!this.playerData.HasStoredMemoryState)
			{
				this.playerData.PreMemoryState = HeroItemsState.Record(this.hero_ctrl);
				this.playerData.HasStoredMemoryState = true;
				this.playerData.CaptureToolAmountsOverride();
				EventRegister.SendEvent("END FOLLOWERS INSTANT", null);
				this.hero_ctrl.MaxHealthKeepBlue();
				return;
			}
		}
		else if (this.playerData.HasStoredMemoryState && GameManager.IsMemoryScene(previousMapZone))
		{
			this.playerData.PreMemoryState.Apply(this.hero_ctrl);
			this.playerData.HasStoredMemoryState = false;
			this.playerData.ClearToolAmountsOverride();
			EventRegister.SendEvent("END FOLLOWERS INSTANT", null);
		}
	}

	// Token: 0x06002408 RID: 9224 RVA: 0x000A69EC File Offset: 0x000A4BEC
	public void LoadedFromMenu()
	{
		if (this.playerData.vampireGnatRequestedAid)
		{
			this.playerData.vampireGnatRequestedAid = false;
		}
		if (this.playerData.garmondAidForumBattle)
		{
			this.playerData.garmondAidForumBattle = false;
		}
		if (this.playerData.shakraAidForumBattle)
		{
			this.playerData.shakraAidForumBattle = false;
		}
	}

	// Token: 0x06002409 RID: 9225 RVA: 0x000A6A44 File Offset: 0x000A4C44
	public void FixUpSaveState()
	{
	}

	// Token: 0x170003CE RID: 974
	// (get) Token: 0x0600240A RID: 9226 RVA: 0x000A6A46 File Offset: 0x000A4C46
	// (set) Token: 0x0600240B RID: 9227 RVA: 0x000A6A4E File Offset: 0x000A4C4E
	public bool BlockNextVibrationFadeIn { get; set; }

	// Token: 0x0600240C RID: 9228 RVA: 0x000A6A58 File Offset: 0x000A4C58
	public void FadeSceneIn()
	{
		if (GameManager.IsWaitingForSceneReady)
		{
			this.shouldFadeInScene = true;
			return;
		}
		if (!this.BlockNextVibrationFadeIn)
		{
			VibrationManager.FadeVibration(1f, 0.25f);
		}
		else
		{
			this.BlockNextVibrationFadeIn = false;
		}
		this.cameraCtrl.FadeSceneIn();
		this.screenFader_fsm.SendEvent("SCENE FADE IN");
		this.shouldFadeInScene = false;
	}

	// Token: 0x0600240D RID: 9229 RVA: 0x000A6AB6 File Offset: 0x000A4CB6
	public IEnumerator FadeSceneInWithDelay(float delay)
	{
		if (delay >= 0f)
		{
			yield return new WaitForSeconds(delay);
		}
		else
		{
			yield return null;
		}
		this.FadeSceneIn();
		yield break;
	}

	// Token: 0x0600240E RID: 9230 RVA: 0x000A6ACC File Offset: 0x000A4CCC
	public bool IsGamePaused()
	{
		return this.GameState == GameState.PAUSED;
	}

	// Token: 0x0600240F RID: 9231 RVA: 0x000A6ADA File Offset: 0x000A4CDA
	public void SetGameMap(GameObject go_gameMap)
	{
		this.gameMap = go_gameMap.GetComponent<GameMap>();
	}

	// Token: 0x06002410 RID: 9232 RVA: 0x000A6AE8 File Offset: 0x000A4CE8
	public void CalculateNotchesUsed()
	{
		this.playerData.CalculateNotchesUsed();
	}

	// Token: 0x06002411 RID: 9233 RVA: 0x000A6AF5 File Offset: 0x000A4CF5
	public string GetLanguageAsString()
	{
		return this.gameSettings.gameLanguage.ToString();
	}

	// Token: 0x06002412 RID: 9234 RVA: 0x000A6B0D File Offset: 0x000A4D0D
	public string GetEntryGateName()
	{
		return this.entryGateName;
	}

	// Token: 0x06002413 RID: 9235 RVA: 0x000A6B15 File Offset: 0x000A4D15
	public void SetDeathRespawnSimple(string respawnMarkerName, int respawnType, bool respawnFacingRight)
	{
		this.playerData.respawnMarkerName = respawnMarkerName;
		this.playerData.respawnType = respawnType;
		this.playerData.respawnScene = this.sceneName;
		this.SetCurrentMapZoneAsRespawn();
	}

	// Token: 0x06002414 RID: 9236 RVA: 0x000A6B46 File Offset: 0x000A4D46
	public void SetNonlethalDeathRespawn(string respawnMarkerName, int respawnType, bool respawnFacingRight)
	{
		this.playerData.nonLethalRespawnMarker = respawnMarkerName;
		this.playerData.nonLethalRespawnType = respawnType;
		this.playerData.nonLethalRespawnScene = this.sceneName;
	}

	// Token: 0x06002415 RID: 9237 RVA: 0x000A6B71 File Offset: 0x000A4D71
	public void SetPlayerDataBool(string boolName, bool value)
	{
		this.playerData.SetBool(boolName, value);
	}

	// Token: 0x06002416 RID: 9238 RVA: 0x000A6B80 File Offset: 0x000A4D80
	public void SetPlayerDataInt(string intName, int value)
	{
		this.playerData.SetInt(intName, value);
	}

	// Token: 0x06002417 RID: 9239 RVA: 0x000A6B8F File Offset: 0x000A4D8F
	public void SetPlayerDataFloat(string floatName, float value)
	{
		this.playerData.SetFloat(floatName, value);
	}

	// Token: 0x06002418 RID: 9240 RVA: 0x000A6B9E File Offset: 0x000A4D9E
	public void SetPlayerDataString(string stringName, string value)
	{
		this.playerData.SetString(stringName, value);
	}

	// Token: 0x06002419 RID: 9241 RVA: 0x000A6BAD File Offset: 0x000A4DAD
	public void IncrementPlayerDataInt(string intName)
	{
		this.playerData.IncrementInt(intName);
	}

	// Token: 0x0600241A RID: 9242 RVA: 0x000A6BBB File Offset: 0x000A4DBB
	public void DecrementPlayerDataInt(string intName)
	{
		this.playerData.DecrementInt(intName);
	}

	// Token: 0x0600241B RID: 9243 RVA: 0x000A6BC9 File Offset: 0x000A4DC9
	public void IntAdd(string intName, int amount)
	{
		this.playerData.IntAdd(intName, amount);
	}

	// Token: 0x0600241C RID: 9244 RVA: 0x000A6BD8 File Offset: 0x000A4DD8
	public bool GetPlayerDataBool(string boolName)
	{
		return this.playerData.GetBool(boolName);
	}

	// Token: 0x0600241D RID: 9245 RVA: 0x000A6BE6 File Offset: 0x000A4DE6
	public int GetPlayerDataInt(string intName)
	{
		return this.playerData.GetInt(intName);
	}

	// Token: 0x0600241E RID: 9246 RVA: 0x000A6BF4 File Offset: 0x000A4DF4
	public float GetPlayerDataFloat(string floatName)
	{
		return this.playerData.GetFloat(floatName);
	}

	// Token: 0x0600241F RID: 9247 RVA: 0x000A6C02 File Offset: 0x000A4E02
	public string GetPlayerDataString(string stringName)
	{
		return this.playerData.GetString(stringName);
	}

	// Token: 0x06002420 RID: 9248 RVA: 0x000A6C10 File Offset: 0x000A4E10
	public void SetPlayerDataVector3(string vectorName, Vector3 value)
	{
		this.playerData.SetVector3(vectorName, value);
	}

	// Token: 0x06002421 RID: 9249 RVA: 0x000A6C1F File Offset: 0x000A4E1F
	public Vector3 GetPlayerDataVector3(string vectorName)
	{
		return this.playerData.GetVector3(vectorName);
	}

	// Token: 0x06002422 RID: 9250 RVA: 0x000A6C2D File Offset: 0x000A4E2D
	public T GetPlayerDataVariable<T>(string fieldName)
	{
		return this.playerData.GetVariable(fieldName);
	}

	// Token: 0x06002423 RID: 9251 RVA: 0x000A6C3B File Offset: 0x000A4E3B
	public void SetPlayerDataVariable<T>(string fieldName, T value)
	{
		this.playerData.SetVariable(fieldName, value);
	}

	// Token: 0x06002424 RID: 9252 RVA: 0x000A6C4A File Offset: 0x000A4E4A
	public int GetNextMossberryValue()
	{
		return this.playerData.GetNextMossberryValue();
	}

	// Token: 0x06002425 RID: 9253 RVA: 0x000A6C57 File Offset: 0x000A4E57
	public int GetNextSilkGrubValue()
	{
		return this.playerData.GetNextSilkGrubValue();
	}

	// Token: 0x06002426 RID: 9254 RVA: 0x000A6C64 File Offset: 0x000A4E64
	public void EquipCharm(int charmNum)
	{
		this.playerData.EquipCharm(charmNum);
	}

	// Token: 0x06002427 RID: 9255 RVA: 0x000A6C72 File Offset: 0x000A4E72
	public void UnequipCharm(int charmNum)
	{
		this.playerData.UnequipCharm(charmNum);
	}

	// Token: 0x06002428 RID: 9256 RVA: 0x000A6C80 File Offset: 0x000A4E80
	public void RefreshOvercharm()
	{
	}

	// Token: 0x06002429 RID: 9257 RVA: 0x000A6C82 File Offset: 0x000A4E82
	public void UpdateBlueHealth()
	{
		this.hero_ctrl.UpdateBlueHealth();
	}

	// Token: 0x0600242A RID: 9258 RVA: 0x000A6C90 File Offset: 0x000A4E90
	public void AddBlueHealthQueued()
	{
		int num = this.QueuedBlueHealth;
		this.QueuedBlueHealth = num + 1;
		EventRegister.SendEvent(EventRegisterEvents.AddQueuedBlueHealth, null);
	}

	// Token: 0x0600242B RID: 9259 RVA: 0x000A6CB8 File Offset: 0x000A4EB8
	public void SetCurrentMapZoneAsRespawn()
	{
		this.playerData.mapZone = this.sm.mapZone;
		this.playerData.extraRestZone = this.sm.extraRestZone;
	}

	// Token: 0x0600242C RID: 9260 RVA: 0x000A6CE6 File Offset: 0x000A4EE6
	public void SetOverrideMapZoneAsRespawn(MapZone mapZone)
	{
		this.playerData.mapZone = mapZone;
		this.playerData.extraRestZone = this.sm.extraRestZone;
	}

	// Token: 0x0600242D RID: 9261 RVA: 0x000A6D0C File Offset: 0x000A4F0C
	public void SetMapZoneToSpecific(string mapZone)
	{
		object obj = Enum.Parse(typeof(MapZone), mapZone);
		if (obj != null)
		{
			this.playerData.mapZone = (MapZone)obj;
		}
		else
		{
			Debug.LogError("Couldn't convert " + mapZone + " to a MapZone");
		}
		this.playerData.extraRestZone = ExtraRestZones.None;
	}

	// Token: 0x0600242E RID: 9262 RVA: 0x000A6D64 File Offset: 0x000A4F64
	public bool UpdateGameMapWithPopup(float delay = 0f)
	{
		GameObject mapUpdatedPopupPrefab = UI.MapUpdatedPopupPrefab;
		if (mapUpdatedPopupPrefab != null)
		{
			Object.Instantiate<GameObject>(mapUpdatedPopupPrefab).GetComponent<PlayMakerFSM>().FsmVariables.FindFsmFloat("Delay").Value = delay;
		}
		return this.UpdateGameMap();
	}

	// Token: 0x0600242F RID: 9263 RVA: 0x000A6DA8 File Offset: 0x000A4FA8
	public bool UpdateGameMap()
	{
		bool result = this.gameMap.UpdateGameMap();
		this.gameMap.SetupMap(false);
		if (this.playerData.HasAnyPin && !CollectableItemManager.IsInHiddenMode() && MapPin.DidActivateNewPin)
		{
			MapPin.DidActivateNewPin = false;
			result = true;
		}
		this.playerData.mapUpdateQueued = false;
		return result;
	}

	// Token: 0x06002430 RID: 9264 RVA: 0x000A6DFD File Offset: 0x000A4FFD
	public bool UpdateGameMapPins()
	{
		this.gameMap.SetupMap(true);
		bool didActivateNewPin = MapPin.DidActivateNewPin;
		MapPin.DidActivateNewPin = false;
		if (didActivateNewPin)
		{
			this.DidPurchasePin = true;
		}
		return didActivateNewPin;
	}

	// Token: 0x170003CF RID: 975
	// (get) Token: 0x06002431 RID: 9265 RVA: 0x000A6E20 File Offset: 0x000A5020
	// (set) Token: 0x06002432 RID: 9266 RVA: 0x000A6E28 File Offset: 0x000A5028
	public bool DidPurchasePin { get; set; }

	// Token: 0x170003D0 RID: 976
	// (get) Token: 0x06002433 RID: 9267 RVA: 0x000A6E31 File Offset: 0x000A5031
	// (set) Token: 0x06002434 RID: 9268 RVA: 0x000A6E39 File Offset: 0x000A5039
	public bool DidPurchaseMap { get; set; }

	// Token: 0x06002435 RID: 9269 RVA: 0x000A6E42 File Offset: 0x000A5042
	public bool DoShopCloseGameMapUpdate()
	{
		if (this.DidPurchasePin || this.DidPurchaseMap)
		{
			this.DidPurchasePin = false;
			this.DidPurchaseMap = false;
			this.UpdateGameMapWithPopup(0f);
			return true;
		}
		return false;
	}

	// Token: 0x06002436 RID: 9270 RVA: 0x000A6E71 File Offset: 0x000A5071
	public void AddToScenesVisited(string scene)
	{
		scene = scene.Trim();
		if (string.IsNullOrWhiteSpace(scene))
		{
			return;
		}
		if (this.playerData.scenesVisited.Contains(scene))
		{
			return;
		}
		this.playerData.scenesVisited.Add(scene);
	}

	// Token: 0x06002437 RID: 9271 RVA: 0x000A6EAA File Offset: 0x000A50AA
	public void AddToBenchList()
	{
		if (!this.playerData.scenesEncounteredBench.Contains(this.GetSceneNameString()))
		{
			this.playerData.scenesEncounteredBench.Add(this.GetSceneNameString());
		}
	}

	// Token: 0x06002438 RID: 9272 RVA: 0x000A6EDB File Offset: 0x000A50DB
	public void AddToGrubList()
	{
	}

	// Token: 0x06002439 RID: 9273 RVA: 0x000A6EDD File Offset: 0x000A50DD
	public void AddToFlameList()
	{
	}

	// Token: 0x0600243A RID: 9274 RVA: 0x000A6EDF File Offset: 0x000A50DF
	public void AddToCocoonList()
	{
		if (!this.playerData.scenesEncounteredCocoon.Contains(this.GetSceneNameString()))
		{
			this.playerData.scenesEncounteredCocoon.Add(this.GetSceneNameString());
		}
	}

	// Token: 0x0600243B RID: 9275 RVA: 0x000A6F10 File Offset: 0x000A5110
	public void AddToDreamPlantList()
	{
		Debug.LogError("DEPRECATED");
	}

	// Token: 0x0600243C RID: 9276 RVA: 0x000A6F1C File Offset: 0x000A511C
	public void AddToDreamPlantCList()
	{
		Debug.LogError("DEPRECATED");
	}

	// Token: 0x0600243D RID: 9277 RVA: 0x000A6F28 File Offset: 0x000A5128
	public void CountGameCompletion()
	{
		this.playerData.CountGameCompletion();
	}

	// Token: 0x0600243E RID: 9278 RVA: 0x000A6F35 File Offset: 0x000A5135
	public void CountJournalEntries()
	{
	}

	// Token: 0x0600243F RID: 9279 RVA: 0x000A6F37 File Offset: 0x000A5137
	public void ActivateTestingCheats()
	{
		this.playerData.ActivateTestingCheats();
	}

	// Token: 0x06002440 RID: 9280 RVA: 0x000A6F44 File Offset: 0x000A5144
	public void GetAllPowerups()
	{
		this.playerData.GetAllPowerups();
	}

	// Token: 0x06002441 RID: 9281 RVA: 0x000A6F54 File Offset: 0x000A5154
	public void MapperLeavePreviousLocations(string seenBool)
	{
		if (this.playerData.SeenMapperBoneForest && seenBool != "SeenMapperBoneForest")
		{
			this.playerData.MapperLeftBoneForest = true;
		}
		if (this.playerData.SeenMapperCoralCaverns && seenBool != "SeenMapperCoralCaverns")
		{
			this.playerData.MapperLeftCoralCaverns = true;
		}
		if (this.playerData.SeenMapperCrawl && seenBool != "SeenMapperCrawl")
		{
			this.playerData.MapperLeftCrawl = true;
		}
		if (this.playerData.SeenMapperDocks && seenBool != "SeenMapperDocks")
		{
			this.playerData.MapperLeftDocks = true;
		}
		if (this.playerData.SeenMapperDustpens && seenBool != "SeenMapperDustpens")
		{
			this.playerData.MapperLeftDustpens = true;
		}
		if (this.playerData.SeenMapperGreymoor && seenBool != "SeenMapperGreymoor")
		{
			this.playerData.MapperLeftGreymoor = true;
		}
		if (this.playerData.SeenMapperHuntersNest && seenBool != "SeenMapperHuntersNest")
		{
			this.playerData.MapperLeftHuntersNest = true;
		}
		if (this.playerData.SeenMapperJudgeSteps && seenBool != "SeenMapperJudgeSteps")
		{
			this.playerData.MapperLeftJudgeSteps = true;
		}
		if (this.playerData.SeenMapperPeak && seenBool != "SeenMapperPeak")
		{
			this.playerData.MapperLeftPeak = true;
		}
		if (this.playerData.SeenMapperShadow && seenBool != "SeenMapperShadow")
		{
			this.playerData.MapperLeftShadow = true;
		}
		if (this.playerData.SeenMapperShellwood && seenBool != "SeenMapperShellwood")
		{
			this.playerData.MapperLeftShellwood = true;
		}
		if (this.playerData.SeenMapperWilds && seenBool != "SeenMapperWilds")
		{
			this.playerData.MapperLeftWilds = true;
		}
	}

	// Token: 0x06002442 RID: 9282 RVA: 0x000A7129 File Offset: 0x000A5329
	public void AwardAchievement(string key)
	{
		this.achievementHandler.AwardAchievementToPlayer(key);
	}

	// Token: 0x06002443 RID: 9283 RVA: 0x000A7137 File Offset: 0x000A5337
	public void QueueAchievementProgress(string key, int current, int max)
	{
		this.achievementHandler.QueueAchievementProgress(key, current, max);
	}

	// Token: 0x06002444 RID: 9284 RVA: 0x000A7147 File Offset: 0x000A5347
	public void UpdateAchievementProgress(string key, int current, int max)
	{
		if (current >= max)
		{
			this.AwardAchievement(key);
		}
		this.achievementHandler.UpdateAchievementProgress(key, current, max);
	}

	// Token: 0x06002445 RID: 9285 RVA: 0x000A7162 File Offset: 0x000A5362
	public void QueueAchievement(string key)
	{
		this.achievementHandler.QueueAchievement(key);
	}

	// Token: 0x06002446 RID: 9286 RVA: 0x000A7170 File Offset: 0x000A5370
	public void AwardQueuedAchievements(float delay)
	{
		if (delay > 0f)
		{
			this.StartTimerRoutine(delay, 0f, null, new Action(this.AwardQueuedAchievements), null, false);
			return;
		}
		this.AwardQueuedAchievements();
	}

	// Token: 0x06002447 RID: 9287 RVA: 0x000A71A0 File Offset: 0x000A53A0
	public void AwardQueuedAchievements()
	{
		this.achievementHandler.AwardQueuedAchievements();
		foreach (string key in this.queuedMenuStyles)
		{
			MenuStyleUnlock.Unlock(key, true);
		}
		this.queuedMenuStyles.Clear();
	}

	// Token: 0x06002448 RID: 9288 RVA: 0x000A7208 File Offset: 0x000A5408
	public void QueuedMenuStyleUnlock(string key)
	{
		if (!this.queuedMenuStyles.Contains(key))
		{
			this.queuedMenuStyles.Add(key);
		}
	}

	// Token: 0x06002449 RID: 9289 RVA: 0x000A7224 File Offset: 0x000A5424
	public static bool CanUnlockMenuStyle(string key)
	{
		GameManager instance = GameManager.instance;
		if (instance == null)
		{
			return true;
		}
		if (instance.achievementHandler != null && instance.achievementHandler.QueuedAchievements.Contains(key))
		{
			instance.QueuedMenuStyleUnlock(key);
			return false;
		}
		return true;
	}

	// Token: 0x0600244A RID: 9290 RVA: 0x000A726D File Offset: 0x000A546D
	public bool IsAchievementAwarded(string key)
	{
		return this.achievementHandler.AchievementWasAwarded(key);
	}

	// Token: 0x0600244B RID: 9291 RVA: 0x000A727B File Offset: 0x000A547B
	public void ClearAllAchievements()
	{
		this.achievementHandler.ResetAllAchievements();
	}

	// Token: 0x0600244C RID: 9292 RVA: 0x000A7288 File Offset: 0x000A5488
	public void CheckAllAchievements()
	{
		bool isFiringAchievementsFromSavesAllowed = Platform.Current.IsFiringAchievementsFromSavesAllowed;
	}

	// Token: 0x0600244D RID: 9293 RVA: 0x000A7298 File Offset: 0x000A5498
	public void CheckBellwayAchievements()
	{
		int num = 0;
		if (this.playerData.UnlockedDocksStation)
		{
			num++;
		}
		if (this.playerData.UnlockedBoneforestEastStation)
		{
			num++;
		}
		if (this.playerData.UnlockedGreymoorStation)
		{
			num++;
		}
		if (this.playerData.UnlockedBelltownStation)
		{
			num++;
		}
		if (this.playerData.UnlockedCoralTowerStation)
		{
			num++;
		}
		if (this.playerData.UnlockedCityStation)
		{
			num++;
		}
		if (this.playerData.UnlockedPeakStation)
		{
			num++;
		}
		if (this.playerData.UnlockedShellwoodStation)
		{
			num++;
		}
		if (this.playerData.UnlockedShadowStation)
		{
			num++;
		}
		if (this.playerData.UnlockedAqueductStation)
		{
			num++;
		}
		this.UpdateAchievementProgress("BELLWAYS_FULL", num, 10);
	}

	// Token: 0x0600244E RID: 9294 RVA: 0x000A7360 File Offset: 0x000A5560
	public void CheckTubeAchievements()
	{
		int num = 0;
		if (this.playerData.UnlockedSongTube)
		{
			num++;
		}
		if (this.playerData.UnlockedUnderTube)
		{
			num++;
		}
		if (this.playerData.UnlockedCityBellwayTube)
		{
			num++;
		}
		if (this.playerData.UnlockedHangTube)
		{
			num++;
		}
		if (this.playerData.UnlockedEnclaveTube)
		{
			num++;
		}
		if (this.playerData.UnlockedArboriumTube)
		{
			num++;
		}
		this.UpdateAchievementProgress("TUBES_FULL", num, 6);
	}

	// Token: 0x0600244F RID: 9295 RVA: 0x000A73E2 File Offset: 0x000A55E2
	public void CheckMapAchievements()
	{
		this.UpdateAchievementProgress("ALL_MAPS", this.playerData.MapCount, 28);
	}

	// Token: 0x06002450 RID: 9296 RVA: 0x000A73FC File Offset: 0x000A55FC
	public void CheckSubQuestAchievements()
	{
		int num = 0;
		if (this.playerData.HasMelodyArchitect)
		{
			num++;
		}
		if (this.playerData.HasMelodyConductor)
		{
			num++;
		}
		if (this.playerData.HasMelodyLibrarian)
		{
			num++;
		}
		this.UpdateAchievementProgress("CITADEL_SONG", num, 3);
	}

	// Token: 0x06002451 RID: 9297 RVA: 0x000A744C File Offset: 0x000A564C
	public void CheckHeartAchievements()
	{
		int num = (this.playerData.maxHealthBase - 5) * 4 + this.playerData.heartPieces;
		if (num < 4)
		{
			this.QueueAchievementProgress("FIRST_MASK", num, 4);
		}
		this.QueueAchievementProgress("ALL_MASKS", num, 20);
	}

	// Token: 0x06002452 RID: 9298 RVA: 0x000A7494 File Offset: 0x000A5694
	public void CheckSilkSpoolAchievements()
	{
		int num = (this.playerData.silkMax - 9) * 2 + this.playerData.silkSpoolParts;
		if (num < 2)
		{
			this.QueueAchievementProgress("FIRST_SILK_SPOOL", num, 2);
		}
		this.QueueAchievementProgress("ALL_SILK_SPOOLS", num, 18);
	}

	// Token: 0x06002453 RID: 9299 RVA: 0x000A74E0 File Offset: 0x000A56E0
	public void CheckCompletionAchievements()
	{
		this.CountGameCompletion();
		bool flag = this.playerData.permadeathMode > PermadeathModes.Off;
		bool flag2 = this.playerData.completionPercentage >= 100f;
		if (flag)
		{
			this.AwardAchievement("STEEL_SOUL");
			MenuStyleUnlock.Unlock("COMPLETED_STEEL", false);
		}
		if (flag2)
		{
			this.AwardAchievement("COMPLETION");
			if (flag)
			{
				this.AwardAchievement("STEEL_SOUL_FULL");
			}
		}
		if (this.playerData.playTime <= 18000f)
		{
			this.AwardAchievement("SPEEDRUN_1");
		}
		if (flag2 && this.playerData.playTime <= 108000f)
		{
			this.AwardAchievement("SPEED_COMPLETION");
		}
	}

	// Token: 0x06002454 RID: 9300 RVA: 0x000A758A File Offset: 0x000A578A
	public void RecordGameComplete()
	{
	}

	// Token: 0x06002455 RID: 9301 RVA: 0x000A758C File Offset: 0x000A578C
	public void SetStatusRecordInt(string key, int value)
	{
		Platform.Current.RoamingSharedData.SetInt(key, value);
	}

	// Token: 0x06002456 RID: 9302 RVA: 0x000A759F File Offset: 0x000A579F
	public int GetStatusRecordInt(string key)
	{
		return Platform.Current.RoamingSharedData.GetInt(key, 0);
	}

	// Token: 0x06002457 RID: 9303 RVA: 0x000A75B2 File Offset: 0x000A57B2
	public void ResetStatusRecords()
	{
		Platform.Current.RoamingSharedData.DeleteKey("RecPermadeathMode");
	}

	// Token: 0x06002458 RID: 9304 RVA: 0x000A75C8 File Offset: 0x000A57C8
	public void SaveStatusRecords()
	{
		Platform.Current.RoamingSharedData.Save();
	}

	// Token: 0x06002459 RID: 9305 RVA: 0x000A75D9 File Offset: 0x000A57D9
	public void SetState(GameState newState)
	{
		this.GameState = newState;
		GameManager.GameStateEvent gameStateChange = this.GameStateChange;
		if (gameStateChange == null)
		{
			return;
		}
		gameStateChange(newState);
	}

	// Token: 0x0600245A RID: 9306 RVA: 0x000A75F4 File Offset: 0x000A57F4
	public void LoadScene(string destScene)
	{
		PersistentAudioManager.OnLeaveScene();
		PersistentAudioManager.QueueSceneEntry();
		this.startedOnThisScene = false;
		this.nextSceneName = destScene;
		Action nextSceneWillActivate = this.NextSceneWillActivate;
		if (nextSceneWillActivate != null)
		{
			nextSceneWillActivate();
		}
		Action unloadingLevel = this.UnloadingLevel;
		if (unloadingLevel != null)
		{
			unloadingLevel();
		}
		AsyncOperationHandle<SceneInstance> fromOperationHandle = Addressables.LoadSceneAsync("Scenes/" + destScene, LoadSceneMode.Single, true, 100, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded);
		this.LastSceneLoad = new SceneLoad(fromOperationHandle, new GameManager.SceneLoadInfo
		{
			SceneName = destScene
		});
	}

	// Token: 0x0600245B RID: 9307 RVA: 0x000A7669 File Offset: 0x000A5869
	public IEnumerator LoadSceneAdditive(string destScene)
	{
		this.startedOnThisScene = false;
		this.nextSceneName = destScene;
		this.waitForManualLevelStart = true;
		GameManager.IsWaitingForSceneReady = true;
		Action nextSceneWillActivate = this.NextSceneWillActivate;
		if (nextSceneWillActivate != null)
		{
			nextSceneWillActivate();
		}
		Action unloadingLevel = this.UnloadingLevel;
		if (unloadingLevel != null)
		{
			unloadingLevel();
		}
		string exitingScene = SceneManager.GetActiveScene().name;
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(destScene, LoadSceneMode.Additive);
		asyncOperation.allowSceneActivation = true;
		yield return asyncOperation;
		this.UnloadScene(exitingScene, null);
		this.RefreshTilemapInfo(destScene);
		if (SceneLoad.IsClearMemoryRequired())
		{
			yield return SceneLoad.TryClearMemory(true, true);
		}
		else
		{
			GCManager.Collect();
		}
		this.SetupSceneRefs(true);
		this.BeginScene();
		this.OnNextLevelReady();
		this.waitForManualLevelStart = false;
		yield break;
	}

	// Token: 0x0600245C RID: 9308 RVA: 0x000A7680 File Offset: 0x000A5880
	public void OnNextLevelReady()
	{
		this.nextLevelEntryNumber++;
		PersistentAudioManager.AttachToObject(this.cameraCtrl.transform);
		if (!this.IsGameplayScene())
		{
			GameManager.IsWaitingForSceneReady = false;
			if (this.shouldFadeInScene)
			{
				this.FadeSceneIn();
			}
			AudioManager.UnpauseActorSnapshot(null);
			return;
		}
		this.SimulateSceneStartPhysics();
		GameManager.IsWaitingForSceneReady = false;
		this.SetState(GameState.ENTERING_LEVEL);
		this.playerData.disablePause = false;
		this.inputHandler.AllowPause();
		this.inputHandler.StartAcceptingInput();
		HeroController instance = HeroController.instance;
		if (GameManager.SuppressRegainControl)
		{
			GameManager.SuppressRegainControl = false;
		}
		else
		{
			instance.RegainControl(false);
		}
		instance.StartAnimationControl();
		this.EnterHero();
		AudioManager.UnpauseActorSnapshot(delegate
		{
			if (!this.SkipNormalActorFadeIn())
			{
				this.actorSnapshotUnpaused.TransitionToSafe(this.sceneTransitionActorFadeUp);
			}
		});
		this.UpdateUIStateFromGameState();
		if (!GameManager.IsMemoryScene(this.sm.mapZone))
		{
			this.playerData.nonLethalRespawnScene = null;
			this.playerData.nonLethalRespawnMarker = null;
			this.playerData.nonLethalRespawnType = -1;
		}
		if (this.shouldFadeInScene)
		{
			this.FadeSceneIn();
			return;
		}
		if (!this.BlockNextVibrationFadeIn)
		{
			VibrationManager.FadeVibration(1f, 0.25f);
			return;
		}
		this.BlockNextVibrationFadeIn = false;
	}

	// Token: 0x0600245D RID: 9309 RVA: 0x000A77A6 File Offset: 0x000A59A6
	public bool SkipNormalActorFadeIn()
	{
		return this.skipActorEntryFade >= this.nextLevelEntryNumber;
	}

	// Token: 0x0600245E RID: 9310 RVA: 0x000A77B9 File Offset: 0x000A59B9
	public void SetSkipNextLevelReadyActorFadeIn(bool skip)
	{
		if (skip)
		{
			this.skipActorEntryFade = this.nextLevelEntryNumber + 1;
			return;
		}
		this.skipActorEntryFade = -1;
	}

	// Token: 0x0600245F RID: 9311 RVA: 0x000A77D4 File Offset: 0x000A59D4
	private void SimulateSceneStartPhysics()
	{
		this.hero_ctrl.AffectedByGravity(false);
		this.hero_ctrl.ResetVelocity();
		GameManager.Rb2dState[] array = (from body in Object.FindObjectsOfType<Rigidbody2D>()
		where body.gameObject.layer == 11
		select new GameManager.Rb2dState
		{
			Body = body,
			Simulated = body.simulated
		}).ToArray<GameManager.Rb2dState>();
		GameManager.Rb2dState[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Body.simulated = false;
		}
		GameManager.ISceneManualSimulatePhysics[] array3 = Object.FindObjectsOfType<MonoBehaviour>().OfType<GameManager.ISceneManualSimulatePhysics>().ToArray<GameManager.ISceneManualSimulatePhysics>();
		SimulationMode2D simulationMode = Physics2D.simulationMode;
		Physics2D.simulationMode = SimulationMode2D.Script;
		GameManager.ISceneManualSimulatePhysics[] array4 = array3;
		for (int i = 0; i < array4.Length; i++)
		{
			array4[i].PrepareManualSimulate();
		}
		float fixedDeltaTime;
		for (float num = 3f; num > 0f; num -= fixedDeltaTime)
		{
			fixedDeltaTime = Time.fixedDeltaTime;
			Physics2D.Simulate(fixedDeltaTime);
			array4 = array3;
			for (int i = 0; i < array4.Length; i++)
			{
				array4[i].OnManualPhysics(fixedDeltaTime);
			}
		}
		Physics2D.simulationMode = simulationMode;
		foreach (GameManager.Rb2dState rb2dState in array)
		{
			rb2dState.Body.simulated = rb2dState.Simulated;
		}
		array4 = array3;
		for (int i = 0; i < array4.Length; i++)
		{
			array4[i].OnManualSimulateFinished();
		}
	}

	// Token: 0x06002460 RID: 9312 RVA: 0x000A7951 File Offset: 0x000A5B51
	public void OnWillActivateFirstLevel()
	{
		HeroController.instance.isEnteringFirstLevel = true;
		this.entryGateName = "top1";
		this.SetState(GameState.PLAYING);
		this.ui.ConfigureMenu();
	}

	// Token: 0x06002461 RID: 9313 RVA: 0x000A797B File Offset: 0x000A5B7B
	public IEnumerator LoadFirstScene()
	{
		yield return new WaitForEndOfFrame();
		this.OnWillActivateFirstLevel();
		this.LoadScene("Tut_01");
		yield break;
	}

	// Token: 0x06002462 RID: 9314 RVA: 0x000A798A File Offset: 0x000A5B8A
	public void LoadPermadeathUnlockScene()
	{
		if (this.GetStatusRecordInt("RecPermadeathMode") == 0)
		{
			this.LoadScene("PermaDeath_Unlock");
			return;
		}
		base.StartCoroutine(this.ReturnToMainMenu(true, null, false, false));
	}

	// Token: 0x06002463 RID: 9315 RVA: 0x000A79B6 File Offset: 0x000A5BB6
	public void LoadOpeningCinematic()
	{
		this.SetState(GameState.CUTSCENE);
		this.LoadScene("Intro_Cutscene");
	}

	// Token: 0x06002464 RID: 9316 RVA: 0x000A79CC File Offset: 0x000A5BCC
	private void PositionHeroAtSceneEntrance()
	{
		Vector2 position = this.FindEntryPoint(this.entryGateName, default(Scene)) ?? new Vector2(-20000f, 20000f);
		if (this.hero_ctrl != null)
		{
			this.hero_ctrl.transform.SetPosition2D(position);
		}
	}

	// Token: 0x06002465 RID: 9317 RVA: 0x000A7A30 File Offset: 0x000A5C30
	private Vector2? FindEntryPoint(string entryPointName, Scene filterScene)
	{
		if (this.RespawningHero)
		{
			Transform transform = this.hero_ctrl.LocateSpawnPoint();
			if (transform != null)
			{
				return new Vector2?(transform.transform.position);
			}
			return null;
		}
		else
		{
			if (this.hazardRespawningHero)
			{
				return new Vector2?(this.playerData.hazardRespawnLocation);
			}
			TransitionPoint transitionPoint = this.FindTransitionPoint(entryPointName, filterScene, true);
			if (transitionPoint != null)
			{
				return new Vector2?(transitionPoint.transform.position + transitionPoint.entryOffset);
			}
			return null;
		}
	}

	// Token: 0x06002466 RID: 9318 RVA: 0x000A7AD8 File Offset: 0x000A5CD8
	private TransitionPoint FindTransitionPoint(string entryPointName, Scene filterScene, bool fallbackToAnyAvailable)
	{
		List<TransitionPoint> transitionPoints = TransitionPoint.TransitionPoints;
		foreach (TransitionPoint transitionPoint in transitionPoints)
		{
			if (!(transitionPoint.name != entryPointName) && (!filterScene.IsValid() || transitionPoint.gameObject.scene == filterScene))
			{
				return transitionPoint;
			}
		}
		if (fallbackToAnyAvailable && transitionPoints.Count > 0)
		{
			TransitionPoint transitionPoint2 = transitionPoints[0];
			Debug.LogWarning(string.Concat(new string[]
			{
				"Couldn't find transition point \"",
				entryPointName,
				"\", falling back to first available: \"",
				transitionPoint2.name,
				"\""
			}));
			return transitionPoint2;
		}
		return null;
	}

	// Token: 0x06002467 RID: 9319 RVA: 0x000A7BA4 File Offset: 0x000A5DA4
	private void EnterHero()
	{
		GameManager.<>c__DisplayClass360_0 CS$<>8__locals1 = new GameManager.<>c__DisplayClass360_0();
		CS$<>8__locals1.<>4__this = this;
		if (this.entryGateName == "door_dreamReturn" && !string.IsNullOrEmpty(this.playerData.bossReturnEntryGate))
		{
			this.entryGateName = this.playerData.bossReturnEntryGate;
			this.playerData.bossReturnEntryGate = string.Empty;
		}
		if (this.RespawningHero)
		{
			Transform transform = this.hero_ctrl.LocateSpawnPoint();
			CS$<>8__locals1.respawnMarker = (transform ? transform.GetComponent<RespawnMarker>() : null);
			CS$<>8__locals1.hasRespawnMarker = (CS$<>8__locals1.respawnMarker != null);
			if (CS$<>8__locals1.hasRespawnMarker)
			{
				CS$<>8__locals1.respawnMarker.PrepareRespawnHere();
			}
			CS$<>8__locals1.hasFaded = false;
			if (this.needFirstFadeIn && (!CS$<>8__locals1.hasRespawnMarker || !CS$<>8__locals1.respawnMarker.customWakeUp) && (!CS$<>8__locals1.hasRespawnMarker || !CS$<>8__locals1.respawnMarker.customFadeDuration.IsEnabled))
			{
				this.screenFader_fsm.SendEventSafe("SCENE FADE OUT INSTANT");
				base.StartCoroutine(this.FadeSceneInWithDelay(0.3f));
				CS$<>8__locals1.hasFaded = true;
			}
			this.needFirstFadeIn = false;
			if (CS$<>8__locals1.hasFaded || this.cameraCtrl.HasBeenPositionedAtHero)
			{
				CS$<>8__locals1.<EnterHero>g__DoFadeIn|1();
			}
			else
			{
				this.cameraCtrl.PositionedAtHero += CS$<>8__locals1.<EnterHero>g__OnHeroInPosition|0;
			}
			base.StartCoroutine(this.hero_ctrl.Respawn(transform));
			this.RespawningHero = false;
			return;
		}
		if (this.hazardRespawningHero)
		{
			base.StartCoroutine(this.hero_ctrl.HazardRespawn());
			this.FinishedEnteringScene();
			this.hazardRespawningHero = false;
			return;
		}
		if (this.entryGateName == "dreamGate")
		{
			this.hero_ctrl.EnterSceneDreamGate();
			return;
		}
		if (!this.startedOnThisScene)
		{
			this.SetState(GameState.ENTERING_LEVEL);
			bool enterSkip = this.sceneLoad != null && this.sceneLoad.SceneLoadInfo.EntrySkip;
			if (string.IsNullOrEmpty(this.entryGateName))
			{
				this.FinishedEnteringScene();
				return;
			}
			TransitionPoint transitionPoint = this.FindTransitionPoint(this.entryGateName, default(Scene), true);
			if (transitionPoint)
			{
				base.StartCoroutine(this.hero_ctrl.EnterScene(transitionPoint, this.entryDelay, false, null, enterSkip));
				return;
			}
			if (ProjectBenchmark.IsRunning)
			{
				TransitionPoint transitionPoint2 = TransitionPoint.TransitionPoints.FirstOrDefault<TransitionPoint>();
				if (transitionPoint2)
				{
					base.StartCoroutine(this.hero_ctrl.EnterScene(transitionPoint2, this.entryDelay, false, null, enterSkip));
					return;
				}
			}
		}
		else
		{
			if (!this.IsGameplayScene())
			{
				return;
			}
			this.FinishedEnteringScene();
			this.FadeSceneIn();
		}
	}

	// Token: 0x06002468 RID: 9320 RVA: 0x000A7E27 File Offset: 0x000A6027
	public void FinishedEnteringScene()
	{
		if (this.GameState != GameState.CUTSCENE)
		{
			this.SetState(GameState.PLAYING);
		}
		this.entryDelay = 0f;
		this.hasFinishedEnteringScene = true;
		EventRegister.SendEvent(EventRegisterEvents.HeroEnteredScene, null);
		GameManager.EnterSceneEvent onFinishedEnteringScene = this.OnFinishedEnteringScene;
		if (onFinishedEnteringScene == null)
		{
			return;
		}
		onFinishedEnteringScene();
	}

	// Token: 0x06002469 RID: 9321 RVA: 0x000A7E68 File Offset: 0x000A6068
	private void SetupGameRefs()
	{
		if (this.hasSetup)
		{
			return;
		}
		this.hasSetup = true;
		this.playerData = PlayerData.instance;
		this.playerData.SetupExistingPlayerData();
		this.sceneData = SceneData.instance;
		this.gameCams = GameCameras.instance;
		this.cameraCtrl = this.gameCams.cameraController;
		this.gameSettings = new GameSettings();
		this.inputHandler = base.GetComponent<InputHandler>();
		this.achievementHandler = base.GetComponent<AchievementHandler>();
		this.SpawnInControlManager();
		GameObject gameplayChild = this.gameCams.hudCamera.GetComponent<HUDCamera>().GameplayChild;
		this.screenFader_fsm = gameplayChild.LocateMyFSM("Screen Fader");
		this.inventoryFSM = gameplayChild.transform.Find("Inventory").gameObject.GetComponent<PlayMakerFSM>();
		if (AchievementPopupHandler.Instance)
		{
			AchievementPopupHandler.Instance.Setup(this.achievementHandler);
		}
		Platform.Current.AdjustGraphicsSettings(this.gameSettings);
		if (this.inputHandler == null)
		{
			Debug.LogError("Couldn't find InputHandler component.");
		}
		if (this.achievementHandler == null)
		{
			Debug.LogError("Couldn't find AchievementHandler component.");
		}
		SceneManager.activeSceneChanged += this.LevelActivated;
		this.NextSceneWillActivate += AutoRecycleSelf.RecycleActiveRecyclers;
		this.NextSceneWillActivate += PlayAudioAndRecycle.RecycleActiveRecyclers;
		this.NextSceneWillActivate += ResetDynamicHierarchy.ForceReconnectAll;
		this.RegisterEvents();
	}

	// Token: 0x0600246A RID: 9322 RVA: 0x000A7FDC File Offset: 0x000A61DC
	private void RegisterEvents()
	{
		if (!this.registerEvents)
		{
			this.registerEvents = true;
			this.subbedCamShake = GlobalSettings.Camera.MainCameraShakeManager;
			this.subbedCamShake.CameraShakedWorldForce += CurrencyObjectBase.SendOnCameraShakedWorldForce;
			this.cameraCtrl.PositionedAtHero += PersistentAudioManager.OnEnteredNextScene;
		}
	}

	// Token: 0x0600246B RID: 9323 RVA: 0x000A8034 File Offset: 0x000A6234
	private void UnregisterEvents()
	{
		if (this.registerEvents)
		{
			this.registerEvents = false;
			if (this.subbedCamShake)
			{
				this.subbedCamShake.CameraShakedWorldForce -= CurrencyObjectBase.SendOnCameraShakedWorldForce;
				this.subbedCamShake = null;
			}
			if (this.cameraCtrl)
			{
				this.cameraCtrl.PositionedAtHero -= PersistentAudioManager.OnEnteredNextScene;
			}
		}
	}

	// Token: 0x0600246C RID: 9324 RVA: 0x000A80A0 File Offset: 0x000A62A0
	private void FindSceneManager()
	{
		GameObject gameObject = GameObject.FindGameObjectWithTag("SceneManager");
		if (gameObject != null)
		{
			this.sm = gameObject.GetComponent<CustomSceneManager>();
			CustomSceneManager.IncrementVersion();
			return;
		}
		Debug.Log("Scene Manager missing from scene " + this.sceneName);
	}

	// Token: 0x0600246D RID: 9325 RVA: 0x000A80E8 File Offset: 0x000A62E8
	public void EnsureGlobalPool()
	{
		if (this.globalPoolPrefabHandle.IsValid())
		{
			return;
		}
		if (!this.IsGameplayScene())
		{
			return;
		}
		Object.Instantiate<GameObject>(this.LoadGlobalPoolPrefab().WaitForCompletion());
	}

	// Token: 0x0600246E RID: 9326 RVA: 0x000A8120 File Offset: 0x000A6320
	public void SetupSceneRefs(bool refreshTilemapInfo)
	{
		this.forceCurrentSceneMemory = false;
		this.UpdateSceneName();
		if (this.ui == null)
		{
			this.ui = UIManager.instance;
		}
		this.FindSceneManager();
		if (this.IsGameplayScene())
		{
			this.EnsureGlobalPool();
			ObjectPool.CreateStartupPools();
			if (this.hero_ctrl == null)
			{
				this.SetupHeroRefs();
			}
			this.inputHandler.AttachHeroController(this.hero_ctrl);
			if (refreshTilemapInfo)
			{
				this.RefreshTilemapInfo(this.sceneName);
			}
			this.soulOrb_fsm = this.gameCams.soulOrbFSM;
			this.soulVessel_fsm = this.gameCams.soulVesselFSM;
		}
		if (this.sceneSeedTrackers == null)
		{
			this.sceneSeedTrackers = new List<GameManager.SceneSeedTracker>();
		}
		GameManager.SceneSeedTracker sceneSeedTracker = null;
		for (int i = this.sceneSeedTrackers.Count - 1; i >= 0; i--)
		{
			GameManager.SceneSeedTracker sceneSeedTracker2 = this.sceneSeedTrackers[i];
			sceneSeedTracker2.TransitionsLeft--;
			if (sceneSeedTracker2.TransitionsLeft <= 0)
			{
				this.sceneSeedTrackers.RemoveAt(i);
			}
			else if (sceneSeedTracker2.SceneNameHash == this.sceneNameHash)
			{
				sceneSeedTracker = sceneSeedTracker2;
			}
		}
		if (sceneSeedTracker == null)
		{
			sceneSeedTracker = new GameManager.SceneSeedTracker
			{
				SceneNameHash = this.sceneNameHash,
				Seed = Random.Range(0, 99999),
				TransitionsLeft = 3
			};
			this.sceneSeedTrackers.Add(sceneSeedTracker);
		}
		else
		{
			sceneSeedTracker.TransitionsLeft = 3;
		}
		this.SceneSeededRandom = new Random(sceneSeedTracker.Seed);
	}

	// Token: 0x0600246F RID: 9327 RVA: 0x000A8284 File Offset: 0x000A6484
	public void SetupHeroRefs()
	{
		this.hero_ctrl = HeroController.instance;
		if (this.hero_ctrl)
		{
			this.heroLight = this.hero_ctrl.heroLight.GetComponent<SpriteRenderer>();
		}
	}

	// Token: 0x06002470 RID: 9328 RVA: 0x000A82B4 File Offset: 0x000A64B4
	public void BeginScene()
	{
		ObjectPool.PurgeRecentRecycled();
		this.timeInScene = 0f;
		this.inputHandler.SceneInit();
		this.ui.SceneInit();
		bool flag = this.IsMenuScene();
		if (!flag)
		{
			this.SetupHeroRefs();
			if (this.hero_ctrl)
			{
				this.hero_ctrl.SceneInit();
			}
		}
		this.gameCams.SceneInit();
		if (!flag && this.SceneInit != null)
		{
			this.SceneInit();
		}
		if (flag)
		{
			this.SetState(GameState.MAIN_MENU);
			this.UpdateUIStateFromGameState();
			return;
		}
		if (this.IsGameplayScene())
		{
			if ((!Application.isEditor && !Debug.isDebugBuild) || Time.renderedFrameCount > 3)
			{
				this.PositionHeroAtSceneEntrance();
				return;
			}
		}
		else
		{
			if (this.IsNonGameplayScene())
			{
				this.SetState(GameState.CUTSCENE);
				this.UpdateUIStateFromGameState();
				return;
			}
			this.UpdateUIStateFromGameState();
		}
	}

	// Token: 0x06002471 RID: 9329 RVA: 0x000A8388 File Offset: 0x000A6588
	private void UpdateUIStateFromGameState()
	{
		if (this.ui != null)
		{
			this.ui.SetUIStartState(this.GameState);
			return;
		}
		this.ui = Object.FindObjectOfType<UIManager>();
		if (this.ui != null)
		{
			this.ui.SetUIStartState(this.GameState);
		}
	}

	// Token: 0x06002472 RID: 9330 RVA: 0x000A83DF File Offset: 0x000A65DF
	public void SkipCutscene()
	{
		base.StartCoroutine(this.SkipCutsceneNoMash());
	}

	// Token: 0x06002473 RID: 9331 RVA: 0x000A83EE File Offset: 0x000A65EE
	public void RegisterSkippable(GameManager.ISkippable skippable)
	{
		if (this.skippables == null)
		{
			this.skippables = new List<GameManager.ISkippable>();
		}
		this.skippables.Add(skippable);
	}

	// Token: 0x06002474 RID: 9332 RVA: 0x000A840F File Offset: 0x000A660F
	public void DeregisterSkippable(GameManager.ISkippable skippable)
	{
		this.skippables.Remove(skippable);
	}

	// Token: 0x06002475 RID: 9333 RVA: 0x000A841E File Offset: 0x000A661E
	private IEnumerator SkipCutsceneNoMash()
	{
		if (this.GameState != GameState.CUTSCENE)
		{
			yield break;
		}
		this.ui.HideCutscenePrompt(true, null);
		if (this.skippables != null)
		{
			using (List<GameManager.ISkippable>.Enumerator enumerator = this.skippables.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					GameManager.ISkippable skippable = enumerator.Current;
					yield return base.StartCoroutine(skippable.Skip());
					this.inputHandler.skippingCutscene = false;
					yield break;
				}
			}
			List<GameManager.ISkippable>.Enumerator enumerator = default(List<GameManager.ISkippable>.Enumerator);
		}
		EventRegister.SendEvent(EventRegisterEvents.CustomCutsceneSkip, null);
		this.inputHandler.skippingCutscene = false;
		yield break;
		yield break;
	}

	// Token: 0x06002476 RID: 9334 RVA: 0x000A842D File Offset: 0x000A662D
	public void NoLongerFirstGame()
	{
		if (this.playerData.isFirstGame)
		{
			this.playerData.isFirstGame = false;
		}
		this.IsFirstLevelForPlayer = false;
	}

	// Token: 0x06002477 RID: 9335 RVA: 0x000A8450 File Offset: 0x000A6650
	private void SetupStatusModifiers()
	{
		if (this.gameConfig.clearRecordsOnStart)
		{
			this.ResetStatusRecords();
		}
		if (this.gameConfig.unlockPermadeathMode)
		{
			this.SetStatusRecordInt("RecPermadeathMode", 1);
		}
		if (this.gameConfig.unlockBossRushMode)
		{
			this.SetStatusRecordInt("RecBossRushMode", 1);
		}
		if (this.gameConfig.clearPreferredLanguageSetting)
		{
			Platform.Current.LocalSharedData.DeleteKey("GameLangSet");
		}
		if (this.gameSettings.CommandArgumentUsed("-forcelang"))
		{
			Debug.Log("== Language option forced on by command argument.");
			this.gameConfig.hideLanguageOption = true;
		}
	}

	// Token: 0x06002478 RID: 9336 RVA: 0x000A84EB File Offset: 0x000A66EB
	public void RefreshLocalization()
	{
		Action refreshLanguageText = this.RefreshLanguageText;
		if (refreshLanguageText == null)
		{
			return;
		}
		refreshLanguageText();
	}

	// Token: 0x06002479 RID: 9337 RVA: 0x000A84FD File Offset: 0x000A66FD
	public void RefreshParticleSystems()
	{
		Action refreshParticleLevel = this.RefreshParticleLevel;
		if (refreshParticleLevel == null)
		{
			return;
		}
		refreshParticleLevel();
	}

	// Token: 0x0600247A RID: 9338 RVA: 0x000A850F File Offset: 0x000A670F
	public void ApplyNativeInput()
	{
	}

	// Token: 0x0600247B RID: 9339 RVA: 0x000A8511 File Offset: 0x000A6711
	public void EnablePermadeathMode()
	{
		this.SetStatusRecordInt("RecPermadeathMode", 1);
	}

	// Token: 0x0600247C RID: 9340 RVA: 0x000A8520 File Offset: 0x000A6720
	public string GetCurrentMapZone()
	{
		if (this.mapZoneStringVersion == CustomSceneManager.Version)
		{
			return this.mapZoneString;
		}
		this.mapZoneStringVersion = CustomSceneManager.Version;
		this.mapZoneString = this.GetCurrentMapZoneEnum().ToString();
		return this.mapZoneString;
	}

	// Token: 0x0600247D RID: 9341 RVA: 0x000A856C File Offset: 0x000A676C
	public MapZone GetCurrentMapZoneEnum()
	{
		if (this.mapZoneVersion == CustomSceneManager.Version)
		{
			return this.currentMapZone;
		}
		if (!this.sm)
		{
			this.FindSceneManager();
			if (!this.sm)
			{
				this.mapZoneVersion = CustomSceneManager.Version;
				this.currentMapZone = MapZone.NONE;
				return MapZone.NONE;
			}
		}
		this.mapZoneVersion = CustomSceneManager.Version;
		this.currentMapZone = this.sm.mapZone;
		return this.currentMapZone;
	}

	// Token: 0x0600247E RID: 9342 RVA: 0x000A85E3 File Offset: 0x000A67E3
	public float GetSceneWidth()
	{
		if (this.IsGameplayScene())
		{
			return this.sceneWidth;
		}
		return 0f;
	}

	// Token: 0x0600247F RID: 9343 RVA: 0x000A85F9 File Offset: 0x000A67F9
	public float GetSceneHeight()
	{
		if (this.IsGameplayScene())
		{
			return this.sceneHeight;
		}
		return 0f;
	}

	// Token: 0x06002480 RID: 9344 RVA: 0x000A860F File Offset: 0x000A680F
	public GameObject GetSceneManager()
	{
		if (!this.sm)
		{
			this.FindSceneManager();
			if (!this.sm)
			{
				return null;
			}
		}
		return this.sm.gameObject;
	}

	// Token: 0x06002481 RID: 9345 RVA: 0x000A863E File Offset: 0x000A683E
	public string GetFormattedMapZoneString(MapZone mapZone)
	{
		return Language.Get(mapZone.ToString(), "Map Zones");
	}

	// Token: 0x06002482 RID: 9346 RVA: 0x000A8657 File Offset: 0x000A6857
	public static string GetFormattedMapZoneStringV2(MapZone mapZone)
	{
		return Language.Get(mapZone.ToString(), "Map Zones");
	}

	// Token: 0x06002483 RID: 9347 RVA: 0x000A8670 File Offset: 0x000A6870
	public static string GetFormattedAutoSaveNameString(AutoSaveName autoSaveName)
	{
		return Language.Get(autoSaveName.ToString(), "AutoSaveNames");
	}

	// Token: 0x06002484 RID: 9348 RVA: 0x000A868C File Offset: 0x000A688C
	public void UpdateSceneName()
	{
		string b = this.sceneName;
		string name = SceneManager.GetActiveScene().name;
		if (name == this.rawSceneName)
		{
			return;
		}
		this.rawSceneName = name;
		this.sceneName = GameManager.GetBaseSceneName(name);
		this.sceneNameHash = this.sceneName.GetHashCode();
		if (this.sceneName != b)
		{
			this.lastSceneName = b;
		}
	}

	// Token: 0x06002485 RID: 9349 RVA: 0x000A86F6 File Offset: 0x000A68F6
	public static string GetBaseSceneName(string fullSceneName)
	{
		if (fullSceneName == GameManager.lastFullSceneName)
		{
			return GameManager.fixedSceneName;
		}
		GameManager.lastFullSceneName = fullSceneName;
		GameManager.fixedSceneName = GameManager.InternalBaseSceneName(fullSceneName);
		return GameManager.fixedSceneName;
	}

	// Token: 0x06002486 RID: 9350 RVA: 0x000A8724 File Offset: 0x000A6924
	private static string InternalBaseSceneName(string fullSceneName)
	{
		if (string.IsNullOrEmpty(fullSceneName))
		{
			return string.Empty;
		}
		foreach (string text in WorldInfo.SubSceneNameSuffixes)
		{
			if (fullSceneName.EndsWith(text, StringComparison.InvariantCultureIgnoreCase))
			{
				int length = text.Length;
				return fullSceneName.Substring(0, fullSceneName.Length - length);
			}
		}
		return fullSceneName;
	}

	// Token: 0x06002487 RID: 9351 RVA: 0x000A877D File Offset: 0x000A697D
	public string GetSceneNameString()
	{
		this.UpdateSceneName();
		return this.sceneName;
	}

	// Token: 0x06002488 RID: 9352 RVA: 0x000A878B File Offset: 0x000A698B
	private static tk2dTileMap GetTileMap(GameObject gameObject)
	{
		if (gameObject.CompareTag("TileMap"))
		{
			return gameObject.GetComponent<tk2dTileMap>();
		}
		return null;
	}

	// Token: 0x06002489 RID: 9353 RVA: 0x000A87A4 File Offset: 0x000A69A4
	public void RefreshTilemapInfo(string targetScene)
	{
		tk2dTileMap tk2dTileMap = null;
		int num = 0;
		while (tk2dTileMap == null && num < SceneManager.sceneCount)
		{
			Scene sceneAt = SceneManager.GetSceneAt(num);
			if (string.IsNullOrEmpty(targetScene) || !(sceneAt.name != targetScene))
			{
				GameObject[] rootGameObjects = sceneAt.GetRootGameObjects();
				int num2 = 0;
				while (tk2dTileMap == null && num2 < rootGameObjects.Length)
				{
					tk2dTileMap = GameManager.GetTileMap(rootGameObjects[num2]);
					num2++;
				}
			}
			num++;
		}
		if (tk2dTileMap == null)
		{
			Debug.LogWarningFormat("Using fallback 1 to find tilemap. Scene {0} requires manual fixing.", new object[]
			{
				targetScene
			});
			GameObject[] array = GameObject.FindGameObjectsWithTag("TileMap");
			int num3 = 0;
			while (tk2dTileMap == null && num3 < array.Length)
			{
				GameObject gameObject = array[num3];
				if (string.IsNullOrEmpty(targetScene) || !(gameObject.scene.name != targetScene))
				{
					tk2dTileMap = gameObject.GetComponent<tk2dTileMap>();
				}
				num3++;
			}
		}
		if (tk2dTileMap == null)
		{
			Debug.LogErrorFormat("Failed to find tilemap in " + targetScene + " entirely.", Array.Empty<object>());
			return;
		}
		this.tilemap = tk2dTileMap;
		this.sceneWidth = (float)this.tilemap.width;
		this.sceneHeight = (float)this.tilemap.height;
	}

	// Token: 0x0600248A RID: 9354 RVA: 0x000A88DA File Offset: 0x000A6ADA
	public void SaveLevelState()
	{
		Action savePersistentObjects = this.SavePersistentObjects;
		if (savePersistentObjects == null)
		{
			return;
		}
		savePersistentObjects();
	}

	// Token: 0x0600248B RID: 9355 RVA: 0x000A88EC File Offset: 0x000A6AEC
	public void ResetSemiPersistentItems()
	{
		Action resetSemiPersistentObjects = this.ResetSemiPersistentObjects;
		if (resetSemiPersistentObjects != null)
		{
			resetSemiPersistentObjects();
		}
		this.sceneData.ResetSemiPersistentItems();
	}

	// Token: 0x0600248C RID: 9356 RVA: 0x000A890A File Offset: 0x000A6B0A
	public bool IsMenuScene()
	{
		this.UpdateSceneName();
		return this.sceneName == "Menu_Title";
	}

	// Token: 0x0600248D RID: 9357 RVA: 0x000A8927 File Offset: 0x000A6B27
	public bool IsTitleScreenScene()
	{
		this.UpdateSceneName();
		return string.Compare(this.sceneName, "Title_Screens", true) == 0;
	}

	// Token: 0x0600248E RID: 9358 RVA: 0x000A8945 File Offset: 0x000A6B45
	public bool IsGameplayScene()
	{
		this.UpdateSceneName();
		return !this.IsNonGameplayScene();
	}

	// Token: 0x0600248F RID: 9359 RVA: 0x000A8958 File Offset: 0x000A6B58
	public bool IsNonGameplayScene()
	{
		this.UpdateSceneName();
		if (this.IsMenuScene())
		{
			return true;
		}
		if (this.IsCinematicScene())
		{
			return true;
		}
		string text = this.sceneName;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 2300701789U)
		{
			if (num <= 889398844U)
			{
				if (num != 870319515U)
				{
					if (num != 888263840U)
					{
						if (num != 889398844U)
						{
							goto IL_21F;
						}
						if (!(text == "Cutscene_Boss_Door"))
						{
							goto IL_21F;
						}
					}
					else if (!(text == "GG_Unlock"))
					{
						goto IL_21F;
					}
				}
				else if (!(text == "GG_End_Sequence"))
				{
					goto IL_21F;
				}
			}
			else if (num <= 2063976584U)
			{
				if (num != 953782380U)
				{
					if (num != 2063976584U)
					{
						goto IL_21F;
					}
					if (!(text == "Knight Pickup"))
					{
						goto IL_21F;
					}
				}
				else if (!(text == "End_Game_Completion"))
				{
					goto IL_21F;
				}
			}
			else if (num != 2108561388U)
			{
				if (num != 2300701789U)
				{
					goto IL_21F;
				}
				if (!(text == "GG_Boss_Door_Entrance"))
				{
					goto IL_21F;
				}
			}
			else if (!(text == "Quit_To_Menu"))
			{
				goto IL_21F;
			}
		}
		else if (num <= 2782808483U)
		{
			if (num <= 2452993033U)
			{
				if (num != 2367494732U)
				{
					if (num != 2452993033U)
					{
						goto IL_21F;
					}
					if (!(text == "PermaDeath_Unlock"))
					{
						goto IL_21F;
					}
				}
				else if (!(text == "PermaDeath"))
				{
					goto IL_21F;
				}
			}
			else if (num != 2582472295U)
			{
				if (num != 2782808483U)
				{
					goto IL_21F;
				}
				if (!(text == "GG_Entrance_Cutscene"))
				{
					goto IL_21F;
				}
			}
			else if (!(text == "End_Credits"))
			{
				goto IL_21F;
			}
		}
		else if (num <= 3095434068U)
		{
			if (num != 2976727477U)
			{
				if (num != 3095434068U)
				{
					goto IL_21F;
				}
				if (!(text == "BetaEnd"))
				{
					goto IL_21F;
				}
			}
			else if (!(text == "Pre_Menu_Intro"))
			{
				goto IL_21F;
			}
		}
		else if (num != 3364296698U)
		{
			if (num != 3530372151U)
			{
				goto IL_21F;
			}
			if (!(text == "Menu_Title"))
			{
				goto IL_21F;
			}
		}
		else if (!(text == "Demo Start"))
		{
			goto IL_21F;
		}
		return true;
		IL_21F:
		return InGameCutsceneInfo.IsInCutscene;
	}

	// Token: 0x06002490 RID: 9360 RVA: 0x000A8B8C File Offset: 0x000A6D8C
	public bool IsCinematicScene()
	{
		this.UpdateSceneName();
		string text = this.sceneName;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 1678151941U)
		{
			if (num <= 1519761440U)
			{
				if (num <= 906312887U)
				{
					if (num != 234124091U)
					{
						if (num != 906312887U)
						{
							return false;
						}
						if (!(text == "Menu_Credits"))
						{
							return false;
						}
					}
					else if (!(text == "Opening_Sequence_Act3"))
					{
						return false;
					}
				}
				else if (num != 1281130730U)
				{
					if (num != 1519761440U)
					{
						return false;
					}
					if (!(text == "Cinematic_Ending_E"))
					{
						return false;
					}
				}
				else if (!(text == "Intro_Cutscene_Prologue"))
				{
					return false;
				}
			}
			else if (num <= 1586871916U)
			{
				if (num != 1536539059U)
				{
					if (num != 1586871916U)
					{
						return false;
					}
					if (!(text == "Cinematic_Ending_A"))
					{
						return false;
					}
				}
				else if (!(text == "Cinematic_Ending_D"))
				{
					return false;
				}
			}
			else if (num != 1620427154U)
			{
				if (num != 1637204773U)
				{
					if (num != 1678151941U)
					{
						return false;
					}
					if (!(text == "Opening_Sequence"))
					{
						return false;
					}
				}
				else if (!(text == "Cinematic_Ending_B"))
				{
					return false;
				}
			}
			else if (!(text == "Cinematic_Ending_C"))
			{
				return false;
			}
		}
		else if (num <= 2977801185U)
		{
			if (num <= 2097224486U)
			{
				if (num != 1963908850U)
				{
					if (num != 2097224486U)
					{
						return false;
					}
					if (!(text == "Cinematic_Submarine_travel"))
					{
						return false;
					}
				}
				else if (!(text == "Cinematic_MrMushroom"))
				{
					return false;
				}
			}
			else if (num != 2367494732U)
			{
				if (num != 2977801185U)
				{
					return false;
				}
				if (!(text == "Cinematic_Stag_travel"))
				{
					return false;
				}
			}
			else if (!(text == "PermaDeath"))
			{
				return false;
			}
		}
		else if (num <= 3032265966U)
		{
			if (num != 2981538746U)
			{
				if (num != 3032265966U)
				{
					return false;
				}
				if (!(text == "Prologue_Excerpt"))
				{
					return false;
				}
			}
			else if (!(text == "Intro_Cutscene"))
			{
				return false;
			}
		}
		else if (num != 3095434068U)
		{
			if (num != 3364296698U)
			{
				if (num != 4173540169U)
				{
					return false;
				}
				if (!(text == "End_Credits_Scroll"))
				{
					return false;
				}
			}
			else if (!(text == "Demo Start"))
			{
				return false;
			}
		}
		else if (!(text == "BetaEnd"))
		{
			return false;
		}
		return true;
	}

	// Token: 0x06002491 RID: 9361 RVA: 0x000A8E18 File Offset: 0x000A7018
	public bool IsStagTravelScene()
	{
		this.UpdateSceneName();
		return this.sceneName == "Cinematic_Stag_travel";
	}

	// Token: 0x06002492 RID: 9362 RVA: 0x000A8E30 File Offset: 0x000A7030
	public bool IsBossDoorScene()
	{
		this.UpdateSceneName();
		return this.sceneName == "Cutscene_Boss_Door";
	}

	// Token: 0x06002493 RID: 9363 RVA: 0x000A8E48 File Offset: 0x000A7048
	public bool ShouldKeepHUDCameraActive()
	{
		this.UpdateSceneName();
		return this.sceneName == "GG_Entrance_Cutscene" || this.sceneName == "GG_Boss_Door_Entrance" || this.sceneName == "GG_End_Sequence" || InGameCutsceneInfo.IsInCutscene;
	}

	// Token: 0x06002494 RID: 9364 RVA: 0x000A8E98 File Offset: 0x000A7098
	public void HasSaveFile(int saveSlot, Action<bool> callback)
	{
		if (DemoHelper.IsDemoMode)
		{
			bool obj = DemoHelper.HasSaveFile(saveSlot);
			if (callback != null)
			{
				callback(obj);
			}
			return;
		}
		Platform.Current.IsSaveSlotInUse(saveSlot, callback);
	}

	// Token: 0x06002495 RID: 9365 RVA: 0x000A8ECA File Offset: 0x000A70CA
	public void SaveGame(Action<bool> callback)
	{
		if (this.isAutoSaveQueued)
		{
			this.SaveGameWithAutoSave(this.queuedAutoSaveName, callback);
			return;
		}
		this.FixUpSaveState();
		this.SaveGame(this.profileID, callback, false, AutoSaveName.NONE);
		this.isSaveGameQueued = false;
	}

	// Token: 0x06002496 RID: 9366 RVA: 0x000A8EFE File Offset: 0x000A70FE
	public void SaveGameWithAutoSave(AutoSaveName autoSaveName, Action<bool> callback)
	{
		this.FixUpSaveState();
		this.SaveGame(this.profileID, callback, true, autoSaveName);
		this.isSaveGameQueued = false;
		this.isAutoSaveQueued = false;
		this.queuedAutoSaveName = AutoSaveName.NONE;
	}

	// Token: 0x06002497 RID: 9367 RVA: 0x000A8F2A File Offset: 0x000A712A
	public void QueueSaveGame()
	{
		this.isSaveGameQueued = true;
	}

	// Token: 0x06002498 RID: 9368 RVA: 0x000A8F33 File Offset: 0x000A7133
	public void QueueAutoSave(AutoSaveName autoSaveName)
	{
		this.queuedAutoSaveName = autoSaveName;
		this.isAutoSaveQueued = true;
	}

	// Token: 0x06002499 RID: 9369 RVA: 0x000A8F43 File Offset: 0x000A7143
	public void DoQueuedSaveGame()
	{
		if (this.isSaveGameQueued)
		{
			this.SaveGame(null);
			return;
		}
		if (this.isAutoSaveQueued)
		{
			this.isAutoSaveQueued = false;
			this.CreateRestorePoint(this.queuedAutoSaveName, null);
		}
	}

	// Token: 0x0600249A RID: 9370 RVA: 0x000A8F71 File Offset: 0x000A7171
	public void CreateRestorePoint(AutoSaveName autoSaveName, Action<bool> callback = null)
	{
		this.FixUpSaveState();
		this.CreateRestorePoint(this.profileID, autoSaveName, callback);
	}

	// Token: 0x0600249B RID: 9371 RVA: 0x000A8F87 File Offset: 0x000A7187
	public SaveGameData CreateSaveGameData(int saveSlot)
	{
		this.PreparePlayerDataForSave(saveSlot);
		return new SaveGameData(this.playerData, this.sceneData);
	}

	// Token: 0x0600249C RID: 9372 RVA: 0x000A8FA4 File Offset: 0x000A71A4
	public void CreateRestorePoint(int saveSlot, AutoSaveName autoSaveName, Action<bool> callback = null)
	{
		if (CheatManager.AllowSaving)
		{
			if (saveSlot >= 0)
			{
				this.SaveLevelState();
				this.isAutoSaveQueued = false;
				if (!this.gameConfig.disableSaveGame)
				{
					if (DemoHelper.IsDemoMode)
					{
						Action<bool> callback2 = callback;
						if (callback2 == null)
						{
							return;
						}
						callback2(true);
						return;
					}
					else
					{
						try
						{
							SaveGameData saveData = this.CreateSaveGameData(saveSlot);
							Action <>9__3;
							SaveDataUtility.AddTaskToAsyncQueue(delegate()
							{
								try
								{
									saveData.playerData.UpdateDate();
									this.DoSaveRestorePoint(saveSlot, autoSaveName, saveData, callback);
								}
								catch (Exception)
								{
									if (callback != null)
									{
										Action action;
										if ((action = <>9__3) == null)
										{
											action = (<>9__3 = delegate()
											{
												callback(false);
											});
										}
										CoreLoop.InvokeSafe(action);
									}
								}
							});
							return;
						}
						catch (Exception)
						{
							if (callback != null)
							{
								CoreLoop.InvokeSafe(delegate
								{
									callback(false);
								});
							}
							return;
						}
					}
				}
				if (callback != null)
				{
					CoreLoop.InvokeNext(delegate
					{
						callback(false);
					});
					return;
				}
			}
			else if (callback != null)
			{
				CoreLoop.InvokeNext(delegate
				{
					callback(false);
				});
			}
			return;
		}
		Action<bool> callback3 = callback;
		if (callback3 == null)
		{
			return;
		}
		callback3(true);
	}

	// Token: 0x0600249D RID: 9373 RVA: 0x000A90C4 File Offset: 0x000A72C4
	private void ShowSaveIcon()
	{
		UIManager instance = UIManager.instance;
		if (instance != null && this.saveIconShowCounter == 0)
		{
			CheckpointSprite checkpointSprite = instance.checkpointSprite;
			if (checkpointSprite != null)
			{
				checkpointSprite.Show();
			}
		}
		this.saveIconShowCounter++;
	}

	// Token: 0x0600249E RID: 9374 RVA: 0x000A910C File Offset: 0x000A730C
	private void HideSaveIcon()
	{
		this.saveIconShowCounter--;
		UIManager instance = UIManager.instance;
		if (instance != null && this.saveIconShowCounter == 0)
		{
			CheckpointSprite checkpointSprite = instance.checkpointSprite;
			if (checkpointSprite != null)
			{
				checkpointSprite.Hide();
			}
		}
	}

	// Token: 0x0600249F RID: 9375 RVA: 0x000A9154 File Offset: 0x000A7354
	private void ResetGameTimer()
	{
		this.sessionPlayTimer = 0f;
		this.sessionStartTime = this.playerData.playTime;
	}

	// Token: 0x060024A0 RID: 9376 RVA: 0x000A9174 File Offset: 0x000A7374
	public void IncreaseGameTimer(ref float timer)
	{
		if (this.GameState != GameState.PLAYING && this.GameState != GameState.ENTERING_LEVEL && this.GameState != GameState.EXITING_LEVEL)
		{
			return;
		}
		if (PlayerData.instance.isInventoryOpen)
		{
			return;
		}
		if (Time.unscaledDeltaTime < 1f)
		{
			timer += Time.unscaledDeltaTime;
		}
	}

	// Token: 0x060024A1 RID: 9377 RVA: 0x000A91C0 File Offset: 0x000A73C0
	public SaveGameData GetSaveGameData(int saveSlot)
	{
		this.FixUpSaveState();
		if (this.achievementHandler != null)
		{
			this.achievementHandler.FlushRecordsToDisk();
		}
		else
		{
			Debug.LogError("Error saving achievements (PlayerAchievements is null)");
		}
		if (this.playerData != null)
		{
			this.playerData.playTime += this.sessionPlayTimer;
			this.ResetGameTimer();
			this.playerData.version = "1.0.28324";
			this.playerData.RevisionBreak = 28104;
			this.playerData.profileID = saveSlot;
			this.playerData.CountGameCompletion();
			this.playerData.OnBeforeSave();
		}
		return new SaveGameData(this.playerData, this.sceneData);
	}

	// Token: 0x060024A2 RID: 9378 RVA: 0x000A9274 File Offset: 0x000A7474
	private void SaveGame(int saveSlot, Action<bool> ogCallback, bool withAutoSave = false, AutoSaveName autoSaveName = AutoSaveName.NONE)
	{
		GameManager.<>c__DisplayClass432_0 CS$<>8__locals1 = new GameManager.<>c__DisplayClass432_0();
		CS$<>8__locals1.ogCallback = ogCallback;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.saveSlot = saveSlot;
		CS$<>8__locals1.withAutoSave = withAutoSave;
		CS$<>8__locals1.autoSaveName = autoSaveName;
		CS$<>8__locals1.callback = delegate(bool didSave, string errorInfo)
		{
			if (!didSave)
			{
				CheatManager.LastErrorText = errorInfo;
				string errorKey = "SAVE_FAILED";
				Action onOk;
				if ((onOk = CS$<>8__locals1.<>9__3) == null)
				{
					onOk = (CS$<>8__locals1.<>9__3 = delegate()
					{
						Action<bool> ogCallback3 = CS$<>8__locals1.ogCallback;
						if (ogCallback3 == null)
						{
							return;
						}
						ogCallback3(false);
					});
				}
				GenericMessageCanvas.Show(errorKey, onOk);
				return;
			}
			Action<bool> ogCallback2 = CS$<>8__locals1.ogCallback;
			if (ogCallback2 == null)
			{
				return;
			}
			ogCallback2(true);
		};
		if (!CheatManager.AllowSaving)
		{
			CS$<>8__locals1.callback(true, null);
			return;
		}
		if (CS$<>8__locals1.saveSlot < 0)
		{
			CoreLoop.InvokeNext(delegate
			{
				CS$<>8__locals1.callback(false, string.Format("Save game slot not valid: {0}", CS$<>8__locals1.saveSlot));
			});
			return;
		}
		this.SaveLevelState();
		if (this.gameConfig.disableSaveGame)
		{
			CoreLoop.InvokeNext(delegate
			{
				CS$<>8__locals1.callback(false, "Saving game disabled. No save file written.");
			});
			return;
		}
		if (DemoHelper.IsDemoMode)
		{
			CS$<>8__locals1.callback(true, null);
			return;
		}
		this.ShowSaveIcon();
		this.PreparePlayerDataForSave(CS$<>8__locals1.saveSlot);
		SaveGameData saveData = new SaveGameData(this.playerData, this.sceneData);
		Action <>9__10;
		Action<byte[]> <>9__7;
		SaveDataUtility.AddTaskToAsyncQueue(delegate(TaskCompletionSource<string> tcs)
		{
			saveData.playerData.UpdateDate();
			string result = SaveDataUtility.SerializeSaveData<SaveGameData>(saveData);
			tcs.SetResult(result);
		}, delegate(bool success, string result)
		{
			if (!success)
			{
				CS$<>8__locals1.<>4__this.HideSaveIcon();
				CoreLoop.InvokeNext(delegate
				{
					CS$<>8__locals1.callback(false, result);
				});
				return;
			}
			try
			{
				GameManager <>4__this = CS$<>8__locals1.<>4__this;
				string result2 = result;
				Action<byte[]> callback;
				if ((callback = <>9__7) == null)
				{
					callback = (<>9__7 = delegate(byte[] fileBytes)
					{
						Platform platform = Platform.Current;
						int saveSlot2 = CS$<>8__locals1.saveSlot;
						Action<bool> callback2;
						if ((callback2 = CS$<>8__locals1.<>9__8) == null)
						{
							callback2 = (CS$<>8__locals1.<>9__8 = delegate(bool didSave)
							{
								CS$<>8__locals1.<>4__this.HideSaveIcon();
								CS$<>8__locals1.callback(didSave, null);
							});
						}
						platform.WriteSaveSlot(saveSlot2, fileBytes, callback2);
						try
						{
							CS$<>8__locals1.<>4__this.ShowSaveIcon();
							Platform platform2 = Platform.Current;
							int saveSlot3 = CS$<>8__locals1.saveSlot;
							Action<bool> callback3;
							if ((callback3 = CS$<>8__locals1.<>9__9) == null)
							{
								callback3 = (CS$<>8__locals1.<>9__9 = delegate(bool _)
								{
									CS$<>8__locals1.<>4__this.HideSaveIcon();
								});
							}
							platform2.WriteSaveBackup(saveSlot3, fileBytes, callback3);
						}
						catch (Exception)
						{
							CS$<>8__locals1.<>4__this.HideSaveIcon();
						}
						if (CS$<>8__locals1.withAutoSave)
						{
							try
							{
								CS$<>8__locals1.<>4__this.ShowSaveIcon();
								Action task;
								if ((task = <>9__10) == null)
								{
									task = (<>9__10 = delegate()
									{
										GameManager <>4__this2 = CS$<>8__locals1.<>4__this;
										int saveSlot4 = CS$<>8__locals1.saveSlot;
										AutoSaveName autoSaveName2 = CS$<>8__locals1.autoSaveName;
										SaveGameData saveData = saveData;
										Action<bool> callback4;
										if ((callback4 = CS$<>8__locals1.<>9__11) == null)
										{
											callback4 = (CS$<>8__locals1.<>9__11 = delegate(bool _)
											{
												CS$<>8__locals1.<>4__this.HideSaveIcon();
											});
										}
										<>4__this2.DoSaveRestorePoint(saveSlot4, autoSaveName2, saveData, callback4);
									});
								}
								SaveDataUtility.AddTaskToAsyncQueue(task);
							}
							catch (Exception)
							{
								CS$<>8__locals1.<>4__this.HideSaveIcon();
							}
						}
					});
				}
				<>4__this.GetBytesForSaveJsonAsync(result2, callback);
			}
			catch (Exception ex)
			{
				Exception e2 = ex;
				Exception e = e2;
				CS$<>8__locals1.<>4__this.HideSaveIcon();
				CoreLoop.InvokeNext(delegate
				{
					CS$<>8__locals1.callback(false, e.ToString());
				});
			}
		});
	}

	// Token: 0x060024A3 RID: 9379 RVA: 0x000A9384 File Offset: 0x000A7584
	public void SaveGameData(int saveSlot, SaveGameData saveData, bool showSaveIcon, Action<bool> ogCallback)
	{
		Action <>9__3;
		Action<bool, string> callback = delegate(bool didSave, string errorInfo)
		{
			if (!didSave)
			{
				CheatManager.LastErrorText = errorInfo;
				string errorKey = "SAVE_FAILED";
				Action onOk;
				if ((onOk = <>9__3) == null)
				{
					onOk = (<>9__3 = delegate()
					{
						Action<bool> ogCallback3 = ogCallback;
						if (ogCallback3 == null)
						{
							return;
						}
						ogCallback3(false);
					});
				}
				GenericMessageCanvas.Show(errorKey, onOk);
				return;
			}
			Action<bool> ogCallback2 = ogCallback;
			if (ogCallback2 == null)
			{
				return;
			}
			ogCallback2(true);
		};
		if (showSaveIcon)
		{
			this.ShowSaveIcon();
		}
		Action<bool> <>9__6;
		Action<bool> <>9__7;
		Action<byte[]> <>9__5;
		SaveDataUtility.AddTaskToAsyncQueue(delegate(TaskCompletionSource<string> tcs)
		{
			saveData.playerData.UpdateDate();
			string result = SaveDataUtility.SerializeSaveData<SaveGameData>(saveData);
			tcs.SetResult(result);
		}, delegate(bool success, string result)
		{
			Action<byte[]> callback;
			if (!success)
			{
				if (showSaveIcon)
				{
					this.HideSaveIcon();
				}
				CoreLoop.InvokeNext(delegate
				{
					callback(false, result);
				});
				return;
			}
			try
			{
				GameManager <>4__this = this;
				string result2 = result;
				if ((callback = <>9__5) == null)
				{
					callback = (<>9__5 = delegate(byte[] fileBytes)
					{
						Platform platform = Platform.Current;
						int saveSlot2 = saveSlot;
						Action<bool> callback2;
						if ((callback2 = <>9__6) == null)
						{
							callback2 = (<>9__6 = delegate(bool didSave)
							{
								if (showSaveIcon)
								{
									this.HideSaveIcon();
								}
								callback(didSave, null);
							});
						}
						platform.WriteSaveSlot(saveSlot2, fileBytes, callback2);
						try
						{
							if (showSaveIcon)
							{
								this.ShowSaveIcon();
							}
							Platform platform2 = Platform.Current;
							int saveSlot3 = saveSlot;
							Action<bool> callback3;
							if ((callback3 = <>9__7) == null)
							{
								callback3 = (<>9__7 = delegate(bool _)
								{
									this.HideSaveIcon();
								});
							}
							platform2.WriteSaveBackup(saveSlot3, fileBytes, callback3);
						}
						catch (Exception)
						{
							if (showSaveIcon)
							{
								this.HideSaveIcon();
							}
						}
					});
				}
				<>4__this.GetBytesForSaveJsonAsync(result2, callback);
			}
			catch (Exception ex)
			{
				Exception e2 = ex;
				Exception e = e2;
				if (showSaveIcon)
				{
					this.HideSaveIcon();
				}
				CoreLoop.InvokeNext(delegate
				{
					callback(false, e.ToString());
				});
			}
		});
	}

	// Token: 0x060024A4 RID: 9380 RVA: 0x000A93F8 File Offset: 0x000A75F8
	private void DoSaveRestorePoint(int saveSlot, AutoSaveName autoSaveName, SaveGameData saveData, Action<bool> callback)
	{
		RestorePointData restorePointData = new RestorePointData(saveData, autoSaveName);
		restorePointData.SetVersion();
		restorePointData.SetDateString();
		string jsonData = SaveDataUtility.SerializeSaveData<RestorePointData>(restorePointData);
		byte[] bytesForSaveJson = this.GetBytesForSaveJson(jsonData);
		bool noTrim = autoSaveName == AutoSaveName.ACT_1 || autoSaveName == AutoSaveName.ACT_2 || autoSaveName == AutoSaveName.ACT_3;
		Platform.Current.CreateSaveRestorePoint(saveSlot, autoSaveName.ToString(), noTrim, bytesForSaveJson, callback);
	}

	// Token: 0x060024A5 RID: 9381 RVA: 0x000A9454 File Offset: 0x000A7654
	private void PreparePlayerDataForSave(int saveSlot)
	{
		if (this.achievementHandler != null)
		{
			this.achievementHandler.FlushRecordsToDisk();
		}
		if (this.playerData != null)
		{
			this.playerData.playTime += this.sessionPlayTimer;
			this.ResetGameTimer();
			this.playerData.version = "1.0.28324";
			this.playerData.RevisionBreak = 28104;
			this.playerData.profileID = saveSlot;
			this.playerData.CountGameCompletion();
			this.playerData.OnBeforeSave();
		}
	}

	// Token: 0x060024A6 RID: 9382 RVA: 0x000A94E2 File Offset: 0x000A76E2
	public void LoadGameFromUI(int saveSlot)
	{
		base.StartCoroutine(this.LoadGameFromUIRoutine(saveSlot));
	}

	// Token: 0x060024A7 RID: 9383 RVA: 0x000A94F2 File Offset: 0x000A76F2
	public void LoadGameFromUI(int saveSlot, SaveGameData saveGameData)
	{
		base.StartCoroutine(this.LoadGameFromUIRoutine(saveSlot, saveGameData));
	}

	// Token: 0x060024A8 RID: 9384 RVA: 0x000A9503 File Offset: 0x000A7703
	private IEnumerator LoadGameFromUIRoutine(int saveSlot)
	{
		this.ui.ContinueGame();
		bool finishedLoading = false;
		bool successfullyLoaded = false;
		this.LoadGame(saveSlot, delegate(bool didLoad)
		{
			finishedLoading = true;
			successfullyLoaded = didLoad;
		});
		while (!finishedLoading)
		{
			yield return null;
		}
		if (successfullyLoaded)
		{
			this.ContinueGame();
		}
		else
		{
			this.ui.UIGoToMainMenu();
		}
		yield break;
	}

	// Token: 0x060024A9 RID: 9385 RVA: 0x000A9519 File Offset: 0x000A7719
	private IEnumerator LoadGameFromUIRoutine(int saveSlot, SaveGameData saveGameData)
	{
		this.ui.ContinueGame();
		bool successfullyLoaded = false;
		if (saveGameData == null)
		{
			bool finishedLoading = false;
			this.LoadGame(saveSlot, delegate(bool didLoad)
			{
				finishedLoading = true;
				successfullyLoaded = didLoad;
			});
			while (!finishedLoading)
			{
				yield return null;
			}
		}
		else
		{
			this.SetLoadedGameData(saveGameData, saveSlot);
			successfullyLoaded = true;
		}
		if (successfullyLoaded)
		{
			this.ContinueGame();
		}
		else
		{
			this.ui.UIGoToMainMenu();
		}
		yield break;
	}

	// Token: 0x060024AA RID: 9386 RVA: 0x000A9538 File Offset: 0x000A7738
	public void LoadGame(int saveSlot, Action<bool> callback)
	{
		if (!Platform.IsSaveSlotIndexValid(saveSlot))
		{
			Debug.LogErrorFormat(string.Format("Cannot load from invalid save slot index {0}", saveSlot), Array.Empty<object>());
			if (callback != null)
			{
				CoreLoop.InvokeNext(delegate
				{
					callback(false);
				});
			}
			return;
		}
		if (!DemoHelper.IsDemoMode || saveSlot <= 0)
		{
			Platform.Current.ReadSaveSlot(saveSlot, delegate(byte[] fileBytes)
			{
				if (fileBytes == null)
				{
					Action<bool> callback3 = callback;
					if (callback3 == null)
					{
						return;
					}
					callback3(false);
					return;
				}
				else
				{
					if (!CheatManager.UseAsyncSaveFileLoad)
					{
						bool obj;
						try
						{
							string jsonForSaveBytes = this.GetJsonForSaveBytes(fileBytes);
							this.SetLoadedGameData(jsonForSaveBytes, saveSlot);
							obj = true;
						}
						catch (Exception arg)
						{
							Debug.LogFormat(string.Format("Error loading save file for slot {0}: {1}", saveSlot, arg), Array.Empty<object>());
							obj = false;
						}
						callback(obj);
						return;
					}
					SaveDataUtility.AddTaskToAsyncQueue(delegate()
					{
						bool success;
						try
						{
							string jsonForSaveBytes2 = this.GetJsonForSaveBytes(fileBytes);
							this.SetLoadedGameData(jsonForSaveBytes2, saveSlot);
							success = true;
						}
						catch (Exception arg2)
						{
							Debug.LogFormat(string.Format("Error loading save file for slot {0}: {1}", saveSlot, arg2), Array.Empty<object>());
							success = false;
						}
						if (callback != null)
						{
							CoreLoop.InvokeSafe(delegate
							{
								callback(success);
							});
						}
					});
					return;
				}
			});
			return;
		}
		string jsonData = null;
		bool flag = DemoHelper.TryGetSaveData(saveSlot - 1, out jsonData);
		if (flag)
		{
			this.SetLoadedGameData(jsonData, saveSlot);
		}
		Action<bool> callback2 = callback;
		if (callback2 == null)
		{
			return;
		}
		callback2(flag);
	}

	// Token: 0x060024AB RID: 9387 RVA: 0x000A9608 File Offset: 0x000A7808
	private void SetLoadedGameData(string jsonData, int saveSlot)
	{
		SaveGameData saveGameData = SaveDataUtility.DeserializeSaveData<SaveGameData>(jsonData);
		this.SetLoadedGameData(saveGameData, saveSlot);
	}

	// Token: 0x060024AC RID: 9388 RVA: 0x000A9624 File Offset: 0x000A7824
	private void SetLoadedGameData(SaveGameData saveGameData, int saveSlot)
	{
		PlayerData playerData = saveGameData.playerData;
		SceneData instance = saveGameData.sceneData;
		playerData.ResetNonSerializableFields();
		PlayerData.instance = playerData;
		this.playerData = playerData;
		SceneData.instance = instance;
		this.sceneData = instance;
		this.profileID = saveSlot;
		playerData.silk = 0;
		playerData.silkParts = 0;
		playerData.SetupExistingPlayerData();
		this.inputHandler.RefreshPlayerData();
		QuestManager.UpgradeQuests();
		if (Platform.Current)
		{
			Platform.Current.OnSetGameData(this.profileID);
		}
	}

	// Token: 0x060024AD RID: 9389 RVA: 0x000A96A8 File Offset: 0x000A78A8
	public void ClearSaveFile(int saveSlot, Action<bool> callback)
	{
		if (!Platform.IsSaveSlotIndexValid(saveSlot))
		{
			Debug.LogErrorFormat(string.Format("Cannot clear invalid save slot index {0}", saveSlot), Array.Empty<object>());
			if (callback != null)
			{
				CoreLoop.InvokeNext(delegate
				{
					callback(false);
				});
			}
			return;
		}
		Debug.LogFormat(string.Format("Save file {0} {1}", saveSlot, "clearing..."), Array.Empty<object>());
		if (!DemoHelper.IsDemoMode)
		{
			Platform.Current.ClearSaveSlot(saveSlot, delegate(bool didClear)
			{
				Debug.LogFormat(string.Format("Save file {0} {1}", saveSlot, didClear ? "cleared" : "failed to clear"), Array.Empty<object>());
				Action<bool> callback3 = callback;
				if (callback3 != null)
				{
					callback3(didClear);
				}
				Platform.Current.DeleteRestorePointsForSlot(saveSlot, delegate(bool success)
				{
				});
			});
			return;
		}
		Debug.LogFormat(string.Format("Save file {0} not cleared - cannot clear save files in demo mode!", saveSlot), Array.Empty<object>());
		Action<bool> callback2 = callback;
		if (callback2 == null)
		{
			return;
		}
		callback2(false);
	}

	// Token: 0x060024AE RID: 9390 RVA: 0x000A9788 File Offset: 0x000A7988
	public void GetSaveStatsForSlot(int saveSlot, Action<SaveStats, string> callback)
	{
		if (!Platform.IsSaveSlotIndexValid(saveSlot))
		{
			if (callback != null)
			{
				CoreLoop.InvokeNext(delegate
				{
					callback(null, string.Format("Cannot get save stats for invalid slot {0}", saveSlot));
				});
			}
			return;
		}
		if (DemoHelper.IsDemoMode && saveSlot > 0)
		{
			SaveStats saveStats;
			if (!Platform.Current.WillPreloadSaveFiles || CheatManager.UseAsyncSaveFileLoad)
			{
				SaveDataUtility.AddTaskToAsyncQueue(delegate()
				{
					string jsonData2;
					bool flag2 = DemoHelper.TryGetSaveData(saveSlot - 1, out jsonData2);
					SaveStats saveStats = null;
					if (flag2)
					{
						saveStats = this.GetLoadedSaveSlotData(jsonData2);
					}
					if (callback != null)
					{
						CoreLoop.InvokeSafe(delegate
						{
							callback(saveStats, null);
						});
					}
				});
				return;
			}
			string jsonData;
			bool flag = DemoHelper.TryGetSaveData(saveSlot - 1, out jsonData);
			saveStats = null;
			if (flag)
			{
				saveStats = this.GetLoadedSaveSlotData(jsonData);
			}
			if (callback != null)
			{
				CoreLoop.InvokeNext(delegate
				{
					callback(saveStats, null);
				});
				return;
			}
		}
		else
		{
			Action <>9__5;
			Platform.Current.ReadSaveSlot(saveSlot, delegate(byte[] fileBytes)
			{
				if (fileBytes == null)
				{
					if (callback != null)
					{
						Action action;
						if ((action = <>9__5) == null)
						{
							action = (<>9__5 = delegate()
							{
								callback(null, null);
							});
						}
						CoreLoop.InvokeNext(action);
					}
					return;
				}
				if (Platform.Current.WillPreloadSaveFiles && !CheatManager.UseAsyncSaveFileLoad)
				{
					try
					{
						string jsonForSaveBytes = this.GetJsonForSaveBytes(fileBytes);
						SaveStats saveStats = this.GetLoadedSaveSlotData(jsonForSaveBytes);
						if (callback != null)
						{
							CoreLoop.InvokeNext(delegate
							{
								callback(saveStats, null);
							});
						}
					}
					catch (Exception ex)
					{
						Exception e2 = ex;
						Exception e = e2;
						Debug.LogError(string.Format("Error while loading save file for slot {0} Exception: {1}", saveSlot, e));
						if (callback != null)
						{
							CoreLoop.InvokeNext(delegate
							{
								callback(null, e.ToString());
							});
						}
					}
					return;
				}
				if (CheatManager.UseTasksForJsonConversion)
				{
					Task.Run(delegate()
					{
						try
						{
							string jsonForSaveBytes2 = this.GetJsonForSaveBytes(fileBytes);
							SaveGameData saveGameData = SaveDataUtility.DeserializeSaveData<SaveGameData>(jsonForSaveBytes2);
							if (callback != null)
							{
								CoreLoop.InvokeSafe(delegate
								{
									SaveStats saveStatsFromData = GameManager.GetSaveStatsFromData(saveGameData);
									callback(saveStatsFromData, null);
								});
							}
						}
						catch (Exception ex2)
						{
							Exception e3 = ex2;
							Exception e = e3;
							if (callback != null)
							{
								CoreLoop.InvokeSafe(delegate
								{
									callback(null, e.ToString());
								});
							}
						}
					});
					return;
				}
				SaveDataUtility.AddTaskToAsyncQueue(delegate()
				{
					try
					{
						string jsonForSaveBytes2 = this.GetJsonForSaveBytes(fileBytes);
						SaveStats saveStats = this.GetLoadedSaveSlotData(jsonForSaveBytes2);
						if (callback != null)
						{
							CoreLoop.InvokeSafe(delegate
							{
								callback(saveStats, null);
							});
						}
					}
					catch (Exception ex2)
					{
						Exception e3 = ex2;
						Exception e = e3;
						if (callback != null)
						{
							CoreLoop.InvokeSafe(delegate
							{
								callback(null, e.ToString());
							});
						}
					}
				});
			});
		}
	}

	// Token: 0x060024AF RID: 9391 RVA: 0x000A9884 File Offset: 0x000A7A84
	public string GetJsonForSaveBytes(byte[] fileBytes)
	{
		string result;
		if (this.gameConfig.useSaveEncryption && !Platform.Current.IsFileSystemProtected)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream serializationStream = new MemoryStream(fileBytes);
			result = Encryption.Decrypt((string)binaryFormatter.Deserialize(serializationStream));
		}
		else
		{
			result = Encoding.UTF8.GetString(fileBytes);
		}
		return result;
	}

	// Token: 0x060024B0 RID: 9392 RVA: 0x000A98DC File Offset: 0x000A7ADC
	public static string GetJsonForSaveBytesStatic(byte[] fileBytes)
	{
		if (GameManager.instance)
		{
			return GameManager.instance.GetJsonForSaveBytes(fileBytes);
		}
		string result;
		if (!Platform.Current.IsFileSystemProtected)
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream serializationStream = new MemoryStream(fileBytes);
			result = Encryption.Decrypt((string)binaryFormatter.Deserialize(serializationStream));
		}
		else
		{
			result = Encoding.UTF8.GetString(fileBytes);
		}
		return result;
	}

	// Token: 0x060024B1 RID: 9393 RVA: 0x000A993C File Offset: 0x000A7B3C
	public byte[] GetBytesForSaveJson(string jsonData)
	{
		byte[] result;
		if (this.gameConfig.useSaveEncryption && !Platform.Current.IsFileSystemProtected)
		{
			string graph = Encryption.Encrypt(jsonData);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream();
			binaryFormatter.Serialize(memoryStream, graph);
			result = memoryStream.ToArray();
			memoryStream.Close();
		}
		else
		{
			result = Encoding.UTF8.GetBytes(jsonData);
		}
		return result;
	}

	// Token: 0x060024B2 RID: 9394 RVA: 0x000A999D File Offset: 0x000A7B9D
	public void GetBytesForSaveJsonAsync(string jsonData, Action<byte[]> callback)
	{
		SaveDataUtility.AddTaskToAsyncQueue(delegate()
		{
			byte[] bytes = this.GetBytesForSaveJson(jsonData);
			if (callback != null)
			{
				CoreLoop.InvokeSafe(delegate
				{
					callback(bytes);
				});
			}
		});
	}

	// Token: 0x060024B3 RID: 9395 RVA: 0x000A99CC File Offset: 0x000A7BCC
	public static byte[] GetBytesForSaveJsonStatic(string jsonData)
	{
		if (GameManager.instance)
		{
			return GameManager.instance.GetBytesForSaveJson(jsonData);
		}
		Debug.LogError("Missing Game Manager. Using fallback get bytes method.");
		byte[] result;
		if (!Platform.Current.IsFileSystemProtected)
		{
			string graph = Encryption.Encrypt(jsonData);
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			MemoryStream memoryStream = new MemoryStream();
			binaryFormatter.Serialize(memoryStream, graph);
			result = memoryStream.ToArray();
			memoryStream.Close();
		}
		else
		{
			result = Encoding.UTF8.GetBytes(jsonData);
		}
		return result;
	}

	// Token: 0x060024B4 RID: 9396 RVA: 0x000A9A3F File Offset: 0x000A7C3F
	public static string GetJson<T>(T dataClassInstance)
	{
		return SaveDataUtility.SerializeSaveData<T>(dataClassInstance);
	}

	// Token: 0x060024B5 RID: 9397 RVA: 0x000A9A47 File Offset: 0x000A7C47
	public static byte[] DataToBytes<T>(T dataClassInstance)
	{
		return GameManager.GetBytesForSaveJsonStatic(GameManager.GetJson<T>(dataClassInstance));
	}

	// Token: 0x060024B6 RID: 9398 RVA: 0x000A9A54 File Offset: 0x000A7C54
	public static T BytesToData<T>(byte[] bytes) where T : new()
	{
		return SaveDataUtility.DeserializeSaveData<T>(GameManager.GetJsonForSaveBytesStatic(bytes));
	}

	// Token: 0x060024B7 RID: 9399 RVA: 0x000A9A64 File Offset: 0x000A7C64
	public byte[] GetBytesForSaveData(SaveGameData saveGameData)
	{
		string json = GameManager.GetJson<SaveGameData>(saveGameData);
		return this.GetBytesForSaveJson(json);
	}

	// Token: 0x060024B8 RID: 9400 RVA: 0x000A9A80 File Offset: 0x000A7C80
	private SaveStats GetLoadedSaveSlotData(string jsonData)
	{
		SaveStats result;
		try
		{
			SaveGameData saveGameData = SaveDataUtility.DeserializeSaveData<SaveGameData>(jsonData);
			result = new SaveStats(saveGameData.playerData, saveGameData);
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			result = SaveStats.Blank;
		}
		return result;
	}

	// Token: 0x060024B9 RID: 9401 RVA: 0x000A9AC4 File Offset: 0x000A7CC4
	public static SaveStats GetSaveStatsFromData(SaveGameData saveGameData)
	{
		return new SaveStats(saveGameData.playerData, saveGameData);
	}

	// Token: 0x060024BA RID: 9402 RVA: 0x000A9AD2 File Offset: 0x000A7CD2
	public IEnumerator PauseGameToggleByMenu()
	{
		yield return null;
		IEnumerator iterator = this.PauseGameToggle(false);
		while (iterator.MoveNext())
		{
			object obj = iterator.Current;
			yield return obj;
		}
		yield break;
	}

	// Token: 0x060024BB RID: 9403 RVA: 0x000A9AE1 File Offset: 0x000A7CE1
	public IEnumerator PauseGameToggle(bool playSound)
	{
		if (this.TimeSlowed)
		{
			yield break;
		}
		if (GenericMessageCanvas.IsActive)
		{
			yield break;
		}
		bool flag = InteractManager.BlockingInteractable && !this.playerData.atBench;
		if (!this.playerData.disablePause && this.GameState == GameState.PLAYING && !flag)
		{
			this.isPaused = true;
			this.ui.SetState(UIState.PAUSED);
			this.SetPausedState(true);
			this.SetState(GameState.PAUSED);
			if (HeroController.instance != null)
			{
				HeroController.instance.Pause();
			}
			this.gameCams.MoveMenuToHUDCamera();
			this.inputHandler.PreventPause();
			this.inputHandler.StopUIInput();
			if (playSound)
			{
				this.ui.uiAudioPlayer.PlayPause();
			}
			yield return new WaitForSecondsRealtime(0.3f);
			this.inputHandler.AllowPause();
		}
		else if (this.GameState == GameState.PAUSED)
		{
			this.isPaused = false;
			this.inputHandler.PreventPause();
			this.ui.SetState(UIState.PLAYING);
			this.SetPausedState(false);
			this.SetState(GameState.PLAYING);
			if (HeroController.instance != null)
			{
				HeroController.instance.UnPause();
			}
			MenuButtonList.ClearAllLastSelected();
			if (playSound)
			{
				this.ui.uiAudioPlayer.PlayUnpause();
			}
			yield return new WaitForSecondsRealtime(0.3f);
			this.inputHandler.AllowPause();
		}
		yield break;
	}

	// Token: 0x060024BC RID: 9404 RVA: 0x000A9AF8 File Offset: 0x000A7CF8
	private void SetPausedState(bool value)
	{
		if (value)
		{
			this.gameCams.StopCameraShake();
			this.actorSnapshotPaused.TransitionToSafe(0f);
			this.ui.AudioGoToPauseMenu(0.2f);
			this.SetTimeScale(0f);
			GlobalSettings.Camera.MainCameraShakeManager.ApplyOffsets();
		}
		else
		{
			this.gameCams.ResumeCameraShake();
			this.actorSnapshotUnpaused.TransitionToSafe(0f);
			this.ui.AudioGoToGameplay(0.2f);
			this.SetTimeScale(1f);
		}
		GameManager.PausedEvent gamePausedChange = this.GamePausedChange;
		if (gamePausedChange == null)
		{
			return;
		}
		gamePausedChange(value);
	}

	// Token: 0x060024BD RID: 9405 RVA: 0x000A9B91 File Offset: 0x000A7D91
	private IEnumerator SetTimeScale(float newTimeScale, float duration)
	{
		float lastTimeScale = TimeManager.TimeScale;
		for (float timer = 0f; timer < duration; timer += Time.unscaledDeltaTime)
		{
			float t = Mathf.Clamp01(timer / duration);
			this.SetTimeScale(Mathf.Lerp(lastTimeScale, newTimeScale, t));
			yield return null;
		}
		this.SetTimeScale(newTimeScale);
		yield break;
	}

	// Token: 0x060024BE RID: 9406 RVA: 0x000A9BAE File Offset: 0x000A7DAE
	public void SetTimeScale(float newTimeScale)
	{
		if (this.timeSlowedCount > 1)
		{
			newTimeScale = Mathf.Min(newTimeScale, TimeManager.TimeScale);
		}
		TimeManager.TimeScale = ((newTimeScale > 0.01f) ? newTimeScale : 0f);
	}

	// Token: 0x060024BF RID: 9407 RVA: 0x000A9BDB File Offset: 0x000A7DDB
	private IEnumerator SetTimeScale(TimeManager.TimeControlInstance controlInstance, float newTimeScale, float duration)
	{
		float lastTimeScale = controlInstance.TimeScale;
		for (float timer = 0f; timer < duration; timer += Time.unscaledDeltaTime)
		{
			float t = Mathf.Clamp01(timer / duration);
			controlInstance.TimeScale = Mathf.Lerp(lastTimeScale, newTimeScale, t);
			yield return null;
		}
		controlInstance.TimeScale = newTimeScale;
		yield break;
	}

	// Token: 0x060024C0 RID: 9408 RVA: 0x000A9BF8 File Offset: 0x000A7DF8
	public void FreezeMoment(int type)
	{
		this.FreezeMoment((FreezeMomentTypes)type, null);
	}

	// Token: 0x060024C1 RID: 9409 RVA: 0x000A9C04 File Offset: 0x000A7E04
	public void FreezeMoment(FreezeMomentTypes type, Action onFinish = null)
	{
		switch (type)
		{
		case FreezeMomentTypes.HeroDamage:
			base.StartCoroutine(this.FreezeMoment(0.01f, 0.28f, 0.1f, 0f, onFinish));
			return;
		case FreezeMomentTypes.EnemyDeath:
			base.StartCoroutine(this.FreezeMoment(0.04f, 0.024f, 0.04f, 0f, onFinish));
			return;
		case FreezeMomentTypes.BossDeathStrike:
			base.StartCoroutine(this.FreezeMoment(0f, 0.35f, 0.1f, 0f, onFinish));
			return;
		case FreezeMomentTypes.NailClashEffect:
			base.StartCoroutine(this.FreezeMoment(0.01f, 0.25f, 0.1f, 0f, onFinish));
			return;
		case FreezeMomentTypes.BossStun:
			base.StartCoroutine(this.FreezeMoment(0f, 0.25f, 0.1f, 0f, onFinish));
			return;
		case FreezeMomentTypes.EnemyDeathShort:
			base.StartCoroutine(this.FreezeMoment(0.04f, 0.015f, 0.04f, 0f, onFinish));
			return;
		case FreezeMomentTypes.QuickFreeze:
			base.StartCoroutine(this.FreezeMoment(0.0001f, 0.02f, 0.0001f, 0.0001f, onFinish));
			return;
		case FreezeMomentTypes.ZapFreeze:
			base.StartCoroutine(this.FreezeMoment(0f, 0.1f, 0f, 0f, onFinish));
			return;
		case FreezeMomentTypes.WitchBindHit:
			base.StartCoroutine(this.FreezeMoment(0.04f, 0.03f, 0.04f, 0f, onFinish));
			return;
		case FreezeMomentTypes.HeroDamageShort:
			base.StartCoroutine(this.FreezeMoment(0.001f, 0.15f, 0.05f, 0f, onFinish));
			return;
		case FreezeMomentTypes.BossDeathSlow:
			base.StartCoroutine(this.FreezeMoment(0.1f, 1.15f, 0.1f, 0.05f, onFinish));
			return;
		case FreezeMomentTypes.RaceWinSlow:
			base.StartCoroutine(this.FreezeMoment(0.5f, 3f, 0.3f, 0.1f, onFinish));
			return;
		case FreezeMomentTypes.EnemyBattleEndSlow:
			base.StartCoroutine(this.FreezeMoment(0.1f, 1f, 0.75f, 0.25f, onFinish));
			return;
		case FreezeMomentTypes.BigEnemyDeathSlow:
			base.StartCoroutine(this.FreezeMoment(0.04f, 0.06f, 0.04f, 0f, onFinish));
			return;
		case FreezeMomentTypes.BindBreak:
			base.StartCoroutine(this.FreezeMoment(0.01f, 0.4f, 0.1f, 0f, onFinish));
			return;
		default:
			return;
		}
	}

	// Token: 0x060024C2 RID: 9410 RVA: 0x000A9E60 File Offset: 0x000A8060
	public IEnumerator FreezeMoment(float rampDownTime, float waitTime, float rampUpTime, float targetSpeed, Action onFinish = null)
	{
		this.timeSlowedCount++;
		TimeManager.TimeControlInstance timeControl = TimeManager.CreateTimeControl(1f, TimeManager.TimeControlInstance.Type.Multiplicative);
		try
		{
			yield return base.StartCoroutine(this.SetTimeScale(timeControl, targetSpeed, rampDownTime));
			for (float timer = 0f; timer < waitTime; timer += Time.unscaledDeltaTime)
			{
				yield return null;
			}
			yield return base.StartCoroutine(this.SetTimeScale(timeControl, 1f, rampUpTime));
		}
		finally
		{
			timeControl.Release();
			this.timeSlowedCount--;
			if (onFinish != null)
			{
				onFinish();
			}
		}
		yield break;
		yield break;
	}

	// Token: 0x060024C3 RID: 9411 RVA: 0x000A9E94 File Offset: 0x000A8094
	public IEnumerator FreezeMomentGC(float rampDownTime, float waitTime, float rampUpTime, float targetSpeed)
	{
		this.timeSlowedCount++;
		yield return base.StartCoroutine(this.SetTimeScale(targetSpeed, rampDownTime));
		for (float timer = 0f; timer < waitTime; timer += Time.unscaledDeltaTime)
		{
			yield return null;
		}
		GCManager.Collect();
		yield return base.StartCoroutine(this.SetTimeScale(1f, rampUpTime));
		this.timeSlowedCount--;
		yield break;
	}

	// Token: 0x060024C4 RID: 9412 RVA: 0x000A9EC0 File Offset: 0x000A80C0
	public void EnsureSaveSlotSpace(Action<bool> callback)
	{
		Platform.Current.EnsureSaveSlotSpace(this.profileID, callback);
	}

	// Token: 0x060024C5 RID: 9413 RVA: 0x000A9ED4 File Offset: 0x000A80D4
	public void StartNewGame(bool permadeathMode = false, bool bossRushMode = false)
	{
		this.playerData = PlayerData.CreateNewSingleton(false);
		this.playerData.permadeathMode = (permadeathMode ? PermadeathModes.On : PermadeathModes.Off);
		Platform.Current.PrepareForNewGame(this.profileID);
		if (bossRushMode)
		{
			this.playerData.AddGGPlayerDataOverrides();
			base.StartCoroutine(this.RunContinueGame(true));
			return;
		}
		base.StartCoroutine(this.RunStartNewGame());
	}

	// Token: 0x060024C6 RID: 9414 RVA: 0x000A9F39 File Offset: 0x000A8139
	public IEnumerator RunStartNewGame()
	{
		this.ui.FadeScreenOut();
		this.noMusicSnapshot.TransitionToSafe(2f);
		this.noAtmosSnapshot.TransitionToSafe(2f);
		yield return new WaitForSeconds(2.6f);
		Platform.Current.SetSceneLoadState(true, true);
		Platform.Current.DeleteVersionBackupsForSlot(this.profileID, null);
		this.ui.MakeMenuLean();
		AsyncOperationHandle<GameObject> handle = this.LoadGlobalPoolPrefab();
		yield return handle;
		Object.Instantiate<GameObject>(handle.Result);
		handle = default(AsyncOperationHandle<GameObject>);
		ObjectPool.CreateStartupPools();
		this.BeginSceneTransition(new GameManager.SceneLoadInfo
		{
			AlwaysUnloadUnusedAssets = true,
			IsFirstLevelForPlayer = true,
			PreventCameraFadeOut = true,
			WaitForSceneTransitionCameraFade = false,
			SceneName = "Opening_Sequence",
			Visualization = GameManager.SceneLoadVisualizations.Custom
		});
		yield break;
	}

	// Token: 0x060024C7 RID: 9415 RVA: 0x000A9F48 File Offset: 0x000A8148
	public void ContinueGame()
	{
		base.StartCoroutine(this.RunContinueGame(this.IsMenuScene()));
	}

	// Token: 0x060024C8 RID: 9416 RVA: 0x000A9F5D File Offset: 0x000A815D
	public IEnumerator RunContinueGame(bool fromMenu = true)
	{
		if (fromMenu)
		{
			this.ui.FadeScreenOut();
			this.noMusicSnapshot.TransitionToSafe(2f);
			this.noAtmosSnapshot.TransitionToSafe(2f);
			yield return new WaitForSeconds(1f);
			this.ui.FadeOutBlackThreadLoop();
			yield return new WaitForSeconds(1.6f);
			this.audioManager.ApplyMusicCue(this.noMusicCue, 0f, 0f, false);
			this.ui.MakeMenuLean();
		}
		else
		{
			this.SetPausedState(false);
			this.isPaused = false;
		}
		Platform.Current.SetSceneLoadState(true, true);
		this.isLoading = true;
		this.SetState(GameState.LOADING);
		if (this.playerData.IsAct3IntroQueued)
		{
			this.loadVisualization = GameManager.SceneLoadVisualizations.Custom;
			this.StartAct3();
		}
		else
		{
			this.loadVisualization = GameManager.SceneLoadVisualizations.ContinueFromSave;
		}
		SaveDataUpgradeHandler.UpgradeSaveData(ref this.playerData);
		this.TimePassesLoadedIn();
		AsyncOperationHandle<GameObject> handle = this.LoadGlobalPoolPrefab();
		yield return handle;
		Object.Instantiate<GameObject>(handle.Result);
		handle = default(AsyncOperationHandle<GameObject>);
		ObjectPool.CreateStartupPools();
		handle = this.LoadHeroPrefab();
		yield return handle;
		Object.Instantiate<GameObject>(handle.Result);
		handle = default(AsyncOperationHandle<GameObject>);
		this.SetupSceneRefs(false);
		yield return null;
		yield return null;
		Platform.Current.SetSceneLoadState(false, false);
		this.needFirstFadeIn = true;
		this.isLoading = false;
		if (this.hero_ctrl == null)
		{
			this.SetupHeroRefs();
		}
		if (this.hero_ctrl != null)
		{
			this.hero_ctrl.IgnoreInput();
		}
		if (this.playerData.IsAct3IntroQueued)
		{
			this.BeginSceneTransition(new GameManager.SceneLoadInfo
			{
				AlwaysUnloadUnusedAssets = true,
				IsFirstLevelForPlayer = true,
				PreventCameraFadeOut = true,
				WaitForSceneTransitionCameraFade = false,
				SceneName = "Opening_Sequence_Act3",
				Visualization = GameManager.SceneLoadVisualizations.Custom
			});
		}
		else
		{
			this.ReadyForRespawn(true);
		}
		yield break;
	}

	// Token: 0x060024C9 RID: 9417 RVA: 0x000A9F73 File Offset: 0x000A8173
	public AsyncOperationHandle<GameObject> LoadGlobalPoolPrefab()
	{
		if (this.globalPoolPrefabHandle.IsValid())
		{
			return this.globalPoolPrefabHandle;
		}
		this.globalPoolPrefabHandle = Addressables.LoadAssetAsync<GameObject>("GlobalPool");
		return this.globalPoolPrefabHandle;
	}

	// Token: 0x060024CA RID: 9418 RVA: 0x000A9F9F File Offset: 0x000A819F
	public void UnloadGlobalPoolPrefab()
	{
		if (!this.globalPoolPrefabHandle.IsValid())
		{
			return;
		}
		Addressables.Release<GameObject>(this.globalPoolPrefabHandle);
		this.globalPoolPrefabHandle = default(AsyncOperationHandle<GameObject>);
	}

	// Token: 0x060024CB RID: 9419 RVA: 0x000A9FC6 File Offset: 0x000A81C6
	public AsyncOperationHandle<GameObject> LoadHeroPrefab()
	{
		if (this.heroPrefabHandle.IsValid())
		{
			return this.heroPrefabHandle;
		}
		this.heroPrefabHandle = Addressables.LoadAssetAsync<GameObject>("Hero_Hornet");
		return this.heroPrefabHandle;
	}

	// Token: 0x060024CC RID: 9420 RVA: 0x000A9FF2 File Offset: 0x000A81F2
	public void UnloadHeroPrefab()
	{
		if (!this.heroPrefabHandle.IsValid())
		{
			return;
		}
		Addressables.Release<GameObject>(this.heroPrefabHandle);
		this.heroPrefabHandle = default(AsyncOperationHandle<GameObject>);
	}

	// Token: 0x060024CD RID: 9421 RVA: 0x000AA019 File Offset: 0x000A8219
	public IEnumerator ReturnToMainMenu(bool willSave, Action<bool> callback = null, bool isEndGame = false, bool forceMainMenu = false)
	{
		PersistentAudioManager.OnLeaveScene();
		this.inputHandler.PreventPause();
		VibrationManager.StopAllVibration();
		this.AwardQueuedAchievements();
		if (BossSequenceController.IsInSequence)
		{
			BossSequenceController.RestoreBindings();
		}
		this.TimePasses();
		if (willSave)
		{
			GameManager.<>c__DisplayClass475_1 CS$<>8__locals2 = new GameManager.<>c__DisplayClass475_1();
			CS$<>8__locals2.saveComplete = null;
			this.SaveGame(delegate(bool didSave)
			{
				CS$<>8__locals2.saveComplete = new bool?(didSave);
			});
			while (CS$<>8__locals2.saveComplete == null)
			{
				yield return null;
			}
			if (callback != null)
			{
				callback(CS$<>8__locals2.saveComplete.Value);
			}
			if (!forceMainMenu && !CS$<>8__locals2.saveComplete.Value)
			{
				yield break;
			}
			CS$<>8__locals2 = null;
		}
		else if (callback != null)
		{
			callback(false);
		}
		string previousSceneName = SceneManager.GetActiveScene().name;
		AsyncOperationHandle<SceneInstance> opHandle = Addressables.LoadSceneAsync("Scenes/Quit_To_Menu", LoadSceneMode.Single, false, 100, SceneReleaseMode.ReleaseSceneWhenSceneUnloaded);
		opHandle.Completed += delegate(AsyncOperationHandle<SceneInstance> _)
		{
			GameManager.ReportUnload(previousSceneName);
		};
		this.cameraCtrl.FreezeInPlace(true);
		this.cameraCtrl.FadeOut(CameraFadeType.TO_MENU);
		this.silentSnapshot.TransitionToSafe(2.5f);
		for (float timer = 0f; timer < 2.5f; timer += Time.unscaledDeltaTime)
		{
			yield return null;
		}
		this.audioManager.StopAndClearMusic();
		this.audioManager.StopAndClearAtmos();
		EventRegister.SendEvent(EventRegisterEvents.FsmCancel, null);
		Platform.Current.SetSceneLoadState(true, true);
		StandaloneLoadingSpinner standaloneLoadingSpinner = Object.Instantiate<StandaloneLoadingSpinner>(isEndGame ? this.standaloneLoadingSpinnerEndGamePrefab : this.standaloneLoadingSpinnerPrefab);
		standaloneLoadingSpinner.Setup(this);
		Object.DontDestroyOnLoad(standaloneLoadingSpinner.gameObject);
		if (this.UnloadingLevel != null)
		{
			try
			{
				this.UnloadingLevel();
			}
			catch (Exception ex)
			{
				Debug.LogErrorFormat("Error while UnloadingLevel in QuitToMenu, attempting to continue regardless.", Array.Empty<object>());
				CheatManager.LastErrorText = ex.ToString();
				Debug.LogException(ex);
			}
		}
		if (this.NextSceneWillActivate != null)
		{
			try
			{
				this.NextSceneWillActivate();
			}
			catch (Exception ex2)
			{
				Debug.LogErrorFormat("Error while DestroyingPersonalPools in QuitToMenu, attempting to continue regardless.", Array.Empty<object>());
				CheatManager.LastErrorText = ex2.ToString();
				Debug.LogException(ex2);
			}
		}
		PlayMakerFSM.BroadcastEvent("QUIT TO MENU");
		this.waitForManualLevelStart = true;
		StaticVariableList.Clear();
		TrackingTrail.ClearStatic();
		this.sceneSeedTrackers.Clear();
		yield return opHandle;
		yield return opHandle.Result.ActivateAsync();
		this.inputHandler.AllowPause();
		this.didEmergencyQuit = false;
		yield break;
	}

	// Token: 0x060024CE RID: 9422 RVA: 0x000AA045 File Offset: 0x000A8245
	public void ReturnToMainMenuNoSave()
	{
		base.StartCoroutine(this.ReturnToMainMenu(false, null, false, false));
	}

	// Token: 0x060024CF RID: 9423 RVA: 0x000AA058 File Offset: 0x000A8258
	private void EmergencyReturnToMenu()
	{
		this.didEmergencyQuit = true;
		base.StartCoroutine(this.ui.EmergencyReturnToMainMenu());
	}

	// Token: 0x060024D0 RID: 9424 RVA: 0x000AA073 File Offset: 0x000A8273
	public void DoEmergencyQuit()
	{
		if (this.didEmergencyQuit)
		{
			return;
		}
		this.EmergencyReturnToMenu();
	}

	// Token: 0x060024D1 RID: 9425 RVA: 0x000AA084 File Offset: 0x000A8284
	public IEnumerator QuitGame()
	{
		FSMUtility.SendEventToGameObject(GameObject.Find("Quit Blanker"), "START FADE", false);
		yield return new WaitForSeconds(0.5f);
		Application.Quit();
		yield break;
	}

	// Token: 0x060024D2 RID: 9426 RVA: 0x000AA08C File Offset: 0x000A828C
	public void LoadedBoss()
	{
		GameManager.BossLoad onLoadedBoss = this.OnLoadedBoss;
		if (onLoadedBoss == null)
		{
			return;
		}
		onLoadedBoss();
	}

	// Token: 0x060024D3 RID: 9427 RVA: 0x000AA09E File Offset: 0x000A829E
	public void DoDestroyPersonalPools()
	{
		Action nextSceneWillActivate = this.NextSceneWillActivate;
		if (nextSceneWillActivate == null)
		{
			return;
		}
		nextSceneWillActivate();
	}

	// Token: 0x060024D4 RID: 9428 RVA: 0x000AA0B0 File Offset: 0x000A82B0
	public float GetImplicitCinematicVolume()
	{
		return Mathf.Clamp01(this.gameSettings.masterVolume / 10f) * Mathf.Clamp01(this.gameSettings.soundVolume / 10f);
	}

	// Token: 0x060024D5 RID: 9429 RVA: 0x000AA0E0 File Offset: 0x000A82E0
	public void SetIsInventoryOpen(bool value)
	{
		if (GameManager._inventoryInputBlocker == null)
		{
			GameManager._inventoryInputBlocker = new object();
		}
		PlayerData.instance.isInventoryOpen = value;
		this.SetPausedState(value);
		if (value)
		{
			this.hero_ctrl.AddInputBlocker(GameManager._inventoryInputBlocker);
			return;
		}
		this.hero_ctrl.RemoveInputBlocker(GameManager._inventoryInputBlocker);
	}

	// Token: 0x060024D6 RID: 9430 RVA: 0x000AA134 File Offset: 0x000A8334
	public bool CanPickupsExist()
	{
		return !BossSceneController.IsBossScene && !this.IsMemoryScene();
	}

	// Token: 0x060024D7 RID: 9431 RVA: 0x000AA148 File Offset: 0x000A8348
	public bool IsMemoryScene()
	{
		bool flag = this.forceCurrentSceneMemory || GameManager.IsMemoryScene(this.GetCurrentMapZoneEnum());
		if (flag)
		{
			bool flag2 = this.sm != null;
			if (flag2)
			{
				this.GetSceneManager();
				flag2 = (this.sm != null);
			}
			if (flag2 && this.sm.ForceNotMemory)
			{
				return false;
			}
		}
		return flag;
	}

	// Token: 0x060024D8 RID: 9432 RVA: 0x000AA1A6 File Offset: 0x000A83A6
	public static bool IsMemoryScene(MapZone mapZone)
	{
		return mapZone == MapZone.CLOVER || mapZone == MapZone.MEMORY;
	}

	// Token: 0x060024D9 RID: 9433 RVA: 0x000AA1B5 File Offset: 0x000A83B5
	public void ForceCurrentSceneIsMemory(bool value)
	{
		this.forceCurrentSceneMemory = true;
	}

	// Token: 0x060024DC RID: 9436 RVA: 0x000AA1F5 File Offset: 0x000A83F5
	[CompilerGenerated]
	internal static void <TimePassesElsewhere>g__CheckReadyToLeave|249_0(ref NPCEncounterState state)
	{
		if (state == NPCEncounterState.ReadyToLeave)
		{
			state = NPCEncounterState.AuthorisedToLeave;
		}
	}

	// Token: 0x0400224E RID: 8782
	private const float TIME_PASSES_SCENE_LIMIT = 300f;

	// Token: 0x04002250 RID: 8784
	public bool isPaused;

	// Token: 0x04002254 RID: 8788
	private int timeSlowedCount;

	// Token: 0x04002255 RID: 8789
	public string sceneName;

	// Token: 0x04002256 RID: 8790
	public int sceneNameHash;

	// Token: 0x04002257 RID: 8791
	public string nextSceneName;

	// Token: 0x04002258 RID: 8792
	public string entryGateName;

	// Token: 0x04002259 RID: 8793
	private TransitionPoint callingGate;

	// Token: 0x0400225A RID: 8794
	private Vector3 entrySpawnPoint;

	// Token: 0x0400225B RID: 8795
	private float entryDelay;

	// Token: 0x0400225C RID: 8796
	public float sceneWidth;

	// Token: 0x0400225D RID: 8797
	public float sceneHeight;

	// Token: 0x0400225E RID: 8798
	public string lastSceneName;

	// Token: 0x0400225F RID: 8799
	private List<GameManager.SceneSeedTracker> sceneSeedTrackers;

	// Token: 0x04002261 RID: 8801
	public GameConfig gameConfig;

	// Token: 0x04002263 RID: 8803
	public GameCameras gameCams;

	// Token: 0x04002265 RID: 8805
	private List<string> queuedMenuStyles = new List<string>();

	// Token: 0x04002266 RID: 8806
	[SerializeField]
	private AudioManager audioManager;

	// Token: 0x04002267 RID: 8807
	[SerializeField]
	private InControlManager inControlManagerPrefab;

	// Token: 0x04002269 RID: 8809
	[SerializeField]
	public GameSettings gameSettings;

	// Token: 0x0400226A RID: 8810
	public TimeScaleIndependentUpdate timeTool;

	// Token: 0x0400226B RID: 8811
	public GameMap gameMap;

	// Token: 0x04002276 RID: 8822
	[SerializeField]
	public PlayerData playerData;

	// Token: 0x04002277 RID: 8823
	[SerializeField]
	public SceneData sceneData;

	// Token: 0x04002278 RID: 8824
	public int profileID;

	// Token: 0x04002279 RID: 8825
	private bool needsFlush;

	// Token: 0x0400227A RID: 8826
	private float sessionStartTime;

	// Token: 0x0400227B RID: 8827
	private float sessionPlayTimer;

	// Token: 0x0400227C RID: 8828
	private float timeInScene;

	// Token: 0x0400227D RID: 8829
	private float timeSinceLastTimePasses;

	// Token: 0x0400227E RID: 8830
	public string lastTimePassesMapZone;

	// Token: 0x0400227F RID: 8831
	public bool startedOnThisScene = true;

	// Token: 0x04002281 RID: 8833
	private bool hazardRespawningHero;

	// Token: 0x04002282 RID: 8834
	private string targetScene;

	// Token: 0x04002283 RID: 8835
	private bool needFirstFadeIn;

	// Token: 0x04002284 RID: 8836
	private bool waitForManualLevelStart;

	// Token: 0x04002285 RID: 8837
	private AsyncOperationHandle<GameObject> globalPoolPrefabHandle;

	// Token: 0x04002286 RID: 8838
	private AsyncOperationHandle<GameObject> heroPrefabHandle;

	// Token: 0x04002287 RID: 8839
	private int heroDeathCount;

	// Token: 0x04002288 RID: 8840
	private bool startedSteamEnabled;

	// Token: 0x04002289 RID: 8841
	private bool startedGOGEnabled;

	// Token: 0x0400228A RID: 8842
	private bool startedLanguageDisabled;

	// Token: 0x0400228B RID: 8843
	private bool isSaveGameQueued;

	// Token: 0x0400228C RID: 8844
	private bool isAutoSaveQueued;

	// Token: 0x0400228D RID: 8845
	private AutoSaveName queuedAutoSaveName;

	// Token: 0x0400228E RID: 8846
	private bool forceCurrentSceneMemory;

	// Token: 0x0400228F RID: 8847
	public AudioMixerSnapshot actorSnapshotUnpaused;

	// Token: 0x04002290 RID: 8848
	public AudioMixerSnapshot actorSnapshotPaused;

	// Token: 0x04002291 RID: 8849
	[SerializeField]
	private float sceneTransitionActorFadeDown;

	// Token: 0x04002292 RID: 8850
	[SerializeField]
	private float sceneTransitionActorFadeUp;

	// Token: 0x04002293 RID: 8851
	public AudioMixerSnapshot silentSnapshot;

	// Token: 0x04002294 RID: 8852
	public AudioMixerSnapshot noMusicSnapshot;

	// Token: 0x04002295 RID: 8853
	public MusicCue noMusicCue;

	// Token: 0x04002296 RID: 8854
	public AudioMixerSnapshot noAtmosSnapshot;

	// Token: 0x04002297 RID: 8855
	[NonSerialized]
	private int nextLevelEntryNumber;

	// Token: 0x04002298 RID: 8856
	[NonSerialized]
	private int skipActorEntryFade = -1;

	// Token: 0x040022A5 RID: 8869
	private bool hasFinishedEnteringScene;

	// Token: 0x040022A6 RID: 8870
	private bool didEmergencyQuit;

	// Token: 0x040022A7 RID: 8871
	private bool isLoading;

	// Token: 0x040022A8 RID: 8872
	private GameManager.SceneLoadVisualizations loadVisualization;

	// Token: 0x040022A9 RID: 8873
	private float currentLoadDuration;

	// Token: 0x040022AA RID: 8874
	private int sceneLoadsWithoutGarbageCollect;

	// Token: 0x040022AB RID: 8875
	private int queuedBlueHealth;

	// Token: 0x040022AC RID: 8876
	[SerializeField]
	private StandaloneLoadingSpinner standaloneLoadingSpinnerPrefab;

	// Token: 0x040022AD RID: 8877
	[SerializeField]
	private StandaloneLoadingSpinner standaloneLoadingSpinnerEndGamePrefab;

	// Token: 0x040022B0 RID: 8880
	private bool shouldFadeInScene;

	// Token: 0x040022B1 RID: 8881
	public static GameManager _instance;

	// Token: 0x040022B2 RID: 8882
	private static bool isFirstStartup = true;

	// Token: 0x040022B3 RID: 8883
	private SceneLoad sceneLoad;

	// Token: 0x040022B9 RID: 8889
	private bool hasSetup;

	// Token: 0x040022BA RID: 8890
	private bool registerEvents;

	// Token: 0x040022BB RID: 8891
	private CameraManagerReference subbedCamShake;

	// Token: 0x040022BC RID: 8892
	private List<GameManager.ISkippable> skippables;

	// Token: 0x040022BD RID: 8893
	private int mapZoneStringVersion = -1;

	// Token: 0x040022BE RID: 8894
	private string mapZoneString;

	// Token: 0x040022BF RID: 8895
	private int mapZoneVersion = -1;

	// Token: 0x040022C0 RID: 8896
	private MapZone currentMapZone;

	// Token: 0x040022C1 RID: 8897
	private string rawSceneName;

	// Token: 0x040022C2 RID: 8898
	private static string lastFullSceneName;

	// Token: 0x040022C3 RID: 8899
	private static string fixedSceneName;

	// Token: 0x040022C4 RID: 8900
	private int saveIconShowCounter;

	// Token: 0x040022C5 RID: 8901
	private static object _inventoryInputBlocker;

	// Token: 0x020016AF RID: 5807
	// (Invoke) Token: 0x06008AB4 RID: 35508
	public delegate void PausedEvent(bool isPaused);

	// Token: 0x020016B0 RID: 5808
	// (Invoke) Token: 0x06008AB8 RID: 35512
	public delegate void GameStateEvent(GameState gameState);

	// Token: 0x020016B1 RID: 5809
	private class SceneSeedTracker
	{
		// Token: 0x04008BB7 RID: 35767
		public int SceneNameHash;

		// Token: 0x04008BB8 RID: 35768
		public int Seed;

		// Token: 0x04008BB9 RID: 35769
		public int TransitionsLeft;
	}

	// Token: 0x020016B2 RID: 5810
	// (Invoke) Token: 0x06008ABD RID: 35517
	public delegate void BossLoad();

	// Token: 0x020016B3 RID: 5811
	// (Invoke) Token: 0x06008AC1 RID: 35521
	public delegate void EnterSceneEvent();

	// Token: 0x020016B4 RID: 5812
	// (Invoke) Token: 0x06008AC5 RID: 35525
	public delegate void SceneTransitionFinishEvent();

	// Token: 0x020016B5 RID: 5813
	public enum SceneLoadVisualizations
	{
		// Token: 0x04008BBB RID: 35771
		Custom = -1,
		// Token: 0x04008BBC RID: 35772
		Default,
		// Token: 0x04008BBD RID: 35773
		ContinueFromSave,
		// Token: 0x04008BBE RID: 35774
		ThreadMemory
	}

	// Token: 0x020016B6 RID: 5814
	public class SceneLoadInfo
	{
		// Token: 0x06008AC8 RID: 35528 RVA: 0x00280FC5 File Offset: 0x0027F1C5
		public SceneLoadInfo()
		{
			this.TransitionID = GameManager.SceneLoadInfo.transitionCounter++;
		}

		// Token: 0x06008AC9 RID: 35529 RVA: 0x00280FE8 File Offset: 0x0027F1E8
		public override string ToString()
		{
			return string.Format("Scene Load #{0} : Scene Name: {1} : Entry Gate Name {2} : Entry Skip {3} : Prevent Camera Fade {4} : Wait For Scene Transition {5} : Force Fetch Wait {6}", new object[]
			{
				this.TransitionID,
				this.SceneName,
				this.EntryGateName,
				this.EntrySkip,
				this.PreventCameraFadeOut,
				this.WaitForSceneTransitionCameraFade,
				this.ForceWaitFetch
			});
		}

		// Token: 0x06008ACA RID: 35530 RVA: 0x0028105D File Offset: 0x0027F25D
		public virtual void NotifyFadedOut()
		{
		}

		// Token: 0x06008ACB RID: 35531 RVA: 0x0028105F File Offset: 0x0027F25F
		public virtual void NotifyFetchComplete()
		{
		}

		// Token: 0x06008ACC RID: 35532 RVA: 0x00281061 File Offset: 0x0027F261
		public virtual bool IsReadyToActivate()
		{
			return true;
		}

		// Token: 0x06008ACD RID: 35533 RVA: 0x00281064 File Offset: 0x0027F264
		public virtual void NotifyFinished()
		{
		}

		// Token: 0x04008BBF RID: 35775
		public bool IsFirstLevelForPlayer;

		// Token: 0x04008BC0 RID: 35776
		public string SceneName;

		// Token: 0x04008BC1 RID: 35777
		public IResourceLocation SceneResourceLocation;

		// Token: 0x04008BC2 RID: 35778
		public int AsyncPriority = -1;

		// Token: 0x04008BC3 RID: 35779
		public GatePosition? HeroLeaveDirection;

		// Token: 0x04008BC4 RID: 35780
		public string EntryGateName;

		// Token: 0x04008BC5 RID: 35781
		public float EntryDelay;

		// Token: 0x04008BC6 RID: 35782
		public bool EntrySkip;

		// Token: 0x04008BC7 RID: 35783
		public bool PreventCameraFadeOut;

		// Token: 0x04008BC8 RID: 35784
		public bool WaitForSceneTransitionCameraFade;

		// Token: 0x04008BC9 RID: 35785
		public GameManager.SceneLoadVisualizations Visualization;

		// Token: 0x04008BCA RID: 35786
		public bool AlwaysUnloadUnusedAssets;

		// Token: 0x04008BCB RID: 35787
		public bool ForceWaitFetch;

		// Token: 0x04008BCC RID: 35788
		public int TransitionID;

		// Token: 0x04008BCD RID: 35789
		protected static int transitionCounter;
	}

	// Token: 0x020016B7 RID: 5815
	// (Invoke) Token: 0x06008ACF RID: 35535
	public delegate void SceneTransitionBeganDelegate(SceneLoad sceneLoad);

	// Token: 0x020016B8 RID: 5816
	private struct Rb2dState
	{
		// Token: 0x04008BCE RID: 35790
		public Rigidbody2D Body;

		// Token: 0x04008BCF RID: 35791
		public bool Simulated;
	}

	// Token: 0x020016B9 RID: 5817
	public interface ISceneManualSimulatePhysics
	{
		// Token: 0x06008AD2 RID: 35538
		void OnManualPhysics(float deltaTime);

		// Token: 0x06008AD3 RID: 35539
		void PrepareManualSimulate();

		// Token: 0x06008AD4 RID: 35540
		void OnManualSimulateFinished();
	}

	// Token: 0x020016BA RID: 5818
	public interface ISkippable
	{
		// Token: 0x06008AD5 RID: 35541
		IEnumerator Skip();
	}
}
