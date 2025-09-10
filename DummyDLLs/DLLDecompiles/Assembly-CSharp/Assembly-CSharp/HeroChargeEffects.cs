using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200018B RID: 395
public class HeroChargeEffects : ManagerSingleton<HeroChargeEffects>
{
	// Token: 0x1400001D RID: 29
	// (add) Token: 0x06000CE7 RID: 3303 RVA: 0x00039A20 File Offset: 0x00037C20
	// (remove) Token: 0x06000CE8 RID: 3304 RVA: 0x00039A58 File Offset: 0x00037C58
	public event Action ChargeBurst;

	// Token: 0x1400001E RID: 30
	// (add) Token: 0x06000CE9 RID: 3305 RVA: 0x00039A90 File Offset: 0x00037C90
	// (remove) Token: 0x06000CEA RID: 3306 RVA: 0x00039AC8 File Offset: 0x00037CC8
	public event Action ChargeEnd;

	// Token: 0x17000158 RID: 344
	// (get) Token: 0x06000CEB RID: 3307 RVA: 0x00039AFD File Offset: 0x00037CFD
	// (set) Token: 0x06000CEC RID: 3308 RVA: 0x00039B05 File Offset: 0x00037D05
	public bool IsCharging { get; private set; }

	// Token: 0x06000CED RID: 3309 RVA: 0x00039B0E File Offset: 0x00037D0E
	protected override void Awake()
	{
		base.Awake();
		this.animator = base.GetComponent<tk2dSpriteAnimator>();
		this.spriteFlash = base.GetComponent<SpriteFlash>();
		if (this.bindSilk)
		{
			this.bindSilkRenderer = this.bindSilk.GetComponent<MeshRenderer>();
		}
	}

	// Token: 0x06000CEE RID: 3310 RVA: 0x00039B4C File Offset: 0x00037D4C
	private void OnDisable()
	{
		if (this.blockedInput)
		{
			HeroController instance = HeroController.instance;
			this.blockedInput = false;
			instance.RemoveInputBlocker(this);
		}
	}

