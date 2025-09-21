using System;
using UnityEngine;
using UnityEngine.UI;

namespace TeamCherry.DebugMenu
{
	// Token: 0x020008BC RID: 2236
	public class DebugMenuGridLayoutHelper : MonoBehaviour
	{
		// Token: 0x06004D84 RID: 19844 RVA: 0x0016C17E File Offset: 0x0016A37E
		private void OnValidate()
		{
			if (!this.gridLayoutGroup)
			{
				this.gridLayoutGroup = base.GetComponent<GridLayoutGroup>();
			}
		}

		// Token: 0x06004D85 RID: 19845 RVA: 0x0016C19C File Offset: 0x0016A39C
		[ContextMenu("Apply Layout")]
		private void CalculateLayout()
		{
			if (!this.gridLayoutGroup)
			{
				return;
			}
			base.gameObject.RecordUndoChanges("Apply Layout");
			if (this.fitInRect)
			{
				Rect rect = (this.gridLayoutGroup.transform as RectTransform).rect;
				this.width = rect.width - (float)this.gridLayoutGroup.padding.left - (float)this.gridLayoutGroup.padding.right;
				this.height = rect.height - (float)this.gridLayoutGroup.padding.top - (float)this.gridLayoutGroup.padding.bottom;
			}
			if (this.targetColumns > 0)
			{
				float num = this.width - this.spacing.x * (float)(this.targetColumns - 1);
				this.cellSize.x = num / (float)this.targetColumns;
			}
			if (this.targetRows > 0)
			{
				float num2 = this.height - this.spacing.y * (float)(this.targetRows - 1);
				this.cellSize.y = num2 / (float)this.targetRows;
			}
			this.gridLayoutGroup.spacing = this.spacing;
			this.gridLayoutGroup.cellSize = this.cellSize;
			base.gameObject.ApplyPrefabInstanceModifications();
		}

		// Token: 0x04004E43 RID: 20035
		[SerializeField]
		private GridLayoutGroup gridLayoutGroup;

		// Token: 0x04004E44 RID: 20036
		[Header("Settings")]
		[SerializeField]
		private bool fitInRect;

		// Token: 0x04004E45 RID: 20037
		[Space]
		[SerializeField]
		private float width;

		// Token: 0x04004E46 RID: 20038
		[SerializeField]
		private float height;

		// Token: 0x04004E47 RID: 20039
		[Space]
		private Vector2 cellSize;

		// Token: 0x04004E48 RID: 20040
		[Space]
		[SerializeField]
		private Vector2 spacing;

		// Token: 0x04004E49 RID: 20041
		[Space]
		[SerializeField]
		private int targetRows = -1;

		// Token: 0x04004E4A RID: 20042
		[SerializeField]
		private int targetColumns = -1;
	}
}
