using System;
using System.Collections;
using InControl;
using TeamCherry.Localization;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020005D8 RID: 1496
public class OnScreenDebugInfo : MonoBehaviour, IOnGUI
{
	// Token: 0x0600351D RID: 13597 RVA: 0x000EB9F0 File Offset: 0x000E9BF0
	private void Awake()
	{
		this.fpsRect = new Rect(7f, 5f, 100f, 25f);
		this.infoRect = new Rect((float)(Screen.width - 105), 5f, 100f, 70f);
		this.inputRect = new Rect(7f, 65f, 300f, 120f);
		this.loadProfilerRect = new Rect((float)(Screen.width / 2) - 50f, 5f, 100f, 25f);
		this.tfrRect = new Rect(7f, 20f, 100f, 25f);
	}

	// Token: 0x0600351E RID: 13598 RVA: 0x000EBAA5 File Offset: 0x000E9CA5
	private IEnumerator Start()
	{
		this.gm = GameManager.instance;
		this.gm.UnloadingLevel += this.OnLevelUnload;
		this.ih = this.gm.inputHandler;
		this.RetrieveInfo();
		GUI.depth = 2;
		while (this.showFPS)
		{
			if (Time.timeScale == 1f)
			{
				yield return new WaitForSeconds(0.1f);
				this.frameRate = 1f / Time.deltaTime;
				this.fps = "FPS :" + Mathf.Round(this.frameRate).ToString();
			}
			else
			{
				this.fps = "Pause";
			}
			yield return new WaitForSeconds(0.5f);
		}
		yield break;
	}

	// Token: 0x0600351F RID: 13599 RVA: 0x000EBAB4 File Offset: 0x000E9CB4
	private void LevelActivated(Scene sceneFrom, Scene sceneTo)
	{
		this.RetrieveInfo();
		if (this.showLoadingTime)
		{
			this.loadTime = (float)Math.Round((double)(Time.realtimeSinceStartup - this.unloadTime), 2);
		}
	}

	// Token: 0x06003520 RID: 13600 RVA: 0x000EBADE File Offset: 0x000E9CDE
	private void OnEnable()
	{
		SceneManager.activeSceneChanged += this.LevelActivated;
	}

	// Token: 0x06003521 RID: 13601 RVA: 0x000EBAF1 File Offset: 0x000E9CF1
	private void OnDisable()
	{
		SceneManager.activeSceneChanged -= this.LevelActivated;
		if (this.gm != null)
		{
			this.gm.UnloadingLevel -= this.OnLevelUnload;
		}
		GUIDrawer.RemoveDrawer(this);
	}

	// Token: 0x170005DA RID: 1498
	// (get) Token: 0x06003522 RID: 13602 RVA: 0x000EBB2F File Offset: 0x000E9D2F
	public int GUIDepth
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x06003523 RID: 13603 RVA: 0x000EBB34 File Offset: 0x000E9D34
	public void DrawGUI()
	{
		if (this.showInfo)
		{
			if (this.showFPS)
			{
				GUI.Label(this.fpsRect, this.fps);
			}
			if (this.showInfo)
			{
				GUI.Label(this.infoRect, this.infoString);
			}
			if (this.showInput)
			{
				GUI.Label(this.inputRect, this.ReadInput());
			}
			if (this.showLoadingTime)
			{
				GUI.Label(this.loadProfilerRect, this.loadTime.ToString() + "s");
			}
			if (this.showTFR)
			{
				GUI.Label(this.tfrRect, "TFR: " + Application.targetFrameRate.ToString());
			}
		}
	}

	// Token: 0x06003524 RID: 13604 RVA: 0x000EBBE9 File Offset: 0x000E9DE9
	public void ShowFPS()
	{
		this.showFPS = !this.showFPS;
	}

