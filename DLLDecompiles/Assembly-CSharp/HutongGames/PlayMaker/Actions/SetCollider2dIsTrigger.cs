using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FD8 RID: 4056
	[ActionCategory(ActionCategory.Physics2D)]
	[Tooltip("Set the isTrigger option of a Collider2D. Optionally set all collider2D found on the gameobject Target.")]
	public class SetCollider2dIsTrigger : FsmStateAction
	{
		// Token: 0x06006FC0 RID: 28608 RVA: 0x00228E7C File Offset: 0x0022707C
		public override void Reset()
		{
			this.gameObject = null;
			this.isTrigger = false;
			this.setAllColliders = false;
		}

		// Token: 0x06006FC1 RID: 28609 RVA: 0x00228E98 File Offset: 0x00227098
		public override void OnEnter()
		{
			this.DoSetIsTrigger();
			base.Finish();
		}

		// Token: 0x06006FC2 RID: 28610 RVA: 0x00228EA8 File Offset: 0x002270A8
		private void DoSetIsTrigger()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.setAllColliders)
			{
				Collider2D[] components = ownerDefaultTarget.GetComponents<Collider2D>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].isTrigger = this.isTrigger.Value;
				}
				return;
			}
			if (ownerDefaultTarget.GetComponent<Collider2D>() != null)
			{
				ownerDefaultTarget.GetComponent<Collider2D>().isTrigger = this.isTrigger.Value;
			}
		}

		// Token: 0x04006FBB RID: 28603
		[RequiredField]
		[CheckForComponent(typeof(Collider2D))]
		[Tooltip("The GameObject with the Collider2D attached")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006FBC RID: 28604
		[RequiredField]
		[Tooltip("The flag value")]
		public FsmBool isTrigger;

		// Token: 0x04006FBD RID: 28605
		[Tooltip("Set all Colliders on the GameObject target")]
		public bool setAllColliders;
	}
}
