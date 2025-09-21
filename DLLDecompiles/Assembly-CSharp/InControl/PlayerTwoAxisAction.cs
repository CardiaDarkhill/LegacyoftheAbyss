using System;

namespace InControl
{
	// Token: 0x020008EA RID: 2282
	public class PlayerTwoAxisAction : TwoAxisInputControl
	{
		// Token: 0x17000A6B RID: 2667
		// (get) Token: 0x06004FED RID: 20461 RVA: 0x00171CFB File Offset: 0x0016FEFB
		// (set) Token: 0x06004FEE RID: 20462 RVA: 0x00171D03 File Offset: 0x0016FF03
		public bool InvertXAxis { get; set; }

		// Token: 0x17000A6C RID: 2668
		// (get) Token: 0x06004FEF RID: 20463 RVA: 0x00171D0C File Offset: 0x0016FF0C
		// (set) Token: 0x06004FF0 RID: 20464 RVA: 0x00171D14 File Offset: 0x0016FF14
		public bool InvertYAxis { get; set; }

		// Token: 0x140000F8 RID: 248
		// (add) Token: 0x06004FF1 RID: 20465 RVA: 0x00171D20 File Offset: 0x0016FF20
		// (remove) Token: 0x06004FF2 RID: 20466 RVA: 0x00171D58 File Offset: 0x0016FF58
		public event Action<BindingSourceType> OnLastInputTypeChanged;

		// Token: 0x17000A6D RID: 2669
		// (get) Token: 0x06004FF3 RID: 20467 RVA: 0x00171D8D File Offset: 0x0016FF8D
		// (set) Token: 0x06004FF4 RID: 20468 RVA: 0x00171D95 File Offset: 0x0016FF95
		public object UserData { get; set; }

		// Token: 0x06004FF5 RID: 20469 RVA: 0x00171D9E File Offset: 0x0016FF9E
		internal PlayerTwoAxisAction(PlayerAction negativeXAction, PlayerAction positiveXAction, PlayerAction negativeYAction, PlayerAction positiveYAction)
		{
			this.negativeXAction = negativeXAction;
			this.positiveXAction = positiveXAction;
			this.negativeYAction = negativeYAction;
			this.positiveYAction = positiveYAction;
			this.InvertXAxis = false;
			this.InvertYAxis = false;
			this.Raw = true;
		}

		// Token: 0x06004FF6 RID: 20470 RVA: 0x00171DD8 File Offset: 0x0016FFD8
		internal void Update(ulong updateTick, float deltaTime)
		{
			this.ProcessActionUpdate(this.negativeXAction);
			this.ProcessActionUpdate(this.positiveXAction);
			this.ProcessActionUpdate(this.negativeYAction);
			this.ProcessActionUpdate(this.positiveYAction);
			float x = Utility.ValueFromSides(this.negativeXAction, this.positiveXAction, this.InvertXAxis);
			float y = Utility.ValueFromSides(this.negativeYAction, this.positiveYAction, InputManager.InvertYAxis || this.InvertYAxis);
			base.UpdateWithAxes(x, y, updateTick, deltaTime);
		}

		// Token: 0x06004FF7 RID: 20471 RVA: 0x00171E70 File Offset: 0x00170070
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

		// Token: 0x17000A6E RID: 2670
		// (get) Token: 0x06004FF8 RID: 20472 RVA: 0x00171EC9 File Offset: 0x001700C9
		// (set) Token: 0x06004FF9 RID: 20473 RVA: 0x00171ED0 File Offset: 0x001700D0
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

		// Token: 0x17000A6F RID: 2671
		// (get) Token: 0x06004FFA RID: 20474 RVA: 0x00171ED2 File Offset: 0x001700D2
		// (set) Token: 0x06004FFB RID: 20475 RVA: 0x00171ED9 File Offset: 0x001700D9
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

		// Token: 0x04005090 RID: 20624
		private PlayerAction negativeXAction;

		// Token: 0x04005091 RID: 20625
		private PlayerAction positiveXAction;

		// Token: 0x04005092 RID: 20626
		private PlayerAction negativeYAction;

		// Token: 0x04005093 RID: 20627
		private PlayerAction positiveYAction;

		// Token: 0x04005096 RID: 20630
		public BindingSourceType LastInputType;
	}
}
