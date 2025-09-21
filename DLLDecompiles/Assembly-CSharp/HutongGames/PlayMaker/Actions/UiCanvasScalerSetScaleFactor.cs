using System;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001112 RID: 4370
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the ScaleFactor of a CanvasScaler.")]
	public class UiCanvasScalerSetScaleFactor : ComponentAction<CanvasScaler>
	{
		// Token: 0x06007619 RID: 30233 RVA: 0x00240E82 File Offset: 0x0023F082
		public override void Reset()
		{
			this.gameObject = null;
			this.scaleFactor = null;
			this.everyFrame = false;
		}

		// Token: 0x0600761A RID: 30234 RVA: 0x00240E9C File Offset: 0x0023F09C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.component = this.cachedComponent;
			}
			this.DoSetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600761B RID: 30235 RVA: 0x00240EE4 File Offset: 0x0023F0E4
		public override void OnUpdate()
		{
			this.DoSetValue();
		}

		// Token: 0x0600761C RID: 30236 RVA: 0x00240EEC File Offset: 0x0023F0EC
		private void DoSetValue()
		{
			if (this.component != null)
			{
				this.component.scaleFactor = this.scaleFactor.Value;
			}
		}

		// Token: 0x04007681 RID: 30337
		[RequiredField]
		[CheckForComponent(typeof(CanvasScaler))]
		[Tooltip("The GameObject with a UI CanvasScaler component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007682 RID: 30338
		[RequiredField]
		[Tooltip("The scaleFactor of the UI CanvasScaler.")]
		public FsmFloat scaleFactor;

		// Token: 0x04007683 RID: 30339
		[Tooltip("Repeats every frame, useful for animation")]
		public bool everyFrame;

		// Token: 0x04007684 RID: 30340
		private CanvasScaler component;
	}
}
