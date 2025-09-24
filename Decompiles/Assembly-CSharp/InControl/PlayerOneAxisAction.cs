using System;

namespace InControl
{
	// Token: 0x020008E9 RID: 2281
	public class PlayerOneAxisAction : OneAxisInputControl
	{
		// Token: 0x140000F7 RID: 247
		// (add) Token: 0x06004FE2 RID: 20450 RVA: 0x00171BA8 File Offset: 0x0016FDA8
		// (remove) Token: 0x06004FE3 RID: 20451 RVA: 0x00171BE0 File Offset: 0x0016FDE0
		public event Action<BindingSourceType> OnLastInputTypeChanged;

		// Token: 0x17000A68 RID: 2664
		// (get) Token: 0x06004FE4 RID: 20452 RVA: 0x00171C15 File Offset: 0x0016FE15
		// (set) Token: 0x06004FE5 RID: 20453 RVA: 0x00171C1D File Offset: 0x0016FE1D
		public object UserData { get; set; }

		// Token: 0x06004FE6 RID: 20454 RVA: 0x00171C26 File Offset: 0x0016FE26
		internal PlayerOneAxisAction(PlayerAction negativeAction, PlayerAction positiveAction)
		{
			this.negativeAction = negativeAction;
			this.positiveAction = positiveAction;
			this.Raw = true;
		}

		// Token: 0x06004FE7 RID: 20455 RVA: 0x00171C44 File Offset: 0x0016FE44
		internal void Update(ulong updateTick, float deltaTime)
		{
			this.ProcessActionUpdate(this.negativeAction);
			this.ProcessActionUpdate(this.positiveAction);
			float value = Utility.ValueFromSides(this.negativeAction, this.positiveAction);
			base.CommitWithValue(value, updateTick, deltaTime);
		}

		// Token: 0x06004FE8 RID: 20456 RVA: 0x00171C90 File Offset: 0x0016FE90
		private void ProcessActionUpdate(PlayerAction action)
		{
			BindingSourceType lastInputType = this.LastInputType;
			if (action.UpdateTick > base.UpdateTick)
			{
				base.UpdateTick = action.UpdateTick;
				lastInputType = action.LastInputType;
			}
			if (this.LastInputType != lastInputType)
			{
				this.LastInputType = lastInputType;
				if (this.OnLastInputTypeChanged != null)
				{
					this.OnLastInputTypeChanged(lastInputType);
				}
			}
		}

		// Token: 0x17000A69 RID: 2665
		// (get) Token: 0x06004FE9 RID: 20457 RVA: 0x00171CE9 File Offset: 0x0016FEE9
		// (set) Token: 0x06004FEA RID: 20458 RVA: 0x00171CF0 File Offset: 0x0016FEF0
		[Obsolete("Please set this property on device controls directly. It does nothing here.")]
		public new float LowerDeadZone
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		// Token: 0x17000A6A RID: 2666
		// (get) Token: 0x06004FEB RID: 20459 RVA: 0x00171CF2 File Offset: 0x0016FEF2
		// (set) Token: 0x06004FEC RID: 20460 RVA: 0x00171CF9 File Offset: 0x0016FEF9
		[Obsolete("Please set this property on device controls directly. It does nothing here.")]
		public new float UpperDeadZone
		{
			get
			{
				return 0f;
			}
			set
			{
			}
		}

		// Token: 0x0400508B RID: 20619
		private PlayerAction negativeAction;

		// Token: 0x0400508C RID: 20620
		private PlayerAction positiveAction;

		// Token: 0x0400508D RID: 20621
		public BindingSourceType LastInputType;
	}
}
