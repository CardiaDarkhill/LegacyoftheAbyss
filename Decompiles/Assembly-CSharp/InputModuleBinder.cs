using System;
using InControl;
using UnityEngine;

// Token: 0x020003C9 RID: 969
[RequireComponent(typeof(InControlInputModule))]
public class InputModuleBinder : MonoBehaviour
{
	// Token: 0x06002104 RID: 8452 RVA: 0x00099114 File Offset: 0x00097314
	private void OnEnable()
	{
		this.actions = new InputModuleBinder.MyActionSet();
		this.BindAndApplyActions();
		InControlInputModule component = base.GetComponent<InControlInputModule>();
		component.SubmitAction = this.actions.Submit;
		component.CancelAction = this.actions.Cancel;
		component.MoveAction = this.actions.Move;
		Platform.AcceptRejectInputStyleChanged += this.OnAcceptRejectInputStyleChanged;
	}

	// Token: 0x06002105 RID: 8453 RVA: 0x0009917B File Offset: 0x0009737B
	private void OnDisable()
	{
		Platform.AcceptRejectInputStyleChanged -= this.OnAcceptRejectInputStyleChanged;
		this.actions.Destroy();
	}

	// Token: 0x06002106 RID: 8454 RVA: 0x00099199 File Offset: 0x00097399
	private void OnAcceptRejectInputStyleChanged()
	{
		this.BindAndApplyActions();
	}

	// Token: 0x06002107 RID: 8455 RVA: 0x000991A4 File Offset: 0x000973A4
	private void BindAndApplyActions()
	{
		this.actions.Submit.ClearBindings();
		this.actions.Cancel.ClearBindings();
		Platform.AcceptRejectInputStyles acceptRejectInputStyle = Platform.Current.AcceptRejectInputStyle;
		if (acceptRejectInputStyle == Platform.AcceptRejectInputStyles.NonJapaneseStyle || acceptRejectInputStyle != Platform.AcceptRejectInputStyles.JapaneseStyle)
		{
			this.actions.Submit.AddDefaultBinding(InputControlType.Action1);
			this.actions.Submit.AddDefaultBinding(new Key[]
			{
				Key.Space
			});
			this.actions.Submit.AddDefaultBinding(new Key[]
			{
				Key.Return
			});
			this.actions.Cancel.AddDefaultBinding(InputControlType.Action2);
			this.actions.Cancel.AddDefaultBinding(new Key[]
			{
				Key.Escape
			});
		}
		else
		{
			this.actions.Cancel.AddDefaultBinding(InputControlType.Action1);
			this.actions.Cancel.AddDefaultBinding(new Key[]
			{
				Key.Escape
			});
			this.actions.Submit.AddDefaultBinding(InputControlType.Action2);
			this.actions.Submit.AddDefaultBinding(new Key[]
			{
				Key.Space
			});
			this.actions.Submit.AddDefaultBinding(new Key[]
			{
				Key.Return
			});
		}
		this.actions.Up.ClearBindings();
		this.actions.Up.AddDefaultBinding(new Key[]
		{
			Key.UpArrow
		});
		this.actions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
		this.actions.Up.AddDefaultBinding(InputControlType.DPadUp);
		this.actions.Down.ClearBindings();
		this.actions.Down.AddDefaultBinding(new Key[]
		{
			Key.DownArrow
		});
		this.actions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
		this.actions.Down.AddDefaultBinding(InputControlType.DPadDown);
		this.actions.Left.ClearBindings();
		this.actions.Left.AddDefaultBinding(new Key[]
		{
			Key.LeftArrow
		});
		this.actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
		this.actions.Left.AddDefaultBinding(InputControlType.DPadLeft);
		this.actions.Right.ClearBindings();
		this.actions.Right.AddDefaultBinding(new Key[]
		{
			Key.RightArrow
		});
		this.actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
		this.actions.Right.AddDefaultBinding(InputControlType.DPadRight);
	}

	// Token: 0x04002015 RID: 8213
	private InputModuleBinder.MyActionSet actions;

	// Token: 0x02001683 RID: 5763
	public class MyActionSet : PlayerActionSet
	{
		// Token: 0x06008A4B RID: 35403 RVA: 0x0027F6EC File Offset: 0x0027D8EC
		public MyActionSet()
		{
			this.Submit = base.CreatePlayerAction("Submit");
			this.Cancel = base.CreatePlayerAction("Cancel");
			this.Left = base.CreatePlayerAction("Left");
			this.Right = base.CreatePlayerAction("Right");
			this.Up = base.CreatePlayerAction("Up");
			this.Down = base.CreatePlayerAction("Down");
			this.Move = base.CreateTwoAxisPlayerAction(this.Left, this.Right, this.Down, this.Up);
		}

		// Token: 0x04008B0C RID: 35596
		public PlayerAction Submit;

		// Token: 0x04008B0D RID: 35597
		public PlayerAction Cancel;

		// Token: 0x04008B0E RID: 35598
		public PlayerAction Left;

		// Token: 0x04008B0F RID: 35599
		public PlayerAction Right;

		// Token: 0x04008B10 RID: 35600
		public PlayerAction Up;

		// Token: 0x04008B11 RID: 35601
		public PlayerAction Down;

		// Token: 0x04008B12 RID: 35602
		public PlayerTwoAxisAction Move;
	}
}
