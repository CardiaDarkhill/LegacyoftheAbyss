using System;
using InControl;
using UnityEngine;

// Token: 0x020003D1 RID: 977
[RequireComponent(typeof(InControlInputModule))]
public class PreMenuInputModuleActionAdaptor : MonoBehaviour
{
	// Token: 0x0600216C RID: 8556 RVA: 0x0009A734 File Offset: 0x00098934
	private void OnEnable()
	{
		this.CreateActions();
		InControlInputModule component = base.GetComponent<InControlInputModule>();
		if (component != null)
		{
			component.SubmitAction = this.actions.Submit;
			component.CancelAction = this.actions.Cancel;
			component.MoveAction = this.actions.Move;
		}
	}

	// Token: 0x0600216D RID: 8557 RVA: 0x0009A78A File Offset: 0x0009898A
	private void OnDisable()
	{
		this.DestroyActions();
	}

	// Token: 0x0600216E RID: 8558 RVA: 0x0009A794 File Offset: 0x00098994
	private void CreateActions()
	{
		this.actions = new PreMenuInputModuleActionAdaptor.PreMenuInputModuleActions();
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
		this.actions.Up.AddDefaultBinding(InputControlType.LeftStickUp);
		this.actions.Up.AddDefaultBinding(InputControlType.DPadUp);
		this.actions.Up.AddDefaultBinding(new Key[]
		{
			Key.UpArrow
		});
		this.actions.Down.AddDefaultBinding(InputControlType.LeftStickDown);
		this.actions.Down.AddDefaultBinding(InputControlType.DPadDown);
		this.actions.Down.AddDefaultBinding(new Key[]
		{
			Key.DownArrow
		});
		this.actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
		this.actions.Left.AddDefaultBinding(InputControlType.DPadLeft);
		this.actions.Left.AddDefaultBinding(new Key[]
		{
			Key.LeftArrow
		});
		this.actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
		this.actions.Right.AddDefaultBinding(InputControlType.DPadRight);
		this.actions.Right.AddDefaultBinding(new Key[]
		{
			Key.RightArrow
		});
	}

	// Token: 0x0600216F RID: 8559 RVA: 0x0009A919 File Offset: 0x00098B19
	private void DestroyActions()
	{
		this.actions.Destroy();
	}

	// Token: 0x04002046 RID: 8262
	private PreMenuInputModuleActionAdaptor.PreMenuInputModuleActions actions;

	// Token: 0x02001686 RID: 5766
	public class PreMenuInputModuleActions : PlayerActionSet
	{
		// Token: 0x06008A4D RID: 35405 RVA: 0x0027F7A8 File Offset: 0x0027D9A8
		public PreMenuInputModuleActions()
		{
			this.Submit = base.CreatePlayerAction("Submit");
			this.Cancel = base.CreatePlayerAction("Cancel");
			this.Left = base.CreatePlayerAction("Move Left");
			this.Right = base.CreatePlayerAction("Move Right");
			this.Up = base.CreatePlayerAction("Move Up");
			this.Down = base.CreatePlayerAction("Move Down");
			this.Move = base.CreateTwoAxisPlayerAction(this.Left, this.Right, this.Down, this.Up);
		}

		// Token: 0x04008B33 RID: 35635
		public PlayerAction Submit;

		// Token: 0x04008B34 RID: 35636
		public PlayerAction Cancel;

		// Token: 0x04008B35 RID: 35637
		public PlayerAction Left;

		// Token: 0x04008B36 RID: 35638
		public PlayerAction Right;

		// Token: 0x04008B37 RID: 35639
		public PlayerAction Up;

		// Token: 0x04008B38 RID: 35640
		public PlayerAction Down;

		// Token: 0x04008B39 RID: 35641
		public PlayerTwoAxisAction Move;
	}
}
