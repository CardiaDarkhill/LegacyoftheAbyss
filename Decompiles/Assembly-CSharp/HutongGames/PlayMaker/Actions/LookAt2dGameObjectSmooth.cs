using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B17 RID: 2839
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Rotates a 2d Game Object on it's z axis so its forward vector points at a Target. Rotates th eobject per frame via speed.")]
	public class LookAt2dGameObjectSmooth : FsmStateAction
	{
		// Token: 0x0600594F RID: 22863 RVA: 0x001C4E20 File Offset: 0x001C3020
		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.debug = false;
			this.debugLineColor = Color.green;
		}

		// Token: 0x06005950 RID: 22864 RVA: 0x001C4E4C File Offset: 0x001C304C
		public override void OnEnter()
		{
			this.DoLookAt();
		}

		// Token: 0x06005951 RID: 22865 RVA: 0x001C4E54 File Offset: 0x001C3054
		public override void OnUpdate()
		{
			this.DoLookAt();
		}

		// Token: 0x06005952 RID: 22866 RVA: 0x001C4E5C File Offset: 0x001C305C
		private void DoLookAt()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.goTarget = this.targetObject.Value;
			if (this.go == null || this.targetObject == null)
			{
				return;
			}
			float y = this.goTarget.transform.position.y - this.go.transform.position.y;
			float x = this.goTarget.transform.position.x - this.go.transform.position.x;
			float num;
			for (num = Mathf.Atan2(y, x) * 57.295776f; num < 0f; num += 360f)
			{
			}
			if (this.go.transform.eulerAngles.z < num - this.rotationOffset.Value)
			{
				this.go.transform.Rotate(0f, 0f, this.speed.Value);
				if (this.go.transform.eulerAngles.z > num - this.rotationOffset.Value)
				{
					this.go.transform.rotation = Quaternion.Euler(0f, 0f, num - this.rotationOffset.Value);
				}
			}
			if (this.go.transform.eulerAngles.z > num - this.rotationOffset.Value)
			{
				this.go.transform.Rotate(0f, 0f, -this.speed.Value);
				if (this.go.transform.eulerAngles.z < num - this.rotationOffset.Value)
				{
					this.go.transform.rotation = Quaternion.Euler(0f, 0f, num - this.rotationOffset.Value);
				}
			}
			if (this.debug.Value)
			{
				Debug.DrawLine(this.go.transform.position, this.goTarget.transform.position, this.debugLineColor.Value);
			}
		}

		// Token: 0x040054AD RID: 21677
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040054AE RID: 21678
		[Tooltip("The GameObject to Look At.")]
		public FsmGameObject targetObject;

		// Token: 0x040054AF RID: 21679
		[Tooltip("Set the GameObject starting offset. In degrees. 0 if your object is facing right, 180 if facing left etc...")]
		public FsmFloat rotationOffset;

		// Token: 0x040054B0 RID: 21680
		[RequiredField]
		[Tooltip("Speed the object rotates at to meet its target angle (in degrees per frame).")]
		public FsmFloat speed;

		// Token: 0x040054B1 RID: 21681
		[Title("Draw Debug Line")]
		[Tooltip("Draw a debug line from the GameObject to the Target.")]
		public FsmBool debug;

		// Token: 0x040054B2 RID: 21682
		[Tooltip("Color to use for the debug line.")]
		public FsmColor debugLineColor;

		// Token: 0x040054B3 RID: 21683
		private GameObject go;

		// Token: 0x040054B4 RID: 21684
		private GameObject goTarget;

		// Token: 0x040054B5 RID: 21685
		private Vector3 lookAtPos;
	}
}
