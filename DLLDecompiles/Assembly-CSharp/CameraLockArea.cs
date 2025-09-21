using System;
using System.Collections;
using GlobalEnums;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x0200015F RID: 351
public class CameraLockArea : TrackTriggerObjects
{
	// Token: 0x14000013 RID: 19
	// (add) Token: 0x06000AE2 RID: 2786 RVA: 0x00031438 File Offset: 0x0002F638
	// (remove) Token: 0x06000AE3 RID: 2787 RVA: 0x00031470 File Offset: 0x0002F670
	public event Action<CameraLockArea> OnDestroyEvent;

	// Token: 0x06000AE4 RID: 2788 RVA: 0x000314A5 File Offset: 0x0002F6A5
	private void OnValidate()
	{
		if (this.maxPriority)
		{
			this.maxPriority = false;
			this.priority = 1;
		}
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x000314C0 File Offset: 0x0002F6C0
	protected override void Awake()
	{
		base.Awake();
		this.box2d = base.GetComponent<Collider2D>();
		this.gcams = GameCameras.instance;
		this.cameraCtrl = this.gcams.cameraController;
		this.camTarget = this.gcams.cameraTarget;
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x0003150C File Offset: 0x0002F70C
	protected override void OnEnable()
	{
		if (this.positionSpace == Space.Self)
		{
			this.ValidateBounds();
		}
		base.OnEnable();
		if (!this.hasStarted)
		{
			this.hasStarted = true;
			base.StartCoroutine(this.StartRoutine());
		}
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x00031540 File Offset: 0x0002F740
	private void OnDestroy()
	{
		Action<CameraLockArea> onDestroyEvent = this.OnDestroyEvent;
		if (onDestroyEvent != null)
		{
			onDestroyEvent(this);
		}
		this.OnDestroyEvent = null;
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x0003155B File Offset: 0x0002F75B
	private IEnumerator StartRoutine()
	{
		Scene scene = base.gameObject.scene;
		while (this.cameraCtrl.tilemap == null || this.cameraCtrl.tilemap.gameObject.scene != scene)
		{
			yield return null;
		}
		this.ValidateBounds();
		if (this.box2d != null)
		{
			Bounds bounds = this.box2d.bounds;
			this.leftSideX = bounds.min.x;
			this.rightSideX = bounds.max.x;
			this.botSideY = bounds.min.y;
			this.topSideY = bounds.max.y;
		}
		yield break;
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x0003156C File Offset: 0x0002F76C
	public static bool IsInApplicableGameState()
	{
		GameManager unsafeInstance = GameManager.UnsafeInstance;
		return !(unsafeInstance == null) && (unsafeInstance.GameState == GameState.PLAYING || unsafeInstance.GameState == GameState.ENTERING_LEVEL || unsafeInstance.GameState == GameState.CUTSCENE);
	}

	// Token: 0x06000AEA RID: 2794 RVA: 0x000315A8 File Offset: 0x0002F7A8
	protected override void OnInsideStateChanged(bool isInside)
	{
		HeroController silentInstance = HeroController.SilentInstance;
		if (!silentInstance)
		{
			return;
		}
		this.heroPos = silentInstance.transform.position;
		if (!isInside)
		{
			if (this.box2d != null)
			{
				if (this.heroPos.x > this.leftSideX - 1f && this.heroPos.x < this.leftSideX + 1f)
				{
					this.camTarget.exitedLeft = true;
				}
				else
				{
					this.camTarget.exitedLeft = false;
				}
				if (this.heroPos.x > this.rightSideX - 1f && this.heroPos.x < this.rightSideX + 1f)
				{
					this.camTarget.exitedRight = true;
				}
				else
				{
					this.camTarget.exitedRight = false;
				}
				if (this.heroPos.y > this.topSideY - 2f && this.heroPos.y < this.topSideY + 2f)
				{
					this.camTarget.exitedTop = true;
				}
				else
				{
					this.camTarget.exitedTop = false;
				}
				if (this.heroPos.y > this.botSideY - 1f && this.heroPos.y < this.botSideY + 1f)
				{
					this.camTarget.exitedBot = true;
				}
				else
				{
					this.camTarget.exitedBot = false;
				}
			}
			this.cameraCtrl.ReleaseLock(this);
			return;
		}
		if (!CameraLockArea.IsInApplicableGameState())
		{
			return;
		}
		if (this.box2d != null)
		{
			if (this.heroPos.x > this.leftSideX - 1f && this.heroPos.x < this.leftSideX + 1f)
			{
				this.camTarget.enteredLeft = true;
			}
			else
			{
				this.camTarget.enteredLeft = false;
			}
			if (this.heroPos.x > this.rightSideX - 1f && this.heroPos.x < this.rightSideX + 1f)
			{
				this.camTarget.enteredRight = true;
			}
			else
			{
				this.camTarget.enteredRight = false;
			}
			if (this.heroPos.y > this.topSideY - 2f && this.heroPos.y < this.topSideY + 2f)
			{
				this.camTarget.enteredTop = true;
			}
			else
			{
				this.camTarget.enteredTop = false;
			}
			if (this.heroPos.y > this.botSideY - 1f && this.heroPos.y < this.botSideY + 1f)
			{
				this.camTarget.enteredBot = true;
			}
			else
			{
				this.camTarget.enteredBot = false;
			}
		}
		this.cameraCtrl.LockToArea(this);
	}

	// Token: 0x06000AEB RID: 2795 RVA: 0x00031880 File Offset: 0x0002FA80
	private bool ValidateBounds()
	{
		this.GetWorldBounds(out this.cameraXMin, out this.cameraYMin, out this.cameraXMax, out this.cameraYMax, out this.lookYMin, out this.lookYMax);
		this.positionSpace = Space.World;
		if (this.cameraXMin < 0f)
		{
			this.cameraXMin = 14.6f;
		}
		if (this.cameraXMax < 0f)
		{
			this.cameraXMax = this.cameraCtrl.xLimit;
		}
		if (this.cameraYMin < 0f)
		{
			this.cameraYMin = 8.3f;
		}
		if (this.cameraYMax < 0f)
		{
			this.cameraYMax = this.cameraCtrl.yLimit;
		}
		if (this.lookYMin < 0f)
		{
			this.lookYMin = this.cameraYMin;
		}
		if (this.lookYMax < 0f)
		{
			this.lookYMax = this.cameraYMax;
		}
		return Math.Abs(this.cameraXMin) > Mathf.Epsilon || Math.Abs(this.cameraXMax) > Mathf.Epsilon || Math.Abs(this.cameraYMin) > Mathf.Epsilon || Math.Abs(this.cameraYMax) > Mathf.Epsilon;
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x000319A7 File Offset: 0x0002FBA7
	public void SetXMin(float xMin)
	{
		this.cameraXMin = xMin;
		this.DidResetBounds();
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x000319B6 File Offset: 0x0002FBB6
	public void SetXMax(float xMax)
	{
		this.cameraXMax = xMax;
		this.DidResetBounds();
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x000319C5 File Offset: 0x0002FBC5
	public void SetYMin(float yMin)
	{
		this.cameraYMin = yMin;
		this.DidResetBounds();
	}

	// Token: 0x06000AEF RID: 2799 RVA: 0x000319D4 File Offset: 0x0002FBD4
	public void SetYMax(float yMax)
	{
		this.cameraYMax = yMax;
		this.DidResetBounds();
	}

	// Token: 0x06000AF0 RID: 2800 RVA: 0x000319E3 File Offset: 0x0002FBE3
	private void DidResetBounds()
	{
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x000319E8 File Offset: 0x0002FBE8
	public void GetWorldBounds(out float outCameraXMin, out float outCameraYMin, out float outCameraXMax, out float outCameraYMax, out float outLookYMin, out float outLookYMax)
	{
		if (this.positionSpace == Space.World)
		{
			outCameraXMin = this.cameraXMin;
			outCameraYMin = this.cameraYMin;
			outCameraXMax = this.cameraXMax;
			outCameraYMax = this.cameraYMax;
			outLookYMin = this.lookYMin;
			outLookYMax = this.lookYMax;
			return;
		}
		Vector2 vector = new Vector2(this.cameraXMin, this.cameraYMin);
		Vector2 vector2 = new Vector2(this.cameraXMax, this.cameraYMax);
		Vector2 vector3 = base.transform.position;
		vector += vector3;
		vector2 += vector3;
		outCameraXMin = vector.x;
		outCameraYMin = vector.y;
		outCameraXMax = vector2.x;
		outCameraYMax = vector2.y;
		outLookYMin = this.lookYMin + vector3.y;
		outLookYMax = this.lookYMax + vector3.y;
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x00031AB9 File Offset: 0x0002FCB9
	public override void AddDebugDrawComponent()
	{
		DebugDrawColliderRuntime.AddOrUpdate(base.gameObject, DebugDrawColliderRuntime.ColorType.CameraLock, false);
	}

	// Token: 0x04000A5F RID: 2655
	public float cameraXMin = -1f;

	// Token: 0x04000A60 RID: 2656
	public float cameraYMin = -1f;

	// Token: 0x04000A61 RID: 2657
	public float cameraXMax = -1f;

	// Token: 0x04000A62 RID: 2658
	public float cameraYMax = -1f;

	// Token: 0x04000A63 RID: 2659
	public Space positionSpace;

	// Token: 0x04000A64 RID: 2660
	[Space]
	public bool preventLookUp;

	// Token: 0x04000A65 RID: 2661
	[ModifiableProperty]
	[Conditional("preventLookUp", true, false, false)]
	public float lookYMax = -1f;

	// Token: 0x04000A66 RID: 2662
	public bool preventLookDown;

	// Token: 0x04000A67 RID: 2663
	[ModifiableProperty]
	[Conditional("preventLookDown", true, false, false)]
	public float lookYMin = -1f;

	// Token: 0x04000A68 RID: 2664
	[Space]
	public int priority;

	// Token: 0x04000A69 RID: 2665
	[Obsolete]
	[SerializeField]
	[HideInInspector]
	private bool maxPriority;

	// Token: 0x04000A6A RID: 2666
	private float leftSideX;

	// Token: 0x04000A6B RID: 2667
	private float rightSideX;

	// Token: 0x04000A6C RID: 2668
	private float topSideY;

	// Token: 0x04000A6D RID: 2669
	private float botSideY;

	// Token: 0x04000A6E RID: 2670
	private Vector3 heroPos;

	// Token: 0x04000A6F RID: 2671
	private bool enteredLeft;

	// Token: 0x04000A70 RID: 2672
	private bool enteredRight;

	// Token: 0x04000A71 RID: 2673
	private bool enteredTop;

	// Token: 0x04000A72 RID: 2674
	private bool enteredBot;

	// Token: 0x04000A73 RID: 2675
	private bool exitedLeft;

	// Token: 0x04000A74 RID: 2676
	private bool exitedRight;

	// Token: 0x04000A75 RID: 2677
	private bool exitedTop;

	// Token: 0x04000A76 RID: 2678
	private bool exitedBot;

	// Token: 0x04000A77 RID: 2679
	private bool hasStarted;

	// Token: 0x04000A79 RID: 2681
	private GameCameras gcams;

	// Token: 0x04000A7A RID: 2682
	private CameraController cameraCtrl;

	// Token: 0x04000A7B RID: 2683
	private CameraTarget camTarget;

	// Token: 0x04000A7C RID: 2684
	private Collider2D box2d;
}
