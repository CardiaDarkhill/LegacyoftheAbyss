using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000F28 RID: 3880
	[ActionCategory("iTween")]
	[Tooltip("Similar to RotateTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a 'live' set of changing values. Does not utilize an EaseType.")]
	public class iTweenRotateUpdate : FsmStateAction
	{
		// Token: 0x06006C3C RID: 27708 RVA: 0x0021C37C File Offset: 0x0021A57C
		public override void Reset()
		{
			this.transformRotation = new FsmGameObject
			{
				UseVariable = true
			};
			this.vectorRotation = new FsmVector3
			{
				UseVariable = true
			};
			this.time = 1f;
			this.space = Space.World;
		}

		// Token: 0x06006C3D RID: 27709 RVA: 0x0021C3BC File Offset: 0x0021A5BC
		public override void OnEnter()
		{
			this.hash = new Hashtable();
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				base.Finish();
				return;
			}
			if (this.transformRotation.IsNone)
			{
				this.hash.Add("rotation", this.vectorRotation.IsNone ? Vector3.zero : this.vectorRotation.Value);
			}
			else if (this.vectorRotation.IsNone)
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform);
			}
			else if (this.space == Space.World)
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform.eulerAngles + this.vectorRotation.Value);
			}
			else
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform.localEulerAngles + this.vectorRotation.Value);
			}
			this.hash.Add("time", this.time.IsNone ? 1f : this.time.Value);
			this.hash.Add("islocal", this.space == Space.Self);
			this.DoiTween();
		}

		// Token: 0x06006C3E RID: 27710 RVA: 0x0021C54B File Offset: 0x0021A74B
		public override void OnExit()
		{
		}

		// Token: 0x06006C3F RID: 27711 RVA: 0x0021C550 File Offset: 0x0021A750
		public override void OnUpdate()
		{
			this.hash.Remove("rotation");
			if (this.transformRotation.IsNone)
			{
				this.hash.Add("rotation", this.vectorRotation.IsNone ? Vector3.zero : this.vectorRotation.Value);
			}
			else if (this.vectorRotation.IsNone)
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform);
			}
			else if (this.space == Space.World)
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform.eulerAngles + this.vectorRotation.Value);
			}
			else
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform.localEulerAngles + this.vectorRotation.Value);
			}
			this.DoiTween();
		}

		// Token: 0x06006C40 RID: 27712 RVA: 0x0021C666 File Offset: 0x0021A866
		private void DoiTween()
		{
			iTween.RotateUpdate(this.go, this.hash);
		}

		// Token: 0x04006BF0 RID: 27632
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006BF1 RID: 27633
		[Tooltip("Rotate to a transform rotation.")]
		public FsmGameObject transformRotation;

		// Token: 0x04006BF2 RID: 27634
		[Tooltip("A rotation the GameObject will animate from.")]
		public FsmVector3 vectorRotation;

		// Token: 0x04006BF3 RID: 27635
		[Tooltip("The time in seconds the animation will take to complete. If transformRotation is set, this is used as an offset.")]
		public FsmFloat time;

		// Token: 0x04006BF4 RID: 27636
		[Tooltip("Whether to animate in local or world space.")]
		public Space space;

		// Token: 0x04006BF5 RID: 27637
		private Hashtable hash;

		// Token: 0x04006BF6 RID: 27638
		private GameObject go;
	}
}
