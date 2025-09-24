using System;
using InControl;
using UnityEngine;

// Token: 0x020003C8 RID: 968
[RequireComponent(typeof(HollowKnightInputModule))]
public class InputModuleActionAdaptor : MonoBehaviour
{
	// Token: 0x06002100 RID: 8448 RVA: 0x00098EDC File Offset: 0x000970DC
	private void Start()
	{
		this.inputHandler = GameManager.instance.inputHandler;
		this.inputModule = base.GetComponent<HollowKnightInputModule>();
		if (this.inputHandler != null && this.inputModule != null)
		{
			this.inputModule.MoveAction = this.inputHandler.inputActions.MoveVector;
			this.inputModule.SubmitAction = this.inputHandler.inputActions.MenuSubmit;
			this.inputModule.CancelAction = this.inputHandler.inputActions.MenuCancel;
			this.inputModule.JumpAction = this.inputHandler.inputActions.Jump;
			this.inputModule.AttackAction = this.inputHandler.inputActions.Attack;
			this.inputModule.CastAction = this.inputHandler.inputActions.Cast;
			this.inputModule.MoveAction = this.inputHandler.inputActions.MoveVector;
		}
		else
		{
			Debug.LogError("Unable to bind player action set to Input Module.");
		}
		InputHandler.OnUpdateHeroActions += this.OnInputHandlerOnOnUpdateHeroActions;
	}

	// Token: 0x06002101 RID: 8449 RVA: 0x00099001 File Offset: 0x00097201
	private void OnDestroy()
	{
		InputHandler.OnUpdateHeroActions -= this.OnInputHandlerOnOnUpdateHeroActions;
	}

	// Token: 0x06002102 RID: 8450 RVA: 0x00099014 File Offset: 0x00097214
	private void OnInputHandlerOnOnUpdateHeroActions(HeroActions actions)
	{
		if (this.inputHandler != null && this.inputModule != null)
		{
			this.inputModule.MoveAction = this.inputHandler.inputActions.MoveVector;
			this.inputModule.SubmitAction = this.inputHandler.inputActions.MenuSubmit;
			this.inputModule.CancelAction = this.inputHandler.inputActions.MenuCancel;
			this.inputModule.JumpAction = this.inputHandler.inputActions.Jump;
			this.inputModule.AttackAction = this.inputHandler.inputActions.Attack;
			this.inputModule.CastAction = this.inputHandler.inputActions.Cast;
			this.inputModule.MoveAction = this.inputHandler.inputActions.MoveVector;
			return;
		}
		Debug.LogError("Unable to bind player action set to Input Module.");
	}

	// Token: 0x04002013 RID: 8211
	private InputHandler inputHandler;

	// Token: 0x04002014 RID: 8212
	private HollowKnightInputModule inputModule;
}
