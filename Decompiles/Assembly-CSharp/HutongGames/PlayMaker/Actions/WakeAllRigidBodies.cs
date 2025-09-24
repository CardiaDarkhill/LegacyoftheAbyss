using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000FB8 RID: 4024
	[ActionCategory(ActionCategory.Physics)]
	[Tooltip("Rigid bodies start sleeping when they come to rest. This action wakes up all rigid bodies in the scene. E.g., if you Set Gravity and want objects at rest to respond.See Unity Docs: <a href=\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody.WakeUp.html\">Rigidbody.WakeUp</a>.")]
	[SeeAlso("<a href =\"http://unity3d.com/support/documentation/ScriptReference/Rigidbody.WakeUp.html\">Rigidbody.WakeUp</a>")]
	public class WakeAllRigidBodies : FsmStateAction
	{
		// Token: 0x06006F0F RID: 28431 RVA: 0x0022566A File Offset: 0x0022386A
		public override void Reset()
		{
			this.everyFrame = false;
		}

		// Token: 0x06006F10 RID: 28432 RVA: 0x00225673 File Offset: 0x00223873
		public override void OnEnter()
		{
			this.bodies = (Object.FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[]);
			this.DoWakeAll();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006F11 RID: 28433 RVA: 0x002256A3 File Offset: 0x002238A3
		public override void OnUpdate()
		{
			this.DoWakeAll();
		}

		// Token: 0x06006F12 RID: 28434 RVA: 0x002256AC File Offset: 0x002238AC
		private void DoWakeAll()
		{
			this.bodies = (Object.FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[]);
			if (this.bodies != null)
			{
				Rigidbody[] array = this.bodies;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].WakeUp();
				}
			}
		}

		// Token: 0x04006EB5 RID: 28341
		[Tooltip("Do it every frame - use with caution! Sleeping is an important physics optimization!")]
		public bool everyFrame;

		// Token: 0x04006EB6 RID: 28342
		private Rigidbody[] bodies;
	}
}
