using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E59 RID: 3673
	[ActionCategory(ActionCategory.Character)]
	[Tooltip("Modify various character controller settings.\n'None' leaves the setting unchanged.")]
	public class ControllerSettings : ComponentAction<CharacterController>
	{
		// Token: 0x17000BEB RID: 3051
		// (get) Token: 0x060068E9 RID: 26857 RVA: 0x0020F2A3 File Offset: 0x0020D4A3
		private CharacterController controller
		{
			get
			{
				return this.cachedComponent;
			}
		}

		// Token: 0x060068EA RID: 26858 RVA: 0x0020F2AC File Offset: 0x0020D4AC
		public override void Reset()
		{
			this.gameObject = null;
			this.height = new FsmFloat
			{
				UseVariable = true
			};
			this.radius = new FsmFloat
			{
				UseVariable = true
			};
			this.slopeLimit = new FsmFloat
			{
				UseVariable = true
			};
			this.stepOffset = new FsmFloat
			{
				UseVariable = true
			};
			this.center = new FsmVector3
			{
				UseVariable = true
			};
			this.detectCollisions = new FsmBool
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x060068EB RID: 26859 RVA: 0x0020F333 File Offset: 0x0020D533
		public override void OnEnter()
		{
			this.DoControllerSettings();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068EC RID: 26860 RVA: 0x0020F349 File Offset: 0x0020D549
		public override void OnUpdate()
		{
			this.DoControllerSettings();
		}

		// Token: 0x060068ED RID: 26861 RVA: 0x0020F354 File Offset: 0x0020D554
		private void DoControllerSettings()
		{
			if (!base.UpdateCache(base.Fsm.GetOwnerDefaultTarget(this.gameObject)))
			{
				return;
			}
			if (!this.height.IsNone)
			{
				this.controller.height = this.height.Value;
			}
			if (!this.radius.IsNone)
			{
				this.controller.radius = this.radius.Value;
			}
			if (!this.slopeLimit.IsNone)
			{
				this.controller.slopeLimit = this.slopeLimit.Value;
			}
			if (!this.stepOffset.IsNone)
			{
				this.controller.stepOffset = this.stepOffset.Value;
			}
			if (!this.center.IsNone)
			{
				this.controller.center = this.center.Value;
			}
			if (!this.detectCollisions.IsNone)
			{
				this.controller.detectCollisions = this.detectCollisions.Value;
			}
		}

		// Token: 0x04006833 RID: 26675
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject that owns the CharacterController.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006834 RID: 26676
		[Tooltip("The height of the character's capsule.")]
		public FsmFloat height;

		// Token: 0x04006835 RID: 26677
		[Tooltip("The radius of the character's capsule.")]
		public FsmFloat radius;

		// Token: 0x04006836 RID: 26678
		[Tooltip("The character controllers slope limit in degrees.")]
		public FsmFloat slopeLimit;

		// Token: 0x04006837 RID: 26679
		[Tooltip("The character controllers step offset in meters.")]
		public FsmFloat stepOffset;

		// Token: 0x04006838 RID: 26680
		[Tooltip("The center of the character's capsule relative to the transform's position")]
		public FsmVector3 center;

		// Token: 0x04006839 RID: 26681
		[Tooltip("Should other RigidBodies or CharacterControllers collide with this character controller (By default always enabled).")]
		public FsmBool detectCollisions;

		// Token: 0x0400683A RID: 26682
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
