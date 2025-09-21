using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000441 RID: 1089
public static class TimeManager
{
	// Token: 0x1400007B RID: 123
	// (add) Token: 0x060025A4 RID: 9636 RVA: 0x000AB8F4 File Offset: 0x000A9AF4
	// (remove) Token: 0x060025A5 RID: 9637 RVA: 0x000AB928 File Offset: 0x000A9B28
	public static event TimeManager.TimeScaleUpdateDelegate OnTimeScaleUpdated;

	// Token: 0x170003FF RID: 1023
	// (get) Token: 0x060025A6 RID: 9638 RVA: 0x000AB95B File Offset: 0x000A9B5B
	// (set) Token: 0x060025A7 RID: 9639 RVA: 0x000AB962 File Offset: 0x000A9B62
	public static float TimeScale
	{
		get
		{
			return TimeManager._timeScale;
		}
		set
		{
			TimeManager._timeScale = Mathf.Max(0f, value);
			TimeManager.UpdateTimeScale();
		}
	}

	// Token: 0x17000400 RID: 1024
	// (get) Token: 0x060025A8 RID: 9640 RVA: 0x000AB979 File Offset: 0x000A9B79
	// (set) Token: 0x060025A9 RID: 9641 RVA: 0x000AB980 File Offset: 0x000A9B80
	public static float DebugTimeScale
	{
		get
		{
			return TimeManager._debugTimeScale;
		}
		set
		{
			TimeManager._debugTimeScale = Mathf.Clamp(value, 0f, 100f);
			TimeManager.UpdateTimeScale();
		}
	}

	// Token: 0x17000401 RID: 1025
	// (get) Token: 0x060025AA RID: 9642 RVA: 0x000AB99C File Offset: 0x000A9B9C
	// (set) Token: 0x060025AB RID: 9643 RVA: 0x000AB9A3 File Offset: 0x000A9BA3
	public static float CameraShakeTimeScale
	{
		get
		{
			return TimeManager._cameraShakeTimeScale;
		}
		set
		{
			TimeManager._cameraShakeTimeScale = Mathf.Max(0f, value);
			TimeManager.UpdateTimeScale();
		}
	}

	// Token: 0x17000402 RID: 1026
	// (get) Token: 0x060025AC RID: 9644 RVA: 0x000AB9BA File Offset: 0x000A9BBA
	// (set) Token: 0x060025AD RID: 9645 RVA: 0x000AB9C1 File Offset: 0x000A9BC1
	public static float PlatformBackgroundTimeScale
	{
		get
		{
			return TimeManager._platformBackgroundTimeScale;
		}
		set
		{
			TimeManager._platformBackgroundTimeScale = Mathf.Max(0f, value);
			TimeManager.UpdateTimeScale();
		}
	}

	// Token: 0x17000403 RID: 1027
	// (get) Token: 0x060025AE RID: 9646 RVA: 0x000AB9D8 File Offset: 0x000A9BD8
	// (set) Token: 0x060025AF RID: 9647 RVA: 0x000AB9DF File Offset: 0x000A9BDF
	public static bool IsCheatMenuOpen
	{
		get
		{
			return TimeManager._isCheatMenuOpen;
		}
		set
		{
			TimeManager._isCheatMenuOpen = value;
			TimeManager.UpdateTimeScale();
		}
	}

	// Token: 0x17000404 RID: 1028
	// (get) Token: 0x060025B0 RID: 9648 RVA: 0x000AB9EC File Offset: 0x000A9BEC
	// (set) Token: 0x060025B1 RID: 9649 RVA: 0x000AB9F3 File Offset: 0x000A9BF3
	public static float CheatMenuTimeScale
	{
		get
		{
			return TimeManager._cheatMenuTimeScale;
		}
		set
		{
			TimeManager._cheatMenuTimeScale = Mathf.Max(value, 0f);
			TimeManager.UpdateTimeScale();
		}
	}

	// Token: 0x060025B2 RID: 9650 RVA: 0x000ABA0C File Offset: 0x000A9C0C
	private static void UpdateTimeScale()
	{
		float num;
		if (TimeManager._isCheatMenuOpen)
		{
			num = TimeManager._cheatMenuTimeScale;
		}
		else
		{
			num = TimeManager.TimeScale * TimeManager.CameraShakeTimeScale * TimeManager.DebugTimeScale * TimeManager.PlatformBackgroundTimeScale;
		}
		if (TimeManager._timeControlInstances.Count > 0)
		{
			for (int i = 0; i < TimeManager._timeControlInstances.Count; i++)
			{
				TimeManager.TimeControlInstance timeControlInstance = TimeManager._timeControlInstances[i];
				TimeManager.TimeControlInstance.Type controlType = timeControlInstance.ControlType;
				if (controlType != TimeManager.TimeControlInstance.Type.Multiplicative)
				{
					if (controlType == TimeManager.TimeControlInstance.Type.MinValue)
					{
						num = Mathf.Min(num, timeControlInstance.TimeScale);
					}
				}
				else
				{
					num *= timeControlInstance.TimeScale;
				}
			}
		}
		float num2 = Mathf.Max(0f, num);
		if (Time.timeScale != num2)
		{
			Time.timeScale = num2;
			TimeManager.TimeScaleUpdateDelegate onTimeScaleUpdated = TimeManager.OnTimeScaleUpdated;
			if (onTimeScaleUpdated == null)
			{
				return;
			}
			onTimeScaleUpdated(num2);
		}
	}

