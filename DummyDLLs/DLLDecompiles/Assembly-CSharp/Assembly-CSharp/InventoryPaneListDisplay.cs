using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006B2 RID: 1714
public class InventoryPaneListDisplay : MonoBehaviour
{
	// Token: 0x06003DC9 RID: 15817 RVA: 0x0010F759 File Offset: 0x0010D959
	public void PreInstantiate(int panesCount)
	{
		this.InstantiateNeededItems(panesCount);
	}

	// Token: 0x06003DCA RID: 15818 RVA: 0x0010F764 File Offset: 0x0010D964
	public void UpdateDisplay(int currentPaneIndex, List<InventoryPane> panes, int cycleDirection)
	{
		int count = panes.Count;
		if (count <= 1)
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		if (cycleDirection > 0)
		{
			this.DoAnimationRoutine(this.DoArrowAnimation(1, this.rightArrowChild), ref this.arrowAnimationRoutine, ref this.onArrowAnimationEnd);
		}
		else if (cycleDirection < 0)
		{
			this.DoAnimationRoutine(this.DoArrowAnimation(-1, this.leftArrowChild), ref this.arrowAnimationRoutine, ref this.onArrowAnimationEnd);
		}
		this.InstantiateNeededItems(count);
		for (int i = 0; i < this.items.Count; i++)
		{
			if (i < count)
			{
				this.items[i].gameObject.SetActive(true);
			}
			else
			{
				this.items[i].gameObject.SetActive(false);
			}
		}
		float num = this.itemSpacing * (float)(count - 1) / 2f;
		for (int j = 0; j < count; j++)
		{
			int num2 = j;
			bool isSelected = j == currentPaneIndex;
			InventoryPaneListItem inventoryPaneListItem = this.items[num2];
			inventoryPaneListItem.UpdateValues(panes[j], isSelected);
			inventoryPaneListItem.transform.SetLocalPositionX(Mathf.Lerp(-num, num, (float)num2 / (float)(count - 1)));
		}
		if (this.leftArrow)
		{
			this.leftArrow.transform.SetLocalPositionX(-num - this.arrowOffset);
		}
		if (this.rightArrow)
		{
			this.rightArrow.transform.SetLocalPositionX(num + this.arrowOffset);
		}
	}

	// Token: 0x06003DCB RID: 15819 RVA: 0x0010F8D6 File Offset: 0x0010DAD6
	private IEnumerator DoArrowAnimation(int direction, Transform arrow)
	{
		if (!arrow)
		{
			yield break;
		}
		float startX = arrow.localPosition.x;
		this.onArrowAnimationEnd = delegate()
		{
			arrow.SetLocalPositionX(startX);
		};
		for (float elapsed = 0f; elapsed < this.arrowMoveXDuration; elapsed += Time.unscaledDeltaTime)
		{
			arrow.SetLocalPositionX(startX + this.arrowMoveXCurve.Evaluate(elapsed / this.arrowMoveXDuration) * (float)direction);
			yield return null;
		}
		this.onArrowAnimationEnd();
		this.arrowAnimationRoutine = null;
		yield break;
	}

	// Token: 0x06003DCC RID: 15820 RVA: 0x0010F8F4 File Offset: 0x0010DAF4
	private void InstantiateNeededItems(int count)
	{
		if (this.items == null)
		{
			this.items = new List<InventoryPaneListItem>(count);
		}
		this.template.gameObject.SetActive(true);
		int num = count - this.items.Count;
		for (int i = 0; i < num; i++)
		{
			InventoryPaneListItem inventoryPaneListItem = Object.Instantiate<InventoryPaneListItem>(this.template, this.template.transform.parent);
			this.items.Add(inventoryPaneListItem);
			inventoryPaneListItem.gameObject.name = string.Format("{0} ({1})", this.template.gameObject.name, this.items.Count);
		}
		this.template.gameObject.SetActive(false);
	}

	// Token: 0x06003DCD RID: 15821 RVA: 0x0010F9AE File Offset: 0x0010DBAE
	private void DoAnimationRoutine(IEnumerator enumerator, ref Coroutine routine, ref Action endAction)
	{
		if (routine != null)
		{
			base.StopCoroutine(routine);
		}
		if (endAction != null)
		{
			endAction();
		}
		routine = base.StartCoroutine(enumerator);
	}

	// Token: 0x04003F62 RID: 16226
	[SerializeField]
	private InventoryPaneListItem template;

	// Token: 0x04003F63 RID: 16227
	[SerializeField]
	private float itemSpacing = 2f;

	// Token: 0x04003F64 RID: 16228
	[SerializeField]
	private float arrowOffset = 1f;

	// Token: 0x04003F65 RID: 16229
	[SerializeField]
	private Transform leftArrow;

	// Token: 0x04003F66 RID: 16230
	[SerializeField]
	private Transform rightArrow;

	// Token: 0x04003F67 RID: 16231
	[Space]
	[SerializeField]
	private Transform leftArrowChild;

	// Token: 0x04003F68 RID: 16232
	[SerializeField]
	private Transform rightArrowChild;

	// Token: 0x04003F69 RID: 16233
	[SerializeField]
	private AnimationCurve arrowMoveXCurve;

	// Token: 0x04003F6A RID: 16234
	[SerializeField]
	private float arrowMoveXDuration = 0.2f;

	// Token: 0x04003F6B RID: 16235
	private List<InventoryPaneListItem> items;

	// Token: 0x04003F6C RID: 16236
	private Coroutine arrowAnimationRoutine;

	// Token: 0x04003F6D RID: 16237
	private Action onArrowAnimationEnd;
}
