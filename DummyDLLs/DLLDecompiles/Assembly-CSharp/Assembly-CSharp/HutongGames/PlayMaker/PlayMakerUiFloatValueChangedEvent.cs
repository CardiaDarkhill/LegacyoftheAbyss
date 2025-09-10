using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AEF RID: 2799
	[AddComponentMenu("PlayMaker/UI/UI Float Value Changed Event")]
	public class PlayMakerUiFloatValueChangedEvent : PlayMakerUiEventBase
	{
		// Token: 0x060058DF RID: 22751 RVA: 0x001C3480 File Offset: 0x001C1680
		protected override void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			this.initialized = true;
			if (this.slider == null)
			{
				this.slider = base.GetComponent<Slider>();
			}
			if (this.slider != null)
			{
				this.slider.onValueChanged.AddListener(new UnityAction<float>(this.OnValueChanged));
			}
			if (this.scrollbar == null)
			{
				this.scrollbar = base.GetComponent<Scrollbar>();
			}
			if (this.scrollbar != null)
			{
				this.scrollbar.onValueChanged.AddListener(new UnityAction<float>(this.OnValueChanged));
			}
		}

		// Token: 0x060058E0 RID: 22752 RVA: 0x001C3528 File Offset: 0x001C1728
		protected void OnDisable()
		{
			this.initialized = false;
			if (this.slider != null)
			{
				this.slider.onValueChanged.RemoveListener(new UnityAction<float>(this.OnValueChanged));
			}
			if (this.scrollbar != null)
			{
				this.scrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.OnValueChanged));
			}
		}

		// Token: 0x060058E1 RID: 22753 RVA: 0x001C3590 File Offset: 0x001C1790
		private void OnValueChanged(float value)
		{
			Fsm.EventData.FloatData = value;
			base.SendEvent(FsmEvent.UiFloatValueChanged);
		}

		// Token: 0x04005419 RID: 21529
		public Slider slider;

		// Token: 0x0400541A RID: 21530
		public Scrollbar scrollbar;
	}
}
