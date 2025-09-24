using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001132 RID: 4402
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Gets the navigation mode of a UI Selectable component.")]
	public class UiNavigationGetMode : ComponentAction<Selectable>
	{
		// Token: 0x060076A1 RID: 30369 RVA: 0x00242C2C File Offset: 0x00240E2C
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060076A2 RID: 30370 RVA: 0x00242C38 File Offset: 0x00240E38
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.selectable = this.cachedComponent;
			}
			this.DoGetValue();
			base.Finish();
		}

		// Token: 0x060076A3 RID: 30371 RVA: 0x00242C78 File Offset: 0x00240E78
		private void DoGetValue()
		{
			if (this.selectable == null)
			{
				return;
			}
			this.navigationMode.Value = this.selectable.navigation.mode.ToString();
			if (this.selectable.navigation.mode == Navigation.Mode.None)
			{
				base.Fsm.Event(this.noNavigationEvent);
				return;
			}
			if (this.selectable.navigation.mode == Navigation.Mode.Automatic)
			{
				base.Fsm.Event(this.automaticEvent);
				return;
			}
			if (this.selectable.navigation.mode == Navigation.Mode.Vertical)
			{
				base.Fsm.Event(this.verticalEvent);
				return;
			}
			if (this.selectable.navigation.mode == Navigation.Mode.Horizontal)
			{
				base.Fsm.Event(this.horizontalEvent);
				return;
			}
			if (this.selectable.navigation.mode == Navigation.Mode.Explicit)
			{
				base.Fsm.Event(this.explicitEvent);
			}
		}

		// Token: 0x04007702 RID: 30466
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007703 RID: 30467
		[Tooltip("The navigation mode value")]
		public FsmString navigationMode;

		// Token: 0x04007704 RID: 30468
		[Tooltip("Event sent if transition is ColorTint")]
		public FsmEvent automaticEvent;

		// Token: 0x04007705 RID: 30469
		[Tooltip("Event sent if transition is ColorTint")]
		public FsmEvent horizontalEvent;

		// Token: 0x04007706 RID: 30470
		[Tooltip("Event sent if transition is SpriteSwap")]
		public FsmEvent verticalEvent;

		// Token: 0x04007707 RID: 30471
		[Tooltip("Event sent if transition is Animation")]
		public FsmEvent explicitEvent;

		// Token: 0x04007708 RID: 30472
		[Tooltip("Event sent if transition is none")]
		public FsmEvent noNavigationEvent;

		// Token: 0x04007709 RID: 30473
		private Selectable selectable;
	}
}
