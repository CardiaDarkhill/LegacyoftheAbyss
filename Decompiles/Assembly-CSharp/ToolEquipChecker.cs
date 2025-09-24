using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020005E2 RID: 1506
public class ToolEquipChecker : MonoBehaviour
{
	// Token: 0x06003569 RID: 13673 RVA: 0x000EC68C File Offset: 0x000EA88C
	private void Awake()
	{
		this.eventRegister = EventRegister.GetRegisterGuaranteed(base.gameObject, "TOOL EQUIPS CHANGED");
		if (this.activateEvent)
		{
			this.activateEvent.ReceivedEvent += this.OnActivate;
			return;
		}
		this.isActive = true;
	}

	// Token: 0x0600356A RID: 13674 RVA: 0x000EC6DB File Offset: 0x000EA8DB
	private void OnEnable()
	{
		this.eventRegister.ReceivedEvent += this.OnToolEquipsChanged;
		this.OnToolEquipsChanged();
	}

	// Token: 0x0600356B RID: 13675 RVA: 0x000EC6FA File Offset: 0x000EA8FA
	private void OnDisable()
	{
		this.eventRegister.ReceivedEvent -= this.OnToolEquipsChanged;
	}

	// Token: 0x0600356C RID: 13676 RVA: 0x000EC713 File Offset: 0x000EA913
	private void OnActivate()
	{
		this.activateEvent.ReceivedEvent -= this.OnActivate;
		this.isActive = true;
		this.OnToolEquipsChanged();
	}

	// Token: 0x0600356D RID: 13677 RVA: 0x000EC73C File Offset: 0x000EA93C
	private void OnToolEquipsChanged()
	{
		if (!this.isActive)
		{
			return;
		}
		bool flag;
		if (this.tool != null)
		{
			ToolItem toolItem = this.tool as ToolItem;
			flag = ((toolItem != null) ? ToolItemManager.IsToolEquipped(toolItem, (base.gameObject.layer == 5) ? ToolEquippedReadSource.Hud : ToolEquippedReadSource.Active) : this.tool.IsEquipped);
		}
		else
		{
			flag = false;
		}
		float num = flag ? this.toolEquippedDelay : this.toolNotEquippedDelay;
		if (this.invokeDelayedRoutine != null)
		{
			base.StopCoroutine(this.invokeDelayedRoutine);
			this.invokeDelayedRoutine = null;
		}
		if (num > 0f)
		{
			this.invokeDelayedRoutine = base.StartCoroutine(this.InvokeDelayed(flag, num));
			return;
		}
		this.SendEvents(flag);
	}

	// Token: 0x0600356E RID: 13678 RVA: 0x000EC7EC File Offset: 0x000EA9EC
	private void SendEvents(bool isEquipped)
	{
		UnityEvent<bool> toolEquippedDynamic = this.ToolEquippedDynamic;
		if (toolEquippedDynamic != null)
		{
			toolEquippedDynamic.Invoke(isEquipped);
		}
		UnityEvent<bool> toolEquippedDynamicReversed = this.ToolEquippedDynamicReversed;
		if (toolEquippedDynamicReversed != null)
		{
			toolEquippedDynamicReversed.Invoke(!isEquipped);
		}
		if (isEquipped)
		{
			UnityEvent toolEquipped = this.ToolEquipped;
			if (toolEquipped == null)
			{
				return;
			}
			toolEquipped.Invoke();
			return;
		}
		else
		{
			UnityEvent toolNotEquipped = this.ToolNotEquipped;
			if (toolNotEquipped == null)
			{
				return;
			}
			toolNotEquipped.Invoke();
			return;
		}
	}

	// Token: 0x0600356F RID: 13679 RVA: 0x000EC844 File Offset: 0x000EAA44
	private IEnumerator InvokeDelayed(bool isEquipped, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.SendEvents(isEquipped);
		yield break;
	}

	// Token: 0x040038C8 RID: 14536
	[SerializeField]
	private ToolBase tool;

	// Token: 0x040038C9 RID: 14537
	[Space]
	[SerializeField]
	private EventRegister activateEvent;

	// Token: 0x040038CA RID: 14538
	[Space]
	public UnityEvent<bool> ToolEquippedDynamic;

	// Token: 0x040038CB RID: 14539
	public UnityEvent<bool> ToolEquippedDynamicReversed;

	// Token: 0x040038CC RID: 14540
	[Space]
	[SerializeField]
	private float toolEquippedDelay;

	// Token: 0x040038CD RID: 14541
	public UnityEvent ToolEquipped;

	// Token: 0x040038CE RID: 14542
	[SerializeField]
	private float toolNotEquippedDelay;

	// Token: 0x040038CF RID: 14543
	public UnityEvent ToolNotEquipped;

	// Token: 0x040038D0 RID: 14544
	private Coroutine invokeDelayedRoutine;

	// Token: 0x040038D1 RID: 14545
	private bool isActive;

	// Token: 0x040038D2 RID: 14546
	private EventRegister eventRegister;
}
