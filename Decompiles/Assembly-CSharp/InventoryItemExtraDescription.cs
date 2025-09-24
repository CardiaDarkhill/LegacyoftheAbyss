using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200068C RID: 1676
public class InventoryItemExtraDescription : MonoBehaviour
{
	// Token: 0x140000C7 RID: 199
	// (add) Token: 0x06003BC9 RID: 15305 RVA: 0x00106D68 File Offset: 0x00104F68
	// (remove) Token: 0x06003BCA RID: 15306 RVA: 0x00106DA0 File Offset: 0x00104FA0
	public event Action<GameObject> ActivatedDesc;

	// Token: 0x170006C3 RID: 1731
	// (get) Token: 0x06003BCB RID: 15307 RVA: 0x00106DD5 File Offset: 0x00104FD5
	public bool WillDisplay
	{
		get
		{
			return this.condition.IsFulfilled;
		}
	}

	// Token: 0x170006C4 RID: 1732
	// (get) Token: 0x06003BCC RID: 15308 RVA: 0x00106DE2 File Offset: 0x00104FE2
	// (set) Token: 0x06003BCD RID: 15309 RVA: 0x00106DEC File Offset: 0x00104FEC
	public GameObject ExtraDescPrefab
	{
		get
		{
			return this.extraDescPrefab;
		}
		set
		{
			if (this.extraDescPrefab == value)
			{
				return;
			}
			GameObject gameObject;
			if (this.isSelected && this.extraDescPrefab && InventoryItemExtraDescription._spawnedExtraDescriptions.TryGetValue(this.extraDescPrefab, out gameObject))
			{
				gameObject.SetActive(false);
			}
			this.extraDescPrefab = value;
			this.OnUpdateDisplay(this.selectable);
		}
	}

	// Token: 0x06003BCE RID: 15310 RVA: 0x00106E4C File Offset: 0x0010504C
	private void Awake()
	{
		this.selectable = base.GetComponent<InventoryItemSelectable>();
		this.selectable.OnSelected += this.OnSelected;
		this.selectable.OnDeselected += this.OnDeselected;
		this.selectable.OnUpdateDisplay += this.OnUpdateDisplay;
	}

	// Token: 0x06003BCF RID: 15311 RVA: 0x00106EAC File Offset: 0x001050AC
	private void OnSelected(InventoryItemSelectable self)
	{
		if (!this.condition.IsFulfilled)
		{
			return;
		}
		this.isSelected = true;
		if (!this.extraDescPrefab)
		{
			return;
		}
		if (!InventoryItemExtraDescription._spawnedExtraDescriptions.ContainsKey(this.extraDescPrefab))
		{
			return;
		}
		GameObject gameObject = InventoryItemExtraDescription._spawnedExtraDescriptions[this.extraDescPrefab];
		if (!gameObject)
		{
			return;
		}
		gameObject.SetActive(true);
		Action<GameObject> activatedDesc = this.ActivatedDesc;
		if (activatedDesc == null)
		{
			return;
		}
		activatedDesc(gameObject);
	}

	// Token: 0x06003BD0 RID: 15312 RVA: 0x00106F24 File Offset: 0x00105124
	private void OnDeselected(InventoryItemSelectable self)
	{
		this.isSelected = false;
		if (!this.extraDescPrefab)
		{
			return;
		}
		GameObject gameObject;
		if (InventoryItemExtraDescription._spawnedExtraDescriptions.TryGetValue(this.extraDescPrefab, out gameObject))
		{
			gameObject.SetActive(false);
		}
	}

	// Token: 0x06003BD1 RID: 15313 RVA: 0x00106F64 File Offset: 0x00105164
	private void OnUpdateDisplay(InventoryItemSelectable self)
	{
		if (!this.extraDescPrefab || !this.descSectionParent)
		{
			return;
		}
		GameObject gameObject;
		if (InventoryItemExtraDescription._spawnedExtraDescriptions.TryGetValue(this.extraDescPrefab, out gameObject) && gameObject)
		{
			if (this.isSelected)
			{
				Action<GameObject> activatedDesc = this.ActivatedDesc;
				if (activatedDesc == null)
				{
					return;
				}
				activatedDesc(gameObject);
			}
			return;
		}
		GameObject gameObject2 = Object.Instantiate<GameObject>(this.extraDescPrefab, this.descSectionParent, false);
		InventoryItemExtraDescription._spawnedExtraDescriptions[this.extraDescPrefab] = gameObject2;
		gameObject2.SetActive(this.isSelected);
		if (this.isSelected)
		{
			Action<GameObject> activatedDesc2 = this.ActivatedDesc;
			if (activatedDesc2 == null)
			{
				return;
			}
			activatedDesc2(gameObject2);
		}
	}

	// Token: 0x04003DF8 RID: 15864
	[SerializeField]
	private Transform descSectionParent;

	// Token: 0x04003DF9 RID: 15865
	[SerializeField]
	private GameObject extraDescPrefab;

	// Token: 0x04003DFA RID: 15866
	[SerializeField]
	private PlayerDataTest condition;

	// Token: 0x04003DFB RID: 15867
	private InventoryItemSelectable selectable;

	// Token: 0x04003DFC RID: 15868
	private bool isSelected;

	// Token: 0x04003DFD RID: 15869
	private static readonly Dictionary<GameObject, GameObject> _spawnedExtraDescriptions = new Dictionary<GameObject, GameObject>();
}
