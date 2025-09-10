using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FD1 RID: 4049
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Rotates a 2d Game Object on it's z axis so its forward vector points at a Target.")]
	public class LookAt2dGameObject : FsmStateAction
	{
		// Token: 0x06006F9C RID: 28572 RVA: 0x002280CC File Offset: 0x002262CC
		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.debug = false;
			this.debugLineColor = Color.green;
			this.everyFrame = true;
		}

		// Token: 0x06006F9D RID: 28573 RVA: 0x002280FF File Offset: 0x002262FF
		public override void OnEnter()
		{
			this.DoLookAt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F9E RID: 28574 RVA: 0x00228115 File Offset: 0x00226315
		public override void OnUpdate()
		{
			this.DoLookAt();
		}

		// Token: 0x06006F9F RID: 28575 RVA: 0x00228120 File Offset: 0x00226320
		private void DoLookAt()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.goTarget = this.targetObject.Value;
			if (this.go == null || this.goTarget == null)
			{
				return;
			}
			Vector3 vector = this.goTarget.transform.position - this.go.transform.position;
			vector.Normalize();
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			this.go.transform.rotation = Quaternion.Euler(0f, 0f, num - this.rotationOffset.Value);
			if (this.debug.Value)
			{
				Debug.DrawLine(this.go.transform.position, this.goTarget.transform.position, this.debugLineColor.Value);
			}
		}

		// Token: 0x04006F79 RID: 28537
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006F7A RID: 28538
		[Tooltip("The GameObject to Look At.")]
		public FsmGameObject targetObject;

		// Token: 0x04006F7B RID: 28539
		[Tooltip("Set the GameObject starting offset. In degrees. 0 if your object is facing right, 180 if facing left etc...")]
		public FsmFloat rotationOffset;

		// Token: 0x04006F7C RID: 28540
		[Title("Draw Debug Line")]
		[Tooltip("Draw a debug line from the GameObject to the Target.")]
		public FsmBool debug;

		// Token: 0x04006F7D RID: 28541
		[Tooltip("Color to use for the debug line.")]
		public FsmColor debugLineColor;

		// Token: 0x04006F7E RID: 28542
		[Tooltip("Repeat every frame.")]
		public bool everyFrame = true;

		// Token: 0x04006F7F RID: 28543
		private GameObject go;

		// Token: 0x04006F80 RID: 28544
		private GameObject goTarget;
	}
}
