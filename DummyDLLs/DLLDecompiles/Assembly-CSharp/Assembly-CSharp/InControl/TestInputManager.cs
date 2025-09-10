using System;
using System.Collections.Generic;
using UnityEngine;

namespace InControl
{
	// Token: 0x02000948 RID: 2376
	public class TestInputManager : MonoBehaviour
	{
		// Token: 0x060054A5 RID: 21669 RVA: 0x00181844 File Offset: 0x0017FA44
		private void OnEnable()
		{
			Application.targetFrameRate = -1;
			QualitySettings.vSyncCount = 0;
			this.isPaused = false;
			Time.timeScale = 1f;
			Logger.OnLogMessage += delegate(LogMessage logMessage)
			{
				this.logMessages.Add(logMessage);
			};
			InputManager.OnDeviceAttached += delegate(InputDevice inputDevice)
			{
				Debug.Log("Attached: " + inputDevice.Name);
			};
			InputManager.OnDeviceDetached += delegate(InputDevice inputDevice)
			{
				Debug.Log("Detached: " + inputDevice.Name);
			};
			InputManager.OnActiveDeviceChanged += delegate(InputDevice inputDevice)
			{
				Debug.Log("Active device changed to: " + inputDevice.Name);
			};
			InputManager.OnUpdate += this.HandleInputUpdate;
		}

		// Token: 0x060054A6 RID: 21670 RVA: 0x001818FC File Offset: 0x0017FAFC
		private void HandleInputUpdate(ulong updateTick, float deltaTime)
		{
			this.CheckForPauseButton();
			int count = InputManager.Devices.Count;
			for (int i = 0; i < count; i++)
			{
				InputDevice inputDevice = InputManager.Devices[i];
				if (inputDevice.LeftBumper || inputDevice.RightBumper)
				{
					inputDevice.VibrateTriggers(inputDevice.LeftTrigger, inputDevice.RightTrigger);
					inputDevice.Vibrate(0f, 0f);
				}
				else
				{
					inputDevice.Vibrate(inputDevice.LeftTrigger, inputDevice.RightTrigger);
					inputDevice.VibrateTriggers(0f, 0f);
				}
				Color color = Color.HSVToRGB(Mathf.Repeat(Time.realtimeSinceStartup * 0.1f, 1f), 1f, 1f);
				inputDevice.SetLightColor(color.r, color.g, color.b);
			}
		}

		// Token: 0x060054A7 RID: 21671 RVA: 0x001819E9 File Offset: 0x0017FBE9
		private void Start()
		{
		}

		// Token: 0x060054A8 RID: 21672 RVA: 0x001819EB File Offset: 0x0017FBEB
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				Utility.LoadScene("TestInputManager");
			}
			if (Input.GetKeyDown(KeyCode.E))
			{
				InputManager.Enabled = !InputManager.Enabled;
			}
		}

		// Token: 0x060054A9 RID: 21673 RVA: 0x00181A16 File Offset: 0x0017FC16
		private void CheckForPauseButton()
		{
			if (Input.GetKeyDown(KeyCode.P) || InputManager.CommandWasPressed)
			{
				Time.timeScale = (this.isPaused ? 1f : 0f);
				this.isPaused = !this.isPaused;
			}
		}

		// Token: 0x060054AA RID: 21674 RVA: 0x00181A50 File Offset: 0x0017FC50
		private void SetColor(Color color)
		{
			this.style.normal.textColor = color;
		}

		// Token: 0x060054AB RID: 21675 RVA: 0x00181A64 File Offset: 0x0017FC64
		private void DrawUnityInputDebugger()
		{
			int num = 300;
			int num2 = Screen.width / 2;
			int num3 = 10;
			int num4 = 20;
			this.SetColor(Color.white);
			string[] joystickNames = Input.GetJoystickNames();
			int num5 = joystickNames.Length;
			for (int i = 0; i < num5; i++)
			{
				string text = joystickNames[i];
				int num6 = i + 1;
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), string.Concat(new string[]
				{
					"Joystick ",
					num6.ToString(),
					": \"",
					text,
					"\""
				}), this.style);
				num3 += num4;
				string text2 = "Buttons: ";
				for (int j = 0; j < 20; j++)
				{
					if (Input.GetKey("joystick " + num6.ToString() + " button " + j.ToString()))
					{
						text2 = text2 + "B" + j.ToString() + "  ";
					}
				}
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text2, this.style);
				num3 += num4;
				string text3 = "Analogs: ";
				for (int k = 0; k < 20; k++)
				{
					float axisRaw = Input.GetAxisRaw("joystick " + num6.ToString() + " analog " + k.ToString());
					if (Utility.AbsoluteIsOverThreshold(axisRaw, 0.2f))
					{
						text3 = string.Concat(new string[]
						{
							text3,
							"A",
							k.ToString(),
							": ",
							axisRaw.ToString("0.00"),
							"  "
						});
					}
				}
				GUI.Label(new Rect((float)num2, (float)num3, (float)(num2 + num), (float)(num3 + 10)), text3, this.style);
				num3 += num4;
				num3 += 25;
			}
		}

		// Token: 0x060054AC RID: 21676 RVA: 0x00181C48 File Offset: 0x0017FE48
		private void OnDrawGizmos()
		{
			InputDevice activeDevice = InputManager.ActiveDevice;
			Vector2 vector = activeDevice.Direction.Vector;
			Gizmos.color = Color.blue;
			Vector2 vector2 = new Vector2(-3f, -1f);
			Vector2 v = vector2 + vector * 2f;
			Gizmos.DrawSphere(vector2, 0.1f);
			Gizmos.DrawLine(vector2, v);
			Gizmos.DrawSphere(v, 1f);
			Gizmos.color = Color.red;
			Vector2 vector3 = new Vector2(3f, -1f);
			Vector2 v2 = vector3 + activeDevice.RightStick.Vector * 2f;
			Gizmos.DrawSphere(vector3, 0.1f);
			Gizmos.DrawLine(vector3, v2);
			Gizmos.DrawSphere(v2, 1f);
		}

		// Token: 0x040053B6 RID: 21430
		public Font font;

		// Token: 0x040053B7 RID: 21431
		private readonly GUIStyle style = new GUIStyle();

		// Token: 0x040053B8 RID: 21432
		private readonly List<LogMessage> logMessages = new List<LogMessage>();

		// Token: 0x040053B9 RID: 21433
		private bool isPaused;
	}
}
