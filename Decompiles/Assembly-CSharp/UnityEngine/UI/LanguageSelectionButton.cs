using System;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
	// Token: 0x0200086E RID: 2158
	public class LanguageSelectionButton : MenuButton, ISubmitHandler, IEventSystemHandler, IPointerClickHandler
	{
		// Token: 0x170008E6 RID: 2278
		// (get) Token: 0x06004AE7 RID: 19175 RVA: 0x00162CB4 File Offset: 0x00160EB4
		public string Language
		{
			get
			{
				return this.language;
			}
		}

		// Token: 0x06004AE8 RID: 19176 RVA: 0x00162CBC File Offset: 0x00160EBC
		protected override void Awake()
		{
			base.Awake();
			if (!this.languageSelector)
			{
				this.languageSelector = base.GetComponentInParent<LanguageSelector>();
			}
		}

		// Token: 0x06004AE9 RID: 19177 RVA: 0x00162CDD File Offset: 0x00160EDD
		public new void OnSubmit(BaseEventData eventData)
		{
			if (!base.interactable)
			{
				return;
			}
			base.OnSubmit(eventData);
			if (this.languageSelector)
			{
				this.languageSelector.SetLanguage(this);
			}
		}

		// Token: 0x06004AEA RID: 19178 RVA: 0x00162D08 File Offset: 0x00160F08
		public new void OnPointerClick(PointerEventData eventData)
		{
			this.OnSubmit(eventData);
		}

		// Token: 0x04004C93 RID: 19603
		[Header("Language")]
		[SerializeField]
		private string language;

		// Token: 0x04004C94 RID: 19604
		[SerializeField]
		private LanguageSelector languageSelector;
	}
}
