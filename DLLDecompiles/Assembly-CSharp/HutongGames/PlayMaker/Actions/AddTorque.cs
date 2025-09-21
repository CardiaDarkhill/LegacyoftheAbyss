using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F9B RID: 3995
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Adds torque (rotational force) to a Game Object. NOTE: The game object requires a Rigid Body.")]
	public class AddTorque : ComponentAction<Rigidbody>
	{
		// Token: 0x06006E58 RID: 28248 RVA: 0x00222E2C File Offset: 0x0022102C
		public override void Reset()
		{
			this.gameObject = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.z = new FsmFloat
			{
				UseVariable = true
			};
			this.space = Space.World;
			this.forceMode = ForceMode.Force;
			this.everyFrame = false;
		}

		// Token: 0x06006E59 RID: 28249 RVA: 0x00222E8B File Offset: 0x0022108B
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006E5A RID: 28250 RVA: 0x00222E99 File Offset: 0x00221099
		public override void OnEnter()
		{
			this.DoAddTorque();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006E5B RID: 28251 RVA: 0x00222EAF File Offset: 0x002210AF
		public override void OnFixedUpdate()
		{
			this.DoAddTorque();
		}

		// Token: 0x06006E5C RID: 28252 RVA: 0x00222EB8 File Offset: 0x002210B8
		private void DoAddTorque()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector3 torque = this.vector.IsNone ? new Vector3(this.x.Value, this.y.Value, this.z.Value) : this.vector.Value;
			if (!this.x.IsNone)
			{
				torque.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				torque.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				torque.z = this.z.Value;
			}
			if (this.space == Space.World)
			{
				base.rigidbody.AddTorque(torque, this.forceMode);
				return;
			}
			base.rigidbody.AddRelativeTorque(torque, this.forceMode);
		}

		// Token: 0x04006E03 RID: 28163
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The GameObject to add torque (rotational force) to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006E04 RID: 28164
		[UIHint(UIHint.Variable)]
		[Tooltip("A Vector3 torque. Optionally override any axis with the X, Y, Z parameters.")]
		public FsmVector3 vector;

		// Token: 0x04006E05 RID: 28165
		[Tooltip("Torque around the X axis. To leave unchanged, set to 'None'.")]
		public FsmFloat x;

		// Token: 0x04006E06 RID: 28166
		[Tooltip("Torque around the Y axis. To leave unchanged, set to 'None'.")]
		public FsmFloat y;

		// Token: 0x04006E07 RID: 28167
		[Tooltip("Torque around the Z axis. To leave unchanged, set to 'None'.")]
		public FsmFloat z;

		// Token: 0x04006E08 RID: 28168
		[Tooltip("Apply the force in world or local space.")]
		public Space space;

		// Token: 0x04006E09 RID: 28169
		[Tooltip("The type of force to apply. See Unity Physics docs.")]
		public ForceMode forceMode;

		// Token: 0x04006E0A RID: 28170
		[Tooltip("Apply the force every frame that the State is active.")]
		public bool everyFrame;
	}
}
