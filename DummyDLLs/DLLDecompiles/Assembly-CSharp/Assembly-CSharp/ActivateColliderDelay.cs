using System;
using TeamCherry.SharedUtils;
using UnityEngine;

// Token: 0x0200004C RID: 76
public class ActivateColliderDelay : MonoBehaviour
{
	// Token: 0x06000218 RID: 536 RVA: 0x0000D6F4 File Offset: 0x0000B8F4
	private void OnEnable()
	{
		ComponentSingleton<ActivateColliderDelayCallbackHooks>.Instance.OnUpdate += this.OnUpdate;
		this.timer = this.time;
		this.targetCollider.enabled = this.activateonEnable;
		if (this.activateChance < 1f)
		{
			this.didActivation = (Random.Range(0f, 1f) > this.activateChance);
			return;
		}
		this.didActivation = false;
	}

	// Token: 0x06000219 RID: 537 RVA: 0x0000D766 File Offset: 0x0000B966
	private void OnDisable()
	{
		ComponentSingleton<ActivateColliderDelayCallbackHooks>.Instance.OnUpdate -= this.OnUpdate;
	}

	// Token: 0x0600021A RID: 538 RVA: 0x0000D780 File Offset: 0x0000B980
	private void OnUpdate()
	{
		if (this.didActivation)
		{
			return;
		}
		if (this.timer > 0f)
		{
			this.timer -= Time.deltaTime;
			return;
		}
		this.targetCollider.enabled = this.activate;
		this.didActivation = true;
	}

	// Token: 0x040001C9 RID: 457
	[SerializeField]
	private Collider2D targetCollider;

	// Token: 0x040001CA RID: 458
	[SerializeField]
	private float time;

	// Token: 0x040001CB RID: 459
	[SerializeField]
	private bool activate;

	// Token: 0x040001CC RID: 460
	[SerializeField]
	[Range(0f, 1f)]
	private float activateChance = 1f;

	// Token: 0x040001CD RID: 461
	[SerializeField]
	private bool activateonEnable;

	// Token: 0x040001CE RID: 462
	private float timer;

	// Token: 0x040001CF RID: 463
	private bool didActivation;
}
