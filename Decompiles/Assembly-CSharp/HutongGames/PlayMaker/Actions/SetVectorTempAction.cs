using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D86 RID: 3462
	public abstract class SetVectorTempAction : FsmStateAction
	{
		// Token: 0x060064CD RID: 25805 RVA: 0x001FD11C File Offset: 0x001FB31C
		public bool VectorIsNotNone()
		{
			return !this.Vector.IsNone;
		}

		// Token: 0x060064CE RID: 25806 RVA: 0x001FD12C File Offset: 0x001FB32C
		public virtual bool HideSpace()
		{
			return false;
		}

		// Token: 0x060064CF RID: 25807 RVA: 0x001FD12F File Offset: 0x001FB32F
		public override void Reset()
		{
			this.Target = null;
			this.Vector = null;
			this.X = null;
			this.Y = null;
			this.Z = null;
			this.Space = Space.World;
		}

		// Token: 0x060064D0 RID: 25808 RVA: 0x001FD15C File Offset: 0x001FB35C
		public override void OnEnter()
		{
			GameObject obj = this.Target.GetSafe(this);
			if (!obj)
			{
				base.Finish();
				return;
			}
			Vector3 initialVector = this.GetVector(obj.transform);
			if (this.VectorIsNotNone())
			{
				this.SetVector(obj.transform, this.Vector.Value);
			}
			else
			{
				Vector3 initialVector2 = initialVector;
				if (!this.X.IsNone)
				{
					initialVector2.x = this.X.Value;
				}
				if (!this.Y.IsNone)
				{
					initialVector2.y = this.Y.Value;
				}
				if (!this.Z.IsNone)
				{
					initialVector2.z = this.Z.Value;
				}
				this.SetVector(obj.transform, initialVector2);
			}
			RecycleResetHandler.Add(obj, delegate()
			{
				this.SetVector(obj.transform, initialVector);
			});
			base.Finish();
		}

		// Token: 0x060064D1 RID: 25809
		protected abstract void SetVector(Transform transform, Vector3 vector);

		// Token: 0x060064D2 RID: 25810
		protected abstract Vector3 GetVector(Transform transform);

		// Token: 0x040063D0 RID: 25552
		public FsmOwnerDefault Target;

		// Token: 0x040063D1 RID: 25553
		public FsmVector3 Vector;

		// Token: 0x040063D2 RID: 25554
		[HideIf("VectorIsNotNone")]
		public FsmFloat X;

		// Token: 0x040063D3 RID: 25555
		[HideIf("VectorIsNotNone")]
		public FsmFloat Y;

		// Token: 0x040063D4 RID: 25556
		[HideIf("VectorIsNotNone")]
		public FsmFloat Z;

		// Token: 0x040063D5 RID: 25557
		[HideIf("HideSpace")]
		public Space Space;
	}
}
