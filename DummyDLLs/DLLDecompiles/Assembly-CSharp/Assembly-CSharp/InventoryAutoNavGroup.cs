using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200067D RID: 1661
public class InventoryAutoNavGroup : MonoBehaviour
{
	// Token: 0x06003B74 RID: 15220 RVA: 0x001055A8 File Offset: 0x001037A8
	public void Register(InventoryItemSelectable selectable)
	{
		this.selectables.AddIfNotPresent(selectable);
	}

	// Token: 0x06003B75 RID: 15221 RVA: 0x001055B7 File Offset: 0x001037B7
	public void Deregister(InventoryItemSelectable selectable)
	{
		this.selectables.Remove(selectable);
	}

	// Token: 0x06003B76 RID: 15222 RVA: 0x001055C6 File Offset: 0x001037C6
	public InventoryItemSelectable GetNextSelectable(InventoryItemSelectable currentSelected, InventoryItemManager.SelectionDirection direction, Func<InventoryItemSelectable, bool> predicate = null)
	{
		return this.GetNextSelectable<InventoryItemSelectable>(currentSelected, direction, predicate);
	}

	// Token: 0x06003B77 RID: 15223 RVA: 0x001055D4 File Offset: 0x001037D4
	public InventoryItemSelectable GetNextSelectable<T>(InventoryItemSelectable fromSelectable, InventoryItemManager.SelectionDirection direction, Func<T, bool> predicate = null) where T : InventoryItemSelectable
	{
		Vector2 b = fromSelectable.transform.TransformPoint(fromSelectable.NavigationOffset);
		float directionAngle = this.GetDirectionAngle(direction);
		Vector2 vector;
		switch (direction)
		{
		case InventoryItemManager.SelectionDirection.Up:
			vector = new Vector2(1.15f, 1f);
			break;
		case InventoryItemManager.SelectionDirection.Down:
			vector = new Vector2(1.15f, 1f);
			break;
		case InventoryItemManager.SelectionDirection.Left:
			vector = new Vector2(1f, 1.15f);
			break;
		case InventoryItemManager.SelectionDirection.Right:
			vector = new Vector2(1f, 1.15f);
			break;
		default:
			throw new ArgumentOutOfRangeException("direction", direction, null);
		}
		Vector2 b2 = vector;
		InventoryItemSelectable result = null;
		float num = float.MaxValue;
		foreach (InventoryItemSelectable inventoryItemSelectable in this.selectables)
		{
			if (!(inventoryItemSelectable == fromSelectable))
			{
				T t = inventoryItemSelectable as T;
				if (t && (predicate == null || predicate(t)))
				{
					Vector2 a = inventoryItemSelectable.transform.TransformPoint(inventoryItemSelectable.NavigationOffset) - b;
					if (Vector2.SignedAngle(Vector2.right, a.normalized).IsAngleWithinTolerance(67.5f, directionAngle))
					{
						float magnitude = (a * b2).magnitude;
						if (magnitude < num)
						{
							num = magnitude;
							result = inventoryItemSelectable;
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06003B78 RID: 15224 RVA: 0x00105768 File Offset: 0x00103968
	private float GetDirectionAngle(InventoryItemManager.SelectionDirection direction)
	{
		switch (direction)
		{
		case InventoryItemManager.SelectionDirection.Up:
			return 90f;
		case InventoryItemManager.SelectionDirection.Down:
			return 270f;
		case InventoryItemManager.SelectionDirection.Left:
			return 180f;
		case InventoryItemManager.SelectionDirection.Right:
			return 0f;
		default:
			throw new NotImplementedException();
		}
	}

	// Token: 0x04003DAF RID: 15791
	private readonly List<InventoryItemSelectable> selectables = new List<InventoryItemSelectable>();
}
