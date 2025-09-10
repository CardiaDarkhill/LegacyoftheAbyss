using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D27 RID: 3367
	[ActionCategory("Physics 2d")]
	[Tooltip("Set Collider2D to active or inactive. Can only be one collider on object. ")]
	public class SetColliderV2 : FsmStateAction
	{
		// Token: 0x06006336 RID: 25398 RVA: 0x001F5A38 File Offset: 0x001F3C38
		public override void Reset()
		{
			this.Target = null;
			this.SetActive = null;
			this.EveryFrame = false;
			this.ResetOnExit = false;
		}

		// Token: 0x06006337 RID: 25399 RVA: 0x001F5A58 File Offset: 0x001F3C58
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.Target);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			if (this.Target != null)
			{
				this.col = ownerDefaultTarget.GetComponent<Collider2D>();
				if (this.col != null)
				{
					this.initialState = this.col.enabled;
					this.col.enabled = this.SetActive.Value;
				}
			}
			if (!this.EveryFrame || !this.col)
			{
				base.Finish();
			}
		}

		// Token: 0x06006338 RID: 25400 RVA: 0x001F5AEB File Offset: 0x001F3CEB
		public override void OnUpdate()
		{
			if (this.col)
			{
				this.col.enabled = this.SetActive.Value;
			}
		}

		// Token: 0x06006339 RID: 25401 RVA: 0x001F5B10 File Offset: 0x001F3D10
		public override void OnExit()
		{
			if (!this.ResetOnExit)
			{
				return;
			}
			if (this.col != null)
			{
				this.col.enabled = this.initialState;
			}
		}

		// Token: 0x040061A0 RID: 24992
		[RequiredField]
		[Tooltip("The particle emitting GameObject")]
		public FsmOwnerDefault Target;

		// Token: 0x040061A1 RID: 24993
		public FsmBool SetActive;

		// Token: 0x040061A2 RID: 24994
		public bool EveryFrame;

		// Token: 0x040061A3 RID: 24995
		public bool ResetOnExit;

		// Token: 0x040061A4 RID: 24996
		private Collider2D col;

		// Token: 0x040061A5 RID: 24997
		private bool initialState;
	}
}
