using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace InControl
{
	// Token: 0x0200092B RID: 2347
	[ExecuteInEditMode]
	public class TouchManager : SingletonMonoBehavior<TouchManager>
	{
		// Token: 0x14000103 RID: 259
		// (add) Token: 0x06005327 RID: 21287 RVA: 0x0017C878 File Offset: 0x0017AA78
		// (remove) Token: 0x06005328 RID: 21288 RVA: 0x0017C8AC File Offset: 0x0017AAAC
		public static event Action OnSetup;

		// Token: 0x06005329 RID: 21289 RVA: 0x0017C8DF File Offset: 0x0017AADF
		protected TouchManager()
		{
		}

		// Token: 0x0600532A RID: 21290 RVA: 0x0017C908 File Offset: 0x0017AB08
		private void OnEnable()
		{
			if (base.GetComponent<InControlManager>() == null)
			{
				Logger.LogError("Touch Manager component can only be added to the InControl Manager object.");
				Object.DestroyImmediate(this);
				return;
			}
			if (base.EnforceSingleton)
			{
				return;
			}
			this.touchControls = base.GetComponentsInChildren<TouchControl>(true);
			if (Application.isPlaying)
			{
				InputManager.OnSetup += this.Setup;
				InputManager.OnUpdateDevices += this.UpdateDevice;
				InputManager.OnCommitDevices += this.CommitDevice;
			}
		}

		// Token: 0x0600532B RID: 21291 RVA: 0x0017C984 File Offset: 0x0017AB84
		private void OnDisable()
		{
			if (Application.isPlaying)
			{
				InputManager.OnSetup -= this.Setup;
				InputManager.OnUpdateDevices -= this.UpdateDevice;
				InputManager.OnCommitDevices -= this.CommitDevice;
			}
			this.Reset();
		}

		// Token: 0x0600532C RID: 21292 RVA: 0x0017C9D1 File Offset: 0x0017ABD1
		private void Setup()
		{
			this.UpdateScreenSize(this.GetCurrentScreenSize());
			this.CreateDevice();
			this.CreateTouches();
			if (TouchManager.OnSetup != null)
			{
				TouchManager.OnSetup();
				TouchManager.OnSetup = null;
			}
		}

		// Token: 0x0600532D RID: 21293 RVA: 0x0017CA04 File Offset: 0x0017AC04
		private void Reset()
		{
			this.device = null;
			for (int i = 0; i < 3; i++)
			{
				this.mouseTouches[i] = null;
			}
			this.cachedTouches = null;
			this.activeTouches = null;
			this.readOnlyActiveTouches = null;
			this.touchControls = null;
			TouchManager.OnSetup = null;
		}

		// Token: 0x0600532E RID: 21294 RVA: 0x0017CA4F File Offset: 0x0017AC4F
		private IEnumerator UpdateScreenSizeAtEndOfFrame()
		{
			yield return new WaitForEndOfFrame();
			this.UpdateScreenSize(this.GetCurrentScreenSize());
			yield return null;
			yield break;
		}

		// Token: 0x0600532F RID: 21295 RVA: 0x0017CA60 File Offset: 0x0017AC60
		private void Update()
		{
			Vector2 currentScreenSize = this.GetCurrentScreenSize();
			if (!this.isReady)
			{
				base.StartCoroutine(this.UpdateScreenSizeAtEndOfFrame());
				this.UpdateScreenSize(currentScreenSize);
				this.isReady = true;
				return;
			}
			if (this.screenSize != currentScreenSize)
			{
				this.UpdateScreenSize(currentScreenSize);
			}
			if (TouchManager.OnSetup != null)
			{
				TouchManager.OnSetup();
				TouchManager.OnSetup = null;
			}
		}

		// Token: 0x06005330 RID: 21296 RVA: 0x0017CAC4 File Offset: 0x0017ACC4
		private void CreateDevice()
		{
			this.device = new TouchInputDevice();
			this.device.AddControl(InputControlType.LeftStickLeft, "LeftStickLeft");
			this.device.AddControl(InputControlType.LeftStickRight, "LeftStickRight");
			this.device.AddControl(InputControlType.LeftStickUp, "LeftStickUp");
			this.device.AddControl(InputControlType.LeftStickDown, "LeftStickDown");
			this.device.AddControl(InputControlType.RightStickLeft, "RightStickLeft");
			this.device.AddControl(InputControlType.RightStickRight, "RightStickRight");
			this.device.AddControl(InputControlType.RightStickUp, "RightStickUp");
			this.device.AddControl(InputControlType.RightStickDown, "RightStickDown");
			this.device.AddControl(InputControlType.DPadUp, "DPadUp");
			this.device.AddControl(InputControlType.DPadDown, "DPadDown");
			this.device.AddControl(InputControlType.DPadLeft, "DPadLeft");
			this.device.AddControl(InputControlType.DPadRight, "DPadRight");
			this.device.AddControl(InputControlType.LeftTrigger, "LeftTrigger");
			this.device.AddControl(InputControlType.RightTrigger, "RightTrigger");
			this.device.AddControl(InputControlType.LeftBumper, "LeftBumper");
			this.device.AddControl(InputControlType.RightBumper, "RightBumper");
			for (InputControlType inputControlType = InputControlType.Action1; inputControlType <= InputControlType.Action12; inputControlType++)
			{
				this.device.AddControl(inputControlType, inputControlType.ToString());
			}
			this.device.AddControl(InputControlType.Menu, "Menu");
			for (InputControlType inputControlType2 = InputControlType.Button0; inputControlType2 <= InputControlType.Button19; inputControlType2++)
			{
				this.device.AddControl(inputControlType2, inputControlType2.ToString());
			}
			InputManager.AttachDevice(this.device);
		}

		// Token: 0x06005331 RID: 21297 RVA: 0x0017CC79 File Offset: 0x0017AE79
		private void UpdateDevice(ulong updateTick, float deltaTime)
		{
			this.UpdateTouches(updateTick, deltaTime);
			this.SubmitControlStates(updateTick, deltaTime);
		}

		// Token: 0x06005332 RID: 21298 RVA: 0x0017CC8B File Offset: 0x0017AE8B
		private void CommitDevice(ulong updateTick, float deltaTime)
		{
			this.CommitControlStates(updateTick, deltaTime);
		}

		// Token: 0x06005333 RID: 21299 RVA: 0x0017CC98 File Offset: 0x0017AE98
		private void SubmitControlStates(ulong updateTick, float deltaTime)
		{
			int num = this.touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				TouchControl touchControl = this.touchControls[i];
				if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
				{
					touchControl.SubmitControlState(updateTick, deltaTime);
				}
			}
		}

		// Token: 0x06005334 RID: 21300 RVA: 0x0017CCE0 File Offset: 0x0017AEE0
		private void CommitControlStates(ulong updateTick, float deltaTime)
		{
			int num = this.touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				TouchControl touchControl = this.touchControls[i];
				if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
				{
					touchControl.CommitControlState(updateTick, deltaTime);
				}
			}
		}

		// Token: 0x06005335 RID: 21301 RVA: 0x0017CD28 File Offset: 0x0017AF28
		private void UpdateScreenSize(Vector2 currentScreenSize)
		{
			this.touchCamera.rect = new Rect(0f, 0f, 0.99f, 1f);
			this.touchCamera.rect = new Rect(0f, 0f, 1f, 1f);
			this.screenSize = currentScreenSize;
			this.halfScreenSize = this.screenSize / 2f;
			this.viewSize = this.ConvertViewToWorldPoint(Vector2.one) * 0.02f;
			this.percentToWorld = Mathf.Min(this.viewSize.x, this.viewSize.y);
			this.halfPercentToWorld = this.percentToWorld / 2f;
			if (this.touchCamera != null)
			{
				this.halfPixelToWorld = this.touchCamera.orthographicSize / this.screenSize.y;
				this.pixelToWorld = this.halfPixelToWorld * 2f;
			}
			if (this.touchControls != null)
			{
				int num = this.touchControls.Length;
				for (int i = 0; i < num; i++)
				{
					this.touchControls[i].ConfigureControl();
				}
			}
		}

		// Token: 0x06005336 RID: 21302 RVA: 0x0017CE50 File Offset: 0x0017B050
		private void CreateTouches()
		{
			this.cachedTouches = new TouchPool();
			for (int i = 0; i < 3; i++)
			{
				this.mouseTouches[i] = new Touch();
				this.mouseTouches[i].fingerId = -2;
			}
			this.activeTouches = new List<Touch>(32);
			this.readOnlyActiveTouches = new ReadOnlyCollection<Touch>(this.activeTouches);
		}

		// Token: 0x06005337 RID: 21303 RVA: 0x0017CEB0 File Offset: 0x0017B0B0
		private void UpdateTouches(ulong updateTick, float deltaTime)
		{
			this.activeTouches.Clear();
			this.cachedTouches.FreeEndedTouches();
			for (int i = 0; i < 3; i++)
			{
				if (this.mouseTouches[i].SetWithMouseData(i, updateTick, deltaTime))
				{
					this.activeTouches.Add(this.mouseTouches[i]);
				}
			}
			for (int j = 0; j < Input.touchCount; j++)
			{
				Touch touch = Input.GetTouch(j);
				Touch touch2 = this.cachedTouches.FindOrCreateTouch(touch.fingerId);
				touch2.SetWithTouchData(touch, updateTick, deltaTime);
				this.activeTouches.Add(touch2);
			}
			int count = this.cachedTouches.Touches.Count;
			for (int k = 0; k < count; k++)
			{
				Touch touch3 = this.cachedTouches.Touches[k];
				if (touch3.phase != TouchPhase.Ended && touch3.updateTick != updateTick)
				{
					touch3.phase = TouchPhase.Ended;
					this.activeTouches.Add(touch3);
				}
			}
			this.InvokeTouchEvents();
		}

		// Token: 0x06005338 RID: 21304 RVA: 0x0017CFAC File Offset: 0x0017B1AC
		private void SendTouchBegan(Touch touch)
		{
			int num = this.touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				TouchControl touchControl = this.touchControls[i];
				if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
				{
					touchControl.TouchBegan(touch);
				}
			}
		}

		// Token: 0x06005339 RID: 21305 RVA: 0x0017CFF4 File Offset: 0x0017B1F4
		private void SendTouchMoved(Touch touch)
		{
			int num = this.touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				TouchControl touchControl = this.touchControls[i];
				if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
				{
					touchControl.TouchMoved(touch);
				}
			}
		}

		// Token: 0x0600533A RID: 21306 RVA: 0x0017D03C File Offset: 0x0017B23C
		private void SendTouchEnded(Touch touch)
		{
			int num = this.touchControls.Length;
			for (int i = 0; i < num; i++)
			{
				TouchControl touchControl = this.touchControls[i];
				if (touchControl.enabled && touchControl.gameObject.activeInHierarchy)
				{
					touchControl.TouchEnded(touch);
				}
			}
		}

		// Token: 0x0600533B RID: 21307 RVA: 0x0017D084 File Offset: 0x0017B284
		private void InvokeTouchEvents()
		{
			int count = this.activeTouches.Count;
			if (this.enableControlsOnTouch && count > 0 && !this.controlsEnabled)
			{
				TouchManager.Device.RequestActivation();
				this.controlsEnabled = true;
			}
			for (int i = 0; i < count; i++)
			{
				Touch touch = this.activeTouches[i];
				switch (touch.phase)
				{
				case TouchPhase.Began:
					this.SendTouchBegan(touch);
					break;
				case TouchPhase.Moved:
					this.SendTouchMoved(touch);
					break;
				case TouchPhase.Stationary:
					break;
				case TouchPhase.Ended:
					this.SendTouchEnded(touch);
					break;
				case TouchPhase.Canceled:
					this.SendTouchEnded(touch);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		// Token: 0x0600533C RID: 21308 RVA: 0x0017D128 File Offset: 0x0017B328
		private bool TouchCameraIsValid()
		{
			return !(this.touchCamera == null) && !Utility.IsZero(this.touchCamera.orthographicSize) && (!Utility.IsZero(this.touchCamera.rect.width) || !Utility.IsZero(this.touchCamera.rect.height)) && (!Utility.IsZero(this.touchCamera.pixelRect.width) || !Utility.IsZero(this.touchCamera.pixelRect.height));
		}

		// Token: 0x0600533D RID: 21309 RVA: 0x0017D1C8 File Offset: 0x0017B3C8
		private Vector3 ConvertScreenToWorldPoint(Vector2 point)
		{
			if (this.TouchCameraIsValid())
			{
				return this.touchCamera.ScreenToWorldPoint(new Vector3(point.x, point.y, -this.touchCamera.transform.position.z));
			}
			return Vector3.zero;
		}

		// Token: 0x0600533E RID: 21310 RVA: 0x0017D218 File Offset: 0x0017B418
		private Vector3 ConvertViewToWorldPoint(Vector2 point)
		{
			if (this.TouchCameraIsValid())
			{
				return this.touchCamera.ViewportToWorldPoint(new Vector3(point.x, point.y, -this.touchCamera.transform.position.z));
			}
			return Vector3.zero;
		}

		// Token: 0x0600533F RID: 21311 RVA: 0x0017D268 File Offset: 0x0017B468
		private Vector3 ConvertScreenToViewPoint(Vector2 point)
		{
			if (this.TouchCameraIsValid())
			{
				return this.touchCamera.ScreenToViewportPoint(new Vector3(point.x, point.y, -this.touchCamera.transform.position.z));
			}
			return Vector3.zero;
		}

		// Token: 0x06005340 RID: 21312 RVA: 0x0017D2B5 File Offset: 0x0017B4B5
		private Vector2 GetCurrentScreenSize()
		{
			if (this.TouchCameraIsValid())
			{
				return new Vector2((float)this.touchCamera.pixelWidth, (float)this.touchCamera.pixelHeight);
			}
			return new Vector2((float)Screen.width, (float)Screen.height);
		}

		// Token: 0x17000B63 RID: 2915
		// (get) Token: 0x06005341 RID: 21313 RVA: 0x0017D2EE File Offset: 0x0017B4EE
		// (set) Token: 0x06005342 RID: 21314 RVA: 0x0017D2F8 File Offset: 0x0017B4F8
		public bool controlsEnabled
		{
			get
			{
				return this._controlsEnabled;
			}
			set
			{
				if (this._controlsEnabled != value)
				{
					int num = this.touchControls.Length;
					for (int i = 0; i < num; i++)
					{
						this.touchControls[i].enabled = value;
					}
					this._controlsEnabled = value;
				}
			}
		}

		// Token: 0x17000B64 RID: 2916
		// (get) Token: 0x06005343 RID: 21315 RVA: 0x0017D338 File Offset: 0x0017B538
		public static ReadOnlyCollection<Touch> Touches
		{
			get
			{
				return SingletonMonoBehavior<TouchManager>.Instance.readOnlyActiveTouches;
			}
		}

		// Token: 0x17000B65 RID: 2917
		// (get) Token: 0x06005344 RID: 21316 RVA: 0x0017D344 File Offset: 0x0017B544
		public static int TouchCount
		{
			get
			{
				return SingletonMonoBehavior<TouchManager>.Instance.activeTouches.Count;
			}
		}

		// Token: 0x06005345 RID: 21317 RVA: 0x0017D355 File Offset: 0x0017B555
		public static Touch GetTouch(int touchIndex)
		{
			return SingletonMonoBehavior<TouchManager>.Instance.activeTouches[touchIndex];
		}

		// Token: 0x06005346 RID: 21318 RVA: 0x0017D367 File Offset: 0x0017B567
		public static Touch GetTouchByFingerId(int fingerId)
		{
			return SingletonMonoBehavior<TouchManager>.Instance.cachedTouches.FindTouch(fingerId);
		}

		// Token: 0x06005347 RID: 21319 RVA: 0x0017D379 File Offset: 0x0017B579
		public static Vector3 ScreenToWorldPoint(Vector2 point)
		{
			return SingletonMonoBehavior<TouchManager>.Instance.ConvertScreenToWorldPoint(point);
		}

		// Token: 0x06005348 RID: 21320 RVA: 0x0017D386 File Offset: 0x0017B586
		public static Vector3 ViewToWorldPoint(Vector2 point)
		{
			return SingletonMonoBehavior<TouchManager>.Instance.ConvertViewToWorldPoint(point);
		}

		// Token: 0x06005349 RID: 21321 RVA: 0x0017D393 File Offset: 0x0017B593
		public static Vector3 ScreenToViewPoint(Vector2 point)
		{
			return SingletonMonoBehavior<TouchManager>.Instance.ConvertScreenToViewPoint(point);
		}

		// Token: 0x0600534A RID: 21322 RVA: 0x0017D3A0 File Offset: 0x0017B5A0
		public static float ConvertToWorld(float value, TouchUnitType unitType)
		{
			return value * ((unitType == TouchUnitType.Pixels) ? TouchManager.PixelToWorld : TouchManager.PercentToWorld);
		}

		// Token: 0x0600534B RID: 21323 RVA: 0x0017D3B4 File Offset: 0x0017B5B4
		public static Rect PercentToWorldRect(Rect rect)
		{
			return new Rect((rect.xMin - 50f) * TouchManager.ViewSize.x, (rect.yMin - 50f) * TouchManager.ViewSize.y, rect.width * TouchManager.ViewSize.x, rect.height * TouchManager.ViewSize.y);
		}

		// Token: 0x0600534C RID: 21324 RVA: 0x0017D41C File Offset: 0x0017B61C
		public static Rect PixelToWorldRect(Rect rect)
		{
			return new Rect(Mathf.Round(rect.xMin - TouchManager.HalfScreenSize.x) * TouchManager.PixelToWorld, Mathf.Round(rect.yMin - TouchManager.HalfScreenSize.y) * TouchManager.PixelToWorld, Mathf.Round(rect.width) * TouchManager.PixelToWorld, Mathf.Round(rect.height) * TouchManager.PixelToWorld);
		}

		// Token: 0x0600534D RID: 21325 RVA: 0x0017D48C File Offset: 0x0017B68C
		public static Rect ConvertToWorld(Rect rect, TouchUnitType unitType)
		{
			if (unitType != TouchUnitType.Pixels)
			{
				return TouchManager.PercentToWorldRect(rect);
			}
			return TouchManager.PixelToWorldRect(rect);
		}

		// Token: 0x17000B66 RID: 2918
		// (get) Token: 0x0600534E RID: 21326 RVA: 0x0017D49F File Offset: 0x0017B69F
		public static Camera Camera
		{
			get
			{
				return SingletonMonoBehavior<TouchManager>.Instance.touchCamera;
			}
		}

		// Token: 0x17000B67 RID: 2919
		// (get) Token: 0x0600534F RID: 21327 RVA: 0x0017D4AB File Offset: 0x0017B6AB
		public static InputDevice Device
		{
			get
			{
				return SingletonMonoBehavior<TouchManager>.Instance.device;
			}
		}

		// Token: 0x17000B68 RID: 2920
		// (get) Token: 0x06005350 RID: 21328 RVA: 0x0017D4B7 File Offset: 0x0017B6B7
		public static Vector3 ViewSize
		{
			get
			{
				return SingletonMonoBehavior<TouchManager>.Instance.viewSize;
			}
		}

		// Token: 0x17000B69 RID: 2921
		// (get) Token: 0x06005351 RID: 21329 RVA: 0x0017D4C3 File Offset: 0x0017B6C3
		public static float PercentToWorld
		{
			get
			{
				return SingletonMonoBehavior<TouchManager>.Instance.percentToWorld;
			}
		}

		// Token: 0x17000B6A RID: 2922
		// (get) Token: 0x06005352 RID: 21330 RVA: 0x0017D4CF File Offset: 0x0017B6CF
		public static float HalfPercentToWorld
		{
			get
			{
				return SingletonMonoBehavior<TouchManager>.Instance.halfPercentToWorld;
			}
		}

		// Token: 0x17000B6B RID: 2923
		// (get) Token: 0x06005353 RID: 21331 RVA: 0x0017D4DB File Offset: 0x0017B6DB
		public static float PixelToWorld
		{
			get
			{
				return SingletonMonoBehavior<TouchManager>.Instance.pixelToWorld;
			}
		}

		// Token: 0x17000B6C RID: 2924
		// (get) Token: 0x06005354 RID: 21332 RVA: 0x0017D4E7 File Offset: 0x0017B6E7
		public static float HalfPixelToWorld
		{
			get
			{
				return SingletonMonoBehavior<TouchManager>.Instance.halfPixelToWorld;
			}
		}

		// Token: 0x17000B6D RID: 2925
		// (get) Token: 0x06005355 RID: 21333 RVA: 0x0017D4F3 File Offset: 0x0017B6F3
		public static Vector2 ScreenSize
		{
			get
			{
				return SingletonMonoBehavior<TouchManager>.Instance.screenSize;
			}
		}

		// Token: 0x17000B6E RID: 2926
		// (get) Token: 0x06005356 RID: 21334 RVA: 0x0017D4FF File Offset: 0x0017B6FF
		public static Vector2 HalfScreenSize
		{
			get
			{
				return SingletonMonoBehavior<TouchManager>.Instance.halfScreenSize;
			}
		}

		// Token: 0x17000B6F RID: 2927
		// (get) Token: 0x06005357 RID: 21335 RVA: 0x0017D50B File Offset: 0x0017B70B
		public static TouchManager.GizmoShowOption ControlsShowGizmos
		{
			get
			{
				return SingletonMonoBehavior<TouchManager>.Instance.controlsShowGizmos;
			}
		}

		// Token: 0x17000B70 RID: 2928
		// (get) Token: 0x06005358 RID: 21336 RVA: 0x0017D517 File Offset: 0x0017B717
		// (set) Token: 0x06005359 RID: 21337 RVA: 0x0017D523 File Offset: 0x0017B723
		public static bool ControlsEnabled
		{
			get
			{
				return SingletonMonoBehavior<TouchManager>.Instance.controlsEnabled;
			}
			set
			{
				SingletonMonoBehavior<TouchManager>.Instance.controlsEnabled = value;
			}
		}

		// Token: 0x0600535A RID: 21338 RVA: 0x0017D530 File Offset: 0x0017B730
		public static implicit operator bool(TouchManager instance)
		{
			return instance != null;
		}

		// Token: 0x04005349 RID: 21321
		[Space(10f)]
		public Camera touchCamera;

		// Token: 0x0400534A RID: 21322
		public TouchManager.GizmoShowOption controlsShowGizmos = TouchManager.GizmoShowOption.Always;

		// Token: 0x0400534B RID: 21323
		[HideInInspector]
		public bool enableControlsOnTouch;

		// Token: 0x0400534C RID: 21324
		[SerializeField]
		[HideInInspector]
		private bool _controlsEnabled = true;

		// Token: 0x0400534D RID: 21325
		[HideInInspector]
		public int controlsLayer = 5;

		// Token: 0x0400534F RID: 21327
		private InputDevice device;

		// Token: 0x04005350 RID: 21328
		private Vector3 viewSize;

		// Token: 0x04005351 RID: 21329
		private Vector2 screenSize;

		// Token: 0x04005352 RID: 21330
		private Vector2 halfScreenSize;

		// Token: 0x04005353 RID: 21331
		private float percentToWorld;

		// Token: 0x04005354 RID: 21332
		private float halfPercentToWorld;

		// Token: 0x04005355 RID: 21333
		private float pixelToWorld;

		// Token: 0x04005356 RID: 21334
		private float halfPixelToWorld;

		// Token: 0x04005357 RID: 21335
		private TouchControl[] touchControls;

		// Token: 0x04005358 RID: 21336
		private TouchPool cachedTouches;

		// Token: 0x04005359 RID: 21337
		private List<Touch> activeTouches;

		// Token: 0x0400535A RID: 21338
		private ReadOnlyCollection<Touch> readOnlyActiveTouches;

		// Token: 0x0400535B RID: 21339
		private bool isReady;

		// Token: 0x0400535C RID: 21340
		private readonly Touch[] mouseTouches = new Touch[3];

		// Token: 0x02001B67 RID: 7015
		public enum GizmoShowOption
		{
			// Token: 0x04009CC7 RID: 40135
			Never,
			// Token: 0x04009CC8 RID: 40136
			WhenSelected,
			// Token: 0x04009CC9 RID: 40137
			UnlessPlaying,
			// Token: 0x04009CCA RID: 40138
			Always
		}
	}
}
