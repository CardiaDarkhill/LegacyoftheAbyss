using System;
using System.Collections.Generic;
using System.Threading;
using InControl.Internal;
using InControl.UnityDeviceProfiles;
using UnityEngine;
using XInputDotNetPure;

namespace InControl
{
	// Token: 0x02000947 RID: 2375
	public class XInputDeviceManager : InputDeviceManager
	{
		// Token: 0x0600549C RID: 21660 RVA: 0x00181538 File Offset: 0x0017F738
		public XInputDeviceManager()
		{
			if (InputManager.XInputUpdateRate == 0U)
			{
				this.timeStep = Mathf.FloorToInt(Time.fixedDeltaTime * 1000f);
			}
			else
			{
				this.timeStep = Mathf.FloorToInt(1f / InputManager.XInputUpdateRate * 1000f);
			}
			this.bufferSize = (int)Math.Max(InputManager.XInputBufferSize, 1U);
			for (int i = 0; i < 4; i++)
			{
				this.gamePadState[i] = new RingBuffer<GamePadState>(this.bufferSize);
			}
			this.StartWorker();
			for (int j = 0; j < 4; j++)
			{
				this.devices.Add(new XInputDevice(j, this));
			}
			this.Update(0UL, 0f);
		}

		// Token: 0x0600549D RID: 21661 RVA: 0x00181601 File Offset: 0x0017F801
		private void StartWorker()
		{
			if (this.thread == null)
			{
				this.thread = new Thread(new ThreadStart(this.Worker));
				this.thread.IsBackground = true;
				this.thread.Start();
			}
		}

		// Token: 0x0600549E RID: 21662 RVA: 0x00181639 File Offset: 0x0017F839
		private void StopWorker()
		{
			if (this.thread != null)
			{
				this.thread.Abort();
				this.thread.Join();
				this.thread = null;
			}
		}

		// Token: 0x0600549F RID: 21663 RVA: 0x00181660 File Offset: 0x0017F860
		private void Worker()
		{
			for (;;)
			{
				for (int i = 0; i < 4; i++)
				{
					this.gamePadState[i].Enqueue(GamePad.GetState((PlayerIndex)i));
				}
				Thread.Sleep(this.timeStep);
			}
		}

		// Token: 0x060054A0 RID: 21664 RVA: 0x00181698 File Offset: 0x0017F898
		internal GamePadState GetState(int deviceIndex)
		{
			return this.gamePadState[deviceIndex].Dequeue();
		}

		// Token: 0x060054A1 RID: 21665 RVA: 0x001816A8 File Offset: 0x0017F8A8
		public override void Update(ulong updateTick, float deltaTime)
		{
			for (int i = 0; i < 4; i++)
			{
				XInputDevice xinputDevice = this.devices[i] as XInputDevice;
				if (!xinputDevice.IsConnected)
				{
					xinputDevice.GetState();
				}
				if (xinputDevice.IsConnected != this.deviceConnected[i])
				{
					if (xinputDevice.IsConnected)
					{
						InputManager.AttachDevice(xinputDevice);
					}
					else
					{
						InputManager.DetachDevice(xinputDevice);
					}
					this.deviceConnected[i] = xinputDevice.IsConnected;
				}
			}
		}

		// Token: 0x060054A2 RID: 21666 RVA: 0x00181715 File Offset: 0x0017F915
		public override void Destroy()
		{
			this.StopWorker();
		}

		// Token: 0x060054A3 RID: 21667 RVA: 0x00181720 File Offset: 0x0017F920
		public static bool CheckPlatformSupport(ICollection<string> errors)
		{
			if (Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsEditor)
			{
				return false;
			}
			try
			{
				GamePad.GetState(PlayerIndex.One);
			}
			catch (DllNotFoundException ex)
			{
				if (errors != null)
				{
					errors.Add(ex.Message + ".dll could not be found or is missing a dependency.");
				}
				return false;
			}
			return true;
		}

		// Token: 0x060054A4 RID: 21668 RVA: 0x0018177C File Offset: 0x0017F97C
		internal static void Enable()
		{
			List<string> list = new List<string>();
			if (XInputDeviceManager.CheckPlatformSupport(list))
			{
				InputManager.HideDevicesWithProfile(typeof(Xbox360WindowsUnityProfile));
				InputManager.HideDevicesWithProfile(typeof(XboxOneWindowsUnityProfile));
				InputManager.HideDevicesWithProfile(typeof(XboxOneWindows10UnityProfile));
				InputManager.HideDevicesWithProfile(typeof(XboxOneWindows10AEUnityProfile));
				InputManager.HideDevicesWithProfile(typeof(LogitechF310ModeXWindowsUnityProfile));
				InputManager.HideDevicesWithProfile(typeof(LogitechF510ModeXWindowsUnityProfile));
				InputManager.HideDevicesWithProfile(typeof(LogitechF710ModeXWindowsUnityProfile));
				InputManager.AddDeviceManager<XInputDeviceManager>();
				return;
			}
			foreach (string text in list)
			{
				Logger.LogError(text);
			}
		}

		// Token: 0x040053B0 RID: 21424
		private readonly bool[] deviceConnected = new bool[4];

		// Token: 0x040053B1 RID: 21425
		private const int maxDevices = 4;

		// Token: 0x040053B2 RID: 21426
		private readonly RingBuffer<GamePadState>[] gamePadState = new RingBuffer<GamePadState>[4];

		// Token: 0x040053B3 RID: 21427
		private Thread thread;

		// Token: 0x040053B4 RID: 21428
		private readonly int timeStep;

		// Token: 0x040053B5 RID: 21429
		private int bufferSize;
	}
}
