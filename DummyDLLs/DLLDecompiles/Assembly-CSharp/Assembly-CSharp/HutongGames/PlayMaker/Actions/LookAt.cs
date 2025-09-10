using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010DE RID: 4318
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Rotates a Game Object so its forward vector points at a Target. The Target can be specified as a GameObject or a world Position. If you specify both, then Position specifies a local offset from the target object's Position.")]
	public class LookAt : FsmStateAction
	{
		// Token: 0x060074D8 RID: 29912 RVA: 0x0023BB90 File Offset: 0x00239D90
		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.targetPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.upVector = new FsmVector3
			{
				UseVariable = true
			};
			this.keepVertical = true;
			this.debug = false;
			this.debugLineColor = Color.yellow;
			this.everyFrame = true;
		}

		// Token: 0x060074D9 RID: 29913 RVA: 0x0023BBFE File Offset: 0x00239DFE
		public override void OnPreprocess()
		{
			base.Fsm.HandleLateUpdate = true;
		}

		// Token: 0x060074DA RID: 29914 RVA: 0x0023BC0C File Offset: 0x00239E0C
		public override void OnEnter()
		{
			this.DoLookAt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060074DB RID: 29915 RVA: 0x0023BC22 File Offset: 0x00239E22
		public override void OnLateUpdate()
		{
			this.DoLookAt();
		}

		// Token: 0x060074DC RID: 29916 RVA: 0x0023BC2C File Offset: 0x00239E2C
		private void DoLookAt()
		{
			if (!this.UpdateLookAtPosition())
			{
				return;
			}
			this.go.transform.LookAt(this.lookAtPos, this.upVector.IsNone ? Vector3.up : this.upVector.Value);
			if (this.debug.Value)
			{
				Debug.DrawLine(this.go.transform.position, this.lookAtPos, this.debugLineColor.Value);
			}
		}

		// Token: 0x060074DD RID: 29917 RVA: 0x0023BCAC File Offset: 0x00239EAC
		public bool UpdateLookAtPosition()
		{
			if (base.Fsm == null)
			{
				return false;
			}
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				return false;
			}
			this.goTarget = this.targetObject.Value;
			if (this.goTarget == null && this.targetPosition.IsNone)
			{
				return false;
			}
			if (this.goTarget != null)
			{
				this.lookAtPos = ((!this.targetPosition.IsNone) ? this.goTarget.transform.TransformPoint(this.targetPosition.Value) : this.goTarget.transform.position);
			}
			else
			{
				this.lookAtPos = this.targetPosition.Value;
			}
			this.lookAtPosWithVertical = this.lookAtPos;
			if (this.keepVertical.Value)
			{
				this.lookAtPos.y = this.go.transform.position.y;
			}
			return true;
		}

		// Token: 0x060074DE RID: 29918 RVA: 0x0023BDB3 File Offset: 0x00239FB3
		public Vector3 GetLookAtPosition()
		{
			return this.lookAtPos;
		}

		// Token: 0x060074DF RID: 29919 RVA: 0x0023BDBB File Offset: 0x00239FBB
		public Vector3 GetLookAtPositionWithVertical()
		{
			return this.lookAtPosWithVertical;
		}

		// Token: 0x04007526 RID: 29990
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04007527 RID: 29991
		[Tooltip("The GameObject to Look At.")]
		public FsmGameObject targetObject;

		// Token: 0x04007528 RID: 29992
		[Tooltip("World position to look at, or local offset from Target Object if specified.")]
		public FsmVector3 targetPosition;

		// Token: 0x04007529 RID: 29993
		[Tooltip("Rotate the GameObject to point its up direction vector in the direction hinted at by the Up Vector. See Unity Look At docs for more details.")]
		public FsmVector3 upVector;

		// Token: 0x0400752A RID: 29994
		[Tooltip("Don't rotate vertically.")]
		public FsmBool keepVertical;

		// Token: 0x0400752B RID: 29995
		[Title("Draw Debug Line")]
		[Tooltip("Draw a debug line from the GameObject to the Target.")]
		public FsmBool debug;

		// Token: 0x0400752C RID: 29996
		[Tooltip("Color to use for the debug line.")]
		public FsmColor debugLineColor;

		// Token: 0x0400752D RID: 29997
		[Tooltip("Repeat every frame.")]
		public bool everyFrame = true;

		// Token: 0x0400752E RID: 29998
		private GameObject go;

		// Token: 0x0400752F RID: 29999
		private GameObject goTarget;

		// Token: 0x04007530 RID: 30000
		private Vector3 lookAtPos;

		// Token: 0x04007531 RID: 30001
		private Vector3 lookAtPosWithVertical;
	}
}
