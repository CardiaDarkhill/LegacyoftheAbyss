using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004DB RID: 1243
public class ExtenderPlatLink : MonoBehaviour
{
	// Token: 0x1700051D RID: 1309
	// (get) Token: 0x06002CAC RID: 11436 RVA: 0x000C36DB File Offset: 0x000C18DB
	public bool IsPlatformActive
	{
		get
		{
			return this.platform && this.platform.gameObject.activeSelf;
		}
	}

	// Token: 0x06002CAD RID: 11437 RVA: 0x000C36FC File Offset: 0x000C18FC
	private void Awake()
	{
		if (this.platform != null)
		{
			this.activePlatZ = this.platform.transform.localPosition.z;
		}
		this.tiltPlat = ExtenderPlatLink.GetFirstTiltPlat(base.transform);
	}

	// Token: 0x06002CAE RID: 11438 RVA: 0x000C3738 File Offset: 0x000C1938
	private static TiltPlat GetFirstTiltPlat(Transform transform)
	{
		TiltPlat component = transform.GetComponent<TiltPlat>();
		if (component)
		{
			return component;
		}
		foreach (object obj in transform)
		{
			Transform transform2 = (Transform)obj;
			if (!transform2.GetComponent<ExtenderPlatLink>())
			{
				TiltPlat firstTiltPlat = ExtenderPlatLink.GetFirstTiltPlat(transform2);
				if (firstTiltPlat)
				{
					return firstTiltPlat;
				}
			}
		}
		return null;
	}

	// Token: 0x06002CAF RID: 11439 RVA: 0x000C37C0 File Offset: 0x000C19C0
	public void LinkRotationStarted()
	{
		this.linkUnfoldSound.SpawnAndPlayOneShot(base.transform.position, null);
	}

	// Token: 0x06002CB0 RID: 11440 RVA: 0x000C37DC File Offset: 0x000C19DC
	public void UpdateLinkRotation(float time)
	{
		time = Mathf.Clamp01(time);
		time = this.linkExtendCurve.Evaluate(time);
		float num = Mathf.LerpUnclamped(this.initialLinkRotation, this.extendedLinkRotation, time);
		if (this.platform.localScale.x < 0f)
		{
			num *= -1f;
		}
		base.transform.SetLocalRotation2D(num);
	}

	// Token: 0x06002CB1 RID: 11441 RVA: 0x000C383D File Offset: 0x000C1A3D
	public void PlatRotationStarted()
	{
		this.platUnfoldSound.SpawnAndPlayOneShot(base.transform.position, null);
	}

	// Token: 0x06002CB2 RID: 11442 RVA: 0x000C3858 File Offset: 0x000C1A58
	public void UpdatePlatRotation(float time)
	{
		time = Mathf.Clamp01(time);
		time = this.platExtendCurve.Evaluate(time);
		float num = Mathf.LerpUnclamped(this.initialPlatRotation, this.extendedPlatRotation, time);
		if (this.platform.localScale.x < 0f)
		{
			num *= -1f;
		}
		this.platform.SetLocalRotation2D(num);
	}

	// Token: 0x06002CB3 RID: 11443 RVA: 0x000C38BC File Offset: 0x000C1ABC
	public void SetActive(bool value, bool isInstant)
	{
		if (this.platform != null)
		{
			this.platform.SetLocalPositionZ(value ? this.activePlatZ : this.inactivePlatZ);
		}
		this.OnSetActive.Invoke(value);
		if (value)
		{
			this.OnActivate.Invoke();
			if (this.tiltPlat)
			{
				this.tiltPlat.ActivateTiltPlat(isInstant);
			}
		}
		else
		{
			this.OnDeactive.Invoke();
		}
		if (isInstant)
		{
			return;
		}
		this.platActivateCamShake.DoShake(this, true);
	}

	// Token: 0x04002E42 RID: 11842
	[SerializeField]
	private float initialLinkRotation;

	// Token: 0x04002E43 RID: 11843
	[SerializeField]
	private float extendedLinkRotation;

	// Token: 0x04002E44 RID: 11844
	[SerializeField]
	private AnimationCurve linkExtendCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002E45 RID: 11845
	[SerializeField]
	private AudioEventRandom linkUnfoldSound;

	// Token: 0x04002E46 RID: 11846
	[SerializeField]
	private Transform platform;

	// Token: 0x04002E47 RID: 11847
	[SerializeField]
	private float initialPlatRotation;

	// Token: 0x04002E48 RID: 11848
	[SerializeField]
	private float extendedPlatRotation;

	// Token: 0x04002E49 RID: 11849
	[SerializeField]
	private AnimationCurve platExtendCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x04002E4A RID: 11850
	[SerializeField]
	private AudioEventRandom platUnfoldSound;

	// Token: 0x04002E4B RID: 11851
	[SerializeField]
	private float inactivePlatZ;

	// Token: 0x04002E4C RID: 11852
	[SerializeField]
	private CameraShakeTarget platActivateCamShake;

	// Token: 0x04002E4D RID: 11853
	[Space]
	public ExtenderPlatLink.UnityBoolEvent OnSetActive;

	// Token: 0x04002E4E RID: 11854
	public UnityEvent OnActivate;

	// Token: 0x04002E4F RID: 11855
	public UnityEvent OnDeactive;

	// Token: 0x04002E50 RID: 11856
	private float activePlatZ;

	// Token: 0x04002E51 RID: 11857
	private TiltPlat tiltPlat;

	// Token: 0x020017E3 RID: 6115
	[Serializable]
	public class UnityBoolEvent : UnityEvent<bool>
	{
	}
}
