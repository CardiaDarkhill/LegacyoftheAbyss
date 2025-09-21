using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000572 RID: 1394
public class ThreadSpinner : MonoBehaviour, IHitResponder
{
	// Token: 0x060031DF RID: 12767 RVA: 0x000DD710 File Offset: 0x000DB910
	private void Awake()
	{
		if (this.threadSpool)
		{
			this.initialXScale = this.threadSpool.transform.localScale.x;
		}
		if (this.persistent)
		{
			this.persistent.OnGetSaveState += delegate(out int value)
			{
				value = this.hits;
			};
			this.persistent.OnSetSaveState += delegate(int value)
			{
				this.hits = value;
				if (!this.threadSpool)
				{
					return;
				}
				if (this.hits <= this.hitStageScales.Length && this.hitStageScales.Length != 0)
				{
					if (this.hits > 0)
					{
						ThreadSpinner.ScaleAppearance scaleAppearance = this.hitStageScales[this.hits - 1];
						this.threadSpool.transform.SetScaleX(scaleAppearance.Scale);
					}
					this.threadSpool.gameObject.SetActive(true);
					return;
				}
				if (this.hits <= 0)
				{
					this.threadSpool.transform.SetScaleX(this.initialXScale);
					this.threadSpool.gameObject.SetActive(true);
					return;
				}
				this.threadSpool.gameObject.SetActive(false);
				this.hits = this.hitStageScales.Length + 1;
			};
			ResetDynamicHierarchy resetter = base.gameObject.AddComponent<ResetDynamicHierarchy>();
			this.persistent.SemiPersistentReset += delegate()
			{
				this.hits = 0;
				resetter.DoReset(true);
				if (this.threadSpool)
				{
					this.threadSpool.gameObject.SetActive(true);
					this.threadSpool.transform.SetScaleX(this.initialXScale);
					this.threadSpool.Play(this.idleAnim);
				}
			};
		}
	}

	// Token: 0x060031E0 RID: 12768 RVA: 0x000DD7B8 File Offset: 0x000DB9B8
	public IHitResponder.HitResponse Hit(HitInstance damageInstance)
	{
		if (!damageInstance.IsNailDamage)
		{
			return IHitResponder.Response.None;
		}
		bool flag = false;
		bool flag2 = false;
		if (this.hits > this.hitStageScales.Length)
		{
			if (this.hits == this.hitStageScales.Length + 1)
			{
				flag = true;
			}
			else
			{
				if (this.hits != this.hitStageScales.Length + 2)
				{
					return IHitResponder.Response.None;
				}
				flag2 = true;
			}
		}
		if (this.animRoutine != null)
		{
			base.StopCoroutine(this.animRoutine);
		}
		bool flag3 = false;
		bool flag4 = false;
		Vector3 position = this.threadSpawn ? this.threadSpawn.position : base.transform.position;
		HitInstance.HitDirection hitDirection = damageInstance.GetHitDirection(HitInstance.TargetType.Regular);
		bool flag5 = hitDirection == HitInstance.HitDirection.Left || (hitDirection != HitInstance.HitDirection.Right && Random.Range(0, 2) == 0);
		if (this.hits < this.hitStageScales.Length)
		{
			flag3 = true;
			flag4 = true;
			if (this.threadSpool)
			{
				ThreadSpinner.ScaleAppearance scaleAppearance = this.hitStageScales[this.hits];
				Action onTimerEnd;
				if (!string.IsNullOrEmpty(scaleAppearance.Animation))
				{
					this.threadSpool.Play(scaleAppearance.Animation);
					onTimerEnd = null;
				}
				else
				{
					this.threadSpool.Play(this.spinAnim);
					onTimerEnd = delegate()
					{
						this.threadSpool.Play(this.idleAnim);
					};
				}
				this.startLerp = this.threadSpool.transform.localScale.x;
				this.endLerp = scaleAppearance.Scale;
				this.animRoutine = this.StartTimerRoutine(0f, this.spinDownTime, delegate(float time)
				{
					this.threadSpool.transform.SetScaleX(Mathf.LerpUnclamped(this.startLerp, this.endLerp, time));
				}, null, onTimerEnd, false);
			}
		}
		else if (this.hits == this.hitStageScales.Length)
		{
			flag3 = true;
			flag4 = true;
			if (this.threadSpool)
			{
				this.threadSpool.transform.SetScaleX(1f);
				this.threadSpool.Play(this.disperseAnim);
			}
		}
		else if (flag)
		{
			flag3 = true;
			this.dislodgeSound.SpawnAndPlayOneShot(position, null);
			this.OnDislodge.Invoke();
			Vector3 localEulerAngles = base.transform.localEulerAngles;
			this.startLerp = localEulerAngles.z;
			this.endLerp = localEulerAngles.z + (flag5 ? (-this.lastHitRotation) : this.lastHitRotation);
			this.animRoutine = this.StartTimerRoutine(0f, this.lastHitDuration, delegate(float time)
			{
				time = this.lastHitCurve.Evaluate(time);
				base.transform.SetLocalRotation2D(Mathf.LerpUnclamped(this.startLerp, this.endLerp, time));
			}, null, null, false);
		}
		else if (flag2)
		{
			flag3 = true;
			this.flingSound.SpawnAndPlayOneShot(position, null);
			Collider2D component = base.GetComponent<Collider2D>();
			if (component)
			{
				component.enabled = false;
			}
			FlingUtils.ChildrenConfig childrenConfig = this.flingConfig;
			if (flag5)
			{
				childrenConfig.AngleMin = Helper.GetReflectedAngle(childrenConfig.AngleMin, true, false, false);
				childrenConfig.AngleMax = Helper.GetReflectedAngle(childrenConfig.AngleMax, true, false, false);
			}
			if (childrenConfig.Parent)
			{
				childrenConfig.Parent.transform.SetParent(null, true);
			}
			FlingUtils.FlingChildren(childrenConfig, childrenConfig.Parent ? childrenConfig.Parent.transform : null, Vector3.zero, null);
			this.OnFling.Invoke();
			base.transform.SetPosition2D(-2000f, -2000f);
		}
		if (flag3)
		{
			this.hits++;
			if (flag4)
			{
				if (this.threadPrefab)
				{
					int num = this.amountPerHit;
					for (int i = 0; i < num; i++)
					{
						this.threadPrefab.Spawn(position);
					}
				}
				this.spawnEffectPrefabs.SpawnAll(position);
				this.hitSound.SpawnAndPlayOneShot(position, null);
				base.StartCoroutine(this.AddSilkDelayed());
				if (this.HitEvent != null)
				{
					this.HitEvent.Invoke();
				}
				if (this.HitEventDirectional != null)
				{
					this.HitEventDirectional.Invoke(flag5);
				}
			}
			else
			{
				this.inertHitEffectPrefabs.SpawnAll(position);
			}
		}
		return flag3 ? IHitResponder.Response.GenericHit : IHitResponder.Response.None;
	}

