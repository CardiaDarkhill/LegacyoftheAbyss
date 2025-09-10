using System;
using GlobalEnums;
using InControl;
using UnityEngine;

// Token: 0x0200076F RID: 1903
public class IdleMemoryCleaner : MonoBehaviour
{
	// Token: 0x060043EA RID: 17386 RVA: 0x0012A311 File Offset: 0x00128511
	private void OnEnable()
	{
		base.gameObject.hideFlags = HideFlags.HideAndDontSave;
		Object.DontDestroyOnLoad(this);
	}

	// Token: 0x060043EB RID: 17387 RVA: 0x0012A328 File Offset: 0x00128528
	private void Update()
	{
		GameState gameState = GameManager.instance.GameState;
		if (gameState != GameState.PLAYING && gameState != GameState.PAUSED)
		{
			this.lastInteractionTime = Time.realtimeSinceStartup;
			return;
		}
		if (this.HadAnyInputLastFrame())
		{
			this.UpdateLastInteractionTime();
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		bool flag = realtimeSinceStartup - this.lastInteractionTime >= 300f;
		bool flag2 = realtimeSinceStartup - this.lastCleanupTime <= 120f;
		if (!flag || flag2 || this.isCleaning)
		{
			return;
		}
		double num = (double)GCManager.GetMonoHeapUsage() / 1024.0 / 1024.0;
		double num2 = GCManager.HeapUsageThreshold * 0.5;
		if (num < num2)
		{
			return;
		}
		this.CleanMemory();
	}

	// Token: 0x060043EC RID: 17388 RVA: 0x0012A3E3 File Offset: 0x001285E3
	private void UpdateLastInteractionTime()
	{
		this.lastInteractionTime = Time.realtimeSinceStartup;
	}

	// Token: 0x060043ED RID: 17389 RVA: 0x0012A3F0 File Offset: 0x001285F0
	private void CleanMemory()
	{
		this.isCleaning = true;
		GCManager.ForceCollect(true, true);
		this.lastCleanupTime = Time.realtimeSinceStartup;
		this.isCleaning = false;
	}

	// Token: 0x060043EE RID: 17390 RVA: 0x0012A414 File Offset: 0x00128614
	private bool HadAnyInputLastFrame()
	{
		InputDevice gameController = ManagerSingleton<InputHandler>.Instance.gameController;
		if (gameController == null)
		{
			return false;
		}
		TwoAxisInputControl leftStick = gameController.LeftStick;
		bool flag = ((leftStick != null) ? leftStick.Value.magnitude : 0f) > 0.3f;
		TwoAxisInputControl rightStick = gameController.RightStick;
		bool flag2 = ((rightStick != null) ? rightStick.Value.magnitude : 0f) > 0.3f;
		TwoAxisInputControl dpad = gameController.DPad;
		bool flag3 = ((dpad != null) ? dpad.Value.magnitude : 0f) > 0.3f;
		InputControl anyButton = gameController.AnyButton;
		bool flag4 = anyButton != null && anyButton.WasPressed;
		bool anyKeyDown = Input.anyKeyDown;
		return flag || flag2 || flag3 || flag4 || anyKeyDown;
	}

	// Token: 0x04004536 RID: 17718
	private const float IDLE_TIME_THRESHOLD = 300f;

	// Token: 0x04004537 RID: 17719
	private const float CLEANUP_INTERVAL = 120f;

	// Token: 0x04004538 RID: 17720
	private const float TWO_AXIS_INPUT_RELEVANCE_THRESHOLD = 0.3f;

	// Token: 0x04004539 RID: 17721
	private float lastInteractionTime;

	// Token: 0x0400453A RID: 17722
	private float lastCleanupTime;

	// Token: 0x0400453B RID: 17723
	private bool isCleaning;
}
