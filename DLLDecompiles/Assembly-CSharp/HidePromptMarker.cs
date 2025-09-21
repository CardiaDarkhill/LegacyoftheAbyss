using System;
using HutongGames.PlayMaker;

// Token: 0x02000486 RID: 1158
public class HidePromptMarker : FsmStateAction
{
	// Token: 0x060029D5 RID: 10709 RVA: 0x000B5D7F File Offset: 0x000B3F7F
	public override void Reset()
	{
		this.storedObject = new FsmGameObject();
	}

	// Token: 0x060029D6 RID: 10710 RVA: 0x000B5D8C File Offset: 0x000B3F8C
	public override void OnEnter()
	{
		if (this.storedObject.Value)
		{
			PromptMarker component = this.storedObject.Value.GetComponent<PromptMarker>();
			if (component)
			{
				component.Hide();
				this.storedObject.Value = null;
			}
		}
		base.Finish();
	}

	// Token: 0x04002A54 RID: 10836
	[UIHint(UIHint.Variable)]
	public FsmGameObject storedObject;
}
