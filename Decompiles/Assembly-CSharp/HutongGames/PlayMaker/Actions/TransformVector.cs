using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D9A RID: 3482
	public abstract class TransformVector<T> : FsmStateAction where T : NamedVariable
	{
		// Token: 0x0600652A RID: 25898 RVA: 0x001FE66B File Offset: 0x001FC86B
		public override void Reset()
		{
			this.Transform = null;
			this.Transformation = TransformVector<T>.Transformations.TransformPoint;
			this.Vector = default(T);
			this.StoreResult = default(T);
			this.EveryFrame = false;
		}

		// Token: 0x0600652B RID: 25899 RVA: 0x001FE69A File Offset: 0x001FC89A
		public override void OnEnter()
		{
			this.DoAction();
			if (!this.EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600652C RID: 25900 RVA: 0x001FE6B0 File Offset: 0x001FC8B0
		public override void OnUpdate()
		{
			this.DoAction();
		}

		// Token: 0x0600652D RID: 25901 RVA: 0x001FE6B8 File Offset: 0x001FC8B8
		private void DoAction()
		{
			GameObject safe = this.Transform.GetSafe(this);
			if (!safe)
			{
				return;
			}
			Func<Vector3, Vector3> func = null;
			switch (this.Transformation)
			{
			case TransformVector<T>.Transformations.TransformPoint:
				func = new Func<Vector3, Vector3>(safe.transform.TransformPoint);
				break;
			case TransformVector<T>.Transformations.TransformDirection:
				func = new Func<Vector3, Vector3>(safe.transform.TransformDirection);
				break;
			case TransformVector<T>.Transformations.TransformVector:
				func = new Func<Vector3, Vector3>(safe.transform.TransformVector);
				break;
			case TransformVector<T>.Transformations.InverseTransformPoint:
				func = new Func<Vector3, Vector3>(safe.transform.InverseTransformPoint);
				break;
			case TransformVector<T>.Transformations.InverseTransformDirection:
				func = new Func<Vector3, Vector3>(safe.transform.InverseTransformDirection);
				break;
			case TransformVector<T>.Transformations.InverseTransformVector:
				func = new Func<Vector3, Vector3>(safe.transform.InverseTransformVector);
				break;
			}
			if (func != null)
			{
				this.SetStoreResult(func(this.GetInputVector()));
			}
		}

		// Token: 0x0600652E RID: 25902
		protected abstract void SetStoreResult(Vector3 value);

		// Token: 0x0600652F RID: 25903
		protected abstract Vector3 GetInputVector();

		// Token: 0x04006425 RID: 25637
		public FsmOwnerDefault Transform;

		// Token: 0x04006426 RID: 25638
		public TransformVector<T>.Transformations Transformation;

		// Token: 0x04006427 RID: 25639
		public T Vector;

		// Token: 0x04006428 RID: 25640
		[UIHint(UIHint.Variable)]
		public T StoreResult;

		// Token: 0x04006429 RID: 25641
		public bool EveryFrame;

		// Token: 0x02001B91 RID: 7057
		public enum Transformations
		{
			// Token: 0x04009D98 RID: 40344
			TransformPoint,
			// Token: 0x04009D99 RID: 40345
			TransformDirection,
			// Token: 0x04009D9A RID: 40346
			TransformVector,
			// Token: 0x04009D9B RID: 40347
			InverseTransformPoint,
			// Token: 0x04009D9C RID: 40348
			InverseTransformDirection,
			// Token: 0x04009D9D RID: 40349
			InverseTransformVector
		}
	}
}
