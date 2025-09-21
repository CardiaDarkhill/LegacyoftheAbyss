using System;
using UnityEngine.EventSystems;

// Token: 0x02000731 RID: 1841
public class StartGameEventTrigger : MenuButtonListCondition, ISubmitHandler, IEventSystemHandler, IPointerClickHandler
{
	// Token: 0x060041C7 RID: 16839 RVA: 0x001217D5 File Offset: 0x0011F9D5
	public void OnSubmit(BaseEventData eventData)
	{
		UIManager.instance.StartNewGame(this.permaDeath, this.bossRush);
	}

	// Token: 0x060041C8 RID: 16840 RVA: 0x001217ED File Offset: 0x0011F9ED
	public void OnPointerClick(PointerEventData eventData)
	{
		this.OnSubmit(eventData);
	}

	// Token: 0x060041C9 RID: 16841 RVA: 0x001217F8 File Offset: 0x0011F9F8
	public override bool IsFulfilled()
	{
		bool result = true;
		if (this.permaDeath && GameManager.instance.GetStatusRecordInt("RecPermadeathMode") == 0)
		{
			result = false;
		}
		if (this.bossRush && GameManager.instance.GetStatusRecordInt("RecBossRushMode") == 0)
		{
			result = false;
		}
		return result;
	}

	// Token: 0x0400435B RID: 17243
	public bool permaDeath;

	// Token: 0x0400435C RID: 17244
	public bool bossRush;
}
