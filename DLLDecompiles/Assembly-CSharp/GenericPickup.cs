using System;
using System.Collections;
using TeamCherry.NestedFadeGroup;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001C1 RID: 449
public class GenericPickup : MonoBehaviour
{
	// Token: 0x14000033 RID: 51
	// (add) Token: 0x06001170 RID: 4464 RVA: 0x00051704 File Offset: 0x0004F904
	// (remove) Token: 0x06001171 RID: 4465 RVA: 0x0005173C File Offset: 0x0004F93C
	public event Func<bool> PickupAction;

	// Token: 0x170001D6 RID: 470
	// (get) Token: 0x06001172 RID: 4466 RVA: 0x00051771 File Offset: 0x0004F971
	// (set) Token: 0x06001173 RID: 4467 RVA: 0x00051778 File Offset: 0x0004F978
	public static bool IsPickupPaused { get; set; }

	// Token: 0x06001174 RID: 4468 RVA: 0x00051780 File Offset: 0x0004F980
	private void Awake()
	{
		if (this.interactEvents)
		{
			this.interactEvents.Interacted += this.DoPickup;
		}
	}

	// Token: 0x06001175 RID: 4469 RVA: 0x000517A6 File Offset: 0x0004F9A6
	private void DoPickup()
	{
		if (this.activated)
		{
			return;
		}
		base.StartCoroutine(this.Pickup());
	}

	// Token: 0x06001176 RID: 4470 RVA: 0x000517BE File Offset: 0x0004F9BE
	private bool DoPickupAction(bool breakIfAtMax)
	{
		if (this.PickupAction == null)
		{
			return false;
		}
		CollectableItemHeroReaction.DoReaction(new Vector2(0f, -0.76f), false);
		return this.PickupAction();
	}

	// Token: 0x06001177 RID: 4471 RVA: 0x000517EA File Offset: 0x0004F9EA
	private IEnumerator Pickup()
	{
		HeroController.instance.StopAnimationControl();
		tk2dSpriteAnimator animator = HeroController.instance.GetComponent<tk2dSpriteAnimator>();
		animator.Play((this.pickupAnim == GenericPickup.PickupAnimations.Normal) ? "Collect Normal 1" : "Collect Stand 1");
		yield return new WaitForSeconds(0.75f);
		bool didPickup = this.DoPickupAction(false);
		animator.Play((this.pickupAnim == GenericPickup.PickupAnimations.Normal) ? "Collect Normal 2" : "Collect Stand 2");
		if (didPickup)
		{
			this.SetActive(false, false);
			this.activated = true;
			float waitTime = 0.5f;
			while (GenericPickup.IsPickupPaused)
			{
				waitTime -= Time.deltaTime;
				yield return null;
			}
			if (waitTime > 0f)
			{
				yield return new WaitForSeconds(waitTime);
			}
		}
		yield return base.StartCoroutine(animator.PlayAnimWait((this.pickupAnim == GenericPickup.PickupAnimations.Normal) ? "Collect Normal 3" : "Collect Stand 3", null));
		HeroController.instance.StartAnimationControl();
		if (this.interactEvents)
		{
			this.interactEvents.EndInteraction();
			if (didPickup)
			{
				this.interactEvents.Deactivate(false);
			}
		}
		yield break;
	}

	// Token: 0x06001178 RID: 4472 RVA: 0x000517FC File Offset: 0x0004F9FC
	public void SetActive(bool value, bool isInstant = false)
	{
		if (this.activated)
		{
			return;
		}
		if (value)
		{
			this.interactEvents.Activate();
			this.OnBecameActive.Invoke();
		}
		else
		{
			this.interactEvents.Deactivate(false);
			this.OnBecameInactive.Invoke();
		}
		if (this.group)
		{
			this.group.FadeTo(value ? 1f : 0f, isInstant ? 0f : this.fadeTime, null, false, null);
		}
	}

	// Token: 0x04001061 RID: 4193
	[SerializeField]
	private InteractEvents interactEvents;

	// Token: 0x04001062 RID: 4194
	[SerializeField]
	private NestedFadeGroupBase group;

	// Token: 0x04001063 RID: 4195
	[SerializeField]
	private float fadeTime;

	// Token: 0x04001064 RID: 4196
	[Space]
	[SerializeField]
	[ModifiableProperty]
	[Conditional("interactEvents", true, false, false)]
	private GenericPickup.PickupAnimations pickupAnim;

	// Token: 0x04001065 RID: 4197
	[Space]
	public UnityEvent OnBecameActive;

	// Token: 0x04001066 RID: 4198
	public UnityEvent OnBecameInactive;

	// Token: 0x04001067 RID: 4199
	private bool activated;

	// Token: 0x02001502 RID: 5378
	private enum PickupAnimations
	{
		// Token: 0x04008585 RID: 34181
		Normal,
		// Token: 0x04008586 RID: 34182
		Stand
	}
}
