using System;
using System.Collections.Generic;
using System.Text;
using TeamCherry.BuildBot;
using TeamCherry.SharedUtils;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

// Token: 0x0200035C RID: 860
public class PerformanceHud : MonoBehaviour, IOnGUI
{
	// Token: 0x170002F2 RID: 754
	// (get) Token: 0x06001DA8 RID: 7592 RVA: 0x00088C37 File Offset: 0x00086E37
	// (set) Token: 0x06001DA9 RID: 7593 RVA: 0x00088C3E File Offset: 0x00086E3E
	public static PerformanceHud Shared { get; private set; }

	// Token: 0x06001DAA RID: 7594 RVA: 0x00088C46 File Offset: 0x00086E46
	public static void Init()
	{
		if (PerformanceHud.Shared != null)
		{
			return;
		}
		GameObject gameObject = new GameObject("PerformanceHud");
		PerformanceHud.Shared = gameObject.AddComponent<PerformanceHud>();
		PerformanceHud.Shared.DisplayState = PerformanceHud.DisplayStates.Hidden;
		Object.DontDestroyOnLoad(gameObject);
	}

	// Token: 0x06001DAB RID: 7595 RVA: 0x00088C7B File Offset: 0x00086E7B
	public static void ReInit()
	{
		if (PerformanceHud.Shared)
		{
			Object.Destroy(PerformanceHud.Shared.gameObject);
			PerformanceHud.Shared = null;
		}
		PerformanceHud.Init();
	}

	// Token: 0x170002F3 RID: 755
	// (get) Token: 0x06001DAC RID: 7596 RVA: 0x00088CA3 File Offset: 0x00086EA3
	// (set) Token: 0x06001DAD RID: 7597 RVA: 0x00088CAA File Offset: 0x00086EAA
	public static bool ShowVibration
	{
		get
		{
			return PerformanceHud._showVibrations;
		}
		set
		{
			if (PerformanceHud._showVibrations != value)
			{
				PerformanceHud._showVibrations = value;
				if (PerformanceHud.Shared != null)
				{
					PerformanceHud.Shared.UpdateDrawState();
				}
			}
		}
	}

	// Token: 0x170002F4 RID: 756
	// (get) Token: 0x06001DAE RID: 7598 RVA: 0x00088CD1 File Offset: 0x00086ED1
	private GUIStyle RightAlignedStyle
	{
		get
		{
			if (this.rightAlignedStyle == null)
			{
				this.rightAlignedStyle = new GUIStyle(GUI.skin.label);
				this.rightAlignedStyle.alignment = TextAnchor.MiddleRight;
			}
			return this.rightAlignedStyle;
		}
	}

	// Token: 0x170002F5 RID: 757
	// (get) Token: 0x06001DAF RID: 7599 RVA: 0x00088D02 File Offset: 0x00086F02
	private static int LineHeight
	{
		get
		{
			return Mathf.RoundToInt(24f * CheatManager.Multiplier);
		}
	}

	// Token: 0x170002F6 RID: 758
	// (get) Token: 0x06001DB0 RID: 7600 RVA: 0x00088D14 File Offset: 0x00086F14
	// (set) Token: 0x06001DB1 RID: 7601 RVA: 0x00088D1C File Offset: 0x00086F1C
	public bool IsMonoGroupEnabled
	{
		get
		{
			return this.isMonoGroupEnabled;
		}
		set
		{
			this.isMonoGroupEnabled = value;
			if (this.monoGroup != null)
			{
				this.monoGroup.style.display = (value ? DisplayStyle.Flex : DisplayStyle.None);
			}
		}
	}