	// Token: 0x06000CEF RID: 3311 RVA: 0x00039B68 File Offset: 0x00037D68
	public void StartCharge(Color tintColor)
	{
		HeroController instance = HeroController.instance;
		bool atBench = PlayerData.instance.atBench;
		this.wasAtBench = atBench;
		this.didChargeBurst = false;
		this.didChargeEnd = false;
		this.IsCharging = true;
		if (atBench)
		{
			this.chargeAnim = "Charge Up Bench";
			if (!this.useItem || !this.useItem.SkipBenchUseEffect)
			{
				if (this.bindSilkRenderer)
				{
					this.bindSilkRenderer.enabled = true;
				}
				if (this.bindSilk)
				{
					this.bindSilk.PlayFromFrame("Charge Up Bench Silk", 0);
				}
			}
		}
		else
		{
			this.chargeAnim = (instance.cState.onGround ? "Charge Up" : "Charge Up Air");
		}
		this.takeAnimationControl = false;
		if (!atBench)
		{
			if (instance.controlReqlinquished && !InteractManager.BlockingInteractable)
			{
				this.DoChargeBurst();
				this.DoChargeEnd();
				return;
			}
			if (instance.cState.onGround)
			{
				this.blockedInput = true;
				instance.AddInputBlocker(this);
			}
			instance.StopAnimationControl();
			this.takeAnimationControl = true;
		}
		this.animator.PlayFromFrame(this.chargeAnim, 0);
		tk2dSpriteAnimator tk2dSpriteAnimator = this.animator;
		tk2dSpriteAnimator.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Combine(tk2dSpriteAnimator.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.OnAnimationEventTriggered));
		tk2dSpriteAnimator tk2dSpriteAnimator2 = this.animator;
		tk2dSpriteAnimator2.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(tk2dSpriteAnimator2.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted));
		this.animator.AnimationChanged += this.OnAnimationChanged;
	}

	// Token: 0x06000CF0 RID: 3312 RVA: 0x00039CF2 File Offset: 0x00037EF2
	private void OnAnimationEventTriggered(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip, int frame)
	{
		this.DoChargeBurst();
		anim.AnimationEventTriggered = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>)Delegate.Remove(anim.AnimationEventTriggered, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.OnAnimationEventTriggered));
	}

	// Token: 0x06000CF1 RID: 3313 RVA: 0x00039D1C File Offset: 0x00037F1C
	private void DoChargeBurst()
	{
		this.didChargeBurst = true;
		this.IsCharging = false;
		this.burstCameraShake.DoShake(this, true);
		if (this.ChargeBurst != null)
		{
			this.ChargeBurst();
		}
		if (this.useItem)
		{
			this.useItem.ConsumeItemResponse();
			this.useItem = null;
		}
		if ((this.wasAtBench || HeroController.instance.controlReqlinquished) && this.spriteFlash)
		{
			this.spriteFlash.flashFocusHeal();
		}
	}

	// Token: 0x06000CF2 RID: 3314 RVA: 0x00039DA4 File Offset: 0x00037FA4
	private void OnAnimationCompleted(tk2dSpriteAnimator anim, tk2dSpriteAnimationClip clip)
	{
		if (this.didChargeEnd)
		{
			return;
		}
		this.didChargeEnd = true;
		if (!this.didChargeBurst)
		{
			this.DoChargeBurst();
		}
		this.DoChargeEnd();
		anim.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Remove(anim.AnimationCompleted, new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.OnAnimationCompleted));
		anim.AnimationChanged -= this.OnAnimationChanged;
	}

	// Token: 0x06000CF3 RID: 3315 RVA: 0x00039E09 File Offset: 0x00038009
	private void OnAnimationChanged(tk2dSpriteAnimator tk2dSpriteAnimator, tk2dSpriteAnimationClip previousclip, tk2dSpriteAnimationClip newclip)
	{
		this.OnAnimationCompleted(tk2dSpriteAnimator, previousclip);
		tk2dSpriteAnimator.AnimationChanged -= this.OnAnimationChanged;
	}

	// Token: 0x06000CF4 RID: 3316 RVA: 0x00039E28 File Offset: 0x00038028
	private void DoChargeEnd()
	{
		if (this.wasAtBench)
		{
			EventRegister.SendEvent(EventRegisterEvents.BenchRegainControl, null);
		}
		else if (this.takeAnimationControl)
		{
			HeroController.instance.StartAnimationControlToIdle();
		}
		if (this.blockedInput)
		{
			HeroController instance = HeroController.instance;
			this.blockedInput = false;
			instance.RemoveInputBlocker(this);
		}
		if (this.ChargeEnd != null)
		{
			this.ChargeEnd();
		}
	}

	// Token: 0x06000CF5 RID: 3317 RVA: 0x00039E89 File Offset: 0x00038089
	public void DoUseBenchItem(CollectableItem item)
	{
		this.useItem = item;
		EventRegister.SendEvent(EventRegisterEvents.BenchRelinquishControl, null);
		base.StartCoroutine(this.StartChargeDelayed(this.benchUseDelay, Color.white));
	}

	// Token: 0x06000CF6 RID: 3318 RVA: 0x00039EB5 File Offset: 0x000380B5
	private IEnumerator StartChargeDelayed(float delay, Color chargeColor)
	{
		yield return new WaitForSeconds(delay);
		this.StartCharge(chargeColor);
		yield break;
	}

	// Token: 0x04000C6B RID: 3179
	[SerializeField]
	private CameraShakeTarget burstCameraShake;

	// Token: 0x04000C6C RID: 3180
	[Space]
	[SerializeField]
	private float benchUseDelay;

	// Token: 0x04000C6D RID: 3181
	[SerializeField]
	private tk2dSpriteAnimator bindSilk;

	// Token: 0x04000C6E RID: 3182
	private string chargeAnim;

	// Token: 0x04000C6F RID: 3183
	private CollectableItem useItem;

	// Token: 0x04000C70 RID: 3184
	private bool wasAtBench;

	// Token: 0x04000C71 RID: 3185
	private bool takeAnimationControl;

	// Token: 0x04000C72 RID: 3186
	private bool didChargeBurst;

	// Token: 0x04000C73 RID: 3187
	private bool didChargeEnd;

	// Token: 0x04000C74 RID: 3188
	private bool blockedInput;

	// Token: 0x04000C75 RID: 3189
	private MeshRenderer bindSilkRenderer;

	// Token: 0x04000C76 RID: 3190
	private tk2dSpriteAnimator animator;

	// Token: 0x04000C77 RID: 3191
	private SpriteFlash spriteFlash;
}
