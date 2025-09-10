using System;
using HutongGames.PlayMaker;

// Token: 0x020003E0 RID: 992
[ActionCategory("Hollow Knight")]
public class FireGrimmBall : FsmStateAction
{
	// Token: 0x060021F7 RID: 8695 RVA: 0x0009C7A1 File Offset: 0x0009A9A1
	public override void Reset()
	{
		this.storedObject = null;
		this.tweenY = null;
		this.force = null;
	}

	// Token: 0x060021F8 RID: 8696 RVA: 0x0009C7B8 File Offset: 0x0009A9B8
	public override void OnEnter()
	{
		if (this.storedObject.Value)
		{
			GrimmballControl component = this.storedObject.Value.GetComponent<GrimmballControl>();
			if (component)
			{
				component.TweenY = this.tweenY.Value;
				component.Force = this.force.Value;
				component.Fire();
			}
		}
		base.Finish();
	}

	// Token: 0x040020B4 RID: 8372
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmGameObject storedObject;

	// Token: 0x040020B5 RID: 8373
	public FsmFloat tweenY;

	// Token: 0x040020B6 RID: 8374
	public FsmFloat force;
}
