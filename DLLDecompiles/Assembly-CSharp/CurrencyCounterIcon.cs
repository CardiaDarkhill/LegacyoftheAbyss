using System;
using UnityEngine;

// Token: 0x02000625 RID: 1573
public class CurrencyCounterIcon : MonoBehaviour
{
	// Token: 0x06003815 RID: 14357 RVA: 0x000F7DCC File Offset: 0x000F5FCC
	public void Appear()
	{
		if (this.scaleRoutine != null)
		{
			base.StopCoroutine(this.scaleRoutine);
		}
		if (this.fsm)
		{
			this.fsm.SendEvent("APPEAR");
			return;
		}
		if (this.animator)
		{
			this.animator.Play("Appear");
			return;
		}
		base.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
		base.transform.ScaleTo(this, Vector3.one, 0.2f, 0f, false, false, null);
	}

	// Token: 0x06003816 RID: 14358 RVA: 0x000F7E68 File Offset: 0x000F6068
	public void Disappear()
	{
		if (this.scaleRoutine != null)
		{
			base.StopCoroutine(this.scaleRoutine);
		}
		if (this.fsm)
		{
			this.fsm.SendEvent("DISAPPEAR");
			return;
		}
		if (this.animator)
		{
			this.animator.Play("Disappear");
			return;
		}
		this.scaleFrom = base.transform.localScale;
		this.scaleRoutine = this.StartTimerRoutine(0f, 0.2f, delegate(float t)
		{
			base.transform.localScale = Vector3.Lerp(this.scaleFrom, new Vector3(0.5f, 0.5f, 1f), t);
		}, null, delegate
		{
			base.transform.localScale = Vector3.zero;
		}, false);
	}

	// Token: 0x06003817 RID: 14359 RVA: 0x000F7F08 File Offset: 0x000F6108
	public void HideInstant()
	{
		if (this.scaleRoutine != null)
		{
			base.StopCoroutine(this.scaleRoutine);
		}
		if (this.fsm)
		{
			this.fsm.enabled = false;
			this.fsm.enabled = true;
			return;
		}
		if (this.animator)
		{
			base.gameObject.SetActive(false);
			base.gameObject.SetActive(true);
			return;
		}
		base.transform.localScale = Vector3.zero;
	}

	// Token: 0x06003818 RID: 14360 RVA: 0x000F7F85 File Offset: 0x000F6185
	public void Idle()
	{
		if (this.fsm)
		{
			this.fsm.SendEvent("IDLE");
		}
	}

	// Token: 0x06003819 RID: 14361 RVA: 0x000F7FA4 File Offset: 0x000F61A4
	public void Get()
	{
		if (this.fsm)
		{
			this.fsm.SendEvent("GET");
		}
	}

	// Token: 0x0600381A RID: 14362 RVA: 0x000F7FC3 File Offset: 0x000F61C3
	public void GetSingle()
	{
		if (this.fsm)
		{
			this.fsm.SendEvent("GET SINGLE");
		}
	}

	// Token: 0x0600381B RID: 14363 RVA: 0x000F7FE2 File Offset: 0x000F61E2
	public void Take()
	{
		if (this.fsm)
		{
			this.fsm.SendEvent("TAKE");
		}
	}

	// Token: 0x04003B0F RID: 15119
	[SerializeField]
	private PlayMakerFSM fsm;

	// Token: 0x04003B10 RID: 15120
	[SerializeField]
	private Animator animator;

	// Token: 0x04003B11 RID: 15121
	private Vector3 scaleFrom;

	// Token: 0x04003B12 RID: 15122
	private Coroutine scaleRoutine;
}
