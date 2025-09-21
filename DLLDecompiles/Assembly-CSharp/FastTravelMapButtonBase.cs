using System;
using TeamCherry.SharedUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000659 RID: 1625
public class FastTravelMapButtonBase<T> : MonoBehaviour where T : struct, IComparable
{
	// Token: 0x06003A1F RID: 14879 RVA: 0x000FEE38 File Offset: 0x000FD038
	private void Awake()
	{
		this.parentMap = base.GetComponentInParent<FastTravelMapBase<T>>();
		if (this.parentMap != null)
		{
			this.parentMap.Opening += this.OnOpening;
			this.parentMap.Opened += this.OnOpened;
		}
		this.item = base.GetComponent<UISelectionListItem>();
		if (this.item)
		{
			this.item.AutoSelect = new Func<bool>(this.IsCurrentLocation);
			this.item.SubmitPressed.AddListener(new UnityAction(this.Submit));
			this.item.CancelPressed.AddListener(new UnityAction(this.Cancel));
			this.item.Selected.AddListener(new UnityAction(this.Select));
			this.item.Deselected.AddListener(new UnityAction(this.Deselect));
		}
	}

	// Token: 0x06003A20 RID: 14880 RVA: 0x000FEF30 File Offset: 0x000FD130
	private void OnOpening()
	{
		if (!this.IsUnlocked())
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
	}

	// Token: 0x06003A21 RID: 14881 RVA: 0x000FEF53 File Offset: 0x000FD153
	private void OnOpened()
	{
		if (this.IsCurrentLocation())
		{
			this.parentMap.SetCurrentLocationIndicatorPosition(base.transform.position.y);
		}
	}

	// Token: 0x06003A22 RID: 14882 RVA: 0x000FEF78 File Offset: 0x000FD178
	public void Submit()
	{
		if (this.parentMap == null)
		{
			return;
		}
		this.parentMap.ConfirmLocation(this.targetLocation);
	}

	// Token: 0x06003A23 RID: 14883 RVA: 0x000FEF9C File Offset: 0x000FD19C
	public void Cancel()
	{
		if (this.parentMap == null)
		{
			return;
		}
		this.parentMap.ConfirmLocation(default(T));
	}

	// Token: 0x06003A24 RID: 14884 RVA: 0x000FEFCC File Offset: 0x000FD1CC
	private void Select()
	{
		if (this.Selected != null)
		{
			this.Selected();
		}
	}

	// Token: 0x06003A25 RID: 14885 RVA: 0x000FEFE1 File Offset: 0x000FD1E1
	private void Deselect()
	{
		if (this.Deselected != null)
		{
			this.Deselected();
		}
	}

	// Token: 0x06003A26 RID: 14886 RVA: 0x000FEFF8 File Offset: 0x000FD1F8
	public bool IsCurrentLocation()
	{
		if (!this.IsUnlocked())
		{
			return false;
		}
		if (this.parentMap != null)
		{
			T autoSelectLocation = this.parentMap.AutoSelectLocation;
			return autoSelectLocation.CompareTo(this.targetLocation) == 0;
		}
		return false;
	}

	// Token: 0x06003A27 RID: 14887 RVA: 0x000FF046 File Offset: 0x000FD246
	public bool IsUnlocked()
	{
		return string.IsNullOrEmpty(this.playerDataBool) || PlayerData.instance.GetVariable(this.playerDataBool);
	}

	// Token: 0x04003CBC RID: 15548
	public Action Selected;

	// Token: 0x04003CBD RID: 15549
	public Action Deselected;

	// Token: 0x04003CBE RID: 15550
	[FormerlySerializedAs("targetlocation")]
	[SerializeField]
	private T targetLocation;

	// Token: 0x04003CBF RID: 15551
	[SerializeField]
	[PlayerDataField(typeof(bool), false)]
	private string playerDataBool;

	// Token: 0x04003CC0 RID: 15552
	private FastTravelMapBase<T> parentMap;

	// Token: 0x04003CC1 RID: 15553
	private UISelectionListItem item;
}
