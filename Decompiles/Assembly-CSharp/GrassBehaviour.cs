using System;
using GlobalSettings;
using UnityEngine;

// Token: 0x0200023B RID: 571
public class GrassBehaviour : MonoBehaviour, IUpdateBatchableFixedUpdate, IUpdateBatchableLateUpdate
{
	// Token: 0x17000247 RID: 583
	// (get) Token: 0x060014DD RID: 5341 RVA: 0x0005E24F File Offset: 0x0005C44F
	public bool ShouldUpdate
	{
		get
		{
			return this.hasStarted;
		}
	}

	// Token: 0x060014DE RID: 5342 RVA: 0x0005E258 File Offset: 0x0005C458
	private void Awake()
	{
		this.source = base.GetComponent<AudioSource>();
		this.animator = base.GetComponentInChildren<Animator>();
		this.col = base.GetComponent<Collider2D>();
		this.propertyBlock = new MaterialPropertyBlock();
		this.updateBatcher = GameManager.instance.GetComponent<UpdateBatcher>();
		this.updateBatcher.Add(this);
	}

	// Token: 0x060014DF RID: 5343 RVA: 0x0005E2B0 File Offset: 0x0005C4B0
	private void OnDestroy()
	{
		if (!this.updateBatcher)
		{
			return;
		}
		this.updateBatcher.Remove(this);
		this.updateBatcher = null;
	}

	// Token: 0x060014E0 RID: 5344 RVA: 0x0005E2D4 File Offset: 0x0005C4D4
	private void Start()
	{
		if (!base.transform.IsOnHeroPlane())
		{
			GrassCut component = base.GetComponent<GrassCut>();
			if (component)
			{
				Object.Destroy(component);
			}
			Collider2D[] componentsInChildren = base.GetComponentsInChildren<Collider2D>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Object.Destroy(componentsInChildren[i]);
			}
		}
		this.renderers = base.GetComponentsInChildren<SpriteRenderer>(true);
		this.pushAmountError = Random.Range(-0.01f, 0.01f);
		if (this.disableSizeMultiplier)
		{
			this.camShakePushMultiplier = 1f;
		}
		else
		{
			float num = 0f;
			foreach (SpriteRenderer spriteRenderer in this.renderers)
			{
				this.SetPushAmount(spriteRenderer, this.pushAmountError);
				float y = spriteRenderer.bounds.size.y;
				if (y > num)
				{
					num = y;
				}
			}
			this.camShakePushMultiplier = 0.5f * (3f / num);
		}
		base.transform.SetPositionZ(base.transform.position.z + Random.Range(-0.0001f, 0.0001f));
		this.hasStarted = true;
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x0005E3EC File Offset: 0x0005C5EC
	private void OnEnable()
	{
		CameraManagerReference mainCameraShakeManager = GlobalSettings.Camera.MainCameraShakeManager;
		if (mainCameraShakeManager)
		{
			mainCameraShakeManager.CameraShakedWorldForce += this.OnCameraShaked;
		}
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x0005E41C File Offset: 0x0005C61C
	private void OnDisable()
	{
		CameraManagerReference mainCameraShakeManager = GlobalSettings.Camera.MainCameraShakeManager;
		if (mainCameraShakeManager)
		{
			mainCameraShakeManager.CameraShakedWorldForce -= this.OnCameraShaked;
		}
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x0005E44C File Offset: 0x0005C64C
	public void BatchedLateUpdate()
	{
		if (this.returned)
		{
			return;
		}
		float num = (this.animLength > 0f) ? (this.curve.Evaluate(this.animElapsed / this.animLength) * this.pushAmount * this.pushDirection * Mathf.Sign(base.transform.lossyScale.x) + this.pushAmountError) : 0f;
		if (!Mathf.Approximately(this.previousPushAmount, num))
		{
			foreach (SpriteRenderer rend in this.renderers)
			{
				this.SetPushAmount(rend, num);
			}
			this.previousPushAmount = num;
		}
		if (this.animElapsed >= this.animLength)
		{
			this.returned = true;
			if (this.animator && this.animator.HasParameter(this.pushAnim, new AnimatorControllerParameterType?(AnimatorControllerParameterType.Bool)))
			{
				this.animator.SetBool(this.pushAnim, false);
			}
		}
		this.animElapsed += Time.deltaTime;
	}

	// Token: 0x060014E4 RID: 5348 RVA: 0x0005E550 File Offset: 0x0005C750
	public void BatchedFixedUpdate()
	{
		if (!this.player || !this.returned || Mathf.Abs(this.player.linearVelocity.x) < 0.1f)
		{
			return;
		}
		this.pushDirection = Mathf.Sign(this.player.linearVelocity.x);
		this.returned = false;
		this.animElapsed = 0f;
		this.pushAmount = this.walkReactAmount;
		this.curve = this.walkReactCurve;
		this.animLength = this.walkReactLength;
		if (!this.isCut)
		{
			this.PlayRandomSound(this.pushSounds, false);
		}
		if (!this.animator)
		{
			return;
		}
		if (this.animator.HasParameter(this.pushAnim, new AnimatorControllerParameterType?(AnimatorControllerParameterType.Bool)))
		{
			this.animator.SetBool(this.pushAnim, true);
			return;
		}
		if (this.animator.HasParameter(this.pushAnim, new AnimatorControllerParameterType?(AnimatorControllerParameterType.Trigger)))
		{
			this.animator.SetTrigger(this.pushAnim);
		}
	}

	// Token: 0x060014E5 RID: 5349 RVA: 0x0005E65C File Offset: 0x0005C85C
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!this.returned)
		{
			return;
		}
		if (GameManager.instance.IsInSceneTransition)
		{
			return;
		}
		Transform transform = base.transform;
		Vector2 a = this.col ? this.col.bounds.center : transform.position;
		float num;
		for (num = transform.eulerAngles.z; num < 0f; num += 360f)
		{
		}
		while (num > 360f)
		{
			num -= 360f;
		}
		Vector2 rhs = new Vector2(Mathf.Cos(num), Mathf.Sin(num));
		Rigidbody2D component = collision.GetComponent<Rigidbody2D>();
		Vector2 lhs;
		if (component != null)
		{
			lhs = component.linearVelocity;
		}
		else
		{
			lhs = a - collision.transform.position;
		}
		if (lhs.magnitude > 0f)
		{
			lhs.Normalize();
		}
		this.pushDirection = Mathf.Sign(Vector2.Dot(lhs, rhs));
		this.returned = false;
		this.animElapsed = 0f;
		if (GrassCut.ShouldCut(collision))
		{
			this.pushAmount = this.attackReactAmount;
			this.curve = this.attackReactCurve;
			this.animLength = this.attackReactLength;
			if (!this.isCut)
			{
				this.PlayRandomSound(this.pushSounds, false);
			}
		}
		else
		{
			this.pushAmount = this.walkReactAmount;
			this.curve = this.walkReactCurve;
			this.animLength = this.walkReactLength;
			if (collision.CompareTag("Player"))
			{
				this.player = collision.GetComponent<Rigidbody2D>();
			}
			if (!this.isCut)
			{
				this.PlayRandomSound(this.pushSounds, false);
			}
		}
		if (this.animator && this.animator.HasParameter(this.pushAnim, null))
		{
			this.animator.SetBool(this.pushAnim, true);
		}
	}

