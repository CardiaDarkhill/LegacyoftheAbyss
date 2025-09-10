using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FA9 RID: 4009
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Tests if a rigid body is sleeping. See Unity docs: <a href=\"http://unity3d.com/support/documentation/Components/RigidbodySleeping.html\">Rigidbody Sleeping</a>.")]
	public class IsSleeping : ComponentAction<Rigidbody>
	{
		// Token: 0x06006EBC RID: 28348 RVA: 0x00224208 File Offset: 0x00222408
		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.store = null;
			this.everyFrame = false;
		}

		// Token: 0x06006EBD RID: 28349 RVA: 0x0022422D File Offset: 0x0022242D
		public override void OnEnter()
		{
			this.DoIsSleeping();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006EBE RID: 28350 RVA: 0x00224243 File Offset: 0x00222443
		public override void OnUpdate()
		{
			this.DoIsSleeping();
		}

		// Token: 0x06006EBF RID: 28351 RVA: 0x0022424C File Offset: 0x0022244C
		private void DoIsSleeping()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				bool flag = base.rigidbody.IsSleeping();
				this.store.Value = flag;
				base.Fsm.Event(flag ? this.trueEvent : this.falseEvent);
			}
		}

		// Token: 0x04006E57 RID: 28247
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The game object to test.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006E58 RID: 28248
		[Tooltip("Event to send if sleeping.")]
		public FsmEvent trueEvent;

		// Token: 0x04006E59 RID: 28249
		[Tooltip("Event to send if not sleeping.")]
		public FsmEvent falseEvent;

		// Token: 0x04006E5A RID: 28250
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool Variable.")]
		public FsmBool store;

		// Token: 0x04006E5B RID: 28251
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
