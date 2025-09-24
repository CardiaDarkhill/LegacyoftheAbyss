using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D6A RID: 3434
	[ActionCategory(ActionCategory.Camera)]
	[Tooltip("Follows a target GameObject smoothly in 2D space")]
	public class SmoothFollowTarget2D : FsmStateAction
	{
		// Token: 0x06006459 RID: 25689 RVA: 0x001F9D96 File Offset: 0x001F7F96
		public override void Reset()
		{
			this.dampTime = 0.1f;
		}

		// Token: 0x0600645A RID: 25690 RVA: 0x001F9DA4 File Offset: 0x001F7FA4
		public override void OnEnter()
		{
			this.camera = base.Fsm.GetOwnerDefaultTarget(this.gameObject).GetComponent<Camera>();
			if (this.targetObject == null || this.camera == null)
			{
				return;
			}
			this.target = this.targetObject.Value;
		}

		// Token: 0x0600645B RID: 25691 RVA: 0x001F9DF8 File Offset: 0x001F7FF8
		public override void OnUpdate()
		{
			Vector3 vector = this.camera.WorldToViewportPoint(this.target.transform.position);
			Vector3 b = this.target.transform.position - this.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, vector.z));
			Vector3 vector2 = this.camera.transform.position + b;
			Vector3 zero = Vector3.zero;
			this.camera.transform.position = Vector3.SmoothDamp(this.camera.transform.position, vector2, ref zero, this.dampTime);
		}

		// Token: 0x040062D9 RID: 25305
		[RequiredField]
		[Tooltip("Camera to control.")]
		[CheckForComponent(typeof(Camera))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040062DA RID: 25306
		[Tooltip("The GameObject to follow.")]
		public FsmGameObject targetObject;

		// Token: 0x040062DB RID: 25307
		[RequiredField]
		public float dampTime;

		// Token: 0x040062DC RID: 25308
		private Camera camera;

		// Token: 0x040062DD RID: 25309
		private GameObject target;

		// Token: 0x040062DE RID: 25310
		private Transform transform;
	}
}