	// Token: 0x170002F7 RID: 759
	// (get) Token: 0x06001DB2 RID: 7602 RVA: 0x00088D49 File Offset: 0x00086F49
	// (set) Token: 0x06001DB3 RID: 7603 RVA: 0x00088D54 File Offset: 0x00086F54
	public PerformanceHud.DisplayStates DisplayState
	{
		get
		{
			return this.displayState;
		}
		set
		{
			if (this.displayState == value)
			{
				return;
			}
			PerformanceHud.DisplayStates displayStates = this.displayState;
			this.displayState = value;
			this.UpdateDrawState();
			if (this.displayState == PerformanceHud.DisplayStates.Hidden)
			{
				if (this.uiDoc)
				{
					this.uiDoc.visualTreeAsset = null;
					this.sceneLabel = null;
					this.cpuMemLabel = null;
					this.resolutionLabel = null;
					this.fpsLabel = null;
					this.profileLabel = null;
					this.monoGroup = null;
					this.gcLabel = null;
					this.heapLabel = null;
					this.fpsStringValues = null;
					this.lastScreenHeight = 0;
					this.lastScreenWidth = 0;
					this.lastScreenHeightScaled = 0;
					this.lastScreenWidthScaled = 0;
					this.previousFpsValue = 0;
					this.lastGcMode = null;
				}
				return;
			}
			if (displayStates != PerformanceHud.DisplayStates.Hidden)
			{
				return;
			}
			if (!this.uiDoc)
			{
				this.uiDoc = base.gameObject.AddComponent<UIDocument>();
				this.uiDoc.panelSettings = Object.Instantiate<PanelSettings>(Resources.Load<PanelSettings>("DebugPanelSettings"));
			}
			VisualTreeAsset visualTreeAsset = Resources.Load<VisualTreeAsset>("PerformanceHud");
			StyleSheet styleSheet = Resources.Load<StyleSheet>("PerformanceHud");
			this.uiDoc.visualTreeAsset = visualTreeAsset;
			this.uiDoc.rootVisualElement.styleSheets.Add(styleSheet);
			VisualElement rootVisualElement = this.uiDoc.rootVisualElement;
			TextElement textElement = rootVisualElement.Q("revisionLabel", null);
			BuildMetadata embedded = BuildMetadata.Embedded;
			textElement.text = ((embedded != null) ? ("r" + embedded.Revision + " - " + embedded.MachineName) : "No Build Metadata");
			this.sceneLabel = rootVisualElement.Q("sceneLabel", null);
			this.cpuMemLabel = rootVisualElement.Q("cpuMemLabel", null);
			this.resolutionLabel = rootVisualElement.Q("resolutionLabel", null);
			this.fpsLabel = rootVisualElement.Q("fpsLabel", null);
			this.profileLabel = rootVisualElement.Q("profileLabel", null);
			this.monoGroup = rootVisualElement.Q("monoGroup", null);
			this.IsMonoGroupEnabled = this.IsMonoGroupEnabled;
			this.gcLabel = rootVisualElement.Q("gcLabel", null);
			this.UpdateGCMode(GarbageCollector.GCMode);
			this.heapLabel = rootVisualElement.Q("monoHeapLabel", null);
			this.UpdateAll();
			this.UpdateScene();
		}
	}

	// Token: 0x170002F8 RID: 760
	// (get) Token: 0x06001DB4 RID: 7604 RVA: 0x00088F89 File Offset: 0x00087189
	// (set) Token: 0x06001DB5 RID: 7605 RVA: 0x00088F94 File Offset: 0x00087194
	public bool EnableProfileRecording
	{
		get
		{
			return this.isProfileRecordingEnabled;
		}
		set
		{
			if (value == this.isProfileRecordingEnabled)
			{
				return;
			}
			this.isProfileRecordingEnabled = value;
			if (value)
			{
				this.setPassCallRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count", 1, ProfilerRecorderOptions.Default);
				this.trianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count", 1, ProfilerRecorderOptions.Default);
			}
			else
			{
				this.setPassCallRecorder.Dispose();
				this.trianglesRecorder.Dispose();
			}
			this.UpdateResolution();
		}
	}

	// Token: 0x06001DB6 RID: 7606 RVA: 0x00089004 File Offset: 0x00087204
	protected void Awake()
	{
		this.frameCounter = 0;
		this.lastSecond = (int)Time.realtimeSinceStartup;
		this.framesColor = Color.gray;
		this.memoryContent = new GUIContent("N/A");
		this.loadReports = new List<PerformanceHud.LoadReport>();
		this.vibrationHudDrawer = new PerformanceHud.VibrationHudDrawer(this);
	}

	// Token: 0x06001DB7 RID: 7607 RVA: 0x00089058 File Offset: 0x00087258
	protected void OnEnable()
	{
		GameManager.SceneTransitionBegan += this.GameManager_SceneTransitionBegan;
		SceneManager.activeSceneChanged += this.OnActiveSceneChanged;
		SceneManager.sceneLoaded += this.OnSceneLoaded;
		SceneManager.sceneUnloaded += this.OnSceneUnloaded;
		GarbageCollector.GCModeChanged += this.UpdateGCMode;
	}

