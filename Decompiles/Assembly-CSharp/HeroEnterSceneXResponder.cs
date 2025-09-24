using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004FA RID: 1274
public class HeroEnterSceneXResponder : MonoBehaviour
{
	// Token: 0x06002D9C RID: 11676 RVA: 0x000C7760 File Offset: 0x000C5960
	private void Awake()
	{
		this.hc = HeroController.instance;
		if (this.hc.isHeroInPosition)
		{
			this.DoSet();
			return;
		}
		this.hc.heroInPosition += this.OnHeroInPosition;
	}

	// Token: 0x06002D9D RID: 11677 RVA: 0x000C7798 File Offset: 0x000C5998
	private void OnHeroInPosition(bool forcedirect)
	{
		this.hc.heroInPosition -= this.OnHeroInPosition;
		this.DoSet();
	}

	// Token: 0x06002D9E RID: 11678 RVA: 0x000C77B8 File Offset: 0x000C59B8
	public void DoSet()
	{
		ref Vector3 position = this.hc.transform.position;
		Vector3 position2 = base.transform.position;
		if (position.x < position2.x)
		{
			this.OnEnterSceneLeft.Invoke();
			return;
		}
		this.OnEnterSceneRight.Invoke();
	}

	// Token: 0x06002D9F RID: 11679 RVA: 0x000C7808 File Offset: 0x000C5A08
	public void SetScaleSignX(float xSign)
	{
		Vector3 localScale = base.transform.localScale;
		localScale.x = Mathf.Abs(localScale.x) * xSign;
		base.transform.localScale = localScale;
	}

	// Token: 0x04002F76 RID: 12150
	public UnityEvent OnEnterSceneLeft;

	// Token: 0x04002F77 RID: 12151
	public UnityEvent OnEnterSceneRight;

	// Token: 0x04002F78 RID: 12152
	private HeroController hc;
}
