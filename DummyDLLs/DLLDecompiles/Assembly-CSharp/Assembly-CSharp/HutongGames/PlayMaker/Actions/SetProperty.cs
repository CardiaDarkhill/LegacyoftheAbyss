using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001180 RID: 4480
	[ActionCategory(ActionCategory.UnityObject)]
	[ActionTarget(typeof(Component), "targetProperty", false)]
	[ActionTarget(typeof(GameObject), "targetProperty", false)]
	[Tooltip("Sets the value of any public property or field on the targeted Unity Object. E.g., Drag and drop any component attached to a Game Object to access its properties.")]
	public class SetProperty : FsmStateAction
	{
		// Token: 0x06007826 RID: 30758 RVA: 0x002470BC File Offset: 0x002452BC
		public override void Reset()
		{
			this.targetProperty = new FsmProperty
			{
				setProperty = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06007827 RID: 30759 RVA: 0x002470D7 File Offset: 0x002452D7
		public override void OnEnter()
		{
			this.targetProperty.SetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007828 RID: 30760 RVA: 0x002470F2 File Offset: 0x002452F2
		public override void OnUpdate()
		{
			this.targetProperty.SetValue();
		}

		// Token: 0x040078A1 RID: 30881
		[Tooltip("Target Property. See below for more details.")]
		public FsmProperty targetProperty;

		// Token: 0x040078A2 RID: 30882
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
