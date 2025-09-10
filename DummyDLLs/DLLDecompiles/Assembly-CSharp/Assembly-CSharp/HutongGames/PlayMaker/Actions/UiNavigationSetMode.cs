using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001133 RID: 4403
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the navigation mode of a UI Selectable component.")]
	public class UiNavigationSetMode : ComponentAction<Selectable>
	{
		// Token: 0x060076A5 RID: 30373 RVA: 0x00242D8E File Offset: 0x00240F8E
		public override void Reset()
		{
			this.gameObject = null;
			this.navigationMode = Navigation.Mode.Automatic;
			this.resetOnExit = false;
		}

		// Token: 0x060076A6 RID: 30374 RVA: 0x00242DAC File Offset: 0x00240FAC
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.selectable = this.cachedComponent;
			}
			if (this.selectable != null && this.resetOnExit.Value)
			{
				this.originalValue = this.selectable.navigation.mode;
			}
			this.DoSetValue();
			base.Finish();
		}

		// Token: 0x060076A7 RID: 30375 RVA: 0x00242E20 File Offset: 0x00241020
		private void DoSetValue()
		{
			if (this.selectable != null)
			{
				this._navigation = this.selectable.navigation;
				this._navigation.mode = this.navigationMode;
				this.selectable.navigation = this._navigation;
			}
		}

		// Token: 0x060076A8 RID: 30376 RVA: 0x00242E70 File Offset: 0x00241070
		public override void OnExit()
		{
			if (this.selectable == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this._navigation = this.selectable.navigation;
				this._navigation.mode = this.originalValue;
				this.selectable.navigation = this._navigation;
			}
		}

		// Token: 0x0400770A RID: 30474
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400770B RID: 30475
		[Tooltip("The navigation mode value")]
		public Navigation.Mode navigationMode;

		// Token: 0x0400770C RID: 30476
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x0400770D RID: 30477
		private Selectable selectable;

		// Token: 0x0400770E RID: 30478
		private Navigation _navigation;

		// Token: 0x0400770F RID: 30479
		private Navigation.Mode originalValue;
	}
}