	// Token: 0x06003525 RID: 13605 RVA: 0x000EBBFA File Offset: 0x000E9DFA
	public void ShowGameInfo()
	{
		this.showInfo = !this.showInfo;
		if (this.showInfo)
		{
			GUIDrawer.AddDrawer(this);
			return;
		}
		GUIDrawer.RemoveDrawer(this);
	}

	// Token: 0x06003526 RID: 13606 RVA: 0x000EBC20 File Offset: 0x000E9E20
	public void ShowInput()
	{
		this.showInput = !this.showInput;
	}

	// Token: 0x06003527 RID: 13607 RVA: 0x000EBC31 File Offset: 0x000E9E31
	public void ShowLoadingTime()
	{
		this.showLoadingTime = !this.showLoadingTime;
	}

	// Token: 0x06003528 RID: 13608 RVA: 0x000EBC42 File Offset: 0x000E9E42
	public void ShowTargetFrameRate()
	{
		this.showTFR = !this.showTFR;
	}

	// Token: 0x06003529 RID: 13609 RVA: 0x000EBC53 File Offset: 0x000E9E53
	private void OnLevelUnload()
	{
		this.unloadTime = Time.realtimeSinceStartup;
	}

	// Token: 0x0600352A RID: 13610 RVA: 0x000EBC60 File Offset: 0x000E9E60
	private void RetrieveInfo()
	{
		if (this.gm == null)
		{
			this.gm = GameManager.instance;
		}
		this.versionNumber = "1.0.28324";
		this.infoString = string.Concat(new string[]
		{
			Language.Get("GAME_TITLE"),
			"\r\n",
			this.versionNumber,
			" ",
			Language.CurrentLanguage().ToString(),
			"\r\n",
			this.gm.GetSceneNameString()
		});
	}

	// Token: 0x0600352B RID: 13611 RVA: 0x000EBCF4 File Offset: 0x000E9EF4
	private string ReadInput()
	{
		string str = "";
		string format = "Move Vector: {0}, {1}";
		Vector2 vector = this.ih.inputActions.MoveVector.Vector;
		object arg = vector.x.ToString();
		vector = this.ih.inputActions.MoveVector.Vector;
		return str + string.Format(format, arg, vector.y.ToString()) + string.Format("\nMove Pressed: {0}", this.ih.inputActions.Left.IsPressed || this.ih.inputActions.Right.IsPressed) + string.Format("\nMove Raw L: {0} R: {1}", this.ih.inputActions.Left.RawValue, this.ih.inputActions.Right.RawValue) + string.Format("\nAny Key Down: {0}", InputManager.AnyKeyIsPressed);
	}

	// Token: 0x0400387E RID: 14462
	private GameManager gm;

	// Token: 0x0400387F RID: 14463
	private InputHandler ih;

	// Token: 0x04003880 RID: 14464
	private float unloadTime;

	// Token: 0x04003881 RID: 14465
	private float loadTime;

	// Token: 0x04003882 RID: 14466
	private float frameRate;

	// Token: 0x04003883 RID: 14467
	private string fps;

	// Token: 0x04003884 RID: 14468
	private string infoString;

	// Token: 0x04003885 RID: 14469
	private string versionNumber;

	// Token: 0x04003886 RID: 14470
	private const float textWidth = 100f;

	// Token: 0x04003887 RID: 14471
	private Rect loadProfilerRect;

	// Token: 0x04003888 RID: 14472
	private Rect fpsRect;

	// Token: 0x04003889 RID: 14473
	private Rect infoRect;

	// Token: 0x0400388A RID: 14474
	private Rect inputRect;

	// Token: 0x0400388B RID: 14475
	private Rect tfrRect;

	// Token: 0x0400388C RID: 14476
	private bool showFPS;

	// Token: 0x0400388D RID: 14477
	private bool showInfo;

	// Token: 0x0400388E RID: 14478
	private bool showInput;

	// Token: 0x0400388F RID: 14479
	private bool showLoadingTime;

	// Token: 0x04003890 RID: 14480
	private bool showTFR;
}
