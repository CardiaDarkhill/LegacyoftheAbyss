using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C2C RID: 3116
	[ActionCategory("Enemy AI")]
	[Tooltip("Object rotates to face direction it is travelling in.")]
	public class FaceAngleV2 : RigidBody2dActionBase
	{
		// Token: 0x06005ECE RID: 24270 RVA: 0x001E012F File Offset: 0x001DE32F
		public override void Reset()
		{
			this.gameObject = null;
			this.angleOffset = 0f;
			this.everyFrame = false;
		}

		// Token: 0x06005ECF RID: 24271 RVA: 0x001E014F File Offset: 0x001DE34F
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005ED0 RID: 24272 RVA: 0x001E015D File Offset: 0x001DE35D
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005ED1 RID: 24273 RVA: 0x001E016C File Offset: 0x001DE36C
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

		// Token: 0x06005ED2 RID: 24274 RVA: 0x001E01C0 File Offset: 0x001DE3C0
		public override void OnUpdate()
		{
			this.DoAngle();
		}

		// Token: 0x06005ED3 RID: 24275 RVA: 0x001E01C8 File Offset: 0x001DE3C8
		private void DoAngle()
		{
			if (this.rb2d == null)
			{
				return;
			}
			Vector2 linearVelocity = this.rb2d.linearVelocity;
			float z = Mathf.Atan2(linearVelocity.y, linearVelocity.x) * 57.295776f + this.angleOffset.Value;
			if (this.worldSpace.Value)
			{
				this.target.Value.transform.eulerAngles = new Vector3(0f, 0f, z);
				return;
			}
			this.target.Value.transform.localEulerAngles = new Vector3(0f, 0f, z);
		}

		// Token: 0x04005B70 RID: 23408
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody2D))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005B71 RID: 23409
		[Tooltip("Offset the angle. If sprite faces right, leave as 0.")]
		public FsmFloat angleOffset;

		// Token: 0x04005B72 RID: 23410
		[Tooltip("Use local or world space.")]
		public FsmBool worldSpace;

		// Token: 0x04005B73 RID: 23411
		public bool everyFrame;

		// Token: 0x04005B74 RID: 23412
		private FsmGameObject target;
	}
}
