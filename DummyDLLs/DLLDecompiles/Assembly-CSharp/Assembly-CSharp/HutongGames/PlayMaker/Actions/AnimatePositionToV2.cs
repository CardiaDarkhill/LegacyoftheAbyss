using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA6 RID: 2982
	[ActionCategory(ActionCategory.AnimateVariables)]
	public class AnimatePositionToV2 : EaseFsmAction
	{
		// Token: 0x06005C1F RID: 23583 RVA: 0x001CFCB7 File Offset: 0x001CDEB7
		[UsedImplicitly]
		public bool HasVectorValue()
		{
			return !this.ToValue.IsNone;
		}

		// Token: 0x06005C20 RID: 23584 RVA: 0x001CFCC7 File Offset: 0x001CDEC7
		public override void Reset()
		{
			base.Reset();
			this.GameObject = null;
			this.ToValue = new FsmVector3
			{
				UseVariable = true
			};
			this.ToX = null;
			this.ToY = null;
			this.ToZ = null;
			this.LocalSpace = false;
		}

		// Token: 0x06005C21 RID: 23585 RVA: 0x001CFD04 File Offset: 0x001CDF04
		public override void OnEnter()
		{
			base.OnEnter();
			this.objectTransform = base.Fsm.GetOwnerDefaultTarget(this.GameObject).transform;
			if (this.objectTransform == null)
			{
				return;
			}
			this.fromValue = (this.LocalSpace ? this.objectTransform.localPosition : this.objectTransform.position);
			this.toValue = ((!this.ToValue.IsNone) ? this.ToValue.Value : new Vector3(this.ToX.Value, this.ToY.Value, this.ToZ.Value));
			this.fromFloats = new float[3];
			this.fromFloats[0] = this.fromValue.x;
			this.fromFloats[1] = this.fromValue.y;
			this.fromFloats[2] = this.fromValue.z;
			this.toFloats = new float[3];
			this.toFloats[0] = this.toValue.x;
			this.toFloats[1] = this.toValue.y;
			this.toFloats[2] = this.toValue.z;
			this.resultFloats = new float[3];
			this.finishInNextStep = false;
		}

		// Token: 0x06005C22 RID: 23586 RVA: 0x001CFE4C File Offset: 0x001CE04C
		public override void OnUpdate()
		{
			base.OnUpdate();
			if (this.isRunning)
			{
				if (this.LocalSpace)
				{
					this.objectTransform.localPosition = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
				}
				else
				{
					this.objectTransform.position = new Vector3(this.resultFloats[0], this.resultFloats[1], this.resultFloats[2]);
				}
			}
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
				if (this.LocalSpace)
				{
					this.objectTransform.localPosition = new Vector3(this.reverse.IsNone ? this.toValue.x : (this.reverse.Value ? this.fromValue.x : this.toValue.x), this.reverse.IsNone ? this.toValue.y : (this.reverse.Value ? this.fromValue.y : this.toValue.y), this.reverse.IsNone ? this.toValue.z : (this.reverse.Value ? this.fromValue.z : this.toValue.z));
				}
				else
				{
					this.objectTransform.position = new Vector3(this.reverse.IsNone ? this.toValue.x : (this.reverse.Value ? this.fromValue.x : this.toValue.x), this.reverse.IsNone ? this.toValue.y : (this.reverse.Value ? this.fromValue.y : this.toValue.y), this.reverse.IsNone ? this.toValue.z : (this.reverse.Value ? this.fromValue.z : this.toValue.z));
				}
				this.finishInNextStep = true;
			}
		}

		// Token: 0x0400578A RID: 22410
		public FsmOwnerDefault GameObject;

		// Token: 0x0400578B RID: 22411
		public FsmVector3 ToValue;

		// Token: 0x0400578C RID: 22412
		[HideIf("HasVectorValue")]
		public FsmFloat ToX;

		// Token: 0x0400578D RID: 22413
		[HideIf("HasVectorValue")]
		public FsmFloat ToY;

		// Token: 0x0400578E RID: 22414
		[HideIf("HasVectorValue")]
		public FsmFloat ToZ;

		// Token: 0x0400578F RID: 22415
		public bool LocalSpace;

		// Token: 0x04005790 RID: 22416
		private bool finishInNextStep;

		// Token: 0x04005791 RID: 22417
		private Transform objectTransform;

		// Token: 0x04005792 RID: 22418
		private Vector3 fromValue;

		// Token: 0x04005793 RID: 22419
		private Vector3 toValue;
	}
}
