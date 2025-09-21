using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000702 RID: 1794
public class ResolutionCountdownTimer : MonoBehaviour
{
	// Token: 0x06004013 RID: 16403 RVA: 0x0011A652 File Offset: 0x00118852
	private void Start()
	{
		this.ih = GameManager.instance.inputHandler;
	}

	// Token: 0x06004014 RID: 16404 RVA: 0x0011A664 File Offset: 0x00118864
	public void StartTimer()
	{
		this.currentTime = this.timerDuration;
		this.UpdateTimerText();
		this.running = true;
		base.StartCoroutine("CountDown");
	}

	// Token: 0x06004015 RID: 16405 RVA: 0x0011A68B File Offset: 0x0011888B
	public void CancelTimer()
	{
		this.running = false;
		base.StopCoroutine("CountDown");
	}

	// Token: 0x06004016 RID: 16406 RVA: 0x0011A6A0 File Offset: 0x001188A0
	private void TickDown()
	{
		if (this.currentTime == 0)
		{
			this.timerText.text = "";
			this.running = false;
			this.CancelTimer();
			base.StartCoroutine(this.RollbackRes());
			return;
		}
		this.UpdateTimerText();
		this.currentTime--;
	}

	// Token: 0x06004017 RID: 16407 RVA: 0x0011A6F4 File Offset: 0x001188F4
	private void UpdateTimerText()
	{
		this.timerText.text = this.currentTime.ToString();
	}

	// Token: 0x06004018 RID: 16408 RVA: 0x0011A70C File Offset: 0x0011890C
	private IEnumerator CountDown()
	{
		int num;
		for (int i = 0; i < 20; i = num + 1)
		{
			if (this.running)
			{
				this.TickDown();
				yield return GameManager.instance.timeTool.TimeScaleIndependentWaitForSeconds(1f);
			}
			num = i;
		}
		yield break;
	}

	// Token: 0x06004019 RID: 16409 RVA: 0x0011A71B File Offset: 0x0011891B
	private IEnumerator RollbackRes()
	{
		this.ih.StopUIInput();
		yield return null;
		UIManager.instance.UIGoToVideoMenu(true);
		yield break;
	}

	// Token: 0x040041C0 RID: 16832
	public int timerDuration;

	// Token: 0x040041C1 RID: 16833
	public Text timerText;

	// Token: 0x040041C2 RID: 16834
	private int currentTime;

	// Token: 0x040041C3 RID: 16835
	private bool running;

	// Token: 0x040041C4 RID: 16836
	private InputHandler ih;
}
