using System;
using System.Collections;
using GlobalSettings;
using TeamCherry.Localization;
using TMProOld;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001BC RID: 444
public class CollectableUIMsg : UIMsgPopupBase<ICollectableUIMsgItem, CollectableUIMsg>
{
	// Token: 0x0600113D RID: 4413 RVA: 0x00050E4F File Offset: 0x0004F04F
	public static CollectableUIMsg Spawn(ICollectableUIMsgItem item, CollectableUIMsg replacing = null, bool forceReplacingEffect = false)
	{
		return CollectableUIMsg.Spawn(item, Color.white, replacing, forceReplacingEffect);
	}

	// Token: 0x0600113E RID: 4414 RVA: 0x00050E60 File Offset: 0x0004F060
	public static CollectableUIMsg Spawn(ICollectableUIMsgItem item, Color textColor, CollectableUIMsg replacing = null, bool forceReplacingEffect = false)
	{
		CollectableUIMsg collectableUIMsgPrefab = UI.CollectableUIMsgPrefab;
		if (!collectableUIMsgPrefab)
		{
			return null;
		}
		CollectableUIMsg collectableUIMsg = UIMsgPopupBase<ICollectableUIMsgItem, CollectableUIMsg>.SpawnInternal(collectableUIMsgPrefab, item, replacing, forceReplacingEffect);
		if (!collectableUIMsg)
		{
			return null;
		}
		if (collectableUIMsg.nameText)
		{
			collectableUIMsg.nameText.color = textColor;
		}
		return collectableUIMsg;
	}

	// Token: 0x0600113F RID: 4415 RVA: 0x00050EAC File Offset: 0x0004F0AC
	public static void ShowTakeMsg(ICollectableUIMsgItem item, TakeItemTypes takeItemType)
	{
		if (takeItemType == TakeItemTypes.Silent)
		{
			return;
		}
		LocalisedString s;
		switch (takeItemType)
		{
		case TakeItemTypes.Taken:
			s = UI.ItemTakenPopup;
			break;
		case TakeItemTypes.Given:
			s = UI.ItemGivenPopup;
			break;
		case TakeItemTypes.Deposited:
			s = UI.ItemDepositedPopup;
			break;
		default:
			throw new ArgumentOutOfRangeException("takeItemType", takeItemType, null);
		}
		string text = s;
		CollectableItem collectableItem = item as CollectableItem;
		string text2;
		Sprite uimsgSprite;
		if (collectableItem != null)
		{
			text2 = collectableItem.GetDisplayName(CollectableItem.ReadSource.TakePopup);
			uimsgSprite = collectableItem.GetIcon(CollectableItem.ReadSource.TakePopup);
		}
		else
		{
			text2 = item.GetUIMsgName();
			uimsgSprite = item.GetUIMsgSprite();
		}
		string name;
		text.TryFormat(out name, new object[]
		{
			text2
		});
		CollectableUIMsg.Spawn(new UIMsgDisplay
		{
			Name = name,
			Icon = uimsgSprite,
			IconScale = item.GetUIMsgIconScale(),
			RepresentingObject = item.GetRepresentingObject()
		}, null, false);
	}

	// Token: 0x06001140 RID: 4416 RVA: 0x00050F84 File Offset: 0x0004F184
	protected override void UpdateDisplay(ICollectableUIMsgItem item)
	{
		if (item == null)
		{
			Debug.LogError("item was null", this);
			return;
		}
		if (this.icon)
		{
			this.icon.sprite = item.GetUIMsgSprite();
			this.icon.transform.localScale = Vector3.one * item.GetUIMsgIconScale();
		}
		if (this.nameText)
		{
			this.nameText.text = item.GetUIMsgName().ToSingleLine();
		}
		if (this.layoutGroup)
		{
			this.layoutGroup.ForceUpdateLayoutNoCanvas();
		}
		if (this.upgradeIcon)
		{
			this.upgradeIcon.gameObject.SetActive(item.HasUpgradeIcon());
		}
	}

	// Token: 0x06001141 RID: 4417 RVA: 0x0005103C File Offset: 0x0004F23C
	public void Replace(float delay, ICollectableUIMsgItem item)
	{
		if (this.replaceRoutine != null)
		{
			base.StopCoroutine(this.replaceRoutine);
		}
		this.replaceRoutine = base.StartCoroutine(this.ReplaceDelayed(delay, item));
	}

	// Token: 0x06001142 RID: 4418 RVA: 0x00051066 File Offset: 0x0004F266
	private IEnumerator ReplaceDelayed(float delay, ICollectableUIMsgItem item)
	{
		yield return new WaitForSeconds(delay);
		CollectableUIMsg.Spawn(item, this, true);
		this.replaceRoutine = null;
		yield break;
	}

	// Token: 0x04001044 RID: 4164
	[Space]
	[SerializeField]
	private SpriteRenderer icon;

	// Token: 0x04001045 RID: 4165
	[SerializeField]
	private SpriteRenderer upgradeIcon;

	// Token: 0x04001046 RID: 4166
	[SerializeField]
	private TextMeshPro nameText;

	// Token: 0x04001047 RID: 4167
	[SerializeField]
	private LayoutGroup layoutGroup;

	// Token: 0x04001048 RID: 4168
	private Coroutine replaceRoutine;
}
