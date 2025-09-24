using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000657 RID: 1623
public abstract class FastTravelMapBase<TLocation> : MonoBehaviour, IFastTravelMap
{
	// Token: 0x140000C0 RID: 192
	// (add) Token: 0x06003A0A RID: 14858 RVA: 0x000FE97C File Offset: 0x000FCB7C
	// (remove) Token: 0x06003A0B RID: 14859 RVA: 0x000FE9B4 File Offset: 0x000FCBB4
	public event Action Opening;

	// Token: 0x140000C1 RID: 193
	// (add) Token: 0x06003A0C RID: 14860 RVA: 0x000FE9EC File Offset: 0x000FCBEC
	// (remove) Token: 0x06003A0D RID: 14861 RVA: 0x000FEA24 File Offset: 0x000FCC24
	public event Action Opened;

	// Token: 0x140000C2 RID: 194
	// (add) Token: 0x06003A0E RID: 14862 RVA: 0x000FEA5C File Offset: 0x000FCC5C
	// (remove) Token: 0x06003A0F RID: 14863 RVA: 0x000FEA94 File Offset: 0x000FCC94
	public event Action<TLocation> LocationConfirmed;

	// Token: 0x140000C3 RID: 195
	// (add) Token: 0x06003A10 RID: 14864 RVA: 0x000FEACC File Offset: 0x000FCCCC
	// (remove) Token: 0x06003A11 RID: 14865 RVA: 0x000FEB04 File Offset: 0x000FCD04
	public event Action PaneClosed;

	// Token: 0x17000693 RID: 1683
	// (get) Token: 0x06003A12 RID: 14866 RVA: 0x000FEB39 File Offset: 0x000FCD39
	// (set) Token: 0x06003A13 RID: 14867 RVA: 0x000FEB41 File Offset: 0x000FCD41
	public TLocation AutoSelectLocation { get; set; }

	// Token: 0x06003A14 RID: 14868 RVA: 0x000FEB4C File Offset: 0x000FCD4C
	private void Awake()
	{
		this.list = base.GetComponent<UISelectionList>();
		this.pane = base.GetComponent<InventoryPaneStandalone>();
		if (this.pane)
		{
			this.pane.PaneOpenedAnimEnd += delegate()
			{
				if (this.list)
				{
					this.list.SetActive(true);
				}
			};
			this.pane.PaneClosedAnimEnd += delegate()
			{
				Action paneClosed = this.PaneClosed;
				if (paneClosed == null)
				{
					return;
				}
				paneClosed();
			};
		}
	}

	// Token: 0x06003A15 RID: 14869 RVA: 0x000FEBAC File Offset: 0x000FCDAC
	public void Open()
	{
		if (this.listLocationIndicator)
		{
			this.listLocationIndicator.gameObject.SetActive(false);
		}
		if (this.mapLocationIndicator)
		{
			this.mapLocationIndicator.gameObject.SetActive(false);
		}
		GameCameras.instance.HUDOut();
		if (this.list)
		{
			this.list.SetActive(false);
		}
		if (this.pane)
		{
			this.pane.PaneStart();
		}
		Action opening = this.Opening;
		if (opening != null)
		{
			opening();
		}
		if (this.listLayout)
		{
			this.listLayout.UpdatePositions();
		}
		if (this.layoutGroup)
		{
			this.layoutGroup.ForceUpdateLayoutNoCanvas();
		}
		Action opened = this.Opened;
		if (opened != null)
		{
			opened();
		}
		this.onOpened.Invoke();
	}

	// Token: 0x06003A16 RID: 14870 RVA: 0x000FEC90 File Offset: 0x000FCE90
	public void ConfirmLocation(TLocation location)
	{
		Action<TLocation> locationConfirmed = this.LocationConfirmed;
		if (locationConfirmed != null)
		{
			locationConfirmed(location);
		}
		if (this.pane)
		{
			this.pane.PaneEnd();
		}
		if (this.list != null)
		{
			this.list.SetActive(false);
		}
	}

	// Token: 0x06003A17 RID: 14871 RVA: 0x000FECE1 File Offset: 0x000FCEE1
	public void SetCurrentLocationIndicatorPosition(float positionY)
	{
		if (!this.listLocationIndicator)
		{
			return;
		}
		this.listLocationIndicator.gameObject.SetActive(true);
		this.listLocationIndicator.SetPositionY(positionY + this.listLocationIndicatorYOffset);
	}

	// Token: 0x06003A18 RID: 14872 RVA: 0x000FED15 File Offset: 0x000FCF15
	public void SetMapIndicatorPosition(Vector2 position)
	{
		if (!this.mapLocationIndicator)
		{
			return;
		}
		this.mapLocationIndicator.gameObject.SetActive(true);
		this.mapLocationIndicator.SetPosition2D(position);
	}

	// Token: 0x06003A19 RID: 14873 RVA: 0x000FED44 File Offset: 0x000FCF44
	public void SetMapSelectorPosition(Vector2 position, bool isInstant)
	{
		if (!this.mapSelector)
		{
			return;
		}
		if (this.moveSelectorRoutine != null)
		{
			base.StopCoroutine(this.moveSelectorRoutine);
		}
		if (isInstant)
		{
			this.mapSelector.SetPosition2D(position);
			return;
		}
		this.mapSelectorStartPos = this.mapSelector.position;
		this.mapSelectorEndPos = new Vector3(position.x, position.y, this.mapSelectorStartPos.z);
		this.moveSelectorRoutine = this.StartTimerRoutine(0f, this.mapSelectorTweenTime, delegate(float time)
		{
			this.mapSelector.position = Vector3.Lerp(this.mapSelectorStartPos, this.mapSelectorEndPos, time);
		}, null, null, false);
	}

	// Token: 0x04003CAE RID: 15534
	[SerializeField]
	private Transform listLocationIndicator;

	// Token: 0x04003CAF RID: 15535
	[SerializeField]
	private float listLocationIndicatorYOffset;

	// Token: 0x04003CB0 RID: 15536
	[Space]
	[SerializeField]
	private Transform mapLocationIndicator;

	// Token: 0x04003CB1 RID: 15537
	[SerializeField]
	private Transform mapSelector;

	// Token: 0x04003CB2 RID: 15538
	[SerializeField]
	private float mapSelectorTweenTime;

	// Token: 0x04003CB3 RID: 15539
	[SerializeField]
	private TransformLayout listLayout;

	// Token: 0x04003CB4 RID: 15540
	[SerializeField]
	private LayoutGroup layoutGroup;

	// Token: 0x04003CB5 RID: 15541
	[Space]
	[SerializeField]
	private UnityEvent onOpened;

	// Token: 0x04003CB6 RID: 15542
	private Coroutine moveSelectorRoutine;

	// Token: 0x04003CB7 RID: 15543
	private Vector3 mapSelectorStartPos;

	// Token: 0x04003CB8 RID: 15544
	private Vector3 mapSelectorEndPos;

	// Token: 0x04003CB9 RID: 15545
	private InventoryPaneStandalone pane;

	// Token: 0x04003CBA RID: 15546
	private UISelectionList list;
}
