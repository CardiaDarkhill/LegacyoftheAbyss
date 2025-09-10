using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x0200087B RID: 2171
	public class MenuStyleSetting : MenuOptionHorizontal, IMoveHandler, IEventSystemHandler, IPointerClickHandler, ISubmitHandler
	{
		// Token: 0x06004B8E RID: 19342 RVA: 0x00164F18 File Offset: 0x00163118
		private new void OnEnable()
		{
			this.styles = MenuStyles.Instance;
			if (!this.styles || this.styles.Styles.Length == 0)
			{
				return;
			}
			this.indexList.Clear();
			List<string> list = new List<string>();
			for (int i = 0; i < this.styles.Styles.Length; i++)
			{
				MenuStyles.MenuStyle menuStyle = this.styles.Styles[i];
				if (menuStyle.IsAvailable)
				{
					list.Add(menuStyle.DisplayName);
					this.indexList.Add(i);
				}
			}
			this.optionList = list.ToArray();
			this.selectedOptionIndex = this.indexList.IndexOf(Mathf.Min(new int[]
			{
				this.styles.CurrentStyle
			}));
			if (this.selectedOptionIndex < 0)
			{
				this.selectedOptionIndex = 0;
			}
			this.UpdateText();
		}

		// Token: 0x06004B8F RID: 19343 RVA: 0x00164FEF File Offset: 0x001631EF
		public new void OnMove(AxisEventData move)
		{
			if (!base.interactable)
			{
				return;
			}
			if (base.MoveOption(move.moveDir))
			{
				this.UpdateStyle();
				return;
			}
			base.OnMove(move);
		}

		// Token: 0x06004B90 RID: 19344 RVA: 0x00165016 File Offset: 0x00163216
		public new void OnPointerClick(PointerEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			base.PointerClickCheckArrows(eventData);
			this.UpdateStyle();
		}

		// Token: 0x06004B91 RID: 19345 RVA: 0x0016502E File Offset: 0x0016322E
		public new void OnSubmit(BaseEventData eventData)
		{
			base.MoveOption(MoveDirection.Right);
			this.UpdateStyle();
		}

		// Token: 0x06004B92 RID: 19346 RVA: 0x00165040 File Offset: 0x00163240
		private void UpdateStyle()
		{
			if (this.styles)
			{
				if (this.indexList.Count <= 0)
				{
					return;
				}
				this.selectedOptionIndex = Mathf.Clamp(this.selectedOptionIndex, 0, this.indexList.Count - 1);
				this.styles.SetStyle(this.indexList[this.selectedOptionIndex], true, true);
			}
		}

		// Token: 0x04004CF4 RID: 19700
		private MenuStyles styles;

		// Token: 0x04004CF5 RID: 19701
		private readonly List<int> indexList = new List<int>();
	}
}
