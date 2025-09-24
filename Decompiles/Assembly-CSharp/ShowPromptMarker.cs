using System;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x02000485 RID: 1157
public class ShowPromptMarker : FsmStateAction
{
	// Token: 0x060029D2 RID: 10706 RVA: 0x000B5C76 File Offset: 0x000B3E76
	public override void Reset()
	{
		this.prefab = new FsmGameObject();
		this.labelName = new FsmString();
		this.spawnPoint = new FsmGameObject();
		this.storeObject = new FsmGameObject();
	}

	// Token: 0x060029D3 RID: 10707 RVA: 0x000B5CA4 File Offset: 0x000B3EA4
	public override void OnEnter()
	{
		if (this.prefab.Value && this.spawnPoint.Value)
		{
			GameObject gameObject;
			if (this.storeObject.Value)
			{
				gameObject = this.storeObject.Value;
			}
			else
			{
				gameObject = this.prefab.Value.Spawn();
				this.storeObject.Value = gameObject;
			}
			gameObject.transform.SetPosition2D(this.spawnPoint.Value.transform.position);
			PromptMarker component = gameObject.GetComponent<PromptMarker>();
			if (component)
			{
				component.SetLabel(this.labelName.Value);
				component.SetOwner(base.Owner);
				component.Show();
			}
		}
		base.Finish();
	}

	// Token: 0x04002A50 RID: 10832
	public FsmGameObject prefab;

	// Token: 0x04002A51 RID: 10833
	public FsmString labelName;

	// Token: 0x04002A52 RID: 10834
	[UIHint(UIHint.Variable)]
	public FsmGameObject spawnPoint;

	// Token: 0x04002A53 RID: 10835
	[UIHint(UIHint.Variable)]
	public FsmGameObject storeObject;
}
