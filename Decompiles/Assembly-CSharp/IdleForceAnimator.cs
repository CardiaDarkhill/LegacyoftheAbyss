using System;
using System.Collections.Generic;
using System.Linq;
using GlobalEnums;
using GlobalSettings;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x02000509 RID: 1289
public class IdleForceAnimator : MonoBehaviour, GameManager.ISceneManualSimulatePhysics
{
	// Token: 0x06002E11 RID: 11793 RVA: 0x000CA2AC File Offset: 0x000C84AC
	private void OnValidate()
	{
		bool flag = false;
		if (this.bodies != null)
		{
			Rigidbody2D[] array = this.bodies;
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i] != null))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			this.bodies = (from body in this.bodies
			where body != null
			select body).ToArray<Rigidbody2D>();
		}
	}

	// Token: 0x06002E12 RID: 11794 RVA: 0x000CA320 File Offset: 0x000C8520
	private void Awake()
	{
		this.OnValidate();
		ReplaceWithTemplate[] componentsInChildren = base.GetComponentsInChildren<ReplaceWithTemplate>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Awake();
		}
		this.parentConfig = base.GetComponentInParent<ParentSwayConfig>();
		this.isIdleSwayActive = (!this.parentConfig || this.parentConfig.HasIdleSway);
		if (this.useChildren)
		{
			this.bodies = base.GetComponentsInChildren<Rigidbody2D>();
		}
		Rigidbody2D[] array = this.bodies;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].WakeUp();
		}
		this.visibilityGroup = base.gameObject.AddComponentIfNotPresent<VisibilityGroup>();
		this.isVisible = this.visibilityGroup.IsVisible;
		this.visibilityGroup.OnVisibilityChanged += delegate(bool visible)
		{
			this.isVisible = visible;
		};
		if (!this.disableCamShakeEventHandling && (this.ignoreExistingCamEventReceiver || base.GetComponentsInChildren<CameraShakeEventReceiver>().Length == 0))
		{
			GlobalSettings.Camera.MainCameraShakeManager.CameraShakedWorldForce += this.OnCameraShaked;
			this.subscribedCameraShaked = true;
		}
	}

	// Token: 0x06002E13 RID: 11795 RVA: 0x000CA41D File Offset: 0x000C861D
	private void Start()
	{
		this.gm = GameManager.instance;
		this.started = true;
		ComponentSingleton<IdleForceAnimatorCallbackHooks>.Instance.OnFixedUpdate += this.OnFixedUpdate;
	}

	// Token: 0x06002E14 RID: 11796 RVA: 0x000CA447 File Offset: 0x000C8647
	private void OnDestroy()
	{
		if (this.subscribedCameraShaked)
		{
			GlobalSettings.Camera.MainCameraShakeManager.CameraShakedWorldForce -= this.OnCameraShaked;
		}
	}

	// Token: 0x06002E15 RID: 11797 RVA: 0x000CA467 File Offset: 0x000C8667
	private void OnEnable()
	{
		if (this.started)
		{
			ComponentSingleton<IdleForceAnimatorCallbackHooks>.Instance.OnFixedUpdate += this.OnFixedUpdate;
		}
		IdleForceAnimator._activeAnimators.Add(this);
	}

	// Token: 0x06002E16 RID: 11798 RVA: 0x000CA492 File Offset: 0x000C8692
	private void OnDisable()
	{
		ComponentSingleton<IdleForceAnimatorCallbackHooks>.Instance.OnFixedUpdate -= this.OnFixedUpdate;
		IdleForceAnimator._activeAnimators.Remove(this);
		base.StopAllCoroutines();
	}

	// Token: 0x06002E17 RID: 11799 RVA: 0x000CA4BC File Offset: 0x000C86BC
	private void OnFixedUpdate()
	{
		if (!this.isVisible)
		{
			return;
		}
		if (!this.isIdleSwayActive)
		{
			return;
		}
		this.OnManualPhysics(Time.deltaTime);
	}

	// Token: 0x06002E18 RID: 11800 RVA: 0x000CA4DC File Offset: 0x000C86DC
	public void OnManualPhysics(float deltaTime)
	{
		this.elapsedTime += deltaTime * this.SpeedMultiplier;
		float num = this.idleSwingForceCurve.Evaluate(this.elapsedTime + base.transform.position.x * this.worldXTimeOffsetAmount);
		float num2 = num * this.idleSwingForceMagnitude * this.SpeedMultiplier;
		float num3 = num * this.torqueMagnitude * this.SpeedMultiplier;
		if (this.gm.GetCurrentMapZoneEnum() == MapZone.JUDGE_STEPS && (!this.parentConfig || this.parentConfig.ApplyMapZoneSway))
		{
			num2 *= 4f;
			num3 *= 4f;
			num2 += 10f * this.SpeedMultiplier;
		}
		num2 += this.ExtraHorizontalSpeed;
		foreach (Rigidbody2D rigidbody2D in this.bodies)
		{
			if (rigidbody2D)
			{
				if (Mathf.Abs(num2) > Mathf.Epsilon)
				{
					rigidbody2D.AddForce(new Vector2(num2, 0f), ForceMode2D.Force);
				}
				if (Mathf.Abs(num3) > Mathf.Epsilon)
				{
					rigidbody2D.AddTorque(num3, ForceMode2D.Force);
				}
			}
		}
	}

	// Token: 0x06002E19 RID: 11801 RVA: 0x000CA5F0 File Offset: 0x000C87F0
	public void PrepareManualSimulate()
	{
		this.gm = GameManager.instance;
		foreach (Rigidbody2D rigidbody2D in this.bodies)
		{
			rigidbody2D.WakeUp();
			HingeJoint2D component = rigidbody2D.GetComponent<HingeJoint2D>();
			if (component)
			{
				component.autoConfigureConnectedAnchor = false;
			}
			this.rigidbody2Ds.Add(rigidbody2D);
		}
		if (!this.useChildren)
		{
			foreach (Rigidbody2D rigidbody2D2 in base.GetComponentsInChildren<Rigidbody2D>())
			{
				rigidbody2D2.WakeUp();
				HingeJoint2D component2 = rigidbody2D2.GetComponent<HingeJoint2D>();
				if (component2)
				{
					component2.autoConfigureConnectedAnchor = false;
				}
				this.rigidbody2Ds.Add(rigidbody2D2);
			}
		}
	}

	// Token: 0x06002E1A RID: 11802 RVA: 0x000CA69C File Offset: 0x000C889C
	public void OnManualSimulateFinished()
	{
		foreach (Rigidbody2D rigidbody2D in this.rigidbody2Ds)
		{
			rigidbody2D.Sleep();
			if (this.resetVelocityAfterSimulation)
			{
				rigidbody2D.linearVelocity = Vector2.zero;
				rigidbody2D.angularVelocity = 0f;
			}
		}
	}

	// Token: 0x06002E1B RID: 11803 RVA: 0x000CA70C File Offset: 0x000C890C
	private void OnCameraShaked(Vector2 cameraPosition, CameraShakeWorldForceIntensities intensity)
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		float num;
		if (intensity >= CameraShakeWorldForceIntensities.Intense)
		{
			num = 15f;
		}
		else
		{
			if (intensity < CameraShakeWorldForceIntensities.Medium)
			{
				return;
			}
			num = 5f;
		}
		num *= this.idleSwingForceMagnitude;
		float num2 = (float)(Time.timeAsDouble - this.lastFullCamShakeTime);
		if (num2 < 0.01f)
		{
			float num3 = num2 / 0.01f;
			if (num3 < 0.5f)
			{
				num3 = 0.5f;
			}
			num *= num3;
			this.lastFullCamShakeTime = Time.timeAsDouble;
		}
		Vector2 vector = new Vector2(-0.5f, 1f);
		Vector2 vector2 = new Vector2(0.5f, 1f);
		num = Mathf.Min(num, 175f);
		foreach (Rigidbody2D rigidbody2D in this.bodies)
		{
			if (rigidbody2D && (this.shakeReactionDiminishingReturn || rigidbody2D.linearVelocity.sqrMagnitude <= 625f))
			{
				Vector2 a = new Vector2(Random.Range(vector.x, vector2.x), Random.Range(vector.y, vector2.y));
				a.Normalize();
				float num4 = num;
				if (this.shakeReactionDiminishingReturn)
				{
					float magnitude = rigidbody2D.linearVelocity.magnitude;
					num4 *= Mathf.Lerp(1f, 0.25f, magnitude / 25f);
				}
				Vector2 force = a * num4;
				rigidbody2D.AddForce(force, ForceMode2D.Impulse);
			}
		}
	}

	// Token: 0x06002E1C RID: 11804 RVA: 0x000CA882 File Offset: 0x000C8A82
	public static IEnumerable<IdleForceAnimator> EnumerateActiveAnimators()
	{
		foreach (IdleForceAnimator idleForceAnimator in IdleForceAnimator._activeAnimators)
		{
			yield return idleForceAnimator;
		}
		List<IdleForceAnimator>.Enumerator enumerator = default(List<IdleForceAnimator>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x04003049 RID: 12361
	[SerializeField]
	private float idleSwingForceMagnitude;

	// Token: 0x0400304A RID: 12362
	[SerializeField]
	private float torqueMagnitude;

	// Token: 0x0400304B RID: 12363
	[SerializeField]
	private AnimationCurve idleSwingForceCurve;

	// Token: 0x0400304C RID: 12364
	[SerializeField]
	private float worldXTimeOffsetAmount;

	// Token: 0x0400304D RID: 12365
	[SerializeField]
	private bool useChildren;

	// Token: 0x0400304E RID: 12366
	[SerializeField]
	private Rigidbody2D[] bodies;

	// Token: 0x0400304F RID: 12367
	[SerializeField]
	private bool disableCamShakeEventHandling;

	// Token: 0x04003050 RID: 12368
	[SerializeField]
	private bool ignoreExistingCamEventReceiver;

	// Token: 0x04003051 RID: 12369
	[SerializeField]
	private bool resetVelocityAfterSimulation;

	// Token: 0x04003052 RID: 12370
	[SerializeField]
	private bool shakeReactionDiminishingReturn;

	// Token: 0x04003053 RID: 12371
	private bool subscribedCameraShaked;

	// Token: 0x04003054 RID: 12372
	private bool isIdleSwayActive;

	// Token: 0x04003055 RID: 12373
	private double lastFullCamShakeTime;

	// Token: 0x04003056 RID: 12374
	private float elapsedTime;

	// Token: 0x04003057 RID: 12375
	public float SpeedMultiplier = 1f;

	// Token: 0x04003058 RID: 12376
	public float ExtraHorizontalSpeed;

	// Token: 0x04003059 RID: 12377
	private ParentSwayConfig parentConfig;

	// Token: 0x0400305A RID: 12378
	private static readonly List<IdleForceAnimator> _activeAnimators = new List<IdleForceAnimator>();

	// Token: 0x0400305B RID: 12379
	private HashSet<Rigidbody2D> rigidbody2Ds = new HashSet<Rigidbody2D>();

	// Token: 0x0400305C RID: 12380
	private GameManager gm;

	// Token: 0x0400305D RID: 12381
	private VisibilityGroup visibilityGroup;

	// Token: 0x0400305E RID: 12382
	private bool isVisible;

	// Token: 0x0400305F RID: 12383
	private bool started;
}
