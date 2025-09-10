using System;
using UnityEngine;

// Token: 0x0200065D RID: 1629
public class FastTravelMapPieceBase<TButtonType, TMapType, TLocation> : MonoBehaviour, IFastTravelMapPiece where TButtonType : FastTravelMapButtonBase<TLocation> where TMapType : FastTravelMapBase<TLocation> where TLocation : struct, IComparable
{
	// Token: 0x17000696 RID: 1686
	// (get) Token: 0x06003A30 RID: 14896 RVA: 0x000FF17A File Offset: 0x000FD37A
	public bool IsVisible
	{
		get
		{
			return this.pairedButton && this.pairedButton.IsUnlocked();
		}
	}

	// Token: 0x06003A31 RID: 14897 RVA: 0x000FF1A0 File Offset: 0x000FD3A0
	private void OnDrawGizmosSelected()
	{
		Gizmos.matrix = base.transform.localToWorldMatrix;
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(this.indicatorOffset, 0.1f);
	}

	// Token: 0x06003A32 RID: 14898 RVA: 0x000FF1D4 File Offset: 0x000FD3D4
	private void Awake()
	{
		this.parentMap = base.GetComponentInParent<TMapType>();
		if (this.parentMap != null)
		{
			this.parentMap.Opened += this.OnOpened;
		}
		if (this.pairedButton)
		{
			TButtonType tbuttonType = this.pairedButton;
			tbuttonType.Selected = (Action)Delegate.Combine(tbuttonType.Selected, new Action(this.Select));
		}
	}

	// Token: 0x06003A33 RID: 14899 RVA: 0x000FF25C File Offset: 0x000FD45C
	private void OnOpened()
	{
		if (!this.pairedButton || !this.pairedButton.IsUnlocked())
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		if (!this.pairedButton.IsCurrentLocation())
		{
			return;
		}
		Vector3 v = base.transform.TransformPoint(this.indicatorOffset);
		this.parentMap.SetMapIndicatorPosition(v);
		this.parentMap.SetMapSelectorPosition(v, true);
	}

	// Token: 0x06003A34 RID: 14900 RVA: 0x000FF300 File Offset: 0x000FD500
	private void Select()
	{
		if (this.parentMap != null)
		{
			this.parentMap.SetMapSelectorPosition(base.transform.TransformPoint(this.indicatorOffset), false);
		}
	}

	// Token: 0x04003CC3 RID: 15555
	[SerializeField]
	private TButtonType pairedButton;

	// Token: 0x04003CC4 RID: 15556
	[SerializeField]
	private Vector2 indicatorOffset;

	// Token: 0x04003CC5 RID: 15557
	private TMapType parentMap;
}
