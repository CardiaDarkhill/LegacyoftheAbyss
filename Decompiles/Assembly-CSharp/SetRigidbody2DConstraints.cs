using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x0200002B RID: 43
public class SetRigidbody2DConstraints : FsmStateAction
{
	// Token: 0x06000161 RID: 353 RVA: 0x0000818C File Offset: 0x0000638C
	public override void Reset()
	{
		this.freezePositionX = new FsmBool
		{
			UseVariable = true
		};
		this.freezePositionY = new FsmBool
		{
			UseVariable = true
		};
		this.freezeRotation = new FsmBool
		{
			UseVariable = true
		};
	}

	// Token: 0x06000162 RID: 354 RVA: 0x000081C4 File Offset: 0x000063C4
	public override void OnEnter()
	{
		this.DoSetConstraints();
		base.Finish();
	}

	// Token: 0x06000163 RID: 355 RVA: 0x000081D4 File Offset: 0x000063D4
	private void DoSetConstraints()
	{
		GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
		if (ownerDefaultTarget)
		{
			Rigidbody2D component = ownerDefaultTarget.GetComponent<Rigidbody2D>();
			if (component)
			{
				bool flag = false;
				bool flag2 = false;
				bool flag3 = false;
				if (!this.freezePositionX.IsNone)
				{
					flag = this.freezePositionX.Value;
				}
				if (!this.freezePositionY.IsNone)
				{
					flag2 = this.freezePositionY.Value;
				}
				if (!this.freezeRotation.IsNone)
				{
					flag3 = this.freezeRotation.Value;
				}
				if (flag && !flag2 && !flag3)
				{
					component.constraints = RigidbodyConstraints2D.FreezePositionX;
					return;
				}
				if (!flag && flag2 && !flag3)
				{
					component.constraints = RigidbodyConstraints2D.FreezePositionY;
					return;
				}
				if (flag && flag2 && !flag3)
				{
					component.constraints = RigidbodyConstraints2D.FreezePosition;
					return;
				}
				if (!flag && !flag2 && flag3)
				{
					component.constraints = RigidbodyConstraints2D.FreezeRotation;
					return;
				}
				if (flag && flag2 && flag3)
				{
					component.constraints = RigidbodyConstraints2D.FreezeAll;
					return;
				}
				component.constraints = RigidbodyConstraints2D.None;
			}
		}
	}

	// Token: 0x04000102 RID: 258
	[RequiredField]
	[CheckForComponent(typeof(Rigidbody2D))]
	public FsmOwnerDefault gameObject;

	// Token: 0x04000103 RID: 259
	public FsmBool freezePositionX;

	// Token: 0x04000104 RID: 260
	public FsmBool freezePositionY;

	// Token: 0x04000105 RID: 261
	public FsmBool freezeRotation;
}
