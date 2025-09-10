using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AEA RID: 2794
	[AddComponentMenu("PlayMaker/UI/UI Click Event")]
	public class PlayMakerUiClickEvent : PlayMakerUiEventBase
	{
		// Token: 0x060058CA RID: 22730 RVA: 0x001C31F8 File Offset: 0x001C13F8
		protected override void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			this.initialized = true;
			if (this.button == null)
			{
				this.button = base.GetComponent<Button>();
			}
			if (this.button != null)
			{
				this.button.onClick.AddListener(new UnityAction(this.DoOnClick));
			}
		}

		// Token: 0x060058CB RID: 22731 RVA: 0x001C3259 File Offset: 0x001C1459
		protected void OnDisable()
		{
			this.initialized = false;
			if (this.button != null)
			{
				this.button.onClick.RemoveListener(new UnityAction(this.DoOnClick));
			}
		}

		// Token: 0x060058CC RID: 22732 RVA: 0x001C328C File Offset: 0x001C148C
		private void DoOnClick()
		{
			base.SendEvent(FsmEvent.UiClick);
		}

		// Token: 0x04005415 RID: 21525
		public Button button;
	}
}
