using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D4B RID: 3403
	[ActionCategory(ActionCategory.UnityObject)]
	[ActionTarget(typeof(Component), "targetProperty", false)]
	[ActionTarget(typeof(GameObject), "targetProperty", false)]
	[Tooltip("Sets the value of any public property or field on the targeted Unity Object. E.g., Drag and drop any component attached to a Game Object to access its properties.")]
	public class SetPropertyV2 : FsmStateAction
	{
		// Token: 0x060063C8 RID: 25544 RVA: 0x001F7578 File Offset: 0x001F5778
		public override void Reset()
		{
			this.TargetProperty = new FsmProperty
			{
				setProperty = true
			};
			this.EveryFrame = false;
			this.ResetOnExit = false;
		}

		// Token: 0x060063C9 RID: 25545 RVA: 0x001F75A4 File Offset: 0x001F57A4
		public override void OnEnter()
		{
			if (this.ResetOnExit.Value)
			{
				object rawValue = this.TargetProperty.GetVariable().RawValue;
				this.TargetProperty.GetValue();
				this.originalValue = this.TargetProperty.GetVariable().RawValue;
				this.TargetProperty.GetVariable().RawValue = rawValue;
			}
			this.TargetProperty.SetValue();
			if (!this.EveryFrame.Value && !this.ResetOnExit.Value)
			{
				base.Finish();
			}
		}

		// Token: 0x060063CA RID: 25546 RVA: 0x001F762C File Offset: 0x001F582C
		public override void OnUpdate()
		{
			if (this.EveryFrame.Value)
			{
				this.TargetProperty.SetValue();
			}
		}

		// Token: 0x060063CB RID: 25547 RVA: 0x001F7648 File Offset: 0x001F5848
		public override void OnExit()
		{
			if (!this.ResetOnExit.Value)
			{
				return;
			}
			object rawValue = this.TargetProperty.GetVariable().RawValue;
			this.TargetProperty.GetVariable().RawValue = this.originalValue;
			this.TargetProperty.SetValue();
			this.TargetProperty.GetVariable().RawValue = rawValue;
		}

		// Token: 0x0400621F RID: 25119
		[Tooltip("Target Property. See below for more details.")]
		public FsmProperty TargetProperty;

		// Token: 0x04006220 RID: 25120
		public FsmBool EveryFrame;

		// Token: 0x04006221 RID: 25121
		public FsmBool ResetOnExit;

		// Token: 0x04006222 RID: 25122
		private object originalValue;
	}
}
