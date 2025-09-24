using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200112F RID: 4399
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the explicit navigation properties of a UI Selectable component. Note that it will have no effect until Navigation mode is set to 'Explicit'.")]
	public class UiNavigationExplicitSetProperties : ComponentAction<Selectable>
	{
		// Token: 0x06007692 RID: 30354 RVA: 0x00242748 File Offset: 0x00240948
		public override void Reset()
		{
			this.gameObject = null;
			this.selectOnDown = new FsmGameObject
			{
				UseVariable = true
			};
			this.selectOnUp = new FsmGameObject
			{
				UseVariable = true
			};
			this.selectOnLeft = new FsmGameObject
			{
				UseVariable = true
			};
			this.selectOnRight = new FsmGameObject
			{
				UseVariable = true
			};
			this.resetOnExit = false;
		}

		// Token: 0x06007693 RID: 30355 RVA: 0x002427B0 File Offset: 0x002409B0
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.selectable = this.cachedComponent;
			}
			if (this.selectable != null && this.resetOnExit.Value)
			{
				this.originalState = this.selectable.navigation;
			}
			this.DoSetValue();
			base.Finish();
		}

		// Token: 0x06007694 RID: 30356 RVA: 0x0024281C File Offset: 0x00240A1C
		private void DoSetValue()
		{
			if (this.selectable == null)
			{
				return;
			}
			this.navigation = this.selectable.navigation;
			if (!this.selectOnDown.IsNone)
			{
				this.navigation.selectOnDown = UiNavigationExplicitSetProperties.GetComponentFromFsmGameObject<Selectable>(this.selectOnDown);
			}
			if (!this.selectOnUp.IsNone)
			{
				this.navigation.selectOnUp = UiNavigationExplicitSetProperties.GetComponentFromFsmGameObject<Selectable>(this.selectOnUp);
			}
			if (!this.selectOnLeft.IsNone)
			{
				this.navigation.selectOnLeft = UiNavigationExplicitSetProperties.GetComponentFromFsmGameObject<Selectable>(this.selectOnLeft);
			}
			if (!this.selectOnRight.IsNone)
			{
				this.navigation.selectOnRight = UiNavigationExplicitSetProperties.GetComponentFromFsmGameObject<Selectable>(this.selectOnRight);
			}
			this.selectable.navigation = this.navigation;
		}

		// Token: 0x06007695 RID: 30357 RVA: 0x002428E8 File Offset: 0x00240AE8
		public override void OnExit()
		{
			if (this.selectable == null)
			{
				return;
			}
			if (this.resetOnExit.Value)
			{
				this.navigation = this.selectable.navigation;
				this.navigation.selectOnDown = this.originalState.selectOnDown;
				this.navigation.selectOnLeft = this.originalState.selectOnLeft;
				this.navigation.selectOnRight = this.originalState.selectOnRight;
				this.navigation.selectOnUp = this.originalState.selectOnUp;
				this.selectable.navigation = this.navigation;
			}
		}

		// Token: 0x06007696 RID: 30358 RVA: 0x0024298C File Offset: 0x00240B8C
		private static T GetComponentFromFsmGameObject<T>(FsmGameObject variable) where T : Component
		{
			if (variable.Value != null)
			{
				return variable.Value.GetComponent<T>();
			}
			return default(T);
		}

		// Token: 0x040076EB RID: 30443
		[RequiredField]
		[CheckForComponent(typeof(Selectable))]
		[Tooltip("The GameObject with the UI Selectable component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040076EC RID: 30444
		[Tooltip("The down Selectable. Leave as None for no effect")]
		[CheckForComponent(typeof(Selectable))]
		public FsmGameObject selectOnDown;

		// Token: 0x040076ED RID: 30445
		[Tooltip("The up Selectable.  Leave as None for no effect")]
		[CheckForComponent(typeof(Selectable))]
		public FsmGameObject selectOnUp;

		// Token: 0x040076EE RID: 30446
		[Tooltip("The left Selectable.  Leave as None for no effect")]
		[CheckForComponent(typeof(Selectable))]
		public FsmGameObject selectOnLeft;

		// Token: 0x040076EF RID: 30447
		[Tooltip("The right Selectable.  Leave as None for no effect")]
		[CheckForComponent(typeof(Selectable))]
		public FsmGameObject selectOnRight;

		// Token: 0x040076F0 RID: 30448
		[Tooltip("Reset when exiting this state.")]
		public FsmBool resetOnExit;

		// Token: 0x040076F1 RID: 30449
		private Selectable selectable;

		// Token: 0x040076F2 RID: 30450
		private Navigation navigation;

		// Token: 0x040076F3 RID: 30451
		private Navigation originalState;
	}
}
