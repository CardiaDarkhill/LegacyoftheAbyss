using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnums;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

// Token: 0x020005FD RID: 1533
public class TransitionPoint : InteractableBase, ISceneLintUpgrader
{
	// Token: 0x140000AC RID: 172
	// (add) Token: 0x060036A7 RID: 13991 RVA: 0x000F1140 File Offset: 0x000EF340
	// (remove) Token: 0x060036A8 RID: 13992 RVA: 0x000F1178 File Offset: 0x000EF378
	public event TransitionPoint.BeforeTransitionEvent OnBeforeTransition;

	// Token: 0x1700064D RID: 1613
	// (get) Token: 0x060036A9 RID: 13993 RVA: 0x000F11AD File Offset: 0x000EF3AD
	// (set) Token: 0x060036AA RID: 13994 RVA: 0x000F11B5 File Offset: 0x000EF3B5
	public ITransitionPointDoorAnim DoorAnimHandler { get; set; }

	// Token: 0x1700064E RID: 1614
	// (get) Token: 0x060036AB RID: 13995 RVA: 0x000F11BE File Offset: 0x000EF3BE
	// (set) Token: 0x060036AC RID: 13996 RVA: 0x000F11C5 File Offset: 0x000EF3C5
	public static bool IsTransitionBlocked { get; set; }

	// Token: 0x1700064F RID: 1615
	// (get) Token: 0x060036AD RID: 13997 RVA: 0x000F11CD File Offset: 0x000EF3CD
	// (set) Token: 0x060036AE RID: 13998 RVA: 0x000F11D4 File Offset: 0x000EF3D4
	public static List<TransitionPoint> TransitionPoints { get; private set; }

	// Token: 0x060036AF RID: 13999 RVA: 0x000F11DC File Offset: 0x000EF3DC
	protected override bool EnableInteractableFields()
	{
		return this.isADoor && !this.isInactive;
	}

	// Token: 0x060036B0 RID: 14000 RVA: 0x000F11F1 File Offset: 0x000EF3F1
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Init()
	{
		TransitionPoint.TransitionPoints = new List<TransitionPoint>();
	}

