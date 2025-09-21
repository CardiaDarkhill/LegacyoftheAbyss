using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D26 RID: 3366
	[ActionCategory("Physics 2d")]
	[Tooltip("Set Collider2D to active or inactive.")]
	public class SetCollider : FsmStateAction
	{
		// Token: 0x06006332 RID: 25394 RVA: 0x001F594C File Offset: 0x001F3B4C
		public override void Reset()
		{
			this.gameObject = null;
			this.active = false;
		}

		// Token: 0x06006333 RID: 25395 RVA: 0x001F5964 File Offset: 0x001F3B64
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			if (this.gameObject != null)
			{
				Collider2D[] components = ownerDefaultTarget.GetComponents<Collider2D>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].enabled = this.active.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x06006334 RID: 25396 RVA: 0x001F59CC File Offset: 0x001F3BCC
		public override void OnExit()
		{
			if (this.resetOnExit)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				if (this.gameObject != null)
				{
					Collider2D[] components = ownerDefaultTarget.GetComponents<Collider2D>();
					for (int i = 0; i < components.Length; i++)
					{
						components[i].enabled = !this.active.Value;
					}
				}
			}
		}

		// Token: 0x0400619D RID: 24989
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400619E RID: 24990
		public FsmBool active;

		// Token: 0x0400619F RID: 24991
		public bool resetOnExit;
	}
}
