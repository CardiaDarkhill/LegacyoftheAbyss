using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FA8 RID: 4008
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Tests if a rigid body is controlled by physics. See unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody-isKinematic.html\">IsKinematic</a>.")]
	public class IsKinematic : ComponentAction<Rigidbody>
	{
		// Token: 0x06006EB7 RID: 28343 RVA: 0x0022415E File Offset: 0x0022235E
		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.store = null;
			this.everyFrame = false;
		}

		// Token: 0x06006EB8 RID: 28344 RVA: 0x00224183 File Offset: 0x00222383
		public override void OnEnter()
		{
			this.DoIsKinematic();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006EB9 RID: 28345 RVA: 0x00224199 File Offset: 0x00222399
		public override void OnUpdate()
		{
			this.DoIsKinematic();
		}

		// Token: 0x06006EBA RID: 28346 RVA: 0x002241A4 File Offset: 0x002223A4
		private void DoIsKinematic()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				bool isKinematic = base.rigidbody.isKinematic;
				this.store.Value = isKinematic;
				base.Fsm.Event(isKinematic ? this.trueEvent : this.falseEvent);
			}
		}

		// Token: 0x04006E52 RID: 28242
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The game object to test.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006E53 RID: 28243
		[Tooltip("Event sent if it is kinematic (not controlled by physics).")]
		public FsmEvent trueEvent;

		// Token: 0x04006E54 RID: 28244
		[Tooltip("Event sent if it is not kinematic (controlled by physics).")]
		public FsmEvent falseEvent;

		// Token: 0x04006E55 RID: 28245
		[UIHint(UIHint.Variable)]
		[Tooltip("Store the result in a Bool Variable")]
		public FsmBool store;

		// Token: 0x04006E56 RID: 28246
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
