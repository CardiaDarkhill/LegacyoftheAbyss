using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FB0 RID: 4016
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Controls whether physics affects the game object. See unity docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody-isKinematic.html\">IsKinematic</a>.")]
	public class SetIsKinematic : ComponentAction<Rigidbody>
	{
		// Token: 0x06006EDF RID: 28383 RVA: 0x00224D58 File Offset: 0x00222F58
		public override void Reset()
		{
			this.gameObject = null;
			this.isKinematic = false;
		}

		// Token: 0x06006EE0 RID: 28384 RVA: 0x00224D6D File Offset: 0x00222F6D
		public override void OnEnter()
		{
			this.DoSetIsKinematic();
			base.Finish();
		}

		// Token: 0x06006EE1 RID: 28385 RVA: 0x00224D7C File Offset: 0x00222F7C
		private void DoSetIsKinematic()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.rigidbody.isKinematic = this.isKinematic.Value;
			}
		}

		// Token: 0x04006E97 RID: 28311
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The Game Object to set.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04006E98 RID: 28312
		[RequiredField]
		[Tooltip("Set is kinematic true/false.")]
		public FsmBool isKinematic;
	}
}
