using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010E1 RID: 4321
	[ActionCategory(ActionCategory.Transform)]
	[Tooltip("Moves a Game Object towards a Target. Optionally sends an event when successful. The Target can be specified as a Game Object or a world Position. If you specify both, then the Position is used as a local offset from the Object's Position.")]
	public class MoveTowards : FsmStateAction
	{
		// Token: 0x060074EC RID: 29932 RVA: 0x0023C1E7 File Offset: 0x0023A3E7
		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.maxSpeed = 10f;
			this.finishDistance = 1f;
			this.finishEvent = null;
		}

		// Token: 0x060074ED RID: 29933 RVA: 0x0023C21E File Offset: 0x0023A41E
		public override void OnUpdate()
		{
			this.DoMoveTowards();
		}

		// Token: 0x060074EE RID: 29934 RVA: 0x0023C228 File Offset: 0x0023A428
		private void DoMoveTowards()
		{
			if (!this.UpdateTargetPos())
			{
				return;
			}
			this.go.transform.position = Vector3.MoveTowards(this.go.transform.position, this.targetPos, this.maxSpeed.Value * Time.deltaTime);
			if ((this.go.transform.position - this.targetPos).magnitude < this.finishDistance.Value)
			{
				base.Fsm.Event(this.finishEvent);
				base.Finish();
			}
		}

		// Token: 0x060074EF RID: 29935 RVA: 0x0023C2C4 File Offset: 0x0023A4C4
		public bool UpdateTargetPos()
		{
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
				this.targetPos = ((!this.targetPosition.IsNone) ? this.goTarget.transform.TransformPoint(this.targetPosition.Value) : this.goTarget.transform.position);
			}
			else
			{
				this.targetPos = this.targetPosition.Value;
			}
			this.targetPosWithVertical = this.targetPos;
			if (this.ignoreVertical.Value)
			{
				this.targetPos.y = this.go.transform.position.y;
			}
			return true;
		}

		// Token: 0x060074F0 RID: 29936 RVA: 0x0023C3C1 File Offset: 0x0023A5C1
		public Vector3 GetTargetPos()
		{
			return this.targetPos;
		}

		// Token: 0x060074F1 RID: 29937 RVA: 0x0023C3C9 File Offset: 0x0023A5C9
		public Vector3 GetTargetPosWithVertical()
		{
			return this.targetPosWithVertical;
		}

		// Token: 0x0400753D RID: 30013
		[RequiredField]
		[Tooltip("The GameObject to Move")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400753E RID: 30014
		[Tooltip("A target GameObject to move towards. Or use a world Target Position below.")]
		public FsmGameObject targetObject;

		// Token: 0x0400753F RID: 30015
		[Tooltip("A world position to move towards, if no Target Object is set. Otherwise used as a local offset from the Target Object.")]
		public FsmVector3 targetPosition;

		// Token: 0x04007540 RID: 30016
		[Tooltip("Ignore any height difference in the target.")]
		public FsmBool ignoreVertical;

		// Token: 0x04007541 RID: 30017
		[HasFloatSlider(0f, 20f)]
		[Tooltip("The maximum movement speed (Unity units per second). HINT: You can make this a variable to change it over time.")]
		public FsmFloat maxSpeed;

		// Token: 0x04007542 RID: 30018
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Distance at which the move is considered finished, and the Finish Event is sent.")]
		public FsmFloat finishDistance;

		// Token: 0x04007543 RID: 30019
		[Tooltip("Event to send when the Finish Distance is reached. Use this to transition to the next state.")]
		public FsmEvent finishEvent;

		// Token: 0x04007544 RID: 30020
		private GameObject go;

		// Token: 0x04007545 RID: 30021
		private GameObject goTarget;

		// Token: 0x04007546 RID: 30022
		private Vector3 targetPos;

		// Token: 0x04007547 RID: 30023
		private Vector3 targetPosWithVertical;
	}
}