	// Token: 0x060025B3 RID: 9651 RVA: 0x000ABAC4 File Offset: 0x000A9CC4
	public static void Reset()
	{
		Time.timeScale = 1f;
		TimeManager._timeScale = 1f;
		TimeManager._debugTimeScale = 1f;
		TimeManager._cameraShakeTimeScale = 1f;
		TimeManager._cheatMenuTimeScale = 0f;
		TimeManager.TimeScaleUpdateDelegate onTimeScaleUpdated = TimeManager.OnTimeScaleUpdated;
		if (onTimeScaleUpdated == null)
		{
			return;
		}
		onTimeScaleUpdated(1f);
	}

	// Token: 0x060025B4 RID: 9652 RVA: 0x000ABB17 File Offset: 0x000A9D17
	public static void SpeedUpDebug()
	{
		TimeManager.DebugTimeScale *= 2f;
	}

	// Token: 0x060025B5 RID: 9653 RVA: 0x000ABB29 File Offset: 0x000A9D29
	public static void SlowDownDebug()
	{
		TimeManager.DebugTimeScale *= 0.5f;
	}

	// Token: 0x060025B6 RID: 9654 RVA: 0x000ABB3B File Offset: 0x000A9D3B
	public static void ResetDebug()
	{
		TimeManager.DebugTimeScale = 1f;
	}

	// Token: 0x060025B7 RID: 9655 RVA: 0x000ABB47 File Offset: 0x000A9D47
	public static void ResetCheatMenuTimeScale()
	{
		TimeManager._cheatMenuTimeScale = 0f;
		TimeManager.UpdateTimeScale();
	}

	// Token: 0x060025B8 RID: 9656 RVA: 0x000ABB58 File Offset: 0x000A9D58
	public static TimeManager.TimeControlInstance CreateTimeControl(float timeScale, TimeManager.TimeControlInstance.Type controlType = TimeManager.TimeControlInstance.Type.Multiplicative)
	{
		return new TimeManager.TimeControlInstance(timeScale, controlType);
	}

	// Token: 0x04002334 RID: 9012
	private static float _timeScale = 1f;

	// Token: 0x04002335 RID: 9013
	private static float _debugTimeScale = 1f;

	// Token: 0x04002336 RID: 9014
	private static float _cameraShakeTimeScale = 1f;

	// Token: 0x04002337 RID: 9015
	private static float _platformBackgroundTimeScale = 1f;

	// Token: 0x04002338 RID: 9016
	private static float _cheatMenuTimeScale = 0f;

	// Token: 0x04002339 RID: 9017
	private static bool _isCheatMenuOpen;

	// Token: 0x0400233A RID: 9018
	private static List<TimeManager.TimeControlInstance> _timeControlInstances = new List<TimeManager.TimeControlInstance>(20);

	// Token: 0x020016FC RID: 5884
	// (Invoke) Token: 0x06008BEB RID: 35819
	public delegate void TimeScaleUpdateDelegate(float timeScale);

	// Token: 0x020016FD RID: 5885
	public sealed class TimeControlInstance
	{
		// Token: 0x17000EAE RID: 3758
		// (get) Token: 0x06008BEE RID: 35822 RVA: 0x00284F04 File Offset: 0x00283104
		// (set) Token: 0x06008BEF RID: 35823 RVA: 0x00284F0C File Offset: 0x0028310C
		public float TimeScale
		{
			get
			{
				return this.timeScale;
			}
			set
			{
				this.timeScale = value;
				TimeManager.UpdateTimeScale();
			}
		}

		// Token: 0x17000EAF RID: 3759
		// (get) Token: 0x06008BF0 RID: 35824 RVA: 0x00284F1A File Offset: 0x0028311A
		// (set) Token: 0x06008BF1 RID: 35825 RVA: 0x00284F22 File Offset: 0x00283122
		public TimeManager.TimeControlInstance.Type ControlType
		{
			get
			{
				return this.controlType;
			}
			set
			{
				this.controlType = value;
				TimeManager.UpdateTimeScale();
			}
		}

		// Token: 0x06008BF2 RID: 35826 RVA: 0x00284F30 File Offset: 0x00283130
		~TimeControlInstance()
		{
			this.Release();
		}

		// Token: 0x06008BF3 RID: 35827 RVA: 0x00284F5C File Offset: 0x0028315C
		public TimeControlInstance(float timeScale, TimeManager.TimeControlInstance.Type controlType)
		{
			this.timeScale = timeScale;
			this.controlType = controlType;
			TimeManager._timeControlInstances.Add(this);
			TimeManager.UpdateTimeScale();
		}

		// Token: 0x06008BF4 RID: 35828 RVA: 0x00284F8D File Offset: 0x0028318D
		public void Release()
		{
			if (TimeManager._timeControlInstances.Remove(this))
			{
				TimeManager.UpdateTimeScale();
			}
		}

		// Token: 0x04008CCE RID: 36046
		private float timeScale = 1f;

		// Token: 0x04008CCF RID: 36047
		private TimeManager.TimeControlInstance.Type controlType;

		// Token: 0x02001C18 RID: 7192
		public enum Type
		{
			// Token: 0x04009FFE RID: 40958
			Multiplicative,
			// Token: 0x04009FFF RID: 40959
			MinValue
		}
	}
}
