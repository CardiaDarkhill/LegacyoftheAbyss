using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003F3 RID: 1011
public class JitterSelf : MonoBehaviour, IUpdateBatchableUpdate
{
	// Token: 0x1400006A RID: 106
	// (add) Token: 0x0600227B RID: 8827 RVA: 0x0009EC0C File Offset: 0x0009CE0C
	// (remove) Token: 0x0600227C RID: 8828 RVA: 0x0009EC44 File Offset: 0x0009CE44
	public event Action PositionRestored;

	// Token: 0x17000393 RID: 915
	// (get) Token: 0x0600227D RID: 8829 RVA: 0x0009EC79 File Offset: 0x0009CE79
	private Transform Transform
	{
		get
		{
			if (!this.overrideTransform)
			{
				return base.transform;
			}
			return this.overrideTransform;
		}
	}

	// Token: 0x17000394 RID: 916
	// (get) Token: 0x0600227E RID: 8830 RVA: 0x0009EC98 File Offset: 0x0009CE98
	public bool ShouldUpdate
	{
		get
		{
			if (this.isChangeQueued)
			{
				return true;
			}
			if (!this.isActive)
			{
				return false;
			}
			JitterSelfConfig jitterSelfConfig = this.Config;
			if (jitterSelfConfig.AmountMin.magnitude <= Mathf.Epsilon && jitterSelfConfig.AmountMax.magnitude <= Mathf.Epsilon)
			{
				return false;
			}
			if (this.hasRenderer && this.renderer.enabled)
			{
				return this.isVisible;
			}
			if (this.childRenderers.Length != 0)
			{
				Renderer[] array = this.childRenderers;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].isVisible)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	// Token: 0x17000395 RID: 917
	// (get) Token: 0x0600227F RID: 8831 RVA: 0x0009ED2F File Offset: 0x0009CF2F
	// (set) Token: 0x06002280 RID: 8832 RVA: 0x0009ED37 File Offset: 0x0009CF37
	public float Multiplier
	{
		get
		{
			return this.multiplier;
		}
		set
		{
			this.multiplier = value;
		}
	}

	// Token: 0x17000396 RID: 918
	// (get) Token: 0x06002281 RID: 8833 RVA: 0x0009ED40 File Offset: 0x0009CF40
	private JitterSelfConfig Config
	{
		get
		{
			if (this.hasProfile)
			{
				return this.profile.Config;
			}
			return this.config;
		}
	}

	// Token: 0x06002282 RID: 8834 RVA: 0x0009ED5C File Offset: 0x0009CF5C
	private void OnValidate()
	{
		if (this.frequency > 0f)
		{
			this.config = new JitterSelfConfig
			{
				Frequency = this.frequency,
				AmountMin = this.amount,
				AmountMax = this.amount,
				UseCameraRenderHooks = this.useCameraRenderHooks
			};
			this.frequency = 0f;
		}
		this.hasProfile = this.profile;
	}

	// Token: 0x06002283 RID: 8835 RVA: 0x0009EDD8 File Offset: 0x0009CFD8
	private void Reset()
	{
		this.config = new JitterSelfConfig
		{
			Frequency = 30f
		};
	}

	// Token: 0x06002284 RID: 8836 RVA: 0x0009EE00 File Offset: 0x0009D000
	private void Awake()
	{
		this.OnValidate();
		this.hasProfile = this.profile;
	}

	// Token: 0x06002285 RID: 8837 RVA: 0x0009EE1C File Offset: 0x0009D01C
	private void Start()
	{
		if (!this.isActive)
		{
			this.isActive = !this.startInactive;
			this.multiplier = 1f;
		}
		if (this.Config.UseCameraRenderHooks)
		{
			CameraRenderHooks.CameraPreCull += this.OnCameraPreCull;
			CameraRenderHooks.CameraPostRender += this.OnCameraPostRender;
		}
	}

	// Token: 0x06002286 RID: 8838 RVA: 0x0009EE7C File Offset: 0x0009D07C
	private void OnEnable()
	{
		this.ResetAmountLerp();
		this.SetInitPos();
		this.renderer = base.GetComponent<Renderer>();
		this.hasRenderer = this.renderer;
		if (this.hasRenderer)
		{
			this.isVisible = this.renderer.isVisible;
		}
		Transform transform = this.Transform;
		this.childRenderers = transform.GetComponentsInChildren<Renderer>(true);
		this.updateBatcher = GameManager.instance.GetComponent<UpdateBatcher>();
		this.updateBatcher.Add(this);
	}

	// Token: 0x06002287 RID: 8839 RVA: 0x0009EEFB File Offset: 0x0009D0FB
	private void OnDisable()
	{
		this.updateBatcher.Remove(this);
		this.updateBatcher = null;
		if (this.startInactive)
		{
			this.isActive = false;
		}
	}

	// Token: 0x06002288 RID: 8840 RVA: 0x0009EF20 File Offset: 0x0009D120
	private void OnBecameVisible()
	{
		this.isVisible = true;
	}

	// Token: 0x06002289 RID: 8841 RVA: 0x0009EF29 File Offset: 0x0009D129
	private void OnBecameInvisible()
	{
		this.isVisible = false;
	}

	// Token: 0x0600228A RID: 8842 RVA: 0x0009EF32 File Offset: 0x0009D132
	private void OnDestroy()
	{
		if (this.Config.UseCameraRenderHooks)
		{
			CameraRenderHooks.CameraPreCull -= this.OnCameraPreCull;
			CameraRenderHooks.CameraPostRender -= this.OnCameraPostRender;
		}
	}

	// Token: 0x0600228B RID: 8843 RVA: 0x0009EF64 File Offset: 0x0009D164
	public void BatchedUpdate()
	{
		if (this.isChangeQueued && Time.timeAsDouble >= this.queueEndTime)
		{
			this.isChangeQueued = false;
			if (this.queueActiveState)
			{
				this.InternalStartJitter();
			}
			else
			{
				this.InternalStopJitter(this.stoppingWithDecay);
			}
		}
		if (!this.isActive)
		{
			return;
		}
		JitterSelfConfig jitterSelfConfig = this.Config;
		double num = this.isRealtime ? Time.unscaledTimeAsDouble : Time.timeAsDouble;
		if (num < this.nextJitterTime)
		{
			return;
		}
		float num2 = (jitterSelfConfig.Frequency > 0f) ? jitterSelfConfig.Frequency : 60f;
		this.nextJitterTime = num + (double)(1f / num2);
		Vector3 original = Vector3.Lerp(jitterSelfConfig.AmountMin, jitterSelfConfig.AmountMax, this.amountLerp);
		this.targetPosition = this.initialPosition + original.RandomInRange() * this.multiplier;
		if (!jitterSelfConfig.UseCameraRenderHooks)
		{
			this.Transform.localPosition = this.targetPosition;
		}
	}

	// Token: 0x0600228C RID: 8844 RVA: 0x0009F058 File Offset: 0x0009D258
	private void OnCameraPreCull(CameraRenderHooks.CameraSource cameraType)
	{
		if (!this.isActive)
		{
			return;
		}
		if (cameraType != this.hookCamera || !base.isActiveAndEnabled)
		{
			return;
		}
		if (this.hookCamera == CameraRenderHooks.CameraSource.MainCamera && Time.timeScale <= Mathf.Epsilon)
		{
			return;
		}
		this.preCullPosition = this.Transform.localPosition;
		this.Transform.localPosition = this.targetPosition;
	}

	// Token: 0x0600228D RID: 8845 RVA: 0x0009F0B8 File Offset: 0x0009D2B8
	private void OnCameraPostRender(CameraRenderHooks.CameraSource cameraType)
	{
		if (!this.isActive)
		{
			return;
		}
		if (cameraType != this.hookCamera || !base.isActiveAndEnabled)
		{
			return;
		}
		if (this.hookCamera == CameraRenderHooks.CameraSource.MainCamera && Time.timeScale <= Mathf.Epsilon)
		{
			return;
		}
		this.Transform.localPosition = this.preCullPosition;
		this.initialPosition = this.preCullPosition;
		if (this.PositionRestored != null)
		{
			this.PositionRestored();
		}
	}

	// Token: 0x0600228E RID: 8846 RVA: 0x0009F126 File Offset: 0x0009D326
	private void ResetAmountLerp()
	{
		this.amountLerp = Random.Range(0f, 1f);
	}

	// Token: 0x0600228F RID: 8847 RVA: 0x0009F13D File Offset: 0x0009D33D
	private void SetInitPos()
	{
		this.initialPosition = this.Transform.localPosition;
		this.targetPosition = this.initialPosition;
	}

	// Token: 0x06002290 RID: 8848 RVA: 0x0009F15C File Offset: 0x0009D35C
	public void StartJitter()
	{
		float randomValue = this.Config.Delay.GetRandomValue();
		if (randomValue > 0f)
		{
			this.isChangeQueued = true;
			this.queueActiveState = true;
			this.queueEndTime = Time.timeAsDouble + (double)randomValue;
			return;
		}
		this.isChangeQueued = false;
		this.InternalStartJitter();
	}

	// Token: 0x06002291 RID: 8849 RVA: 0x0009F1B0 File Offset: 0x0009D3B0
	private void InternalStartJitter()
	{
		if (this.isActive)
		{
			this.InternalStopJitter(false);
		}
		this.isActive = true;
		this.multiplier = 1f;
		this.ResetAmountLerp();
		this.SetInitPos();
		if (this.OnJitterStart != null)
		{
			this.OnJitterStart.Invoke();
		}
	}

	// Token: 0x06002292 RID: 8850 RVA: 0x0009F1FD File Offset: 0x0009D3FD
	public void StopJitter()
	{
		this.StopJitterShared(false);
	}

	// Token: 0x06002293 RID: 8851 RVA: 0x0009F206 File Offset: 0x0009D406
	public void StopJitterWithDecay()
	{
		this.StopJitterShared(true);
	}

	// Token: 0x06002294 RID: 8852 RVA: 0x0009F20F File Offset: 0x0009D40F
	protected virtual void OnStopJitter()
	{
	}

	// Token: 0x06002295 RID: 8853 RVA: 0x0009F214 File Offset: 0x0009D414
	private void StopJitterShared(bool withDecay)
	{
		if (!this.isActive)
		{
			return;
		}
		this.stoppingWithDecay = withDecay;
		float randomValue = this.Config.Delay.GetRandomValue();
		if (randomValue > 0f)
		{
			this.isChangeQueued = true;
			this.queueActiveState = false;
			this.queueEndTime = Time.timeAsDouble + (double)randomValue;
			return;
		}
		this.isChangeQueued = false;
		this.InternalStopJitter(withDecay);
	}

	// Token: 0x06002296 RID: 8854 RVA: 0x0009F278 File Offset: 0x0009D478
	protected void InternalStopJitter(bool withDecay)
	{
		if (withDecay)
		{
			if (this.decayRoutine == null)
			{
				this.decayRoutine = base.StartCoroutine(this.JitterDecayThenStop());
				return;
			}
		}
		else
		{
			this.StopJitterCompletely();
		}
	}

	// Token: 0x06002297 RID: 8855 RVA: 0x0009F29E File Offset: 0x0009D49E
	private void StopJitterCompletely()
	{
		this.OnStopJitter();
		this.isActive = false;
		if (!this.config.UseCameraRenderHooks)
		{
			this.Transform.localPosition = this.initialPosition;
		}
		if (this.OnJitterEnd != null)
		{
			this.OnJitterEnd.Invoke();
		}
	}

	// Token: 0x06002298 RID: 8856 RVA: 0x0009F2DE File Offset: 0x0009D4DE
	private IEnumerator JitterDecayThenStop()
	{
		for (float elapsed = 0f; elapsed < 0.5f; elapsed += Time.deltaTime)
		{
			this.multiplier = 1f - elapsed / 0.5f;
			yield return null;
		}
		this.multiplier = 0f;
		this.decayRoutine = null;
		this.StopJitterCompletely();
		yield break;
	}

	// Token: 0x06002299 RID: 8857 RVA: 0x0009F2ED File Offset: 0x0009D4ED
	public static JitterSelf Add(GameObject gameObject, JitterSelfConfig config, CameraRenderHooks.CameraSource hookCamera)
	{
		JitterSelf jitterSelf = gameObject.AddComponent<JitterSelf>();
		jitterSelf.startInactive = true;
		jitterSelf.config = config;
		jitterSelf.hookCamera = hookCamera;
		return jitterSelf;
	}

	// Token: 0x04002151 RID: 8529
	[SerializeField]
	private JitterSelfProfile profile;

	// Token: 0x04002152 RID: 8530
	[SerializeField]
	[ModifiableProperty]
	[Conditional("profile", false, false, false)]
	protected JitterSelfConfig config;

	// Token: 0x04002153 RID: 8531
	[HideInInspector]
	[SerializeField]
	[Obsolete]
	private float frequency;

	// Token: 0x04002154 RID: 8532
	[HideInInspector]
	[SerializeField]
	[Obsolete]
	private Vector3 amount;

	// Token: 0x04002155 RID: 8533
	[HideInInspector]
	[SerializeField]
	[Obsolete]
	private bool useCameraRenderHooks;

	// Token: 0x04002156 RID: 8534
	[SerializeField]
	protected bool startInactive;

	// Token: 0x04002157 RID: 8535
	[SerializeField]
	private bool isRealtime;

	// Token: 0x04002158 RID: 8536
	[SerializeField]
	private CameraRenderHooks.CameraSource hookCamera = CameraRenderHooks.CameraSource.MainCamera;

	// Token: 0x04002159 RID: 8537
	[Space]
	[SerializeField]
	private Transform overrideTransform;

	// Token: 0x0400215A RID: 8538
	[Space]
	public UnityEvent OnJitterStart;

	// Token: 0x0400215B RID: 8539
	public UnityEvent OnJitterEnd;

	// Token: 0x0400215C RID: 8540
	private bool isChangeQueued;

	// Token: 0x0400215D RID: 8541
	private bool queueActiveState;

	// Token: 0x0400215E RID: 8542
	private double queueEndTime;

	// Token: 0x0400215F RID: 8543
	private bool isActive;

	// Token: 0x04002160 RID: 8544
	private double nextJitterTime;

	// Token: 0x04002161 RID: 8545
	private Vector3 initialPosition;

	// Token: 0x04002162 RID: 8546
	private Vector3 targetPosition;

	// Token: 0x04002163 RID: 8547
	private Vector3 preCullPosition;

	// Token: 0x04002164 RID: 8548
	private float amountLerp;

	// Token: 0x04002165 RID: 8549
	private float multiplier;

	// Token: 0x04002166 RID: 8550
	private bool stoppingWithDecay;

	// Token: 0x04002167 RID: 8551
	private Coroutine decayRoutine;

	// Token: 0x04002168 RID: 8552
	private UpdateBatcher updateBatcher;

	// Token: 0x04002169 RID: 8553
	private Renderer renderer;

	// Token: 0x0400216A RID: 8554
	private bool hasRenderer;

	// Token: 0x0400216B RID: 8555
	private bool isVisible;

	// Token: 0x0400216C RID: 8556
	private Renderer[] childRenderers;

	// Token: 0x0400216D RID: 8557
	private bool hasProfile;
}
