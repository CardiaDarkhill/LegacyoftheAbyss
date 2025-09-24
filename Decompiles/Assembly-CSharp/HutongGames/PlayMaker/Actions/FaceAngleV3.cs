using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C2D RID: 3117
	[ActionCategory("Enemy AI")]
	[Tooltip("Object rotates to face direction it is travelling in.")]
	public class FaceAngleV3 : FsmStateAction
	{
		// Token: 0x06005ED5 RID: 24277 RVA: 0x001E0274 File Offset: 0x001DE474
		public override void Reset()
		{
			this.GameObject = null;
			this.AngleOffset = 0f;
			this.EveryFrame = false;
			this.SpriteFacesRight = null;
		}

		// Token: 0x06005ED6 RID: 24278 RVA: 0x001E029B File Offset: 0x001DE49B
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005ED7 RID: 24279 RVA: 0x001E02A9 File Offset: 0x001DE4A9
		public override void OnPreprocess()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06005ED8 RID: 24280 RVA: 0x001E02B8 File Offset: 0x001DE4B8
		public override void OnEnter()
		{
			this.target = base.Fsm.GetOwnerDefaultTarget(this.GameObject);
			this.transform = this.target.Value.transform;
			this.body = this.target.Value.GetComponent<Rigidbody2D>();
			if (!this.skipOnEnter)
			{
				this.DoAngle();
			}
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06005ED9 RID: 24281 RVA: 0x001E0329 File Offset: 0x001DE529
		public override void OnFixedUpdate()
		{
			this.DoAngle();
		}

		// Token: 0x06005EDA RID: 24282 RVA: 0x001E0334 File Offset: 0x001DE534
		private void DoAngle()
		{
			if (this.body.linearVelocity.magnitude < Mathf.Epsilon)
			{
				return;
			}
			Vector2 normalized = this.body.linearVelocity.normalized;
			float num = this.AngleOffset.Value;
			if (this.SpriteFacesRight.Value)
			{
				num += 180f;
			}
			if (normalized.x < 0f)
			{
				num += 180f;
			}
			float num2 = Mathf.Atan2(normalized.y, normalized.x) * 57.29578f;
			num2 += num;
			if (this.WorldSpace.Value)
			{
				this.target.Value.transform.eulerAngles = new Vector3(0f, 0f, num2);
				return;
			}
			this.target.Value.transform.localEulerAngles = new Vector3(0f, 0f, num2);
		}

		// Token: 0x04005B75 RID: 23413
		[RequiredField]
		public FsmOwnerDefault GameObject;

		// Token: 0x04005B76 RID: 23414
		public FsmFloat AngleOffset;

		// Token: 0x04005B77 RID: 23415
		public FsmBool WorldSpace;

		// Token: 0x04005B78 RID: 23416
		public FsmBool SpriteFacesRight;

		// Token: 0x04005B79 RID: 23417
		public bool EveryFrame;

		// Token: 0x04005B7A RID: 23418
		public bool skipOnEnter;

		// Token: 0x04005B7B RID: 23419
		private FsmGameObject target;

		// Token: 0x04005B7C RID: 23420
		private Transform transform;

		// Token: 0x04005B7D RID: 23421
		private Rigidbody2D body;
	}
}