	// Token: 0x060036B1 RID: 14001 RVA: 0x000F1200 File Offset: 0x000EF400
	protected override void Awake()
	{
		base.Awake();
		this.collider = base.GetComponent<Collider2D>();
		this.OnSceneLintUpgrade(true);
		TransitionPoint.TransitionPoints.Add(this);
		if (!this.EnableInteractableFields())
		{
			base.Deactivate(false);
		}
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.TransitionPoint, true);
	}

	// Token: 0x060036B2 RID: 14002 RVA: 0x000F124E File Offset: 0x000EF44E
	protected void OnDestroy()
	{
		TransitionPoint.TransitionPoints.Remove(this);
		this.ClearHandles();
	}

	// Token: 0x060036B3 RID: 14003 RVA: 0x000F1264 File Offset: 0x000EF464
	protected override void OnDisable()
	{
		base.OnDisable();
		if (this.ignoredInput)
		{
			HeroController instance = HeroController.instance;
			if (instance != null)
			{
				instance.AcceptInput();
			}
			this.ignoredInput = false;
		}
	}

	// Token: 0x060036B4 RID: 14004 RVA: 0x000F129C File Offset: 0x000EF49C
	private void Start()
	{
		this.gm = GameManager.instance;
		if (!this.nonHazardGate && !this.respawnMarker)
		{
			HazardRespawnMarker componentInChildren = base.GetComponentInChildren<HazardRespawnMarker>();
			if (componentInChildren)
			{
				this.respawnMarker = componentInChildren;
			}
		}
		this.SetTargetScene(this.targetScene);
	}

	// Token: 0x060036B5 RID: 14005 RVA: 0x000F12EB File Offset: 0x000EF4EB
	protected override void OnTriggerEnter2D(Collider2D movingObj)
	{
		base.OnTriggerEnter2D(movingObj);
		if (this.isADoor)
		{
			return;
		}
		if (movingObj.gameObject.layer == 9)
		{
			this.TryDoTransition(movingObj);
		}
	}

	// Token: 0x060036B6 RID: 14006 RVA: 0x000F1313 File Offset: 0x000EF513
	private void OnTriggerStay2D(Collider2D movingObj)
	{
		if (this.activated)
		{
			return;
		}
		if (this.isADoor)
		{
			return;
		}
		if (movingObj.gameObject.layer == 9)
		{
			this.TryDoTransition(movingObj);
		}
	}

	// Token: 0x060036B7 RID: 14007 RVA: 0x000F1340 File Offset: 0x000EF540
	private void TryDoTransition(Collider2D heroCollider)
	{
		if (!this.gm)
		{
			return;
		}
		if (TransitionPoint.IsTransitionBlocked)
		{
			return;
		}
		HeroController instance = HeroController.instance;
		GatePosition gatePosition = this.GetGatePosition();
		if (this.gm.GameState == GameState.ENTERING_LEVEL)
		{
			if (gatePosition != GatePosition.bottom)
			{
				return;
			}
			if (!instance.isHeroInPosition)
			{
				return;
			}
			if (instance.Body.linearVelocity.y >= 0f)
			{
				return;
			}
		}
		else if (this.gm.GameState != GameState.PLAYING)
		{
			return;
		}
		bool flag = heroCollider.transform.localScale.x < 0f;
		bool flag2 = false;
		if (instance.cState.isBinding || instance.cState.recoiling || instance.cState.isInCutsceneMovement)
		{
			flag2 = true;
		}
		else if (gatePosition != GatePosition.right)
		{
			if (gatePosition == GatePosition.left)
			{
				if ((flag && !instance.cState.isBackSprinting) || (!flag && instance.cState.isBackSprinting))
				{
					flag2 = true;
				}
			}
		}
		else if ((!flag && !instance.cState.isBackSprinting) || (flag && instance.cState.isBackSprinting))
		{
			flag2 = true;
		}
		if (flag2 && (gatePosition == GatePosition.right || gatePosition == GatePosition.left))
		{
			Rigidbody2D component = heroCollider.GetComponent<Rigidbody2D>();
			if (component)
			{
				Rigidbody2D body = component;
				float? x = new float?(0f);
				float? y = null;
				body.SetVelocity(x, y);
			}
			Bounds bounds = this.collider.bounds;
			Bounds bounds2 = heroCollider.bounds;
			float x2;
			if (gatePosition == GatePosition.right)
			{
				x2 = bounds.min.x - bounds2.max.x;
			}
			else
			{
				x2 = bounds.max.x - bounds2.min.x;
			}
			Vector2 v = new Vector2(x2, 0f);
			heroCollider.transform.Translate(v, Space.World);
			return;
		}
		if (flag2 && (gatePosition == GatePosition.top || gatePosition == GatePosition.bottom))
		{
			Rigidbody2D component2 = heroCollider.GetComponent<Rigidbody2D>();
			if (component2)
			{
				Rigidbody2D body2 = component2;
				float? y = new float?(0f);
				body2.SetVelocity(null, y);
			}
			Bounds bounds3 = this.collider.bounds;
			Bounds bounds4 = heroCollider.bounds;
			float y2;
			if (gatePosition == GatePosition.top)
			{
				y2 = bounds3.min.y - bounds4.max.y;
			}
			else
			{
				y2 = bounds3.max.y - bounds4.min.y;
			}
			Vector2 v2 = new Vector2(0f, y2);
			heroCollider.transform.Translate(v2, Space.World);
			return;
		}
		if (!string.IsNullOrEmpty(this.targetScene) && !string.IsNullOrEmpty(this.entryPoint))
		{
			this.activated = true;
			if (gatePosition == GatePosition.bottom && (instance.cState.isBackSprinting || instance.cState.isBackScuttling))
			{
				EventRegister.SendEvent(EventRegisterEvents.FsmCancel, null);
			}
			this.DoFadeOut();
			this.DoSceneTransition(true);
		}
	}

	// Token: 0x060036B8 RID: 14008 RVA: 0x000F1601 File Offset: 0x000EF801
	private bool SceneGateExists(Dictionary<string, SceneTeleportMap.SceneInfo> teleportMap, string sceneName, string gateName)
	{
		return this.skipSceneMapCheck || (teleportMap.ContainsKey(sceneName) && teleportMap[sceneName].TransitionGates.Contains(gateName));
	}

	// Token: 0x060036B9 RID: 14009 RVA: 0x000F162C File Offset: 0x000EF82C
	private void DoSceneTransition(bool doFade)
	{
		TransitionPoint.BeforeTransitionEvent onBeforeTransition = this.OnBeforeTransition;
		if (onBeforeTransition != null)
		{
			onBeforeTransition();
		}
		string name = this.targetScene;
		string name2 = this.entryPoint;
		if (!DemoHelper.IsDemoMode)
		{
			bool flag = false;
			Dictionary<string, SceneTeleportMap.SceneInfo> teleportMap = SceneTeleportMap.GetTeleportMap();
			bool flag2 = this.SceneGateExists(teleportMap, name, name2);
			if (!flag2)
			{
				foreach (string str in WorldInfo.SubSceneNameSuffixes)
				{
					string sceneName = name + str;
					if (this.SceneGateExists(teleportMap, sceneName, name2))
					{
						flag2 = true;
						break;
					}
				}
			}
			if (!flag2)
			{
				Debug.LogErrorFormat(this, "Transition will fail! Returning to current scene. Target Scene: {0}, Target Gate: {1}", new object[]
				{
					name,
					name2
				});
				flag = true;
			}
			if (flag)
			{
				GameObject gameObject = base.gameObject;
				name = gameObject.scene.name;
				name2 = gameObject.name;
			}
		}
		if (name != this.lastResourceLocationScene)
		{
			this.ClearHandles();
		}
		TransitionPoint.SceneLoadInfo sceneLoadInfo = new TransitionPoint.SceneLoadInfo
		{
			SceneName = name,
			SceneResourceLocation = this.targetSceneResourceLocation,
			EntryGateName = name2,
			HeroLeaveDirection = new GatePosition?(this.GetGatePosition()),
			EntryDelay = this.entryDelay,
			WaitForSceneTransitionCameraFade = true,
			PreventCameraFadeOut = (this.customFadeFSM != null || !doFade),
			Visualization = this.sceneLoadVisualization,
			AlwaysUnloadUnusedAssets = this.alwaysUnloadUnusedAssets,
			ForceWaitFetch = this.forceWaitFetch
		};
		TransitionPoint.SceneLoadInfo sceneLoadInfo2 = sceneLoadInfo;
		sceneLoadInfo2.FadedOut = (Action)Delegate.Combine(sceneLoadInfo2.FadedOut, new Action(delegate()
		{
			EventRegister.SendEvent(EventRegisterEvents.InventoryOpenComplete, null);
		}));
		if (this.cutsceneFsmHolder)
		{
			GameObject cutsceneObj = this.cutsceneFsmHolder.ReferencedGameObject;
			if (cutsceneObj)
			{
				FSMUtility.SendEventToGameObject(cutsceneObj, "DOOR TOUCHED", false);
				TransitionPoint.SceneLoadInfo sceneLoadInfo3 = sceneLoadInfo;
				sceneLoadInfo3.FadedOut = (Action)Delegate.Combine(sceneLoadInfo3.FadedOut, new Action(delegate()
				{
					FSMUtility.SendEventToGameObject(cutsceneObj, "DOOR ENTERED", false);
				}));
				bool canActivate = false;
				EventRegister.GetRegisterGuaranteed(base.gameObject, "DOOR ENTER COMPLETE").ReceivedEvent += delegate()
				{
					canActivate = true;
					HeroController.instance.ResetSceneExitedStates();
				};
				sceneLoadInfo.CanActivateFunc = (() => canActivate);
				sceneLoadInfo.EntrySkip = true;
			}
		}
		HeroController instance = HeroController.instance;
		if (instance != null)
		{
			instance.RecordLeaveSceneCState();
			instance.IgnoreInput();
		}
		this.gm.AwardQueuedAchievements();
		this.gm.BeginSceneTransition(sceneLoadInfo);
	}

	// Token: 0x060036BA RID: 14010 RVA: 0x000F18B4 File Offset: 0x000EFAB4
	private void DoFadeOut()
	{
		if (this.customFadeFSM)
		{
			this.customFadeFSM.SendEventSafe("FADE");
		}
		if (this.additionalFadeFSM)
		{
			this.additionalFadeFSM.SendEventSafe("FADE");
		}
		if (this.atmosSnapshot != null)
		{
			AudioManager.TransitionToAtmosOverride(this.atmosSnapshot, this.AudioTransitionTime);
		}
		if (this.enviroSnapshot != null)
		{
			this.enviroSnapshot.TransitionTo(this.AudioTransitionTime);
		}
		if (this.actorSnapshot != null)
		{
			this.actorSnapshot.TransitionTo(this.AudioTransitionTime);
		}
		if (this.musicSnapshot != null)
		{
			this.musicSnapshot.TransitionTo(this.AudioTransitionTime);
		}
		VibrationManager.FadeVibration(0f, 0.25f);
	}

	// Token: 0x060036BB RID: 14011 RVA: 0x000F1988 File Offset: 0x000EFB88
	public GatePosition GetGatePosition()
	{
		string name = base.name;
		if (name.Contains("top"))
		{
			return GatePosition.top;
		}
		if (name.Contains("right"))
		{
			return GatePosition.right;
		}
		if (name.Contains("left"))
		{
			return GatePosition.left;
		}
		if (name.Contains("bot"))
		{
			return GatePosition.bottom;
		}
		if (name.Contains("door") || this.isADoor)
		{
			return GatePosition.door;
		}
		return GatePosition.unknown;
	}

	// Token: 0x060036BC RID: 14012 RVA: 0x000F19F0 File Offset: 0x000EFBF0
	public void SetTargetScene(string newScene)
	{
		this.ClearHandles();
		this.targetScene = newScene;
		if (string.IsNullOrEmpty(this.targetScene) || this.targetScene == "[dynamic]")
		{
			return;
		}
		this.lastResourceLocationScene = this.targetScene;
		if (this.targetSceneResourceHandle.IsValid())
		{
			Addressables.Release<IList<IResourceLocation>>(this.targetSceneResourceHandle);
		}
		this.targetSceneResourceHandle = Addressables.LoadResourceLocationsAsync("Scenes/" + this.targetScene, null);
		this.targetSceneResourceHandle.Completed += delegate(AsyncOperationHandle<IList<IResourceLocation>> handle)
		{
			IList<IResourceLocation> result = handle.Result;
			int count = result.Count;
			if (count <= 1 && count != 0)
			{
				this.targetSceneResourceLocation = result[0];
			}
		};
	}

	// Token: 0x060036BD RID: 14013 RVA: 0x000F1A81 File Offset: 0x000EFC81
	private void ClearHandles()
	{
		this.targetSceneResourceLocation = null;
		this.lastResourceLocationScene = null;
		if (this.targetSceneResourceHandle.IsValid())
		{
			Addressables.Release<IList<IResourceLocation>>(this.targetSceneResourceHandle);
		}
		this.targetSceneResourceHandle = default(AsyncOperationHandle<IList<IResourceLocation>>);
	}

	// Token: 0x060036BE RID: 14014 RVA: 0x000F1AB5 File Offset: 0x000EFCB5
	public void SetTargetDoor(string doorName)
	{
		this.entryPoint = doorName;
	}

	// Token: 0x060036BF RID: 14015 RVA: 0x000F1ABE File Offset: 0x000EFCBE
	public void SetCustomFade(bool value)
	{
		this.customFade = value;
	}

	// Token: 0x060036C0 RID: 14016 RVA: 0x000F1AC8 File Offset: 0x000EFCC8
	public string OnSceneLintUpgrade(bool doUpgrade)
	{
		PlayMakerFSM playMakerFSM = FSMUtility.LocateFSM(base.gameObject, "Door Control");
		if (!playMakerFSM)
		{
			return null;
		}
		FsmString fsmString = playMakerFSM.FsmVariables.FindFsmString("Entry Gate");
		this.entryPoint = fsmString.Value;
		FsmString fsmString2 = playMakerFSM.FsmVariables.FindFsmString("New Scene");
		this.targetScene = fsmString2.Value;
		FsmBool fsmBool = playMakerFSM.FsmVariables.FindFsmBool("Over Hero");
		this.IsOverHero = (fsmBool != null && fsmBool.Value);
		FsmObject fsmObject = playMakerFSM.FsmVariables.FindFsmObject("Atmos Snapshot");
		this.atmosSnapshot = (fsmObject.Value as AudioMixerSnapshot);
		FsmObject fsmObject2 = playMakerFSM.FsmVariables.FindFsmObject("Enviro Snapshot");
		this.enviroSnapshot = (fsmObject2.Value as AudioMixerSnapshot);
		FsmObject fsmObject3 = playMakerFSM.FsmVariables.FindFsmObject("Music Snapshot");
		this.musicSnapshot = (fsmObject3.Value as AudioMixerSnapshot);
		FsmFloat fsmFloat = playMakerFSM.FsmVariables.FindFsmFloat("Audio Transition Time");
		this.AudioTransitionTime = fsmFloat.Value;
		Object.DestroyImmediate(playMakerFSM);
		return "Door Control FSM was upgraded to TransitionPoint";
	}

	// Token: 0x060036C1 RID: 14017 RVA: 0x000F1BE3 File Offset: 0x000EFDE3
	public override void Interact()
	{
		this.activated = true;
		base.StartCoroutine(this.EnterDoorSequence());
	}

	// Token: 0x060036C2 RID: 14018 RVA: 0x000F1BF9 File Offset: 0x000EFDF9
	private IEnumerator EnterDoorSequence()
	{
		base.DisableInteraction();
		this.OnDoorEnter.Invoke();
		HeroController hc = HeroController.instance;
		hc.RelinquishControl();
		hc.IgnoreInput();
		this.ignoredInput = true;
		if (this.DoorAnimHandler != null)
		{
			yield return this.DoorAnimHandler.GetDoorAnimRoutine();
		}
		hc.StopAnimationControl();
		PlayerData instance = PlayerData.instance;
		instance.disablePause = true;
		instance.isInvincible = true;
		FSMUtility.SendEventToGameObject(base.gameObject, "DOOR ENTER", false);
		tk2dSpriteAnimator component = hc.GetComponent<tk2dSpriteAnimator>();
		HeroAnimationController component2 = hc.GetComponent<HeroAnimationController>();
		component.Play(component2.GetClip(this.IsOverHero ? "Exit" : "Enter"));
		this.DoFadeOut();
		hc.ForceWalkingSound = true;
		if (this.isTransitionWaiting)
		{
			yield return new WaitUntil(() => !this.isTransitionWaiting);
		}
		else
		{
			this.gm.screenFader_fsm.SendEvent("SCENE FADE OUT");
		}
		yield return new WaitForSeconds(0.5f);
		hc.ForceWalkingSound = false;
		hc.StartAnimationControl();
		if (InteractManager.BlockingInteractable == this)
		{
			InteractManager.BlockingInteractable = null;
		}
		base.Deactivate(false);
		this.DoSceneTransition(false);
		yield break;
	}

	// Token: 0x060036C3 RID: 14019 RVA: 0x000F1C08 File Offset: 0x000EFE08
	public void SetTransitionWait(bool value)
	{
		this.isTransitionWaiting = value;
	}

	// Token: 0x060036C4 RID: 14020 RVA: 0x000F1C11 File Offset: 0x000EFE11
	public void PrepareEntry()
	{
		if (ProjectBenchmark.IsRunning)
		{
			return;
		}
		if (!this.customEntryFSM)
		{
			return;
		}
		this.customEntryFSM.SendEvent("PREPARE ENTRY");
	}

	// Token: 0x060036C5 RID: 14021 RVA: 0x000F1C39 File Offset: 0x000EFE39
	public void BeforeEntry()
	{
		if (ProjectBenchmark.IsRunning)
		{
			return;
		}
		if (!this.customEntryFSM)
		{
			return;
		}
		this.customEntryFSM.SendEvent("START ENTRY");
	}

	// Token: 0x060036C6 RID: 14022 RVA: 0x000F1C61 File Offset: 0x000EFE61
	public void AfterEntry()
	{
		if (ProjectBenchmark.IsRunning)
		{
			return;
		}
		if (!this.customEntryFSM)
		{
			return;
		}
		this.customEntryFSM.SendEvent("FINISH ENTRY");
	}

	// Token: 0x060036C7 RID: 14023 RVA: 0x000F1C8C File Offset: 0x000EFE8C
	public void SetIsInactive(bool value)
	{
		bool flag = this.EnableInteractableFields();
		this.isInactive = value;
		bool flag2 = this.EnableInteractableFields();
		if (flag)
		{
			if (!flag2)
			{
				base.Deactivate(false);
				return;
			}
		}
		else if (flag2)
		{
			base.Activate();
		}
	}

	// Token: 0x04003974 RID: 14708
	private GameManager gm;

	// Token: 0x04003975 RID: 14709
	private bool activated;

	// Token: 0x04003976 RID: 14710
	[Header("Door Type Gate Settings")]
	[Space(5f)]
	public bool isInactive;

	// Token: 0x04003977 RID: 14711
	public bool isADoor;

	// Token: 0x04003978 RID: 14712
	public bool dontWalkOutOfDoor;

	// Token: 0x04003979 RID: 14713
	public bool IsOverHero;

	// Token: 0x0400397A RID: 14714
	[Header("Gate Entry")]
	[UnityEngine.Tooltip("The wait time before entering from this gate (not the target gate).")]
	public float entryDelay;

	// Token: 0x0400397B RID: 14715
	public bool alwaysEnterRight;

	// Token: 0x0400397C RID: 14716
	public bool alwaysEnterLeft;

	// Token: 0x0400397D RID: 14717
	public PlayMakerFSM customEntryFSM;

	// Token: 0x0400397E RID: 14718
	[Header("Force Hard Land (Top Gates Only)")]
	[Space(5f)]
	public bool hardLandOnExit;

	// Token: 0x0400397F RID: 14719
	[Header("Destination Scene")]
	[Space(5f)]
	public string targetScene;

	// Token: 0x04003980 RID: 14720
	public string entryPoint;

	// Token: 0x04003981 RID: 14721
	[SerializeField]
	private bool skipSceneMapCheck;

	// Token: 0x04003982 RID: 14722
	public Vector2 entryOffset;

	// Token: 0x04003983 RID: 14723
	[SerializeField]
	private bool alwaysUnloadUnusedAssets;

	// Token: 0x04003984 RID: 14724
	public PlayMakerFSM customFadeFSM;

	// Token: 0x04003985 RID: 14725
	public PlayMakerFSM additionalFadeFSM;

	// Token: 0x04003986 RID: 14726
	[Space]
	[SerializeField]
	private GuidReferenceHolder cutsceneFsmHolder;

	// Token: 0x04003987 RID: 14727
	[Header("Hazard Respawn")]
	[Space(5f)]
	public bool nonHazardGate;

	// Token: 0x04003988 RID: 14728
	public HazardRespawnMarker respawnMarker;

	// Token: 0x04003989 RID: 14729
	[Header("Set Audio Snapshots")]
	[Space(5f)]
	public AudioMixerSnapshot atmosSnapshot;

	// Token: 0x0400398A RID: 14730
	public AudioMixerSnapshot enviroSnapshot;

	// Token: 0x0400398B RID: 14731
	public AudioMixerSnapshot actorSnapshot;

	// Token: 0x0400398C RID: 14732
	public AudioMixerSnapshot musicSnapshot;

	// Token: 0x0400398D RID: 14733
	public float AudioTransitionTime = 1.5f;

	// Token: 0x0400398E RID: 14734
	[Header("Cosmetics")]
	public GameManager.SceneLoadVisualizations sceneLoadVisualization;

	// Token: 0x0400398F RID: 14735
	public bool customFade;

	// Token: 0x04003990 RID: 14736
	public bool forceWaitFetch;

	// Token: 0x04003991 RID: 14737
	[Space]
	public UnityEvent OnDoorEnter;

	// Token: 0x04003992 RID: 14738
	private Collider2D collider;

	// Token: 0x04003993 RID: 14739
	private AsyncOperationHandle<IList<IResourceLocation>> targetSceneResourceHandle;

	// Token: 0x04003994 RID: 14740
	private string lastResourceLocationScene;

	// Token: 0x04003995 RID: 14741
	private IResourceLocation targetSceneResourceLocation;

	// Token: 0x04003996 RID: 14742
	private bool isTransitionWaiting;

	// Token: 0x0400399A RID: 14746
	private bool ignoredInput;

	// Token: 0x02001902 RID: 6402
	private class SceneLoadInfo : GameManager.SceneLoadInfo
	{
		// Token: 0x060092D4 RID: 37588 RVA: 0x0029C9C9 File Offset: 0x0029ABC9
		public override void NotifyFadedOut()
		{
			Action fadedOut = this.FadedOut;
			if (fadedOut != null)
			{
				fadedOut();
			}
			base.NotifyFadedOut();
		}

		// Token: 0x060092D5 RID: 37589 RVA: 0x0029C9E2 File Offset: 0x0029ABE2
		public override bool IsReadyToActivate()
		{
			Func<bool> canActivateFunc = this.CanActivateFunc;
			if (canActivateFunc == null)
			{
				return base.IsReadyToActivate();
			}
			return canActivateFunc();
		}

		// Token: 0x0400941C RID: 37916
		public Action FadedOut;

		// Token: 0x0400941D RID: 37917
		public Func<bool> CanActivateFunc;
	}

	// Token: 0x02001903 RID: 6403
	// (Invoke) Token: 0x060092D8 RID: 37592
	public delegate void BeforeTransitionEvent();
}
