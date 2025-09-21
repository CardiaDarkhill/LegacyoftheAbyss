using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HutongGames.PlayMaker
{
	// Token: 0x02000AF0 RID: 2800
	[AddComponentMenu("PlayMaker/UI/UI Int Value Changed Event")]
	public class PlayMakerUiIntValueChangedEvent : PlayMakerUiEventBase
	{
		// Token: 0x060058E3 RID: 22755 RVA: 0x001C35B0 File Offset: 0x001C17B0
		protected override void Initialize()
		{
			if (this.initialized)
			{
				return;
			}
			this.initialized = true;
			if (this.dropdown == null)
			{
				this.dropdown = base.GetComponent<Dropdown>();
			}
			if (this.dropdown != null)
			{
				this.dropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnValueChanged));
			}
		}

		// Token: 0x060058E4 RID: 22756 RVA: 0x001C3611 File Offset: 0x001C1811
		protected void OnDisable()
		{
			this.initialized = false;
			if (this.dropdown != null)
			{
				this.dropdown.onValueChanged.RemoveListener(new UnityAction<int>(this.OnValueChanged));
			}
		}

		// Token: 0x060058E5 RID: 22757 RVA: 0x001C3644 File Offset: 0x001C1844
		private void OnValueChanged(int value)
		{
			Fsm.EventData.IntData = value;
			base.SendEvent(FsmEvent.UiIntValueChanged);
		}

		// Token: 0x0400541B RID: 21531
		public Dropdown dropdown;
	}
}
