using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C2B RID: 3115
	[ActionCategory("Enemy AI")]
	[Tooltip("Object rotates to face direction it is travelling in.")]
	public class FaceAngle : RigidBody2dActionBase
	{
		// Token: 0x06005EC9 RID: 24265 RVA: 0x001DFFBC File Offset: 0x001DE1BC
		public override void Reset()
		{
			this.gameObject = null;
			this.angleOffset = 0f;
			this.everyFrame = false;
			this.otherGameObject = null;
		}

		// Token: 0x06005ECA RID: 24266 RVA: 0x001DFFE4 File Offset: 0x001DE1E4
		public override void OnEnter()
		{
			base.CacheRigidBody2d(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			this.target = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.DoAngle();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005ECB RID: 24267 RVA: 0x001E0033 File Offset: 0x001DE233
		public override void OnUpdate()
		{
			this.DoAngle();
		}

		// Token: 0x06005ECC RID: 24268 RVA: 0x001E003C File Offset: 0x001DE23C
		private void DoAngle()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			float value;
			if (this.angleOffsetIfMirrored.IsNone || this.target.transform.localScale.x > 0f)
			{
				value = this.angleOffset.Value;
			}
			else
			{
				value = this.angleOffsetIfMirrored.Value;
			}
			float z = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * 57.295776f + value;
			if (!this.otherGameObject.IsNone && this.otherGameObject.Value != null)
			{
				this.otherGameObject.Value.transform.localEulerAngles = new Vector3(0f, 0f, z);
				return;
			}
			this.target.transform.localEulerAngles = new Vector3(0f, 0f, z);
		}

		// Token: 0x04005B6A RID: 23402
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005B6B RID: 23403
		[Tooltip("Offset the angle. If sprite faces right, leave as 0.")]
		public FsmFloat angleOffset;

		// Token: 0x04005B6C RID: 23404
		public FsmFloat angleOffsetIfMirrored;

		// Token: 0x04005B6D RID: 23405
		[Tooltip("Optionally, rotate another object based on our own velocity")]
		public FsmGameObject otherGameObject;

		// Token: 0x04005B6E RID: 23406
		public bool everyFrame;

		// Token: 0x04005B6F RID: 23407
		private GameObject target;
	}
}
