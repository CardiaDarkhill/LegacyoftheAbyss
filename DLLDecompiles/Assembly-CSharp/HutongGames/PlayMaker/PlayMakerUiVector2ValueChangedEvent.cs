using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AF2 RID: 2802
	[AddComponentMenu("PlayMaker/UI/UI Vector2 Value Changed Event")]
	public class PlayMakerUiVector2ValueChangedEvent : PlayMakerUiEventBase
	{
		// Token: 0x060058ED RID: 22765 RVA: 0x001C36CC File Offset: 0x001C18CC
		protected override void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			this.initialized = true;
			if (this.scrollRect == null)
			{
				this.scrollRect = base.GetComponent<ScrollRect>();
			}
			if (this.scrollRect != null)
			{
				this.scrollRect.onValueChanged.AddListener(new UnityAction<Vector2>(this.OnValueChanged));
			}
		}

		// Token: 0x060058EE RID: 22766 RVA: 0x001C372D File Offset: 0x001C192D
		protected void OnDisable()
		{
			this.initialized = false;
			if (this.scrollRect != null)
			{
				this.scrollRect.onValueChanged.RemoveListener(new UnityAction<Vector2>(this.OnValueChanged));
			}
		}

		// Token: 0x060058EF RID: 22767 RVA: 0x001C3760 File Offset: 0x001C1960
		private void OnValueChanged(Vector2 value)
		{
			Fsm.EventData.Vector2Data = value;
			base.SendEvent(FsmEvent.UiVector2ValueChanged);
		}

		// Token: 0x0400541C RID: 21532
		public ScrollRect scrollRect;
	}
}