	// Token: 0x06001DB8 RID: 7608 RVA: 0x000890BC File Offset: 0x000872BC
	protected void OnDisable()
	{
		GameManager.SceneTransitionBegan -= this.GameManager_SceneTransitionBegan;
		SceneManager.activeSceneChanged -= this.OnActiveSceneChanged;
		SceneManager.sceneLoaded -= this.OnSceneLoaded;
		SceneManager.sceneUnloaded -= this.OnSceneUnloaded;
		GarbageCollector.GCModeChanged -= this.UpdateGCMode;
		this.EnableProfileRecording = false;
	}

	// Token: 0x06001DB9 RID: 7609 RVA: 0x00089125 File Offset: 0x00087325
	private void OnDestroy()
	{
		this.ToggleDraw(false);
	}

	// Token: 0x06001DBA RID: 7610 RVA: 0x00089130 File Offset: 0x00087330
	protected void Update()
	{
		if (this.DisplayState == PerformanceHud.DisplayStates.Hidden)
		{
			return;
		}
		this.frameCounter++;
		int num = (int)Time.realtimeSinceStartup;
		if (num != this.lastSecond)
		{
			this.framesLastSecond = this.frameCounter;
			int num2 = this.framesLastSecond;
			Color color;
			if (num2 < 58)
			{
				if (num2 < 50)
				{
					color = Color.red;
				}
				else
				{
					color = Color.yellow;
				}
			}
			else
			{
				color = Color.green;
			}
			this.framesColor = color;
			this.lastSecond = num;
			this.frameCounter = 0;
			this.UpdateAll();
		}
		if (PerformanceHud.ShowVibration)
		{
			this.vibrationHudDrawer.Update();
		}
	}

	// Token: 0x06001DBB RID: 7611 RVA: 0x000891C6 File Offset: 0x000873C6
	private void ToggleDraw(bool draw)
	{
		if (this.isDrawing != draw)
		{
			this.isDrawing = draw;
			if (draw)
			{
				GUIDrawer.AddDrawer(this);
			}
			else
			{
				GUIDrawer.RemoveDrawer(this);
			}
			if (this.uiDoc)
			{
				this.uiDoc.enabled = draw;
			}
		}
	}

	// Token: 0x06001DBC RID: 7612 RVA: 0x00089202 File Offset: 0x00087402
	private void UpdateDrawState()
	{
		if (this.displayState == PerformanceHud.DisplayStates.Hidden)
		{
			this.ToggleDraw(PerformanceHud.ShowVibration);
			return;
		}
		this.ToggleDraw(true);
	}

	// Token: 0x06001DBD RID: 7613 RVA: 0x00089220 File Offset: 0x00087420
	private void GameManager_SceneTransitionBegan(SceneLoad sceneLoad)
	{
		PerformanceHud.LoadReport loadReport = new PerformanceHud.LoadReport
		{
			Color = Color.white,
			Content = new GUIContent()
		};
		this.loadReports.Add(loadReport);
		while (this.loadReports.Count > 2)
		{
			this.loadReports.RemoveAt(0);
		}
		sceneLoad.FetchComplete += delegate()
		{
			PerformanceHud.UpdateSceneLoadRecordContent(sceneLoad, loadReport);
		};
		sceneLoad.ActivationComplete += delegate()
		{
			PerformanceHud.UpdateSceneLoadRecordContent(sceneLoad, loadReport);
		};
		sceneLoad.Complete += delegate()
		{
			PerformanceHud.UpdateSceneLoadRecordContent(sceneLoad, loadReport);
		};
		sceneLoad.StartCalled += delegate()
		{
			PerformanceHud.UpdateSceneLoadRecordContent(sceneLoad, loadReport);
		};
		sceneLoad.BossLoaded += delegate()
		{
			PerformanceHud.UpdateSceneLoadRecordContent(sceneLoad, loadReport);
		};
		sceneLoad.Finish += delegate()
		{
			PerformanceHud.UpdateSceneLoadRecordContent(sceneLoad, loadReport);
		};
		PerformanceHud.UpdateSceneLoadRecordContent(sceneLoad, loadReport);
	}

