using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02001114 RID: 4372
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("The eventType will be executed on all components on the GameObject that can handle it.")]
	public class UiEventSystemCurrentRayCastAll : FsmStateAction
	{
		// Token: 0x06007622 RID: 30242 RVA: 0x00241014 File Offset: 0x0023F214
		public override void Reset()
		{
			this.screenPosition = null;
			this.orScreenPosition2d = new FsmVector2
			{
				UseVariable = true
			};
			this.gameObjectList = null;
			this.hitCount = null;
			this.everyFrame = false;
		}

		// Token: 0x06007623 RID: 30243 RVA: 0x00241044 File Offset: 0x0023F244
		public override void OnEnter()
		{
			this.ExecuteRayCastAll();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06007624 RID: 30244 RVA: 0x0024105A File Offset: 0x0023F25A
		public override void OnUpdate()
		{
			this.ExecuteRayCastAll();
		}

		// Token: 0x06007625 RID: 30245 RVA: 0x00241064 File Offset: 0x0023F264
		private void ExecuteRayCastAll()
		{
			this.pointer = new PointerEventData(EventSystem.current);
			if (!this.orScreenPosition2d.IsNone)
			{
				this.pointer.position = this.orScreenPosition2d.Value;
			}
			else
			{
				this.pointer.position = new Vector2(this.screenPosition.Value.x, this.screenPosition.Value.y);
			}
			EventSystem.current.RaycastAll(this.pointer, this.raycastResults);
			if (!this.hitCount.IsNone)
			{
				this.hitCount.Value = this.raycastResults.Count;
			}
			this.gameObjectList.Resize(this.raycastResults.Count);
			int index = 0;
			foreach (RaycastResult raycastResult in this.raycastResults)
			{
				if (!this.gameObjectList.IsNone)
				{
					this.gameObjectList.Set(index, raycastResult.gameObject);
				}
			}
		}

		// Token: 0x04007689 RID: 30345
		[RequiredField]
		[Tooltip("The ScreenPosition in pixels")]
		public FsmVector3 screenPosition;

		// Token: 0x0400768A RID: 30346
		[Tooltip("The ScreenPosition in a Vector2")]
		public FsmVector2 orScreenPosition2d;

		// Token: 0x0400768B RID: 30347
		[Tooltip("GameObjects hit by the raycast")]
		[UIHint(UIHint.Variable)]
		[ArrayEditor(VariableType.GameObject, "", 0, 0, 65536)]
		public FsmArray gameObjectList;

		// Token: 0x0400768C RID: 30348
		[Tooltip("Number of hits")]
		[UIHint(UIHint.Variable)]
		public FsmInt hitCount;

		// Token: 0x0400768D RID: 30349
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x0400768E RID: 30350
		private PointerEventData pointer;

		// Token: 0x0400768F RID: 30351
		private List<RaycastResult> raycastResults = new List<RaycastResult>();
	}
}
