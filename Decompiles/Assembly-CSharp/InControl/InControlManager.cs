using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InControl
{
	// Token: 0x020008EF RID: 2287
	public class InControlManager : SingletonMonoBehavior<InControlManager>
	{
		// Token: 0x06005020 RID: 20512 RVA: 0x00172438 File Offset: 0x00170638
		private void OnEnable()
		{
			if (base.EnforceSingleton)
			{
				return;
			}
			InputManager.InvertYAxis = this.invertYAxis;
			InputManager.SuspendInBackground = this.suspendInBackground;
			InputManager.EnableICade = this.enableICade;
			InputManager.EnableXInput = this.enableXInput;
			InputManager.XInputUpdateRate = (uint)Mathf.Max(this.xInputUpdateRate, 0);
			InputManager.XInputBufferSize = (uint)Mathf.Max(this.xInputBufferSize, 0);
			InputManager.EnableNativeInput = this.enableNativeInput;
			InputManager.NativeInputEnableXInput = this.nativeInputEnableXInput;
			InputManager.NativeInputEnableMFi = this.nativeInputEnableMFi;
			InputManager.NativeInputUpdateRate = (uint)Mathf.Max(this.nativeInputUpdateRate, 0);
			InputManager.NativeInputPreventSleep = this.nativeInputPreventSleep;
			if (this.logDebugInfo)
			{
				Logger.OnLogMessage -= InControlManager.LogMessage;
				Logger.OnLogMessage += InControlManager.LogMessage;
			}
			InputManager.SetupInternal();
			SceneManager.sceneLoaded -= this.OnSceneWasLoaded;
			SceneManager.sceneLoaded += this.OnSceneWasLoaded;
			if (this.dontDestroyOnLoad)
			{
				Object.DontDestroyOnLoad(this);
			}
		}

		// Token: 0x06005021 RID: 20513 RVA: 0x00172539 File Offset: 0x00170739
		private void OnDisable()
		{
			if (base.IsNotTheSingleton)
			{
				return;
			}
			SceneManager.sceneLoaded -= this.OnSceneWasLoaded;
			InputManager.ResetInternal();
		}

		// Token: 0x06005022 RID: 20514 RVA: 0x0017255A File Offset: 0x0017075A
		private void Update()
		{
			if (base.IsNotTheSingleton)
			{
				return;
			}
			if (this.applicationHasQuit)
			{
				return;
			}
			if (this.updateMode == InControlUpdateMode.Default || (this.updateMode == InControlUpdateMode.FixedUpdate && Utility.IsZero(Time.timeScale)))
			{
				InputManager.UpdateInternal();
			}
		}

		// Token: 0x06005023 RID: 20515 RVA: 0x00172590 File Offset: 0x00170790
		private void FixedUpdate()
		{
			if (base.IsNotTheSingleton)
			{
				return;
			}
			if (this.applicationHasQuit)
			{
				return;
			}
			if (this.updateMode == InControlUpdateMode.FixedUpdate)
			{
				InputManager.UpdateInternal();
			}
		}

		// Token: 0x06005024 RID: 20516 RVA: 0x001725B2 File Offset: 0x001707B2
		private void OnApplicationFocus(bool focusState)
		{
			if (base.IsNotTheSingleton)
			{
				return;
			}
			if (this.applicationHasQuit)
			{
				return;
			}
			InputManager.OnApplicationFocus(focusState);
		}

		// Token: 0x06005025 RID: 20517 RVA: 0x001725CC File Offset: 0x001707CC
		private void OnApplicationPause(bool pauseState)
		{
			if (base.IsNotTheSingleton)
			{
				return;
			}
			if (this.applicationHasQuit)
			{
				return;
			}
			InputManager.OnApplicationPause(pauseState);
		}

		// Token: 0x06005026 RID: 20518 RVA: 0x001725E6 File Offset: 0x001707E6
		private void OnApplicationQuit()
		{
			if (base.IsNotTheSingleton)
			{
				return;
			}
			if (this.applicationHasQuit)
			{
				return;
			}
			InputManager.OnApplicationQuit();
			this.applicationHasQuit = true;
		}

		// Token: 0x06005027 RID: 20519 RVA: 0x00172606 File Offset: 0x00170806
		private void OnSceneWasLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			if (base.IsNotTheSingleton)
			{
				return;
			}
			if (this.applicationHasQuit)
			{
				return;
			}
			if (loadSceneMode == LoadSceneMode.Single)
			{
				InputManager.OnLevelWasLoaded();
			}
		}

		// Token: 0x06005028 RID: 20520 RVA: 0x00172622 File Offset: 0x00170822
		private static void LogMessage(LogMessage logMessage)
		{
		}

		// Token: 0x040050A5 RID: 20645
		public bool logDebugInfo = true;

		// Token: 0x040050A6 RID: 20646
		public bool invertYAxis;

		// Token: 0x040050A7 RID: 20647
		[SerializeField]
		private bool useFixedUpdate;

		// Token: 0x040050A8 RID: 20648
		public bool dontDestroyOnLoad = true;

		// Token: 0x040050A9 RID: 20649
		public bool suspendInBackground;

		// Token: 0x040050AA RID: 20650
		public InControlUpdateMode updateMode;

		// Token: 0x040050AB RID: 20651
		public bool enableICade;

		// Token: 0x040050AC RID: 20652
		public bool enableXInput;

		// Token: 0x040050AD RID: 20653
		public bool xInputOverrideUpdateRate;

		// Token: 0x040050AE RID: 20654
		public int xInputUpdateRate;

		// Token: 0x040050AF RID: 20655
		public bool xInputOverrideBufferSize;

		// Token: 0x040050B0 RID: 20656
		public int xInputBufferSize;

		// Token: 0x040050B1 RID: 20657
		public bool enableNativeInput = true;

		// Token: 0x040050B2 RID: 20658
		public bool nativeInputEnableXInput = true;

		// Token: 0x040050B3 RID: 20659
		public bool nativeInputEnableMFi = true;

		// Token: 0x040050B4 RID: 20660
		public bool nativeInputPreventSleep;

		// Token: 0x040050B5 RID: 20661
		public bool nativeInputOverrideUpdateRate;

		// Token: 0x040050B6 RID: 20662
		public int nativeInputUpdateRate;

		// Token: 0x040050B7 RID: 20663
		private bool applicationHasQuit;
	}
}
