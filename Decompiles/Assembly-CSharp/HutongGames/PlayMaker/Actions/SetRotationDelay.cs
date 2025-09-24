using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D4F RID: 3407
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Sets the Rotation of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	public class SetRotationDelay : FsmStateAction
	{
		// Token: 0x060063D8 RID: 25560 RVA: 0x001F78F4 File Offset: 0x001F5AF4
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
			this.delay = null;
		}

		// Token: 0x060063D9 RID: 25561 RVA: 0x001F795A File Offset: 0x001F5B5A
		public override void OnEnter()
		{
			this.timer = 0f;
		}

		// Token: 0x060063DA RID: 25562 RVA: 0x001F7967 File Offset: 0x001F5B67
		public override void OnUpdate()
		{
			if (this.timer < this.delay.Value)
			{
				this.timer += Time.deltaTime;
				return;
			}
			this.DoSetRotation();
			base.Finish();
		}

		// Token: 0x060063DB RID: 25563 RVA: 0x001F799C File Offset: 0x001F5B9C
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

		// Token: 0x04006231 RID: 25137
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006232 RID: 25138
		[UIHint(UIHint.Variable)]
		[Tooltip("Use a stored quaternion, or vector angles below.")]
		public FsmQuaternion quaternion;

		// Token: 0x04006233 RID: 25139
		[UIHint(UIHint.Variable)]
		[Title("Euler Angles")]
		[Tooltip("Use euler angles stored in a Vector3 variable, and/or set each axis below.")]
		public FsmVector3 vector;

		// Token: 0x04006234 RID: 25140
		public FsmFloat xAngle;

		// Token: 0x04006235 RID: 25141
		public FsmFloat yAngle;

		// Token: 0x04006236 RID: 25142
		public FsmFloat zAngle;

		// Token: 0x04006237 RID: 25143
		[Tooltip("Use local or world space.")]
		public Space space;

		// Token: 0x04006238 RID: 25144
		public FsmFloat delay;

		// Token: 0x04006239 RID: 25145
		private float timer;
	}
}
