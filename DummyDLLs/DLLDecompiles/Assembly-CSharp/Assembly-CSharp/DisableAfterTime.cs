using System;
using UnityEngine;

// Token: 0x0200020A RID: 522
public class DisableAfterTime : MonoBehaviour
{
	// Token: 0x06001395 RID: 5013 RVA: 0x000594A2 File Offset: 0x000576A2
	private void OnEnable()
	{
		this.timeLeft = this.waitTime;
	}

	// Token: 0x06001396 RID: 5014 RVA: 0x000594B0 File Offset: 0x000576B0
	private void OnDisable()
	{
		GameObject gameObject = base.gameObject;
		if (gameObject != null && !gameObject.activeInHierarchy && gameObject.activeSelf)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001397 RID: 5015 RVA: 0x000594E4 File Offset: 0x000576E4
	private void Update()
	{
		this.timeLeft -= (this.isRealtime ? Time.unscaledDeltaTime : Time.deltaTime);
		if (this.timeLeft > 0f)
		{
			return;
		}
		if (!string.IsNullOrEmpty(this.sendEvent))
		{
			FSMUtility.SendEventToGameObject(base.gameObject, this.sendEvent, false);
			return;
		}
		base.gameObject.SetActive(false);
	}

	// Token: 0x040011F8 RID: 4600
	[SerializeField]
	private float waitTime = 5f;

	// Token: 0x040011F9 RID: 4601
	[SerializeField]
	private bool isRealtime;

	// Token: 0x040011FA RID: 4602
	[SerializeField]
	private string sendEvent;

	// Token: 0x040011FB RID: 4603
	private float timeLeft;
}
