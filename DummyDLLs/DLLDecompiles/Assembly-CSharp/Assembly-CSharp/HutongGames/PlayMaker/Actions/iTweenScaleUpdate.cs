using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F2D RID: 3885
	[ActionCategory("iTween")]
	[Tooltip("CSimilar to ScaleTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a 'live' set of changing values. Does not utilize an EaseType.")]
	public class iTweenScaleUpdate : FsmStateAction
	{
		// Token: 0x06006C56 RID: 27734 RVA: 0x0021D15C File Offset: 0x0021B35C
		public override void Reset()
		{
			this.transformScale = new FsmGameObject
			{
				UseVariable = true
			};
			this.vectorScale = new FsmVector3
			{
				UseVariable = true
			};
			this.time = 1f;
		}

		// Token: 0x06006C57 RID: 27735 RVA: 0x0021D194 File Offset: 0x0021B394
		public override void OnEnter()
		{
			this.hash = new Hashtable();
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				base.Finish();
				return;
			}
			if (this.transformScale.IsNone)
			{
				this.hash.Add("scale", this.vectorScale.IsNone ? Vector3.zero : this.vectorScale.Value);
			}
			else if (this.vectorScale.IsNone)
			{
				this.hash.Add("scale", this.transformScale.Value.transform);
			}
			else
			{
				this.hash.Add("scale", this.transformScale.Value.transform.localScale + this.vectorScale.Value);
			}
			this.hash.Add("time", this.time.IsNone ? 1f : this.time.Value);
			this.DoiTween();
		}

		// Token: 0x06006C58 RID: 27736 RVA: 0x0021D2BE File Offset: 0x0021B4BE
		public override void OnExit()
		{
		}

		// Token: 0x06006C59 RID: 27737 RVA: 0x0021D2C0 File Offset: 0x0021B4C0
		public override void OnUpdate()
		{
			this.hash.Remove("scale");
			if (this.transformScale.IsNone)
			{
				this.hash.Add("scale", this.vectorScale.IsNone ? Vector3.zero : this.vectorScale.Value);
			}
			else if (this.vectorScale.IsNone)
			{
				this.hash.Add("scale", this.transformScale.Value.transform);
			}
			else
			{
				this.hash.Add("scale", this.transformScale.Value.transform.localScale + this.vectorScale.Value);
			}
			this.DoiTween();
		}

		// Token: 0x06006C5A RID: 27738 RVA: 0x0021D38F File Offset: 0x0021B58F
		private void DoiTween()
		{
			iTween.ScaleUpdate(this.go, this.hash);
		}

		// Token: 0x04006C19 RID: 27673
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006C1A RID: 27674
		[Tooltip("Scale To a transform scale.")]
		public FsmGameObject transformScale;

		// Token: 0x04006C1B RID: 27675
		[Tooltip("A scale vector the GameObject will animate To.")]
		public FsmVector3 vectorScale;

		// Token: 0x04006C1C RID: 27676
		[Tooltip("The time in seconds the animation will take to complete. If transformScale is set, this is used as an offset.")]
		public FsmFloat time;

		// Token: 0x04006C1D RID: 27677
		private Hashtable hash;

		// Token: 0x04006C1E RID: 27678
		private GameObject go;
	}
}
