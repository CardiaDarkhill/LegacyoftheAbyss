using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200002C RID: 44
public class SetRigidbodySimulated2D : FsmStateAction
{
	// Token: 0x06000165 RID: 357 RVA: 0x000082D0 File Offset: 0x000064D0
	public override void Reset()
	{
		this.gameObject = null;
		this.isSimulated = true;
	}

	// Token: 0x06000166 RID: 358 RVA: 0x000082E5 File Offset: 0x000064E5
	public override void OnEnter()
	{
		this.DoSetIsKinematic();
		base.Finish();
	}

	// Token: 0x06000167 RID: 359 RVA: 0x000082F4 File Offset: 0x000064F4
	private void DoSetIsKinematic()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
		if (ownerDefaultTarget)
		{
			Rigidbody2D component = ownerDefaultTarget.GetComponent<Rigidbody2D>();
			if (component)
			{
				component.simulated = this.isSimulated.Value;
			}
		}
	}

	// Token: 0x04000106 RID: 262
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	public FsmOwnerDefault gameObject;

	// Token: 0x04000107 RID: 263
	[RequiredField]
	public FsmBool isSimulated;
}
