using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D50 RID: 3408
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Rotation of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetRotationTimer : FsmStateAction
	{
		// Token: 0x060063DD RID: 25565 RVA: 0x001F7AAC File Offset: 0x001F5CAC
		public override void Reset()
		{
			this.gameObject = null;
			this.quaternion = null;
			this.vector = null;
			this.xAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.yAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.zAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.space = Space.World;
		}

		// Token: 0x060063DE RID: 25566 RVA: 0x001F7B0B File Offset: 0x001F5D0B
		public override void OnEnter()
		{
			this.timer = 0f;
			this.DoSetRotation();
		}

		// Token: 0x060063DF RID: 25567 RVA: 0x001F7B20 File Offset: 0x001F5D20
		public override void OnUpdate()
		{
			this.timer += Time.deltaTime;
			if (this.timer >= this.time.Value)
			{
				this.DoSetRotation();
				this.timer -= this.time.Value;
			}
		}

		// Token: 0x060063E0 RID: 25568 RVA: 0x001F7B70 File Offset: 0x001F5D70
		private void DoSetRotation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector;
			if (!this.quaternion.IsNone)
			{
				vector = this.quaternion.Value.eulerAngles;
			}
			else if (!this.vector.IsNone)
			{
				vector = this.vector.Value;
			}
			else
			{
				vector = ((this.space == Space.Self) ? ownerDefaultTarget.transform.localEulerAngles : ownerDefaultTarget.transform.eulerAngles);
			}
			if (!this.xAngle.IsNone)
			{
				vector.x = this.xAngle.Value;
			}
			if (!this.yAngle.IsNone)
			{
				vector.y = this.yAngle.Value;
			}
			if (!this.zAngle.IsNone)
			{
				vector.z = this.zAngle.Value;
			}
			if (this.space == Space.Self)
			{
				ownerDefaultTarget.transform.localEulerAngles = vector;
				return;
			}
			ownerDefaultTarget.transform.eulerAngles = vector;
		}

		// Token: 0x0400623A RID: 25146
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400623B RID: 25147
		[UIHint(UIHint.Variable)]
		[Tooltip("Use a stored quaternion, or vector angles below.")]
		public FsmQuaternion quaternion;

		// Token: 0x0400623C RID: 25148
		[UIHint(UIHint.Variable)]
		[Title("Euler Angles")]
		[Tooltip("Use euler angles stored in a Vector3 variable, and/or set each axis below.")]
		public FsmVector3 vector;

		// Token: 0x0400623D RID: 25149
		public FsmFloat xAngle;

		// Token: 0x0400623E RID: 25150
		public FsmFloat yAngle;

		// Token: 0x0400623F RID: 25151
		public FsmFloat zAngle;

		// Token: 0x04006240 RID: 25152
		public FsmFloat time;

		// Token: 0x04006241 RID: 25153
		[Tooltip("Use local or world space.")]
		public Space space;

		// Token: 0x04006242 RID: 25154
		private float timer;
	}
}
