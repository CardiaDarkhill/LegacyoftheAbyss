using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200117E RID: 4478
	[ActionCategory(ActionCategory.UnityObject)]
	[ActionTarget(typeof(Component), "targetProperty", false)]
	[ActionTarget(typeof(GameObject), "targetProperty", false)]
	[Tooltip("Gets the value of any public property or field on the targeted Unity Object and stores it in a variable. E.g., Drag and drop any component attached to a Game Object to access its properties.")]
	public class GetProperty : FsmStateAction
	{
		// Token: 0x0600781E RID: 30750 RVA: 0x00247014 File Offset: 0x00245214
		public override void Reset()
		{
			this.targetProperty = new FsmProperty
			{
				setProperty = false
			};
			this.everyFrame = false;
		}

		// Token: 0x0600781F RID: 30751 RVA: 0x0024702F File Offset: 0x0024522F
		public override void OnEnter()
		{
			this.targetProperty.GetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007820 RID: 30752 RVA: 0x0024704A File Offset: 0x0024524A
		public override void OnUpdate()
		{
			this.targetProperty.GetValue();
		}

		// Token: 0x0400789C RID: 30876
		[Tooltip("TargetObject:\nAny object derived from UnityEngine.Object. For example, you can drag a Component from the Unity Inspector into this field. HINT: Use\u00a0{{Lock}}\u00a0to lock the current\u00a0FSM selection if you need to drag a component from another GameObject.\nProperty:\nUse the property selection menu to select the property to get. Note: You can drill into the property, e.g., transform.localPosition.x.\nStore Result:\nStore the result in a variable.")]
		public FsmProperty targetProperty;

		// Token: 0x0400789D RID: 30877
		[Tooltip("Repeat every frame. Useful if the property is changing over time.")]
		public bool everyFrame;
	}
}
