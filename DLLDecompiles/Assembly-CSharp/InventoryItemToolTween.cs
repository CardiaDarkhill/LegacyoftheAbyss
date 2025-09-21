using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020006A9 RID: 1705
public class InventoryItemToolTween : MonoBehaviour
{
	// Token: 0x06003D32 RID: 15666 RVA: 0x0010D46F File Offset: 0x0010B66F
	private void Awake()
	{
		this.manager = base.GetComponentInParent<InventoryItemToolManager>();
	}

	// Token: 0x06003D33 RID: 15667 RVA: 0x0010D480 File Offset: 0x0010B680
	public void DoPlace(Vector2 fromPosition, Vector2 toPosition, ToolItem tool, Action onTweenEnd)
	{
		if (tool.Type == ToolItemType.Skill)
		{
			this.placeSkillSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		}
		else
		{
			this.placeSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		}
		this.DoMove(this.MoveRoutine(fromPosition, toPosition, (tool.Type == ToolItemType.Skill) ? "Place Skill" : "Place"), tool, onTweenEnd, this.placeShake);
	}

	// Token: 0x06003D34 RID: 15668 RVA: 0x0010D500 File Offset: 0x0010B700
	public void DoReturn(Vector2 fromPosition, Vector2 toPosition, ToolItem tool, Action onTweenEnd)
	{
		if (tool.Type == ToolItemType.Skill)
		{
			this.returnSkillSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		}
		else
		{
			this.returnSound.SpawnAndPlayOneShot(this.audioSourcePrefab, base.transform.position, null);
		}
		this.returnShake.DoShake(this, true);
		this.DoMove(this.MoveRoutine(fromPosition, toPosition, null), tool, onTweenEnd, null);
	}

	// Token: 0x06003D35 RID: 15669 RVA: 0x0010D574 File Offset: 0x0010B774
	private void DoMove(IEnumerator routine, ToolItem tool, Action onTweenEnd, CameraShakeTarget endShake)
	{
		base.gameObject.SetActive(true);
		Color toolTypeColor = this.manager.GetToolTypeColor(tool.Type);
		if (this.spriteRenderer)
		{
			this.spriteRenderer.enabled = true;
			this.spriteRenderer.sprite = tool.InventorySpriteBase;
		}
		if (this.trailParticles)
		{
			this.trailParticles.main.startColor = toolTypeColor;
			this.trailParticles.Play(true);
		}
		SpriteRenderer[] array = this.toolColoured;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = toolTypeColor;
		}
		this.manager.IsActionsBlocked = true;
		this.CancelRoutines();
		this.queuedTweenEnd = delegate()
		{
			CameraShakeTarget endShake2 = endShake;
			if (endShake2 != null)
			{
				endShake2.DoShake(this, true);
			}
			this.manager.IsActionsBlocked = false;
			onTweenEnd();
		};
		this.moveRoutine = base.StartCoroutine(routine);
	}

	// Token: 0x06003D36 RID: 15670 RVA: 0x0010D66B File Offset: 0x0010B86B
	private void CancelRoutines()
	{
		if (this.moveRoutine != null)
		{
			base.StopCoroutine(this.moveRoutine);
		}
		if (this.queuedTweenEnd != null)
		{
			this.queuedTweenEnd();
			this.queuedTweenEnd = null;
		}
	}

	// Token: 0x06003D37 RID: 15671 RVA: 0x0010D69B File Offset: 0x0010B89B
	public void Cancel()
	{
		this.CancelRoutines();
		base.gameObject.SetActive(false);
	}

	// Token: 0x06003D38 RID: 15672 RVA: 0x0010D6AF File Offset: 0x0010B8AF
	private IEnumerator MoveRoutine(Vector2 fromPosition, Vector2 toPosition, string endAnimationState)
	{
		InventoryItemToolTween.<>c__DisplayClass22_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.fromPosition = fromPosition;
		CS$<>8__locals1.toPosition = toPosition;
		if (this.animator)
		{
			this.animator.Play("Idle");
		}
		for (float elapsed = 0f; elapsed < this.moveDuration; elapsed += Time.unscaledDeltaTime)
		{
			this.<MoveRoutine>g__SetPosition|22_0(elapsed / this.moveDuration, ref CS$<>8__locals1);
			yield return null;
		}
		this.<MoveRoutine>g__SetPosition|22_0(1f, ref CS$<>8__locals1);
		if (this.queuedTweenEnd != null)
		{
			this.queuedTweenEnd();
			this.queuedTweenEnd = null;
		}
		if (this.animator && !string.IsNullOrEmpty(endAnimationState))
		{
			this.animator.Play(endAnimationState);
			yield return null;
			yield return new WaitForSeconds(this.animator.GetCurrentAnimatorStateInfo(0).length);
		}
		if (this.spriteRenderer)
		{
			this.spriteRenderer.enabled = false;
		}
		if (this.trailParticles)
		{
			this.trailParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
			while (this.trailParticles.IsAlive(true))
			{
				yield return null;
			}
		}
		this.moveRoutine = null;
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06003D3A RID: 15674 RVA: 0x0010D705 File Offset: 0x0010B905
	[CompilerGenerated]
	private void <MoveRoutine>g__SetPosition|22_0(float time, ref InventoryItemToolTween.<>c__DisplayClass22_0 A_2)
	{
		base.transform.SetPosition2D(Vector2.Lerp(A_2.fromPosition, A_2.toPosition, this.moveCurve.Evaluate(time)));
	}

	// Token: 0x04003EE7 RID: 16103
	[SerializeField]
	private SpriteRenderer spriteRenderer;

	// Token: 0x04003EE8 RID: 16104
	[SerializeField]
	private Animator animator;

	// Token: 0x04003EE9 RID: 16105
	[SerializeField]
	private ParticleSystem trailParticles;

	// Token: 0x04003EEA RID: 16106
	[SerializeField]
	private SpriteRenderer[] toolColoured;

	// Token: 0x04003EEB RID: 16107
	[Space]
	[SerializeField]
	private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	// Token: 0x04003EEC RID: 16108
	[SerializeField]
	private float moveDuration = 0.1f;

	// Token: 0x04003EED RID: 16109
	[SerializeField]
	private CameraShakeTarget placeShake;

	// Token: 0x04003EEE RID: 16110
	[SerializeField]
	private CameraShakeTarget returnShake;

	// Token: 0x04003EEF RID: 16111
	[Space]
	[SerializeField]
	private AudioSource audioSourcePrefab;

	// Token: 0x04003EF0 RID: 16112
	[SerializeField]
	private AudioEvent placeSound;

	// Token: 0x04003EF1 RID: 16113
	[SerializeField]
	private AudioEvent returnSound;

	// Token: 0x04003EF2 RID: 16114
	[SerializeField]
	private AudioEvent placeSkillSound;

	// Token: 0x04003EF3 RID: 16115
	[SerializeField]
	private AudioEvent returnSkillSound;

	// Token: 0x04003EF4 RID: 16116
	private Coroutine moveRoutine;

	// Token: 0x04003EF5 RID: 16117
	private Action queuedTweenEnd;

	// Token: 0x04003EF6 RID: 16118
	private InventoryItemToolManager manager;
}
