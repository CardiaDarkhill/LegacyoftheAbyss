using System;
using UnityEngine.EventSystems;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x0200112B RID: 4395
	[ActionCategory(ActionCategory.UI)]
	[Tooltip("Sets the EventSystem's currently select GameObject.")]
	public class UiSetSelectedGameObject : FsmStateAction
	{
		// Token: 0x06007680 RID: 30336 RVA: 0x0024209E File Offset: 0x0024029E
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06007681 RID: 30337 RVA: 0x002420A7 File Offset: 0x002402A7
		public override void OnEnter()
		{
			this.DoSetSelectedGameObject();
			base.Finish();
		}

		// Token: 0x06007682 RID: 30338 RVA: 0x002420B5 File Offset: 0x002402B5
		private void DoSetSelectedGameObject()
		{
			EventSystem.current.SetSelectedGameObject(this.gameObject.Value);
		}

		// Token: 0x040076CB RID: 30411
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[Tooltip("The GameObject to select.")]
		public FsmGameObject gameObject;
	}
}
