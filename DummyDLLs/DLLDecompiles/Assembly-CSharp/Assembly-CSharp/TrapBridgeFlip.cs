using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200057D RID: 1405
public class TrapBridgeFlip : TrapBridge
{
	// Token: 0x06003249 RID: 12873 RVA: 0x000E0234 File Offset: 0x000DE434
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.DrawWireSphere(new Vector3(this.originXOffset, 0f, 0f), 0.1f);
	}

	// Token: 0x0600324A RID: 12874 RVA: 0x000E0265 File Offset: 0x000DE465
	private void OnEnable()
	{
		this.SetCollidersOpened(false);
	}

	// Token: 0x0600324B RID: 12875 RVA: 0x000E026E File Offset: 0x000DE46E
	private void PlayAnim(int animID)
	{
		if (!this.animator)
		{
			return;
		}
		this.animator.Play(animID);
	}

	// Token: 0x0600324C RID: 12876 RVA: 0x000E028A File Offset: 0x000DE48A
	public void ReportOpened()
	{
		this.isWaitingForOpen = false;
	}

	// Token: 0x0600324D RID: 12877 RVA: 0x000E0294 File Offset: 0x000DE494
	private void SetCollidersOpened(bool isOpened)
	{
		if (this.closedColliders)
		{
			this.closedColliders.SetActive(!isOpened);
		}
		if (this.openedColliders)
		{
			this.openedColliders.SetActive(isOpened);
		}
		this.activateOnOpened.SetAllActive(isOpened);
		this.deActivateOnOpened.SetAllActive(!isOpened);
	}

	// Token: 0x0600324E RID: 12878 RVA: 0x000E02F1 File Offset: 0x000DE4F1
	private int GetOpenAnimID()
	{
		if (this.IsHeroOnLeft())
		{
			return TrapBridgeFlip.OpenUpFirstAnim;
		}
		return TrapBridgeFlip.OpenDownFirstAnim;
	}

	// Token: 0x0600324F RID: 12879 RVA: 0x000E0306 File Offset: 0x000DE506
	private int GetCloseAnimID()
	{
		if (this.IsHeroOnLeft())
		{
			return TrapBridgeFlip.CloseDownFirstAnim;
		}
		return TrapBridgeFlip.CloseUpFirstAnim;
	}

	// Token: 0x06003250 RID: 12880 RVA: 0x000E031C File Offset: 0x000DE51C
	private bool IsHeroOnLeft()
	{
		float x = HeroController.instance.transform.position.x;
		float x2 = base.transform.TransformPoint(new Vector3(this.originXOffset, 0f, 0f)).x;
		bool flag = x < x2;
		if (base.transform.lossyScale.x < 0f)
		{
			flag = !flag;
		}
		return flag;
	}

	// Token: 0x06003251 RID: 12881 RVA: 0x000E0384 File Offset: 0x000DE584
	protected override IEnumerator DoOpenAnim()
	{
		this.isWaitingForOpen = true;
		this.PlayAnim(this.GetOpenAnimID());
		while (this.isWaitingForOpen)
		{
			yield return null;
		}
		this.SetCollidersOpened(true);
		yield break;
	}

	// Token: 0x06003252 RID: 12882 RVA: 0x000E0393 File Offset: 0x000DE593
	protected override IEnumerator DoCloseAnim()
	{
		this.SetCollidersOpened(false);
		this.PlayAnim(this.GetCloseAnimID());
		yield return null;
		yield break;
	}

	// Token: 0x040035F8 RID: 13816
	[Header("Flipper")]
	[SerializeField]
	private Animator animator;

	// Token: 0x040035F9 RID: 13817
	[SerializeField]
	private GameObject closedColliders;

	// Token: 0x040035FA RID: 13818
	[SerializeField]
	private GameObject openedColliders;

	// Token: 0x040035FB RID: 13819
	[SerializeField]
	private float originXOffset;

	// Token: 0x040035FC RID: 13820
	[Space]
	[SerializeField]
	private GameObject[] activateOnOpened;

	// Token: 0x040035FD RID: 13821
	[SerializeField]
	private GameObject[] deActivateOnOpened;

	// Token: 0x040035FE RID: 13822
	private bool isWaitingForOpen;

	// Token: 0x040035FF RID: 13823
	private static readonly int ClosedAnim = Animator.StringToHash("Closed");

	// Token: 0x04003600 RID: 13824
	private static readonly int OpenUpFirstAnim = Animator.StringToHash("Open Up First");

	// Token: 0x04003601 RID: 13825
	private static readonly int OpenDownFirstAnim = Animator.StringToHash("Open Down First");

	// Token: 0x04003602 RID: 13826
	private static readonly int CloseUpFirstAnim = Animator.StringToHash("Close Up First");

	// Token: 0x04003603 RID: 13827
	private static readonly int CloseDownFirstAnim = Animator.StringToHash("Close Down First");
}