	// Token: 0x060031E1 RID: 12769 RVA: 0x000DDBC7 File Offset: 0x000DBDC7
	private IEnumerator AddSilkDelayed()
	{
		yield return new WaitForSeconds(this.addSilkDelay);
		HeroController.instance.AddSilk(this.addSilkPerHit, true);
		yield break;
	}

	// Token: 0x04003557 RID: 13655
	private readonly int idleAnim = Animator.StringToHash("Idle");

	// Token: 0x04003558 RID: 13656
	private readonly int spinAnim = Animator.StringToHash("Spin");

	// Token: 0x04003559 RID: 13657
	private readonly int disperseAnim = Animator.StringToHash("Disperse");

	// Token: 0x0400355A RID: 13658
	[SerializeField]
	private PersistentIntItem persistent;

	// Token: 0x0400355B RID: 13659
	[SerializeField]
	private Animator threadSpool;

	// Token: 0x0400355C RID: 13660
	[SerializeField]
	private GameObject threadPrefab;

	// Token: 0x0400355D RID: 13661
	[SerializeField]
	private Transform threadSpawn;

	// Token: 0x0400355E RID: 13662
	[SerializeField]
	private int amountPerHit;

	// Token: 0x0400355F RID: 13663
	[SerializeField]
	private float addSilkDelay;

	// Token: 0x04003560 RID: 13664
	[SerializeField]
	private int addSilkPerHit;

	// Token: 0x04003561 RID: 13665
	[SerializeField]
	private ThreadSpinner.ScaleAppearance[] hitStageScales;

	// Token: 0x04003562 RID: 13666
	[SerializeField]
	private float spinDownTime;

	// Token: 0x04003563 RID: 13667
	[SerializeField]
	private GameObject[] spawnEffectPrefabs;

	// Token: 0x04003564 RID: 13668
	[SerializeField]
	private AudioEventRandom hitSound;

	// Token: 0x04003565 RID: 13669
	[SerializeField]
	private AudioEventRandom dislodgeSound;

	// Token: 0x04003566 RID: 13670
	[SerializeField]
	private AudioEventRandom flingSound;

	// Token: 0x04003567 RID: 13671
	[Space]
	public ThreadSpinner.UnityBoolEvent HitEventDirectional;

	// Token: 0x04003568 RID: 13672
	public UnityEvent HitEvent;

	// Token: 0x04003569 RID: 13673
	[Space]
	[SerializeField]
	private float lastHitRotation;

	// Token: 0x0400356A RID: 13674
	[SerializeField]
	private AnimationCurve lastHitCurve;

	// Token: 0x0400356B RID: 13675
	[SerializeField]
	private float lastHitDuration;

	// Token: 0x0400356C RID: 13676
	[SerializeField]
	private GameObject[] inertHitEffectPrefabs;

	// Token: 0x0400356D RID: 13677
	[SerializeField]
	private FlingUtils.ChildrenConfig flingConfig;

	// Token: 0x0400356E RID: 13678
	[Space]
	public UnityEvent OnDislodge;

	// Token: 0x0400356F RID: 13679
	public UnityEvent OnFling;

	// Token: 0x04003570 RID: 13680
	private Coroutine animRoutine;

	// Token: 0x04003571 RID: 13681
	private float startLerp;

	// Token: 0x04003572 RID: 13682
	private float endLerp;

	// Token: 0x04003573 RID: 13683
	private int hits;

	// Token: 0x04003574 RID: 13684
	private float initialXScale;

	// Token: 0x02001876 RID: 6262
	[Serializable]
	private struct ScaleAppearance
	{
		// Token: 0x04009225 RID: 37413
		public float Scale;

		// Token: 0x04009226 RID: 37414
		public string Animation;
	}

	// Token: 0x02001877 RID: 6263
	[Serializable]
	public class UnityBoolEvent : UnityEvent<bool>
	{
	}
}