	// Token: 0x060014E6 RID: 5350 RVA: 0x0005E838 File Offset: 0x0005CA38
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			this.player = null;
		}
	}

	// Token: 0x060014E7 RID: 5351 RVA: 0x0005E850 File Offset: 0x0005CA50
	public void CutReact(Collider2D collision)
	{
		if (!this.isCut)
		{
			if (collision)
			{
				this.pushDirection = Mathf.Sign(base.transform.position.x - collision.transform.position.x);
			}
			else
			{
				this.pushDirection = (float)((Random.Range(0, 2) > 0) ? -1 : 1);
			}
			this.returned = false;
			this.animElapsed = 0f;
			this.pushAmount = this.attackReactAmount;
			this.curve = this.attackReactCurve;
			this.animLength = this.attackReactLength;
		}
		this.PlayRandomSound(this.cutSounds, true);
		this.isCut = true;
	}

	// Token: 0x060014E8 RID: 5352 RVA: 0x0005E900 File Offset: 0x0005CB00
	public void WindReact(Collider2D collision)
	{
		if (!this.returned)
		{
			return;
		}
		this.pushDirection = Mathf.Sign(base.transform.position.x - collision.transform.position.x);
		this.returned = false;
		this.animElapsed = 0f;
		this.pushAmount = this.walkReactAmount;
		this.curve = this.walkReactCurve;
		this.animLength = this.walkReactLength;
		if (!this.isCut)
		{
			this.PlayRandomSound(this.pushSounds, false);
		}
	}

	// Token: 0x060014E9 RID: 5353 RVA: 0x0005E990 File Offset: 0x0005CB90
	private void PlayRandomSound(AudioClip[] clips, bool limitFrequency)
	{
		if (!this.source || clips.Length == 0)
		{
			return;
		}
		if (limitFrequency)
		{
			if (Time.unscaledTimeAsDouble < GrassBehaviour._audioPlayCooldown)
			{
				return;
			}
			GrassBehaviour._audioPlayCooldown = Time.unscaledTimeAsDouble + (double)Audio.AudioEventFrequencyLimit;
		}
		AudioClip clip = clips[Random.Range(0, clips.Length)];
		this.source.PlayOneShot(clip);
	}

	// Token: 0x060014EA RID: 5354 RVA: 0x0005E9E8 File Offset: 0x0005CBE8
	private void SetPushAmount(Renderer rend, float value)
	{
		rend.GetPropertyBlock(this.propertyBlock);
		this.propertyBlock.SetFloat(GrassBehaviour._pushAmountShaderProp, value);
		rend.SetPropertyBlock(this.propertyBlock);
	}

	// Token: 0x060014EB RID: 5355 RVA: 0x0005EA14 File Offset: 0x0005CC14
	private void OnCameraShaked(Vector2 cameraPosition, CameraShakeWorldForceIntensities intensity)
	{
		if (intensity < CameraShakeWorldForceIntensities.Medium)
		{
			return;
		}
		this.pushDirection = (float)((Random.Range(0, 2) > 0) ? -1 : 1);
		this.returned = false;
		this.animElapsed = 0f;
		if (intensity <= CameraShakeWorldForceIntensities.Medium)
		{
			this.pushAmount = this.walkReactAmount;
			this.curve = this.walkReactCurve;
			this.animLength = this.walkReactLength;
		}
		else
		{
			this.pushAmount = this.attackReactAmount;
			this.curve = this.attackReactCurve;
			this.animLength = this.attackReactLength;
		}
		this.pushAmount *= this.camShakePushMultiplier;
		this.animLength += Random.Range(-0.3f, 0.3f);
		if (this.animator && this.animator.HasParameter(this.pushAnim, null))
		{
			this.animator.SetBool(this.pushAnim, true);
		}
	}

	// Token: 0x04001359 RID: 4953
	[Header("Animation")]
	public float walkReactAmount = 1f;

	// Token: 0x0400135A RID: 4954
	public AnimationCurve walkReactCurve;

	// Token: 0x0400135B RID: 4955
	public float walkReactLength;

	// Token: 0x0400135C RID: 4956
	[Space]
	public float attackReactAmount = 2f;

	// Token: 0x0400135D RID: 4957
	public AnimationCurve attackReactCurve;

	// Token: 0x0400135E RID: 4958
	public float attackReactLength;

	// Token: 0x0400135F RID: 4959
	[Space]
	public bool disableSizeMultiplier;

	// Token: 0x04001360 RID: 4960
	[Space]
	public string pushAnim = "Push";

	// Token: 0x04001361 RID: 4961
	private Animator animator;

	// Token: 0x04001362 RID: 4962
	[Header("Sound")]
	public AudioClip[] pushSounds;

	// Token: 0x04001363 RID: 4963
	public AudioClip[] cutSounds;

	// Token: 0x04001364 RID: 4964
	private AudioSource source;

	// Token: 0x04001365 RID: 4965
	private AnimationCurve curve;

	// Token: 0x04001366 RID: 4966
	private float animLength = 2f;

	// Token: 0x04001367 RID: 4967
	private float animElapsed;

	// Token: 0x04001368 RID: 4968
	private float pushAmount = 1f;

	// Token: 0x04001369 RID: 4969
	private float pushDirection;

	// Token: 0x0400136A RID: 4970
	private bool returned = true;

	// Token: 0x0400136B RID: 4971
	private bool isCut;

	// Token: 0x0400136C RID: 4972
	private float pushAmountError;

	// Token: 0x0400136D RID: 4973
	private Rigidbody2D player;

	// Token: 0x0400136E RID: 4974
	private Collider2D col;

	// Token: 0x0400136F RID: 4975
	private SpriteRenderer[] renderers;

	// Token: 0x04001370 RID: 4976
	private float camShakePushMultiplier;

	// Token: 0x04001371 RID: 4977
	private MaterialPropertyBlock propertyBlock;

	// Token: 0x04001372 RID: 4978
	private static double _audioPlayCooldown;

	// Token: 0x04001373 RID: 4979
	private static readonly int _pushAmountShaderProp = Shader.PropertyToID("_PushAmount");

	// Token: 0x04001374 RID: 4980
	private float previousPushAmount = -1f;

	// Token: 0x04001375 RID: 4981
	private bool hasStarted;

	// Token: 0x04001376 RID: 4982
	private UpdateBatcher updateBatcher;
}
