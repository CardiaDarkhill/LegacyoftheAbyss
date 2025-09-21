using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x020010E0 RID: 4320
	[ActionCategory(ActionCategory.Transform)]
	[HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=4758.0")]
	[Tooltip("Move a GameObject to another GameObject using easing functions. Works like iTween Move To, but with better performance.")]
	public class MoveObject : EaseFsmAction
	{
		// Token: 0x060074E8 RID: 29928 RVA: 0x0023BECF File Offset: 0x0023A0CF
		public override void Reset()
		{
			base.Reset();
			this.fromValue = null;
			this.toVector = null;
			this.finishInNextStep = false;
			this.fromVector = null;
		}

		// Token: 0x060074E9 RID: 29929 RVA: 0x0023BEF4 File Offset: 0x0023A0F4
		public override void OnEnter()
		{
			base.OnEnter();
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.objectToMove);
			this.fromVector = ownerDefaultTarget.transform.position;
			this.toVector = this.destination.Value.transform.position;
			this.fromFloats = new float[3];
			this.fromFloats[0] = this.fromVector.Value.x;
			this.fromFloats[1] = this.fromVector.Value.y;
			this.fromFloats[2] = this.fromVector.Value.z;
			this.toFloats = new float[3];
			this.toFloats[0] = this.toVector.Value.x;
			this.toFloats[1] = this.toVector.Value.y;
			this.toFloats[2] = this.toVector.Value.z;
			this.resultFloats = new float[3];
			this.resultFloats[0] = this.fromVector.Value.x;
			this.resultFloats[1] = this.fromVector.Value.y;
			this.resultFloats[2] = this.fromVector.Value.z;
			this.finishInNextStep = false;
		}

		// Token: 0x060074EA RID: 29930 RVA: 0x0023C054 File Offset: 0x0023A254
		public override void OnUpdate()
		{
			base.OnUpdate();
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.objectToMove);
			ownerDefaultTarget.transform.position = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
			if (this.finishInNextStep)
			{
				base.Finish();
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
			}
			if (this.finishAction && !this.finishInNextStep)
			{
				ownerDefaultTarget.transform.position = new Vector3(this.reverse.IsNone ? this.toVector.Value.x : (this.reverse.Value ? this.fromValue.Value.x : this.toVector.Value.x), this.reverse.IsNone ? this.toVector.Value.y : (this.reverse.Value ? this.fromValue.Value.y : this.toVector.Value.y), this.reverse.IsNone ? this.toVector.Value.z : (this.reverse.Value ? this.fromValue.Value.z : this.toVector.Value.z));
				this.finishInNextStep = true;
			}
		}

		// Token: 0x04007537 RID: 30007
		[RequiredField]
		[Tooltip("The GameObject to move.")]
		public FsmOwnerDefault objectToMove;

		// Token: 0x04007538 RID: 30008
		[RequiredField]
		[Tooltip("The target GamObject.")]
		public FsmGameObject destination;

		// Token: 0x04007539 RID: 30009
		private FsmVector3 fromValue;

		// Token: 0x0400753A RID: 30010
		private FsmVector3 toVector;

		// Token: 0x0400753B RID: 30011
		private FsmVector3 fromVector;

		// Token: 0x0400753C RID: 30012
		private bool finishInNextStep;
	}
}
