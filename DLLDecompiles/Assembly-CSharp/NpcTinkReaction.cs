using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000099 RID: 153
public class NpcTinkReaction : MonoBehaviour
{
	// Token: 0x060004C2 RID: 1218 RVA: 0x0001969C File Offset: 0x0001789C
	private void Reset()
	{
		this.npcControl = base.GetComponent<NPCControlBase>();
	}

	// Token: 0x060004C3 RID: 1219 RVA: 0x000196AA File Offset: 0x000178AA
	private void Awake()
	{
		if (!this.npcControl)
		{
			this.npcControl = base.GetComponent<NPCControlBase>();
		}
	}

	// Token: 0x060004C4 RID: 1220 RVA: 0x000196C5 File Offset: 0x000178C5
	private void OnEnable()
	{
		this.tinkEffect.HitInDirection += this.OnHitInDirection;
	}

	// Token: 0x060004C5 RID: 1221 RVA: 0x000196DE File Offset: 0x000178DE
	private void OnDisable()
	{
		if (this.sendEndEventWhenInterrupted && this.animateRoutine != null)
		{
			this.OnBounceAnimEnd.Invoke();
		}
		this.StopBehaviour();
		this.tinkEffect.HitInDirection -= this.OnHitInDirection;
	}

	// Token: 0x060004C6 RID: 1222 RVA: 0x00019718 File Offset: 0x00017918
	private void OnHitInDirection(GameObject source, HitInstance.HitDirection direction)
	{
		this.StopBehaviour();
		this.animateRoutine = base.StartCoroutine(this.Animate());
	}

	// Token: 0x060004C7 RID: 1223 RVA: 0x00019732 File Offset: 0x00017932
	private void StopBehaviour()
	{
		if (this.animateRoutine != null)
		{
			base.StopCoroutine(this.animateRoutine);
			this.animateRoutine = null;
		}
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x0001974F File Offset: 0x0001794F
	private IEnumerator Animate()
	{
		if (this.lookAnimNPC)
		{
			this.lookAnimNPC.Deactivate();
		}
		if (this.npcControl)
		{
			this.npcControl.Deactivate(false);
		}
		this.OnBounced.Invoke();
		if (this.animator)
		{
			if (this.conditionalBounceAnim != null && this.conditionalBounceAnim.CanPlayAnimation())
			{
				yield return this.conditionalBounceAnim.PlayAndWait();
			}
			else if (!string.IsNullOrEmpty(this.bounceAnim))
			{
				yield return base.StartCoroutine(this.animator.PlayAnimWait(this.bounceAnim, null));
			}
			if (!string.IsNullOrEmpty(this.returnToIdleAnim))
			{
				yield return base.StartCoroutine(this.animator.PlayAnimWait(this.returnToIdleAnim, null));
			}
		}
		if (this.lookAnimNPC)
		{
			switch (this.lookAnimNPCActivate)
			{
			case NpcTinkReaction.LookAnimNpcActivateBehaviours.Any:
				this.lookAnimNPC.Activate();
				break;
			case NpcTinkReaction.LookAnimNpcActivateBehaviours.FaceLeft:
				this.lookAnimNPC.Activate(true);
				break;
			case NpcTinkReaction.LookAnimNpcActivateBehaviours.FaceRight:
				this.lookAnimNPC.Activate(false);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		this.OnBounceAnimEnd.Invoke();
		yield return new WaitForSeconds(0.5f);
		if (this.npcControl)
		{
			this.npcControl.Activate();
		}
		this.animateRoutine = null;
		yield break;
	}

	// Token: 0x04000491 RID: 1169
	[SerializeField]
	private TinkEffect tinkEffect;

	// Token: 0x04000492 RID: 1170
	[Space]
	[SerializeField]
	private NPCControlBase npcControl;

	// Token: 0x04000493 RID: 1171
	[SerializeField]
	private tk2dSpriteAnimator animator;

	// Token: 0x04000494 RID: 1172
	[SerializeField]
	private LookAnimNPC lookAnimNPC;

	// Token: 0x04000495 RID: 1173
	[SerializeField]
	[ModifiableProperty]
	[Conditional("lookAnimNPC", true, false, false)]
	private NpcTinkReaction.LookAnimNpcActivateBehaviours lookAnimNPCActivate;

	// Token: 0x04000496 RID: 1174
	[Space]
	[SerializeField]
	private string bounceAnim;

	// Token: 0x04000497 RID: 1175
	[SerializeField]
	private string returnToIdleAnim;

	// Token: 0x04000498 RID: 1176
	[SerializeField]
	private ConditionalAnimation conditionalBounceAnim;

	// Token: 0x04000499 RID: 1177
	[Space]
	public UnityEvent OnBounced;

	// Token: 0x0400049A RID: 1178
	public UnityEvent OnBounceAnimEnd;

	// Token: 0x0400049B RID: 1179
	[Space]
	[SerializeField]
	private bool sendEndEventWhenInterrupted;

	// Token: 0x0400049C RID: 1180
	private Coroutine animateRoutine;

	// Token: 0x0200140C RID: 5132
	private enum LookAnimNpcActivateBehaviours
	{
		// Token: 0x040081B3 RID: 33203
		Any,
		// Token: 0x040081B4 RID: 33204
		FaceLeft,
		// Token: 0x040081B5 RID: 33205
		FaceRight
	}
}
