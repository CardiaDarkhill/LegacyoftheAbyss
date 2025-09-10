using System;
using UnityEngine;
using UnityEngine.AI;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E05 RID: 3589
	[ActionCategory(ActionCategory.Animator)]
	[Tooltip("Synchronize a NavMesh Agent velocity and rotation with the animator process.")]
	public class NavMeshAgentAnimatorSynchronizer : FsmStateAction
	{
		// Token: 0x06006772 RID: 26482 RVA: 0x0020A16A File Offset: 0x0020836A
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06006773 RID: 26483 RVA: 0x0020A173 File Offset: 0x00208373
		public override void OnPreprocess()
		{
			base.Fsm.HandleAnimatorMove = true;
		}

		// Token: 0x06006774 RID: 26484 RVA: 0x0020A184 File Offset: 0x00208384
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			this._agent = ownerDefaultTarget.GetComponent<NavMeshAgent>();
			this._animator = ownerDefaultTarget.GetComponent<Animator>();
			if (this._animator == null)
			{
				base.Finish();
				return;
			}
			this._trans = ownerDefaultTarget.transform;
		}

		// Token: 0x06006775 RID: 26485 RVA: 0x0020A1EC File Offset: 0x002083EC
		public override void DoAnimatorMove()
		{
			this._agent.velocity = this._animator.deltaPosition / Time.deltaTime;
			this._trans.rotation = this._animator.rootRotation;
		}

		// Token: 0x040066B6 RID: 26294
		[RequiredField]
		[CheckForComponent(typeof(NavMeshAgent))]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The Agent target. An Animator component and a NavMeshAgent component are required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040066B7 RID: 26295
		private Animator _animator;

		// Token: 0x040066B8 RID: 26296
		private NavMeshAgent _agent;

		// Token: 0x040066B9 RID: 26297
		private Transform _trans;
	}
}
