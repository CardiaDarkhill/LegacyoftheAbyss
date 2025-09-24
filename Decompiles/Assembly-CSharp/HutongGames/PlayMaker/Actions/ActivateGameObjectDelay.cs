using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B9E RID: 2974
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Activates/deactivates a Game Object. Use this to hide/show areas, or enable/disable many Behaviours at once.")]
	public class ActivateGameObjectDelay : FsmStateAction
	{
		// Token: 0x06005BED RID: 23533 RVA: 0x001CEA1D File Offset: 0x001CCC1D
		public override void Reset()
		{
			this.gameObject = null;
			this.activate = true;
			this.resetOnExit = false;
			this.delay = null;
			this.timer = 0f;
		}

		// Token: 0x06005BEE RID: 23534 RVA: 0x001CEA4B File Offset: 0x001CCC4B
		public override void OnEnter()
		{
			if (this.delay.Value <= 0f)
			{
				this.DoActivateGameObject();
				return;
			}
			this.timer = this.delay.Value;
		}

		// Token: 0x06005BEF RID: 23535 RVA: 0x001CEA77 File Offset: 0x001CCC77
		public override void OnUpdate()
		{
			if (this.timer > 0f)
			{
				this.timer -= Time.deltaTime;
				return;
			}
			this.DoActivateGameObject();
		}

		// Token: 0x06005BF0 RID: 23536 RVA: 0x001CEA9F File Offset: 0x001CCC9F
		public override void OnExit()
		{
			if (this.activatedGameObject == null)
			{
				return;
			}
			if (this.resetOnExit)
			{
				this.activatedGameObject.SetActive(this.initialActiveState);
			}
		}

		// Token: 0x06005BF1 RID: 23537 RVA: 0x001CEACC File Offset: 0x001CCCCC
		private void DoActivateGameObject()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.initialActiveState = ownerDefaultTarget.activeSelf;
			ownerDefaultTarget.SetActive(this.activate.Value);
			this.activatedGameObject = ownerDefaultTarget;
			base.Finish();
		}

		// Token: 0x0400574E RID: 22350
		[RequiredField]
		[Tooltip("The GameObject to activate/deactivate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400574F RID: 22351
		[RequiredField]
		[Tooltip("Check to activate, uncheck to deactivate Game Object.")]
		public FsmBool activate;

		// Token: 0x04005750 RID: 22352
		[Tooltip("Reset the game objects when exiting this state. Useful if you want an object to be active only while this state is active.\nNote: Only applies to the last Game Object activated/deactivated (won't work if Game Object changes).")]
		public bool resetOnExit;

		// Token: 0x04005751 RID: 22353
		public FsmFloat delay;

		// Token: 0x04005752 RID: 22354
		private float timer;

		// Token: 0x04005753 RID: 22355
		private GameObject activatedGameObject;

		// Token: 0x04005754 RID: 22356
		private bool initialActiveState;
	}
}