	// Token: 0x06001DBE RID: 7614 RVA: 0x00089324 File Offset: 0x00087524
	private static void UpdateSceneLoadRecordContent(SceneLoad sceneLoad, PerformanceHud.LoadReport report)
	{
		StringBuilder tempStringBuilder = global::Helper.GetTempStringBuilder(sceneLoad.TargetSceneName);
		tempStringBuilder.Append(":    ");
		float num = 0f;
		for (int i = 0; i < 9; i++)
		{
			SceneLoad.Phases phase = (SceneLoad.Phases)i;
			float? duration = sceneLoad.GetDuration(phase);
			if (duration != null && duration.Value > Mathf.Epsilon)
			{
				tempStringBuilder.Append(phase.ToString());
				tempStringBuilder.Append(": ");
				tempStringBuilder.Append(duration.Value.ToString("0.00s"));
				tempStringBuilder.Append("    ");
				num += duration.Value;
			}
		}
		if (num > Mathf.Epsilon)
		{
			tempStringBuilder.Append("Total: ");
			tempStringBuilder.Append(num.ToString("0.00s"));
		}
		Color color;
		if (num <= 3.5f)
		{
			if (num <= 3f)
			{
				color = Color.white;
			}
			else
			{
				color = Color.yellow;
			}
		}
		else
		{
			color = Color.red;
		}
		report.Color = color;
		report.Content.text = tempStringBuilder.ToString();
	}

	// Token: 0x06001DBF RID: 7615 RVA: 0x0008943B File Offset: 0x0008763B
	private void UpdateAll()
	{
		this.UpdateMemory();
		this.UpdateResolution();
	}

	// Token: 0x06001DC0 RID: 7616 RVA: 0x0008944C File Offset: 0x0008764C
	private void UpdateMemory()
	{
		double num = (double)GCManager.GetMemoryUsage() / 1024.0 / 1024.0;
		double num2 = (double)GCManager.GetMemoryTotal() / 1024.0 / 1024.0;
		double num3 = (double)((long)SystemInfo.systemMemorySize);
		if (this.cpuMemLabel != null)
		{
			this.cpuMemLabel.text = string.Format("CPU Mem.: {0:n} / {1:n} / {2:n}", num, num2, num3);
		}
		Label label = this.heapLabel;
		if (label != null && label.visible)
		{
			double num4 = (double)GCManager.GetMonoHeapUsage() / 1024.0 / 1024.0;
			double num5 = (double)GCManager.GetMonoHeapTotal() / 1024.0 / 1024.0;
			this.heapLabel.text = string.Format("Heap: {0:n} / {1:n} / {2:n}", num4, num5, GCManager.HeapUsageThreshold);
		}
	}

	// Token: 0x06001DC1 RID: 7617 RVA: 0x00089540 File Offset: 0x00087740
	private void UpdateGCMode(GarbageCollector.Mode mode)
	{
		if (this.gcLabel != null)
		{
			GarbageCollector.Mode mode2 = mode;
			GarbageCollector.Mode? mode3 = this.lastGcMode;
			if (!(mode2 == mode3.GetValueOrDefault() & mode3 != null))
			{
				this.gcLabel.text = "GC: " + mode.ToString();
				this.lastGcMode = new GarbageCollector.Mode?(mode);
				return;
			}
		}
	}

	// Token: 0x06001DC2 RID: 7618 RVA: 0x0008959F File Offset: 0x0008779F
	private void OnActiveSceneChanged(Scene fromScene, Scene toScene)
	{
		this.UpdateScene();
	}

	// Token: 0x06001DC3 RID: 7619 RVA: 0x000895A7 File Offset: 0x000877A7
	private void OnSceneUnloaded(Scene arg0)
	{
		this.UpdateScene();
	}

	// Token: 0x06001DC4 RID: 7620 RVA: 0x000895AF File Offset: 0x000877AF
	private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
	{
		this.UpdateScene();
	}

