using System;
using InControl;

// Token: 0x02000189 RID: 393
public class HeroActions : PlayerActionSet
{
	// Token: 0x06000CCA RID: 3274 RVA: 0x00038F50 File Offset: 0x00037150
	public HeroActions()
	{
		this.MenuSubmit = base.CreatePlayerAction("Submit");
		this.MenuCancel = base.CreatePlayerAction("Cancel");
		this.MenuExtra = base.CreatePlayerAction("Menu Extra");
		this.MenuSuper = base.CreatePlayerAction("Menu Super");
		this.Left = base.CreatePlayerAction("Left");
		this.Left.StateThreshold = 0.3f;
		this.Right = base.CreatePlayerAction("Right");
		this.Right.StateThreshold = 0.3f;
		this.Up = base.CreatePlayerAction("Up");
		this.Up.StateThreshold = 0.5f;
		this.Down = base.CreatePlayerAction("Down");
		this.Down.StateThreshold = 0.5f;
		this.MoveVector = base.CreateTwoAxisPlayerAction(this.Left, this.Right, this.Down, this.Up);
		this.MoveVector.LowerDeadZone = 0.15f;
		this.MoveVector.UpperDeadZone = 0.95f;
		this.RsUp = base.CreatePlayerAction("RS_Up");
		this.RsUp.StateThreshold = 0.5f;
		this.RsDown = base.CreatePlayerAction("RS_Down");
		this.RsDown.StateThreshold = 0.5f;
		this.RsLeft = base.CreatePlayerAction("RS_Left");
		this.RsLeft.StateThreshold = 0.3f;
		this.RsRight = base.CreatePlayerAction("RS_Right");
		this.RsRight.StateThreshold = 0.3f;
		this.RightStick = base.CreateTwoAxisPlayerAction(this.RsLeft, this.RsRight, this.RsDown, this.RsUp);
		this.RightStick.LowerDeadZone = 0.15f;
		this.RightStick.UpperDeadZone = 0.95f;
		this.Jump = base.CreatePlayerAction("Jump");
		this.Attack = base.CreatePlayerAction("Attack");
		this.Evade = base.CreatePlayerAction("Evade");
		this.Dash = base.CreatePlayerAction("Dash");
		this.SuperDash = base.CreatePlayerAction("Super Dash");
		this.DreamNail = base.CreatePlayerAction("Dream Nail");
		this.Cast = base.CreatePlayerAction("Cast");
		this.QuickMap = base.CreatePlayerAction("Quick Map");
		this.QuickCast = base.CreatePlayerAction("Quick Cast");
		this.Taunt = base.CreatePlayerAction("Taunt");
		this.PaneRight = base.CreatePlayerAction("Pane Right");
		this.PaneLeft = base.CreatePlayerAction("Pane Left");
		this.OpenInventory = base.CreatePlayerAction("openInventory");
		this.OpenInventoryMap = base.CreatePlayerAction("openInventoryMap");
		this.OpenInventoryJournal = base.CreatePlayerAction("openInventoryJournal");
		this.OpenInventoryTools = base.CreatePlayerAction("openInventoryTools");
		this.OpenInventoryQuests = base.CreatePlayerAction("openInventoryQuests");
		this.SwipeInventoryMap = base.CreatePlayerAction("swipeInventoryMap");
		this.SwipeInventoryJournal = base.CreatePlayerAction("swipeInventoryJournal");
		this.SwipeInventoryTools = base.CreatePlayerAction("swipeInventoryTools");
		this.SwipeInventoryQuests = base.CreatePlayerAction("swipeInventoryQuests");
		this.Pause = base.CreatePlayerAction("Pause");
	}

	// Token: 0x04000C3B RID: 3131
	public readonly PlayerAction Left;

	// Token: 0x04000C3C RID: 3132
	public readonly PlayerAction Right;

	// Token: 0x04000C3D RID: 3133
	public readonly PlayerAction Up;

	// Token: 0x04000C3E RID: 3134
	public readonly PlayerAction Down;

	// Token: 0x04000C3F RID: 3135
	public readonly PlayerAction MenuSubmit;

	// Token: 0x04000C40 RID: 3136
	public readonly PlayerAction MenuCancel;

	// Token: 0x04000C41 RID: 3137
	public readonly PlayerAction MenuExtra;

	// Token: 0x04000C42 RID: 3138
	public readonly PlayerAction MenuSuper;

	// Token: 0x04000C43 RID: 3139
	public readonly PlayerTwoAxisAction MoveVector;

	// Token: 0x04000C44 RID: 3140
	public readonly PlayerAction RsUp;

	// Token: 0x04000C45 RID: 3141
	public readonly PlayerAction RsDown;

	// Token: 0x04000C46 RID: 3142
	public readonly PlayerAction RsLeft;

	// Token: 0x04000C47 RID: 3143
	public readonly PlayerAction RsRight;

	// Token: 0x04000C48 RID: 3144
	public readonly PlayerTwoAxisAction RightStick;

	// Token: 0x04000C49 RID: 3145
	public readonly PlayerAction Jump;

	// Token: 0x04000C4A RID: 3146
	public readonly PlayerAction Evade;

	// Token: 0x04000C4B RID: 3147
	public readonly PlayerAction Dash;

	// Token: 0x04000C4C RID: 3148
	public readonly PlayerAction SuperDash;

	// Token: 0x04000C4D RID: 3149
	public readonly PlayerAction DreamNail;

	// Token: 0x04000C4E RID: 3150
	public readonly PlayerAction Attack;

	// Token: 0x04000C4F RID: 3151
	public readonly PlayerAction Cast;

	// Token: 0x04000C50 RID: 3152
	public readonly PlayerAction QuickMap;

	// Token: 0x04000C51 RID: 3153
	public readonly PlayerAction QuickCast;

	// Token: 0x04000C52 RID: 3154
	public readonly PlayerAction Taunt;

	// Token: 0x04000C53 RID: 3155
	public readonly PlayerAction PaneRight;

	// Token: 0x04000C54 RID: 3156
	public readonly PlayerAction PaneLeft;

	// Token: 0x04000C55 RID: 3157
	public readonly PlayerAction OpenInventory;

	// Token: 0x04000C56 RID: 3158
	public readonly PlayerAction OpenInventoryMap;

	// Token: 0x04000C57 RID: 3159
	public readonly PlayerAction OpenInventoryJournal;

	// Token: 0x04000C58 RID: 3160
	public readonly PlayerAction OpenInventoryTools;

	// Token: 0x04000C59 RID: 3161
	public readonly PlayerAction OpenInventoryQuests;

	// Token: 0x04000C5A RID: 3162
	public readonly PlayerAction SwipeInventoryMap;

	// Token: 0x04000C5B RID: 3163
	public readonly PlayerAction SwipeInventoryJournal;

	// Token: 0x04000C5C RID: 3164
	public readonly PlayerAction SwipeInventoryTools;

	// Token: 0x04000C5D RID: 3165
	public readonly PlayerAction SwipeInventoryQuests;

	// Token: 0x04000C5E RID: 3166
	public readonly PlayerAction Pause;
}
