using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000480 RID: 1152
public class GetPreInstantiatedGameObject : FsmStateAction
{
	// Token: 0x060029A6 RID: 10662 RVA: 0x000B53C9 File Offset: 0x000B35C9
	public override void Reset()
	{
		this.target = new FsmOwnerDefault();
	}

	// Token: 0x060029A7 RID: 10663 RVA: 0x000B53D8 File Offset: 0x000B35D8
	public override void OnEnter()
	{
		GameObject safe = this.target.GetSafe(this);
		if (safe)
		{
			PreInstantiateGameObject component = safe.GetComponent<PreInstantiateGameObject>();
			if (component)
			{
				GameObject instantiatedGameObject = component.InstantiatedGameObject;
				if (instantiatedGameObject)
				{
					instantiatedGameObject.SetActive(true);
					this.storeGameObject.Value = instantiatedGameObject;
				}
			}
		}
		base.Finish();
	}

	// Token: 0x04002A38 RID: 10808
	[RequiredField]
	public FsmOwnerDefault target;

	// Token: 0x04002A39 RID: 10809
	[RequiredField]
	[UIHint(UIHint.Variable)]
	public FsmGameObject storeGameObject;
}