	// Token: 0x06001DC5 RID: 7621 RVA: 0x000895B8 File Offset: 0x000877B8
	private void UpdateScene()
	{
		if (this.sceneInfoBuilder == null)
		{
			this.sceneInfoBuilder = new StringBuilder();
		}
		else
		{
			this.sceneInfoBuilder.Clear();
		}
		Scene activeScene = SceneManager.GetActiveScene();
		bool flag = false;
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			Scene sceneAt = SceneManager.GetSceneAt(i);
			if (sceneAt.isLoaded)
			{
				if (flag)
				{
					this.sceneInfoBuilder.Append(" + ");
				}
				bool flag2 = false;
				if (sceneAt != activeScene)
				{
					this.sceneInfoBuilder.Append("<color=#808080>");
					flag2 = true;
				}
				this.sceneInfoBuilder.Append(sceneAt.name);
				flag = true;
				if (flag2)
				{
					this.sceneInfoBuilder.Append("</color>");
				}
			}
		}
		if (this.sceneLabel != null)
		{
			this.sceneLabel.text = this.sceneInfoBuilder.ToString();
		}
	}

	// Token: 0x06001DC6 RID: 7622 RVA: 0x0008968C File Offset: 0x0008788C
	private void UpdateResolution()
	{
		if (this.profileLabel != null)
		{
			if (this.isProfileRecordingEnabled)
			{
				this.profileLabel.text = string.Format("- SetPass Calls: {0}, Triangles: {1}", this.setPassCallRecorder.LastValue, this.trianglesRecorder.LastValue);
				this.profileLabel.style.display = DisplayStyle.Flex;
			}
			else
			{
				this.profileLabel.style.display = DisplayStyle.None;
			}
		}
		int width = Screen.width;
		int height = Screen.height;
		ScreenRes resolution = CameraRenderScaled.Resolution;
		if (this.resolutionLabel != null && (width != this.lastScreenWidth || height != this.lastScreenHeight || resolution.Width != this.lastScreenWidthScaled || resolution.Height != this.lastScreenHeightScaled))
		{
			this.resolutionLabel.text = string.Format("{0}x{1} [{2}x{3}]", new object[]
			{
				width,
				height,
				resolution.Width,
				resolution.Height
			});
			this.lastScreenWidth = width;
			this.lastScreenHeight = height;
			this.lastScreenWidthScaled = resolution.Width;
			this.lastScreenHeightScaled = resolution.Height;
			this.uiDoc.panelSettings.scale = CheatManager.Multiplier;
		}
		if (this.fpsLabel != null && this.framesLastSecond != this.previousFpsValue)
		{
			if (this.fpsStringValues == null)
			{
				this.fpsStringValues = new Dictionary<int, string>(60);
			}
			string text;
			if (!this.fpsStringValues.TryGetValue(this.framesLastSecond, out text))
			{
				text = (this.fpsStringValues[this.framesLastSecond] = this.framesLastSecond.ToString());
			}
			this.fpsLabel.text = text;
			this.fpsLabel.style.color = this.framesColor;
			this.previousFpsValue = this.framesLastSecond;
		}
	}

	// Token: 0x170002F9 RID: 761
	// (get) Token: 0x06001DC7 RID: 7623 RVA: 0x00089872 File Offset: 0x00087A72
	public int GUIDepth
	{
		get
		{
			return 1;
		}
	}

	// Token: 0x06001DC8 RID: 7624 RVA: 0x00089878 File Offset: 0x00087A78
	public void DrawGUI()
	{
		if (PerformanceHud.ShowVibration)
		{
			this.vibrationHudDrawer.OnGUI();
		}
		this.rightLineIndex = 1;
		if (this.DisplayState != PerformanceHud.DisplayStates.Full)
		{
			return;
		}
		int num = this.IsMonoGroupEnabled ? 4 : 3;
		this.OnGUIFull(ref num);
		GUI.color = Color.white;
		PerformanceHud.LabelWithShadow(new GUIContent("Boost Mode: " + (CheatManager.BoostModeActive ? "Enabled" : "Disabled")), ref num);
		MazeController newestInstance = MazeController.NewestInstance;
		if (newestInstance)
		{
			PerformanceHud.LabelWithShadowRight(new GUIContent(string.Format("Incorrect Doors Left: {0}", newestInstance.IncorrectDoorsLeft)), ref this.rightLineIndex);
			PerformanceHud.LabelWithShadowRight(new GUIContent(string.Format("Correct Doors Left: {0}", newestInstance.CorrectDoorsLeft)), ref this.rightLineIndex);
			foreach (TransitionPoint transitionPoint in newestInstance.EnumerateCorrectDoors())
			{
				PerformanceHud.LabelWithShadowRight(new GUIContent("Correct Door: " + (transitionPoint ? transitionPoint.gameObject.name : "none")), ref this.rightLineIndex);
			}
		}
	}

	// Token: 0x06001DC9 RID: 7625 RVA: 0x000899B8 File Offset: 0x00087BB8
	private void OnGUIFull(ref int lineIndex)
	{
		GUI.color = Color.white;
		PerformanceHud.LabelWithShadow(this.memoryContent, ref lineIndex);
		foreach (PerformanceHud.LoadReport loadReport in this.loadReports)
		{
			GUI.color = loadReport.Color;
			PerformanceHud.LabelWithShadow(loadReport.Content, ref lineIndex);
		}
		if (GameManager.instance && GameManager.instance.sm)
		{
			CustomSceneManager sm = GameManager.instance.sm;
			string text = string.Format("Saturation: {0}, Adjusted: {1}", sm.saturation, sm.AdjustSaturation(sm.saturation));
			GUI.color = Color.white;
			PerformanceHud.LabelWithShadow(new GUIContent(text), ref lineIndex);
		}
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		if (unsafeInstance)
		{
			PerformanceHud.LabelWithShadow(new GUIContent("MapZone: " + unsafeInstance.GetCurrentMapZone()), ref lineIndex);
		}
		GUI.color = Color.white;
		PerformanceHud.LabelWithShadow(new GUIContent("Interaction: " + (InteractManager.CanInteract ? "Enabled" : "Disabled") + ", Blocked by: " + (InteractManager.BlockingInteractable ? InteractManager.BlockingInteractable.gameObject.name : "None")), ref lineIndex);
	}

	// Token: 0x170002FA RID: 762
	// (get) Token: 0x06001DCA RID: 7626 RVA: 0x00089B14 File Offset: 0x00087D14
	public static float ScreenEdgePadding
	{
		get
		{
			return 5f * CheatManager.Multiplier;
		}
	}

	// Token: 0x06001DCB RID: 7627 RVA: 0x00089B21 File Offset: 0x00087D21
	private static void LabelWithShadow(GUIContent content, ref int lineIndex)
	{
		lineIndex++;
		PerformanceHud.LabelWithShadow(new Rect(PerformanceHud.ScreenEdgePadding, (float)(Screen.height - PerformanceHud.LineHeight * lineIndex) - PerformanceHud.ScreenEdgePadding, (float)Screen.width - PerformanceHud.ScreenEdgePadding, (float)PerformanceHud.LineHeight), content);
	}

	// Token: 0x06001DCC RID: 7628 RVA: 0x00089B60 File Offset: 0x00087D60
	private static void LabelWithShadowRight(GUIContent content, ref int lineIndex)
	{
		lineIndex++;
		Vector2 vector = CheatManager.LabelStyle.CalcSize(content);
		PerformanceHud.LabelWithShadow(new Rect((float)Screen.width - vector.x - PerformanceHud.ScreenEdgePadding, (float)(Screen.height - PerformanceHud.LineHeight * lineIndex) - PerformanceHud.ScreenEdgePadding, vector.x, (float)PerformanceHud.LineHeight), content);
	}

	// Token: 0x06001DCD RID: 7629 RVA: 0x00089BC0 File Offset: 0x00087DC0
	private static void LabelWithShadow(Rect rect, GUIContent content)
	{
		GUIStyle labelStyle = CheatManager.LabelStyle;
		Vector2 vector = labelStyle.CalcSize(content);
		Color color = GUI.color;
		try
		{
			GUI.color = new Color(0f, 0f, 0f, 0.5f);
			GUI.DrawTexture(new Rect(rect.x, rect.y, vector.x, rect.height), Texture2D.whiteTexture);
			GUI.color = Color.black;
			GUI.Label(new Rect(rect.x + 2f, rect.y + 2f, rect.width, rect.height), content, labelStyle);
			GUI.color = color;
			GUI.Label(new Rect(rect.x + 0f, rect.y + 0f, rect.width, rect.height), content, labelStyle);
		}
		finally
		{
			GUI.color = color;
		}
	}

	// Token: 0x04001CDA RID: 7386
	private int frameCounter;

	// Token: 0x04001CDB RID: 7387
	private int lastSecond;

	// Token: 0x04001CDC RID: 7388
	private int framesLastSecond;

	// Token: 0x04001CDD RID: 7389
	private Color framesColor;

	// Token: 0x04001CDE RID: 7390
	private UIDocument uiDoc;

	// Token: 0x04001CDF RID: 7391
	private Label sceneLabel;

	// Token: 0x04001CE0 RID: 7392
	private Label cpuMemLabel;

	// Token: 0x04001CE1 RID: 7393
	private Label resolutionLabel;

	// Token: 0x04001CE2 RID: 7394
	private Label fpsLabel;

	// Token: 0x04001CE3 RID: 7395
	private Label profileLabel;

	// Token: 0x04001CE4 RID: 7396
	private VisualElement monoGroup;

	// Token: 0x04001CE5 RID: 7397
	private Label gcLabel;

	// Token: 0x04001CE6 RID: 7398
	private Label heapLabel;

	// Token: 0x04001CE7 RID: 7399
	private int previousFpsValue;

	// Token: 0x04001CE8 RID: 7400
	private Dictionary<int, string> fpsStringValues;

	// Token: 0x04001CE9 RID: 7401
	private int lastScreenWidth;

	// Token: 0x04001CEA RID: 7402
	private int lastScreenHeight;

	// Token: 0x04001CEB RID: 7403
	private int lastScreenWidthScaled;

	// Token: 0x04001CEC RID: 7404
	private int lastScreenHeightScaled;

	// Token: 0x04001CED RID: 7405
	private StringBuilder sceneInfoBuilder;

	// Token: 0x04001CEE RID: 7406
	private GUIContent memoryContent;

	// Token: 0x04001CEF RID: 7407
	private GarbageCollector.Mode? lastGcMode;

	// Token: 0x04001CF0 RID: 7408
	private List<PerformanceHud.LoadReport> loadReports;

	// Token: 0x04001CF1 RID: 7409
	private static bool _showVibrations;

	// Token: 0x04001CF2 RID: 7410
	private bool isProfileRecordingEnabled;

	// Token: 0x04001CF3 RID: 7411
	private ProfilerRecorder setPassCallRecorder;

	// Token: 0x04001CF4 RID: 7412
	private ProfilerRecorder trianglesRecorder;

	// Token: 0x04001CF5 RID: 7413
	private GUIStyle rightAlignedStyle;

	// Token: 0x04001CF6 RID: 7414
	private bool isDrawing;

	// Token: 0x04001CF7 RID: 7415
	private const int LINE_HEIGHT = 24;

	// Token: 0x04001CF8 RID: 7416
	private PerformanceHud.VibrationHudDrawer vibrationHudDrawer;

	// Token: 0x04001CF9 RID: 7417
	private int rightLineIndex;

	// Token: 0x04001CFA RID: 7418
	private PerformanceHud.DisplayStates displayState;

	// Token: 0x04001CFB RID: 7419
	private bool isMonoGroupEnabled = true;

	// Token: 0x04001CFC RID: 7420
	private const float SCREEN_EDGE_PADDING = 5f;

	// Token: 0x02001611 RID: 5649
	private class LoadReport
	{
		// Token: 0x0400899A RID: 35226
		public Color Color;

		// Token: 0x0400899B RID: 35227
		public GUIContent Content;
	}

	// Token: 0x02001612 RID: 5650
	public enum DisplayStates
	{
		// Token: 0x0400899D RID: 35229
		Hidden,
		// Token: 0x0400899E RID: 35230
		Minimal,
		// Token: 0x0400899F RID: 35231
		Full
	}

	// Token: 0x02001613 RID: 5651
	private class VibrationHudDrawer
	{
		// Token: 0x17000E14 RID: 3604
		// (get) Token: 0x060088D2 RID: 35026 RVA: 0x0027B771 File Offset: 0x00279971
		public GUIContent VibrationsContent
		{
			get
			{
				return this.vibrationsContent;
			}
		}

		// Token: 0x060088D3 RID: 35027 RVA: 0x0027B779 File Offset: 0x00279979
		public VibrationHudDrawer(PerformanceHud performanceHud)
		{
			this.performanceHud = performanceHud;
		}

		// Token: 0x060088D4 RID: 35028 RVA: 0x0027B7B0 File Offset: 0x002799B0
		public void Update()
		{
			VibrationMixer mixer = VibrationManager.GetMixer();
			if (mixer != null)
			{
				for (int i = 0; i < mixer.PlayingEmissionCount; i++)
				{
					VibrationEmission playingEmission = mixer.GetPlayingEmission(i);
					if (this.activeEmissions.Add(playingEmission))
					{
						this.trackers.Add(new PerformanceHud.VibrationHudDrawer.VibrationTracker(playingEmission));
					}
				}
			}
			for (int j = this.trackers.Count - 1; j >= 0; j--)
			{
				PerformanceHud.VibrationHudDrawer.VibrationTracker vibrationTracker = this.trackers[j];
				if (!vibrationTracker.Update())
				{
					this.activeEmissions.Remove(vibrationTracker.Emission);
					this.trackers.RemoveAt(j);
				}
			}
		}

		// Token: 0x060088D5 RID: 35029 RVA: 0x0027B84C File Offset: 0x00279A4C
		public void OnGUI()
		{
			GUI.color = Color.white;
			for (int i = this.trackers.Count - 1; i >= 0; i--)
			{
				PerformanceHud.VibrationHudDrawer.VibrationTracker vibrationTracker = this.trackers[i];
				this.vibrationsContent.text = vibrationTracker.ToString();
				PerformanceHud.LabelWithShadowRight(this.vibrationsContent, ref this.performanceHud.rightLineIndex);
			}
			if (this.trackers.Count > 0)
			{
				this.performanceHud.rightLineIndex++;
			}
		}

		// Token: 0x040089A0 RID: 35232
		private PerformanceHud performanceHud;

		// Token: 0x040089A1 RID: 35233
		private const float DISPLAY_TIME = 5f;

		// Token: 0x040089A2 RID: 35234
		private HashSet<VibrationEmission> activeEmissions = new HashSet<VibrationEmission>();

		// Token: 0x040089A3 RID: 35235
		private List<PerformanceHud.VibrationHudDrawer.VibrationTracker> trackers = new List<PerformanceHud.VibrationHudDrawer.VibrationTracker>();

		// Token: 0x040089A4 RID: 35236
		private GUIContent vibrationsContent = new GUIContent("");

		// Token: 0x02001C12 RID: 7186
		private class VibrationTracker
		{
			// Token: 0x170011BF RID: 4543
			// (get) Token: 0x06009AC3 RID: 39619 RVA: 0x002B4451 File Offset: 0x002B2651
			public VibrationEmission Emission
			{
				get
				{
					return this.emission;
				}
			}

			// Token: 0x06009AC4 RID: 39620 RVA: 0x002B4459 File Offset: 0x002B2659
			public VibrationTracker(VibrationEmission emission)
			{
				this.emission = emission;
				this.timer = 5f;
			}

			// Token: 0x06009AC5 RID: 39621 RVA: 0x002B4473 File Offset: 0x002B2673
			public bool Update()
			{
				if (this.Emission == null)
				{
					return false;
				}
				if (!this.Emission.IsPlaying)
				{
					this.timer -= Time.deltaTime;
				}
				return this.timer > 0f;
			}

			// Token: 0x06009AC6 RID: 39622 RVA: 0x002B44AB File Offset: 0x002B26AB
			public override string ToString()
			{
				if (this.emission == null)
				{
					return "Empty";
				}
				if (this.emission.IsPlaying)
				{
					return string.Format("{0}", this.emission);
				}
				return string.Format("{0} finished", this.emission);
			}

			// Token: 0x06009AC7 RID: 39623 RVA: 0x002B44EC File Offset: 0x002B26EC
			public override bool Equals(object obj)
			{
				if (obj == null || base.GetType() != obj.GetType())
				{
					return false;
				}
				PerformanceHud.VibrationHudDrawer.VibrationTracker vibrationTracker = (PerformanceHud.VibrationHudDrawer.VibrationTracker)obj;
				return object.Equals(this.Emission, vibrationTracker.Emission);
			}

			// Token: 0x06009AC8 RID: 39624 RVA: 0x002B4529 File Offset: 0x002B2729
			public override int GetHashCode()
			{
				VibrationEmission vibrationEmission = this.Emission;
				if (vibrationEmission == null)
				{
					return 0;
				}
				return vibrationEmission.GetHashCode();
			}

			// Token: 0x04009FEC RID: 40940
			private VibrationEmission emission;

			// Token: 0x04009FED RID: 40941
			private float timer;
		}
	}
}
